using System;

namespace Hive.IO.EnergySystems
{
    #region MiscSupply

    public class Chiller : ConversionTech
    {
        /// <summary>
        /// Ambient air carrier. This will influence COP of the Chiller
        /// </summary>
        public Air AmbientAir { get; private set; }
        public double EtaRef { get; private set; }
        public Chiller(double investmentCost, double embodiedGhg, double capacity, double etaRef) 
            : base(investmentCost, embodiedGhg, capacity, "kW", false, true, false)
        {
            this.EtaRef = etaRef;
            base.Name = "Chiller";
        }


        public void SetInputOutputSimple(Electricity electricityIn, double[] coolingGenerated, double [] tempWarm, double [] tempCold)
        {
            int horizon = coolingGenerated.Length;

            var tempWarmHorizon = new double[horizon];
            var tempColdHorizon = new double[horizon];

            var elecConsumed = new double[horizon];
            var elecPrice = new double[horizon];
            var elecEmissionsFactor = new double[horizon];

            if (horizon == Misc.MonthsPerYear)
            {
                elecPrice = Misc.GetAverageMonthlyValue(electricityIn.EnergyPrice);
                elecEmissionsFactor = Misc.GetAverageMonthlyValue(electricityIn.GhgEmissionsFactor);
                tempWarmHorizon = Misc.GetAverageMonthlyValue(tempWarm);
                tempColdHorizon = Misc.GetAverageMonthlyValue(tempCold);
            }
            else
            {
                elecPrice = electricityIn.EnergyPrice;
                elecEmissionsFactor = electricityIn.GhgEmissionsFactor;
                tempWarmHorizon = tempWarm;
                tempColdHorizon = tempCold;
            }

            for (int t = 0; t < horizon; t++)
            {
                double COP = this.EtaRef * (tempWarmHorizon[t] / (tempWarmHorizon[t] - tempColdHorizon[t]));
                elecConsumed[t] = coolingGenerated[t] / COP;
            }

            Electricity electricityConsumedCarrier = new Electricity(horizon, elecConsumed, elecPrice, elecEmissionsFactor);
            base.InputCarrier = electricityConsumedCarrier;

            base.OutputCarriers = new Carrier[1];
            base.OutputCarriers[0] = new Water(horizon, coolingGenerated, null, null, tempCold);
            
        }


        /// <summary>
        /// 10.1016/j.apenergy.2019.03.177
        /// Eq. A.8
        /// </summary>
        /// <param name="electricityIn">Empty electricity carrier, i.e. no information yet how much is consumed. Will be computed here and be infused into this.InputCarrier</param>
        /// <param name="airIn"></param>
        /// <param name="coolingGenerated"></param>
        /// <param name="supplyTemp"></param>
        public void SetInputOutput(Electricity electricityIn, Air airIn, double[] coolingGenerated, double [] supplyTemp)
        {
            int horizon = coolingGenerated.Length;

            var elecConsumed = new double[horizon];
            var elecPrice = new double[horizon];
            var elecEmissionsFactor = new double[horizon];
            var airTemp = new double[horizon];

            if (horizon == Misc.MonthsPerYear)
            {
                elecPrice = Misc.GetAverageMonthlyValue(electricityIn.EnergyPrice);
                elecEmissionsFactor = Misc.GetAverageMonthlyValue(electricityIn.GhgEmissionsFactor);
                airTemp = airIn.MonthlyTemperature;
            }
            else
            {
                elecPrice = electricityIn.EnergyPrice;
                elecEmissionsFactor = electricityIn.GhgEmissionsFactor;
                airTemp = airIn.Temperature;
            }

            for (int t = 0; t < horizon; t++)
            {
                double COP = (638.95 - 4.238 * airTemp[t]) / (100.0 + 3.534 * airTemp[t]);
                elecConsumed[t] = coolingGenerated[t] / COP;
            }

            this.AmbientAir = new Air(horizon, null, null, null, airTemp); // how would I know air energy? i'd need that for exergy calculation?

            Electricity electricityConsumedCarrier = new Electricity(horizon, elecConsumed, elecPrice, elecEmissionsFactor);
            base.InputCarrier = electricityConsumedCarrier;

            base.OutputCarriers = new Carrier[1];
            base.OutputCarriers[0] = new Water(horizon, coolingGenerated, null, null, supplyTemp);
        }
    }


