using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using Hive.IO.DataHandling;

namespace Hive.IO.Plots
{
    public class EnergyBalancePlot: IVisualizerPlot
    {
        private RectangleF _bounds;

        public void Render(ResultsPlotting results, Graphics graphics, RectangleF bounds)
        {
            _bounds = bounds;
            RenderHouse(graphics, bounds);
        }

        private void RenderHouse(Graphics graphics, RectangleF bounds)
        {
            var xLeft = bounds.Left;
            var xRight = bounds.Right;
            var xMiddle = xLeft + bounds.Width / 2;
            var yBottom = bounds.Y + bounds.Height;
            var yTop = bounds.Top;
            var yRoof = bounds.Y + bounds.Height * 0.66f;
            var house = new PointF[]
            {
                // start at bottom left, clockwise
                new PointF(xLeft, yBottom), 
                new PointF(xLeft, yRoof),
                new PointF(xMiddle, yTop),
                new PointF(xRight, yRoof),
                new PointF(xRight, yBottom),
            };
            graphics.FillPolygon(new HatchBrush(HatchStyle.DarkHorizontal, Color.Aqua), house);
        }

        public bool Contains(PointF location)
        {
            return _bounds.Contains(location);
        }

        public void Clicked(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            throw new NotImplementedException();
        }

        public void NewData(ResultsPlotting results)
        {
            // ignore for now
        }
    }
}
