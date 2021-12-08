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
clr.AddReferenceToFileAndPath(os.path.join(path, "Libraries\Hive", "SolarModel.dll"))
import SolarModel as sm


def main(tilt, azimuth, DHI, DNI, latitude, longitude, solarazi, solaralti, Aw, timezone):
    paropts = System.Threading.Tasks.ParallelOptions()
    paropts.MaxDegreeOfParallelism = 1

    year = 2013 # ASSUMPTION

    recursion = 2
    horizon = 8760

    if (not solaralti) or (not solarazi):
        sunvectors = sm.SunVector.Create8760SunVectors(longitude, latitude, year)   # Blanco-Muriel (2001)
    else:
        sunvectors = sm.SunVector.Create8760SunVectors(longitude, latitude, year, System.Array[float](solarazi), System.Array[float](solaralti))

    # shifting list of sunvectors according to timezone, so it matches weather file data
    # if timezone != 0:
    #     copy_array = []
    #     shifted_indices = range(0, horizon)
    #     shifted_indices = shifted_indices[(timezone*-1):] + shifted_indices[:(timezone*-1)]
    #     for i in range(0, horizon):
    #         copy_array.append(sunvectors[i])
    #     for i in range(0, horizon):
    #         sunvectors[i] = copy_array[shifted_indices[i]]

    if timezone != 0:
        sm.SunVector.ShiftSunVectorsByTimezone(sunvectors, timezone)

    # dummy variables, won't be used in this simplified calculation
    coord = [sm.Sensorpoints.p3d()] * 1
    coord[0].X, coord[0].Y, coord[0].Z = 0, 0, 0
    normal = [sm.Sensorpoints.v3d()] * 1
    normal[0].X, normal[0].Y, normal[0].Z = 0, 1, 0

    albedo = [0.3] * horizon    # ASSUMPTION
    weather = sm.Context.cWeatherdata()
    sm.Context.cWeatherdata.DHI.SetValue(weather, System.Collections.Generic.List[float]())
    sm.Context.cWeatherdata.DNI.SetValue(weather, System.Collections.Generic.List[float]())
    sm.Context.cWeatherdata.Snow.SetValue(weather, System.Collections.Generic.List[float]())
    for i in range(0, horizon):
        weather.DHI.Add(DHI[i])
        weather.DNI.Add(DNI[i])

    #  Calculation points
    p = sm.Sensorpoints(System.Array[float]([tilt]), System.Array[float]([azimuth]), System.Array[sm.Sensorpoints.p3d](coord), System.Array[sm.Sensorpoints.v3d](normal), recursion)
    p.SetSimpleSkyMT(System.Array[float]([tilt]), paropts)
    p.SetSimpleGroundReflectionMT(System.Array[float]([tilt]), System.Array[float](albedo), weather, System.Array[sm.SunVector](sunvectors), paropts)
    p.CalcIrradiationMT(weather, System.Array[sm.SunVector](sunvectors), paropts)

    total = [0.0] * horizon
    # beam = [0.0] * horizon
    # diff = [0.0] * horizon
    # diff_refl = [0.0] * horizon
    for i in range(0, horizon):
        irrad = p.I[0][i]
        if irrad < 0.0:
            irrad = 0.0
        total[i] = irrad * Aw
        # beam[i] = p.Ibeam[0][i]
        # diff[i] = p.Idiff[0][i]
        # diff_refl[i] = p.Irefl_diff[0][i]
    return total


def visualize_as_mesh():
    return 0


if __name__ == '__main__':
    tilt = 45
    azimuth = 180
    DHI = [10] * 8760
    DNI = [10] * 8760
    latitude = 47
    longitude = 8.55
    solarazi = None
    solaralti = None

    main(tilt, azimuth, DHI, DNI, latitude, longitude, solarazi, solaralti)