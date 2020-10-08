using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;

namespace Hive.IO.GhInputOutput
{
    public class GhResultsParameter: GH_PersistentParam<ResultsDataType>
    {
        public GhResultsParameter() : base("HiveResultsParameter", "HiveResults",
            "A parameter for Hive.IO.Results.Results objects",
            "[hive]", "IO")
        {
        }
        public GhResultsParameter(GH_InstanceDescription nTag) : base(nTag)
        {
        }

        public GhResultsParameter(GH_InstanceDescription nTag, bool bIsListParam) : base(nTag, bIsListParam)
        {
        }

        public GhResultsParameter(string name, string nickname, string description, string category, string subcategory) : base(name, nickname, description, category, subcategory)
        {
        }

        public override Guid ComponentGuid => new Guid("4056dfe8-5fdc-48e9-999c-20faa8b20eba");
        protected override GH_GetterResult Prompt_Singular(ref ResultsDataType value)
        {
            throw new NotImplementedException();
        }

        protected override GH_GetterResult Prompt_Plural(ref List<ResultsDataType> values)
        {
            throw new NotImplementedException();
        }
    }

    public class ResultsDataType : GH_Goo<Results.Results>
    {
        public override IGH_Goo Duplicate()
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            throw new NotImplementedException();
        }

        public override bool IsValid => true;
        public override string TypeName => "ResultsDataType";
        public override string TypeDescription => "A GH_GOO wrapper for Hive.IO.Results.Results";
    }
}
