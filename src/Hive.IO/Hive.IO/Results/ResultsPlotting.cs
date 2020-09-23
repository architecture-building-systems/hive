using System.Linq;
using System.Security.Permissions;
using System.Windows.Forms.VisualStyles;
using Grasshopper.Kernel.Geometry.SpatialTrees;

namespace Hive.IO.Results
{
    // An adaptor for extracting aggregated hive results.
    public class ResultsPlotting
    {
        public Results Results { get; }

        public ResultsPlotting(Results results)
        {
            Results = results;
        }

        public double TotalFloorArea => Results.TotalFloorArea;

        #region Emissions

        public double[] EmbodiedEmissionsBuildingsMonthly(bool normalized)
        {
            double totalEmissions = this.Results.TotalEmbodiedConstructionEmissions;
            double[] result = new double[Misc.MonthsPerYear].Select(r => totalEmissions / Misc.MonthsPerYear).ToArray();
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
        // energy that has been used for construction of materials. like kgCO2eq., but in kWh
        public double[] EmbodiedEnergyBuildingsMonthly(bool normalized)
        {
            // FIXME: plug in real values here...
            double dummy = 0.0;
            double[] result = new double[12].Select(r => dummy).ToArray();
            return normalized ? result.Select(r => r / TotalFloorArea).ToArray() : result;
        }
        public double EmbodiedEnergyBuildings(bool normalized) => EmbodiedEnergyBuildingsMonthly(normalized).Sum();

        // embodied energy that has been used for construction of systems. like kgCO2eq., but in kWh
        public double[] EmbodiedEnergySystemsMonthly(bool normalized)
        {
            double[] result = Results.TotalOperationalEmissionsMonthly;
            return normalized ? result.Select(r => r / TotalFloorArea).ToArray() : result;
        }
        public double EmbodiedEnergySystems(bool normalized) => EmbodiedEnergySystemsMonthly(normalized).Sum();

        // Ideal demands, a.k.a. final energy demand
        public double[] OperationEnergyBuildingsMonthly(bool normalized)
        {
            //double[] result = Results.TotalFinalCoolingMonthly
            //    .Select((x, index) => x + Results.TotalFinalDomesticHotWaterMonthly[index])
            //    .Select((x, index) => x + Results.TotalFinalElectricityMonthly[index])
            //    .Select((x, index) => x + Results.TotalFinalHeatingMonthly[index])
            //    .ToArray();

            double [] result = new double[Misc.MonthsPerYear];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = Results.TotalFinalCoolingMonthly[i] 
                            + Results.TotalFinalDomesticHotWaterMonthly[i]
                            + Results.TotalFinalElectricityMonthly[i] 
                            + Results.TotalFinalHeatingMonthly[i];
            }

            return normalized ? result.Select(r => r / TotalFloorArea).ToArray() : result;
        }
        public double OperationEnergyBuildings(bool normalized) => OperationEnergyBuildingsMonthly(normalized).Sum();

        // Primary energy demand, incl. conversion losses
        public double[] OperationEnergySystemsMonthly(bool normalized)
        {
            double[] result = Results.TotalPrimaryEnergyMonthlyNonRenewable.ToArray();
            return normalized ? result.Select(r => r / TotalFloorArea).ToArray() : result;
        }
        public double OperationEnergySystems(bool normalized) => OperationEnergySystemsMonthly(normalized).Sum();

        // FIX ME what is embodied energy?
        public double TotalEmbodiedEnergy(bool normalized) =>
            EmbodiedEnergyBuildings(normalized) + EmbodiedEnergySystems(normalized);

        // FIX ME wrong addition. final energy is subset of primary energy
        public double TotalOperationEnergy(bool normalized) =>
            OperationEnergyBuildings(normalized) + OperationEnergySystems(normalized);

        // FIX ME what is this?
        public double TotalEnergy(bool normalized) =>
            TotalEmbodiedEnergy(normalized) + TotalOperationEnergy(normalized);

        #endregion Energy

        #region EnergyBalance

        // bug: with PV energy, following numbers become negative:
        // - (IN) TotalPrimaryEnergyMonthly
        // - (OUT) TotalFinalElectricityMonthly
        // - (OUT) TotalSystemLosses

        // ingoing energy
        public float SolarGains => (float)Results.TotalSolarGains;
        public float InternalGains => (float)Results.TotalInternalGains;
        public float PrimaryEnergy => (float)Results.TotalPrimaryEnergyMonthlyNonRenewable.Sum(); //inputCarriers from conversionTech, except renewable tech (solar). What to do with Electricity Grid? How to account for negative numbers
        public float RenewableEnergy => (float)Results.TotalFinalEnergyMonthlyRenewable.Sum(); 
        
        // outgoing energy
        public float Electricity => (float)Results.TotalFinalElectricityMonthly.Sum(); // without feedin!!!! TO FIX
        public float VentilationLosses => (float)Results.TotalVentilationHeatLosses;
        public float EnvelopeLosses => (float)Results.TotalOpaqueTransmissionHeatLosses;
        public float WindowsLosses => (float)Results.TotalWindowTransmissionHeatLosses;
        public float SystemLosses => (float)Results.TotalSystemLosses; 
        public float PrimaryTransferLosses => 0f; // ignore?


        #endregion EnergyBalance
    }
}
