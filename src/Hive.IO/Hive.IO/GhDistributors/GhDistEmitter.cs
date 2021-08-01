using System;
using System.Collections.Generic;
using Grasshopper.Kernel.Types;
using Grasshopper.Kernel;
using Hive.IO.EnergySystems;

namespace Hive.IO.GhDistributors
{
    public class GhDistEmitter : GH_Component
    {
        public GhDistEmitter()
          : base("Distributor Emitter Hive", "HiveDistEmitter",
              "Distributor for Hive emitters",
              "[hive]", "IO-Core")
        {
        }

        public override GH_Exposure Exposure => GH_Exposure.secondary;


        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Emitters", "Emitters", "Hive heat and cold emitters (Raumabgabe) of type <Hive.IO.EnergySystems.Emitters>", GH_ParamAccess.list);
        }


        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("HourlyHtgSupTemp", "HourlyHtgSupTemp", "Hourly heating supply temperature", GH_ParamAccess.list);
            pManager.AddNumberParameter("HourlyHtgRetTemp", "HourlyHtgRetTemp", "Hourly heating return temperature", GH_ParamAccess.list);
            pManager.AddNumberParameter("HourlyClgSupTemp", "HourlyClgSupTemp", "Hourly cooling supply temperature", GH_ParamAccess.list);
            pManager.AddNumberParameter("HourlyClgRetTemp", "HourlyClgRetTemp", "Hourly cooling return temperature", GH_ParamAccess.list);
        }


        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var inputEmitters = new List<Emitter>();
            if (!DA.GetDataList(0, inputEmitters)) return;

            var hourlyHtgSupTemp = new List<double>();
            var hourlyHtgRetTemp = new List<double>();
            var hourlyClgSupTemp = new List<double>();
            var hourlyClgRetTemp = new List<double>();


            foreach (Emitter emitter in inputEmitters)
            {
                if (emitter.IsCooling)
                {
                    hourlyClgSupTemp = new List<double>(emitter.InletCarrier.Temperature);
                    hourlyClgRetTemp = new List<double>(emitter.ReturnCarrier.Temperature);
                }
                else if (emitter.IsHeating)
                {
                    hourlyHtgSupTemp = new List<double>(emitter.InletCarrier.Temperature);
                    hourlyHtgRetTemp = new List<double>(emitter.ReturnCarrier.Temperature);
                }
            }

            DA.SetDataList(0, hourlyHtgSupTemp);
            DA.SetDataList(1, hourlyHtgRetTemp);
            DA.SetDataList(2, hourlyClgSupTemp);
            DA.SetDataList(3, hourlyClgRetTemp);

        }


        protected override System.Drawing.Bitmap Icon => Properties.Resources.IOCore_DistEmitter;


        public override Guid ComponentGuid => new Guid("11520a55-b99e-43aa-8f02-4bedc9f7c7f1"); 
       
    }
}