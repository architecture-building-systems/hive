using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Hive.IO.EnergySystems;


namespace Hive.IO.GhMergers
{
    public class GhElectricalSubstation : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GhGridSubstation class.
        /// </summary>
        public GhElectricalSubstation()
          : base("Merger GridSubstation Hive", "HiveMergerGrid",
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
            pManager.AddGenericParameter("GridElectricity", "GridElectricity", "GridElectricity, as <Electricity>", GH_ParamAccess.item);

            pManager.AddNumberParameter("elecDemand", "elecDemand", "elecDemand that will be provided by grid", GH_ParamAccess.list);
            
            pManager.AddGenericParameter("GridSubstation", "GridSubstation", "GridSubstation", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("GridSubstation", "GridSubstation", "GridSubstation, infused with elecConsumed from the grid (input carrier) and provided into the building (output carrier), and operational cost and emissions (input carrier)", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Electricity elec = null;
            DA.GetData(0, ref elec);

            var elecGenerated = new List<double>();
            DA.GetDataList(1, elecGenerated);

            HeatCoolingExchanger substation = null;
            DA.GetData(4, ref substation);

            //substation.SetInputOutput(elec, elecGenerated.ToArray());

            DA.SetData(0, substation);

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
            get { return new Guid("7644b488-19ed-4819-8195-bc1e24428cca"); }
        }
    }
}