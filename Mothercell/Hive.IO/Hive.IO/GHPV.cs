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
        public double Form_pv_eff { get; set; }
        public double Form_pv_cost { get; set; }
        public double Form_pv_co2 { get; set; }
        public string Form_pv_name { get; set; }
        private int Indexnow { get; set; }


        public GHPV()
          : base("HiveIOPV", "IO_PV",
              "Hive.IO PV component",
              "[hive]", "IO")
        {
            //Form_pv_eff = 0.15;
            //PVName = "Mono-crystalline";
            Indexnow = 0;

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
            _form.comboBox1.SelectedIndex= Indexnow;
            _form.textBox1.Text = _form.Efficiency[Indexnow].ToString();// Convert.ToString(Form_pv_eff);
            _form.textBox2.Text = _form.Cost[Indexnow].ToString();
            _form.textBox3.Text = _form.CO2[Indexnow].ToString();
            _form.pictureBox1.Image = _form.Image[Indexnow];
            _form.helpProvider1.SetHelpString(_form.pictureBox1, _form.HelperText[Indexnow]);

            _form.FormClosed += OnFormClosed;

            _form.comboBox1.SelectedIndexChanged += ComboBox1_ItemChanged;
            _form.textBox1.TextChanged += TextBox1_TextChanged;

            GH_WindowsFormUtil.CenterFormOnCursor(_form, true);
            _form.Show(Grasshopper.Instances.DocumentEditor);
            _form.Location = Cursor.Position;
        }

        private void ComboBox1_ItemChanged(object sender, EventArgs e)
        {
            int i = _form.comboBox1.SelectedIndex;
            
            _form.pictureBox1.Image = _form.Image[i];
            _form.textBox1.Text = _form.Efficiency[i].ToString();
            _form.helpProvider1.SetHelpString(_form.pictureBox1, _form.HelperText[i]);

            Form_Update();
        }
        private void TextBox1_TextChanged(object sender, EventArgs e)
        {
            Form_Update();
        }

        private void Form_Update()
        {
            Form_pv_eff = Convert.ToDouble(_form.textBox1.Text);
            Form_pv_name = _form.comboBox1.SelectedItem.ToString();
            Indexnow = _form.comboBox1.SelectedIndex;
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
            
            double refEff = Form_pv_eff;
            //if (!DA.GetData(1, ref refEff)) { refEff = 0.19; }
            string pvname = Form_pv_name;

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