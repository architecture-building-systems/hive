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
              "Description",
              "[hive]", "IO-Core")
        {
        }


        public override GH_Exposure Exposure => GH_Exposure.tertiary;


        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("DistrictCooling", "DistrictCooling", "DistrictCooling, as <Water>", GH_ParamAccess.item);

            pManager.AddNumberParameter("coolDemand", "coolDemand", "coolDemand", GH_ParamAccess.list);
            pManager.AddNumberParameter("suppTemp", "suppTemp", "suppTemp", GH_ParamAccess.list);
            pManager.AddNumberParameter("returnTemp", "returnTemp", "returnTemp", GH_ParamAccess.list);

            pManager.AddGenericParameter("CoolingExchanger", "CoolingExchanger", "CoolingExchanger", GH_ParamAccess.item);

        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("CoolingExchanger", "CoolingExchanger", "CoolingExchanger, infused with coolingConsumed and 'generated' and operational cost and emissions", GH_ParamAccess.item);
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