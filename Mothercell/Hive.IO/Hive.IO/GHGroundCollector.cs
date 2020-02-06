using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Hive.IO
{
    public class GHGroundCollector : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the HiveIOGroundCollector class.
        /// </summary>
        public GHGroundCollector()
          : base("HiveIOGroundCollector", "IO_GroundColl",
              "Hive.IO Ground solar collector component",
              "[hive]", "IO")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddMeshParameter("mesh", "mesh", "Mesh geometry of the collector", GH_ParamAccess.item);
            pManager.AddNumberParameter("refeffthermal", "refeffthermal", "Reference thermal efficiency. E.g. 0.8.", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("GroundCollectorObj", "GroundCollectorObj", "Hive.IO.EnergySystems.GroundCollector Object", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Mesh mesh = new Mesh();
            if (!DA.GetData(0, ref mesh)) { return; }

            double refEffThermal = 0.8;
            if (!DA.GetData(1, ref refEffThermal)) { refEffThermal = 0.8; }

            EnergySystem.GroundCollector gc = new EnergySystem.GroundCollector(mesh, refEffThermal, "name");

            DA.SetData(0, gc);
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
            get { return new Guid("6bbd8316-4f3c-45ed-a00a-e921bb9c82c2"); }
        }
    }
}