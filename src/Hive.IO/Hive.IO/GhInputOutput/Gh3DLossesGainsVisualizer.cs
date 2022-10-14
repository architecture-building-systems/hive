using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using Grasshopper.GUI.Gradient;
using Rhino.Geometry;

namespace Hive.IO.GhInputOutput
{
    public class Gh3DLossesGainsVisualizer : GH_Component
    {
        public Gh3DLossesGainsVisualizer()
          : base("3DLossesGainsVisualizer", "3DLossesGainsVisualizer",
              "Display vectors to represent gains/losses in 3D",
              "[hive]", "IO-Core")
        {
        }

        public override GH_Exposure Exposure => GH_Exposure.secondary;

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBooleanParameter("Display Toggle", "DisplayToggle", "Boolean toggle to display arrows",GH_ParamAccess.item);
            pManager.AddNumberParameter("Value scale factor", "ScaleFactor", "Value to scale size of vectors", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Display Mode", "DisplayMode", "Display Modes: 0 = All gains and losses, 1 = All losses, 2 = Only wall losses, 3 = Only window losses, 4 = Only windows gains", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Color Mode", "ColorMode", "Color according to type (wall loss, windows loss, window gain) or according to value", GH_ParamAccess.item);
            pManager.AddGenericParameter("Walls surface collection", "Walls", "Collection of all walls", GH_ParamAccess.list);
            pManager.AddGenericParameter("Windows surface collection", "Windows", "Collection of all windows", GH_ParamAccess.list);
            pManager.AddNumberParameter("Total Losses Per Window", "TotLossPerWin", "Total transmission losses per window surface, in kWh", GH_ParamAccess.list);
            pManager.AddNumberParameter("Total Losses Per Opaque", "TotLossPerOpaq", "Total transmission losses per opaque surface, in kWh", GH_ParamAccess.list);
            pManager.AddNumberParameter("Total Gains Per Window", "TotGainsPerWin", "Total solar gains per window surface, in kWh", GH_ParamAccess.list);
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

        private List<int> displayModes = new List<int> { 0, 1, 2, 3, 4 };
        private List<int> colorModes = new List<int> { 0, 1 };

        private bool display = false;

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            DA.GetData(0, ref display);
            var scaleFactor = 1.0;
            DA.GetData(1, ref scaleFactor);

            int displayMode = 0;
            DA.GetData(2, ref displayMode);
            if (!displayModes.Contains(displayMode))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Not a valid display mode. Display modes are 0, 1, 2, 3, 4. Reverting to default mode 0");
                displayMode = 0;
            }

