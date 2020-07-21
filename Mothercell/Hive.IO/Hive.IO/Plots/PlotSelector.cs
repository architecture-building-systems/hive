using System;
using System.Drawing;
using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using Rhino;

namespace Hive.IO.Plots
{
    public delegate MenuButtonPanel PanelFactory();

    public enum PerformanceResolution { Yearly, Monthly, Hourly }
    public enum Kpi { Energy, Emissions, Costs, None } // None is used for when we're not showing performance plots


    /// <summary>
    /// Manages the display of plots based on clicks to MenuButton objects. Also manages which MenuButtons
    /// are currently shown.
    /// </summary>
    public class PlotSelector
    {
        // Performance panel state
        private PerformanceResolution _performanceResolution = PerformanceResolution.Yearly;
        private Kpi _currentKpi = Kpi.Energy;
        private bool _normalized;
        private bool _breakdown;
        private PanelFactory _panelFactory;
        private MenuButtonPanel _currentPanel;


        public MenuButtonPanel CreatePerformancePanel()
        {
            var mbCategory = new CategoryMenuButton("P");
            mbCategory.OnClicked += (sender, e) =>
            {
                _panelFactory = CreateSystemsPanel;
                _currentKpi = Kpi.None;
            };
            mbCategory.OnClicked += (sender, args) => RhinoApp.Write("P was clicked!");

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
                mbBreakdown,
            });
        }

        private MenuButtonPanel CreateSystemsPanel()
        {
            var mbCategory = new CategoryMenuButton("S");
            mbCategory.OnClicked += (sender, e) =>
            {
                _panelFactory = CreateOtherPanel;
            };

            return new MenuButtonPanel(new[]
            {
                mbCategory,
                new MenuButton("SIZ"),
                new MenuButton("CHA"),
            });
        }

        private MenuButtonPanel CreateOtherPanel()
        {
            var mbCategory = new CategoryMenuButton("O");
            mbCategory.OnClicked += (sender, e) =>
            {
                _panelFactory = CreatePerformancePanel;
                _currentKpi = Kpi.Energy;
            };

            return new MenuButtonPanel(new[]
            {
                mbCategory,
                new MenuButton("SvD"),
                new MenuButton("GvL"),
                new MenuButton("SAN"),
            });
        }

        public IVisualizerPlot CurrentPlot { get; }

        public bool Normalized
        {
            get => CurrentKpi != Kpi.None && _normalized;
            set => _normalized = value;
        }

        public Kpi CurrentKpi => _currentKpi;

        public PlotSelector()
        {
            _panelFactory = CreatePerformancePanel;
            _currentPanel = _panelFactory();
            CurrentPlot = new AmrPlotBase();
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

        public void Render(Results results, Graphics graphics, RectangleF bounds)
        {
            _currentPanel.Render(results, graphics, bounds);
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
            if (CurrentKpi != Kpi.None)
            {
                _currentKpi = Kpi.Costs;

            }
        }

        public void EmissionsKpiClicked(object sender, EventArgs e)
        {

            if (CurrentKpi != Kpi.None)
            {
                _currentKpi = Kpi.Emissions;

            }
        }

        public void EnergyKpiClicked(object sender, EventArgs e)
        {
            if (CurrentKpi != Kpi.None)
            {
                _currentKpi = Kpi.Energy;

            }
        }
    }
}
