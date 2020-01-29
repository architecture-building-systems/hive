"""
Chiller (air con)
Simple equation: electricity loads = cooling loads / COP

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
