using System;
using Grasshopper.Kernel;

namespace Hive.IO.GhEnergySystems
{
    public class GhASHPSimple : GH_Component
    {

        //Simple heat pump calculation according to Energy and Climate Systems lecture FS 2019
        //E = Q / COP

        //input:
        //Q = heating loads[kWh]
        //COP = coefficient of performance[-]

        //output:
        //E = electricity loads[kWh]
        public GhASHPSimple()
            : base("Heat pump Energy System", "HeatPumpSimple",
              "Calculates total consumed electricity of a simple heat pump, given a Coefficient of Performance, to meet heating energy demand.",
              "[hive]", "Energy Systems"
            )
        {
        }

        public override GH_Exposure Exposure => GH_Exposure.secondary;

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("Q_th", "Q_th", "Heating loads [kWh]", GH_ParamAccess.item);
            pManager.AddNumberParameter("COP", "COP", "Coefficient of performance of COP [-]", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("E", "E", "Electrity loads required to operate heat pump, in [kWh]", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            double Q = 0.0;
            if (!DA.GetData(0, ref Q)) return;
            double COP = 0.0;
            if (!DA.GetData(0, ref COP)) return;

            var E = Q / COP;

            DA.SetData(0, E);
        }

        protected override System.Drawing.Bitmap Icon => Properties.Resources.EnergySystems_Heatpump_AirSource;

        public override Guid ComponentGuid => new Guid("1a1f54f6-9955-4168-9c4a-da5d669022f2");
    }
}
