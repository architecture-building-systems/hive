using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Hive.IO.EnergySystems;


namespace Hive.IO.GhMergers
{
    public class GhHeatExchanger : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GhHeatExchanger class.
        /// </summary>
        public GhHeatExchanger()
          : base("Merger HeatExchanger Hive", "HiveMergerHX",
              "Hive Merger for a Heat Exchanger (<Hive.IO.EnergySystems.HeatCoolingExchanger>). It merges all kinds of data together to update the Heat Exchanger with information on consumed energy, cost, operational schedule, etc. that have been computed in other components.",
              "[hive]", "IO-Core")
        {
        }


        public override GH_Exposure Exposure => GH_Exposure.tertiary;


        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("District Heating Carrier", "DistrictHeating", "District Heating as <Hive.IO.EnergySystems.Water> energy carrier that is used as input fluid for this heat exchanger.", GH_ParamAccess.item);

            pManager.AddNumberParameter("Heating Generated", "heatGenerated", "Time series of heating energy generated (kWh). Either annual hourly( 8760), or monthly (12) time series.", GH_ParamAccess.list);
            pManager.AddNumberParameter("Supply Temperature", "supplyTemp", "Time series of the supply temperature (warm reservoir).", GH_ParamAccess.list);
            pManager.AddNumberParameter("Return Temperature", "returnTemp", "Time series of the return temperature (cold reservoir).", GH_ParamAccess.list);

            pManager.AddGenericParameter("Hive Heat Exchanger", "HeatingExchanger", "Hive Heating Exchanger (<Hive.IO.EnergySystems.HeatHeatExchanger>) that will be infused with information from above inputs.", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Hive Heat Exchanger", "HeatExchanger", "Hive Heat Exchanger with infused data about consumed energy, operational schedule, etc.", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Water dh = null;
            DA.GetData(0, ref dh);

            var heatGenerated = new List<double>();
            DA.GetDataList(1, heatGenerated);

            var supplyTemp = new List<double>();
            DA.GetDataList(2, supplyTemp);

            var returnTemp = new List<double>();
            DA.GetDataList(3, returnTemp);

            HeatCoolingExchanger hx = null;
            DA.GetData(4, ref hx);

            hx.SetInputOutput(dh, heatGenerated.ToArray(), supplyTemp.ToArray(), returnTemp.ToArray());

            DA.SetData(0, hx);
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon => Properties.Resources.IOCore_Mergercoolingexchanger;

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid => new Guid("a6278ca2-bc25-4a01-a017-c3f746120270");
    }
}