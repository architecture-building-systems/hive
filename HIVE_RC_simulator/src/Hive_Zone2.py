# This comoponent creates a zone made of elements: the user is allowed 
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
Create a customized zone using elements as inputs.
Parameters left blank will be filled with default values.
-
Provided by Hive 0.0.1
    
    Args:
        glazed_elements: Element objects with additional glazing properties
        opaque_elements: Element objects
        thermal_bridges: Linear thermal bridge objects
        floor_area: [default=] The conditioned floor area within the zone
        zone_volume: [default=] Volume of the zone being simulated [m^2]
        thermal_capacitance_per_floor_area: [default=165000] Thermal capacitance of the room per 
            floor area [J/m2K]. Lightweight = 18000, medium weight = 165000, heavyweight = 360000
        lighting_load: [default=11.7] Lighting Load [W/m2] 
        lighting_control: [default=300] Lux threshold at which the lights turn on [Lx]
        lighting_utilization_factor: [default=0.45] How the light entering the window is 
            transmitted to the working plane
        lighting_maintenance_factor: [default=0.9] How dirty the window is
        ach_vent: [default= 1.5] Air changes per hour through ventilation [Air Changes Per Hour]
        ach_infl: [default= 0.5]Air changes per hour through infiltration [Air Changes Per Hour]
        ventilation_efficiency: [default=1] The efficiency of the heat recovery system for ventilation. Set to 0 if there is no heat 
            recovery.
        t_set_heating : [default=20] Thermal heating set point [C]
        t_set_cooling: [default=26] Thermal cooling set point [C]
        max_cooling_energy_per_floor_area: [default=12] Maximum cooling load. Set to -np.inf for unrestricted cooling [C]
        max_heating_energy_per_floor_area: [default=-12] Maximum heating load per floor area. Set to no.inf for unrestricted heating [C]
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
        Zone: ModularRCZone object
        Zone1_string_: a string which can be pasted into a Python script to test the Building object
        Zone2_string_: a string which can be pasted into a Python script to test the ElementBuilding object
        
