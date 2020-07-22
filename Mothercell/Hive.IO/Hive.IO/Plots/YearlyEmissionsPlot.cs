namespace Hive.IO.Plots
{
    public class YearlyEmissionsPlot : AmrPlotBase
    {
        protected override string Title => "CO₂ Emissions";
        protected override string Unit => "kgCO₂";
        protected override float EmbodiedSystems => (float)Results.EmbodiedEmissionsSystems;
        protected override float EmbodiedBuildings => (float)Results.EmbodiedEmissionsBuildings;
        protected override float OperationSystems => (float)Results.OperationEmissionsSystems;
        protected override float OperationBuildings => (float)Results.OperationEmissionsBuildings;
    }
}
