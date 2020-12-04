using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Hive.IO.EnergySystems;


namespace Hive.IO.GhMergers
{
    public class GhCoolingExchanger : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GhCoolingExchanger class.
        /// </summary>
        public GhCoolingExchanger()
          : base("Merger CoolingExchanger Hive", "HiveMergerCX",
              "Hive Merger for a Cold Exchanger (<Hive.IO.EnergySystems.HeatCoolingExchanger>). It merges all kinds of data together to update the Cold Exchanger with information on consumed energy, cost, operational schedule, etc. that have been computed in other components.",
              "[hive]", "IO-Core")
        {
        }


        public override GH_Exposure Exposure => GH_Exposure.tertiary;


        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("District Cooling Carrier", "DistrictCooling", "District Cooling as <Hive.IO.EnergySystems.Water> energy carrier that is used as input fluid for this cold exchanger.", GH_ParamAccess.item);

            pManager.AddNumberParameter("Cooling Generated", "coolGenerated", "Time series of cooling energy generated (kWh). Either annual hourly( 8760), or monthly (12) time series.", GH_ParamAccess.list);
            pManager.AddNumberParameter("Supply Temperature", "supplyTemp", "Time series of the supply temperature (cold reservoir).", GH_ParamAccess.list);
            pManager.AddNumberParameter("Return Temperature", "returnTemp", "Time series of the return temperature (warm reservoir).", GH_ParamAccess.list);

            pManager.AddGenericParameter("Hive Cooling Exchanger", "CoolingExchanger", "Hive Cooling Exchanger (<Hive.IO.EnergySystems.HeatCoolingExchanger>) that will be infused with information from above inputs.", GH_ParamAccess.item);

        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Hive Cooling Exchanger", "CoolingExchanger", "Hive Cooling Exchanger with infused data about consumed energy, operational schedule, etc.", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Water dc = null;
            DA.GetData(0, ref dc);

            var coolDemand = new List<double>();
            DA.GetDataList(1, coolDemand);

            var supplyTemp = new List<double>();
            DA.GetDataList(2, supplyTemp);

            var returnTemp = new List<double>();
            DA.GetDataList(3, returnTemp);

            HeatCoolingExchanger cx = null;
            DA.GetData(4, ref cx);

            cx.SetInputOutput(dc, coolDemand.ToArray(), supplyTemp.ToArray(), returnTemp.ToArray());

            DA.SetData(0, cx);
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon => Properties.Resources.IOCore_Mergercoolingexchanger;
        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid => new Guid("d0e958ac-4d82-4942-adc4-89b53f1d3c85");
    }
}