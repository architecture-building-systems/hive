using System;
using System.Collections.Generic;
using System.Drawing;
using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using Hive.IO.Results;

namespace Hive.IO.Plots
{
    public delegate MenuButtonPanel PanelFactory();

    public enum PerformanceResolution
    {
        Lifetime,
        Yearly,
        Monthly,
        //Hourly
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
        //GvL,
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
            _panelFactory = CreatePerformancePanel;
            _panelFactory = CreateSystemsPanel;

            _currentPanel = _panelFactory();
        }

        public bool Normalized
        {
            get => CurrentKpi != Kpi.None && _normalized;
            set => _normalized = value;
        }

        public Kpi CurrentKpi { get; private set; } = Kpi.Energy;
        public IVisualizerPlot _currentPlot;


        private string Category => _currentPanel.Category;


        public MenuButtonPanel CreatePerformancePanel()
        {
            var mbCategory = new CategoryMenuButton("P");
            mbCategory.OnClicked += (sender, e) =>
            {
                _panelFactory = CreateSystemsPanel;
                //CurrentKpi = Kpi.None;
            };
            //mbCategory.OnClicked += (sender, args) => RhinoApp.Write("P was clicked!");

            MenuButton mbResolution;
            switch (_performanceResolution)
            {
                case PerformanceResolution.Lifetime:
                    mbResolution = new BlackMenuButton("TOT");
                    break;
                case PerformanceResolution.Monthly:
                    mbResolution = new BlackMenuButton("M");
                    break;
                //case PerformanceResolution.Hourly:
                //    mbResolution = new BlackMenuButton("D");
                //    break;
                default:
                    mbResolution = new BlackMenuButton("Y");
                    break;
            }

            mbResolution.OnClicked += CycleResolution;

            var mbNormalize = Normalized ? new BlackMenuButton("/m²") : new MenuButton("/m²");
            mbNormalize.OnClicked += (sender, e) => Normalized = !Normalized;

            // TODO implement breakdown
            //var mbBreakdown = _breakdown ? new BlackMenuButton("BRK") : new MenuButton("BRK");
            //mbBreakdown.OnClicked += (sender, e) => _breakdown = !_breakdown;

            return new MenuButtonPanel(new[]
            {
                mbCategory,
                mbNormalize,
                mbResolution,
                //mbBreakdown
            });
        }

        private MenuButtonPanel CreateSystemsPanel()
        {
            var mbCategory = new CategoryMenuButton("S");
            mbCategory.OnClicked += (sender, e) =>
            {
                _panelFactory = CreateOtherPanel; // link to next panel in list
            };

            var mbNormalize = Normalized ? new BlackMenuButton("/m²") : new MenuButton("/m²");
            mbNormalize.OnClicked += (sender, e) => Normalized = !Normalized;

            return new MenuButtonPanel(new[]
            {
                mbCategory,
                mbNormalize,
                // new MenuButton("SIZ"),
                // new MenuButton("CHA")
            });
        }

        private MenuButtonPanel CreateOtherPanel()
        {
            var mbCategory = new CategoryMenuButton("O");
            mbCategory.OnClicked += (sender, e) =>
            {
                _panelFactory = CreatePerformancePanel; // link to next panel in list
                CurrentKpi = Kpi.Energy;
            };

            var mbNormalize = Normalized ? new BlackMenuButton("/m²") : new MenuButton("/m²");
            mbNormalize.OnClicked += (sender, e) => Normalized = !Normalized;

            var mbSvD = _otherSubcategory == OtherSubcategory.SvD ? new BlackMenuButton("SvD") : new MenuButton("SvD");
            //var mbGvL = _otherSubcategory == OtherSubcategory.GvL ? new BlackMenuButton("GvL") : new MenuButton("GvL");
            var mbSol = _otherSubcategory == OtherSubcategory.Sol ? new BlackMenuButton("SOL") : new MenuButton("SOL");

            mbSvD.OnClicked += (sender, args) => _otherSubcategory = OtherSubcategory.SvD;
            //mbGvL.OnClicked += (sender, args) => _otherSubcategory = OtherSubcategory.GvL;
            mbSol.OnClicked += (sender, args) => _otherSubcategory = OtherSubcategory.Sol;

            return new MenuButtonPanel(new[]
            {
                mbCategory,
                mbNormalize,
                mbSvD,
                //mbGvL,
                mbSol
            });
        }


        /// <summary>
        ///     This is where the plot selection logic comes from. Let's just brute-force it.
        /// </summary>
        private IVisualizerPlot SelectCurrentPlot(ResultsPlotting results)
        {
            if (Category == "P") // Performance
            {
                if (_performanceResolution == PerformanceResolution.Lifetime)
                    return LifetimePerformancePlot(CurrentKpi, results, _normalized, _breakdown);
                if (_performanceResolution == PerformanceResolution.Yearly)
                    return YearlyPerformancePlot(CurrentKpi, results, _normalized, _breakdown);
                if (_performanceResolution == PerformanceResolution.Monthly)
                    return MonthlyPerformancePlot(CurrentKpi, results, _normalized, _breakdown);
                return new AmrPlotBase("TODO: Implement this plot!", "",
                    new EnergyDataAdaptor(results, _normalized),
                    new EnergyPlotStyle(), true);
            }
            else if (Category == "O") // Other
            {
                switch (_otherSubcategory)
                {
                    case OtherSubcategory.SvD:
                        if (_normalized)
                            return new EnergyBalanceNormalizedPlot();
                        else
                            return new EnergyBalancePlot();
                    //case OtherSubcategory.GvL:
                    //    return new EnergyBalancePlot();
                    case OtherSubcategory.Sol:
                        if (_normalized)
                            return new SolarGainsPerWindowNormalizedPlot();
                        else
                            return new SolarGainsPerWindowPlot();
                    default:
                        return new EnergyBalancePlot();
                }
            }
            else // Monthly Energy Demand
            {
                if (_normalized) 
                    return new DemandMonthlyNormalizedPlot();
                else 
                    return new DemandMonthlyPlot();
            }
        }

