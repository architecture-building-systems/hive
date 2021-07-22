using Hive.IO.Util;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hive.IO.Building
{

    public class Sia380ConstructionAssembly : ConstructionAssembly
    {
        [JsonProperty]
        public string ConstructionType;
        [JsonProperty]
        public string ConstructionTypePretty;
        [JsonProperty]
        public string Description;
        [JsonProperty]
        public double RoomSpecificHeatCapacity;

        [JsonProperty(Required = Required.Default)]
        public double WallsCapacity => WallsConstruction?.Capacitance ?? RoomSpecificHeatCapacity;
        [JsonProperty(Required = Required.Default)]
        public double FloorsCapacity => FloorsConstruction?.Capacitance ?? RoomSpecificHeatCapacity;
        [JsonProperty(Required = Required.Default)]
        public double RoofsCapacity => RoofsConstruction?.Capacitance ?? RoomSpecificHeatCapacity;

        public void SetCapacities(double floorArea, double wallArea, double roofArea)
        {
            var all_areas = floorArea + wallArea + roofArea;
            WallsConstruction = new OpaqueConstruction(Name)
            {
                Capacitance = RoomSpecificHeatCapacity * wallArea / all_areas
            };
            FloorsConstruction = new OpaqueConstruction(Name)
            {
                Capacitance = RoomSpecificHeatCapacity * floorArea / all_areas
            };
            RoofsConstruction = new OpaqueConstruction(Name)
            {
                Capacitance = RoomSpecificHeatCapacity * roofArea / all_areas
            };
            WindowsConstruction = new TransparentConstruction(Name);
        }
    }

    ///
    /// Based on SIA 380 construction types from 3.5.6.1 for determing Heat Capacitance per m2 of floor area.
    ///
    public static class Sia380Constructions
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
