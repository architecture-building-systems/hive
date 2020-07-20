using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using Rhino.Render;

namespace Hive.IO.Plots
{
    public interface IVisualizerControl
    {
        /// <summary>
        /// Called when the control is to be drawn.
        /// </summary>
        /// <param name="results"></param>
        /// <param name="graphics"></param>
        /// <param name="bounds"></param>
        void Render(Results results, Graphics graphics, RectangleF bounds);

        bool Contains(PointF location);

        void Clicked(GH_Canvas sender, GH_CanvasMouseEvent e);
    }

    /// <summary>
    /// Defines the interface for being a plot in the GHVisualizer component.
    /// </summary>
    public interface IVisualizerPlot : IVisualizerControl
    {
        /// <summary>
        /// Called by the GHVisualizer every time new data is collected. Implementations that
        /// are caching their render output can use this to invalidate the cache.
        /// </summary>
        /// <param name="results"></param>
        void NewData(Results results);
    }
}
