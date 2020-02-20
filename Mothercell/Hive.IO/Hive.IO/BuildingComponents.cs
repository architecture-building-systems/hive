using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using rg = Rhino.Geometry;

namespace Hive.IO
{
    /// <summary>
    /// Building components indicate adjacencies to other components, surface areas, flags like external or internal, cost and emissions
    /// </summary>
    namespace BuildingComponents
    {
        public abstract class BuildingComponent
        {
            /// <summary>
            /// Rhino Geometry of this component
            /// </summary>
            public rg.Brep Geometry;
            /// <summary>
            /// Unique identifier
            /// </summary>
            public int Index;
            /// <summary>
            /// Affiliation to Zone Index
            /// </summary>
            public int ZoneIdentifier;
            /// <summary>
            /// Flag for external, i.e. access to solar radiation and ambient environment
            /// </summary>
            public bool IsExternal;
            /// <summary>
            /// Total Cost in [Currency]
            /// </summary>
            public double Cost;
            /// <summary>
            /// Total CO2 emissions [kgCO2eq.]
            /// </summary>
            public double CO2;
            /// <summary>
            /// Total surface area of this component, in [sqm]
            /// </summary>
            public double Area;
            /// <summary>
            /// Total weight of the entire component, in [kg]
            /// </summary>
            public double Weight;
            /// <summary>
            /// U-Value of this component
            /// </summary>
            public double UValue;
            /// <summary>
            /// Wind pressure coefficient, C_p, of this component
            /// </summary>
            public double WindPressureCoefficient;

            /// <summary>
            /// Indices of adjacent components
            /// </summary>
            public int[] AdjacentComponents;
            /// <summary>
            /// Congruent area of respective adjacent component, in [sqm]
            /// </summary>
            public double[] CongruentArea;
        }



        /// <summary>
        /// Openings on building hull, e.g. windows or doors. Could be opaque or transparent.
        /// </summary>
        public class Opening : BuildingComponent
        {
            // Should also contain information for dynamic shading
            // static shading is defined as static shading object

        }


        /// <summary>
        /// Internal or external wall element
        /// </summary>
        public class Wall : BuildingComponent
        {
            // Wall, Roof, Floor, Ceiling are not input manually. But they need to be own classes, because they'll contain information like construction.
        }


        /// <summary>
        /// Roof. Always external
        /// </summary>
        public class Roof : BuildingComponent
        {

        }


        /// <summary>
        /// Ceiling, i.e. internal surface
        /// </summary>
        public class Ceiling : BuildingComponent
        {

        }


        /// <summary>
        /// Floor, i.e. internal surface
        /// </summary>
        public class Floor : BuildingComponent
        {

        }




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
