using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Attributes;
using Hive.IO.Plots;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot.WindowsForms;
using Rhino;

namespace Hive.IO.GHComponents
{
    public class GHVisualizerAttributes : GH_ResizableAttributes<GHVisualizer>
    {
        private const float ArrowBoxSide = 20f;
        private const float ArrowBoxPadding = 10f;
        private const int Padding = 6;

        private readonly IVisualizerPlot[] plots;
        private int currentPlot;

        public GHVisualizerAttributes(GHVisualizer owner) : base(owner)
        {
            currentPlot = 0;
            plots = new IVisualizerPlot[]
            {
                new DemandMonthlyPlot(),
                new DemandMonthlyNormalizedPlot()
            };
        }

        public void NewData(Results results)
        {
            foreach (var plot in plots)
            {
                plot.NewData(results);
            }
        }


        private void NextPlot()
        {
            currentPlot = (currentPlot + 1) % plots.Length;
        }

        private void PreviousPlot()
        {
            currentPlot = (currentPlot - 1 + plots.Length) % plots.Length;
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

            plots[currentPlot].Render(Owner.Results, graphics, PlotBounds);
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
    }
}