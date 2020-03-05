using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Hive.IO
{
    public class GHDistributorSupplySystem : GH_Component
    {

        public GHDistributorSupplySystem()
          : base("DistributorSupplySystem", "DistributorSupplySystem",
              "DistributorSupplySystem",
              "[hive]", "Mothercell")
        {
        }


        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("PVObj", "PVObj", "PVObj", GH_ParamAccess.list);
            pManager.AddGenericParameter("STObj", "STObj", "STObj", GH_ParamAccess.list);
            pManager.AddGenericParameter("PVTObj", "PVTObj", "PVTObj", GH_ParamAccess.list);

            for (int i=0; i<pManager.ParamCount; i++)
                pManager[i].Optional = true;
        }


        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            //0, 1
            pManager.AddNumberParameter("eta_PV", "eta_PV", "eta_PV", GH_ParamAccess.list);    // could have different technologies
            pManager.AddNumberParameter("PVAreas", "PVAreas", "PVAreas", GH_ParamAccess.list);

            // 2, 3
            pManager.AddNumberParameter("eta_ST", "eta_ST", "eta_ST", GH_ParamAccess.list);
            pManager.AddNumberParameter("STAreas", "STAreas", "STAreas", GH_ParamAccess.list);

            // 4, 5, 6
            pManager.AddNumberParameter("eta_PVT_el", "eta_PVT_el", "eta_PVT_el", GH_ParamAccess.list);
            pManager.AddNumberParameter("eta_PVT_th", "eta_PVT_th", "eta_PVT_th", GH_ParamAccess.list);
            pManager.AddNumberParameter("PVTAreas", "PVTAreas", "PVTAreas", GH_ParamAccess.list);
        }


        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<EnergySystem.PV> pv = new List<EnergySystem.PV>();
            List<EnergySystem.PVT> pvt = new List<EnergySystem.PVT>();
            List<EnergySystem.ST> st = new List<EnergySystem.ST>();

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

            foreach(EnergySystem.PV _pv in pv)
            {
                etas_PV.Add(_pv.RefEfficiencyElectric);
                A_PV.Add(AreaMassProperties.Compute(_pv.SurfaceGeometry).Area);
            }

            foreach (EnergySystem.ST _st in st)
            {
                etas_ST.Add(_st.RefEfficiencyThermal);
                A_ST.Add(AreaMassProperties.Compute(_st.SurfaceGeometry).Area);
            }

            foreach (EnergySystem.PVT _pvt in pvt)
            {
                etas_PVT_el.Add(_pvt.RefEfficiencyElectric);
                etas_PVT_therm.Add(_pvt.RefEfficiencyThermal);
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