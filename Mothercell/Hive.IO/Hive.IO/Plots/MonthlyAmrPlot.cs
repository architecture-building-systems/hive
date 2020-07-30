using System.Drawing;

namespace Hive.IO.Plots
{
    public class MonthlyAmrPlot: AmrPlotBase
    {
        public MonthlyAmrPlot(string title, AmrPlotDataAdaptor data, AmrPlotStyle style) : base(title, data, style)
        {
        }

        protected override void RenderPlot(Graphics graphics)
        {
            RenderEmbodiedBuildings(graphics);
            RenderEmbodiedSystems(graphics);
            RenderOperationBuildings(graphics);
            RenderOperationSystems(graphics);
        }

        private void RenderOperationBuildings(Graphics graphics)
        {
        }

        private void RenderOperationSystems(Graphics graphics)
        {
            
        }

        private void RenderEmbodiedSystems(Graphics graphics)
        {
            
        }

        private void RenderEmbodiedBuildings(Graphics graphics)
        {
            
        }
    }
}
