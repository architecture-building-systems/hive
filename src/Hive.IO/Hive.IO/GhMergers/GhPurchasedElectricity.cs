using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Hive.IO.EnergySystems;


namespace Hive.IO.GhMergers
{
    public class GhPurchasedElectricity : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GhGridSubstation class.
        /// </summary>
        public GhPurchasedElectricity()
          : base("Merger ElectricalSubstation Hive", "HiveMergerSubstation",
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
            pManager.AddGenericParameter("GridElectricity", "GridElectricity", "GridElectricity, as <Electricity>", GH_ParamAccess.item);

            pManager.AddNumberParameter("elecPurchased", "elecPurchased", "elecDemand that will be purchased from the grid", GH_ParamAccess.list);
            
            pManager.AddGenericParameter("DirectElectricity", "DirectElectricity", "DirectElectricity", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("DirectElectricity", "DirectElectricity", "DirectElectricity, 'fake' conversion tech 'infused' with elecConsumed from the grid (input carrier) and provided into the building (output carrier), and operational cost and emissions (input carrier)", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Electricity elec = null;
            DA.GetData(0, ref elec);

            var elecPurchased = new List<double>();
            DA.GetDataList(1, elecPurchased);

            DirectElectricity substation = null;
            DA.GetData(2, ref substation);

            substation.SetInputOutput(elec, elecPurchased.ToArray());

            DA.SetData(0, substation);

        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon => Properties.Resources.IOCore_Mergerelectricalsustation;

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid => new Guid("7644b488-19ed-4819-8195-bc1e24428cca");
    }
}