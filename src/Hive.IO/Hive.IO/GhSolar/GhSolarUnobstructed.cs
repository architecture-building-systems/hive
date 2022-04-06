using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Grasshopper.Kernel;

using sm = SolarModel;

namespace Hive.IO.GhSolar
{
    public class GhSolarUnobstructed : GH_Component
    {
        public GhSolarUnobstructed()
          : base("Solar Potentials Unobstructed C#", "SolarPotentialsUnobstructed",
              "Calculates solar irradiance on an unobstructed plane, using GhSolar.gha and SolarModel.dll (https://github.com/christophwaibel/GH_Solar_V2)",
              "[hive]", "Solar C#")
        {
        }

        public override GH_Exposure Exposure => GH_Exposure.secondary;

        #region defaults
        const double LatitudeDefault = 47.36667;
        const double LongitudeDefault = 8.55;
        const double AwDefault= 1.0;
        const int TimezoneDefault = 0;
        #endregion defaults


        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("Tilt", "Tilt", "Panel tilt in degree", GH_ParamAccess.item);
            pManager.AddNumberParameter("Azimuth", "Azimuth", "Panel azimuth in degree", GH_ParamAccess.item);
            pManager.AddNumberParameter("DHI", "DHI", "Diffuse Horizontal Irradiance time series, 8760 values in [W/m²] from a weatherfile.", GH_ParamAccess.list);
            pManager.AddNumberParameter("DNI", "DNI", "Direct Normal Irradiance time series, 8760 values in [W/m²] from a weatherfile.", GH_ParamAccess.list);
            
            pManager.AddNumberParameter("Latitude", "Latitude", "Latitude of the location. Default value is for Zurich, Switzerland", GH_ParamAccess.item, LatitudeDefault);
            pManager[4].Optional = true;
            pManager.AddNumberParameter("Longitude", "Longitude", "Longitude of the location. Default value is for Zurich, Switzerland", GH_ParamAccess.item, LongitudeDefault);
            pManager[5].Optional = true;
            pManager.AddNumberParameter("SolarAzi", "Solarazi", "8760 time series of solar azimuth angles in [°], e.g from a weatherfile. If no data provided, azimuth will be calculated according to Blanco-Muriel (2001).", GH_ParamAccess.list, new List<double>());
            pManager[6].Optional = true;
            pManager.AddNumberParameter("SolarAlti", "Solaralti", "8760 time series of solar altitude angles in [°], e.g from a weatherfile. If no data provided, altitude will be calculated according to Blanco-Muriel (2001).", GH_ParamAccess.list, new List<double>());
            pManager[7].Optional = true;
            pManager.AddNumberParameter("Aw", "Aw", "Panel surface area. optional", GH_ParamAccess.item, AwDefault);
            pManager[8].Optional = true;
            pManager.AddIntegerParameter("Timezone", "Timezone", "timezone?", GH_ParamAccess.item, TimezoneDefault);
            pManager[9].Optional = true;
        }


        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("Annual potential", "Annual", "annual hourly solar irradiance [Wh/m²] of panel", GH_ParamAccess.item);
        }


        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Required params
            double tilt = new double();
            if (!DA.GetData(0, ref tilt)) return;
            double azimuth = new double();
            if (!DA.GetData(1, ref azimuth)) return;
            List<double> DHI = new List<double>();
            if (!DA.GetDataList(2, DHI)) return;
            List<double> DNI = new List<double>();
            if (!DA.GetDataList(3, DNI)) return;

            // Optional params
            double latitude = 0.0;
            DA.GetData(4, ref latitude);
            double longitude = 0.0;
            DA.GetData(5, ref longitude);
            var solarazi = new List<double>();
            DA.GetDataList(6, solarazi);
            var solaralti = new List<double>();
            DA.GetDataList(7, solaralti);
            double Aw = 0.0;
            DA.GetData(8, ref Aw);
            int timezone = 0;
            DA.GetData(9, ref timezone);

            // Calc and output
            var annualPotential = ComputeAnnualPotential(tilt, azimuth, DHI, DNI, latitude, longitude, solarazi, solaralti, Aw, timezone);
            DA.SetDataList(0, annualPotential);
        }

        public double[] ComputeAnnualPotential(
            double tilt, double azimuth,
            List<double> DHI, List<double> DNI,
            double latitude, double longitude,
            List<double> solarazi, List<double> solaralti,
            double Aw, int timezone)
        {
            var paropts = new ParallelOptions() { MaxDegreeOfParallelism = 1 };

            var year = 2013; // ASSUMPTION

            var recursion = 2;
            var horizon = 8760;

            List<sm.SunVector> sunvectors;
            if (solaralti?.Count > 0 || solarazi?.Count > 0) // == horizon?
            {
                sm.SunVector.Create8760SunVectors(out sunvectors, longitude, latitude, year, solarazi.ToArray(), solaralti.ToArray());
            }
            else
            {
                sm.SunVector.Create8760SunVectors(out sunvectors, longitude, latitude, year); // Blanco-Muriel (2001)
            }
            // shifting list of sunvectors according to timezone, so it matches weather file data
            // if timezone != 0:
            //     copy_array = []
            //     shifted_indices = range(0, horizon)
            //     shifted_indices = shifted_indices[(timezone*-1):] + shifted_indices[:(timezone*-1)]
            //     for i in range(0, horizon):
            //         copy_array.append(sunvectors[i])
            //     for i in range(0, horizon):
            //         sunvectors[i] = copy_array[shifted_indices[i]]

            if (timezone != 0)
            {
                sm.SunVector.ShiftSunVectorsByTimezone(ref sunvectors, timezone);
            }

            // dummy variables, won't be used in this simplified calculation
            var coord = new List<sm.Sensorpoints.p3d> {
                new sm.Sensorpoints.p3d {X = 0, Y = 0, Z = 0}
            };
            var normal = new List<sm.Sensorpoints.v3d> {
                new sm.Sensorpoints.v3d {X = 0, Y = 1, Z = 0}
            };
            var albedo = new double[horizon];
            albedo.Populate(0.3); // ASSUMPTION

            // watch out copying lists?
            var weather = new sm.Context.cWeatherdata
            {
                DHI = DHI,
                DNI = DNI,
                Snow = new List<double>()
            };

            // Calculation points
            // TODO will these arrays be modified in SolarModel? Do we need to create copies?
            var p = new sm.Sensorpoints(new double[1]{ tilt }, new double[1] { azimuth }, coord.ToArray(), normal.ToArray(), recursion);
            p.SetSimpleSkyMT(new double[1] { tilt }, paropts);
            p.SetSimpleGroundReflectionMT(new double[1] { tilt }, albedo, weather, sunvectors.ToArray(), paropts);
            p.CalcIrradiationMT(weather, sunvectors.ToArray(), paropts);

            var total = new double[horizon];
            // beam = [0.0] * horizon
            // diff = [0.0] * horizon
            // diff_refl = [0.0] * horizon

            for (int i = 0; i < horizon; i++)
            {
                var irrad = p.I[0][i];
                if (irrad < 0.0)
                {
                    irrad = 0.0;
                }
                total[i] = irrad * Aw;
                // beam[i] = p.Ibeam[0][i]
                // diff[i] = p.Idiff[0][i]
                // diff_refl[i] = p.Irefl_diff[0][i]
            }

            return total;
        }

        public int VisualiseAsMesh()
        {
            return 0;
        }


        protected override System.Drawing.Bitmap Icon => Properties.Resources.Core_Solar_Potentials_Unobstructed;


        public override Guid ComponentGuid => new Guid("59130451-9E1D-4BAF-A253-5AF0A80562D0");

    }
}
