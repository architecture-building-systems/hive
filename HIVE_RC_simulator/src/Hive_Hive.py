#
# Hive: An educational plugin developed by the A/S chair at ETH Zurich
#
# This comoponent contains an object-oriented adaptation of the RC model 
# referred to as the 'Simple Hourly Method' in ISO 13790, (superceded by 
# EN ISO 52016-1).

# The code contains the version of building_physics.py found in the nested_rc 
# branch. The modifications make it easier to accomodate a more modular zone 
# definiton, made of a zone and elements. 
# See https://github.com/architecture-building-systems/RC_BuildingSimulator for
# extensive documentation is available on the project wiki.

# Authors: Prageeth Jayathissa <jayathissa@arch.ethz.ch>, Justin Zarb 
# <zarbj@student.ethz.ch>
# Adapted for Grasshopper by Justin Zarb
#
# This file is part of Hive
#
#TODO: Licensing/Copyright and liability stuff


"""
Place this component in the grasshopper workspace so that zones can be defined and simulations run.
-
Provided by Hive 0.0.1
"""

ghenv.Component.Name = "Hive_Hive"
ghenv.Component.NickName = 'Hive'
ghenv.Component.Message = 'VER 0.0.1\nMAY_14_2018'
ghenv.Component.IconDisplayMode = ghenv.Component.IconDisplayMode.application
ghenv.Component.Category = "Hive"
ghenv.Component.SubCategory = "0 | Core"
# ComponentExposure=1

import scriptcontext as sc
import rhinoscriptsyntax as rs
import Rhino as rc
import ghpythonlib.components as gh
import Grasshopper.Kernel as ghKernel
import math
import datetime
import time

from itertools import product
from operator import itemgetter
from Grasshopper.Kernel.Data import GH_Path
from Grasshopper import DataTree
from System.Collections.Generic import List
import System.Reflection
import os 

clipper_path = os.path.join(os.getenv('APPDATA'), 'Grasshopper', 'Libraries', 'clipper_library.dll')
clipper_library = System.Reflection.Assembly.LoadFrom(clipper_path)

clipper = clipper_library.ClipperLib.Clipper()
polyTree = clipper_library.ClipperLib.PolyTree
polyType = clipper_library.ClipperLib.PolyType
clipType = clipper_library.ClipperLib.ClipType
polyFillType = clipper_library.ClipperLib.PolyFillType

IntPoint = clipper_library.ClipperLib.IntPoint


class HivePreparation(object):
    """ 
    Credits: Ladybug
    This object contains some hand-picked functions from the Ladybug Preparation component.
    It provides support for other components which have been adapted from Ladybug to Hive.
    """

    def hour2Date(self, hour, alternate = False):
        numOfDays = [0, 31, 59, 90, 120, 151, 181, 212, 243, 273, 304, 334, 365]
        numOfHours = [24 * numOfDay for numOfDay in numOfDays]
        #print hour/24
        if hour%8760==0 and not alternate: return `31`+ ' ' + 'DEC' + ' 24:00'
        elif hour%8760==0: return 31, 11, 24
    
        for h in range(len(numOfHours)-1):
            if hour <= numOfHours[h+1]: month = self.monthList[h]; break
        try: month
        except: month = self.monthList[h] # for the last hour of the year
    
        if (hour)%24 == 0:
            day = int((hour - numOfHours[h]) / 24)
            time = `24` + ':00'
            hour = 24
        else:
            day = int((hour - numOfHours[h]) / 24) + 1
            minutes = `int(round((hour - math.floor(hour)) * 60))`
            if len(minutes) == 1: minutes = '0' + minutes
            time = `int(hour%24)` + ':' + minutes
        if alternate:
            time = hour%24
            if time == 0: time = 24
            month = self.monthList.index(month)
            return day, month, time
            
        return (`day` + ' ' + month + ' ' + time)

    def __init__(self):
        self.monthList = ['JAN', 'FEB', 'MAR', 'APR', 'MAY', 'JUN', 'JUL', 'AUG', 'SEP', 'OCT', 'NOV', 'DEC']
        self.numOfDays = [0, 31, 59, 90, 120, 151, 181, 212, 243, 273, 304, 334, 365]
        self.numOfDaysEachMonth = [31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31]
        self.numOfHours = [24 * numOfDay for numOfDay in self.numOfDays]
    
    strToBeFound = 'key:location/dataType/units/frequency/startsAt/endsAt'
    
    def getJD(self, month, day):
        numOfDays = [0, 31, 59, 90, 120, 151, 181, 212, 243, 273, 304, 334]
        return numOfDays[int(month)-1] + int(day)
    
    def epwLocation(self, epw_file):
        epwfile = open(epw_file,"r")
        headline = epwfile.readline()
        csheadline = headline.split(',')
        while 1>0: #remove empty cells from the end of the list if any
            try: float(csheadline[-1]); break
            except: csheadline.pop()
        locName = ''
        for hLine in range(1,4):
            if csheadline[hLine] != '-':
                locName = locName + csheadline[hLine] + '_'
        locName = locName[:-1]
        lat = csheadline[-4]
        lngt = csheadline[-3]
        timeZone = csheadline[-2]
        elev = csheadline[-1].strip()
        epwfile.close
        return locName, lat, lngt, timeZone, elev
    
    
    def cleanAndCoerceList(self, brepList):
        """ This definition clean the list and add them to RhinoCommon"""
        outputMesh = []
        outputBrep = []
        
        for id in brepList:
            if rs.IsMesh(id):
                geo = rs.coercemesh(id)
                if geo is not None:
                    outputMesh.append(geo)
                    try: rs.DeleteObject(id)
                    except: pass
                
            elif rs.IsBrep(id):
                geo = rs.coercebrep(id)
                if geo is not None:
                    outputBrep.append(geo)
                    try: rs.DeleteObject(id)
                    except: pass
                    
                else:
                    # the idea was to remove the problematice surfaces
                    # not all the geometry which is not possible since
                    # badGeometries won't pass rs.IsBrep()
                    tempBrep = []
                    surfaces = rs.ExplodePolysurfaces(id)
                    for surface in surfaces:
                        geo = rs.coercesurface(surface)
                        if geo is not None:
                            tempBrep.append(geo)
                            try: rs.DeleteObject(surface)
                            except: pass
                    geo = rc.Geometry.Brep.JoinBreps(tempBrep, sc.doc.ModelAbsoluteTolerance)
                    for Brep in tempBrep:
                        Brep.Dispose()
                        try: rs.DeleteObject(id)
                        except: pass
                    outputBrep.append(geo)
        return outputMesh, outputBrep
        
    def epwDataReader(self, epw_file, location = 'Somewhere!'):
        # weather data
        modelYear = [self.strToBeFound, location, 'Year', 'Year', 'Hourly', (1, 1, 1), (12, 31, 24)];
        dbTemp = [self.strToBeFound, location, 'Dry Bulb Temperature', 'C', 'Hourly', (1, 1, 1), (12, 31, 24)];
        dewPoint = [self.strToBeFound, location, 'Dew Point Temperature', 'C', 'Hourly', (1, 1, 1), (12, 31, 24)];
        RH = [self.strToBeFound, location, 'Relative Humidity', '%', 'Hourly', (1, 1, 1), (12, 31, 24)];
        windSpeed = [self.strToBeFound, location, 'Wind Speed', 'm/s', 'Hourly', (1, 1, 1), (12, 31, 24)];
        windDir = [self.strToBeFound, location, 'Wind Direction', 'degrees', 'Hourly', (1, 1, 1), (12, 31, 24)];
        dirRad = [self.strToBeFound, location, 'Direct Normal Radiation', 'Wh/m2', 'Hourly', (1, 1, 1), (12, 31, 24)];
        difRad = [self.strToBeFound, location, 'Diffuse Horizontal Radiation', 'Wh/m2', 'Hourly', (1, 1, 1), (12, 31, 24)];
        glbRad = [self.strToBeFound, location, 'Global Horizontal Radiation', 'Wh/m2', 'Hourly', (1, 1, 1), (12, 31, 24)];
        infRad = [self.strToBeFound, location, 'Horizontal Infrared Radiation Intensity', 'Wh/m2', 'Hourly', (1, 1, 1), (12, 31, 24)];
        dirIll = [self.strToBeFound, location, 'Direct Normal Illuminance', 'lux', 'Hourly', (1, 1, 1), (12, 31, 24)];
        difIll = [self.strToBeFound, location, 'Diffuse Horizontal Illuminance', 'lux', 'Hourly', (1, 1, 1), (12, 31, 24)];
        glbIll = [self.strToBeFound, location, 'Global Horizontal Illuminance', 'lux', 'Hourly', (1, 1, 1), (12, 31, 24)];
        cloudCov = [self.strToBeFound, location, 'Total Cloud Cover', 'tenth', 'Hourly', (1, 1, 1), (12, 31, 24)];
        visibility = [self.strToBeFound, location, 'Visibility', 'km', 'Hourly', (1, 1, 1), (12, 31, 24)];
        barPress = [self.strToBeFound, location, 'Barometric Pressure', 'Pa', 'Hourly', (1, 1, 1), (12, 31, 24)];
        epwfile = open(epw_file,"r")
        lnum = 1 # line number
        for line in epwfile:
            if lnum > 8:
                modelYear.append(float(line.split(',')[0]))
                dbTemp.append(float(line.split(',')[6]))
                dewPoint.append(float(line.split(',')[7]))
                RH.append(float(line.split(',')[8]))
                barPress.append(float(line.split(',')[9]))
                windSpeed.append(float(line.split(',')[21]))
                windDir.append(float(line.split(',')[20]))
                dirRad.append(float(line.split(',')[14]))
                difRad.append(float(line.split(',')[15]))
                glbRad.append(float(line.split(',')[13]))
                infRad.append(float(line.split(',')[12]))
                dirIll.append(float(line.split(',')[17]))
                difIll.append(float(line.split(',')[18]))
                glbIll.append(float(line.split(',')[16]))
                cloudCov.append(float(line.split(',')[22]))
            lnum += 1
        epwfile.close()
        return dbTemp, dewPoint, RH, windSpeed, windDir, dirRad, difRad, glbRad, dirIll, difIll, glbIll, cloudCov, infRad, barPress, modelYear
    
    def list_to_tree(self, nestedlist):
        layerTree = DataTree[object]()
        for i, item_list in enumerate(nestedlist):
            path = GH_Path(i)
            layerTree.AddRange(item_list,path)
        return layerTree
   
    def dict_to_tree(self, dict):
        dictTree = DataTree[object]()
        for day,value in dict.iteritems():
            path = GH_Path(day)
            dictTree.Add(value,path)
        return dictTree


