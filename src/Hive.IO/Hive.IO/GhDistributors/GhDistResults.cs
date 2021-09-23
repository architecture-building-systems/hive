using System;
using Grasshopper.Kernel;

namespace Hive.IO.GhDistributors
{
    public class GhDistResults : GH_Component
    {

        public GhDistResults()
          : base("Distributor Results Hive", "HiveDistResults",
              "Distributor for Hive Results",
              "[hive]", "IO-Core")
        {
        }

        public override GH_Exposure Exposure => GH_Exposure.secondary;


        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Results", "Results", "Hive results of type <Hive.IO.Results>", GH_ParamAccess.item);
        }


        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            // TODO

            //pManager.AddNumberParameter("HourlyHtgSupTemp", "HourlyHtgSupTemp", "Hourly heating supply temperature", GH_ParamAccess.list);
            //pManager.AddNumberParameter("HourlyHtgRetTemp", "HourlyHtgRetTemp", "Hourly heating return temperature", GH_ParamAccess.list);
            //pManager.AddNumberParameter("HourlyClgSupTemp", "HourlyClgSupTemp", "Hourly cooling supply temperature", GH_ParamAccess.list);
            //pManager.AddNumberParameter("HourlyClgRetTemp", "HourlyClgRetTemp", "Hourly cooling return temperature", GH_ParamAccess.list);
        }


        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var results = new Results.Results();
            if (!DA.GetData(0, ref results)) return;

            // TODO

            //DA.SetDataList(0, hourlyHtgSupTemp);
            //DA.SetDataList(1, hourlyHtgRetTemp);
            //DA.SetDataList(2, hourlyClgSupTemp);
            //DA.SetDataList(3, hourlyClgRetTemp);
        }


        protected override System.Drawing.Bitmap Icon => Properties.Resources.IO_CoreMerger; // FIXME should create dedicated icon


        public override Guid ComponentGuid => new Guid("4C5EFA32-D74E-4F09-96FB-BD98A8A13B68");
    }
}
