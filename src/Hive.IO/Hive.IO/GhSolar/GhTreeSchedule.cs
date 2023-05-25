using Grasshopper.Kernel.Special;
using Grasshopper.Kernel;
using Hive.IO.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Reflection;
using System.Drawing;

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

        public class TreeSchedules
        {
            public List<double> fruiting { get; set; }
            public List<double> mallow { get; set; }
            public List<double> viburnum { get; set; }
            public List<double> willow { get; set; }
            public List<double> witch_hazel { get; set; }
            public List<double> sycamore { get; set; }
            public List<double> oak { get; set; }
            public List<double> walnut { get; set; }
            public List<double> katsura { get; set; }
            public List<double> dogwood { get; set; }
            public List<double> conifer { get; set; }
            public List<double> olive { get; set; }
            public List<double> bamboo { get; set; }
            public List<double> staff_vine { get; set; }
            public List<double> birch { get; set; }
            public List<double> maple { get; set; }
        }

        private static TreeSchedules treeSchedules_; //JsonResource backing field
        TreeSchedules treeSchedules = new TreeSchedules();

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
                
            treeSchedules = JsonResource.ReadRecords(ResourceName, ref treeSchedules_);

            var schedule = treeSchedules.GetType().GetProperty(treeType).GetValue(treeSchedules) as IEnumerable<double>;

            DA.SetDataList(0, schedule);
        }

        protected override Bitmap Icon => Properties.Resources.Solar_TreeSchedules;

        public override Guid ComponentGuid => new Guid("53ffee32-d80e-48a3-bc1f-07e3b0e411d7");
    }
}
