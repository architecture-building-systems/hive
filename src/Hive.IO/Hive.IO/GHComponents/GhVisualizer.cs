using System;
using System.Drawing;
using System.Linq;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Hive.IO.DataHandling;
using Hive.IO.Plots;

namespace Hive.IO.GHComponents
{

    public class GhVisualizer : GH_Param<GH_ObjectWrapper>
    {

        public GhVisualizer() : base("Hive.IO.Visualizer", "Hive.IO.Visualizer",
            "Hive Visualizer for simulation results",
            "[hive]", "IO", GH_ParamAccess.item)
        {
            Results = new ResultsPlotting(new Results());
        }

        public ResultsPlotting Results { get; private set; }


        public override GH_ParamKind Kind => GH_ParamKind.floating;

        public override string TypeName => "HiveResults";

        public override GH_Exposure Exposure => GH_Exposure.secondary;

        private GhVisualizerAttributes VisualizerAttributes => (GhVisualizerAttributes) m_attributes;


        /// <summary>
        /// FIXME: print out some of the data to see what it looks like
        /// </summary>
        protected override void OnVolatileDataCollected()
        {
            if (m_data.IsEmpty)
            {
                Results = new ResultsPlotting(new Results());
            }
            else
            {
                Results = new ResultsPlotting(m_data.First().Value as Results);
            }


            // notify the plots in case they are caching
            VisualizerAttributes.NewData(Results);
        }


        public override void CreateAttributes()
        {
            m_attributes = new GhVisualizerAttributes(this);
        }

        public override Guid ComponentGuid => new Guid("7b4ece55-07a0-4e87-815a-e3724a1317b1");

        //You can add image files to your project resources and access them like this:
        // return Resources.IconForThisComponent;
        protected override Bitmap Icon => Properties.Resources.IO_Visualizer;
    }
}
