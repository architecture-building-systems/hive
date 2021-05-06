using Hive.IO.Util;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hive.IO.Building
{

    /// <summary>
    /// Schedules that define annual hourly internal loads schedules
    /// </summary>
    //[JsonProperty]
    //public StructSchedules Schedules;
    public class Sia2024Schedule
    {
        public string RoomType { get; set; }
        public OccupancySchedule OccupancySchedule { get; set; }
        public DevicesSchedule DevicesSchedule { get; set; }
        public LightingSchedule LightingSchedule { get; set; }
    }

    public struct OccupancySchedule
    {
        /// <summary>
        /// Days per week when schedule applies.
        /// </summary>
        public int DaysOffPerWeek { get; set; }
        /// <summary>
        /// Days per year when schedule applies. Probably not needed here
        /// </summary>
        public int DaysUsedPerYear { get; set; }
        /// <summary>
        /// Multiplier of how many days (0=no days, 1=all days) to apply the daily profile per month over the year (length=12).
        /// </summary>
        public double[] DailyProfile { get; set; }
        /// <summary>
        /// Multiplier of how many days (0=no days, 1=all days) to apply the daily profile per month over the year (length=12).
        /// </summary>
        public double[] YearlyProfile { get; set; }
    }

    /// <summary>
    /// From SIA 2024 Geräte TagesProfil
    /// </summary>
    public struct DevicesSchedule
    {
        /// <summary>
        /// Multiplier of how many days (0=no days, 1=all days) to apply the daily profile per month over the year (length=12).
        /// </summary>
        public double[] DailyProfile { get; set; }

        /// <summary>
        /// Mutliplier (0 to 1) when no occupancy.
        /// </summary>
        public double LoadWhenUnoccupied { get; set; }

    }

    public struct LightingSchedule
    {
        /// <summary>
        /// Number of hours on between 7-18h. SIA2024: Nutzungsstunden pro Tag
        /// </summary>
        public int HoursPerDay { get; set; }
        /// <summary>
        /// Number of hours on between 18h-7h. SIA2024: Nutzungsstunden pro Nacht
        /// </summary>
        public int HoursPerNight { get; set; }


        // TODO Korrekturfaktor für Präsenzregelung
    }

    public static class Sia2024Schedules
    {
        private static Sia2024Schedule[] _records;
        private static Dictionary<string, Sia2024Schedule> _recordLookup;

        private static Sia2024Schedule[] ReadRecords()
        {
            return JsonResource.ReadRecords("Hive.IO.Building.sia2024_schedules.json", ref _records);
        }

        public static Sia2024Schedule Lookup(string roomType)
        {
            if (_recordLookup == null)
            {
                _recordLookup = new Dictionary<string, Sia2024Schedule>();
                foreach (var record in ReadRecords())
                    _recordLookup.Add(record.RoomType, record);
            }

            return _recordLookup[roomType];
        }

        public static string ToJson(string roomType)
        {
            return JsonConvert.SerializeObject(Lookup(roomType));
        }
    }
}
