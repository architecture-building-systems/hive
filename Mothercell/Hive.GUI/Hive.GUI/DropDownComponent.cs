using System;
using Grasshopper.Kernel;

namespace Hive.GUI
{
    public class DropDownComponent : GH_Component
    {
        public DropDownComponent()
          : base("DropDownComponent",
                 "DDComp",
                 "GUI testing 2",
                 "[hive]",
                 "GUI")
        {
        }

        public override void CreateAttributes()
        {
            m_attributes = new DropDownComponentCustom(this);
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
                return Icons._1;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("a198af1f-b633-48d6-aaa2-ae9ff307ffe5"); }
        }
    }
}