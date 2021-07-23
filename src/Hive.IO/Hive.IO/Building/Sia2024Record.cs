using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Hive.IO.Util;
using Newtonsoft.Json;

namespace Hive.IO.Building
{
    /// <summary>
    ///     Stores data from the sia2024_room_data.json resource - enriched with information about
    ///     building-use-type and quality, which are used to select a specific Sia2024Record from
    ///     the json file. Use Sia202Record - this class is just for loading the data and filtering.
    /// </summary>
    public class Sia2024RecordEx : Sia2024Record
    {
        public string BuildingUseType;
        public string Quality;
        public string Construction;

        public Sia2024RecordEx()
        {
        }

        public Sia2024RecordEx(Sia2024Record room)
        {
            Quality = "<Custom>";
            BuildingUseType = "<Custom>";
            Construction = "<Custom>";

            RoomType = room.RoomType;
            RoomConstant = room.RoomConstant;
            CapacitancePerFloorArea = room.CapacitancePerFloorArea;
            CoolingSetpoint = room.CoolingSetpoint;
            HeatingSetpoint = room.HeatingSetpoint;
            CoolingSetback = room.CoolingSetback;
            HeatingSetback = room.HeatingSetback;
            FloorArea = room.FloorArea;
            EnvelopeArea = room.EnvelopeArea;
            GlazingRatio = room.GlazingRatio;
            UValueOpaque = room.UValueOpaque;
            UValueTransparent = room.UValueTransparent;
            GValue = room.GValue;
            GValueTotal = room.GValueTotal;
            ShadingSetpoint = room.ShadingSetpoint;
            WindowFrameReduction = room.WindowFrameReduction;
            AirChangeRate = room.AirChangeRate;
            Infiltration = room.Infiltration;
            HeatRecovery = room.HeatRecovery;
            OccupantLoads = room.OccupantLoads;
            LightingLoads = room.LightingLoads;
            EquipmentLoads = room.EquipmentLoads;
            OccupantYearlyHours = room.OccupantYearlyHours;
            LightingYearlyHours = room.LightingYearlyHours;
            EquipmentYearlyHours = room.EquipmentYearlyHours;
            OpaqueCost = room.OpaqueCost;
            TransparentCost = room.TransparentCost;
            OpaqueEmissions = room.OpaqueEmissions;
            TransparentEmissions = room.TransparentEmissions;
        }

        public new static Sia2024RecordEx FromJson(string json)
        {
            return new Sia2024RecordEx(Sia2024Record.FromJson(json));
        }

        public Sia2024RecordEx Clone()
        {
            return MemberwiseClone() as Sia2024RecordEx;
        }
    }

    /// <summary>
    ///     Describes a SIA 2024 room and can load / save this information to JSON.
    /// </summary>
    public class Sia2024Record
    { 
        private static Sia2024RecordEx[] _records;

        private static Dictionary<Tuple<string, string, string>, Sia2024RecordEx> _recordLookup;
        public double AirChangeRate; // Aussenluft-Volumenstrom (pro NGF)
        public double CoolingSetpoint; // Raumlufttemperatur Auslegung Kuehlung (Sommer)
        public double CoolingSetback; // TODO Raumlufttemperatur Auslegung Kuehlung (Sommer)
        public double EnvelopeArea; // Thermische Gebaeudehuellflaeche
        public double EquipmentLoads; // Waermeeintragsleistung der Geraete
        public double EquipmentYearlyHours; // Jaehrliche Vollaststunden der Geraete
        public double FloorArea; // Nettogeschossflaeche
        public double GlazingRatio; // Glasanteil
        public double GValue; // Gesamtenergiedurchlassgrad Verglasung
        public double GValueTotal; // Gesamtenergiedurchlassgrad Verglasung und Sonnenschutz
        public double ShadingSetpoint; // shading setpoint in W/m^2 at which GValueTotal is used instead of GValue
        public double HeatingSetpoint; // Raumlufttemperatur Auslegung Heizen (Winter)
        public double HeatingSetback; // TODO Raumlufttemperatur Auslegung Heizen (Winter)
        public double HeatRecovery; // Temperatur-Aenderungsgrad der Waermerueckgewinnung
        public double Infiltration; // Aussenluft-Volumenstrom durch Infiltration
        public double LightingLoads; // Waermeeintragsleistung der Raumbeleuchtung
        public double LightingYearlyHours; // Jaehrliche Vollaststunden der Raumbeleuchtung
        public double OccupantLoads; // Waermeeintragsleistung Personen (bei 24.0 deg C, bzw. 70 W)
        public double OccupantYearlyHours; // Vollaststunden pro Jahr (Personen)
        public double OpaqueCost; // Kosten opake Bauteile
        public double OpaqueEmissions; // Emissionen opake Bauteile
        public double RoomConstant; // Zeitkonstante
        public double CapacitancePerFloorArea; // Waermespeicherfaehigkeit des Raumes
        public string RoomType; // description, e.g. "1.1 Wohnen Mehrfamilienhaus"
        public double TransparentCost; // Kosten transparente Bauteile
        public double TransparentEmissions; // Emissionen transparente Bauteile
        public double UValueOpaque; // U-Wert opake Bauteile
        public double UValueTransparent; // U-Wert Fenster
        public double WindowFrameReduction; // Abminderungsfaktor fuer Fensterrahmen

