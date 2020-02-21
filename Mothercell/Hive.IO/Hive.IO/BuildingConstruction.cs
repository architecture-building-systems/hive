

namespace Hive.IO
{
    namespace BuildingConstruction
    {
        public abstract class Construction
        {
            /// <summary>
            /// Name of construction element, e.g. 'concrete30mm'
            /// </summary>
            public string Name { get; private set; }
            /// <summary>
            /// Material name, e.g. 'concrete'
            /// </summary>
            public string Material { get; private set; }
            /// <summary>
            /// [mm]
            /// </summary>
            public double Thickness { get; private set; }


            #region High level physical properties
            /// <summary>
            /// U-Value of this component
            /// </summary>
            public double UValue { get; private set; }
            /// <summary>
            /// Absorptivity
            /// </summary>
            public double Absorbtivity { get; private set; }
            /// <summary>
            /// Reflectivity
            /// </summary>
            public double Reflectivity { get; private set; }
            /// <summary>
            /// Transmissivity
            /// </summary>
            public double Transmissivity { get; private set; }
            #endregion
        }

        /// <summary>
        /// Opaque construction element
        /// </summary>
        public class Opaque : Construction
        {
          
        }

        public class Transparent : Construction
        {

        }
    }
}
