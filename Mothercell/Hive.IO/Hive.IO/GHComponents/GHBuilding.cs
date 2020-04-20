using System;
using System.Windows.Forms;
using System.Collections.Generic;

using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Attributes;
using Grasshopper.Kernel.Types;

using rg = Rhino.Geometry;
using ri = Rhino.Input.Custom;
using System.Web.Script.Serialization;

namespace Hive.IO
{
    public class GHBuilding : GH_Component
    {
        public GHBuilding()
          : base("Hive.IO.Building", "Hive.IO.Bldg",
              "Hive Building, representing the building geometry, as well as thermal and construction properties.",
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
            pManager.AddTextParameter("SIA2024 Room", "SiaRoom", "SIA 2024 Room definition." +
                "\nMust be a string containing information about the zone (internal loads, construction, etc)." +
                "\nMust be in the correct format as defined in the Hive SIA 2024 Rooms list component." +
                "\nOptional input.", GH_ParamAccess.item);
            pManager[2].Optional = true;
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

            //// NOTE! activate for Hive 0.3
            //public override GH_ObjectResponse RespondToMouseDoubleClick(GH_Canvas sender, GH_CanvasMouseEvent e)
            //{
            //    (Owner as GHBuilding)?.DisplayForm();
            //    return GH_ObjectResponse.Handled;
            //}
        }

        FormBuilding _form;
        public void DisplayForm()
        {
            if (_form != null) 
                return;

            _form = new FormBuilding();
            _form.SetRhinoDoc(Rhino.RhinoDoc.ActiveDoc);

            _form.button11.MouseClick += OnButton11Click;

            _form.FormClosed += OnFormClosed;

            GH_WindowsFormUtil.CenterFormOnCursor(_form, true);
            _form.Show(Grasshopper.Instances.DocumentEditor);
            _form.Location = Cursor.Position;
        }

        //maybe in the FormBuilding.cs?
        // trying to jump into rhino viewport for selecting geometry there.
        // leave for Hive 0.3
        private void OnButton11Click(object sender, EventArgs e)
        {
            ri.GetObject go = new ri.GetObject();
            go.SetCommandPrompt("pick building brep");
            go.GroupSelect = false; //set to true for Hive0.3
            if (go.CommandResult() != Rhino.Commands.Result.Success)
                return;

            List<Guid> ids = new List<Guid>();
            for (int i=0; i<go.ObjectCount; i++)
            {
                ids.Add(go.Object(i).ObjectId);
            }
            Rhino.RhinoDoc.ActiveDoc.Views.Redraw();

        }

        private void OnFormClosed(object sender, FormClosedEventArgs formClosedEventArgs)
        {
            _form = null;
        }



        protected override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu)
        {
            Menu_AppendItem(menu, "Show UI", ShowUiClicked, null, true, false);
        }
        private void ShowUiClicked(object sender, EventArgs e)
        {
            DisplayForm();
        }


        protected override void SolveInstance(IGH_DataAccess DA)
        {
            rg.Brep zoneBrep = new rg.Brep();
            if (!DA.GetData(0, ref zoneBrep)) return;

            List<rg.BrepFace> windows = new List<rg.BrepFace>();
            DA.GetDataList(1, windows);

            string json = null;
            if (!DA.GetData(2, ref json)) return;
            var jss = new JavaScriptSerializer();
            var sia2024 = (IDictionary<string, object>)jss.DeserializeObject(json);
            
            BuildingType bldg_type;
            string zone_description = sia2024["description"].ToString();
            switch (zone_description)
            {
                case "1.1 Wohnen Mehrfamilienhaus":
                case "1.2 Wohnen Einfamilienhaus":
                    bldg_type = BuildingType.Residential;
                    break;
                case "3.1 Einzel-, Gruppenbuero":
                case "3.2 Grossraumbuero":
                    bldg_type = BuildingType.Office;
                    break;
                case "9.1 Produktion (grobe Arbeit)":
                case "9.2 Produktion (feine Arbeit)":
                    bldg_type = BuildingType.Industry;
                    break;
                case "9.3 Laborraum":
                    bldg_type = BuildingType.Laboratory;
                    break;
                case "4.1 Schulzimmer":
                case "4.2 Lehrerzimmer":
                case "4.3 Bibliothek":
                case "4.4 Hoersaal":
                    bldg_type = BuildingType.School;
                    break;
                case "5.1 Lebensmittelverkauf":
                    bldg_type = BuildingType.Supermarket;
                    break;
                default:
                    bldg_type = BuildingType.Undefined;
                    break;
            }

            double tolerance = Rhino.RhinoDoc.ActiveDoc.ModelAbsoluteTolerance;
            Zone zone = new Zone(zoneBrep, 0, tolerance, zone_description, windows.ToArray());
            if (!zone.IsValid)
            {
                this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, zone.ErrorText);
                return;
            }
            if (!zone.IsValidEPlus)
            {
                this.AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, zone.ErrorText);
            }

            Building building = new Building(new Zone [1]{ zone }, bldg_type);
            building.SetSIA2024((Dictionary<string, object>)sia2024, building.Zones);   // can be changed in the future via Windows Form: FormBuilding.cs



            DA.SetData(0, building);
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