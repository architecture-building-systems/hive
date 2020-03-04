# coding=utf-8
"""
Calculates total thermal energy generation of a Solar Thermal Collector using simple equation from Energy and Climate Systems course FS2019:
Q_th = G * F_F * A * eta_K * R_V

Inputs:
- G - Global horizontal irradiation [kWh/m2]
- F_F - Form factor, also: Orientation factor [0.0, >1.0]
- A - Collector area [m2]
- eta_K - Thermal efficiency of collector [0.0, 1.0]
- R_V - Distribution loss coefficient [0.0, 1.0]

outputs:
- Q_th - Thermal energy generation [kWh]
"""


def simple_solar_thermal(G, F_F, A, eta_K, R_V):
    Q_th = G * F_F * A * eta_K * R_V
    return Q_th
