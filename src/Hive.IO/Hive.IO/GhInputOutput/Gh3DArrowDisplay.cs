using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using Grasshopper.GUI.Gradient;
using Rhino.Geometry;

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

        private List<Line> windowLossLines = new List<Line>();
        private List<Line> windowGainLines = new List<Line>();
        private List<Line> wallLossLines = new List<Line>();

        private List<Color> windowLossColors = new List<Color>();
        private List<Color> windowGainColors = new List<Color>();
        private List<Color> wallLossColors = new List<Color>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="DA"></param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var displayModes = new List<int> { 0, 1, 2, 3};

            int displayMode = 0;
            DA.GetData(0, ref displayMode);
            if (!displayModes.Contains(displayMode))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Not a valid display mode. Display modes are 0, 1, 2. Reverting to default mode 0");
                displayMode = 0;
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

            windowLossLines.Clear();
            windowGainLines.Clear();
            wallLossLines.Clear();

            windowLossColors.Clear();
            windowGainColors.Clear();
            wallLossColors.Clear();

            if (displayMode == 0)
            {
                windowLossColors = Enumerable.Repeat(Color.Blue, windowLossVectors.Count).ToList();
                windowGainColors = Enumerable.Repeat(Color.LimeGreen, windowGainVectors.Count).ToList();
                wallLossColors = Enumerable.Repeat(Color.Red, wallLossVectors.Count).ToList(); 

                windowLossLines = MakeLines(windowLossAnchors, windowLossVectors);
                windowGainLines = MakeLines(windowGainAnchors, windowGainVectors);
                wallLossLines = MakeLines(wallLossAnchors, wallLossVectors);
            } 
            //display only losses
            else if (displayMode == 1)
            {
                var colors = CalculateGradientColors(new List<List<Vector3d>>{ windowLossVectors, wallLossVectors });
                windowLossColors = colors[0];
                wallLossColors = colors[1];

                windowLossLines = MakeLines(windowLossAnchors, windowLossVectors);
                windowGainLines = MakeLines(windowGainAnchors, windowGainVectors);
                wallLossLines = MakeLines(wallLossAnchors, wallLossVectors);
            }
            //display only gains
            else if (displayMode == 2)
            {
                var colors = CalculateGradientColors(new List<List<Vector3d>> { windowGainVectors });
                windowGainColors = colors[0];

                windowLossLines = MakeLines(windowLossAnchors, windowLossVectors);
                windowGainLines = MakeLines(windowGainAnchors, windowGainVectors);
                wallLossLines = MakeLines(wallLossAnchors, wallLossVectors);
            }
            else if (displayMode == 3)
            {
                var windowLossBrep = GetBrep(windowLossAnchors, windowLossVectors, false);
                var windowGainBrep = GetBrep(windowGainAnchors, windowGainVectors, true);
                var wallLossBrep = GetBrep(wallLossAnchors, wallLossVectors, false);

                DA.SetDataList(0, windowLossBrep);
                DA.SetDataList(1, windowGainBrep);
                DA.SetDataList(2, wallLossBrep);
            }
        }

        private List<Brep> GetBrep(List<Point3d> anchors, List<Vector3d> vectors, bool gainVector)
        {
            const double radius = 0.3;
            var arrows = new List<Brep>();

            for (int i = 0; i < anchors.Count; i++)
            {
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

        private List<Line> MakeLines(List<Point3d> anchors, List<Vector3d> vectors)
        {
            var lines = new List<Line>();

            for (int i = 0; i < anchors.Count; i++)
            {
                lines.Add(new Line(anchors[i], vectors[i]));
            }

            return lines;
        }

        private List<List<Color>> CalculateGradientColors(List<List<Vector3d>> vectors)
        {
            var vectorColors = new List<List<Color>>();

            var minLength = vectors.SelectMany(vectorList => vectorList).Select(vector => vector.Length).Min();
            var maxLength = vectors.SelectMany(vectorList => vectorList).Select(vector => vector.Length).Max();

            var gradient = new GH_Gradient();
            gradient.AddGrip(new GH_Grip(minLength, Color.Blue));
            gradient.AddGrip(new GH_Grip((maxLength + minLength)/2, Color.Red));
            gradient.AddGrip(new GH_Grip(maxLength, Color.Yellow));

            for (int i = 0; i < vectors.Count; i++)
            {
                var colors = new List<Color>();
                foreach(var vector in vectors[i])
                {
                    var color = gradient.ColourAt(vector.Length);
                    colors.Add(color);
                }
                vectorColors.Add(colors);
            }

            return vectorColors;
        }

        public override void DrawViewportWires(IGH_PreviewArgs args)
        {
            base.DrawViewportWires(args);

            if(windowLossColors.Count != 0)
            {
                for (int i = 0; i < windowLossLines.Count; i++)
                {
                    args.Display.DrawArrow(windowLossLines[i], windowLossColors[i]);
                }
            }

            if (windowGainColors.Count != 0)
            {
                for (int i = 0; i < windowGainLines.Count; i++)
                {
                    args.Display.DrawArrow(windowGainLines[i], windowGainColors[i]);
                }
            }

            if (wallLossColors.Count != 0)
            {
                for (int i = 0; i < wallLossLines.Count; i++)
                {
                    args.Display.DrawArrow(wallLossLines[i], wallLossColors[i]);
                }
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
