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
        #region MainProperties
        /// <summary>
        /// The actual zone geometry, as rhino Brep
        /// </summary>
        public rg.Brep Geometry { get; private set; }
        /// <summary>
        /// Unique index, used to identify the zone when it is part of a Building object
        /// </summary>
        public int Index { get; private set; }


        /// <summary>
        /// Internal loads.
        /// Values indicate full / maximal value.
        /// Unit in W/sqm
        /// </summary>
        public struct InternalLoads
        {
            public double Occupants;
            public double Devices;
            public double Lighting;
        }
        /// <summary>
        /// Time-resolved schedules, values are [0, 1].
        /// Value of 1 indicates full load as defined in this.InternalLoads
        /// </summary>
        public struct Schedules
        {
            public double[] Occupants;
            public double[] Devices;
            public double[] Lighting;
        }


        /// <summary>
        /// fix the horizon to one year, hourly
        /// </summary>
        private const int _horizon = 8760;
        #endregion

        #region BuildingComponents
        /// <summary>
        /// Wall components of this zone. Cannot be empty.
        /// </summary>
        public Wall[] Walls { get; private set; }
        /// <summary>
        /// Ceiling components of this zone. Cannot be empty.
        /// </summary>
        public Ceiling[] Ceilings { get; private set; }
        /// <summary>
        /// Floor components of this zone. Cannot be empty. A void would also be a floor, but with material property 'air' or something
        /// </summary>
        public Floor[] Floors { get; private set; }
        /// <summary>
        /// Apertures of this zone, e.g. windows, skylights, doors, ventilation openings, etc.. Can be empty.
        /// </summary>
        public Opening[] Openings { get; private set; }
        /// <summary>
        /// Roof components of this zone. Can be empty.
        /// </summary>
        public Roof[] Roofs { get; private set; }
        #endregion

        #region Boolean properties
        /// <summary>
        /// For simplicity of thermal calculations, avoid curves etc., only accept linear floorplans and geometries
        /// </summary>
        public bool IsLinear { get; private set; }
        /// <summary>
        /// For simplicity of thermal calculations, only accept convex zones
        /// </summary>
        public bool IsConvex { get; private set; }
        /// <summary>
        /// Zone geometry must be a closed Brep, since it defines a thermal space
        /// </summary>
        public bool IsClosed { get; private set; }
        /// <summary>
        /// Main bool, if this is false, then no thermal simulations can be done
        /// </summary>
        public bool IsValid { get; private set; }
        #endregion



        #region Constructor
        /// <summary>
        /// Create an instance of a Zone object
        /// </summary>
        /// <param name="geometry">Brep geometry. Must be closed, linear and convex.</param>
        /// <param name="index">Unique identifier</param>
        public Zone(rg.Brep geometry, int index)
        {
            this.Index = index;

            this.Geometry = geometry;
            this.IsLinear = CheckLinearity(this.Geometry);
            if (this.IsLinear)
            {
                this.IsConvex = CheckConvexity(this.Geometry);
                this.IsClosed = CheckClosedness(this.Geometry);
            }
            else
            {
                // these might still be true, but let's set to false to save unnecessary computation
                this.IsConvex = false;
                this.IsClosed = false;
            }
            this.IsValid = CheckValidity(this.IsClosed, this.IsConvex, this.IsLinear);

            // define standard constructions?

        }
        #endregion


        #region internalMethods
        /// <summary>
        /// Check the linearity of the brep zone geometry. No curves allowed for simplicity.
        /// </summary>
        /// <param name="brep"></param>
        /// <returns>True if all is linear</returns>
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

        /// <summary>
        /// Check the convexity of the zone. Only convex spaces allowed for simplicity.
        /// </summary>
        /// <param name="brep"></param>
        /// <returns>True, if convex</returns>
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

                    // test
                }
            }
            return isConvex;
        }

        /// <summary>
        /// Check the closedness of the brep. No open breps allowed, since a the brep is used to define a thermal space.
        /// </summary>
        /// <param name="brep"></param>
        /// <returns>True, if it is closed</returns>
        private bool CheckClosedness(rg.Brep brep)
        {
            return brep.IsSolid;
        }

        /// <summary>
        /// Check if all three conditions (Closedness, Convexity, Linearity) are fulffilled.
        /// </summary>
        /// <returns>True, if zone geometry is valid</returns>
        private bool CheckValidity(bool closed, bool convex, bool linear)
        {
            if (convex && closed && linear)
                return true;
            else
                return false;
        }
        #endregion
    }

}