        private IVisualizerPlot LifetimePerformancePlot(Kpi currentKpi, ResultsPlotting results, bool normalized,
            bool breakdown)
        {
            IVisualizerPlot plot;
            switch (currentKpi)
            {
                case Kpi.Energy:
                    plot = new LifetimeAmrPlot(AmrPlotConstants.EnergyTitle, AmrPlotConstants.EnergyDescription,
                        new EnergyDataAdaptor(results, normalized), new EnergyPlotStyle(), displaySumsAndAverages: false);
                    break;
                case Kpi.Emissions:
                    plot = new LifetimeAmrPlot(AmrPlotConstants.EmissionsTitle, AmrPlotConstants.EmissionsDescription, 
                        new EmissionsDataAdaptor(results, normalized), new EmissionsPlotStyle());
                    break;
                case Kpi.Costs:
                    plot = new LifetimeAmrPlot(AmrPlotConstants.CostTitle, AmrPlotConstants.CostDescription, 
                        new CostsDataAdaptor(results, normalized), new CostsPlotStyle());
                    break;
                default:
                    // this shouldn't happen...
                    plot = new AmrPlotBase(
                        "TODO: Implement this plot!", "",
                        new EnergyDataAdaptor(results, normalized),
                        new EnergyPlotStyle(), true);
                    break;
            }

            return plot;
        }
        
        private IVisualizerPlot YearlyPerformancePlot(Kpi currentKpi, ResultsPlotting results, bool normalized,
            bool breakdown)
        {
            IVisualizerPlot plot;
            switch (currentKpi)
            {
                case Kpi.Energy:
                    plot = new YearlyAmrPlot(AmrPlotConstants.EnergyTitle, AmrPlotConstants.EnergyDescription,
                        new EnergyDataAdaptor(results, normalized), new EnergyPlotStyle(), displaySumsAndAverages: false);
                    break;
                case Kpi.Emissions:
                    plot = new YearlyAmrPlot(AmrPlotConstants.EmissionsTitle, AmrPlotConstants.EmissionsDescription, 
                        new EmissionsDataAdaptor(results, normalized), new EmissionsPlotStyle());
                    break;
                case Kpi.Costs:
                    plot = new YearlyAmrPlot(AmrPlotConstants.CostTitle, AmrPlotConstants.CostDescription, 
                        new CostsDataAdaptor(results, normalized), new CostsPlotStyle());
                    break;
                default:
                    // this shouldn't happen...
                    plot = new AmrPlotBase("TODO: Implement this plot!", "",
                        new EnergyDataAdaptor(results, normalized), new EnergyPlotStyle(), true);
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
                    plot = new MonthlyAmrPlot(AmrPlotConstants.EnergyTitle, AmrPlotConstants.EnergyDescription,
                        new EnergyDataAdaptor(results, normalized), new EnergyPlotStyle(), displaySumsAndAverages: false);
                    break;
                case Kpi.Emissions:
                    plot = new MonthlyAmrPlot(AmrPlotConstants.EmissionsTitle, AmrPlotConstants.EmissionsDescription,
                        new EmissionsDataAdaptor(results, normalized), new EmissionsPlotStyle());
                    break;
                case Kpi.Costs:
                    plot = new MonthlyAmrPlot(AmrPlotConstants.CostTitle, AmrPlotConstants.CostDescription, 
                        new CostsDataAdaptor(results, normalized), new CostsPlotStyle());
                    break;
                default:
                    // this shouldn't happen...
                    plot = new AmrPlotBase("TODO: Implement this plot!", "",
                        new EnergyDataAdaptor(results, normalized), new EnergyPlotStyle(), true);
                    break;
            }

            return plot;
        }

        public bool Contains(PointF location, bool usePlotBounds = false)
        {
            return usePlotBounds ? _currentPlot.Contains(location) : _currentPanel.Contains(location);
        }

        public void Clicked(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            _currentPanel.Clicked(sender, e);
            _currentPanel = _panelFactory();
        }

        public void RenderMenuPanel(ResultsPlotting results, Graphics graphics, RectangleF bounds)
        {
            _currentPanel.Render(results, null, graphics, bounds);
        }

        public void RenderCurrentPlot(ResultsPlotting results, Dictionary<string, string> plotProperties, Graphics graphics, RectangleF bounds)
        {
            _currentPlot = SelectCurrentPlot(results);
            _currentPlot.Render(results, plotProperties, graphics, bounds);
        }

        private void CycleResolution(object sender, EventArgs e)
        {
            PerformanceResolution nextResolution;
            switch (_performanceResolution)
            {
                case PerformanceResolution.Lifetime:
                    nextResolution = PerformanceResolution.Yearly;
                    break;
                case PerformanceResolution.Yearly:
                    nextResolution = PerformanceResolution.Monthly;
                    break;
                case PerformanceResolution.Monthly:
                //    nextResolution = PerformanceResolution.Hourly;
                //    break;
                //case PerformanceResolution.Hourly:
                    nextResolution = PerformanceResolution.Lifetime;
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