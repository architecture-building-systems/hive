import rhinoscriptsyntax as rs
import Rhino as rc
import ghpythonlib.components as gh
import math
import datetime



def test_sun(hoy,year,ladybug_location):
    lat,long,utc_offset = extract_location_data(ladybug_location)
    sun_alt,sun_az = calc_sun_position(lat,long,year,hoy,utc_offset)
    sun_vec = sunvec(sun_alt,sun_az)
    
    start_of_year = datetime.datetime(year, 1, 1, 0, 0, 0, 0);
    utc_datetime = start_of_year + datetime.timedelta(hours = hoy);
    return sun_alt, sun_az, sun_vec, utc_datetime

def azimuth_angle(vector):
    north = rc.Geometry.Vector3d(0,1,0)
    az = rs.VectorAngle(north,vector)
    if vector[0] < 0:
        az = 360-abs(az)
    return az


def sunvec(sun_alt,sun_az):
    sun_az_rad, sun_alt_rad = map(math.radians, [sun_az, sun_alt])
    
    sunvec_x = -math.sin(sun_az_rad) * math.cos(sun_alt_rad);
    sunvec_y = -math.cos(sun_az_rad) * math.cos(sun_alt_rad);
    sunvec_z = -math.sin(sun_alt_rad)
    
    return rc.Geometry.Vector3d(sunvec_x,sunvec_y,sunvec_z)

def is_sunny(window_geometry, sun_vector):
    """
    Checks wether is sunny or not by comparing the sun vector and window normal.
    :param window_geometry: geometry object representing window
    :param sun_vector: vector representing sun rays.
    :rtype: bool
    """
    window_normal = rs.SurfaceNormal(window_geometry,[0.5,0.5])

    x_win = window_normal[0] > 0
    x_sun = sun_vector[0] > 0
    y_win = window_normal[1] > 0
    y_sun = sun_vector[1] > 0
    
    x = (x_win and not x_sun) or (x_sun and not x_win)
    y = (y_win and not y_sun) or (y_sun and not y_win)
    z = sun_vector[2] < 0
    
    return x and y and z

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

def unshaded_window_surfaces(window, shade, vector):
    """
    returns a set of surfaces representing the unshaded parts of the window
    
    """
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
        if shadow_points is not None:
            shadow = rs.AddSrfPt(shadow_points)
            if shadow is None:
                shadow = gh.SurfaceFromPoints(shadow_points)

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
            for ff in range(0,len(unshaded_fragments)):
                intersection = rs.BooleanIntersection(unshaded_fragments[ff], shadow, False)
                
                # Discard intersections outside window geometry
                new_fragments = []
                for i in intersection:
                    intersection_centroid = rs.SurfaceAreaCentroid(i)[0]
                    if point_in_window(intersection_centroid,window_geometry):
                        new_fragments.append(i)
                
                # replace existing fragment with newly created fragments
                unshaded_fragments[ff] = new_fragments

            # Flatten new list of fragments for the next shadow iteration
            if any(type(f) is list for f in unshaded_fragments):
                unshaded_fragments = [item for sublist in unshaded_fragments for item in sublist]
      

        return unshaded_fragments

    else:
        return window

