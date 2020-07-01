using System;
using System.Drawing;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using Rhino;

namespace Hive.IO.Plots
{
    public delegate double QueryResults(Results results);

    public struct EnergyPlotProperties
    {
        public Color Color;
        public Color BenchmarkFailedColor;
        public String UnitText;
        public String SpecificUnitText;
        public QueryResults Data;
        public QueryResults SpecificData;
    }

    public class OperationalPerformancePlot: IVisualizerPlot
    {
        private readonly Font _standardFont;
        private readonly Font _boldFont;
        private readonly float _penWidth;
        private bool _normalized = false;  // the state we start out in
        private readonly EnergyPlotProperties _properties;
        private RectangleF _bounds = RectangleF.Empty;

        /// <summary>
        /// Draw a square box with data inside and a unit string above.
        /// </summary>
        /// <param name="totalHeight">the total height available including unit string</param>
        /// <param name="properties">EnergyPlotProperties to use for this plot</param>
        public OperationalPerformancePlot(EnergyPlotProperties properties)
        {
            _standardFont = GH_FontServer.Standard;
            _boldFont = GH_FontServer.StandardBold;
            _properties = properties;
            _penWidth = 30f;
        }

        private String UnitText => _normalized ? _properties.SpecificUnitText : _properties.UnitText;

        private String Data(Results results)
        {
            var value = _normalized ? _properties.SpecificData(results) : _properties.Data(results);
            return $"{value:F1}";
        }

        public bool Contains(PointF location) => _bounds.Contains(location);

        public void Clicked(GH_Canvas sender)
        {
            _normalized = !_normalized;
            sender.Invalidate();
        }

        /// <summary>
        /// Draw a box with some text
        /// The bold text (Data) goes bang in the middle, centered vertically and horizontally.
        /// The standard text (Unit) goes in the lower part, bang in the middle, centered horizontally
        /// </summary>
        /// <param name="results"></param>
        /// <param name="graphics"></param>
        /// <param name="bounds">expected height of bounds should be TotalHeight</param>
        public void Render(Results results, Graphics graphics, RectangleF bounds)
        {
            // save bounds for Contains method - when user clicks on the plot
            _bounds = bounds;
            var brush = new SolidBrush(Color.Black);
            var pen = new Pen(_properties.Color, _penWidth);

            // draw box
            var box = new RectangleF(bounds.Location, bounds.Size);
            box.Inflate(-_penWidth / 2, -_penWidth / 2); // make sure lines fit _inside_ box
            graphics.DrawRectangle(pen, box.Left, box.Top, box.Width, box.Height);
            if (!_normalized)
            {
                graphics.FillRectangle(new SolidBrush(_properties.Color), box);
            }

            // center data in the box
            var data = Data(results);
            var dataSize = GH_FontServer.MeasureString(data, _boldFont);
            var dataX = bounds.Left + (bounds.Width - dataSize.Width) / 2;
            var dataY = bounds.Top + bounds.Height / 2 - (float)dataSize.Height / 2;
            graphics.DrawString(data, _boldFont, brush, dataX, dataY);
            RhinoApp.WriteLine($"Rendering {data} to {dataX}, {dataY}");

            // center unit below data
            var unitX = bounds.Left + (bounds.Width - GH_FontServer.StringWidth(UnitText, _standardFont)) / 2;
            var unitY = dataY + dataSize.Height;
            graphics.DrawString(UnitText, _standardFont, brush, unitX, unitY);
            RhinoApp.WriteLine($"Rendering {UnitText} to {unitX}, {unitY}");
        }

        public void NewData(Results results)
        {
            // for the moment, we'll not do any caching...
        }
    }
}