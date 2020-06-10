﻿using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Attributes;
using Grasshopper.Kernel.Types;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot.WindowsForms;
using Rhino;

namespace Hive.IO.GHComponents
{
    public enum VisualizerPlot
    {
        DemandMonthly,
        DemandMonthlyNormalized,
    }

    public class GHVisualizer : GH_Param<GH_ObjectWrapper>
    {

        public GHVisualizer() : base("Hive.IO.Visualizer", "Hive.IO.Visualizer",
              "Hive Visualizer for simulation results",
              "[hive]", "IO", GH_ParamAccess.item)
        {
        }

        public Results Results { get; private set; }


        public override GH_ParamKind Kind => GH_ParamKind.floating;

        public override string TypeName => "HiveResults";

        public override GH_Exposure Exposure => GH_Exposure.secondary;


        /// <summary>
        /// FIXME: print out some of the data to see what it looks like
        /// </summary>
        protected override void OnVolatileDataCollected()
        {
            if (m_data.IsEmpty)
            {
                Results = new Results();
                return;
            }

            Results = m_data.First().Value as Results;
            ((GHVisualizerAttributes) m_attributes).ClearBitmapCache();
        }


        public override void CreateAttributes()
        {
            m_attributes = new GHVisualizerAttributes(this);
        }

        public override Guid ComponentGuid => new Guid("7b4ece55-07a0-4e87-815a-e3724a1317b1");

        //You can add image files to your project resources and access them like this:
        // return Resources.IconForThisComponent;
        protected override Bitmap Icon => Properties.Resources.IO_Visualizer;
    }

    public class GHVisualizerAttributes : GH_ResizableAttributes<GHVisualizer>
    {
        private const float ArrowBoxSide = 20f;
        private const float ArrowBoxPadding = 10f;
        private const int Padding = 6;

        // keep track of the last exported bitmap to avoid re-exporting unnecessarily...
        private int _lastPlotWidth;
        private int _lastPlotHeight;
        private VisualizerPlot _lastPlot = VisualizerPlot.DemandMonthly;
        private Bitmap _lastBitmap;

        private VisualizerPlot _currentPlot;

        // colors for plots
        private static readonly OxyColor SpaceHeatingColor = OxyColor.FromRgb(255, 0, 0);
        private static readonly OxyColor SpaceCoolingColor = OxyColor.FromRgb(0, 176, 240);
        private static readonly OxyColor ElectricityColor = OxyColor.FromRgb(255, 217, 102);
        private static readonly OxyColor DhwColor = OxyColor.FromRgb(192, 0, 0);
        private static readonly OxyColor BackgroundColor = OxyColor.FromArgb(0, 0, 0, 0);

        public GHVisualizerAttributes(GHVisualizer owner) : base(owner)
        {
            this._currentPlot = VisualizerPlot.DemandMonthly;
        }


        private void NextPlot()
        {

            var values = (VisualizerPlot[])Enum.GetValues(typeof(VisualizerPlot));
            var currentIndex = Array.FindIndex(values, t => t == this._currentPlot);
            this._currentPlot = values[(currentIndex + 1) % values.Length];
        }

        private void PreviousPlot()
        {
            var values = (VisualizerPlot[])Enum.GetValues(typeof(VisualizerPlot));
            var currentIndex = Array.FindIndex(values, t => t == this._currentPlot);
            this._currentPlot = values[(currentIndex - 1 + values.Length) % values.Length];
        }

        // FIXME: what goes here?
        public override string PathName => "PathName_GHVisualizer";

        protected override Size MinimumSize => new Size(50, 50);

        protected override Padding SizingBorders => new Padding(Padding);

        protected override void Layout()
        {
            // make sure we have a minimum size
            var minWidth = 200;
            var minHeight = 150;

            var bounds = this.Bounds;
            bounds.Width = Math.Max(bounds.Width, minWidth);
            bounds.Height = Math.Max(bounds.Height, minHeight);

            this.Bounds = new RectangleF(this.Pivot, bounds.Size);
        }

