using System;
using System.Drawing;
using System.Windows.Forms;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using OxyPlot;
using Rhino;
using LineJoin = System.Drawing.Drawing2D.LineJoin;

namespace Hive.IO.Plots
{
    public class OperationalPerformancePlot: IVisualizerPlot
    {
        private readonly float _totalHeight;
        private readonly Font _standardFont;
        private readonly Font _boldFont;
        private readonly float _padding;
        
        /// <summary>
        /// Draw a square box with data inside and a unit string above.
        /// </summary>
        /// <param name="totalHeight">the total height available including unit string</param>
        public OperationalPerformancePlot(float totalHeight)
        {
            _totalHeight = totalHeight;
            _standardFont = GH_FontServer.Standard;
            _boldFont = GH_FontServer.StandardBold;
            _padding = 6f;
        }

        public float Width => _totalHeight - _standardFont.Height - _padding;

        /// <summary>
        /// Draw a box with some text
        ///
        /// The height of the box is calculated by having the unit text height
        /// subtracted from the bounds, with a small padding.
        /// </summary>
        /// <param name="results"></param>
        /// <param name="graphics"></param>
        /// <param name="bounds">expected height of bounds should be TotalHeight</param>
        public void Render(Results results, Graphics graphics, RectangleF bounds)
        {
            var boxHeight = Width;
            var boxWidth = Width; // squares
            var boxTop = bounds.Bottom - boxHeight;

            // DUMMY Values for the data:
            var data = 1540.ToString();
            var unit = "kWh";
            
            // draw box
            var color = Color.Aqua;
            var pen = new Pen(color, 1f) { LineJoin = LineJoin.Round };
            graphics.DrawRectangle(pen, bounds.Left, boxTop, boxWidth, boxHeight);

            // center unit above box
            var brush = new SolidBrush(Color.Black);
            var unitStart = bounds.Left + (boxWidth - GH_FontServer.StringWidth(unit, _standardFont)) / 2;
            graphics.DrawString(unit, _standardFont, brush, unitStart, bounds.Top);
            RhinoApp.WriteLine($"Rendering {unit} to {unitStart}, {bounds.Top}");

            // center data in the box
            var dataSize = GH_FontServer.MeasureString(data, _boldFont);
            var dataStart = bounds.Left + (boxWidth - dataSize.Width) / 2;
            var dataTop = boxTop + boxHeight / 2 - (float)dataSize.Height / 2;
            graphics.DrawString(data, _boldFont, brush, dataStart, dataTop);
            RhinoApp.WriteLine($"Rendering {data} to {dataStart}, {dataTop}");

            // draw a dummy rectangle of the bounds
            pen = new Pen(Color.Red, 1f);
            graphics.DrawRectangle(pen, bounds.Left, bounds.Top, bounds.Width, bounds.Height);
        }

        public void NewData(Results results)
        {
            // for the moment, we'll not do any caching...
        }
    }
}