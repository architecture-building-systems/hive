using System.Collections.Generic;
using System.Drawing;

namespace Hive.IO.Plots
{
    /// <summary>
    /// Manages the display of plots based on clicks to MenuButton objects. Also manages which MenuButtons
    /// are currently shown.
    /// </summary>
    public class PlotSelector
    {
        private MenuButtonPanel _performancePanel;
        private MenuButtonPanel _systemsPanel;
        private MenuButtonPanel _otherPanel;

        public MenuButtonPanel CurrentPanel { get; private set; }

        public PlotSelector()
        {
            _performancePanel = new MenuButtonPanel(new MenuButton[]
            {
                new MenuButton("P"),
                new MenuButton("Y"),
                new MenuButton("M"), 
                new MenuButton("TOT"), 
                new MenuButton("/m2"), 
                new MenuButton("BRK"),
            });
            CurrentPanel = _performancePanel;
        }
    }
}
