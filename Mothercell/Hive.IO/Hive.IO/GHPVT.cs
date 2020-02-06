using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Hive.IO
{
    public class GHPVT : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the HiveIOPVT class.
        /// </summary>
        public GHPVT()
          : base("HiveIOPVT", "IO_PVT",
              "Hive.IO PVT component",
              "[hive]", "IO")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddMeshParameter("mesh", "mesh", "Mesh geometry of PVT", GH_ParamAccess.item);
            pManager.AddNumberParameter("refeffthermal", "refeffthermal", "Reference thermal efficiency. E.g. 0.8.", GH_ParamAccess.item);
            pManager.AddNumberParameter("reffeffelectric", "refeffelectric", "Reference electric efficiency. E.g. 0.19", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("PVTObj", "PVTObj", "Hive.IO.EnergySystems.PVT Object", GH_ParamAccess.item);
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

            double refEffElectric = 0.19;
            if (!DA.GetData(2, ref refEffElectric)) { refEffElectric = 0.19; }

            EnergySystem.PVT pvt = new EnergySystem.PVT(mesh, refEffThermal, refEffElectric, "name");

            DA.SetData(0, pvt);
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
            get { return new Guid("b59de947-e92e-495d-9c3d-6fc628c5caf5"); }
        }
    }
}