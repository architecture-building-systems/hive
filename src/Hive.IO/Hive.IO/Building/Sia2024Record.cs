using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;

namespace Hive.IO.Building
{
    /// <summary>
    /// Stores data from the Sia2024.json resource and loads an array of that information.
    /// </summary>
    public struct Sia2024Record
    {
        public string Quality;
        public string BuildingUseType;
        public string RoomType;

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
        /// Serialize to JSON - we might need that for debugging in the future.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

        private static Sia2024Record[]_records = null;
        public static Sia2024Record[] ReadRecords()
        {
            if (_records == null)
            {
                var assembly = Assembly.GetExecutingAssembly();
                var resourceName = "Hive.IO.Building.sia2024_room_data.json";
                var serializer = new JsonSerializer();
                using (var stream = assembly.GetManifestResourceStream(resourceName))
                {
                    using (var streamReader = new StreamReader(stream))
                    { 
                        using (var jsonTextReader = new JsonTextReader(streamReader))
                        {
                            _records = serializer.Deserialize<Sia2024Record[]>(jsonTextReader);
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
                .First(r => r.BuildingUseType == useType && r.RoomType == roomType && r.Quality == quality);
    }
}