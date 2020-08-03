"""
Implementation of Joan's code using clipper
"""
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

import System.Reflection
import os 

clipper_path = os.getcwd() + '\clipper_library.dll'
clipper = System.Reflection.Assembly.LoadFrom(clipper_path)
clp = clipper.ClipperLib.Clipper
IntPoint = clipper.ClipperLib.IntPoint


from System.Collections.Generic import List


class SunPath(object):
    def __init__(self,location,azimuth_tilt_rad,altitude_tilt_rad):
        # Extract location data
        self.latitude_deg = float(location[0])
        self.longitude_deg = float(location[1])
        self.utc_offset = float(location[2])
        self.year = int(location[3])
        
        self.azimuth_tilt_rad = azimuth_tilt_rad
        self.altitude_tilt_rad = altitude_tilt_rad
    
    def is_sunny(sun_alt, relative_sun_az):
        return sun_alt >= 0.0 and abs(asf_sun_az) < 90.0
    
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
        relative_sun_az = self.calc_relative_position(sun_az)
        
        return sun_alt, relative_sun_az
    
    def calc_sun_position(self, hoy):
        """
        Credits: Prageeth Jayathissa, Joan Domènech Masferrer 
    
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
    
    def calc_relative_position(self, sun_az):
        """
        Calculates the sun position relative to the facade orientation.
        :param sun_az: Sun azimuth [Degrees] (Traditional convention)
        :type sun_az: float
        :param asf_normal_az: Azimuth position of normal to ASF plane  [Degrees] (Traditional convention)
        :type asf_normal_az: int
        :return: asf_sun_az: Sun azimuth [Degrees] (Facade convention)
        :rtype: tuple
        """
        relative_sun_az = sun_az - math.degrees(self.azimuth_tilt_rad)
        if relative_sun_az < - 180:
            relative_sun_az = 360 - abs(relative_sun_az)
        return relative_sun_az


class ShadedWindow(object):
    """
    Adapted from shaded_window.py from the ASF_Control repository by Joan DM
    
    returns window and self shading for an arbitrary set of shading geometry.
    """

    def __init__(self, window_geometry, context_geometry,
                 glass_solar_transmittance=0.7, glass_light_transmittance=0.8):
        
        self.window_geometry = window_geometry
        
        # initialize window centroid, vertices, normal and tilt
        self.extract_window_geometry()
        
        #TODO: Explode polysurfaces into surfaces
        self.transform_context_geometry(context_geometry)
        
        self.glass_solar_transmittance = glass_solar_transmittance
        self.glass_light_transmittance = glass_light_transmittance
    
    def extract_window_geometry(self):
        """
        This function creates the following attributes during __init__:
        self.window_centroid
        self.window_normal
        self.window_vertices
        self.altitude_tilt_rad
        self.azimuth_tilt_rad
        """
        north = rc.Geometry.Vector3d(0,1,0)
        vertical = rc.Geometry.Vector3d(0,0,1)
        faces_west = False
        self.window_centroid = rs.SurfaceAreaCentroid(self.window_geometry)[0]
        self.window_area = rs.Area(self.window_geometry)
        normal = rs.SurfaceNormal(self.window_geometry,[0.5,0.5])
        
        self.window_normal = normal
        self.window_vertices = rs.SurfaceEditPoints(self.window_geometry)
        
        if normal[0] < 0:
            faces_west = True
        else:
            faces_west = False
        
        normal_xy = rc.Geometry.Vector3d(normal[0],normal[1],0)
        normal_xz = rc.Geometry.Vector3d(normal[0],0,normal[2])
        
        try:
            azimuth = rs.VectorAngle(north,normal_xy)
            if faces_west:
                azimuth = 360-azimuth
        except ValueError:
            azimuth = 0
        
        try: 
            altitude = rs.VectorAngle(vertical,normal_xz)
        except ValueError:
            altitude = 0
        
        self.altititude_tilt_rad = math.radians(altitude)
        self.azimuth_tilt_rad = math.radians(azimuth)
    
    def orient_point_2d_window(self,point):
        window_plane = rs.SurfaceFrame(self.window_geometry,[0.5,0.5])
        x,y = rs.PlaneClosestPoint(window_plane,point,return_point=False)
        z = rs.DistanceToPlane(window_plane,point)
        return rc.Geometry.Point3d(x,y,z)
    
    def surface_to_clockwise_points(self,geometry):
        """
        :return sorted_points: list of points sorted clockwise
        :rtype sorted_points: list
        """
        points = rs.SurfacePoints(geometry,True)
        centroid, error= rs.SurfaceAreaCentroid(geometry)
        normal = rs.SurfaceNormal(geometry,[0.5,0.5])

        # Generate plane opposing the dominant direction of the surface
        if normal[0] == 0 and normal[2] == 0:
            if normal[1]>0:
                plane = rc.Geometry.Plane(centroid,rc.Geometry.Vector3d(0,-1,0))
            else:
                plane = rc.Geometry.Plane(centroid,rc.Geometry.Vector3d(0,1,0))
        
        if normal[1] == 0 and normal[2] == 0:
            if normal[0]>0:
                plane = rc.Geometry.Plane(centroid,rc.Geometry.Vector3d(-1,0,0))
            else:
                plane = rc.Geometry.Plane(centroid,rc.Geometry.Vector3d(1,0,0))
        
        if normal[2] != 0:
            plane = rc.Geometry.Plane(centroid,rc.Geometry.Vector3d(0,0,-1))
       
        # calculate vectors from center to points
        vecs = [centroid - point for point in points]
        
        # calculate angles between vectors and plane x-axis
        angles = [rc.Geometry.Vector3d.VectorAngle(plane.XAxis,v,plane) for v in vecs]
        sorted_points = [p[1] for p in sorted(zip(angles,points))] 
        return centroid, sorted_points
    
    def transform_shade(self,shade):
        """
        Return a list of edge points for a shading surface
        """
        centroid, shade_points = self.surface_to_clockwise_points(shade)
        
        transformed_points = []
        for p in shade_points:
            transformed_points.append(self.orient_point_2d_window(p))
        
        return self.orient_point_2d_window(centroid), transformed_points
    
    def transform_context_geometry(self,context_geometry):
        """
        Transform context and arrange in any detectable array form
        Return a dictionary with
        key: shade centroid
        value: transformed points for each shade
        """
        context = []
        centroids = []
        
        for xg in context_geometry:
            c,p = self.transform_shade(xg) 
            context.append(p)
            centroids.append(c)
        
        def round2(x):
            return int(x*100)/100
        
        unique_x = sorted(list(set([round2(x[0]) for x in centroids])))
        unique_y = sorted(list(set([round2(x[1]) for x in centroids])))
        
        # create indices for each unique value of x and y
        x_dict = {v:k for k,v in enumerate(unique_x)}
        y_dict = {v:k for k,v in enumerate(unique_y)}
        
        self.nrows = len(x_dict)
        self.ncols = len(y_dict)
        
        # create dictionary
        self.context = {}
        for c,x in zip(centroids,context):
            self.context[x_dict[round2(c[0])],y_dict[round2(c[1])]] = x
    
    
    def sort_right_to_left(self,centroid_dict):
        """
        see sort_points_left. In this case, the points are sorted from bottom 
        to top and right to left.
        """
        flipped_centroids = []
        for i,c in centroid_dict.iteritems():
            flipped_centroids.append([-c[0],c[1],i])
        
        flipped_and_sorted_centroids = sorted(flipped_centroids,key=itemgetter(0,1))
        
        sorted_points = []
        for fsc in flipped_and_sorted_centroids:
            sorted_points.append([round(-fsc[0],2),round(fsc[1],2),fsc[2]])
        
        return sorted_points
    
    def sort_left_to_right(self,centroid_dict):
        """
        Sort a set of 3d points on a horizontal plane
        
        This method is used in extract_self_shading. Shadows need to be 
        overlayed with the shadow furthest from the light source at the bottom. 
        The points are sorted from bottom to top and from left to right.

        :param points: points
        :type shades: list
        """
        unsorted_points = centroid_dict.values()
        
        p_dict = {} # {x0:[[y0,i0],[y1,i1],..],x1:...}
        unique_x = sorted(list(set([round(x[0],2) for x in unsorted_points])))
        
        # ugly manual sort
        for x in unique_x:
            this_x = []
            for i,up in zip(centroid_dict.keys(),unsorted_points):
                if round(up[0],2) == x:
                    this_x.append([up[1],i])
            p_dict[x] = sorted(this_x,key=itemgetter(0))
        
        # flatten p_dict
        index_list = []
        for x in unique_x:
            for y in p_dict[x]:
                index_list.append(y[1])
        
        sorted_points = []
        for i in index_list:
            sorted_points.append([centroid_dict[i][0],centroid_dict[i][1],i])
        
        return sorted_points
    
    
    def calc_shadow(self, shade, sun_alt, relative_sun_az):
        """
        Coordinate system:
        X = Horizontal direction, parallel to glazing surface
        Y = Vertical direction, parallel to glazing surface
        Z = Perpendicular in respect of glazing surface
    
        """
    
        # Align relative_sun_az with window plane, convert angles to radians
        # Note: this procedure is only done when is sunny, hence relative_sun_az in range[-90, 90];
        relative_sun_az_rad, sun_alt_rad = map(math.radians, [relative_sun_az + 90, sun_alt]);
    
        # Create sun vector
        sunvec_x = math.cos(relative_sun_az_rad) * math.cos(sun_alt_rad);
        sunvec_y = math.sin(sun_alt_rad);
        sunvec_z = math.sin(relative_sun_az_rad) * math.cos(sun_alt_rad);
    
        # Initialize shadow, which represents the projection of the shade panel
        shadow = {}
    
        # Calculate the projection on a 2-D surface (x,y)
        for index, point_3d in enumerate(shade):
            
            # Define t paramater from parametric equation. Window plane is placed z=0 
            t = - point_3d[2] / sunvec_z;
    
            # Project points using sun vector
            x_point = point_3d[0] + sunvec_x * t;
            y_point = point_3d[1] + sunvec_y * t;
            
            # print dir(clipper.ClipperLib.Clipper)
            shadow[index] = [x_point, y_point];
    
        return shadow
    
    def calc_gross_shadows(self,sun_alt,relative_sun_az):
        """
        projects all shading objects onto window coordinate plane
        """
        gross_shadows = {}
        for k in self.context.keys():
            gross_shadows[k] = self.calc_shadow(self.context[k],sun_alt,relative_sun_az)
        return gross_shadows
    
    # Still in the process of converting from ASF to generic:
    
    def extract_self_shading_right(self, sun_alt, relative_sun_az):
        """
        Subtract from gross_shadows dictionary the mutual shading between panels.
        
        *** Pyclipper.Area returns a positive or negative value if polygon definition 
        is counterclockwise or clockwise, respectively.
    
        :param gross_shadows: Shadow on generic wall for every ASF panel
        :type: dict{key:identifier(integer), value:np.ndarray(floats)}
        :return net_shadows: Shadow on generic wall without mutual shading
        :rtype: dict{key:integer, value:np.ndarray(floats)}
        
        Pyclipper functionality:
            - We start with a subject polygon. i.e. the gross shadow:
                ndarray -> [[S0],[S1],[S2],[S3]] (2 levels of [] pairing, shape = 2)
            - We select a surrounding shadow potential to perform shading on subject as clip:
                ndarray -> [[S0],[S1],[S2],[S3]] (2 levels of [] pairing, shape = 2)
                
            - We compute the intersection by a clipping boolean operation.
                - Scenario 1: No intersection, or polygon partly intersected:
                    - Clipper output [[[[P1],[P2],...]]] (3 levels of [] pairing, shape = 3)
                    - The longitude of the list output will be 1 since there is only 1 polygon
                - Scenario 2: Severe intersection, polygon is fragmented:
                    - Clipper output [[[[P1],[P2],...]]] (3 levels of [] pairing, shape = 3)
                    - The longitude of the list output will be >1 since there are several polygons
        
        Conclusion:
            To keep track if there is only one subject polygon or several, use subj.shape.
            If Scenario 1 is performed, save output list as np.asarray(outuput[0]), so shape is 2    
        
        """
        gross_shadows = self.calc_gross_shadows(sun_alt,relative_sun_az)
        net_shadows = {} # Dictionary containing all the shadow geometries with self-shading extracted
        self_shading_param = {} # Dictionary containing self shading area and width
        self_shading_area = 0 # Initialize variable for cases when net_shadow is sliced
    
        # Check mutual shading between panels and subtract areas
        for row_index,col_index in product(reversed(range(self.nrows)),reversed(range(self.ncols))): # From bottom to top (big to low)
            # Shadow from which extract self-shading is subject
            
            #Createa .net List of IntPoint objects from the panel being analysed
            subj = List[IntPoint]([IntPoint(s[0],s[1]) for s in gross_shadows[row_index, col_index].values()])
            gross_shadow_area = abs(float(clp.Area(subj)))
            
            if not gross_shadow_area == 0: # Shadow to which extract self-shading is not null
                for index2, shadow2 in gross_shadows.iteritems(): #check shadow with surrounders
                
                    if (sun_alt >= 50 and (index2[0] < row_index or index2[0] == row_index and index2[1] < col_index )) or \
                       (sun_alt < 50 and ((index2[0] <= row_index and index2[1] < col_index ) or \
                       (index2[0] >= row_index and index2[1] < col_index))):
                        # Pseudocode:
                        # if sun_alt >= 50 and neighbour is lower or neigbour is to the right of current panel
                        # or if sun_alt < 50 and neighbour is same/lower and to the right of the current panel
                        # or if neighbour is on same/higher row and to the right of the current panel.
                        
                        clip = List[IntPoint]([IntPoint(s[0],s[1]) for s in shadow2.values()])
                        
                        if not clp.Area(clip) == 0:
                            cl = clipper.ClipperLib
                            # print dir(cl.ClipperBase)
                            # clipper.ClipperLib.ClipperBase.AddPath()
                            print clipper.AddPath()
                            clp.AddPolygon(clip, cl.PolyType.ptClip, True)
                            
                            if len(subj.shape) == 2: # Shape is pairs of []
                                clp.AddPath(subj, clp.PT_SUBJECT, True) # Subject is one polygon 
                                
                            else: # Subject consists of several polygons (previously sliced)
                                clp.AddPaths(subj, clp.PT_SUBJECT, True)
                            
                            # Execute the boolean difference between sub and clip
                            diff = clp.Execute(clp.CT_DIFFERENCE, clp.PFT_EVENODD, clp.PFT_EVENODD)
                            
                            if diff: # Boolean operation successful (not null)
                                if len(np.asarray(diff))  == 1: # diff contains 1 element. has shape 3
                                    subj = np.asarray(diff[0]) # Rebuild the subject shadow
                                else: # diff contains more than one elment has shape 3
                                    subj = np.asarray(diff)
                # Extraction of surrounding shadows completed. Store results
                if len(subj.shape) == 2:  # Net shadow is one polygon (2d matrix)

                    # Store % shaded area from mutual shading
                    self_shading_area = abs(clp.Area(subj))
                    self_shading_param[row_index, col_index] = [1 - (self_shading_area / gross_shadow_area)]

                    # If % shaded area is significant, store its width
                    if self_shading_param[row_index, col_index][0] >= 0.1:
                        self_shading_param[row_index, col_index].append(calc_hor_shading(subj, gross_shadows[row_index, col_index]))
                    else:
                        self_shading_param[row_index, col_index].append(0)
                        
                else: # Net shadow consists of several polygons (previously sliced)
                    
                    for shadow_portion in subj:
                        self_shading_area += abs(clp.Area(shadow_portion))   
                        
                    # Store % shaded area from mutual shading
                    self_shading_param[row_index, col_index] = [1 - (self_shading_area / gross_shadow_area)]
                    
                    # If % shaded area is significant, store its width
                    if self_shading_param[row_index, col_index][0] >= 0.1:
                        dist = 0
                        for shadow_portion in subj:  
                            param = calc_hor_shading(shadow_portion, gross_shadows[row_index, col_index])
                            if param > dist:
                                dist = param
                        self_shading_param[row_index, col_index].append(dist)
                    else:
                        self_shading_param[row_index, col_index].append(0)

                net_shadows[row_index, col_index] = np.asarray(subj) # add to the result the shadow o the unified result
                self_shading_area = 0 # reset variable
        return net_shadows, self_shading_param
    
    def extract_self_shading_left(self, sun_alt, relative_sun_az):
        gross_shadows = self.calc_gross_shadows(sun_alt,relative_sun_az)
        net_shadows = {} # Dictionary containing all the shadow geometries with self-shading extracted
        self_shading_param = {} # Dictionary containing self shading area and width
        self_shading_area = 0 # Initialize variable for cases when net_shadow is sliced
    
        # Check mutual shading between panels and subtract areas
        for row_index in reversed(range(nrows)): # From bottom to top (big to low)
            for col_index in range(ncols): # From left to right (low to big)
                if row_index%2 == col_index%2:
                    
                    # Shadow to which extract self-shading is subject
                    subj = gross_shadows[row_index, col_index]
                    gross_shadow_area = abs(float(clp.Area(subj)))
                    if not gross_shadow_area == 0: # Shadow to which extract self-shading is not null
                        for index2, shadow2 in gross_shadows.iteritems(): # Check shadow with surrounders
                            if (sun_alt >= 50 and (index2[0] < row_index or index2[0] == row_index and index2[1] > col_index )) or (sun_alt < 50 and ((index2[0] <= row_index and index2[1] > col_index ) or (index2[0] >= row_index and index2[1] > col_index))): 

                                # Check surrounding shadow on top of subj and the ones to the left
                                # Surrounding shadows are clip (considered individually)
                                clip = shadow2
                                clp = clp.Pyclipper()
                                if not clp.Area(clip) == 0:
                                    clp.AddPath(clip, clp.PT_CLIP, True)
                                    if len(subj.shape) == 2: # Shape is pairs of []
                                        clp.AddPath(subj, clp.PT_SUBJECT, True) # Subject is one polygon 
                                        
                                    else: # Subject consists of several polygons (previously sliced)
                                        clp.AddPaths(subj, clp.PT_SUBJECT, True)
                                    
                                    # Execute the boolean difference between sub and clip
                                    diff = clp.Execute(clp.CT_DIFFERENCE, clp.PFT_EVENODD, clp.PFT_EVENODD)
                                    
                                    if diff: # Boolean operation successful (not null)
                                        if len(np.asarray(diff))  == 1: # diff contains 1 element. has shape 3
                                            subj = np.asarray(diff[0]) # Rebuild the subject shadow
                                        else: # diff contains more than one elment has shape 3
                                            subj = np.asarray(diff)
                        # Extraction of surrounding shadows completed. Store results
                        if len(subj.shape) == 2:  # Net shadow is one polygon (2d matrix)
    
                            # Store % shaded area from mutual shading
                            self_shading_area = abs(clp.Area(subj))
                            self_shading_param[row_index, col_index] = [1 - (self_shading_area / gross_shadow_area)]
    
                            # If % shaded area is significant, store its width
                            if self_shading_param[row_index, col_index][0] >= 0.1:
                                self_shading_param[row_index, col_index].append(calc_hor_shading(subj, gross_shadows[row_index, col_index]))
                            else:
                                self_shading_param[row_index, col_index].append(0)
                                
                        else: # Net shadow consists of several polygons (previously sliced)
                            
                            for shadow_portion in subj:
                                self_shading_area += abs(clp.Area(shadow_portion))   
                                
                            # Store % shaded area from mutual shading
                            self_shading_param[row_index, col_index] = [1 - (self_shading_area / gross_shadow_area)]
                            
                            # If % shaded area is significant, store its width
                            if self_shading_param[row_index, col_index][0] >= 0.1:
                                dist = 0
                                for shadow_portion in subj:  
                                    param = calc_hor_shading(shadow_portion, gross_shadows[row_index, col_index])
                                    if param > dist:
                                        dist = param
                                self_shading_param[row_index, col_index].append(dist)
                            else:
                                self_shading_param[row_index, col_index].append(0)
    
                    net_shadows[row_index, col_index] = np.asarray(subj) # add to the result the shadow o the unified result
                    self_shading_area = 0 # reset variable
                        
        return net_shadows, self_shading_param
    
    def calc_shadow_window(net_shadows):
        """
        Subtract from net_shadows dictionary the regions outside the window frame and
        compute the total shadow laying on the window as a % of its area.
    
        :param net_shadows: Shadow on generic wall for every ASF panel with mutual_shading extracted
        :type: dict{key:identifier(tuple), value:np.ndarray(floats)}
        :param window_width: Width of the window behind the ASF [mm].
        :type window_width: float    
        :param window_height: Height of the window behind the ASF [mm].
        :type window_height: float
        :param panel_offset: Cartesian distance between window frame and nearest PV centerpoint [mm].
        :type panel_offset: float
        :return area_shadow_window: Global net ASF shadow laying on window as % of its area
        :rtype: float
        """
        # Construct window frame geometry
        window_frame1 = []
        for p in self.window_vertices:
            window_frame1.append([p[0],p[1]])
        
        window_width = 4
        window_height = 2
        
        window_frame = np.array([[0, 0], [window_width, 0], [window_width, - window_height], [0, - window_height]])
        window_frame += [ - panel_offset, panel_offset]
        clip = window_frame # Window frame is clip
        
        print window_frame1
        print clip
        
        # Initialize dictionary and result variable
        shadow_window = {}
        area_shadow_window = 0
        
        for index, shadow in net_shadows.iteritems():
            subj = shadow # Shadow is subj
        
            if not (len(shadow.shape) == 2 and clp.Area(shadow) == 0):            
                clp = clp.Pyclipper()
                clp.AddPath(clip, clp.PT_CLIP, True)
                if len(subj.shape) == 2: # Subject is one polygon
                    clp.AddPath(subj, clp.PT_SUBJECT, True)
                else: # Subject consists of several polygons (previously sliced)
                    clp.AddPaths(subj, clp.PT_SUBJECT, True)
        
                # Exectue boolean intersection
                solution = clp.Execute(clp.CT_INTERSECTION, clp.PFT_EVENODD, clp.PFT_EVENODD)
                
                if solution: # Boolean operation successful (not null)
                    if len(np.asarray(solution)) == 1: # diff contains 1 element. has shape 3
                        subj = np.asarray(solution[0]) # Rebuild the subject shadow
                    else: # diff contains more than one element has shape 3
                        subj = np.asarray(solution)
                else: # Intersection not successful (all the shadow is outside window frame)
                    subj *= 0
            else:
                subj *= 0
    
            shadow_window[index] = np.asarray(subj) # Add to the result
    
        for index, shadow in sorted(shadow_window.iteritems(), reverse = True):
            if shadow.any():
                if len(shadow.shape) == 2:  # Shadow is one polygon
                    area_shadow_window += clp.Area(shadow)
                    
                else: # Shadow consists of several polygons (previously sliced)
                    for shadow_portion in shadow:
                        test = np.asarray(shadow_portion)
                        area_shadow_window += clp.Area(test)
        
        return area_shadow_window
    
    def shading_pattern(self, sun_alt, relative_sun_az):
        """    
        credits: JoanDM
        
        For a given sun position, calculate the shaded area on the window 
        [% window_area], and the mutual shading between panels.
        
        Main strategy -> Instead of computing sun-rays and 3D intersections, map 
                        the shadows in 2D space (window and wall). Output 
                        percentual data for applicable results.
    
        Steps:              
            Project every panel on the wall according to sun rays -> Map the shadows on the wall
            Detect and extract mutual shading -> Extract self intersections of the mapped shadows
            Quantify mutual shading area as % panel area -> Divide new shadow and raw shadow areas
            Particularize shading as % panel side -> Divide new shadow width (parallel to S0-S1) by S0-S3 vector
            Detect and extract shadows outside window -> Keep only the resulting shadows inside window frame
            Quantify shadow on window as % area -> Divide total net shadow area by window area
            
        :param panel_angles: Slope and azimuth of each panel [Deg].
        :type panel_angles: dict{(identifier): [panel_tilt, panel_az]} (Facade convention)
    
            :identifier: (row_number, column_number). (0, 0). Top row, left column
            :type identifier: tuple (int, int)
            :panel_tilt range [0,90). 0 = open position.
            :type panel_tilt: int
            panel_az range [-45,45]. Example facade south: -45 = SE, 45 = SW. (Facade convention)
            :type panel_az: int
    
        :param sun_alt: Sun altitude [Deg].
        :type sun_alt: float
        :param relative_sun_az: Sun azimuth [Deg]. (Facade convention, i.e. sun_position.py output)
        :type relative_sun_az: float
    
            relative_sun_az is a relative angle in respect to ASF plane normal. From there, 
            it ranges clockwise and counterclockise, so its range is [-180, 180)

        :return shading_result: Window shaded area [% window_area], mutual shading [% panel_area, % horizontal]
        :rtype: dict{key:string, value:float or np.ndarray(floats)}
        """
        #???
        #shading_result = dict.fromkeys(panel_angles, [0, 0])
    
        if self.is_sunny(sun_alt, asf_sun_az): # Check if sun rays interact with the ASF
        
            # Calculate the shadow on the window for every PV panel of the ASF
            gross_shadows = self.calc_gross_shadow(sun_alt, asf_sun_az)
    
            # Extract mutual shading between panels by checking the intersection of gross_shadows
            net_shadows, self_shading_param = extract_self_shading(gross_shadows, sun_alt, asf_sun_az)
            
            # Calculate the resulting shadow that lies on the window frame as a % area.
            area_shadow_window = calc_shadow_window(net_shadows)
            
            shading_result = self_shading_param
            shading_result["shadow_on_window"] = area_shadow_window/self.window_area
    
        else: # It's dark. No sun direct rays interact with the facade 
    
            shading_result['shadow_on_window'] = 0.0
    
        return shading_result

""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""



Window = ShadedWindow(window_geometry=_window_geometry,
                    context_geometry=context_geometry)

Sun = SunPath(location=location,
              azimuth_tilt_rad=Window.azimuth_tilt_rad,
              altitude_tilt_rad=Window.altititude_tilt_rad)

sun_alt,relative_sun_az = Sun.calc_relative_sun_position(hoy)


window_centroid = Window.window_centroid
window_normal = Window.window_normal

left_right_points = []
right_left_points = []



Window.extract_self_shading_right(sun_alt,relative_sun_az)
