# This component can be used to simulate a single timestep, which is mostly useful for testing and debugging.
#
# Nest: A energy simulation plugin developed by the A/S chair at ETH Zurich
# This component is based on examples\hourSimulation.py in the RC_BuildingSimulator Github repository
# https://github.com/architecture-building-systems/RC_BuildingSimulator
# Extensive documentation is available on the project wiki.
#
# Author: Justin Zarb <zarbj@student.ethz.ch>
#
# This file is part of Nest
#
# Licensing/Copyright and liability comments go here.
# <Copyright 2018, Architecture and Building Systems - ETH Zurich>
# <Licence: MIT>

"""
Use this component to simulate a single timestep. You may add a custom zone or leave the Zone arg blank for the default zone.
-
Provided by Nest 0.0.1
    
    Args:
        Zone: Input a customized Zone from the Zone component.
        outdoor_air_temperature: The outdoor air temperature for the hour being simulated
        previous_mass_temperature: The temperature of the mass node during the previous hour. This temperature represents the average temperature of the building envelope itself.
        internal_gains: internal heat gains for the hour being simulated in [Watts]
        solar_gains: Solar irradiation gains for the hour being simulated in [Watts]. Does not account for losses through window!
        illuminance: Illuminance after transmitting through the window [Lumens]
        occupancy: Occupancy for the timestep [people/hour/square_meter]
    Returns:
        readMe!: ...
        indoor_air_temperature: Indoor air temperature for the given time step
        mass_temperature: The mass node temperature for the hour simulated
        lighting_demand: lighting energy demand for the given time step
        heating_demand: heating energy demand required to maintain the heating setpoint temperature defined in the Zone.
        heating_sys_electricity: Heating electricity consumption
        cooling_demand: cooling energy demand required to maintain the cooling setpoint temperature defined in the Zone.
        cooling_sys_electricity: Cooling electricity consumption
        energy_demand: Sum of heating, cooling and lighting demand for the given timestep.
        cop: coefficient of performance of the heating/cooling system for this hour
 
"""

ghenv.Component.Name = "Simulate a Single Time Step"
ghenv.Component.NickName = 'SimulateOneTimeStep'
ghenv.Component.Message = 'VER 0.0.1\nFEB_28_2018'
ghenv.Component.IconDisplayMode = ghenv.Component.IconDisplayMode.application
ghenv.Component.Category = "Nest"
ghenv.Component.SubCategory = "Simulation"
#compatibleNestVersion = VER 0.0.1\nFEB_21_2018
try: ghenv.Component.AdditionalHelpFromDocStrings = "1"
except: pass

import Grasshopper.Kernel as gh
import scriptcontext as sc

inputs = {'outdoor_air_temperature':outdoor_air_temperature,
        'previous_mass_temperature':previous_mass_temperature,
        'internal_gains':internal_gains,
        'solar_gains':solar_gains,
        'illuminance':illuminance,
        'occupancy':occupancy,
        'Zone':Zone}

defaults = {'outdoor_air_temperature':10,
        'previous_mass_temperature':20,
        'internal_gains':10,
        'solar_gains':2000,
        'illuminance':44000,
        'occupancy':0.1,
        'Zone':sc.sticky["RC_Zone"]()}

# Initialise parameters
def assign_default_values(input):
    try:
        assert inputs[input] is not None
    except:
        inputs[input] = defaults[input]
    print input,inputs[input]

for i in inputs.keys():
    assign_default_values(i)

Zone = inputs['Zone']
Zone.solve_building_energy(inputs['internal_gains'], 
                           inputs['solar_gains'],
                           inputs['outdoor_air_temperature'],
                           inputs['previous_mass_temperature'])

Zone.solve_building_lighting(inputs['illuminance'],
                             inputs['occupancy'])

indoor_air_temperature = Zone.t_air
mass_temperature = Zone.t_m  # Printing Room Temperature of the medium
lighting_demand =  Zone.lighting_demand  # Print Lighting Demand
energy_demand = Zone.energy_demand  # Print heating/cooling loads
heating_demand = Zone.heating_demand
heating_sys_electricity = Zone.heating_sys_electricity
cooling_demand = Zone.cooling_demand
cooling_sys_electricity = Zone.cooling_sys_electricity
try:
    cop = Zone.cop
except AttributeError:
    cop = None