        // as per #466, allow Sia2024Record to have separate values for walls, floors, roofs. Fall back to the Opaque values
        private double? _uValueFloors = null;
        private double? _capacityFloors = null;
        private double? _costFloors = null;
        private double? _emissionsFloors = null;

        private double? _uValueRoofs = null;
        private double? _capacityRoofs = null;
        private double? _costRoofs = null;
        private double? _emissionsRoofs = null;

        private double? _uValueWalls = null;
        private double? _capacityWalls = null;
        private double? _costWalls = null;
        private double? _emissionsWalls = null;

        public double UValueFloors
        {
            get => _uValueFloors ?? UValueOpaque;
            set => _uValueFloors = value;
        }
        public double CapacityFloors
        {
            get => _capacityFloors ?? CapacitancePerFloorArea;
            set => _capacityFloors = value;
        }

        public double CostFloors
        {
            get => _costFloors ?? OpaqueCost;
            set => _costFloors = value;
        }

        public double EmissionsFloors
        {
            get => _emissionsFloors ?? OpaqueEmissions;
            set => _emissionsFloors = value;
        }

        public double UValueRoofs
        {
            get => _uValueRoofs ?? UValueOpaque;
            set => _uValueRoofs = value;
        }
        public double CapacityRoofs
        {
            get => _capacityRoofs ?? CapacitancePerFloorArea;
            set => _capacityRoofs = value;
        }

        public double CostRoofs
        {
            get => _costRoofs ?? OpaqueCost;
            set => _costRoofs = value;
        }

        public double EmissionsRoofs
        {
            get => _emissionsRoofs ?? OpaqueEmissions;
            set => _emissionsRoofs = value;
        }

        public double UValueWalls
        {
            get => _uValueWalls ?? UValueOpaque;
            set => _uValueWalls = value;
        }

        public double CapacityWalls
        {
            get => _capacityWalls ?? CapacitancePerFloorArea;
            set => _capacityWalls = value;
        }

        public double CostWalls
        {
            get => _costWalls ?? OpaqueCost;
            set => _costWalls = value;
        }

        public double EmissionsWalls
        {
            get => _emissionsWalls ?? OpaqueEmissions;
            set => _emissionsWalls = value;
        }

