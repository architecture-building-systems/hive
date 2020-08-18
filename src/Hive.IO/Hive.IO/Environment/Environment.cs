using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using rg = Rhino.Geometry;
using Hive.IO.EnergySystems;

namespace Hive.IO.Environment
{
    /// <summary>
    /// Environment class, defining the environment the building is embedded in. 
    /// Contains climate data, location, adjacent obstacles and trees, etc.
    /// Grid access and renewable potentials as well?
    /// </summary>
    public class Environment
    {
        public Climate ClimateData { get; private set; }
        public Location LocationData { get; private set; }

        public string EpwPath { get; private set; }

        public rg.Mesh[] Geometry { get; private set; }

        /// <summary>
        /// Irradiation, ambient air, etc.
        /// </summary>
        public EnergyCarrier [] EnergyPotentials { get; private set; }


        public Environment(string epwFilePath, rg.Mesh [] geometry = null)
        {
            this.Geometry = geometry;
            this.EpwPath = epwFilePath;
        }

        /// <summary>
        /// parse data from an epw into LocationData and ClimateData
        /// </summary>
        private void ReadEPW()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        private void SetLocation()
        {

        }

        private void SetClimate()
        {

        }


        // setting all inpuit energy carriers as part of Hive.IO.Environment
        // gas, oil, biogas, wood, district heating, electricity grid, etc.
        internal void SetEnergyPotentials(EnergyCarrier [] inputCarriers)
        {
            this.EnergyPotentials = new EnergyCarrier[inputCarriers.Length];
            inputCarriers.CopyTo(this.EnergyPotentials, 0);
        }



        /// <summary>
        /// Location properties
        /// </summary>
        public struct Location
        {
            public int TimeZone;
            public double Altitude;
            public double Longitude;
            public double Latitude;
        }

        /// <summary>
        /// Climate properties
        /// </summary>
        public struct Climate
        {
            public double[] GHI;
            public double[] DNI;
            public double[] DHI;
            public double[] DryBulbTemperature;
            public double[] RelativeHumidity;
            public double[] WindSpeed;
            public double[] WindDirection;
            public double[] Precipitation;
            public double[] Snowfall;
        }

    }
}
