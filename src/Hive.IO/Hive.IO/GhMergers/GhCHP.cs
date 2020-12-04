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
              "Hive Merger for a Combined Heat and Power (CHP) (<Hive.IO.EnergySystems.CombinedHeatPower>). It merges all kinds of data together to update the CHP with information on consumed energy, cost, operational schedule, etc. that have been computed in other components.",
              "[hive]", "IO-Core")
        {
        }


        public override GH_Exposure Exposure => GH_Exposure.tertiary;


        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Gas Carrier", "Gas", "Gas energy carrier (e.g. from a gas grid) that is used as fuel for this CHP.", GH_ParamAccess.item);
            pManager.AddNumberParameter("Energy Generated", "energyGenerated", "Generated heating or electricity energy time series (kWh). By default it is electricity. Either annual hourly( 8760), or monthly (12) time series.", GH_ParamAccess.list);
            pManager.AddBooleanParameter("IsHeatingGenerated?", "IsHeatingGenerated?", "Is the 'energyGenerated' input parameter heating energy? If 'False', it will be assumed as electricity instead.", GH_ParamAccess.item, false);
            pManager.AddNumberParameter("Supply Temperature", "suppTemp", "Supply temperature time series of the heated water from the CHP. Used to calculate COP (Coefficient of Performance) of the CHP.", GH_ParamAccess.list);

            pManager.AddGenericParameter("Hive Combined Heat and Power", "CombinedHeatPower", "Hive Combined Heat and Power (CHP) (<Hive.IO.EnergySystems.CombinedHeatPower>) that will be infused with information from above inputs. ", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Hive Combined Heat and Power", "CombinedHeatPower", "Hive CHP with infused data about consumed energy, operational schedule, etc.", GH_ParamAccess.item);
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
        protected override System.Drawing.Bitmap Icon => Properties.Resources.IOCore_Mergercombinedheatpower;

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid => new Guid("c46bd779-a529-4b02-b59e-0b07cb6b5979");
    }
}