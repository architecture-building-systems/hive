using Grasshopper.Kernel.Special;
using Grasshopper.Kernel;
using Hive.IO.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Hive.IO.GhValueLists
{
    public class GhTreeSchedule : GH_Component
    {
        public GhTreeSchedule()
        {
            this.Name = "Tree Schedules";
            this.NickName = "TreeSchedule";
            this.Description = "Outputs the tree leaf schedule for the desired tree type";
            this.Category = "[hive]";
            this.SubCategory = "Solar";
        }

        public override GH_Exposure Exposure => GH_Exposure.secondary;

        Dictionary<string, List<double>> treeSchedules = new Dictionary<string, List<double>>();

        public static string ResourceName = "Hive.IO.GhSolar.tree_schedules.json";
        dynamic TreeSchedules => JsonResource.ReadRecords(ResourceName, ref treeSchedules);

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Tree Type", "TreeType", "Name of the tree type", GH_ParamAccess.item);
            //pManager.AddGenericParameter("Tree Mesh", "TreeMesh", "Mesh for the tree", GH_ParamAccess.list);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Tree Profile", "TreeProfile", "List of annual tree leaf cover as a value between 0 - 1", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string type = "";
            if (!DA.GetData(0, ref type)) return;




        }

        public override Guid ComponentGuid => new Guid("53ffee32-d80e-48a3-bc1f-07e3b0e411d7");
    }
}
