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
            pManager.AddGenericParameter("DistrictHeating", "DistrictHeating", "DistrictHeating, as <Water>", GH_ParamAccess.item);

            pManager.AddNumberParameter("heatDemand", "heatDemand", "heatDemand", GH_ParamAccess.list);
            pManager.AddNumberParameter("suppTemp", "suppTemp", "suppTemp", GH_ParamAccess.list);
            pManager.AddNumberParameter("returnTemp", "returnTemp", "returnTemp", GH_ParamAccess.list);

            pManager.AddGenericParameter("HeatExchanger", "HeatExchanger", "HeatExchanger", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("HeatExchanger", "HeatExchanger", "HeatExchanger, infused with heatingConsumed and 'generated' and operational cost and emissions", GH_ParamAccess.item);
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
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return null;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("a6278ca2-bc25-4a01-a017-c3f746120270"); }
        }
    }
}