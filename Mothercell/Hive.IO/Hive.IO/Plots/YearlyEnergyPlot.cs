namespace Hive.IO.Plots
{
    public class YearlyEnergyPlot: AmrPlotBase
    {
        protected override string Title => "Energy";
        protected override string Unit => "kWh";
        protected override double EmbodiedSystems => Results.EmbodiedEnergySystems;
        protected override double EmbodiedBuildings => Results.EmbodiedEnergyBuildings;
        protected override double OperationSystems => Results.OperationEnergySystems;
        protected override double OperationBuildings => Results.OperationEnergyBuildings;
    }
}
