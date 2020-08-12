

namespace Hive.IO.EnergySystems
{
    #region MiscSupply

    // these are all networks, containing input energy carriers. but not conversion tech
    /*
    public class ElectricityGrid : ConversionTech
    {
        public ElectricityGrid(double investmentCost, double embodiedGhg) 
            : base(investmentCost, embodiedGhg, double.MaxValue, "kW", false, false, true)
        {
        }


    }

    public class DistrictHeating : ConversionTech
    {
        public DistrictHeating(double investmentCost, double embodiedGhg) 
            : base(investmentCost, embodiedGhg, Double.MaxValue, "kW", true, false, false)
        {
        }


    }


    public class DistrictCooling : ConversionTech
    {
        public DistrictCooling(double investmentCost, double embodiedGhg) 
            : base(investmentCost, embodiedGhg, double.MaxValue, "kW", false, true, false)
        {
        }



    }
    */


    public class Chiller : ConversionTech
    {
        public Chiller(double investmentCost, double embodiedGhg, double capacity) 
            : base(investmentCost, embodiedGhg, capacity, "kW", false, true, false)
        {
        }


        public double[] SetConversionEfficiencyCooling()
        {
            return new double[]{};
        }
    }


    public class AirSourceHeatPump : ConversionTech
    {
        /// <summary>
        /// Ambient air carrier. This will influence COP of the ASHP
        /// </summary>
        public Air AmbientAir { get; private set; }
		public double COP { get; private set; }
        public AirSourceHeatPump(double investmentCost, double embodiedGhg, double capacity, double COP) 
            : base(investmentCost, embodiedGhg, capacity, "kW", true, false, false)
        {
			this.COP = COP;
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



        protected ConversionTech(double investmentCost, double embodiedGhg,
            double capacity, string capacityUnity,
            bool isHeating, bool isCooling, bool isElectric)
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
