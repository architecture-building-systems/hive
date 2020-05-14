using System;
using System.Drawing;
using Grasshopper.Kernel;

namespace Hive.GUI
{
    public class HiveGUIInfo : GH_AssemblyInfo
    {
        public override string Name
        {
            get
            {
                return "HiveGUI";
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
                return new Guid("fda39963-3793-47cb-88fb-79fc5db3ffff");
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
