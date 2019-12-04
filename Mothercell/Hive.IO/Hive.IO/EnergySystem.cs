using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hive.IO
{
    /// <summary>
    /// Namespace for Energy Systems geometry and properties
    /// </summary>
    namespace EnergySystem
    {
        public abstract class SurfaceSystem
        {
            public double Area { get; private set; }
            public double RefEfficiencyThermal { get; private set; }
            public double RefEfficiencyElectric { get; private set; }

            protected SurfaceSystem(double area, double refEfficiencyThermal, double refEfficiencyElectric)
            {
                Area = area;
                RefEfficiencyThermal = refEfficiencyThermal;
                RefEfficiencyElectric = refEfficiencyElectric;
            }
        }


        /// <summary>
        /// Photovoltaic
        /// </summary>
        public class PV : SurfaceSystem
        {
            public PV(double area, double refEfficiencyThermal, double refEfficiencyElectric) 
                : base(area, refEfficiencyThermal, refEfficiencyElectric){ }
        }


        /// <summary>
        /// Solar Thermal
        /// </summary>
        public class ST : SurfaceSystem
        {
            public ST(double area, double refEfficiencyThermal, double refEfficiencyElectric)
                : base(area, refEfficiencyThermal, refEfficiencyElectric) { }
        }


        /// <summary>
        /// Hybrid Solar Photovolatic Thermal
        /// </summary>
        public class PVT : SurfaceSystem
        {
            public PVT(double area, double refEfficiencyThermal, double refEfficiencyElectric)
                : base(area, refEfficiencyThermal, refEfficiencyElectric) { }
        }


        /// <summary>
        /// Horizontal Ground Solar Collector
        /// </summary>
        public class GroundCollector : SurfaceSystem
        {
            public GroundCollector(double area, double refEfficiencyThermal, double refEfficiencyElectric)
                : base(area, refEfficiencyThermal, refEfficiencyElectric) { }
        }
    }
}
