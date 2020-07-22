using System;
using System.Drawing;

namespace Hive.IO.Plots
{
    public class YearlyEnergyPlot: YearlyPlotBase
    {
        protected override SolidBrush BuildingsBrush => new SolidBrush(Color.FromArgb(221, 229, 124));
        protected override SolidBrush SystemsBrush => new SolidBrush(Color.FromArgb(145, 153, 48));

        protected override string Title => "Energy";
        protected override string Unit => "kWh";
        protected override float EmbodiedSystems => (float)Results.EmbodiedEnergySystems;
        protected override float EmbodiedBuildings => (float)Results.EmbodiedEnergyBuildings;
        protected override float OperationSystems => (float) Results.OperationEnergySystems;
        protected override float OperationBuildings => (float) Results.OperationEnergyBuildings;
    }
}

