using System;
using Grasshopper.Kernel;

namespace Hive.GUI
{
    public class HiveGUIComponent : GH_Component
    {
        public HiveGUIComponent()
          : base("Hive.GUI", "HiveGUI",
              "GUI testing",
              "[hive]", "GUI")
        {
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("xx", "xx", "xx", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("xx", "xx", "xx", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return null;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("42fc54f3-90ad-4d11-8104-44f42521a265"); }
        }
    }
}
