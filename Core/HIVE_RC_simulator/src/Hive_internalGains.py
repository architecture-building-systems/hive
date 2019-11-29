# Generate occupancy and internal gains profiles. A constant value or a profile loaded from csv may be used.
#
# Hive: A energy simulation plugin developed by the A/S chair at ETH Zurich
# The occupancy profile
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
        Zone: plug in the Zone component (floor area is extracted from here)
        simulation_period: default=[0,8760] connect this to the ReadMe! outpt of the getWeatherData component, or input a string with the starting HOY and ending HOY. 
        max_occupancy: default=1 person. the maximum number of people expected to occupy the zone.
        appliance_gains: default=14W/m2.
        gains_per_person: default=100W.
        load_pattern: add a file path to occupancy_schedules.csv, which is available in the auxiliary folder (the file is based on Swiss statistical data from the CEA Toolbox).
        building_type: default=OFFICE. Works with occupancy_schedules.csv. Possible profiles are 1:MULTI_RES, 2:SINGLE_RES, 3:HOTEL, 4:OFFICE, 5:RETAIL, 6:FOODSTORE, 7:RESTAURANT, 8:INDUSTRIAL, 9:SCHOOL, 10:HOSPITAL, 11:GYM
        define_pattern: Input a list of values between 0 and 1 indicating the % occupied. The list must be the same length as the simulation period.
        
    Returns:
        readMe!: ...
        occupancy: occupancy profile (people/h)
        internal_gains: internal gains profile (Wh/h)
"""

ghenv.Component.Name = "Hive_internalGains"
ghenv.Component.NickName = 'internalGains'
ghenv.Component.Message = 'VER 0.0.1\nMAY_29_2018'
ghenv.Component.IconDisplayMode = ghenv.Component.IconDisplayMode.application
ghenv.Component.Category = "Hive"
ghenv.Component.SubCategory = "1. Environment"
# ComponentExposure=1


import scriptcontext as sc
import rhinoscriptsyntax as rs
import re
import Grasshopper.Kernel as ghKernel

def read_csv(path,col,simulation_period):
    f = open(path, 'r')
    occupancy_m2 = []
    for line in f.readlines():
        values = line.strip().split(',')
        try:
            occupancy_m2.append(values[col])
        except:
            raise_error('The index submitted in building_type does not correspond to any column in the csv.')
    return occupancy_m2[0],occupancy_m2[simulation_period[0]:simulation_period[-1]+1]
    f.close()

def raise_warning(warning_str):
    warning = warning_str
    w = ghKernel.GH_RuntimeMessageLevel.Warning
    ghenv.Component.AddRuntimeMessage(w, warning)

def raise_error(error_str):
    error = error_str
    e = ghKernel.GH_RuntimeMessageLevel.Error
    ghenv.Component.AddRuntimeMessage(e, error)

def main(Zone,max_occupancy,appliance_gains,gains_per_person,load_pattern,building_type,define_pattern):
    if not sc.sticky.has_key('HivePreparation'): 
        raise_warning("Add HIVE to the canvas!")
        return -1
    HivePreparation = sc.sticky['HivePreparation']()
    
    try:
        floor_area = float(Zone.floor_area)
    except:
        raise_error('Add a valid zone')
    
    max_occupancy = 1 if max_occupancy is None else max_occupancy
    appliance_gains = 14 if appliance_gains is None else appliance_gains
    gains_per_person = 100 if gains_per_person is None else gains_per_person
    
    # Use stickys to get simulation period from getWeatherData component
    simulation_period = sc.sticky['simulation_period']
    simulation_period = [int(i) for i in re.findall(r'\d+',simulation_period)]
    
    if load_pattern:
        path = r'%s'%load_pattern
        building_type = 3 if building_type is None else int(building_type)
        profile_type, occupancy_profile = read_csv(path,building_type,simulation_period)
    
    if define_pattern:
        if any([x<0 or x>1 for x in define_pattern]):
            raise_warning('values in custom pattern must be between 0 and 1')
            occupancy_profile = define_pattern
            profile_type = 'user defined pattern'
        if len(define_pattern) != range(simulation_period):
            raise_error('Custom pattern does not match the length of the simulation period.')
    
    # Calculate occupancy profile and internal gains
    if 'occupancy_profile' in locals():
        occupancy = [float(p)*max_occupancy for p in occupancy_profile]
        occupancy_tree = HivePreparation.list_to_tree([[o] for o in occupancy_profile])
        
        internal_gains = [x*gains_per_person + floor_area * appliance_gains for x in occupancy]
        internal_gains_tree = HivePreparation.list_to_tree([[ig] for ig in internal_gains])
        
        print 'profile with %i values'%len(occupancy)
        print
        print 'Occupancy pattern: ',profile_type
        print 'occupancy = occupancy_pattern * %i people'%max_occupancy
        print 'internal gains = occupant gains + appliance gains'
        print 'occupant gains = 100W * occupancy_pattern'
        print 'appliance gains = 14W * %fm2'%floor_area
        
        return occupancy_tree, internal_gains_tree

    else:
        raise_warning('Occupancy pattern is missing')
        return -1


""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""

result = main(Zone, max_occupancy, appliance_gains, gains_per_person, load_pattern, building_type,define_pattern)

if result != -1:
    occupancy_profile, internal_gains = result