using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using Rhino;

namespace Hive.IO.Plots
{
    /// <summary>
    /// MenuButtons are clickable regions that can render themselves to to a canvas.
    /// They are managed by the PlotSelector class.
    /// </summary>
    public class MenuButton: IVisualizerControl
    {
        public event EventHandler OnClicked;

        private Pen _borderPen = new Pen(Color.FromArgb(217, 217, 217));
        private string _text;
        private Font _font;
        private Brush _textBrush = new SolidBrush(Color.Black);
        private RectangleF _bounds = RectangleF.Empty;

        public MenuButton(string text, EventHandler onClicked): this(text, GH_FontServer.Standard, onClicked)
        {
        }

        public MenuButton(string text, Font font, EventHandler onClicked)
        {
            _text = text;
            _font = font;
            OnClicked += onClicked;
        }

        public void Render(Results results, Graphics graphics, RectangleF bounds)
        {
            _bounds = bounds;

            var format = StringFormat.GenericTypographic;
            format.Alignment = StringAlignment.Center;
            format.LineAlignment = StringAlignment.Center;

            graphics.DrawString(_text, _font, _textBrush, bounds, format);
            graphics.DrawRectangleF(_borderPen, bounds);
        }

        public bool Contains(PointF location)
        {
            return _bounds.Contains(location);
        }

        public void Clicked(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            RhinoApp.WriteLine($"MenuButton({_text}) clicked!");
            if (OnClicked != null)
            {
                OnClicked(this, e);
            }
        }
    }
}
