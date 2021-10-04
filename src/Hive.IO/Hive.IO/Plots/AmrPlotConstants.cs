
namespace Hive.IO.Plots
{
    public static class AmrPlotConstants
    {
        // Energy Plots
        public static readonly string EnergyTitle = "Energy";
        public static readonly string EnergyDescription = 
            $"Building Lifetime = {Misc.DefaultBuildingLifetime} year\n" 
            + "Operational Building = total demand\n" 
            + "Operational Systems = non-renewable primary energy (all imported energy)";

        // Emissions Plots
        public static readonly string EmissionsTitle = "CO₂ Emissions";
        public static readonly string EmissionsDescription = $"Building Lifetime = {Misc.DefaultBuildingLifetime} years";

        // Cost Plots
        public static readonly string CostTitle = "Cost";
        public static readonly string CostDescription = 
            $"Building Lifetime = {Misc.DefaultBuildingLifetime} years\n" 
            + "Interest Rate = {Misc.DefaultInterestRate * 100} %";

        // Tooltips

        public static readonly string EnergyEmbodiedBuildingsTooltip = "";
        public static readonly string EnergyEmbodiedSystemsTooltip = "";
        public static readonly string EnergyOperationBuildingsTooltip = "";
        public static readonly string EnergyOperationSystemsTooltip = "";

        public static readonly string EmissionsEmbodiedBuildingsTooltip = "Embodied greenhouse gas emissions for the fabrication and construction of the building structure, in kgCO2-equivalent.";
        public static readonly string EmissionsEmbodiedSystemsTooltip = "Embodied greenhouse gas emissions for the creation and installation of the systems, in kgCO2-equivalent.";
        public static readonly string EmissionsOperationBuildingsTooltip = "Greenhouse gas emissions related to maintenance and parts replacement of the building structure, in kgCO2-equivalent.";
        public static readonly string EmissionsOperationSystemsTooltip = "Greenhouse gas emissions related to running the systems (including from imported energy like electricity or gas), in kgCO2-equivalent.";

        public static readonly string CostEmbodiedBuildingsTooltip = "Levelised capital cost of constructing the building, given an interest rate and expected building lifetime.";
        public static readonly string CostEmbodiedSystemsTooltip = "Levelised capital cost of constructing the systems, given an interest rate and expected building lifetime.";
        public static readonly string CostOperationBuildingsTooltip = "Levelised operating costs for maintenance of the building (cleaning, replacing windows, ...), insurance, taxes, etc.";
        public static readonly string CostOperationSystemsTooltip = "Levelised operating costs for running the systems, including energy costs and maintenance.";


    }
}
