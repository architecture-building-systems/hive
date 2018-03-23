#
# Hive: An educational plugin developed by the A/S chair at ETH Zurich
# 
# This file is part of Hive.
# 
#TODO: Check terms of GNU modification.
#
# This component is based on the Ladybug_Export Ladybug component. It has been
# slightly modified to work without Ladybug, but it does exactly the same thing.
# 
# Ladybug is a free software registered under a GNU public license.
# 

"""
Extract temperature, location and irradiation data from a weather file. 


-
Provided by Hive 0.0.1

    Args:
        _components: Any output from a new Ladybug (or Honeybee) component that you wish to export. Right now, only one component can be connected at a time but you can input a "*" (without quotation marsk) to search all changed Ladybug components on a grasshopper canvas.
        _targetFolder: A file path on your system which you would like to export the user object and source code to.  For most code developers, this file path will lead to their Github folder for Ladybug (or Honeybee), which is usually installed in "My Documents" by default. Exported source code will be saved at .\src and exported userObjects will be saved at .\userObjects in this _targetFolder.
        _export: Set to "True" to export Ladybug (or Honeybee) components to the _targerFolder.
    Returns:
        readMe!: ...
"""
ghenv.Component.Name = "Hive_getSimulationData"
ghenv.Component.NickName = 'getSimulationData'
ghenv.Component.Message = 'VER 0.0.1\nMAR_23_2018'
ghenv.Component.Category = "Hive"
ghenv.Component.SubCategory = "2 | Simulation"
# ComponentExposure=1

import rhinoscriptsyntax as rs
import scriptcontext as sc


def open_epw(open):
    #Based on Ladybug_Open EPW Weather File
    if open == True:
        filter = "EPW file (*.epw)|*.epw|All Files (*.*)|*.*||"
        epwFile = rs.OpenFileName("Open .epw Weather File", filter)
        print 'Done!'
        return epwFile
    else:
        print 'Please set open to True'

def set_simulation_period(start_hoy,end_hoy):
    if start_hoy is not None:
        if 0 <= int(start_hoy) < 8760:
            period = [start_hoy]
        else:
            # raise warning invalid start_hoy
            return None
    else:
        period = [0]
    
    if end_hoy is not None:
        if period[0] <= end_hoy < 8760:
            period.append(end_hoy)
        else:
            # raise warning invalid end_hoy
            return None
    else:
        period.append(8759)
    
    return period

def main(epw_file,simulation_period):
    if not sc.sticky.has_key('HivePreparation'): return "Add the modular RC component to the canvas!"
    #TODO: Set up compatibility checks like in Ladybug.
    
    hive_preparation = sc.sticky["HivePreparation"]()
    
    locationData = hive_preparation.epwLocation(epw_file)
    weatherData = hive_preparation.epwDataReader(epw_file, locationData[0])
    
    return locationData, weatherData

if _open:
    epw_file = open_epw(_open)
    simulation_period = set_simulation_period(start_HOY_,end_HOY_)
    if (epw_file is not None) and (simulation_period is not None):
        result = main(epw_file,simulation_period)