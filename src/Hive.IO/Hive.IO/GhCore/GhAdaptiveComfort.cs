
using System;
using System.Collections.Generic;
using Grasshopper.Kernel.Types;
using Grasshopper.Kernel;
using Hive.IO.EnergySystems;
using Hive.IO.Core;

namespace Hive.IO.GhDemand
{
    public class GhAdaptiveComfort : GH_Component
    {
        public GhAdaptiveComfort()
          : base("Adaptive Comfort", "AdaptiveComfort",
              "Adaptive thermal comfort temperature as a function of ambient air temperature. Source: PLEA Notes 3 - Thermal Comfort. Auliciems and Szokolay 2007.",
              "[hive]", "Demand")
        {
        }

        public override GH_Exposure Exposure => GH_Exposure.secondary;


        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("Air temperature", "T_e", "Average ambient air temperature in Â°C, either hourly or averaged monthly.", GH_ParamAccess.list);
        }


        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("Thermal comfort", "T_n", "Mean adaptive thermal comfort temperature in Â°C.", GH_ParamAccess.list);
            pManager.AddNumberParameter("Comfort, 80% upper bound", "T_n80ub", "Adaptive thermal comfort temperature in Â°C (upper bound of 80% acceptance).", GH_ParamAccess.list);
            pManager.AddNumberParameter("Comfort, 80% lower bound", "T_n80lb", "Adaptive thermal comfort temperature in Â°C (lower bound of 80% acceptance).", GH_ParamAccess.list);
            pManager.AddNumberParameter("Comfort, 90% upper bound", "T_n90ub", "Adaptive thermal comfort temperature in Â°C (upper bound of 90% acceptance).", GH_ParamAccess.list);
            pManager.AddNumberParameter("Comfort, 90% lower bound", "T_n90lb", "Adaptive thermal comfort temperature in Â°C (lower bound of 90% acceptance).", GH_ParamAccess.list);
        }


        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<double> AmbientTemperature = new List<double>();
            if (!DA.GetDataList(0, AmbientTemperature)) return;

            var ac = new AdaptiveComfort(AmbientTemperature);

            DA.SetDataList(0, ac.Setpoints);
            DA.SetDataList(1, ac.SetpointsUB80);
            DA.SetDataList(2, ac.SetpointsLB80);
            DA.SetDataList(3, ac.SetpointsUB90);
            DA.SetDataList(4, ac.SetpointsLB90);
        }


        protected override System.Drawing.Bitmap Icon => Properties.Resources.Core_AdaptiveComfort;


        public override Guid ComponentGuid => new Guid("83d5d815-a306-4555-81f8-887132d0711f");

    }
}
