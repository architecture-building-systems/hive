"""
reads in a Hive.IO.EnergySystems.Solar (Energy Carrier) and infuses Hive.IO.EnSys.Photovoltaic with output electricity
"""

from __future__ import division
import System
import Grasshopper as gh
path = gh.Folders.AppDataFolder
import clr
import os
clr.AddReferenceToFileAndPath(os.path.join(path, "Libraries\hive", "Hive.IO.gha"))
import Hive.IO.EnergySystems as ensys
import Rhino.RhinoApp as RhinoApp

def pv_electricity(pv, solar_carrier, time_resolution, amb_T_carrier, beta, NOCT, NOCT_ref, NOCT_sol):
    electricity_generated = pv_yield(pv.SurfaceArea, pv.RefEfficiencyElectric, beta, NOCT, NOCT_ref, NOCT_sol, amb_T_carrier.AvailableEnergy, solar_carrier.AvailableEnergy)
    electricity_horizon = []
    if time_resolution == "hourly":
        horizon = 8760
        electricity_horizon = electricity_generated[0]
    else:   # monthly
        horizon = 12
        days_per_month = [31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31]
        hours_per_day = 24
        total_months = 12
        for month in range(total_months):
            start_hour = int(hours_per_day * sum(days_per_month[0:month]))
            end_hour = int(hours_per_day * sum(days_per_month[0:month + 1]))
            # hours_per_month = days_per_month[month] * hours_per_day # could be used to compute average values
            electricity_horizon.append(sum(electricity_generated[0][start_hour:end_hour]))

    electricity_carrier = ensys.Electricity(horizon, System.Array[float](electricity_horizon), System.Array[float]([0.0] * horizon), System.Array[float]([0.0] * horizon))
    pv.SetInputOutput(solar_carrier, electricity_carrier)
    return pv


def pv_yield(A, eta_PVref, beta, NOCT, NOCT_ref, NOCT_sol, T_amb, I):
    """
    Computes PV electricity yield
    :param A:
    :param eta_PVref:
    :param beta:
    :param NOCT:
    :param NOCT_ref:
    :param NOCT_sol:
    :param T_amb:
    :param I:
    :return:
    """
    eta_pv = pv_efficiency(eta_PVref, beta, NOCT, NOCT_ref, NOCT_sol, T_amb, I)
    pvyield = [0.0] * len(eta_pv)
    for i in range(0, len(eta_pv)):
        pvyield[i] = A * eta_pv[i] * I[i]
    return pvyield, eta_pv


def pv_efficiency(eta_PVref, beta, NOCT, NOCT_ref, NOCT_sol, T_amb, I):
    """
    Calculates time resolved PV efficiency [-]
    :param eta_PVref: Reference PV efficiency under NOCT [-]
    :param beta: Temperature coefficient [-]
    :param NOCT: Nominal operating cell temperature [deg C]
    :param NOCT_ref: Reference temperature [deg C]
    :param NOCT_sol: Reference irradiance [W/m2]
    :param T_amb: Ambient temperature [deg C]
    :param I: Irradiance on panel [W/m2]. 8760 time series
    :return: Time resolved PV efficiency [-], 8760 entries
    """
    horizon = len(T_amb)
    etapv = [0.0] * horizon
    for i in range(0, horizon):
        Tpv = T_amb[i] + ((NOCT - NOCT_ref) / NOCT_sol) * I[i]
        etapv[i] = eta_PVref * (1 - beta * (Tpv - 25))
    return etapv


