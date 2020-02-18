using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;
using Grasshopper;
using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Attributes;
using Grasshopper.Kernel.Special;

namespace Hive.GUI
{
    public class GH_ClusterAttributes : GH_ComponentAttributes
    {
        private static readonly SortedDictionary<int, Bitmap> Patterns = new SortedDictionary<int, Bitmap>();

        public GH_ClusterAttributes(ClusterTest owner) : base((IGH_Component)owner)
        {
        }

        private GH_Document ClusterDocument()
        {
            return ((ClusterTest)this.Owner).m_internalDocument;
        }

        public override void SetupTooltip(PointF point, GH_TooltipDisplayEventArgs e)
        {
            e.Title = this.Owner.NickName;
            e.Text = this.Owner.Description;
            e.Icon = this.Owner.Icon_24x24;
            switch (((ClusterTest)this.Owner).Synchronisation)
            {
                case GH_Synchronisation.MissingReference:
                    e.Description = "This cluster references a file which no longer exists.";
                    if (GH_Canvas.ZoomFadeMedium <= 0)
                        break;
                    e.Description = "This cluster is based on a file which no longer exists.";
                    break;
                case GH_Synchronisation.OutOfDate:
                    e.Description = "This cluster is out-of-date with the file on the disk.";
                    if (GH_Canvas.ZoomFadeMedium <= 0)
                        break;
                    e.Description = "This cluster is out-of-date. Click here to re-load from the disk.";
                    break;
                default:
                    ClusterTest owner = (ClusterTest)this.Owner;
                    if (this.ClusterDocument() == null)
                    {
                        e.Description = "Empty Cluster";
                        break;
                    }
                    Bitmap bitmap1 = this.DocumentDiagram();
                    int num1 = 0;
                    GH_Document ghDocument = this.Owner.OnPingDocument();
                    if (ghDocument != null)
                        num1 = ghDocument.ClusterInstanceCount(owner.DocumentId);
                    int height = Global_Proc.UiAdjust(26);
                    int num2 = 0;
                    if (owner.ProtectionLevel == GH_ClusterProtection.Protected)
                        num2 += height;
                    if (num1 > 0)
                        num2 += height;
                    if (!string.IsNullOrEmpty(owner.FilePath))
                        num2 += height;
                    if (!string.IsNullOrEmpty(owner.Author.Name))
                        num2 += height;
                    if (!string.IsNullOrEmpty(owner.Author.Company))
                        num2 += height;
                    if (!string.IsNullOrEmpty(owner.Author.Copyright))
                        num2 += height;
                    if (num2 > 0)
                    {
                        Bitmap bitmap2 = bitmap1;
                        bitmap1 = new Bitmap(bitmap1.Width, bitmap1.Height + num2, PixelFormat.Format24bppRgb);
                        Graphics g = Graphics.FromImage((Image)bitmap1);
                        g.TextRenderingHint = GH_TextRenderingConstants.GH_CrispText;
                        g.InterpolationMode = InterpolationMode.NearestNeighbor;
                        g.Clear(SystemColors.ControlLight);
                        g.DrawImage((Image)bitmap2, 0, num2, bitmap2.Width, bitmap2.Height);
                        bitmap2.Dispose();
                        int x1 = Global_Proc.UiAdjust(26);
                        int y = 0;
                        if (owner.ProtectionLevel == GH_ClusterProtection.Protected)
                        {
                            g.DrawImage((Image)Res_GUI.Locked_20x20, Global_Proc.UiAdjust(2), y + Global_Proc.UiAdjust(2), Global_Proc.UiAdjust(20), Global_Proc.UiAdjust(20));
                            Rectangle rectangle = new Rectangle(x1, y + Global_Proc.UiAdjust(4), bitmap1.Width - x1, height - Global_Proc.UiAdjust(4));
                            g.DrawString("Cluster is password protected.", GH_FontServer.StandardItalic, Brushes.Black, (RectangleF)rectangle, GH_TextRenderingConstants.NearCenter);
                            GH_GraphicsUtil.EtchFadingHorizontal(g, rectangle.Left, rectangle.Right, rectangle.Bottom, 200, 35);
                            y += height;
                        }
                        if (num1 > 0)
                        {
                            g.DrawImage((Image)Res_GUI.ClusterInstances_20x20, Global_Proc.UiAdjust(2), y + Global_Proc.UiAdjust(2), Global_Proc.UiAdjust(20), Global_Proc.UiAdjust(20));
                            Rectangle rectangle = new Rectangle(x1, y, bitmap1.Width - x1, height);
                            string empty = string.Empty;
                            string str1 = "This cluster occurs";
                            string str2 = "in this document.";
                            string str3;
                            switch (num1)
                            {
                                case 0:
                                    str1 = "This cluster does not occur in this document";
                                    str2 = string.Empty;
                                    str3 = string.Empty;
                                    break;
                                case 1:
                                    str3 = "once";
                                    break;
                                case 2:
                                    str3 = "twice";
                                    break;
                                case 3:
                                    str3 = "trice";
                                    break;
                                default:
                                    str3 = num1.ToString() + " times";
                                    break;
                            }
                            int left = rectangle.Left;
                            int num3 = left + GH_FontServer.StringWidth(str1, GH_FontServer.StandardItalic) + Global_Proc.UiAdjust(2);
                            int x2 = num3;
                            int num4 = x2 + GH_FontServer.StringWidth(str3, GH_FontServer.StandardBold) + Global_Proc.UiAdjust(8);
                            int x3 = num4 + Global_Proc.UiAdjust(3);
                            int num5 = x3 + GH_FontServer.StringWidth(str2, GH_FontServer.StandardItalic) + Global_Proc.UiAdjust(2);
                            Rectangle rec = new Rectangle(x2, rectangle.Y, num4 - x2, rectangle.Height);
                            rec.Inflate(0, -Global_Proc.UiAdjust(6));
                            --rec.Height;
                            GraphicsPath roundedRectangle = GH_CapsuleRenderEngine.CreateRoundedRectangle(rec, rectangle.Height);
                            Pen pen = new Pen(Color.FromArgb(50, 50, 50, 50), (float)Global_Proc.UiAdjust(3));
                            SolidBrush solidBrush = new SolidBrush(Color.FromArgb(50, 50, 50));
                            g.SmoothingMode = SmoothingMode.HighQuality;
                            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                            g.DrawPath(pen, roundedRectangle);
                            g.FillPath((Brush)solidBrush, roundedRectangle);
                            g.PixelOffsetMode = PixelOffsetMode.None;
                            solidBrush.Dispose();
                            pen.Dispose();
                            roundedRectangle.Dispose();
                            g.DrawString(str1, GH_FontServer.StandardItalic, Brushes.Black, (RectangleF)new Rectangle(left, rectangle.Y, num3 - left, rectangle.Height), GH_TextRenderingConstants.NearCenter);
                            g.DrawString(str3, GH_FontServer.StandardBold, Brushes.White, (RectangleF)new Rectangle(x2, rectangle.Y, num4 - x2, rectangle.Height), GH_TextRenderingConstants.CenterCenter);
                            g.DrawString(str2, GH_FontServer.StandardItalic, Brushes.Black, (RectangleF)new Rectangle(x3, rectangle.Y, num5 - x3, rectangle.Height), GH_TextRenderingConstants.NearCenter);
                            GH_GraphicsUtil.EtchFadingHorizontal(g, rectangle.Left, rectangle.Right, rectangle.Bottom, 200, 35);
                            y += height;
                        }
                        if (!string.IsNullOrEmpty(owner.FilePath))
                        {
                            g.DrawImage((Image)Res_MainMenu.Save_20x20, Global_Proc.UiAdjust(2), y + Global_Proc.UiAdjust(2), Global_Proc.UiAdjust(20), Global_Proc.UiAdjust(20));
                            Rectangle rectangle = new Rectangle(x1, y, bitmap1.Width - x1, height);
                            string s = GH_Format.FormatFilePath(owner.FilePath, 100);
                            g.DrawString(s, GH_FontServer.StandardItalic, Brushes.Black, (RectangleF)rectangle, GH_TextRenderingConstants.NearCenter);
                            GH_GraphicsUtil.EtchFadingHorizontal(g, rectangle.Left, rectangle.Right, rectangle.Bottom, 200, 35);
                            y += height;
                        }
                        if (!string.IsNullOrEmpty(owner.Author.Company))
                        {
                            Rectangle rectangle = new Rectangle(x1, y, bitmap1.Width - x1, height);
                            g.DrawString(owner.Author.Company, GH_FontServer.StandardItalic, Brushes.Black, (RectangleF)rectangle, GH_TextRenderingConstants.NearCenter);
                            y += height;
                        }
                        if (!string.IsNullOrEmpty(owner.Author.Name))
                        {
                            Rectangle rectangle = new Rectangle(x1, y, bitmap1.Width - x1, height);
                            g.DrawString(owner.Author.Name, GH_FontServer.StandardItalic, Brushes.Black, (RectangleF)rectangle, GH_TextRenderingConstants.NearCenter);
                            y += height;
                        }
                        if (!string.IsNullOrEmpty(owner.Author.Copyright))
                        {
                            Rectangle rectangle = new Rectangle(x1, y, bitmap1.Width - x1, height);
                            g.DrawString(owner.Author.Copyright, GH_FontServer.StandardItalic, Brushes.Black, (RectangleF)rectangle, GH_TextRenderingConstants.NearCenter);
                            int num3 = y + height;
                        }
                        GH_GraphicsUtil.ShadowHorizontal(g, 0, bitmap1.Width, num2, Global_Proc.UiAdjust(6), false, 60);
                        g.DrawLine(Pens.Black, 0, num2, bitmap1.Width, num2);
                    }
                    e.Diagram = bitmap1;
                    break;
            }
        }