        private RectangleF PlotBounds
        {
            get
            {
                var plotBounds = this.Bounds;
                plotBounds.Inflate(-Padding, -Padding);
                return plotBounds;
            }
        }

        private PointF PlotLocation
        {
            get
            {
                var plotLocation = this.Bounds.Location;
                plotLocation.X += Padding;
                plotLocation.Y += Padding;
                return plotLocation;
            }
        }

        public override GH_ObjectResponse RespondToMouseDown(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            if (e.Button != MouseButtons.Left)
            {
                return base.RespondToMouseDown(sender, e);
            }

            if (LeftArrowBox.Contains(e.CanvasLocation))
            {
                this.PreviousPlot();
            }

            if (RightArrowBox.Contains(e.CanvasLocation))
            {
                this.NextPlot();
            }
            
            return base.RespondToMouseDown(sender, e);
        }

        protected override void Render(GH_Canvas canvas, Graphics graphics, GH_CanvasChannel channel)
        {
            if (channel == GH_CanvasChannel.Wires && this.Owner.SourceCount > 0)
                this.RenderIncomingWires(canvas.Painter, this.Owner.Sources, this.Owner.WireDisplay);
            if (channel != GH_CanvasChannel.Objects)
                return;

            RenderCapsule(graphics);
            RenderPlot(graphics);
            RenderArrows(graphics);
        }

        /// <summary>
        /// Render the arrows next to the title - we'll be making these click-able to cycle through the plots
        /// </summary>
        /// <param name="graphics"></param>
        private void RenderArrows(Graphics graphics)
        {
            // the style to draw the arrows with
            GH_PaletteStyle impliedStyle = GH_CapsuleRenderEngine.GetImpliedStyle(GH_Palette.Normal, this);
            Color color = impliedStyle.Text;
            var pen = new Pen(color, 1f) { LineJoin = System.Drawing.Drawing2D.LineJoin.Round };

            // the base arrow polygons
            var leftArrow = new[] { new PointF(ArrowBoxSide, 0), new PointF(0, ArrowBoxSide / 2), new PointF(ArrowBoxSide, ArrowBoxSide) };
            var rightArrow = new[] { new PointF(0, 0), new PointF(ArrowBoxSide, ArrowBoxSide / 2), new PointF(0, ArrowBoxSide) };
            
            // shift the polygons to their positions
            leftArrow = leftArrow.Select(p => new PointF(p.X + LeftArrowBox.Left, p.Y + LeftArrowBox.Top)).ToArray();
            rightArrow = rightArrow.Select(p => new PointF(p.X + RightArrowBox.Left, p.Y + RightArrowBox.Top)).ToArray();
            
            graphics.DrawPolygon(pen, leftArrow);
            graphics.DrawPolygon(pen, rightArrow);

            // fill out the polygon
            LinearGradientBrush leftBrush = new LinearGradientBrush(LeftArrowBox, color,
                GH_GraphicsUtil.OffsetColour(color, 50), LinearGradientMode.Vertical) {WrapMode = WrapMode.TileFlipXY};
            graphics.FillPolygon(leftBrush, leftArrow);
            leftBrush.Dispose();

            LinearGradientBrush rightBrush = new LinearGradientBrush(RightArrowBox, color,
                GH_GraphicsUtil.OffsetColour(color, 50), LinearGradientMode.Vertical) {WrapMode = WrapMode.TileFlipXY};
            graphics.FillPolygon(rightBrush, rightArrow);
            rightBrush.Dispose();
        }

        private RectangleF LeftArrowBox => new RectangleF(PlotBounds.Left + ArrowBoxPadding, PlotBounds.Top + ArrowBoxPadding, ArrowBoxSide, ArrowBoxSide);
        private RectangleF RightArrowBox => new RectangleF(PlotBounds.Right - ArrowBoxSide - ArrowBoxPadding, PlotBounds.Top + ArrowBoxPadding, ArrowBoxSide, ArrowBoxSide);

