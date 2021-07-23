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
}
