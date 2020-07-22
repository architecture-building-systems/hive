namespace Hive.IO
{
    // An adaptor for extracting aggregated hive results.
    public class ResultsPlotting
    {
        private Results _results;

        public ResultsPlotting(Results results)
        {
            _results = results;
        }

        public double TotalFloorArea => 180;

        #region Emissions
        public double EmbodiedEmissionsBuildings(bool normalized) => normalized ? 100 / TotalFloorArea : 100;
        public double EmbodiedEmissionsSystems(bool normalized) => normalized ? 200 / TotalFloorArea : 200;
        public double OperationEmissionsBuildings(bool normalized) => normalized? 300 / TotalFloorArea: 300;
        public double OperationEmissionsSystems(bool normalized) => normalized? 400 / TotalFloorArea: 400;
        #endregion Emissions

        #region Costs
        public double EmbodiedCostsBuildings(bool normalized) => normalized? 110 / TotalFloorArea: 110;
        public double EmbodiedCostsSystems(bool normalized) => normalized? 220 / TotalFloorArea: 220;
        public double OperationCostsBuildings(bool normalized) => normalized? 330/TotalFloorArea: 330;
        public double OperationCostsSystems(bool normalized) =>normalized? 440/TotalFloorArea: 440;
        #endregion Costs

        #region Energy
        public double EmbodiedEnergyBuildings(bool normalized) =>normalized? 130/TotalFloorArea: 130;
        public double EmbodiedEnergySystems(bool normalized) => normalized? 230/TotalFloorArea: 230;
        public double OperationEnergyBuildings(bool normalized) => normalized? 330/TotalFloorArea: 330;
        public double OperationEnergySystems(bool normalized) => normalized ? 440 / TotalFloorArea : 440;
        #endregion Energy
    }
}
