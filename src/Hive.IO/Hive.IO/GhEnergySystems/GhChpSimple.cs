
using System;
using Grasshopper.Kernel;

namespace Hive.IO.GhEnergySystems
{
    // 
    // should do same as boiler, but it has both electricity and heating output
    // So what are inputs / outputs then?
    // Maybe component that could work with both:
    // either input electricity, and outputs heating loads
    // or input heating load, and it tells how much electricity we also get
    // 
    // output for sure: cost and carbon
    // 
    // inputs:
    //     - eta (efficiency from gas to elec)
    //     - htp (heat-to-power ratio)
    // 
    public class GhChpSimple : GH_Component
    {
        public GhChpSimple()
          : base("Combined Heat and Power Energy System C#", "ChpSimple",
              "Calculates total heating or electricity generation, consumed fuel, operating cost and carbon emissions from a Combined Heat and Power system.",
              "[hive]", "Energy Systems C#")
        {
        }

        public override GH_Exposure Exposure => GH_Exposure.primary;


        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("htg_or_elec_in", "str", "Heating or electricity in as loads? {'heating_in', 'elec_in'}. (default: 'heating_in')", GH_ParamAccess.item, "heating_in");
            pManager.AddNumberParameter("loads", "loads", "Loads in [kWh]. Either heating or electricity loads that need to be supplied.", GH_ParamAccess.item);
            pManager.AddNumberParameter("eta", "eta", "Electric efficiency of CHP, i.e. from fuel into electricity [-]", GH_ParamAccess.item);
            pManager.AddNumberParameter("htp", "htp", "Heat-to-power ratio, i.e. how much heat comes with generated electricity [-]. E.g. htp=1.5 will yield in 1.5 kW heat for 1 kW elec", GH_ParamAccess.item);
            pManager.AddNumberParameter("fuelcost", "cost", "Fuel cost [CHF/kWh]", GH_ParamAccess.item);
            pManager.AddNumberParameter("fuelemissions", "carbon", "Fuel emissions [kgCO2/kWh]", GH_ParamAccess.item);
            pManager[0].Optional = true;
        }


        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("gas_consumed", "gas", "Total gas consumed [kWh eq.]", GH_ParamAccess.item);
            pManager.AddNumberParameter("total_cost", "cost", "Total operation cost [CHF]", GH_ParamAccess.item);
            pManager.AddNumberParameter("total_emissions", "carbon", "Total carbon emissions from operation [kgCO2]", GH_ParamAccess.item);
            pManager.AddNumberParameter("heating_gen", "htg_gen", "Heating energy generated from CHP [kWh]", GH_ParamAccess.item);
            pManager.AddNumberParameter("elec_gen", "el_gen", "Electricity generated from CHP [kWh]", GH_ParamAccess.item);
        }


        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string htg_or_elec_in = "heating_in";
            if (!DA.GetData(0, ref htg_or_elec_in)) return;
            double loads = new double();
            if (!DA.GetData(1, ref loads)) return;
            double eta = new double();
            if (!DA.GetData(2, ref eta)) return;
            double htp = new double();
            if (!DA.GetData(3, ref htp)) return;
            double fuelcost = new double();
            if (!DA.GetData(4, ref fuelcost)) return;
            double fuelemissions = new double();
            if (!DA.GetData(5, ref fuelemissions)) return;

            double heatingGen;
            double elecGen;

            // if we get heating loads in, we wanna know how much electricity is produced with the CHP
            if (htg_or_elec_in == "heating_in")
            {
                heatingGen = loads;
                elecGen = heatingGen / htp;
            }
            else
            {
                elecGen = loads;
                heatingGen = elecGen * htp;
            }

            double fuelConsumed = elecGen / eta;
            double totalCost = fuelConsumed * fuelcost;
            double totalEmissions = fuelConsumed * fuelemissions;

            DA.SetData(0, fuelConsumed);
            DA.SetData(1, totalCost);
            DA.SetData(2, totalEmissions);
            DA.SetData(3, heatingGen);
            DA.SetData(4, elecGen);
        }

        protected override System.Drawing.Bitmap Icon => Properties.Resources.EnergySystems_Combined;


        public override Guid ComponentGuid => new Guid("15ea9737-e422-4e02-bec8-44fdf1531568");
    }  
}
