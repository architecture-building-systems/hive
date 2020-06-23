namespace Hive.IO.EnergySystems
{
    /// <summary>
    /// Energy Carrier
    /// </summary>
    public abstract class Carrier
    {
        protected CarrierType Type;
        protected enum CarrierType
        {
            Air,            // for air source heat pump e.g.
            Wind,           // for wind turbine e.g.
            Solar,          // solar potentials
            NaturalGas,     // from gas grid or conversion technologies
            BioGas,         // same
            Oil,            // same
            BioMass,        // such as wood chips
            Electricity,    // from the grid or conversion technologies
            Water,          // primarily for thermal energy. can be both for cooling and heating. for cooling, Carrier.Quantity would take negative values (in degrees C). Can be from district network or from conversion tech
            MiscFluid
        }

        /// <summary>
        /// Horizon of time series data
        /// </summary>
        public int Horizon { get; protected set; }
        /// <summary>
        /// Time series of available quantity of this carrier. Unit defined in Carrier.Unit. E.g. could be solar potentials in kWh/sqm
        /// </summary>
        public double[] Quantity { get; protected set; }
        /// <summary>
        /// Unit of this energy carrier (e.g. kWh, kg, liters, ...)
        /// </summary>
        public string Unit { get; protected set; }
        /// <summary>
        /// Time series for Unit cost of carrier. E.g. 0.2 CHF for 1 kWh of Electricity
        /// </summary>
        public double [] UnitCost { get; protected set; }
        /// <summary>
        /// Time series of Greenhouse gas emissions in kgCO2eq/this.Unit, e.g. 0.2 kgCO2eq/kWh in case of electricity
        /// </summary>
        public double[] GhgEmissions { get; protected set; }


        protected Carrier(CarrierType type)
        {
            Type = type;
        }
    }
}