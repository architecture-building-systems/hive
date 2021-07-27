using System;
using System.Collections.Generic;
using System.Windows.Forms;
using GH_IO.Serialization;
using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Attributes;
using Hive.IO.Building;
using Hive.IO.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Rhino;
using rg = Rhino.Geometry;

namespace Hive.IO.GhInputOutput
{
    public class GhBuilding : GH_Component
    {
        public GhBuilding()
          : base("Input Building Hive", "HiveInputBuilding",
              "Hive Building input component and form to define building geometry (zone, floors and windows), as well as thermal, ventilation and construction properties. The form opens with a double click onto the component.",
              "[hive]", "IO")
        {
            // NOTE: Will be overwritten in Read() and SolveInstance
            _buildingInputState = new BuildingInputState(Sia2024Record.First(), new Zone(), true);
        }

        private static readonly ITraceWriter TraceWriter = new MemoryTraceWriter();
        private static readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings
        {
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
            TypeNameHandling = TypeNameHandling.All,
            TraceWriter = TraceWriter
        };

        private BuildingInputState _buildingInputState;
        
        /// <summary>
        /// Save BuildingInputState to the document.
        /// </summary>
        /// <param name="writer"></param>
        /// <returns></returns>
        public override bool Write(GH_IWriter writer)
        {
            if (_buildingInputState == null)
            {
                return base.Write(writer);
            }

            if (!_buildingInputState.IsEditable)
            {
                // don't bother writing down parametric input
                return base.Write(writer);
            }

            try
            {
                writer.SetString("SiaRoom", JsonConvert.SerializeObject(_buildingInputState.SiaRoom, Formatting.Indented, JsonSerializerSettings));
                return base.Write(writer);
            }
            catch (Exception ex)
            {
                RhinoApp.WriteLine($"GhBuilding.Write() failed!! {ex}");
                Message = "Failed to write state to Document";
                return base.Write(writer);
            }
        }

