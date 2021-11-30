using System.Drawing;

namespace Hive.IO.Plots
{
    public class LifetimeAmrPlot : AmrPlotBase
    {
        public LifetimeAmrPlot(string title, string description, AmrPlotDataAdaptor data, AmrPlotStyle style, bool displaySumsAndAverages = true) : base(title, description, data, style, displaySumsAndAverages)
        {
        }

        protected override float AxisMax => Data.EmbodiedBuildings + Data.EmbodiedSystems + Data.OperationBuildings +
                                   Data.OperationSystems;

        protected override float TotalBuildings => Data.TotalBuildings;
        protected override float TotalSystems => Data.TotalSystems;
        protected override float TotalEmbodied => Data.TotalEmbodied;
        protected override float TotalOperation => Data.TotalOperation;

        protected RectangleF RenderEmbodiedBuildings(Graphics graphics)
        {
            var plotBounds = EmbodiedBuildingsPlotBounds.Clone();
            plotBounds.Width /= 2;
            plotBounds.X += plotBounds.Width;
            var newMaxValue = EmbodiedBuildingsPlotBounds.Height;
            plotBounds.Height = Data.EmbodiedBuildings.Scale(AxisMax, newMaxValue);
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
            plotBounds.Height = Data.EmbodiedSystems.Scale(AxisMax, newMaxValue);
            plotBounds.Offset(-1, 1);
            graphics.FillRectangle(Style.SystemsBrush, plotBounds);
            return plotBounds;
        }

        protected RectangleF RenderOperationBuildings(Graphics graphics)
        {
            var plotBounds = OperationBuildingsPlotBounds.Clone();
            plotBounds.Width /= 2;
            var newMaxValue = OperationBuildingsPlotBounds.Height;
            plotBounds.Height = Data.OperationBuildings.Scale(AxisMax, newMaxValue);
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
            plotBounds.Height = Data.OperationSystems.Scale(AxisMax, newMaxValue);
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

            var total = Data.EmbodiedBuildings + Data.EmbodiedSystems + Data.OperationBuildings + Data.OperationSystems;
            string Caption(float value) => $"{value:0} ({value / total * 100:0}%)";

            var format = StringFormat.GenericTypographic;
            format.Alignment = StringAlignment.Center;
            format.LineAlignment = StringAlignment.Center;

            graphics.DrawString(Caption(Data.EmbodiedBuildings), NormalFont, TextBrush, ebBounds, format);
            graphics.DrawString(Caption(Data.EmbodiedSystems), NormalFont, TextBrush, esBounds, format);
            graphics.DrawString(Caption(Data.OperationBuildings), NormalFont, TextBrush, obBounds, format);
            graphics.DrawString(Caption(Data.OperationSystems), NormalFont, TextBrush, osBounds, format);
        }
    }
}