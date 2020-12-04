using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Hive.IO.EnergySystems;

namespace Hive.IO.GhMergers
{
    public class GhChiller : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GhMergerChiller class.
        /// </summary>
        public GhChiller()
          : base("Merger Chiller Hive", "HiveMergerChiller",
              "Hive Merger for a Chiller (<Hive.IO.EnergySystems.Chiller>). It merges all kinds of data together to update the Chiller with information on consumed energy, cost, operational schedule, etc. that have been computed in other components.",
              "[hive]", "IO-Core")
        {
        }


        public override GH_Exposure Exposure => GH_Exposure.tertiary;


        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Air Carrier", "Air", "Air energy carrier from weather file", GH_ParamAccess.item);
            pManager.AddGenericParameter("Electricity Carrier", "Electricity", "Electricity energy carrier, e.g. from an electricity grid.", GH_ParamAccess.item);
            pManager.AddNumberParameter("Cooling Generated", "coolGenerated", "Time series of cooling energy generated (kWh). Either annual hourly( 8760), or monthly (12) time series.", GH_ParamAccess.list);
            pManager.AddNumberParameter("Supply Temperature", "supplyTemp", "Time series of the supply / cooling temperature (cold reservoir)", GH_ParamAccess.list);
            pManager.AddBooleanParameter("SimpleMode?", "simpleMode?", "Compute simple efficiency? If 'False', Eqt. (7) from Choi et al (2005) is used (10.1016/j.energy.2004.08.004).", GH_ParamAccess.item, true);

            pManager.AddGenericParameter("Hive Chiller", "Chiller", "Hive Chiller (<Hive.IO.EnergySystems.Chiller>) that will be infused with information from above inputs.", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Hive Chiller", "Chiller", "Hive Chiller with infused data about consumed energy, operational schedule, etc.", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Air air = null;
            DA.GetData(0, ref air);

            Electricity electricity = null;
            DA.GetData(1, ref electricity);

            var energyGenerated = new List<double>();
            DA.GetDataList(2, energyGenerated);

            var supplyTemp = new List<double>();
            DA.GetDataList(3, supplyTemp);

            bool simple = true;
            DA.GetData(4, ref simple);

            Chiller chiller = null;
            DA.GetData(5, ref chiller);


            if (simple)
                chiller.SetInputOutputSimple(electricity, air, energyGenerated.ToArray(), supplyTemp.ToArray());
            else
                chiller.SetInputOutput(electricity, air, energyGenerated.ToArray(), supplyTemp.ToArray());

            DA.SetData(0, chiller);
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon => Properties.Resources.IOCore_Mergerchiller;

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid => new Guid("addae68a-f507-47dd-8769-3550e87ea7b7");
    }
}