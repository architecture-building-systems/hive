﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace Hive.IO.GhInputOutput
{
    public class Gh3DArrowDisplay : GH_Component
    {
        public Gh3DArrowDisplay()
          : base("Gh3DArrowDisplay", "Gh3DArrowDisplay",
              "Draw arrows to display gains and losses through walls/windows",
              "[hive]", "IO-Core")
        {
        }

        public override GH_Exposure Exposure => GH_Exposure.secondary;

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddIntegerParameter("Display Mode", "DisplayMode", "Display Modes: [0, 1]", GH_ParamAccess.item);
            pManager.AddGenericParameter("Window loss anchor points", "WindowLossAnchors", "Starting points of loss arrows for windows", GH_ParamAccess.list);
            pManager.AddGenericParameter("Window loss vectors", "WindowLossVectors", "Vectors of loss arrows for windows", GH_ParamAccess.list);
            pManager.AddGenericParameter("Window gain anchor points", "WindowGainAnchors", "Starting points of gain arrows for windows", GH_ParamAccess.list);
            pManager.AddGenericParameter("Window gain vectors", "WindowGainVectors", "Vectors of gain arrows for walls", GH_ParamAccess.list);
            pManager.AddGenericParameter("Wall loss anchor points", "WallLossAnchors", "Starting points of loss arrows for walls", GH_ParamAccess.list);
            pManager.AddGenericParameter("Wall loss vectors", "WallLossVectors", "Vectors of loss arrows for walls", GH_ParamAccess.list);
            //pManager.AddGenericParameter("Wall gain anchor points", "WallGainAnchors", "Starting points of gain arrows for walls", GH_ParamAccess.list);
            //pManager.AddGenericParameter("Wall gain vectors", "WallGainVectors", "Vectors of gain arrows for walls", GH_ParamAccess.list);
            //pManager.AddGenericParameter("Window loss color", "WindowLossColor", "Color of loss arrows for windows (optional)", GH_ParamAccess.item);
            //pManager.AddGenericParameter("Window gain color", "WindowGainColor", "Color of gain arrows for windows (optional)", GH_ParamAccess.item);
            //pManager.AddGenericParameter("Wall loss color", "WallLossColor", "Color of loss arrows for walls (optional)", GH_ParamAccess.item);

            pManager[0].Optional = true;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Window loss arrows", "WindowLossArrows", "", GH_ParamAccess.list);
            pManager.AddGenericParameter("Window gain arrows", "WindowGainArrows", "", GH_ParamAccess.list);
            pManager.AddGenericParameter("Wall loss arrows", "WallLossArrows", "", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var displayModes = new List<int> { 0, 1 };

            int displayMode = 0;
            DA.GetData(0, ref displayMode);
            if (!displayModes.Contains(displayMode))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Not a valid display mode. Display modes are 0, 1. Reverting to default mode 0");
            }

            //Window Loss
            var windowLossAnchors = new List<Point3d>();
            if (!DA.GetDataList(1, windowLossAnchors)) return;
            var windowLossVectors = new List<Vector3d>();
            if (!DA.GetDataList(2, windowLossVectors)) return;

            //Window Gain
            var windowGainAnchors = new List<Point3d>();
            if (!DA.GetDataList(3, windowGainAnchors)) return;
            var windowGainVectors = new List<Vector3d>();
            if (!DA.GetDataList(4, windowGainVectors)) return;

            //Wall Loss
            var wallLossAnchors = new List<Point3d>();
            if (!DA.GetDataList(5, wallLossAnchors)) return;
            var wallLossVectors = new List<Vector3d>();
            if (!DA.GetDataList(6, wallLossVectors)) return;

            var preview1 = new GH_PreviewUtil();
            var preview2 = new GH_PreviewUtil();
            var preview3 = new GH_PreviewUtil();

            if (displayMode == 0)
            {
                preview1.Enabled = false;
                preview2.Enabled = false;
                preview3.Enabled = false;

                var windowLossBrep = GetBrep(windowLossAnchors, windowLossVectors, false);
                var windowGainBrep = GetBrep(windowGainAnchors, windowGainVectors, true);
                var wallLossBrep = GetBrep(wallLossAnchors, wallLossVectors, false);

                DA.SetDataList(0, windowLossBrep);
                DA.SetDataList(1, windowGainBrep);
                DA.SetDataList(2, wallLossBrep);
            } 
            else if (displayMode == 1)
            {
                preview1.Enabled = true;
                preview2.Enabled = true;
                preview3.Enabled = true;

                SetPreviewVectors(windowLossAnchors, windowLossVectors, preview1, System.Drawing.Color.Blue);
                SetPreviewVectors(windowGainAnchors, windowGainVectors, preview2, System.Drawing.Color.LimeGreen);
                SetPreviewVectors(wallLossAnchors, wallLossVectors, preview3, System.Drawing.Color.Red);
            }
        }

        private List<Brep> GetBrep(List<Point3d> anchors, List<Vector3d> vectors, bool gainVector)
        {
            const double radius = 0.3;
            var arrows = new List<Brep>();

            for (int i = 0; i < anchors.Count; i++)
            {
                var brepsToJoin = new List<Brep>();
                var orig = anchors[i];
                var vec = vectors[i];
                var unitVec = vec;
                unitVec.Unitize();

                //calculate origin plane for cylinders
                Plane pln = gainVector ? new Plane(orig - unitVec, vec) : new Plane(orig, vec);

                Circle circle = new Circle(pln, radius);
                Cylinder cylinder = new Cylinder(circle, vec.Length);
                Brep brepCyl = cylinder.ToBrep(true, true);
                arrows.Add(brepCyl);

                //calculate origin plane for cones
                var lossOrigin = orig + vec + unitVec;
                var gainOrigin = orig + vec;

                Plane conepln = gainVector ? new Plane(gainOrigin, Vector3d.Negate(vec)) : new Plane(lossOrigin, Vector3d.Negate(vec));
                Cone cone = new Cone(conepln, 1, 0.5);
                Brep brepCone = cone.ToBrep(true);
                arrows.Add(brepCone);
            }

            return arrows;
        }

        private void SetPreviewVectors(List<Point3d> anchors, List<Vector3d> vectors, GH_PreviewUtil preview, System.Drawing.Color color)
        {
            preview.WireColour = color;
            for (int i = 0; i < anchors.Count; i++)
            {
                var line = new Line(anchors[i], vectors[i]);
                preview.AddVector(line);
            }
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
            get { return new Guid("10dae766-9880-440d-a9bf-07eadcca0402"); }
        }
    }
}
