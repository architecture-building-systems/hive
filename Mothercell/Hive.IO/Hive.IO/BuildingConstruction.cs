

namespace Hive.IO
{
    namespace BuildingConstruction
    {
        public abstract class Construction
        {
            /// <summary>
            /// Name of construction element, e.g. 'concrete30mm'
            /// </summary>
            public string Name { get; internal set; }
            /// <summary>
            /// Layers of various materials, e.g. 'concrete', 'insulation', 'render'. From outside to inside
            /// </summary>
            public BuildingMaterial.Material [] Layers { get; set; }
            /// <summary>
            /// Thickness of each material layer in [mm]
            /// </summary>
            public double [] Thickness { get; set; }


            #region High level physical properties
            /// <summary>
            /// U-Value of this component
            /// </summary>
            public double UValue { get; set; }
            /// <summary>
            /// Absorptivity
            /// </summary>
            public double Absorbtivity { get; set; }
            /// <summary>
            /// Reflectivity
            /// </summary>
            public double Reflectivity { get; set; }
            /// <summary>
            /// Transmissivity
            /// </summary>
            public double Transmissivity { get; set; }
            #endregion
        }

        /// <summary>
        /// Opaque construction element
        /// </summary>
        public class Opaque : Construction
        {
          public Opaque(string name)
            {
                this.Name = name;
            }

            
        }

        public class Transparent : Construction
        {
            public Transparent(string name)
            {
                this.Name = name;
            }
        }
    }
}
