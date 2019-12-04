using Grasshopper.GUI;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Attributes;
using System.Drawing;

namespace Hive.GUI
{
    public class DrawingComponentCustom : GH_ComponentAttributes
    {
        public DrawingComponentCustom(DrawingComponent owner) : base(owner) { }

        protected override void Layout()
        {
            Pivot = GH_Convert.ToPoint(Pivot);

            m_innerBounds = new RectangleF(Pivot.X, Pivot.Y, 125, 100);
            LayoutInputParams(Owner, m_innerBounds);
            LayoutOutputParams(Owner, m_innerBounds);
            Bounds = LayoutBounds(Owner, m_innerBounds);
        }

        protected override void Render(Grasshopper.GUI.Canvas.GH_Canvas canvas, Graphics graphics, Grasshopper.GUI.Canvas.GH_CanvasChannel channel)
        {
            if (channel != Grasshopper.GUI.Canvas.GH_CanvasChannel.Objects)
            {
                base.Render(canvas, graphics, channel);
                return;
            }

            RenderComponentCapsule(canvas, graphics, true, false, false, true, true, true);

            Rectangle rec = GH_Convert.ToRectangle(m_innerBounds);
            graphics.FillRectangle(Brushes.White, rec);

            if (Grasshopper.GUI.Canvas.GH_Canvas.ZoomFadeLow > 0)
            {
                Rectangle reci = rec;
                reci.Inflate(-10, -10);

                Point pA = new Point(reci.Left, reci.Bottom);
                Point pB = new Point(reci.Right, reci.Bottom);
                Point pC = new Point(reci.Right, reci.Top);

                Point pP = new Point((pA.X + pB.X) / 2, (pA.Y + pB.Y) / 2);
                Point pQ = new Point((pB.X + pC.X) / 2, (pB.Y + pC.Y) / 2);
                Point pR = new Point((pA.X + pC.X) / 2, (pA.Y + pC.Y) / 2);
                pP.Offset(16, 1);
                pQ.Offset(0, 10);

                int radius = 14;
                Rectangle rP = new Rectangle(pP, new Size(radius, radius));
                Rectangle rQ = new Rectangle(pQ, new Size(radius, radius));
                Rectangle rR = new Rectangle(pR, new Size(radius, radius));
                rP.Offset(-radius / 2, -radius / 2);
                rQ.Offset(-radius / 2, -radius / 2);
                rR.Offset(-radius / 2, -radius / 2);

                Point pAlpha = new Point(pA.X + 26, pA.Y - 8);
                Point pBeta = new Point(pC.X - 10, pC.Y + 24);
                Rectangle rAlpha = new Rectangle(pA.X - 1, pA.Y - 1, 2, 2);
                Rectangle rBeta = new Rectangle(pC.X - 1, pC.Y - 1, 2, 2);
                rAlpha.Inflate(20, 20);
                rBeta.Inflate(18, 18);

                Pen triPen = new Pen(Color.Black, 3);
                triPen.LineJoin = System.Drawing.Drawing2D.LineJoin.Round;
                graphics.DrawPolygon(triPen, new Point[] { pA, pB, pC, pA });
                triPen.Dispose();

                graphics.DrawArc(Pens.Black, rAlpha, 323, 37);
                graphics.DrawArc(Pens.Black, rBeta, 90, 53);
                graphics.DrawLine(Pens.Black, pB.X - 8, pB.Y, pB.X - 8, pB.Y - 8);
                graphics.DrawLine(Pens.Black, pB.X - 8, pB.Y - 8, pB.X, pB.Y - 8);

                graphics.FillEllipse(Brushes.Black, rP);
                graphics.FillEllipse(Brushes.Black, rQ);
                graphics.FillEllipse(Brushes.Black, rR);

                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (canvas.Viewport.Zoom == 1.0f)
                {
                    pP.Offset(0, 1);
                    pQ.Offset(0, 1);
                    pR.Offset(0, 1);
                    pAlpha.Offset(0, 1);
                    pBeta.Offset(0, 1);
                }
                GH_GraphicsUtil.RenderCenteredText(graphics, "P", GH_FontServer.ConsoleAdjusted, Color.White, pP);
                GH_GraphicsUtil.RenderCenteredText(graphics, "Q", GH_FontServer.ConsoleAdjusted, Color.White, pQ);
                GH_GraphicsUtil.RenderCenteredText(graphics, "R", GH_FontServer.ConsoleAdjusted, Color.White, pR);
                GH_GraphicsUtil.RenderCenteredText(graphics, "α", GH_FontServer.ConsoleAdjusted, Color.Black, pAlpha);
                GH_GraphicsUtil.RenderCenteredText(graphics, "β", GH_FontServer.ConsoleAdjusted, Color.Black, pBeta);
                GH_GraphicsUtil.ShadowRectangle(graphics, rec);
            }
            if (Grasshopper.GUI.Canvas.GH_Canvas.ZoomFadeLow < 255)
            {
                Brush blendfill = new SolidBrush(Color.FromArgb(255 - Grasshopper.GUI.Canvas.GH_Canvas.ZoomFadeLow, Color.White));
                graphics.FillRectangle(blendfill, rec);
                blendfill.Dispose();
            }
            graphics.DrawRectangle(Pens.Black, rec);
        }
    }
}
