using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Hive.IO
{
    public class GHDistributorSolar : GH_Component
    {
        public GHDistributorSolar()
          : base("DistributorSolar", "DistributorSolar",
              "DistributorSolar",
              "[hive]", "Mothercell")
        {
        }


        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("BuildingObj", "BuildingObj", "BuildingObj", GH_ParamAccess.item);
            pManager.AddGenericParameter("EnvironmentObj", "EnvironmentObj", "EnvironmentObj", GH_ParamAccess.item);
            pManager.AddGenericParameter("PVObj", "PVObj", "PVObj", GH_ParamAccess.list);
            pManager.AddGenericParameter("PVTObj", "PVTObj", "PVTObj", GH_ParamAccess.list);
            pManager.AddGenericParameter("STObj", "STObj", "STObj", GH_ParamAccess.list);
        }


        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddMeshParameter("EnvironmentMesh", "EnvironmentMesh", "EnvironmentMesh", GH_ParamAccess.list);    // 0
            pManager.AddPointParameter("BldCentroid", "BldCentroid", "BldCentroid", GH_ParamAccess.item);               // 1
            pManager.AddSurfaceParameter("ExtSrfs", "ExtSrfs", "ExtSrfs", GH_ParamAccess.list);                         // 2
            pManager.AddMeshParameter("PVMesh", "PVMesh", "PVMesh", GH_ParamAccess.list);                               // 3
            pManager.AddMeshParameter("PVTMesh", "PVTMesh", "PVTMesh", GH_ParamAccess.list);                            // 4
            pManager.AddMeshParameter("STMesh", "StMesh", "STMesh", GH_ParamAccess.list);                               // 5
        }


        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Building building = null;
            if (!DA.GetData(0, ref building)) return;

            Environment environment = null;
            if (!DA.GetData(1, ref environment)) return;

            List<EnergySystem.PV> pv = new List<EnergySystem.PV>();
            DA.GetDataList(2, pv);

            List<EnergySystem.PVT> pvt = new List<EnergySystem.PVT>();
            DA.GetDataList(3, pvt);

            List<EnergySystem.ST> st = new List<EnergySystem.ST>();
            DA.GetDataList(4, st);


            if (building != null) Rhino.RhinoApp.WriteLine("Building '{0}' read successfully", building.Type.ToString());
            Rhino.RhinoApp.WriteLine("Surface Energy Systems read in: \n PV: {0}; ST: {1}; PVT: {2}", pv.Count, st.Count, pvt.Count);


            int zoneCount = building.Zones.Length;
            List<Surface> extSrfs = new List<Surface>();

            for (int i = 0; i < zoneCount; i++)
            {
                Zone zone = building.Zones[i];

                foreach (BuildingComponents.Wall wall in zone.Walls)
                {
                    // TO DO: check, if external. VERY IMPORTANT
                    extSrfs.Add(wall.SurfaceGeometry);
                }
                foreach (BuildingComponents.Roof roof in zone.Roofs)
                {
                    //TO DO: check if external. VERY IMPORTANT
                    extSrfs.Add(roof.SurfaceGeometry);
                }
            }

            // get centroid of building. just use a floor for now. TO DO for Hive 0.2: Come up with better idea
            Point3d centroid = AreaMassProperties.Compute(building.Zones[0].Floors[0].SurfaceGeometry).Centroid;

            List<Mesh> environmentList = new List<Mesh>();
            if (environment != null)
            {
                foreach (Mesh msh in environment.Geometry)
                    environmentList.Add(msh);
            }


            List<Mesh> pvSurfaces = new List<Mesh>();
            List<Mesh> pvtSurfaces = new List<Mesh>();
            List<Mesh> stSurfaces = new List<Mesh>();
            foreach (EnergySystem.PV _pv in pv)
                pvSurfaces.Add(_pv.SurfaceGeometry);
            foreach (EnergySystem.PVT _pvt in pvt)
                pvtSurfaces.Add(_pvt.SurfaceGeometry);
            foreach (EnergySystem.ST _st in st)
                stSurfaces.Add(_st.SurfaceGeometry);



            DA.SetDataList(0, environmentList);
            DA.SetData(1, centroid);
            DA.SetDataList(2, extSrfs);

            DA.SetDataList(3, pvSurfaces);
            DA.SetDataList(4, pvtSurfaces);
            DA.SetDataList(5, stSurfaces);

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