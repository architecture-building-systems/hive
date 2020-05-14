using System;
using Grasshopper.Kernel;

namespace Hive.GUI
{
    public class DrawingComponent : GH_Component
    {
        public DrawingComponent()
          : base("DrawingComponent",
                 "DComp",
                 "GUI testing 3",
                 "[hive]",
                 "GUI")
        {
        }

        public override void CreateAttributes()
        {
            m_attributes = new DrawingComponentCustom(this);
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("in1", "in1", "in1", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("out1", "out1", "out1", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Icons._2;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("98930c09-1152-4241-a80a-fda44a295be4"); }
        }
    }
}