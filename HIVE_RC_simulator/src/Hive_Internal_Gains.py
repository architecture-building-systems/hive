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
        custom_pattern: a list of values between 0 and 1 indicating the % occupied
        _file_path: path to a .csv file containing occupancy patterns. occupancy_schedules.csv (based on data used in the CEA Toolbox) is available in the auxiliary folder, but any profile may be used.
        _building_type: integer which refers to a specific column in the file. occupancy_schedules.csv contains the following profiles: 1:MULTI_RES, 2:SINGLE_RES, 3:HOTEL, 4:OFFICE, 5:RETAIL, 6:FOODSTORE, 7:RESTAURANT, 8:INDUSTRIAL, 9:SCHOOL, 10:HOSPITAL, 11:GYM
        start_HOY: optional hour of the year at which to start the profile. 1 by default
        end_HOY: optional hour of the year at which to end the profile. 8760 by default
        _floor_area: the occupied floor area
        _max_occupancy: the maximum number of people expected to occupy the zone.
    
    Returns:
        readMe!: ...
        occupancy: occupancy profile (people per hour)
        internal_gains: internal gains profile (Watts per hour)
"""

ghenv.Component.Name = "Hive_Internal_Gains"
ghenv.Component.NickName = 'InternalGains'
ghenv.Component.Message = 'VER 0.0.1\nMAY_16_2018'
ghenv.Component.IconDisplayMode = ghenv.Component.IconDisplayMode.application
ghenv.Component.Category = "Hive"
ghenv.Component.SubCategory = "2 | Simulation"
# ComponentExposure=1


import scriptcontext as sc
import rhinoscriptsyntax as rs

def read_csv(path,col,start,end):
    f = open(path, 'r')
    occupancy_m2 = []
    for line in f.readlines():
        values = line.strip().split(',')
        # do something with values here
        occupancy_m2.append(values[col])
    return occupancy_m2[0],occupancy_m2[start:end+1]
    f.close()

""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""
if not sc.sticky.has_key('HivePreparation'): 
    print "Add the modular RC component to the canvas!"
else:
    gains_per_person = 100 #W
    appliance_gains = 14  # W per sqm
    
    profile_type = None
    occupancy_profile = None
    
    if len(custom_pattern) > 0:
        if any([x<0 or x>1 for x in custom_pattern]):
            HiveOreparation.raise_warning('values in custom pattern must be between 0 and 1')
        occupancy_profile = custom_pattern
        profile_type = 'user defined pattern'
    
    elif _file_path:
        HivePreparation = sc.sticky['HivePreparation']()
        start,end = HivePreparation.set_simulation_period(start_HOY,end_HOY)
        path = r'%s'%_file_path
        building_type = 3 if _building_type is None else int(_building_type)
        profile_type, occupancy_profile = read_csv(path,_building_type,start,end)
    
    if occupancy_profile is not None:
        max_occupancy = 1 if _max_occupancy is None else _max_occupancy
        occupancy = [float(p)*max_occupancy for p in occupancy_profile]
        
        if type(_floor_area) is float:
            internal_gains = [x*gains_per_person + _floor_area * appliance_gains for x in occupancy]
        
        print 'profile with %i values'%len(occupancy)
        print
        print 'Pattern type: ',profile_type
        print 'occupancy = occupancy_pattern * %i people'%max_occupancy
        print 'internal gains = occupant gains + appliance gains'
        print 'occupant gains = 100W * occupancy_pattern'
        print 'appliance gains = 14W * area'
    
    else:
        print 'Specify an occupancy profile to generate occupancy and internal gains'
    
    occupancy = HivePreparation.list_to_tree([[o] for o in occupancy])
    internal_gains = HivePreparation.list_to_tree([[ig] for ig in internal_gains])