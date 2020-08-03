"""
Co-generation Combined Heat and Power (CHP), time resolved

inputs:
    - htg_or_elec: string indicator whether we provide heating loads or electricity loads.
        E.g. if heating load is provided (i.e. this load must be met by the CHP),
        then the component will calculate the resulting electricity generation {'heating_in', 'elec_in'}
    - loads: heating or electricity loads, time resolved
    - eta (efficiency from gas to electricity), constant
    - htp (heat-to-power ratio), constant
    - fuel_cost, time resolved [CHF/kWh eq.]
    - fuel_emissions, time resolved [kgCO2/kWh eq.]

outputs (time resolved):
    - total_cost
    - total_carbon
    - htg_gen: generated heating energy
    - elec_gen: generated electricity
"""


def main(htg_or_elec, loads, eta, htp, fuel_cost, fuel_emissions):

    # horizon. get the shortest array, in case inputs are not consistent
    horizon = len(loads)
    if horizon > len(fuel_cost):
        horizon = len(fuel_cost)
    if horizon > len(fuel_emissions):
        horizon = len(fuel_emissions)

    # initialize empty arrays. Daren wants me to use .append
    htg_gen = []
    elec_gen = []
    total_carbon = []
    total_cost = []
    fuel = []

    # if we get heating loads in, we wanna know how much electricity is produced with the CHP
    if htg_or_elec == "heating_in":
        htg_gen = loads
        for i in range(horizon):
            elec_gen.append(htg_gen[i] * htp)
    else:
        elec_gen = loads
        for i in range(horizon):
            htg_gen.append(elec_gen[i] / htp)

    for i in range(horizon):
        fuel.append(elec_gen[i] / eta)
        total_cost.append(fuel[i] * fuel_cost[i])
        total_carbon.append(fuel[i] * fuel_emissions[i])

    return total_cost, total_carbon, htg_gen, elec_gen
