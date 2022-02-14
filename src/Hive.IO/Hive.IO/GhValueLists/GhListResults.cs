using Grasshopper.Kernel.Special;
using System;
using Hive.IO.Results;
using System.Linq;
using System.Collections.Generic;
using Grasshopper.Kernel.Expressions;
using Grasshopper.Kernel.Data;
using System.Text.RegularExpressions;
using Grasshopper.Kernel;

namespace Hive.IO.GhValueLists
{
    /// <summary>
    /// A ValueList for all the properties from the Results class that we want to expose to the user.
    /// </summary>
    public class GhListResults : GH_ValueList
    {
        public GhListResults()
        {
            this.Name = "Results Type";
            this.NickName = "HiveResultsType";
            this.Description = "Names of possible results to extract from <Hive.IO.Results.Results>, sorted by KPI, time resolution, and more.";
            this.Category = "[hive]";
            this.SubCategory = "IO";
            Load();
        }

        public override GH_Exposure Exposure => GH_Exposure.secondary;

        private void Load()
        {
            var propsChosen = new Dictionary<string, string>();
            var items = new Dictionary<string, string>();

            foreach (var prop in typeof(Results.Results).GetProperties())
            {
                var attrs = Attribute.GetCustomAttributes(prop);
                var prettyName = attrs
                    .Where(a => a is IResultAttribute)
                    .Cast<IResultAttribute>()
                    .OrderBy(a => a.Rank)
                    .Select(a => a.Name)
                    .ToArray();

                if (prettyName.Length > 0)
                {
                    items[prop.Name] =
                        prettyName[0] == "Energy"
                            ? prettyName[0] + " - " + SplitCamelCase(prop.Name.Replace("Total", ""))
                            : prettyName[0] + " - " + string.Join(" ", prettyName.Skip(1));
                }
            }

            this.ListItems.Clear();
            this.ListItems.AddRange(items
                .OrderBy(prop => prop.Value)
                .Select(p => new GH_ValueListItem(p.Value, p.Key))
            );
        }

        // See https://stackoverflow.com/questions/28695172/converting-c-sharp-regex-to-javascript-gives-error-invalid-group
        private string SplitCamelCase(string key)
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

       // protected override System.Drawing.Bitmap Icon => Properties.Resources.IO_Para_InputSIAroom; // FIXME

        public override Guid ComponentGuid => new Guid("7CD42802-4586-4821-B511-F07A819A3E2D");
    }
}
