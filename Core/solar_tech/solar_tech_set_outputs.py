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


def solar_tech_set_outputs(GHSolar_CResults, Hive_SurfaceBased, amb_T_carrier, time_resolution):
    if time_resolution == "hourly":
        horizon = 8760
    else:
        horizon = 12

    surface_based_tech_infused = []
    i = 0
    for solar_tech in Hive_SurfaceBased:
        if solar_tech.ToString() == "Hive.IO.EnergySystems.Photovoltaic":
            solar_tech.SetInputComputeOutput(GHSolar_CResults[i].I_hourly, amb_T_carrier)
        if solar_tech.ToString() == "Hive.IO.EnergySystems.SolarThermal":
            solar_tech.SetInputComputeOutputSimple(GHSolar_CResults[i].I_hourly)
        if solar_tech.ToString() == "Hive.IO.EnergySystems.GroundCollector":
            print("test")
            # hot_water_generated =
            # solar_tech.SetInputOutput(solar_carrier, hot_water_generated)
        if solar_tech.ToString() == "Hive.IO.EnergySystems.PVT":
            print("test")
            # electricity_generated =
            # hot_water_generated =
            # solar_tech.SetInputOutput(solar_carrier, electricity_generated, hot_water_generated)
        surface_based_tech_infused.append(solar_tech)
        i = i + 1


    return surface_based_tech_infused

