"""
should do same as boiler, but it has both electricity and heating output
So what are inputs / outputs then?
Maybe component that could work with both:
either input electricity, and outputs heating loads
or input heating load, and it tells how much electricity we also get

output for sure: cost and carbon

inputs:
    - eta (efficiency from gas to elec)
    - htp (heat-to-power ratio)
"""


def main(htg_or_elec, loads, eta, htp, fuel_cost, fuel_emissions):
    # either way, we wanna know cost and carbon emissions
    htg_gen = 0.0
    elec_gen = 0.0
    total_carbon = 0.0
    total_cost = 0.0
    fuel = 0.0

    # if we get heating loads in, we wanna know how much electricity is produced with the CHP
    if htg_or_elec == "heating_in":
        htg_gen = loads
        elec_gen = htg_gen * htp  # loads is heating demand here
    else:
        elec_gen = loads
        htg_gen = elec_gen / htp

    fuel = elec_gen / eta
    total_cost = fuel * fuel_cost
    total_carbon = fuel * fuel_emissions
    return total_cost, total_carbon, htg_gen, elec_gen
