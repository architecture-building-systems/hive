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
        /// <summary>
        /// Draw a box with some text
        ///
        /// The height of the box is calculated by having the unit text height
        /// subtracted from the bounds, with a small padding.
        /// </summary>
        /// <param name="results"></param>
        /// <param name="graphics"></param>
        /// <param name="bounds"></param>
        public void Render(Results results, Graphics graphics, RectangleF bounds)
        {
            var standardFont = GH_FontServer.Standard;  // text above the box
            var boldFont = GH_FontServer.StandardBold;  // text below the box

            var padding = 6f;

            var boxHeight = bounds.Height - standardFont.Height - padding;
            var boxWidth = boxHeight; // squares
            var boxTop = bounds.Top + standardFont.Height + padding;

            // DUMMY Values for the data:
            var data = 1540.ToString();
            var unit = "kWh";
            
            var color = Color.Aqua;
            var pen = new Pen(color, 1f) { LineJoin = LineJoin.Round };
            graphics.DrawRectangle(pen, bounds.Left, boxTop, boxWidth, boxHeight);

            var brush = new SolidBrush(Color.Black);
            var unitStart = bounds.Left + (boxWidth - GH_FontServer.StringWidth(unit, standardFont)) / 2;
            graphics.DrawString(unit, standardFont, brush, unitStart, bounds.Top);
            RhinoApp.WriteLine($"Rendering {unit} to {unitStart}, {bounds.Top}");

            var dataSize = GH_FontServer.MeasureString(data, boldFont);
            var dataStart = bounds.Left + (boxWidth - dataSize.Width) / 2;
            var dataTop = boxTop + boxHeight / 2 - (float)dataSize.Height / 2;
            graphics.DrawString(data, boldFont, brush, dataStart, dataTop);
            RhinoApp.WriteLine($"Rendering {data} to {dataStart}, {dataTop}");
        }

        public void NewData(Results results)
        {
            // for the moment, we'll not do any caching...
        }
    }
}