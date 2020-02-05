using System;
using System.Drawing;
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


        public GHPV()
          : base("HiveIOPV", "IO_PV",
              "Hive.IO PV component",
              "[hive]", "IO")
        {
            Value = 0.15;
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            //pManager.AddMeshParameter("mesh", "mesh", "Mesh geometry of the PV", GH_ParamAccess.item);
            //pManager.AddNumberParameter("refefficiency", "refeff", "Reference efficiency. E.g. 0.19.", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            //pManager.AddGenericParameter("PVObj", "PVObj", "Hive.IO.EnergySystems.PV Object", GH_ParamAccess.item);
            pManager.AddNumberParameter("eff", "eff", "eff", GH_ParamAccess.item);
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

            _form.FormClosed += OnFormClosed;
            _form.button1.MouseClick += Button1_ValueChanged;

            GH_WindowsFormUtil.CenterFormOnCursor(_form, true);
            _form.Show(Grasshopper.Instances.DocumentEditor);
        }

        private void Button1_ValueChanged(object sender, EventArgs e)
        {
            Value = Convert.ToDouble(_form.textBox1.Text);
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
            //Mesh mesh = new Mesh();
            //if(!DA.GetData(0, ref mesh)) { return; }

            double refEff = Value;
            //if (!DA.GetData(1, ref refEff)) { refEff = 0.19; }


            //EnergySystem.PV pv = new EnergySystem.PV(mesh, 0.0, refEff);
            //DA.SetData(0, pv);
            DA.SetData(0, refEff);
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