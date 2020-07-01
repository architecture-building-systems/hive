"""
Calculating solar irradiance on external building surfaces.
Using external libraries SolarModel.dll and GHSolar.gha
"""
import System
import Rhino.Geometry as rg
import Grasshopper as gh

path = gh.Folders.AppDataFolder
import clr
import os

clr.AddReferenceToFileAndPath(os.path.join(path, "Libraries", "GHSolar.gha"))
import GHSolar as ghs

clr.AddReferenceToFileAndPath(os.path.join(path, "Libraries\hive", "Hive.IO.gha"))
import Hive.IO.EnergySystems as ensys


def simulate_obstructed_panel(mesh_analysis, mesh_obstructions,
                              dhi_in, dni_in, latitude, longitude, solarazi, solaralti,
                              max_value, min_value):
    horizon = 8760

    # Model parameters
    # see Waibel et al. (2017) 'Efficient time-resolved 3D solar potential modelling' for description of parameters
    main_sky_res = 2  # MainSkyRes
    main_interp_mode = 0  # InterpMode
    spec_bounces = 0  # SpecBounces
    spec_interp_mode = 0  # SpecInterp
    diff_sky_res = 0  # DiffSkyRes
    diff_2ndsky_res = 0  # Diff2ndSkyRes
    diff_refl_mode = 0  # DiffMode
    mt = True  # .Net multi-threading

    snow_threshold = 1.0  # no sun penetrates if snow value on surface is 1.0. time series from weather file [0.0, 1.0]
    tilt_threshold = 60.0  # in degrees. with tilts higher than this value, snow is assumed to slip down the surface entirely (snow ignored)

    dni = System.Collections.Generic.List[float]()  # 8760 time series for direct normal irradiance
    dhi = System.Collections.Generic.List[float]()  # 8760 time series for diffuse horizontal irradiance
    snow = System.Collections.Generic.List[float]()  # 8760 time series for snow coverage [0.0, 1.0]
    ground_albedo = System.Collections.Generic.List[
        float]()  # 8760 time series for surface albedo (e.g. in winter higher albedo because of snow)
    solar_azimuth = System.Collections.Generic.List[
        float]()  # optional. 8760 time series. If no data provided, azimuth will be computed according to algorithm by Blanco-Muriel et al. (2001)
    solar_altitude = System.Collections.Generic.List[float]()  # same as azimuth

    building_albedos = System.Collections.Generic.List[float]()  # 8760 albedos of the building surfaces
    building_specular = System.Collections.Generic.List[float]()  # 8760 specular coefficients of building surfaces
    building_refltype = 0  # reflection type. 0=diffuse, 1=specular (expensive!), 2=diffuse and specular, 3=blind (no reflection)
    building_tolerance = 0.001  # tolerance used to offset analysis surface from original geometry to avoid self-obstruction
    building_name = "analysis_surface"
    building_mt = True

    # replace this later with weather file inputs
    for i in range(0, horizon):
        snow.Add(0.0)
        ground_albedo.Add(0.2)
        dni.Add(dni_in[i])
        dhi.Add(dhi_in[i])
        building_albedos.Add(0.3)
    if solarazi and solaralti:
        for i in range(0, horizon):
            solar_altitude.Add(solaralti[i])
            solar_azimuth.Add(solarazi[i])

    year = 2013  # ASSUMPTION

    tree_obj = System.Collections.Generic.List[ghs.CPermObject]()
    mesh_obj = ghs.CObstacleObject(mesh_analysis, building_albedos, building_specular,
                                   building_refltype, building_tolerance, building_name, building_mt)
    obstacles_obj = System.Collections.Generic.List[ghs.CObstacleObject]()
    for i in range(len(mesh_obstructions)):
        obstacles_obj.Add(ghs.CObstacleObject(mesh_obstructions[i], building_albedos, building_specular,
                                              building_refltype, building_tolerance, "obstructions", building_mt))

    calc_mesh = ghs.CCalculateSolarMesh(mesh_obj, obstacles_obj, tree_obj, latitude, longitude, dni, dhi, snow,
                                        ground_albedo, snow_threshold, tilt_threshold, year, None, mt,
                                        solar_azimuth, solar_altitude)

    calc_mesh.RunAnnualSimulation_MT(mesh_obj.tolerance, main_sky_res, main_interp_mode, spec_bounces,
                                     spec_interp_mode, diff_sky_res, diff_2ndsky_res, diff_refl_mode)

    results = calc_mesh.getResults()  # GHSolar.CResults

    # use GHSolar.GHResultsRead component to read GHSolar.CResults
    irradiance = read_results(results, mesh_analysis)

    # call visualize_as_mesh()
    mesh_visu = visualize_as_mesh(results, mesh_analysis, max_value, min_value)

    solar_carrier = ensys.Solar(horizon, System.Array[float](irradiance), None)

    return [irradiance, mesh_visu, solar_carrier]


