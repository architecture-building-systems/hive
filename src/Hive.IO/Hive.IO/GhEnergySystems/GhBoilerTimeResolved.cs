using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using System.Linq;

namespace Hive.IO.GhEnergySystems
{
    public class GhBoilerTimeResolved : GH_Component
    {
        public GhBoilerTimeResolved()
          : base("Boiler time resolved Energy System", "BoilerTimeResolved",
              "Calculates time resolved consumed fuel, operating cost and carbon emissions of a boiler to meet heating loads.",
              "[hive]", "Energy Systems")
        {
        }

        public override GH_Exposure Exposure => GH_Exposure.secondary;


        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("heating_loads", "htg", "Heating loads, time resolved [kWh/h]", GH_ParamAccess.list);
            pManager.AddNumberParameter("carrier_cost", "cost", "Cost of energy carrier, time resolved [CHF/kWh]", GH_ParamAccess.list);
            pManager.AddNumberParameter("carrier_emissions", "CO2", "Carbon emissions of energy carrier time resolved [kgCO2/kWheq.]", GH_ParamAccess.list);
            pManager.AddNumberParameter("efficiency", "eta", "Efficiency of boiler, time resolved [-/h]. Could change depending on supply temp.", GH_ParamAccess.list);
        }


        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("gas_consumed", "gas", "Total gas consumed [kWh eq.]", GH_ParamAccess.list);
            pManager.AddNumberParameter("cost", "cost", "Total operation cost [CHF/h]", GH_ParamAccess.list);
            pManager.AddNumberParameter("carbon", "carbon", "Total operation carbon emissions [kgCO2eq./h]", GH_ParamAccess.list);
        }


        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<double> heating_loads = new List<double>();
            if (!DA.GetDataList(0, heating_loads)) return;
            List<double> carrier_cost = new List<double>();
            if (!DA.GetDataList(1, carrier_cost)) return;
            List<double> carrier_emissions = new List<double>();
            if (!DA.GetDataList(2, carrier_emissions)) return;
            List<double> eta = new List<double>();
            if (!DA.GetDataList(3, eta)) return;

            var horizon = new[] { heating_loads.Count, carrier_cost.Count, carrier_emissions.Count, eta.Count }.Min();

            var cost = new double[horizon];
            var carbon = new double[horizon];
            var gas_consumed = new double[horizon];

            foreach (var t in Enumerable.Range(0, horizon))
            {
                gas_consumed[t] = heating_loads[t] * eta[t];
                cost[t] = gas_consumed[t] * carrier_cost[t];
                carbon[t] = gas_consumed[t] * carrier_emissions[t];
            }

            DA.SetData(0, gas_consumed);
            DA.SetData(1, cost);
            DA.SetData(2, carbon);
        }


        protected override System.Drawing.Bitmap Icon => Properties.Resources.EnergySystems_Boiler_TimeResult;


        public override Guid ComponentGuid => new Guid("b74557e1-e382-44fd-b4a9-7c315e670b51");
    }

}
