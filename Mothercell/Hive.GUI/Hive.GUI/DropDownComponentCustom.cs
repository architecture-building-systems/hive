using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Attributes;
using System.Drawing;

namespace Hive.GUI
{
    class DropDownComponentCustom : GH_ComponentAttributes
    {
        public DropDownComponentCustom(IGH_Component owner) : base(owner) { }

        protected override void Layout()
        {
            base.Layout();

            //Rectangle rectangle1 = GH_Convert.ToRectangle(this.Bounds);
            //rectangle1.Height += 44;
            Rectangle textBox = GH_Convert.ToRectangle(Bounds); ;
            textBox.X = (int)Bounds.X + 23;
            textBox.Y = (int)Bounds.Y + 45;
            textBox.Width = 340;
            textBox.Height = 20;
            textBox.Inflate(5, 5);
            RectangleF newBounds = new RectangleF(Pivot.X, Pivot.Y, 300, 150);
            LayoutInputParams(Owner, newBounds);
            LayoutOutputParams(Owner, newBounds);
            Bounds = LayoutBounds(Owner, newBounds);
            TextBounds = textBox;
        }

        private Rectangle TextBounds { get; set; }


        protected override void Render(GH_Canvas canvas, Graphics graphics, GH_CanvasChannel channel)
        {
            if (channel == GH_CanvasChannel.Wires)
            {
                base.Render(canvas, graphics, channel);
            }

            if (channel == GH_CanvasChannel.Objects)
            {
                GH_Capsule capsule = GH_Capsule.CreateCapsule(Bounds, GH_Palette.Grey, 5, 0);
                capsule.AddInputGrip(InputGrip.X, InputGrip.Y);
                capsule.AddOutputGrip(OutputGrip.X, OutputGrip.Y);
                capsule.Render(graphics, Selected, Owner.Locked, true);
                capsule.Dispose();

                Font font = new Font("Arial", 14f, FontStyle.Bold);
                GH_Capsule textCapsule = GH_Capsule.CreateTextCapsule(TextBounds, TextBounds, GH_Palette.Black, "Horizontal text", font, 5, 0);
                textCapsule.Render(graphics, Selected, Owner.Locked, false);
                textCapsule.Dispose();
            }

            if (channel != GH_CanvasChannel.Objects)
                return;
        }
    }
}
