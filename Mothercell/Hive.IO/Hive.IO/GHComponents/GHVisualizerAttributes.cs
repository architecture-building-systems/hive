using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Attributes;
using Hive.IO.Plots;

namespace Hive.IO.GHComponents
{
    public class GhVisualizerAttributes : GH_ResizableAttributes<GHVisualizer>
    {
        // make sure we have a minimum size
        private const float TitleBarHeight = 200f;
        private const float MinWidth = 200f;
        private const float MinHeight = TitleBarHeight + 150f;

        private const float ArrowBoxSide = 20f;
        private const float ArrowBoxPadding = 10f;
        private const int Padding = 6;

        private readonly IVisualizerPlot[] _plots = {
            new DemandMonthlyPlot(),
            new DemandMonthlyNormalizedPlot()
        };

        private readonly OperationalPerformancePlot[] _titleBarPlots;
        private int _currentPlot;

        public GhVisualizerAttributes(GHVisualizer owner) : base(owner)
        {
            _currentPlot = 0;


            var energyPlotConfig = new EnergyPlotProperties
            {
                Color = Color.FromArgb(144, 153, 40),
                BenchmarkFailedColor = Color.FromArgb(166, 78, 2),
                UnitText = "kWh",
                SpecificUnitText = "kWh/m²",
                Data = results => 1530.0,
                SpecificData = results => 153.0
            };
            var emissionsPlotConfig = new EnergyPlotProperties
            {
                Color = Color.FromArgb(136, 219, 68),
                BenchmarkFailedColor = Color.FromArgb(166, 78, 2),
                UnitText = "kgCO2",
                SpecificUnitText = "kgCO2//m²",
                Data = results => 790.0,
                SpecificData = results => 79.0
            };
            var costsPlotConfig = new EnergyPlotProperties
            {
                Color = Color.FromArgb(222, 180, 109),
                BenchmarkFailedColor = Color.FromArgb(166, 78, 2),
                UnitText = "CHF",
                SpecificUnitText = "CHF/m²",
                Data = results => 1000.0,
                SpecificData = results => 120.0
            };

            _titleBarPlots = new[]
            {
                // from the right
                new OperationalPerformancePlot(TitleBarHeight, costsPlotConfig),
                new OperationalPerformancePlot(TitleBarHeight, emissionsPlotConfig),
                new OperationalPerformancePlot(TitleBarHeight, energyPlotConfig)
            };
        }

        public void NewData(Results results)
        {
            foreach (var plot in AllPlots)
            {
                plot.NewData(results);
            }
        }

        private IEnumerable<IVisualizerPlot> AllPlots => _plots?.AsEnumerable().Concat(_titleBarPlots) 
                                                         ?? new List<IVisualizerPlot>();

        private void NextPlot()
        {
            _currentPlot = (_currentPlot + 1) % _plots.Length;
        }

        private void PreviousPlot()
        {
            _currentPlot = (_currentPlot - 1 + _plots.Length) % _plots.Length;
        }

        // FIXME: what goes here?
        public override string PathName => "PathName_GHVisualizer";

        protected override Size MinimumSize => new Size(50, 50);

        protected override Padding SizingBorders => new Padding(Padding);

        protected override void Layout()
        {
            var bounds = Bounds;
            bounds.Width = Math.Max(bounds.Width, MinWidth);
            bounds.Height = Math.Max(bounds.Height, MinHeight);

            Bounds = new RectangleF(Pivot, bounds.Size);
        }

        private RectangleF PlotBounds
        {
            get
            {
                var plotBounds = Bounds;
                plotBounds.Height -= TitleBarHeight;
                plotBounds.Offset(0, TitleBarHeight);

                plotBounds.Inflate(-Padding, -Padding);
                return plotBounds;
            }
        }

        public override GH_ObjectResponse RespondToMouseDown(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            if (e.Button != MouseButtons.Left)
            {
                return base.RespondToMouseDown(sender, e);
            }

            foreach (var plot in _titleBarPlots)
            {
                if (plot.Contains(e.CanvasLocation))
                {
                    plot.Clicked(sender);
                }
            }

            if (LeftArrowBox.Contains(e.CanvasLocation))
            {
                PreviousPlot();
            }

            if (RightArrowBox.Contains(e.CanvasLocation))
            {
                NextPlot();
            }

            return base.RespondToMouseDown(sender, e);
        }

        protected override void Render(GH_Canvas canvas, Graphics graphics, GH_CanvasChannel channel)
        {
            if (channel == GH_CanvasChannel.Wires && Owner.SourceCount > 0)
                RenderIncomingWires(canvas.Painter, Owner.Sources, Owner.WireDisplay);
            if (channel != GH_CanvasChannel.Objects)
                return;

            RenderCapsule(graphics);
            RenderPlot(graphics);
            RenderTitleBar(graphics);
            RenderArrows(graphics);
        }

