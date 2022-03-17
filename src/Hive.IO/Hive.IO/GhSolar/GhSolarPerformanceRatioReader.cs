using System;
using System.Collections.Generic;
using System.Linq;
using Grasshopper.Kernel;

namespace Hive.IO.GhSolar
{
    public class GhSolarPerformanceRatioReader : GH_Component
    {
        public GhSolarPerformanceRatioReader() :
            base("Solar Performance Ratio Reader C#", "PerformanceRatioReader",
                "Converts a Solar Performance Ratio Json to a numeric value",
                "[hive]", "Solar C#")
        {
        }

        public override GH_Exposure Exposure => GH_Exposure.secondary;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("performance_scenario", "perf_scen", "Scenario for performance ratio, like dirty roof, clean facade, shaded roof, etc", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("output", "o", "Performance ratio of surface", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
/*            List<object> perf_scen = new List<object>();
            if (!DA.GetDataList(0, perf_scen)) return;*/

            double perf_scen = 0.0;
            if (!DA.GetData(0, ref perf_scen)) return;

            System.Diagnostics.Debug.WriteLine(perf_scen);

            DA.SetData(0, perf_scen);
        }

        protected override System.Drawing.Bitmap Icon => Properties.Resources.Solar_PerformanceRatio;

        //public override Guid ComponentGuid => new Guid("6ff9899d-64a9-46ef-817c-f1521fa605e1");
        public override Guid ComponentGuid => Guid.NewGuid();
    }
}