        /// <summary>
        ///     Serialize to JSON - using the interchange format used in the Hive.Core components.
        /// </summary>
        /// <returns></returns>
        public string ToJson()
        {
            var result = new Dictionary<string, object>
            {
                {"description", RoomType},
                {"Zeitkonstante", RoomConstant},
                {"Waermespeicherfaehigkeit des Raumes", CapacitancePerFloorArea},
                {"Raumlufttemperatur Auslegung Kuehlung (Sommer)", CoolingSetpoint},
                {"Raumlufttemperatur Auslegung Heizen (Winter)", HeatingSetpoint},
                {"Raumlufttemperatur Auslegung Kuehlung (Sommer) - Absenktemperatur", CoolingSetback},
                {"Raumlufttemperatur Auslegung Heizen (Winter) - Absenktemperatur", HeatingSetback},
                {"Nettogeschossflaeche", FloorArea},
                {"Thermische Gebaeudehuellflaeche", EnvelopeArea},
                {"Glasanteil", GlazingRatio},
                {"U-Wert opake Bauteile", UValueOpaque},
                {"U-Wert Fenster", UValueTransparent},
                {"Gesamtenergiedurchlassgrad Verglasung", GValue},
                {"Gesamtenergiedurchlassgrad Verglasung und Sonnenschutz", GValueTotal},
                {"Strahlungsleistung fuer Betaetigung Sonnenschutz", ShadingSetpoint},
                {"Abminderungsfaktor fuer Fensterrahmen", WindowFrameReduction},
                {"Aussenluft-Volumenstrom (pro NGF)", AirChangeRate},
                {"Aussenluft-Volumenstrom durch Infiltration", Infiltration},
                {"Temperatur-Aenderungsgrad der Waermerueckgewinnung", HeatRecovery},
                {"Waermeeintragsleistung Personen (bei 24.0 deg C, bzw. 70 W)", OccupantLoads},
                {"Waermeeintragsleistung der Raumbeleuchtung", LightingLoads},
                {"Waermeeintragsleistung der Geraete", EquipmentLoads},
                {"Vollaststunden pro Jahr (Personen)", OccupantYearlyHours},
                {"Jaehrliche Vollaststunden der Raumbeleuchtung", LightingYearlyHours},
                {"Jaehrliche Vollaststunden der Geraete", EquipmentYearlyHours},
                {"Kosten opake Bauteile", OpaqueCost},
                {"Kosten transparente Bauteile", TransparentCost},
                {"Emissionen opake Bauteile", OpaqueEmissions},
                {"Emissionen transparente Bauteile", TransparentEmissions},

                {"U-Wert Boeden", UValueFloors },
                {"U-Wert Daecher", UValueRoofs },
                {"U-Wert Waende", UValueWalls },
                {"Waermespeicherfaehigkeit Boeden", CapacityFloors },
                {"Waermespeicherfaehigkeit Daecher", CapacityRoofs },
                {"Waermespeicherfaehigkeit Waende", CapacityWalls },
                {"Kosten Boeden", CostFloors},
                {"Kosten Daecher", CostRoofs },
                {"Kosten Waende", CostWalls },
                {"Emissionen Boeden", EmissionsFloors },
                {"Emissionen Daecher", EmissionsRoofs },
                {"Emissionen Waende", EmissionsWalls }
            };
            return JsonConvert.SerializeObject(result);
        }

