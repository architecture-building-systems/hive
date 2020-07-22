namespace Hive.IO.Plots
{
    public class YearlyEmissionsPlot: AmrPlotBase
    {
        protected override string Title => "CO₂ Emissions";
        protected override string Unit => "kgCO₂";
        protected override double EmbodiedSystems => Results.EmbodiedEmissionsSystems;
        protected override double EmbodiedBuildings => Results.EmbodiedEmissionsBuildings;
        protected override double OperationSystems => Results.OperationEmissionsSystems;
        protected override double OperationBuildings => Results.OperationEmissionsBuildings;
    }
}
