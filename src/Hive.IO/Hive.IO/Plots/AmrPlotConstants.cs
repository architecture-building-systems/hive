
namespace Hive.IO.Plots
{
    public static class AmrPlotConstants
    {
        // Energy Plots
        public static readonly string EnergyTitle = "Energy";
        public static readonly string EnergyDescription = $"Building Lifetime = {Misc.DefaultBuildingLifetime} years\nOperational Building = total demand\nOperatioanl Systems = non-renewable primary energy (all imported energy)";

        // Emissions Plots
        public static readonly string EmissionsTitle = "CO₂ Emissions";
        public static readonly string EmissionsDescription = $"Building Lifetime = {Misc.DefaultBuildingLifetime} years";

        // Cost Plots
        public static readonly string CostTitle = "Cost";
        public static readonly string CostDescription = $"Building Lifetime = {Misc.DefaultBuildingLifetime} years\nInterest Rate = {Misc.DefaultInterestRate * 100} %";

    }
}
