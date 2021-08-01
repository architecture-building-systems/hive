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
        public double CapacitancePerFloorArea;

        [JsonProperty(Required = Required.Default)]
        public double WallsCapacity => WallsConstruction?.Capacitance ?? CapacitancePerFloorArea;
        [JsonProperty(Required = Required.Default)]
        public double FloorsCapacity => FloorsConstruction?.Capacitance ?? CapacitancePerFloorArea;
        [JsonProperty(Required = Required.Default)]
        public double RoofsCapacity => RoofsConstruction?.Capacitance ?? CapacitancePerFloorArea;

        public string Name => ConstructionType;

        public void SetCapacities(double floorArea, double wallArea, double roofArea)
        {
            var areas = floorArea + wallArea + roofArea;
            var capacitanceRoom = CapacitancePerFloorArea * floorArea;

            // Set walls
            if (WallsConstruction != null)
            {
                WallsConstruction.Capacitance = capacitanceRoom * wallArea / areas;
            }
            else
            {
                WallsConstruction = new OpaqueConstruction(Name)
                {
                    Capacitance = capacitanceRoom * wallArea / areas
                };
            }

            // Set floors
            if (FloorsConstruction != null)
            {
                FloorsConstruction.Capacitance = capacitanceRoom * floorArea / areas;
            }
            else
            {
                FloorsConstruction = new OpaqueConstruction(Name)
                {
                    Capacitance = capacitanceRoom * floorArea / areas
                };
            }

            // Set roofs
            if (RoofsConstruction != null)
            {
                RoofsConstruction.Capacitance = capacitanceRoom * roofArea / areas;
            }
            else
            {
                RoofsConstruction = new OpaqueConstruction(Name)
                {
                    Capacitance = capacitanceRoom * roofArea / areas
                };
            }

            // Set windows
            if (WindowsConstruction != null)
            {
                WindowsConstruction = new TransparentConstruction(Name);
            }
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
