using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Attributes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Hive.GUI
{
    class HiveGUIComponentCustom : GH_ComponentAttributes
    {
        public HiveGUIComponentCustom(IGH_Component owner) : base(owner) { }
        public bool IsBtnDown { get; set; }
        //public RectangleF rec2;

        private Form window;
        private ComboBox componentSelect;
        private string component = "";

        //private GH_Capsule capsuleBtn;

        protected override void Layout()
        {
            Pivot = GH_Convert.ToPoint(Pivot);

            m_innerBounds = new RectangleF(Pivot.X, Pivot.Y, 175, 120);
            LayoutInputParams(Owner, m_innerBounds);
            LayoutOutputParams(Owner, m_innerBounds);
            Bounds = LayoutBounds(Owner, m_innerBounds);
        }

        public override GH_ObjectResponse RespondToMouseDoubleClick(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            CreateWindow();

            return base.RespondToMouseDoubleClick(sender, e);
        }

        public override GH_ObjectResponse RespondToMouseUp(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            bool bd = IsBtnDown;
            if ((e.Button == MouseButtons.Left) && (this.Owner != null) && (bd == true))
            {
                CreateWindow();
            }
            return base.RespondToMouseUp(sender, e);
        }

        private GH_ObjectResponse CreateWindow()
        {
            IsBtnDown = false;
            Owner.ExpireSolution(true);

            window = new Form();
            window.Text = "Testing popup window";
            window.Size = new Size(590, 175);
            window.StartPosition = FormStartPosition.CenterScreen;
            window.FormBorderStyle = FormBorderStyle.FixedSingle;
            window.MaximizeBox = false;
            window.MinimizeBox = false;
            window.ShowIcon = false;
            window.Name = "popup";

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

            return GH_ObjectResponse.Release;
        }

        private void ComponentSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            component = componentSelect.SelectedItem.ToString();
            TestData.xxx = componentSelect.SelectedItem.ToString();
        }

        private void Cancelbtn_Click(object sender, EventArgs e)
        {
            window.Close();
        }

        private void OKbtn_Click(object sender, EventArgs e)
        {
            window.Close();
        }

        protected override void Render(GH_Canvas canvas, Graphics graphics, GH_CanvasChannel channel)
        {
            if(channel == GH_CanvasChannel.Wires)
            {
                base.Render(canvas, graphics, channel);
            }

            GH_Capsule capsule = GH_Capsule.CreateCapsule(Bounds, GH_Palette.Transparent, 0, 0);
            capsule.AddInputGrip(InputGrip.X, InputGrip.Y);
            capsule.AddOutputGrip(OutputGrip.X, OutputGrip.Y - 40);
            capsule.AddOutputGrip(OutputGrip.X, OutputGrip.Y);
            capsule.AddOutputGrip(OutputGrip.X, OutputGrip.Y + 40);
            capsule.Render(graphics, Selected, Owner.Locked, true);
            capsule.Dispose();
            capsule = null;  

            StringFormat format = new StringFormat();
            format.Alignment = StringAlignment.Center;
            format.LineAlignment = StringAlignment.Center;

            RectangleF textRectangle = Bounds;
            textRectangle.Height = 40;

            graphics.DrawString(Owner.NickName, GH_FontServer.Large, Brushes.Black, textRectangle, format);
            graphics.DrawString("out", GH_FontServer.Console, Brushes.Black, m_innerBounds.X + 145, m_innerBounds.Y + 75, format);
            graphics.DrawImage(Icons.Hexagon_hive_mc, m_innerBounds.X - 27, m_innerBounds.Y - 65, 240, 250);

            SolidBrush brush = new SolidBrush(Color.Pink);
            Pen pen = new Pen(brush, 1);
            Font font = new Font("MS UI Gothic", 3, FontStyle.Italic);

            //rec2 = new RectangleF(m_innerBounds.X + 5, m_innerBounds.Y + 130, 80, 20);
            //capsuleBtn = GH_Capsule.CreateTextCapsule(rec2, rec2, GH_Palette.Grey, "click", 2, 0);
            //capsuleBtn.Render(graphics, this.Selected, base.Owner.Locked, false);
            //capsuleBtn.Dispose();

            InputGrip.Equals(component);
            OutputGrip.Equals(component);

            format.Dispose();
        }
    }
}
