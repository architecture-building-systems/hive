using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using Hive.IO.EnergySystems;

namespace Hive.IO.GHComponents
{
    public class GhEmitter : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GhEnergySystems class.
        /// </summary>
        public GhEmitter()
          : base("Hive.IO.EnergySystems.Emitter", "EmitterInputs",
              "Heat/Cold Emitter Design Inputs (radiator, floor heating, ...).",
              "[hive]", "IO")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("Vorlauf", "Vorlauf", "Vorlauf", GH_ParamAccess.item);
            pManager.AddNumberParameter("Rücklauf", "Rücklauf", "Rücklauf", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Emitter", "Emitter", "Emitter", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            double inTemp = 70;
            if(!DA.GetData(0, ref inTemp)) inTemp = 70;
            double returnTemp = 55;
            if (!DA.GetData(1, ref returnTemp)) returnTemp = 55;

            DA.SetData(0, new Radiator(100.0, 100.0, true, false, inTemp, returnTemp));
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
            get { return new Guid("7766d14d-fa68-42d1-86ec-51d28892c265"); }
        }
    }
}