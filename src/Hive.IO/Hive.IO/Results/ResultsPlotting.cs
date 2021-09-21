using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Windows.Forms;
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
            => normalized
                ? new double[Misc.MonthsPerYear].Select(r => this.Results.TotalEmissionsEmbodiedConstructionMonthlyLevelised / TotalFloorArea).ToArray()
                : new double[Misc.MonthsPerYear].Select(r => this.Results.TotalEmissionsEmbodiedConstructionMonthlyLevelised).ToArray();

        public double EmbodiedEmissionsBuildingsYearly(bool normalized)
            => normalized
                ? this.Results.TotalEmissionsEmbodiedConstructionYearlyLevelised / TotalFloorArea
                : this.Results.TotalEmissionsEmbodiedConstructionYearlyLevelised;

        public double EmbodiedEmissionsBuildings(bool normalized)
            => normalized
                ? this.Results.TotalEmissionsEmbodiedConstruction / TotalFloorArea
                : this.Results.TotalEmissionsEmbodiedConstruction;

        public double[] EmbodiedEmissionsSystemsMonthly(bool normalized)
            => normalized
                ? new double[Misc.MonthsPerYear].Select(r => this.Results.TotalEmissionsEmbodiedSystemsMonthlyLevelised / TotalFloorArea).ToArray()
                : new double[Misc.MonthsPerYear].Select(r => this.Results.TotalEmissionsEmbodiedSystemsMonthlyLevelised).ToArray();

        public double EmbodiedEmissionsSystemsYearly(bool normalized)
            => normalized
                ? this.Results.TotalEmissionsEmbodiedSystemsYearlyLevelised / TotalFloorArea
                : this.Results.TotalEmissionsEmbodiedSystemsYearlyLevelised;

        public double EmbodiedEmissionsSystems(bool normalized)
            => normalized
                ? this.Results.TotalEmissionsEmbodiedSystems / TotalFloorArea
                : this.Results.TotalEmissionsEmbodiedSystems;

        public double[] OperationEmissionsBuildingsMonthly(bool normalized)
           => normalized
                ? new double[Misc.MonthsPerYear].Select(r => this.Results.TotalEmissionsOperationalConstructionMonthlyLevelised / TotalFloorArea).ToArray()
                : new double[Misc.MonthsPerYear].Select(r => this.Results.TotalEmissionsOperationalConstructionMonthlyLevelised).ToArray();

        public double OperationEmissionsBuildingsYearly(bool normalized)
            => normalized
                ? this.Results.TotalEmissionsOperationalConstructionYearlyLevelised / TotalFloorArea
                : this.Results.TotalEmissionsOperationalConstructionYearlyLevelised;

        public double OperationEmissionsBuildings(bool normalized) 
            => normalized
                ? this.Results.TotalEmissionsOperationalConstruction / TotalFloorArea
                : this.Results.TotalEmissionsOperationalConstruction;

        public double[] OperationEmissionsSystemsMonthly(bool normalized)
            => normalized
                ? this.Results.TotalEmissionsOperationalSystemsMonthly.Select(r=> r / TotalFloorArea).ToArray()
                : this.Results.TotalEmissionsOperationalSystemsMonthly;

        public double OperationEmissionsSystemsYearly(bool normalized)
            => normalized
                ? this.Results.TotalEmissionsOperationalSystemsYearly / TotalFloorArea
                : this.Results.TotalEmissionsOperationalSystemsYearly;

        public double OperationEmissionsSystems(bool normalized) 
            => normalized
                ? this.Results.TotalEmissionsOperationalSystems / TotalFloorArea
                : this.Results.TotalEmissionsOperationalSystems;

        public double TotalEmbodiedEmissions(bool normalized) 
            => normalized
                ? this.Results.TotalEmissionsEmbodied / TotalFloorArea
                : this.Results.TotalEmissionsEmbodied;

        public double TotalOperationEmissions(bool normalized)
            => normalized
                ? this.Results.TotalEmissionsOperational / TotalFloorArea
                : this.Results.TotalEmissionsOperational;

        public double TotalEmissions(bool normalized)
            => normalized
                ? this.Results.TotalEmissions / TotalFloorArea
                : this.Results.TotalEmissions;

        #endregion Emissions

        #region Costs

        public double[] EmbodiedCostsBuildingsMonthly(bool normalized) 
            => normalized
                ? new double[Misc.MonthsPerYear].Select(r => this.Results.TotalCostEmbodiedConstructionMonthlyLevelised / TotalFloorArea).ToArray()
                : new double[Misc.MonthsPerYear].Select(r => this.Results.TotalCostEmbodiedConstructionMonthlyLevelised).ToArray();

        public double EmbodiedCostsBuildingsYearly(bool normalized)
            => normalized
            ? this.Results.TotalCostEmbodiedConstructionYearlyLevelised / TotalFloorArea
            : this.Results.TotalCostEmbodiedConstructionYearlyLevelised;

        public double EmbodiedCostsBuildings(bool normalized)
            => normalized
            ? this.Results.TotalCostEmbodiedConstruction / TotalFloorArea
            : this.Results.TotalCostEmbodiedConstruction;

        public double[] EmbodiedCostsSystemsMonthly(bool normalized)
            => normalized
                ? new double[Misc.MonthsPerYear].Select(r => this.Results.TotalCostEmbodiedSystemsMonthlyLevelized / TotalFloorArea).ToArray()
                : new double[Misc.MonthsPerYear].Select(r => this.Results.TotalCostEmbodiedSystemsMonthlyLevelized).ToArray();

        public double EmbodiedCostsSystemsYearly(bool normalized)
            => normalized
            ? this.Results.TotalCostEmbodiedSystemsYearlyLevelised / TotalFloorArea
            : this.Results.TotalCostEmbodiedSystemsYearlyLevelised;

        public double EmbodiedCostsSystems(bool normalized) 
            => normalized
            ? this.Results.TotalCostEmbodiedSystems / TotalFloorArea
            : this.Results.TotalCostEmbodiedSystems;

        public double[] OperationCostsBuildingsMonthly(bool normalized)
            => normalized
                ? new double[Misc.MonthsPerYear].Select(r => this.Results.TotalCostOperationalConstructionMonthlyLevelized / TotalFloorArea).ToArray()
                : new double[Misc.MonthsPerYear].Select(r => this.Results.TotalCostOperationalConstructionMonthlyLevelized).ToArray();

        public double OperationCostsBuildingsYearly(bool normalized)
            => normalized
            ? this.Results.TotalCostOperationalConstructionYearlyLevelised / TotalFloorArea
            : this.Results.TotalCostOperationalConstructionYearlyLevelised;

        public double OperationCostsBuildings(bool normalized) => this.Results.TotalCostOperationalConstruction;

        public double[] OperationCostsSystemsMonthly(bool normalized)
            => normalized
                ? this.Results.TotalCostOperationalSystemsMonthly.Select(r => r / TotalFloorArea).ToArray()
                : this.Results.TotalCostOperationalSystemsMonthly;

        public double OperationCostsSystemsYearly(bool normalized)
            => normalized
            ? this.Results.TotalCostOperationalSystemsYearly / TotalFloorArea
            : this.Results.TotalCostOperationalSystemsYearly;

        public double OperationCostsSystems(bool normalized) 
            => normalized
            ? this.Results.TotalCostOperationalSystems / TotalFloorArea
            : this.Results.TotalCostOperationalSystems;

        public double TotalEmbodiedCosts(bool normalized) 
            => normalized
            ? this.Results.TotalCostEmbodied / TotalFloorArea
            : this.Results.TotalCostEmbodied;

        public double TotalOperationCosts(bool normalized) 
            => normalized
            ? this.Results.TotalCostOperational / TotalFloorArea
            : this.Results.TotalCostOperational;

        public double TotalCosts(bool normalized) => TotalEmbodiedCosts(normalized) + TotalOperationCosts(normalized);
        #endregion Costs

        #region Energy
        // energy that has been used for construction of materials. like kgCO2eq., but in kWh
        public double[] EmbodiedEnergyBuildingsMonthly(bool normalized)
            // FIXME
            => new double[Misc.MonthsPerYear].Select(r => 0.0).ToArray();
            //=> normalized
            //    ? new double[Misc.MonthsPerYear].Select(r => this.Results.TotalEnergyEmbodiedConstructionMonthlyLevelised / TotalFloorArea).ToArray()
            //    : new double[Misc.MonthsPerYear].Select(r => this.Results.TotalEnergyEmbodiedConstructionMonthlyLevelised).ToArray();

        // FIXME
        public double EmbodiedEnergyBuildingsYearly(bool normalized) => EmbodiedEnergyBuildingsMonthly(normalized).Sum();


        public double EmbodiedEnergyBuildings(bool normalized) => EmbodiedEnergyBuildingsMonthly(normalized).Sum();

        // embodied energy that has been used for construction of systems. like kgCO2eq., but in kWh
        public double[] EmbodiedEnergySystemsMonthly(bool normalized)
            // FIXME
            => new double[Misc.MonthsPerYear].Select(r => 0.0).ToArray();
            //=> normalized
            //    ? new double[Misc.MonthsPerYear].Select(r => this.Results.TotalEnergyEmbodiedSystemsMonthlyLevelised / TotalFloorArea).ToArray()
            //    : new double[Misc.MonthsPerYear].Select(r => this.Results.TotalEnergyEmbodiedSystemsMonthlyLevelised).ToArray();

        // FIXME
        public double EmbodiedEnergySystemsYearly(bool normalized) => EmbodiedEnergySystemsMonthly(normalized).Sum();

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

        public double OperationEnergyBuildingsYearly(bool normalized) => OperationEnergyBuildingsMonthly(normalized).Sum();

        public double OperationEnergyBuildings(bool normalized) => OperationEnergyBuildingsMonthly(normalized).Sum();

        // Primary energy demand, incl. conversion losses
        public double[] OperationEnergySystemsMonthly(bool normalized)
        {
            double[] result = Results.TotalPrimaryEnergyNonRenewableMonthly.ToArray();
            return normalized ? result.Select(r => r / TotalFloorArea).ToArray() : result;
        }

        // FIXME
        public double OperationEnergySystemsYearly(bool normalized) => 0.0;
        
        public double OperationEnergySystems(bool normalized) => OperationEnergySystemsMonthly(normalized).Sum();


        // FIX ME what is embodied energy?
        public double TotalEmbodiedEnergy(bool normalized) =>
            EmbodiedEnergyBuildings(normalized) + EmbodiedEnergySystems(normalized);

        // FIX ME wrong addition. final energy is subset of primary energy
        public double TotalOperationEnergy(bool normalized) =>
            OperationEnergyBuildings(normalized) + OperationEnergySystems(normalized);

        /// <summary>
        /// The total energy of the building (embodied + operational).
        /// </summary>
        /// <param name="normalized"></param>
        /// <returns></returns>
        public double TotalEnergy(bool normalized) =>
            TotalEmbodiedEnergy(normalized) + TotalOperationEnergy(normalized);

        #endregion Energy

        #region EnergyBalance

        // ingoing energy
        public float SolarGains => (float)Results.TotalSolarGains;
        public float InternalGains => (float)Results.TotalInternalGains;
        public float PrimaryEnergy => (float)Results.TotalPrimaryEnergyNonRenewableMonthly.Sum(); //inputCarriers from conversionTech, except renewable tech (solar). 
        public float RenewableEnergy => (float)Results.TotalFinalEnergyRenewableMonthly.Sum(); // solar tech
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

        public List<double[]> IrradiationOnWindows => Results.MonthlySolarGainsPerWindow;

        #endregion Irradiation
    }
}
