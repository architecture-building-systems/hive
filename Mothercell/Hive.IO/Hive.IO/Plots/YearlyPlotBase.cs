using System;
using System.Drawing;
using System.Windows.Forms;
using Grasshopper.GUI.Base;
using Rhino;

namespace Hive.IO.Plots
{
    public abstract class YearlyPlotBase : AmrPlotBase
    {
        protected abstract SolidBrush BuildingsBrush { get; }
        protected abstract SolidBrush SystemsBrush { get; }

        protected RectangleF RenderEmbodiedBuildings(Graphics graphics, Brush brush, float value, RectangleF bounds)
        {
            var plotBounds = bounds.Clone();
            plotBounds.Width /= 2;
            plotBounds.X += plotBounds.Width;
            plotBounds.Height = Scale(value, AxisMax, bounds.Height);
            plotBounds.Y += (bounds.Height - plotBounds.Height);
            plotBounds.Offset(-1, -1);
            graphics.FillRectangle(brush, plotBounds);
            return plotBounds;
        }

        protected RectangleF RenderEmbodiedSystems(Graphics graphics, Brush brush, float value, RectangleF bounds)
        {
            var plotBounds = bounds.Clone();
            plotBounds.Width /= 2;
            plotBounds.X += plotBounds.Width;
            plotBounds.Height = Scale(value, AxisMax, bounds.Height);
            plotBounds.Offset(-1, 1);
            graphics.FillRectangle(brush, plotBounds);
            return plotBounds;
        }

        protected RectangleF RenderOperationBuildings(Graphics graphics, Brush brush, float value, RectangleF bounds)
        {
            var plotBounds = bounds.Clone();
            plotBounds.Width /= 2;
            plotBounds.Height = Scale(value, AxisMax, bounds.Height);
            plotBounds.Y += (bounds.Height - plotBounds.Height);
            plotBounds.Offset(1, 1);
            graphics.FillRectangle(brush, plotBounds);
            return plotBounds;
        }

        protected RectangleF RenderOperationSystems(Graphics graphics, Brush brush, float value, RectangleF bounds)
        {
            var plotBounds = bounds.Clone();
            plotBounds.Width /= 2;
            plotBounds.Height = Scale(value, AxisMax, bounds.Height);
            plotBounds.Offset(1, 1);
            graphics.FillRectangle(brush, plotBounds);
            return plotBounds;
        }

        private float Scale(float value, float maxValue, float newMaxValue)
        {
            if (value >= maxValue)
            {
                return newMaxValue;
            }

            return value / maxValue * newMaxValue;
        }

        protected override void RenderPlot(Graphics graphics)
        {
            var ebBounds = RenderEmbodiedBuildings(graphics, BuildingsBrush, EmbodiedBuildings, EmbodiedBuildingsPlotBounds);
            var esBounds = RenderEmbodiedSystems(graphics, SystemsBrush, EmbodiedSystems, EmbodiedSystemsPlotBounds);
            var obBounds = RenderOperationBuildings(graphics, BuildingsBrush, OperationBuildings, OperationBuildingsPlotBounds);
            var osBounds = RenderOperationSystems(graphics, SystemsBrush, OperationSystems, OperationSystemsPlotBounds);

            var total = EmbodiedBuildings + EmbodiedSystems + OperationBuildings + OperationSystems;
            string Caption(float value) => $"{value:0} ({value / total * 100:0}%)";

            var format = StringFormat.GenericTypographic;
            format.Alignment = StringAlignment.Center;
            format.LineAlignment = StringAlignment.Center;

            graphics.DrawString(Caption(EmbodiedBuildings), NormalFont, TextBrush, ebBounds, format);
            graphics.DrawString(Caption(EmbodiedSystems), NormalFont, TextBrush, esBounds, format);
            graphics.DrawString(Caption(OperationBuildings), NormalFont, TextBrush, obBounds, format);
            graphics.DrawString(Caption(OperationSystems), NormalFont, TextBrush, osBounds, format);
        }
    }
}