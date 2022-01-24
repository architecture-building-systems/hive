using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Hive.IO.EnergySystems;
using Rhino.Geometry;

namespace Hive.IO.GhDistributors
{
    public class GhDistSolarTech : GH_Component
    {

        public GhDistSolarTech()
          : base("Distributor SolarTech Hive", "HiveDistSolarTech",
              "Distributor for Solar technologies." +
                "\nOutputs relevant parameters for Solar Energy Generation calculations (PV, ST, PVT, GC).",
              "[hive]", "IO-Core")
        {
        }


        public override GH_Exposure Exposure => GH_Exposure.secondary;


        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Hive SurfaceBasedTech", "SurfaceBasedTech", "Reads in Hive SurfaceBasedTech (Photovoltaic, GroundCollector, SolarThermal, PVT).", GH_ParamAccess.list);

            for (int i=0; i<pManager.ParamCount; i++)
                pManager[i].Optional = true;
        }


        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            //0, 1
            pManager.AddNumberParameter("Efficiency Photovoltaic", "eta_PV", "Efficiency [0, 1] of Photovoltaic (PV) systems.", GH_ParamAccess.list);    // could have different technologies
            pManager.AddNumberParameter("Photovoltaic Areas", "PVAreas", "Surface areas of the PV panels.", GH_ParamAccess.list);

            // 2, 3
            pManager.AddNumberParameter("Efficiency Solar Thermal", "eta_ST", "Efficiency [0, 1] of Solar Thermal (ST) systems.", GH_ParamAccess.list);
            pManager.AddNumberParameter("Solar Thermal Areas", "STAreas", "Surface areas of the Solar Thermal (ST) panels.", GH_ParamAccess.list);

            // 4, 5, 6
            pManager.AddNumberParameter("Electric Efficiency hybrid PVT", "eta_PVT_el", "Electric efficiency [0, 1] of the hybrid photovoltaic and solar thermal (PVT) systems.", GH_ParamAccess.list);
            pManager.AddNumberParameter("Thermal Efficiency hybrid PVT", "eta_PVT_th", "Thermal efficiency [0, 1] of the hybrid photovoltaic and solar thermal (PVT) systems.", GH_ParamAccess.list);
            pManager.AddNumberParameter("Hybrid PVT Areas", "PVTAreas", "Surface areas of the hybrid photovoltaic and solar thermal (PVT) panels.", GH_ParamAccess.list);
        }


        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<SurfaceBasedTech> srfTech = new List<SurfaceBasedTech>();
            DA.GetDataList(0, srfTech);

            //List<Photovoltaic> pv = new List<Photovoltaic>();
            //List<PVT> pvt = new List<PVT>();
            //List<SolarThermal> st = new List<SolarThermal>();

            List<double> etas_PV = new List<double>();
            List<double> etas_BIPV = new List<double>();
            List<double> etas_ST = new List<double>();
            List<double> etas_PVT_el = new List<double>();
            List<double> etas_PVT_therm = new List<double>();

            List<double> A_PV = new List<double>();
            List<double> A_BIPV = new List<double>();
            List<double> A_ST = new List<double>();
            List<double> A_PVT = new List<double>();

            foreach (SurfaceBasedTech tech in srfTech)
            {
                switch (tech.ToString())
                {
                    case "Hive.IO.EnergySystems.Photovoltaic":
                        Photovoltaic _pv = (Photovoltaic)tech;
                        etas_PV.Add(_pv.RefEfficiencyElectric);
                        A_PV.Add(AreaMassProperties.Compute(_pv.SurfaceGeometry).Area);
                        break;
                    case "Hive.IO.EnergySystems.BuildingIntegratedPV":
                        BuildingIntegratedPV _bipv = (BuildingIntegratedPV)tech;
                        etas_BIPV.Add(_bipv.RefEfficiencyElectric);
                        A_BIPV.Add(AreaMassProperties.Compute(_bipv.SurfaceGeometry).Area);
                        break;
                    case "Hive.IO.EnergySystems.SolarThermal":
                        SolarThermal _st = (SolarThermal) tech;
                        etas_ST.Add(_st.RefEfficiencyHeating);
                        A_ST.Add(AreaMassProperties.Compute(_st.SurfaceGeometry).Area);
                        break;
                    case "Hive.IO.EnergySystems.PVT":
                        PVT _pvt = (PVT) tech;
                        etas_PVT_el.Add(_pvt.RefEfficiencyElectric);
                        etas_PVT_therm.Add(_pvt.RefEfficiencyHeating);
                        A_PVT.Add(AreaMassProperties.Compute(_pvt.SurfaceGeometry).Area);
                        break;
                    case "Hive.IO.EnergySystems.GroundCollector":

                        break;
                }
            }


            DA.SetDataList(0, etas_PV);
            DA.SetDataList(1, A_PV);
            DA.SetDataList(2, etas_BIPV);
            DA.SetDataList(3, A_BIPV);
            DA.SetDataList(4, etas_ST);
            DA.SetDataList(5, A_ST);
            DA.SetDataList(6, etas_PVT_el);
            DA.SetDataList(7, etas_PVT_therm);
        }


        protected override System.Drawing.Bitmap Icon => Properties.Resources.IOCore_Distsolartech;


        public override Guid ComponentGuid => new Guid("fc6f580c-cefe-48ee-beaf-308566df19f3");
    }
}