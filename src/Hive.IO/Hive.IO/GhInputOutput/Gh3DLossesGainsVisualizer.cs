﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using Grasshopper.Kernel;
using Grasshopper.GUI.Gradient;
using Rhino.Geometry;
using Rhino.Display;

namespace Hive.IO.GhInputOutput
{
    public class Gh3DLossesGainsVisualizer : GH_Component
    {
        public Gh3DLossesGainsVisualizer()
          : base("3D Losses Gains Visualizer", "3DLossesGainsVisualizer",
              "Display vectors to represent gains/losses in 3D",
              "[hive]", "IO")
        {
        }

        public override GH_Exposure Exposure => GH_Exposure.secondary;

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBooleanParameter("Display Toggle", "DisplayToggle", "Boolean toggle to display arrows",GH_ParamAccess.item);
            pManager.AddBooleanParameter("Display Values", "DisplayValues", "Boolean toggle to display values at tips of arrows", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Scale Midpoint", "ScaleMidpoint", "Set which value maps to 0.5 for the arrow scaling", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Scale Length", "ScaleLength", "Scale all arrow lengths", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Display Mode", "DisplayMode", "Display Modes: 0 = All gains and losses, 1 = All losses, 2 = Only wall losses, 3 = Only window losses, 4 = Only windows gains", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Color Mode", "ColorMode", "Color Mode: 0 = Color according to type (wall loss, windows loss, window gain), or 1 = according to value", GH_ParamAccess.item);
            pManager.AddGenericParameter("Walls surface collection", "Walls", "Collection of all walls", GH_ParamAccess.list);
            pManager.AddGenericParameter("Windows surface collection", "Windows", "Collection of all windows", GH_ParamAccess.list);
            pManager.AddNumberParameter("Total Losses Per Window", "TotLossPerWin", "Total transmission losses per window surface, in kWh", GH_ParamAccess.list);
            pManager.AddNumberParameter("Total Losses Per Opaque", "TotLossPerOpaq", "Total transmission losses per opaque surface, in kWh", GH_ParamAccess.list);
            pManager.AddNumberParameter("Total Gains Per Window", "TotGainsPerWin", "Total solar gains per window surface, in kWh", GH_ParamAccess.list);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
        }

        private List<Line> windowLossLines = new List<Line>();
        private List<Line> windowGainLines = new List<Line>();
        private List<Line> wallLossLines = new List<Line>();

        private List<Color> windowLossColors = new List<Color>();
        private List<Color> windowGainColors = new List<Color>();
        private List<Color> wallLossColors = new List<Color>();

        private List<int> displayModes = new List<int> { 0, 1, 2, 3, 4 };
        private List<int> colorModes = new List<int> { 0, 1 };

        private List<double> lossesPerWindow = new List<double>();
        private List<double> lossesPerOpaque = new List<double>();
        private List<double> gainsPerWindow = new List<double>();

        private bool display = false;
        private bool values = false;

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            DA.GetData(0, ref display);
            DA.GetData(1, ref values);

            int midpoint = new int();
            if (!DA.GetData(2, ref midpoint)) return;

            int scaleLength = new int();
            if (!DA.GetData(3, ref scaleLength)) return;

