using System;
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

        public double TemperaturAenderungsgradDerWaermerueckgewinnung;
        public double KostenOpakeBauteile;
        public double VollaststundenGeraete;
        public double Nettogeschossflaeche;
        public double Zeitkonstante;
        public double ThermischeGebaeudehuellflaeche;
        public double Glasanteil;
        public double GesamtenergiedurchlasgradVerglasung;
        public double RaumlufttemperaturAuslegungHeizen;
        public double AussenluftVolumenstromDurchInfiltration;
        public double VollaststundenPersonen;
        public double KostenTransparenteBauteile;
        public double VollaststundenRaumbeleuchtung;
        public double UWertOpakeBauteile;
        public double WaermeeintragsleistungGeraete;
        public double RaumlufttemperaturAuslegungKuehlen;
        public double AbminderungsfaktorFuerFensterrahmen;
        public double WaermeeintragsleistungRaumbeleuchtung;
        public double EmissionenTransparenteBauteile;
        public double? AussenluftVolumenstrom;
        public double WaermeeintragsleistungPersonen;
        public double EmissionenOpakeBauteile;
        public double UWertFenster;

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
    }
}