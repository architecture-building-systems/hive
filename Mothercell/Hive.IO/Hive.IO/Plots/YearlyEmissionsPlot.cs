using System.Drawing;

namespace Hive.IO.Plots
{
    public class YearlyEmissionsPlot : YearlyPlotBase
    {
        protected override SolidBrush BuildingsBrush => new SolidBrush(Color.FromArgb(221, 229, 124));
        protected override SolidBrush SystemsBrush => new SolidBrush(Color.FromArgb(145, 153, 48));

        protected override string Title => "CO₂ Emissions";
        protected override string Unit => "kgCO₂";

        protected override float EmbodiedSystems => (float)Results.EmbodiedEmissionsSystems;
        protected override float EmbodiedBuildings => (float)Results.EmbodiedEmissionsBuildings;
        protected override float OperationSystems => (float)Results.OperationEmissionsSystems;
        protected override float OperationBuildings => (float)Results.OperationEmissionsBuildings;
    }
}