            int displayMode = 0;
            DA.GetData(4, ref displayMode);
            if (!displayModes.Contains(displayMode))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Not a valid display mode. Display modes are 0, 1, 2, 3, 4. Reverting to default mode 0");
                displayMode = 0;
            }

            int colorMode = 0;
            DA.GetData(5, ref colorMode);
            if (!colorModes.Contains(colorMode))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Not a valid color mode. Display modes are 0 or 1. Reverting to default mode 0");
                colorMode = 0;
            }

            var walls = new List<Brep>();
            if (!DA.GetDataList(6, walls)) return;
            var windows = new List<Brep>();
            if (!DA.GetDataList(7, windows)) return;

            lossesPerOpaque.Clear();
            lossesPerWindow.Clear();
            gainsPerWindow.Clear();

            if (!DA.GetDataList(8, lossesPerWindow)) return;
            if (!DA.GetDataList(9, lossesPerOpaque)) return;
            if (!DA.GetDataList(10, gainsPerWindow)) return;

            /////////////////////////////////////////////// CALCULATE VECTORS & ORIGIN POINTS //////////////////////////////////////////////////////////////////////////////////////

            var lossesPerWindowScaled = lossesPerWindow.Select(i => (1 - midpoint / (midpoint + i)) * scaleLength).ToList();
            var lossesPerOpaqueScaled = lossesPerOpaque.Select(i => (1 - midpoint / (midpoint + i)) * scaleLength).ToList();
            var gainsPerWindowScaled = gainsPerWindow.Select(i => (1 - midpoint / (midpoint + i)) * scaleLength).ToList();

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

                //get two points next tot the centroid as origin points for the window loss and gain vectors,
                //so they dont overlap when displayed simultaneously
                var nurbs = window.Faces[0].ToNurbsSurface();
                nurbs.IncreaseDegreeU(2);
                nurbs.IncreaseDegreeV(3);

                var pointsList = nurbs.Points.ToList();
                windowLossAnchors.Add(new Point3d(pointsList[5].X, pointsList[5].Y, pointsList[5].Z));
                windowGainAnchors.Add(new Point3d(pointsList[6].X, pointsList[6].Y, pointsList[6].Z));
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
                Enumerable.Repeat(Color.DarkGreen, windowGainVectors.Count).ToList(),
                Enumerable.Repeat(Color.Red, wallLossVectors.Count).ToList()
            };

            if (displayMode == 0)
            {
                var colors = CalculateGradientColors(new List<List<double>> { lossesPerWindow, gainsPerWindow, lossesPerOpaque }, colorMode, displayMode, typeColorArray);
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
                var colors = CalculateGradientColors(new List<List<double>> { lossesPerWindow, lossesPerOpaque }, colorMode, displayMode, typeColorArray);
                windowLossColors = colors[0];
                wallLossColors = colors[1];

                windowLossLines = MakeLines(windowCenterAnchors, windowLossVectors);
                windowGainLines = MakeLines(windowCenterAnchors, windowGainVectors);
                wallLossLines = MakeLines(wallLossAnchors, wallLossVectors);
            }
            //display only wall losses
            else if (displayMode == 2)
            {
                var colors = CalculateGradientColors(new List<List<double>> { lossesPerOpaque }, colorMode, displayMode, typeColorArray);
                wallLossColors = colors[0];

                windowLossLines = MakeLines(windowCenterAnchors, windowLossVectors);
                windowGainLines = MakeLines(windowCenterAnchors, windowGainVectors);
                wallLossLines = MakeLines(wallLossAnchors, wallLossVectors);
            }
            //display only window losses
            else if (displayMode == 3)
            {
                var colors = CalculateGradientColors(new List<List<double>> { lossesPerWindow }, colorMode, displayMode, typeColorArray);
                windowLossColors = colors[0];

                windowLossLines = MakeLines(windowCenterAnchors, windowLossVectors);
                windowGainLines = MakeLines(windowCenterAnchors, windowGainVectors);
                wallLossLines = MakeLines(wallLossAnchors, wallLossVectors);
            }
            //display only gains
            else if (displayMode == 4)
            {
                var colors = CalculateGradientColors(new List<List<double>> { gainsPerWindow }, colorMode, displayMode, typeColorArray);
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

        private List<List<Color>> CalculateGradientColors(List<List<double>> values, int colorMode, int displayMode, List<List<Color>> typeColorArray)
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

            var minLength = values.SelectMany(valueList => valueList).Select(value => value).Min();
            var maxLength = values.SelectMany(valueList => valueList).Select(value => value).Max();

            var gradient = new GH_Gradient();
            gradient.AddGrip(new GH_Grip(minLength, Color.Blue));
            gradient.AddGrip(new GH_Grip((maxLength + minLength) / 2, Color.Red));
            gradient.AddGrip(new GH_Grip(maxLength, Color.Yellow));

            for (int i = 0; i < values.Count; i++)
            {
                var colors = new List<Color>();
                foreach (var value in values[i])
                {
                    var color = gradient.ColourAt(value);
                    colors.Add(color);
                }
                vectorColors.Add(colors);
            }

            return vectorColors;
        }

        private Plane GetTextPlane(Point3d arrowTip, Vector3d arrowDirection)
        {
            var plane = new Plane();

            if (arrowDirection.IsParallelTo(new Vector3d(0, 0, 1)) == 1)
            {
                plane = new Plane(arrowTip, new Vector3d(0, 0, 1), new Vector3d(1, 0, 0));
            } else
            {
                plane = new Plane(arrowTip, arrowDirection, new Vector3d(0, 0, 1));
            }

            return plane;
        }

        /// <summary>
        /// Draws the Arrows and their value text into the Viewport
        /// </summary>
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

                        if (values)
                        {
                            var textPlane = GetTextPlane(windowLossLines[i].To, windowLossLines[i].Direction);
                            var text = new Text3d(Math.Round(lossesPerWindow[i], 1).ToString(), textPlane, 0.5);
                            args.Display.Draw3dText(text, windowLossColors[i], windowLossLines[i].To);
                        }
                    }
                }

                if (windowGainColors.Count != 0)
                {
                    for (int i = 0; i < windowGainLines.Count; i++)
                    {
                        args.Display.DrawArrow(windowGainLines[i], windowGainColors[i]);

                        if (values)
                        {
                            var textPlane = GetTextPlane(windowGainLines[i].To, Vector3d.Negate(windowGainLines[i].Direction));
                            var text = new Text3d(Math.Round(gainsPerWindow[i], 1).ToString(), textPlane, 0.5);
                            args.Display.Draw3dText(text, windowGainColors[i], windowGainLines[i].To);
                        }
                    }
                }

                if (wallLossColors.Count != 0)
                {
                    for (int i = 0; i < wallLossLines.Count; i++)
                    {
                        args.Display.DrawArrow(wallLossLines[i], wallLossColors[i]);

                        if(values)
                        {
                            var textPlane = GetTextPlane(wallLossLines[i].To, wallLossLines[i].Direction);
                            var text = new Text3d(Math.Round(lossesPerOpaque[i], 1).ToString(), textPlane, 0.5);
                            args.Display.Draw3dText(text, wallLossColors[i], wallLossLines[i].To);
                        }
                    }
                }
            }
        }

        protected override Bitmap Icon => Properties.Resources.IOCore_VisualizerLossesGains;

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("59a5de85-7066-4610-b7d1-3ae430e8829e"); }
        }
    }
}
