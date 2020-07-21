using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Xml;
using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using Rhino;

namespace Hive.IO.Plots
{
    public delegate MenuButtonPanel PanelFactory();

    public enum PerformanceResolution { Yearly, Monthly, Hourly }
    public enum KPI { Energy, Emissions, Costs }


    /// <summary>
    /// Manages the display of plots based on clicks to MenuButton objects. Also manages which MenuButtons
    /// are currently shown.
    /// </summary>
    public class PlotSelector
    {
        // Performance panel state
        private PerformanceResolution _performanceResolution = PerformanceResolution.Yearly;
        private KPI _currentKPI = KPI.Energy;
        private bool _normalized = false;
        private bool _breakdown = false;
        private PanelFactory _panelFactory;
        private MenuButtonPanel _currentPanel;

        private readonly SolidBrush _blackBrush = new SolidBrush(Color.Black);
        private readonly SolidBrush _greyBrush = new SolidBrush(Color.FromArgb(217, 217, 217));


        public MenuButtonPanel CreatePerformancePanel()
        {
            var mbCategory = new CategoryMenuButton("P");
            mbCategory.OnClicked += (sender, e) => _panelFactory = CreateSystemsPanel;
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
            };
            mbResolution.OnClicked += CycleResolution;

            var mbNormalize = _normalized ? new BlackMenuButton("/m²") : new MenuButton("TOT"); 
            mbNormalize.OnClicked += (sender, e) => _normalized = !_normalized;

            var mbBreakdown = _breakdown ? new BlackMenuButton("BRK") : new MenuButton("BRK");
            mbBreakdown.OnClicked += (sender, e) => _breakdown = !_breakdown;

            return new MenuButtonPanel(new MenuButton[]
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
            mbCategory.OnClicked += (sender, e) => _panelFactory = CreateOtherPanel;

            return new MenuButtonPanel(new MenuButton[]
            {
                mbCategory,
                new MenuButton("SIZ"),
                new MenuButton("CHA"),
            });
        }

        private MenuButtonPanel CreateOtherPanel()
        {
            var mbCategory = new CategoryMenuButton("O");
            mbCategory.OnClicked += (sender, e) => _panelFactory = CreatePerformancePanel;

            return new MenuButtonPanel(new MenuButton[]
            {
                mbCategory,
                new MenuButton("SvD"),
                new MenuButton("GvL"),
                new MenuButton("SAN"),
            });
        }

        public IVisualizerPlot CurrentPlot { get; }

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
    }
}
