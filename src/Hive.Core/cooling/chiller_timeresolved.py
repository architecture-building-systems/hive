# coding=utf-8
"""
Chiller (air con), time resolved
Eq. (A.8) from: 10.1016/j.apenergy.2019.03.177
found in: 10.1016/j.energy.2004.08.004

arguments:
    - cooling loads [kWh], time resolved
    - electricity cost [CHF/kWh], time resolved
    - electricity emissions [kgCO2/kWh eq.], time resolved
    - ambient temperature [°C], time resolved

outputs (all time resolved):
    - electricity loads [kWh]
    - total cost [CHF]
    - total carbon [kgCO2]
    - COP of chiller [-]
"""


def main(clg_load, elec_cost, elec_emissions, temperature):
    # parameters from Eq. (A.8) in 10.1016/j.apenergy.2019.03.177
    ac_1 = 638.95
    ac_2 = 4.238
    ac_3 = 100.0
    ac_4 = 3.534

    horizon = len(clg_load)
    if horizon > len(elec_cost):
        horizon = len(elec_cost)
    if horizon > len(elec_emissions):
        horizon = len(elec_emissions)
    if horizon > len(temperature):
        horizon = len(temperature)

    COP = []
    elec_load = []
    total_cost = []
    total_emissions = []
    for i in range(horizon):
        COP.append((ac_1 - ac_2 * temperature[i]) / (ac_3 + ac_4 * temperature[i]))
        elec_load.append(clg_load[i] / COP[i])
        total_cost.append(elec_load[i] * elec_cost[i])
        total_emissions.append(elec_load[i] * elec_emissions[i])

    return elec_load, total_cost, total_emissions, COP
