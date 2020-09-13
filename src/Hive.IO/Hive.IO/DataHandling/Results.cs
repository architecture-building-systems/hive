using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Hive.IO.Building;
using Hive.IO.EnergySystems;
using rg = Rhino.Geometry;

namespace Hive.IO.DataHandling
{
    /// <summary>
    /// Results class that contains all kinds of building simulation results
    /// - demand
    /// - solar potentials
    /// - OPEX CAPEX
    /// - carbon emissions
    /// - etc
    /// Also includes rhino geometry that represent e.g. solar potentials on buildings
    /// </summary>
    public class Results
    {



        #region General

        public double InterestRate { get; private set; }

        // implemented in DataHandling.ResultsPlotting
        public double TotalFloorArea { get; private set; }

        #endregion


        #region Total Loads

        // implemented in Plots.DemandMonthlyPlot
        public double[] TotalFinalElectricityMonthly { get; private set; }
        public double[] TotalFinalCoolingMonthly { get; private set; }
        public double[] TotalFinalHeatingMonthly { get; private set; }
        public double[] TotalFinalDomesticHotWaterMonthly { get; private set; }

        public double [] TotalPrimaryEnergyMonthly { get; private set; }


        //public double[] TotalFinalElectricityHourly { get; private set; }
        //public double[] TotalFinalCoolingHourly { get; private set; }
        //public double[] TotalFinalHeatingHourly { get; private set; }
        //public double[] TotalFinalDomesticHotWaterHourly { get; private set; }
        #endregion


        #region Total Emissions

        public double TotalEmbodiedConstructionEmissions { get; private set; }
        public double [] TotalOperationalEmissionsMonthly { get; private set; }

        #endregion


        #region Total Losses and Gains
        public double TotalOpaqueTransmissionHeatLosses { get; private set; }
        public double TotalWindowTransmissionHeatLosses { get; private set; }
        public double TotalVentilationHeatLosses { get; private set; }
        public double TotalInternalGains { get; private set; }
        public double TotalSolarGains { get; private set; }

        public double TotalSystemLosses { get; private set; }
        #endregion


        #region Zone-wise loads
        public double[][] ZoneElectricityMonthly { get; private set; }
        public double[][] ZoneElectricityHourly { get; private set; }
        public double[][] ZoneCoolingMonthly { get; private set; }
        public double[][] ZoneCoolingHourly { get; private set; }
        public double[][] ZoneHeatingMonthly { get; private set; }
        public double[][] ZoneHeatingHourly { get; private set; }
        public double[][] ZoneDHWMonthly { get; private set; }
        public double[][] ZoneDHWHourly { get; private set; }
        #endregion


        #region Energy Supply Systems
        /// <summary>
        /// Names for each supply technology, e.g. "Boiler", "CHP", ...
        /// </summary>
        public string[] SupplyNames { get; private set; }
        /// <summary>
        /// Matrix defining the suitability of each technology per carrier type.
        /// Rows: Technologies
        /// Columns: [0]: Electricity, [1]: Heating, [2]: Cooling
        /// </summary>
        public bool[,] SupplyTypes { get; private set; }
        /// <summary>
        /// Capacity per technology. Unit is defined in 'SupplyCapUnits
        /// </summary>
        public double[] SupplyCapacities { get; private set; }
        /// <summary>
        /// Defining Capacity unit per technology. E.g. m2, kW, kWh, ...
        /// </summary>
        public string[] SupplyCapUnits { get; private set; }
        /// <summary>
        /// Total investment cost per technology
        /// </summary>
        public double[] SupplyCapex { get; private set; }
        /// <summary>
        /// Levelized investment cost per technology (considering their lifetime and interest rate)
        /// </summary>
        public double[] SupplyCapexLev { get; private set; }
        /// <summary>
        /// Total levelized operation expenditures per supply technology
        /// </summary>
        public double[] SupplyOpexLev { get; private set; }
        /// <summary>
        /// Time-resolved (hourly for a year) OPEX per technology
        /// </summary>
        public double[][] SupplyOpexHourly { get; private set; }
        /// <summary>
        /// Operation schedule per technology and hour. Unit defined in 'SupplyOperationUnit'
        /// </summary>
        public double[][] SupplyOperationHourly { get; private set; }
        /// <summary>
        /// Operation schedule per technology and month. Unit defined in 'SupplyOperationUnit'
        /// </summary>
        public double[][] SupplyOperationMonthly { get; private set; }
        /// <summary>
        /// Defining unit of operation per technology, e.g. "kWh", "Wh", ...
        /// </summary>
        public string[] SupplyOperationUnit { get; private set; }
        #endregion


