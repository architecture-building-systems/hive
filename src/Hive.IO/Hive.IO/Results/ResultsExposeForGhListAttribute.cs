using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Hive.IO.Results
{


    /// <summary>
    /// Attribute for which properties to include in the GhListResults class.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ExposeForGhListAttribute : Attribute
    {
        internal string Name;
    }

    internal enum Keys
    {
        Energy = 0, Emissions = 1, Cost = 2,
        Lifetime = 10, Yearly = 11, Monthly = 12, 
        Embodied = 20, Operational = 21, 
        Construction = 30, Systems = 31,

        None = 100
    }

    /// <summary>
    /// Attribute for which properties to include in the GhListResults class.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    internal class ResultsExposeForGhListAttribute : ExposeForGhListAttribute
    {
        // this is not pretty... 
        public readonly Keys Kpi;
        public readonly Keys EmbodiedOperational;
        public readonly Keys ConstructionSystems;
        public readonly Keys TimeResolution;
        public readonly bool Levelised;

        public ResultsExposeForGhListAttribute(Keys kpi,
            Keys timeResolution,
            Keys embodiedOrOperational = Keys.None,
            Keys constructionOrSystems = Keys.None,
            bool levelised = false)
        {
            Kpi = kpi;
            TimeResolution = timeResolution;
            EmbodiedOperational = embodiedOrOperational;
            ConstructionSystems = constructionOrSystems;
            Levelised = levelised;

            string GetName(Keys x) => x == Keys.None ? "" : Enum.GetName(typeof(Keys), x);

            if (kpi == Keys.Emissions || kpi == Keys.Cost)
            {
                base.Name = $"{GetName(kpi)} {GetName(timeResolution)} {(levelised ? "(Levelised)" : "")} - {GetName(embodiedOrOperational)} {GetName(constructionOrSystems)}";
            }
            else if (kpi == Keys.Energy)
            {
                base.Name = $"{GetName(kpi)}"; /// TODO prettify happens elsewhere
            }
        }
    }
}
