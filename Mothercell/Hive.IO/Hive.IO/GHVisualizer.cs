using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;

using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Grasshopper.Kernel.Attributes;
using System.Windows.Forms;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot.WindowsForms;

namespace Hive.IO
{
    public enum VisualizerPlot
    {
        DemandMonthly,
        DemandHourly,
    }

    public class GHVisualizer : GH_Param<GH_ObjectWrapper>
    {
        private VisualizerPlot currentPlot;

        public GHVisualizer() : base("Hive.IO.Visualizer", "Hive.IO.Visualizer",
              "Hive Visualizer for simulation results",
              "[hive]", "IO", GH_ParamAccess.item)
        {
            currentPlot = VisualizerPlot.DemandMonthly;
        }

        public Results Results { get; private set; }

        public VisualizerPlot CurrentPlot => currentPlot;

        public void NextPlot()
        {
            var values = (VisualizerPlot[])Enum.GetValues(typeof(VisualizerPlot));
            var currentIndex = Array.FindIndex(values, t => t == this.currentPlot);
            this.currentPlot = values[(currentIndex + 1) % values.Length];
        }

        public void PreviousPlot()
        {
            var values = (VisualizerPlot[])Enum.GetValues(typeof(VisualizerPlot));
            var currentIndex = Array.FindIndex(values, t => t == this.currentPlot);
            this.currentPlot = values[(currentIndex - 1) % values.Length];
        }

        public override GH_ParamKind Kind
        {
            get
            {
                return GH_ParamKind.floating;
            }
        }

        public override string TypeName
        {
            get
            {
                return "HiveResults";
            }
        }

        public override GH_Exposure Exposure
        {
            get
            {
                return GH_Exposure.primary;
            }
        }

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
        }


        public override void CreateAttributes()
        {
            m_attributes = new GHVisualizerAttributes(this);
        }

