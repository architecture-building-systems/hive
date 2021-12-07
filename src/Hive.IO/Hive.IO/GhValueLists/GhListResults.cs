using Grasshopper.Kernel.Special;
using System;
using Hive.IO.Results;
using System.Linq;
using System.Collections.Generic;
using Grasshopper.Kernel.Expressions;
using Grasshopper.Kernel.Data;
using System.Text.RegularExpressions;

namespace Hive.IO.GhValueLists
{
    public class GhListResults : GH_ValueList
    {
        public GhListResults()
        {
            this.Name = "Hive Results Type";
            this.NickName = "HiveResultsType";
            this.Description = "Names of possible results to extract from <Hive.IO.Results.Results>, sorted by KPI, time resolution, and more.";
            this.Category = "[hive]";
            this.SubCategory = "IO-Core";
            Load();
        }

        private void Load()
        {
            this.ListItems.Clear();
            var propsChosen = new Dictionary<string, ResultsExposeForGhListAttribute>();

            foreach (var prop in typeof(Results.Results).GetProperties())
            {
                if (Attribute.IsDefined(prop, typeof(ResultsExposeForGhListAttribute)))
                {
                    var attr = Attribute.GetCustomAttribute(prop, typeof(ResultsExposeForGhListAttribute));
                    ResultsExposeForGhListAttribute attrResults = (ResultsExposeForGhListAttribute)attr;
                    propsChosen[prop.Name] = attrResults;
                }
            }

            foreach (var prop in propsChosen.OrderBy(x => x.Value.Kpi).ThenBy(x => x.Value.TimeResolution))
            {
                if (prop.Value.Kpi == Keys.Energy) prop.Value.Name += " - " + PrettifyPropertyName(prop.Key).Replace("Total ", "");
                var item = new GH_ValueListItem(prop.Value.Name, prop.Key);
                item.ExpireValue();
                this.ListItems.Add(item);
            }
        }

        private string PrettifyPropertyName(string key)
        {
            var r = new Regex(@"
                (?<=[A-Z])(?=[A-Z][a-z]) |
                (?<=[^A-Z])(?=[A-Z]) |
                (?<=[A-Za-z])(?=[^A-Za-z])", RegexOptions.IgnorePatternWhitespace);

            return r.Replace(key, " ").Trim();
        }



        // from honeybadger.
        protected override void CollectVolatileData_Custom()
        {
            this.m_data.Clear();
            foreach (GH_ValueListItem item in this.SelectedItems)
            {
                GH_Variant value = new GH_Variant(item.Expression);
                this.m_data.Append(value.ToGoo(), new GH_Path(0));
            }
        }

        protected override System.Drawing.Bitmap Icon => Properties.Resources.IO_Para_InputSIAroom; // FIXME

        public override Guid ComponentGuid => new Guid("7CD42802-4586-4821-B511-F07A819A3E2D");
    }
}
