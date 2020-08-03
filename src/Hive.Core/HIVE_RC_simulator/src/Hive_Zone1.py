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
ghenv.Component.Message = 'VER 0.0.1\nMAY_29_2018'
ghenv.Component.IconDisplayMode = ghenv.Component.IconDisplayMode.application
ghenv.Component.Category = "Hive"
ghenv.Component.SubCategory = "1 | Zone"
# ComponentExposure=2

import Grasshopper.Kernel as gh
import scriptcontext as sc
import re


def read_parameter_dictionary(parameter_dictionary, zone_attributes):
   #Clean dirty string
    parameter_dictionary = ' '.join(parameter_dictionary.split())
    if parameter_dictionary[-1] == ',':
        parameter_dictionary = parameter_dictionary[:-1]
    parameter_dictionary = parameter_dictionary.split(',')

    print 'parameter dictionary detected'
    for line in parameter_dictionary:
        
        equal = line.find('=')
        key = line[:equal]
        if re.findall(r'\d+',line) != []:
            # value contains digits
            comma = line.find(',')
            semicolon = line.find(';')
            if comma != -1:
                value = line[equal+1:comma]
            if semicolon!= -1:
                value = line[equal+1:semicolon]
            else:
                value = line[equal+1:]
            value = float(value)
        elif '-inf' in line:
            value = -float('inf')
        elif 'np.inf' in line:
            value = float('inf')
        else:
            # Supply and emission systems
            fullstop = line.find('.')
            comma = line.find(',')
            if comma!=-1:
                value = sc.sticky[line[fullstop+1:comma]]
            else:
                value = sc.sticky[line[fullstop+1:]]
        if key in zone_attributes.keys():
            zone_attributes[key] = value
    return zone_attributes

def main(parameter_dictionary,local_inputs):
    if not sc.sticky.has_key('ThermalZone'): return "Add the modular RC component to the canvas!"
    
    #  Set default values if no input is detected
    zone_attributes = {'room_width': 5.0, 
                       'room_depth': 7.0, 
                       'room_height': 3.0, 
                       'window_area': 4.0, 
                       'external_envelope_area': 15.0, 
                       'u_walls': 0.2, 
                       'u_windows': 1.1,
                       'lighting_maintenance_factor': 0.9,
                       'thermal_capacitance_per_floor_area': 165000, 
                       'lighting_load': 11.7, 
                       'lighting_control': 300.0,
                       'ach_infl': 0.5,
                       'ach_vent': 1.5,
                       'ventilation_efficiency': 0.6, 
                       't_set_cooling': 26.0, 
                       'lighting_utilisation_factor': 0.455, 
                       'max_heating_energy_per_floor_area': 10000, 
                       'max_cooling_energy_per_floor_area': -10000, 
                       't_set_heating': 20.0, 
                       'heating_supply_system': sc.sticky["OilBoilerNew"], 
                       'cooling_supply_system': sc.sticky["HeatPumpAir"], 
                       'heating_emission_system': sc.sticky["OldRadiators"], 
                       'cooling_emission_system': sc.sticky["AirConditioning"], 
                       }
    
    if parameter_dictionary is not None:
        zone_attributes = read_parameter_dictionary(parameter_dictionary,zone_attributes)
    
    for l in local_inputs.keys():
        zone_attributes[l] = local_inputs[l]
    
    #Initialise zone object
    Zone = sc.sticky["RCModelClassic"](
         window_area=zone_attributes['window_area'],
         external_envelope_area=zone_attributes['external_envelope_area'],
         room_depth=zone_attributes['room_depth'],
         room_width=zone_attributes['room_width'],
         room_height=zone_attributes['room_height'],
         lighting_load=zone_attributes['lighting_load'],
         lighting_control=zone_attributes['lighting_control'],
         lighting_utilisation_factor=zone_attributes['lighting_utilisation_factor'],
         lighting_maintenance_factor=zone_attributes['lighting_maintenance_factor'],
         u_walls=zone_attributes['u_walls'],
         u_windows=zone_attributes['u_windows'],
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
    
    
    input_string = 'Building('
    for k,v in zone_attributes.iteritems():
        if 'supply' in k:
            v = 'supply_system.'+str(zone_attributes[k])[9:]
        if 'emission' in k:
            v = 'emission_system.'+str(zone_attributes[k])[9:]
        input_string += k
        input_string += '='
        input_string += str(v)
        input_string += ', \n'
    input_string += ')'
    
    return Zone, input_string

""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""
# individual inputs override defaults and parameter_dictionary
local_inputs = {}
inputs = ['window_area','external_envelope_area','room_depth',
                  'room_width', 'room_height','lighting_load','lighting_control',
                  'lighting_utilisation_factor',  'lighting_maintenance_factor',
                  'u_walls', 'u_windows', 'g_windows', 'ach_vent', 'ach_infl',
                  'ventilation_efficiency','thermal_capacitance_per_floor_area',
                  't_set_heating', 't_set_cooling', 'max_cooling_energy_per_floor_area',
                  'max_heating_energy_per_floor_area', 'heating_supply_system',  
                  'cooling_supply_system', 'heating_emission_system',
                  'cooling_emission_system']

locals_dict = locals().copy()
for l in locals_dict.keys():
    if l in inputs and locals_dict[l] is not None:
        local_inputs[l] = locals_dict[l]
        print 'User defined values detected for %s'%l


Zone, input_string = main(parameter_dictionary,local_inputs)