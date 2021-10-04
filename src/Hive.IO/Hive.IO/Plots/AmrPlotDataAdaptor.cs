using Hive.IO.Results;
using System.Linq;

namespace Hive.IO.Plots
{
    /// <summary>
    /// Describes the interface amr plots use to query data
    /// (basically used to select cost / emissions / energy data from a Hive.IO.Results.ResultsPlotting object.
    /// </summary>
    public abstract class AmrPlotDataAdaptor
    {
        protected ResultsPlotting Results { get; private set; }
        protected bool Normalized { get; }

        /// <summary>
        /// Allow the underlying data (ResultsPlotting) to be updated.
        /// </summary>
        /// <param name="results"></param>
        public void NewData(ResultsPlotting results)
        {
            Results = results;
        }

        public AmrPlotDataAdaptor(ResultsPlotting results, bool normalized)
        {
            Results = results;
            Normalized = normalized;
        }

        public abstract string Unit { get; }

        public abstract float EmbodiedSystems { get; }
        public abstract float EmbodiedBuildings { get; }
        public abstract float OperationSystems { get; }
        public abstract float OperationBuildings { get; }
        public float TotalEmbodied => EmbodiedSystems + EmbodiedBuildings;
        public float TotalOperation => OperationBuildings + OperationSystems;
        public float TotalBuildings => EmbodiedBuildings + OperationBuildings;
        public float TotalSystems => EmbodiedSystems + OperationSystems;
        public float Total => TotalEmbodied + TotalOperation;

        public abstract float EmbodiedSystemsYearly { get; }
        public abstract float EmbodiedBuildingsYearly { get; }
        public abstract float OperationSystemsYearly { get; }
        public abstract float OperationBuildingsYearly{ get; }
        public float TotalEmbodiedYearly => EmbodiedBuildingsYearly + EmbodiedSystemsYearly;
        public float TotalOperationYearly => OperationSystemsYearly + OperationBuildingsYearly;
        public float TotalBuildingsYearly => EmbodiedBuildingsYearly + OperationBuildingsYearly;
        public float TotalSystemsYearly => EmbodiedSystemsYearly + OperationSystemsYearly;


        public abstract float[] EmbodiedSystemsMonthly { get; }
        public abstract float[] EmbodiedBuildingsMonthly { get; }
        public abstract float[] OperationSystemsMonthly { get; }
        public abstract float[] OperationBuildingsMonthly { get; }
        public float AverageEmbodiedMonthly => EmbodiedBuildingsMonthly.Zip(EmbodiedSystemsMonthly, (a, b) => a + b).Average();
        public float AverageOperationMonthly => OperationBuildingsMonthly.Zip(OperationSystemsMonthly, (a, b) => a + b).Average();
        public float AverageBuildingsMonthly => EmbodiedBuildingsMonthly.Zip(OperationBuildingsMonthly, (a, b) => a + b).Average();
        public float AverageSystemsMonthly => EmbodiedSystemsMonthly.Zip(OperationSystemsMonthly, (a, b) => a + b).Average();
    }

    public class CostsDataAdaptor : AmrPlotDataAdaptor
    {
        public CostsDataAdaptor(ResultsPlotting results, bool normalized) : base(results, normalized)
        {
        }

        public override string Unit => Normalized ? "CHF/m²" : "CHF";
        public override float EmbodiedSystems => (float)Results.EmbodiedCostsSystems(Normalized);
        public override float EmbodiedBuildings => (float)Results.EmbodiedCostsBuildings(Normalized);
        public override float OperationSystems => (float)Results.OperationCostsSystems(Normalized);
        public override float OperationBuildings => (float)Results.OperationCostsBuildings(Normalized);
        //public override float TotalEmbodied => (float)Results.TotalEmbodiedCosts(Normalized);
        //public override float TotalOperation => (float)Results.TotalOperationCosts(Normalized);
        //public override float Total => (float)Results.TotalCosts(Normalized);

        public override float EmbodiedSystemsYearly =>
            (float)Results.EmbodiedCostsSystemsYearly(Normalized);

        public override float EmbodiedBuildingsYearly =>
            (float)Results.EmbodiedCostsBuildingsYearly(Normalized);

        public override float OperationSystemsYearly =>
            (float)Results.OperationCostsSystemsYearly(Normalized);

        public override float OperationBuildingsYearly =>
            (float)Results.OperationCostsBuildingsYearly(Normalized);

        public override float[] EmbodiedSystemsMonthly =>
            Results.EmbodiedCostsSystemsMonthly(Normalized).ToFloatArray();

