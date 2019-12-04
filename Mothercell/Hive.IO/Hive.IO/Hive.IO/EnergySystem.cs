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
            public double[] Irradiance { get; private set; }
            public double[] AirTemperature { get; private set; }
            public int Horizon { get; private set; }

            protected SurfaceSystem(double area, double refEfficiency, double[] irradiance, double[] airTemperature)
            {
                Area = area;
                RefEfficiency = refEfficiency;
                Irradiance = irradiance;
                AirTemperature = airTemperature;
                Horizon = GetHorizon(Irradiance, AirTemperature);
            }

            protected static int GetHorizon(double[] array1, double[] array2) { return (array1.Length < array2.Length) ? array1.Length : array2.Length; }
        }


        /// <summary>
        /// Photovoltaic
        /// </summary>
        public class PV : SurfaceSystem
        {
            public PV(double area, double refEfficiency, double[] irradiance, double[] airTemperature) 
                : base(area, refEfficiency, irradiance, airTemperature){ }
        }


        /// <summary>
        /// Solar Thermal
        /// </summary>
        public class ST : SurfaceSystem
        {
            public ST(double area, double refEfficiency, double[] irradiance, double[] airTemperature)
                : base(area, refEfficiency, irradiance, airTemperature) { }
        }


        /// <summary>
        /// Hybrid Solar Photovolatic Thermal
        /// </summary>
        public class PVT : SurfaceSystem
        {
            public PVT(double area, double refEfficiency, double[] irradiance, double[] airTemperature)
                : base(area, refEfficiency, irradiance, airTemperature) { }
        }


        /// <summary>
        /// Horizontal Ground Solar Collector
        /// </summary>
        public class GroundCollector : SurfaceSystem
        {
            public GroundCollector(double area, double refEfficiency, double[] irradiance, double[] airTemperature)
                : base(area, refEfficiency, irradiance, airTemperature) { }
        }
    }
}