class RelativeSun(object):
    def __init__(self,location,window_azimuth_rad, window_altitude_rad, normal):
        # Extract location data
        self.latitude_deg = float(location[0])
        self.longitude_deg = float(location[1])
        self.utc_offset = float(location[2])
        self.year = int(location[3])
        
        self.window_azimuth_rad = window_azimuth_rad
        self.window_altitude_rad = window_altitude_rad
        self.normal = normal
    
    def is_sunny(self, sun_alt, relative_sun_az):
        return sun_alt >= 0.0 and abs(relative_sun_az) < 90.0
    
    def calc_relative_sun_position(self, hoy):
        """
        Credits: JoanDM
        Calculate sun position in respect to the glazed surface. 
        - Traditional convention:
            Sun azimuth: Clockwise due north. i.e. [0,360) degrees
        - Facade convention:
            Sun azimuth: Zero is coincident with the normal to glazed surface. From there, 
                         it ranges clockwise and counter clockwise till 180, i.e. 
                         [-180, 180) relative degrees.

        :param utc_offset: Time zone offset i.r. to Prime Meridian [hours]
        :type utc_offset: int
        :param latitude_deg: Local Latitude [Degrees]. North+, South-.
        :type latitude_deg: float
        :param longitude_deg: Local Longitude [Degrees]. East+, West-.
        :type longitude_deg: float
        :param asf_normal_az: Azimuth position of normal to ASF plane  [Degrees](Traditional convention)
        :type asf_normal_az: int
        :return: sun_alt, asf_sun_az: Sun altitude and azimuth [Degrees] (Facade convention)
        :rtype: tuple (float, float)
        """
        
        # Calculate sun position following traditional convention
        sun_alt, sun_az = self.calc_sun_position(hoy)
        
        # Redefine azimuth following ASF convention
        relative_sun_az = self.calc_relative_azimuth(sun_az)
        relative_sun_alt = self.calc_relative_altitude(sun_alt)
        return relative_sun_alt, relative_sun_az
    
    def calc_sun_position(self, hoy):
        """
        Credits: Prageeth Jayathissa, Joan Domnech Masferrer 
    
        Calculates the sun position for a specific hour of the year and location,
        according to traditional convention.
        :param latitude_deg: Local Latitude [Degrees]. North+, South-.
        :type latitude_deg: float
        :param longitude_deg: Local Longitude [Degrees]. East+, West-.
        :type longitude_deg: float
        :param year: year
        :type year: int
        :param hoy: Hour of the year from the start. The first hour of January is 1
        :type hoy: int
        :param utc_offset: Time zone offset i.r. to Prime Meridian [hours]
        :type utc_offset: int
        :return: altitude, azimuth: Sun altitude and azimuth [Degrees] (Traditional convention)
        :rtype: tuple
        """
        # Source http://www.pveducation.org/pvcdrom/properties-of-sunlight/declination-angle
    
        # Set the date in UTC based off the hour of year and the year itself
        start_of_year = datetime.datetime(self.year, 1, 1, 0, 0, 0, 0);
        utc_datetime = start_of_year + datetime.timedelta(hours = float(hoy));
    
        # Determine the day of the year
        day_of_year = utc_datetime.timetuple().tm_yday;
    
        # Calculate B parameter (in radians) for equation of time
        b_factor = (day_of_year - 1) * ((2 * math.pi) / 365);
    
        # Equation of time: empirical equation that corrects for the eccentricity of 
        # the Earth's orbit and the Earth's axial tilt        
        equation_of_time = 229.2 * 0.000075 + 229.2 * (0.001868 * math.cos(b_factor) - \
                                                       0.032077 * math.sin(b_factor)) - \
                                                       229.2 * (0.014615 * math.cos(2 * b_factor) + \
                                                                0.04089 * math.sin(2 * b_factor));
    
        # Local Standart Time Meridian (Earth rotation 15 degrees/h)
        standard_time = 15 * self.utc_offset
    
        # Time correction between local standard time and true solar time (in minutes) 
        time_correction = 4 * (self.longitude_deg - standard_time) + equation_of_time 
    
        # Local Solar Time (in hours) = Local Time + TC (in hours)
        solar_time = utc_datetime.hour + (utc_datetime.minute + time_correction) / 60.0;
    
        # Translate Local Solar Time to an angle 
        hour_angle_rad = (math.pi * 2 / 24) * (solar_time - 12);
    
        # Calculate the declination angle: The variation due to the earths tilt
        declination_rad = math.radians(23.45 * math.sin((2 * math.pi / 365.0) * 
                                                        (day_of_year - 81)));
    
        # Convert latitude to to radians
        latitude_rad = math.radians(self.latitude_deg);
    
        # Altitude Position of the sun in radians
        sun_alt_rad = math.asin(math.cos(latitude_rad) * math.cos(declination_rad) * 
                                 math.cos(hour_angle_rad) +
                                 math.sin(latitude_rad) * math.sin(declination_rad));
    
        # Azimuth Position fo the sun in radians
        sun_az_rad = math.acos((math.sin(declination_rad) *
                                    math.cos(latitude_rad) - math.cos(declination_rad) *
                                    math.sin(latitude_rad) * math.cos(hour_angle_rad)) /
                                    math.cos(sun_alt_rad))
    
        # Range azimuth [0, 360) degrees
        if hour_angle_rad > 0 or hour_angle_rad < - math.pi :
            sun_az_rad = math.pi * 2 - sun_az_rad
    
        return math.degrees(sun_alt_rad), math.degrees(sun_az_rad)
    
    def calc_sun_vector(self,sun_alt, sun_az):
        sun_alt_rad = math.radians(sun_alt)
        sun_az_rad = math.radians(sun_az)
        sunvec_x = -math.sin(sun_az_rad) * math.cos(sun_alt_rad)
        sunvec_y = -math.cos(sun_az_rad) * math.cos(sun_alt_rad)
        sunvec_z = -math.sin(sun_alt_rad)
        
        return rc.Geometry.Vector3d(sunvec_x,sunvec_y,sunvec_z)

    def calc_relative_azimuth(self, sun_az):
        """
        Calculates the sun position relative to the facade orientation.
        :param sun_az: Sun azimuth [Degrees] (Traditional convention)
        :type sun_az: float
        :return: relative_sun_az: Sun azimuth [Degrees] (Facade convention)
        :rtype: float
        """
                    
        relative_sun_az = 180 - sun_az + math.degrees(self.window_azimuth_rad)
        if relative_sun_az < -180:
            relative_sun_az +=360
        return relative_sun_az
    
    def calc_relative_altitude(self, sun_alt):
        relative_sun_alt = sun_alt - 90 + math.degrees(self.window_altitude_rad)
        return relative_sun_alt


