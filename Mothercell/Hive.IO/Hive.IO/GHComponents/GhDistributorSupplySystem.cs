using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Hive.IO.EnergySystems;
using Rhino.Geometry;

namespace Hive.IO.GHComponents
{
    public class GhDistributorSupplySystem : GH_Component
    {

        public GhDistributorSupplySystem()
          : base("Hive.IO.DistributorSupplySystem", "HiveIODistrSupSys",
              "Distributor for the Energy Supply Systems models." +
                "\nOutputs relevant parameters for Energy Supply Systems calculations.",
              "[hive]", "Mothercell")
        {
        }


        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Hive.IO.EnergySystem.PV", "HiveIOEnSysPV", "Reads in Hive.IO.EnergySystem.PV objects.", GH_ParamAccess.list);
            pManager.AddGenericParameter("Hive.IO.EnergySystem.ST", "HiveIOEnSysST", "Reads in Hive.IO.EnergySystem.PVT objects.", GH_ParamAccess.list);
            pManager.AddGenericParameter("Hive.IO.EnergySystem.PVT", "HiveIOEnSysPVT", "Reads in Hive.IO.EnergySystem.ST objects.", GH_ParamAccess.list);

            for (int i=0; i<pManager.ParamCount; i++)
                pManager[i].Optional = true;
        }


        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            //0, 1
            pManager.AddNumberParameter("Efficiency PV", "eta_PV", "Efficiency [0, 1] of Photovoltaic (PV) systems.", GH_ParamAccess.list);    // could have different technologies
            pManager.AddNumberParameter("PV Areas", "PVAreas", "Surface areas of the PV panels.", GH_ParamAccess.list);

            // 2, 3
            pManager.AddNumberParameter("Efficiency ST", "eta_ST", "Efficiency [0, 1] of Solar Thermal (ST) systems.", GH_ParamAccess.list);
            pManager.AddNumberParameter("ST Areas", "STAreas", "Surface areas of the ST panels.", GH_ParamAccess.list);

            // 4, 5, 6
            pManager.AddNumberParameter("Electric Efficiency PVT", "eta_PVT_el", "Electric efficiency [0, 1] of hybrid PVT systems.", GH_ParamAccess.list);
            pManager.AddNumberParameter("Thermal Efficiency PVT", "eta_PVT_th", "Thermal efficiency [0, 1] of hybrid PVT systems.", GH_ParamAccess.list);
            pManager.AddNumberParameter("PVT Areas", "PVTAreas", "Surface areas of the PVT panels.", GH_ParamAccess.list);
        }


        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Photovoltaic> pv = new List<Photovoltaic>();
            List<PVT> pvt = new List<PVT>();
            List<SolarThermal> st = new List<SolarThermal>();

            DA.GetDataList(0, pv);
            DA.GetDataList(1, st);
            DA.GetDataList(2, pvt);

            List<double> etas_PV = new List<double>();
            List<double> etas_ST = new List<double>();
            List<double> etas_PVT_el = new List<double>();
            List<double> etas_PVT_therm = new List<double>();

            List<double> A_PV = new List<double>();
            List<double> A_ST = new List<double>();
            List<double> A_PVT = new List<double>();

            foreach(Photovoltaic _pv in pv)
            {
                etas_PV.Add(_pv.RefEfficiencyElectric);
                A_PV.Add(AreaMassProperties.Compute(_pv.SurfaceGeometry).Area);
            }

            foreach (SolarThermal _st in st)
            {
                etas_ST.Add(_st.RefEfficiencyHeating);
                A_ST.Add(AreaMassProperties.Compute(_st.SurfaceGeometry).Area);
            }

            foreach (PVT _pvt in pvt)
            {
                etas_PVT_el.Add(_pvt.RefEfficiencyElectric);
                etas_PVT_therm.Add(_pvt.RefEfficiencyHeating);
                A_PVT.Add(AreaMassProperties.Compute(_pvt.SurfaceGeometry).Area);
            }


            DA.SetDataList(0, etas_PV);
            DA.SetDataList(1, A_PV);
            DA.SetDataList(2, etas_ST);
            DA.SetDataList(3, A_ST);
            DA.SetDataList(4, etas_PVT_el);
            DA.SetDataList(5, etas_PVT_therm);
        }


        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return null;
            }
        }


        public override Guid ComponentGuid
        {
            get { return new Guid("fc6f580c-cefe-48ee-beaf-308566df19f3"); }
        }
    }
}