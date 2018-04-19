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


class SunPath(object):
    def __init__(self,location,azimuth_tilt_rad,altitude_tilt_rad):
        # Extract location data
        self.latitude_deg = float(location[0])
        self.longitude_deg = float(location[1])
        self.utc_offset = float(location[2])
        self.year = int(location[3])
        
        self.azimuth_tilt_rad = azimuth_tilt_rad
        self.altitude_tilt_rad = altitude_tilt_rad
    
    def is_sunny(self,sun_alt, relative_sun_az):
        return sun_alt >= 0.0 
    
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
    Returns window and self shading for user-defined geometries.
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
        self.window_plane = rs.PlaneFromPoints(self.window_centroid,self.window_vertices[0],self.window_vertices[1])
        
        edges = rs.DuplicateEdgeCurves(self.window_geometry)
        edge_lengths = [rs.CurveLength(e) for e in edges]
        edge_vectors = [round(rs.VectorCreate(rs.CurveStartPoint(e),rs.CurveEndPoint(e))[2]) for e in edges] 
        self.window_width = edge_lengths[0] if edge_vectors[0] == 0 else edge_lengths[1]
        self.window_height = edge_lengths[0] if edge_vectors[0] !=0 else edge_lengths[1]
        self.window_points = [rc.Geometry.Point2d(0,0),rc.Geometry.Point2d(self.window_width,0),rc.Geometry.Point2d(self.window_width,self.window_height),rc.Geometry.Point2d(0,self.window_height)]
        
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
    
    def calc_shadow(self, shade, sun_alt, relative_sun_az):
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
        relative_sun_az_rad, sun_alt_rad = map(math.radians, [relative_sun_az + 90, sun_alt]);
    
        # Create sun vector
        sunvec_x = math.cos(relative_sun_az_rad) * math.cos(sun_alt_rad);
        sunvec_y = math.sin(sun_alt_rad);
        sunvec_z = math.sin(relative_sun_az_rad) * math.cos(sun_alt_rad);
        
        shadowintpoints = [] # IntPoints for clipping
        shadowpoints = [] # 3d points
        pv_a, pv_b = self.dominant_shade_vector(shade)
        pv_a_shadow = transform_point(pv_a,sunvec_x,sunvec_y,sunvec_z)
        pv_b_shadow = transform_point(pv_b,sunvec_x,sunvec_y,sunvec_z)
        pv_shadow_vector = [pv_b_shadow[0] - pv_a_shadow[0], pv_b_shadow[1] - pv_a_shadow[1]]
        
        # Calculate the projection on a 2-D surface (x,y)
        for point_3d in shade:
            x_point, y_point = transform_point(point_3d,sunvec_x,sunvec_y,sunvec_z)
            shadowintpoints.append(IntPoint(10000*x_point,10000*y_point))
            shadowpoints.append(rc.Geometry.Point3d(x_point,y_point,0))
            
        return List[IntPoint](shadowintpoints), pv_shadow_vector, shadowpoints
    
    def calc_gross_shadows(self,sun_alt,relative_sun_az):
        """
        projects all shading objects onto window coordinate plane
        """
        gross_shadows = {}
        pv_vectors = {}
        if type(self.context.keys()[0]) == float:
            for k in self.context.keys():
                gross_shadows[k] = self.calc_shadow(self.context[k],sun_alt,relative_sun_az)
        elif type(self.context.keys()[0]) == tuple:
            # an array configuration has been identified.
            for i,j in self.context.keys():
                gross_shadows[i,j] = self.calc_shadow(self.context[i,j],sun_alt,relative_sun_az)
        return gross_shadows
    
    # Still being adapted for a generic geometry:
    
    def calc_shadow_window(self,gross_shadows):
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
        shadows = List[List[IntPoint]]([x[0] for x in gross_shadows.values()])
        
        shadow_points = [x[2] for x in gross_shadows.values()]
        shadow_points = [item for sublist in shadow_points for item in sublist]
        
        window_vertices = [IntPoint(0,0), IntPoint(10000*self.window_width, 0), IntPoint(10000*self.window_width, 10000*self.window_height), IntPoint(0, 10000*self.window_height)]
        window_frame = List[IntPoint](window_vertices)
        
        print 'window frame:', clipper.Area(window_frame)/100000000
        
        # Initialize dictionary and result variable
        shadow_window = {}
        area_shadow_window = 0
        
        clipper.AddPolygons(shadows,polyType.ptSubject)
        
        solution_tree = List[List[IntPoint]]()
        union = clipper.Execute(clipType.ctUnion, solution_tree, polyFillType.pftNonZero, polyFillType.pftNonZero)
        shadow_area = 0
        
        print 'number of shadows:', len(solution_tree)
        for sol in solution_tree:
            shadow_area += clipper.Area(sol)
        print 'shadow area:', shadow_area/100000000
        
        merged_shadows = solution_tree
        
        solution_tree = List[List[IntPoint]]()
        clipper.AddPolygon(window_frame,polyType.ptSubject)
        clipper.AddPolygons(merged_shadows,polyType.ptClip)

        diff = clipper.Execute(clipType.ctDifference, solution_tree, polyFillType.pftEvenOdd, polyFillType.pftEvenOdd)
        if diff:
            print 'number of clipped shadows:', len(solution_tree)
            shaded_area = 0
            for sol in solution_tree:
                shaded_area += clipper.Area(sol)
            print 'unshaded area:', shaded_area/100000000

        """
        # Add all shadows
        for index,shadow in .iteritems():
            subj = shadow[0] # Shadow is subj
            
            if not clipper.Area(shadow) == 0:      
                clipper.AddPolygon(clip, polyType.PT_CLIP)
                if len(subj.shape) == 2: # Subject is one polygon
                    clipper.AddPath(subj, clipper.PT_SUBJECT, True)
                else: # Subject consists of several polygons (previously sliced)
                    clipper.AddPaths(subj, clipper.PT_SUBJECT, True)
                
                # Exectue boolean intersection
                solution = clipper.Execute(clipper.CT_INTERSECTION, clipper.PFT_EVENODD, clipper.PFT_EVENODD)
                
                if solution: # Boolean operation successful (not null)
                    if len(np.asarray(solution)) == 1: # solution contains 1 element. has shape 3
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
                    area_shadow_window += clipper.Area(shadow)
                    
                else: # Shadow consists of several polygons (previously sliced)
                    for shadow_portion in shadow:
                        test = np.asarray(shadow_portion)
                        area_shadow_window += clipper.Area(test)
        
        return area_shadow_window
        """
    
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

