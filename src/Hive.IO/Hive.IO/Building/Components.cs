using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using rg = Rhino.Geometry;

namespace Hive.IO.Building
{
    /// <summary>
    /// Building components indicate adjacencies to other components, surface areas, flags like external or internal, cost and emissions
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public abstract class Component
    {
        /// <summary>
        /// Surface Components that are on this component. E.g. a window as part of a wall. Important to know for Area calculation.
        /// </summary>
        [JsonProperty]
        public List<Component> SubComponents { get; internal set; }


        /// <summary>
        /// Rhino BrepFace of this component
        /// </summary>
        [JsonProperty]
        public rg.Brep BrepGeometry { get; private set; }

        /// <summary>
        /// Unique identifier
        /// </summary>
        [JsonProperty]
        public int Index { get; private set; }
        /// <summary>
        /// Affiliation to Zone Index.
        /// Index that serves as zone identifier. E.g. ZoneIdentifier = 0 means that this Component belongs to Zone 0
        /// Components without zone affiliation can't exist. Otherwise, they would be an obstacle object in Environment.cs
        /// </summary>
        [JsonProperty]
        public int ZoneIdentifier { get; private set; }
        /// <summary>
        /// Flag for external, i.e. access to solar radiation and ambient environment
        /// </summary>
        [JsonProperty]
        public bool IsExternal { get; private set; }
        /// <summary>
        /// Total Cost in [Currency]
        /// </summary>
        [JsonProperty]
        public double SpecificCost { get; protected set; }

        private double? _totalCost = null;

        [JsonProperty]
        public double TotalCost
        {
            get
            {
                if (_totalCost == null)
                    _totalCost = this.SpecificCost * this.Area;

                return (double) _totalCost;
            }
            private set => _totalCost = value;
        }

        /// <summary>
        /// Specific CO2 emissions [kgCO2eq./m^2]
        /// </summary>
        [JsonProperty]
        public double SpecificCo2 { get; protected set; }

        private double? _totalCo2 = null;

        [JsonProperty]
        public double TotalCo2
        {
            get
            {
                if (_totalCo2 == null)
                    _totalCo2 = this.SpecificCo2 * this.Area;

                return (double) _totalCo2;
            }
            private set => _totalCo2 = value;
        }

        /// <summary>
        /// Total surface area of this component, in [sqm]
        /// </summary>
        public double Area => rg.AreaMassProperties.Compute(BrepGeometry).Area -
                              (SubComponents?.Select(c => c.Area).Sum() ?? 0.0);

        /// <summary>
        /// Total weight of the entire component, in [kg]
        /// </summary>
        [JsonProperty]
        public double Weight { get; private set; }
        /// <summary>
        /// Wind pressure coefficient, C_p, of this component
        /// </summary>
        [JsonProperty] 
        public double WindPressureCoefficient { get; private set; }

        /// <summary>
        /// Indices of adjacent components
        /// </summary>
        [JsonProperty] 
        public int[] AdjacentComponents { get; private set; }
        /// <summary>
        /// Congruent area of respective adjacent component, in [sqm]
        /// </summary>
        [JsonProperty] 
        public double[] CongruentArea { get; private set; }

        /// <summary>
        /// Building construction of this component
        /// </summary>
        [JsonProperty] public Construction Construction { get; set; }


        public Component(rg.BrepFace surfaceGeometry)
        {
            this.BrepGeometry = surfaceGeometry.DuplicateFace(false);
        }

        [JsonConstructor]
        protected Component()
        {
            // only to be used during deserialization
        }


        public virtual void ApplySia2024Construction(Sia2024Record siaRoom)
        {

            OpaqueConstruction opaqueConstruction = new OpaqueConstruction("SIA2024_Opaque")
            {
                UValue = siaRoom.UValueOpaque,
                Capacitance = siaRoom.CapacitancePerFloorArea
            };
            this.SpecificCo2 = siaRoom.OpaqueEmissions;
            this.SpecificCost = siaRoom.OpaqueCost;
            this.Construction = opaqueConstruction;
        }
    }




    /// <summary>
    /// Transparent surfaces on building hull
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class Window : Component
    {
        // Should also contain information for dynamic shading
        // static shading is defined as static shading object
        public Window(rg.BrepFace surfaceGeometry) : base(surfaceGeometry)
        {

        }
        [JsonConstructor] 
        protected Window() { }

        public override void ApplySia2024Construction(Sia2024Record siaRoom)
        {
            TransparentConstruction transparentConstruction = new TransparentConstruction("SIA2024_Window")
            {
                UValue = siaRoom.UValueTransparent,
                Transmissivity = siaRoom.GValue
            };
            base.SpecificCo2 = siaRoom.TransparentEmissions;
            base.SpecificCost = siaRoom.TransparentCost;
            base.Construction = transparentConstruction;
        }
    }