class RadiationWindow(object):
    """
    Adapted from RC-building simulator's Window object in radiation.py
    
    returns solar_gains, illuminance through window as a list
    """

    def __init__(self, window_geometry, context_geometry, location, year, 
                 glass_solar_transmittance=0.7, glass_light_transmittance=0.8):
        
        self.window_geometry = window_geometry
        self.context_geometry = context_geometry
        self.calculate_window_tilt()
        
        #TODO: make this part of location
        self.year = year 
        
        # Extract location data
        x_lines = [s.strip() for s in location.splitlines()]
        self.latitude_deg = float(x_lines[2].split(',')[0])
        self.longitude_deg = float(x_lines[3].split(',')[0])
        self.utc_offset = float(x_lines[4].split(',')[0])

        self.glass_solar_transmittance = glass_solar_transmittance
        self.glass_light_transmittance = glass_light_transmittance

    def calc_window_tilt(self):
        
        north = rc.Geometry.Vector3d(0,1,0)
        vertical = rc.Geometry.Vector3d(0,0,1)
        faces_west = False
        centroid = rs.SurfaceAreaCentroid(self.window_geometry)[0]
        normal = rs.SurfaceNormal(self.window_geometry,[0.5,0.5])
        
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
        
        self.alititude_tilt_rad = math.radians(altitude)
        self.azimuth_tilt_rad = math.radians(azimuth)

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
        :return: altitude, azimuth, sunvec: Sun altitude and azimuth [Degrees] (Traditional convention) and sun vecto
        :rtype: float,float,Vector3d
        """
        # Source http://www.pveducation.org/pvcdrom/properties-of-sunlight/declination-angle
    
        # Set the date in UTC based off the hour of year and the year itself
        start_of_year = datetime.datetime(self.year, 1, 1, 0, 0, 0, 0);
        utc_datetime = start_of_year + datetime.timedelta(hours = hoy);
    
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
        standard_time = 15 * utc_offset
    
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
        
        sunvec_x = -math.sin(sun_az_rad) * math.cos(sun_alt_rad);
        sunvec_y = -math.cos(sun_az_rad) * math.cos(sun_alt_rad);
        sunvec_z = -math.sin(sun_alt_rad)
        sunvec = rc.Geometry.Vector3d(sunvec_x,sunvec_y,sunvec_z)
    
        return math.degrees(sun_alt_rad), math.degrees(sun_az_rad), sunvec



    def calc_solar_gains(self, sun_altitude, sun_azimuth, normal_direct_radiation, horizontal_diffuse_radiation):
        """
        Calculates the Solar Gains in the building zone through the set Window

        :param sun_altitude: Altitude Angle of the Sun in Degrees
        :type sun_altitude: float
        :param sun_azimuth: Azimuth angle of the sun in degrees
        :type sun_azimuth: float
        :param normal_direct_radiation: Normal Direct Radiation from weather file
        :type normal_direct_radiation: float
        :param horizontal_diffuse_radiation: Horizontal Diffuse Radiation from weather file
        :type horizontal_diffuse_radiation: float
        :return: self.incident_solar, Incident Solar Radiation on window
        :return: self.solar_gains - Solar gains in building after transmitting through the window
        :rtype: float
        """

        direct_factor = self.calc_direct_solar_factor(sun_altitude, sun_azimuth,)
        diffuse_factor = self.calc_diffuse_solar_factor()

        direct_solar = direct_factor * normal_direct_radiation
        diffuse_solar = horizontal_diffuse_radiation * diffuse_factor
        self.incident_solar = (direct_solar + diffuse_solar) * self.area

        self.solar_gains = self.incident_solar * self.glass_solar_transmittance

    def calc_illuminance(self, sun_altitude, sun_azimuth, normal_direct_illuminance, horizontal_diffuse_illuminance):
        """
        Calculates the Illuminance in the building zone through the set Window

        :param sun_altitude: Altitude Angle of the Sun in Degrees
        :type sun_altitude: float
        :param sun_azimuth: Azimuth angle of the sun in degrees
        :type sun_azimuth: float
        :param normal_direct_illuminance: Normal Direct Illuminance from weather file [Lx]
        :type normal_direct_illuminance: float
        :param horizontal_diffuse_illuminance: Horizontal Diffuse Illuminance from weather file [Lx]
        :type horizontal_diffuse_illuminance: float
        :return: self.incident_illuminance, Incident Illuminance on window [Lumens]
        :return: self.transmitted_illuminance - Illuminance in building after transmitting through the window [Lumens]
        :rtype: float
        """

        direct_factor = self.calc_direct_solar_factor(sun_altitude, sun_azimuth,)
        diffuse_factor = self.calc_diffuse_solar_factor()

        direct_illuminance = direct_factor * normal_direct_illuminance
        diffuse_illuminance = diffuse_factor * horizontal_diffuse_illuminance

        self.incident_illuminance = (
            direct_illuminance + diffuse_illuminance) * self.area
        self.transmitted_illuminance = self.incident_illuminance * \
            self.glass_light_transmittance

    def calc_direct_solar_factor(self, sun_altitude, sun_azimuth):
        """
        Calculates the cosine of the angle of incidence on the window 
        """
        sun_altitude_rad = math.radians(sun_altitude)
        sun_azimuth_rad = math.radians(sun_azimuth)

        # If the sun is in front of the window surface
        if math.cos(sun_azimuth_rad - self.azimuth_tilt_rad) > 0:
            # Proportion of the radiation incident on the window (cos of the
            # incident ray)
            direct_factor = math.cos(sun_altitude_rad) * math.cos(sun_azimuth_rad - self.azimuth_tilt_rad) + \
                math.sin(sun_altitude_rad) * math.cos(self.alititude_tilt_rad)

        else:
            # If sun is behind the window surface
            direct_factor = 0

        return direct_factor

    def calc_diffuse_solar_factor(self):
        """Calculates the proportion of diffuse radiation"""
        # Proportion of incident light on the window surface
        return (1 + math.cos(self.alititude_tilt_rad)) / 2



def main(window_geometry, shading_geometry, hoy, year, ladybug_location):
    # Check that window and shading are planar surfaces

    # Get location data
    lat,long,utc_offset = extract_location_data(ladybug_location)
    
    # Calculate sun position
    sun_alt,sun_az = calc_sun_position(lat,long,year,hoy,utc_offset)
    sun_vec = sunvec(sun_alt,sun_az)

    if is_sunny(window_geometry,sun_vec): 
        return unshaded_window_surfaces(window_geometry,shading_geometry,sun_vec)
    
    else:
        return None


"""
Pseudo code

# initialize window
Window = RadiationWindow(window_geometry, 
                         context_geometry, 
                         location,
                         glass_solar_transmittance, 
                         glass_light_transmittance)

solar_gains = []
illuminance = []

for ii in range(0,len(dhi)):
    dhi = 
    dni = 
    hoy =
    
    sg = Window.calc_solar_gains(dni,dhi,hoy)
    ill = Window.calc_illuminance(dni,dhi,hoy)

    solar_gains.append(sg)
    illuminance.append(ill)

"""