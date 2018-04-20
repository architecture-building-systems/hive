"""
Implementation of Joan's code using clipper
"""
import scriptcontext as sc
import rhinoscriptsyntax as rs
import Rhino as rc
import ghpythonlib.components as gh
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

clipper_path = os.getcwd() + '\clipper_library.dll'
clipper_library = System.Reflection.Assembly.LoadFrom(clipper_path)

clipper = clipper_library.ClipperLib.Clipper()
polyTree = clipper_library.ClipperLib.PolyTree
polyType = clipper_library.ClipperLib.PolyType
clipType = clipper_library.ClipperLib.ClipType
polyFillType = clipper_library.ClipperLib.PolyFillType

IntPoint = clipper_library.ClipperLib.IntPoint


class RelativeSun(object):
    def __init__(self,location,azimuth_tilt_rad, altitude_tilt_rad, normal):
        # Extract location data
        self.latitude_deg = float(location[0])
        self.longitude_deg = float(location[1])
        self.utc_offset = float(location[2])
        self.year = int(location[3])
        
        self.azimuth_tilt_rad = azimuth_tilt_rad
        self.altitude_tilt_rad = altitude_tilt_rad
        self.normal = normal
    
    def is_sunny(self,relative_sun_alt, relative_sun_az):
        return relative_sun_alt >= 0.0 and abs(relative_sun_az) < 90.0
    
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
        relative_sun_az = sun_az - math.degrees(self.azimuth_tilt_rad)
        if relative_sun_az < - 180:
            relative_sun_az = 360 - abs(relative_sun_az)
        return relative_sun_az
    
    def calc_relative_altitude(self, sun_alt):
        relative_sun_alt = sun_alt - math.degrees(self.altitude_tilt_rad)
        return relative_sun_alt
    


