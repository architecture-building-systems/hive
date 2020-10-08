using System;
using System.Drawing;
using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using Hive.IO.Results;

namespace Hive.IO.Plots
{
    public delegate double QueryResults(ResultsPlotting results, bool normalized);

    public struct KpiPlotProperties
    {
        public Color Color;
        public Color BenchmarkFailedColor;
        public string UnitText;
        public string NormalizedUnitText;
        public QueryResults Data;
        public Kpi Kpi;
    }

    public class OperationalPerformancePlot: IVisualizerControl
    {
        public event EventHandler OnClicked;

        private readonly Font _standardFont;
        private readonly Font _boldFont;
        private readonly float _penWidth;
        private readonly KpiPlotProperties _properties;
        private RectangleF _bounds = RectangleF.Empty;

        /// <summary>
        /// Draw a square box with data inside and a unit string above.
        /// </summary>
        /// <param name="properties">KpiPlotProperties to use for this plot</param>
        public OperationalPerformancePlot(KpiPlotProperties properties)
        {
            _standardFont = GH_FontServer.Standard;
            _boldFont = GH_FontServer.StandardBold;
            _properties = properties;
            _penWidth = GH_FontServer.MeasureString("abc", _standardFont).Height;
        }

        public Kpi Kpi => _properties.Kpi;

        private String UnitText => Normalized ? _properties.NormalizedUnitText : _properties.UnitText;

        public bool Normalized { get; set; }

        private String Data(ResultsPlotting results)
        {
            var value = _properties.Data(results, Normalized);
            return $"{value:F1}";
        }

        public void Render(ResultsPlotting results, Graphics graphics, RectangleF bounds)
        {
            Render(results, graphics, bounds, false);
        }

        public bool Contains(PointF location) => _bounds.Contains(location);

        public void Clicked(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            OnClicked?.Invoke(this, e);
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
        /// <param name="selected">draw a black bar at top if selected</param>
        public void Render(ResultsPlotting results, Graphics graphics, RectangleF bounds, bool selected)
        {
            // save bounds for Contains method - when user clicks on the plot
            _bounds = bounds;
            var brush = new SolidBrush(Color.Black);
            var pen = new Pen(_properties.Color, _penWidth);

            // draw box
            var box = new RectangleF(bounds.Location, bounds.Size);
            box.Inflate(-_penWidth / 2, -_penWidth / 2); // make sure lines fit _inside_ box
            graphics.DrawRectangle(pen, box.Left, box.Top, box.Width, box.Height);
            if (!Normalized)
            {
                graphics.FillRectangle(new SolidBrush(_properties.Color), box);
            }

            if (selected)
            {
                // draw black selected bar on top
                var selectionBounds = bounds.Clone();
                selectionBounds.Height = _penWidth;
                graphics.FillRectangle(new SolidBrush(Color.Black), selectionBounds);
            }
           

            // center data in the box
            var data = Data(results);
            var dataSize = GH_FontServer.MeasureString(data, _boldFont);
            var dataX = bounds.Left + (bounds.Width - dataSize.Width) / 2;
            var dataY = bounds.Top + bounds.Height / 2 - (float)dataSize.Height / 2;
            graphics.DrawString(data, _boldFont, brush, dataX, dataY);
            // center unit below data
            var unitX = bounds.Left + (bounds.Width - GH_FontServer.StringWidth(UnitText, _standardFont)) / 2;
            var unitY = dataY + dataSize.Height;
            graphics.DrawString(UnitText, _standardFont, brush, unitX, unitY);
        }

        public void NewData(Results.Results results)
        {
            // for the moment, we'll not do any caching...
        }
    }
}