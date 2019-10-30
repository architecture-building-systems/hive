"""
Calculating hourly electricity generation by PV system, given
    - ambient temperature
    - solar irradiance
    - PV system parameters
    - PV system area
"""


def main(A, eta_PVref, beta, NOCT, NOCT_ref, NOCT_sol, T_amb, I):
    return pv_yield(A, eta_PVref, beta, NOCT, NOCT_ref, NOCT_sol, T_amb, I)


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


def pv_yield(A, eta_PVref, beta, NOCT, NOCT_ref, NOCT_sol, T_amb, I):
    eta_pv = pv_efficiency(eta_PVref, beta, NOCT, NOCT_ref, NOCT_sol, T_amb, I)
    pvyield = [0.0] * len(eta_pv)
    for i in range(0, len(eta_pv)):
        pvyield[i] = A * eta_pv[i]
    return pvyield