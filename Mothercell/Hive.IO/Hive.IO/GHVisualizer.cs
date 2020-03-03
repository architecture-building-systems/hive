using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Grasshopper.Kernel.Attributes;
using System.Windows.Forms;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.WindowsForms;

namespace Hive.IO
{
    public class GHVisualizer : GH_Param<GH_ObjectWrapper>
    {
        private readonly GH_ObjectWrapper m_results;

        public GHVisualizer() : base("Hive.IO.Visualizer", "Hive.IO.Visualizer",
              "Hive Visualizer for simulation results",
              "[hive]", "IO", GH_ParamAccess.item)
        {
        }

        public override GH_ParamKind Kind
        {
            get
            {
                return GH_ParamKind.floating;
            }
        }

        public override string TypeName
        {
            get
            {
                return "HiveResults";
            }
        }

        public override GH_Exposure Exposure
        {
            get
            {
                return GH_Exposure.primary;
            }
        }


        public override void CreateAttributes()
        {
            m_attributes = new GHVisualizerAttributes(this);
        }

        public override Guid ComponentGuid
        {
            get
            {
                return new Guid("7b4ece55-07a0-4e87-815a-e3724a1317b1");
            }
        }
    }

    public class GHVisualizerAttributes : GH_ResizableAttributes<GHVisualizer>
    {
        private int m_padding = 6;

        public GHVisualizerAttributes(GHVisualizer owner) : base(owner)
        {
        }

        public override string PathName
        {
            get
            {
                // FIXME: what goes here?
                return "PathName_GHVisualizer";
            }
        }

        protected override Size MinimumSize
        {
            get
            {
                return new Size(50, 50);
            }
        }

        protected override Padding SizingBorders => new Padding(this.m_padding);

        protected override void Layout()
        {
            // make sure we have a minimum size
            var minWidth = 200;
            var minHeight = 150;

            var bounds = this.Bounds;
            bounds.Width = Math.Max(bounds.Width, minWidth);
            bounds.Height = Math.Max(bounds.Height, minHeight);
            this.Bounds = bounds;

            Rhino.RhinoApp.WriteLine(string.Format("f<Layout {0} x {1}", bounds.Width, bounds.Height));
        }

        private RectangleF PlotBounds 
        {
            get 
            {
                var plotBounds = this.Bounds;
                plotBounds.Inflate(-m_padding, -m_padding);
                return plotBounds;
            }
        }

        private PointF PlotLocation
        {
            get
            {
                var plotLocation = this.Bounds.Location;
                plotLocation.X += m_padding;
                plotLocation.Y += m_padding;
                return plotLocation;
            }
        }

        protected override void Render(GH_Canvas canvas, Graphics graphics, GH_CanvasChannel channel)
        {
            if (channel == GH_CanvasChannel.Wires && this.Owner.SourceCount > 0)
                this.RenderIncomingWires(canvas.Painter, (IEnumerable<IGH_Param>)this.Owner.Sources, this.Owner.WireDisplay);
            if (channel != GH_CanvasChannel.Objects)
                return;

            GH_Viewport viewport = canvas.Viewport;
            RectangleF bounds = this.Bounds;
            GH_Capsule capsule = this.Owner.RuntimeMessageLevel != GH_RuntimeMessageLevel.Error ? GH_Capsule.CreateCapsule(this.Bounds, GH_Palette.Hidden, 5, 30) : GH_Capsule.CreateCapsule(this.Bounds, GH_Palette.Error, 5, 30);
            capsule.SetJaggedEdges(false, true);
            capsule.AddInputGrip(this.InputGrip);
            capsule.Render(graphics, this.Selected, this.Owner.Locked, true);
            capsule.Dispose();

            // FIXME: Figure this out from the inputs
            var myModel = new PlotModel { Title = "Example 1" };
            myModel.Series.Add(new FunctionSeries(Math.Cos, 0, 10, 0.1, "cos(x)"));
            var pngExporter = new PngExporter { Width = (int) this.PlotBounds.Width, Height = (int) this.PlotBounds.Height, Background = OxyColors.White };
            var bitmap = pngExporter.ExportToBitmap(myModel);
            graphics.DrawImage(bitmap, this.PlotBounds.Location);


            Rhino.RhinoApp.WriteLine("Completed DrawImage");
            Rhino.RhinoApp.WriteLine("In Render");

        }
    }
}
