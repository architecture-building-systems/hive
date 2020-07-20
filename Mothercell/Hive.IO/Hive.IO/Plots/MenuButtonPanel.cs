using System.Drawing;
using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using Rhino;

namespace Hive.IO.Plots
{
    /// <summary>
    /// A MenuButtonPanel contains a list of MenuButtons.
    /// </summary>
    public class MenuButtonPanel : IVisualizerControl
    {
        private const float sideLength = 100;
        private const float spacer = 50; // space between menu buttons
        private MenuButton[] _menuButtons;

        public MenuButtonPanel(MenuButton[] menuButtons)
        {
            _menuButtons = menuButtons;
        }

        public void Render(Results results, Graphics graphics, RectangleF bounds)
        {
            var x = bounds.X;
            var y = bounds.Y + bounds.Height / 2 - sideLength / 2;

            foreach (var mb in _menuButtons)
            {
                mb.Render(results, graphics, new RectangleF(x, y, sideLength, sideLength));
                x += sideLength + spacer;
            }
        }

        public bool Contains(PointF location)
        {
            return false;
        }

        public void Clicked(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            // figure out which menu button got clicked and pass it on
            RhinoApp.WriteLine("MenuButtonPanel.Clicked()");
        }
    }
}