using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using Hive.IO.EnergySystems;

namespace Hive.IO.GhMergers
{
    public class GhGasBoiler : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GhMergerGasBoiler class.
        /// </summary>
        public GhGasBoiler()
          : base("Merger GasBoiler Hive", "HiveMergerBoiler",
              "Hive Merger for a Boiler (<Hive.IO.EnergySystems.Boiler>). It merges all kinds of data together to update the Boiler with information on consumed energy, cost, operational schedule, etc. that have been computed in other components.",
              "[hive]", "IO-Core")
        {
        }


        public override GH_Exposure Exposure => GH_Exposure.tertiary;


        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Gas Carrier", "Gas", "Gas energy carrier (e.g. from a gas grid) that is used as fuel for this boiler.", GH_ParamAccess.item);
            pManager.AddNumberParameter("Heating Generated", "heatGenerated", "Generated heating energy time series (kWh). Either annual hourly( 8760), or monthly (12) time series.", GH_ParamAccess.list);
            pManager.AddNumberParameter("Supply Temperature", "suppTemp", "Supply temperature time series of the heated water at the boiler outlet. Used to compute efficiencies of the boiler.", GH_ParamAccess.list);

            pManager.AddGenericParameter("Hive Gas Boiler", "GasBoiler", "Hive Gas Boiler (<Hive.IO.EnergySystems.GasBoiler>) that will be infused with information from above inputs.", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Hive Gas Boiler", "GasBoiler", "Hive Gas Boiler with infused data about consumed energy, operational schedule, etc.", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Gas gas = null;
            DA.GetData(0, ref gas);
          
            var heatingDemand = new List<double>();
            DA.GetDataList(1, heatingDemand);

            var supplyTemp = new List<double>();
            DA.GetDataList(2, supplyTemp);

            GasBoiler boiler = null;
            DA.GetData(3, ref boiler);

            boiler.SetInputOutput(gas, heatingDemand.ToArray(), supplyTemp.ToArray());
            DA.SetData(0, boiler);
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon => Properties.Resources.IOCore_Merger_Gasboiler;

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid => new Guid("a8196942-924f-44d7-83e7-19565b462b6f");
    }
}