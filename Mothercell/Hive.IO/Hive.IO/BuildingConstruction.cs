

namespace Hive.IO
{
    namespace BuildingConstruction
    {
        /// <summary>
        /// Opaque construction element
        /// </summary>
        public struct Opaque
        {
            /// <summary>
            /// Name of construction element, e.g. 'concrete30mm'
            /// </summary>
            public string Name;
            /// <summary>
            /// Material name, e.g. 'concrete'
            /// </summary>
            public string Material;
            /// <summary>
            /// [mm]
            /// </summary>
            public double Thickness;
        }

        public struct Transparent
        {

        }
    }
}
