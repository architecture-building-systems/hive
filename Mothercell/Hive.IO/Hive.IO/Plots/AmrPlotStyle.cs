using System.Drawing;

namespace Hive.IO.Plots
{
    public abstract class AmrPlotStyle
    {
        public  AmrPlotStyle()
        {
        }

        public abstract SolidBrush BuildingsBrush { get; }
        public abstract SolidBrush SystemsBrush { get; }
    }

    public class EnergyPlotStyle : AmrPlotStyle
    {
        public override SolidBrush BuildingsBrush => new SolidBrush(Color.FromArgb(221, 229, 124));
        public override SolidBrush SystemsBrush => new SolidBrush(Color.FromArgb(145, 153, 48));
    }

    public class CostsPlotStyle : AmrPlotStyle
    {
        public override SolidBrush BuildingsBrush => new SolidBrush(Color.FromArgb(219, 198, 163));
        public override SolidBrush SystemsBrush => new SolidBrush(Color.FromArgb(143, 122, 87));
    }

    public class EmissionsPlotStyle : AmrPlotStyle
    {
        public override SolidBrush BuildingsBrush => new SolidBrush(Color.FromArgb(177, 218, 143));
        public override SolidBrush SystemsBrush => new SolidBrush(Color.FromArgb(100, 141, 66));
    }
}