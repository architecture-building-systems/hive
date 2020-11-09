using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Hive.IO.EnergySystems;
using Rhino.Geometry;

namespace Hive.IO.GhDistributors
{
    public class GhConversionTech : GH_Component
    {
        public GhConversionTech()
          : base("Distributor ConversionTech Hive", "HiveDistConversionTech",
              "Distributor for Hive.IO.EnergySystems.ConversionTech",
              "[hive]", "IO-Core")
        {
        }


        public override GH_Exposure Exposure => GH_Exposure.secondary;


        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Hive.IO.EnergySystem.ConversionTech", "ConversionTech", "Hive.IO.EnergySystem.ConversionTech; Boiler, ASHP, CHP, chiller, etc.", GH_ParamAccess.list);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Hive.IO.EnergySystem.AirSourceHeatPump", "AirSourceHeatPump", "Hive.IO.EnergySystems.AirSourceHeatPump", GH_ParamAccess.item);
            pManager.AddGenericParameter("Hive.IO.EnergySystem.Chiller", "Chiller", "Hive.IO.EnergySystems.Chiller", GH_ParamAccess.item);
            pManager.AddGenericParameter("Hive.IO.EnergySystems.Boiler", "Boiler", "Hive.IO.EnergySystems.Boiler", GH_ParamAccess.item);
            pManager.AddGenericParameter("Hive.IO.EnergySystems.CombinedHeatPowert", "CHP", "Hive.IO.EnergySystems.CombinedHeatPower", GH_ParamAccess.item);
            pManager.AddGenericParameter("Hive.IO.EnergySystems.HeatExchanger", "HX", "Hive.IO.EnergySystems.HeatCoolExchanger, this one here is heat exchanger", GH_ParamAccess.item);
            pManager.AddGenericParameter("Hive.IO.EnergySystems.CoolExchanger", "CX", "Hive.IO.EnergySystems.HeatCoolExchanger, this one here is cooling exchanger", GH_ParamAccess.item);
            pManager.AddGenericParameter("Hive.IO.EnergySystem.DirectElectricity", "DirectElectricity", "Hive.IO.EnergySystems.DirectElectricity", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var inputObjects = new List<GH_ObjectWrapper>();
            if (!DA.GetDataList(0, inputObjects)) return;

            AirSourceHeatPump ashp = null;
            Chiller chiller = null;
            CombinedHeatPower chp = null;
            GasBoiler boiler = null;
            HeatCoolingExchanger hx = null;
            HeatCoolingExchanger cx = null;
            HeatCoolingExchanger hcx = null;
            DirectElectricity elec = new DirectElectricity(0.0, 0.0, 0.0, 1.0);

            

            foreach (GH_ObjectWrapper convTech in inputObjects)
            {
                if (convTech.Value is AirSourceHeatPump)
                {
                    //ashp.Add(convTech.Value as AirSourceHeatPump); 
                    ashp = convTech.Value as AirSourceHeatPump;
                }
                else if (convTech.Value is Chiller)
                {
                    //chiller.Add(convTech.Value as Chiller);
                    chiller = convTech.Value as Chiller;
                }
                else if (convTech.Value is CombinedHeatPower)
                    chp = convTech.Value as CombinedHeatPower;
                else if (convTech.Value is GasBoiler)
                    boiler = convTech.Value as GasBoiler;
                else if (convTech.Value is HeatCoolingExchanger)
                {
                    hcx = convTech.Value as HeatCoolingExchanger;
                    if (hcx.IsHeating && !hcx.IsCooling)
                        hx = hcx;
                    else if (hcx.IsCooling && !hcx.IsHeating)
                        cx = hcx;
                }
                //else if(convTech.Value is DirectElectricity)
                //    elec= convTech.Value as DirectElectricity;
            }

            DA.SetData(0, ashp);
            DA.SetData(1, chiller);
            DA.SetData(2, boiler);
            DA.SetData(3, chp);
            DA.SetData(4, hx);
            DA.SetData(5, cx);
            DA.SetData(6, elec);
        }

        protected override System.Drawing.Bitmap Icon => Properties.Resources.IOCore_Distconvtech;

        public override Guid ComponentGuid => new Guid("23d890ab-f206-4d5a-85c3-52d17c5865ec");
    }
}