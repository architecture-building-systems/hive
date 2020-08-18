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
    gas_consumed = heating_loads * eta
    total_cost = gas_consumed * carrier_cost
    total_emissions = gas_consumed * carrier_emissions
    return gas_consumed, total_cost, total_emissions
