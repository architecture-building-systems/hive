using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Rhino.Geometry;
using Hive.IO.EnergySystems;

namespace Hive.IO.GHComponents
{
    public class GhDistributorSolar : GH_Component
    {
        public GhDistributorSolar()
          : base("Hive.IO.DistributorSolar", "HiveIODistrSolar",
              "Distributor for solar simulations. Reads in and outputs all relevant geometric, geographic and climatic information necessary for solar simulations.",
              "[hive]", "Mothercell")
        {
        }


        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Hive.IO.Building", "HiveIOBldg", "Reads in an Hive.IO.Building object.", GH_ParamAccess.item);
            pManager.AddGenericParameter("Hive.IO.Environment", "HiveIOEnv", "Reads in an Hive.IO.Environment object.", GH_ParamAccess.item);
            pManager.AddGenericParameter("Hive.IO.EnergySystem.PV", "HiveIOEnSysPV", "Reads in an Hive.IO.EnergySystem.PV object.", GH_ParamAccess.list);
            pManager.AddGenericParameter("Hive.IO.EnergySystem.PVT", "HiveIOEnSysPVT", "Reads in an Hive.IO.EnergySystem.PVT object.", GH_ParamAccess.list);
            pManager.AddGenericParameter("Hive.IO.EnergySystem.ST", "HiveIOEnSysST", "Reads in an Hive.IO.EnergySystem.ST object.", GH_ParamAccess.list);
        }


        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddMeshParameter("Environment Mesh", "EnvMesh", "Mesh geometries of the environment (adjacent buildings, trees, obstacles, etc.).", GH_ParamAccess.list);    // 0
            pManager.AddPointParameter("Building Centroid", "BldCentroid", "Centroid of the building bounding box.", GH_ParamAccess.item);               // 1
            pManager.AddBrepParameter("External Surfaces", "ExtSrfs", "External Surfaces of the building that have access to the outside (i.e. are not internal walls).", GH_ParamAccess.list);                         // 2
            pManager.AddMeshParameter("PV Mesh", "PVMesh", "Mesh geometries of the Photovoltaic (PV) objects.", GH_ParamAccess.list);                               // 3
            pManager.AddMeshParameter("PVT Mesh", "PVTMesh", "Mesh geometries of the hybrid PVT objects.", GH_ParamAccess.list);                            // 4
            pManager.AddMeshParameter("ST Mesh", "STMesh", "Mesh geometries of the Solar Thermal (ST) objects.", GH_ParamAccess.list);                               // 5

            pManager.AddBrepParameter("Window Geometries", "WinBreps", "All window Brep geometries of the building.", GH_ParamAccess.list);
        }


        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Building building = null;
            if (!DA.GetData(0, ref building)) return;

            Environment environment = null;
            if (!DA.GetData(1, ref environment)) return;

            List<Photovoltaic> pv = new List<Photovoltaic>();
            DA.GetDataList(2, pv);

            List<PVT> pvt = new List<PVT>();
            DA.GetDataList(3, pvt);

            List<SolarThermal> st = new List<SolarThermal>();
            DA.GetDataList(4, st);


            if (building != null) Rhino.RhinoApp.WriteLine("Building '{0}' read successfully", building.Type.ToString());
            Rhino.RhinoApp.WriteLine("Surface Energy Systems read in: \n PV: {0}; ST: {1}; PVT: {2}", pv.Count, st.Count, pvt.Count);


            int zoneCount = building.Zones.Length;
            List<Brep> extSrfs = new List<Brep>();
            List<Brep> windows = new List<Brep>();

            for (int i = 0; i < zoneCount; i++)
            {
                Zone zone = building.Zones[i];

                foreach (BuildingComponents.Wall wall in zone.Walls)
                {
                    // TO DO: check, if external. VERY IMPORTANT
                    extSrfs.Add(wall.BrepGeometry);
                }
                foreach (BuildingComponents.Roof roof in zone.Roofs)
                {
                    //TO DO: check if external. VERY IMPORTANT
                    extSrfs.Add(roof.BrepGeometry);
                }

                foreach(BuildingComponents.Opening opening in zone.Openings)
                {
                    windows.Add(opening.BrepGeometry);
                }
            }

            // get centroid of building. just use a floor for now. TO DO for Hive 0.2: Come up with better idea
            Point3d centroid = AreaMassProperties.Compute(building.Zones[0].Floors[0].BrepGeometry).Centroid;

            List<Mesh> environmentList = new List<Mesh>();
            if (environment != null)
            {
                foreach (Mesh msh in environment.Geometry)
                    environmentList.Add(msh);
            }


            List<Mesh> pvSurfaces = new List<Mesh>();
            List<Mesh> pvtSurfaces = new List<Mesh>();
            List<Mesh> stSurfaces = new List<Mesh>();
            foreach (Photovoltaic _pv in pv)
                pvSurfaces.Add(_pv.SurfaceGeometry);
            foreach (PVT _pvt in pvt)
                pvtSurfaces.Add(_pvt.SurfaceGeometry);
            foreach (SolarThermal _st in st)
                stSurfaces.Add(_st.SurfaceGeometry);



            DA.SetDataList(0, environmentList);
            DA.SetData(1, centroid);
            DA.SetDataList(2, extSrfs);

            DA.SetDataList(3, pvSurfaces);
            DA.SetDataList(4, pvtSurfaces);
            DA.SetDataList(5, stSurfaces);

            DA.SetDataList(6, windows);

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
            get { return new Guid("12d66e12-f837-44c9-9bad-de7e245134bb"); }
        }
    }
}