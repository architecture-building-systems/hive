
using System;
using Grasshopper.Kernel;

namespace Hive.IO.GhEnergySystems
{
    public class GhBoilerSimple : GH_Component
    {
        /// <summary>
        // could be gas, oil, bio boiler
        // because its just efficiency times fuel [kWh]
        // 
        // arguments:
        //     - cost of carrier [CHF/kWh]
        //     - emmissions of carrier [kgCO2/kWheq.]
        //     - efficiency [-]
        //     - heating loads [kWh]
        // 
        // should return
        //     - cost [CHF]
        //     - carbon emissions [kgCO2eq.]        
        /// </summary>
        public GhBoilerSimple()
          : base("Boiler Energy System", "BoilerSimple",
              "Calculates total operating cost and carbon emissions of a boiler to meet heating loads.",
              "[hive]", "Energy Systems")
        {
        }

        public override GH_Exposure Exposure => GH_Exposure.secondary;


        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("heating_loads", "htg", "Heating loads [kWh]", GH_ParamAccess.item);
            pManager.AddNumberParameter("carrier_cost", "cost", "Cost of energy carrier [CHF/kWh]", GH_ParamAccess.item);
            pManager.AddNumberParameter("carrier_emissions", "CO2", "Carbon emissions of energy carrier [kgCO2/kWheq.]", GH_ParamAccess.item);
            pManager.AddNumberParameter("efficiency", "eta", "Efficiency of boiler [-]", GH_ParamAccess.item);
        }


        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("gas_consumed", "gas", "Total gas consumed [kWh eq.]", GH_ParamAccess.item);
            pManager.AddNumberParameter("cost", "cost", "Total operation cost [CHF]", GH_ParamAccess.item);
            pManager.AddNumberParameter("carbon", "carbon", "Total operation carbon emissions [kgCO2eq.]", GH_ParamAccess.item);
        }


        protected override void SolveInstance(IGH_DataAccess DA)
        {
            double heating_loads = 0.0;
			if (!DA.GetData(0, ref heating_loads)) return;
            double carrier_cost = 0.0;
			if (!DA.GetData(1, ref carrier_cost)) return;
            double carrier_emissions = 0.0;
			if (!DA.GetData(2, ref carrier_emissions)) return;
            double efficiency = 0.0;
			if (!DA.GetData(3, ref efficiency)) return;
            
            var gas_consumed = heating_loads * efficiency;
            var total_cost = gas_consumed * carrier_cost;
            var total_emissions = gas_consumed * carrier_emissions;

            DA.SetData(0, gas_consumed);
            DA.SetData(1, total_cost);
            DA.SetData(2, total_emissions);
        }


        protected override System.Drawing.Bitmap Icon => Properties.Resources.EnergySystems_Boiler_Simple;


        public override Guid ComponentGuid => new Guid("1d47ce6e-44ed-4ac6-9890-93dc91fb8529"); 
       
    }
}
