
using System;
using System.Collections.Generic;
using Grasshopper.Kernel.Types;
using Grasshopper.Kernel;
using Hive.IO.EnergySystems;

namespace Hive.IO.GhDemand
{
    public class AdaptiveComfort : GH_Component
    {
        public AdaptiveComfort()
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
            List<double> Air_temperature = new List<double>();
			if (!DA.GetDataList(0, ref Air_temperature)) return;
            

            var Thermal_comfort = new List<List<double>>();
            var Comfort,_80%_upper_bound = new List<List<double>>();
            var Comfort,_80%_lower_bound = new List<List<double>>();
            var Comfort,_90%_upper_bound = new List<List<double>>();
            var Comfort,_90%_lower_bound = new List<List<double>>();

            // TODO

            DA.SetDataList(0, Thermal_comfort);
            DA.SetDataList(1, Comfort,_80%_upper_bound);
            DA.SetDataList(2, Comfort,_80%_lower_bound);
            DA.SetDataList(3, Comfort,_90%_upper_bound);
            DA.SetDataList(4, Comfort,_90%_lower_bound);
        }


        protected override System.Drawing.Bitmap Icon => Properties.Resources.demand_adaptivecomfort.png;


        public override Guid ComponentGuid => new Guid("83d5d815-a306-4555-81f8-887132d0711f"); 
       
    }
}
