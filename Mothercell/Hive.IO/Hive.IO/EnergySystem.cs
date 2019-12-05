﻿using Rhino.Geometry;

namespace Hive.IO
{
    /// <summary>
    /// Namespace for Energy Systems geometry and properties
    /// </summary>
    namespace EnergySystem
    {
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

            protected SurfaceSystem(Mesh surfaceGeometry, double refEfficiencyThermal, double refEfficiencyElectric)
            {
                SurfaceGeometry = surfaceGeometry;
                RefEfficiencyThermal = refEfficiencyThermal;
                RefEfficiencyElectric = refEfficiencyElectric;
            }
        }


        /// <summary>
        /// Photovoltaic
        /// </summary>
        public class PV : SurfaceSystem
        {
            public PV(Mesh surfaceGeometry, double refEfficiencyThermal, double refEfficiencyElectric) 
                : base(surfaceGeometry, refEfficiencyThermal, refEfficiencyElectric){ }
        }


        /// <summary>
        /// Solar Thermal
        /// </summary>
        public class ST : SurfaceSystem
        {
            public ST(Mesh surfaceGeometry, double refEfficiencyThermal, double refEfficiencyElectric)
                : base(surfaceGeometry, refEfficiencyThermal, refEfficiencyElectric) { }
        }


        /// <summary>
        /// Hybrid Solar Photovolatic Thermal
        /// </summary>
        public class PVT : SurfaceSystem
        {
            public PVT(Mesh surfaceGeometry, double refEfficiencyThermal, double refEfficiencyElectric)
                : base(surfaceGeometry, refEfficiencyThermal, refEfficiencyElectric) { }
        }


        /// <summary>
        /// Horizontal Ground Solar Collector
        /// </summary>
        public class GroundCollector : SurfaceSystem
        {
            public GroundCollector(Mesh surfaceGeometry, double refEfficiencyThermal, double refEfficiencyElectric)
                : base(surfaceGeometry, refEfficiencyThermal, refEfficiencyElectric) { }
        }
    }
}