    public class AirSourceHeatPump : ConversionTech
    {
        /// <summary>
        /// Ambient air carrier. This will influence COP of the ASHP
        /// </summary>
        public Air AmbientAir { get; private set; }
		public double EtaRef { get; private set; }
        public AirSourceHeatPump(double investmentCost, double embodiedGhg, double capacity, double etaRef) 
            : base(investmentCost, embodiedGhg, capacity, "kW", true, false, false)
        {
			this.EtaRef = etaRef;
            base.Name = "AirSourceHeatPump";
        }


        // this is a copy of Chiller.setinpuitoutputsimple... can we avoid redundancy? Make abstract class HeatPump?
        public void SetInputOutputSimple(Electricity electricityIn, double[] heatingGenerated, double[] tempWarm, double[] tempCold)
        {
            int horizon = heatingGenerated.Length;

            var tempWarmHorizon = new double[horizon];
            var tempColdHorizon = new double[horizon];

            var elecConsumed = new double[horizon];
            var elecPrice = new double[horizon];
            var elecEmissionsFactor = new double[horizon];

            if (horizon == Misc.MonthsPerYear)
            {
                elecPrice = Misc.GetAverageMonthlyValue(electricityIn.EnergyPrice);
                elecEmissionsFactor = Misc.GetAverageMonthlyValue(electricityIn.GhgEmissionsFactor);
                tempWarmHorizon = Misc.GetAverageMonthlyValue(tempWarm);
                tempColdHorizon = Misc.GetAverageMonthlyValue(tempCold);
            }
            else
            {
                elecPrice = electricityIn.EnergyPrice;
                elecEmissionsFactor = electricityIn.GhgEmissionsFactor;
                tempWarmHorizon = tempWarm;
                tempColdHorizon = tempCold;
            }

            for (int t = 0; t < horizon; t++)
            {
                double COP = this.EtaRef * (tempWarmHorizon[t] / (tempWarmHorizon[t] - tempColdHorizon[t]));
                elecConsumed[t] = heatingGenerated[t] / COP;
            }

            Electricity electricityConsumedCarrier = new Electricity(horizon, elecConsumed, elecPrice, elecEmissionsFactor);
            base.InputCarrier = electricityConsumedCarrier;

            base.OutputCarriers = new Carrier[1];
            base.OutputCarriers[0] = new Water(horizon, heatingGenerated, null, null, tempCold);

        }


        /// <summary>
        /// 10.1016/j.apenergy.2019.03.177  Eq. (A.7)
        /// </summary>
        /// <param name="electricityIn"></param>
        /// <param name="airIn"></param>
        /// <param name="heatingGenerated"></param>
        /// <param name="supplyTemp"></param>
        public void SetInputOutput(Electricity electricityIn, Air airIn, double[] heatingGenerated, double[] supplyTemp, 
            double pi1 = 13.39, double pi2 = -0.047, double pi3 = 1.109, double pi4 = 0.012)
        {
            int horizon = heatingGenerated.Length;

            var elecConsumed = new double[horizon];
            var elecPrice = new double[horizon];
            var elecEmissionsFactor = new double[horizon];
            var airTemp = new double[horizon];

            if (horizon == Misc.MonthsPerYear)
            {
                elecPrice = Misc.GetAverageMonthlyValue(electricityIn.EnergyPrice);
                elecEmissionsFactor = Misc.GetAverageMonthlyValue(electricityIn.GhgEmissionsFactor);
                airTemp = airIn.MonthlyTemperature;
            }
            else
            {
                elecPrice = electricityIn.EnergyPrice;
                elecEmissionsFactor = electricityIn.GhgEmissionsFactor;
                airTemp = airIn.Temperature;
            }

            for (int t = 0; t < horizon; t++)
            {
                double COP = pi1 * Math.Exp(pi2 * (supplyTemp[t] - airTemp[t])) + pi3 * Math.Exp(pi4 * (supplyTemp[t] - airTemp[t]));
                elecConsumed[t] = heatingGenerated[t] / COP;
            }

            Electricity electricityConsumedCarrier = new Electricity(horizon, elecConsumed, elecPrice, elecEmissionsFactor);
            base.InputCarrier = electricityConsumedCarrier;

            base.OutputCarriers = new Carrier[1];
            base.OutputCarriers[0] = new Water(horizon, heatingGenerated, null, null, supplyTemp);
        }
    }