        /// <summary>
        /// Render the title bar at the top with the dropdown for selecting the plot and the
        /// operational performance metrics.
        /// </summary>
        /// <param name="graphics"></param>
        private void RenderTitleBar(Graphics graphics)
        {
            // the style to draw the dropdown arrow with
            var impliedStyle = GH_CapsuleRenderEngine.GetImpliedStyle(GH_Palette.Normal, this);
            var color = impliedStyle.Text;
            var pen = new Pen(color, 1f) { LineJoin = LineJoin.Round };

            // draw the dropdown for the selecting the plots
            var dropDownArrow = new[] { new PointF(0, 0), new PointF(ArrowBoxSide, 0), new PointF(ArrowBoxSide / 2, ArrowBoxSide) };
            dropDownArrow = dropDownArrow.Select(p => new PointF(p.X + DropDownArrowBox.Left, p.Y + DropDownArrowBox.Top)).ToArray();

            graphics.DrawPolygon(pen, dropDownArrow);

            // render the three operational performance plots
            var plotWidth = _titleBarPlots[0].Width;
            var bounds = new RectangleF(Bounds.Right - plotWidth - Padding, Bounds.Location.Y, plotWidth, TitleBarHeight);
            foreach (var plot in _titleBarPlots)
            {
                plot.Render(Owner.Results, graphics, bounds);
                bounds.Offset(-(plotWidth + Padding), 0);
            }
        }

        private RectangleF DropDownArrowBox => new RectangleF(Bounds.Left + Padding, Bounds.Top + Padding, ArrowBoxSide, ArrowBoxSide);

        /// <summary>
        /// Render the arrows next to the title - we'll be making these click-able to cycle through the plots
        /// </summary>
        /// <param name="graphics"></param>
        private void RenderArrows(Graphics graphics)
        {
            // the style to draw the arrows with
            var impliedStyle = GH_CapsuleRenderEngine.GetImpliedStyle(GH_Palette.Normal, this);
            var color = impliedStyle.Text;
            var pen = new Pen(color, 1f) { LineJoin = LineJoin.Round };

            // the base arrow polygons
            var leftArrow = new[] { new PointF(ArrowBoxSide, 0), new PointF(0, ArrowBoxSide / 2), new PointF(ArrowBoxSide, ArrowBoxSide) };
            var rightArrow = new[] { new PointF(0, 0), new PointF(ArrowBoxSide, ArrowBoxSide / 2), new PointF(0, ArrowBoxSide) };

            // shift the polygons to their positions
            leftArrow = leftArrow.Select(p => new PointF(p.X + LeftArrowBox.Left, p.Y + LeftArrowBox.Top)).ToArray();
            rightArrow = rightArrow.Select(p => new PointF(p.X + RightArrowBox.Left, p.Y + RightArrowBox.Top)).ToArray();

            graphics.DrawPolygon(pen, leftArrow);
            graphics.DrawPolygon(pen, rightArrow);

            // fill out the polygon
            var leftBrush = new LinearGradientBrush(LeftArrowBox, color,
                GH_GraphicsUtil.OffsetColour(color, 50), LinearGradientMode.Vertical)
            { WrapMode = WrapMode.TileFlipXY };
            graphics.FillPolygon(leftBrush, leftArrow);
            leftBrush.Dispose();

            var rightBrush = new LinearGradientBrush(RightArrowBox, color,
                GH_GraphicsUtil.OffsetColour(color, 50), LinearGradientMode.Vertical)
            { WrapMode = WrapMode.TileFlipXY };
            graphics.FillPolygon(rightBrush, rightArrow);
            rightBrush.Dispose();
        }

        private RectangleF LeftArrowBox => new RectangleF(PlotBounds.Left + ArrowBoxPadding, PlotBounds.Top + ArrowBoxPadding, ArrowBoxSide, ArrowBoxSide);
        private RectangleF RightArrowBox => new RectangleF(PlotBounds.Right - ArrowBoxSide - ArrowBoxPadding, PlotBounds.Top + ArrowBoxPadding, ArrowBoxSide, ArrowBoxSide);

        private void RenderPlot(Graphics graphics)
        {
            _plots[_currentPlot].Render(Owner.Results, graphics, PlotBounds);
        }

        private void RenderCapsule(Graphics graphics)
        {
            var capsule = Owner.RuntimeMessageLevel != GH_RuntimeMessageLevel.Error
                ? GH_Capsule.CreateCapsule(Bounds, GH_Palette.Hidden, 5, 30)
                : GH_Capsule.CreateCapsule(Bounds, GH_Palette.Error, 5, 30);
            capsule.SetJaggedEdges(false, true);
            capsule.AddInputGrip(InputGrip);
            capsule.Render(graphics, Selected, Owner.Locked, true);
            capsule.Dispose();
        }
    }
}