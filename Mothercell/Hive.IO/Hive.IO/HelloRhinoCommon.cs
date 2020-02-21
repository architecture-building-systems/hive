using System;
using Rhino;
using Rhino.Commands;

namespace Hive.IO
{
    public class HelloRhinoCommon : Command
    {
        static HelloRhinoCommon _instance;
        public HelloRhinoCommon()
        {
            _instance = this;
        }

        ///<summary>The only instance of the HelloRhinoCommon command.</summary>
        public static HelloRhinoCommon Instance
        {
            get { return _instance; }
        }

        public override string EnglishName
        {
            get { return "HelloRhinoCommon"; }
        }

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            // TODO: complete command.
            return Result.Success;
        }
    }
}