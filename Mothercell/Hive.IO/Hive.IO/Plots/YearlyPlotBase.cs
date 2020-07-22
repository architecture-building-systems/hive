using System.Drawing;

namespace Hive.IO.Plots
{
    public abstract class YearlyPlotBase : AmrPlotBase
    {
        protected abstract SolidBrush BuildingsBrush { get; }
        protected abstract SolidBrush SystemsBrush { get;  }

        protected RectangleF RenderEmbodiedBuildings(Graphics graphics, Brush brush, float value, RectangleF bounds)
        {
            var plotBounds = bounds.Clone();
            plotBounds.Width /= 2;
            plotBounds.X += plotBounds.Width;
            plotBounds.Height *= (value / bounds.Height);
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
            plotBounds.Height *= value / bounds.Height;
            plotBounds.Offset(-1, 1);
            graphics.FillRectangle(brush, plotBounds);
            return plotBounds;
        }

        protected RectangleF RenderOperationBuildings(Graphics graphics, Brush brush, float value, RectangleF bounds)
        {
            var plotBounds = bounds.Clone();
            plotBounds.Width /= 2;
            plotBounds.Height *= (value / bounds.Height);
            plotBounds.Y += (bounds.Height - plotBounds.Height);
            plotBounds.Offset(1, 1);
            graphics.FillRectangle(brush, plotBounds);
            return plotBounds;
        }

        protected RectangleF RenderOperationSystems(Graphics graphics, Brush brush, float value, RectangleF bounds)
        {
            var plotBounds = bounds.Clone();
            plotBounds.Width /= 2;
            plotBounds.Height *= value / bounds.Height;
            plotBounds.Offset(1, 1);
            graphics.FillRectangle(brush, plotBounds);
            return plotBounds;
        }

        protected override void RenderPlot(Graphics graphics)
        {
            RenderEmbodiedBuildings(graphics, BuildingsBrush, EmbodiedBuildings, EmbodiedBuildingsPlotBounds);
            RenderEmbodiedSystems(graphics, SystemsBrush, EmbodiedSystems, EmbodiedSystemsPlotBounds);
            RenderOperationBuildings(graphics, BuildingsBrush, OperationBuildings, OperationBuildingsPlotBounds);
            RenderOperationSystems(graphics, SystemsBrush, OperationSystems, OperationSystemsPlotBounds);
        }
    }
}