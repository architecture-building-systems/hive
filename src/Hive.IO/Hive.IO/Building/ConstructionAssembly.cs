namespace Hive.IO.Building
{
    /// <summary>
    /// Assembly of different constructions to represent a zone or room. Only construction / material properties, geometry information should be stored per Zone.
    /// </summary>
    public abstract class ConstructionAssembly
    {
        internal string Name { get; set; }
        internal OpaqueConstruction WallsConstruction { get; set; }
        internal OpaqueConstruction FloorsConstruction { get; set; }
        internal OpaqueConstruction RoofsConstruction { get; set; }
        internal TransparentConstruction WindowsConstruction { get; set; }
    }

    /// <summary>
    /// SIA 380 "sehr-leicht"
    /// Example: Industrie-Stahlbau
    /// </summary>
    public class SIASuperLightWeightConstruction : ConstructionAssembly
    {
        /// <summary>
        /// A not so elegant way of propagating capacitance to different building elements based on a given capacitance per floor area (from SIA 380).
        /// </summary>
        /// <param name="floorArea"></param>
        /// <param name="wallArea"></param>
        /// <param name="roofArea"></param>
        public SIASuperLightWeightConstruction(double floorArea, double wallArea, double roofArea)
        {
            Name = "superlightweight";
            var capacitancePerFloorArea = 10.0;
            var all_areas = floorArea + wallArea + roofArea;
            WallsConstruction = new OpaqueConstruction(Name) { 
                Capacitance = capacitancePerFloorArea * wallArea / all_areas
            };
            FloorsConstruction = new OpaqueConstruction(Name)
            {
                Capacitance = capacitancePerFloorArea * floorArea / all_areas
            };
            RoofsConstruction = new OpaqueConstruction(Name)
            {
                Capacitance = capacitancePerFloorArea * roofArea / all_areas
            };
            WindowsConstruction = new TransparentConstruction(Name);
        }
    }

    /// <summary>
    /// Light construction template, such as wood construction.
    /// SIA 380 "leicht"
    /// Example:
    /// - Boden: Aus Massivholz und Bodenplatten
    /// - Aussenwände: Beplankt mit Tanne/Fichte
    /// - Innenwände: Beplankt mit Tanne/Fichte
    /// - Decke: Beplankt mit Tanne/Fichte
    /// </summary>
    public class SIALightWeightConstruction : ConstructionAssembly
    {
        public SIALightWeightConstruction(double floorArea, double wallArea, double roofArea)
        {
            Name = "lightweight";
            var capacitancePerFloorArea = 50.0;
            var all_areas = floorArea + wallArea + roofArea;
            WallsConstruction = new OpaqueConstruction(Name)
            {
                Capacitance = capacitancePerFloorArea * wallArea / all_areas
            };
            FloorsConstruction = new OpaqueConstruction(Name)
            {
                Capacitance = capacitancePerFloorArea * floorArea / all_areas
            };
            RoofsConstruction = new OpaqueConstruction(Name)
            {
                Capacitance = capacitancePerFloorArea * roofArea / all_areas
            };
            WindowsConstruction = new TransparentConstruction(Name);
        }
    }

    public class SIAMediumWeightConstruction : ConstructionAssembly
    {
        public SIAMediumWeightConstruction(double floorArea, double wallArea, double roofArea)
        {
            this.Name = "mediumweight";
            var capacitancePerFloorArea = 100.0;
            var all_areas = floorArea + wallArea + roofArea;
            WallsConstruction = new OpaqueConstruction(Name)
            {
                Capacitance = capacitancePerFloorArea * wallArea / all_areas
            };
            FloorsConstruction = new OpaqueConstruction(Name)
            {
                Capacitance = capacitancePerFloorArea * floorArea / all_areas
            };
            RoofsConstruction = new OpaqueConstruction(Name)
            {
                Capacitance = capacitancePerFloorArea * roofArea / all_areas
            };
            WindowsConstruction = new TransparentConstruction(Name);
        }
    }

    public class SIAHeavyWeightConstruction : ConstructionAssembly
    {
        public SIAHeavyWeightConstruction(double floorArea, double wallArea, double roofArea)
        {
            this.Name = "heavyweight";
            var capacitancePerFloorArea = 150.0;
            var all_areas = floorArea + wallArea + roofArea;
            WallsConstruction = new OpaqueConstruction(Name)
            {
                Capacitance = capacitancePerFloorArea * wallArea / all_areas
            };
            FloorsConstruction = new OpaqueConstruction(Name)
            {
                Capacitance = capacitancePerFloorArea * floorArea / all_areas
            };
            RoofsConstruction = new OpaqueConstruction(Name)
            {
                Capacitance = capacitancePerFloorArea * roofArea / all_areas
            };
            WindowsConstruction = new TransparentConstruction(Name);
        }
    }
}
