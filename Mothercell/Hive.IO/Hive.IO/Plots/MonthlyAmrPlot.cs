﻿using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Hive.IO.Plots
{
    public class MonthlyAmrPlot: AmrPlotBase
    {
        public MonthlyAmrPlot(string title, AmrPlotDataAdaptor data, AmrPlotStyle style) : base(title, data, style)
        {
        }

        protected override float AxisMax => Data.EmbodiedBuildingsMonthly.Max() + Data.EmbodiedSystemsMonthly.Max() +
                                            Data.OperationBuildingsMontholy.Max() + Data.OperationSystemsMonthly.Max();

        protected override void RenderPlot(Graphics graphics)
        {
            RenderEmbodiedBuildings(graphics);
            RenderEmbodiedSystems(graphics);
            RenderOperationBuildings(graphics);
            RenderOperationSystems(graphics);
        }

        private void RenderOperationBuildings(Graphics graphics)
        {
        }

        private void RenderOperationSystems(Graphics graphics)
        {
            
        }

        private void RenderEmbodiedSystems(Graphics graphics)
        {
            
        }

        private void RenderEmbodiedBuildings(Graphics graphics)
        {
            var plotBounds = EmbodiedBuildingsPlotBounds.CloneInflate(-1, -1); // allow a bit of whitespace
            var columnWidth = plotBounds.Width / 12;
            var x = plotBounds.Left;
            foreach (var value in Data.EmbodiedBuildingsMonthly)
            {
                var height = value.Scale(AxisMax, plotBounds.Height);
                var y = plotBounds.Y + (plotBounds.Height - height);
                var columnBounds = new RectangleF(x, y, columnWidth, height);
                graphics.FillRectangle(Style.BuildingsBrush, columnBounds);
                graphics.DrawRectangleF(new Pen(Color.White), columnBounds);
                x += columnWidth;
            }
        }
    }
}
