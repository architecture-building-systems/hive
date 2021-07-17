namespace Hive.IO.Building
{
    /// <summary>
    /// For applying a construction template at building level rather than specifying per component. Useful if you want to use a template.
    /// </summary>
    public abstract class ConstructionAssembly
    {
        internal string Name { get; set; }
        internal double CapacitancePerFloorArea { get; set; }

        internal OpaqueConstruction ExternalWallConstruction;
        internal OpaqueConstruction InternalWallConstruction;
        internal OpaqueConstruction GroundSlabConstruction;
        internal OpaqueConstruction InternalSlabConstruction;
        internal OpaqueConstruction RoofSlabConstruction;
        internal TransparentConstruction ExternalWindowsConstruction;
    }

    /// <summary>
    /// SIA 380 :
    /// - "sehr-leicht"
    /// - Industrie-Stahlbau
    /// </summary>
    public class SuperLightWeightConstruction : ConstructionAssembly
    {
        public SuperLightWeightConstruction()
        {
            this.Name = "superlightweight";
            this.CapacitancePerFloorArea = 10.0; // from SIA 380
        }
    }

    /// <summary>
    /// Light construction template, such as wood construction.
    /// SIA 380 example:
    /// - Boden: Aus Massivholz und Bodenplatten
    /// - Aussenwände: Beplankt mit Tanne/Fichte
    /// - Innenwände: Beplankt mit Tanne/Fichte
    /// - Decke: Beplankt mit Tanne/Fichte
    /// </summary>
    public class LightWeightConstruction : ConstructionAssembly
    {
        public LightWeightConstruction()
        {
            this.Name = "lightweight";
            this.CapacitancePerFloorArea = 10.0; // from SIA 380
        }
    }

    public class MediumWeightConstruction : ConstructionAssembly
    {
        public MediumWeightConstruction()
        {
            this.Name = "mediumweight";
            this.CapacitancePerFloorArea = 100.0; // from SIA 380
        }
    }

    public class HeavyWeightConstruction : ConstructionAssembly
    {
        public HeavyWeightConstruction()
        {
            this.Name = "heavyweight";
            this.CapacitancePerFloorArea = 150.0; // from SIA 380
        }
    }
}