        #region Geometry
        public rg.Mesh SkyViewFactors { get; private set; }
        public rg.Curve[] SkySunPath { get; private set; }
        public rg.Mesh[] IrradiationSurfaces { get; private set; }
        #endregion


        #region Hive Core Objects

        /// <summary>
        /// Building object that contains geometric, construction, energy demand, cost (not operational), and LCA (not operational) information 
        /// </summary>
        public Building.Building Building { get; private set; }
        /// <summary>
        /// Energy conversion technologies (e.g. boiler, PV, heatpump, etc.). Include operation schedules, operational LCA, embodied LCA of technologies, and operational and investment cost of technologies
        /// </summary>
        public List<ConversionTech> ConversionTechnologies { get; private set; }
        /// <summary>
        /// Heat and Cooling emitters of the building
        /// </summary>
        public List<Emitter> Emitters { get; private set; }
        /// <summary>
        /// Final output energy streams to meet building energy demands
        /// </summary>
        public List<Carrier> OutputEnergyStreams { get; private set; }
        /// <summary>
        /// Initial input energy streams into the system. That might include Grid Electricity, Solar Potentials, District Heating, ...
        /// </summary>
        public List<Carrier> InputEnergyStreams { get; private set; }

        #endregion


        /// <summary>
        /// empty constructor
        /// </summary>
        public Results()
        {
            this.TotalFloorArea = 0.0;
            this.TotalEmbodiedConstructionEmissions = 0.0;

            this.TotalFinalHeatingMonthly = new double[Misc.MonthsPerYear];
            this.TotalFinalDomesticHotWaterMonthly = new double[Misc.MonthsPerYear];
            this.TotalFinalCoolingMonthly = new double[Misc.MonthsPerYear];
            this.TotalFinalElectricityMonthly = new double[Misc.MonthsPerYear];

            this.TotalPrimaryEnergyMonthly = new double[Misc.MonthsPerYear];
            this.TotalOperationalEmissionsMonthly = new double[Misc.MonthsPerYear];

            //this.TotalFinalCoolingHourly = new double[Misc.HoursPerYear];
            //this.TotalFinalHeatingHourly = new double[Misc.HoursPerYear];
            //this.TotalFinalElectricityHourly = new double[Misc.HoursPerYear];
            //this.TotalFinalDomesticHotWaterHourly = new double[Misc.HoursPerYear];

            this.SupplyNames = null;
            this.SupplyTypes = null;
            this.SupplyCapacities = null;
            this.SupplyCapUnits = null;

            this.SkyViewFactors = null;
            this.SkySunPath = null;
            this.IrradiationSurfaces = null;
        }


        /// <summary>
        /// proper constructor
        /// </summary>
        /// <param name="building"></param>
        /// <param name="conversionTech"></param>
        /// <param name="emitters"></param>
        /// <param name="outputEnergy"></param>
        /// <param name="inputEnergy"></param>
        public Results(Building.Building building, List<ConversionTech> conversionTech, List<Emitter> emitters, List<Carrier> outputEnergy, List<Carrier> inputEnergy)
        {
            this.TotalFloorArea = GetTotalFloorArea(building);
            this.TotalEmbodiedConstructionEmissions = GetTotalEmbodiedConstructionEmissions(building);

            this.TotalFinalHeatingMonthly = GetTotalMonthlyFinalLoads(building, "heating");
            this.TotalFinalDomesticHotWaterMonthly = GetTotalMonthlyFinalLoads(building, "dhw");
            this.TotalFinalCoolingMonthly = GetTotalMonthlyFinalLoads(building, "cooling");
            this.TotalFinalElectricityMonthly = GetTotalMonthlyFinalLoads(building, "electricity");

            this.TotalPrimaryEnergyMonthly = GetTotalMonthlyPrimaryLoads(conversionTech);
            this.TotalOperationalEmissionsMonthly = GetTotalMonthlyOperationalEmissions(conversionTech);


            //this.TotalFinalCoolingHourly = new double[Misc.HoursPerYear];
            //this.TotalFinalHeatingHourly = new double[Misc.HoursPerYear];
            //this.TotalFinalElectricityHourly = new double[Misc.HoursPerYear];
            //this.TotalFinalDomesticHotWaterHourly = new double[Misc.HoursPerYear];

            this.TotalOpaqueTransmissionHeatLosses = GetTotalGainsOrLosses(building, "Qt_opaque");
            this.TotalWindowTransmissionHeatLosses = GetTotalGainsOrLosses(building, "Qt_transparent");
            this.TotalVentilationHeatLosses = GetTotalGainsOrLosses(building, "Qv");
            this.TotalInternalGains = GetTotalGainsOrLosses(building, "Qi");
            this.TotalSolarGains = GetTotalGainsOrLosses(building, "Qs");
            this.TotalSystemLosses = GetTotalMonthlySystemLosses(this, this.TotalPrimaryEnergyMonthly);

            this.SupplyNames = null;
            this.SupplyTypes = null;
            this.SupplyCapacities = null;
            this.SupplyCapUnits = null;

            this.SkyViewFactors = null;
            this.SkySunPath = null;
            this.IrradiationSurfaces = null;

            this.Building = building;
            this.ConversionTechnologies = conversionTech;
            this.Emitters = emitters;
            this.OutputEnergyStreams = outputEnergy;
            this.InputEnergyStreams = inputEnergy;
        }