        public override float[] EmbodiedBuildingsMonthly =>
            Results.EmbodiedCostsBuildingsMonthly(Normalized).ToFloatArray();

        public override float[] OperationSystemsMonthly =>
            Results.OperationCostsSystemsMonthly(Normalized).ToFloatArray();

        public override float[] OperationBuildingsMonthly =>
            Results.OperationCostsBuildingsMonthly(Normalized).ToFloatArray();
    }

    public class EmissionsDataAdaptor : AmrPlotDataAdaptor
    {
        public EmissionsDataAdaptor(ResultsPlotting results, bool normalized) : base(results, normalized)
        {
        }

        public override string Unit => Normalized ? "kgCO₂/m²" : "kgCO₂";
        public override float EmbodiedSystems => (float)Results.EmbodiedEmissionsSystems(Normalized);
        public override float EmbodiedBuildings => (float)Results.EmbodiedEmissionsBuildings(Normalized);
        public override float OperationSystems => (float)Results.OperationEmissionsSystems(Normalized);
        public override float OperationBuildings => (float)Results.OperationEmissionsBuildings(Normalized);
        //public override float TotalEmbodied => (float)Results.TotalEmbodiedEmissions(Normalized);
        //public override float TotalOperation => (float)Results.TotalOperationEmissions(Normalized);
        //public override float Total => (float)Results.TotalEmissions(Normalized);

        public override float EmbodiedSystemsYearly =>
            (float)Results.EmbodiedEmissionsSystemsYearly(Normalized);

        public override float EmbodiedBuildingsYearly =>
            (float)Results.EmbodiedEmissionsBuildingsYearly(Normalized);

        public override float OperationSystemsYearly =>
            (float)Results.OperationEmissionsSystemsYearly(Normalized);

        public override float OperationBuildingsYearly =>
            (float)Results.OperationEmissionsBuildingsYearly(Normalized);

        public override float[] EmbodiedSystemsMonthly =>
            Results.EmbodiedEmissionsSystemsMonthly(Normalized).ToFloatArray();

        public override float[] EmbodiedBuildingsMonthly =>
            Results.EmbodiedEmissionsBuildingsMonthly(Normalized).ToFloatArray();

        public override float[] OperationSystemsMonthly =>
            Results.OperationEmissionsSystemsMonthly(Normalized).ToFloatArray();

        public override float[] OperationBuildingsMonthly =>
            Results.OperationEmissionsBuildingsMonthly(Normalized).ToFloatArray();
    }

    public class EnergyDataAdaptor : AmrPlotDataAdaptor
    {
        public EnergyDataAdaptor(ResultsPlotting results, bool normalized) : base(results, normalized)
        {
        }

        public override string Unit => Normalized ? "kWh/m²" : "kWh";
        public override float EmbodiedSystems => (float)Results.EmbodiedEnergySystems(Normalized);
        public override float EmbodiedBuildings => (float)Results.EmbodiedEnergyBuildings(Normalized);
        public override float OperationSystems => (float)Results.OperationEnergySystems(Normalized);
        public override float OperationBuildings => (float)Results.OperationEnergyBuildings(Normalized);
        //public override float TotalEmbodied => (float)Results.TotalEmbodiedEnergy(Normalized);
        //public override float TotalOperation => (float)Results.TotalOperationEnergy(Normalized);
        //public override float Total => (float)Results.TotalEnergy(Normalized);

        public override float EmbodiedSystemsYearly =>
            (float)Results.EmbodiedEnergySystemsYearly(Normalized);

        public override float EmbodiedBuildingsYearly =>
            (float)Results.EmbodiedEnergyBuildingsYearly(Normalized);

        public override float OperationSystemsYearly =>
            (float)Results.OperationEnergySystemsYearly(Normalized);

        public override float OperationBuildingsYearly =>
            (float)Results.OperationEnergyBuildingsYearly(Normalized);

        public override float[] EmbodiedSystemsMonthly =>
            Results.EmbodiedEnergySystemsMonthly(Normalized).ToFloatArray();

        public override float[] EmbodiedBuildingsMonthly =>
            Results.EmbodiedEnergyBuildingsMonthly(Normalized).ToFloatArray();

        public override float[] OperationSystemsMonthly =>
            Results.OperationEnergySystemsMonthly(Normalized).ToFloatArray();

        public override float[] OperationBuildingsMonthly =>
            Results.OperationEnergyBuildingsMonthly(Normalized).ToFloatArray();
    }

}
