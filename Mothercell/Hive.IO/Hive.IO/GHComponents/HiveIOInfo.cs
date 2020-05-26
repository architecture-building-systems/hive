using System;
using System.Drawing;
using Grasshopper.Kernel;

namespace Hive.IO
{
    public class HiveIOInfo : GH_AssemblyInfo
    {
        public override string Name
        {
            get
            {
                return "HiveIO";
            }
        }
        public override Bitmap Icon
        {
            get
            {
                //Return a 24x24 pixel bitmap to represent this GHA library.
                return null;
            }
        }
        public override string Description
        {
            get
            {
                //Return a short string describing the purpose of this GHA library.
                return "";
            }
        }
        public override Guid Id
        {
            get
            {
                return new Guid("1da9c6d0-f813-4e78-8c56-fc2672430613");
            }
        }

        public override string AuthorName
        {
            get
            {
                //Return a string identifying you or your company.
                return "ETH Zurich";
            }
        }
        public override string AuthorContact
        {
            get
            {
                //Return a string representing your preferred contact details.
                return "";
            }
        }
    }
}
public class HiveCategoryIcon : GH_AssemblyPriority
{
    public override GH_LoadingInstruction PriorityLoad()
    {
        Grasshopper.Instances.ComponentServer.AddCategoryIcon("[hive]", Hive.IO.Properties.Resources.Hive_Logo);
        Grasshopper.Instances.ComponentServer.AddCategorySymbolName("[hive]", 'H');
        return GH_LoadingInstruction.Proceed;
    }
}
