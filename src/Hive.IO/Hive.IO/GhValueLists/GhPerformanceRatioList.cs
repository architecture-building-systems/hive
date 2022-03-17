using Grasshopper.Kernel.Special;
using System;
using Hive.IO.Results;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using Grasshopper.Kernel.Expressions;
using Grasshopper.Kernel.Data;
using System.Text.RegularExpressions;
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

        private void Load()
        {
            string[] lines = System.IO.File.ReadAllLines("../Resources/performance_ratio.csv");
            lines = lines.Skip(1).ToArray();

            Dictionary<string, string> performanceRatio = lines.Select(line => line.Split(',')).ToDictionary(line => line[0], line => line[1]);

            this.ListItems.Clear();
            foreach(var item in performanceRatio)
            {
                this.ListItems.Add(new GH_ValueListItem(item.Key, item.Value));
            }
        }

        public override Guid ComponentGuid => Guid.NewGuid();
    }
}
