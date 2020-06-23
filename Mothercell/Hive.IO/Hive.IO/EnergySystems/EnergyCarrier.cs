using System.Runtime.InteropServices;

namespace Hive.IO.EnergySystems
{
    #region implemented Energy Carriers
    /// <summary>
    /// Air energy carrier, e.g. for air source heat pump
    /// </summary>
    public class Air : EnergyCarrier
    {
        public Air(int horizon, double[] airTemperature) 
            : base(horizon, EnergyCarrier.EnergyUnit.DegreeCelsius, airTemperature, null, null) { }
    }


    /// <summary>
    /// Solar energy carrier, e.g. for PV and solar thermal
    /// </summary>
    public class Solar : EnergyCarrier
    {
        public Solar(int horizon, double[] irradiation) 
            : base(horizon, EnergyCarrier.EnergyUnit.KiloWattHoursPerSquareMeters, irradiation, null, null) { }
    }


    /// <summary>
    /// Water as energy carrier, e.g. the output of a boiler, heat pump, or solar thermal collector
    /// </summary>
    public class Water : EnergyCarrier
    {
        public Water(int horizon, double[] waterTemperature) 
            : base(horizon, EnergyCarrier.EnergyUnit.DegreeCelsius, waterTemperature, null, null) { }
    }


    /// <summary>
    /// Electricity grid provides electricity
    /// </summary>
    public class ElectricityGrid : EnergyCarrier
    {
        public ElectricityGrid(int horizon, double[] energyCost, double[] ghgEmissions) 
            : base(horizon, EnergyCarrier.EnergyUnit.KiloWattHours, null, energyCost, ghgEmissions) { }
    }


    /// <summary>
    /// District Heating provides hot water
    /// </summary>
    public class DistrictHeating : EnergyCarrier
    {
        public DistrictHeating(int horizon, double[] availableHeating, double[] energyCost, double[] ghgEmissions) 
            : base(horizon, EnergyCarrier.EnergyUnit.KiloWattHours, availableHeating, energyCost, ghgEmissions) { }
    }


    /// <summary>
    /// District cooling provides cool water
    /// </summary>
    public class DistrictCooling : EnergyCarrier
    {
        public DistrictCooling(int horizon, double[] availableCooling, double[] energyCost, double[] ghgEmissions) 
            : base(horizon, EnergyCarrier.EnergyUnit.KiloWattHours, availableCooling, energyCost, ghgEmissions) { }
    }


    /// <summary>
    /// BioGas. Availability might be limited
    /// </summary>
    public class BioGas : EnergyCarrier
    {
        public BioGas(int horizon, double[] biogasAvailability, double[] energyCost, double[] ghgEmissions) 
            : base(horizon, EnergyCarrier.EnergyUnit.KiloWattHours, biogasAvailability, energyCost, ghgEmissions)
        {
        }
    }


    /// <summary>
    /// Wood pellets. Availability might be limited
    /// </summary>
    public class Pellets : EnergyCarrier
    {
        public Pellets(int horizon, double[] pelletsAvailability, double[] energyCost, double[] ghgEmissions) 
            : base(horizon, EnergyCarrier.EnergyUnit.KiloWattHours, pelletsAvailability, energyCost, ghgEmissions)
        {
        }
    }
#endregion



    /// <summary>
    /// Energy Carrier
    /// </summary>
    public abstract class EnergyCarrier
    {
        /// <summary>
        /// Pre-defined energy units
        /// </summary>
        public enum EnergyUnit
        {
            KiloWattHours,
            KiloWattHoursPerSquareMeters,
            DegreeCelsius
        }

        /// <summary>
        /// Horizon of time series data
        /// </summary>
        public int Horizon { get; protected set; }
        /// <summary>
        /// Time series of available/provided energy of this carrier in kWh per time step
        /// </summary>
        public double [] Energy { get; protected set; }

        /// <summary>
        /// Setting the energy unit to kWh
        /// </summary>
        public EnergyUnit Unit { get; protected set; }

        /// <summary>
        /// Time series with cost coefficients per kWh
        /// </summary>
        public double [] EnergyCost { get; protected set; }
        /// <summary>
        /// Time series of Greenhouse gas emissions in kgCO2eq/kWh
        /// </summary>
        public double [] GhgEmissions { get; protected set; }


        protected EnergyCarrier(int horizon, EnergyUnit unit, double [] energy, double [] energyCost, double [] ghgEmissions)
        {
            this.Horizon = horizon;
            this.Unit = unit;

            // energy could be unbound, e.g. electricity grid, in which case energy == null
            if (energy != null && energy.Length != 0)
            {
                this.Energy = new double[energy.Length];
                energy.CopyTo(this.Energy, 0);
            }

            // energy could be free, e.g. air, in which case energyCost == null
            if (energyCost != null && energyCost.Length != 0)
            {
                this.EnergyCost = new double[energyCost.Length];
                energyCost.CopyTo(this.EnergyCost, 0);
            }

            // energy could have zero emissions, e.g. air or solar, in which case ghgEmissions == null
            if (ghgEmissions != null && ghgEmissions.Length != 0)
            {
                this.GhgEmissions = new double[ghgEmissions.Length];
                ghgEmissions.CopyTo(this.GhgEmissions, 0);
            }
        }
    }
}