"""

ghenv.Component.Name = "Hive_Zone2"
ghenv.Component.NickName = 'Zone2'
ghenv.Component.Message = 'VER 0.0.1\nMAY_17_2018'
ghenv.Component.IconDisplayMode = ghenv.Component.IconDisplayMode.application
ghenv.Component.Category = "Hive"
ghenv.Component.SubCategory = "1 | Zone"
# ComponentExposure=2

import scriptcontext as sc
import Grasshopper.Kernel as gh
import Grasshopper.Kernel as ghKernel
HivePreparation = sc.sticky['HivePreparation']()
import math

""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""
def main(elements,thermal_bridges,thermal_attributes,lighting_attributes):
    if not sc.sticky.has_key('ThermalZone'): return "Add the modular RC component to the canvas!"
    
    #Declare zone
    ThermalZone = sc.sticky['ThermalZone'](elements = elements,
                             thermal_bridges = thermal_bridges,
                             floor_area = thermal_attributes['floor_area'],
                             volume = thermal_attributes['volume'],
                             thermal_capacitance_per_floor_area=thermal_attributes['thermal_capacitance_per_floor_area'],
                             ach_vent=thermal_attributes['ach_vent'],
                             ach_infl=thermal_attributes['ach_infl'],
                             ventilation_efficiency=thermal_attributes['ventilation_efficiency'],
                             t_set_heating = thermal_attributes['t_set_heating'],
                             t_set_cooling = thermal_attributes['t_set_cooling'],
                             max_heating_energy_per_floor_area = thermal_attributes['max_heating_energy_per_floor_area'],
                             max_cooling_energy_per_floor_area = thermal_attributes['max_cooling_energy_per_floor_area'],
                             heating_supply_system=thermal_attributes['heating_supply_system'],
                             cooling_supply_system=thermal_attributes['cooling_supply_system'],
                             heating_emission_system=thermal_attributes['heating_emission_system'],
                             cooling_emission_system=thermal_attributes['cooling_emission_system'],
                             )
    ThermalZone.summary()
    
    # Zone with thermal and lighting attributes
    Zone = sc.sticky['RCModel'](zone=ThermalZone,                 
                  lighting_load=lighting_attributes['lighting_load'],
                  lighting_control=lighting_attributes['lighting_control'],
                  lighting_utilisation_factor=lighting_attributes['lighting_utilisation_factor'],
                  lighting_maintenance_factor=lighting_attributes['lighting_maintenance_factor'])
    
    return Zone

def raise_error(error_str):
    error = error_str
    e = ghKernel.GH_RuntimeMessageLevel.Error
    ghenv.Component.AddRuntimeMessage(e, error)
        
def raise_warning(warning_str):
    warning = warning_str
    w = ghKernel.GH_RuntimeMessageLevel.Warning
    ghenv.Component.AddRuntimeMessage(w, warning)

def zone_object_string(Zone,unique_inputs):
    
    room_depth = math.sqrt(Zone.floor_area)
    room_width = room_depth
    room_height = volume/Zone.floor_area
    if 'elements' in unique_inputs.keys():
        window_area =  sum([e.area for e in unique_inputs['elements'] if not e.opaque])
        external_envelope_area =  sum([e.area for e in unique_inputs['elements']])
        wall_area = sum([e.area for e in unique_inputs['elements'] if e.opaque])
        u_windows = sum([e.area * e.u_value for e in unique_inputs['elements'] if not e.opaque])/window_area
        u_walls = sum([e.area * e.u_value for e in unique_inputs['elements'] if e.opaque])/wall_area
    
    classic_zone_inputs = {'window_area':window_area,
                           'external_envelope_area':external_envelope_area,
                           'u_windows':u_windows,
                           'u_walls':u_walls}
    for k in unique_inputs:
        if k not in ['floor_area','thermal_bridges','elements','volume']:
            classic_zone_inputs[k] = unique_inputs[k]

    zone1 = 'Building('
    for k,v in classic_zone_inputs.iteritems():
        zone1 += k
        zone1 += '='
        zone1 += str(v)
        zone1 += ', '
    zone1 += ')'
    
    zone2 = 'ElementBuilding('
    for k,v in unique_inputs.iteritems():
        zone2 += k
        zone2 += '='
        zone2 += str(v)
        zone2 += ', '
    zone2 += ')'

    return zone1, zone2
    
""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""

#  Initialize default values which will be used if no input is detected
thermal_attributes = {"elements":None,
              "thermal_bridges":None,
              "floor_area":34.3,
              "volume":106.33,
              "thermal_capacitance_per_floor_area":165000,
              "ach_vent":1.5,
              "ach_infl":0.5,
              "ventilation_efficiency":1,
              "t_set_heating":20,
              "t_set_cooling":26,
              "max_heating_energy_per_floor_area":12,
              "max_cooling_energy_per_floor_area":-12,
              "heating_supply_system":sc.sticky["DirectHeater"],
              "cooling_supply_system":sc.sticky["DirectCooler"],
              "heating_emission_system":sc.sticky["AirConditioning"],
              "cooling_emission_system":sc.sticky["AirConditioning"]
              }
lighting_attributes = {'lighting_load':11.7,
                       'lighting_control':300.0,
                       'lighting_utilisation_factor':0.45,
                       'lighting_maintenance_factor':0.9}


# Replace default values with whatever is inputted to the component
unique_inputs = {}
for t in thermal_attributes.keys():
    if locals()[t] is not None:
        thermal_attributes[t] = locals()[t]
        # Add item to unique_inputs
        if 'supply' not in t and 'emission' not in t:
            value = locals()[t]
        elif 'supply' in t:
            value = t+': supply_system.'+str(locals()[t])[22:-2]
        elif 'emission' in t:
            value = t+': emission_system.'+str(locals()[t])[22:-2]
        unique_inputs[t] = value

# Add lighting attributes
for l in lighting_attributes.keys():
    if locals()[l] is not None:
        lighting_attributes[l] = locals()[l]
        value = l+':'+str(locals()[l])
        unique_inputs[l] = value

# Initialise thermal bridges
t = [x for x in thermal_bridges if x is sc.sticky['ThermalBridge']]
if len(t) != len(thermal_bridges):
    raise_error("Invalid thermal bridge detected")
if len(t) == 0:
    thermal_bridges = None

# Initialise elements
if any([type(e).__name__!='Element' for e in elements]):
    raise_error('Invalid Element input')
elif len(elements) == 0:
    Zone = main(None,thermal_bridges,thermal_attributes,lighting_attributes)
    Zone1_string_, Zone2_string_ = zone_object_string(Zone,unique_inputs)
else:
    Zone = main(elements,thermal_bridges,thermal_attributes,lighting_attributes)
    Zone1_string_, Zone2_string_ = zone_object_string(Zone,unique_inputs)