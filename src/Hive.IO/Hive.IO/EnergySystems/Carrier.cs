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
    public class Air : Carrier
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="horizon">Time horizon</param>
        /// <param name="airEnergy">kWh</param>
        /// <param name="airPrice">CHF/kWh</param>
        /// <param name="ghgEmissionsFactor">kgCO2eq./kWh</param>
        /// <param name="airTemperature">deg C</param>
        public Air(int horizon, double [] airEnergy, double [] airPrice, double [] ghgEmissionsFactor, double[] airTemperature)
            : base(horizon, Carrier.EnergyUnit.KiloWattHours, airEnergy, airPrice, ghgEmissionsFactor, 1.0) 
        {
            if (airTemperature != null && airTemperature.Length > 0)
            {
                base.Temperature = new double[airTemperature.Length];
                airTemperature.CopyTo(base.Temperature, 0);
            }
        }
    }


    /// <summary>
    /// Water as energy carrier, e.g. the output of a boiler, heat pump, or solar thermal collector
    /// Could also be used for district heating/cooling
    /// </summary>
    public class Water : Carrier
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="horizon">Time horizon</param>
        /// <param name="waterEnergy">kWh</param>
        /// <param name="waterPrice">CHF/kWh</param>
        /// <param name="ghgEmissionsFactor">kgCO2eq./kWh</param>
        /// <param name="waterTemperature">deg C</param>
        public Water(int horizon, double[] waterEnergy, double[] waterPrice, double[] ghgEmissionsFactor, double[] waterTemperature, double primaryEnergyFactor)
            : base(horizon, Carrier.EnergyUnit.KiloWattHours, waterEnergy, waterPrice, ghgEmissionsFactor, primaryEnergyFactor)
        {
            if (waterTemperature != null && waterTemperature.Length > 0)
            {
                base.Temperature = new double[waterTemperature.Length];
                waterTemperature.CopyTo(base.Temperature, 0);
            }
        }
    }


    /// <summary>
    /// Radiation energy carrier, e.g. solar radiation for PV and solar thermal
    /// </summary>
    public class Radiation : Carrier
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="horizon">Time horizon</param>
        /// <param name="irradiation">solar energy in kWh. If you only know solar irradiance in kWh/m2, use that and leave default 'surfaceArea = 1'. Then, irradiation = irradiance</param>
        /// <param name="surfaceArea"></param>
        /// <param name="vertexId"></param>
        /// <param name="radiationType"></param>
        public Radiation(int horizon, double[] irradiation, double surfaceArea = 1, int? vertexId = null, RadiationType radiationType = RadiationType.GHI)
            : base(horizon, Carrier.EnergyUnit.KiloWattHours, irradiation, null, null, 1.0)
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
    /// Electricity. Could be from the grid or from a conversion technology
    /// </summary>
    public class Electricity : Carrier
    {
        /// <summary>
        /// Electricity
        /// </summary>
        /// <param name="horizon">Time horizon</param>
        /// <param name="electricEnergy">kWh</param>
        /// <param name="electricityPrice">CHF/kWh</param>
        /// <param name="ghgEmissionsFactor">kgCO2eq./kWh</param>
        public Electricity(int horizon, double[] electricEnergy, double[] electricityPrice, double[] ghgEmissionsFactor, double primaryEnergyFactor)
            : base(horizon, Carrier.EnergyUnit.KiloWattHours, electricEnergy, electricityPrice, ghgEmissionsFactor, primaryEnergyFactor) { }
    }


    /// <summary>
    /// Gas (natural gas or biogas)
    /// </summary>
    public class Gas : Carrier
    {
        /// <summary>
        /// Gas
        /// </summary>
        /// <param name="horizon">Time horizon</param>
        /// <param name="gasEnergy">kWh</param>
        /// <param name="gasPrice">CHF/kWh</param>
        /// <param name="ghgEmissionsFactor">kgCO2eq./kWh</param>
        public Gas(int horizon, double[] gasEnergy, double[] gasPrice, double[] ghgEmissionsFactor, double primaryEnergyFactor)
            : base(horizon, Carrier.EnergyUnit.KiloWattHours, gasEnergy, gasPrice, ghgEmissionsFactor, primaryEnergyFactor) { }
    }



    /// <summary>
    /// Wood pellets. Availability might be limited
    /// </summary>
    public class Pellets : Carrier
    {
        /// <summary>
        /// Wood pellets
        /// </summary>
        /// <param name="horizon">Time horizon</param>
        /// <param name="pelletEnergy">Time-resolved pellets in kWh</param>
        /// <param name="pelletsPrice">price per kWh</param>
        /// <param name="ghgEmissionsFactor">kgCO2eq./kWh</param>
        public Pellets(int horizon, double[] pelletEnergy, double[] pelletsPrice, double[] ghgEmissionsFactor, double primaryEnergyFactor)
            : base(horizon, Carrier.EnergyUnit.KiloWattHours, pelletEnergy, pelletsPrice, ghgEmissionsFactor, primaryEnergyFactor) { }
    }
    #endregion



    /// <summary>
    /// Energy Carrier
    /// </summary>
    public abstract class Carrier
    {
        // specifying the carrier. e.g. for <Electricity>, name could be 'UTC-Grid', or for <Gas> it could be 'BioGasZurich' or 'NaturalGasRussia'
        public string Name { get; internal set; }

        /// <summary>
        /// Pre-defined energy units
        /// </summary>
        public enum EnergyUnit
        {
            KiloWattHours
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
        public double[] Energy { get; protected set; }
        /// <summary>
        /// Time series with cost coefficients per this.Unit
        /// </summary>
        public double[] EnergyPrice { get; protected set; }
        public double[] TotalEnergyCost { get; protected set; }
        /// <summary>
        /// Time series of Greenhouse gas emissions in kgCO2eq/this.Unit
        /// </summary>
        public double[] GhgEmissionsFactor { get; protected set; }
        public double[] TotalGhgEmissions { get; protected set; }


        public double PrimaryEnergyFactor { get; protected set; }


        /// <summary>
        /// Temperature of the medium in degree Celsius. Not all carriers will need to implement this
        /// </summary>
        public double [] Temperature { get; protected set; }

        protected Carrier(int horizon, EnergyUnit unit, double[] energy, double[] energyPrice, double[] ghgEmissionsFactor, double primaryEnergyFactor)
        {
            this.Horizon = horizon;
            this.Unit = unit;
            this.PrimaryEnergyFactor = primaryEnergyFactor;

            // energy could be unbound, e.g. electricity grid, in which case energy == null
            if (energy != null && energy.Length != 0)
            {
                this.Energy = new double[energy.Length];
                energy.CopyTo(this.Energy, 0);
            }
            else
            {
                this.Energy = new double[horizon].Select(x => 0.0).ToArray();
            }

            // energy could be free, e.g. air, in which case energyCost == null
            if (energyPrice != null && energyPrice.Length != 0)
            {
                this.EnergyPrice = new double[energyPrice.Length];
                energyPrice.CopyTo(this.EnergyPrice, 0);
            }
            else
            {
                this.EnergyPrice = new double[horizon].Select(x => 0.0).ToArray();
            }

            // energy could have zero emissions, e.g. air or solar, in which case ghgEmissions == null
            if (ghgEmissionsFactor != null && ghgEmissionsFactor.Length != 0)
            {
                this.GhgEmissionsFactor = new double[ghgEmissionsFactor.Length];
                ghgEmissionsFactor.CopyTo(this.GhgEmissionsFactor, 0);
            }
            else
            {
                this.GhgEmissionsFactor = new double[horizon].Select(x => 0.0).ToArray();
            }

            if(this.Energy != null && this.EnergyPrice != null && this.Energy.Length == this.EnergyPrice.Length)
            {
                int _horizon = this.Energy.Length;
                this.TotalEnergyCost = new double[_horizon];
                for (int t = 0; t < _horizon; t++)
                {
                    this.TotalEnergyCost[t] = this.Energy[t] * this.EnergyPrice[t];
                }
            }
            else
            {
                this.TotalEnergyCost = new double[horizon].Select(x=>0.0).ToArray();
            }

            if (this.Energy != null && this.GhgEmissionsFactor != null && this.Energy.Length == this.GhgEmissionsFactor.Length)
            {
                int _horizon = this.Energy.Length;
                this.TotalGhgEmissions = new double[_horizon];
                for (int t = 0; t < _horizon; t++)
                {
                    this.TotalGhgEmissions[t] = this.Energy[t] * this.GhgEmissionsFactor[t];
                }
            }
            else
            {
                this.TotalGhgEmissions = new double[horizon].Select(x=>0.0).ToArray();
            }
        }


        private double[] _monthlyCumulativeEmissions = null;
        public double[] MonthlyCumulativeEmissions
        {
            get
            {
                if (_monthlyCumulativeEmissions == null)
                    _monthlyCumulativeEmissions = Misc.GetCumulativeMonthlyValue(this.TotalGhgEmissions);
                return _monthlyCumulativeEmissions;
            }
            private set { ; }
        }


        private double[] _monthlyCumulativeEnergy = null;
        public double[] MonthlyCumulativeEnergy
        {
            get
            {
                if (_monthlyCumulativeEnergy == null)
                    _monthlyCumulativeEnergy = Misc.GetCumulativeMonthlyValue(this.Energy);
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
                    _monthlyAverageEnergy = Misc.GetAverageMonthlyValue(this.Energy);
                return _monthlyAverageEnergy;
            }
            private set { ; }
        }


        private double[] _monthlyTemperature = null;
        public double[] MonthlyTemperature
        {
            get
            {
                if (_monthlyTemperature == null)
                    _monthlyTemperature = Misc.GetAverageMonthlyValue(this.Temperature);
                return _monthlyTemperature;
            }
            private set {; }
        }

    }
}