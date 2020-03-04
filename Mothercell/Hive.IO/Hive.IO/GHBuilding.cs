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
          : base("Hive.IO.Building", "Hive.IO.Building",
              "Hive Building, representing thermal and construction properties. Like a multi-zone building model.",
              "[hive]", "IO")
        {
        }


        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBrepParameter("Zone Geometry", "Zone Geometry", "Zone geometry. Breps. Only one box for now. Must be closed and convex,", GH_ParamAccess.item);
            pManager.AddSurfaceParameter("Windows", "Windows", "Window surfaces that lie on the zone geometry", GH_ParamAccess.list);
            pManager.AddTextParameter("SIA2024dict", "SIA2024dict", "SIA2024dict, defining which SIA 2024 room type this here is.", GH_ParamAccess.item);
        }


        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Hive.IO.Building", "Hive.IO.Building", "Hive.IO.Building Object, that contains all zones and windows. Solved for adjacencies", GH_ParamAccess.item);

            //pManager.AddBooleanParameter("conxex", "convex", "convex", GH_ParamAccess.item);
            //pManager.AddBooleanParameter("linear", "linear", "linear", GH_ParamAccess.item);
            //pManager.AddBooleanParameter("closed", "closed", "closed", GH_ParamAccess.item);
            //pManager.AddBooleanParameter("valid", "valid", "valid", GH_ParamAccess.item);
        }


        public override void CreateAttributes()
        {
            m_attributes = new BuildingComponentAttributes(this);
        }

        private class BuildingComponentAttributes : GH_ComponentAttributes
        {
            public BuildingComponentAttributes(IGH_Component component) : base(component) { }

            //// Hive 0.2
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
        // leave for Hive 0.2
        private void OnButton11Click(object sender, EventArgs e)
        {
            ri.GetObject go = new ri.GetObject();
            go.SetCommandPrompt("pick building brep");
            go.GroupSelect = false; //set to true for Hive0.2
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

            List<rg.Surface> windows = new List<rg.Surface>();
            if (!DA.GetDataList(1, windows)) return;

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

            Building building = new Building(new Zone [1]{ zone }, bldg_type);
            building.SetSIA2024((Dictionary<string, object>)sia2024, building.Zones);   // can be changed in the future via Windows Form: FormBuilding.cs



            DA.SetData(0, building);
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
            get { return new Guid("43a45a89-485b-4134-b073-17bac23e76d5"); }
        }
    }
}