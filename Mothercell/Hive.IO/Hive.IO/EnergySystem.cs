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
        public abstract class SurfaceSystem
        {
            /// <summary>
            /// Rhino mesh geometry object representing the energy system. Can be quad or triangles.
            /// </summary>
            public Mesh SurfaceGeometry { get; private set; }
            /// <summary>
            /// Reference thermal efficiency. Functional efficiencies (e.g. time-resolved and/or based on irradiance) are computed in Hive.CORE components.
            /// </summary>
            public double RefEfficiencyThermal { get; private set; }
            /// <summary>
            /// Reference electric efficiency. Functional efficiencies (e.g. time-resolved and/or based on irradiance) are computed in Hive.CORE components.
            /// </summary>
            public double RefEfficiencyElectric { get; private set; }
            /// <summary>
            /// Investment cost per m2
            /// </summary>
            public double Cost { get; private set; }
            /// <summary>
            /// Life cycle GHG emissions, in kgCO2eq./m2
            /// </summary>
            public double GHG { get; private set; }
            /// <summary>
            /// Name of the technology (e.g. 'Mono-cristalline PV')
            /// </summary>
            public string Name { get; private set; }

            protected SurfaceSystem(Mesh surfaceGeometry, double refEfficiencyThermal, double refEfficiencyElectric, 
                double cost, double ghg, string name)
            {
                SurfaceGeometry = surfaceGeometry;
                RefEfficiencyThermal = refEfficiencyThermal;
                RefEfficiencyElectric = refEfficiencyElectric;
                Cost = cost;
                GHG = ghg;
                Name = name;
            }
        }


        /// <summary>
        /// Photovoltaic
        /// </summary>
        public class PV : SurfaceSystem
        {
            public PV(Mesh surfaceGeometry, double refEfficiencyElectric, double cost, double ghg, string name) 
                : base(surfaceGeometry, 0.0, refEfficiencyElectric, cost, ghg, name){ }
        }


        /// <summary>
        /// Solar Thermal
        /// </summary>
        public class ST : SurfaceSystem
        {
            public ST(Mesh surfaceGeometry, double refEfficiencyThermal, double cost, double ghg, string name)
                : base(surfaceGeometry, refEfficiencyThermal, 0.0, cost, ghg, name) { }
        }


        /// <summary>
        /// Hybrid Solar Photovolatic Thermal
        /// </summary>
        public class PVT : SurfaceSystem
        {
            public PVT(Mesh surfaceGeometry, double refEfficiencyThermal, double refEfficiencyElectric, double cost, double ghg, string name)
                : base(surfaceGeometry, refEfficiencyThermal, refEfficiencyElectric, cost, ghg, name) { }
        }


        /// <summary>
        /// Horizontal Ground Solar Collector
        /// </summary>
        public class GroundCollector : SurfaceSystem
        {
            public GroundCollector(Mesh surfaceGeometry, double refEfficiencyThermal, double cost, double ghg, string name)
                : base(surfaceGeometry, refEfficiencyThermal, 0.0, cost, ghg, name) { }
        }
        #endregion


        /// <summary>
        /// Electric lighting and daylight
        /// Including set points, daylight controls, luminaire technology, etc...
        /// </summary>
        public abstract class LightingSystems
        {

        }

        /// <summary>
        /// Domestic hot water systems
        /// E.g. showers, sinks, tanks...? Do I need that?
        /// </summary>
        public abstract class DomesticHotWater
        {
            
        }

        /// <summary>
        /// Ventilation systems
        /// heat recovery, windows, mechanical ventilation, ...
        /// </summary>
        public abstract class VentilationSystem
        {

        }

        /// <summary>
        /// Heating, Cooling, Electricity generation systems (no solar tech though)
        /// E.g. CHP, boiler, heat pump, chiller, ...
        /// </summary>
        public abstract class GenerationSystem
        {
            public enum TechnologyNames
            {
                Boiler,
                CombinedHeatPower,
                MicroCHP,
                AirSourceHeatPump,
                GroundSourceHeatPump,
                Photovoltaic,
                HybridPhotovoltaicThermal,
                SolarCollector
            }
        }

        /// <summary>
        /// Storage systems
        /// E.g. batteries, hot water tanks, ice storage, ...
        /// </summary>
        public abstract class StorageSystem
        {
            public enum TechnologyNames
            {
                Battery,
                WaterTank,
                IceStorage,
                GroundStorage,
            }

        }

        /// <summary>
        /// Systems that distribute heat/cooling to the room
        /// E.g. Underfloor heater, radiator, floor heating, ceiling cooling panels, etc.
        /// Also setting supply and return temperatures, cooling and heating set points...
        /// </summary>
        public abstract class DistributionSystem
        {

        }

    }
}
