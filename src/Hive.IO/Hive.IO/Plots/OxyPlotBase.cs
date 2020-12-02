using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using Hive.IO.Results;
using OxyPlot;
using OxyPlot.WindowsForms;

namespace Hive.IO.Plots
{
    /// <summary>
    ///     A base class for plots based on the OxyPlot library. Implements a caching mechanism for
    ///     speeding up re-drawing when the GHVisualizer is moved around.
    /// </summary>
    public abstract class OxyPlotBase : IVisualizerPlot
    {
        // colors for plots
        protected static readonly OxyColor SpaceHeatingColor = OxyColor.FromRgb(255, 0, 0);
        protected static readonly OxyColor SpaceCoolingColor = OxyColor.FromRgb(0, 176, 240);
        protected static readonly OxyColor ElectricityColor = OxyColor.FromRgb(255, 217, 102);
        protected static readonly OxyColor DhwColor = OxyColor.FromRgb(192, 0, 0);
        protected static readonly OxyColor BackgroundColor = OxyColors.Transparent;
        private Bitmap _bitmapCache;
        private RectangleF _bounds;
        private int _lastPlotHeight;
        private string _lastPlotParameters;
        private int _lastPlotWidth;

        public void Render(ResultsPlotting results, Dictionary<string, string> plotParameters, Graphics graphics,
            RectangleF bounds)
        {
            _bounds = bounds; // store for Contains check

            var plotWidth = (int) bounds.Width;
            var plotHeight = (int) bounds.Height;

            var bitmap = IsBitmapCacheStillValid(plotWidth, plotHeight, plotParameters)
                ? _bitmapCache
                : RenderToBitmap(results, plotWidth, plotHeight, plotParameters);

            graphics.DrawImage(bitmap, bounds.Location.X, bounds.Location.Y, bounds.Width, bounds.Height);

            // save cache for next call
            _lastPlotWidth = plotWidth;
            _lastPlotHeight = plotHeight;
            _lastPlotParameters = StringifyPlotParameters(plotParameters);
            _bitmapCache = bitmap;
        }

        public bool Contains(PointF location)
        {
            return _bounds.Contains(location);
        }

        public void Clicked(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
        }

        public void NewData(ResultsPlotting results)
        {
            _bitmapCache = null;
        }

        /// <summary>
        ///     We only really need to know if the plot parameters changed for caching purposes,
        ///     not what the real value is...
        /// </summary>
        /// <param name="plotParameters"></param>
        /// <returns></returns>
        private string StringifyPlotParameters(Dictionary<string, string> plotParameters)
        {
            return string.Join("|", plotParameters.Keys.OrderBy(s => s).Select(s => plotParameters[s]));
        }

        private Bitmap RenderToBitmap(ResultsPlotting results, int plotWidth, int plotHeight,
            Dictionary<string, string> plotParameters)
        {
            Bitmap bitmap;
            var model = CreatePlotModel(results, plotParameters);
            var pngExporter = new PngExporter
            {
                Width = plotWidth,
                Height = plotHeight,
                Background = OxyColors.Transparent
            };
            bitmap = pngExporter.ExportToBitmap(model);
            return bitmap;
        }

        protected abstract PlotModel CreatePlotModel(ResultsPlotting results, Dictionary<string, string> plotParameters);

        private bool IsBitmapCacheStillValid(int plotWidth, int plotHeight, Dictionary<string, string> plotParameters)
        {
            return !(_bitmapCache is null) && plotWidth == _lastPlotWidth && plotHeight == _lastPlotHeight &&
                   StringifyPlotParameters(plotParameters) == _lastPlotParameters;
        }
    }

    public static class StringDictionaryExtensions
    {
        public static double? ReadDouble(this Dictionary<string, string> plotParameters, string key)
        {
            try
            {
                return double.Parse(plotParameters[key]);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}