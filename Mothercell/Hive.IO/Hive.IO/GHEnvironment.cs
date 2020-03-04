using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;


namespace Hive.IO
{
    public class GHEnvironment : GH_Component
    {
        public GHEnvironment()
          : base("Hive.IO.Environment", "Hive.IO.Environment",
              "Hive.IO.Environment",
              "[hive]", "IO")
        {
        }


        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("epwPath", "epwPath", "epwPath", GH_ParamAccess.item);
            pManager.AddMeshParameter("Geometry", "Geometry", "Geometry", GH_ParamAccess.list);
        }


        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("EnvironmentObj", "EnvironmentObj", "EnvironmentObj", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string path = null;
            if (!DA.GetData(0, ref path)) return;

            List<Mesh> geometry = new List<Mesh>();
            DA.GetDataList(1, geometry);

            Environment environment = new Environment(path, geometry.ToArray());
            DA.SetData(0, environment);
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
            get { return new Guid("c11567a1-b864-4e4e-a192-24bb487a4bac"); }
        }
    }
}