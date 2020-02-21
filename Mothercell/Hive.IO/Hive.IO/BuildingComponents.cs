using rg = Rhino.Geometry;

namespace Hive.IO
{
    /// <summary>
    /// Building components indicate adjacencies to other components, surface areas, flags like external or internal, cost and emissions
    /// </summary>
    namespace BuildingComponents
    {
        public abstract class Component
        {
            /// <summary>
            /// Rhino Geometry of this component
            /// </summary>
            public rg.Brep Geometry { get; private set; }
            /// <summary>
            /// Unique identifier
            /// </summary>
            public int Index { get; private set; }
            /// <summary>
            /// Affiliation to Zone Index.
            /// Index that serves as zone identifier. E.g. ZoneIdentifier = 0 means that this Component belongs to Zone 0
            /// Components without zone affiliation can't exist. Otherwise, they would be an obstacle object in Environment.cs
            /// </summary>
            public int ZoneIdentifier { get; private set; }
            /// <summary>
            /// Flag for external, i.e. access to solar radiation and ambient environment
            /// </summary>
            public bool IsExternal { get; private set; }
            /// <summary>
            /// Total Cost in [Currency]
            /// </summary>
            public double Cost { get; private set; }
            /// <summary>
            /// Total CO2 emissions [kgCO2eq.]
            /// </summary>
            public double CO2 { get; private set; }
            /// <summary>
            /// Total surface area of this component, in [sqm]
            /// </summary>
            public double Area { get; private set; }
            /// <summary>
            /// Total weight of the entire component, in [kg]
            /// </summary>
            public double Weight { get; private set; }
            /// <summary>
            /// Wind pressure coefficient, C_p, of this component
            /// </summary>
            public double WindPressureCoefficient { get; private set; }

            /// <summary>
            /// Indices of adjacent components
            /// </summary>
            public int[] AdjacentComponents { get; private set; }
            /// <summary>
            /// Congruent area of respective adjacent component, in [sqm]
            /// </summary>
            public double[] CongruentArea { get; private set; }
        }




        /// <summary>
        /// Openings on building hull, e.g. windows or doors. Could be opaque or transparent.
        /// </summary>
        public class Opening : Component
        {
            // Should also contain information for dynamic shading
            // static shading is defined as static shading object

        }


        /// <summary>
        /// Internal or external wall element
        /// </summary>
        public class Wall : Component
        {
            // Wall, Roof, Floor, Ceiling are not input manually. But they need to be own classes, because they'll contain information like construction.
        }


        /// <summary>
        /// Roof. Always external
        /// </summary>
        public class Roof : Component
        {

        }


        /// <summary>
        /// Ceiling, i.e. internal surface
        /// </summary>
        public class Ceiling : Component
        {

        }


        /// <summary>
        /// Floor, i.e. internal surface
        /// </summary>
        public class Floor : Component
        {

        }



        public class Shading : Component
        {

        }

        /// <summary>
        /// Dynamic shading object, like louvers. Adjacent buildings are part of 'Environment.cs'
        /// Can be mesh or brep.
        /// </summary>
        public class DynamicShading : Component
        {


            /// <summary>
            /// indicating whether this shading device is dynamic, meaning it can move, like a louver. if false, it is static
            /// Furthermore, if true, it needs rg.Mesh [] DynamicGeometry that defines the different geometry states.
            /// If false, base.Geometry is enough
            /// </summary>
            public bool IsDynamic { get; private set; }

            /// <summary>
            /// Dynamic geometry as mesh. 
            /// Is an array, since it may contain different states, but only when (this.IsDynamic == true), otherwise this.ShadingGeometry.Length = 1
            /// It get's its normal state from base.Geometry
            /// </summary>
            public rg.Mesh[] DynamicGeometry { get; private set; }



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






        }





    }
}