        private void RenderPlot(Graphics graphics)
        {
            var plotWidth = (int) this.PlotBounds.Width;
            var plotHeight = (int) this.PlotBounds.Height;

            if (IsBitmapCacheStillValid(plotWidth, plotHeight))
            {
                graphics.DrawImage(_lastBitmap, this.PlotLocation.X, this.PlotLocation.Y, this.PlotBounds.Width, this.PlotBounds.Height);
            }
            else
            {
                PlotModel model;
                switch (this._currentPlot)
                {
                    case VisualizerPlot.DemandMonthly:
                        model = DemandMonthlyPlotModel();
                        break;
                    case VisualizerPlot.DemandMonthlyNormalized:
                        model = DemandMonthlyNormalizedPlotModel();
                        break;
                    default:
                        model = DemandMonthlyPlotModel();
                        break;
                }

                RhinoApp.WriteLine("RenderPlot: Before exporting Bitmap");
                var pngExporter = new PngExporter
                    { Width = plotWidth, Height = plotHeight, Background = OxyColors.White };
                var bitmap = pngExporter.ExportToBitmap(model);
                RhinoApp.WriteLine("RenderPlot: After exporting Bitmap");
                graphics.DrawImage(bitmap, this.PlotLocation.X, this.PlotLocation.Y, this.PlotBounds.Width, this.PlotBounds.Height);

                _lastPlotWidth = plotWidth;
                _lastPlotHeight = plotHeight;
                _lastPlot = _currentPlot;
                _lastBitmap = bitmap;
            }
        }

        private bool IsBitmapCacheStillValid(int plotWidth, int plotHeight)
        {
            return !(_lastBitmap is null) && plotWidth == _lastPlotWidth && plotHeight == _lastPlotHeight && _currentPlot == _lastPlot;
        }

        private void RenderCapsule(Graphics graphics)
        {
            GH_Capsule capsule = this.Owner.RuntimeMessageLevel != GH_RuntimeMessageLevel.Error
                ? GH_Capsule.CreateCapsule(this.Bounds, GH_Palette.Hidden, 5, 30)
                : GH_Capsule.CreateCapsule(this.Bounds, GH_Palette.Error, 5, 30);
            capsule.SetJaggedEdges(false, true);
            capsule.AddInputGrip(this.InputGrip);
            capsule.Render(graphics, this.Selected, this.Owner.Locked, true);
            capsule.Dispose();
        }

        private PlotModel DemandMonthlyPlotModel()
        {
            const int months = 12;
            var model = new PlotModel {Title = "Energy demand (Total Monthly)"};

            var resultsTotalHeatingMonthly = Owner.Results.TotalHeatingMonthly ?? new double[months];
            var demandHeating = new ColumnSeries
            {
                ItemsSource = resultsTotalHeatingMonthly.Select(demand => new ColumnItem {Value = demand}),
                Title = " Space Heating",
                FillColor = SpaceHeatingColor
            };
            model.Series.Add(demandHeating);

            var resultsTotalCoolingMonthly = Owner.Results.TotalCoolingMonthly ?? new double[months];
            var demandCooling = new ColumnSeries
            {
                ItemsSource = resultsTotalCoolingMonthly.Select(demand => new ColumnItem {Value = demand}),
                Title = " Space Cooling",
                FillColor = SpaceCoolingColor
            };
            model.Series.Add(demandCooling);

            var resultsTotalElectricityMonthly = Owner.Results.TotalElectricityMonthly ?? new double[months];
            var demandElectricity = new ColumnSeries
            {
                ItemsSource = resultsTotalElectricityMonthly.Select(demand => new ColumnItem {Value = demand}),
                Title = " Electricity",
                FillColor = ElectricityColor
            };
            model.Series.Add(demandElectricity);

            var resultsTotalDwhMonthly = Owner.Results.TotalDHWMonthly ?? new double[months];
            var demandDhw = new ColumnSeries
            {
                ItemsSource = resultsTotalDwhMonthly.Select(demand => new ColumnItem {Value = demand}),
                Title = " DWH",
                FillColor = DhwColor,
            };
            model.Series.Add(demandDhw);

            model.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Left,
                Key = "Demand",
                Title = "kWh"
            });
            
