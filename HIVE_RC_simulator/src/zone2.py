# This comoponent creates a zone made of elements: the user is allowed 
# to override the default attributes and customize it.
#
# Oasys: A energy simulation plugin developed by the A/S chair at ETH Zurich
# This component is based on building_physics.py in the RC_BuildingSimulator Github repository
# https://github.com/architecture-building-systems/RC_BuildingSimulator
# Extensive documentation is available on the project wiki.
#
# Author: Justin Zarb <zarbj@student.ethz.ch>
#
# This file is part of Oasys
#
# Licensing/Copyright and liability comments go here.
# <Copyright 2018, Architecture and Building Systems - ETH Zurich>
# <Licence: MIT>

"""
Create a customized zone using elements as inputs.
Parameters left blank will be filled with default values.
-
Provided by Oasys 0.0.1
    
    Args:
        glazed_elements: Element objects with additional glazing properties
        opaque_elements: Element objects
        thermal_bridges: Linear thermal bridge objects
        floor_area: The conditioned floor area within the zone
        zone_volume: Volume of the zone being simulated [m^2]
        thermal_capacitance_per_floor_area: Thermal capacitance of the room per 
            floor area [J/m2K]
        lighting_load: Lighting Load [W/m2] 
        lighting_control: Lux threshold at which the lights turn on [Lx]
        lighting_utilization_factor: How the light entering the window is 
            transmitted to the working plane []
        lighting_maintenance_factor: How dirty the window is[]
        ach_vent: Air changes per hour through ventilation [Air Changes Per Hour]
        ach_infl: Air changes per hour through infiltration [Air Changes Per Hour]
        ventilation_efficiency: The efficiency of the heat recovery system for ventilation. Set to 0 if there is no heat 
            recovery []
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
        Zone: ModularRCZone object
        local_and_global_variables: a list of tuples seperated by a colon which 
            can be used to export and quickly reproduce the zone properties in 
            a Python-based testing environment.
        
"""

ghenv.Component.Name = "Zone2"
ghenv.Component.NickName = 'Zone2'
ghenv.Component.Message = 'VER 0.0.1\nFEB_28_2018'
ghenv.Component.IconDisplayMode = ghenv.Component.IconDisplayMode.application
ghenv.Component.Category = "Oasys"
ghenv.Component.SubCategory = " 1 | Zone"
#compatibleOasysVersion = VER 0.0.1\nFEB_21_2018
try: ghenv.Component.AdditionalHelpFromDocStrings = "1"
except: pass

import scriptcontext as sc
import Grasshopper.Kernel as gh


#==============================================================================
#                            Set up thermal zone
#==============================================================================

#  Initialize default values if no input is detected
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

def validate_element(element):
    # Check if the element object has the mian three parameters
    if [x in dir(element) for x in ['name','area','u_value']]:
        return True
    else:
        return False

# keep valid element objects and combine them into a single list.
g = [x for x in glazed_elements if validate_element(x)]
o = [x for x in opaque_elements if validate_element(x)]

if len(g) != len(glazed_elements):
    warning = "Invalid glazed element detected"
    w = gh.GH_RuntimeMessageLevel.Warning
    ghenv.Component.AddRuntimeMessage(w, warning)
if len(o) != len(opaque_elements):
    warning = "Invalid opaque element detected"
    w = gh.GH_RuntimeMessageLevel.Warning
    ghenv.Component.AddRuntimeMessage(w, warning)    
elements = g + o
if len(elements) == 0:
    elements = None

# keep valid thermal bridge objects
t = [x for x in thermal_bridges if x is sc.sticky['ThermalBridge']]
if len(t) != len(thermal_bridges):
    warning = "Invalid thermal bridge detected"
    w = gh.GH_RuntimeMessageLevel.Warning
    ghenv.Component.AddRuntimeMessage(w, warning)
if len(t) == 0:
    thermal_bridges = None

# Replace default values with whatever is inputted to the component
unique_inputs = {}
for t in thermal_attributes.keys():
    if locals()[t] is not None:
        thermal_attributes[t] = locals()[t]
        # Add item to unique_inputs
        if 'supply' not in t and 'emission' not in t:
            value = t+':'+str(locals()[t])
        elif 'supply' in t:
            value = t+': supply_system.'+str(locals()[t])[22:-2]
        elif 'emission' in t:
            value = t+': emission_system.'+str(locals()[t])[22:-2]
        unique_inputs[t] = value

#Declare zone
ThermalZone = sc.sticky['Zone'](elements = elements,
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


#==============================================================================
#                        Add lighting attributes
#==============================================================================

lighting_attributes = {'lighting_load':11.7,
                       'lighting_control':300.0,
                       'lighting_utilisation_factor':0.45,
                       'lighting_maintenance_factor':0.9}
                       
for l in lighting_attributes.keys():
    if locals()[l] is not None:
        lighting_attributes[l] = locals()[l]
        value = l+':'+str(locals()[l])
        unique_inputs[l] = value

# Zone with thermal and lighting attributes
Zone = sc.sticky['ModularRCZone'](zone=ThermalZone,                 
              lighting_load=lighting_attributes['lighting_load'],
              lighting_control=lighting_attributes['lighting_control'],
              lighting_utilisation_factor=lighting_attributes['lighting_utilisation_factor'],
              lighting_maintenance_factor=lighting_attributes['lighting_maintenance_factor'])