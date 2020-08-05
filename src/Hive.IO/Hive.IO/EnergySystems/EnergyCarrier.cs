using System.Runtime.InteropServices;
using Grasshopper.GUI;

namespace Hive.IO.EnergySystems
{
    #region implemented Energy Carriers
    /// <summary>
    /// Air energy carrier
    /// e.g. input for air source heat pump (in which case zero cost and emissions)
    /// or output from AirCon (in which case it has cost and emissions)
    /// </summary>
    public class Air : EnergyCarrier
    {
        public Air(int horizon, double[] airTemperature) 
            : base(horizon, EnergyCarrier.EnergyUnit.DegreeCelsius, airTemperature, null, null) { }
    }


    /// <summary>
    /// Radiation energy carrier, e.g. solar radiation for PV and solar thermal
    /// </summary>
    public class Radiation : EnergyCarrier
    {
        /// <summary>
        /// ID of the mesh vertex that this solar carrier corresponds to (solar panel). In case of Sun, ID can be ignored
        /// </summary>
        public int? MeshVertexId { get; }

        /// <summary>
        /// W/m2
        /// </summary>
        public double[] Irradiance { get; }

        public Radiation(int horizon, double[] irradiation, double surfaceArea = 1, int? vertexId = null)
            : base(horizon, EnergyCarrier.EnergyUnit.KiloWattHours, irradiation, null, null)
        {
            this.Irradiance = new double[irradiation.Length];
            for (int i = 0; i < Irradiance.Length; i++)
                this.Irradiance[i] = irradiation[i] / surfaceArea;

            if (vertexId.HasValue)
                this.MeshVertexId = vertexId;
        }
    }


    /// <summary>
    /// Water as energy carrier, e.g. the output of a boiler, heat pump, or solar thermal collector
    /// Could also be used for district heating/cooling
    /// </summary>
    public class Water : EnergyCarrier
    {
        public double[] SupplyTemperature { get; private set; }
        public Water(int horizon, double[] availableEnergy, double[] energyCost, double[] ghgEmissions,
            double[] supplyTemperature)
            : base(horizon, EnergyCarrier.EnergyUnit.KiloWattHours, availableEnergy, energyCost, ghgEmissions)
        {
            this.SupplyTemperature = new double[supplyTemperature.Length];
            supplyTemperature.CopyTo(this.SupplyTemperature, 0);
        }
    }


    /// <summary>
    /// Electricity. Could be from the grid or from a conversion technology
    /// </summary>
    public class Electricity : EnergyCarrier
    {
        public Electricity(int horizon, double [] availableElectricity, double[] energyCost, double[] ghgEmissions) 
            : base(horizon, EnergyCarrier.EnergyUnit.KiloWattHours, availableElectricity, energyCost, ghgEmissions) { }
    }


    /// <summary>
    /// BioGas. Availability might be limited
    /// </summary>
    public class BioGas : EnergyCarrier
    {
        public BioGas(int horizon, double[] availableBiogas, double[] energyCost, double[] ghgEmissions) 
            : base(horizon, EnergyCarrier.EnergyUnit.KiloWattHours, availableBiogas, energyCost, ghgEmissions)
        {
        }
    }


    /// <summary>
    /// 
    /// </summary>
    public class NaturalGas : EnergyCarrier
    {
        public NaturalGas(int horizon, double[] availableBiogas, double[] energyCost, double[] ghgEmissions)
            : base(horizon, EnergyCarrier.EnergyUnit.KiloWattHours, availableBiogas, energyCost, ghgEmissions)
        {
        }
    }



    /// <summary>
    /// Wood pellets. Availability might be limited
    /// </summary>
    public class Pellets : EnergyCarrier
    {
        public Pellets(int horizon, double[] availablePellets, double[] energyCost, double[] ghgEmissions) 
            : base(horizon, EnergyCarrier.EnergyUnit.KiloWattHours, availablePellets, energyCost, ghgEmissions)
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
            DegreeCelsius
        }
        /// <summary>
        /// Setting the energy unit from this.EnergyUnit
        /// </summary>
        public EnergyUnit Unit { get; protected set; }

        /// <summary>
        /// Horizon of time series data
        /// </summary>
        public int Horizon { get; protected set; }
        /// <summary>
        /// Time series of available/provided energy of this carrier in this.Unit per time step.
        /// Could be the potentials (solar) or the operation from a conversion technology
        /// </summary>
        public double [] AvailableEnergy { get; protected set; }
        /// <summary>
        /// Time series with cost coefficients per this.Unit
        /// </summary>
        public double [] EnergyCost { get; protected set; }
        /// <summary>
        /// Time series of Greenhouse gas emissions in kgCO2eq/this.Unit
        /// </summary>
        public double [] GhgEmissions { get; protected set; }


        protected EnergyCarrier(int horizon, EnergyUnit unit, double [] availableEnergy, double [] energyCost, double [] ghgEmissions)
        {
            this.Horizon = horizon;
            this.Unit = unit;

            // energy could be unbound, e.g. electricity grid, in which case energy == null
            if (availableEnergy != null && availableEnergy.Length != 0)
            {
                this.AvailableEnergy = new double[availableEnergy.Length];
                availableEnergy.CopyTo(this.AvailableEnergy, 0);
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