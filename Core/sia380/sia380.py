# coding=utf-8
"""
Monthly heating, cooling and electricity demand calculation according to SIA 380.
Variables names according to SIA 380.

heating and cooling demand: SIA 380.1
thermal balance depends on individual surfaces, i.e. each building surface (both transparent and opaque) can have
individual proporties (U-value, g-value, infiltration, etc.)

temperature set-point determined according to adaptive thermal comfort ... (i.e. function of ambient air temperature)

electricity demand: currently simply by using sqm and internal loads for lighting and equipment
"""

from __future__ import division
import math


def main(room_properties, ambient_temperature, solar_irradiation, surface_areas, surface_is_transparent):
    '''
    Computes monthly heating, cooling and electricity demand for a thermal zone, based on SIA 380.1
    :param room_properties: room properties in json format
    :param ambient_temperature: monthly average ambient air temperature in degree Celsius
    :param solar_irradiation: monthly solar irradiation in W/m2 per building surface. must be in correct order with the next 2 parameters 'surface_areas' and 'surface_is_transparent'
    :param surface_areas: building surface areas that are used for fabric gains/losses computation
    :param surface_is_transparent: indicator if this surface is transparent or not. if yes, it will be assigned the transparent construction from room properties
    :return: Monthly cooling, heating and electricity loads for a thermal zone
    '''

    rho = 1.2       # Luftdichte in kg/m^3
    c_p = 1005      # Spez. Wärmekapazität Luft in J/(kgK)
    hours_per_day = 24
    months_per_year = 12
    days_per_year = 365
    days_per_month = [31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31]
    t = [(hours_per_day * i) for i in days_per_month]

    # READ INPUTS
    # needs to read in zone class from Hive.IO
    # because it contains building elements with properties
    surface_type = ["op", "op", "transp", "transp"]
    surface_area = [44.0, 62.3, 4.0, 5.2]
    num_surfaces = len(surface_type)
    U_values = [0.4, 0.4, 1.2, 1.2]
    g_values = [0.0, 0.0, 0.5, 0.5]
    A = 200                 # floor area (GEOMETRY)
    Vdot_e_spec = 0.6       # Aussenluftvolumenstrom in m^3/h per m^2 (SIA 2024)
    Vdot_inf_spec = 0.15    # Infiltration in m^3/h per m^2 (SIA 2024)
    eta_rec = 0             # Wärmerückgewinnung (SIA 2024)
    t_P = 100.0             # Volllaststunden Personen entire year (SIA 2024)
    t_L = 100.0             # Volllaststunden Beleuchtung entire year (SIA 2024)
    t_A = 100.0             # Volllaststunden Geräte entire year (SIA 2024)

    # Compute adaptive temperature set point and average ambient temperature
    T_i = adaptive_comfort(ambient_temperature, days_per_month)
    T_e = average_temperature(ambient_temperature, days_per_month)

    # calculations from Illias Excel sheet:
    # preAllocate arrays. its a bit faster than append (https://levelup.gitconnected.com/faster-lists-in-python-4c4287502f0a)
    Q_i = [0.0] * months_per_year
    Q_s = [0.0] * months_per_year
    Q_V = [0.0] * months_per_year
    Q_T = [0.0] * months_per_year

    # For each month, compute:
    for month in range(months_per_year):
        # pre-allocate jagged arrays
        Q_s[month] = [0.0] * num_surfaces
        Q_T[month] = [0.0] * num_surfaces

        """ 
        External air flowrate (thermisch wirksamer Aussenluftvolumenstrom)
        # Vdot_th = Vdot_e * (1 - eta_rec) * Vdot_inf
        # [Vdot_th] = m^3/h
        # [Vdot_e] = m^3/h (Aussenluftvolumenstrom durch Lüftung)
        # [Vdot_inf] = m^3/h (Aussenluftvolumenstrom durch Infiltration)
        # [eta_rec] = - (Nutzungsgrad der Wärmerückgewinnung)
        """
        Vdot_e = Vdot_e_spec * A
        Vdot_inf = Vdot_inf_spec * A
        Vdot_th = Vdot_e * (1 - eta_rec) * Vdot_inf

        """
        Ventilation heat loss coefficient (Lüftungs-Wärmetransferkoeffizient), H_V
        H_V = Vdot_th * rho * c_p
        [H_V] = W/K
        [Vdot_th] = m^3/s
        [rho] = kg/m^3
        [c_p] = J/(kgK)
        """
        H_V = Vdot_th * rho * c_p

        """ 
        Ventilation losses (Lüftungswärmeverluste), Q_V
        Q_V = H_V * (T_i - T_e) * t
        [Q_V] = Wh
        [H_V] = W/K
        [T_i] = K oder °C
        [T_e] = K oder °C
        [t] = h (Länge der Berechnungsperiode)
        """
        Q_V[month] = H_V * (T_i[month] - T_e[month]) * t[month]

        """ 
        Internal loads (interne Wärmeeinträge)
        # Q_i = Phi_P * t_P + Phi_L * t_L + Phi_A * t_A
        # [Q_i] in Wh
        # [Phi_P] = W (Wärmeabgabe der Personen)
        # [Phi_L] = W (Wärmeabgabe der Beleuchtung)
        # [Phi_A] = W (Wärmeabgabe der Geräte)
        # [t_P] = h (Volllaststunden Personen)
        # [t_L] = h (Volllaststunden Beleuchtung)
        # [t_A] = h (Volllaststunden Geräte)
        """
        Phi_P = Phi_P_spec * A
        Phi_L = Phi_L_spec * A
        Phi_A = Phi_A_spec * A
        t_P_month = t_P * days_per_month[month] / days_per_year
        t_L_month = t_L * days_per_month[month] / days_per_year
        t_A_month = t_A * days_per_month[month] / days_per_year
        Q_i[month] = Phi_P * t_P_month + Phi_L * t_L_month + Phi_A * t_A_month

        for surface in range(num_surfaces):
            # solar gains (solare Wärmeeinträge), Q_s, (PER SURFACE)
            # unobstructed or obstructed, both using SolarModel.dll and GHSolar.gha

            """ 
            Transmission heat transfer coefficient (Transmissions-Wärmetransferkoeffizient), H_T, (PER SURFACE)
            H_T = A_op * U_op + A_w * U_w
            [H_T] in W/K
            [A_op] in m^2 (opaque surface area) 
            [A_w] in m^2 (windows surface area)
            [U_op] in W/(m^2K) (U-value opaque surface)
            [U_w] in W/(m^2K) (U-value window surface)
            """

            """
            Transmission losses (Transmissionswärmeverluste), Q_T, (PER SURFACE, because function of H_T)
            Q_T = H_T * (T_i + T_e) * t
            [Q_T] in Wh
            [H_T] in W/K
            [T_i] in K or °C
            [T_e] in K or °C
            [t] in h
            """




        # Heatgains/-losses ratio (Wärmeeintrag/-verlust Verhältnis), gamma

        # usage of heat gains (Ausnutzungsgrad für Wärmegewinne), eta_g

        # heating demand (Heizwärmebedarf), Q_H
        # if negative, assign Q_H = 0 and set value to Q_K (Kühlenergiebedarf)

        # Strombedarf, Q_E

    return 0.0



