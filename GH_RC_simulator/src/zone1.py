# This comoponent creates a basic default zone: the user is allowed 
# to override the default attributes and customize it.
#
# Nest: A energy simulation plugin developed by the A/S chair at ETH Zurich
# This component is based on building_physics.py in the RC_BuildingSimulator Github repository
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
Create a custom thermal zone. This method is more manual than the
element-based approach. Parameters left blank will be filled with default values.
-
Provided by Nest 0.0.1
    
    Args:
        window_area: Window area used to calculate losses.
        external_envelope_area: Area of all envelope surfaces, including windows in contact with the outside
        room_depth: Depth of the modelled room [m]
        room_width: Width of the modelled room [m] 
        room_height: Height of the modelled room [m]
        lighting_load: Lighting Load [W/m2] 
        lighting_control: Lux threshold at which the lights turn on [Lx]
        u_walls: U value of opaque surfaces  [W/m2K]
        u_windows: U value of glazed surfaces [W/m2K]
        ach_vent: Air changes per hour through ventilation [Air Changes Per Hour]
        ach_infl: Air changes per hour through infiltration [Air Changes Per Hour]
        ventilation_efficiency: The efficiency of the heat recovery system for ventilation. Set to 0 if there is no heat 
            recovery []
        thermal_capacitance_per_floor_area: Thermal capacitance of the room per floor area [J/m2K]
        t_set_heating : Thermal heating set point [C]
        t_set_cooling: Thermal cooling set point [C]
        max_cooling_energy_per_floor_area: Maximum cooling load. Set to -np.inf for unrestricted cooling [C]
        max_heating_energy_per_floor_area: Maximum heating load per floor area. Set to no.inf for unrestricted heating [C]
        heating_supply_system: The type of heating system. Choices are DirectHeater, ResistiveHeater, HeatPumpHeater. 
            Direct heater has no changes to the heating demand load, a resistive heater takes an efficiency into account, 
            HeatPumpHeatercalculates a COP based on the outdoor and system supply temperature 
        cooling_supply_system: The type of cooling system. Choices are DirectCooler HeatPumpCooler. 
            DirectCooler has no changes to the cooling demand load, 
            HeatPumpCooler calculates a COP based on the outdoor and system supply temperature 
        heating_emission_system: How the heat is distributed to the building
        cooling_emission_system: How the cooling energy is distributed to the building
    Returns:
        readMe!: ...
        Zone: A Zone object described by the args.
        unique_inputs: a dictionary of non-default inputs, which can be used to reproduce results in python
        zone_variables: variables of the Zone object, for diagnostics.
        
"""

ghenv.Component.Name = "Zone1"
ghenv.Component.NickName = 'Zone1'
ghenv.Component.Message = 'VER 0.0.1\nMAR_06_2018'
ghenv.Component.IconDisplayMode = ghenv.Component.IconDisplayMode.application
ghenv.Component.Category = "Nest"
ghenv.Component.SubCategory = " 1 | Zone"
#compatibleNestVersion = VER 0.0.1\nFEB_21_2018
try: ghenv.Component.AdditionalHelpFromDocStrings = "1"
except: pass

import Grasshopper.Kernel as gh
import scriptcontext as sc

#  Initialize default values if no input is detected
zone_attributes = ['window_area','external_envelope_area','room_depth',
                  'room_width', 'room_height','lighting_load','lighting_control',
                  'lighting_utilisation_factor',  'lighting_maintenance_factor',
                  'u_walls', 'u_windows', 'g_windows', 'ach_vent', 'ach_infl',
                  'ventilation_efficiency','thermal_capacitance_per_floor_area',
                  't_set_heating', 't_set_cooling', 'max_cooling_energy_per_floor_area',
                  'max_heating_energy_per_floor_area', 'heating_supply_system',  
                  'cooling_supply_system', 'heating_emission_system',
                  'cooling_emission_system']

default_values = [4.0,15.0,7.0,5.0,3.0,11.7,300.0,0.455,0.9,0.2,1.1,0.6,1.5,0.5,
                  0.6,165000,20.0,26.0,-float("inf"),float("inf"),
                  sc.sticky["OilBoilerNew"],sc.sticky["HeatPumpAir"],
                  sc.sticky["OldRadiators"],sc.sticky["AirConditioning"]]

default_attributes = {}
for a,v in zip(zone_attributes,default_values):
    default_attributes[a] = v

unique_inputs = {}
for t in default_attributes.keys():
#    print locals()[t]
    if locals()[t] is not None:
        # replace the default value with the value given to the component
        default_attributes[t] = locals()[t]
        # Generate a useful text output which can be used as an input in python
        if 'supply' not in t and 'emission' not in t:
            value = t+':'+str(locals()[t])
        elif 'supply' in t:
            value = t+': supply_system.'+str(locals()[t])[22:-2]
        elif 'emission' in t:
            value = t+': emission_system.'+str(locals()[t])[22:-2]
        unique_inputs[t] = value


#Initialise zone object
Zone = sc.sticky["RC_Zone"](
     window_area=default_attributes['window_area'],
     external_envelope_area=default_attributes['external_envelope_area'],
     room_depth=default_attributes['room_depth'],
     room_width=default_attributes['room_width'],
     room_height=default_attributes['room_width'],
     lighting_load=default_attributes['lighting_load'],
     lighting_control=default_attributes['lighting_control'],
     lighting_utilisation_factor=default_attributes['lighting_utilisation_factor'],
     lighting_maintenance_factor=default_attributes['lighting_maintenance_factor'],
     u_walls=default_attributes['u_walls'],
     u_windows=default_attributes['u_windows'],
     g_windows=default_attributes['g_windows'],
     ach_vent=default_attributes['ach_vent'],
     ach_infl=default_attributes['ach_infl'],
     ventilation_efficiency=default_attributes['ventilation_efficiency'],
     thermal_capacitance_per_floor_area=default_attributes['thermal_capacitance_per_floor_area'],
     t_set_heating=default_attributes['t_set_heating'],
     t_set_cooling=default_attributes['t_set_cooling'],
     max_cooling_energy_per_floor_area=default_attributes['max_cooling_energy_per_floor_area'],
     max_heating_energy_per_floor_area=default_attributes['max_heating_energy_per_floor_area'],
     heating_supply_system=default_attributes['heating_supply_system'],  
     cooling_supply_system=default_attributes['cooling_supply_system'],
     heating_emission_system=default_attributes['heating_emission_system'],
     cooling_emission_system=default_attributes['cooling_emission_system'])

zone_variables = vars(Zone)

for i in unique_inputs:
    print i,unique_inputs[i]

