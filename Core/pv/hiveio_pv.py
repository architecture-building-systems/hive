"""

"""
import Grasshopper as gh

path = gh.Folders.AppDataFolder
import clr
import os

clr.AddReferenceToFileAndPath(os.path.join(path, "Libraries", "Hive.IO.gha"))
import Hive.IO as hio

def main(hivepv):
    return [hivepv.Area, hivepv.RefEfficiency, hivepv.AirTemperature, hivepv.Irradiance]