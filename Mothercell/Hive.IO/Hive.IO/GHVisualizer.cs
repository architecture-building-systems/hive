using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Attributes;

namespace Hive.IO
{
    public class GHVisualizer : GH_Component
    {
        private readonly Grasshopper.Kernel.Parameters.Param_GenericObject m_results;

        public GHVisualizer() : base("Hive.IO.Visualizer", "Hive.IO.Visualizer",
              "Hive Visualizer for simulation results",
              "[hive]", "IO")
        {
            Rhino.RhinoApp.WriteLine("hello, world!");
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

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {            
            pManager.AddGenericParameter("Results", 
                "Results", 
                "Simulation results dictionary", 
                GH_ParamAccess.list);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
        }
    }

    public class GHVisualizerAttributes : GH_ComponentAttributes
    {
        public GHVisualizerAttributes(GHVisualizer owner) : base(owner)
        {
        }

        protected override void Layout()
        {
            // Bounds = new RectangleF(Pivot, new SizeF(500, 600));
            base.Layout();
        }

        protected override void Render(GH_Canvas canvas, Graphics graphics, GH_CanvasChannel channel)
        {
            // Render all the wires that connect the Owner to all its Sources.
            if (channel == GH_CanvasChannel.Wires)
            {
                //RenderIncomingWires(canvas.Painter, Owner.Sources, Owner.WireDisplay);
                base.Render(canvas, graphics, channel);
                return;
            }

            // Render the parameter capsule and any additional text on top of it.
            if (channel == GH_CanvasChannel.Objects)
            {
                // GH_Capsule capsule = GH_Capsule.CreateCapsule(Bounds, palette);
                base.Render(canvas, graphics, channel);
                return;
            }

            // Render an image??
            if (channel == GH_CanvasChannel.Overlay)
            {
                base.Render(canvas, graphics, channel);
                return;
            }

        }
    }
}