class ShadedWindow(object):
    """
    Adapted from shaded_window.py from the ASF_Control repository by Joan DM
    Returns window and self shading for user-defined geometries.
    """

    def __init__(self, window_geometry, context_geometry, point_in_zone,
                 glass_solar_transmittance=0.7, glass_light_transmittance=0.8):
        
        self.window_geometry = window_geometry
        self.extract_window_geometry(point_in_zone)
        self.transform_context_geometry(context_geometry)
        
        self.glass_solar_transmittance = glass_solar_transmittance
        self.glass_light_transmittance = glass_light_transmittance
        
        self.clipper_accuracy = 100000
    
    def extract_window_geometry(self,point_in_zone):
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
        
        self.window_centroid = rs.SurfaceAreaCentroid(self.window_geometry)[0]
        normal = rs.SurfaceNormal(self.window_geometry,[0.5,0.5])
        
        # Ensure window normal is facing outwards
        window_zone_vector = gh.Vector2Pt(self.window_centroid,point_in_zone)
        if abs(rs.VectorAngle(normal,window_zone_vector[0])) < 90:
            self.window_normal = rs.VectorReverse(normal)
        else:
            self.window_normal = normal

        self.window_area = rs.Area(self.window_geometry)

        self.window_vertices = rs.SurfaceEditPoints(self.window_geometry)
        self.window_plane = rs.PlaneFromPoints(self.window_centroid,self.window_vertices[0],self.window_vertices[1])
        
        edges = rs.DuplicateEdgeCurves(self.window_geometry)
        edge_lengths = [rs.CurveLength(e) for e in edges]
        edge_vectors = [round(rs.VectorCreate(rs.CurveStartPoint(e),rs.CurveEndPoint(e))[2]) for e in edges] 
        self.window_width = edge_lengths[0] if edge_vectors[0] == 0 else edge_lengths[1]
        self.window_height = edge_lengths[0] if edge_vectors[0] !=0 else edge_lengths[1]
        self.window_points = [rc.Geometry.Point2d(0,0),rc.Geometry.Point2d(self.window_width,0),rc.Geometry.Point2d(self.window_width,self.window_height),rc.Geometry.Point2d(0,self.window_height)]
        
        normal_xy = rc.Geometry.Vector3d(self.window_normal[0],self.window_normal[1],0)
        
        try:
            azimuth = rs.VectorAngle(north,normal_xy)
            if normal[0] < 0:
                # Faces west
                azimuth = 360-azimuth
        except ValueError:
            azimuth = 0
        
        try:
            altitude = rs.VectorAngle(rc.Geometry.Vector3d(normal[0],normal[1],0),normal)
            if normal[2]<0:
                altitude = -altitude
        except ValueError:
            altitude = 0
        
        self.altititude_tilt_rad = math.radians(altitude)
        self.azimuth_tilt_rad = math.radians(azimuth)
    
    def orient_point_2d_window(self,point):
        u,v = rs.SurfaceClosestPoint(self.window_geometry,point)
        x = u
        y = v
        z = abs(rs.DistanceToPlane(self.window_plane,point))
        return rc.Geometry.Point3d(x,y,z)
    
    def surface_to_clockwise_points(self,geometry):
        """
        :return sorted_points: list of points sorted clockwise
        :rtype sorted_points: list
        """
        
        points = rs.SurfacePoints(geometry,True)
        centroid, e= rs.SurfaceAreaCentroid(geometry)
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
    
    def dominant_shade_vector(self,shade_points):
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
    
    def transform_shade(self,shade):
        """
        Return a list of edge points for a shading surface
        """
        
        def round2(x):
            return int(round(x*100))
            
        centroid, shade_points = self.surface_to_clockwise_points(shade)
        
        transformed_points = []
        for p in shade_points:
            transformed_points.append(self.orient_point_2d_window(p))
        
        oriented_centroid = self.orient_point_2d_window(centroid)
        rounded_oriented_centroid = [round2(oriented_centroid[0]),round2(oriented_centroid[1])]
        
        return rounded_oriented_centroid, transformed_points
    
    def transform_context_geometry(self,context_geometry):
        """
        Transform context and arrange in any detectable array form
        Return a dictionary with
        key: shade centroid
        value: transformed points for each shade
        """
        #TODO: Explode shading polysurfaces into surfaces
        centroids_and_points = {}
        for xg in context_geometry:
            c,p = self.transform_shade(xg) 
            centroids_and_points[c[0],c[1]] = p
        
        #TODO: give this more tolerance so that it can also pick up array-like configurations.
        unique_x = sorted(list(set([x[0] for x in centroids_and_points.keys()])))
        x_dict = {k:v for k,v in enumerate(unique_x)}
        self.nrows = len(unique_x)
        
        unique_y = sorted(list(set([x[1] for x in centroids_and_points.keys()])))
        y_dict = {k:v for k,v in enumerate(unique_y)}
        self.ncols = len(y_dict)
        
        # Create new dictionary with tuple keys
        self.context = {}
        for x,y in product(range(self.nrows),range(self.ncols)):
            self.context[x,y] = centroids_and_points[x_dict[x],y_dict[y]]
    
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
        sunvec_x = math.cos(relative_sun_az_rad) * math.cos(relative_sun_alt_rad);
        sunvec_y = math.sin(relative_sun_alt_rad);
        sunvec_z = math.sin(relative_sun_az_rad) * math.cos(relative_sun_alt_rad);
        
        shadowintpoints = [] # IntPoints for clipping
        shadowpoints = [] # 3d points
        pv_a, pv_b = self.dominant_shade_vector(shade)
        pv_a_shadow = transform_point(pv_a,sunvec_x,sunvec_y,sunvec_z)
        pv_b_shadow = transform_point(pv_b,sunvec_x,sunvec_y,sunvec_z)
        pv_shadow_vector = [pv_b_shadow[0] - pv_a_shadow[0], pv_b_shadow[1] - pv_a_shadow[1]]
        
        # Calculate the projection on a 2-D surface (x,y)
        for point_3d in shade:
            x_point, y_point = transform_point(point_3d,sunvec_x,sunvec_y,sunvec_z)
            shadowintpoints.append(IntPoint(self.clipper_accuracy*x_point,self.clipper_accuracy*y_point))
            shadowpoints.append(rc.Geometry.Point3d(x_point,y_point,0))
            
        return List[IntPoint](shadowintpoints)#, pv_shadow_vector, shadowpoints
    
    def calc_gross_shadows(self,relative_sun_alt,relative_sun_az):
        """
        projects all shading objects onto window coordinate plane
        """
        gross_shadows = {}
        pv_vectors = {}
        if type(self.context.keys()[0]) == float:
            for k in self.context.keys():
                gross_shadows[k] = self.calc_shadow(self.context[k],relative_sun_alt,relative_sun_az)
        elif type(self.context.keys()[0]) == tuple:
            # an array configuration has been identified.
            for i,j in self.context.keys():
                gross_shadows[i,j] = self.calc_shadow(self.context[i,j],relative_sun_alt,relative_sun_az)
        return gross_shadows
    
    def draw_shadow_geometry(self,clipper_result):
        clipper_result_geometry = []
        for ms in clipper_result:
            points = []
            for p in ms:
                points.append(rc.Geometry.Point3d(p.X/self.clipper_accuracy,p.Y/self.clipper_accuracy,0))
            clipper_result_geometry.append(gh.PolyLine(points,closed=True))
        return clipper_result_geometry

    def clip_shadows_with_window(self,gross_shadows):
        
        # Initialise shadows list
        subj = []
        shadows = List[List[IntPoint]](gross_shadows.values())
        merged_shadows = List[List[IntPoint]]()
        clipper.Clear()
        clipper.AddPolygons(shadows,polyType.ptSubject)
        union = clipper.Execute(clipType.ctUnion, merged_shadows, polyFillType.pftNonZero, polyFillType.pftNonZero)

        # Construct window frame geometry
        window_vertices = [IntPoint(0,0), IntPoint(self.clipper_accuracy*self.window_width, 0), IntPoint(self.clipper_accuracy*self.window_width, self.clipper_accuracy*self.window_height), IntPoint(0, self.clipper_accuracy*self.window_height)]
        window_frame = List[IntPoint](window_vertices)
        
        # Remove parts of shadows which lie outside the window frame
        unshaded_polygons = List[List[IntPoint]]()
        clipper.Clear()
        clipper.AddPolygon(window_frame,polyType.ptClip)
        clipper.AddPolygons(merged_shadows,polyType.ptSubject)
        diff = clipper.Execute(clipType.ctIntersection, unshaded_polygons, polyFillType.pftNonZero, polyFillType.pftNonZero)
        
        if diff:
            return unshaded_polygons, window_frame
        else:
            return None
        
    def calc_shadow_window(self,gross_shadows):
        """
        For a given set of shadows (one hour)
        Subtract from net_shadows dictionary the regions outside the window frame and
        compute the total shadow laying on the window as a % of its area.
        """
        
        unshaded_polygons, window_frame = self.clip_shadows_with_window(gross_shadows)
        
        if unshaded_polygons is not None:
            shaded_area = 0
            for p in unshaded_polygons:
                shaded_area += abs(clipper.Area(p))
            unshaded_ratio = 1 - shaded_area/clipper.Area(window_frame)
            
            try:
                assert unshaded_ratio < 1.0
                return unshaded_ratio
            except AssertionError:
                # clipper failed and returned overlapping geometries.
                print 'clipping warning for hour: ', h

        else:
            #no clipping occured between shadow and window
            return 1
    
    """

    def calc_diffuse_irradiation():
        return window_diffuse irradiation
    
    def calc_ground_ref_irradiation():
        return window_ground_ref_irradiation
    
    def radiation(self,normal_irradiation,horizontal_irradiation,unshaded_ratio):
        
        # Direct Irradiation
        if not normal_irradiation == 0:
            dir_irradiation = normal_radiation * math.cos(angle_incidence)
        
        # Diffuse Irradiation
        if not horizontal_irradiation == 0:
            
            # Calculate the amount of diffuse radiation on the panel
            # no idea why 0.4 (https://github.com/architecture-building-systems/ASF_Control/blob/662c06393c1cd9e2452375c9c9d47f17f03a6e64/ASF_simulation_framework/Radiation/dev_radiation.py#L468)
            view_factor = 0.4 * unshaded_area
            
            diff_irradiation = calc_diff_irradiation(normal_irradiation, horizontal_irradiation, 
                                                     view_factor, panel_tilt, panel_az,
                                                     panel_size, sun_alt, 
                                                     angle_incidence, perez_coef)  
                            
            # Calculate diffuse and reflected irradiation on the ASF
            ground_ref_irradiation = calc_ground_ref_irradiation(albedo, 
                                                                          norm_radiation, 
                                                                          normal_irradiation, sun_alt,
                                                                          view_factor)  
        
        window_irradiation = window_dir_irradiation + window_diff_irradiation + window_ground_ref_irradiation
        
        window_diff_lux = hor_lux * view_factor
        window_ground_ref_lux = albedo * hor_lux * (1 - view_factor)
        window_lighting =  window_dir_lux + window_diff_lux + window_ground_ref_lux
        
        return window_irradiation, window_lighting
    """

