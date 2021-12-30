using System;
using Grasshopper.Kernel;

namespace Hive.IO.GhSolar
{
    public class GhOrientationFactor : GH_Component
    {
        public GhOrientationFactor() :
            base("Solar Orientation Factor", "OrientationFactor",
                "Solar orientation factor of a surface",
                "[hive]", "Solar")
        {
        }

        public override GH_Exposure Exposure => GH_Exposure.secondary;

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("G_flat", "G_flat", "Global irradiation, i.e. on the flat, in [kWh/m2]", GH_ParamAccess.list);
            pManager.AddNumberParameter("G_angled", "G_angled", "Irradiation on an angle in [kWh/m2]", GH_ParamAccess.list);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("OF", "OF", "Orientation factor as a fraction [0.0, >1.0]", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            double G_flat = 0.0;
            if (!DA.GetData(0, ref G_flat)) return;
            double G_angled = 0.0;
            if (!DA.GetData(1, ref G_angled)) return;

            var OF = G_angled / G_flat;

            DA.SetData(0, OF);
        }

        protected override System.Drawing.Bitmap Icon => Properties.Resources.Solar_OrientationFactor;

        public override Guid ComponentGuid => new Guid("dfde3d64-e63d-4715-a6b9-b045920d8c97");
    }
}
