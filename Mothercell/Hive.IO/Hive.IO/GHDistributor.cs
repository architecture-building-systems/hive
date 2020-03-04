using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;

namespace Hive.IO
{
    public class GHDistributor : GH_Component
    {
        public GHDistributor()
          : base("Distributor", "Distributor",
              "Distributor",
              "[hive]", "Mothercell")
        {
        }

        /// <summary>
        /// Takes ALL Hive Input objects (e.g. Hive.IO.PV, Hive.IO.Building, etc.)
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Hive Input Objects", "Hive Input Objects", "Hive Input Objects, all comes in here", GH_ParamAccess.list);
        }

        /// <summary>
        /// Output data that needs to be distributed within the mothercell to each respective simulation/calculation component
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            // 0, 1, 2, 3
            pManager.AddGenericParameter("IO.Building", "IO.Building", "IO.Building", GH_ParamAccess.item);
            pManager.AddGenericParameter("IO.EnergySystem.PV", "IO.EnergySystem.PV", "IO.EnergySystem.PV", GH_ParamAccess.list);
            pManager.AddGenericParameter("IO.EnergySystem.ST", "IO.EnergySystem.ST", "IO.EnergySystem.ST", GH_ParamAccess.list);
            pManager.AddGenericParameter("IO.EnergySystem.PVT", "IO.EnergySystem.PVT", "IO.EnergySystem.PVT", GH_ParamAccess.list);

            // 4, 5
            pManager.AddNumberParameter("ZonesAreas", "ZoneAreas", "ZoneAreas", GH_ParamAccess.list);
            pManager.AddTextParameter("SIA2024json", "SIA2024json", "SIA2024jsoN", GH_ParamAccess.item);

            // 6, 7
            pManager.AddNumberParameter("WindowAreas", "WindowAreas", "WindowAreas", GH_ParamAccess.list);
            pManager.AddNumberParameter("ExternalSrfsAreas", "ExternalSrfsAreas", "ExternalSrfsAreas", GH_ParamAccess.list);
            
            // 8, 9, 10
            pManager.AddPointParameter("BuildingCentroid", "BuildingCentroid", "BuildingCentroid", GH_ParamAccess.item);
            pManager.AddMeshParameter("EnvironmentMesh", "EnvironmentMesh", "EnvironmentMesh", GH_ParamAccess.list);
            pManager.AddTextParameter("epwPath", "epwPath", "epwPath", GH_ParamAccess.item);
        }

        /// <summary>
        /// Manages all the incoming Hive.IO objects, and splits it into required output data
        /// </summary>
        /// <param name="DA"></param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<GH_ObjectWrapper> input_objects = new List<GH_ObjectWrapper>();
            if (!DA.GetDataList(0, input_objects)) return;
            
            List<EnergySystem.PV> pv = new List<EnergySystem.PV>();
            List<EnergySystem.PVT> pvt = new List<EnergySystem.PVT>();
            List<EnergySystem.ST> st = new List<EnergySystem.ST>();
            Building building = null;
            Environment environment = null;

            foreach (GH_ObjectWrapper hive_input in input_objects)
            {
                switch (hive_input.Value.ToString())
                {
                    case "Hive.IO.EnergySystem.PV":
                        pv.Add(hive_input.Value as EnergySystem.PV);
                        break;
                    case "Hive.IO.EnergySystem.ST":
                        st.Add(hive_input.Value as EnergySystem.ST);
                        break;
                    case "Hive.IO.EnergySystem.PVT":
                        pvt.Add(hive_input.Value as EnergySystem.PVT);
                        break;
                    case "Hive.IO.Building":
                        building = hive_input.Value as Building;
                        break;
                    case "Hive.IO.Environment":
                        environment = hive_input.Value as Environment;
                        break;
                }
            }

            if (building != null) Rhino.RhinoApp.WriteLine("Building '{0}' read successfully", building.Type.ToString());
            Rhino.RhinoApp.WriteLine("Surface Energy Systems read in: \n PV: {0}; ST: {1}; PVT: {2}", pv.Count, st.Count, pvt.Count);


            int zoneCount = building.Zones.Length;
            double[] zoneAreas = new double[zoneCount];
            double[] windowAreas = new double[zoneCount];
            double[] extSrfAreas = new double[zoneCount];
            for (int i = 0; i < zoneCount; i++)
            {
                Zone zone = building.Zones[i];

                zoneAreas[i] = 0.0;
                foreach (BuildingComponents.Floor floor in zone.Floors)
                {
                    // TO DO: make check that it's not a void
                    zoneAreas[i] += floor.Area;
                }

                windowAreas[i] = 0.0;
                foreach(BuildingComponents.Opening opening in zone.Openings)
                {
                    windowAreas[i] += opening.Area;
                }

                extSrfAreas[i] = 0.0;
                foreach(BuildingComponents.Wall wall in zone.Walls)
                {
                    // TO DO: for Hive 0.2
                    //if (wall.IsExternal)
                    //{

                    //}
                    extSrfAreas[i] += wall.Area;
                }
                foreach(BuildingComponents.Roof roof in zone.Roofs)
                    extSrfAreas[i] += roof.Area;

                //// Ceiling is always internal? 
                //foreach (BuildingComponents.Ceiling ceiling in zone.Ceilings)
                //    extSrfAreas[i] += ceiling.Area;

                //// TO DO: only if the surface below is air, like an overhanging floor / cantilever. work with IxExternal
                //foreach (BuildingComponents.Floor floor in zone.Floors)
                //    extSrfAreas[i] += floor.Area;
            }


            // serialize sia2024 dictionary back into a json. can we avoid this double work? (Deserialized in GHBuilding, now serialized again)
            var json = building.SIA2024;
            JavaScriptSerializer js = new JavaScriptSerializer();


            // get centroid of building. just use a floor for now. TO DO for Hive 0.2: Come up with better idea
            Point3d centroid = AreaMassProperties.Compute(building.Zones[0].Floors[0].SurfaceGeometry).Centroid;

            List<Mesh> environmentList = new List<Mesh>();
            string epwPath = null;
            if (environment != null) 
            { 
                foreach (Mesh msh in environment.Geometry)
                    environmentList.Add(msh);
                epwPath = environment.EpwPath;
            }
            
            DA.SetData(0, building);
            DA.SetDataList(1, pv);
            DA.SetDataList(2, st);
            DA.SetDataList(3, pvt);

            DA.SetDataList(4, zoneAreas);
            DA.SetData(5, (string)js.Serialize(json));

            DA.SetDataList(6, windowAreas);
            DA.SetDataList(7, extSrfAreas);
            DA.SetData(8, centroid);
            DA.SetDataList(9, environmentList);
            DA.SetData(10, epwPath);
        
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
            get { return new Guid("8757ee6f-03c4-4f5e-ac6d-db04b4d20297"); }
        }
    }
}