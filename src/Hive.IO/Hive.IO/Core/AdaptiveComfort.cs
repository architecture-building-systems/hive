using System;
using System.Collections.Generic;

namespace Hive.IO.Core
{
    public class AdaptiveComfort
    {

        const int MONTHS_IN_YEAR = 12;
        const int HOURS_IN_YEAR = 8760;

        public readonly List<double> Setpoints = new List<double>();
        public readonly List<double> SetpointsUB80 = new List<double>();
        public readonly List<double> SetpointsLB80 = new List<double>();
        public readonly List<double> SetpointsUB90 = new List<double>();
        public readonly List<double> SetpointsLB90 = new List<double>();

        // 
        //     Computes adaptive thermal comfort according to Thermal Comfort - PLEA Notes 3. Auliciems and Szokolay 2007
        //     :param T_m: Ambient temperature in deg C, either averaged monthly or hourly
        //     :return: Monthly or hourly adaptive thermal comfort temperature T_n, as well as upper and lower bound for 80 and 90 % acceptance
        //     
        public AdaptiveComfort(List<double> AmbientTemperature)
        {
            // from PLEA NOTES 3 - Thermal Comfort, by Andris Auliciems and Steven V. Szokolay. 2nd edition 2007
            // T_n = 21.5 + 0.11 T_m (Eq. 3.7, for mechanically heated or cooled buildings)
            // where T_n is adaptive thermal comfort temperature and T_m is mean monthly ambient temp
            // for 90% acceptability limits, T_n +/- 2.5 K, for 80 % T_n +/- 3.5 K

            // Check temperature length
            if (AmbientTemperature.Count != MONTHS_IN_YEAR || AmbientTemperature.Count != HOURS_IN_YEAR) 
                throw new ArgumentException("Only hourly or monthly averaged temperatures are supported.");
                
            for (int i = 0; i < AmbientTemperature.Count; i++)
            {
                var sp = 21.5 + 0.11 * AmbientTemperature[i];
                Setpoints[i] = sp;
                SetpointsUB80[i] = sp + 3.5;
                SetpointsLB80[i] = sp - 3.5;
                SetpointsUB90[i] = sp + 2.5;
                SetpointsLB90[i] = sp - 2.5;
            }
        }
    }
}
