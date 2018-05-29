# Opaque element
#
# Oasys: A energy simulation plugin developed by the A/S chair at ETH Zurich
# This component is based on building_physics.py in the RC_BuildingSimulator Github repository
# https://github.com/architecture-building-systems/RC_BuildingSimulator
# Extensive documentation is available on the project wiki.
#
# Author: Justin Zarb <zarbj@student.ethz.ch>
#
# This file is part of Oasys
#
# Licensing/Copyright and liability comments go here.
# <Copyright 2018, Architecture and Building Systems - ETH Zurich>
# <Licence: MIT>

"""
Define an opaque by adding a surface.
-
Provided by Oasys 0.0.1
    
    Args:
        geometry: A brep or list of breps representing the heat-transfer area of the element
        element_name: optional element name
        u_value: element u-value in W/(m^2.K)
    Returns:
        centers: list of center points to check input
        normals: list of normals to check input
        opaque_elements: list of element objects representing each surface that was inputted.
"""

ghenv.Component.Name = "Hive_OpaqueElement"
ghenv.Component.NickName = 'OpaqueElement'
ghenv.Component.Message = 'VER 0.0.1\nAPR_24_2018'
ghenv.Component.IconDisplayMode = ghenv.Component.IconDisplayMode.application
ghenv.Component.Category = "Hive"
ghenv.Component.SubCategory = "1 | Zone"
# ComponentExposure=1

import scriptcontext as sc

def main(element_name,u_value,_geometry):
    if not sc.sticky.has_key('ElementBuilder'): 
        print "Add the modular RC component to the canvas!"
        return -1

    element_name = 'Opaque Element' if element_name is None else element_name
    Builder = sc.sticky['ElementBuilder'](element_name,u_value,1,True)
    
    HivePreparation = sc.sticky['HivePreparation']()
    surfaces = HivePreparation.deconstruct_input_geometry(_geometry)
    
    centers = []
    normals = []
    elements = []
    
    for s in surfaces:
        c, n, element = Builder.add_element(s)
        centers.append(c)
        normals.append(n)
        elements+=element
    
        for e in element:
            print e.name,'\n U=',e.u_value,'W/m2K, area:',round(e.area,2),'m2'
    
    return centers, normals, elements

result = main(element_name,u_value,_geometry)
if result != -1:
    centers,normals,elements = result
