using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;

namespace Hive.IO.Building
{
    /// <summary>
    /// Stores data from the sia2024_room_data.json resource - enriched with information about
    /// building-use-type and quality, which are used to select a specific Sia2024Record from
    /// the json file. Use Sia202Record - this class is just for loading the data and filtering.
    /// </summary>
    public class Sia2024RecordEx: Sia2024Record
    {
        public string Quality;
        public string BuildingUseType;
    }
    
    /// <summary>
    /// Describes a SIA 2024 room and can load / save this information to JSON.
    /// </summary>
    public class Sia2024Record
    {
        public string RoomType;  // description, e.g. "1.1 Wohnen Mehrfamilienhaus"
        public double RoomConstant; // Zeitkonstante
        public double CoolingSetpoint; // Raumlufttemperatur Auslegung Kuehlung (Sommer)
        public double HeatingSetpoint; // Raumlufttemperatur Auslegung Heizen (Winter)
        public double FloorArea; // Nettogeschossflaeche
        public double EnvelopeArea; // Thermische Gebaeudehuellflaeche
        public double GlazingRatio; // Glasanteil
        public double UValueOpaque; // U-Wert opake Bauteile
        public double UValueTransparent; // U-Wert Fenster
        public double GValue; // Gesamtenergiedurchlassgrad Verglasung
        public double WindowFrameReduction; // Abminderungsfaktor fuer Fensterrahmen
        public double AirChangeRate; // Aussenluft-Volumenstrom (pro NGF)
        public double Infiltration; // Aussenluft-Volumenstrom durch Infiltration
        public double HeatRecovery; // Temperatur-Aenderungsgrad der Waermerueckgewinnung
        public double OccupantLoads; // Waermeeintragsleistung Personen (bei 24.0 deg C, bzw. 70 W)
        public double LightingLoads; // Waermeeintragsleistung der Raumbeleuchtung
        public double EquipmentLoads; // Waermeeintragsleistung der Geraete
        public double OccupantYearlyHours; // Vollaststunden pro Jahr (Personen)
        public double LightingYearlyHours; // Jaehrliche Vollaststunden der Raumbeleuchtung
        public double EquipmentYearlyHours; // Jaehrliche Vollaststunden der Geraete
        public double OpaqueCost; // Kosten opake Bauteile
        public double TransparentCost; // Kosten transparente Bauteile
        public double OpaqueEmissions; // Emissionen opake Bauteile
        public double TransparentEmissions; // Emissionen transparente Bauteile


        /// <summary>
        /// Serialize to JSON - using the interchange format used in the Hive.Core components.
        /// </summary>
        /// <returns></returns>
        public string ToJson()
        {
            var result = new Dictionary<string, object>
            {
                { "description", RoomType },
                { "Zeitkonstante", RoomConstant},
                { "Raumlufttemperatur Auslegung Kuehlung (Sommer)", CoolingSetpoint},
                { "Raumlufttemperatur Auslegung Heizen (Winter)", HeatingSetpoint},
                { "Nettogeschossflaeche", FloorArea},
                { "Thermische Gebaeudehuellflaeche", EnvelopeArea},
                { "Glasanteil", GlazingRatio},
                { "U-Wert opake Bauteile", UValueOpaque},
                { "U-Wert Fenster", UValueTransparent},
                { "Gesamtenergiedurchlassgrad Verglasung", GValue},
                { "Abminderungsfaktor fuer Fensterrahmen", WindowFrameReduction},
                { "Aussenluft-Volumenstrom (pro NGF)", AirChangeRate},
                { "Aussenluft-Volumenstrom durch Infiltration", Infiltration},
                { "Temperatur-Aenderungsgrad der Waermerueckgewinnung", HeatRecovery},
                { "Waermeeintragsleistung Personen (bei 24.0 deg C, bzw. 70 W)", OccupantLoads},
                { "Waermeeintragsleistung der Raumbeleuchtung", LightingLoads},
                { "Waermeeintragsleistung der Geraete", EquipmentLoads},
                { "Vollaststunden pro Jahr (Personen)", OccupantYearlyHours},
                { "Jaehrliche Vollaststunden der Raumbeleuchtung", LightingYearlyHours},
                { "Jaehrliche Vollaststunden der Geraete", EquipmentYearlyHours},
                { "Kosten opake Bauteile", OpaqueCost},
                { "Kosten transparente Bauteile", TransparentCost},
                { "Emissionen opake Bauteile", OpaqueEmissions},
                { "Emissionen transparente Bauteile", TransparentEmissions}
            };
            return JsonConvert.SerializeObject(result);
        }

