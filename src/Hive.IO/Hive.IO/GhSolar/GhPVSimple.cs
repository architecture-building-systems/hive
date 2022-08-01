using System;
using Grasshopper.Kernel;

namespace Hive.IO.GhSolar
{
    public class GhPVSimple : GH_Component
    {
        //Calculates total solar electricity yield using simple equation from Energy and Climate Systems course FS2019:
        //E_PV = G* F_F * A * eta_PV * PR

        //Inputs:
        //- G - Global horizontal irradiation[kWh / m2]
        //- F_F - Form factor, also: Orientation factor[0.0, > 1.0]
        //- A - PV area[m2]
        //- eta_PV - PV module efficiency[0.0, 1.0]
        //- PR - Performance ratio[0.0, 1.0]

        //outputs:
        //- E_PV - PV electricity generation[kWh]

        public GhPVSimple() :
            base("Photovoltaic Energy System", "PvSimple",
                "Calculates total photovoltaic electricity generation using simplified a equation (E_PV = G * F_F * A * eta_PV * PR)", 
                "[hive]", "Solar")
        {
        }

        public override GH_Exposure Exposure => GH_Exposure.secondary;

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("Global_Irradiation", "G", "Global horizontal irradiation, in [kWh/m²]", GH_ParamAccess.item);
            pManager.AddNumberParameter("FormFactor", "F_f", "Form factor, also known as orientation factor, that defines how much of the global annual irradiation is effectively used, depending on the angle of the PV surface. Fractional value [0.0, >1.0]. (Default: 1.0)", GH_ParamAccess.item, 1.0);
            pManager.AddNumberParameter("SurfaceArea", "A", "PV surface are in [m²]. (Default: 1.0)", GH_ParamAccess.item, 1.0);
            pManager.AddNumberParameter("PVEfficiency", "eta_PV", "PV technology efficiency, fractional value [0.0, 1.0]. (Default: 0.15)", GH_ParamAccess.item, 0.15);
            pManager.AddNumberParameter("PerformanceRatio", "PR", "Performance Ratio, depends on how well the PV is exposed (shaded, not) and how clean it is (dirty, dusty, super clean). Fractional [0.0, 1.0]. (Default: 0.75)", GH_ParamAccess.item, 0.75);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("AnnualYield", "annual", "Total solar electricity generation [kWh]", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            double G = 0.0;
            if (!DA.GetData(0, ref G)) return;
            double F_f = 0.0;
            if (!DA.GetData(1, ref F_f)) return;
            double A = 0.0;
            if (!DA.GetData(2, ref A)) return;
            double eta_PV = 0.0;
            if (!DA.GetData(3, ref eta_PV)) return;
            double PR = 0.0;
            if (!DA.GetData(4, ref PR)) return;


            var E_PV = G * F_f * A * eta_PV * PR;

            DA.SetData(0, E_PV);
        }

        protected override System.Drawing.Bitmap Icon => Properties.Resources.Solar_PV_Simple;

        public override Guid ComponentGuid => new Guid("5c9d60fe-755c-420a-81d9-e15e786e48b5");
    }
}
