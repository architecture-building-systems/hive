# Downsample data
#
# HIVE: A energy simulation plugin developed by the A/S chair at ETH Zurich
# This component is based on building_physics.py in the RC_BuildingSimulator Github repository
# https://github.com/architecture-building-systems/RC_BuildingSimulator
# Extensive documentation is available on the project wiki.
#
# Author: Justin Zarb <zarbj@student.ethz.ch>
#
# This file is part of HIVE
#
# Licensing/Copyright and liability comments go here.
# <Copyright 2018, Architecture and Building Systems - ETH Zurich>
# <Licence: MIT>

"""
Define an opaque by adding a surface.
-
Provided by HIVE 0.0.1
    
    Args:
        _hourly_data: a list of hourly values
        _start_hoy: the hour of the year corresponding to the first value.
    Returns:
        daily_values:
        monthly_values:
        annual_value:
"""

ghenv.Component.Name = "Hive_Downsample"
ghenv.Component.NickName = 'GlazedElement'
ghenv.Component.Message = 'VER 0.0.1\nMAY_07_2018'
ghenv.Component.IconDisplayMode = ghenv.Component.IconDisplayMode.application
ghenv.Component.Category = "Hive"
ghenv.Component.SubCategory = "5 | Results"
# ComponentExposure=5

import scriptcontext as sc
from Grasshopper import DataTree
from Grasshopper.Kernel.Data import GH_Path

def main(_start_hoy, _hourly_data):
    if not sc.sticky.has_key('HivePreparation'): 
        return "Add the modular RC component to the canvas!"
    hive_preparation = sc.sticky['HivePreparation']()
    data_dict = dict(zip(range(int(_start_hoy),int(_start_hoy+len(_hourly_data))),_hourly_data))
    dates = [hive_preparation.hour2Date(hoy,True) for hoy in data_dict.keys()]
    
    days = max(data_dict.keys())//24+1
    daily = dict(zip(range(0,days), [0]*days))
    monthly = dict(zip(range(0,12),[0]*12))
    annual = sum(_hourly_data)
    
    for hoy,data in data_dict.iteritems():
        date = hive_preparation.hour2Date(hoy,True)
        print date[1]
        monthly[date[1]] += data
        doy = (hoy-1)//24
        daily[doy] += data
    
    dailyTree = hive_preparation.dict_to_tree(daily)
    monthlyTree = hive_preparation.dict_to_tree(monthly)
    
    return dailyTree, monthlyTree, annual

if _start_hoy and _hourly_data:
    daily_values, monthly_values, annual_value = main(_start_hoy,_hourly_data)