using System.Drawing;
using OxyPlot;
using OxyPlot.WindowsForms;

namespace Hive.IO.Plots
{
    /// <summary>
    /// A base class for plots based on the OxyPlot library. Implements a caching mechanism for
    /// speeding up re-drawing when the GHVisualizer is moved around.
    /// </summary>
    public abstract class OxyPlotBase : IVisualizerPlot
    {
        private Bitmap _bitmapCache;
        private int _lastPlotWidth;
        private int _lastPlotHeight;

        // colors for plots
        protected static readonly OxyColor SpaceHeatingColor = OxyColor.FromRgb(255, 0, 0);
        protected static readonly OxyColor SpaceCoolingColor = OxyColor.FromRgb(0, 176, 240);
        protected static readonly OxyColor ElectricityColor = OxyColor.FromRgb(255, 217, 102);
        protected static readonly OxyColor DhwColor = OxyColor.FromRgb(192, 0, 0);
        protected static readonly OxyColor BackgroundColor = OxyColor.FromArgb(0, 0, 0, 0);

        public void Render(Results results, Graphics graphics, RectangleF bounds)
        {
            var plotWidth = (int)bounds.Width;
            var plotHeight = (int)bounds.Height;

            var bitmap = IsBitmapCacheStillValid(plotWidth, plotHeight) ? _bitmapCache : RenderToBitmap(results, plotWidth, plotHeight);

            graphics.DrawImage(bitmap, bounds.Location.X, bounds.Location.Y, bounds.Width, bounds.Height);

            // save cache for next call
            _lastPlotWidth = plotWidth;
            _lastPlotHeight = plotHeight;
            _bitmapCache = bitmap;
        }

        private Bitmap RenderToBitmap(Results results, int plotWidth, int plotHeight)
        {
            Bitmap bitmap;
            var model = CreatePlotModel(results);
            var pngExporter = new PngExporter
            {
                Width = plotWidth,
                Height = plotHeight,
                Background = OxyColors.White
            };
            bitmap = pngExporter.ExportToBitmap(model);
            return bitmap;
        }

        public void NewData(Results results)
        {
            _bitmapCache = null;
        }

        protected abstract PlotModel CreatePlotModel(Results results);

        private bool IsBitmapCacheStillValid(int plotWidth, int plotHeight)
        {
            return !(_bitmapCache is null) && plotWidth == _lastPlotWidth && plotHeight == _lastPlotHeight;
        }
    }
}
