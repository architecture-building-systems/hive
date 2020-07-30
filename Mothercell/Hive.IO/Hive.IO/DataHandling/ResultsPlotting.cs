using System.Linq;

namespace Hive.IO.DataHandling
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

        public double[] EmbodiedEmissionsBuildingsMonthly(bool normalized)
        {
            // FIXME: plug in real values here...
            double dummy = 100.0;
            double[] result = new double[12].Select(r => dummy).ToArray();
            return normalized ? result.Select(r => r / TotalFloorArea).ToArray() : result;
        }
        public double EmbodiedEmissionsBuildings(bool normalized) => EmbodiedEmissionsBuildingsMonthly(normalized).Sum();
        public double[] EmbodiedEmissionsSystemsMonthly(bool normalized)
        {
            // FIXME: plug in real values here...
            double dummy = 200.0;
            double[] result = new double[12].Select(r => dummy).ToArray();
            return normalized ? result.Select(r => r / TotalFloorArea).ToArray() : result;
        }
        public double EmbodiedEmissionsSystems(bool normalized) => EmbodiedEmissionsSystemsMonthly(normalized).Sum();

        public double[] OperationEmissionsBuildingsMonthly(bool normalized)
        {
            // FIXME: plug in real values here...
            double dummy = 300.0;
            double[] result = new double[12].Select(r => dummy).ToArray();
            return normalized ? result.Select(r => r / TotalFloorArea).ToArray() : result;

        }
        public double OperationEmissionsBuildings(bool normalized) => OperationEmissionsBuildingsMonthly(normalized).Sum();

        public double[] OperationEmissionsSystemsMonthly(bool normalized)
        {
            // FIXME: plug in real values here...
            double dummy = 400.0;
            double[] result = new double[12].Select(r => dummy).ToArray();
            return normalized ? result.Select(r => r / TotalFloorArea).ToArray() : result;
        }
        public double OperationEmissionsSystems(bool normalized) => OperationEmissionsSystemsMonthly(normalized).Sum();

        public double TotalEmbodiedEmissions(bool normalized) =>
            EmbodiedEmissionsBuildings(normalized) + EmbodiedEmissionsSystems(normalized);

        public double TotalOperationEmissions(bool normalized) =>
            OperationEmissionsBuildings(normalized) + OperationEmissionsSystems(normalized);

        public double TotalEmissions(bool normalized) =>
            TotalEmbodiedEmissions(normalized) + TotalOperationEmissions(normalized);
        #endregion Emissions

        #region Costs
        public double[] EmbodiedCostsBuildingsMonthly(bool normalized)
        {
            // FIXME: plug in real values here...
            double dummy = 110.0;
            double[] result = new double[12].Select(r => dummy).ToArray();
            return normalized ? result.Select(r => r / TotalFloorArea).ToArray() : result;
        }

        public double EmbodiedCostsBuildings(bool normalized) => EmbodiedCostsBuildingsMonthly(normalized).Sum();

        public double[] EmbodiedCostsSystemsMonthly(bool normalized)
        {
            // FIXME: plug in real values here...
            double dummy = 220.0;
            double[] result = new double[12].Select(r => dummy).ToArray();
            return normalized ? result.Select(r => r / TotalFloorArea).ToArray() : result;
        }
        public double EmbodiedCostsSystems(bool normalized) => EmbodiedCostsSystemsMonthly(normalized).Sum();

        public double[] OperationCostsBuildingsMonthly(bool normalized)
        {
            // FIXME: plug in real values here...
            double dummy = 330.0;
            double[] result = new double[12].Select(r => dummy).ToArray();
            return normalized ? result.Select(r => r / TotalFloorArea).ToArray() : result;

        }
        public double OperationCostsBuildings(bool normalized) => OperationCostsBuildingsMonthly(normalized).Sum();
        public double[] OperationCostsSystemsMonthly(bool normalized)
        {
            // FIXME: plug in real values here...
            double dummy = 440.0;
            double[] result = new double[12].Select(r => dummy).ToArray();
            return normalized ? result.Select(r => r / TotalFloorArea).ToArray() : result;

        }

        public double OperationCostsSystems(bool normalized) => OperationCostsSystemsMonthly(normalized).Sum();

        public double TotalEmbodiedCosts(bool normalized) =>
            EmbodiedCostsBuildings(normalized) + EmbodiedCostsSystems(normalized);

        public double TotalOperationCosts(bool normalized) =>
            OperationCostsBuildings(normalized) + OperationCostsSystems(normalized);

        public double TotalCosts(bool normalized) => TotalEmbodiedCosts(normalized) + TotalOperationCosts(normalized);
        #endregion Costs

        #region Energy
        public double[] EmbodiedEnergyBuildingsMonthly(bool normalized)
        {
            // FIXME: plug in real values here...
            double dummy = 123.0;
            double[] result = new double[12].Select(r => dummy).ToArray();
            return normalized ? result.Select(r => r / TotalFloorArea).ToArray() : result;
        }

        public double EmbodiedEnergyBuildings(bool normalized) => EmbodiedEnergyBuildingsMonthly(normalized).Sum();
        public double[] EmbodiedEnergySystemsMonthly(bool normalized)
        {
            // FIXME: plug in real values here...
            double dummy = 234.0;
            double[] result = new double[12].Select(r => dummy).ToArray();
            return normalized ? result.Select(r => r / TotalFloorArea).ToArray() : result;
        }

        public double EmbodiedEnergySystems(bool normalized) => EmbodiedEnergySystemsMonthly(normalized).Sum();
        public double[] OperationEnergyBuildingsMonthly(bool normalized)
        {
            // FIXME: plug in real values here...
            double dummy = 345.0;
            double[] result = new double[12].Select(r => dummy).ToArray();
            return normalized ? result.Select(r => r / TotalFloorArea).ToArray() : result;
        }

        public double OperationEnergyBuildings(bool normalized) => OperationEnergyBuildingsMonthly(normalized).Sum();
        public double[] OperationEnergySystemsMonthly(bool normalized)
        {
            // FIXME: plug in real values here...
            double dummy = 456.0;
            double[] result = new double[12].Select(r => dummy).ToArray();
            return normalized ? result.Select(r => r / TotalFloorArea).ToArray() : result;
        }

        public double OperationEnergySystems(bool normalized) => OperationEnergySystemsMonthly(normalized).Sum();

        public double TotalEmbodiedEnergy(bool normalized) =>
            EmbodiedEnergyBuildings(normalized) + EmbodiedEnergySystems(normalized);

        public double TotalOperationEnergy(bool normalized) =>
            OperationEnergyBuildings(normalized) + OperationEnergySystems(normalized);

        public double TotalEnergy(bool normalized) =>
            TotalEmbodiedEnergy(normalized) + TotalOperationEnergy(normalized);

        #endregion Energy
    }
}
