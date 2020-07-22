using System.Drawing;

namespace Hive.IO.Plots
{
    public class YearlyCostsPlot : YearlyPlotBase
    {
        protected override SolidBrush BuildingsBrush => new SolidBrush(Color.FromArgb(219, 198, 163));
        protected override SolidBrush SystemsBrush => new SolidBrush(Color.FromArgb(143, 122, 87));

        protected override string Title => "Cost";
        protected override string Unit => "CHF";
        protected override float EmbodiedSystems => (float)Results.EmbodiedCostsSystems;
        protected override float EmbodiedBuildings => (float)Results.EmbodiedCostsBuildings;
        protected override float OperationSystems => (float)Results.OperationCostsSystems;
        protected override float OperationBuildings => (float)Results.OperationCostsBuildings;
    }
}