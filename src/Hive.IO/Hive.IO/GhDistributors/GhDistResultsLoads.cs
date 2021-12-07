using System;
using System.Collections.Generic;
using System.Linq;
using Grasshopper.Kernel;

namespace Hive.IO.GhDistributors
{
    public class GhDistResultsLoads : GH_Component
    {
        public GhDistResultsLoads()
          : base("Distributor Results Loads Hive", "HiveDistResultsLoads",
              "Distributor for Hive results with loads, gains, losses",
              "[hive]", "IO-Core")
        {
        }

        public override GH_Exposure Exposure => GH_Exposure.quarternary;


        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Results", "Results", "Hive Results of type <Hive.IO.Results.Results>", GH_ParamAccess.item);
        }


        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("HeatingLoads", "HeatingLoads", "The selected result to extract.", GH_ParamAccess.list);
            pManager.AddGenericParameter("CoolingLoads", "CoolingLoads", "The selected result to extract.", GH_ParamAccess.list);
            pManager.AddGenericParameter("ElectricityLoads", "ElectricityLoads", "The selected result to extract.", GH_ParamAccess.list);
            pManager.AddGenericParameter("WindowSolarGains", "WindowSolarGains", "The selected result to extract.", GH_ParamAccess.list);
            pManager.AddGenericParameter("GainsAndLosses", "GainsAndLosses", "The selected result to extract.", GH_ParamAccess.list);
        }


        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var x = new Results.Results();
            if (!DA.GetData(0, ref x)) return;

            var heatingLoads = x.TotalFinalHeatingMonthly;
            var coolingLoads = x.TotalFinalCoolingMonthly;
            var electricityLoads = x.TotalFinalElectricityMonthly;

            var windowSolarGains = new List<object>();
            int windowCount = 0;
            foreach (double[] window in x.MonthlySolarGainsPerWindow) {
                windowSolarGains.Add("______");
                windowSolarGains.Add("window " + windowCount);
                foreach (double solarGain in window) {
                    windowSolarGains.Add(solarGain);
                }
                windowCount++;
            }

            var gainsAndLosses = new List<string>()
            {
                "SolarGains: " + x.TotalSolarGains,
                "InternalGains: " + x.TotalInternalGains ,
                "PrimaryEnergy: " + x.TotalPrimaryEnergyNonRenewableMonthly.Sum() ,
                "VentilationHeatGains: " + x.TotalVentilationHeatGains ,
                "EnvelopeHeatGains: " + x.TotalOpaqueTransmissionHeatGains ,
                "WindowHeatGains: " + x.TotalWindowTransmissionHeatGains ,
                "ElectricityDemand: " + x.TotalConsumedElectricityMonthly.Sum() ,
                "VentilationHeatLosses: " + x.TotalVentilationHeatLosses, 
                "EnvelopeHeatLosses: " + x.TotalOpaqueTransmissionHeatLosses,  
                "WindowHeatLosses: " + x.TotalWindowTransmissionHeatLosses,
                "SystemConversionLosses: " + x.TotalSystemLosses
            };

            DA.SetDataList(0, heatingLoads);
            DA.SetDataList(1, coolingLoads);
            DA.SetDataList(2, electricityLoads);
            DA.SetDataList(3, windowSolarGains);
            DA.SetDataList(4, gainsAndLosses);
        }


        protected override System.Drawing.Bitmap Icon => Properties.Resources.IO_CoreMerger; // FIXME


        public override Guid ComponentGuid => new Guid("EBE100F9-6F0C-4F8E-9758-F5E21EA27CA4");

    }
}