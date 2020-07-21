using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Channels;
using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
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


        // starting panels (cycle through these when clicking on first MenuButton)
        public MenuButtonPanel CreatePerformancePanel()
        {
            var mbCategory = new MenuButton("P");
            mbCategory.OnClicked += (sender, e) => _panelFactory = CreateSystemsPanel;
            mbCategory.OnClicked += (sender, args) => RhinoApp.Write("P was clicked!");

            var mbResolution = new MenuButton(_performanceResolution.ToString().Substring(0, 1));
            mbResolution.OnClicked += CycleResolution;

            var mbNormalize = _normalized ? new MenuButton("/m²") : new MenuButton("TOT");
            mbNormalize.OnClicked += (sender, e) => _normalized = !_normalized;

            var mbBreakdown = _breakdown ? new MenuButton("+BRK") : new MenuButton("-BRK");
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
            var mbCategory = new MenuButton("S");
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
            var mbCategory = new MenuButton("O");
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
