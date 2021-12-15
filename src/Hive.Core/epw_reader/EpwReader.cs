
using System;
using System.Collections.Generic;
using Grasshopper.Kernel.Types;
using Grasshopper.Kernel;
using Hive.IO.EnergySystems;

namespace Hive.IO.GhMisc
{
    public class EpwReader : GH_Component
    {
        public EpwReader()
          : base("EPW reader", "EpwReader",
              "Reading .epw weather files, given a local path",
              "[hive]", "Misc")
        {
        }

        public override GH_Exposure Exposure => GH_Exposure.secondary;


        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Path", "Path", "Path of the epw file", GH_ParamAccess.item);
        }


        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("Latitude", "Latitude", "Latitude in deg", GH_ParamAccess.item);
            pManager.AddNumberParameter("Longitude", "Longitude", "Longitude in deg", GH_ParamAccess.item);
            pManager.AddTextParameter("City, Country", "City-Country", "City and Country", GH_ParamAccess.item);
            pManager.AddNumberParameter("GHI", "GHI", "Global Horizontal Irradiance, in Wh/m2.", GH_ParamAccess.item);
            pManager.AddNumberParameter("DNI", "DNI", "Direct Normal Irradiance, in Wh/m2.", GH_ParamAccess.item);
            pManager.AddNumberParameter("DHI", "DHI", "Diffuse Horizontal Irradiance, in Wh/m2.", GH_ParamAccess.item);
            pManager.AddNumberParameter("DryBulb", "DryBulb", "Dry Bulb Temperature, in °C.", GH_ParamAccess.item);
            pManager.AddNumberParameter("DewPoint", "DewPoint", "Dew Point Temperature, in °C.", GH_ParamAccess.item);
            pManager.AddNumberParameter("RH", "RH", "Relative Humidity, in %.", GH_ParamAccess.item);
            pManager.AddNumberParameter("GHIMonthly", "GHIMonthly", "Global Horizontal Irradiance (monthly total), in kWh/m2.", GH_ParamAccess.item);
            pManager.AddNumberParameter("DryBulbMonthly", "DryBulbMonthly", "Dry Bulb Temperature (monthly average), in °C.", GH_ParamAccess.item);
            pManager.AddNumberParameter("RHMonthly", "RHMonthly", "Relative Humidity (monthly average), in %.", GH_ParamAccess.item);
            pManager.AddGenericObjectParameter("AmbientTemperatureCarrier", "AmbTempCarrier", "Hive.IO.EnergySystems.Air containing time resolved (hourly, i.e. 8760 entries) ambient temperature in deg C.", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Timezone", "Timezone", "Timezone of the location", GH_ParamAccess.item);
        }


        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string Path = new string();
			if (!DA.GetData(0, ref Path)) return;
            

            var Latitude = new double();
            var Longitude = new double();
            var City, Country = new string();
            var GHI = new double();
            var DNI = new double();
            var DHI = new double();
            var DryBulb = new double();
            var DewPoint = new double();
            var RH = new double();
            var GHIMonthly = new double();
            var DryBulbMonthly = new double();
            var RHMonthly = new double();
            var AmbientTemperatureCarrier = new object();
            var Timezone = new int();

            // TODO

            DA.SetData(0, Latitude);
            DA.SetData(1, Longitude);
            DA.SetData(2, City, Country);
            DA.SetData(3, GHI);
            DA.SetData(4, DNI);
            DA.SetData(5, DHI);
            DA.SetData(6, DryBulb);
            DA.SetData(7, DewPoint);
            DA.SetData(8, RH);
            DA.SetData(9, GHIMonthly);
            DA.SetData(10, DryBulbMonthly);
            DA.SetData(11, RHMonthly);
            DA.SetData(12, AmbientTemperatureCarrier);
            DA.SetData(13, Timezone);
        }


        protected override System.Drawing.Bitmap Icon => Properties.Resources.epw_reader.png;


        public override Guid ComponentGuid => new Guid("56f34cc1-0ee0-4104-85cc-4b4f9179d75e"); 
       
    }
}
