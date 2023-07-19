using System;
using System.Collections.Generic;
using System.Windows.Media;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;

namespace Hive.IO.GhSolar
{
    public class GhTrimChecker : GH_Component
    {
        public GhTrimChecker()
            : base("Trim Checker", "TrimChecker",
                  "Checks if surfaces are trimmed or untrimmed",
                  "[hive]", "")
        {
        }

        public override GH_Exposure Exposure => GH_Exposure.secondary;

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Surface Input", "SI", "List of surfaces", GH_ParamAccess.list);
            //maybe Brep parameter?
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Surface Output", "O", "Checked list of surfaces", GH_ParamAccess.list);
            pManager.AddGenericParameter("Area Output", "A", "Area check", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            //List<Surface> surfaces = new List<Surface>();
            //if (!DA.GetDataList(0, surfaces)) return;

            var surfaces = new List<GH_Surface>();
            DA.GetDataList(0, surfaces);

            List<Interval> trims = new List<Interval>();
            List<bool> bools = new List<bool>();
            

            foreach (var s in surfaces)
            {
                //if (ghObj.Value is Surface surface)
                //meshList.Add(ghMesh.Value);
                //type = "Surface";
                //else if (ghObj.Value is Brep brep)
                //meshList.Add(mesh);
                //type = "Trimmed";

                foreach (var t in s.Value.Trims) { trims.Add(t.Domain); };

                double area1 = s.Boundingbox.Area;

                // Mesh mesh = Mesh.CreateFromBox(s.Boundingbox, 4, 4, 4);
                //double area2 = mesh.GetBoundingBox(true).Area;
                //areas.Add(area1 == area2);

                s.Untrim();

                double area2 = s.Boundingbox.Area;

                bools.Add(area1 == area2);
            }

            DA.SetDataList(0, trims);
            DA.SetDataList(1, bools);
        }

        //protected override System.Drawing.Bitmap Icon => Properties.Resources.Solar_ST_Simple;

        public override Guid ComponentGuid => new Guid("63f4af2f-c75c-4f33-bfa1-46fba385ebb8");
    }
}

