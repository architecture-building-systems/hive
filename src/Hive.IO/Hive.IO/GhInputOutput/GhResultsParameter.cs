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

        public GhResultsParameter(string name, string nickname, string description, string category, string subcategory)
            : base(name, nickname, description, category, subcategory)
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
            RhinoApp.WriteLine("ResultsDataType.Duplicate()");

            string serialized;
            try
            {
                serialized = JsonConvert.SerializeObject(Value, Formatting.Indented, JsonSerializerSettings);
            }
            catch (Exception ex)
            {
                RhinoApp.WriteLine($"Could not serialize: {ex}");
                return new ResultsDataType();
            }

            File.WriteAllText(@"c:\Users\darthoma\Downloads\before.json", serialized);

            Results.Results duplicatedResults;
            try
            {
                duplicatedResults = JsonConvert.DeserializeObject<Results.Results>(serialized, JsonSerializerSettings);
            }
            catch (Exception ex)
            {
                RhinoApp.WriteLine($"Could not deserialize: {ex}");
                File.WriteAllText(@"c:\Users\darthoma\Downloads\tracewriter.txt", TraceWriter.ToString());
                return new ResultsDataType();
            }


            // FIXME: remove once tested
            var reSerialized =
                JsonConvert.SerializeObject(duplicatedResults, Formatting.Indented, JsonSerializerSettings);
            File.WriteAllText(@"c:\Users\darthoma\Downloads\after.json", reSerialized);

            return new ResultsDataType(duplicatedResults);
        }

        public override string ToString()
        {
            RhinoApp.WriteLine("ResultsDataType.ToString()");

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

            RhinoApp.WriteLine($"ResultsDataType.CastFrom: {source.GetType().FullName}");

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
            RhinoApp.WriteLine("ResultsDataType.Write()");

            try
            {
                var jss = new JsonSerializerSettings();
                jss.TypeNameHandling = TypeNameHandling.All;
                writer.SetString("json", JsonConvert.SerializeObject(Value, Formatting.Indented, jss));
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
            RhinoApp.WriteLine("ResultsDataType.Read()");

            try
            {
                var jss = new JsonSerializerSettings();
                jss.TypeNameHandling = TypeNameHandling.All;
                var json = reader.GetString("json");
                Value = JsonConvert.DeserializeObject<Results.Results>(json, jss);
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