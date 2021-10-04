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

        public MonthlyAmrPlot(string title, string description, AmrPlotDataAdaptor data, AmrPlotStyle style, bool displaySumsAndAverages = true) : base(title, description, data, style, displaySumsAndAverages)
        {
        }

        protected override float AxisMax => Data.EmbodiedBuildingsMonthly.Max() + Data.EmbodiedSystemsMonthly.Max() +
                                            Data.OperationBuildingsMonthly.Max() + Data.OperationSystemsMonthly.Max();

        protected override float TotalBuildings => Data.AverageBuildingsMonthly;
        protected override float TotalSystems => Data.AverageSystemsMonthly;
        protected override float TotalEmbodied => Data.AverageEmbodiedMonthly;
        protected override float TotalOperation => Data.AverageOperationMonthly;

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

                // Value label
                var format = StringFormat.GenericTypographic;
                format.Alignment = StringAlignment.Center;
                format.LineAlignment = StringAlignment.Center;
                var textHeight = GH_FontServer.MeasureString(value.ToString(), SmallFont).Height;
                var valueLabelBounds = new RectangleF(x, y - 2 * textHeight, columnWidth, 2 * textHeight);
                graphics.DrawString(PlotCaption(value), SmallFont, TextBrush, valueLabelBounds, format);

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
                graphics.DrawString(m.ToString(), BoldFont, TextBrush, monthBounds, format);
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
                var monthBounds = new RectangleF(x, bounds.Bottom - textHeight, columnWidth, textHeight);
                graphics.DrawString(m.ToString(), BoldFont, TextBrush, monthBounds, format);
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

                // Value label
                var format = StringFormat.GenericTypographic;
                format.Alignment = StringAlignment.Center;
                format.LineAlignment = StringAlignment.Center;
                var textHeight = GH_FontServer.MeasureString(value.ToString(), SmallFont).Height;
                var valueLabelBounds = new RectangleF(x, y + height, columnWidth, textHeight * 2);
                graphics.DrawString(PlotCaption(value), SmallFont, TextBrush, valueLabelBounds, format);

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

                // Value label
                var format = StringFormat.GenericTypographic;
                format.Alignment = StringAlignment.Center;
                format.LineAlignment = StringAlignment.Center;
                var textHeight = GH_FontServer.MeasureString(value.ToString(), SmallFont).Height;
                var valueLabelBounds = new RectangleF(x, y + height, columnWidth, textHeight * 2);
                graphics.DrawString(PlotCaption(value), SmallFont, TextBrush, valueLabelBounds, format);

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

                // Value label
                var format = StringFormat.GenericTypographic;
                format.Alignment = StringAlignment.Center;
                format.LineAlignment = StringAlignment.Center;
                var textHeight = GH_FontServer.MeasureString(value.ToString(), SmallFont).Height;
                var valueLabelBounds = new RectangleF(x, y - 2 * textHeight, columnWidth, 2 * textHeight);
                graphics.DrawString(PlotCaption(value), SmallFont, TextBrush, valueLabelBounds, format);

                x += columnWidth;
            }
            RenderMonthLabelsBuildingsRow(graphics, plotBounds);
        }

        // Without units to save space
        private string PlotCaption(double x) => $"{x:0}";

    }
}
