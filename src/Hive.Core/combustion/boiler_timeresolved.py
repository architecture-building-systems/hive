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
    for t in range(horizon):
        total_cost[t] = heating_loads[t] * eta[t] * carrier_cost[t]
        total_emissions[t] = heating_loads[t] * eta[t] * carrier_emissions[t]
    return total_cost, total_emissions
