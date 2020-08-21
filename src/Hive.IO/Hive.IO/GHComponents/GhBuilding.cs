using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Attributes;
using rg = Rhino.Geometry;
using ri = Rhino.Input.Custom;
using Hive.IO.Building;
using Hive.IO.Forms;

namespace Hive.IO.GHComponents
{
    public class GhBuilding : GH_Component
    {
        public GhBuilding()
          : base("Input Building Hive", "HiveInputBuilding",
              "Hive.IO.Building input, representing the building geometry, as well as thermal and construction properties.",
              "[hive]", "IO")
        {
        }


        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            // NOTE! the descriptions are not fully accurate for the current Hive version. Zone Brep currently only takes one zone (in future it should take multiple zones), and SIARoom should also take multiple rooms in future, one for each zone).
            pManager.AddBrepParameter("Zone Brep Geometries", "ZoneBreps", "Zone geometries as Breps." +
                "\nStrict conditions: Must be a closed Polysurface." +
                "\nOptional conditions (necessary for EnergyPlus): (i) Linearity of edges, (ii) convexity, (iii) planarity of faces.", GH_ParamAccess.item);
            pManager.AddSurfaceParameter("Windows Surfaces", "WinSrfs", "Windows surfaces that lie on a zone Brep." +
                "\nStrict conditions: (i) Windows must not intersect and (ii) Windows must lie entirely on a Brep face." +
                "\nOptional input.", GH_ParamAccess.list);
            pManager[1].Optional = true;
            pManager.AddSurfaceParameter("Additional floor surfaces", "FloorSrfs", "Additional floor surface that must lie within the zone Brep", GH_ParamAccess.list);
            pManager[2].Optional = true;
            pManager.AddTextParameter("SIA2024 Room", "SiaRoom", "SIA 2024 Room definition." +
                "\nMust be a string containing information about the zone (internal loads, construction, etc)." +
                "\nMust be in the correct format as defined in the Hive SIA 2024 Rooms list component." +
                "\nOptional input.", GH_ParamAccess.item);
            pManager[3].Optional = true;
        }


        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            // NOTE! description not accurate, zone adjacencies not implemented yet.
            pManager.AddGenericParameter("Hive.IO.Building", "HiveIOBldg", "Creates an instance of a Hive.IO.Building." +
                "\nIt contains all geometric information, such as zone definitions and windows, as well as thermal and construction properties." +
                "\nThe object will also contain surface and zone adjacencies.", GH_ParamAccess.item);
        }


        public override void CreateAttributes()
        {
            m_attributes = new BuildingComponentAttributes(this);
        }

        private class BuildingComponentAttributes : GH_ComponentAttributes
        {
            public BuildingComponentAttributes(IGH_Component component) : base(component) { }
            public override GH_ObjectResponse RespondToMouseDoubleClick(GH_Canvas sender, GH_CanvasMouseEvent e)
            {
                ((GhBuilding)Owner).ShowUiClicked(this, null);
                return GH_ObjectResponse.Handled;
            }
        }


        protected override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu)
        {
            Menu_AppendItem(menu, "Show UI", ShowUiClicked, null, true, false);
        }

        public void ShowUiClicked(object sender, EventArgs e)
        {
            var form = new BuildingInput();
            form.ShowDialog();
        }


        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var zoneBrep = new rg.Brep();
            if (!DA.GetData(0, ref zoneBrep)) return;

            var windows = new List<rg.BrepFace>();
            DA.GetDataList(1, windows);

            var floors = new List<rg.BrepFace>();
            DA.GetDataList(2, floors);

            
            // figure out output of this component - either from the input parameter (if one is specified)
            // or a building from the form.
            Building.Building building = GetBuildingFromFormInput();

            string json = null;
            var parametricSiaRoomSpecified = DA.GetData(3, ref json);
            if (parametricSiaRoomSpecified)
            {
                building = CreateParametricBuilding(json, zoneBrep, windows, floors);
            }

            DA.SetData(0, building);
        }

        /// <summary>
        /// Create a building based on the form input. Note, that if no form input
        /// was given by the user ("double-click" on the form), then default to null.
        /// (This avoids surprises for the user)
        /// </summary>
        private Building.Building GetBuildingFromFormInput()
        {
            throw new NotImplementedException();
        }

        private Building.Building CreateParametricBuilding(string json, rg.Brep zoneBrep, List<rg.BrepFace> windows, List<rg.BrepFace> floors)
        {
            var siaRoom = Sia2024Record.FromJson(json);

            BuildingType buildingType;
            switch (siaRoom.RoomType)
            {
                case "1.1 Wohnen Mehrfamilienhaus":
                case "1.2 Wohnen Einfamilienhaus":
                    buildingType = BuildingType.Residential;
                    break;
                case "3.1 Einzel-, Gruppenbuero":
                case "3.2 Grossraumbuero":
                    buildingType = BuildingType.Office;
                    break;
                case "9.1 Produktion (grobe Arbeit)":
                case "9.2 Produktion (feine Arbeit)":
                    buildingType = BuildingType.Industry;
                    break;
                case "9.3 Laborraum":
                    buildingType = BuildingType.Laboratory;
                    break;
                case "4.1 Schulzimmer":
                case "4.2 Lehrerzimmer":
                case "4.3 Bibliothek":
                case "4.4 Hoersaal":
                    buildingType = BuildingType.School;
                    break;
                case "5.1 Lebensmittelverkauf":
                    buildingType = BuildingType.Supermarket;
                    break;
                default:
                    buildingType = BuildingType.Undefined;
                    break;
            }

            var tolerance = Rhino.RhinoDoc.ActiveDoc.ModelAbsoluteTolerance;
            var zone = new Zone(zoneBrep, 0, tolerance, siaRoom.RoomType, windows.ToArray(), floors.ToArray());
            if (!zone.IsValid)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, zone.ErrorText);
                return null;
            }

            if (!zone.IsValidEPlus || !zone.IsFloorInZone)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, zone.ErrorText);
            }

            var building = new Building.Building(new[] {zone}, buildingType);
            building.ApplySia2024Constructions(siaRoom, building.Zones);
            return building;
        }


        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Hive.IO.Properties.Resources.IO_Building;
            }
        }


        public override Guid ComponentGuid
        {
            get { return new Guid("43a45a89-485b-4134-b073-17bac23e76d5"); }
        }
    }
}