    /// <summary>
    /// Internal or external wall element
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class Wall : Component
    {

        // Wall, Roof, Floor, Ceiling are not input manually. But they need to be own classes, because they'll contain information like construction.
        public Wall(rg.BrepFace surfaceGeometry) : base(surfaceGeometry)
        {

        }
        [JsonConstructor] 
        protected Wall() { }

        public override void ApplySia2024Construction(Sia2024Record siaRoom)
        {

            OpaqueConstruction opaqueConstruction = new OpaqueConstruction("SIA2024_Opaque")
            {
                UValue = siaRoom.UValueWalls,
                Capacitance = siaRoom.CapacityWalls
            };
            this.SpecificCo2 = siaRoom.EmissionsWalls;
            this.SpecificCost = siaRoom.CostWalls;
            this.Construction = opaqueConstruction;
        }

    }


    /// <summary>
    /// Roof. Always external
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class Roof : Component
    {
        public Roof(rg.BrepFace surfaceGeometry) : base(surfaceGeometry)
        {

        }
        [JsonConstructor] 
        protected Roof() { }

        public override void ApplySia2024Construction(Sia2024Record siaRoom)
        {

            OpaqueConstruction opaqueConstruction = new OpaqueConstruction("SIA2024_Opaque")
            {
                UValue = siaRoom.UValueRoofs,
                Capacitance = siaRoom.CapacityRoofs
            };
            this.SpecificCo2 = siaRoom.EmissionsRoofs;
            this.SpecificCost = siaRoom.CostRoofs;
            this.Construction = opaqueConstruction;
        }

    }


    /// <summary>
    /// Ceiling, i.e. internal surface
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class Ceiling : Component
    {
        public Ceiling(rg.BrepFace surfaceGeometry) : base(surfaceGeometry)
        {

        }
        [JsonConstructor] 
        protected Ceiling() { }
    }


    /// <summary>
    /// Floor, i.e. internal surface
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class Floor : Component
    {
        public Floor(rg.BrepFace surfaceGeometry) : base(surfaceGeometry)
        {

        }
        [JsonConstructor] 
        protected Floor() { }

        public override void ApplySia2024Construction(Sia2024Record siaRoom)
        {

            OpaqueConstruction opaqueConstruction = new OpaqueConstruction("SIA2024_Opaque")
            {
                UValue = siaRoom.UValueFloors,
                Capacitance = siaRoom.CapacityFloors
            };
            this.SpecificCo2 = siaRoom.EmissionsFloors;
            this.SpecificCost = siaRoom.CostFloors;
            this.Construction = opaqueConstruction;
        }

    }



    [JsonObject(MemberSerialization.OptIn)]
    public class Shading : Component
    {
        public Shading(rg.BrepFace surfaceGeometry) : base(surfaceGeometry)
        {

        }

        [JsonConstructor]
        protected Shading()
        {
        }
    }

    /// <summary>
    /// Dynamic shading object, like louvers. Adjacent buildings are part of 'Environment.cs'
    /// Can be mesh or brep.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class DynamicShading : Component
    {


        /// <summary>
        /// indicating whether this shading device is dynamic, meaning it can move, like a louver. if false, it is static
        /// Furthermore, if true, it needs rg.Mesh [] DynamicGeometry that defines the different geometry states.
        /// If false, base.Geometry is enough
        /// </summary>
        [JsonProperty]
        public bool IsDynamic { get; private set; }

        /// <summary>
        /// Dynamic geometry as mesh. 
        /// Is an array, since it may contain different states, but only when (this.IsDynamic == true), otherwise this.ShadingGeometry.Length = 1
        /// It get's its normal state from base.Geometry
        /// </summary>
        [JsonProperty]
        public rg.Mesh[] DynamicGeometry { get; private set; }



        /// <summary>
        /// Time horizon for the schedule
        /// </summary>
        [JsonProperty] 
        public int Horizon { get; private set; }

        /// <summary>
        /// Time-resolved geometry schedule of the shading device. Only necessary if (this.IsDynamic == true)
        /// It refers to the indices in this.ShadingGeometry.
        /// E.g. this.GeometrySchedule[0] = 1 means that at timestep 0, geometry state 1 is active
        /// </summary>
        [JsonProperty] 
        public int[] GeometrySchedule { get; private set; }

        /// <summary>
        /// Time-resolved transmissivity schedule [0,1].
        /// When this.TransmissivitySchedule[timestep] = 1, then the transmissivity of the shading device at that timestep equals this.Transmissivity.
        /// Only necessary if (this.IsDynamic == true)
        /// </summary>
        [JsonProperty] 
        public double[] TransmissivitySchedule { get; private set; }
        [JsonProperty] 
        public double[] AbsorbtivitySchedule { get; private set; }
        [JsonProperty] 
        public double[] ReflectivitySchedule { get; private set; }


        public DynamicShading(rg.BrepFace surfaceGeometry) : base(surfaceGeometry)
        {

        }

        [JsonConstructor]
        protected DynamicShading()
        {
        }
    }


}