def list_to_tree(nestedlist):
    layerTree = DataTree[object]()
    for i, item_list in enumerate(nestedlist):
        path = GH_Path(i)
        layerTree.AddRange(item_list,path)
    return layerTree

""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""
Window = ShadedWindow(window_geometry=_window_geometry,point_in_zone=_point_in_zone,
                    context_geometry=context_geometry)

Sun = RelativeSun(location=location,
                  azimuth_tilt_rad=Window.azimuth_tilt_rad,
                  altitude_tilt_rad=Window.altititude_tilt_rad,
                  normal = Window.window_normal)

window_centroid = Window.window_centroid
window_normal = Window.window_normal

unshaded_ratio = []
direct_solar = []
#unshaded_polys = []
for h in hoy:
    
    relative_sun_alt,relative_sun_az = Sun.calc_relative_sun_position(h)
    sun_alt,sun_az = Sun.calc_sun_position(h)
    
    incidence = math.acos(math.cos(math.radians(sun_alt)) * math.cos(math.radians(relative_sun_az)))
    
    
    if Sun.is_sunny(relative_sun_alt,relative_sun_az):
        shadow_dict = Window.calc_gross_shadows(relative_sun_alt,relative_sun_az)
        unshaded = Window.calc_shadow_window(shadow_dict)
        unshaded_ratio.append(unshaded)
        #TODO: collect points and merge shadows in pyclipper for a faster shading visualisation.

#        unshaded_polys.append(geometry)
    else:
        direct_solar.append(0)
        unshaded_ratio.append(0)
        unshaded_polys.append(rc.Geometry.Polyline())

visualise = list_to_tree(unshaded_polys)
