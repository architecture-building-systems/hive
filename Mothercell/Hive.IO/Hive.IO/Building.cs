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


        /// <summary>
        /// Static shading object, like louvers. Adjacent buildings are part of 'Environment.cs'
        /// Can be mesh or brep.
        /// </summary>
        public struct ShadingDevice
        {
            /// <summary>
            /// indicating whether this shading device is dynamic, meaning it can move, like a louver. if false, it is static
            /// </summary>
            public bool IsDynamic { get; private set; }

            /// <summary>
            /// Indicating whether this shading device is internal, i.e. inside the zone.
            /// </summary>
            public bool IsInternal { get; private set; }

            /// <summary>
            /// Absorptivity
            /// </summary>
            public double Absorbtivity { get; private set; }
            /// <summary>
            /// Reflectivity
            /// </summary>
            public double Reflectivity { get; private set; }
            /// <summary>
            /// Transmissivity
            /// </summary>
            public double Transmissivity { get; private set; }
            /// <summary>
            /// Shading geometry as mesh. 
            /// Is an array, since it may contain different states, but only when (this.IsDynamic == true), otherwise this.ShadingGeometry.Length = 1
            /// </summary>
            public rg.Mesh[] ShadingGeometry { get; private set; }

            /// <summary>
            /// Time horizon for the schedule
            /// </summary>
            public int Horizon { get; private set; }

            /// <summary>
            /// Time-resolved geometry schedule of the shading device. Only necessary if (this.IsDynamic == true)
            /// It refers to the indices in this.ShadingGeometry.
            /// E.g. this.GeometrySchedule[0] = 1 means that at timestep 0, geometry state 1 is active
            /// </summary>
            public int[] GeometrySchedule { get; private set; }

            /// <summary>
            /// Time-resolved transmissivity schedule [0,1].
            /// When this.TransmissivitySchedule[timestep] = 1, then the transmissivity of the shading device at that timestep equals this.Transmissivity.
            /// Only necessary if (this.IsDynamic == true)
            /// </summary>
            public double[] TransmissivitySchedule { get; private set; }
            public double[] AbsorbtivitySchedule { get; private set; }
            public double[] ReflectivitySchedule { get; private set; }

            /// <summary>
            /// Index that serves as zone identifier. E.g. ZoneIdentifier = 0 means that this ShadingDevice belongs to Zone 0
            /// Shading device without zone can't exist. Otherwise, it would belong to Environment.cs as obstacle object
            /// </summary>
            public int ZoneIdentifier { get; private set; }





        }
    }
}
