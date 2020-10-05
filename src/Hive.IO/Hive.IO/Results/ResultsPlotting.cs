using System.Collections.Generic;
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

        #region Demand Monthly
        public double[] TotalHeatingMonthly => Results.TotalFinalHeatingMonthly ?? new double[Misc.MonthsPerYear];
        public double[] TotalCoolingMonthly => Results.TotalFinalCoolingMonthly ?? new double[Misc.MonthsPerYear];
        public double[] TotalElectricityMonthly => Results.TotalFinalElectricityMonthly ?? new double[Misc.MonthsPerYear];
        public double[] TotalDomesticHotWaterMonthly => Results.TotalFinalDomesticHotWaterMonthly ?? new double[Misc.MonthsPerYear];
        #endregion

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

        // ingoing energy
        public float SolarGains => (float)Results.TotalSolarGains;
        public float InternalGains => (float)Results.TotalInternalGains;
        public float PrimaryEnergy => (float)Results.TotalPrimaryEnergyMonthlyNonRenewable.Sum(); //inputCarriers from conversionTech, except renewable tech (solar). 
        public float RenewableEnergy => (float)Results.TotalFinalEnergyMonthlyRenewable.Sum(); // solar tech
        public float VentilationGains => (float) Results.TotalVentilationHeatGains;   
        public float EnvelopeGains => (float) Results.TotalOpaqueTransmissionHeatGains; 
        public float WindowsGains => (float) Results.TotalWindowTransmissionHeatGains; 


        // outgoing energy
        public float Electricity => (float)Results.TotalConsumedElectricityMonthly.Sum(); // consumed electricity. it is not electricity loads, which could become negative with e.g. pv electricity
        public float VentilationLosses => (float)Results.TotalVentilationHeatLosses;
        public float EnvelopeLosses => (float)Results.TotalOpaqueTransmissionHeatLosses;
        public float WindowsLosses => (float)Results.TotalWindowTransmissionHeatLosses;

        public float SystemLosses => (float)Results.TotalSystemLosses; // system losses only from fuel based systems. Heatpump electricity is accounted to Electricity (out)
        public float ActiveCooling => (float)Results.TotalActiveCoolingMonthly.Sum();           // new arrow for active cooling @Daren
        public float SurplusElectricity => (float)Results.TotalFeedInElectricityMonthly.Sum();      // new arrow for surplus electricity @Daren
        public float SurplusHeating => (float)Results.TotalSurplusHeatingMonthly.Sum();          // new arrow for surplus heating energy @Daren

        public float PrimaryTransferLosses => 0f;   // deactivate for now @Daren


        #endregion EnergyBalance

        #region Irradiation

        public List<double[]> IrradiationOnWindows => new List<double[]>
        {
            new double[]{29, 49, 88, 124, 153, 166, 168, 143, 99, 58, 30, 22},
            new double[]{19, 39, 78, 114, 143, 156, 158, 133, 89, 48, 20, 13},
            new double[]{15, 24, 63, 99, 128, 141, 143, 118, 74, 33, 15, 14}
        };

        #endregion Irradiation
    }
}
