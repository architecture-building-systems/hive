using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using rg = Rhino.Geometry;
using Hive.IO.EnergySystems;
using System.IO;

namespace Hive.IO.Environment
{
    /// <summary>
    /// Environment class, defining the environment the building is embedded in. 
    /// Contains climate data, location, adjacent obstacles and trees, etc.
    /// Grid access and renewable potentials as well?
    /// </summary>
    public class Environment
    {
        public readonly int totalPotentials = 10; // gas, biogas, wood, dh, dc, grid, ambientair, ghi, dni, dhi
        public readonly int Horizon = Misc.HoursPerYear;

        public Climate ClimateData; 
        public Location LocationData;


        public string EpwPath { get; private set; }

        public rg.Mesh[] Geometry { get; private set; }

        /// <summary>
        /// Irradiation, ambient air, etc.
        /// </summary>
        public Carrier[] EnergyPotentials { get; private set; }


        public Environment(string epwFilePath, rg.Mesh[] geometry = null)
        {
            this.Geometry = geometry;
            this.EpwPath = epwFilePath;

            Environment.ReadEPW(EpwPath, this); // initializing ClimateData.. and Location
        }

        /// <summary>
        /// parse data from an epw into LocationData and ClimateData
        /// </summary>
        internal static void ReadEPW(string path, Environment environment)
        {
            //Import an epw file and output timeseries for each physical quantity

            //format of an EPW: https://energyplus.net/sites/all/modules/custom/nrel_custom/pdfs/pdfs_v9.2.0/AuxiliaryPrograms.pdf

            //It's a .csv with following format:

            //LOCATION, row [0]:
            //[0] header (i.e. 'LOCATION'), [1] City, [2] State, [3] Country, [4] Source, [5] WMO (6-digit code),
            //[6] Latitude [deg] -90.0 to +90.0, [7] Longitude [deg] -180.0 to 180.0,
            //[8] Timezone -12 to 12, [9] Elevation [m]

            //Starting at row [8]:
            //[0] Year, [1] Month, [2] Day, [3] Hour, [4] Minute, [5] ???,
            //[6] Dry Bulb [deg C], [7] Dew Point [deg C], [8] Rel. hum. [%], [9] Atm. Station Press. [Pa],
            //[10] Extraterr. Hor. Rad. [Wh/m2], [11] Extraterr. Direct Normal Rad. [Wh/m2],
            //[12] Hor. Infrared Rad. Intensity [Wh/m2], [13] Global Hor. Rad. [Wh/m2],
            //[14] Dir. Norm. Rad. [Wh/m2], [15] Diff. Hor. Rad. [Wh/m2],
            //[16] Glob. Hor. Ill. [lux], [17] Dir. Norm. Ill. [lux], [18] Diff. Hor. Ill. [lux], [19] Zenith Luminance [Cd/m2],
            //[20] Wind Dir. [deg], [21] Wind Speed [m/s],
            //[22] Total Sky Cover, [23] Opaque Sky Cover, [24] Visibility [km], [25] Ceiling Height [m],
            //[26] Present Weather Observation, [27] Present Weather Codes,
            //[28] Precipitable Water [mm], [29] Aerosol Optical depth [0.001],
            //[30] Snow Depth [cm], [31] Days Since Last Snowfall, [32] Albedo,
            //[33] Liquid Precipitation Depth [mm], [34] Liquid Precipitation Quantity [hr]


            // .epw format:
            const int DryBulbIndex = 6;
            const int GhiIndex = 13;
            const int DniIndex = 14;
            const int DhiIndex = 15;
            const int RhIndex = 8;

            var dryBulb = new List<double>();
            var ghi = new List<double>();
            var dni = new List<double>();
            var dhi = new List<double>();
            var rh = new List<double>();


            using (var reader = new StreamReader(@environment.EpwPath))
            {
                int counter = 0;
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');
                    double temp;
                    if (counter == 0 && String.Equals(values[0], "LOCATION"))
                    {
                        environment.LocationData.City = values[1];
                        environment.LocationData.Country = values[3];
                        environment.LocationData.Latitude = Convert.ToDouble(values[5]);
                        environment.LocationData.Longitude = Convert.ToDouble(values[6]);
                        environment.LocationData.Elevation = Convert.ToDouble(values[9]);
                        environment.LocationData.TimeZone = Convert.ToDouble(values[8]);
                    }
                    else if (double.TryParse(values[0], out temp))
                    {
                        dryBulb.Add(Convert.ToDouble(values[DryBulbIndex]));
                        ghi.Add(Convert.ToDouble(values[GhiIndex]));
                        dni.Add(Convert.ToDouble(values[DniIndex]));
                        dhi.Add(Convert.ToDouble(values[DhiIndex]));
                        rh.Add(Convert.ToDouble(values[RhIndex]));
                    }

                    counter++;
                }
            }

            environment.ClimateData.DryBulbTemperature = dryBulb.ToArray();
            environment.ClimateData.GHI = ghi.ToArray();
            environment.ClimateData.DHI = dhi.ToArray();
            environment.ClimateData.DNI = dni.ToArray();
            environment.ClimateData.RelativeHumidity = rh.ToArray();
        }




