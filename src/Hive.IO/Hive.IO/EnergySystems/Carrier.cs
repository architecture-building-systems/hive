using System.Linq;
using Newtonsoft.Json;

namespace Hive.IO.EnergySystems
{
    #region implemented Energy Carriers

    /// <summary>
    ///     Air energy carrier
    ///     e.g. input for air source heat pump (in which case zero cost and emissions)
    ///     or output from AirCon (in which case it has cost and emissions)
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class Air : Carrier
    {
        /// <summary>
        /// </summary>
        /// <param name="horizon">Time horizon</param>
        /// <param name="airEnergy">kWh</param>
        /// <param name="airPrice">CHF/kWh</param>
        /// <param name="ghgEmissionsFactor">kgCO2eq./kWh</param>
        /// <param name="airTemperature">deg C</param>
        public Air(int horizon, double[] airEnergy, double[] airPrice, double[] ghgEmissionsFactor,
            double[] airTemperature)
            : base(horizon, EnergyUnit.KiloWattHours, airEnergy, airPrice, ghgEmissionsFactor, 1.0)
        {
            if (airTemperature != null && airTemperature.Length > 0)
            {
                Temperature = new double[airTemperature.Length];
                airTemperature.CopyTo(Temperature, 0);
            }
        }

        protected Air()
        {
        }
    }


    /// <summary>
    ///     Water as energy carrier, e.g. the output of a boiler, heat pump, or solar thermal collector
    ///     Could also be used for district heating/cooling
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class Water : Carrier
    {
        /// <summary>
        /// </summary>
        /// <param name="horizon">Time horizon</param>
        /// <param name="waterEnergy">kWh</param>
        /// <param name="waterPrice">CHF/kWh</param>
        /// <param name="ghgEmissionsFactor">kgCO2eq./kWh</param>
        /// <param name="waterTemperature">deg C</param>
        public Water(int horizon, double[] waterEnergy, double[] waterPrice, double[] ghgEmissionsFactor,
            double[] waterTemperature, double primaryEnergyFactor)
            : base(horizon, EnergyUnit.KiloWattHours, waterEnergy, waterPrice, ghgEmissionsFactor, primaryEnergyFactor)
        {
            if (waterTemperature != null && waterTemperature.Length > 0)
            {
                Temperature = new double[waterTemperature.Length];
                waterTemperature.CopyTo(Temperature, 0);
            }
        }

        protected Water()
        {
        }
    }


    /// <summary>
    ///     Radiation energy carrier, e.g. solar radiation for PV and solar thermal
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class Radiation : Carrier
    {
        public enum RadiationType
        {
            GHI,
            DNI,
            DHI
        }

        /// <summary>
        /// </summary>
        /// <param name="horizon">Time horizon</param>
        /// <param name="irradiation">
        ///     solar energy in kWh. If you only know solar irradiance in kWh/m2, use that and leave default
        ///     'surfaceArea = 1'. Then, irradiation = irradiance
        /// </param>
        /// <param name="surfaceArea"></param>
        /// <param name="vertexId"></param>
        /// <param name="radiationType"></param>
        public Radiation(int horizon, double[] irradiation, double surfaceArea = 1, int? vertexId = null,
            RadiationType radiationType = RadiationType.GHI)
            : base(horizon, EnergyUnit.KiloWattHours, irradiation, null, null, 1.0)
        {
            Irradiance = new double[irradiation.Length];
            for (var i = 0; i < Irradiance.Length; i++)
                Irradiance[i] = irradiation[i] / surfaceArea;

            if (vertexId.HasValue)
                MeshVertexId = vertexId;

            if (radiationType != RadiationType.GHI)
                Description = radiationType;
        }

        protected Radiation()
        {
        }

        /// <summary>
        ///     ID of the mesh vertex that this solar carrier corresponds to (solar panel). In case of Sun, ID can be ignored
        /// </summary>
        [JsonProperty]
        public int? MeshVertexId { get; }

        /// <summary>
        ///     W/m2
        /// </summary>
        [JsonProperty]
        public double[] Irradiance { get; }

        [JsonProperty] public RadiationType Description { get; private set; }
    }


