using rg = Rhino.Geometry;

namespace Hive.IO
{
    /// <summary>
    /// Static shading object, like louvers. Adjacent buildings are part of 'Environment.cs'
    /// Can be mesh or brep.
    /// </summary>
    public class ShadingDevice
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
        public rg.Mesh [] ShadingGeometry { get; private set; }

        /// <summary>
        /// Time horizon for the schedule
        /// </summary>
        public int Horizon { get; private set; }

        /// <summary>
        /// Time-resolved geometry schedule of the shading device. Only necessary if (this.IsDynamic == true)
        /// It refers to the indices in this.ShadingGeometry.
        /// E.g. this.GeometrySchedule[0] = 1 means that at timestep 0, geometry state 1 is active
        /// </summary>
        public int [] GeometrySchedule { get; private set; }

        /// <summary>
        /// Time-resolved transmissivity schedule [0,1].
        /// When this.TransmissivitySchedule[timestep] = 1, then the transmissivity of the shading device at that timestep equals this.Transmissivity.
        /// Only necessary if (this.IsDynamic == true)
        /// </summary>
        public double [] TransmissivitySchedule { get; private set; }
        public double [] AbsorbtivitySchedule { get; private set; }
        public double [] ReflectivitySchedule { get; private set; }

        /// <summary>
        /// Index that serves as zone identifier. E.g. ZoneIdentifier = 0 means that this ShadingDevice belongs to Zone 0
        /// Shading device without zone can't exist. Otherwise, it would belong to Environment.cs as obstacle object
        /// </summary>
        public int ZoneIdentifier { get; private set; }



        public ShadingDevice(rg.Mesh [] shadingGeometry, int zoneIdentifier)
        {
            ShadingGeometry = shadingGeometry;
            ZoneIdentifier = zoneIdentifier;

            // initialize with standard properties
            Reflectivity = 0.1;
            Transmissivity = 0.1;
            Absorbtivity = 0.1;
            IsDynamic = false;

        }


    }
}
