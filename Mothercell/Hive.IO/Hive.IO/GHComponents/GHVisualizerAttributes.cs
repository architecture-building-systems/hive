using System;
using System.Drawing;
using System.Windows.Forms;
using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Attributes;
using Hive.IO.DataHandling;
using Hive.IO.Plots;

namespace Hive.IO.GHComponents
{
    public class GhVisualizerAttributes : GH_ResizableAttributes<GhVisualizer>
    {
        // make sure we have a minimum size
        private const float TitleBarHeight = 200f;
        private const float MinWidth = 200f;
        private const float MinHeight = TitleBarHeight + 150f;

        private const int Padding = 6;

        private readonly PlotSelector _plotSelector = new PlotSelector();
        private readonly OperationalPerformancePlot[] _kpiPlots;

        public GhVisualizerAttributes(GhVisualizer owner) : base(owner)
        {
            var energyKpiConfig = new KpiPlotProperties
            {
                Color = Color.FromArgb(225, 242, 31),
                BenchmarkFailedColor = Color.FromArgb(166, 78, 2),
                UnitText = "kWh",
                NormalizedUnitText = "kWh/m²",
                Data = (results, normalized) => results.TotalEnergy(normalized),
                Kpi = Kpi.Energy
            };
            var emissionsKpiConfig = new KpiPlotProperties
            {
                Color = Color.FromArgb(136, 219, 68),
                BenchmarkFailedColor = Color.FromArgb(166, 78, 2),
                UnitText = "kgCO₂",
                NormalizedUnitText = "kgCO₂/m²",
                Data = (results, normalized) => results.TotalEmissions(normalized),
                Kpi = Kpi.Emissions
            };
            var costsKpiConfig = new KpiPlotProperties
            {
                Color = Color.FromArgb(222, 180, 109),
                BenchmarkFailedColor = Color.FromArgb(166, 78, 2),
                UnitText = "CHF",
                NormalizedUnitText = "CHF/m²",
                Data = (results, normalized) => results.TotalCosts(normalized),
                Kpi = Kpi.Costs
            };

            var costsKpi = new OperationalPerformancePlot(costsKpiConfig);
            costsKpi.OnClicked += _plotSelector.CostsKpiClicked;

            var emissionsKpi = new OperationalPerformancePlot(emissionsKpiConfig);
            emissionsKpi.OnClicked += _plotSelector.EmissionsKpiClicked;

            var energyKpi = new OperationalPerformancePlot(energyKpiConfig);
            energyKpi.OnClicked += _plotSelector.EnergyKpiClicked;

            _kpiPlots = new[]
            {
                // from the right
                costsKpi,
                emissionsKpi,
                energyKpi
            };
        }

        public void NewData(ResultsPlotting results)
        {
            _plotSelector.CurrentPlot.NewData(results);
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

        private RectangleF InnerBounds => Bounds.CloneInflate(-Padding, -Padding);

        private RectangleF PlotBounds
        {
            get
            {
                var plotBounds = InnerBounds;
                plotBounds.Height -= TitleBarHeight;
                plotBounds.Offset(0, TitleBarHeight);
                return plotBounds;
            }
        }

        public override GH_ObjectResponse RespondToMouseDown(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            if (e.Button != MouseButtons.Left)
            {
                return base.RespondToMouseDown(sender, e);
            }

            if (_plotSelector.Contains(e.CanvasLocation))
            {
                _plotSelector.Clicked(sender, e);
                return base.RespondToMouseDown(sender, e);
            }

            foreach (var kpi in _kpiPlots)
            {
                if (kpi.Contains(e.CanvasLocation))
                {
                    kpi.Clicked(sender, e);
                    return base.RespondToMouseDown(sender, e);
                }
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
            graphics.FillRectangle(new SolidBrush(Color.White), InnerBounds);
            RenderPlot(graphics);
            RenderTitleBar(graphics);
        }

        private RectangleF MenuPanelBounds => new RectangleF(InnerBounds.X, InnerBounds.Y, InnerBounds.Width, TitleBarHeight);

        /// <summary>
        /// Render the title bar at the top with the dropdown for selecting the plot and the
        /// operational performance metrics.
        /// </summary>
        /// <param name="graphics"></param>
        private void RenderTitleBar(Graphics graphics)
        {
            _plotSelector.Render(Owner.Results, graphics, MenuPanelBounds);

            // render the three operational performance plots
            var plotWidth = TitleBarHeight;  // squares
            var bounds = new RectangleF(InnerBounds.Right - plotWidth, InnerBounds.Location.Y, plotWidth, TitleBarHeight);
            foreach (var kpi in _kpiPlots)
            {
                kpi.Normalized = _plotSelector.Normalized;
                kpi.Render(Owner.Results, graphics, bounds, _plotSelector.CurrentKpi == kpi.Kpi);
                bounds.Offset(-(plotWidth + Padding), 0);
            }
        }

        private void RenderPlot(Graphics graphics)
        {
            _plotSelector.CurrentPlot.Render(Owner.Results, graphics, PlotBounds);
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