        private Bitmap DocumentDiagram()
        {
            ClusterTest owner = (ClusterTest)this.Owner;
            GH_Document ghDocument = this.ClusterDocument();
            Bitmap bitmap;
            if (ghDocument == null)
            {
                bitmap = (Bitmap)null;
            }
            else
            {
                List<IGH_DocumentObject> ghDocumentObjectList = new List<IGH_DocumentObject>();
                IEnumerator<IGH_DocumentObject> enumerator;
                try
                {
                    enumerator = ghDocument.Objects.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        IGH_DocumentObject current = enumerator.Current;
                        if (!(current is GH_ClusterInputHook) && !(current is GH_ClusterOutputHook) && current is IGH_ActiveObject)
                            ghDocumentObjectList.Add(current);
                    }
                }
                finally
                {
                    enumerator?.Dispose();
                }
                GH_DocDiagramPainter docDiagramPainter = new GH_DocDiagramPainter();
                docDiagramPainter.IgnoreSelectedStates = true;
                docDiagramPainter.PaintDiagram((IEnumerable<IGH_DocumentObject>)ghDocumentObjectList, Global_Proc.UiAdjust(400), Global_Proc.UiAdjust(150), 50);
                Bitmap image = docDiagramPainter.Image;
                if (image == null)
                {
                    bitmap = (Bitmap)null;
                }
                else
                {
                    Graphics.FromImage((Image)image).TextRenderingHint = GH_TextRenderingConstants.GH_CrispText;
                    if (owner.ProtectionLevel == GH_ClusterProtection.Protected)
                    {
                        GH_MemoryBitmap ghMemoryBitmap = new GH_MemoryBitmap(image);
                        ghMemoryBitmap.Filter_Blur(Global_Proc.UiAdjust(15));
                        ghMemoryBitmap.Release(true);
                    }
                    bitmap = image;
                }
            }
            return bitmap;
        }

