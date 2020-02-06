using System;
using System.Drawing;
using System.Collections.Generic;
using System.Windows.Forms;
using Grasshopper.GUI;
using Grasshopper.Kernel;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel.Attributes;
using Rhino.Geometry;

namespace Hive.IO
{
    public class GHPV : GH_Component
    {
        public double Value { get; set; }
        public string PVName { get; set; }
        private int combobox1_indexnow { get; set; }


        public GHPV()
          : base("HiveIOPV", "IO_PV",
              "Hive.IO PV component",
              "[hive]", "IO")
        {
            Value = 0.15;
            PVName = "Mono-cristalline";
            combobox1_indexnow = 0;

            //List<string> combobox1_text = new List<string>();
            //combobox1_text.Add("")
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddMeshParameter("mesh", "mesh", "Mesh geometry of the PV", GH_ParamAccess.item);
            //pManager.AddNumberParameter("refefficiency", "refeff", "Reference efficiency. E.g. 0.19.", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("PVObj", "PVObj", "Hive.IO.EnergySystems.PV Object", GH_ParamAccess.item);
            pManager.AddNumberParameter("eff", "eff", "eff", GH_ParamAccess.item);
            pManager.AddTextParameter("name", "name", "name", GH_ParamAccess.item);
            pManager.AddMeshParameter("geo", "geo", "geo", GH_ParamAccess.item);
        }

        public override void CreateAttributes()
        {
            m_attributes = new MySpecialComponentAttributes(this);
        }
        private class MySpecialComponentAttributes : GH_ComponentAttributes
        {
            public MySpecialComponentAttributes(IGH_Component component)
            : base(component)
            { }

            public override GH_ObjectResponse RespondToMouseDoubleClick(GH_Canvas sender, GH_CanvasMouseEvent e)
            {
                (Owner as GHPV)?.DisplayForm();
                return GH_ObjectResponse.Handled;
            }
        }

        FormEnSysPV _form;
        public void DisplayForm()
        {
            if (_form != null)
                return;

            _form = new FormEnSysPV();
            _form.textBox1.Text = Convert.ToString(Value);
            _form.comboBox1.SelectedIndex= combobox1_indexnow;

            _form.FormClosed += OnFormClosed;
            _form.button1.MouseClick += Button1_ValueChanged;

            _form.comboBox1.SelectedIndexChanged += ComboBox1_ItemChanged;


            GH_WindowsFormUtil.CenterFormOnCursor(_form, true);
            _form.Show(Grasshopper.Instances.DocumentEditor);
            _form.Location = Cursor.Position;
        }

        private void ComboBox1_ItemChanged(object sender, EventArgs e)
        {
            if (_form.comboBox1.SelectedItem.Equals("Mono-cristalline"))
            {
                _form.pictureBox1.Image = global::Hive.IO.Properties.Resources.article_18;
                _form.textBox1.Text = "0.1";
                _form.helpProvider1.SetHelpString(_form.pictureBox1, "Mono-cristalline PV is like super old, boring");
            }
            else if (_form.comboBox1.SelectedItem.Equals("fraunhofer cutting edge"))
            {
                _form.pictureBox1.Image = global::Hive.IO.Properties.Resources.fraunhofer;
                _form.textBox1.Text = "0.2";
                _form.helpProvider1.SetHelpString(_form.pictureBox1, "Breakthrough technology, Fraunhofer have reached a new milestone");
            }
            else if (_form.comboBox1.SelectedItem.Equals("A/S crazy ass invention"))
            {
                _form.pictureBox1.Image = global::Hive.IO.Properties.Resources.asf;
                _form.textBox1.Text = "0.3";
                _form.helpProvider1.SetHelpString(_form.pictureBox1, "A/S shows everyone how to do it. innovations everywhere");
            }
            else if (_form.comboBox1.SelectedItem.Equals("alien-super-technology"))
            {
                _form.pictureBox1.Image = global::Hive.IO.Properties.Resources.stardestroyer;
                _form.textBox1.Text = "0.99";
                _form.helpProvider1.SetHelpString(_form.pictureBox1, "Execute order 66");
            }
            //Value = Convert.ToDouble(_form.textBox1.Text);
            //ExpireSolution(true);       // do i need this? what is it doing?!
        }

        private void Button1_ValueChanged(object sender, EventArgs e)
        {
            Value = Convert.ToDouble(_form.textBox1.Text);
            PVName = _form.comboBox1.SelectedItem.ToString();
            combobox1_indexnow = _form.comboBox1.SelectedIndex;
            ExpireSolution(true);
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
            Mesh mesh = new Mesh();
            if (!DA.GetData(0, ref mesh)) { return; }
            
            double refEff = Value;
            //if (!DA.GetData(1, ref refEff)) { refEff = 0.19; }
            string pvname = PVName;

            EnergySystem.PV pv = new EnergySystem.PV(mesh, refEff, pvname);
            DA.SetData(0, pv);
            DA.SetData(1, pv.RefEfficiencyElectric);
            DA.SetData(2, pv.Name);
            DA.SetData(3, pv.SurfaceGeometry);
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return null;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("59389231-9a1b-4732-99dd-2bdda6b78bb8"); }
        }
    }
}