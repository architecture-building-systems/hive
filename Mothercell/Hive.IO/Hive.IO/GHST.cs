using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Hive.IO
{
    public class GHST : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the HiveIOST class.
        /// </summary>
        public GHST()
          : base("HiveIOST", "IO_ST",
              "Hive.IO Solar Thermal component",
              "[hive]", "IO")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddMeshParameter("mesh", "mesh", "Mesh geometry of the solar thermal collector", GH_ParamAccess.item);
            pManager.AddNumberParameter("refeffthermal", "refeffthermal", "Reference thermal efficiency. E.g. 0.8.", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("STObj", "STObj", "Hive.IO.EnergySystems.ST Object", GH_ParamAccess.item);
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

            EnergySystem.ST st = new EnergySystem.ST(mesh, refEffThermal, "name");

            DA.SetData(0, st);
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
            get { return new Guid("2d332411-db91-4b20-9d0b-70793f22fc83"); }
        }
    }
}