        public override bool Read(GH_IReader reader)
        {
            try
            {
                var json = reader.GetString("SiaRoom");
                _buildingInputState.SiaRoom = JsonConvert.DeserializeObject<Sia2024RecordEx>(json, JsonSerializerSettings);
                
            }
            catch (Exception)
            {
                // let's not worry too much about not being able to read the state...
                Message = "Failed to read state from Document";
            }
            return base.Read(reader);
        }


        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            // FIXME: Should take multiple zones in the future
            pManager.AddBrepParameter("Zone geometry", "ZoneGeometry", 
                "Zone geometry as closed Polysurface. Strict conditions: Geometry must be closed, linear and planar.", 
                GH_ParamAccess.item);
            pManager.AddSurfaceParameter("Windows surfaces", "WindowsSrfs",
                "(Optional input) Windows surfaces of the zone. Strict conditions: (i) Windows must not intersect and (ii) Windows must lie entirely on any of the zone geometry faces.", 
                GH_ParamAccess.list);
            pManager[1].Optional = true;
            pManager.AddSurfaceParameter("Additional floor surfaces", "FloorSrfs",
                "(Optional input) Additional floor surfaces that must lie within the zone geometry. The ground floor is included by default and does not need to be provided separately.", 
                GH_ParamAccess.list);
            pManager[2].Optional = true;
            pManager.AddTextParameter("SIA 2024 Room", "SiaRoom", 
                "(Optional input) SIA 2024 Room definition. Must be a string containing information about the zone (internal loads, construction, etc). " +
                "Furthermore, it must be in the correct format as defined in the Hive SIA 2024 Rooms list component. If no input is provided, the zone needs to be defined via the form (double click onto this component).", 
                GH_ParamAccess.item);
            pManager[3].Optional = true;
        }


        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Hive Building", "Building", 
                "Creates a Building object of type <Hive.IO.Building.Building>. " +
                "It contains all geometric information, such as windows and opaque surfaces, as well as thermal and construction properties.", 
                GH_ParamAccess.item);
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
            if (_buildingInputState == null)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Building input state not set");
                return;
            }

            var form = new BuildingInputForm();
            form.ShowDialog(_buildingInputState);

            // NOTE: actually, I don't think this is necessary, since the state is never reset in the dialog...
            _buildingInputState.SiaRoom = form.State.SiaRoom; // copy over the results (might have changed) 
            _buildingInputState.Zone = form.State.Zone; // copy over the results (might have changed) 

            // make sure down-river components get new output
            ExpireSolution(true);
        }


        protected override void SolveInstance(IGH_DataAccess da)
        {
            var zoneBrep = new rg.Brep();
            if (!da.GetData(0, ref zoneBrep)) return;

            var windows = new List<rg.BrepFace>();
            da.GetDataList(1, windows);

            var floors = new List<rg.BrepFace>();
            da.GetDataList(2, floors);

            
            // figure out output of this component - either from the input parameter (if one is specified)
            // or a building from the form.
            // we need to set _buildingInputState so that the BuildingInput form can be shown...
            string json = null;
            var parametricSiaRoomSpecified = da.GetData(3, ref json);

            Sia2024RecordEx siaRoom;
            if (parametricSiaRoomSpecified)
            {
                try
                {
                    siaRoom = Sia2024RecordEx.FromJson(json);
                    Message = "";
                }
                catch (Exception e)
                {
                    var message = $"ERROR invalid SIA Room input: {e.Message}";
                    RhinoApp.WriteLine(message);
                    Message = message;


                    siaRoom = Sia2024Record.First();
                    siaRoom.Quality = "<Custom>";
                    siaRoom.BuildingUseType = "<Custom>";
                }
            }
            else
            {
                siaRoom = SiaRoomFromBuildingInputState();
            }

            var zone = CreateZone(siaRoom, zoneBrep, windows, floors);

            // BuildingInput form modifies the SiaRoom property, use that. it also uses editable to decide if parametric input was given...
            _buildingInputState = new BuildingInputState(siaRoom, zone, !parametricSiaRoomSpecified);

            var building = CreateBuilding(_buildingInputState.SiaRoom, _buildingInputState.Zone);
            da.SetData(0, building);
        }

        private Sia2024RecordEx SiaRoomFromBuildingInputState()
        {
            try
            {
                // make sure we have clean _buildingInputState if it was parametric before...
                // (the lookup fails for BuildingUseType = "<Custom>" and Quality = "<Custom>")
                Sia2024Record.Lookup(_buildingInputState.SiaRoom);
                return _buildingInputState.SiaRoom;
            }
            catch (Exception)
            {
                return Sia2024Record.First();
            }
        }

        private Building.Building CreateBuilding(Sia2024Record siaRoom, Zone zone)
        {
            var buildingType = BuildingTypeFromDescription(siaRoom.RoomType);
            var building = new Building.Building(new[] {zone}, buildingType);
            building.ApplySia2024Constructions(siaRoom, building.Zones);
            return building;
        }

        private Zone CreateZone(Sia2024Record siaRoom, rg.Brep zoneBrep, List<rg.BrepFace> windows, List<rg.BrepFace> floors)
        {
            var tolerance = Rhino.RhinoDoc.ActiveDoc.ModelAbsoluteTolerance;
            var zone = new Zone(zoneBrep, 0, tolerance, siaRoom.RoomType, windows.ToArray(), floors.ToArray());
            if (!zone.IsValid)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, zone.ErrorText);
                return zone;
            }

            if (!zone.IsValidEPlus || !zone.IsFloorInZone)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, zone.ErrorText);
            }

            zone.RunAdaptiveComfort = _buildingInputState.RunAdaptiveComfort;

            return zone;
        }

        private static BuildingType BuildingTypeFromDescription(string description)
        {
            BuildingType buildingType;
            switch (description)
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

            return buildingType;
        }


        protected override System.Drawing.Bitmap Icon => Properties.Resources.IO_Building;


        public override Guid ComponentGuid => new Guid("43a45a89-485b-4134-b073-17bac23e76d5");
    }
}