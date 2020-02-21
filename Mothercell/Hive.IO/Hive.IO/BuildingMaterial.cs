

namespace Hive.IO
{
    namespace BuildingMaterial
    {
        public abstract class Material
        {
            /// <summary>
            /// Name
            /// </summary>
            public string Name { get; private set; }
            /// <summary>
            /// lambda, [W/mK]
            /// </summary>
            public double Conductivity { get; private set; }
            /// <summary>
            /// rho, [kg/m^3]
            /// </summary>
            public double Density { get; private set; }
            /// <summary>
            /// c, [J/kgK]
            /// </summary>
            public double SpecificHeatCapacity { get; private set; }
            /// <summary>
            /// [-]
            /// </summary>
            public double ThermalAbsorptance { get; private set; }
            /// <summary>
            /// [-]
            /// </summary>
            public double SolarAbsorptance { get; private set; }
            /// <summary>
            /// [-]
            /// </summary>
            public double VisibleAbsorptance { get; private set; }
        }

        public class Opaque : Material
        {

        }

        public class Glass : Material
        {

        }

        public class Gas : Material
        {

        }

    }
}
