# Glazed element
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
Define an opaque by adding a surface.
-
Provided by HIVE 0.0.1
    
    Args:
        _window_geometry: a surface or polysurface representing the 
            heat-transfer area of the element
        window_name: optional element name
        _u_value: element u-value [W/(m^2.K)]
        solar_transmittance: (aka. g-factor) the percentage of radiation that can pass through glazing
        light_transmittance: the percentage of light that passes through glazing
    Returns:
        centers: list of center points to check input
        normals: list of normals to check input
        glazed_elements: list of element objects representing each surface that was inputted.
        solar_gains: hourly solar gains through the glazed element as a list
        illuminance: hourly illuminance through the glazed element as a list
        shadow: surface representing the shadow which corresponds to hour_to_visualise
"""

ghenv.Component.Name = "Hive_GlazedElement"
ghenv.Component.NickName = 'GlazedElement'
ghenv.Component.Message = 'VER 0.0.1\nAPR_23_2018'
ghenv.Component.IconDisplayMode = ghenv.Component.IconDisplayMode.application
ghenv.Component.Category = "Hive"
ghenv.Component.SubCategory = "1 | Zone"
# ComponentExposure=1

import scriptcontext as sc
import rhinoscriptsyntax as rs
import Rhino as rc
import ghpythonlib.components as gh
import math
import datetime

def build_glazed_element(window_name,_window_geometry,u_value,frame_factor):
    if not sc.sticky.has_key('ElementBuilder'): return "Add the modular RC component to the canvas!"
    
    name = 'Window' if window_name is None else window_name

    Builder = sc.sticky['ElementBuilder'](name,u_value,frame_factor,False)
    centers,normals,glazed_elements = Builder.add_element(_window_geometry)
    
    for g in glazed_elements:
        print g.name,':',g.u_value,'W/m2K',g.opaque
    
    return centers, normals, glazed_elements


def solar_gains_through_element(window_geometry, point_in_zone, context_geometry, location, irradiation,solar_transmittance,light_transmittance):
    """
    
    #TODO: Deal with polysurface input
    
    #TODO: account for the following:
    transmittance = {}
    transmittance['st'] = solar_transmittance if solar_transmittance is not None else 0.7
    transmittance['lt'] = light_transmittance if light_transmittance is not None else 0.8
    """
    
    if not sc.sticky.has_key('WindowRadiation'): return "Add the modular building physics component to the canvas!"
    
    Window = sc.sticky["WindowRadiation"](window_geometry=_window_geometry,
                                          point_in_zone=_point_in_zone,
                                          context_geometry=context_geometry)
    try:
        Sun = sc.sticky["RelativeSun"](location=location,
                          window_azimuth_rad=Window.window_azimuth_rad,
                          window_altitude_rad=Window.window_altitude_rad,
                          normal = Window.window_normal)
    except:
        print 'Connect Location data from the Hive_getSimulationData'
    
    window_centroid = Window.window_centroid
    window_normal = Window.window_normal
    
    dir_irradiation = []
    diff_irradiation = []
    ground_ref_irradiation = []
    window_illuminance = []
    unshaded_polys = []
    sun_vectors = []
    window_solar_gains = []
    window_illuminance = []
    dni = []
    
    for b in range(irradiation.BranchCount):
        hoy, normal_irradiation, horizontal_irradiation, normal_illuminance, horizontal_illuminance = list(irradiation.Branch(b))
        
        dni.append(normal_irradiation)
        
        relative_sun_alt,relative_sun_az = Sun.calc_relative_sun_position(hoy)
        sun_alt,sun_az = Sun.calc_sun_position(hoy)
        
        incidence = math.acos(math.cos(math.radians(sun_alt)) * math.cos(math.radians(relative_sun_az)))
        
        if Sun.is_sunny(relative_sun_alt,relative_sun_az):
            
            sun_vectors.append(Sun.calc_sun_vector(sun_alt,sun_az))
            
            if context_geometry == []:
                # Window is unshaded
                unshaded_area = Window.window_area
                unshaded_polys.append(rc.Geometry.Polyline())
            else:
                shadow_dict = Window.calc_gross_shadows(relative_sun_alt,relative_sun_az)
                unshaded_polygons = Window.calc_unshaded_polygons(shadow_dict)
                unshaded_area = Window.calc_unshaded_area(unshaded_polygons)
                unshaded_polys.append(Window.draw_unshaded_polygons(unshaded_polygons))
            
            dnirr, dhirr, grirr, lighting = Window.radiation(sun_alt, incidence, normal_irradiation, horizontal_irradiation, normal_illuminance, horizontal_illuminance, unshaded_area)
            
            print hoy, round(normal_irradiation,2), '    ' ,unshaded_area, '    ' , dnirr
            
            dir_irradiation.append(dnirr)
            diff_irradiation.append(dhirr)
            ground_ref_irradiation.append(grirr)
            window_illuminance.append(lighting)
            window_solar_gains.append(dnirr+dhirr+grirr)
    
            #TODO: collect points and merge shadows in pyclipper for a faster shading visualisation.
    
        else:
            print hoy
            window_solar_gains.append(0)
            window_illuminance.append(0)
            dir_irradiation.append(0)
            diff_irradiation.append(0)
            ground_ref_irradiation.append(0)
            unshaded_polys.append(gh.PolyLine())
            sun_vectors.append(None)
    
    unshaded = None
    
    return dni, window_centroid, window_normal, sun_vectors, window_solar_gains, window_illuminance, dir_irradiation, diff_irradiation, ground_ref_irradiation, unshaded


centers, normals, glazed_elements = build_glazed_element(window_name, _window_geometry,u_value,frame_factor)


dni, window_centroid, window_normal, sun_vectors, solar_gains, illuminance, dir_irradiation, diff_irradiation, ground_ref_irradiation, unshaded = solar_gains_through_element(_window_geometry, _point_in_zone, context_geometry, location, irradiation, solar_transmittance, light_transmittance)
