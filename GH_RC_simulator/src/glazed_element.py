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
"""

ghenv.Component.Name = "Glazed Element"
ghenv.Component.NickName = 'GlazedElement'
ghenv.Component.Message = 'VER 0.0.1\nMar_06_2018'
ghenv.Component.IconDisplayMode = ghenv.Component.IconDisplayMode.application
ghenv.Component.Category = "Oasys"
ghenv.Component.SubCategory = " 1 | Zone"
#compatibleOasysVersion = VER 0.0.1\nFEB_21_2018
try: ghenv.Component.AdditionalHelpFromDocStrings = "2"
except: pass

import scriptcontext as sc
import rhinoscriptsyntax as rs
import Rhino as rc
import ghpythonlib.components as gh
import math
import datetime

"""
Builder object creates the window element object, representing the thermal 
properties of the window
"""

# Create thermal element
e_name = 'Window' if window_name is None else window_name

Builder = sc.sticky['ElementBuilder'](name,u_value,frame_factor,False)
centers,normals,glazed_elements = Builder.add_element(_window_geometry)

for g in glazed_elements:
    print g.name,':',g.u_value,'W/m2K',g.opaque

#==============================================================================#
#==============================================================================#

# Calculate solar gains and illuminance
transmittance = {}
transmittance['st'] = solar_transmittance if solar_transmittance is not None else 0.7
transmittance['lt'] = solar_transmittance if solar_transmittance is not None else 0.8

#TODO: Deal with polysurface input
Window = sc.sticky['RadiationWindow'](window_geometry=_window_geometry,
                                     context_geometry=context_geometry, 
                                     location=location,
                                     year=2005,
                                     glass_solar_transmittance = transmittance['st'],
                                     glass_light_transmittance = transmittance['lt'])

# Optional shadow visualisation
if hour_to_visualise is not None:
    sun_altitude,sun_azimuth,sun_vector = Window.calc_sun_position(hour_to_visualise)
    shadow = Window.unshaded_window_surfaces(sun_vector)

solar_gains = []
illuminance = []
if run_solar:
    for ii in range(0,len(hoy_list)):
        hoy = hoy_list[ii]
        dni = dni_list[ii]
        dhi = dhi_list[ii]
        
        if Window.is_sunny(hoy):
            solar_gains.append(Window.calc_solar_gains(hoy,dni,dhi))
            illuminance.append(Window.calc_illuminance(hoy,dni,dhi))
        else:
            solar_gains.append(0)
            illuminance.append(0)