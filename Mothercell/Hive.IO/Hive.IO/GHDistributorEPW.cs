using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Hive.IO
{
    public class GHDistributorEPW : GH_Component
    {
        public GHDistributorEPW()
          : base("DistributorEPW", "DistributorEPW",
              "DistributorEPW",
              "[hive]", "Mothercell")
        {
        }


        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("EnvironmentObj", "EnvironmentObj", "EnvironmentObj", GH_ParamAccess.item);
        }


        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("filepath", "filepath", "filepath", GH_ParamAccess.item);
        }


        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Environment environment = null;
            if (!DA.GetData(0, ref environment)) return;

            if (environment != null)
                DA.SetData(0, environment.EpwPath);
        }


        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return null;
            }
        }


        public override Guid ComponentGuid
        {
            get { return new Guid("bf00c55e-e0bc-437d-b036-56adf7b149b5"); }
        }
    }
}