window_centroid = Window.window_centroid
window_normal = Window.window_normal

Window = ShadedWindow(window_geometry=_window_geometry,
                    context_geometry=context_geometry)

Sun = SunPath(location=location,
              azimuth_tilt_rad=Window.azimuth_tilt_rad,
              altitude_tilt_rad=Window.altititude_tilt_rad)


"""
def nestedListToDataTree(nestedlist):
    dataTree = DataTree[object]()
    for i, item_list in enumerate(nestedlist):
        path = GH_Path(i)
        dataTree.AddRange(item_list,path)
    return dataTree
visualise = nestedListToDataTree(Window.context.values())
"""

sun_alt,relative_sun_az = Sun.calc_relative_sun_position(hoy)

if Sun.is_sunny(sun_alt,relative_sun_az):
    shadow_dict = Window.calc_gross_shadows(sun_alt,relative_sun_az)
    shadow_points = Window.calc_shadow_window(shadow_dict)
    
    shadows = [gh.PolyLine(Window.window_points,closed=True)]
    for r,c in product(reversed(range(Window.nrows)),reversed(range(Window.ncols))):
        shadows.append(gh.PolyLine(shadow_dict[r,c][2],closed=True))
    visualise = shadows


#net_shadow,self_shading = Window.extract_self_shading_right(sun_alt,relative_sun_az)