using System;
using Grasshopper.Kernel;
using System.Windows.Forms;
using System.Drawing;

namespace Hive.GUI
{
    public class HiveGUIComponent : GH_Component
    {
        private Form window;
        private ComboBox componentSelect;
        private string component = "";

        public HiveGUIComponent()
          : base("Hive.GUI", 
                 "HiveGUI",
                 "GUI testing",
                 "[hive]", 
                 "GUI")
        {
        }

        public override void CreateAttributes()
        {
            m_attributes = new HiveGUIComponentCustom(this);
        }

        public override bool AppendMenuItems(ToolStripDropDown menu)
        {
            Menu_AppendItem(menu, "Get component", Menu_ItemClicked);
            Menu_AppendSeparator(menu);
            Menu_AppendItem(menu, "Read me!");

            return true;
        }

        private void Menu_ItemClicked(object sender, EventArgs e)
        {
            window = new Form();
            window.Text = "Testing popup window";
            window.Size = new Size(590, 175);
            window.StartPosition = FormStartPosition.CenterScreen;
            window.FormBorderStyle = FormBorderStyle.FixedSingle;
            window.MaximizeBox = false;
            window.MinimizeBox = false;
            window.ShowIcon = false;

            var groupLabel = new Label();
            groupLabel.Text = "Select component group:";
            groupLabel.Location = new Point(10, 10);
            groupLabel.Width = 150;
            groupLabel.TextAlign = ContentAlignment.MiddleLeft;
            //groupLabel.BorderStyle = BorderStyle.FixedSingle;

            var groupSelect = new ComboBox();
            groupSelect.Location = new Point(160, 10);
            groupSelect.DropDownStyle = ComboBoxStyle.DropDownList;
            groupSelect.Width = 400;
            groupSelect.Items.Add("group 1");
            groupSelect.Items.Add("group 2");

            var componentLabel = new Label();
            componentLabel.Text = "Select component:";
            componentLabel.Location = new Point(10, 40);
            componentLabel.Width = 150;
            componentLabel.TextAlign = ContentAlignment.MiddleLeft;
            //componentLabel.BorderStyle = BorderStyle.FixedSingle;

            componentSelect = new ComboBox();
            componentSelect.Location = new Point(160, 40);
            componentSelect.DropDownStyle = ComboBoxStyle.DropDownList;
            componentSelect.Width = 400;
            componentSelect.Items.Add("component 1");
            componentSelect.Items.Add("component 2");
            componentSelect.SelectedIndexChanged += ComponentSelect_SelectedIndexChanged;

            var rspLabel = new Label();
            rspLabel.Text = "Reference study period:";
            rspLabel.Location = new Point(10, 70);
            rspLabel.Width = 150;
            rspLabel.TextAlign = ContentAlignment.MiddleLeft;

            var rspTextBox = new TextBox();
            rspTextBox.Location = new Point(160, 70);
            rspTextBox.Width = 50;

            var rspYearsLabel = new Label();
            rspYearsLabel.Text = "(years)";
            rspYearsLabel.Location = new Point(215, 70);
            rspYearsLabel.Width = 50;
            rspYearsLabel.TextAlign = ContentAlignment.MiddleLeft;

            var OKbtn = new Button();
            OKbtn.Text = "Select";
            OKbtn.Location = new Point(10, 100);
            OKbtn.TextAlign = ContentAlignment.MiddleCenter;
            OKbtn.Click += OKbtn_Click;

            var Cancelbtn = new Button();
            Cancelbtn.Text = "Cancel";
            Cancelbtn.Location = new Point(100, 100);
            Cancelbtn.TextAlign = ContentAlignment.MiddleCenter;
            Cancelbtn.Click += Cancelbtn_Click;

            window.Controls.Add(groupLabel);
            window.Controls.Add(groupSelect);
            window.Controls.Add(componentLabel);
            window.Controls.Add(componentSelect);
            window.Controls.Add(rspLabel);
            window.Controls.Add(rspTextBox);
            window.Controls.Add(rspYearsLabel);
            window.Controls.Add(OKbtn);
            window.Controls.Add(Cancelbtn);
            window.Show();
        }

        private void ComponentSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            component = componentSelect.SelectedItem.ToString();
        }

        private void Cancelbtn_Click(object sender, EventArgs e)
        {
            window.Close();
        }

        private void OKbtn_Click(object sender, EventArgs e)
        {
            ExpireSolution(true);
            window.Close();
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("in1", "in1", "in1", GH_ParamAccess.item);
            pManager[0].Optional = true;
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("out1", "out1", "out1", GH_ParamAccess.item);
            pManager.AddTextParameter("out2", "out2", "out2", GH_ParamAccess.item);
            pManager.AddTextParameter("out3", "out3", "out3", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Message = "aaaaaa";

            //GrasshopperDocument = OnPingDocument();

            //Random r = new Random(23);
            //int min = 0;
            //int step = 20;
            //int max = step;
            //Grasshopper.Kernel.Parameters.Param_GenericObject comp = null;
            //Grasshopper.Kernel.Parameters.Param_GenericObject lastComp = null;
            //for (int i = 0; i < 10; i++)
            //{

            //    min += step;
            //    max += step;
            //    for (int j = 0; j < 10; j++)
            //    {
            //        comp = new Grasshopper.Kernel.Parameters.Param_GenericObject();
            //        GrasshopperDocument.AddObject(comp, false, GrasshopperDocument.ObjectCount);
            //        comp.Attributes.Pivot = new System.Drawing.PointF((float)r.Next(min, max), (float)r.Next(0, 2000));
            //    }

            //    if (lastComp != null && comp != null)
            //    {
            //        comp.AddSource(lastComp);
            //    }
            //}


            //comp.Params.Input[0].AddSource(lastComp);
            //lastComp = comp;

            DA.SetData(0, component);
        }

        protected override Bitmap Icon
        {
            get
            {
                return Icons.testSmall;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("42fc54f3-90ad-4d11-8104-44f42521a265"); }
        }
    }
}
