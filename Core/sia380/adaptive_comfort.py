# coding=utf-8

from __future__ import division
import math


def adaptive_comfort(T_m):
    '''
    Computes adaptive thermal comfort according to Thermal Comfort - PLEA Notes 3. Auliciems and Szokolay 2007
    :param T_m: Average monthly ambient temperature in deg C
    :return: Monthly adaptive thermal comfort temperature T_n, as well as upper and lower bound for 80 and 90 % acceptance
    '''

    # from PLEA NOTES 3 - Thermal Comfort, by Andris Auliciems and Steven V. Szokolay. 2nd edition 2007
    # T_n = 21.5 + 0.11 T_m (Eq. 3.7, for mechanically heated or cooled buildings)
    # where T_n is adaptive thermal comfort temperature and T_m is mean monthly ambient temp
    # for 90% acceptability limits, T_n +/- 2.5 K, for 80 % T_n +/- 3.5 K

    setpoints = [21.5 + 0.11 * t for t in T_m]
    setpoints_ub_80 = [sp + 3.5 for sp in setpoints]
    setpoints_lb_80 = [sp - 3.5 for sp in setpoints]
    setpoints_ub_90 = [sp + 2.5 for sp in setpoints]
    setpoints_lb_90 = [sp - 2.5 for sp in setpoints]

    return setpoints, setpoints_ub_80, setpoints_lb_80, setpoints_ub_90, setpoints_lb_90


if __name__ == "__main__":
    T_m = [-4.0, 1.0, 4.4, 6.6, 14.0, 25.0, 18.0, 10.0, 5.0, 1.0, 0.0, -0.4]
    [sp, ub80, lb80, ub90, lb90] = adaptive_comfort(T_m)
    print(sp)
    print(ub80)
    print(lb80)
    print (ub90)
    print (lb90)
