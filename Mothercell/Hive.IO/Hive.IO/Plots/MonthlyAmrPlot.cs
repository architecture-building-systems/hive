using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using Grasshopper.Kernel;

namespace Hive.IO.Plots
{
    public class MonthlyAmrPlot : AmrPlotBase
    {
        private const string months = "JFMAMJJASOND";

        public MonthlyAmrPlot(string title, AmrPlotDataAdaptor data, AmrPlotStyle style) : base(title, data, style)
        {
        }

        protected override float AxisMax => Data.EmbodiedBuildingsMonthly.Max() + Data.EmbodiedSystemsMonthly.Max() +
                                            Data.OperationBuildingsMonthly.Max() + Data.OperationSystemsMonthly.Max();

        protected override void RenderPlot(Graphics graphics)
        {
            RenderEmbodiedBuildings(graphics);
            RenderEmbodiedSystems(graphics);
            RenderOperationBuildings(graphics);
            RenderOperationSystems(graphics);
        }

        private void RenderOperationBuildings(Graphics graphics)
        {
            var plotBounds = OperationBuildingsPlotBounds.CloneInflate(-1, -1); // allow a bit of whitespace
            var columnWidth = plotBounds.Width / 12;
            var x = plotBounds.Left;
            foreach (var value in Data.OperationBuildingsMonthly)
            {
                var height = value.Scale(AxisMax, plotBounds.Height);
                var y = plotBounds.Y + (plotBounds.Height - height);
                var columnBounds = new RectangleF(x, y, columnWidth, height);
                graphics.FillRectangle(Style.BuildingsBrush, columnBounds);
                graphics.DrawRectangleF(new Pen(Color.White), columnBounds);
                x += columnWidth;
            }

            RenderMonthLabelsBuildingsRow(graphics, plotBounds);
        }

        private void RenderMonthLabelsBuildingsRow(Graphics graphics, RectangleF bounds)
        {
            var format = StringFormat.GenericTypographic;
            format.Alignment = StringAlignment.Center;
            format.LineAlignment = StringAlignment.Center;

            var textHeight = GH_FontServer.MeasureString(months, BoldFont).Height;
            var x = bounds.Left;
            var columnWidth = bounds.Width / 12;
            foreach (var m in months)
            {
                var monthBounds = new RectangleF(x, bounds.Y, columnWidth, textHeight);
                var xBounds = monthBounds.CloneDown();
                graphics.DrawString(m.ToString(), BoldFont, TextBrush, monthBounds, format);
                graphics.DrawString("x", NormalFont, TextBrush, xBounds, format);
                x += columnWidth;
            }
        }

        private void RenderMonthLabelsSystemsRow(Graphics graphics, RectangleF bounds)
        {
            var format = StringFormat.GenericTypographic;
            format.Alignment = StringAlignment.Center;
            format.LineAlignment = StringAlignment.Center;

            var textHeight = GH_FontServer.MeasureString(months, BoldFont).Height;
            var x = bounds.Left;
            var columnWidth = bounds.Width / 12;
            foreach (var m in months)
            {
                var xBounds = new RectangleF(x, bounds.Bottom - 2 * textHeight, columnWidth, textHeight);
                var monthBounds = xBounds.CloneDown();
                graphics.DrawString(m.ToString(), BoldFont, TextBrush, monthBounds, format);
                graphics.DrawString("x", NormalFont, TextBrush, xBounds, format);
                x += columnWidth;
            }
        }

        private void RenderOperationSystems(Graphics graphics)
        {
            var plotBounds = OperationSystemsPlotBounds.CloneInflate(-1, -1); // allow a bit of whitespace
            var columnWidth = plotBounds.Width / 12;
            var x = plotBounds.Left;
            foreach (var value in Data.OperationSystemsMonthly)
            {
                var height = value.Scale(AxisMax, plotBounds.Height);
                var y = plotBounds.Y;
                var columnBounds = new RectangleF(x, y, columnWidth, height);
                graphics.FillRectangle(Style.SystemsBrush, columnBounds);
                graphics.DrawRectangleF(new Pen(Color.White), columnBounds);
                x += columnWidth;
            }
            RenderMonthLabelsSystemsRow(graphics, plotBounds);
        }

        private void RenderEmbodiedSystems(Graphics graphics)
        {
            var plotBounds = EmbodiedSystemsPlotBounds.CloneInflate(-1, -1); // allow a bit of whitespace
            var columnWidth = plotBounds.Width / 12;
            var x = plotBounds.Left;
            foreach (var value in Data.EmbodiedSystemsMonthly)
            {
                var height = value.Scale(AxisMax, plotBounds.Height);
                var y = plotBounds.Y;
                var columnBounds = new RectangleF(x, y, columnWidth, height);
                graphics.FillRectangle(Style.SystemsBrush, columnBounds);
                graphics.DrawRectangleF(new Pen(Color.White), columnBounds);
                x += columnWidth;
            }
            RenderMonthLabelsSystemsRow(graphics, plotBounds);
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
            RenderMonthLabelsBuildingsRow(graphics, plotBounds);
        }
    }
}
