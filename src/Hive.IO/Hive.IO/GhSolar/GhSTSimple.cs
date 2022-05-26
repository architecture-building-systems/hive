using System;
using Grasshopper.Kernel;

namespace Hive.IO.GhSolar
{
    public class GhSTSimple : GH_Component
    {
        public GhSTSimple() 
            : base("Solar Thermal Energy System", "SolarThermalSimple",
                  "Calculates the total yield of a solar thermal collector using simplified equation (Q_th = G * F_F * A * eta_K * R_V)",
                  "[hive]", "Solar")
        {
        }

        public override GH_Exposure Exposure => GH_Exposure.secondary;

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("Global_Irradiation", "G", "Global horizontal irradiation, in [kWh/m²]", GH_ParamAccess.item);
            pManager.AddNumberParameter("FormFactor", "F_f", "Form factor, also known as orientation factor, that defines how much of the global annual irradiation is effectively used, depending on the angle of the Solar thermal collector. Fractional value [0.0, >1.0]. (Default: 1.0)", GH_ParamAccess.item, 1.0);
            pManager.AddNumberParameter("SurfaceArea", "A", "Solar thermal collector surface are in [m²]. (Default: 1.0)", GH_ParamAccess.item, 1.0);
            pManager.AddNumberParameter("STEfficiency", "eta_K", "Thermal efficiency of collector, fractional value [0.0, 1.0]. (Default: 0.15)", GH_ParamAccess.item, 0.15);
            pManager.AddNumberParameter("DistributionLosses", "R_V", "Distribution losses coefficient. Fractional [0.0, 1.0]. (Default: 0.75)", GH_ParamAccess.item, 0.75);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("ThermalYield", "yield", "Total thermal energy generation [kWh].", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            double G = 0.0;
            if (!DA.GetData(0, ref G)) return;
            double F_f = 0.0;
            if (!DA.GetData(1, ref F_f)) return;
            double A = 0.0;
            if (!DA.GetData(2, ref A)) return;
            double eta_K = 0.0;
            if (!DA.GetData(3, ref eta_K)) return;
            double R_V = 0.0;
            if (!DA.GetData(4, ref R_V)) return;


            var Q_th = G * F_f * A * eta_K * R_V;

            DA.SetData(0, Q_th);
        }

        protected override System.Drawing.Bitmap Icon => Properties.Resources.Solar_ST_Simple;

        public override Guid ComponentGuid => new Guid("841d16df-763a-475f-91c1-41398e583d7d");

    }
}
