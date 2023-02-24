using Grasshopper.Kernel.Special;
using Grasshopper.Kernel;
using Hive.IO.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Grasshopper.Kernel.Types;

namespace Hive.IO.GhSolar
{
    public class GhTreeGenerator : GH_Component
    {
        public GhTreeGenerator() : base("Tree Generator", "TreeGenerator", "Generates random boxes as input for the Tree Schedules", "[hive]", "Solar")
        {
        }

        public override GH_Exposure Exposure => GH_Exposure.secondary;

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Base Area Surface", "BaseArea", "Base Area surface in which trees are generated", GH_ParamAccess.item);
            pManager.AddGenericParameter("Tree Number", "TreeNumber", "Number of trees to be generated", GH_ParamAccess.item);
            pManager.AddNumberParameter("X Domain Start", "XDomainStart", "Start of the random value domain in X direction", GH_ParamAccess.item);
            pManager.AddNumberParameter("X Domain End", "XDomainEnd", "End of the random value domain in X direction", GH_ParamAccess.item);
            pManager.AddNumberParameter("Y Domain Start", "YDomainStart", "Start of the random value domain in Y direction", GH_ParamAccess.item);
            pManager.AddNumberParameter("Y Domain End", "YDomainStart", "End of the random value domain in Y direction", GH_ParamAccess.item);
            pManager.AddNumberParameter("Z Domain Start", "ZDomainStart", "Start of the random value domain in Z direction", GH_ParamAccess.item);
            pManager.AddNumberParameter("Z Domain End", "ZDomainEnd", "End of the random value domain in Z direction", GH_ParamAccess.item);
            pManager.AddGenericParameter("Random value seed", "RandomSeed", "Optional seed value for random number generation", GH_ParamAccess.item);

            pManager[8].Optional = true;
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Tree Breps", "TreeBreps", "List of generated tree Breps", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            GH_Surface surface = new GH_Surface();
            if (!DA.GetData(0, ref surface)) return;

            int treeNumber = 0;
            if (!DA.GetData(1, ref treeNumber)) return;

            double xStart = 0;
            if (!DA.GetData(2, ref xStart)) return;
            double xEnd = 0;
            if (!DA.GetData(3, ref xEnd)) return;
            double yStart = 0;
            if (!DA.GetData(4, ref yStart)) return;
            double yEnd = 0;
            if (!DA.GetData(5, ref yEnd)) return;
            double zStart = 0;
            if (!DA.GetData(6, ref zStart)) return;
            double zEnd = 0;
            if (!DA.GetData(7, ref zEnd)) return;

            int randSeed = 0;
            if (!DA.GetData(8, ref randSeed)) return;


        }

        public override Guid ComponentGuid => new Guid("c2434e20-9254-4aac-a6a9-e0574efe9827");
    }
}

