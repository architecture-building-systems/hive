"""
could be gas, oil, bio boiler
because its just efficiency times fuel [kWh]

arguments:
    - cost of carrier [CHF/kWh]
    - emmissions of carrier [kgCO2/kWheq.]
    - efficiency [-]
    - heating loads [kWh]

should return
    - cost [CHF]
    - carbon emissions [kgCO2eq.]
"""


def main(heating_loads, carrier_cost, carrier_emissions, eta):
    final_energy = heating_loads * eta
    total_cost = final_energy * carrier_cost
    total_emissions = final_energy * carrier_emissions
    return total_cost, total_emissions
