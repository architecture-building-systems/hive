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

        private readonly IVisualizerPlot[] _plots;
        private readonly OperationalPerformancePlot[] _titleBarPlots;
        private int _currentPlot;

        public GhVisualizerAttributes(GHVisualizer owner) : base(owner)
        {
            _currentPlot = 0;
            _plots = new IVisualizerPlot[]
            {
                new DemandMonthlyPlot(),
                new DemandMonthlyNormalizedPlot()
            };

            _titleBarPlots = new[]
            {
                new OperationalPerformancePlot(TitleBarHeight),
                new OperationalPerformancePlot(TitleBarHeight),
                new OperationalPerformancePlot(TitleBarHeight)
            };
        }

        public void NewData(Results results)
        {
            foreach (var plot in AllPlots)
            {
                plot.NewData(results);
            }
        }

        private IEnumerable<IVisualizerPlot> AllPlots => _plots.AsEnumerable().Concat(_titleBarPlots);

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
            var bounds = this.Bounds;
            bounds.Width = Math.Max(bounds.Width, MinWidth);
            bounds.Height = Math.Max(bounds.Height, MinHeight);

            this.Bounds = new RectangleF(this.Pivot, bounds.Size);
        }

        private RectangleF PlotBounds
        {
            get
            {
                var plotBounds = this.Bounds;
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
                bounds.Offset(-plotWidth, 0);
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
            var capsule = this.Owner.RuntimeMessageLevel != GH_RuntimeMessageLevel.Error
                ? GH_Capsule.CreateCapsule(this.Bounds, GH_Palette.Hidden, 5, 30)
                : GH_Capsule.CreateCapsule(this.Bounds, GH_Palette.Error, 5, 30);
            capsule.SetJaggedEdges(false, true);
            capsule.AddInputGrip(this.InputGrip);
            capsule.Render(graphics, this.Selected, this.Owner.Locked, true);
            capsule.Dispose();
        }
    }
}