using System.Collections.Generic;
using rg = Rhino.Geometry;

namespace Hive.IO
{
    /// <summary>
    /// Namespace for Building geometry and construction properties.
    /// Adjacencies to other zones, determination wether a wall is internal or external, etc, are determined within Hive.Mothercell, in the Hive.IO.Distributor component.
    /// </summary>
    public class Building
    {
        /// <summary>
        /// Indicating adjacencies between zones. 
        /// First index is a sorted list of indices 0,1,2,...
        /// Second index is an array of indices that a zone is connected to.
        /// E.g. Adjacencies[0] = {1,2,3} means that zone 0 is connected to zones 1,2,3
        /// </summary>
        public int[][] Adjacencies { get; private set; }
        
        public ShadingDevice [] ShadingDevices { get; private set; }



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
                    for (int u=0; u<vertexCount; u++)
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


        /// <summary>
        /// Openings on building hull, e.g. windows or doors. Could be opaque or transparent.
        /// </summary>
        public struct Opening
        {
            // Should also contain information for dynamic shading
            // static shading is defined as static shading object

        }


        /// <summary>
        /// Internal or external wall element
        /// </summary>
        public struct Wall
        {
            // Wall, Roof, Floor, Ceiling are not input manually. But they need to be own classes, because they'll contain information like construction.
        }


        /// <summary>
        /// Roof. Always external
        /// </summary>
        public struct Roof
        {

        }


        /// <summary>
        /// Ceiling, i.e. internal surface
        /// </summary>
        public struct Ceiling
        {

        }


        /// <summary>
        /// Floor, i.e. internal surface
        /// </summary>
        public struct Floor
        {

        }
    }
}