class WindowRadiation(object):
    """
    Adapted from shaded_window.py and radiation.py from the ASF_Control 
    repository by Joan DM 
    
    Contains functions to calculate window radiation with shading.
    """

    def __init__(self, window_geometry, context_geometry, point_in_zone, albedo=0.12, glass_solar_transmittance=0.7, glass_light_transmittance=0.8):
        
        self.window_geometry = window_geometry
        self.point_in_zone = point_in_zone
        
        #context geometry can either be a single surface, a polysurface, or a mixed list.
        context_surfaces = []
        for geometry in context_geometry:
            g = rs.coercebrep(geometry)
            brep_faces = gh.DeconstructBrep(g)[0]
            if brep_faces:
                if type(brep_faces) is not list:
                    # geometry is a single surface
                    s = brep_faces.Faces[0].DuplicateSurface()
                    context_surfaces.append(s)
                else:
                    # geometry was a polysurface and got deconstructed into a list
                    for b in brep_faces:
                        s = b.Faces[0].DuplicateSurface()
                        context_surfaces.append(s)
        
        self.clipper_accuracy = 100000
        self.extract_window_geometry()
        self.transform_all_shades(context_surfaces)
        
        self.albedo = albedo
        self.glass_solar_transmittance = glass_solar_transmittance
        self.glass_light_transmittance = glass_light_transmittance
        
        self.perez_coef = {'f_11': [-0.0083117, 0.1299457, 0.3296958, 0.5682053, 0.873028, 1.1326077, 1.0601591, 0.677747],
              'f_12': [0.5877285, 0.6825954, 0.4868735, 0.1874525, -0.3920403, -1.2367284, -1.5999137, -0.3272588],
              'f_13': [-0.0620636, -0.1513752, -0.2210958, -0.295129, -0.3616149, -0.4118494, -0.3589221, -0.2504286],
              'f_21': [-0.0596012, -0.0189325, 0.055414, 0.1088631, 0.2255647, 0.2877813, 0.2642124, 0.1561313],
              'f_22': [0.0721249, 0.065965, -0.0639588, -0.1519229, -0.4620442, -0.8230357, -1.127234, -1.3765031],
              'f_23': [-0.0220216, -0.0288748, -0.0260542, -0.0139754, 0.0012448, 0.0558651, 0.1310694, 0.2506212]}
    
    def extract_window_geometry(self):
        """
        Extract geometry properties 
        """
        
        north = rc.Geometry.Vector3d(0,1,0)
        vertical = rc.Geometry.Vector3d(0,0,1)
        
        # Window centroid
        self.window_centroid = rs.SurfaceAreaCentroid(self.window_geometry)[0]
        
        # Initialize outward facing normal
        normal = rs.SurfaceNormal(self.window_geometry,[0.5,0.5])
        window_zone_vector = gh.Vector2Pt(self.window_centroid,self.point_in_zone)
        if abs(rs.VectorAngle(normal,window_zone_vector[0])) < 90:
            self.window_normal = rs.VectorReverse(normal)
        else:
            self.window_normal = normal
        
        # Initialize window area and plane
        self.window_area = rs.Area(self.window_geometry)
        edges = rs.DuplicateEdgeCurves(self.window_geometry)
        self.window_vertices = rs.SurfaceEditPoints(self.window_geometry)
        
        
        lower_points = [p for p in self.window_vertices if p[2] == min([z[2] for z in self.window_vertices])]
        
        x_n = self.window_normal[0]
        if x_n > 0:
            basepoint = [p for p in lower_points if p[1] == min([y[1] for y in lower_points])][0]
        else:
            basepoint = [p for p in lower_points if p[1] == max([y[1] for y in lower_points])][0]
        
        plane_y = rc.Geometry.Vector3d(0,0,1)
        plane_x = rs.VectorRotate(plane_y,270,self.window_normal)
        self.window_plane = rc.Geometry.Plane(basepoint,plane_x,plane_y)
        
        self.window_transformation = gh.Orient(None,self.window_plane,gh.XYPlane(rc.Geometry.Point3d(0,0,0)))[1]
        
        self.window_edges = rs.JoinCurves(edges)
        edge_lengths = [rs.CurveLength(e) for e in edges]
        edge_vectors = [round(rs.VectorCreate(rs.CurveStartPoint(e),rs.CurveEndPoint(e))[2]) for e in edges] 
        window_width = edge_lengths[0] if edge_vectors[0] == 0 else edge_lengths[1]
        window_height = edge_lengths[0] if edge_vectors[0] !=0 else edge_lengths[1]
        window_intpoint_vertices = [IntPoint(0,0), IntPoint(self.clipper_accuracy*window_width, 0), IntPoint(self.clipper_accuracy*window_width, self.clipper_accuracy*window_height), IntPoint(0, self.clipper_accuracy*window_height)]
        self.window_frame = List[IntPoint](window_intpoint_vertices)

        # Find panel azimuth (X-Z axes). Set it in the proper quarter (+ CW / -CCW i.r to Z axis)
        azimuth = math.pi - math.atan2(-self.window_normal[0], self.window_normal[1])
        if azimuth > math.pi:
            azimuth -= 2*math.pi 
        
        try:
            altitude = rs.VectorAngle(rc.Geometry.Vector3d(0,0,1),normal)
        except ValueError:
            altitude = 0
        
        self.window_altitude_rad = math.radians(altitude)
        self.window_azimuth_rad = azimuth
    
    def sort_surface_vertices_clockwise(self,points,centroid,normal):
        """
        :return sorted_points: list of points sorted clockwise
        :rtype sorted_points: list
        """
        
        # Generate plane opposing the dominant direction of the surface
        if normal[0] == 0 and normal[2] == 0:
            # Normal parallel to y axis
            if normal[1]>0:
                plane = rc.Geometry.Plane(centroid,rc.Geometry.Vector3d(0,-1,0))
            else:
                plane = rc.Geometry.Plane(centroid,rc.Geometry.Vector3d(0,1,0))
        
        if normal[1] == 0 and normal[2] == 0:
            # Normal parallel to x axis
            if normal[0]>0:
                plane = rc.Geometry.Plane(centroid,rc.Geometry.Vector3d(-1,0,0))
            else:
                plane = rc.Geometry.Plane(centroid,rc.Geometry.Vector3d(1,0,0))
        
        if normal[2] != 0:
            # Normal not on xy plane
            plane = rc.Geometry.Plane(centroid,rc.Geometry.Vector3d(0,0,-1))
        
        else:
            rs.VectorReverse(normal)
            # Trying this out! Need to remember what the previous code did...
            plane = rc.Geometry.Plane(centroid,rs.VectorReverse(normal))
            
        # calculate vectors from center to points
        vecs = [centroid - point for point in points]
        
        # calculate angles between vectors and plane x-axis
        angles = [rc.Geometry.Vector3d.VectorAngle(plane.XAxis,v,plane) for v in vecs]

        sorted_points = [p[1] for p in sorted(zip(angles,points))] 
        return sorted_points
    
    def transform_shade(self,shade):
        """
        Transform the vertices of a shading surface to match the window as projected on the xy plane.
        case 1: All vertices are in front of the window
        case 2: Some vertices are behind the window.
        case 3: All the vertices are behind the window, a null value is returned.
        
        :return rounded_oriented_centroid: oriented centroid. rounding affects how easily array patterns are identified.
        :return transformed points: 
        """
        
        # Test shading position
        points = rs.SurfacePoints(shade,True)
        a,b,c,d = rs.PlaneEquation(self.window_plane)
        point_is_outside_window = [a*p[0] + b*p[1] + c*p[2] + d > 0 for p in points]
        if all(point_is_outside_window):
            # Case 1
            pass
        elif any(point_is_outside_window):
            # case 2: add the intersection points to the list of points
            intersection = gh.BrepXPlane(shade,self.window_plane)[0]
            if intersection is not None:
                # Case 2
                points.append(rs.CurveEndPoint(intersection))
                points.append(rs.CurveStartPoint(intersection))
        else:
            # Case 3
            return None, None
            print 'shade element ignored as it is behind the window.'
        
        centroid, e= rs.SurfaceAreaCentroid(shade)
        normal = rs.SurfaceNormal(shade,[0.5,0.5])
        clockwise_shade_points = self.sort_surface_vertices_clockwise(points,centroid,normal)
        
        def round2(x):
            return int(round(x*100))
        
        oriented_centroid = gh.Transform(centroid,self.window_transformation)
        rounded_oriented_centroid = [round2(oriented_centroid[0]),round2(oriented_centroid[1])]
        
        transformed_points = []
        for p in clockwise_shade_points:
            p1 = gh.Transform(p,self.window_transformation)
            if round(p1[2],2) >= 0:
                # point is in front of window
                transformed_points.append(p1)
        
        return rounded_oriented_centroid, transformed_points
    
    def transform_all_shades(self,context_geometry):
        """
        Check if points are aligned horizontally or vertically and create an array
        Transform context
        Return a dictionary with
        key: shade centroid
        value: transformed points for each shade
        """
        self.context = {}
        points_dict = {}
        index = 0
        for xg in context_geometry:
            c,points = self.transform_shade(xg)
            if c is not None:
                self.context[index] = points
                index += 1
    
    def calc_shadow(self, shade, relative_sun_alt, relative_sun_az):
        """
        Coordinate system:
        X = Horizontal direction, parallel to glazing surface
        Y = Vertical direction, parallel to glazing surface
        Z = Perpendicular in respect of glazing surface
        """
        
        def transform_point(point,sunvec_x,sunvec_y,sunvec_z):
            # Define t paramater from parametric equation. Window plane is placed z=0 
            t = - point[2] / sunvec_z;
            # Project points using sun vector
            x_point = point[0] + sunvec_x * t;
            y_point = point[1] + sunvec_y * t;
            return x_point, y_point
        
        # Align relative_sun_az with window plane, convert angles to radians
        # Note: this procedure is only done when is sunny, hence relative_sun_az in range[-90, 90];
        relative_sun_az_rad, relative_sun_alt_rad = map(math.radians, [relative_sun_az + 90, relative_sun_alt]);
    
        # Create sun vector
        sunvec_x = -math.cos(relative_sun_az_rad) * math.cos(relative_sun_alt_rad);
        sunvec_y = math.sin(relative_sun_alt_rad);
        sunvec_z = math.sin(relative_sun_az_rad) * math.cos(relative_sun_alt_rad);
        
        a, b = self.longer_edge_of_shade(shade)
        a_shadow = transform_point(a,sunvec_x,sunvec_y,sunvec_z)
        b_shadow = transform_point(b,sunvec_x,sunvec_y,sunvec_z)
        pv_shadow_vector = [b_shadow[0] - a_shadow[0], b_shadow[1] - a_shadow[1]]
        
        # Calculate the projection on a 2-D surface (x,y)
        shadowintpoints = [] # IntPoints for clipping
        for point_3d in shade:   
            x_point, y_point = transform_point(point_3d,sunvec_x,sunvec_y,sunvec_z)
            shadowintpoints.append(IntPoint(self.clipper_accuracy*x_point,self.clipper_accuracy*y_point))
            
        return List[IntPoint](shadowintpoints)#, pv_shadow_vector
    
    def longer_edge_of_shade(self,shade_points):
        """
        Identify the longer edge of the shading geometry, from 
        the window-side outwards, in the window coordinate system.
        Returns: a (closest to the window), b (farthest from the window)
        """
        long_edge_length = 0
        for ii in range(0,len(shade_points)-1):
            if abs(rs.Distance(shade_points[ii],shade_points[ii+1])) > long_edge_length:
                long_edge_length = abs(rs.Distance(shade_points[ii],shade_points[ii+1]))
                if rs.DistanceToPlane(self.window_plane,shade_points[ii]) < rs.DistanceToPlane(self.window_plane,shade_points[ii+1]):
                    a = shade_points[ii]
                    b = shade_points[ii+1]
                else:
                    a = shade_points[ii+1]
                    b = shade_points[ii]
        return a,b
        
    def calc_gross_shadows(self,relative_sun_alt,relative_sun_az):
        """
        projects all shading objects onto window coordinate plane
        """
        if self.context == {}:
            return None
        
        gross_shadows = {}
        pv_vectors = {}
        for k in self.context.keys():
            gross_shadows[k] = self.calc_shadow(self.context[k],relative_sun_alt,relative_sun_az)
        return gross_shadows
    
    def draw_unshaded_polygons(self,clipper_result):
        """
        optional function which returns polylines of the trimmed shadows for each hour
        :param clipper_result: a .net List of polygons (list of intPoints)
        :type clipper_result: List(List(intPoint))
        """
        clipper_result_geometry = []

        u_domain = rs.SurfaceDomain(self.window_geometry,0)[1]
        
        for ms in clipper_result:
            points = []
            for p in ms:
