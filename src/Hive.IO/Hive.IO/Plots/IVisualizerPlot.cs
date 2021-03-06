﻿using System.Collections.Generic;
using System.Drawing;
using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using Hive.IO.Results;

namespace Hive.IO.Plots
{
    public interface IVisualizerControl
    {
        /// <summary>
        ///     Called when the control is to be drawn.
        /// </summary>
        /// <param name="results"></param>
        /// <param name="graphics"></param>
        /// <param name="bounds"></param>
        void Render(ResultsPlotting results, Dictionary<string, string> plotProperties, Graphics graphics,
            RectangleF bounds);

        bool Contains(PointF location);

        void Clicked(GH_Canvas sender, GH_CanvasMouseEvent e);
    }

    /// <summary>
    ///     Defines the interface for being a plot in the GHVisualizer component.
    /// </summary>
    public interface IVisualizerPlot : IVisualizerControl
    {
        /// <summary>
        ///     Called by the GHVisualizer every time new data is collected. Implementations that
        ///     are caching their render output can use this to invalidate the cache.
        /// </summary>
        /// <param name="results"></param>
        void NewData(ResultsPlotting results);
    }
}