using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Hive.IO.EnergySystems;
using Rhino.Geometry;

namespace Hive.IO.GHComponents
{
    public class GhDistributorConversionTech : GH_Component
    {
        public GhDistributorConversionTech()
          : base("Distributor ConversionTech Hive", "HiveDistConversionTech",
              "Distributor for Hive.IO.EnergySystems.ConversionTech",
              "[hive]", "IO")
        {
        }


        public override GH_Exposure Exposure => GH_Exposure.quarternary;


        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Hive.IO.EnergySystem.ConversionTech", "ConversionTech", "Hive.IO.EnergySystem.ConversionTech; Boiler, ASHP, CHP, chiller, etc.", GH_ParamAccess.list);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Hive.IO.EnergySystem.AirSourceHeatPump", "AirSourceHeatPump", "Hive.IO.EnergySystem.AirSourceHeatPump", GH_ParamAccess.list);
            pManager.AddGenericParameter("Hive.IO.EnergySystem.Chiller", "Chiller", "Hive.IO.EnergySystem.Chiller", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var inputObjects = new List<GH_ObjectWrapper>();
            if (!DA.GetDataList(0, inputObjects)) return;

            var ashp = new List<AirSourceHeatPump>();
            var chiller = new List<Chiller>();

            foreach (GH_ObjectWrapper convTech in inputObjects)
            {
                if(convTech.Value is AirSourceHeatPump)
                    ashp.Add(convTech.Value as AirSourceHeatPump);
                else if(convTech.Value is Chiller)
                    chiller.Add(convTech.Value as Chiller);
            }

            DA.SetDataList(0, ashp);
            DA.SetDataList(1, chiller);
        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return null;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("23d890ab-f206-4d5a-85c3-52d17c5865ec"); }
        }
    }
}