using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using rg = Rhino.Geometry;
using Hive.IO.BuildingComponents;


namespace Hive.IO
{

    /// <summary>
    /// Thermal Zone.
    /// Geometry must be (1) Brep, (2) closed, (3) convex, and (4) not contain any curves, i.e. lines only.
    /// </summary>
    public class Zone
    {
        public Wall[] Walls { get; private set; }
        public Roof[] Roofs { get; private set; }
        public Ceiling[] Ceilings { get; private set; }
        public Floor[] Floors { get; private set; }
        public Opening[] Openings { get; private set; }
        public rg.Brep ZoneGeometry { get; private set; }
        public bool IsLinear { get; private set; }
        public bool IsConvex { get; private set; }
        public bool IsClosed { get; private set; }
        public int Index { get; private set; }


        public Zone(rg.Brep zoneGeometry)
        {
            ZoneGeometry = zoneGeometry;
            IsLinear = CheckLinearity(ZoneGeometry);
            if (IsLinear)
            {
                IsConvex = CheckConvexity(ZoneGeometry);
                IsClosed = CheckClosedness(ZoneGeometry);
            }
            else
            {
                // these might still be true, but let's set to false to save unnecessary computation
                IsConvex = false;
                IsClosed = false;
            }
        }


        private bool CheckLinearity(rg.Brep brep)
        {
            bool isLinear = true;
            foreach (rg.BrepEdge edge in brep.Edges)
            {
                if (edge.IsLinear() == false)
                {
                    isLinear = false;
                    break;
                }
            }
            return isLinear;
        }


        private bool CheckConvexity(rg.Brep brep)
        {
            bool isConvex = true;

            int vertexCount = brep.Vertices.Count;

            for (int i = 0; i < vertexCount; i++)
            {
                rg.BrepVertex vert1 = brep.Vertices[i];
                for (int u = 0; u < vertexCount; u++)
                {
                    if (i == u) continue;

                    rg.BrepVertex vert2 = brep.Vertices[u];
                    rg.Line line = new rg.Line(vert1.Location, vert2.Location);
                    //if((rg.Intersect.Intersection.CurveBrep(line, brep,0.01,0.01,)) 
                    //{
                    //    isConvex = false;
                    //}
                    // !!! connect point i to u and check, if this line is within the brep
                }
            }
            return isConvex;
        }


        private bool CheckClosedness(rg.Brep brep)
        {
            return brep.IsSolid;
        }
    }

}
