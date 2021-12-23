
using System;
using System.Linq;
using Grasshopper.Kernel;
using Hive.IO.Environment;

namespace Hive.IO.GhMisc
{
    public class GhDistEpw : GH_Component
    {
        public GhDistEpw()
          : base("Distributor EPW", "HiveDistEPW",
              "Distributes a given Hive.IO.EPW Class or parsed via a given path, which gathers data from .epw weather files. Note that if a valid path is given, it will override the EPW object.",
              "[hive]", "Weather")
        {
        }

        public override GH_Exposure Exposure => GH_Exposure.primary;


        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("EPW", "EPW", "EPW weather file data stored as a Hive object of type <Hive.IO.EPW>", GH_ParamAccess.item);
            pManager[0].Optional = true;
            pManager.AddTextParameter("EPWFilepath", "EPWFilepath", "The path to a .epw weather file. Overrides the 'EPW' parameter if non-null and path is valid.", GH_ParamAccess.item);
            pManager[1].Optional = true;
            pManager.AddBooleanParameter("WriteGHNumbers", "WriteGHNumbers", "Additionally to an epw-hive-object, this component will dump out GH_Numbers of weather data if set to true.", GH_ParamAccess.item, false);
            pManager[2].Optional = true;
        }


        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("EPW", "EPW", "EPW weather file data stored as a Hive object of type <Hive.IO.EPW>", GH_ParamAccess.item);
            pManager.AddNumberParameter("Latitude", "Latitude", "Latitude in deg", GH_ParamAccess.item);
            pManager.AddNumberParameter("Longitude", "Longitude", "Longitude in deg", GH_ParamAccess.item);
            pManager.AddTextParameter("City, Country", "City-Country", "City and Country", GH_ParamAccess.list);
            pManager.AddNumberParameter("GHI", "GHI", "Global Horizontal Irradiance, in Wh/m2.", GH_ParamAccess.list);
            pManager.AddNumberParameter("DNI", "DNI", "Direct Normal Irradiance, in Wh/m2.", GH_ParamAccess.list);
            pManager.AddNumberParameter("DHI", "DHI", "Diffuse Horizontal Irradiance, in Wh/m2.", GH_ParamAccess.list);
            pManager.AddNumberParameter("DryBulb", "DryBulb", "Dry Bulb Temperature, in °C.", GH_ParamAccess.list);
            pManager.AddNumberParameter("DewPoint", "DewPoint", "Dew Point Temperature, in °C.", GH_ParamAccess.list);
            pManager.AddNumberParameter("RH", "RH", "Relative Humidity, in %.", GH_ParamAccess.list);
            pManager.AddNumberParameter("GHIMonthly", "GHIMonthly", "Global Horizontal Irradiance (monthly total), in kWh/m2.", GH_ParamAccess.list);
            pManager.AddNumberParameter("DryBulbMonthly", "DryBulbMonthly", "Dry Bulb Temperature (monthly average), in °C.", GH_ParamAccess.list);
            pManager.AddNumberParameter("RHMonthly", "RHMonthly", "Relative Humidity (monthly average), in %.", GH_ParamAccess.list);
            pManager.AddGenericParameter("AmbientTemperatureCarrier", "AmbTempCarrier", "Hive.IO.EnergySystems.Air containing time resolved (hourly, i.e. 8760 entries) ambient temperature in deg C.", GH_ParamAccess.item);
            pManager.AddNumberParameter("Timezone", "Timezone", "Timezone of the location", GH_ParamAccess.item);
        }


        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string epwPath = "";
            Epw epw = null;

            // if a path is given, override the EPW object
            if (DA.GetData(1, ref epwPath))
            {
                epw = new Epw() { FilePath = @epwPath };
                epw.Parse();
                DA.SetData(0, epw);
            }
            // if no path given and EPW object is null, return null
            else if (!DA.GetData(0, ref epw))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Neither epw-path nor epw-hive-object are provided as inputs.");
                return;
            }

            bool writeGHLists = false;
            DA.GetData(2, ref writeGHLists);
            if (!writeGHLists) AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, "WriteGHNumbers is set to false; no GH_Numbers will be written.");

            DA.SetData(0, epw);

            if (epw != null && writeGHLists)
            {
                DA.SetData(1, epw.Latitude);
                DA.SetData(2, epw.Longitude);
                DA.SetDataList(3, new string[] { epw.City, epw.Country });
                DA.SetDataList(4, epw.GHI);
                DA.SetDataList(5, epw.DNI);
                DA.SetDataList(6, epw.DHI);
                DA.SetDataList(7, epw.DryBulbTemperature);
                DA.SetDataList(8, epw.DewPointTemperature);
                DA.SetDataList(9, epw.RelativeHumidity);
                DA.SetDataList(10, epw.GHIMonthly);
                DA.SetDataList(11, epw.DryBulbTemperatureMonthly);
                DA.SetDataList(12, epw.RelativeHumidityMonthly);
                DA.SetData(13, epw.AmbientTemperatureCarrier);
                DA.SetData(14, epw.TimeZone);
            }
        }


        protected override System.Drawing.Bitmap Icon => Properties.Resources.Weather_EPW_reader;


        public override Guid ComponentGuid => new Guid("79B7F941-CB32-47FB-8475-E535CB045A23");

    }
}
