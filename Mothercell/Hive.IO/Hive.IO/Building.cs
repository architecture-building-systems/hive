using System.Collections.Generic;
using rg = Rhino.Geometry;
using Hive.IO.BuildingComponents;

namespace Hive.IO
{
    /// <summary>
    /// Namespace for Building geometry and construction properties.
    /// Adjacencies to other zones, determination wether a wall is internal or external, etc, are determined within Hive.Mothercell, in the Hive.IO.Distributor component.
    /// </summary>
    public class Building
    {
        /// <summary>
        /// Building Zones
        /// </summary>
        public Zone[] Zones { get; private set; }

        /// <summary>
        /// Indicating adjacencies between zones. 
        /// First index is a sorted list of indices 0,1,2,...
        /// Second index is an array of indices that a zone is connected to.
        /// E.g. Adjacencies[0] = {1,2,3} means that zone 0 is connected to zones 1,2,3
        /// </summary>
        public int[][] Adjacencies { get; private set; }

        /// <summary>
        /// Describes the building type, e.g. residential, office, school, etc.
        /// </summary>
        public BuildingType Type { get; private set; }

        /// <summary>
        /// Building properties, such as U-values, infiltration, etc., according to SIA2024
        /// </summary>
        public Dictionary<string, object> SIA2024 { get; private set; }


        public Building(Zone[] zones, BuildingType type)
        {
            this.Zones = zones;
            this.Type = type;

            // Hive 0.2
            //this.Adjacencies = SolveAdjacencies(this.Zones);
        }


        /// <summary>
        /// Sets the SIA2024 table. Optional.
        /// </summary>
        /// <param name="sia2024"></param>
        public void SetSIA2024(Dictionary<string, object> sia2024)
        {
            this.SIA2024 = sia2024;
        }


        //// Hive 0.2
        ///// <summary>
        ///// Solving zone adjacencies
        ///// </summary>
        ///// <param name="zones"></param>
        ///// <param name="adjacencies"></param>
        //private int [][] SolveAdjacencies(Zone[] zones)
        //{

        //    return new int[zones.Length][];

        //    // where are congruent surfaces solved? here?
        //    // must be, because in the class 'Zone', there is no global knowledge yet about surrounding zones

        //    // the original brep needs to split into subsurfaces, according to congruencies with adjacent zones
        //    // i.e. if it was a box with only 4 wall surfaces, but there is a partly congruent wall that is
        //    //  e.g. half external and half in contact with another surface, then this 1 surface needs to be split in 2 
        //    //  and this.Zones[i].Geometry updated

        //}
    }


    /// <summary>
    /// Building types
    /// </summary>
    public enum BuildingType
    {
        Residential,
        Office,
        Industry,
        Laboratory,
        Supermarket,
        School,
        Undefined
    }
}
