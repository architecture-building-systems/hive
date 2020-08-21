

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


        /// <summary>
        /// 
        /// </summary>
        /// <param name="electricity"></param>
        /// <param name="tempWarm"></param>
        /// <param name="tempCold"></param>
        /// <param name="coolingDemand"></param>
        /// <param name="electricityIn"></param>
        /// <param name="timeResolution">"'monthly' (12) or 'hourly' (8760)"</param>
        public void ComputeInputOutputSimple(double [] coolingDemand, Electricity electricityIn, double tempWarm, double tempCold, string timeResolution = "monthly")
        {
            int horizon;
            double [] elecPrice = null;
            if (string.Equals(timeResolution, "monthly"))
            {
                horizon = Misc.MonthsPerYear;
                elecPrice = electricityIn.EnergyPrice;
            }
            else
            {
                horizon = Misc.HoursPerYear;
                elecPrice = Misc.GetAverageMonthlyValue(electricityIn.EnergyPrice);
            }

            double COP = this.EtaRef * (tempWarm / (tempWarm - tempCold));

            var elecConsumed = new double[horizon];
            var elecCost = new double[horizon];
            var elecEmissions = new double[horizon];
            for(int t=0; t<horizon; t++)
            {
                elecConsumed[t] = coolingDemand[t] / COP;
                elecCost[t] = elecConsumed[t] * elecPrice[t];
                //elecEmissions[t] = elecConsumed[t] * elecEmissio
            }


            Electricity electricityInInfused = null;
            base.InputCarrier = electricityInInfused;

            base.OutputCarriers = new EnergyCarrier[1];
            base.OutputCarriers[0] = new Water(Misc.HoursPerYear, null, null, null, null);
        }


        /// <summary>
        /// inputs from Hive.IO.Environment. But electricity also needs information on quantity... form Core simulator? 
        /// </summary>
        /// <param name="ambientAir"></param>
        /// <param name="electricity"></param>
        public void SetInput(Air ambientAir, Electricity electricity)
        {
            this.AmbientAir = ambientAir;
            base.InputCarrier = electricity;
        }


        /// <summary>
        /// parameters from a simulator in Hive.IO.Core
        /// </summary>
        /// <param name="horizon"></param>
        /// <param name="availableEnergy"></param>
        /// <param name="energyCost"></param>
        /// <param name="ghgEmissions"></param>
        /// <param name="supplyTemperature"></param>
        public void SetOutput(int horizon, double[] availableEnergy, double[] energyCost, double[] ghgEmissions, double[] supplyTemperature)
        {
            base.OutputCarriers = new EnergyCarrier[1];
            base.OutputCarriers[0] = new Water(horizon, availableEnergy, energyCost, ghgEmissions, supplyTemperature);
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


        /// <summary>
        /// inputs from Hive.IO.Environment. But electricity also needs information on quantity... form Core simulator? 
        /// </summary>
        /// <param name="ambientAir"></param>
        /// <param name="electricity"></param>
        public void SetInput(Air ambientAir, Electricity electricity)
        {
            this.AmbientAir = ambientAir;
            base.InputCarrier = electricity;
        }


        /// <summary>
        /// parameters from a simulator in Hive.IO.Core
        /// </summary>
        /// <param name="horizon"></param>
        /// <param name="availableEnergy"></param>
        /// <param name="energyCost"></param>
        /// <param name="ghgEmissions"></param>
        /// <param name="supplyTemperature"></param>
        public void SetOutput(int horizon, double[] availableEnergy, double[] energyCost, double[] ghgEmissions, double[] supplyTemperature)
        {
            base.OutputCarriers = new EnergyCarrier[1];
            base.OutputCarriers[0] = new Water(horizon, availableEnergy, energyCost, ghgEmissions, supplyTemperature);
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
        public EnergyCarrier InputCarrier { get; protected set; }
        /// <summary>
        /// Output streams. e.g. for a CHP this could be 'Heating' and 'Electricity'
        /// </summary>
        public EnergyCarrier[] OutputCarriers { get; protected set; }



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


        // how can I change parameters of methods in derived classes? the argument should still be an EnergyCarrier, but I wanna specifiy, e.g. restricting to Solar

        ///// <summary>
        ///// Not part of the constructor, because it can be set at a later stage of the program
        ///// For example, for PV, solar potentials need to be calculated first, which might happen after a PV object has been instantiated
        ///// </summary>
        ///// <param name="inputCarrier"></param>
        //public virtual void SetInput(EnergyCarrier inputCarrier) { }
        ///// <summary>
        ///// Same as with inputs, the outputs might be calculated later after a Conversion class has been instantiated
        ///// </summary>
        ///// <param name="outputCarriers"></param>
        //public virtual void SetOutputs(EnergyCarrier[] outputCarriers) { }
    }


    #endregion
}
