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

    setpoints = [i for i in range(len(T_m))]
    setpoints_ub_80 = [i for i in range(len(T_m))]
    setpoints_ub_90 = [i for i in range(len(T_m))]
    setpoints_lb_80 = [i for i in range(len(T_m))]
    setpoints_lb_90 = [i for i in range(len(T_m))]
    for i in range(len(T_m)):
        setpoints[i] = 21.5 + 0.11 * T_m[i]
        setpoints_ub_80[i] = setpoints[i] + 3.5
        setpoints_lb_80[i] = setpoints[i] - 3.5
        setpoints_ub_90[i] = setpoints[i] + 2.5
        setpoints_lb_90[i] = setpoints[i] - 2.5
    return setpoints, setpoints_ub_80, setpoints_lb_80, setpoints_ub_90, setpoints_lb_90
