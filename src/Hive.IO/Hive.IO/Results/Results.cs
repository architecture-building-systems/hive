using System;
using System.Collections.Generic;
using System.Linq;
using Hive.IO.EnergySystems;
using rg = Rhino.Geometry;

namespace Hive.IO.Results
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

        public double[] TotalPrimaryEnergyMonthlyNonRenewable { get; private set; }
        public double[] TotalFinalEnergyMonthlyRenewable { get; private set; }

        public double[] TotalPurchasedElectricityMonthly { get; private set; }
        public double[] TotalFeedInElectricityMonthly { get; private set; }
        public double [] TotalSurplusHeatingMonthly { get; private set; } // just dumped? we can't utilize it without storage model, or connection to district heating network
        public double [] TotalConsumedElectricityMonthly { get; private set; }
        public double[] TotalActiveCoolingMonthly { get; private set; }


        //public double[] TotalFinalElectricityHourly { get; private set; }
        //public double[] TotalFinalCoolingHourly { get; private set; }
        //public double[] TotalFinalHeatingHourly { get; private set; }
        //public double[] TotalFinalDomesticHotWaterHourly { get; private set; }
        #endregion


        #region Total Emissions

        public double TotalEmbodiedConstructionEmissions { get; private set; }
        public double[] TotalOperationalEmissionsMonthly { get; private set; }

        #endregion


        #region Total Losses and Gains
        public double TotalOpaqueTransmissionHeatLosses { get; private set; }
        public double TotalWindowTransmissionHeatLosses { get; private set; }
        public double TotalVentilationHeatLosses { get; private set; }
        public double TotalOpaqueTransmissionHeatGains { get; private set; }
        public double TotalWindowTransmissionHeatGains { get; private set; }
        public double TotalVentilationHeatGains { get; private set; }

        public double TotalInternalGains { get; private set; }
        public double TotalSolarGains { get; private set; }

        public double TotalSystemLosses { get; private set; }

        public double TotalCoolingLosses { get; private set; }
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

            this.TotalPrimaryEnergyMonthlyNonRenewable = new double[Misc.MonthsPerYear];
            this.TotalFinalEnergyMonthlyRenewable = new double[Misc.MonthsPerYear];
            this.TotalOperationalEmissionsMonthly = new double[Misc.MonthsPerYear];
            this.TotalPurchasedElectricityMonthly = new double[Misc.MonthsPerYear];
            this.TotalFeedInElectricityMonthly = new double[Misc.MonthsPerYear];
            this.TotalSurplusHeatingMonthly = new double[Misc.MonthsPerYear];
            this.TotalConsumedElectricityMonthly = new double[Misc.MonthsPerYear];
            this.TotalActiveCoolingMonthly = new double[Misc.MonthsPerYear];
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

            this.TotalPrimaryEnergyMonthlyNonRenewable = GetTotalMonthlyPrimaryLoads(conversionTech);
            this.TotalFinalEnergyMonthlyRenewable = GetTotalMonthlyRenewableEnergy(conversionTech);
            this.TotalOperationalEmissionsMonthly = GetTotalMonthlyOperationalEmissions(conversionTech);

            Tuple<double[], double[]> tuple = GetTotalMonthlyPurchasedAndFeedInElectricity(conversionTech);
            this.TotalPurchasedElectricityMonthly = tuple.Item1;
            this.TotalFeedInElectricityMonthly = tuple.Item2;
            this.TotalSurplusHeatingMonthly = GetTotalMonthlySurplusHeating(building, conversionTech);
            this.TotalConsumedElectricityMonthly = GetTotalMonthlyConsumedElectricity(building, conversionTech);
            this.TotalActiveCoolingMonthly = GetTotalMonthlyActiveCooling(conversionTech);

            //this.TotalFinalCoolingHourly = new double[Misc.HoursPerYear];
            //this.TotalFinalHeatingHourly = new double[Misc.HoursPerYear];
            //this.TotalFinalElectricityHourly = new double[Misc.HoursPerYear];
            //this.TotalFinalDomesticHotWaterHourly = new double[Misc.HoursPerYear];

            this.TotalOpaqueTransmissionHeatLosses = GetTotalGainsOrLosses(building, "Qt_opaque_positive");
            this.TotalWindowTransmissionHeatLosses = GetTotalGainsOrLosses(building, "Qt_transparent_positive");
            this.TotalVentilationHeatLosses = GetTotalGainsOrLosses(building, "Qv_positive");
            this.TotalOpaqueTransmissionHeatGains = GetTotalGainsOrLosses(building, "Qt_opaque_negative");
            this.TotalWindowTransmissionHeatGains = GetTotalGainsOrLosses(building, "Qt_transparent_negative");
            this.TotalVentilationHeatGains = GetTotalGainsOrLosses(building, "Qv_negative");
            this.TotalInternalGains = GetTotalGainsOrLosses(building, "Qi");
            this.TotalSolarGains = GetTotalGainsOrLosses(building, "Qs");
            this.TotalSystemLosses = GetTotalMonthlySystemLossesNonRenewable(conversionTech).Sum();

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


        public static double [] GetTotalMonthlySurplusHeating(Building.Building building, List<ConversionTech> conversionTech)
        {

            double[] totalHeatGenerated = new double[Misc.MonthsPerYear];
            double[] totalHeatLoads = new double[Misc.MonthsPerYear];
            for (int i = 0; i < totalHeatGenerated.Length; i++)
            {
                totalHeatGenerated[i] = 0.0;
                totalHeatLoads[i] = 0.0;
            }

            for (int i = 0; i < totalHeatLoads.Length; i++)
                foreach (var zone in building.Zones)
                    totalHeatLoads[i] += zone.ConsumedHeatingMonthly[i];

            foreach(var tech in conversionTech)
            {
                if (tech.IsHeating) //in case of CHP, I know that it is the first OutputCarrier... no good coding. FIX ME... 
                {
                    for(int i=0; i<totalHeatGenerated.Length; i++)
                    {
                        totalHeatGenerated[i] += tech.OutputCarriers[0].MonthlyCumulativeEnergy[i];
                    }
                }
            }

            var result = Enumerable.Zip(totalHeatLoads, totalHeatGenerated, (a, b) => a - b).ToArray(); 
            for(int i=0; i<result.Length; i++)
                result[i] = result[i] < 0.0 ? Math.Abs(result[i]) : 0.0;

            return result;
        }

        public static double [] GetTotalMonthlyActiveCooling(List<ConversionTech> conversionTech)
        {
            double[] result = new double[Misc.MonthsPerYear];
            for (int i = 0; i < result.Length; i++)
                result[i] = 0.0;
            foreach(var tech in conversionTech)
            {
                if (tech.IsCooling)
                {
                    for(int i=0; i<result.Length; i++)
                    {
                        result[i] += tech.OutputCarriers[0].MonthlyCumulativeEnergy[i];
                    }
                }
            }

            return result;
        }


        // this is not final demand, because final demand could be negative (e.g. from surplus PV). But this is how much electricity we actually consume. we need to know for the Sankey diagram
        public static double [] GetTotalMonthlyConsumedElectricity(Building.Building building, List<ConversionTech> conversionTech)
        {
            // go through all zones and get
            // building.Zones[0].ConsumedElectricityMonthly.Sum()

            // go tzhrough all conversion tech (of type HeatPump only) and check InputCarrier ConsumedElectricity

            double[] result = new double[Misc.MonthsPerYear];
            for (int i = 0; i < result.Length; i++)
                result[i] = 0.0;
            foreach (var zone in building.Zones)
            {
                for (int i = 0; i < result.Length; i++)
                {
                    result[i] += zone.ConsumedElectricityMonthly[i];
                }
            }

            foreach (var tech in conversionTech)
            {
                if(tech is HeatPump)
                {
                    for(int i=0; i<result.Length; i++)
                    {
                        result[i] += tech.InputCarrier.MonthlyCumulativeEnergy[i]; // inputcarrier is always Electricity in HeatPump
                    }
                }
            }

            return result;
        }


        public static Tuple<double [], double[]> GetTotalMonthlyPurchasedAndFeedInElectricity(List<ConversionTech> conversionTech)
        {
            double[] purchased = new double[Misc.MonthsPerYear];
            double[] feedIn = new double[Misc.MonthsPerYear];
            for (int i = 0; i < purchased.Length; i++)
            {
                purchased[i] = 0.0;
                feedIn[i] = 0.0;
            }
            foreach(var tech in conversionTech)
            {
                if(tech is DirectElectricity)
                {
                    for(int i=0; i<purchased.Length; i++)
                    {
                        double _elec = tech.InputCarrier.MonthlyCumulativeEnergy[i];
                        if (_elec > 0.0)
                            purchased[i] += _elec;
                        else
                            feedIn[i] += Math.Abs(_elec);
                    }
                }
            }
            return new Tuple<double[], double []>(purchased, feedIn);
        }


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
            double[] result = new double[Misc.MonthsPerYear];
            for (int i = 0; i < result.Length; i++)
                result[i] = 0.0;
            foreach (var tech in conversionTech)
            {
                for (int i = 0; i < Misc.MonthsPerYear; i++)
                {
                    if (tech is SurfaceBasedTech == false && tech is DirectElectricity == false && tech is HeatPump == false) //no heat pump, because electricity of it is already in DirectElectricity
                    {
                        result[i] += tech.InputCarrier.MonthlyCumulativeEnergy[i] * tech.InputCarrier.PrimaryEnergyFactor;
                    }
                    if(tech is DirectElectricity)
                    {
                        if (tech.InputCarrier.MonthlyCumulativeEnergy[i] > 0)
                            result[i] += tech.InputCarrier.MonthlyCumulativeEnergy[i] * tech.InputCarrier.PrimaryEnergyFactor;
                    }
                }
            }

            return result;
        }


        /// <summary>
        /// final (effective) renewable energy, not primary
        /// </summary>
        /// <param name="renewableConversionTech"></param>
        /// <returns></returns>
        public static double[] GetTotalMonthlyRenewableEnergy(List<ConversionTech> renewableConversionTech)
        {
            double[] result = new double[Misc.MonthsPerYear];
            for (int i = 0; i < result.Length; i++)
                result[i] = 0.0;
            foreach (var tech in renewableConversionTech)
            {
                if (tech is SurfaceBasedTech)
                {
                    for (int i = 0; i < result.Length; i++)
                    {
                        result[i] += tech.OutputCarriers[0].MonthlyCumulativeEnergy[i];
                    }
                }
            }

            return result;
        }


        public static double[] GetTotalMonthlySystemLossesNonRenewable(List<ConversionTech> conversionTech)
        {
            double[] result = new double[Misc.MonthsPerYear];
            for (int i = 0; i < result.Length; i++)
                result[i] = 0.0;
            foreach (var tech in conversionTech)
            {
                for (int i = 0; i < result.Length; i++)
                {
                    // only for fuel based and grid electricity. not for Heatpumps, because electricity from heatpumps will be attributed to consumed electricity (Outgoing arrows in Sankey)
                    if (tech is SurfaceBasedTech == false && tech is HeatPump == false)
                    {
                        if(tech.InputCarrier.MonthlyCumulativeEnergy[i] > 0.0) // don't take negative values
                            result[i] += ((tech.InputCarrier.MonthlyCumulativeEnergy[i] * tech.InputCarrier.PrimaryEnergyFactor) - tech.OutputCarriers[0].MonthlyCumulativeEnergy[i]);
                        
                    }
                }
            }

            return result;
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
                    case "Qt_opaque_positive":
                        result += (from x in zone.OpaqueTransmissionHeatLosses where x > 0.0 select x).Sum();
                        break;
                    case "Qt_transparent_positive":
                        result += (from x in zone.TransparentTransmissionHeatLosses where x > 0.0 select x).Sum();
                        break;
                    case "Qv_positive":
                        result += (from x in zone.VentilationHeatLosses where x > 0.0 select x).Sum();
                        break;
                    case "Qt_opaque_negative":
                        result += Math.Abs((from x in zone.OpaqueTransmissionHeatLosses where x <= 0.0 select x).Sum());
                        break;
                    case "Qt_transparent_negative":
                        result += Math.Abs((from x in zone.TransparentTransmissionHeatLosses where x <= 0.0 select x).Sum());
                        break;
                    case "Qv_negative":
                        result += Math.Abs((from x in zone.VentilationHeatLosses where x <= 0.0 select x).Sum());
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
