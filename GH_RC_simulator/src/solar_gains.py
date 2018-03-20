import rhinoscriptsyntax as rs
import Rhino as rc
import ghpythonlib.components as gh
import math

def extract_location_data(location):
    """
    Extract the longitude, latitude and time zone from ladybug loction output
    """
    x_lines = [s.strip() for s in location.splitlines()]
    
    lat = float(x_lines[2].split(',')[0])
    long = float(x_lines[3].split(',')[0])
    utc_offset = float(x_lines[4].split(',')[0])

    return lat,long,utc_offset
 
def azimuth_angle(vector):
    north = rc.Geometry.Vector3d(0,1,0)
    az = rs.VectorAngle(north,vector)
    if vector[0] < 0:
        az = 360-abs(az)
    return az

"""
================================Credits: Joan================================
"""

def calc_sun_position(latitude_deg, longitude_deg, year, hoy, utc_offset):
    """
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

    # Determine the day of the year
    day_of_year = hoy//24
    utc_hour = hoy - day_of_year*24

    # Calculate B parameter (in radians) for equation of time
    b_factor = (day_of_year - 1) * ((2 * math.pi) / 365);

    # Equation of time: empirical equation that corrects for the eccentricity of 
    # the Earth's orbit and the Earth's axial tilt        
    equation_of_time = 229.2 * 0.000075 + 229.2 * (0.001868 * math.cos(b_factor) - \
                                                   0.032077 * math.sin(b_factor)) - \
                                                   229.2 * (0.014615 * math.cos(2 * b_factor) + \
                                                            0.04089 * math.sin(2 * b_factor));

    # Local Standart Time Meridian (Earth rotation 15 degrees/h)
    standard_time = 15 * utc_offset

    # Time correction between local standard time and true solar time (in minutes) 
    time_correction = 4 * (longitude_deg - standard_time) + equation_of_time 

    # Local Solar Time (in hours) = Local Time + TC (in hours)
    utc_minute = 0 # all analysis will be hourly
    solar_time = utc_hour + (utc_minute + time_correction) / 60.0;

    # Translate Local Solar Time to an angle 
    hour_angle_rad = (math.pi * 2 / 24) * (solar_time - 12);

    # Calculate the declination angle: The variation due to the earths tilt
    declination_rad = math.radians(23.45 * math.sin((2 * math.pi / 365.0) * 
                                                    (day_of_year - 81)));

    # Convert latitude to to radians
    latitude_rad = math.radians(latitude_deg);

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

def sunvec(sun_alt,sun_az):
    sun_az_rad, sun_alt_rad = map(math.radians, [sun_az + 90, sun_alt])
    
    sunvec_x = math.cos(sun_az_rad) * math.cos(sun_alt_rad);
    sunvec_y = math.sin(sun_alt_rad);
    sunvec_z = math.sin(sun_az_rad) * math.cos(sun_alt_rad);
    
    return rc.Geometry.Vector3d(sunvec_x,sunvec_y,sunvec_z)

def is_sunny(window_geometry, sun_alt, sun_az):
    """
    Checks wether is sunny or not by considering sun relative angle with respect to ASF.
    I.e. the sun must not be below the horizon and has to be in front of the facade.   
    :param window_geometry: geometry object representing window
    :param sun_alt: Sun altitude [Deg].
    :type sun_alt: float
    :param sun_az: Sun azimuth [Deg]. (Facade convention, i.e. sun_position.py output)
    :type sun_az: float
    :rtype: bool
    """
    window_normal = rs.SurfaceNormal(window_geometry,[0.5,0.5])
    window_az = azimuth_angle(window_normal)
    window_sun_az = window_az - sun_az
    
    return sun_alt >= 0.0  and window_sun_az>0 and window_sun_az<180

"""
==============================End Credits: Joan==============================
"""

def shade_to_clockwise_points(shade):
    """
    :param shade: surface object representing shading element
    :type shade: rhino surface element
    :return sorted_points: list of points sorted clockwise
    :rtype sorted_points: list
    """
    points = rs.SurfacePoints(shade,True)
    centroid, error= rs.SurfaceAreaCentroid(shade)
    
    # Generate plane oriented to world-Z
    plane = rc.Geometry.Plane(centroid,rc.Geometry.Vector3d(0,0,-1))
    
    # calculate vectors from center to points
    vecs = [centroid - point for point in points]
    
    # calculate angles between vectors and plane x-axis
    angles = [rc.Geometry.Vector3d.VectorAngle(plane.XAxis,v,plane) for v in vecs]
    sorted_points = [p[1] for p in sorted(zip(angles,points))] 
    return sorted_points

def point_in_window(point,window_geometry):
    x = []
    y = []
    z = []

    vertices = rs.SurfaceEditPoints(window_geometry)
    
    for v in vertices:
        x.append(v[0])
        y.append(v[1])
        z.append(v[2])
    
    x_range = min(x) <= point[0] <= max(x)
    y_range = min(y) <= point[1] <= max(y)
    z_range = min(z) <= point[2] <= max(z)
    
    return x_range and y_range and z_range

def shaded_window(window, shade, vector):
    
    # Get window parameters
    window_centroid = rs.EvaluateSurface(window,0.5,0.5)
    # Scale window geometry so that all points hit the surface
    window_plane = rs.ScaleObject(window,window_centroid,(10,10,10),True)
    
    shadow_geometry = []
    
    for shade in shading_geometry:
        # Arrange shade vertices clockwise
        shade_points = shade_to_clockwise_points(shade)

        # Draw Shadow
        shadow_points = rs.ProjectPointToSurface(shade_points,window_plane,vector)
        shadow = rs.AddSrfPt(shadow_points)
        print shadow
        # First shadow
        if shadow_geometry == []:
            shadow_geometry = [shadow]
            
        # Subsequent shadows
        else:
            for existing_shadow in shadow_geometry:
                # try boolean union
                union = rs.BooleanUnion(existing_shadow,shadow)
                
                if union is not None:
                    # current shadow successfully merged with existing shadow
                    existing_shadow = union
                    
                else:
                    # No intersection. current shadow is isolated
                    shadow_geometry.append(shadow)

    # Extract Shadow from window geometry and keep the unshaded fragments in the window.
    if window is not None and shadow_geometry != []:
        
        # Initialise unshaded fragments
        unshaded_fragments = [window]
        
        for shadow in shadow_geometry:
            for existing_fragment in unshaded_fragments:
                intersection = rs.BooleanIntersection(existing_fragment, shadow, False)
                
                # Discard intersections outside window geometry
                new_fragments = []
                for u in intersection:
                    intersection_centroid = rs.SurfaceAreaCentroid(u)[0]
                    if point_in_window(intersection_centroid,window_geometry):
                        new_fragments.append(u)
                # replace existing fragment with newly created fragments
                existing_fragment = new_fragments
            
            # Flatten new list of fragments for the next shadow iteration
            if any(type(f) is list for f in unshaded_fragments):
                unshaded_fragments = [item for sublist in unshaded_fragments for item in sublist]

        return unshaded_fragments

    else:
        return window

def main(window_geometry, shading_geometry, hoy, year, ladybug_location):
    # Check that window and shading are planar surfaces

    # Get location data
    lat,long,utc_offset = extract_location_data(ladybug_location)
    
    # Calculate sun position
    sun_alt,sun_az = calc_sun_position(lat,long,year,hoy,utc_offset)
    
#    if is_sunny(window_geometry,sun_alt,sun_az): 
#        print 'is sunny'
    sun_vec = sunvec(sun_alt,sun_az)
    return shaded_window(window_geometry,shading_geometry,sun_vec)

#    else:
#        return None


shaded_window = main(window_geometry,shading_geometry,hoy,year,ladybug_location)

