using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hive.IO
{
    namespace BuildingMaterial
    {
        public struct Opaque
        {
            /// <summary>
            /// Name
            /// </summary>
            public string Name;
            /// <summary>
            /// lambda, [W/mK]
            /// </summary>
            public double Conductivity;
            /// <summary>
            /// rho, [kg/m^3]
            /// </summary>
            public double Density;
            /// <summary>
            /// c, [J/kgK]
            /// </summary>
            public double SpecificHeatCapacity;
            /// <summary>
            /// [-]
            /// </summary>
            public double ThermalAbsorptance;
            /// <summary>
            /// [-]
            /// </summary>
            public double SolarAbsorptance;
            /// <summary>
            /// [-]
            /// </summary>
            public double VisibleAbsorptance;
        }

        public struct Glass
        {

        }

        public struct Gas
        {

        }

    }
}
