using Rhino.Geometry;

namespace Hive.IO
{
    /// <summary>
    /// Namespace for Energy Systems geometry and properties
    /// </summary>
    namespace EnergySystem
    {
        #region Surface based technology
        /// <summary>
        /// Surface based energy technologies, such as PV, solar thermal, PVT, ground collectors, etc.
        /// </summary>
        public abstract class SurfaceBased : Generation
        {
            /// <summary>
            /// Rhino mesh geometry object representing the energy system. Can be quad or triangles.
            /// </summary>
            public Mesh SurfaceGeometry { get; private set; }


            protected SurfaceBased(Mesh surfaceGeometry, double refEfficiencyThermal, double refEfficiencyElectric, double cost, double ghg, bool isElectric, bool isHeating, bool isCooling) 
                : base(refEfficiencyThermal, refEfficiencyElectric, cost, ghg, isElectric, isHeating, isCooling)
            {
                SurfaceGeometry = surfaceGeometry;
            }
        }


        /// <summary>
        /// Photovoltaic
        /// </summary>
        public class PV : SurfaceBased
        {
            public PV(Mesh surfaceGeometry, double refEfficiencyElectric, double cost, double ghg, string detailedName) 
                : base(surfaceGeometry, 0.0, refEfficiencyElectric, cost, ghg, true, false, false)
            {
                base.DetailedName = detailedName;
                base.Name = TechnologyNames.Photovoltaic;
            }
        }


        /// <summary>
        /// Solar Thermal
        /// </summary>
        public class ST : SurfaceBased
        {
            public ST(Mesh surfaceGeometry, double refEfficiencyThermal, double cost, double ghg, string detailedName)
                : base(surfaceGeometry, refEfficiencyThermal, 0.0, cost, ghg, false, true, false) 
            {
                base.DetailedName = detailedName;
                base.Name = TechnologyNames.SolarCollector;
            }
        }


        /// <summary>
        /// Hybrid Solar Photovolatic Thermal
        /// </summary>
        public class PVT : SurfaceBased
        {
            public PVT(Mesh surfaceGeometry, double refEfficiencyThermal, double refEfficiencyElectric, double cost, double ghg, string detailedName)
                : base(surfaceGeometry, refEfficiencyThermal, refEfficiencyElectric, cost, ghg, true, true, false)
            {
                base.Name = TechnologyNames.HybridPhotovoltaicThermal;
                base.DetailedName = detailedName;
            }
        }


        /// <summary>
        /// Horizontal Ground Solar Collector
        /// </summary>
        public class GroundCollector : SurfaceBased
        {
            public GroundCollector(Mesh surfaceGeometry, double refEfficiencyThermal, double cost, double ghg, string detailedName)
                : base(surfaceGeometry, refEfficiencyThermal, 0.0, cost, ghg, false, true, false) 
            {
                base.Name = TechnologyNames.GroundCollector;
                base.DetailedName = detailedName;
            }
        }
        #endregion


        #region Combustion technology
        public abstract class Combustion : Generation
        {
            public Combustion(double refEfficiencyThermal, double refEfficiencyElectric, double cost, double ghg, bool isElectric, bool isHeating)
                : base(refEfficiencyThermal, refEfficiencyElectric, cost, ghg, isElectric, isHeating, false){ }
        }


        public class Boiler : Combustion
        {
            public Boiler (double refEfficiencyThermal, double cost, double ghg)
                : base(refEfficiencyThermal, 0.0, cost, ghg, false, true) 
            {
                base.Name = TechnologyNames.Boiler;
            }
        }


        public class CombinedHeatPower : Combustion
        {
            public CombinedHeatPower(double refEfficiencyThermal, double refEfficiencyElectric, double cost, double ghg)
                :base(refEfficiencyThermal, refEfficiencyElectric, cost, ghg, true, true)
            { 
                base.Name = TechnologyNames.CombinedHeatPower; 
            }
        }
        #endregion

        /// <summary>
        /// Heating, Cooling, Electricity generation systems (no solar tech though)
        /// E.g. CHP, boiler, heat pump, chiller, ...
        /// </summary>
        public abstract class Generation
        {
            /// <summary>
            /// Name of the technology
            /// </summary>
            public TechnologyNames Name;
            public enum TechnologyNames
            {
                Boiler,
                CombinedHeatPower,
                MicroCHP,
                AirSourceHeatPump,
                GroundSourceHeatPump,
                Photovoltaic,
                HybridPhotovoltaicThermal,
                SolarCollector,
                GroundCollector,
                DistrictHeating,
                DistrictCooling,
                Grid
            }
            /// <summary>
            /// Specification of the technology, e.g. "Mono-cristalline PV"
            /// </summary>
            public string DetailedName { get; protected set; }

            /// <summary>
            /// Reference thermal efficiency. Functional efficiencies (e.g. time-resolved and/or based on irradiance) are computed in Hive.CORE components.
            /// </summary>
            public double RefEfficiencyThermal { get; protected set; }
            /// <summary>
            /// Reference electric efficiency. Functional efficiencies (e.g. time-resolved and/or based on irradiance) are computed in Hive.CORE components.
            /// </summary>
            public double RefEfficiencyElectric { get; protected set; }
            /// <summary>
            /// Investment cost per m2
            /// </summary>
            public double Cost { get; protected set; }
            /// <summary>
            /// Life cycle GHG emissions, in kgCO2eq./m2
            /// </summary>
            public double GHG { get; protected set; }

            /// <summary>
            /// Indicating whether this technology produces electricity
            /// </summary>
            public bool IsElectric { get; protected set; }
            /// <summary>
            /// Indicating whether this technology produces heat
            /// </summary>
            public bool IsHeating { get; protected set; }
            /// <summary>
            /// Indicating whether this technology produces cooling
            /// </summary>
            public bool IsCooling { get; protected set; }

            protected Generation(double refEfficiencyThermal, double refEfficiencyElectric, double cost, double ghg, bool isElectric, bool isHeating, bool isCooling)
            {
                this.RefEfficiencyThermal = refEfficiencyThermal;
                this.RefEfficiencyElectric = refEfficiencyElectric;
                this.Cost = cost;
                this.GHG = ghg;
                this.IsElectric = isElectric;
                this.IsHeating = isHeating;
                this.IsCooling = isCooling;
            }
        }
    }
}
