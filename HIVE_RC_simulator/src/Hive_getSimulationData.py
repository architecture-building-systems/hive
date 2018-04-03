#
# Hive: An educational plugin developed by the A/S chair at ETH Zurich
# 
# This file is part of Hive.
# 
# TODO: Check terms of GNU modification.
#
# This component is a mashup of functions from Ladybug for importing and reading 
# EPW files. Some of 
# 
# 
# 

"""
Extract temperature, location and irradiation data from a weather file. 
-
Provided by Hive 0.0.1

    Args:
        _open: Set Boolean to True to browse for a weather file on your system.
        start_HOY_: optional HOY for the start of the analysis period
        end_HOY_: optional HOY for the end of the analysis period
    Returns:
        out: ...
        location: a list of longitude, latitude, utc offset and year
        temperature: a list of temperatures for the analysis period
        directRadiation: a list of direct radiation for the analysis period
        diffuseRadiation: a list of diffuse radiation for the analysis period
"""

ghenv.Component.Name = "Hive_getSimulationData"
ghenv.Component.NickName = 'getSimulationData'
ghenv.Component.Message = 'VER 0.0.1\nAPR_03_2018'
ghenv.Component.Category = "Hive"
ghenv.Component.SubCategory = "2 | Simulation"
# ComponentExposure=1

import rhinoscriptsyntax as rs
import scriptcontext as sc
from System import Object

def separate_data(inputList,start,end):
    # Based on Ladybug_Separate data, but uses lists instead of DataTree objects
    num = []
    str = []
    lastOne = None
    
    strPath = 0
    numPath = 0
    
    p = []
    numbers = []
    strings = []
    
    for count, item in enumerate(inputList):
        try:
            item = float(item)
            if count == 0: numfirst = True
            if lastOne == None: lastOne = "float"
            if lastOne!= "float":
                lastOne = "float"
                numPath += 1
            if numfirst == False:
                p.append(numPath-1)
                numbers.append(item)
            else:
                p.append(numPath)
                numbers.append(item)
        except:
            if count == 0: numfirst = False
            if lastOne == None: lastOne = "str"
            if lastOne!= "str":
                lastOne = "str"
                strPath += 1
            if numfirst == True:
                p.append(strPath-1)
                strings.append(item)
            else:
                p.append(strPath)
                strings.append(item)
    
    return numbers[start:end]

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
            start = start_hoy
        else:
            warningMsg = "start_HOY_ must be an integer between 0 and 8760"
            w = gh.GH_RuntimeMessageLevel.Warning
            GHComponent.AddRuntimeMessage(w, warningMsg)
            return None
    else:
        start = 0
    
    if end_hoy is not None:
        if start <= end_hoy < 8760:
            end = end_hoy
        else:
            warningMsg = "end_HOY_ must be an integer between start_HOY_ and 8760"
            w = gh.GH_RuntimeMessageLevel.Warning
            GHComponent.AddRuntimeMessage(w, warningMsg)
            return None
    else:
        end = 8759
    
    return int(start),int(end)

def main(epw_file,start_HOY_,end_HOY_):
    if not sc.sticky.has_key('HivePreparation'): return "Add the modular RC component to the canvas!"
    #TODO: Set up compatibility checks like in Ladybug.
    
    hive_preparation = sc.sticky["HivePreparation"]()
    start,end = set_simulation_period(start_HOY_,end_HOY_)
    hoy = [i for i in range(start,end)]
    
    if (epw_file is not None):
        locationData = hive_preparation.epwLocation(epw_file)
        weatherData = hive_preparation.epwDataReader(epw_file, locationData[0])

        location = [locationData[1], locationData[2], locationData[3]]
        dryBulbTemperature, dewPointTemperature, relativeHumidity, windSpeed, windDirection, directNormalRadiation, diffuseHorizontalRadiation, globalHorizontalRadiation, directNormalIlluminance, diffuseHorizontalIlluminance, globalHorizontalIlluminance, totalSkyCover, horizontalInfraredRadiation, barometricPressure, modelYear = weatherData
        location.append(modelYear[7])

        temperature = separate_data(dryBulbTemperature, start, end)
        directRadiation = separate_data(directNormalRadiation, start, end)
        diffuseRadiation = separate_data(diffuseHorizontalRadiation, start, end)

    return location,temperature,directRadiation,diffuseRadiation,hoy


if _open:
    epw_file = open_epw(_open)
    location,temperature,directRadiation,diffuseRadiation,hour_of_year = main(epw_file,start_HOY_,end_HOY_)

