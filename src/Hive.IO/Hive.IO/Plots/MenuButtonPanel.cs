using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using Hive.IO.Results;

namespace Hive.IO.Plots
{
    /// <summary>
    ///     A MenuButtonPanel contains a list of MenuButtons.
    /// </summary>
    public class MenuButtonPanel : IVisualizerControl
    {
        public const float Spacer = 25; // space between menu buttons
        private readonly MenuButton[] _menuButtons;

        public MenuButtonPanel(MenuButton[] menuButtons)
        {
            _menuButtons = menuButtons;
        }

        public static float SideLength => 3 * GH_FontServer.MeasureString("ABC", GH_FontServer.StandardBold).Height;

        public string Category => _menuButtons.First().Text.Substring(0, 1);

        public void Render(ResultsPlotting results, Dictionary<string, string> plotParameters, Graphics graphics,
            RectangleF bounds)
        {
            var x = bounds.X + Spacer;
            var y = bounds.Y + bounds.Height / 2 - SideLength / 2;

            foreach (var mb in _menuButtons)
            {
                mb.Render(results, plotParameters, graphics, new RectangleF(x, y, SideLength, SideLength));
                x += SideLength + Spacer;
            }
        }

        public bool Contains(PointF location)
        {
            return _menuButtons.Any(mb => mb.Contains(location));
        }

        public void Clicked(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            // figure out which menu button got clicked and pass it on
            _menuButtons.First(mb => mb.Contains(e.CanvasLocation)).Clicked(sender, e);
        }

        /// <summary>
        ///     Return a new MenuButtonPanel, with a (single) menu button replaced with a new one.
        /// </summary>
        /// <param name="select">the text of the menu button to replace</param>
        /// <param name="replacement"></param>
        /// <returns></returns>
        public MenuButtonPanel Replace(string select, MenuButton replacement)
        {
            var menuButtons = _menuButtons.Select(mb => mb.Text == select ? replacement : mb);
            return new MenuButtonPanel(menuButtons.ToArray());
        }
    }
}