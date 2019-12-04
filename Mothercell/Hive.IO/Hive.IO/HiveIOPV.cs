using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Hive.IO
{
    public class HiveIOPV : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the HiveIOPV class.
        /// </summary>
        public HiveIOPV()
          : base("HiveIOPV", "Nickname",
              "Description",
              "Category", "Subcategory")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("area", "area", "Area of PV", GH_ParamAccess.item);
            pManager.AddNumberParameter("refefficiency", "refeff", "Reference efficiency. E.g. 0.19.", GH_ParamAccess.item);
            pManager.AddNumberParameter("irradiance", "irradiance", "Irradiance on panel in [W/m²]", GH_ParamAccess.list);
            pManager.AddNumberParameter("airTemp", "airTemp", "Air temperature at the panel", GH_ParamAccess.list);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("PVObj", "PVObj", "Hive.IO.EnergySystems.PV Object", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            double area = 1.0;
            if(!DA.GetData(0, ref area)) { area = 1.0; }

            double refEff = 0.19;
            if (!DA.GetData(1, ref refEff)) { refEff = 0.19; }

            List<double> irradiance = new List<double>();
            DA.GetDataList(2, irradiance);

            List<double> airTemp = new List<double>();
            DA.GetDataList(3, airTemp);

            EnergySystem.PV pv = new EnergySystem.PV(area, refEff, irradiance.ToArray(), airTemp.ToArray());

            DA.SetData(0, pv);
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
            get { return new Guid("59389231-9a1b-4732-99dd-2bdda6b78bb8"); }
        }
    }
}