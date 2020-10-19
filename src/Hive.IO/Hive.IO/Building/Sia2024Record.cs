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

        public Sia2024RecordEx()
        {
        }

        public Sia2024RecordEx(Sia2024Record room)
        {
            Quality = "<Custom>";
            BuildingUseType = "<Custom>";

            RoomType = room.RoomType;
            RoomConstant = room.RoomConstant;
            CoolingSetpoint = room.CoolingSetpoint;
            HeatingSetpoint = room.HeatingSetpoint;
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
        public double EnvelopeArea; // Thermische Gebaeudehuellflaeche
        public double EquipmentLoads; // Waermeeintragsleistung der Geraete
        public double EquipmentYearlyHours; // Jaehrliche Vollaststunden der Geraete
        public double FloorArea; // Nettogeschossflaeche
        public double GlazingRatio; // Glasanteil
        public double GValue; // Gesamtenergiedurchlassgrad Verglasung
        public double GValueTotal; // Gesamtenergiedurchlassgrad Verglasung und Sonnenschutz
        public double ShadingSetpoint; // shading setpoint in W/m^2 at which GValueTotal is used instead of GValue
        public double HeatingSetpoint; // Raumlufttemperatur Auslegung Heizen (Winter)
        public double HeatRecovery; // Temperatur-Aenderungsgrad der Waermerueckgewinnung
        public double Infiltration; // Aussenluft-Volumenstrom durch Infiltration
        public double LightingLoads; // Waermeeintragsleistung der Raumbeleuchtung
        public double LightingYearlyHours; // Jaehrliche Vollaststunden der Raumbeleuchtung
        public double OccupantLoads; // Waermeeintragsleistung Personen (bei 24.0 deg C, bzw. 70 W)
        public double OccupantYearlyHours; // Vollaststunden pro Jahr (Personen)
        public double OpaqueCost; // Kosten opake Bauteile
        public double OpaqueEmissions; // Emissionen opake Bauteile
        public double RoomConstant; // Zeitkonstante
        public string RoomType; // description, e.g. "1.1 Wohnen Mehrfamilienhaus"
        public double TransparentCost; // Kosten transparente Bauteile
        public double TransparentEmissions; // Emissionen transparente Bauteile
        public double UValueOpaque; // U-Wert opake Bauteile
        public double UValueTransparent; // U-Wert Fenster
        public double WindowFrameReduction; // Abminderungsfaktor fuer Fensterrahmen


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
                {"Raumlufttemperatur Auslegung Kuehlung (Sommer)", CoolingSetpoint},
                {"Raumlufttemperatur Auslegung Heizen (Winter)", HeatingSetpoint},
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
                {"Emissionen transparente Bauteile", TransparentEmissions}
            };
            return JsonConvert.SerializeObject(result);
        }

        public static Sia2024Record FromJson(string json)
        {
            var d = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
            return new Sia2024Record
            {
                RoomType = d["description"] as string,
                RoomConstant = (double) d["Zeitkonstante"],
                CoolingSetpoint = (double) d["Raumlufttemperatur Auslegung Kuehlung (Sommer)"],
                HeatingSetpoint = (double) d["Raumlufttemperatur Auslegung Heizen (Winter)"],
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
                TransparentEmissions = (double) d["Emissionen transparente Bauteile"]
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