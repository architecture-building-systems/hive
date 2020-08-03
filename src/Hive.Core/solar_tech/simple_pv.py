# coding=utf-8
"""
Calculates total solar electricity yield using simple equation from Energy and Climate Systems course FS2019:
E_PV = G * F_F * A * eta_PV * PR

Inputs:
- G - Global horizontal irradiation [kWh/m2]
- F_F - Form factor, also: Orientation factor [0.0, >1.0]
- A - PV area [m2]
- eta_PV - PV module efficiency [0.0, 1.0]
- PR - Performance ratio [0.0, 1.0]

outputs:
- E_PV - PV electricity generation [kWh]
"""


def simple_pv(G, F_F, A, eta_PV, PR):
    E_PV = G * F_F * A * eta_PV * PR
    return E_PV
