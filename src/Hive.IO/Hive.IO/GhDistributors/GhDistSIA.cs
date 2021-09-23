using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Hive.IO.Building;

namespace Hive.IO.GhDistributors
{
    public class GhDistSIA : GH_Component
    {
        public GhDistSIA()
          : base("Distributor SIARoom Hive", "HiveDistSIARoom",
              "Sia (Schweizerischer Ingenieur- und Architektenverein) 2024 distributor that reads in a Hive Building of type <Hive.IO.Building.Building> and outputs its SIA 2024 room definition.",
              "[hive]", "IO-Core")
        {
        }

        public override GH_Exposure Exposure => GH_Exposure.secondary;

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Hive Building", "Building", "Reads in a Hive Building.", GH_ParamAccess.item);
        }


        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("Net Zone Areas", "ZoneAreas", "Net Floor areas in [m²] of all zones of the building.", GH_ParamAccess.list);
            pManager.AddNumberParameter("Windows Areas", "WinAreas", "All window areas in [m²] of the building.", GH_ParamAccess.list);
            pManager.AddNumberParameter("External Surfaces Areas", "ExtSrfAreas", "Surface areas of the building that are exposed to the environment (external)", GH_ParamAccess.list);
            pManager.AddTextParameter("SIA 2024 Room", "SiaRoom", "SIA 2024 room definitions for each zone.", GH_ParamAccess.item);
            pManager.AddNumberParameter("All External Surface Areas", "AllExtSrfAreas", "All external surface areas, including opaque and transparent (windows) surfaces.", GH_ParamAccess.list);
            pManager.AddTextParameter("Surface Type", "SrfType", "External surface type: 'window', 'wall', or 'roof' (TODO 'floor').", GH_ParamAccess.list);
            pManager.AddTextParameter("SUA Room Schedules", "SiaRoomSchedules", "Schedules for occupancy, devices, lighting, amd setpoints.", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Use Adaptive Comfort", "UseAdaptiveComfort", "Determines whether to use adaptive comfort (true) or SIA 2024 setpoints (false)", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Use Natural Ventilation", "UseNatVent", "Use natural ventilation in the demand calculation. Based on simplified single sided ventilation", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Use Fixed Time Constant", "UseFixedTimeConstant", "Use the fixed time constant provided by SIA 2024", GH_ParamAccess.item);
        }


        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Building.Building building = null;
            if (!DA.GetData(0, ref building)) return;

            int zoneCount = building.Zones.Length;
            double[] zoneAreas = new double[zoneCount];
            var windowAreas = new List<double>();
            var extSrfAreas = new List<double>();
            var allSrfAreas = new List<double>();
            var srfTypes = new List<string>();  // these strings need to correspond with the sia380.py in Hive.Core. not very elegant... ^^
            var zonesSchedules = new List<ZoneSchedules>();

            for (int i = 0; i < zoneCount; i++)
            {
                Zone zone = building.Zones[i];

                zoneAreas[i] = zone.FloorsAreaNet;
                foreach (Floor floor in zone.Floors)
                {
                    // TO DO: make check that it's not a void
                    if (floor.IsExternal)
                    {
                        extSrfAreas.Add(floor.Area);
                        allSrfAreas.Add(floor.Area);
                        srfTypes.Add("floor");
                    }
                }

                
                foreach (Window opening in zone.Windows)
                {
                    windowAreas.Add(opening.Area);
                    allSrfAreas.Add(opening.Area);
                    srfTypes.Add("window");
                }

                
                foreach (Wall wall in zone.Walls)
                {
                    // TO DO: for Hive 0.2
                    //if (wall.IsExternal)
                    //{

                    //}
                    extSrfAreas.Add(wall.Area);
                    allSrfAreas.Add(wall.Area);
                    srfTypes.Add("wall");
                    // TO DO: check, if external. VERY IMPORTANT
                }
                foreach (Roof roof in zone.Roofs)
                {
                    extSrfAreas.Add(roof.Area);
                    allSrfAreas.Add(roof.Area);
                    srfTypes.Add("roof");
                    //TO DO: check if external. VERY IMPORTANT
                }

                //// Ceiling is always internal? 
                //foreach (BuildingComponents.Ceiling ceiling in zone.Ceilings)
                //    extSrfAreas[i] += ceiling.Area;

                //// TO DO: only if the surface below is air, like an overhanging floor / cantilever. work with IxExternal
                //foreach (BuildingComponents.Floor floor in zone.Floors)
                //    extSrfAreas[i] += floor.Area;

                zonesSchedules.Add(zone.Schedules);
            }


            // serialize sia2024 dictionary back into a json. can we avoid this double work? (Deserialized in GHBuilding, now serialized again)
            //var json = building.SIA2024.ToJson();
            //JavaScriptSerializer js = new JavaScriptSerializer();

            DA.SetDataList(0, zoneAreas);
            DA.SetDataList(1, windowAreas);
            DA.SetDataList(2, extSrfAreas);
            DA.SetData(3, building.SIA2024.ToJson()); // single zone
            DA.SetDataList(4, allSrfAreas);
            DA.SetDataList(5, srfTypes);
            DA.SetData(6, Sia2024Schedules.ToJson(zonesSchedules[0].RoomType)); // single zone !!
            DA.SetData(7, building.Zones[0].RunAdaptiveComfort); // single zone !!
            DA.SetData(8, building.Zones[0].RunNaturalVentilation); // single zone !!
            DA.SetData(9, building.Zones[0].UseFixedTimeConstant); // single zone !!
        }


        protected override System.Drawing.Bitmap Icon => Properties.Resources.IOCore_DistSIAroom;


        public override Guid ComponentGuid => new Guid("a7f469fc-da96-426d-ae01-96ee33810a6e");
    }
}