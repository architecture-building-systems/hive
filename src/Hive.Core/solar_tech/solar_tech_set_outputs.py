# coding=utf-8
"""
reads in a Hive.IO.EnergySystems.Solar (Energy Carrier) and infuses Hive.IO.EnSys.Photovoltaic with output electricity
"""

from __future__ import division
import System
from System import Array
import math
import Grasshopper as gh
path = gh.Folders.AppDataFolder
import clr
import os
clr.AddReferenceToFileAndPath(os.path.join(path, "Libraries", "Hive.IO.gha"))
import Hive.IO.EnergySystems as ensys
import Rhino.RhinoApp as RhinoApp


# WHERE TO MAKE A GH COMPONENT FOR RADIATOR?? PROLLY IN HIVE.IO.INPUTS, IN THE ENERGYSYSTEMS FORM
def solar_tech_set_outputs(GHSolar_CResults, Hive_SurfaceBased, amb_T_carrier, emitter, time_resolution):
    if time_resolution == "hourly":
        horizon = 8760
    else:
        horizon = 12

    surface_based_tech_infused = []
    i = 0
    for solar_tech in Hive_SurfaceBased:
        if isinstance(solar_tech, ensys.Photovoltaic):
            solar_tech.SetInputComputeOutput(GHSolar_CResults[i].I_hourly, amb_T_carrier)
        if isinstance(solar_tech, ensys.SolarThermal):
            solar_tech.SetInputComputeOutput(GHSolar_CResults[i].I_hourly, emitter.ReturnCarrier.Temperature, emitter.InletCarrier.Temperature, amb_T_carrier)
        if isinstance(solar_tech, ensys.GroundCollector):
            pass
            # hot_water_generated =
            # solar_tech.SetInputOutput(solar_carrier, hot_water_generated)
        if isinstance(solar_tech, ensys.PVT):
            pass
            # electricity_generated =
            # hot_water_generated =
            # solar_tech.SetInputOutput(solar_carrier, electricity_generated, hot_water_generated)
        surface_based_tech_infused.append(solar_tech)
        i = i + 1

    return surface_based_tech_infused

