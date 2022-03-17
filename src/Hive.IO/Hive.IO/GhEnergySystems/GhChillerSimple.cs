using System;
using Grasshopper.Kernel;

namespace Hive.IO.GhEnergySystems
{
    public class GhChillerSimple : GH_Component
    {
        // 
        // Chiller (air con)
        // Simple equation: electricity loads = cooling loads / COP
        // 
        // arguments:
        //     - cooling loads [kWh]
        //     - COP [-]
        //     - electricity cost [CHF/kWh]
        //     - electricity emissions [kgCO2/kWh eq.]
        // 
        // outputs:
        //     - electricity loads [kWh]
        //     - total cost [CHF]
        //     - total carbon [kgCO2]
        // 

        public GhChillerSimple() :
            base("Chiller Energy System C#", "ChillerSimple",
                "Calculates total operating cost and carbon emissions of a chiller (split A/C) to meet cooling loads.", 
                "[hive]", "Energy Systems C#")
        {
        }

        public override GH_Exposure Exposure => GH_Exposure.primary;

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("cooling_loads", "clg", "Cooling loads [kWh]", GH_ParamAccess.item);
            pManager.AddNumberParameter("elec_cost", "cost", "Cost of electricity [CHF/kWh]", GH_ParamAccess.item);
            pManager.AddNumberParameter("elec_emissions", "CO2", "Carbon emissions of electricity [kgCO2/kWheq.]", GH_ParamAccess.item);
            pManager.AddNumberParameter("COP", "COP", "COP of chiller [-]", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("elec", "elec", "Electricity loads from operating the chiller [kWh]", GH_ParamAccess.item);
            pManager.AddNumberParameter("cost", "cost", "Total operation cost [CHF]", GH_ParamAccess.item);
            pManager.AddNumberParameter("carbon", "carbon", "Total operation carbon emissions [kgCO2eq.]", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {

            double cooling_loads = 0.0;
            if (!DA.GetData(0, ref cooling_loads)) return;
            double elec_cost = 0.0;
            if (!DA.GetData(1, ref elec_cost)) return;
            double elec_emissions = 0.0;
            if (!DA.GetData(2, ref elec_emissions)) return;
            double COP = 0.0;
            if (!DA.GetData(3, ref COP)) return;

            var elec_load = cooling_loads / COP;
            var total_cost = elec_load * elec_cost;
            var total_emissions = elec_load * elec_emissions;

            DA.SetData(0, elec_load);
            DA.SetData(1, total_cost);
            DA.SetData(2, total_emissions);
        }

        protected override System.Drawing.Bitmap Icon => Properties.Resources.EnergySystems_Chiller_Simple;


        //public override Guid ComponentGuid => new Guid("bc45371e-6be4-49e3-9da4-996700c57cc1");
        public override Guid ComponentGuid => Guid.NewGuid();
    }
}
