using System;
using System.Drawing;
using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using Hive.IO.Results;

namespace Hive.IO.Plots
{
    public delegate MenuButtonPanel PanelFactory();

    public enum PerformanceResolution
    {
        Yearly,
        Monthly,
        Hourly
    }

    public enum Kpi
    {
        Energy,
        Emissions,
        Costs,
        None
    } // None is used for when we're not showing performance plots

    public enum OtherSubcategory
    {
        SvD,
        GvL,
        Sol
    }

    /// <summary>
    ///     Manages the display of plots based on clicks to MenuButton objects. Also manages which MenuButtons
    ///     are currently shown.
    /// </summary>
    public class PlotSelector
    {
        private bool _breakdown;
        private MenuButtonPanel _currentPanel;
        private bool _normalized;
        private OtherSubcategory _otherSubcategory = OtherSubcategory.SvD;

        private PanelFactory _panelFactory;

        // Performance panel state
        private PerformanceResolution _performanceResolution = PerformanceResolution.Yearly;

        public PlotSelector()
        {
            // FIXME: @chris, this needs to be changed back again when we add the amr plots!
            // _panelFactory = CreatePerformancePanel;
            _panelFactory = CreateSystemsPanel;

            _currentPanel = _panelFactory();
        }

        public bool Normalized
        {
            get => CurrentKpi != Kpi.None && _normalized;
            set => _normalized = value;
        }

        public Kpi CurrentKpi { get; private set; } = Kpi.None; // FIXME: use this for amr plots:  Kpi.Energy;

        private string Category => _currentPanel.Category;


        public MenuButtonPanel CreatePerformancePanel()
        {
            var mbCategory = new CategoryMenuButton("P");
            mbCategory.OnClicked += (sender, e) =>
            {
                _panelFactory = CreateSystemsPanel;
                CurrentKpi = Kpi.None;
            };
            // mbCategory.OnClicked += (sender, args) => RhinoApp.Write("P was clicked!");

            MenuButton mbResolution;
            switch (_performanceResolution)
            {
                case PerformanceResolution.Monthly:
                    mbResolution = new BlackMenuButton("M");
                    break;
                case PerformanceResolution.Hourly:
                    mbResolution = new BlackMenuButton("D");
                    break;
                default:
                    mbResolution = new MenuButton("Y");
                    break;
            }

            mbResolution.OnClicked += CycleResolution;

            var mbNormalize = Normalized ? new BlackMenuButton("/m²") : new MenuButton("TOT");
            mbNormalize.OnClicked += (sender, e) => Normalized = !Normalized;

            var mbBreakdown = _breakdown ? new BlackMenuButton("BRK") : new MenuButton("BRK");
            mbBreakdown.OnClicked += (sender, e) => _breakdown = !_breakdown;

            return new MenuButtonPanel(new[]
            {
                mbCategory,
                mbResolution,
                mbNormalize,
                mbBreakdown
            });
        }

        private MenuButtonPanel CreateSystemsPanel()
        {
            var mbCategory = new CategoryMenuButton("S");
            mbCategory.OnClicked += (sender, e) =>
            {
                _panelFactory = CreateOtherPanel; // link to next panel in list
            };

            return new MenuButtonPanel(new[]
            {
                mbCategory,
                // new MenuButton("SIZ"),
                // new MenuButton("CHA")
            });
        }

        private MenuButtonPanel CreateOtherPanel()
        {
            var mbCategory = new CategoryMenuButton("O");
            mbCategory.OnClicked += (sender, e) =>
            {
                // _panelFactory = CreatePerformancePanel; // FIXME @chris change this back when ready for amr plots
                _panelFactory = CreateSystemsPanel; // link to next panel in list
                // CurrentKpi = Kpi.Energy;  // FIXME @chris uncomment this to add amr plots back
            };

            var mbSvD = _otherSubcategory == OtherSubcategory.SvD ? new BlackMenuButton("SvD") : new MenuButton("SvD");
            // var mbGvL = _otherSubcategory == OtherSubcategory.GvL ? new BlackMenuButton("GvL") : new MenuButton("GvL");
            ;
            var mbSol = _otherSubcategory == OtherSubcategory.Sol ? new BlackMenuButton("SOL") : new MenuButton("SOL");
            ;

            mbSvD.OnClicked += (sender, args) => _otherSubcategory = OtherSubcategory.SvD;
            // mbGvL.OnClicked += (sender, args) => _otherSubcategory = OtherSubcategory.GvL;
            mbSol.OnClicked += (sender, args) => _otherSubcategory = OtherSubcategory.Sol;

            return new MenuButtonPanel(new[]
            {
                mbCategory,
                mbSvD,
                // mbGvL,
                mbSol
            });
        }


