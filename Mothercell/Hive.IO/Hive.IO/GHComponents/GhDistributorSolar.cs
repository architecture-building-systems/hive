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
          : base("Hive.IO.DistributorSolar", "DistrSolar",
              "Distributor for 'GHSolar' solar simulations (github/christophwaibel/GH_Solar_V2). Reads in and outputs all relevant geometric, geographic and climatic information necessary for solar simulations.",
              "[hive]", "Mothercell")
        {
        }


        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Hive.IO.Building", "HiveIOBldg", "Reads in an Hive.IO.Building object.", GH_ParamAccess.item);
            pManager.AddGenericParameter("Hive.IO.EnergySystem.SurfaceBased", "HiveIOEnSysSrf", "Reads in Hive.IO.EnergySystem .Photovoltaic; .SolarThermal; .PVT; .GroundCollector objects.", GH_ParamAccess.list);
            pManager.AddGenericParameter("Hive.IO.Environment", "HiveIOEnv", "Reads in an Hive.IO.Environment object.", GH_ParamAccess.item);
        }


        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddMeshParameter("Environment Mesh", "EnvMesh", "Mesh geometries of the environment (adjacent buildings, trees, obstacles, etc.).", GH_ParamAccess.list);    // 0
            pManager.AddPointParameter("Building Centroid", "BldCentroid", "Centroid of the building bounding box.", GH_ParamAccess.item);               // 1
            pManager.AddBrepParameter("External Surfaces", "ExtSrfs", "External Surfaces of the building that have access to the outside (i.e. are not internal walls).", GH_ParamAccess.list);                         // 2
            pManager.AddBrepParameter("Window Geometries", "WinBreps", "All window Brep geometries of the building.", GH_ParamAccess.list); // 3

            pManager.AddMeshParameter("Solar Tech Mesh", "SolarMesh", "Mesh geometries for Hive.IO.EnergySystems.SurfaceBased technologies (Photovoltaic, PVT, Solar thermal, ground collectors).", GH_ParamAccess.list);                               // 4
        }


        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Building building = null;
            if (!DA.GetData(0, ref building)) return;

            List<SurfaceBased> srfBasedTech = new List<SurfaceBased>();
            DA.GetDataList(1, srfBasedTech);

            Environment environment = null;
            if (!DA.GetData(2, ref environment)) return;


            // if (building != null) Rhino.RhinoApp.WriteLine("Building '{0}' read successfully", building.Type.ToString());
            //Rhino.RhinoApp.WriteLine("Surface Based energy systems read in: \n {0}", srfBasedTech.Count);


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

            List<Mesh> srfBasedTechSurfaces = new List<Mesh>();
          
            foreach (SurfaceBased srfTech in srfBasedTech)
                srfBasedTechSurfaces.Add(srfTech.SurfaceGeometry);


            DA.SetDataList(0, environmentList);
            DA.SetData(1, centroid);
            DA.SetDataList(2, extSrfs);
            DA.SetDataList(3, windows);

            DA.SetDataList(4, srfBasedTechSurfaces);
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