#                points.append(rc.Geometry.Point3d(p.X/self.clipper_accuracy, p.Y/self.clipper_accuracy,0))
                points.append(rs.EvaluatePlane(self.window_plane, [p.X/self.clipper_accuracy, p.Y/self.clipper_accuracy]))
            clipper_result_geometry.append(gh.PolyLine(points,closed=True))
        return clipper_result_geometry

    def calc_unshaded_area(self,unshaded_polygons):
        """
        For a given set of shadows (one hour)
        Subtract from net_shadows dictionary the regions outside the window frame and
        compute the total shadow laying on the window as a % of its area.
        """
        
        shaded_area = 0
        for p in unshaded_polygons:
            shaded_area += abs(clipper.Area(p))
         
        try:
            assert clipper.Area(self.window_frame) >= shaded_area
            return (clipper.Area(self.window_frame) - shaded_area)/ self.clipper_accuracy**2
        except AssertionError:
            # clipper failed and returned overlapping geometries.
            print 'clipping warning for hour: ', h

    def calc_unshaded_polygons(self,gross_shadows):
        """
        Combine shadow polygons and remove them from the window frame.
        Returns: polygons representing unshaded areas on the window.
        """
        # Initialise shadows list
        subj = []
        shadows = List[List[IntPoint]](gross_shadows.values())
        merged_shadows = List[List[IntPoint]]()
        clipper.Clear()
        clipper.AddPolygons(shadows,polyType.ptSubject)
        union = clipper.Execute(clipType.ctUnion, merged_shadows, polyFillType.pftNonZero, polyFillType.pftNonZero)
        
        # Remove parts of shadows which lie outside the window frame
        unshaded_polygons = List[List[IntPoint]]()
        clipper.Clear()
        clipper.AddPolygon(self.window_frame,polyType.ptClip)
        clipper.AddPolygons(merged_shadows,polyType.ptSubject)
        diff = clipper.Execute(clipType.ctIntersection, unshaded_polygons, polyFillType.pftNonZero, polyFillType.pftNonZero)
        
        if diff:
            return unshaded_polygons
        else:
            return None

    def perez(self, angle_incidence, sun_alt, norm_radiation, hor_radiation):
        """"
        Credits: JoanDM
        Calculate the diffuse irradiation components (circumsolar and horizon brightening)
        via Perez 1990 model.
    
        :param angle_incidence: Angle of incidence between sun rays and panel [rad]
        :type angle_incidence: float
        :param sun_alt: sun altitude
        :type sun_alt: float
        :param norm_radiation: Direct normal radiation [Wh/m^2]. 
        :type norm_radiation: float
        :param hor_radiation: Diffuse horizontal radiation [Wh/m^2]. 
        :type hor_radiation: float
        :type sun_alt: dict {key:string, value:float}
        :return: circumsolar and horizon brightening components [Wh/m^2].
        :rtype: tuple(float, float)
        """
        
        a = max(0, math.cos(angle_incidence))
        b = max(math.cos(math.radians(85)), math.cos(angle_incidence))
        k = 5.534 / 1000000
        zenith = 90 - sun_alt
    
        if 0 <= zenith <= 87.5:
            epsilon = ((norm_radiation + hor_radiation) / float(hor_radiation) + k * ( zenith ** 3)) / (1 + k * (zenith ** 3))
            
            air_mass = (math.cos(b) + 0.15 * (93.9 - zenith) ** (-1.253) ) ** (-1)
            
            delta = hor_radiation * (air_mass / 1367.0)
            
            if epsilon <= 1.065:
                coeff_selector = 0
            elif epsilon <= 1.23:
                coeff_selector = 1
            elif epsilon <= 1.5:
                coeff_selector = 2
            elif epsilon <= 1.95:
                coeff_selector = 3
            elif epsilon <= 2.8:
                coeff_selector = 4
            elif epsilon <= 4.5:
                coeff_selector = 5
            elif epsilon <= 6.2:
                coeff_selector = 6
            elif epsilon > 6.2:
                coeff_selector = 7
            
            f_11 = self.perez_coef['f_11'][coeff_selector]
            f_12 = self.perez_coef['f_12'][coeff_selector]
            f_13 = self.perez_coef['f_13'][coeff_selector]
            f_21 = self.perez_coef['f_21'][coeff_selector]
            f_22 = self.perez_coef['f_22'][coeff_selector]
            f_23 = self.perez_coef['f_23'][coeff_selector]
            
            zenith_rad = math.radians(zenith)
            f_1 = max(0, f_11 + delta * f_12 + zenith_rad * f_13)
            f_2 = max(0, f_21 + delta * f_22 + zenith_rad * f_23)
            
            diff_circum = hor_radiation * f_1 * a / b
            diff_horizon = hor_radiation * f_2 * math.sin(self.window_altitude_rad)
    
        else: #87.5 < zenith <  90:
            f_1 = 0
            diff_circum = 0
            diff_horizon = 0
    
        return  diff_circum, diff_horizon, f_1
    
    def calc_diffuse_irradiation(self, norm_radiation, hor_radiation, view_factor, sun_alt, angle_incidence):
        """
        credits: JoanDM
        
        TODO: Include the surrounding effect of the shading geometry on the window view factor
        """
        
        diff_circum, diff_horizon, f_1 = self.perez(angle_incidence, sun_alt, norm_radiation, hor_radiation);
        
        diff_isotropic = hor_radiation * view_factor * (1 - f_1)
                                               
        if diff_circum > 2 * diff_isotropic:
            diff_isotropic = 0.0
        
        if diff_circum > 2 * diff_horizon:
            diff_horizon = 0
        return (diff_isotropic + diff_circum + diff_horizon) * self.window_area
    
    def calc_ground_ref_irradiation(self, norm_radiation, hor_radiation, sun_alt, view_factor):
        sun_alt_rad = math.radians(sun_alt)
        global_hor_radiation = norm_radiation * math.sin(sun_alt_rad) + hor_radiation
        ground_ref_radiation = self.albedo * global_hor_radiation * (1 - view_factor)
        
        return ground_ref_radiation
    
    def radiation(self, sun_alt, angle_incidence, normal_irradiation, horizontal_irradiation, normal_lux, horizontal_lux, unshaded_area):
        """
        credits: JoanDM
        """
        dir_irradiation = 0
        diff_irradiation = 0
        diff_irradiation_simple = 0
        ground_ref_irradiation = 0
        normal_lux = 0
        window_diff_lux = 0
        window_ground_ref_lux = 0
        
        # VF is a combination of tilt and shaded area
        view_factor = ((1 + math.cos(self.window_altitude_rad)) / 2) * unshaded_area/self.window_area
        
        # Direct Irradiation
        if normal_irradiation != 0 and math.cos(angle_incidence) > 0:
            dir_irradiation = normal_irradiation * math.cos(angle_incidence) * unshaded_area
        
        if normal_lux != 0:
            window_dir_lux = normal_lux * math.cos(angle_incidence) * unshaded_area
        
        # Diffuse Irradiation
        if not horizontal_irradiation == 0:
            diff_irradiation_simple =  horizontal_irradiation * ((1 + math.cos(self.window_altitude_rad)) / 2) * self.window_area
            diff_irradiation = self.calc_diffuse_irradiation(normal_irradiation, horizontal_irradiation,
                                                             view_factor, sun_alt, angle_incidence)  
            # Calculate diffuse and reflected irradiation
            ground_ref_irradiation = self.calc_ground_ref_irradiation(normal_irradiation, 
                                                                 horizontal_irradiation, sun_alt,
                                                                 view_factor)  
        
        window_diff_lux = horizontal_lux * view_factor
        window_ground_ref_lux = self.albedo * horizontal_lux * (1 - view_factor)
        window_illuminance =  normal_lux + window_diff_lux + window_ground_ref_lux
        
        return dir_irradiation, diff_irradiation, diff_irradiation_simple, ground_ref_irradiation, window_illuminance


class Element(object):
    """
    Element object representing an opaque or transparent element.
    ##OUTLOOK##
    absorptivity, reflectivity: Not yet implemented, but adding these parameters 
    would improve the accuracy of solar gains calculations in opaque and 
    transparent elements.The RC model would need to be adapted for this. Note to 
    future developers: when adding attributes, make sure to upadate the copy 
    method below with the same attributes!
    """
    def __init__(self, name = 'Element', area = 15.0, u_value = 1.0, frame_factor=1.0, opaque = True):

        self.name = name
        self.area = area
        self.u_value = u_value
        self.h_tr = self.u_value * self.area #element conductance [W/K]
        self.frame_factor = frame_factor
        self.opaque = opaque


class ElementBuilder(object):
    def __init__(self,element_name,u_value,frame_factor,opaque):
        self.element_name = element_name if element_name is not None else 'Wall'
        self.opaque = opaque
        if self.opaque:
            self.u_value = 0.2 if u_value is None else u_value
            self.frame_factor = 1
        else:
            self.u_value = 1 if u_value is None else u_value
            self.frame_factor = 1 if frame_factor is None else frame_factor

    def get_data(self,surface):
        centroid = rs.SurfaceAreaCentroid(surface)[0]
        normal = rs.SurfaceNormal(surface,[0.5,0.5])
        area = rs.SurfaceArea(surface)[0]
        return centroid,normal,area

    def add_element(self,geometry):
        centroids = []
        normals = []
        elements = []
        if geometry is not None:
            # Invalid input
            if not (rs.IsSurface(geometry) or rs.IsPolysurface(geometry)):
                error = """geometry is not a surface or polysurface"""
                e = ghKernel.GH_RuntimeMessageLevel.Error
                ghenv.Component.AddRuntimeMessage(e, error)
        
            # Single surface
            elif rs.IsSurface(geometry):
                centroid,normal,area = self.get_data(geometry)
                centroids = centroid
                normals = normal
                elements = [sc.sticky['Element'](
                    name=self.element_name,
                    area=area,
                    u_value=self.u_value, 
                    frame_factor=self.frame_factor,
                    opaque = self.opaque)]

            # Polysurface
            elif rs.IsPolysurface(geometry):
                all_parts = rs.ExplodePolysurfaces(geometry)
                for part in all_parts:
                    centroid,normal,area,azimuth,angle = self.get_data(part)
                    centroids.append(centroid)
                    normals.append(normal)
                    element = sc.sticky['Element'](
                        name=self.element_name,
                        area=area,
                        u_value=self.u_value, 
                        frame_factor=self.frame_factor,
                        opaque = self.opaque)
                    elements.append(element)
        return centroids,normals,elements


class ThermalBridge(object):
    def __init__(self,
                 name,
                 length,
                 linear_conductance):
        self.name = name if name is not None else 'Thermal bridge'
        self.name = name
        self.length = length
        self.linear_conductance = linear_conductance
        self.h_tr = length * linear_conductance


class ThermalZone(object):
    def __init__(self,
                 occupants = 1, 
                 elements = None,
                 thermal_bridges = None,
                 floor_area = 34.3,
                 volume = 106.33,
                 thermal_capacitance_per_floor_area=165000,
                 ach_vent=1.5,
                 ach_infl=0.5,
                 ventilation_efficiency=1,
                 t_set_heating = 20,
                 t_set_cooling = 26,
                 max_heating_energy_per_floor_area = 12,
                 max_cooling_energy_per_floor_area = -12,
                 heating_supply_system=sc.sticky["DirectHeater"],
                 cooling_supply_system=sc.sticky["DirectCooler"],
                 heating_emission_system=sc.sticky["AirConditioning"],
                 cooling_emission_system=sc.sticky["AirConditioning"],
                ):

        # Element objects
        self.elements = elements
        self.elements_added = 0  # for reporting purposes
        self.element_names = []  # for reporting purposes
        
        # Thermal bridges
        self.thermal_bridges = thermal_bridges

        # direct inputs
        self.occupants = occupants
        self.floor_area = floor_area
        self.volume = volume
        self.total_internal_area = floor_area * 4.5 #ISO13790 7.2.2.2
        self.ach_vent = ach_vent
        self.ach_infl = ach_infl
        self.ventilation_efficiency = ventilation_efficiency
        self.thermal_capacitance_per_floor_area=thermal_capacitance_per_floor_area
        self.max_heating_energy_per_floor_area = max_heating_energy_per_floor_area
        self.max_cooling_energy_per_floor_area = max_cooling_energy_per_floor_area
        self.heating_supply_system = heating_supply_system
        self.heating_emission_system = heating_emission_system
        self.cooling_supply_system = cooling_supply_system
        self.cooling_emission_system = cooling_emission_system
        self.t_set_heating = t_set_heating
        self.t_set_cooling = t_set_cooling

        # initialize envelope properties
        self.h_tr_em = 0
        self.h_tr_w = 0
        self.wall_area = 0
        self.window_area = 0
        self.window_wall_ratio = 0

        #if left blank, zone elements will be set to ASF default values
        if self.elements == None:
            Window = Element(name='ASF_window', area=13.5, u_value=1.1)
            Wall = Element(name='ASF_wall', area=1.69, u_value=0.2)
            self.elements = [Window,Wall]

        for element in self.elements:
            self.add_elements(element)

        if self.thermal_bridges is not None:
            for tb in self.thermal_bridges:
                self.add_thermal_bridge(tb)


    def add_elements(self,e):
        self.element_names.append(e.name)
        # add surface conductances to conductance of mass
        if e.opaque:
            self.h_tr_em += e.h_tr
            self.elements_added += 1
            self.wall_area += e.area
        # add window conductance to window conductances
        else:
            self.h_tr_w += e.h_tr
            self.elements_added += 1
            self.window_area += e.area

    def add_thermal_bridge(self,tb):
        self.h_tr_em += tb.h_tr
        self.elements_added += 1

    def summary(self):
        #report the number of elements added to facilitate bug detection
        print 'Zone with %i elements'%len(self.elements)
        print 'Conductance of opaque surfaces to exterior [W/K], h_tr_em:', self.h_tr_em
        print 'Conductance to exterior through glazed surfaces [W/K], h_tr_w', self.h_tr_w
        print 'windows: %f m2, walls: %f m2, total: %f m2'%(self.window_area,self.wall_area,self.window_area+self.wall_area)
        try:
            print 'window to wall ratio: %f %%\n'%(int(round(self.window_area/self.wall_area*100,1)))
        except ZeroDivisionError:
            print '100% glazed'


