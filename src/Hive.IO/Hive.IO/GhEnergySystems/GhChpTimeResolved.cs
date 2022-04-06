
using System;
using System.Collections.Generic;
using Grasshopper.Kernel.Types;
using Grasshopper.Kernel;
using Hive.IO.EnergySystems;
using System.Linq;

namespace Hive.IO.GhEnergySystems
{
    // 
    // Co-generation Combined Heat and Power (CHP), time resolved
    // 
    // inputs:
    //     - htg_or_elec: string indicator whether we provide heating loads or electricity loads.
    //         E.g. if heating load is provided (i.e. this load must be met by the CHP),
    //         then the component will calculate the resulting electricity generation {'heating_in', 'elec_in'}
    //     - loads: heating or electricity loads, time resolved
    //     - eta (efficiency from gas to electricity), constant
    //     - htp (heat-to-power ratio), constant
    //     - fuel_cost, time resolved [CHF/kWh eq.]
    //     - fuel_emissions, time resolved [kgCO2/kWh eq.]
    // 
    // outputs (time resolved):
    //     - total_cost
    //     - total_carbon
    //     - htg_gen: generated heating energy
    //     - elec_gen: generated electricity
    // 
    public class GhChpTimeResolved : GH_Component
    {
        public GhChpTimeResolved()
          : base("Combined Heat and Power time resolved Energy System C#", "ChpTimeResolved",
              "Calculates time resolved heating or electricity generation, consumed fuel, operating cost and carbon emissions from a Combined Heat and Power system.",
              "[hive]", "Energy Systems C#")
        {
        }

        public override GH_Exposure Exposure => GH_Exposure.secondary;


        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("htg_or_elec_in", "str", "Heating or electricity in as loads? {'heating_in', 'elec_in'}. (default: 'heating_in')", GH_ParamAccess.item, "heating_in");
            pManager.AddNumberParameter("loads", "loads", "Loads in [kWh], time series. Either heating or electricity loads that need to be supplied.", GH_ParamAccess.list);
            pManager.AddNumberParameter("eta", "eta", "Electric efficiency of CHP, i.e. from fuel into electricity [-]", GH_ParamAccess.item);
            pManager.AddNumberParameter("htp", "htp", "Heat-to-power ratio, i.e. how much heat comes with generated electricity [-]. E.g. htp=1.5 will yield in 1.5 kW heat for 1 kW elec", GH_ParamAccess.item);
            pManager.AddNumberParameter("fuelcost", "cost", "Fuel cost [CHF/kWh], time series", GH_ParamAccess.list);
            pManager.AddNumberParameter("fuelemissions", "carbon", "Fuel emissions [kgCO2/kWh], time series", GH_ParamAccess.list);
            pManager[0].Optional = true;
        }


        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("gas_consumed", "gas", "Total gas consumed [kWh eq.]", GH_ParamAccess.item);
            pManager.AddNumberParameter("total_cost", "cost", "Time resolved operation cost [CHF]", GH_ParamAccess.item);
            pManager.AddNumberParameter("total_emissions", "carbon", "Time resolved carbon emissions from operation [kgCO2]", GH_ParamAccess.item);
            pManager.AddNumberParameter("heating_gen", "htg_gen", "Time resolved heating energy generated from CHP [kWh]", GH_ParamAccess.item);
            pManager.AddNumberParameter("elec_gen", "el_gen", "Time resolved electricity generated from CHP [kWh]", GH_ParamAccess.item);
        }


        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string htg_or_elec_in = "heating_in";
            if (!DA.GetData(0, ref htg_or_elec_in)) return;
            List<double> loads = new List<double>();
            if (!DA.GetDataList(1, loads)) return;
            double eta = new double();
            if (!DA.GetData(2, ref eta)) return;
            double htp = new double();
            if (!DA.GetData(3, ref htp)) return;
            List<double> fuel_cost = new List<double>();
            if (!DA.GetDataList(4, fuel_cost)) return;
            List<double> fuel_emissions = new List<double>();
            if (!DA.GetDataList(5, fuel_emissions)) return;

            // horizon. get the shortest array, in case inputs are not consistent
            var horizon = new[] { loads.Count, fuel_cost.Count, fuel_emissions.Count }.Min();

            // initialize empty arrays. Daren wants me to use .append
            var htg_gen = new List<double>();
            var elec_gen = new List<double>();
            var total_emissions = new List<double>();
            var total_cost = new List<double>();
            var fuel = new List<double>();

            // if we get heating loads in, we wanna know how much electricity is produced with the CHP
            if (htg_or_elec_in == "heating_in")
            {
                htg_gen = loads;
                foreach (var i in Enumerable.Range(0, horizon))
                {
                    elec_gen.Add(htg_gen[i] / htp);
                }
            }
            else
            {
                elec_gen = loads;
                foreach (var i in Enumerable.Range(0, horizon))
                {
                    htg_gen.Add(elec_gen[i] * htp);
                }
            }

            foreach (var i in Enumerable.Range(0, horizon))
            {
                fuel.Add(elec_gen[i] / eta);
                total_cost.Add(fuel[i] * fuel_cost[i]);
                total_emissions.Add(fuel[i] * fuel_emissions[i]);
            }

            DA.SetDataList(0, fuel);
            DA.SetDataList(1, total_cost);
            DA.SetDataList(2, total_emissions);
            DA.SetDataList(3, htg_gen);
            DA.SetDataList(4, elec_gen);
        }


        protected override System.Drawing.Bitmap Icon => Properties.Resources.EnergySystems_Combined_TimeResult;


        public override Guid ComponentGuid => new Guid("ef18d842-73cc-4f64-baeb-02eb8f77ce55");
    }
}