def read_results(ghsolar_results, mshin):
    """
    Returns the total irradiance of the entire mesh in [W], or [Wh] (same, since hourly resolution). 8760 values
    :param ghsolar_results:
    :param mshin:
    :return:
    """
    timeseries = [0.0] * 8760

    valin = []
    for i in range(0, ghsolar_results.I_hourly.RowCount):
        val_t = [0.0] * ghsolar_results.I_hourly.ColumnCount
        for t in range(0, ghsolar_results.I_hourly.ColumnCount):
            val_t[t] = ghsolar_results.I_hourly[i, t]
        valin.append(val_t)

    mshFaceAreas = [0.0] * mshin.Faces.Count

    for t in range(0, ghsolar_results.I_hourly.ColumnCount):
        totVal = 0
        for i in range(0, mshin.Faces.Count):
            mshFaceAreas[i] = ghs.CMisc.getMeshFaceArea(i, mshin)
            FaceVal = 0.0
            valVertex1 = valin[mshin.Faces[i].A][t]
            valVertex2 = valin[mshin.Faces[i].B][t]
            valVertex3 = valin[mshin.Faces[i].C][t]
            if mshin.Faces[i].IsQuad:
                valVertex4 = valin[mshin.Faces[i].D][t]
                FaceVal = ((valVertex1 + valVertex2 + valVertex3 + valVertex4) / 4) * ghs.CMisc.getMeshFaceArea(i,
                                                                                                                mshin)
            else:
                FaceVal = ((valVertex1 + valVertex2 + valVertex3) / 3) * ghs.CMisc.getMeshFaceArea(i, mshin)
            totVal = totVal + FaceVal
        timeseries[t] = totVal

    return timeseries


def visualize_as_mesh(results, mshin, maxval, minval):
    """
    Script for visualizing solar mesh of GHSolar.gha in python
    Translated from C# from GHResultsVisualize.cs in github.com/christophwaibel/GH_Solar_V2
    :param results: CResults object from GHSolar.gha
    :param mshin: Rhino.Geometry.Mesh
    :param maxval: Float. Max value for coloring (red). In kWh/m2
    :param minval: Float. Min value for coloring (blue). In kWh/m2
    :return: Colored Rhino.Geometry.Mesh, showing solar irradiation of entire mesh, or per vertex
    """

    valin = []
    # if unit == 0 or unit == 1:
    #    for i in range(0, results.I_total.Count):
    #        valin.append(results.I_total[i])
    # elif unit == 2 or unit == 3:
    #    for i in range(0, results.I_hourly.RowCount):
    #        valin.append(results.I_hourly[i, HOUR]
    for i in range(0, results.I_total.Count):
        valin.append(results.I_total[i] * 0.001)

    mshcol = rg.Mesh()
    count = 0

    for i in range(mshin.Faces.Count):
        c = ghs.CMisc.GetRGB(1, valin[mshin.Faces[i].A], maxval, minval)
        mshcol.Vertices.Add(mshin.Vertices[mshin.Faces[i].A])
        mshcol.VertexColors.SetColor(count, c)

        c = ghs.CMisc.GetRGB(1, valin[mshin.Faces[i].B], maxval, minval)
        mshcol.Vertices.Add(mshin.Vertices[mshin.Faces[i].B])
        mshcol.VertexColors.SetColor(count + 1, c)

        c = ghs.CMisc.GetRGB(1, valin[mshin.Faces[i].C], maxval, minval)
        mshcol.Vertices.Add(mshin.Vertices[mshin.Faces[i].C])
        mshcol.VertexColors.SetColor(count + 2, c)

        if mshin.Faces[i].IsQuad:
            c = ghs.CMisc.GetRGB(1, valin[mshin.Faces[i].D], maxval, minval)
            mshcol.Vertices.Add(mshin.Vertices[mshin.Faces[i].D])
            mshcol.VertexColors.SetColor(count + 3, c)
            mshcol.Faces.AddFace(count, count + 1, count + 2, count + 3)
            count = count + 4
        else:
            mshcol.Faces.AddFace(count, count + 1, count + 2)
            count = count + 3

    return mshcol

#
# if __name__ == '__main__':
#     main(tilt, azimuth)
