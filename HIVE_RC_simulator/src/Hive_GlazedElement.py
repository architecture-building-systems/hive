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
ghenv.Component.Message = 'VER 0.0.1\nMAY_03_2018'
ghenv.Component.IconDisplayMode = ghenv.Component.IconDisplayMode.application
ghenv.Component.Category = "Hive"
ghenv.Component.SubCategory = "1 | Zone"
# ComponentExposure=1

import scriptcontext as sc
import rhinoscriptsyntax as rs
import Rhino as rc
import ghpythonlib.components as gh
import Grasshopper.Kernel as ghKernel
import math
import datetime

def build_glazed_element(window_name,_window_geometry,u_value,frame_factor):
    if not sc.sticky.has_key('ElementBuilder'): 
        return "Add the modular RC component to the canvas!"
    
    name = 'Window' if window_name is None else window_name

    Builder = sc.sticky['ElementBuilder'](name,u_value,frame_factor,False)
    centers,normals,glazed_elements = Builder.add_element(_window_geometry)
    
    for g in glazed_elements:
        print 'element name: ',g.name,'\n U-value:',g.u_value,'W/m2K \n area:',g.area,'m2'
    
    return centers, normals, glazed_elements


def solar_gains_through_element(window_geometry, point_in_zone, context_geometry, location, irradiation,solar_transmittance,light_transmittance, draw_shadows):
    """
    #TODO: Deal with polysurface input for shading
    #TODO: collect points and merge shadows in pyclipper for a faster shading visualisation.
    """
    solar_transmittance = solar_transmittance if solar_transmittance is not None else 0.7
    light_transmittance = light_transmittance if light_transmittance is not None else 0.8
    
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
    
    # Results
    dir_irradiation = []
    diff_irradiation = []
    diff_irradiation_simple = []
    ground_ref_irradiation = []
    window_illuminance = []
    window_solar_gains = []
    
    # Diagnostic
    sun_vectors = []
    incidence_angles = []
    unshaded_polys = []
    shading_factor = []
    
    for b in range(irradiation.BranchCount):
        # Initialize results
        incidence = 0
        solar_gains_this_hour = 0
        lighting = 0
        dnirr = 0
        dhirr = 0
        dhirr_simple = 0
        grirr = 0
        sun_vector = None
        unshaded_polys_hour = [None]
        
        # read radiation data
        hoy, normal_irradiation, horizontal_irradiation, normal_illuminance, horizontal_illuminance = list(irradiation.Branch(b))
        
        # Calculate sun
        relative_sun_alt,relative_sun_az = Sun.calc_relative_sun_position(hoy)
        sun_alt,sun_az = Sun.calc_sun_position(hoy)
        # print sun_az, relative_sun_az
        incidence = math.acos(math.cos(math.radians(relative_sun_alt)) * math.cos(math.radians(relative_sun_az)))

        # Calculate shading
        if context_geometry == []:
            # Window is unshaded
            unshaded_area = Window.window_area
            
            if draw_shadows and sun_alt > 0 and abs(relative_sun_az) < 90:
                unshaded_polys_hour = (Window.window_edges)
        else:
            #TODO: add a condition that checks shading geometry
            shadow_dict = Window.calc_gross_shadows(relative_sun_alt,relative_sun_az)
            unshaded_polygons = Window.calc_unshaded_polygons(shadow_dict)
            unshaded_area = Window.calc_unshaded_area(unshaded_polygons)
            if draw_shadows and sun_alt > 0 and abs(relative_sun_az) < 90:
                unshaded_polys_hour = Window.draw_unshaded_polygons(unshaded_polygons)
        
        dnirr, dhirr, dhirr_simple, grirr, lighting = Window.radiation(sun_alt, incidence, normal_irradiation, horizontal_irradiation, normal_illuminance, horizontal_illuminance, unshaded_area)
        solar_gains_this_hour = solar_transmittance * (dnirr + dhirr + grirr)
        shading_factor.append(unshaded_area/Window.window_area)

        if sun_alt > 0 and abs(relative_sun_az) < 90:
            sun_vector = Sun.calc_sun_vector(sun_alt,sun_az)
            incidence = math.degrees(incidence)
        
        # Append results
        dir_irradiation.append(dnirr)
        diff_irradiation.append(dhirr)
        diff_irradiation_simple.append(dhirr_simple)
        sun_vectors.append(sun_vector)
        incidence_angles.append(incidence)
        ground_ref_irradiation.append(grirr)
        window_illuminance.append(lighting)
        window_solar_gains.append(solar_gains_this_hour)
        if draw_shadows and sun_alt>0:
            unshaded_polys.append(unshaded_polys_hour)

    unshaded = sc.sticky['list_to_tree'](unshaded_polys)
    print 'Total solar gains:', round(sum(window_solar_gains),2),'Wh'
    print 'Mean shading factor: ', sum(shading_factor)/len(shading_factor)
     
    return incidence_angle, window_centroid, window_normal, sun_vectors, window_solar_gains, window_illuminance, dir_irradiation, diff_irradiation, diff_irradiation_simple, ground_ref_irradiation, unshaded

""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""

centers, normals, glazed_elements = build_glazed_element(window_name, _window_geometry, u_value, frame_factor)
window_area = [g.area for g in glazed_elements]

if (location and _point_in_zone and  _window_geometry):
    incidence_angle, window_centroid, window_normal, sun_vectors, solar_gains, illuminance, dir_irradiation, diff_irradiation, diff_irradiation_simple, ground_ref_irradiation, unshaded = solar_gains_through_element(_window_geometry, _point_in_zone, context_geometry, location, irradiation, solar_transmittance, light_transmittance, draw_shadows)
    

else:
    warning = """Warning: Insufficient inputs for solar calculations"""
    print 'location:',location
    print '_point_in_zone',_point_in_zone
    print '_window_geometry',_window_geometry
    w = ghKernel.GH_RuntimeMessageLevel.Warning
    ghenv.Component.AddRuntimeMessage(w, warning)