            int colorMode = 0;
            DA.GetData(3, ref colorMode);
            if (!colorModes.Contains(colorMode))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Not a valid color mode. Display modes are 0 or 1. Reverting to default mode 0");
                colorMode = 0;
            }

            var walls = new List<Brep>();
            if (!DA.GetDataList(4, walls)) return;
            var windows = new List<Brep>();
            if (!DA.GetDataList(5, windows)) return;

            var lossesPerWindow = new List<double>();
            if (!DA.GetDataList(6, lossesPerWindow)) return;
            var lossesPerOpaque = new List<double>();
            if (!DA.GetDataList(7, lossesPerOpaque)) return;
            var gainsPerWindow = new List<double>();
            if (!DA.GetDataList(8, gainsPerWindow)) return;

            var lossesPerWindowScaled = lossesPerWindow.Select(i => i * scaleFactor).ToList();
            var lossesPerOpaqueScaled = lossesPerOpaque.Select(i => i * scaleFactor).ToList();
            var gainsPerWindowScaled = gainsPerWindow.Select(i => i * scaleFactor).ToList();

            //Anchor points
            var wallLossAnchors = new List<Point3d>();
            var windowLossAnchors = new List<Point3d>();
            var windowGainAnchors = new List<Point3d>();
            var windowCenterAnchors = new List<Point3d>();

            foreach (Brep wall in walls)
            {
                var amp = AreaMassProperties.Compute(wall);
                wallLossAnchors.Add(amp.Centroid);
            }

            foreach (Brep window in windows)
            {
                var amp = AreaMassProperties.Compute(window);
                var centroid = amp.Centroid;
                windowCenterAnchors.Add(centroid);

                //offset the anchors for gain/loss vectors on the windows, so they dont overlap
                var curvature = window.Faces[0].CurvatureAt(centroid[0], centroid[1]);
                Vector3d curvDir = curvature.Direction(1);

                windowLossAnchors.Add(centroid + curvDir);
                windowGainAnchors.Add(centroid - curvDir);
            }

            //Vectors
            var wallLossVectors = new List<Vector3d>();
            var windowLossVectors = new List<Vector3d>();
            var windowGainVectors = new List<Vector3d>();

            for (int i = 0; i < walls.Count(); i++)
            {
                var wall = walls[i];
                Vector3d vector = wall.Faces[0].NormalAt(wallLossAnchors[i][0], wallLossAnchors[i][1]);
                Vector3d scaledVector = Vector3d.Multiply(vector, lossesPerOpaqueScaled[i]);
                wallLossVectors.Add(scaledVector);
            }

            for (int i = 0; i < windows.Count(); i++)
            {
                var window = windows[i];
                Vector3d vector = window.Faces[0].NormalAt(windowLossAnchors[i][0], windowLossAnchors[i][1]);
                Vector3d scaledVectorLoss = Vector3d.Multiply(vector, lossesPerWindowScaled[i]);
                windowLossVectors.Add(scaledVectorLoss);

                Vector3d scaledVectorGain = Vector3d.Multiply(vector, gainsPerWindowScaled[i]);
                scaledVectorGain.Reverse();
                windowGainVectors.Add(scaledVectorGain);
            }

            //Re-calculate anchor points for the scaled gain vectors that point at the windows, so the tip of the vector is directly at the window outside
            for(int i = 0; i < windows.Count(); i++)
            {
                Point3d gainAnchor = windowGainAnchors[i] - windowGainVectors[i];
                windowGainAnchors[i] = gainAnchor;
            }

            /////////////////////////////////////////////// DRAW VECTORS //////////////////////////////////////////////////////////////////////////////////////

            windowLossLines.Clear();
            windowGainLines.Clear();
            wallLossLines.Clear();

            windowLossColors.Clear();
            windowGainColors.Clear();
            wallLossColors.Clear();

            var typeColorArray = new List<List<Color>>() {
                Enumerable.Repeat(Color.Blue, windowLossVectors.Count).ToList(),
                Enumerable.Repeat(Color.LimeGreen, windowGainVectors.Count).ToList(),
                Enumerable.Repeat(Color.Red, wallLossVectors.Count).ToList()
            };

            if (displayMode == 0)
            {
                var colors = CalculateGradientColors(new List<List<Vector3d>> { windowLossVectors, windowGainVectors, wallLossVectors }, colorMode, displayMode, typeColorArray);
                windowLossColors = colors[0];
                windowGainColors = colors[1];
                wallLossColors = colors[2];

                windowLossLines = MakeLines(windowLossAnchors, windowLossVectors);
                windowGainLines = MakeLines(windowGainAnchors, windowGainVectors);
                wallLossLines = MakeLines(wallLossAnchors, wallLossVectors);
            }
            //display wall and window losses
            else if (displayMode == 1)
            {
                var colors = CalculateGradientColors(new List<List<Vector3d>> { windowLossVectors, wallLossVectors }, colorMode, displayMode, typeColorArray);
                windowLossColors = colors[0];
                wallLossColors = colors[1];

                windowLossLines = MakeLines(windowCenterAnchors, windowLossVectors);
                windowGainLines = MakeLines(windowCenterAnchors, windowGainVectors);
                wallLossLines = MakeLines(wallLossAnchors, wallLossVectors);
            }
            //display only wall losses
            else if (displayMode == 2)
            {
                var colors = CalculateGradientColors(new List<List<Vector3d>> { wallLossVectors }, colorMode, displayMode, typeColorArray);
                wallLossColors = colors[0];

                windowLossLines = MakeLines(windowCenterAnchors, windowLossVectors);
                windowGainLines = MakeLines(windowCenterAnchors, windowGainVectors);
                wallLossLines = MakeLines(wallLossAnchors, wallLossVectors);
            }
            //display only window losses
            else if (displayMode == 3)
            {
                var colors = CalculateGradientColors(new List<List<Vector3d>> { windowLossVectors }, colorMode, displayMode, typeColorArray);
                windowLossColors = colors[0];

                windowLossLines = MakeLines(windowCenterAnchors, windowLossVectors);
                windowGainLines = MakeLines(windowCenterAnchors, windowGainVectors);
                wallLossLines = MakeLines(wallLossAnchors, wallLossVectors);
            }
            //display only gains
            else if (displayMode == 4)
            {
                var colors = CalculateGradientColors(new List<List<Vector3d>> { windowGainVectors }, colorMode, displayMode, typeColorArray);
                windowGainColors = colors[0];

                //offset the starting points for the gain vectors by the vector themselves, so the arrow point is on the window
                for (int i = 0; i < windowCenterAnchors.Count; i++)
                {
                    windowCenterAnchors[i] = windowCenterAnchors[i] - windowGainVectors[i];
                }

                windowLossLines = MakeLines(windowCenterAnchors, windowLossVectors);
                windowGainLines = MakeLines(windowCenterAnchors, windowGainVectors);
                wallLossLines = MakeLines(wallLossAnchors, wallLossVectors);
            }
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

        private List<List<Color>> CalculateGradientColors(List<List<Vector3d>> vectors, int colorMode, int displayMode, List<List<Color>> typeColorArray)
        {
            var vectorColors = new List<List<Color>>();

            if (colorMode == 0)
            {
                if (displayMode == 0)
                {
                    vectorColors = typeColorArray;
                }
                else if (displayMode == 1)
                {
                    vectorColors.Add(typeColorArray[0]);
                    vectorColors.Add(typeColorArray[2]);
                }
                else if (displayMode == 2)
                {
                    vectorColors.Add(typeColorArray[2]);
                }
                else if (displayMode == 3)
                {
                    vectorColors.Add(typeColorArray[0]);
                }
                else if (displayMode == 4)
                {
                    vectorColors.Add(typeColorArray[1]);
                }

                return vectorColors;
            }

            var minLength = vectors.SelectMany(vectorList => vectorList).Select(vector => vector.Length).Min();
            var maxLength = vectors.SelectMany(vectorList => vectorList).Select(vector => vector.Length).Max();

            var gradient = new GH_Gradient();
            gradient.AddGrip(new GH_Grip(minLength, Color.Blue));
            gradient.AddGrip(new GH_Grip((maxLength + minLength) / 2, Color.Red));
            gradient.AddGrip(new GH_Grip(maxLength, Color.Yellow));

            for (int i = 0; i < vectors.Count; i++)
            {
                var colors = new List<Color>();
                foreach (var vector in vectors[i])
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

            if (display)
            {
                if (windowLossColors.Count != 0)
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