        public override GH_ObjectResponse RespondToMouseDoubleClick(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            int modifierKeys = (int)Control.ModifierKeys;
            ((ClusterTest)this.Owner).EditClusterAsSeparateDocument();
            return GH_ObjectResponse.Handled;
        }

        private Rectangle WarningBounds
        {
            get
            {
                Rectangle rectangle = GH_Convert.ToRectangle(this.Bounds);
                return new Rectangle(rectangle.X + rectangle.Width / 2 - 8, rectangle.Bottom - 6, 16, 16);
            }
        }

        protected override void Render(GH_Canvas canvas, Graphics graphics, GH_CanvasChannel channel)
        {
            switch (channel)
            {
                case GH_CanvasChannel.Wires:
                    base.Render(canvas, graphics, channel);
                    break;
                case GH_CanvasChannel.Objects:
                    switch (((ClusterTest)this.Owner).Synchronisation)
                    {
                        case GH_Synchronisation.MissingReference:
                        case GH_Synchronisation.OutOfDate:
                            GH_PaletteStyle style = GH_Skin.palette_grey_standard;
                            if (this.Selected)
                                style = GH_Skin.palette_grey_selected;
                            GH_Capsule capsule = GH_Capsule.CreateCapsule(this.Bounds, GH_Palette.Black);
                            capsule.SetJaggedEdges(this.Owner.Params.Input.Count == 0, this.Owner.Params.Output.Count == 0);
                            List<IGH_Param>.Enumerator enumerator1;
                            try
                            {
                                enumerator1 = this.Owner.Params.Input.GetEnumerator();
                                while (enumerator1.MoveNext())
                                {
                                    IGH_Param current = enumerator1.Current;
                                    capsule.AddInputGrip(current.Attributes.InputGrip.Y);
                                }
                            }
                            finally
                            {
                                enumerator1.Dispose();
                            }
                            List<IGH_Param>.Enumerator enumerator2;
                            try
                            {
                                enumerator2 = this.Owner.Params.Output.GetEnumerator();
                                while (enumerator2.MoveNext())
                                {
                                    IGH_Param current = enumerator2.Current;
                                    capsule.AddOutputGrip(current.Attributes.OutputGrip.Y);
                                }
                            }
                            finally
                            {
                                enumerator2.Dispose();
                            }
                            capsule.RenderEngine.RenderGrips(graphics);
                            capsule.RenderEngine.RenderBackground(graphics, canvas.Viewport.Zoom, style);
                            if (GH_Canvas.ZoomFadeMedium > 5)
                            {
                                int key = 5 * GH_Canvas.ZoomFadeMedium / 5;
                                Bitmap bitmap;
                                if (GH_ClusterAttributes.Patterns.ContainsKey(key))
                                {
                                    bitmap = GH_ClusterAttributes.Patterns[key];
                                }
                                else
                                {
                                    bitmap = Res_GUI.StripePattern_48x48;
                                    GH_MemoryBitmap ghMemoryBitmap = new GH_MemoryBitmap(bitmap);
                                    ghMemoryBitmap.Filter_Multiply(GH_BitmapChannel.A, (double)key / (double)byte.MaxValue);
                                    ghMemoryBitmap.Release(true);
                                    GH_ClusterAttributes.Patterns.Add(key, bitmap);
                                }
                                TextureBrush textureBrush1 = new TextureBrush((Image)bitmap, WrapMode.Tile);
                                TextureBrush textureBrush2 = textureBrush1;
                                PointF pivot = this.Pivot;
                                double x = (double)pivot.X;
                                pivot = this.Pivot;
                                double y = (double)pivot.Y;
                                textureBrush2.TranslateTransform((float)x, (float)y);
                                textureBrush1.ScaleTransform(0.5f, 0.5f);
                                graphics.FillPath((Brush)textureBrush1, capsule.OutlineShape);
                                textureBrush1.Dispose();
                            }
                            if (GH_Canvas.ZoomFadeLow > 5)
                            {
                                PointF center;
                                ref PointF local = ref center;
                                RectangleF bounds = this.Bounds;
                                double x = (double)bounds.X;
                                bounds = this.Bounds;
                                double num1 = 0.5 * (double)bounds.Width;
                                double num2 = x + num1;
                                bounds = this.Bounds;
                                double y = (double)bounds.Y;
                                bounds = this.Bounds;
                                double num3 = 0.5 * (double)bounds.Height;
                                double num4 = y + num3;
                                local = new PointF((float)num2, (float)num4);
                                GH_GraphicsUtil.RenderWarningIcon(graphics, center, 16f, GH_Canvas.ZoomFadeLow);
                            }
                            capsule.RenderEngine.RenderHighlight(graphics);
                            capsule.RenderEngine.RenderOutlines(graphics, canvas.Viewport.Zoom, style);
                            return;
                        default:
                            base.Render(canvas, graphics, channel);
                            return;
                    }
            }
        }
}