        #region Setters
        /// <summary>
        /// Sets the 3D sky dome around the building
        /// </summary>
        /// <param name="viewfactors"></param>
        /// <param name="sunPath"></param>
        public void SetSkyDome(rg.Mesh viewfactors, rg.Curve[] sunPath)
        {
            if (viewfactors != null) this.SkyViewFactors = viewfactors.DuplicateMesh();

            if (sunPath.Length > 0 && sunPath != null)
            {
                this.SkySunPath = new rg.Curve[sunPath.Length];
                sunPath.CopyTo(this.SkySunPath, 0);
            }
        }


        /// <summary>
        /// Sets meshes with coloured vertices representing irradiation
        /// </summary>
        /// <param name="irradiationMesh"></param>
        public void SetIrradiationMesh(rg.Mesh[] irradiationMesh)
        {
            if (irradiationMesh.Length <= 0 || irradiationMesh == null) return;

            this.IrradiationSurfaces = new rg.Mesh[irradiationMesh.Length];
            irradiationMesh.CopyTo(this.IrradiationSurfaces, 0);
        }


        /// <summary>
        /// Sets the capacity of all supply systems
        /// </summary>
        /// <param name="supplyTechNames"></param>
        /// <param name="supplyTypes"></param>
        /// <param name="supplyCapacities"></param>
        /// <param name="supplyCapUnits"></param>
        public void SetSupplySystemsCapacity(string[] supplyTechNames, bool[,] supplyTypes, double[] supplyCapacities, string[] supplyCapUnits)
        {
            // should contain checks, like lengths of arrays match etc
            if (supplyTechNames == null || supplyTypes == null || supplyCapacities == null || supplyCapUnits == null)
                return;

            var shortestLength = int.MaxValue;
            if (supplyTechNames.Length < shortestLength) shortestLength = supplyTechNames.Length;
            if (supplyCapacities.Length < shortestLength) shortestLength = supplyCapacities.Length;
            if (supplyCapUnits.Length < shortestLength) shortestLength = supplyCapUnits.Length;
            if (supplyTypes.GetLength(0) < shortestLength) shortestLength = supplyTypes.GetLength(0);

            this.SupplyNames = new string[supplyTechNames.Length];
            supplyTechNames.CopyTo(this.SupplyNames, 0);

            this.SupplyTypes = supplyTypes.Clone() as bool[,];
            /// Rows: Technologies
            /// Columns: [0]: Electricity, [1]: Heating, [2]: Cooling

            this.SupplyCapacities = new double[supplyCapacities.Length];
            supplyCapacities.CopyTo(this.SupplyCapacities, 0);

            this.SupplyCapUnits = new string[supplyCapUnits.Length];
            supplyCapUnits.CopyTo(this.SupplyCapUnits, 0);
        }


