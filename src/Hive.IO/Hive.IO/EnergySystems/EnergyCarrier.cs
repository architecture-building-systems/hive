using System.Runtime.InteropServices;
using Grasshopper.GUI;
using System.Linq;
using Rhino.Render;

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

        private double[] _monthlySupplyTemperature = null;
        public double[] MonthlySupplyTemperature
        {
            get
            {
                if (_monthlySupplyTemperature == null)
                    _monthlySupplyTemperature = Misc.GetAverageMonthlyValue(this.AvailableEnergy);
                return _monthlySupplyTemperature;
            }
            private set {; }
        }
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

        public enum RadiationType
        {
            GHI,
            DNI,
            DHI
        }

        public RadiationType Description { get; private set; }


        public Radiation(int horizon, double[] irradiation, double surfaceArea = 1, int? vertexId = null, RadiationType radiationType = RadiationType.GHI)
            : base(horizon, EnergyCarrier.EnergyUnit.KiloWattHours, irradiation, null, null)
        {
            this.Irradiance = new double[irradiation.Length];
            for (int i = 0; i < Irradiance.Length; i++)
                this.Irradiance[i] = irradiation[i] / surfaceArea;

            if (vertexId.HasValue)
                this.MeshVertexId = vertexId;

            if (radiationType != RadiationType.GHI)
                this.Description = radiationType;

        }
    }


    /// <summary>
    /// Water as energy carrier, e.g. the output of a boiler, heat pump, or solar thermal collector
    /// Could also be used for district heating/cooling
    /// </summary>
    public class Water : EnergyCarrier
    {
        public double[] SupplyTemperature { get; private set; } //8760
        public Water(int horizon, double[] availableEnergy, double[] energyCost, double[] ghgEmissions,
            double[] supplyTemperature)
            : base(horizon, EnergyCarrier.EnergyUnit.KiloWattHours, availableEnergy, energyCost, ghgEmissions)
        {
            this.SupplyTemperature = new double[supplyTemperature.Length];
            supplyTemperature.CopyTo(this.SupplyTemperature, 0);
        }


        private double[] _monthlySupplyTemperature = null;
        public double[] MonthlySupplyTemperature
        {
            get
            {
                if (_monthlySupplyTemperature == null)
                    _monthlySupplyTemperature = Misc.GetAverageMonthlyValue(this.SupplyTemperature);
                return _monthlySupplyTemperature;
            }
            private set { ; }
        }

    }


    /// <summary>
    /// Electricity. Could be from the grid or from a conversion technology
    /// </summary>
    public class Electricity : EnergyCarrier
    {
        public Electricity(int horizon, double[] availableElectricity, double[] energyCost, double[] ghgEmissions)
            : base(horizon, EnergyCarrier.EnergyUnit.KiloWattHours, availableElectricity, energyCost, ghgEmissions) { }

    }


    /// <summary>
    /// Gas (natural gas or biogas)
    /// </summary>
    public class Gas : EnergyCarrier
    {
        public Gas(int horizon, double[] availableGas, double[] energyCost, double[] ghgEmissions)
            : base(horizon, EnergyCarrier.EnergyUnit.KiloWattHours, availableGas, energyCost, ghgEmissions)
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
        // specifying the carrier. e.g. for <Electricity>, name could be 'UTC-Grid', or for <Gas> it could be 'BioGasZurich' or 'NaturalGasRussia'
        public string Name { get; internal set; }

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
        public double[] AvailableEnergy { get; protected set; }
        /// <summary>
        /// Time series with cost coefficients per this.Unit
        /// </summary>
        public double[] EnergyCost { get; protected set; }
        /// <summary>
        /// Time series of Greenhouse gas emissions in kgCO2eq/this.Unit
        /// </summary>
        public double[] GhgEmissions { get; protected set; }


        protected EnergyCarrier(int horizon, EnergyUnit unit, double[] availableEnergy, double[] energyCost, double[] ghgEmissions)
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


        private double[] _monthlyCumulativeEnergy = null;
        public double[] MonthlyCumulativeEnergy
        {
            get
            {
                if (_monthlyCumulativeEnergy == null)
                    _monthlyCumulativeEnergy = Misc.GetCumulativeMonthlyValue(this.AvailableEnergy);
                return _monthlyCumulativeEnergy;
            }
            private set { ; }
        }

        private double[] _monthlyAverageEnergy = null;
        public double [] MonthlyAverageEnergy
        {
            get
            {
                if (_monthlyAverageEnergy == null)
                    _monthlyAverageEnergy = Misc.GetAverageMonthlyValue(this.AvailableEnergy);
                return _monthlyAverageEnergy;
            }
            private set { ; }
        }


        
    }
}