        public static Sia2024Record FromJson(string json)
        {
            var d = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
            if (!d.ContainsKey("BuildingUseType"))
            {
                d["BuildingUseType"] = "Custom";
            }

            if (!d.ContainsKey("Quality"))
            {
                d["Quality"] = "Standardwert";
            }

            if (!d.ContainsKey("description"))
            {
                d["description"] = "Custom";
            }

            return new Sia2024Record
            {
                RoomType = d["description"] as string,
                RoomConstant = (double)d["Zeitkonstante"],
                CoolingSetpoint = (double)d["Raumlufttemperatur Auslegung Kuehlung (Sommer)"],
                HeatingSetpoint = (double)d["Raumlufttemperatur Auslegung Heizen (Winter)"],
                FloorArea = (double)d["Nettogeschossflaeche"],
                EnvelopeArea = (double)d["Thermische Gebaeudehuellflaeche"],
                GlazingRatio = (double)d["Glasanteil"],
                UValueOpaque = (double)d["U-Wert opake Bauteile"],
                UValueTransparent = (double)d["U-Wert Fenster"],
                GValue = (double)d["Gesamtenergiedurchlassgrad Verglasung"],
                WindowFrameReduction = (double)d["Abminderungsfaktor fuer Fensterrahmen"],
                AirChangeRate = (double)d["Aussenluft-Volumenstrom (pro NGF)"],
                Infiltration = (double)d["Aussenluft-Volumenstrom durch Infiltration"],
                HeatRecovery = (double)d["Temperatur-Aenderungsgrad der Waermerueckgewinnung"],
                OccupantLoads = (double)d["Waermeeintragsleistung Personen (bei 24.0 deg C, bzw. 70 W)"],
                LightingLoads = (double)d["Waermeeintragsleistung der Raumbeleuchtung"],
                EquipmentLoads = (double)d["Waermeeintragsleistung der Geraete"],
                OccupantYearlyHours = (double)d["Vollaststunden pro Jahr (Personen)"],
                LightingYearlyHours = (double)d["Jaehrliche Vollaststunden der Raumbeleuchtung"],
                EquipmentYearlyHours = (double)d["Jaehrliche Vollaststunden der Geraete"],
                OpaqueCost = (double)d["Kosten opake Bauteile"],
                TransparentCost = (double)d["Kosten transparente Bauteile"],
                OpaqueEmissions = (double)d["Emissionen opake Bauteile"],
                TransparentEmissions = (double)d["Emissionen transparente Bauteile"],
            };
        }

        private static Sia2024RecordEx[]_records;
        private static Sia2024RecordEx[] ReadRecords()
        {
            if (_records == null)
            {
                var assembly = Assembly.GetExecutingAssembly();
                var resourceName = "Hive.IO.Building.sia2024_room_data.json";
                var serializer = new JsonSerializer();
                using (var stream = assembly.GetManifestResourceStream(resourceName))
                {
                    using (var streamReader = new StreamReader(stream ?? throw new InvalidOperationException($"Could not find manifest resource '{resourceName}'")))
                    { 
                        using (var jsonTextReader = new JsonTextReader(streamReader))
                        {
                            _records = serializer.Deserialize<Sia2024RecordEx[]>(jsonTextReader);
                        }
                    }
                }
            }
            return _records;
        }

        public static IEnumerable<string> BuildingUseTypes() => ReadRecords().Select(r => r.BuildingUseType).Distinct();

        public static IEnumerable<string> Qualities() => ReadRecords().Select(r => r.Quality).Distinct();

        public static IEnumerable<string> RoomTypes(string useType) =>
            ReadRecords().Where(r => r.BuildingUseType == useType).Select(r => r.RoomType).Distinct();

        public static Sia2024Record Lookup(string useType, string roomType, string quality) =>
            ReadRecords()
                .FirstOrDefault(r => r.BuildingUseType == useType && r.RoomType == roomType && r.Quality == quality);

        public static IEnumerable<Sia2024Record> All() => ReadRecords();
    }
}