            model.Axes.Add(new CategoryAxis
            {
                Position = AxisPosition.Bottom,
                Key = "Months",
                ItemsSource = new[]
                {
                    "Jan",
                    "Feb",
                    "Mar",
                    "Apr",
                    "May",
                    "Jun",
                    "Jul",
                    "Aug",
                    "Sep",
                    "Oct",
                    "Nov",
                    "Dec"
                }
            });
            return model;
        }

        private PlotModel DemandMonthlyNormalizedPlotModel()
        {
            const int months = 12;
            var model = new PlotModel { Title = "Energy demand (Normalized Monthly)" };
            var totalFloorArea = Owner.Results.TotalFloorArea;

            var resultsTotalHeatingMonthly = Owner.Results.TotalHeatingMonthly ?? new double[months];
            var strokeThickness = 4.0;

            var demandHeating = new ColumnSeries
            {
                ItemsSource = resultsTotalHeatingMonthly.Select(demand => new ColumnItem { Value = demand / totalFloorArea }),
                Title = " Space Heating",
                FillColor = BackgroundColor,
                StrokeThickness = strokeThickness,
                StrokeColor = SpaceHeatingColor
            };
            model.Series.Add(demandHeating);

            var resultsTotalCoolingMonthly = Owner.Results.TotalCoolingMonthly ?? new double[months];
            var demandCooling = new ColumnSeries
            {
                ItemsSource = resultsTotalCoolingMonthly.Select(demand => new ColumnItem { Value = demand / totalFloorArea }),
                Title = " Space Cooling",
                FillColor = BackgroundColor,
                StrokeThickness = strokeThickness,
                StrokeColor = SpaceCoolingColor
            };
            model.Series.Add(demandCooling);

            var resultsTotalElectricityMonthly = Owner.Results.TotalElectricityMonthly ?? new double[months];
            var demandElectricity = new ColumnSeries
            {
                ItemsSource = resultsTotalElectricityMonthly.Select(
                    demand => new ColumnItem { Value = demand / totalFloorArea }),
                Title = " Electricity",
                FillColor = BackgroundColor,
                StrokeThickness = strokeThickness,
                StrokeColor = ElectricityColor
            };
            model.Series.Add(demandElectricity);

            var resultsTotalDwhMonthly = Owner.Results.TotalDHWMonthly ?? new double[months];
            var demandDhw = new ColumnSeries
            {
                ItemsSource = resultsTotalDwhMonthly.Select(demand => new ColumnItem { Value = demand / totalFloorArea }),
                Title = " DWH",
                FillColor = BackgroundColor,
                StrokeThickness = strokeThickness,
                StrokeColor = DhwColor,
            };
            model.Series.Add(demandDhw);

            model.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Left,
                Key = "Demand",
                Title = "kWh/m^2"
            });

            model.Axes.Add(new CategoryAxis
            {
                Position = AxisPosition.Bottom,
                Key = "Months",
                ItemsSource = new[]
                {
                    "Jan",
                    "Feb",
                    "Mar",
                    "Apr",
                    "May",
                    "Jun",
                    "Jul",
                    "Aug",
                    "Sep",
                    "Oct",
                    "Nov",
                    "Dec"
                }
            });
            return model;
        }


        public void ClearBitmapCache()
        {
            RhinoApp.WriteLine("Clearing bitmap cache");
            this._lastBitmap = null;
        }
    }
}
