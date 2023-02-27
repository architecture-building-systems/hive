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
using Rhino.Geometry;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;

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
            pManager.AddGenericParameter("Tree Points", "TreePoints", "Starting points for tree generation", GH_ParamAccess.list);
            pManager.AddNumberParameter("X Domain Start", "XDomainStart", "Start of the random value domain in X direction", GH_ParamAccess.item);
            pManager.AddNumberParameter("X Domain End", "XDomainEnd", "End of the random value domain in X direction", GH_ParamAccess.item);
            pManager.AddNumberParameter("Y Domain Start", "YDomainStart", "Start of the random value domain in Y direction", GH_ParamAccess.item);
            pManager.AddNumberParameter("Y Domain End", "YDomainEnd", "End of the random value domain in Y direction", GH_ParamAccess.item);
            pManager.AddNumberParameter("Z Domain Start", "ZDomainStart", "Start of the random value domain in Z direction", GH_ParamAccess.item);
            pManager.AddNumberParameter("Z Domain End", "ZDomainEnd", "End of the random value domain in Z direction", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Random value seed", "RandomSeed", "Optional seed value for random number generation", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Tree Boxes", "TreeBoxes", "List of generated tree boxes", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Point3d> treePoints = new List<Point3d>();
            if (!DA.GetDataList(0, treePoints)) return;

            double xStart = 0.0;
            if (!DA.GetData(1, ref xStart)) return;
            double xEnd = 0.0;
            if (!DA.GetData(2, ref xEnd)) return;
            double yStart = 0.0;
            if (!DA.GetData(3, ref yStart)) return;
            double yEnd = 0.0;
            if (!DA.GetData(4, ref yEnd)) return;
            double zStart = 0.0;
            if (!DA.GetData(5, ref zStart)) return;
            double zEnd = 0.0;
            if (!DA.GetData(6, ref zEnd)) return;

            int randSeed = 0;
            if (!DA.GetData(7, ref randSeed)) return;

            Random rnd = new Random(randSeed);

            var treeBreps = new List<Box>();

            double NextDouble(Random rng, double min, double max)
            {
                return min + (rng.NextDouble() * (max - min));
            }

            for (int i = 0; i < treePoints.Count; i++)
            {
                var xRand = NextDouble(rnd, xStart, xEnd);
                var yRand = NextDouble(rnd, yStart, yEnd);
                var zRand = NextDouble(rnd, zStart, zEnd);

                Vector3d randVector = new Vector3d(xRand, yRand, zRand);

                var endPoint = treePoints[i] + randVector;

                treeBreps.Add(new Box(new BoundingBox(treePoints[i], endPoint)));
            }

            DA.SetDataList(0, treeBreps);
        }

        

        public override Guid ComponentGuid => new Guid("c2434e20-9254-4aac-a6a9-e0574efe9827");
    }
}

