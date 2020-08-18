"""
Time resolved boiler

arguments:
    - cost of carrier [CHF/kWh]
    - emmissions of carrier [kgCO2/kWheq.]
    - efficiency [-/h]
    - heating loads [kWh/h]

should return
    - cost [CHF/h]
    - carbon emissions [kgCO2eq./h]
"""


def main(heating_loads, carrier_cost, carrier_emissions, eta):
    horizon = len(heating_loads)
    if len(carrier_cost) < horizon:
        horizon = len(carrier_cost)
    if len(carrier_emissions) < horizon:
        horizon = len(carrier_emissions)
    if len(eta) < horizon:
        horizon = len(eta)

    total_cost = [0] * horizon
    total_emissions = [0] * horizon
    gas_consumed = [0] * horizon
    for t in range(horizon):
        gas_consumed[t] = heating_loads[t] * eta[t]
        total_cost[t] = gas_consumed[t] * carrier_cost[t]
        total_emissions[t] = gas_consumed[t] * carrier_emissions[t]
    return gas_consumed, total_cost, total_emissions