        /// <summary>
        /// Sets the mpnthly operation schedule for all supply systems
        /// </summary>
        /// <param name="supplyOperationMonthly">first array represents technologies, second array represents schedule</param>
        public void SetSupplySystemsGenerationMonthly(double[][] supplyOperationMonthly)
        {
            if (supplyOperationMonthly.Length <= 0 || supplyOperationMonthly == null) return;

            this.SupplyOperationMonthly = new double[supplyOperationMonthly.Length][];
            for (int i = 0; i < this.SupplyOperationMonthly.Length; i++)
            {
                if (supplyOperationMonthly[i].Length == Misc.MonthsPerYear)
                {
                    this.SupplyOperationMonthly[i] = new double[Misc.MonthsPerYear];
                    supplyOperationMonthly.CopyTo(this.SupplyOperationMonthly, 0);
                }
                else
                    this.SupplyOperationMonthly[i] = null;
            }
        }


        /// <summary>
        /// Sets the hourly generation schedule of all supply systems
        /// </summary>
        /// <param name="supplyOperationHourly">first array represents technologies, second array represents schedule</param>
        public void SetSupplySystemsGenerationHourly(double[][] supplyOperationHourly)
        {

            if (supplyOperationHourly.Length <= 0 || supplyOperationHourly == null) return;

            this.SupplyOperationHourly = new double[supplyOperationHourly.Length][];
            for (int i = 0; i < this.SupplyOperationHourly.Length; i++)
            {
                if (supplyOperationHourly[i].Length == Misc.HoursPerYear)
                {
                    this.SupplyOperationHourly[i] = new double[Misc.HoursPerYear];
                    supplyOperationHourly.CopyTo(this.SupplyOperationHourly, 0);
                }
                else
                    this.SupplyOperationHourly[i] = null;
            }
        }


        /// <summary>
        /// Setting monthly energy demand of the entire building for heating, cooling, electricity, and domestic hot water
        /// </summary>
        /// <param name="coolingDemand"></param>
        /// <param name="heatingDemand"></param>
        /// <param name="electricityDemand"></param>
        /// <param name="dhwDemand"></param>
        public void SetTotalDemandMonthly(double[] coolingDemand, double[] heatingDemand,
            double[] electricityDemand, double[] dhwDemand)
        {
            if (coolingDemand != null && coolingDemand.Length == Misc.MonthsPerYear)
                coolingDemand.CopyTo(this.TotalFinalCoolingMonthly, 0);
            else
                this.TotalFinalCoolingMonthly = null;

            if (heatingDemand != null && heatingDemand.Length == Misc.MonthsPerYear)
                heatingDemand.CopyTo(this.TotalFinalHeatingMonthly, 0);
            else
                this.TotalFinalHeatingMonthly = null;

            if (electricityDemand != null && electricityDemand.Length == Misc.MonthsPerYear)
                electricityDemand.CopyTo(this.TotalFinalElectricityMonthly, 0);
            else
                this.TotalFinalElectricityMonthly = null;

            if (dhwDemand != null && dhwDemand.Length == Misc.MonthsPerYear)
                dhwDemand.CopyTo(this.TotalFinalDomesticHotWaterMonthly, 0);
            else
                this.TotalFinalDomesticHotWaterMonthly = null;
        }


        ///// <summary>
        ///// Setting hourly energy demand of the entire building for heating, cooling, electricity, and domestic hot water
        ///// </summary>
        ///// <param name="coolingDemand"></param>
        ///// <param name="heatingDemand"></param>
        ///// <param name="electricityDemand"></param>
        ///// <param name="dhwDemand"></param>
        //public void SetTotalDemandHourly(double[] coolingDemand, double[] heatingDemand, 
        //    double[] electricityDemand, double [] dhwDemand)
        //{
        //    if (coolingDemand != null && coolingDemand.Length == Misc.HoursPerYear)
        //        coolingDemand.CopyTo(this.TotalFinalCoolingHourly, 0);
        //    else
        //        this.TotalFinalCoolingHourly = null;

        //    if (heatingDemand != null && heatingDemand.Length == Misc.HoursPerYear)
        //        heatingDemand.CopyTo(this.TotalFinalHeatingHourly, 0);
        //    else
        //        this.TotalFinalHeatingHourly = null;

        //    if (electricityDemand != null && electricityDemand.Length == Misc.HoursPerYear)
        //        electricityDemand.CopyTo(this.TotalFinalElectricityHourly, 0);
        //    else
        //        this.TotalFinalElectricityHourly = null;

