using System;
using Grasshopper.Kernel;

namespace Hive.IO.GhEnergySystems
{
    public class GhHeatpumpCOP : GH_Component
    {
        //Simple COP calculation for heat pumps, from lectures Energy and Climate Systems FS 2019

        //COP_W = eta(T_1 / (T_1 - T_2))

        //arguments:
        //    - eta : efficiency HP
        //    - T_1 : temperature warm reservoir[K]
        //    - T_2 : temperature cold reservoir[K]

        //output:
        //    - COP_W : coefficient of performance[-]

        public GhHeatpumpCOP() :
            base("Heat pump COP Energy System", "HpCopSimple",
                "Calculates the Coefficient of Performance (COP) of a simple heat pump.",
                "[hive]", "Energy Systems")
        {
        }

        public override GH_Exposure Exposure => GH_Exposure.tertiary;

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("eta", "eta", "Efficiency of heat pump [-]", GH_ParamAccess.item);
            pManager.AddNumberParameter("T_1", "T_1", "Temperature of warm reservoir [k]", GH_ParamAccess.item);
            pManager.AddNumberParameter("T_2", "T_2", "Temperature of cold reservoir [k]", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("COP_W", "COP_W", "COP of heat pump [-]", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            double eta = 0.0;
            if (!DA.GetData(0, ref eta)) return;
            double T_1 = 0.0;
            if (!DA.GetData(1, ref T_1)) return;
            double T_2 = 0.0;
            if (!DA.GetData(2, ref T_2)) return;

            var kelvin = 273.15;
            T_1 += kelvin;
            T_2 += kelvin;

            var COP_W = eta * (T_1 / (T_1 - T_2));

            DA.SetData(0, COP_W);
        }

        protected override System.Drawing.Bitmap Icon => Properties.Resources.EnergySystems_Heatpump_COP;

        public override Guid ComponentGuid => new Guid("844f55ec-acf3-4f26-bc4a-dda72ae6a5ea");

    }
}