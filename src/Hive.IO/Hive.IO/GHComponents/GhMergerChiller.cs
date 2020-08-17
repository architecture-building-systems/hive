﻿using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Hive.IO.EnergySystems;

namespace Hive.IO.GHComponents
{
    public class GhMergerChiller : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GhMergerChiller class.
        /// </summary>
        public GhMergerChiller()
          : base("Merger Chiller Hive", "HiveMergerChiller",
              "Description",
              "[hive]", "IO")
        {
        }


        public override GH_Exposure Exposure => GH_Exposure.quinary;


        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Air", "Air", "Air energy carrier from weather file", GH_ParamAccess.item);
            pManager.AddGenericParameter("Electricity", "Electricity", "Electricity", GH_ParamAccess.item);
            pManager.AddIntegerParameter("horizon", "horizon", "horizon", GH_ParamAccess.item);
            pManager.AddNumberParameter("coolingGenerated", "coolingGenerated", "coolingGenerated (kWh)", GH_ParamAccess.list);
            pManager.AddNumberParameter("cost", "cost", "cost", GH_ParamAccess.list);
            pManager.AddNumberParameter("ghg", "ghg", "ghg", GH_ParamAccess.list);
            pManager.AddNumberParameter("suppTemp", "suppTemp", "suppTemp for water output. necessary to know for COP calculation (?) but only if the calculation would happen here in Hive.IO", GH_ParamAccess.list);

            pManager.AddGenericParameter("Chiller", "Chiller", "Hive.IO.EnergySystems.Chiller", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Hive.IO.Chiller", "Chiller", "Chiller", GH_ParamAccess.item);
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

            int horizon = 8760;
            DA.GetData(2, ref horizon);

            var energyGenerated = new List<double>();
            DA.GetDataList(3, energyGenerated);

            var energyCost = new List<double>();
            DA.GetDataList(4, energyCost);

            var ghg = new List<double>();
            DA.GetDataList(5, ghg);

            var supplyTemp = new List<double>();
            DA.GetDataList(6, supplyTemp);

            Chiller chiller = null;
            DA.GetData(7, ref chiller);


            chiller.SetInput(air, electricity);

            // this creates a water EnergyCarrier that will be infused into the Chiller
            chiller.SetOutput(horizon, energyGenerated.ToArray(), energyCost.ToArray(), ghg.ToArray(), supplyTemp.ToArray());

            DA.SetData(0, chiller);
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
            get { return new Guid("addae68a-f507-47dd-8769-3550e87ea7b7"); }
        }
    }
}