    /// <summary>
    ///     Electricity. Could be from the grid or from a conversion technology
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class Electricity : Carrier
    {
        /// <summary>
        ///     Electricity
        /// </summary>
        /// <param name="horizon">Time horizon</param>
        /// <param name="electricEnergy">kWh</param>
        /// <param name="electricityPrice">CHF/kWh</param>
        /// <param name="ghgEmissionsFactor">kgCO2eq./kWh</param>
        public Electricity(int horizon, double[] electricEnergy, double[] electricityPrice, double[] ghgEmissionsFactor,
            double primaryEnergyFactor)
            : base(horizon, EnergyUnit.KiloWattHours, electricEnergy, electricityPrice, ghgEmissionsFactor,
                primaryEnergyFactor)
        {
        }

        protected Electricity()
        {
        }
    }


    /// <summary>
    ///     Gas (natural gas or biogas)
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class Gas : Carrier
    {
        /// <summary>
        ///     Gas
        /// </summary>
        /// <param name="horizon">Time horizon</param>
        /// <param name="gasEnergy">kWh</param>
        /// <param name="gasPrice">CHF/kWh</param>
        /// <param name="ghgEmissionsFactor">kgCO2eq./kWh</param>
        public Gas(int horizon, double[] gasEnergy, double[] gasPrice, double[] ghgEmissionsFactor,
            double primaryEnergyFactor)
            : base(horizon, EnergyUnit.KiloWattHours, gasEnergy, gasPrice, ghgEmissionsFactor, primaryEnergyFactor)
        {
        }

        protected Gas()
        {
        }
    }


    /// <summary>
    ///     Wood pellets. Availability might be limited
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class Pellets : Carrier
    {
        /// <summary>
        ///     Wood pellets
        /// </summary>
        /// <param name="horizon">Time horizon</param>
        /// <param name="pelletEnergy">Time-resolved pellets in kWh</param>
        /// <param name="pelletsPrice">price per kWh</param>
        /// <param name="ghgEmissionsFactor">kgCO2eq./kWh</param>
        public Pellets(int horizon, double[] pelletEnergy, double[] pelletsPrice, double[] ghgEmissionsFactor,
            double primaryEnergyFactor)
            : base(horizon, EnergyUnit.KiloWattHours, pelletEnergy, pelletsPrice, ghgEmissionsFactor,
                primaryEnergyFactor)
        {
        }

        protected Pellets()
        {
        }
    }

    #endregion


