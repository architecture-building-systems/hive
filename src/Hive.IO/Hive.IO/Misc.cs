using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hive.IO
{
    public static class Misc
    {
        /// <summary>
        /// Typical calendar year
        /// </summary>
        public static readonly int[] DaysPerMonth = new int[12] { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
        public const int HoursPerDay = 24;
    }
}