class ElementBuilding(object):
    """
    The modular version. 
    Sets the parameters of the building.
    """

    def __init__(self,
                 zone=None,
                 lighting_load=11.7,
                 lighting_control=300.0,
                 lighting_utilisation_factor=0.45,
                 lighting_maintenance_factor=0.9,
                 ):

        # Initialise Zone
        self.zone = zone
        if self.zone == None:
            self.zone = Zone()

        # Fenestration and Lighting Properties
        self.lighting_load = lighting_load  # [kW/m2] lighting load
        self.lighting_control = lighting_control  # [lux] Lighting set point
        # How the light entering the window is transmitted to the working plane
        self.lighting_utilisation_factor = lighting_utilisation_factor
        # How dirty the window is. Section 2.2.3.1 Environmental Science
        # Handbook
        self.lighting_maintenance_factor = lighting_maintenance_factor

        # Calculated Properties
        self.floor_area = self.zone.floor_area # [m2] Floor Area
        # [m2] Effective Mass Area assuming a medium weight building #12.3.1.2
        # very light: 2.5 x Af
        # heavy: 3.0 x Af
        # very heavy: 3.5 x Af
        self.mass_area = self.floor_area * 2.5
        self.room_vol = self.zone.volume# [m3] Room Volume
        self.total_internal_area = self.zone.total_internal_area
        # TODO: Standard doesn't explain what A_t is. Needs to be checked
        self.A_t = self.total_internal_area

        # Single Capacitance  5 conductance Model Parameters
        # [kWh/K] Room Capacitance. Default based on ISO standard 12.3.1.2 for medium heavy buildings
        self.c_m = self.zone.thermal_capacitance_per_floor_area * self.floor_area
        # Conductance of opaque surfaces to exterior [W/K]
        self.h_tr_em = self.zone.h_tr_em
        # Conductance to exterior through glazed surfaces [W/K], based on
        # U-wert of 1W/m2K
        self.h_tr_w = self.zone.h_tr_w
        # Determine the ventilation conductance
        ach_tot = self.zone.ach_infl + self.zone.ach_vent  # Total Air Changes Per Hour
        # temperature adjustment factor taking ventilation and infiltration
        # [ISO: E -27]
        if ach_tot > 0:
            b_ek = 1 - (self.zone.ach_vent / ach_tot) * self.zone.ventilation_efficiency
        else:
            b_ek = 1
        # b_ek = (self.zone.ach_vent/ ach_tot) * self.zone.ventilation_efficiency
        self.h_ve_adj = 1200 * b_ek * self.room_vol * \
            (ach_tot / 3600)  # Conductance through ventilation [W/M]
        # transmittance from the internal air to the thermal mass of the
        # building
        self.h_tr_ms = 9.1 * self.mass_area
        # Conductance from the conditioned air to interior building surface
        self.h_tr_is = self.total_internal_area * 3.45

        # Thermal set points
        self.t_set_heating = self.zone.t_set_heating
        self.t_set_cooling = self.zone.t_set_cooling

        # Thermal Properties
        self.has_heating_demand = False  # Boolean for if heating is required
        self.has_cooling_demand = False  # Boolean for if cooling is required
        self.max_cooling_energy = zone.max_cooling_energy_per_floor_area * \
            self.floor_area  # max cooling load (W/m2)
        self.max_heating_energy = zone.max_heating_energy_per_floor_area * \
            self.floor_area  # max heating load (W/m2)
        # Building System Properties
        self.heating_supply_system = zone.heating_supply_system
        self.cooling_supply_system = zone.cooling_supply_system
        self.heating_emission_system = zone.heating_emission_system
        self.cooling_emission_system = zone.cooling_emission_system

    def solve_building_lighting(self, illuminance, occupancy):
        """
        Calculates the lighting demand for a set timestep
        :param illuminance: Illuminance transmitted through the window [Lumens]
        :type illuminance: float
        :param : Probability of full occupancy
        :type occupancy: float
        :return: self.lighting_demand, Lighting Energy Required for the timestep
        :rtype: float
        """
        # Cite: Environmental Science Handbook, SV Szokolay, Section 2.2.1.3
        # also, this might be sped up by pre-calculating the constants, but idk. first check with profiler...
        lux = (illuminance * self.lighting_utilisation_factor *
               self.lighting_maintenance_factor) / self.floor_area  # [Lux]

        if lux < self.lighting_control and occupancy > 0:
            # Lighting demand for the hour
            self.lighting_demand = self.lighting_load * self.floor_area
        else:
            self.lighting_demand = 0

    def solve_building_energy(self, internal_gains, solar_gains, t_out, t_m_prev):
        """
        Calculates the heating and cooling consumption of a building for a set timestep
        :param internal_gains: internal heat gains from people and appliances [W]
        :type internal_gains: float
        :param solar_gains: solar heat gains [W]
        :type solar_gains: float
        :param t_out: Outdoor air temperature [C]
        :type t_out: float
        :param t_m_prev: Previous air temperature [C]
        :type t_m_prev: float
        :return: self.heating_demand, space heating demand of the building
        :return: self.heating_sys_electricity, heating electricity consumption
        :return: self.heating_sys_fossils, heating fossil fuel consumption
        :return: self.cooling_demand, space cooling demand of the building
        :return: self.cooling_sys_electricity, electricity consumption from cooling
        :return: self.cooling_sys_fossils, fossil fuel consumption from cooling
        :return: self.electricity_out, electricity produced from combined heat pump systems
        :return: self.sys_total_energy, total exergy consumed (electricity + fossils) for heating and cooling
        :return: self.heating_energy, total exergy consumed (electricity + fossils) for heating
        :return: self.cooling_energy, total exergy consumed (electricity + fossils) for cooling
        :return: self.cop, Coefficient of Performance of the heating or cooling system
        :rtype: float
        """
        # Main File
        # Calculate the heat transfer definitions for formula simplification
        self.calc_h_tr_1()
        self.calc_h_tr_2()
        self.calc_h_tr_3()

        # check demand, and change state of self.has_heating_demand, and self._has_cooling_demand
        self.has_demand(internal_gains, solar_gains, t_out, t_m_prev)

        if not self.has_heating_demand and not self.has_cooling_demand:
            # no heating or cooling demand
            # calculate temperatures of building R-C-model and exit
            # --> rc_model_function_1(...)
            self.energy_demand = 0

            # y u no pep8 bra?
            self.heating_demand = 0  # Energy required by the Zone
            self.cooling_demand = 0  # Energy surplus of the Zone
            # Energy (in electricity) required by the supply system to provide
            # HeatingDemand
            self.heating_sys_electricity = 0
            # Energy (in fossil fuel) required by the supply system to provide
            # HeatingDemand
            self.heating_sys_fossils = 0
            # Energy (in electricity) required by the supply system to get rid
            # of CoolingDemand
            self.cooling_sys_electricity = 0
            # Energy (in fossil fuel) required by the supply system to get rid
            # of CoolingDemand
            self.cooling_sys_fossils = 0
            # Electricity produced by the supply system (e.g. CHP)
            self.electricity_out = 0

        else:
            # has heating/cooling demand

            # Calculates energy_demand used below
            self.calc_energy_demand(
                internal_gains, solar_gains, t_out, t_m_prev)

            self.calc_temperatures_crank_nicolson(
                self.energy_demand, internal_gains, solar_gains, t_out, t_m_prev)
            # calculates the actual t_m resulting from the actual heating
            # demand (energy_demand)

            # Calculate the Heating/Cooling Input Energy Required

            supply_director = sc.sticky["SupplyDirector"]()  # Initialise Heating System Manager

            if self.has_heating_demand:
                supply_director.set_builder(self.heating_supply_system(load=self.energy_demand,
                                                                t_out=t_out,
                                                                heating_supply_temperature=self.heating_supply_temperature,
                                                                cooling_supply_temperature=self.cooling_supply_temperature,
                                                                has_heating_demand=self.has_heating_demand,
                                                                has_cooling_demand=self.has_cooling_demand))
                supplyOut = supply_director.calc_system()
                # All Variables explained underneath line 467
                self.heating_demand = self.energy_demand
                self.heating_sys_electricity = supplyOut.electricity_in
                self.heating_sys_fossils = supplyOut.fossils_in
                self.cooling_demand = 0
                self.cooling_sys_electricity = 0
                self.cooling_sys_fossils = 0
                self.electricity_out = supplyOut.electricity_out

            elif self.has_cooling_demand:
                supply_director.set_builder(self.cooling_supply_system(load=self.energy_demand * (-1),
                                                                t_out=t_out,
                                                                heating_supply_temperature=self.heating_supply_temperature,
                                                                cooling_supply_temperature=self.cooling_supply_temperature,
                                                                has_heating_demand=self.has_heating_demand,
                                                                has_cooling_demand=self.has_cooling_demand))
                supplyOut = supply_director.calc_system()
                self.heating_demand = 0
                self.heating_sys_electricity = 0
                self.heating_sys_fossils = 0
                self.cooling_demand = self.energy_demand
                self.cooling_sys_electricity = supplyOut.electricity_in
                self.cooling_sys_fossils = supplyOut.fossils_in
                self.electricity_out = supplyOut.electricity_out

            self.cop = supplyOut.cop

        self.sys_total_energy = self.heating_sys_electricity + self.heating_sys_fossils + \
            self.cooling_sys_electricity + self.cooling_sys_fossils
        self.heating_energy = self.heating_sys_electricity + self.heating_sys_fossils
        self.cooling_energy = self.cooling_sys_electricity + self.cooling_sys_fossils

    # TODO: rename. this is expected to return a boolean. instead, it changes state??? you don't want to change state...
    # why not just return has_heating_demand and has_cooling_demand?? then call the function "check_demand"
    # has_heating_demand, has_cooling_demand = self.check_demand(...)
    def has_demand(self, internal_gains, solar_gains, t_out, t_m_prev):
        """
        Determines whether the building requires heating or cooling
        Used in: solve_building_energy()
        # step 1 in section C.4.2 in [C.3 ISO 13790]
        """
        # set energy demand to 0 and see if temperatures are within the comfort
        # range
        energy_demand = 0
        # Solve for the internal temperature t_Air
        self.calc_temperatures_crank_nicolson(
            energy_demand, internal_gains, solar_gains, t_out, t_m_prev)
        # If the air temperature is less or greater than the set temperature,
        # there is a heating/cooling load
        if self.t_air < self.t_set_heating:
            self.has_heating_demand = True
            self.has_cooling_demand = False
        elif self.t_air > self.t_set_cooling:
            self.has_cooling_demand = True
            self.has_heating_demand = False
        else:
            self.has_heating_demand = False
            self.has_cooling_demand = False

    def calc_temperatures_crank_nicolson(self, energy_demand, internal_gains, solar_gains, t_out, t_m_prev):
        """
        Determines node temperatures and computes derivation to determine the new node temperatures
        Used in: has_demand(), solve_building_energy(), calc_energy_demand()
        # section C.3 in [C.3 ISO 13790]
        """
        self.calc_heat_flow(t_out, internal_gains, solar_gains, energy_demand)

        self.calc_phi_m_tot(t_out)

        # calculates the new bulk temperature POINT from the old one
        self.calc_t_m_next(t_m_prev)

        # calculates the AVERAGE bulk temperature used for the remaining
        # calculation
        self.calc_t_m(t_m_prev)

        self.calc_t_s(t_out)

        self.calc_t_air(t_out)

        self.calc_t_opperative()
        return self.t_m, self.t_air, self.t_opperative

    def calc_energy_demand(self, internal_gains, solar_gains, t_out, t_m_prev):
        """
        Calculates the energy demand of the space if heating/cooling is active
        Used in: solve_building_energy()
        # Step 1 - Step 4 in Section C.4.2 in [C.3 ISO 13790]
        """

        # Step 1: Check if heating or cooling is needed
        #(Not needed, but doing so for readability when comparing with the standard)
        # Set heating/cooling to 0
        energy_demand_0 = 0
        # Calculate the air temperature with no heating/cooling
        t_air_0 = self.calc_temperatures_crank_nicolson(
            energy_demand_0, internal_gains, solar_gains, t_out, t_m_prev)[1]

        # Step 2: Calculate the unrestricted heating/cooling required

        # determine if we need heating or cooling based based on the condition
        # that no heating or cooling is required
        if self.has_heating_demand:
            t_air_set = self.t_set_heating
        elif self.has_cooling_demand:
            t_air_set = self.t_set_cooling
        else:
            raise NameError(
                'heating function has been called even though no heating is required')

        # Set a heating case where the heating load is 10x the floor area (10
        # W/m2)
        energy_floorAx10 = 10 * self.floor_area

        # Calculate the air temperature obtained by having this 10 W/m2
        # setpoint
        t_air_10 = self.calc_temperatures_crank_nicolson(
            energy_floorAx10, internal_gains, solar_gains, t_out, t_m_prev)[1]

        # Determine the unrestricted heating/cooling off the building
        self.calc_energy_demand_unrestricted(
            energy_floorAx10, t_air_set, t_air_0, t_air_10)

        # Step 3: Check if available heating or cooling power is sufficient
        if self.max_cooling_energy <= self.energy_demand_unrestricted <= self.max_heating_energy:

            self.energy_demand = self.energy_demand_unrestricted
            self.t_air_ac = t_air_set  # not sure what this is used for at this stage TODO

        # Step 4: if not sufficient then set the heating/cooling setting to the
        # maximum
        # necessary heating power exceeds maximum available power
        elif self.energy_demand_unrestricted > self.max_heating_energy:
            self.energy_demand = self.max_heating_energy

        # necessary cooling power exceeds maximum available power
        elif self.energy_demand_unrestricted < self.max_cooling_energy:

            self.energy_demand = self.max_cooling_energy

        else:
            self.energy_demand = 0
            raise ValueError('unknown radiative heating/cooling system status')

        # calculate system temperatures for Step 3/Step 4
        self.calc_temperatures_crank_nicolson(
            self.energy_demand, internal_gains, solar_gains, t_out, t_m_prev)

    def calc_energy_demand_unrestricted(self, energy_floorAx10, t_air_set, t_air_0, t_air_10):
        """
        Calculates the energy demand of the system if it has no maximum output restrictions
        # (C.13) in [C.3 ISO 13790]
        Based on the Thales Intercept Theorem.
        Where we set a heating case that is 10x the floor area and determine the temperature as a result
        Assuming that the relation is linear, one can draw a right angle triangle.
        From this we can determine the heating level required to achieve the set point temperature
        This assumes a perfect HVAC control system
        """
        self.energy_demand_unrestricted = energy_floorAx10 * \
            (t_air_set - t_air_0) / (t_air_10 - t_air_0)

    def calc_heat_flow(self, t_out, internal_gains, solar_gains, energy_demand):
        """
        Calculates the heat flow from the solar gains, heating/cooling system, and internal gains into the building
        The input of the building is split into the air node, surface node, and thermal mass node based on
        on the following equations
        #C.1 - C.3 in [C.3 ISO 13790]
        Note that this equation has diverged slightly from the standard
        as the heating/cooling node can enter any node depending on the
        emission system selected
        """

        # Calculates the heat flows to various points of the building based on the breakdown in section C.2, formulas C.1-C.3
        # Heat flow to the air node
        self.phi_ia = 0.5 * internal_gains
        # Heat flow to the surface node
        self.phi_st = (1 - (self.mass_area / self.A_t) - (self.h_tr_w /
                            (9.1 * self.A_t))) * (0.5 * internal_gains + solar_gains)
        # Heatflow to the thermal mass node
        self.phi_m = (self.mass_area / self.A_t) * \
            (0.5 * internal_gains + solar_gains)

        # We call the EmissionDirector to modify these flows depending on the
        # system and the energy demand
        emDirector = sc.sticky["EmissionDirector"]()
        # Set the emission system to the type specified by the user

        emDirector.set_builder(self.heating_emission_system(
            energy_demand=energy_demand))
        # Calculate the new flows to each node based on the heating system
        flows = emDirector.calc_flows()

        # Set modified flows to building object
        self.phi_ia += flows.phi_ia_plus
        self.phi_st += flows.phi_st_plus
        self.phi_m += flows.phi_m_plus

        # Set supply temperature to building object
        # TODO: This currently is constant for all emission systems, to be
        # modified in the future
        self.heating_supply_temperature = flows.heating_supply_temperature
        self.cooling_supply_temperature = flows.cooling_supply_temperature

    def calc_t_m_next(self, t_m_prev):
        """
        Primary Equation, calculates the temperature of the next time step
        # (C.4) in [C.3 ISO 13790]
        """

        self.t_m_next = ((t_m_prev * ((self.c_m / 3600.0) - 0.5 * (self.h_tr_3 + self.h_tr_em))) +
                         self.phi_m_tot) / ((self.c_m / 3600.0) + 0.5 * (self.h_tr_3 + self.h_tr_em))

    def calc_phi_m_tot(self, t_out):
        """
        Calculates a global heat transfer. This is a definition used to simplify equation
        calc_t_m_next so it's not so long to write out
        # (C.5) in [C.3 ISO 13790]
        # h_ve = h_ve_adj and t_supply = t_out [9.3.2 ISO 13790]
        """

        t_supply = t_out  # ASSUMPTION: Supply air comes straight from the outside air
        h_tr_3 = False
        for key in self.__dict__:
            if 'h_tr_3' in key:
                h_tr_3 = True
        if not h_tr_3: # This is the case when heating is supplied and crank-nicholson is called directly
            self.calc_h_tr_3()


        self.phi_m_tot = self.phi_m + self.h_tr_em * t_out + \
            self.h_tr_3 * (self.phi_st + self.h_tr_w * t_out + self.h_tr_1 *
                           ((self.phi_ia / self.h_ve_adj) + t_supply)) / self.h_tr_2

    def calc_h_tr_1(self):
        """
        Definition to simplify calc_phi_m_tot
        # (C.6) in [C.3 ISO 13790]
        """
        self.h_tr_1 = 1.0 / (1.0 / self.h_ve_adj + 1.0 / self.h_tr_is)

    def calc_h_tr_2(self):
        """
        Definition to simplify calc_phi_m_tot
        # (C.7) in [C.3 ISO 13790]
        """
        h_tr_1 = False
        for key in self.__dict__:
            if 'h_tr_1' in key:
                h_tr_1 = True
        if not h_tr_1:
            self.calc_h_tr_1 ()

        self.h_tr_2 = self.h_tr_1 + self.h_tr_w

    def calc_h_tr_3(self):
        """
        Definition to simplify calc_phi_m_tot
        # (C.8) in [C.3 ISO 13790]
        """
        h_tr_2 = False
        for key in self.__dict__:
            if 'h_tr_2' in key:
                h_tr_2 = True
        if not h_tr_2:
            self.calc_h_tr_2()

        self.h_tr_3 = 1.0 / (1.0 / self.h_tr_2 + 1.0 / self.h_tr_ms)

    def calc_t_m(self, t_m_prev):
        """
        Temperature used for the calculations, average between newly calculated and previous bulk temperature
        # (C.9) in [C.3 ISO 13790]
        """
        self.t_m = (self.t_m_next + t_m_prev) / 2.0
    def calc_t_s(self, t_out):
        """
        Calculate the temperature of the inside room surfaces
        # (C.10) in [C.3 ISO 13790]
        # h_ve = h_ve_adj and t_supply = t_out [9.3.2 ISO 13790]
        """

        t_supply = t_out  # ASSUMPTION: Supply air comes straight from the outside air

        self.t_s = (self.h_tr_ms * self.t_m + self.phi_st + self.h_tr_w * t_out + self.h_tr_1 * \
            (t_supply + self.phi_ia / self.h_ve_adj)) / \
            (self.h_tr_ms + self.h_tr_w + self.h_tr_1)

    def calc_t_air(self, t_out):
        """
        Calculate the temperature of the air node
        # (C.11) in [C.3 ISO 13790]
        # h_ve = h_ve_adj and t_supply = t_out [9.3.2 ISO 13790]
        """

        t_supply = t_out

        # Calculate the temperature of the inside air
        self.t_air = (self.h_tr_is * self.t_s + self.h_ve_adj *
                      t_supply + self.phi_ia) / (self.h_tr_is + self.h_ve_adj)

    def calc_t_opperative(self):
        """
        The opperative temperature is a weighted average of the air and mean radiant temperatures.
        It is not used in any further calculation at this stage
        # (C.12) in [C.3 ISO 13790]
        """
        
        self.t_opperative = 0.3 * self.t_air + 0.7 * self.t_s


