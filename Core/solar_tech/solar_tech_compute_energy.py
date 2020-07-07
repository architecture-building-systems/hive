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


def solar_tech_compute_energy(GHSolar_CResults, Hive_SurfaceBased, amb_T_carrier, time_resolution):
    if time_resolution == "hourly":
        horizon = 8760
    else:
        horizon = 12

    surface_based_tech_infused = []
    i = 0
    for solar_tech in Hive_SurfaceBased:
        # will be in the correct order, because List<Hive(...).SurfaceBased> is processed in the Hive(...).Distributor and fed into the solarmodel, returning List<GHSolar.CResults> in the same order
        mesh = solar_tech.SurfaceGeometry
        irradiation = get_mean_hourly_irradiation(GHSolar_CResults[i].I_hourly, mesh, solar_tech.SurfaceArea)
        solar_carrier = ensys.Radiation(horizon, Array[float](irradiation))
        if solar_tech.ToString() == "Hive.IO.EnergySystems.Photovoltaic":
            print("test")
            electricity_generated = pv_yield(solar_tech.SurfaceArea, solar_tech.RefEfficiencyElectric, solar_tech.Beta,
                                             solar_tech.NOCT, solar_tech.NOCT_ref, solar_tech.NOCT_sol,
                                             amb_T_carrier.AvailableEnergy, solar_carrier.AvailableEnergy)
            zero_array = [0.0] * horizon
            electricity_carrier = ensys.Electricity(horizon, Array[float](electricity_generated[0]),
                                                    Array[float](zero_array), Array[float](zero_array))
            solar_tech.SetInputOutput(solar_carrier, electricity_carrier)
        if solar_tech.ToString() == "Hive.IO.EnergySystems.SolarThermal":
            print("test")
            # hot_water_generated =
            # solar_tech.SetInputOutput(solar_carrier, hot_water_generated)
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


    # electricity_horizon = []
    # if time_resolution == "hourly":
    #     horizon = 8760
    #     electricity_horizon = electricity_generated[0]
    # else:   # monthly
    #     horizon = 12
    #     days_per_month = [31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31]
    #     hours_per_day = 24
    #     total_months = 12
    #     for month in range(total_months):
    #         start_hour = int(hours_per_day * sum(days_per_month[0:month]))
    #         end_hour = int(hours_per_day * sum(days_per_month[0:month + 1]))
    #         # hours_per_month = days_per_month[month] * hours_per_day # could be used to compute average values
    #         electricity_horizon.append(sum(electricity_generated[0][start_hour:end_hour]))
    #
    # electricity_carrier = ensys.Electricity(horizon, System.Array[float](electricity_horizon), System.Array[float]([0.0] * horizon), System.Array[float]([0.0] * horizon))
    # pv.SetInputOutput(solar_carrier, electricity_carrier)
    return surface_based_tech_infused


def get_mean_hourly_irradiation(ghsolar_cresults, mesh, total_mesh_area):
    """
    computes mean hourly irradiation of a mesh, given a GHSolar.CResults object.
    CResults has hourly irradiance time series for each vertex of a mesh,
    so this function weights each of the time series with the area of
    each mesh face of the mesh.
    :param ghsolar_cresults: GHSolar.CResults object containing 8760 time series of solar potentials in W/sqm
    :param mesh: Rhino.Geometry.Mesh associated to CResults
    :param total_mesh_area: total mesh area
    :return: Time series of type List<Double> with mesh average solar potentials in W/sqm
    """

    mesh_faces_count = mesh.Faces.Count
    vertex_count = ghsolar_cresults.RowCount
    horizon = ghsolar_cresults.ColumnCount
    all_irradiances = []  # list of 8760 arrays. so all timeseries for each vertex put into a list
    mean_irradiance = [0.0] * horizon

    for i in range(vertex_count):
        val_t = [0.0] * horizon
        for t in range(horizon):
            val_t[t] = ghsolar_cresults[i, t]
        all_irradiances.append(val_t)

    mesh_face_areas = [0.0] * mesh.Faces.Count
    for t in range(horizon):
        total_value = 0.0
        for i in range(mesh_faces_count):
            mesh_face_areas[i] = get_mesh_face_area(i, mesh)
            face_value = 0.0
            value_vertex_1 = all_irradiances[mesh.Faces[i].A][t]
            value_vertex_2 = all_irradiances[mesh.Faces[i].B][t]
            value_vertex_3 = all_irradiances[mesh.Faces[i].B][t]
            if mesh.Faces[i].IsQuad:
                value_vertex_4 = all_irradiances[mesh.Faces[i].D][t]
                face_value = ((value_vertex_1 + value_vertex_2 + value_vertex_3 + value_vertex_4) / 4) * \
                             mesh_face_areas[i]
            else:
                face_value = ((value_vertex_1 + value_vertex_2 + value_vertex_3) / 3) * mesh_face_areas[i]

            total_value = total_value + face_value
        mean_irradiance[t] = total_value / total_mesh_area

    return mean_irradiance