        /// <summary>
        ///     This is where the plot selection logic comes from. Let's just brute-force it.
        /// </summary>
        private IVisualizerPlot SelectCurrentPlot(ResultsPlotting results)
        {
            if (Category == "P")
            {
                if (_performanceResolution == PerformanceResolution.Yearly)
                    return YearlyPerformancePlot(CurrentKpi, results, _normalized, _breakdown);
                if (_performanceResolution == PerformanceResolution.Monthly)
                    return MonthlyPerformancePlot(CurrentKpi, results, _normalized, _breakdown);
                return new AmrPlotBase(
                    "TODO: Implement this plot!",
                    new EnergyDataAdaptor(results, _normalized),
                    new EnergyPlotStyle());
            }

            if (Category == "O")
                switch (_otherSubcategory)
                {
                    case OtherSubcategory.SvD:
                        return new EnergyBalancePlot();
                    case OtherSubcategory.GvL:
                        return new EnergyBalancePlot();
                    case OtherSubcategory.Sol:
                        return new IrradiationOnWindowsPlot();
                    default:
                        return new EnergyBalancePlot();
                }

            return new DemandMonthlyPlot();
        }

        private IVisualizerPlot YearlyPerformancePlot(Kpi currentKpi, ResultsPlotting results, bool normalized,
            bool breakdown)
        {
            IVisualizerPlot plot;
            switch (currentKpi)
            {
                case Kpi.Energy:
                    plot = new YearlyAmrPlot("Energy", new EnergyDataAdaptor(results, normalized),
                        new EnergyPlotStyle());
                    break;
                case Kpi.Emissions:
                    plot = new YearlyAmrPlot("CO₂ Emissions", new EmissionsDataAdaptor(results, normalized),
                        new EmissionsPlotStyle());
                    break;
                case Kpi.Costs:
                    plot = new YearlyAmrPlot("Cost", new CostsDataAdaptor(results, normalized), new CostsPlotStyle());
                    break;
                default:
                    // this shouldn't happen...
                    plot = new AmrPlotBase(
                        "TODO: Implement this plot!",
                        new EnergyDataAdaptor(results, normalized),
                        new EnergyPlotStyle());
                    break;
            }

            return plot;
        }

        private IVisualizerPlot MonthlyPerformancePlot(Kpi currentKpi, ResultsPlotting results, bool normalized,
            bool breakdown)
        {
            IVisualizerPlot plot;
            switch (currentKpi)
            {
                case Kpi.Energy:
                    plot = new MonthlyAmrPlot("Energy", new EnergyDataAdaptor(results, normalized),
                        new EnergyPlotStyle());
                    break;
                case Kpi.Emissions:
                    plot = new MonthlyAmrPlot("CO₂ Emissions", new EmissionsDataAdaptor(results, normalized),
                        new EmissionsPlotStyle());
                    break;
                case Kpi.Costs:
                    plot = new MonthlyAmrPlot("Cost", new CostsDataAdaptor(results, normalized), new CostsPlotStyle());
                    break;
                default:
                    // this shouldn't happen...
                    plot = new AmrPlotBase(
                        "TODO: Implement this plot!",
                        new EnergyDataAdaptor(results, normalized),
                        new EnergyPlotStyle());
                    break;
            }

            return plot;
        }

        public bool Contains(PointF location)
        {
            return _currentPanel.Contains(location);
        }

        public void Clicked(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            _currentPanel.Clicked(sender, e);
            _currentPanel = _panelFactory();
        }

        public void RenderMenuPanel(ResultsPlotting results, Graphics graphics, RectangleF bounds)
        {
            _currentPanel.Render(results, graphics, bounds);
        }

        public void RenderCurrentPlot(ResultsPlotting results, Graphics graphics, RectangleF bounds)
        {
            var plot = SelectCurrentPlot(results);
            plot.Render(results, graphics, bounds);
        }

        private void CycleResolution(object sender, EventArgs e)
        {
            PerformanceResolution nextResolution;
            switch (_performanceResolution)
            {
                case PerformanceResolution.Yearly:
                    nextResolution = PerformanceResolution.Monthly;
                    break;
                case PerformanceResolution.Monthly:
                    nextResolution = PerformanceResolution.Hourly;
                    break;
                case PerformanceResolution.Hourly:
                    nextResolution = PerformanceResolution.Yearly;
                    break;
                default:
                    nextResolution = PerformanceResolution.Yearly;
                    break;
            }

            _performanceResolution = nextResolution;
        }

        public void CostsKpiClicked(object sender, EventArgs e)
        {
            if (CurrentKpi != Kpi.None) CurrentKpi = Kpi.Costs;
        }

        public void EmissionsKpiClicked(object sender, EventArgs e)
        {
            if (CurrentKpi != Kpi.None) CurrentKpi = Kpi.Emissions;
        }

        public void EnergyKpiClicked(object sender, EventArgs e)
        {
            if (CurrentKpi != Kpi.None) CurrentKpi = Kpi.Energy;
        }
    }
}