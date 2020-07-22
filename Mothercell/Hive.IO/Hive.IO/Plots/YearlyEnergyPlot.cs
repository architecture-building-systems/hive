using System;
using System.Drawing;

namespace Hive.IO.Plots
{
    public class YearlyEnergyPlot: YearlyPlotBase
    {
        protected override SolidBrush BuildingsBrush => new SolidBrush(Color.FromArgb(221, 229, 124));
        protected override SolidBrush SystemsBrush => new SolidBrush(Color.FromArgb(145, 153, 48));

        protected override string Title => "Energy";
        protected override string Unit => Normalized? "kWh/m²" : "kWh";
        protected override float EmbodiedSystems => (float)Results.EmbodiedEnergySystems(Normalized);
        protected override float EmbodiedBuildings => (float)Results.EmbodiedEnergyBuildings(Normalized);
        protected override float OperationSystems => (float) Results.OperationEnergySystems(Normalized);
        protected override float OperationBuildings => (float) Results.OperationEnergyBuildings(Normalized);

        public YearlyEnergyPlot(bool normalized) : base(normalized)
        {
        }
    }
}