    public class HeatCoolingExchanger : ConversionTech
    {
        // assume perfect heat exchange?, according to Dr. Somil District Heating Miglani, only distribution losses. but since we don't have network implemented yet, lets use a coefficient
        public double DistributionLosses { get; private set; }


        public HeatCoolingExchanger(double investmentCost, double embodiedGhg, double capacity, double losses, bool heatingExchanger = true, bool coolingExchanger = false)
            : base(investmentCost, embodiedGhg, capacity, "kW", heatingExchanger, coolingExchanger, false)
        {
            this.DistributionLosses = losses;
            if (heatingExchanger && !coolingExchanger)
                base.Name = "HeatExchanger";
            else if (coolingExchanger && !heatingExchanger)
                base.Name = "CoolingExchanger";
            else if (heatingExchanger && coolingExchanger)
                base.Name = "HeatCoolingExchanger";
        }


        public void SetInputOutput(Water districtFluid, double[] generatedEnergy, double [] supplyTemp, double [] returnTemp)
        {
            int horizon = generatedEnergy.Length;
            var consumedEnergy = new double[horizon];
            var energyPrice = new double[horizon];
            var energyGhg = new double[horizon];

            if (horizon == Misc.MonthsPerYear)
            {
                energyPrice = Misc.GetAverageMonthlyValue(districtFluid.EnergyPrice);
                energyGhg = Misc.GetAverageMonthlyValue(districtFluid.GhgEmissionsFactor);
            }
            else
            {
                energyPrice = districtFluid.EnergyPrice;
                energyGhg = districtFluid.GhgEmissionsFactor;
            }

            for(int t=0; t<horizon; t++)
            {
                consumedEnergy[t] = generatedEnergy[t] / (1.0 - this.DistributionLosses);
            }

            base.InputCarrier = new Water(horizon, consumedEnergy, energyPrice, energyGhg, supplyTemp);  // simplified assumption, that DH supply temp is already at the right level

            base.OutputCarriers = new Carrier[1] { new Water(horizon, generatedEnergy, null, null, supplyTemp) };

        }




    }

    #endregion



    #region Base Conversion Class
    /// <summary>
    /// Heating, Cooling, Electricity generation systems
    /// E.g. CHP, boiler, heat pump, chiller, PV ...
    /// </summary>
    public abstract class ConversionTech
    {
        /// <summary>
        /// Technology name
        /// </summary>
        public string Name { get; protected set; }
        /// <summary>
        /// Specification of the technology, e.g. "Mono-cristalline PV"
        /// </summary>
        public string DetailedName { get; protected set; }


        /// <summary>
        /// Indicating whether this technology produces electricity
        /// </summary>
        public bool IsElectric { get; protected set; }
        /// <summary>
        /// Indicating whether this technology produces heat
        /// </summary>
        public bool IsHeating { get; protected set; }
        /// <summary>
        /// Indicating whether this technology produces cooling
        /// </summary>
        public bool IsCooling { get; protected set; }


        /// <summary>
        /// Capacity of technology. Unit is defined in 'CapacityUnit'
        /// </summary>
        public double Capacity { get; protected set; }

        /// <summary>
        /// Capacity unit of technology capacity (e.g. "kW", "kWh", "kW_peak", etc.)
        /// </summary>
        public string CapacityUnit { get; protected set; }

        /// <summary>
        /// Investment cost per this.CapacityUnit
        /// </summary>
        public double SpecificInvestmentCost { get; protected set; }
        /// <summary>
        /// Life cycle GHG emissions, in kgCO2eq. per this.CapacityUnit
        /// </summary>
        public double SpecificEmbodiedGhg { get; protected set; }


        /// <summary>
        /// Input stream. e.g. for a CHP this could be 'NaturalGas'
        /// </summary>
        public Carrier InputCarrier { get; protected set; }
        /// <summary>
        /// Output streams. e.g. for a CHP this could be 'Heating' and 'Electricity'
        /// </summary>
        public Carrier[] OutputCarriers { get; protected set; }


        protected ConversionTech(double investmentCost, double embodiedGhg, double capacity, string capacityUnity, bool isHeating, bool isCooling, bool isElectric)
        {
            this.SpecificInvestmentCost = investmentCost;
            this.SpecificEmbodiedGhg = embodiedGhg;
            this.Capacity = capacity;
            this.CapacityUnit = capacityUnity;
            this.IsHeating = isHeating;
            this.IsCooling = isCooling;
            this.IsElectric = isElectric;
        }

    }


    #endregion
}
