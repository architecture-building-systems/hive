using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hive.IO.EnergySystems
{
    /// <summary>
    /// Storage systems
    /// E.g. batteries, hot water tanks, ice storage, ...
    /// </summary>
    public abstract class Storage
    {
        public enum TechnologyNames
        {
            Battery,
            WaterTank,
            IceStorage,
            GroundStorage,
            Borehole
        }

    }
}