        // setting all inpuit energy carriers as part of Hive.IO.Environment
        // gas, oil, biogas, wood, district heating, electricity grid, etc.
        internal void SetEnergyPotentials(Carrier[] inputCarriers)
        {
            this.EnergyPotentials = new Carrier[this.totalPotentials];
            inputCarriers.CopyTo(this.EnergyPotentials, 0);
            this.EnergyPotentials[6] = new Air(this.Horizon, null, null, null, this.ClimateData.DryBulbTemperature);
            this.EnergyPotentials[6].Name = "DryBulbTemperature";
            this.EnergyPotentials[7] = new Radiation(this.Horizon, this.ClimateData.GHI);
            this.EnergyPotentials[7].Name = "GHI"; // redundant, because <Radiation> has a enum for GHI
            this.EnergyPotentials[8] = new Radiation(this.Horizon, this.ClimateData.DNI, 1, null, Radiation.RadiationType.DNI);
            this.EnergyPotentials[8].Name = "DNI";
            this.EnergyPotentials[9] = new Radiation(this.Horizon, this.ClimateData.DHI, 1, null, Radiation.RadiationType.DHI);
            this.EnergyPotentials[9].Name = "DHI";
        }


        internal void SetDefaultEnergyPotentials()
        {
            this.EnergyPotentials = new Carrier[this.totalPotentials];
            this.EnergyPotentials[0] = new Gas(this.Horizon, Enumerable.Repeat<double>(double.MaxValue, this.Horizon).ToArray(), Enumerable.Repeat<double>(0.09, this.Horizon).ToArray(), Enumerable.Repeat<double>(0.237, this.Horizon).ToArray(), Misc.PEFNaturalGas);
            this.EnergyPotentials[0].Name = "NaturalGas";
            this.EnergyPotentials[1] = new Gas(this.Horizon, Enumerable.Repeat<double>(double.MaxValue, this.Horizon).ToArray(), Enumerable.Repeat<double>(0.15, this.Horizon).ToArray(), Enumerable.Repeat<double>(0.15, this.Horizon).ToArray(), Misc.PEFBioGas);
            this.EnergyPotentials[1].Name = "BioGas";
            this.EnergyPotentials[2] = new Pellets(this.Horizon, Enumerable.Repeat<double>(double.MaxValue, this.Horizon).ToArray(), Enumerable.Repeat<double>(0.2, this.Horizon).ToArray(), Enumerable.Repeat<double>(0.1, this.Horizon).ToArray(), Misc.PEFWoodPellets);
            this.EnergyPotentials[2].Name = "ZueriWald";
            this.EnergyPotentials[3] = new Water(this.Horizon, Enumerable.Repeat<double>(double.MaxValue, this.Horizon).ToArray(), Enumerable.Repeat<double>(0.09, this.Horizon).ToArray(), Enumerable.Repeat<double>(0.237, this.Horizon).ToArray(), Enumerable.Repeat<double>(65.0, this.Horizon).ToArray(), 1.0);
            this.EnergyPotentials[3].Name = "DistrictHeating";
            this.EnergyPotentials[4] = new Water(this.Horizon, Enumerable.Repeat<double>(double.MaxValue, this.Horizon).ToArray(), Enumerable.Repeat<double>(0.09, this.Horizon).ToArray(), Enumerable.Repeat<double>(0.237, this.Horizon).ToArray(), Enumerable.Repeat<double>(15.0, this.Horizon).ToArray(), 1.0);
            this.EnergyPotentials[4].Name = "DistrictCooling";
            this.EnergyPotentials[5] = new Electricity(this.Horizon, Enumerable.Repeat<double>(double.MaxValue, this.Horizon).ToArray(), Enumerable.Repeat<double>(0.12, this.Horizon).ToArray(), Enumerable.Repeat<double>(0.597, this.Horizon).ToArray(), Misc.PEFElectricitySwiss);
            this.EnergyPotentials[5].Name = "UCT-Mix";
            this.EnergyPotentials[6] = new Air(this.Horizon, null, null, null, this.ClimateData.DryBulbTemperature);
            this.EnergyPotentials[6].Name = "DryBulbTemperature";
            this.EnergyPotentials[7] = new Radiation(this.Horizon, this.ClimateData.GHI);
            this.EnergyPotentials[7].Name = "GHI"; // redundant, because <Radiation> has a enum for GHI
            this.EnergyPotentials[8] = new Radiation(this.Horizon, this.ClimateData.DNI, 1, null, Radiation.RadiationType.DNI);
            this.EnergyPotentials[8].Name = "DNI";
            this.EnergyPotentials[9] = new Radiation(this.Horizon, this.ClimateData.DHI, 1, null, Radiation.RadiationType.DHI);
            this.EnergyPotentials[9].Name = "DHI";
        }


        /// <summary>
        /// Location properties
        /// </summary>
        public struct Location
        {
            public string City;
            public string Country;
            public double TimeZone;
            public double Elevation;
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
            //public double[] WindSpeed;
            //public double[] WindDirection;
            //public double[] Precipitation;
            //public double[] Snowfall;
        }

    }
}
