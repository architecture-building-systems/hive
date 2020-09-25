using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using System.Windows.Documents;
using Grasshopper.Kernel;
using Hive.IO.Building;

namespace Hive.IO.GhDistributors
{
    public class GhSIA : GH_Component
    {
        public GhSIA()
          : base("Distributor SIARoom Hive", "HiveDistSIARoom",
              "Sia 2024 distributor that reads in an Hive.IO.Building object and outputs its Sia 2024 room definition.",
              "[hive]", "IO")
        {
        }

        public override GH_Exposure Exposure => GH_Exposure.quarternary;

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Hive.IO.Building", "HiveIOBldg", "Reads in an Hive.IO.Building object.", GH_ParamAccess.item);
        }


        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            // NOTE! descriptions not accurate. Currently, Hive only supports one zone and one room definition.
            pManager.AddNumberParameter("Zone Areas", "ZoneAreas", "Floor areas in [m²] of all zone of the building.", GH_ParamAccess.list);
            pManager.AddNumberParameter("Windows Areas", "WinAreas", "All window areas in [m²] of the building.", GH_ParamAccess.list);
            pManager.AddNumberParameter("External Surfaces Areas", "ExtSrfAreas", "ExtSrfAreas", GH_ParamAccess.list);
            pManager.AddTextParameter("SIA 2024 Room", "SiaRoom", "SIA 2024 room definitions for each zone.", GH_ParamAccess.item);
            pManager.AddNumberParameter("AllExtSrfAreas", "AllExtSrfAreas", "All external surface areas", GH_ParamAccess.list);
            pManager.AddTextParameter("Surface Type", "SrfType", "External Surface type, 'opaque' or 'transp'", GH_ParamAccess.list);
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
            var srfTypes = new List<string>();
            string opaque = "opaque";
            string transp = "transp";

            for (int i = 0; i < zoneCount; i++)
            {
                Zone zone = building.Zones[i];

                zoneAreas[i] = 0.0;
                foreach (Floor floor in zone.Floors)
                {
                    // TO DO: make check that it's not a void
                    zoneAreas[i] += floor.Area;
                }

                
                foreach (Window opening in zone.Windows)
                {
                    windowAreas.Add(opening.Area);
                    allSrfAreas.Add(opening.Area);
                    srfTypes.Add(transp);
                }

                
                foreach (Wall wall in zone.Walls)
                {
                    // TO DO: for Hive 0.2
                    //if (wall.IsExternal)
                    //{

                    //}
                    extSrfAreas.Add(wall.Area);
                    allSrfAreas.Add(wall.Area);
                    srfTypes.Add(opaque);
                    // TO DO: check, if external. VERY IMPORTANT
                }
                foreach (Roof roof in zone.Roofs)
                {
                    extSrfAreas.Add(roof.Area);
                    allSrfAreas.Add(roof.Area);
                    srfTypes.Add(opaque);
                    //TO DO: check if external. VERY IMPORTANT
                }

                //// Ceiling is always internal? 
                //foreach (BuildingComponents.Ceiling ceiling in zone.Ceilings)
                //    extSrfAreas[i] += ceiling.Area;

                //// TO DO: only if the surface below is air, like an overhanging floor / cantilever. work with IxExternal
                //foreach (BuildingComponents.Floor floor in zone.Floors)
                //    extSrfAreas[i] += floor.Area;
            }


            // serialize sia2024 dictionary back into a json. can we avoid this double work? (Deserialized in GHBuilding, now serialized again)
            //var json = building.SIA2024.ToJson();
            //JavaScriptSerializer js = new JavaScriptSerializer();


            DA.SetDataList(0, zoneAreas);
            DA.SetDataList(1, windowAreas);
            DA.SetDataList(2, extSrfAreas);
            DA.SetData(3, building.SIA2024.ToJson());
            DA.SetDataList(4, allSrfAreas);
            DA.SetDataList(5, srfTypes);
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
            get { return new Guid("a7f469fc-da96-426d-ae01-96ee33810a6e"); }
        }
    }
}