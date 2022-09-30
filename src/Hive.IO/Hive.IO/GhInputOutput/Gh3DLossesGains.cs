using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;

namespace Hive.IO.GhInputOutput
{
    public class Gh3DLossesGains : GH_Component
    {
        public Gh3DLossesGains()
          : base("3DLossesGains", "3DLossesGains",
              "Calculate positions and sizes of vectors to represent gains/losses in 3D",
              "[hive]", "IO-Core")
        {
        }

        public override GH_Exposure Exposure => GH_Exposure.secondary;

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBooleanParameter("Display Toggle", "DisplayToggle", "Boolean toggle to display arrows",GH_ParamAccess.item);
            pManager.AddNumberParameter("Value scale factor", "ScaleFactor", "Value to scale size of vectors", GH_ParamAccess.item);
            pManager.AddGenericParameter("Walls surface collection", "Walls", "Collection of all walls", GH_ParamAccess.list);
            pManager.AddGenericParameter("Windows surface collection", "Windows", "Collection of all windows", GH_ParamAccess.list);
            pManager.AddNumberParameter("Total Losses Per Window", "TotLossPerWin", "Total transmission losses per window surface, in kWh", GH_ParamAccess.list);
            pManager.AddNumberParameter("Total Losses Per Opaque", "TotLossPerOpaq", "Total transmission losses per opaque surface, in kWh", GH_ParamAccess.list);
            pManager.AddNumberParameter("Total Gains Per Window", "TotGainsPerWin", "Total solar gains per window surface, in kWh", GH_ParamAccess.list);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Window loss anchor points", "WindowLossAnchors", "Starting points of loss arrows for windows", GH_ParamAccess.list);
            pManager.AddGenericParameter("Window loss vectors", "WindowLossVectors", "Vectors of loss arrows for windows", GH_ParamAccess.list);
            pManager.AddGenericParameter("Window gain anchor points", "WindowGainAnchors", "Starting points of gain arrows for windows", GH_ParamAccess.list);
            pManager.AddGenericParameter("Window gain vectors", "WindowGainVectors", "Vectors of gain arrows for walls", GH_ParamAccess.list);
            pManager.AddGenericParameter("Wall loss anchor points", "WallLossAnchors", "Starting points of loss arrows for walls", GH_ParamAccess.list);
            pManager.AddGenericParameter("Wall loss vectors", "WallLossVectors", "Vectors of loss arrows for walls", GH_ParamAccess.list);
            //pManager.AddGenericParameter("Wall gain anchor points", "WallGainAnchors", "Starting points of gain arrows for walls", GH_ParamAccess.list);
            //pManager.AddGenericParameter("Wall gain vectors", "WallGainVectors", "Vectors of gain arrows for walls", GH_ParamAccess.list);

        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var display = false;
            DA.GetData(0, ref display);
            var scaleFactor = 1.0;
            DA.GetData(1, ref scaleFactor);

            var walls = new List<Brep>();
            if (!DA.GetDataList(2, walls)) return;
            var windows = new List<Brep>();
            if (!DA.GetDataList(3, windows)) return;

            var lossesPerWindow = new List<double>();
            if (!DA.GetDataList(4, lossesPerWindow)) return;
            var lossesPerOpaque = new List<double>();
            if (!DA.GetDataList(5, lossesPerOpaque)) return;
            var gainsPerWindow = new List<double>();
            if (!DA.GetDataList(6, gainsPerWindow)) return;

            var lossesPerWindowScaled = lossesPerWindow.Select(i => i * scaleFactor).ToList();
            var lossesPerOpaqueScaled = lossesPerOpaque.Select(i => i * scaleFactor).ToList();
            var gainsPerWindowScaled = gainsPerWindow.Select(i => i * scaleFactor).ToList();

            //Anchor points
            var wallLossAnchors = new List<Point3d>();
            var windowLossAnchors = new List<Point3d>();
            var windowGainAnchors = new List<Point3d>();

            foreach (Brep wall in walls)
            {
                var amp = AreaMassProperties.Compute(wall);
                wallLossAnchors.Add(amp.Centroid);
            }

            foreach (Brep window in windows)
            {
                var amp = AreaMassProperties.Compute(window);
                windowLossAnchors.Add(amp.Centroid);
            }

            //Vectors
            var wallLossVectors = new List<Vector3d>();
            var windowLossVectors = new List<Vector3d>();
            var windowGainVectors = new List<Vector3d>();

            for (int i = 0; i < walls.Count(); i++)
            {
                Vector3d vector = walls[i].Faces[0].NormalAt(wallLossAnchors[i][0], wallLossAnchors[i][1]);
                Vector3d scaledVector = Vector3d.Multiply(vector, lossesPerOpaqueScaled[i]);
                wallLossVectors.Add(scaledVector);
            }

            for (int i = 0; i < windows.Count(); i++)
            {
                Vector3d vector = windows[i].Faces[0].NormalAt(windowLossAnchors[i][0], windowLossAnchors[i][1]);
                Vector3d scaledVector = Vector3d.Multiply(vector, lossesPerWindowScaled[i]);
                windowLossVectors.Add(scaledVector);
            }

            for (int i = 0; i < windows.Count(); i++)
            {
                Vector3d vector = windows[i].Faces[0].NormalAt(windowLossAnchors[i][0], windowLossAnchors[i][1]);
                Vector3d scaledVector = Vector3d.Multiply(vector, gainsPerWindowScaled[i]);
                scaledVector.Reverse();
                windowGainVectors.Add(scaledVector);
            }

            for(int i = 0; i < windows.Count(); i++)
            {
                Point3d gainAnchor = windowLossAnchors[i] - windowGainVectors[i];
                windowGainAnchors.Add(gainAnchor);
            }

            DA.SetDataList(0, windowLossAnchors);
            DA.SetDataList(1, windowLossVectors);
            DA.SetDataList(2, windowGainAnchors);
            DA.SetDataList(3, windowGainVectors);
            DA.SetDataList(4, wallLossAnchors);
            DA.SetDataList(5, wallLossVectors);
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return null;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("59a5de85-7066-4610-b7d1-3ae430e8829e"); }
        }
    }
}
