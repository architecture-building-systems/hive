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
    public class GhVisualizerAttributes : GH_ResizableAttributes<GhVisualizer>
    {
        // make sure we have a minimum size
        private const float TitleBarHeight = 200f;
        private const float MinWidth = 200f;
        private const float MinHeight = TitleBarHeight + 150f;

        private const float ArrowBoxSide = TitleBarHeight;
        private const float ArrowBoxPadding = 50f;
        private const int Padding = 6;

        private readonly IVisualizerPlot[] _plots = {
            new DemandMonthlyPlot(),
            new DemandMonthlyNormalizedPlot()
        };

        private readonly OperationalPerformancePlot[] _titleBarPlots;
        private int _currentPlot;

        public GhVisualizerAttributes(GhVisualizer owner) : base(owner)
        {
            _currentPlot = 0;


            var energyPlotConfig = new EnergyPlotProperties
            {
                Color = Color.FromArgb(225, 242, 31),
                BenchmarkFailedColor = Color.FromArgb(166, 78, 2),
                UnitText = "kWh",
                NormalizedUnitText = "kWh/m²",
                Data = results => 1530.0,
                NormalizedData = results => 153.0
            };
            var emissionsPlotConfig = new EnergyPlotProperties
            {
                Color = Color.FromArgb(136, 219, 68),
                BenchmarkFailedColor = Color.FromArgb(166, 78, 2),
                UnitText = "kgCO₂",
                NormalizedUnitText = "kgCO₂/m²",
                Data = results => 790.0,
                NormalizedData = results => 79.0
            };
            var costsPlotConfig = new EnergyPlotProperties
            {
                Color = Color.FromArgb(222, 180, 109),
                BenchmarkFailedColor = Color.FromArgb(166, 78, 2),
                UnitText = "CHF",
                NormalizedUnitText = "CHF/m²",
                Data = results => 1000.0,
                NormalizedData = results => 120.0
            };

            _titleBarPlots = new[]
            {
                // from the right
                new OperationalPerformancePlot(costsPlotConfig),
                new OperationalPerformancePlot(emissionsPlotConfig),
                new OperationalPerformancePlot(energyPlotConfig)
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

            if (DropDownArrowBox.Contains(e.CanvasLocation))
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
            var brush = new SolidBrush(color);

            // draw the dropdown for the selecting the plots
            var dropDownArrow = new[] 
            { 
                new PointF(ArrowBoxPadding, ArrowBoxPadding), // top left
                new PointF(ArrowBoxSide - ArrowBoxPadding, ArrowBoxPadding), // top right
                new PointF(ArrowBoxSide / 2, ArrowBoxSide - ArrowBoxPadding) // bottom middle
            };
            dropDownArrow = dropDownArrow.Select(p => new PointF(
                p.X + DropDownArrowBox.Left, 
                p.Y + DropDownArrowBox.Top)).ToArray();

            graphics.FillPolygon(brush, dropDownArrow);

            // render the three operational performance plots
            var plotWidth = TitleBarHeight;  // squares
            var bounds = new RectangleF(Bounds.Right - plotWidth - Padding, Bounds.Location.Y, plotWidth, TitleBarHeight);
            foreach (var plot in _titleBarPlots)
            {
                plot.Render(Owner.Results, graphics, bounds);
                bounds.Offset(-(plotWidth + Padding), 0);
            }
        }

        private RectangleF DropDownArrowBox => new RectangleF(Bounds.Left + Padding, Bounds.Top + Padding, ArrowBoxSide, ArrowBoxSide);

        private void RenderPlot(Graphics graphics)
        {
            _plots[_currentPlot].Render(Owner.Results, graphics, PlotBounds);
        }

        private void RenderCapsule(Graphics graphics)
        {
            var capsule = Owner.RuntimeMessageLevel != GH_RuntimeMessageLevel.Error
                ? GH_Capsule.CreateCapsule(Bounds, GH_Palette.White, 5, 30)
                : GH_Capsule.CreateCapsule(Bounds, GH_Palette.Error, 5, 30);
            capsule.SetJaggedEdges(false, true);
            capsule.AddInputGrip(InputGrip);
            capsule.Render(graphics, Selected, Owner.Locked, true);
            capsule.Dispose();
        }
    }
}