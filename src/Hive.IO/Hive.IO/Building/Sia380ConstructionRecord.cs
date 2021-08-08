using Hive.IO.Util;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Hive.IO.Building
{

    public class Sia380ConstructionAssembly : ConstructionAssembly
    {
        [JsonProperty]
        public string ConstructionType { get; private set; }
        [JsonProperty]
        public string ConstructionTypePretty { get; private set; }
        [JsonProperty]
        public string Description { get; private set; }
        [JsonProperty]
        public double CapacitancePerFloorArea { get; private set; }

        [JsonProperty(Required = Required.Default)]
        public double WallsCapacity => WallsConstruction?.Capacitance ?? CapacitancePerFloorArea;
        [JsonProperty(Required = Required.Default)]
        public double FloorsCapacity => FloorsConstruction?.Capacitance ?? CapacitancePerFloorArea;
        [JsonProperty(Required = Required.Default)]
        public double RoofsCapacity => RoofsConstruction?.Capacitance ?? CapacitancePerFloorArea;

        public new string Name => ConstructionType;

        [OnDeserialized] 
        // This method runs once this class has been instantiated when deserializing in the JSON record.
        // So upon creating the record lookup below, this will run afterwards to instantiate the Construction attributes.
        // (and avoid Null exceptions)
        public void Init(StreamingContext context)
        {
            WallsConstruction = new OpaqueConstruction(Name);
            FloorsConstruction = new OpaqueConstruction(Name);
            RoofsConstruction = new OpaqueConstruction(Name);
            WindowsConstruction = new TransparentConstruction(Name);
        }

        public void SetCapacities(double floorArea, double wallArea, double roofArea)
        {
            var areas = floorArea + wallArea + roofArea;
            var capacitanceRoom = CapacitancePerFloorArea * floorArea;
            WallsConstruction.Capacitance = capacitanceRoom * wallArea / areas;
            FloorsConstruction.Capacitance = capacitanceRoom * floorArea / areas;
            RoofsConstruction.Capacitance = capacitanceRoom * roofArea / areas;
        }
    }

    ///
    /// Based on SIA 380 construction types from 3.5.6.1 for determing Heat Capacitance per m2 of floor area.
    ///
    public static class Sia380ConstructionRecord
    {
        private static Sia380ConstructionAssembly[] _records;
        private static Dictionary<string, Sia380ConstructionAssembly> _recordLookup;

        private static Sia380ConstructionAssembly[] ReadRecords()
        {
            return JsonResource.ReadRecords("Hive.IO.Building.sia380_constructions.json", ref _records);
        }

        public static Sia380ConstructionAssembly Lookup(string constructionType)
        {
            if (_recordLookup == null)
            {
                _recordLookup = new Dictionary<string, Sia380ConstructionAssembly>();
                foreach (var record in ReadRecords())
                    _recordLookup.Add(record.ConstructionType, record);
            }

            return _recordLookup[constructionType];
        }

        public static IEnumerable<string> ConstructionTypes()
        {
            return ReadRecords().Select(r => r.ConstructionType);
        }

        public static string ToJson(string constructionType)
        {
            return JsonConvert.SerializeObject(Lookup(constructionType));
        }
    }
}
