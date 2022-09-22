using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;

namespace Hive.IO.GhDistributors
{
    public class GhDistConduit : GH_Component
    {
        public GhDistConduit()
          : base("Distributor Conduit Hive", "HiveDistConduit",
              "Distributor for PV or Window irradiation data to Conduit HUD chart",
              "[hive]", "IO-Core")
        {
        }

        public override GH_Exposure Exposure => GH_Exposure.secondary;

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBooleanParameter("Chart Toggle", "toggle", "Bool; if true, HUD displays PV irradiation data, if false, window irradiation data", GH_ParamAccess.item);
            pManager.AddGenericParameter("PV data", "PVData", "List of PV irradiation data points", GH_ParamAccess.list);
            pManager.AddGenericParameter("Window data", "WindowData", "List of Window irradiation data points", GH_ParamAccess.list);

            pManager[1].Optional = true;
            pManager[2].Optional = true;
        }

        

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Plot data", "PlotData", "Irradiation data to be displayed in HUD chart", GH_ParamAccess.list);
            pManager.AddGenericParameter("Title Plot 1", "TitlePlot1", "Title of Plot 1", GH_ParamAccess.item);
            pManager.AddGenericParameter("Title Plot 2", "TitlePlot2", "Title of Plot 2", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var toggle = new bool();
            DA.GetData(0, ref toggle);
            var PV_data = new List<GH_ObjectWrapper>();
            DA.GetDataList(1, PV_data);
            var Window_data = new List<GH_ObjectWrapper>();
            DA.GetDataList(2, Window_data);

            var data = new List<GH_ObjectWrapper>();
            var title1 = "";
            var title2 = "";

            if (toggle)
            {
                data = PV_data;
                title1 = "Total PV Irradiance (W)";
                title2 = "PV Irradiance(W)";
            } 
            else
            {
                data = Window_data;
                title1 = "Total Windows Irradiance (W)";
                title2 = "Window Irradiance(W)";
            }

            DA.SetData(0, data);
            DA.SetData(1, title1);
            DA.SetData(2, title2);
        }

        //protected override System.Drawing.Bitmap Icon => Properties.Resources.IOCore_Distconvtech;

        public override Guid ComponentGuid => new Guid("d4c71228-b0bd-4c8b-b24e-a73164189fcd");
    }
}
