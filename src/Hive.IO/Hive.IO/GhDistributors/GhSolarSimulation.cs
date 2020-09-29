using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Rhino.Geometry;
using Hive.IO.EnergySystems;
using Hive.IO.Building;

namespace Hive.IO.GhDistributors
{
    public class GhSolarSimulation : GH_Component
    {
        public GhSolarSimulation()
          : base("Distributor Solar Simulation Hive", "HiveDistSolarSimu",
              "Distributor for 'GHSolar' solar simulations (github/christophwaibel/GH_Solar_V2). Reads in and outputs all relevant geometric, geographic and climatic information necessary for solar simulations.",
              "[hive]", "IO")
        {
        }


        public override GH_Exposure Exposure => GH_Exposure.quarternary;

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Hive.IO.Building", "HiveIOBldg", "Reads in an Hive.IO.Building object.", GH_ParamAccess.item);
            pManager.AddGenericParameter("Hive.IO.EnergySystem.SurfaceBased", "HiveIOEnSysSrf", "Reads in Hive.IO.EnergySystem .Photovoltaic; .SolarThermal; .PVT; .GroundCollector objects.", GH_ParamAccess.list);
            pManager.AddGenericParameter("Hive.IO.Environment", "HiveIOEnv", "Reads in an Hive.IO.Environment object.", GH_ParamAccess.item);
        }


        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddMeshParameter("Environment Mesh", "EnvMesh", "Mesh geometries of the environment (adjacent buildings, trees, obstacles, etc.).", GH_ParamAccess.list); // 0
            pManager.AddPointParameter("Building Centroid", "BldCentroid", "Centroid of the building bounding box.", GH_ParamAccess.item); // 1
            pManager.AddBrepParameter("External Surfaces", "ExtSrfs", "External Surfaces of the building that have access to the outside (i.e. are not internal walls).", GH_ParamAccess.list); // 2
            pManager.AddBrepParameter("Window Geometries", "WinBreps", "All window Brep geometries of the building.", GH_ParamAccess.list); // 3
            pManager.AddNumberParameter("Window Tilt", "WinTilt", "Tilt angles of windows, in deg", GH_ParamAccess.list);
            pManager.AddNumberParameter("Window Azimuth", "WinAzi", "Azimuth angles of windows, in deg", GH_ParamAccess.list);
            pManager.AddNumberParameter("Window Areas", "WinArea", "Window areas, in sqm", GH_ParamAccess.list);

            pManager.AddMeshParameter("Solar Tech Mesh", "SolarMesh", "Mesh geometries for Hive.IO.EnergySystems.SurfaceBased technologies (Photovoltaic, PVT, Solar thermal, ground collectors).", GH_ParamAccess.list); // 7
            pManager.AddNumberParameter("Solar Tech Tilt", "SolarTechTilt", "Tilt angles of solar tech surfaces, in deg", GH_ParamAccess.list);
            pManager.AddNumberParameter("Solar Tech Azimuth", "SolarTechAzimuth", "Azimuth angles of solar tech surfaces, in deg", GH_ParamAccess.list);
            pManager.AddNumberParameter("Solar Tech Area", "SolarTechArea", "Area of solar tech surfaces, in sqm", GH_ParamAccess.list);
        }


        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Building.Building building = null;
            if (!DA.GetData(0, ref building)) return;

            List<SurfaceBasedTech> srfBasedTech = new List<SurfaceBasedTech>();
            DA.GetDataList(1, srfBasedTech);

            Environment.Environment environment = null;
            if (!DA.GetData(2, ref environment)) return;


            // if (building != null) Rhino.RhinoApp.WriteLine("Building '{0}' read successfully", building.Type.ToString());
            //Rhino.RhinoApp.WriteLine("Surface Based energy systems read in: \n {0}", srfBasedTech.Count);


            int zoneCount = building.Zones.Length;
            var extSrfs = new List<Brep>();
            var windows = new List<Brep>();
            var windowAreas = new List<double>();
            var windowTilts = new List<double>();
            var windowAzimuths = new List<double>();

            for (int i = 0; i < zoneCount; i++)
            {
                Zone zone = building.Zones[i];

                foreach (Wall wall in zone.Walls)
                {
                    // TO DO: check, if external. VERY IMPORTANT
                    extSrfs.Add(wall.BrepGeometry);
                }
                foreach (Roof roof in zone.Roofs)
                {
                    //TO DO: check if external. VERY IMPORTANT
                    extSrfs.Add(roof.BrepGeometry);
                }

                foreach(Window opening in zone.Windows)
                {
                    windows.Add(opening.BrepGeometry);
                    double[] winProp = Misc.ComputeTiltAzimuthArea(opening.BrepGeometry);
                    windowTilts.Add(winProp[0]);
                    windowAzimuths.Add(winProp[1]);
                    windowAreas.Add(winProp[2]);
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

            var solarTechSurfaces = new List<Mesh>();
            var solarTechTilts = new List<double>();
            var solarTechAzimuths = new List<double>();
            var solarTechAreas = new List<double>();

            foreach (SurfaceBasedTech srfTech in srfBasedTech)
            {
                solarTechSurfaces.Add(srfTech.SurfaceGeometry);
                double [] solarTechProp = Misc.ComputeTiltAzimuth(srfTech.SurfaceGeometry);
                solarTechTilts.Add(solarTechProp[0]);
                solarTechAzimuths.Add(solarTechProp[1]);
                solarTechAreas.Add(srfTech.SurfaceArea);
            }

            DA.SetDataList(0, environmentList);
            DA.SetData(1, centroid);
            DA.SetDataList(2, extSrfs);
            DA.SetDataList(3, windows);
            DA.SetDataList(4, windowTilts);
            DA.SetDataList(5, windowAzimuths);
            DA.SetDataList(6, windowAreas);

            DA.SetDataList(7, solarTechSurfaces);
            DA.SetDataList(8, solarTechTilts);
            DA.SetDataList(9, solarTechAzimuths);
            DA.SetDataList(10, solarTechAreas);
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