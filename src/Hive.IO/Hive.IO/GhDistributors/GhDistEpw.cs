
using System;
using System.Linq;
using Grasshopper.Kernel;
using Hive.IO.Environment;

namespace Hive.IO.GhMisc
{
    public class GhDistEpw : GH_Component
    {
        public GhDistEpw()
          : base("Distributor EPW", "DistEPW",
              "Distributes the Hive.IO.EPW Class, which gathers data from .epw weather files",
              "[hive]", "Misc")
        {
        }

        public override GH_Exposure Exposure => GH_Exposure.primary;


        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("EPW", "EPW", "The instance of type <Hive.IO.EPW> that represents a .epw weather file", GH_ParamAccess.item);
        }


        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("Latitude", "Latitude", "Latitude in deg", GH_ParamAccess.item);
            pManager.AddNumberParameter("Longitude", "Longitude", "Longitude in deg", GH_ParamAccess.item);
            pManager.AddTextParameter("City, Country", "City-Country", "City and Country", GH_ParamAccess.item);
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
            Epw epw = new Epw();
            if (!DA.GetData(0, ref epw)) return;

            DA.SetData(0, epw.Latitude);
            DA.SetData(1, epw.Longitude);
            DA.SetData(2, new Tuple<string,string>(epw.City, epw.Country));
            DA.SetDataList(3, epw.GHI);
            DA.SetDataList(4, epw.DNI);
            DA.SetDataList(5, epw.DHI);
            DA.SetDataList(6, epw.DryBulbTemperature);
            DA.SetDataList(7, epw.DewPointTemperature);
            DA.SetDataList(8, epw.RelativeHumidity);
            DA.SetDataList(9, epw.GHIMonthly);
            DA.SetDataList(10, epw.DryBulbTemperatureMonthly);
            DA.SetDataList(11, epw.RelativeHumidityMonthly);
            DA.SetData(12, epw.AmbientTemperatureCarrier);
            DA.SetData(13, epw.TimeZone);
        }


        protected override System.Drawing.Bitmap Icon => Properties.Resources.Misc_EPW_reader;


        public override Guid ComponentGuid => new Guid("79B7F941-CB32-47FB-8475-E535CB045A23");

    }
}