    /// <summary>
    ///     Energy Carrier
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public abstract class Carrier
    {
        /// <summary>
        ///     Pre-defined energy units
        /// </summary>
        public enum EnergyUnit
        {
            KiloWattHours
        }

        private double[] _monthlyAverageEnergy;


        private double[] _monthlyCumulativeEmissions;


        private double[] _monthlyCumulativeEnergy;


        private double[] _monthlyTemperature;

        protected Carrier()
        {
        }

        protected Carrier(int horizon, EnergyUnit unit, double[] energy, double[] energyPrice,
            double[] ghgEmissionsFactor, double primaryEnergyFactor)
        {
            Horizon = horizon;
            Unit = unit;
            PrimaryEnergyFactor = primaryEnergyFactor;

            // energy could be unbound, e.g. electricity grid, in which case energy == null
            if (energy != null && energy.Length != 0)
            {
                Energy = new double[energy.Length];
                energy.CopyTo(Energy, 0);
            }
            else
            {
                Energy = new double[horizon].Select(x => 0.0).ToArray();
            }

            // energy could be free, e.g. air, in which case energyCost == null
            if (energyPrice != null && energyPrice.Length != 0)
            {
                EnergyPrice = new double[energyPrice.Length];
                energyPrice.CopyTo(EnergyPrice, 0);
            }
            else
            {
                EnergyPrice = new double[horizon].Select(x => 0.0).ToArray();
            }

            // energy could have zero emissions, e.g. air or solar, in which case ghgEmissions == null
            if (ghgEmissionsFactor != null && ghgEmissionsFactor.Length != 0)
            {
                GhgEmissionsFactor = new double[ghgEmissionsFactor.Length];
                ghgEmissionsFactor.CopyTo(GhgEmissionsFactor, 0);
            }
            else
            {
                GhgEmissionsFactor = new double[horizon].Select(x => 0.0).ToArray();
            }

            if (Energy != null && EnergyPrice != null && Energy.Length == EnergyPrice.Length)
            {
                var _horizon = Energy.Length;
                TotalEnergyCost = new double[_horizon];
                for (var t = 0; t < _horizon; t++) TotalEnergyCost[t] = Energy[t] * EnergyPrice[t];
            }
            else
            {
                TotalEnergyCost = new double[horizon].Select(x => 0.0).ToArray();
            }

            if (Energy != null && GhgEmissionsFactor != null && Energy.Length == GhgEmissionsFactor.Length)
            {
                var _horizon = Energy.Length;
                TotalGhgEmissions = new double[_horizon];
                for (var t = 0; t < _horizon; t++) TotalGhgEmissions[t] = Energy[t] * GhgEmissionsFactor[t];
            }
            else
            {
                TotalGhgEmissions = new double[horizon].Select(x => 0.0).ToArray();
            }
        }

        // specifying the carrier. e.g. for <Electricity>, name could be 'UTC-Grid', or for <Gas> it could be 'BioGasZurich' or 'NaturalGasRussia'
        [JsonProperty] public string Name { get; internal set; }

        /// <summary>
        ///     Setting the energy unit from this.EnergyUnit
        /// </summary>
        [JsonProperty]
        public EnergyUnit Unit { get; protected set; }

        /// <summary>
        ///     Horizon of time series data
        /// </summary>
        [JsonProperty]
        public int Horizon { get; protected set; }

        /// <summary>
        ///     Time series of available/provided energy of this carrier in this.Unit per time step.
        ///     Could be the potentials (solar) or the operation from a conversion technology
        /// </summary>
        [JsonProperty]
        public double[] Energy { get; protected set; }

        /// <summary>
        ///     Time series with cost coefficients per this.Unit
        /// </summary>
        [JsonProperty]
        public double[] EnergyPrice { get; protected set; }

        [JsonProperty] public double[] TotalEnergyCost { get; protected set; }

        /// <summary>
        ///     Time series of Greenhouse gas emissions in kgCO2eq/this.Unit
        /// </summary>
        [JsonProperty]
        public double[] GhgEmissionsFactor { get; protected set; }

        [JsonProperty] public double[] TotalGhgEmissions { get; protected set; }


        [JsonProperty] public double PrimaryEnergyFactor { get; protected set; }


        /// <summary>
        ///     Temperature of the medium in degree Celsius. Not all carriers will need to implement this
        /// </summary>
        [JsonProperty]
        public double[] Temperature { get; protected set; }

        [JsonProperty]
        public double[] MonthlyCumulativeEmissions
        {
            get
            {
                if (_monthlyCumulativeEmissions == null)
                    _monthlyCumulativeEmissions = Misc.GetCumulativeMonthlyValue(TotalGhgEmissions);
                return _monthlyCumulativeEmissions;
            }
            private set => _monthlyCumulativeEmissions = value;
        }

        [JsonProperty]
        public double[] MonthlyCumulativeEnergy
        {
            get
            {
                if (_monthlyCumulativeEnergy == null)
                    _monthlyCumulativeEnergy = Misc.GetCumulativeMonthlyValue(Energy);
                return _monthlyCumulativeEnergy;
            }
            private set => _monthlyCumulativeEmissions = value;
        }

        [JsonProperty]
        public double[] MonthlyAverageEnergy
        {
            get
            {
                if (_monthlyAverageEnergy == null)
                    _monthlyAverageEnergy = Misc.GetAverageMonthlyValue(Energy);
                return _monthlyAverageEnergy;
            }
            private set => _monthlyAverageEnergy = value;
        }

        [JsonProperty]
        public double[] MonthlyTemperature
        {
            get
            {
                if (_monthlyTemperature == null)
                    _monthlyTemperature = Misc.GetAverageMonthlyValue(Temperature);
                return _monthlyTemperature;
            }
            private set => _monthlyTemperature = value;
        }
    }
}