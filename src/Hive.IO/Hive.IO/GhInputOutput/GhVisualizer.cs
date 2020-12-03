using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using GH_IO.Serialization;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Hive.IO.Building;
using Hive.IO.Results;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Rhino;

namespace Hive.IO.GhInputOutput
{

    public class GhVisualizer : GH_Param<GH_ObjectWrapper>
    {

        public GhVisualizer() : base("Results Visualizer Hive", "HiveResultsVisualizer",
            "Hive Visualizer for plotting graphs of Hive simulation results",
            "[hive]", "IO", GH_ParamAccess.item)
        {
            Results = new ResultsPlotting(new Results.Results());
            PlotProperties = new Dictionary<string, string>();  // see Read / Write
        }

        public Dictionary<string, string> PlotProperties { get; private set; }

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
                Results = new ResultsPlotting(new Results.Results());
            }
            else
            {
                if (m_data.First().Value is Results.Results results)
                {
                    Results = new ResultsPlotting(results);
                }
                else if (m_data.First().Value is ResultsDataType resultsDataType)
                {
                    Results = new ResultsPlotting(resultsDataType.Value);
                }
                else
                {
                    Results = new ResultsPlotting(new Results.Results());
                }
            }

            // notify the plots in case they are caching
            VisualizerAttributes.NewData(Results);
        }

        #region PlotProperties
        private static readonly ITraceWriter TraceWriter = new MemoryTraceWriter();
        private static readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings
        {
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
            TypeNameHandling = TypeNameHandling.All,
            TraceWriter = TraceWriter
        };

        public override bool Read(GH_IReader reader)
        {
            try
            {
                var json = reader.GetString("PlotProperties");
                PlotProperties = JsonConvert.DeserializeObject<Dictionary<string, string>>(json, JsonSerializerSettings);

            }
            catch (Exception ex)
            {
                // let's not worry too much about not being able to read the state...
                RhinoApp.Write($"GhVisualizer.Read: Could not read PlotProperties {ex}");
                PlotProperties = new Dictionary<string, string>();
            }
            return base.Read(reader);
        }

        public override bool Write(GH_IWriter writer)
        {
            try
            {
                writer.SetString("PlotProperties", JsonConvert.SerializeObject(PlotProperties, Formatting.Indented, JsonSerializerSettings));
                return base.Write(writer);
            }
            catch (Exception ex)
            {
                // let's not freak out - this probably never happens anyway
                RhinoApp.Write($"GhVisualizer.Write: Could not write PlotProperties {ex}");
                return base.Write(writer);
            }
        }
        #endregion PlotProperties

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
