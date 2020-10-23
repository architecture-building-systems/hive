using Grasshopper.Kernel;

namespace Hive.IO
{
    public class HiveCategoryIcon : GH_AssemblyPriority
    {
        public override GH_LoadingInstruction PriorityLoad()
        {
            Grasshopper.Instances.ComponentServer.AddCategoryIcon("[hive]", Hive.IO.Properties.Resources.Hive_Logo);
            Grasshopper.Instances.ComponentServer.AddCategorySymbolName("[hive]", 'H');
            return GH_LoadingInstruction.Proceed;
        }
    }
}