        public static Sia2024Record FromJson(string json)
        {
            var d = JsonConvert.DeserializeObject<Dictionary<string, object>>(json); // TODO why not deserialise to Sia2024Record with JsonProperty name set to german keys?
            Func<string, double?> readValueOrNull = key => d.ContainsKey(key) ? (double?) d[key] : null;

            return new Sia2024Record
            {
                RoomType = d["description"] as string,
                RoomConstant = (double) d["Zeitkonstante"],
                CapacitancePerFloorArea = (double)d["Waermespeicherfaehigkeit des Raumes"],
                CoolingSetpoint = (double) d["Raumlufttemperatur Auslegung Kuehlung (Sommer)"],
                HeatingSetpoint = (double) d["Raumlufttemperatur Auslegung Heizen (Winter)"],
                CoolingSetback = (double)d["Raumlufttemperatur Auslegung Kuehlung (Sommer) - Absenktemperatur"],
                HeatingSetback = (double)d["Raumlufttemperatur Auslegung Heizen (Winter) - Absenktemperatur"],
                FloorArea = (double) d["Nettogeschossflaeche"],
                EnvelopeArea = (double) d["Thermische Gebaeudehuellflaeche"],
                GlazingRatio = (double) d["Glasanteil"],
                UValueOpaque = (double) d["U-Wert opake Bauteile"],
                UValueTransparent = (double) d["U-Wert Fenster"],
                GValue = (double) d["Gesamtenergiedurchlassgrad Verglasung"],
                GValueTotal = (double)d["Gesamtenergiedurchlassgrad Verglasung und Sonnenschutz"],
                ShadingSetpoint = (double)d["Strahlungsleistung fuer Betaetigung Sonnenschutz"],
                WindowFrameReduction = (double) d["Abminderungsfaktor fuer Fensterrahmen"],
                AirChangeRate = (double) d["Aussenluft-Volumenstrom (pro NGF)"],
                Infiltration = (double) d["Aussenluft-Volumenstrom durch Infiltration"],
                HeatRecovery = (double) d["Temperatur-Aenderungsgrad der Waermerueckgewinnung"],
                OccupantLoads = (double) d["Waermeeintragsleistung Personen (bei 24.0 deg C, bzw. 70 W)"],
                LightingLoads = (double) d["Waermeeintragsleistung der Raumbeleuchtung"],
                EquipmentLoads = (double) d["Waermeeintragsleistung der Geraete"],
                OccupantYearlyHours = (double) d["Vollaststunden pro Jahr (Personen)"],
                LightingYearlyHours = (double) d["Jaehrliche Vollaststunden der Raumbeleuchtung"],
                EquipmentYearlyHours = (double) d["Jaehrliche Vollaststunden der Geraete"],
                OpaqueCost = (double) d["Kosten opake Bauteile"],
                TransparentCost = (double) d["Kosten transparente Bauteile"],
                OpaqueEmissions = (double) d["Emissionen opake Bauteile"],
                TransparentEmissions = (double) d["Emissionen transparente Bauteile"],

                _uValueFloors = readValueOrNull("U-Wert Boeden"),
                _uValueRoofs = readValueOrNull("U-Wert Daecher"),
                _uValueWalls = readValueOrNull("U-Wert Waende"),

                _capacityFloors = readValueOrNull("Waermespeicherfaehigkeit Boeden"),
                _capacityRoofs = readValueOrNull("Waermespeicherfaehigkeit Daecher"),
                _capacityWalls = readValueOrNull("Waermespeicherfaehigkeit Waende"),

                _costFloors = readValueOrNull("Kosten Boeden"),
                _costRoofs = readValueOrNull("Kosten Daecher"),
                _costWalls = readValueOrNull("Kosten Waende"),

                _emissionsFloors = readValueOrNull("Emissionen Boeden"),
                _emissionsRoofs = readValueOrNull("Emissionen Daecher"),
                _emissionsWalls = readValueOrNull("Emissionen Waende"),
            };
        }

        private static Sia2024RecordEx[] ReadRecords()
        {
            return JsonResource.ReadRecords("Hive.IO.Building.sia2024_room_data.json", ref _records);
        }

        public static IEnumerable<string> BuildingUseTypes()
        {
            return ReadRecords().Select(r => r.BuildingUseType).Distinct();
        }

        public static IEnumerable<string> Qualities()
        {
            return ReadRecords().Select(r => r.Quality).Distinct();
        }

        public static IEnumerable<string> RoomTypes(string useType)
        {
            return ReadRecords().Where(r => r.BuildingUseType == useType).Select(r => r.RoomType).Distinct();
        }

        // From SIA 380 !
        public static IEnumerable<string> ConstructionAssemblies()
        {
            //return Enum.GetNames(typeof(BuildingConstructionAssemblyTypes)).Select(c => c.ToLower()).ToList();
            return new[] { "superlightweight", "lightweight", "mediumweight", "heavyweight" };
        }

        public static double RoomSpecificCapacitanceLookup(string construction) => RoomSpecificCapacitanceLookupDict[construction];

        static Dictionary<string, double> RoomSpecificCapacitanceLookupDict = new Dictionary<string, double>()
        {
            { "superlightweight", 10.0},
            { "lightweight", 50.0},
            { "mediumweight", 100.0},
            { "heavyweight", 150.0}
        };

        public static Sia2024Record Lookup(string useType, string roomType, string quality)
        {
            if (_recordLookup == null)
            {
                _recordLookup = new Dictionary<Tuple<string, string, string>, Sia2024RecordEx>();
                foreach (var record in ReadRecords())
                    _recordLookup.Add(
                        new Tuple<string, string, string>(record.BuildingUseType, record.RoomType, record.Quality),
                        record);
            }

            return _recordLookup[new Tuple<string, string, string>(useType, roomType, quality)].Clone();
        }

        public static Sia2024RecordEx First()
        {
            return ReadRecords().First();
        }

        public static Sia2024Record Lookup(Sia2024RecordEx record)
        {
            return Lookup(record.BuildingUseType, record.RoomType, record.Quality);
        }

        public static IEnumerable<Sia2024Record> All()
        {
            return ReadRecords();
        }
    }
}