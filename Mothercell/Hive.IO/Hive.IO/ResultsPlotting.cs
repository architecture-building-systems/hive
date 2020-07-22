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
        public double EmbodiedEmissionsBuildings => 100;
        public double EmbodiedEmissionsSystems => 200;
        public double OperationEmissionsBuildings => 300;
        public double OperationEmissionsSystems => 400;
        #endregion Emissions

        #region Costs
        public double EmbodiedCostsBuildings => 110;
        public double EmbodiedCostsSystems => 220;
        public double OperationCostsBuildings => 330;
        public double OperationCostsSystems => 440;
        #endregion Costs

        #region Energy
        public double EmbodiedEnergyBuildings => 110;
        public double EmbodiedEnergySystems => 220;
        public double OperationEnergyBuildings => 330;
        public double OperationEnergySystems => 440;
        #endregion Energy
    }
}
