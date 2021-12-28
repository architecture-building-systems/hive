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
    public class ZoneSchedules
    {
        public string RoomType { get; set; }
        /// <summary>
        /// Multiplier of how many days (0=no days, 1=all days) to apply the daily profile per month over the year (length=12).
        /// </summary>
        public double[] YearlyProfile { get; set; }
        /// <summary>
        /// Days per week when schedule applies. From SIA 2024.
        /// </summary>
        public int DaysOffPerWeek { get; set; }
        /// <summary>
        /// Days used in the year considering days off per week. From SIA 2024.
        /// </summary>
        public int DaysUsedPerYear { get; set; }
        public OccupancySchedule OccupancySchedule { get; set; }
        public DeviceSchedule DeviceSchedule { get; set; }
        public LightingSchedule LightingSchedule { get; set; }
        public SetpointSchedule SetpointSchedule { get; set; }

        public new static ZoneSchedules FromJson(string json)
        {
            return JsonConvert.DeserializeObject<ZoneSchedules>(json);
        }
    }

    public struct OccupancySchedule
    {
        /// <summary>
        /// Multiplier of how many days (0=no days, 1=all days) to apply the daily profile
        /// per month over the year (length=12). From SIA 2024.
        /// </summary>
        public double[] DailyProfile { get; set; }
        public double Default => 0.0;
    }

    /// <summary>
    /// From SIA 2024 Geräte TagesProfil
    /// </summary>
    public struct DeviceSchedule
    {
        /// <summary>
        /// Multiplier of how many days (0=no days, 1=all days) to apply the daily profile 
        /// per month over the year (length=12). From SIA 2024.
        /// </summary>
        public double[] DailyProfile { get; set; }

        /// <summary>
        /// Mutliplier (0 to 1) when no occupancy. From SIA 2024 Load When Unoccupied.
        /// </summary>
        public double Default { get; set; }

    }

    public struct LightingSchedule
    {
        public double[] DailyProfile { get; set; }
        public double Default => 0;

    }

    /// <summary>
    /// Like Eplus (?) setpoint schedules: 1.0 = setpoint, 0.5 = setback, 0.0 = none.
    /// </summary>
    public struct SetpointSchedule
    {
        public double[] DailyProfile { get; set; }
        public double Default => 0; 
    }

    public static class Sia2024Schedules
    {
        private static ZoneSchedules[] _records;
        private static Dictionary<string, ZoneSchedules> _recordLookup;

        private static ZoneSchedules[] ReadRecords()
        {
            return JsonResource.ReadRecords("Hive.IO.Building.sia2024_schedules.json", ref _records);
        }

        public static ZoneSchedules Lookup(string roomType)
        {
            if (_recordLookup == null)
            {
                _recordLookup = new Dictionary<string, ZoneSchedules>();
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
