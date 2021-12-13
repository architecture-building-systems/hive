using System;

namespace Hive.IO.Results
{
    /// <summary>
    /// Base interface for attributes for results. Name is the name ot be used for display, 
    /// Rank indicates the ordering of each word for the displayed word on the Grasshopper ValueList.
    /// </summary>
    public interface IResultAttribute
    {
        int Rank { get; }
        string Name { get; }
    }


    // KPI Attributes

    [AttributeUsage(AttributeTargets.Property)]
    public class EnergyAttribute : Attribute, IResultAttribute
    {
        public int Rank => 1;
        public string Name => "Energy";
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class EmissionsAttribute : Attribute, IResultAttribute
    {
        public int Rank => 1;
        public string Name => "Emissions";
    }

    [AttributeUsage(AttributeTargets.Property)]
    internal class CostAttribute : Attribute, IResultAttribute
    {
        public int Rank => 1;
        public string Name => "Cost";
    }

    // Time Resolution Attributes

    [AttributeUsage(AttributeTargets.Property)]
    internal class LifetimeAttribute : Attribute, IResultAttribute
    {
        public int Rank => 2;
        public string Name => "Lifetime";
    }

    [AttributeUsage(AttributeTargets.Property)]
    internal class YearlyAttribute : Attribute, IResultAttribute
    {
        public int Rank => 2;
        public string Name => "Yearly";
    }

    [AttributeUsage(AttributeTargets.Property)]
    internal class MonthlyAttribute : Attribute, IResultAttribute
    {
        public int Rank => 2;
        public string Name => "Monthly";
    }

    [AttributeUsage(AttributeTargets.Property)]
    internal class LevelisedAttribute : Attribute, IResultAttribute
    {
        public int Rank => 3;
        public string Name => "Levelized";
    }

    // Embodied / Operational Attributes

    [AttributeUsage(AttributeTargets.Property)]
    internal class EmbodiedAttribute : Attribute, IResultAttribute
    {
        public int Rank => 4;
        public string Name => "Embodied";
    }

    [AttributeUsage(AttributeTargets.Property)]
    internal class OperationalAttribute : Attribute, IResultAttribute
    {
        public int Rank => 4;
        public string Name => "Operational";
    }


    // Construction / Systems Attributes

    [AttributeUsage(AttributeTargets.Property)]
    internal class ConstructionAttribute : Attribute, IResultAttribute
    {
        public int Rank => 5;
        public string Name => "Construction";
    }

    [AttributeUsage(AttributeTargets.Property)]
    internal class SystemsAttribute : Attribute, IResultAttribute
    {
        public int Rank => 5;
        public string Name => "Systems";
    }
}
