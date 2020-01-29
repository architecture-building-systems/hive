"""
Chiller (air con)
Eq. (A.8) from: 10.1016/j.apenergy.2019.03.177
found in: 10.1016/j.energy.2004.08.004

arguments:
    - cooling loads [kWh]
    - COP [-]
    - electricity cost [CHF/kWh]
    - electricity emissions [kgCO2/kWh eq.]

outputs:
    - electricity loads [kWh]
    - total cost [CHF]
    - total carbon [kgCO2]
"""


def main(clg_load, elec_cost, elec_emissions, COP):
    elec_load = clg_load / COP
    total_cost = elec_load * elec_cost
    total_emissions = elec_load * elec_emissions

    return elec_load, total_cost, total_emissions
