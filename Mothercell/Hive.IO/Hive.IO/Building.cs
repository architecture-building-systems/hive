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
        /// Indicating adjacencies between zones. 
        /// First index is a sorted list of indices 0,1,2,...
        /// Second index is an array of indices that a zone is connected to.
        /// E.g. Adjacencies[0] = {1,2,3} means that zone 0 is connected to zones 1,2,3
        /// </summary>
        public int[][] Adjacencies { get; private set; }
        
        public ShadingDevice [] ShadingDevices { get; private set; }




    }
}