        public override Guid ComponentGuid
        {
            get
            {
                return new Guid("7b4ece55-07a0-4e87-815a-e3724a1317b1");
            }
        }
    }

    public class GHVisualizerAttributes : GH_ResizableAttributes<GHVisualizer>
    {
        private const float ArrowBoxSide = 10f;
        private const float ArrowBoxPadding = 10f;
        private int m_padding = 6;

        public GHVisualizerAttributes(GHVisualizer owner) : base(owner)
        {
        }

        public override string PathName
        {
            get
            {
                // FIXME: what goes here?
                return "PathName_GHVisualizer";
            }
        }

        protected override Size MinimumSize
        {
            get
            {
                return new Size(50, 50);
            }
        }

        protected override Padding SizingBorders => new Padding(this.m_padding);

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
                plotBounds.Inflate(-m_padding, -m_padding);
                return plotBounds;
            }
        }

        private PointF PlotLocation
        {
            get
            {
                var plotLocation = this.Bounds.Location;
                plotLocation.X += m_padding;
                plotLocation.Y += m_padding;
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
                this.Owner.PreviousPlot();
                return GH_ObjectResponse.Handled;
            }

            if (RightArrowBox.Contains(e.CanvasLocation))
            {
                this.Owner.NextPlot();
                return GH_ObjectResponse.Handled;
            }
            

            return base.RespondToMouseDown(sender, e);
        }

        protected override void Render(GH_Canvas canvas, Graphics graphics, GH_CanvasChannel channel)
        {
            if (channel == GH_CanvasChannel.Wires && this.Owner.SourceCount > 0)
                this.RenderIncomingWires(canvas.Painter, (IEnumerable<IGH_Param>)this.Owner.Sources, this.Owner.WireDisplay);
            if (channel != GH_CanvasChannel.Objects)
                return;

            RenderCapsule(canvas, graphics);
            RenderPlot(graphics);
            RenderArrows(graphics);
        }

        /// <summary>
        /// Render the arrows next to the title - we'll be making these clickable to cycle through the plots
        /// </summary>
        /// <param name="graphics"></param>
        private void RenderArrows(Graphics graphics)
        {
            // the style to draw the arrows with
            GH_PaletteStyle impliedStyle = GH_CapsuleRenderEngine.GetImpliedStyle(GH_Palette.Normal, (IGH_Attributes)this);
            Color color = impliedStyle.Text;
            var pen = new Pen(color, 1f) { LineJoin = System.Drawing.Drawing2D.LineJoin.Round };

            // the base arrow polygons
            var leftArrow = new PointF[] { new PointF(ArrowBoxSide, 0), new PointF(0, ArrowBoxSide / 2), new PointF(ArrowBoxSide, ArrowBoxSide) };
            var rightArrow = new PointF[] { new PointF(0, 0), new PointF(ArrowBoxSide, ArrowBoxSide / 2), new PointF(0, ArrowBoxSide) };
            
            // shift the polygons to their positions
            leftArrow = leftArrow.Select(p => new PointF(p.X + LeftArrowBox.Left, p.Y + LeftArrowBox.Top)).ToArray();
            rightArrow = rightArrow.Select(p => new PointF(p.X + RightArrowBox.Left, p.Y + RightArrowBox.Top)).ToArray();
            
            graphics.DrawPolygon(pen, leftArrow);
            graphics.DrawPolygon(pen, rightArrow);

            // fill out the polygon
            LinearGradientBrush leftBrush = new LinearGradientBrush(LeftArrowBox, color, GH_GraphicsUtil.OffsetColour(color, 50), LinearGradientMode.Vertical);
            leftBrush.WrapMode = WrapMode.TileFlipXY;
            graphics.FillPolygon((Brush)leftBrush, leftArrow);
            leftBrush.Dispose();

            LinearGradientBrush rightBrush = new LinearGradientBrush(RightArrowBox, color, GH_GraphicsUtil.OffsetColour(color, 50), LinearGradientMode.Vertical);
            rightBrush.WrapMode = WrapMode.TileFlipXY;
            graphics.FillPolygon((Brush)rightBrush, rightArrow);
            rightBrush.Dispose();
        }

        private RectangleF LeftArrowBox => new RectangleF(PlotBounds.Left + ArrowBoxPadding, PlotBounds.Top + ArrowBoxPadding, ArrowBoxSide, ArrowBoxSide);
        private RectangleF RightArrowBox => new RectangleF(PlotBounds.Right - ArrowBoxSide - ArrowBoxPadding, PlotBounds.Top + ArrowBoxPadding, ArrowBoxSide, ArrowBoxSide);

        private void RenderPlot(Graphics graphics)
        {
            PlotModel model;
            switch (this.Owner.CurrentPlot)
            {
                case VisualizerPlot.DemandMonthly:
                    model = DemandMonthlyPlotModel();
                    break;
                case VisualizerPlot.DemandHourly:
                    model = DemandHourlyPlotModel();
                    break;
                default:
                    model = DemandMonthlyPlotModel();
                    break;
            }

            var pngExporter = new PngExporter
                {Width = (int) this.PlotBounds.Width, Height = (int) this.PlotBounds.Height, Background = OxyColors.White};
            var bitmap = pngExporter.ExportToBitmap(model);
            graphics.DrawImage(bitmap, this.PlotLocation.X, this.PlotLocation.Y, this.PlotBounds.Width, this.PlotBounds.Height);
        }

        private void RenderCapsule(GH_Canvas canvas, Graphics graphics)
        {
            GH_Viewport viewport = canvas.Viewport;
            RectangleF bounds = this.Bounds;
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
            var model = new PlotModel {Title = "Demand"};

            var demandHeating = new ColumnSeries
            {
                ItemsSource = Owner.Results.TotalHtgMonthly.Select(demand => new ColumnItem {Value = demand}),

                LabelPlacement = LabelPlacement.Inside,
                LabelFormatString = "{0:.00}",
                Title = " Heating Demand"
            };
            model.Series.Add(demandHeating);

            var demandCooling = new ColumnSeries
            {
                ItemsSource = Owner.Results.TotalClgMonthly.Select(demand => new ColumnItem {Value = demand}),

                LabelPlacement = LabelPlacement.Inside,
                LabelFormatString = "{0:.00}",
                Title = " Cooling Demand"
            };
            model.Series.Add(demandCooling);

            var demandElectricity = new ColumnSeries
            {
                ItemsSource = Owner.Results.TotalElecMonthly.Select(demand => new ColumnItem {Value = demand}),
                LabelPlacement = LabelPlacement.Inside,
                LabelFormatString = "{0:.00}",
                Title = " Electricity Demand"
            };
            model.Series.Add(demandElectricity);

            model.Axes.Add(new CategoryAxis
            {
                Position = AxisPosition.Bottom,
                Key = "Months",
                ItemsSource = new[]
                {
                    "J",
                    "F",
                    "M",
                    "A",
                    "M",
                    "J",
                    "J",
                    "A",
                    "O",
                    "N",
                    "D"
                }
            });
            return model;
        }

        private PlotModel DemandHourlyPlotModel()
        {
            var model = new PlotModel { Title = "Demand Hourly" };

            var demandHeating = new ColumnSeries
            {
                ItemsSource = Owner.Results.TotalHtgHourly.Select(demand => new ColumnItem { Value = demand }),
                Title = " Heating Demand"
            };
            model.Series.Add(demandHeating);

            var demandCooling = new ColumnSeries
            {
                ItemsSource = Owner.Results.TotalClgHourly.Select(demand => new ColumnItem { Value = demand }),
                Title = " Cooling Demand"
            };
            model.Series.Add(demandCooling);
            return model;
        }
    }
}
