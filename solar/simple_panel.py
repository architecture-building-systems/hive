"""
Calculating solar irradiance on an unobstructed surface.
Parameters:
    Tilt
    Azimuth

Using external libraries SolarModel.dll and GHSolar.gha
"""
import System
import Grasshopper as gh
path = gh.Folders.AppDataFolder
import clr
import os
clr.AddReferenceToFileAndPath(os.path.join(path, "Libraries", "SolarModel.dll"))
import SolarModel as sm


def main(tilt, azimuth):
    return simulate_simple_panel(tilt, azimuth)


def simulate_simple_panel(tilt, azimuth):
    # tilt = 40
    # azimuth = 90
    latitude = 47.36667
    longitude = 8.55
    year = 2017

    recursion = 2
    horizon = 8760

    sunvectors = sm.SunVector.Create8760SunVectors(longitude, latitude, year)

    # dummy variables, won't be used in this simplified calculation
    coord = [sm.Sensorpoints.p3d()] * 1
    coord[0].X, coord[0].Y, coord[0].Z = 0, 0, 0
    normal = [sm.Sensorpoints.v3d()] * 1
    normal[0].X, normal[0].Y, normal[0].Z = 0, 1, 0

    # load DNI and DHI from a textfile located in Grasshopper libraries folder
    # weatherfile = open(path + "Libraries\Zurich.csv")

    albedo = [0.3] * horizon
    weather = sm.Context.cWeatherdata()
    sm.Context.cWeatherdata.DHI.SetValue(weather, System.Collections.Generic.List[float]())
    sm.Context.cWeatherdata.DNI.SetValue(weather, System.Collections.Generic.List[float]())
    sm.Context.cWeatherdata.Snow.SetValue(weather, System.Collections.Generic.List[float]())
    for i in range(0, horizon):
        weather.DHI.Add(0)
        weather.DNI.Add(1000)

    #  Calculation points
    p = sm.Sensorpoints(System.Array[float]([tilt]), System.Array[float]([azimuth]), System.Array[sm.Sensorpoints.p3d](coord), System.Array[sm.Sensorpoints.v3d](normal), recursion)
    p.SetSimpleSky(System.Array[float]([tilt]))
    p.SetSimpleGroundReflection(System.Array[float]([tilt]), System.Array[float](albedo), weather, System.Array[sm.SunVector](sunvectors))
    p.CalcIrradiation(weather, System.Array[sm.SunVector](sunvectors))

    total = [0.0] * horizon
    beam = [0.0] * horizon
    diff = [0.0] * horizon
    diff_refl = [0.0] * horizon
    for i in range(0, horizon):
        total[i] = p.I[0][i]
        beam[i] = p.Ibeam[0][i]
        diff[i] = p.Idiff[0][i]
        diff_refl[i] = p.Irefl_diff[0][i]
    return [total, beam, diff, diff_refl]


def visualize_as_mesh():
    return 0


if __name__ == '__main__':
    tilt = 45
    azimuth = 180
    main(tilt, azimuth)