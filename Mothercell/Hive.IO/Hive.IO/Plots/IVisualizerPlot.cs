using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Render;

namespace Hive.IO.Plots
{
    /// <summary>
    /// Defines the interface for being a plot in the GHVisualizer component.
    /// </summary>
    public interface IVisualizerPlot
    {
        /// <summary>
        /// Called when the plot is to be drawn - note that implementations should probably use
        /// some kind of caching mechanism.
        /// </summary>
        /// <param name="results"></param>
        /// <param name="graphics"></param>
        /// <param name="bounds"></param>
        void Render(Results results, Graphics graphics, RectangleF bounds);

        /// <summary>
        /// Called by the GHVisualizer every time new data is collected. Implementations that
        /// are caching their render output can use this to invalidate the cache.
        /// </summary>
        /// <param name="results"></param>
        void NewData(Results results);
    }
}
