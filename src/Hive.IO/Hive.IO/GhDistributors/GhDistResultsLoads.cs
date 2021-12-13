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
            var results = new Results.Results();
            if (!DA.GetData(0, ref results)) return;

            double[] heatingLoads = results.TotalFinalHeatingMonthly;
            double[] coolingLoads = results.TotalFinalCoolingMonthly;
            double[] electricityLoads = results.TotalFinalElectricityMonthly;

            var windowSolarGains = new List<object>();
            int windowCount = 0;
            foreach (double[] window in results.MonthlySolarGainsPerWindow) {
                windowSolarGains.Add("______");
                windowSolarGains.Add("window " + windowCount);
                foreach (double solarGain in window) {
                    windowSolarGains.Add(solarGain);
                }
                windowCount++;
            }

            var gainsAndLosses = new List<string>()
            {
                "SolarGains: " + results.TotalSolarGains,
                "InternalGains: " + results.TotalInternalGains ,
                "PrimaryEnergy: " + results.TotalPrimaryEnergyNonRenewableMonthly.Sum() ,
                "VentilationHeatGains: " + results.TotalVentilationHeatGains ,
                "EnvelopeHeatGains: " + results.TotalOpaqueTransmissionHeatGains ,
                "WindowHeatGains: " + results.TotalWindowTransmissionHeatGains ,
                "ElectricityDemand: " + results.TotalConsumedElectricityMonthly.Sum() ,
                "VentilationHeatLosses: " + results.TotalVentilationHeatLosses, 
                "EnvelopeHeatLosses: " + results.TotalOpaqueTransmissionHeatLosses,  
                "WindowHeatLosses: " + results.TotalWindowTransmissionHeatLosses,
                "SystemConversionLosses: " + results.TotalSystemLosses
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