class Building(object):
    """
    The original code as found in RC_BuildingSimulator
    """

    def __init__(self,
                 window_area=13.5,
                 external_envelope_area=15.19,
                 room_depth=7,
                 room_width=4.9,
                 room_height=3.1,
                 lighting_load=11.7,
                 lighting_control=300,
                 lighting_utilisation_factor=0.45,
                 lighting_maintenance_factor=0.9,
                 u_walls=0.2,
                 u_windows=1.1,
                 g_windows=0.6,
                 ach_vent=1.5,
                 ach_infl=0.5,
                 ventilation_efficiency=0,
                 thermal_capacitance_per_floor_area=165000,
                 t_set_heating=20,
                 t_set_cooling=26,
                 max_cooling_energy_per_floor_area=-12,
                 max_heating_energy_per_floor_area=12,
                 heating_supply_system=sc.sticky["DirectHeater"],
                 cooling_supply_system=sc.sticky["DirectCooler"],
                 heating_emission_system=sc.sticky["AirConditioning"],
                 cooling_emission_system=sc.sticky["AirConditioning"],
                 ):

        # Building Dimensions
        self.window_area = window_area  # [m2] Window Area
        self.room_depth = room_depth  # [m] Room Depth
        self.room_width = room_width  # [m] Room Width
        self.room_height = room_height  # [m] Room Height

        # Fenestration and Lighting Properties
        self.g_windows = g_windows
        self.lighting_load = lighting_load  # [kW/m2] lighting load
        self.lighting_control = lighting_control  # [lux] Lighting set point
        # How the light entering the window is transmitted to the working plane
        self.lighting_utilisation_factor = lighting_utilisation_factor
        # How dirty the window is. Section 2.2.3.1 Environmental Science
        # Handbook
        self.lighting_maintenance_factor = lighting_maintenance_factor

        # Calculated Properties
        self.floor_area = room_depth * room_width  # [m2] Floor Area
        # [m2] Effective Mass Area assuming a medium weight building #12.3.1.2
        self.mass_area = self.floor_area * 2.5
        self.room_vol = room_width * room_depth * \
            room_height  # [m3] Room Volume
        self.total_internal_area = self.floor_area * 2 + \
            room_width * room_height * 2 + room_depth * room_height * 2
        # TODO: Standard doesn't explain what A_t is. Needs to be checked
        self.A_t = self.total_internal_area

        # Single Capacitance  5 conductance Model Parameters
        # [kWh/K] Room Capacitance. Default based on ISO standard 12.3.1.2 for medium heavy buildings
        self.c_m = thermal_capacitance_per_floor_area * self.floor_area
        # Conductance of opaque surfaces to exterior [W/K]
        self.h_tr_em = u_walls * (external_envelope_area - window_area)
        # Conductance to exterior through glazed surfaces [W/K], based on
        # U-wert of 1W/m2K
        self.h_tr_w = u_windows * window_area

        # Determine the ventilation conductance
        ach_tot = ach_infl + ach_vent  # Total Air Changes Per Hour
        # temperature adjustment factor taking ventilation and infiltration
        # [ISO: E -27]
        b_ek = (1 - (ach_vent / (ach_tot)) * ventilation_efficiency)
        self.h_ve_adj = 1200 * b_ek * self.room_vol * \
            (ach_tot / 3600)  # Conductance through ventilation [W/M]
        # transmittance from the internal air to the thermal mass of the
        # building
        self.h_tr_ms = 9.1 * self.mass_area
        # Conductance from the conditioned air to interior building surface
        self.h_tr_is = self.total_internal_area * 3.45

        # Thermal set points
        self.t_set_heating = t_set_heating
        self.t_set_cooling = t_set_cooling

        # Thermal Properties
        self.has_heating_demand = False  # Boolean for if heating is required
        self.has_cooling_demand = False  # Boolean for if cooling is required
        self.max_cooling_energy = max_cooling_energy_per_floor_area * \
            self.floor_area  # max cooling load (W/m2)
        self.max_heating_energy = max_heating_energy_per_floor_area * \
            self.floor_area  # max heating load (W/m2)

        # Building System Properties
        self.heating_supply_system = heating_supply_system
        self.cooling_supply_system = cooling_supply_system
        self.heating_emission_system = heating_emission_system
        self.cooling_emission_system = cooling_emission_system

    def solve_building_lighting(self, illuminance, occupancy):
        """
        Calculates the lighting demand for a set timestep

        :param illuminance: Illuminance transmitted through the window [Lumens]
        :type illuminance: float
        :param occupancy: Probability of full occupancy
        :type occupancy: float

        :return: self.lighting_demand, Lighting Energy Required for the timestep
        :rtype: float

        """
        # Cite: Environmental Science Handbook, SV Szokolay, Section 2.2.1.3
        # also, this might be sped up by pre-calculating the constants, but idk. first check with profiler...
        lux = (illuminance * self.lighting_utilisation_factor *
               self.lighting_maintenance_factor) / self.floor_area  # [Lux]

        if lux < self.lighting_control and occupancy > 0:
            # Lighting demand for the hour
            self.lighting_demand = self.lighting_load * self.floor_area
        else:
            self.lighting_demand = 0

    def solve_building_energy(self, internal_gains, solar_gains, t_out, t_m_prev):
        """
        Calculates the heating and cooling consumption of a building for a set timestep

        :param internal_gains: internal heat gains from people and appliances [W]
        :type internal_gains: float
        :param solar_gains: solar heat gains [W]
        :type solar_gains: float
        :param t_out: Outdoor air temperature [C]
        :type t_out: float
        :param t_m_prev: Previous air temperature [C]
        :type t_m_prev: float

        :return: self.heating_demand, space heating demand of the building
        :return: self.heating_sys_electricity, heating electricity consumption
        :return: self.heating_sys_fossils, heating fossil fuel consumption 
        :return: self.cooling_demand, space cooling demand of the building
        :return: self.cooling_sys_electricity, electricity consumption from cooling
        :return: self.cooling_sys_fossils, fossil fuel consumption from cooling
        :return: self.electricity_out, electricity produced from combined heat pump systems
        :return: self.sys_total_energy, total exergy consumed (electricity + fossils) for heating and cooling
        :return: self.heating_energy, total exergy consumed (electricity + fossils) for heating 
        :return: self.cooling_energy, total exergy consumed (electricity + fossils) for cooling
        :return: self.cop, Coefficient of Performance of the heating or cooling system
        :rtype: float

        """
        # Main File

        # Calculate the heat transfer definitions for formula simplification
        self.calc_h_tr_1()
        self.calc_h_tr_2()
        self.calc_h_tr_3()

        # check demand, and change state of self.has_heating_demand, and self._has_cooling_demand
        self.has_demand(internal_gains, solar_gains, t_out, t_m_prev)

        if not self.has_heating_demand and not self.has_cooling_demand:

            # no heating or cooling demand
            # calculate temperatures of building R-C-model and exit
            # --> rc_model_function_1(...)
            self.energy_demand = 0

            # y u no pep8 bra?
            self.heating_demand = 0  # Energy required by the zone
            self.cooling_demand = 0  # Energy surplus of the zone
            # Energy (in electricity) required by the supply system to provide
            # HeatingDemand
            self.heating_sys_electricity = 0
            # Energy (in fossil fuel) required by the supply system to provide
            # HeatingDemand
            self.heating_sys_fossils = 0
            # Energy (in electricity) required by the supply system to get rid
            # of CoolingDemand
            self.cooling_sys_electricity = 0
            # Energy (in fossil fuel) required by the supply system to get rid
            # of CoolingDemand
            self.cooling_sys_fossils = 0
            # Electricity produced by the supply system (e.g. CHP)
            self.electricity_out = 0

        else:

            # has heating/cooling demand

            # Calculates energy_demand used below
            self.calc_energy_demand(
                internal_gains, solar_gains, t_out, t_m_prev)

            self.calc_temperatures_crank_nicolson(
                self.energy_demand, internal_gains, solar_gains, t_out, t_m_prev)
            # calculates the actual t_m resulting from the actual heating
            # demand (energy_demand)

            # Calculate the Heating/Cooling Input Energy Required

            supply_director = sc.sticky["SupplyDirector"]()  # Initialise Heating System Manager

            if self.has_heating_demand:
                supply_director.set_builder(self.heating_supply_system(load=self.energy_demand, 
                                                                t_out=t_out, 
                                                                heating_supply_temperature=self.heating_supply_temperature,
                                                                cooling_supply_temperature=self.cooling_supply_temperature, 
                                                                has_heating_demand=self.has_heating_demand, 
                                                                has_cooling_demand=self.has_cooling_demand))
                supplyOut = supply_director.calc_system()
                # All Variables explained underneath line 467
                self.heating_demand = self.energy_demand
                self.heating_sys_electricity = supplyOut.electricity_in
                self.heating_sys_fossils = supplyOut.fossils_in
                self.cooling_demand = 0
                self.cooling_sys_electricity = 0
                self.cooling_sys_fossils = 0
                self.electricity_out = supplyOut.electricity_out

            elif self.has_cooling_demand:
                supply_director.set_builder(self.cooling_supply_system(load=self.energy_demand * (-1), 
                                                                t_out=t_out, 
                                                                heating_supply_temperature=self.heating_supply_temperature,
                                                                cooling_supply_temperature=self.cooling_supply_temperature, 
                                                                has_heating_demand=self.has_heating_demand, 
                                                                has_cooling_demand=self.has_cooling_demand))
                supplyOut = supply_director.calc_system()
                self.heating_demand = 0
                self.heating_sys_electricity = 0
                self.heating_sys_fossils = 0
                self.cooling_demand = self.energy_demand
                self.cooling_sys_electricity = supplyOut.electricity_in
                self.cooling_sys_fossils = supplyOut.fossils_in
                self.electricity_out = supplyOut.electricity_out

            self.cop = supplyOut.cop

        self.sys_total_energy = self.heating_sys_electricity + self.heating_sys_fossils + \
            self.cooling_sys_electricity + self.cooling_sys_fossils
        self.heating_energy = self.heating_sys_electricity + self.heating_sys_fossils
        self.cooling_energy = self.cooling_sys_electricity + self.cooling_sys_fossils

    # TODO: rename. this is expected to return a boolean. instead, it changes state??? you don't want to change state...
    # why not just return has_heating_demand and has_cooling_demand?? then call the function "check_demand"
    # has_heating_demand, has_cooling_demand = self.check_demand(...)
    def has_demand(self, internal_gains, solar_gains, t_out, t_m_prev):
        """
        Determines whether the building requires heating or cooling
        Used in: solve_building_energy()

        # step 1 in section C.4.2 in [C.3 ISO 13790]
        """

        # set energy demand to 0 and see if temperatures are within the comfort
        # range
        energy_demand = 0
        # Solve for the internal temperature t_Air
        self.calc_temperatures_crank_nicolson(
            energy_demand, internal_gains, solar_gains, t_out, t_m_prev)

        # If the air temperature is less or greater than the set temperature,
        # there is a heating/cooling load
        if self.t_air < self.t_set_heating:
            self.has_heating_demand = True
            self.has_cooling_demand = False
        elif self.t_air > self.t_set_cooling:
            self.has_cooling_demand = True
            self.has_heating_demand = False
        else:
            self.has_heating_demand = False
            self.has_cooling_demand = False

    def calc_temperatures_crank_nicolson(self, energy_demand, internal_gains, solar_gains, t_out, t_m_prev):
        """
        Determines node temperatures and computes derivation to determine the new node temperatures
        Used in: has_demand(), solve_building_energy(), calc_energy_demand()
        # section C.3 in [C.3 ISO 13790]
        """

        self.calc_heat_flow(t_out, internal_gains, solar_gains, energy_demand)

        self.calc_phi_m_tot(t_out)

        # calculates the new bulk temperature POINT from the old one
        self.calc_t_m_next(t_m_prev)

        # calculates the AVERAGE bulk temperature used for the remaining
        # calculation
        self.calc_t_m(t_m_prev)

        self.calc_t_s(t_out)

        self.calc_t_air(t_out)

        self.calc_t_opperative()

        return self.t_m, self.t_air, self.t_opperative

    def calc_energy_demand(self, internal_gains, solar_gains, t_out, t_m_prev):
        """
        Calculates the energy demand of the space if heating/cooling is active
        Used in: solve_building_energy()
        # Step 1 - Step 4 in Section C.4.2 in [C.3 ISO 13790]
        """

        # Step 1: Check if heating or cooling is needed 
        #(Not needed, but doing so for readability when comparing with the standard)
        # Set heating/cooling to 0
        energy_demand_0 = 0
        # Calculate the air temperature with no heating/cooling
        t_air_0 = self.calc_temperatures_crank_nicolson(
            energy_demand_0, internal_gains, solar_gains, t_out, t_m_prev)[1]

        # Step 2: Calculate the unrestricted heating/cooling required

        # determine if we need heating or cooling based based on the condition
        # that no heating or cooling is required
        if self.has_heating_demand:
            t_air_set = self.t_set_heating
        elif self.has_cooling_demand:
            t_air_set = self.t_set_cooling
        else:
            raise NameError(
                'heating function has been called even though no heating is required')

        # Set a heating case where the heating load is 10x the floor area (10
        # W/m2)
        energy_floorAx10 = 10 * self.floor_area

        # Calculate the air temperature obtained by having this 10 W/m2
        # setpoint
        t_air_10 = self.calc_temperatures_crank_nicolson(
            energy_floorAx10, internal_gains, solar_gains, t_out, t_m_prev)[1]

        # Determine the unrestricted heating/cooling off the building
        self.calc_energy_demand_unrestricted(
            energy_floorAx10, t_air_set, t_air_0, t_air_10)

        # Step 3: Check if available heating or cooling power is sufficient
        if self.max_cooling_energy <= self.energy_demand_unrestricted <= self.max_heating_energy:

            self.energy_demand = self.energy_demand_unrestricted
            self.t_air_ac = t_air_set  # not sure what this is used for at this stage TODO

        # Step 4: if not sufficient then set the heating/cooling setting to the
        # maximum
        # necessary heating power exceeds maximum available power
        elif self.energy_demand_unrestricted > self.max_heating_energy:

            self.energy_demand = self.max_heating_energy

        # necessary cooling power exceeds maximum available power
        elif self.energy_demand_unrestricted < self.max_cooling_energy:

            self.energy_demand = self.max_cooling_energy

        else:
            self.energy_demand = 0
            raise ValueError('unknown radiative heating/cooling system status')

        # calculate system temperatures for Step 3/Step 4
        self.calc_temperatures_crank_nicolson(
            self.energy_demand, internal_gains, solar_gains, t_out, t_m_prev)

    def calc_energy_demand_unrestricted(self, energy_floorAx10, t_air_set, t_air_0, t_air_10):
        """
        Calculates the energy demand of the system if it has no maximum output restrictions
        # (C.13) in [C.3 ISO 13790]


        Based on the Thales Intercept Theorem. 
        Where we set a heating case that is 10x the floor area and determine the temperature as a result 
        Assuming that the relation is linear, one can draw a right angle triangle. 
        From this we can determine the heating level required to achieve the set point temperature
        This assumes a perfect HVAC control system
        """
        self.energy_demand_unrestricted = energy_floorAx10 * \
            (t_air_set - t_air_0) / (t_air_10 - t_air_0)

    def calc_heat_flow(self, t_out, internal_gains, solar_gains, energy_demand):
        """
        Calculates the heat flow from the solar gains, heating/cooling system, and internal gains into the building

        The input of the building is split into the air node, surface node, and thermal mass node based on
        on the following equations

        #C.1 - C.3 in [C.3 ISO 13790]

        Note that this equation has diverged slightly from the standard 
        as the heating/cooling node can enter any node depending on the
        emission system selected

        """

        # Calculates the heat flows to various points of the building based on the breakdown in section C.2, formulas C.1-C.3
        # Heat flow to the air node
        self.phi_ia = 0.5 * internal_gains
        # Heat flow to the surface node
        self.phi_st = (1 - (self.mass_area / self.A_t) - (self.h_tr_w /
                            (9.1 * self.A_t))) * (0.5 * internal_gains + solar_gains)
        # Heatflow to the thermal mass node
        self.phi_m = (self.mass_area / self.A_t) * \
            (0.5 * internal_gains + solar_gains)

        # We call the EmissionDirector to modify these flows depending on the
        # system and the energy demand
        emDirector = sc.sticky["EmissionDirector"]()
        # Set the emission system to the type specified by the user
        emDirector.set_builder(self.heating_emission_system(
            energy_demand=energy_demand))
        # Calculate the new flows to each node based on the heating system
        flows = emDirector.calc_flows()

        # Set modified flows to building object
        self.phi_ia += flows.phi_ia_plus
        self.phi_st += flows.phi_st_plus
        self.phi_m += flows.phi_m_plus

        # Set supply temperature to building object
        # TODO: This currently is constant for all emission systems, to be
        # modified in the future
        self.heating_supply_temperature = flows.heating_supply_temperature
        self.cooling_supply_temperature = flows.cooling_supply_temperature

    def calc_t_m_next(self, t_m_prev):
        """
        Primary Equation, calculates the temperature of the next time step
        # (C.4) in [C.3 ISO 13790]
        """

        self.t_m_next = ((t_m_prev * ((self.c_m / 3600.0) - 0.5 * (self.h_tr_3 + self.h_tr_em))) +
                         self.phi_m_tot) / ((self.c_m / 3600.0) + 0.5 * (self.h_tr_3 + self.h_tr_em))

    def calc_phi_m_tot(self, t_out):
        """
        Calculates a global heat transfer. This is a definition used to simplify equation
        calc_t_m_next so it's not so long to write out
        # (C.5) in [C.3 ISO 13790]
        # h_ve = h_ve_adj and t_supply = t_out [9.3.2 ISO 13790]
        """

        t_supply = t_out  # ASSUMPTION: Supply air comes straight from the outside air

        self.phi_m_tot = self.phi_m + self.h_tr_em * t_out + \
            self.h_tr_3 * (self.phi_st + self.h_tr_w * t_out + self.h_tr_1 *
                           ((self.phi_ia / self.h_ve_adj) + t_supply)) / self.h_tr_2

    def calc_h_tr_1(self):
        """
        Definition to simplify calc_phi_m_tot
        # (C.6) in [C.3 ISO 13790]

        """
        self.h_tr_1 = 1.0 / (1.0 / self.h_ve_adj + 1.0 / self.h_tr_is)

    def calc_h_tr_2(self):
        """
        Definition to simplify calc_phi_m_tot
        # (C.7) in [C.3 ISO 13790]

        """

        self.h_tr_2 = self.h_tr_1 + self.h_tr_w

    def calc_h_tr_3(self):
        """
        Definition to simplify calc_phi_m_tot
        # (C.8) in [C.3 ISO 13790]

        """

        self.h_tr_3 = 1.0 / (1.0 / self.h_tr_2 + 1.0 / self.h_tr_ms)


    def calc_t_m(self, t_m_prev):
        """
        Temperature used for the calculations, average between newly calculated and previous bulk temperature
        # (C.9) in [C.3 ISO 13790]
        """
        self.t_m = (self.t_m_next + t_m_prev) / 2.0

    def calc_t_s(self, t_out):
        """
        Calculate the temperature of the inside room surfaces
        # (C.10) in [C.3 ISO 13790]
        # h_ve = h_ve_adj and t_supply = t_out [9.3.2 ISO 13790]
        """

        t_supply = t_out  # ASSUMPTION: Supply air comes straight from the outside air

        self.t_s = (self.h_tr_ms * self.t_m + self.phi_st + self.h_tr_w * t_out + self.h_tr_1 * \
            (t_supply + self.phi_ia / self.h_ve_adj)) / \
            (self.h_tr_ms + self.h_tr_w + self.h_tr_1)

    def calc_t_air(self, t_out):
        """
        Calculate the temperature of the air node
        # (C.11) in [C.3 ISO 13790]
        # h_ve = h_ve_adj and t_supply = t_out [9.3.2 ISO 13790]
        """

        t_supply = t_out

        # Calculate the temperature of the inside air
        self.t_air = (self.h_tr_is * self.t_s + self.h_ve_adj *
                      t_supply + self.phi_ia) / (self.h_tr_is + self.h_ve_adj)

    def calc_t_opperative(self):
        """
        The opperative temperature is a weighted average of the air and mean radiant temperatures. 
        It is not used in any further calculation at this stage
        # (C.12) in [C.3 ISO 13790]
        """

        self.t_opperative = 0.3 * self.t_air + 0.7 * self.t_s

error = "Connect emissions_systems and supply_systems components!"
e = ghKernel.GH_RuntimeMessageLevel.Error

if systems == [] or len(systems) != 2:
    ghenv.Component.AddRuntimeMessage(e, error)

if any(['Emission' in s for s in systems]) and any(['Supply' in s for s in systems]):
    sc.sticky["RelativeSun"] = RelativeSun
    sc.sticky["WindowRadiation"] = WindowRadiation
    sc.sticky["Element"] = Element
    sc.sticky["ElementBuilder"] = ElementBuilder
    sc.sticky["ThermalBridge"] = ThermalBridge
    sc.sticky["ThermalZone"] = ThermalZone
    sc.sticky["RCModel"] = ElementBuilding
    sc.sticky["RCModelClassic"] = Building
    sc.sticky["HivePreparation"] = HivePreparation
    sc.sticky['pushback']()
    print 'Modular Building Physics is go!'