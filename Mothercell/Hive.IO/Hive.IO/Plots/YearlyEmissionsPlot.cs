using System.Drawing;

namespace Hive.IO.Plots
{
    public class YearlyEmissionsPlot : YearlyPlotBase
    {
        protected override SolidBrush BuildingsBrush => new SolidBrush(Color.FromArgb(177, 218, 143));
        protected override SolidBrush SystemsBrush => new SolidBrush(Color.FromArgb(100, 141, 66));

        protected override string Title => "CO₂ Emissions";
        protected override string Unit => "kgCO₂";

        protected override float EmbodiedSystems => (float)Results.EmbodiedEmissionsSystems;
        protected override float EmbodiedBuildings => (float)Results.EmbodiedEmissionsBuildings;
        protected override float OperationSystems => (float)Results.OperationEmissionsSystems;
        protected override float OperationBuildings => (float)Results.OperationEmissionsBuildings;
    }
}
