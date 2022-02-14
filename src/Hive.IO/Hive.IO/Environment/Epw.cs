using Hive.IO.EnergySystems;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Hive.IO.Environment
{
    public class Epw
    {
        public string FilePath;

        public string City;
        public string Country;
        public double TimeZone;
        public double Elevation;
        public double? Longitude;
        public double? Latitude;

        public double[] GHI;
        public double[] DNI;
        public double[] DHI;
        public double[] DryBulbTemperature;
        public double[] DewPointTemperature;
        public double[] RelativeHumidity;
        //public double[] WindSpeed;
        //public double[] WindDirection;
        //public double[] Precipitation;
        //public double[] Snowfall;

        public double[] GHIMonthly;
        public double[] DNIMonthly;
        public double[] DHIMonthly;
        public double[] DryBulbTemperatureMonthly;
        public double[] RelativeHumidityMonthly;

        public Air AmbientTemperatureCarrier;

        public Epw()
        {

        }

        public bool Parse()
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

            // Check path
            if (Path.GetExtension(FilePath) != ".epw" || !File.Exists(@FilePath)) throw new FileNotFoundException("Could not find EPW file at: " + FilePath);

            // .epw format:
            const int DryBulbIndex = 6;
            const int DewPointIndex = 7;
            const int GhiIndex = 13;
            const int DniIndex = 14;
            const int DhiIndex = 15;
            const int RhIndex = 8;

            const int CityIndex = 1;
            const int CountryIndex = 3;
            const int LatitudeIndex = 6;
            const int LongitutdeIndex = 7;
            const int TimeZoneIndex = 8;
            const int ElevationIndex = 9;

            var dryBulb = new List<double>();
            var dewPoint = new List<double>();
            var ghi = new List<double>();
            var dni = new List<double>();
            var dhi = new List<double>();
            var rh = new List<double>();


            using (var reader = new StreamReader(@FilePath))
            {
                int counter = 0;
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');
                    double temp;
                    if (counter == 0 && string.Equals(values[0], "LOCATION"))
                    {
                        City = values[CityIndex];
                        Country = values[CountryIndex];
                        Latitude = Convert.ToDouble(values[LatitudeIndex]);
                        Longitude = Convert.ToDouble(values[LongitutdeIndex]);
                        Elevation = Convert.ToDouble(values[ElevationIndex]);
                        TimeZone = Convert.ToDouble(values[TimeZoneIndex]);
                    }
                    else if (double.TryParse(values[0], out temp))
                    {
                        dryBulb.Add(Convert.ToDouble(values[DryBulbIndex]));
                        dewPoint.Add(Convert.ToDouble(values[DewPointIndex]));
                        ghi.Add(Convert.ToDouble(values[GhiIndex]));
                        dni.Add(Convert.ToDouble(values[DniIndex]));
                        dhi.Add(Convert.ToDouble(values[DhiIndex]));
                        rh.Add(Convert.ToDouble(values[RhIndex]));
                    }

                    counter++;
                }
            }

            // let's just be sure we actually read an .epw file with a LOCATION entry
            if (Latitude == null || Longitude == null) throw new InvalidDataException("EPW file does not have valid location. Path: " + FilePath);

            DryBulbTemperature = dryBulb.ToArray();
            DewPointTemperature = dewPoint.ToArray();
            GHI = ghi.ToArray();
            DHI = dhi.ToArray();
            DNI = dni.ToArray();
            RelativeHumidity = rh.ToArray();

            // monthly data
            GHIMonthly = new double[Misc.MonthsPerYear];
            DryBulbTemperatureMonthly = new double[Misc.MonthsPerYear];
            RelativeHumidityMonthly = new double[Misc.MonthsPerYear];

            foreach (var month in Enumerable.Range(0, Misc.MonthsPerYear))
            {
                int startHour = Misc.HoursPerMonth.Slice(0, month).Sum();
                int endHour = Misc.HoursPerMonth.Slice(0, month + 1).Sum();
                GHIMonthly[month] = GHI.Slice(startHour, endHour).Sum() / 1000;
                DryBulbTemperatureMonthly[month] = DryBulbTemperature.Slice(startHour, endHour).Sum() / Misc.HoursPerMonth[month];
                RelativeHumidityMonthly[month] = RelativeHumidity.Slice(startHour, endHour).Sum() / Misc.HoursPerMonth[month];
            }

            AmbientTemperatureCarrier = new Air(Misc.HoursPerYear, null, null, null, DryBulbTemperature.ToArray());

            return true;
        }
    }
}
