using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.Remoting.Channels;

namespace Hive.IO.Plots
{
    /// <summary>
    /// Manages the display of plots based on clicks to MenuButton objects. Also manages which MenuButtons
    /// are currently shown.
    /// </summary>
    public class PlotSelector
    {
        // starting panels (cycle through these when clicking on first MenuButton)
        public MenuButtonPanel PerformancePanel
        {
            get
            {
                return new MenuButtonPanel(new MenuButton[]
                {
                    new MenuButton("P", (sender, e) => Transition(SystemsPanel, new AmrPlotBase())),
                    new MenuButton("Y", (sender, e) => Transition(PerformancePanel, new AmrPlotBase())),
                    new MenuButton("TOT", (sender, e) => Transition(PerformancePanel, new AmrPlotBase())),
                    new MenuButton("BRK", (sender, e) => Transition(PerformancePanel, new AmrPlotBase())),
                });
            }
        }

        private MenuButtonPanel SystemsPanel
        {
            get
            {
                return new MenuButtonPanel(new MenuButton[]
                {
                    new MenuButton("S", (sender, e) => Transition(OtherPanel, new AmrPlotBase())),
                    new MenuButton("SIZ", (sender, e) => Transition(SystemsPanel, new AmrPlotBase())),
                    new MenuButton("CHA", (sender, e) => Transition(SystemsPanel, new AmrPlotBase())),
                });
            }
        }

        private MenuButtonPanel OtherPanel
        {
            get
            {
                return new MenuButtonPanel(new MenuButton[]
                {
                    new MenuButton("O", (sender, e) => Transition(PerformancePanel, new DemandMonthlyPlot())),
                    new MenuButton("SvD", (sender, e) => Transition(SystemsPanel, new AmrPlotBase())),
                    new MenuButton("GvL", (sender, e) => Transition(SystemsPanel, new AmrPlotBase())),
                    new MenuButton("SAN", (sender, e) => Transition(SystemsPanel, new AmrPlotBase())),
                });
            }
        }

        public MenuButtonPanel CurrentPanel { get; private set; }
        public IVisualizerPlot CurrentPlot { get; private set; }

        public PlotSelector()
        {
            CurrentPanel = PerformancePanel;
            CurrentPlot = new AmrPlotBase();
        }

        protected void Transition(MenuButtonPanel nextPanel, IVisualizerPlot nextPlot)
        {
            CurrentPanel = nextPanel;
            CurrentPlot = nextPlot;
        }
    }
}
