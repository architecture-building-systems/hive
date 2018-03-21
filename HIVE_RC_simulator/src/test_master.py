# This component runs unit tests on the RC model
#
# Nest: A energy simulation plugin developed by the A/S chair at ETH Zurich
#
# This component is based on tests\testRCmodel.py (accessed 2/22/2018) 
# in the RC_BuildingSimulator Github repository:
# https://github.com/architecture-building-systems/RC_BuildingSimulator
# Documentation is available on the project wiki.
#
# Author: Prageeth Jayathissa
# Adapted for Grasshopper by Justin Zarb <zarbj@student.ethz.ch>
#
# This file is part of Nest
#
# Licensing/Copyright and liability comments go here.
# <Copyright 2018, Architecture and Building Systems - ETH Zurich>
# <Licence: MIT>

"""
Use this component to run standard tests on the RC model within the GH environment.
This test ensures that the grasshopper component returns the same results as the python model.

As opposed to the python unit tests, where test parameters are defined within
the function for that specific test, all the parameters are declared within 
dictionaries at the beginning of the script.
-
Provided by Nest 0.0.1
    
    Args:
        Run: The index of the test to be run. 
    Returns:
        Zone: Zone to be tested
        outdoor_air_temperature: The outdoor air temperature for the hour being simulated
        previous_mass_temperature: The temperature of the mass node during the previous hour. This temperature represents the average temperature of the building envelope itself.
        internal_gains: internal heat gains for the hour being simulated in [Watts]
        solar_irradiation: Solar irradiation gains for the hour being simulated in [Watts]. Does not account for losses through window!
        illuminance: Illuminance after transmitting through the window [Lumens]
        occupancy: Occupancy for the timestep [people/hour/square_meter]
"""

ghenv.Component.Name = "Unit Test Master"
ghenv.Component.NickName = 'unit_test_master'
ghenv.Component.Message = 'VER 0.0.1\nFEB_28_2018'
ghenv.Component.IconDisplayMode = ghenv.Component.IconDisplayMode.application
ghenv.Component.Category = "Nest"
ghenv.Component.SubCategory = "Simulation"
#compatibleNestVersion = VER 0.0.1\nFEB_21_2018
try: ghenv.Component.AdditionalHelpFromDocStrings = "3"
except: pass

import scriptcontext as sc
import time

tests = {'t_out':[10,25,10,30,5,10,10,25,10,30,5,10,25,35,35,10,10],
         't_m_prev':[22,24,20,25,19,22,22,24,20,25,19,22,24,24,24,20,20],
         'solar_gains':[2000,4000,2000,5000,2000,2000,2000,4000,2000,5000,2000,
         2000,4000,4000,4000,2000,2000],
         'internal_gains':[10]*17,
         'ill':[44000]*17,
         'occ':[0.1]*17}
#Change illuminance for the two relevant tests
tests['ill'][5] = 4000
tests['ill'][10] = 14000

zones = [None,None,None,None,None,None,
         sc.sticky['RC_Zone'](ventilation_efficiency=0.66),
         sc.sticky['RC_Zone'](ventilation_efficiency=0.6),
         sc.sticky['RC_Zone'](ventilation_efficiency=0.6),
         sc.sticky['RC_Zone'](ventilation_efficiency=0.6),
         sc.sticky['RC_Zone'](ventilation_efficiency=0.6),
         sc.sticky['RC_Zone'](ventilation_efficiency=0.6),
         sc.sticky['RC_Zone'](cooling_supply_system=sc.sticky["HeatPumpAir"]),
         sc.sticky['RC_Zone'](cooling_supply_system=sc.sticky["HeatPumpAir"]),
         sc.sticky['RC_Zone'](cooling_supply_system=sc.sticky["HeatPumpWater"]),
         sc.sticky['RC_Zone'](ventilation_efficiency=0.6, 
                              heating_supply_system=sc.sticky["HeatPumpAir"]),
         sc.sticky['RC_Zone'](ventilation_efficiency=0.6, 
                              heating_supply_system=sc.sticky["HeatPumpWater"]),
         ]

sc.sticky['expected_results'] = {
    'name':['NoHVACNoLight','CoolingRequired','HeatingRequired',
            'MaxCoolingRequired', 'MaxHeatingRequired','lightingrequired',
            'NoHVACNoLight_infl','CoolingRequired_infl',
            'test_HeatingRequired_infl','MaxCoolingRequired_infl',
            'test_MaxHeatingRequired_infl','test_lightingrequired_infl',
            'HeatPumpCoolingRequiredHighCOP',
            'HeatPumpCoolingRequired',
            'WaterHeatPumpCoolingRequired',
            'AirHeatPump_HeatingRequired_infl',
            'WaterHeatPump_HeatingRequired_infl'],
    'mass_temperature':[22.33,25.15,20.46,26.49,19.39,22.33,22.44,25.15,20.46,
    26.48,19.51,22.43,25.15,25.33,25.33,20.46,20.46],    
    'energy_demand':[None,-264.75,328.09,-411.6,411.6,0,0,-296.65,9.1,-411.6,
    411.6,0,-264.75,-411.6,-411.6,9.1,9.1],    
    'lighting_demand':[0,None,0,0,0,401.31,0,0,0,0,401.31,0,None,0,0,0,0],    
    'heating_sys_electricity':[0,0,328.09,0,411.6,0,0,0,9.1,0,411.6,0,0,0,0,
    2.43,1.97],    
    'cooling_sys_electricity':[0,264.75,0,411.6,0,0,0,296.65,0,411.6,0,0,55.87,
    107.44,52.12,0,0],
    'cop':[None,None,None,None,None,None,None,None,None,None,None,None,4.74,
    3.83,7.9,3.75,4.62]
    }

number_of_tests= len(sc.sticky['expected_results']['name']) -1
print zones[run]
try:
    Zone = zones[run]
    outdoor_air_temperature = tests['t_out'][run]
    previous_mass_temperature = tests['t_m_prev'][run]
    internal_gains = tests['internal_gains'][run]
    solar_gains = tests['solar_gains'][run]
    illuminance = tests['ill'][run]
    occupancy = tests['occ'][run]
except TypeError:
    pass
