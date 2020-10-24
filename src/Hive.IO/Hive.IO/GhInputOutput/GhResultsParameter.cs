using System;
using System.Collections.Generic;
using System.IO;
using GH_IO.Serialization;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Rhino;

namespace Hive.IO.GhInputOutput
{
    public class GhResultsParameter : GH_PersistentParam<ResultsDataType>
    {
        public GhResultsParameter() : base("Results Parameter Hive", "HiveResultsParameter",
            "A parameter to save (internalize) Hive Results objects",
            "[hive]", "IO")
        {
        }

        public GhResultsParameter(GH_InstanceDescription nTag) : base(nTag)
        {
        }

        public GhResultsParameter(GH_InstanceDescription nTag, bool bIsListParam) : base(nTag, bIsListParam)
        {
        }

        public GhResultsParameter(string name, string nickname, string description, string category, string subcategory)
            : base(name, nickname, description, category, subcategory)
        {
        }

        public override GH_Exposure Exposure => GH_Exposure.secondary;

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
        private static readonly ITraceWriter TraceWriter = new MemoryTraceWriter();
        private static readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings
        {
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
            TypeNameHandling = TypeNameHandling.All,
            TraceWriter = TraceWriter
        };

        



        public ResultsDataType()
        {
            m_value = new Results.Results();
        }

        public ResultsDataType(Results.Results results)
        {
            m_value = results;
        }

        public override bool IsValid => true;
        public override string TypeName => "ResultsDataType";
        public override string TypeDescription => "A GH_GOO wrapper for Hive.IO.Results.Results";

        public override IGH_Goo Duplicate()
        {
            string serialized;
            try
            {
                serialized = JsonConvert.SerializeObject(Value, Formatting.Indented, JsonSerializerSettings);
            }
            catch (Exception ex)
            {
                RhinoApp.WriteLine($"Could not serialize: {ex}");
                throw;
            }


            try
            {
                var duplicatedResults = JsonConvert.DeserializeObject<Results.Results>(serialized, JsonSerializerSettings);
                return new ResultsDataType(duplicatedResults);
            }
            catch (Exception ex)
            {
                RhinoApp.WriteLine($"Could not deserialize: {ex}");
                throw;
            }
        }

        public override string ToString()
        {
            try
            {
                return JsonConvert.SerializeObject(Value, Formatting.Indented, JsonSerializerSettings);
            }
            catch (Exception)
            {
                RhinoApp.WriteLine("ResultsDataType.ToString() failed!!");
                return "ResultsDataType (can't convert)";
            }
        }

        public override bool CastFrom(object source)
        {
            if (source == null) return false;
            if (source is Results.Results results)
            {
                Value = results;
                return true;
            }

            // don't know how to convert this...
            return false;
        }

        public override bool Write(GH_IWriter writer)
        {
            try
            {
                writer.SetString("json", JsonConvert.SerializeObject(Value, Formatting.Indented, JsonSerializerSettings));
                return true;
            }
            catch (Exception ex)
            {
                RhinoApp.WriteLine($"ResultsDataType.Write() failed!! {ex}");
                return false;
            }
        }

        public override bool Read(GH_IReader reader)
        {
            try
            {
                var json = reader.GetString("json");
                Value = JsonConvert.DeserializeObject<Results.Results>(json, JsonSerializerSettings);
                return true;
            }
            catch (Exception ex)
            {
                RhinoApp.WriteLine($"ResultsDataType.Read() failed!! {ex}");
                return false;
            }
        }
    }
}