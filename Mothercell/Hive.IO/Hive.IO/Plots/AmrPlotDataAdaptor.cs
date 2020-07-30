using Hive.IO.DataHandling;

namespace Hive.IO.Plots
{
    /// <summary>
    /// Describes the interface amr plots use to query data
    /// (basically used to select cost / emissions / energy data from a Hive.IO.DataHandling.ResultsPlotting object.
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
        public abstract float TotalEmbodied { get; }
        public abstract float TotalOperation { get; }
        public abstract float Total { get; }
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
        public override float TotalEmbodied => (float)Results.TotalEmbodiedCosts(Normalized);
        public override float TotalOperation => (float)Results.TotalOperationCosts(Normalized);
        public override float Total => (float)Results.TotalCosts(Normalized);
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
        public override float TotalEmbodied => (float)Results.TotalEmbodiedEmissions(Normalized);
        public override float TotalOperation => (float)Results.TotalOperationEmissions(Normalized);
        public override float Total => (float)Results.TotalEmissions(Normalized);
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
        public override float TotalEmbodied => (float)Results.TotalOperationEnergy(Normalized);
        public override float TotalOperation => (float)Results.TotalEmbodiedEnergy(Normalized);
        public override float Total => (float)Results.TotalEnergy(Normalized);
    }

}
