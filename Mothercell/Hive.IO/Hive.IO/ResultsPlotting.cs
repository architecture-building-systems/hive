namespace Hive.IO
{
    // An adaptor for extracting aggregated hive results.
    public class ResultsPlotting
    {
        public Results Results { get; }

        public ResultsPlotting(Results results)
        {
            Results = results;
        }

        public double TotalFloorArea => 180;

        #region Emissions
        public double EmbodiedEmissionsBuildings(bool normalized) => normalized ? EmbodiedEmissionsBuildings(false) / TotalFloorArea : 1000;
        public double EmbodiedEmissionsSystems(bool normalized) => normalized ? EmbodiedEmissionsSystems(false) / TotalFloorArea : 2000;
        public double OperationEmissionsBuildings(bool normalized) => normalized? OperationEmissionsBuildings(false) / TotalFloorArea: 3000;
        public double OperationEmissionsSystems(bool normalized) => normalized? OperationEmissionsSystems(false) / TotalFloorArea: 4000;

        public double TotalEmbodiedEmissions(bool normalized) =>
            EmbodiedEmissionsBuildings(normalized) + EmbodiedEmissionsSystems(normalized);

        public double TotalOperationEmissions(bool normalized) =>
            OperationEmissionsBuildings(normalized) + OperationEmissionsSystems(normalized);

        public double TotalEmissions(bool normalized) =>
            TotalEmbodiedEmissions(normalized) + TotalOperationEmissions(normalized);
        #endregion Emissions

        #region Costs
        public double EmbodiedCostsBuildings(bool normalized) => normalized? EmbodiedCostsBuildings(false) / TotalFloorArea: 1100;
        public double EmbodiedCostsSystems(bool normalized) => normalized? EmbodiedCostsSystems(false) / TotalFloorArea: 2200;
        public double OperationCostsBuildings(bool normalized) => normalized? OperationCostsBuildings(false) / TotalFloorArea: 3300;
        public double OperationCostsSystems(bool normalized) =>normalized? OperationCostsSystems(false) / TotalFloorArea: 4400;

        public double TotalEmbodiedCosts(bool normalized) =>
            EmbodiedCostsBuildings(normalized) + EmbodiedCostsSystems(normalized);

        public double TotalOperationCosts(bool normalized) =>
            EmbodiedCostsBuildings(normalized) + EmbodiedCostsSystems(normalized);

        public double TotalCosts(bool normalized) => TotalEmbodiedCosts(normalized) + TotalOperationCosts(normalized);
        #endregion Costs

        #region Energy
        public double EmbodiedEnergyBuildings(bool normalized) =>normalized? EmbodiedEnergyBuildings(false) / TotalFloorArea: 1300;
        public double EmbodiedEnergySystems(bool normalized) => normalized? EmbodiedEnergySystems(false) / TotalFloorArea: 2300;
        public double OperationEnergyBuildings(bool normalized) => normalized? OperationEnergyBuildings(false) / TotalFloorArea: 3300;
        public double OperationEnergySystems(bool normalized) => normalized ? OperationEnergySystems(false) / TotalFloorArea : 4400;

        public double TotalEmbodiedEnergy(bool normalized) =>
            EmbodiedEnergyBuildings(normalized) + EmbodiedEnergySystems(normalized);

        public double TotalOperationEnergy(bool normalized) =>
            OperationEnergyBuildings(normalized) + OperationEnergySystems(normalized);

        public double TotalEnergy(bool normalized) =>
            TotalEmbodiedEnergy(normalized) + TotalOperationEnergy(normalized);

        #endregion Energy
    }
}
