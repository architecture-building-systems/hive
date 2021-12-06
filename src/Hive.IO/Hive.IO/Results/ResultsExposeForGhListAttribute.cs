using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

    //public struct Keys
    //{
    //    string Lifetime => "Lifetime";
    //    string Yearly => "Yearly";
    //    string Monthly => "Monthly";
    //    string Embodied => "Embodied";
    //    string Operational => "Operational";
    //    string Construction => "Construction";
    //    string Systems => "Systems";



    //}
    
    internal enum Keys
    {
        Lifetime, Yearly, Monthly, 
        Energy, Emissions, Cost,
        Embodied, Operational, 
        Construction, Systems,

        None
    }

    /// <summary>
    /// Attribute for which properties to include in the GhListResults class.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    internal class ResultsExposeForGhListAttribute : ExposeForGhListAttribute
    {
        public ResultsExposeForGhListAttribute(Keys kpi,
            Keys embodiedOrOperational,
            Keys constructionOrSystems,
            Keys timeResolution,
            bool levelised = false)
        {
            string GetName(Keys x) => x == Keys.None ? "" : Enum.GetName(typeof(Keys), x);
            base.Name = $"{GetName(kpi)} {GetName(timeResolution)} {(levelised ? "(Levelised)" : "")} - {GetName(embodiedOrOperational)} {GetName(constructionOrSystems)}";
        }
    }
}
