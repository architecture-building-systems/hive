# This comoponent creates a basic default zone: the user is allowed 
# to override the default attributes and customize it.
#
# Hive: A energy simulation plugin developed by the A/S chair at ETH Zurich
# This component is based on building_physics.py in the RC_BuildingSimulator Github repository
# https://github.com/architecture-building-systems/RC_BuildingSimulator
# Extensive documentation is available on the project wiki.
#
# Author: Justin Zarb <zarbj@student.ethz.ch>
#
# This file is part of Hive
#
# Licensing/Copyright and liability comments go here.
# <Copyright 2018, Architecture and Building Systems - ETH Zurich>
# <Licence: MIT>

"""
Create a custom thermal zone in the format used in the main branch of 
RC_BuildingSimulator. Parameters left blank will be replaced by defaults.
-
Provided by Hive 0.0.1
    
    Args:
        parameter_dictionary: a multiline list of building parameters in the following format: 'key = value'
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
        input_string: A string which can be copy-pased into a python script running the rc-model
        zone_variables: variables of the Zone object, for diagnostics.
        
"""

ghenv.Component.Name = "Hive_Zone1"
ghenv.Component.NickName = 'Zone1'
ghenv.Component.Message = 'VER 0.0.1\nMAY_09_2018'
ghenv.Component.IconDisplayMode = ghenv.Component.IconDisplayMode.application
ghenv.Component.Category = "Hive"
ghenv.Component.SubCategory = "1 | Zone"
# ComponentExposure=2

import Grasshopper.Kernel as gh
import scriptcontext as sc
import re

#  Initialize default values if no input is detected
attribute_names = ['window_area','external_envelope_area','room_depth',
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

zone_attributes = dict(zip(attribute_names,default_values))

# override defaults with values detected from parameter_dictionary 
if parameter_dictionary is not []:
    print 'parameter dictionary detected'
    for line in parameter_dictionary:
        equal = line.find('=')
        key = line[:equal]
        value = line[equal+1:]
        if re.findall(r'\d+',value) != []:
            comma = value.find(',')
            semicolon = value.find(';')
            if comma:
                    value = value[:comma]
            if semicolon:
                value = value[:semicolon]
            value = float(value)
        elif '-np.inf' in value:
            value = -100000.0
        elif 'np.inf' in value:
            value = 100000.0
        else:
            fullstop = value.find('.')
            comma = value.find(',')
            value = sc.sticky[value[fullstop+1:comma]]
        
        if key in attribute_names:
            zone_attributes[key] = value

# individual inputs override defaults and parameter_dictionary
for t in zone_attributes.keys():
#    print locals()[t]
    if locals()[t] is not None:
        print 'value detected for',t
        # replace the default value with the value given to the component
        zone_attributes[t] = locals()[t]
        

# Generate a useful text output which can be used as an input in python
locally_defined_values = {}
for k,v in zip(attribute_names,default_values):
    if zone_attributes[k] != v:
        if 'supply' not in k and 'emission' not in k:
            locally_defined_values[k] = zone_attributes[k]
        elif 'supply' in k:
            locally_defined_values[k] = 'supply_system.'+str(zone_attributes[k])[9:]
        elif 'emission' in k:
            locally_defined_values[k] = 'emission_system.'+str(zone_attributes[k])[9:]

#Initialise zone object
Zone = sc.sticky["RCModelClassic"](
     window_area=zone_attributes['window_area'],
     external_envelope_area=zone_attributes['external_envelope_area'],
     room_depth=zone_attributes['room_depth'],
     room_width=zone_attributes['room_width'],
     room_height=zone_attributes['room_width'],
     lighting_load=zone_attributes['lighting_load'],
     lighting_control=zone_attributes['lighting_control'],
     lighting_utilisation_factor=zone_attributes['lighting_utilisation_factor'],
     lighting_maintenance_factor=zone_attributes['lighting_maintenance_factor'],
     u_walls=zone_attributes['u_walls'],
     u_windows=zone_attributes['u_windows'],
     g_windows=zone_attributes['g_windows'],
     ach_vent=zone_attributes['ach_vent'],
     ach_infl=zone_attributes['ach_infl'],
     ventilation_efficiency=zone_attributes['ventilation_efficiency'],
     thermal_capacitance_per_floor_area=zone_attributes['thermal_capacitance_per_floor_area'],
     t_set_heating=zone_attributes['t_set_heating'],
     t_set_cooling=zone_attributes['t_set_cooling'],
     max_cooling_energy_per_floor_area=zone_attributes['max_cooling_energy_per_floor_area'],
     max_heating_energy_per_floor_area=zone_attributes['max_heating_energy_per_floor_area'],
     heating_supply_system=zone_attributes['heating_supply_system'],  
     cooling_supply_system=zone_attributes['cooling_supply_system'],
     heating_emission_system=zone_attributes['heating_emission_system'],
     cooling_emission_system=zone_attributes['cooling_emission_system'])

zone_variables = vars(Zone)

input_string = 'Building('
for k,v in locally_defined_values.iteritems():
    input_string += k
    input_string += '='
    input_string += str(v)
    input_string += ', '
input_string += ')'

