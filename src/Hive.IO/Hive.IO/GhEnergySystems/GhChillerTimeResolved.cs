using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using System.Linq;

namespace Hive.IO.GhEnergySystems
{
    public class GhChillerTimeResolved : GH_Component
    {

        //Chiller (air con), time resolved
        //Eq. (A.8) from: 10.1016/j.apenergy.2019.03.177
        //found in: 10.1016/j.energy.2004.08.004

        //arguments:
        //    - cooling loads [kWh], time resolved
        //    - electricity cost [CHF/kWh], time resolved
        //    - electricity emissions [kgCO2/kWh eq.], time resolved
        //    - ambient temperature [°C], time resolved

        //outputs (all time resolved):
        //    - electricity loads [kWh]
        //    - total cost [CHF]
        //    - total carbon [kgCO2]
        //    - COP of chiller [-]

        public GhChillerTimeResolved() :
            base("Chiller time resolved Energy System", "ChillerTimeResolved",
                "Calculates time resolved operating cost and carbon emissions of a chiller (split A/C) to meet cooling loads.",
                "[hive]", "Energy Systems")
        {
        }

        public override GH_Exposure Exposure => GH_Exposure.secondary;

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("cooling_loads", "clg", "Cooling loads [kWh], time series", GH_ParamAccess.list);
            pManager.AddNumberParameter("elec_cost", "cost", "Cost of electricity [CHF/kWh], time series", GH_ParamAccess.list);
            pManager.AddNumberParameter("elec_emissions", "CO2", "Carbon emissions of electricity [kgCO2/kWh eq.], time series", GH_ParamAccess.list);
            pManager.AddNumberParameter("temperature", "temp", "Ambient temperature at the inlet of the chiller [°C], time series", GH_ParamAccess.list);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("elec", "elec", "Electricity loads from operating the chiller [kWh]", GH_ParamAccess.list);
            pManager.AddNumberParameter("cost", "cost", "Total operation cost [CHF]", GH_ParamAccess.list);
            pManager.AddNumberParameter("carbon", "carbon", "Total operation carbon emissions [kgCO2eq.]", GH_ParamAccess.list);
            pManager.AddNumberParameter("COP", "COP", "Time resolved COP of the chiller [-]", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<double> cooling_loads = new List<double>();
            if (!DA.GetDataList(0, cooling_loads)) return;
            List<double> elec_cost = new List<double>();
            if (!DA.GetDataList(1, elec_cost)) return;
            List<double> elec_emissions = new List<double>();
            if (!DA.GetDataList(2, elec_emissions)) return;
            List<double> temperature = new List<double>();
            if (!DA.GetDataList(3, temperature)) return;


            // parameters from Eq. (A.8) in 10.1016/j.apenergy.2019.03.177
            var ac_1 = 638.95;
            var ac_2 = 4.238;
            var ac_3 = 100.0;
            var ac_4 = 3.534;

            var horizon = new[] { cooling_loads.Count, elec_cost.Count, elec_emissions.Count, temperature.Count }.Min();

            var elec_load = new double[horizon];
            var total_cost = new double[horizon];
            var total_emissions = new double[horizon];
            var COP = new double[horizon];

            foreach (var t in Enumerable.Range(0, horizon))
            {
                COP[t] = ((ac_1 - ac_2 * temperature[t]) / (ac_3 + ac_4 * temperature[t]));
                elec_load[t] = cooling_loads[t] / COP[t];
                total_cost[t] = elec_load[t] * elec_cost[t];
                total_emissions[t] = elec_load[t] * elec_emissions[t];
            }

            DA.SetDataList(0, elec_load);
            DA.SetDataList(1, total_cost);
            DA.SetDataList(2, total_emissions);
            DA.SetDataList(3, COP);
        }

        protected override System.Drawing.Bitmap Icon => Properties.Resources.EnergySystems_Chiller_TimeResult;

        public override Guid ComponentGuid => new Guid("e6b97781-f103-41f2-b967-1254e8e9e220");
    }
}