def get_mesh_face_area(mesh_face_index, mesh):
    # source: http://james-ramsden.com/area-of-a-mesh-face-in-c-in-grasshopper/

    # get points into a nice, concise format
    pts = []
    pts.append(mesh.Vertices[mesh.Faces[mesh_face_index].A])
    pts.append(mesh.Vertices[mesh.Faces[mesh_face_index].B])
    pts.append(mesh.Vertices[mesh.Faces[mesh_face_index].C])
    if mesh.Faces[mesh_face_index].IsQuad:
        pts.append(mesh.Vertices[mesh.Faces[mesh_face_index].D])

    # calculate areas of triangles
    pt_a = pts[0].DistanceTo(pts[1])
    pt_b = pts[1].DistanceTo(pts[2])
    pt_c = pts[2].DistanceTo(pts[0])
    pt_p = 0.5 * (pt_a + pt_b + pt_c)
    area1 = math.sqrt(pt_p * (pt_p - pt_a) * (pt_p - pt_b) * (pt_p - pt_c))

    # if quad, calc area of second triangle
    area2 = 0.0
    if mesh.Faces[mesh_face_index].IsQuad:
        pt_a = pts[0].DistanceTo(pts[2])
        pt_b = pts[2].DistanceTo(pts[3])
        pt_c = pts[3].DistanceTo(pts[0])
        pt_p = 0.5 * (pt_a + pt_b + pt_c)
        area2 = math.sqrt(pt_p * (pt_p - pt_a) * (pt_p - pt_b) * (pt_p - pt_c))

    return area1 + area2


def pv_yield(A, eta_PVref, beta, NOCT, NOCT_ref, NOCT_sol, T_amb, I):
    """
    Computes PV electricity yield
    :param A:
    :param eta_PVref:
    :param beta:
    :param NOCT:
    :param NOCT_ref:
    :param NOCT_sol:
    :param T_amb:
    :param I:
    :return:
    """
    eta_pv = pv_efficiency(eta_PVref, beta, NOCT, NOCT_ref, NOCT_sol, T_amb, I)
    pvyield = [0.0] * len(eta_pv)
    for i in range(0, len(eta_pv)):
        pvyield[i] = A * eta_pv[i] * I[i]
    return pvyield, eta_pv


def pv_efficiency(eta_PVref, beta, NOCT, NOCT_ref, NOCT_sol, T_amb, I):
    """
    Calculates time resolved PV efficiency [-]
    :param eta_PVref: Reference PV efficiency under NOCT [-]
    :param beta: Temperature coefficient [-]
    :param NOCT: Nominal operating cell temperature [deg C]
    :param NOCT_ref: Reference temperature [deg C]
    :param NOCT_sol: Reference irradiance [W/m2]
    :param T_amb: Ambient temperature [deg C]
    :param I: Irradiance on panel [W/m2]. 8760 time series
    :return: Time resolved PV efficiency [-], 8760 entries
    """
    horizon = len(T_amb)
    etapv = [0.0] * horizon
    for i in range(0, horizon):
        Tpv = T_amb[i] + ((NOCT - NOCT_ref) / NOCT_sol) * I[i]
        etapv[i] = eta_PVref * (1 - beta * (Tpv - 25))
    return etapv


# def solar_thermal_yield(inlet_temp, ambient_temp, FRtaualpha, FRUL, irradiance, surface_area):
#     """
#     Calculate heating energy generated from a solar thermal collector
#     :param inlet_temp: Inlet temperature into the collector [°C], time series
#     :param ambient_temp: Ambient air temperature at the collector [°C], time series
#     :param FRtaualpha: Optical efficiency [-], constant
#     :param FRUL: Heat loss coefficient [W/m2K], constant
#     :param irradiance: Irradiance on the collector [W/m2], time series
#     :param surface_area: Surface area of the solar thermal collector [m2]
#     :returns: heating energy [kWh] time resolved, efficiency [-] time resolved
#     """
#
#     horizon = len(inlet_temp)
#     if horizon > len(ambient_temp):
#         horizon = len(ambient_temp)
#     if horizon > len(irradiance):
#         horizon = len(irradiance)
#
#     eta = [0.0] * horizon
#     heating = [0.0] * horizon
#
#     for i in range(horizon):
#         eta_temp = FRtaualpha - ((FRUL * (inlet_temp[i] - ambient_temp[i])) / irradiance[i])
#         eta[i] = max(0, eta_temp)
#         heating[i] = (irradiance[i] * eta[i] * surface_area) / 1000.0
#
#     return heating, eta