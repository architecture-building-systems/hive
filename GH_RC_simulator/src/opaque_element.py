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
        geometry: a surface or polysurface representing the heat-transfer area of the element
        element_name: optional element name
        u_value: element u-value in W/(m^2.K)
    Returns:
        centers: list of center points to check input
        normals: list of normals to check input
        opaque_elements: list of element objects representing each surface that was inputted.
"""

ghenv.Component.Name = "Opaque Element"
ghenv.Component.NickName = 'OpaqueElement'
ghenv.Component.Message = 'VER 0.0.1\nMAR_06_2018'
ghenv.Component.IconDisplayMode = ghenv.Component.IconDisplayMode.application
ghenv.Component.Category = "Oasys"
ghenv.Component.SubCategory = " 1 | Zone"
#compatibleOasysVersion = VER 0.0.1\nFEB_21_2018
try: ghenv.Component.AdditionalHelpFromDocStrings = "2"
except: pass

import scriptcontext as sc

Builder = sc.sticky['ElementBuilder'](element_name,u_value,None,False)
centers,normals,opaque_element = Builder.add_element(_geometry)

for e in opaque_element:
    print e.name,':', e.u_value,'W/m2K'