using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Hive.IO.EnergySystems;

namespace Hive.IO.GhMergers
{
    public class GhCHP : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GhMergerCHP class.
        /// </summary>
        public GhCHP()
          : base("Merger CHP Hive", "HiveMergerCHP",
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
            pManager.AddGenericParameter("Gas", "Gas", "Gas", GH_ParamAccess.item);
            pManager.AddNumberParameter("energyDemand", "energyDemand", "energyDemand (kWh). Can be electricity (default) or heating", GH_ParamAccess.list);
            pManager.AddBooleanParameter("IsHeatingDemand?", "IsHeatingDemand?", "IsHeatingDemand?", GH_ParamAccess.item, false);
            pManager.AddNumberParameter("suppTemp", "suppTemp", "suppTemp for water output. necessary to know for COP calculation", GH_ParamAccess.list);

            pManager.AddGenericParameter("Hive.IO.CombinedHeatPower", "CombinedHeatPower", "CombinedHeatPower", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Hive.IO.CombinedHeatPower", "CombinedHeatPower", "CombinedHeatPower", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Gas gas = null;
            DA.GetData(0, ref gas);

            var energyGenerated = new List<double>();
            DA.GetDataList(1, energyGenerated);

            bool isHeat = false;
            DA.GetData(2, ref isHeat);

            var supplyTemp = new List<double>();
            DA.GetDataList(3, supplyTemp);

            CombinedHeatPower chp = null;
            DA.GetData(4, ref chp);

            chp.SetInputOutput(gas, energyGenerated.ToArray(), supplyTemp.ToArray(), isHeat);

            DA.SetData(0, chp);
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
            get { return new Guid("c46bd779-a529-4b02-b59e-0b07cb6b5979"); }
        }
    }
}