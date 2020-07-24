using System.Drawing;

namespace Hive.IO.Plots
{
    public class YearlyCostsPlot : YearlyPlotBase
    {
        protected override SolidBrush BuildingsBrush => new SolidBrush(Color.FromArgb(219, 198, 163));
        protected override SolidBrush SystemsBrush => new SolidBrush(Color.FromArgb(143, 122, 87));

        protected override string Title => "Cost";
        protected override string Unit => Normalized ? "CHF/m²" : "CHF";
        protected override float EmbodiedSystems => (float)Results.EmbodiedCostsSystems(Normalized);
        protected override float EmbodiedBuildings => (float)Results.EmbodiedCostsBuildings(Normalized);
        protected override float OperationSystems => (float)Results.OperationCostsSystems(Normalized);
        protected override float OperationBuildings => (float)Results.OperationCostsBuildings(Normalized);
        protected override float TotalEmbodied => (float)Results.TotalEmbodiedCosts(Normalized);
        protected override float TotalOperation => (float)Results.TotalOperationCosts(Normalized);
        protected override float Total => (float)Results.TotalCosts(Normalized);

        public YearlyCostsPlot(bool normalized) : base(normalized)
        {
        }
    }
}