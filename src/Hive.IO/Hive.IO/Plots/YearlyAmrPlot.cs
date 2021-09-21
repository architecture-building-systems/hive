using System.Drawing;

namespace Hive.IO.Plots
{
    public class YearlyAmrPlot : AmrPlotBase
    {
        public YearlyAmrPlot(string title, AmrPlotDataAdaptor data, AmrPlotStyle style) : base(title, data, style)
        {
        }

        protected override float AxisMax => Data.EmbodiedBuildingsYearly + Data.EmbodiedSystemsYearly + Data.OperationBuildingsYearly +
                                   Data.OperationSystemsYearly;

        protected RectangleF RenderEmbodiedBuildings(Graphics graphics)
        {
            var plotBounds = EmbodiedBuildingsPlotBounds.Clone();
            plotBounds.Width /= 2;
            plotBounds.X += plotBounds.Width;
            var newMaxValue = EmbodiedBuildingsPlotBounds.Height;
            plotBounds.Height = Data.EmbodiedBuildingsYearly.Scale(AxisMax, newMaxValue);
            plotBounds.Y += (newMaxValue - plotBounds.Height);
            plotBounds.Offset(-1, -1);
            graphics.FillRectangle(Style.BuildingsBrush, plotBounds);
            return plotBounds;
        }

        protected RectangleF RenderEmbodiedSystems(Graphics graphics)
        {
            var plotBounds = EmbodiedSystemsPlotBounds.Clone();
            plotBounds.Width /= 2;
            plotBounds.X += plotBounds.Width;
            var newMaxValue = EmbodiedSystemsPlotBounds.Height;
            plotBounds.Height = Data.EmbodiedSystemsYearly.Scale(AxisMax, newMaxValue);
            plotBounds.Offset(-1, 1);
            graphics.FillRectangle(Style.SystemsBrush, plotBounds);
            return plotBounds;
        }

        protected RectangleF RenderOperationBuildings(Graphics graphics)
        {
            var plotBounds = OperationBuildingsPlotBounds.Clone();
            plotBounds.Width /= 2;
            var newMaxValue = OperationBuildingsPlotBounds.Height;
            plotBounds.Height = Data.OperationBuildingsYearly.Scale(AxisMax, newMaxValue);
            plotBounds.Y += (newMaxValue - plotBounds.Height);
            plotBounds.Offset(1, -1);
            graphics.FillRectangle(Style.BuildingsBrush, plotBounds);
            return plotBounds;
        }

        protected RectangleF RenderOperationSystems(Graphics graphics)
        {
            var plotBounds = OperationSystemsPlotBounds.Clone();
            plotBounds.Width /= 2;
            var newMaxValue = OperationSystemsPlotBounds.Height;
            plotBounds.Height = Data.OperationSystemsYearly.Scale(AxisMax, newMaxValue);
            plotBounds.Offset(1, 1);
            graphics.FillRectangle(Style.SystemsBrush, plotBounds);
            return plotBounds;
        }

        protected override void RenderPlot(Graphics graphics)
        {
            var ebBounds = RenderEmbodiedBuildings(graphics);
            var esBounds = RenderEmbodiedSystems(graphics);
            var obBounds = RenderOperationBuildings(graphics);
            var osBounds = RenderOperationSystems(graphics);

            var total = Data.EmbodiedBuildingsYearly + Data.EmbodiedSystemsYearly + Data.OperationBuildingsYearly + Data.OperationSystemsYearly;
            string Caption(float value) => $"{value:0} ({value / total * 100:0}%)";

            var format = StringFormat.GenericTypographic;
            format.Alignment = StringAlignment.Center;
            format.LineAlignment = StringAlignment.Center;

            graphics.DrawString(Caption(Data.EmbodiedBuildingsYearly), NormalFont, TextBrush, ebBounds, format);
            graphics.DrawString(Caption(Data.EmbodiedSystemsYearly), NormalFont, TextBrush, esBounds, format);
            graphics.DrawString(Caption(Data.OperationBuildingsYearly), NormalFont, TextBrush, obBounds, format);
            graphics.DrawString(Caption(Data.OperationSystemsYearly), NormalFont, TextBrush, osBounds, format);
        }
    }
}