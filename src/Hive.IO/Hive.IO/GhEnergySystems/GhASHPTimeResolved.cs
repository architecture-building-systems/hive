using System;
using Grasshopper.Kernel;
using System.Collections.Generic;
using System.Linq;

namespace Hive.IO.GhEnergySystems
{
    public class GhASHPTimeResolved : GH_Component
    {
        //Calulating time resolved COP of Air-source heat pump and the electricity demand, given heating load

        //Source: 10.1016/j.apenergy.2019.03.177  Eq. (A.7)

        //COP_{HP,t} = pi_{HP,1} * exp[pi_{HP,2} * (T_supply - T_{amb,t}) +pi_{HP,3} * exp[pi_{HP,4} * (T_supply - T_{amb,t})]
        //where T_supply is the supply temperature, T_{amb, t} is the ambient air temperature and pi_HP are parameters
        //depending on the type of the HP.

        //arguments:
        //    -pi_HP,1,2,3,4[-]
        //   - T_supply[°C]
        //   - T_amb,t[°C]
        //   - Q_th[kW]

        //output:
        //-x_el,t[kW]

        public GhASHPTimeResolved() :
            base("Air Source Heat Pump time resolved Energy System", "AshpTimeResolved",
                "Calculates the time resolved Coefficient of Performance (COP) of an air source heat pump, as well as consumed electricity of the heat pump, to meet heating energy demand.",
                "[hive]", "Energy Systems")
        {
        }

        public override GH_Exposure Exposure => GH_Exposure.secondary;

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("Q_th", "Q_th", "Heating load, time resolved [kW]", GH_ParamAccess.list);
            pManager.AddNumberParameter("T_supply", "T_supply", "Supply temperature of ASHP [°C]", GH_ParamAccess.item);
            pManager.AddNumberParameter("T_amb,t", "T_amb,t", "Ambient air temperature, time resolved [°C]", GH_ParamAccess.list);
            pManager.AddNumberParameter("PI_HP,1", "PI_1", "Technology parameter 1 for ASHP", GH_ParamAccess.item);
            pManager.AddNumberParameter("PI_HP,2", "PI_2", "Technology parameter 2 for ASHP", GH_ParamAccess.item);
            pManager.AddNumberParameter("PI_HP,3", "PI_3", "Technology parameter 3 for ASHP", GH_ParamAccess.item);
            pManager.AddNumberParameter("PI_HP,4", "PI_4", "Technology parameter 4 for ASHP", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("x_el", "x_el", "Electricity demand to fulfill heating load [kW]", GH_ParamAccess.list);
            pManager.AddNumberParameter("COP", "COP", "COP of ASHP [-]", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<double> Q_th = new List<double>();
            if (!DA.GetDataList(0, Q_th)) return;
            double T_supply = 0.0;
            if (!DA.GetData(1, ref T_supply)) return;
            List<double> T_amb = new List<double>();
            if (!DA.GetDataList(2, T_amb)) return;
            double pi_1 = 0.0;
            if (!DA.GetData(3, ref pi_1)) return;
            double pi_2 = 0.0;
            if (!DA.GetData(4, ref pi_2)) return;
            double pi_3 = 0.0;
            if (!DA.GetData(5, ref pi_3)) return;
            double pi_4 = 0.0;
            if (!DA.GetData(6, ref pi_4)) return;


            var kelvin = 273.15;
            var T_supply_kelvin = T_supply + kelvin;
            var horizon = T_amb.Count < Q_th.Count ? T_amb.Count : Q_th.Count;

            var x_el = new double[horizon];
            var COP = new double[horizon];

            foreach (var t in Enumerable.Range(0, horizon))
            {
                var T_amb_kelvin = T_amb[t] + kelvin;
                COP[t] = pi_1 * Math.Exp(pi_2 * (T_supply_kelvin - T_amb_kelvin)) + pi_3 * Math.Exp(pi_4 * (T_supply_kelvin - T_amb_kelvin));
                x_el[t] = Q_th[t] / COP[t];
            }

            DA.SetDataList(0, x_el);
            DA.SetDataList(1, COP);

        }

        protected override System.Drawing.Bitmap Icon => Properties.Resources.EnergySystems_Heatpump_AirScource_TimeResult;

        public override Guid ComponentGuid => new Guid("959a2b0b-c23d-4700-93af-b9301ce415b7");
    }
}