        //    if (dhwDemand != null && dhwDemand.Length == Misc.HoursPerYear)
        //        dhwDemand.CopyTo(this.TotalFinalDomesticHotWaterHourly, 0);
        //    else
        //        this.TotalFinalDomesticHotWaterHourly = null;
        //}
        #endregion


        #region Getters

        public static double GetTotalFloorArea(Building.Building building)
        {
            double totalFloorArea = 0.0;
            foreach (var zone in building.Zones)
                foreach (var floor in zone.Floors)
                    totalFloorArea += floor.Area;
            return totalFloorArea;
        }

        public static double GetTotalEmbodiedConstructionEmissions(Building.Building building)
        {
            double totalCo2 = 0.0;
            foreach (var zone in building.Zones)
                foreach (var component in zone.SurfaceComponents)
                    totalCo2 += component.TotalCo2;
            return totalCo2;
        }

        public static double[] GetTotalMonthlyFinalLoads(Building.Building building, string loadType)
        {
            double[] totalLoads = new double[Misc.MonthsPerYear];

            foreach (var zone in building.Zones)
            {
                for (int m = 0; m < Misc.MonthsPerYear; m++)
                {
                    switch (loadType)
                    {
                        case "cooling":
                            totalLoads[m] += zone.CoolingLoadsMonthly[m];
                            break;
                        case "heating":
                            totalLoads[m] += zone.HeatingLoadsMonthly[m];
                            break;
                        case "dhw":
                            totalLoads[m] += zone.DHWLoadsMonthly[m];
                            break;
                        case "electricity":
                            totalLoads[m] += zone.ElectricityLoadsMonthly[m];
                            break;
                    }
                }
            }

            return totalLoads;
        }


        /// <summary>
        /// Reads all input energy carriers from conversion techs and gets their (primary) energy.
        /// Only plug in conversion tech that uses energy from Environment.
        /// Don't use conversion tech that gets its inputEnergy from another conversion tech, because then input energy is counted twice
        /// </summary>
        /// <param name="inputCarriers"></param>
        /// <returns></returns>
        public static double[] GetTotalMonthlyPrimaryLoads(List<ConversionTech> conversionTech)
        {
            double [] result = new double[Misc.MonthsPerYear];
            foreach (var tech in conversionTech)
            {
                for (int i = 0; i < Misc.MonthsPerYear; i++)
                {
                    result[i] += tech.InputCarrier.MonthlyCumulativeEnergy[i] * tech.InputCarrier.PrimaryEnergyFactor;
                }
            }

            return result;
        }


        public static double GetTotalMonthlySystemLosses(Hive.IO.DataHandling.Results results, double[] monthlyPrimaryEnergy)
        {
            var sysLosses = new double[Misc.MonthsPerYear];
            for (int i = 0; i < sysLosses.Length; i++)
            {
                sysLosses[i] = results.TotalPrimaryEnergyMonthly[i]
                               - results.TotalFinalCoolingMonthly[i]
                               - results.TotalFinalHeatingMonthly[i]
                               - results.TotalFinalDomesticHotWaterMonthly[i]
                               - results.TotalFinalElectricityMonthly[i];
            }

            return sysLosses.Sum();
        }


        public static double[] GetTotalMonthlyOperationalEmissions(List<ConversionTech> conversionTech)
        {
            double[] result = new double[Misc.MonthsPerYear];
            foreach (var tech in conversionTech)
            {
                for (int i = 0; i < result.Length; i++)
                {
                    result[i] += tech.InputCarrier.MonthlyCumulativeEmissions[i];
                }
            }

            return result;
        }




        public static double GetTotalGainsOrLosses(Building.Building building, string loadType)
        {
            double result = 0.0;
            foreach (var zone in building.Zones)
            {
                switch (loadType)
                {
                    case "Qt_opaque":
                        result += zone.OpaqueTransmissionHeatLosses.Sum();
                        break;
                    case "Qt_transparent":
                        result += zone.TransparentTransmissionHeatLosses.Sum();
                        break;
                    case "Qv":
                        result += zone.VentilationHeatLosses.Sum();
                        break;
                    case "Qi":
                        result += zone.InternalHeatGains.Sum();
                        break;
                    case "Qs":
                        result += zone.SolarGains.Sum();
                        break;
                }
            }

            return result;
        }


        #endregion

    }
}
