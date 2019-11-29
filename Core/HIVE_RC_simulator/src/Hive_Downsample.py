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
Convert hourly values to daily,monthly or annual results.
-
Provided by HIVE 0.0.1
    
    Args:
        _hourly_data: a list of hourly values
        _start_hoy: the hour of the year corresponding to the first value.
        resample_type: [0:sum, 1:mean] (default=0)
    Returns:
        readMe!:...
        daily_values:
        monthly_values:
        annual_value:
"""

ghenv.Component.Name = "Hive_Downsample"
ghenv.Component.NickName = 'Downsample'
ghenv.Component.Message = 'VER 0.0.1\nMAY_30_2018'
ghenv.Component.IconDisplayMode = ghenv.Component.IconDisplayMode.application
ghenv.Component.Category = "Hive"
ghenv.Component.SubCategory = "4. Results"
# ComponentExposure=1

import scriptcontext as sc
import Grasshopper.Kernel as ghKernel

def main(_start_hoy, hourly_data, resample_type):
    if not sc.sticky.has_key('HivePreparation'): 
        return "Add the modular RC component to the canvas!"
    hive_preparation = sc.sticky['HivePreparation']()
    
    def resample(dict,resample_type):
        if resample_type == 0:
            for key in dict:
                dict[key] = sum(dict[key])
        if resample_type == 1:
            for key in dict:
                dict[key] = sum(dict[key])/len(dict[key])
        return dict.values()
    
    hoys = range(int(_start_hoy),int(_start_hoy+len(hourly_data[0])))
    doys = [(h-1)//24 for h in hoys]
    moys = [hive_preparation.hour2Date(hoy,True)[1] for hoy in hoys]
    
    daily_streams = []
    monthly_streams = []
    annual = []
    
    for stream in hourly_data:
        daily = dict(zip(range(0,365),[list()]*365))
        monthly = dict(zip(range(0,11),[list()]*11))
        day_last_hour = 0
        values_this_day = []
        month_last_hour = 0
        values_this_month = []
        
        for value,doy,moy in zip(hourly_data[stream],doys,moys):
            # Split values into days
            if doy == day_last_hour:
                values_this_day.append(value)
            else:
                daily[day_last_hour] = values_this_day
                values_this_day = []
                day_last_hour = doy
            daily[day_last_hour] = values_this_day
            
            # Split values into months
            if moy == month_last_hour:
                values_this_month.append(value)
            else:
                monthly[month_last_hour] = values_this_month
                values_this_month = []
                month_last_hour = moy
            monthly[month_last_hour] = values_this_month
        
        daily_streams.append(resample(daily,resample_type))
        monthly_streams.append(resample(monthly,resample_type))
        
        if resample_type == 0:
            annual.append([sum(hourly_data[stream])])
        if resample_type == 1:
            annual.append([sum(hourly_data[stream])/len(hourly_data[stream])])
    
    dailyTree = hive_preparation.list_to_tree(daily_streams)
    monthlyTree = hive_preparation.list_to_tree(monthly_streams)
    annualTree = hive_preparation.list_to_tree(annual)
    
    return dailyTree, monthlyTree, annualTree

""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""

resample_type_ = 0 if resample_type_ is None else resample_type_

#convert _hourly_data into a dictionary.
if _hourly_data.BranchCount > 0 :
    hourly_data = {}
    for stream in range(0,len(_hourly_data.Branch(0))):
        hourly_data[stream] = []
        for b in range(0,_hourly_data.BranchCount):
            hourly_data[stream].append(_hourly_data.Branch(b)[stream])
    if not _start_HOY:
        print """Start_HOY assumed to be 0: If this is not the case, add a 
        value to _start_HOY!"""
        _start_HOY = 1
    daily_values, monthly_values, annual_value = main(_start_HOY,hourly_data,resample_type_)
    print '%i data streams resampled'%len(_hourly_data.Branch(0))


else:
    print 'Connect a data stream to _hourly_data'


