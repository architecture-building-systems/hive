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
            public double RefEfficiency { get; private set; }


            protected SurfaceSystem(double area, double refEfficiency)
            {
                Area = area;
                RefEfficiency = refEfficiency;
            }
        }


        /// <summary>
        /// Photovoltaic
        /// </summary>
        public class PV : SurfaceSystem
        {
            public PV(double area, double refEfficiency) 
                : base(area, refEfficiency){ }
        }


        /// <summary>
        /// Solar Thermal
        /// </summary>
        public class ST : SurfaceSystem
        {
            public ST(double area, double refEfficiency)
                : base(area, refEfficiency) { }
        }


        /// <summary>
        /// Hybrid Solar Photovolatic Thermal
        /// </summary>
        public class PVT : SurfaceSystem
        {
            public PVT(double area, double refEfficiency)
                : base(area, refEfficiency) { }
        }


        /// <summary>
        /// Horizontal Ground Solar Collector
        /// </summary>
        public class GroundCollector : SurfaceSystem
        {
            public GroundCollector(double area, double refEfficiency)
                : base(area, refEfficiency) { }
        }
    }
}
