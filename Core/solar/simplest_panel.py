"""
Calculates annual solar electricity yield using simplest equation from Energy and Climate Systems course FS2019:
E_PV = G x F_F x A x eta_PV x PR
"""


def main(G, F_F, A, eta_PV, PR):
    E_PV = G * F_F * A * eta_PV * PR
    return E_PV