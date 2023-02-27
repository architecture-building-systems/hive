using Grasshopper.Kernel.Special;
using Grasshopper.Kernel;
using Hive.IO.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using static Hive.IO.GhValueLists.GhPerformanceRatioList;

namespace Hive.IO.GhValueLists
{
    public class GhTreeSchedule : GH_Component
    {
        public GhTreeSchedule() : base(
            "Tree Schedules", 
            "TreeSchedule", 
            "Outputs the tree leaf schedule for the desired tree type",
            "[hive]",
            "Solar")
            {}

        public override GH_Exposure Exposure => GH_Exposure.secondary;

        public struct TreeSchedule
        {
            public string TreeType;
            public List<double> Schedule;
        }

        private static List<TreeSchedule> treeSchedules_; //JsonResource backing field

        public static string ResourceName = "Hive.IO.GhSolar.tree_schedules.json";
        

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Tree Type", "TreeType", "Desired Tree Type", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Tree Schedule", "TreeSchedule", "Schedule of the selected tree type", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string treeType = "";
            if (!DA.GetData(0, ref treeType)) return;

            List<TreeSchedule> treeSchedules = new List<TreeSchedule>();
                
            treeSchedules = JsonResource.ReadRecords(ResourceName, ref treeSchedules_);

            var schedule = treeSchedules[0];

            DA.SetData(0, schedule);
        }

        public override Guid ComponentGuid => new Guid("53ffee32-d80e-48a3-bc1f-07e3b0e411d7");
    }
}
