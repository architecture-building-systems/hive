﻿using System;
using System.Collections.Generic;

namespace Hive.IO.Core
{
    public class AdaptiveComfort
    {

        const int MONTHS_IN_YEAR = 12;
        const int HOURS_IN_YEAR = 8760;

        public readonly double[] Setpoints;
        public readonly double[] SetpointsUB80;
        public readonly double[] SetpointsLB80;
        public readonly double[] SetpointsUB90;
        public readonly double[] SetpointsLB90;

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

            int resolution = AmbientTemperature.Count;

            // Check temperature length
            if (resolution == MONTHS_IN_YEAR || resolution == HOURS_IN_YEAR)
            {
                Setpoints = new double[resolution];
                SetpointsUB80 = new double[resolution];
                SetpointsLB80 = new double[resolution];
                SetpointsUB90 = new double[resolution];
                SetpointsLB90 = new double[resolution];

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
            else
            {
                throw new ArgumentException("Only hourly or monthly averaged temperatures are supported.");
            }
        }
    }
}
