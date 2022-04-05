using Grasshopper.Kernel.Special;
using System;
using Hive.IO.Util;
using System.Collections.Generic;
using Grasshopper.Kernel;

namespace Hive.IO.GhValueLists
{
    public class GhPerformanceRatioList : GH_ValueList
    {
        public GhPerformanceRatioList()
        {
            this.Name = "Solar Performance Ratio";
            this.NickName = "PerformanceRatio";
            this.Description = "A list of solar performance ratios";
            this.Category = "[hive]";
            this.SubCategory = "Solar C#";
            Load();
        }

        public override GH_Exposure Exposure => GH_Exposure.tertiary;

        public struct PerformanceRatioListItem
        {
            public string Scenario;
            public string PerformanceRatio;
        }

        private static List<PerformanceRatioListItem> performanceRatios_; //JsonResource backing field

        public static string ResourceName = "Hive.IO.GhValueLists.performance_ratios.json";

        List<PerformanceRatioListItem> performanceRatios => JsonResource.ReadRecords(ResourceName, ref performanceRatios_);

        private void Load()
        {
            this.ListItems.Clear();
            foreach(var item in performanceRatios)
            {
                this.ListItems.Add(new GH_ValueListItem(item.Scenario, item.PerformanceRatio));
            }
        }

        public override Guid ComponentGuid => new Guid("882c3d9f-af08-4577-b945-68bcc77cb9df");
    }
}
