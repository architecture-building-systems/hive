# coding=utf-8
"""
Monthly heating, cooling and electricity demand calculation according to SIA 380 with some modifications.
Equations from 'Interaktion Struktu, Klima und Wärmebedarf_HS20.xlsx', 19.10.2020

heating and cooling demand: SIA 380.1
thermal balance depends on individual surfaces, i.e. each building surface (both transparent and opaque) can have
individual proporties (U-value, g-value, infiltration, etc.)

temperature set-point must be an input, e.g. from an adaptive thermal comfort module or taken from the room properties (SIA 2024).

electricity demand: currently simply by using sqm and internal loads for lighting and equipment
"""

from __future__ import division
import math

import ghpythonlib.treehelpers as th

# Constants
MONTHS_PER_YEAR = 12
DAYS_PER_WEEK = 7
DAYS_PER_MONTH = [31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31]
DAYS_PER_YEAR = 365
HOURS_PER_DAY = 24
HOURS_PER_MONTH = [(HOURS_PER_DAY * days) for days in DAYS_PER_MONTH]   # length of calculation period (hours per month) [h]
HOURS_PER_YEAR = 8760  # no leap year    
SECONDS_PER_HOUR = 3600.0

def cleanDictForNaN(d):
    # a = d.values()
    # b = d.keys()
    for i in d:
        if isinstance(d[i],str): continue
        if math.isnan(d[i]) or d[i] == "NaN":
            d[i] = 0.0

    return d


def main(room_properties, room_schedules, floor_area, T_e, setpoints_ub, setpoints_lb, surface_areas, surface_type,
         srf_irrad_obstr_tree, srf_irrad_unobstr_tree, g_value, g_value_total, setpoint_shading, 
         run_obstructed_simulation, hourly, use_adaptive_comfort):
    '''
    Computes monthly heating, cooling and electricity demand for a thermal zone, based on SIA 380.1
    :param room_properties: room properties in json format
    :param room_schedules: room schedules in json format
    :param floor_area: Floor area of the room in m2
    :param T_e: Hourly ambient air temperature in degree Celsius
    :param setpoints_ub: Upper bound for temperature setpoints, hourly.
    :param setpoints_lb: Lower bound for temperature setpoints, hourly.
    :param surface_areas: building surface areas that are used for fabric gains/losses computation
    :param surface_type: indicator if this surface is transparent or not. if yes, it will be assigned the transparent construction from room properties. 'opaque' for opaque or 'transp' for transparent
    :param srf_irrad_obstr_tree: hourly solar irradiation in W/m2 per building surface. Jagged array [months_per_year][num_srfs] or [hours_per_year][num_srfs]. Must be in correct order with the next 2 parameters 'surface_areas' and 'surface_is_transparent'
    :param srf_irrad_unobstr_tree: hourly solar irradiation in W/m2 per building surface. Jagged array [months_per_year][num_srfs] or [hours_per_year][num_srfs]. Must be in correct order with the next 2 parameters 'surface_areas' and 'surface_is_transparent'
    :param g_value: G value of windows.
    :param g_value_total: G value total including sunscreen ('Sonnenschutz') of windows
    :param setpoint_shading: Shading setpoint for activating sunscreen of windows, in W/m^2
    :param run_obstructed_simulation: Boolean to indicate if an obstructed solar simulation is conducted. True if yes.
    :param hourly: Boolean to indicate if hourly values should be returned instead of monthly. True if yes.
    :param use_adaptive_comfort: Boolean to indicate if adaptive comfort should be used instead of fixed setpoints. True if yes. Defaults to yes if setpoints_ub and setpoints_lb are null.
   
    :return: Monthly or hourly cooling, heating and electricity loads for a thermal zone
    '''

    """
    List of equations
    
    Heizwärmebedarf (in Wh):
    Q_H = Q_T + Q_V - eta_g * (Q_i + Q_s)
        
    Ausnutzungsgrad für Wärmegewinne (in -):
    eta_g = (1 - gamma^a) / (1 - gamma^(a+1))
    a = 1 + tau / 15
    
    Wärmeeintrag/-verlust-Verhältnis (in -):
    gamma = (Q_i + Q_s) / (Q_T + Q_V)
    
    Transmissionswärmeverluste (in Wh):
    Q_T = H_T * (T_i - T_e) * t
    
    Transmissions-Wärmetransferkoeffizient (in W/K):
    H_T = A_op * U_op + A_w * U_w
    
    Interne Wärmeeinträge (in Wh):
    Q_i = Phi_P * t_P + Phi_L * t_L + Phi_A * t_A
    
    Lüftungswärmeverluste (in Wh):
    Q_V = H_V * (T_i - T_e) * t
    
    Lüftungs-Wärmetransferkoefizient (in W/K):
    H_V = Vdot_th * rho * c_p
    
    Thermisch wirksamer Aussenluftvolumenstrom (in m^3/h):
    Vdot_th = Vdot_e * (1 - eta_rec) + Vdot_inf
    """

    """
    Nomenclature:
    
    [Q_H] = Wh (Heizwärmebedarf)
    [Q_T] = Wh (Transmissionswärmeverluste)
    [Q_V] = Wh (Lüftungswärmeverluste)
    [Q_i] = Wh (interne Wärmeeinträge)
    [Q_s] = Wh (solare Wärmeeinträge)
    
    [H_V] = W/K (Lüftungs-Wärmetransferkoefizient) 
    [H_T] = W/K (Transmissions-Wärmetransferkoeffizient)
    
    [Phi_P] = W (Wärmeabgabe der Personen)
    [Phi_L] = W (Wärmeabgabe der Beleuchtung)
    [Phi_A] = W (Wärmeabgabe der Geräte)
    [t_P] = h (Volllaststunden Personen)
    [t_L] = h (Volllaststunden Beleuchtung)
    [t_A] = h (Volllaststunden Geräte)    
    
    [gamma] = - (Wärmeeintrag/-verlust-Verhältnis)
    [tau] = h (Zeitkonstante des Gebäudes)
    [t] = h (Länge der Berechnungsperiode)
        
    [Vdot_th] = m^3/h (Thermisch wirksamer Aussenluftvolumenstrom)
    [Vdot_e] = m^3/h (Aussenluftvolumenstrom durch Lüftung)
    [Vdot_inf] = m^3/h (Aussenluftvolumenstrom durch Infiltration)
    
    [eta_rec] = - (Nutzungsgrad der Wärmerückgewinnung) 
    [eta_g] = - (Ausnutzungsgrad für Wärmegewinne)      

    [rho] = kg/m^3 (Luftdichte)
    [c_p] = J/(kgK) (Spez. Wärmekapazität von Luft)
    
    [T_i] = K oder °C (Raumlufttemperatur)
    [T_e] = K oder °C (Aussenlufttemperatur)

    [A_op] = m^2 (opaque surface area) 
    [A_w] = m^2 (windows surface area)
    [U_op] = W/(m^2K) (U-value opaque surface)
    [U_w] = W/(m^2K) (U-value window surface)  
    """

    """
    SIA 2024 variables:

    tau     Zeitkonstante des Gebäudes [h]
    theta_e Aussenlufttemperatur
    theta_i Raumlufttemperatur
    t       Länge der Berechnungsperiode [h]
    A_th    Thermische Gebäudehüllfläche [m2] 
    A_w     Fensterfläche [m2]                      !!!!!! f_g in sia2024 - Glasanteil in [%]
    U_op    Wärmedurchgangskoeffizient Aussenwand [W/m2K]
    U_w     Wärmedurchgangskoeffizient Fenster [W/m2K]
    Vdot_e_spec  Aussenluft-Volumenstrom [m3/m2h]
    Vdot_inf_spec Aussenluft-Volumenstrom durch Infiltration [m3/m2h]
    eta_rec Nutzungsgrad der Wärmerückgewinnung [-]
    phi_P   Wärmeabgabe Personen [W/m2]
    phi_L   Wärmeabgabe Beleuchtung [W/m2]
    phi_A   Wärmeabgabe Geräte [W/m2]
    t_P     Vollaststunden Personen [h]
    t_L     Vollaststunden Beleuchtung [h]
    t_A     Vollaststunden Geräte [h]
    g       g-Wert [-]
    f_sh    Reduktionsfaktor solare Wärmeeinträge [-]
    I       description": "Solare Strahlung [Wh/m2]
    """

    """
    cases according to SIA 2024:2015.
    The norm defines following ROOM and/or BUILDING types (p.7-8):
        _____________________________________________________________________________________
              | abbr.     | description                                    |    code SIA 380
        ______|___________|________________________________________________|_________________
        - 1.1   mfh:        multi-family house (Mehrfamilienhaus)                   HNF1
        - 1.2   efh:        single family house (Einfamilienhaus)                   HNF1
        - 2.1   ...:        Hotelroom                                               HNF1
        - 2.2   ...:        Hotel lobby                                             HNF1
        - ...   ...:
        - 3.1   office:     small office space (Einzel,- Gruppenbüro)               HNF2
        - ...   ...:
        - 4.1   school:     school room (Schulzimmer)                               HNF5
        - ...   ...:
        _____________________________________________________________________________________
    """
    
    rho = 1.2       # Luftdichte in kg/m^3
    c_p = 1005      # Spez. Wärmekapazität Luft in J/(kgK)
   
    # Assert inputs hourly
    adaptive_comfort = use_adaptive_comfort and (setpoints_ub is not None and setpoints_lb is not None) 
    
    if adaptive_comfort:
        assert len(setpoints_ub) == HOURS_PER_YEAR, "'setpoints_ub' (Setpoints upper bound) must be hourly. Length was %d." % (len(setpoints_ub))
        assert len(setpoints_lb) == HOURS_PER_YEAR, "'setpoints_lb' (Setpoints lower bound) must be hourly. Length was %d." % (len(setpoints_lb))
    assert len(T_e) == HOURS_PER_YEAR, "'T_e' (Ambient temperature) must be hourly. Length was %d." % (len(T_e))
   
    # IF setpoints given monthly for hourly simulation, they stay the same for entire month regardless of T_e.
    if not hourly:
        if adaptive_comfort:
            setpoints_ub = get_monthly_avg(setpoints_ub)
            setpoints_lb = get_monthly_avg(setpoints_lb)
        T_e = get_monthly_avg(T_e)

    # read room properties from sia2024
    room_properties = cleanDictForNaN(room_properties)
    
    # f_sh = 0.9  # sia2024, p.12, 1.3.1.9 Reduktion solare Wärmeeinträge
    # g = room_properties["Gesamtenergiedurchlassgrad Verglasung"]
    tau = room_properties["Zeitkonstante"]
    U_value_opaque = room_properties["U-Wert opake Bauteile"]
    U_value_transparent = room_properties["U-Wert Fenster"]
    Vdot_e_spec = room_properties["Aussenluft-Volumenstrom (pro NGF)"]
    Vdot_inf_spec = room_properties["Aussenluft-Volumenstrom durch Infiltration"]
    eta_rec = room_properties["Temperatur-Aenderungsgrad der Waermerueckgewinnung"]
    Phi_P_per_m2 = room_properties["Waermeeintragsleistung Personen (bei 24.0 deg C, bzw. 70 W)"]
    Phi_L_per_m2 = room_properties["Waermeeintragsleistung der Raumbeleuchtung"]
    Phi_A_per_m2 = room_properties["Waermeeintragsleistung der Geraete"]

    # assign room properties to individual surfaces
    #    surface_type = ["opaque", "opaque", "transp", "transp"]
    #    surface_areas = [44.0, 62.3, 4.0, 5.2]
    num_surfaces = len(surface_type)
    
    # Average out the hours of occupancy, lighting, appliances
    P_total_hours = room_properties["Vollaststunden pro Jahr (Personen)"]
    L_total_hours = room_properties["Jaehrliche Vollaststunden der Raumbeleuchtung"]
    A_total_hours = room_properties["Jaehrliche Vollaststunden der Geraete"]
    
    if hourly:
        t_P, t_A, t_L, t_S = get_hourly_schedules(room_schedules)
        # Multiplier to adjust kWh/m2 based on how many yearly hours from SIA 2024 vs how many yearly hours from schedule generator
        # Ensures kWh/m2 always has 
        Phi_P_multiplier = P_total_hours / sum(t_P)
        Phi_L_multiplier = L_total_hours / sum(t_L)
        Phi_A_multiplier = A_total_hours / sum(t_A)
    else:
        Phi_P_multiplier = 1
        Phi_L_multiplier = 1
        Phi_A_multiplier = 1
        t_P = [P_total_hours] * MONTHS_PER_YEAR
        t_L = [L_total_hours] * MONTHS_PER_YEAR
        t_A = [A_total_hours] * MONTHS_PER_YEAR
        # transforming daily sia2024 data to monthly
        DAYS_PER_YEAR_float = float(DAYS_PER_YEAR)
        for i in range(MONTHS_PER_YEAR):
            t_P[i] *= DAYS_PER_MONTH[i] / DAYS_PER_YEAR_float
            t_L[i] *= DAYS_PER_MONTH[i] / DAYS_PER_YEAR_float
            t_A[i] *= DAYS_PER_MONTH[i] / DAYS_PER_YEAR_float

    time_range = HOURS_PER_YEAR if hourly else MONTHS_PER_YEAR  
    
    # Use fixed setpoints / setbacks from room properties if no adaptive comfort
    if not adaptive_comfort:
        setpoint_ub = room_properties["Raumlufttemperatur Auslegung Kuehlung (Sommer)"]
        setpoint_lb = room_properties["Raumlufttemperatur Auslegung Heizen (Winter)"]
        
        if hourly:
            setpoints_ub = []
            setpoints_lb = []
            setback_ub = room_properties["Raumlufttemperatur Auslegung Kuehlung (Sommer) - Absenktemperatur"]
            setback_lb = room_properties["Raumlufttemperatur Auslegung Heizen (Winter) - Absenktemperatur"]
            
            for day in range(DAYS_PER_YEAR):
                for hour in range(HOURS_PER_DAY):
                    if t_S[hour] == 1:
                        setpoints_ub.append(setpoint_ub)
                        setpoints_lb.append(setpoint_lb)
                    elif t_S[hour] == 0.5:
                        setpoints_ub.append(setback_ub)
                        setpoints_lb.append(setback_lb)
                    else:
                        # TODO should be NaN? For now keep setpoint
                        setpoints_ub.append(setpoint_ub)
                        setpoints_lb.append(setpoint_lb)
        else:
            setpoints_ub = [setpoint_ub] * MONTHS_PER_YEAR
            setpoints_lb = [setpoint_lb] * MONTHS_PER_YEAR
            
    # formatting the grasshopper tree that contains solar irradiation time series for each window
    # could be changed later to also include solar irradiation on opaque surfaces...
    # ...would need to be adapted in the 'for surface in range(num_surfaces):' loop as well then
    win_areas = [x for (x, y) in zip(surface_areas, surface_type) if y != "opaque"]
    Q_s_tr_per_surface = None
    
    if (srf_irrad_obstr_tree.Branch(0).Count == 0 and srf_irrad_unobstr_tree.BranchCount == 0):
        num_surfaces_tr = len([s for s in surface_type if s=="transp"])
        Q_s_tr_per_surface = [[0.0]*num_surfaces_tr] * time_range
    else:
        Q_s_tr_per_surface_jagged = calculate_Q_s(run_obstructed_simulation, srf_irrad_obstr_tree, srf_irrad_unobstr_tree,
                                            g_value, g_value_total, 
                                            setpoint_shading, win_areas,
                                            hourly=hourly)
        
        # Transpose to per timestep
        Q_s_tr_per_surface = transpose_jagged_2D_array(Q_s_tr_per_surface_jagged)
    
    # calculations from Illias Excel sheet:
    # preAllocate arrays. its a bit faster than .append (https://levelup.gitconnected.com/faster-lists-in-python-4c4287502f0a)
    Q_i = [0.0] * time_range
    Q_s = [0.0] * time_range
    Q_i_out = [0.0] * time_range
    Q_s_out = [0.0] * time_range
    Q_V_out = [0.0] * time_range
    Q_T_out = [0.0] * time_range
    Q_T_opaque_out = [0.0] * time_range
    Q_T_transparent_out = [0.0] * time_range

    Q_Heat = [0.0] * time_range
    Q_Cool = [0.0] * time_range
    Q_Elec = [0.0] * time_range

    Phi_P = Phi_P_per_m2 * floor_area * Phi_P_multiplier
    Phi_L = Phi_L_per_m2 * floor_area * Phi_L_multiplier
    Phi_A = Phi_A_per_m2 * floor_area * Phi_A_multiplier
            
    # External air flowrate (thermisch wirksamer Aussenluftvolumenstrom)
    Vdot_e = Vdot_e_spec * floor_area
    Vdot_inf = Vdot_inf_spec * floor_area
    Vdot_th = Vdot_e * (1.0 - eta_rec) + Vdot_inf
    Vdot_th_no_heat_recovery = Vdot_e + Vdot_inf

    # Ventilation heat loss coefficient (Lüftungs-Wärmetransferkoeffizient), H_V
    # m3/h to m3/s, hence divide by 3600
    H_V = Vdot_th / SECONDS_PER_HOUR * rho * c_p
    H_V_no_heat_recovery = Vdot_th_no_heat_recovery / SECONDS_PER_HOUR * rho * c_p

    # For each month, compute gains and losses of the zone
    # for some variables, we compute upper and lower bounds (subscripts ub & lb)
    # furthermore, cooling and heating demand requires us to compute certain variables differently.
    # See equation sheet 'EK1_Formelsammlung_HS20.pdf'
    for t in range(HOURS_PER_YEAR if hourly else MONTHS_PER_YEAR):
        num_timesteps = 1 if hourly else HOURS_PER_MONTH[t]
        
        T_i_ub = setpoints_ub[t]
        T_i_lb = setpoints_lb[t]

        # Ventilation losses (Lüftungswärmeverluste)
        # Q_V = H_V * (T_i - T_e) * t
        # we compute with and without heat recovery, ub and lb, and take the respectively best (lowest demand)
        # this assumes, the ventilation system operates ideally and does not, for example, keep warm air in summer
        Q_V_ub = H_V * (T_i_ub - T_e[t]) * num_timesteps
        Q_V_lb = H_V * (T_i_lb - T_e[t]) * num_timesteps
        Q_V_ub_no_hr = H_V_no_heat_recovery * (T_i_ub - T_e[t]) * num_timesteps
        Q_V_lb_no_hr = H_V_no_heat_recovery * (T_i_lb - T_e[t]) * num_timesteps

        # Internal loads (interne Wärmeeinträge)
        # Q_i = Phi_P * t_P + Phi_L * t_L + Phi_A * t_A
        Q_i[t] = Phi_P * t_P[t] + Phi_L * t_L[t] + Phi_A * t_A[t]

        # Transmission heat losses, Q_T, both transparent and opaque
        Q_T_ub = 0.0
        Q_T_lb = 0.0
        Q_T_op_ub = 0.0
        Q_T_op_lb = 0.0
        Q_T_tr_ub = 0.0
        Q_T_tr_lb = 0.0
        
        deltaT_ub = (T_i_ub - T_e[t]) * num_timesteps
        deltaT_lb = (T_i_lb - T_e[t]) * num_timesteps
            
        for surface in range(num_surfaces):
            # Transmission heat transfer coefficient (Transmissions-Wärmetransferkoeffizient), H_T      
            if surface_type[surface] == "opaque":
                H_T = surface_areas[surface] * U_value_opaque
                Q_T_op_ub += H_T * deltaT_ub
                Q_T_op_lb += H_T * deltaT_lb
            else:
                H_T = surface_areas[surface] * U_value_transparent
                Q_T_tr_ub += H_T * deltaT_ub
                Q_T_tr_lb += H_T * deltaT_lb

        # Transmission losses (Transmissionswärmeverluste), Q_T, upper and lower bounds for all surfaces
        Q_T_ub = Q_T_tr_ub + Q_T_op_ub
        Q_T_lb = Q_T_tr_lb + Q_T_op_lb
        
        # solar gains (solare Wärmeeinträge), Q_s, (PER SURFACE)
        # unobstructed or obstructed, both using SolarModel.dll and GHSolar.gha
        Q_s_tr = sum(Q_s_tr_per_surface[t])
        Q_s[t] = Q_s_tr   # currently, only transparent surfaces

        # Heatgains/-losses ratio (Wärmeeintrag/-verlust Verhältnis), gamma
        # calculating for different cases, heating / cooling, upper / lower bounds, with / without heat recovery
        gamma_ub = calc_gamma(Q_i[t], Q_s[t], Q_T_ub, Q_V_ub)
        gamma_lb = calc_gamma(Q_i[t], Q_s[t], Q_T_lb, Q_V_lb)
        gamma_ub_no_hr = calc_gamma(Q_i[t], Q_s[t], Q_T_ub, Q_V_ub_no_hr)
        gamma_lb_no_hr = calc_gamma(Q_i[t], Q_s[t], Q_T_lb, Q_V_lb_no_hr)

        # usage of heat gains (Ausnutzungsgrad für Wärmegewinne), eta_g
        eta_g_heating = calc_eta_g(gamma_lb, tau)
        eta_g_cooling = calc_eta_g(gamma_ub, tau, cooling=True) 
        eta_g_heating_no_hr = calc_eta_g(gamma_lb_no_hr, tau)
        eta_g_cooling_no_hr = calc_eta_g(gamma_ub_no_hr, tau, cooling=True)
 
        # heating demand (Heizwärmebedarf), Q_H
        # Q_H = Q_T + Q_V - eta_g * (Q_i + Q_s)
        # calculating different cases with/without heat recovery (hr)
        Q_H = Q_T_lb + Q_V_lb - eta_g_heating * (Q_i[t] + Q_s[t])
        Q_H_no_hr = Q_T_lb + Q_V_lb_no_hr - eta_g_heating_no_hr * (Q_i[t] + Q_s[t])
        Q_H, Q_H_no_hr = negatives_to_zero([Q_H, Q_H_no_hr])
        
        # cooling demand (Kältebedarf), Q_K
        # calculating different cases with/without heat recovery (hr)
        Q_K = Q_i[t] + Q_s[t] - eta_g_cooling * (Q_T_ub + Q_V_ub)
        Q_K_no_hr = Q_i[t] + Q_s[t] - eta_g_cooling_no_hr * (Q_T_ub + Q_V_ub_no_hr)
        Q_K, Q_K_no_hr = negatives_to_zero([Q_K, Q_K_no_hr])

        # take smaller value of both comfort set points and remember the index
        Q_H, Q_H_index = min_and_index(Q_H, Q_H_no_hr)
        Q_K, Q_K_index = min_and_index(Q_K, Q_K_no_hr)

        # either subtract heating from cooling, but then also account for that in losses/gains by subtracting those too
        # or just take the higher load of both and then take the corresponding losses/gains
        # demand = Q_K + Q_H  # sometimes we have both cooling and heating. so subtract one from another
        # if demand < 0:
        #     Q_Cool[month] = demand * -1
        #     Q_Heat[month] = 0
        # else:
        #     Q_Cool[month] = 0
        #     Q_Heat[month] = demand
        if Q_K > Q_H: # cooling
            # make cooling negative
            Q_Cool[t] = -Q_K
            Q_Heat[t] = 0.0
            
            with_hr = Q_K_index == 0
            eta_g = eta_g_cooling if with_hr else eta_g_cooling_no_hr
            
            Q_T_out[t] = Q_T_ub * eta_g
            Q_T_opaque_out[t] = Q_T_op_ub * eta_g
            Q_T_transparent_out[t] = Q_T_tr_ub * eta_g
            Q_V_out[t] = (Q_V_ub if with_hr else Q_V_ub_no_hr) * eta_g
            Q_i_out[t] = Q_i[t]
            Q_s_out[t] = Q_s[t]
        else: # heating
            Q_Cool[t] = 0.0
            Q_Heat[t] = Q_H
            
            with_hr = Q_H_index == 0
            eta_g = eta_g_heating if with_hr else eta_g_heating_no_hr

            Q_T_out[t] = Q_T_lb
            Q_T_opaque_out[t] = Q_T_op_lb
            Q_T_transparent_out[t] = Q_T_tr_lb
            Q_V_out[t] = (Q_V_lb if with_hr else Q_V_lb_no_hr)
            Q_i_out[t] = Q_i[t] * eta_g
            Q_s_out[t] = Q_s[t] * eta_g
            
        # TODO lighting and utility loads. simplification, because utility and lighting have efficiencies (inefficiencies are heat loads). 
        # Need efficiency or electricity use separately
        Q_Elec[t] = Phi_L * t_L[t] + Phi_A * t_A[t]   


    if Q_s_tr_per_surface_jagged != None:
        # Q_s_tr_tree = th.list_to_tree(Q_s_tr_per_surface_jagged, source=[0, 0])   # import ghpythonlib.treehelpers as th
        Q_s_tr_tree = Q_s_tr_per_surface_jagged # DEBUG
    else:
        Q_s_tr_tree = None

    return toKwh(Q_Heat), toKwh(Q_Cool), toKwh(Q_Elec), \
           toKwh(Q_T_out), toKwh(Q_V_out), toKwh(Q_i_out), \
           toKwh(Q_s_out), toKwh(Q_T_opaque_out), toKwh(Q_T_transparent_out), \
           Q_s_tr_tree
           
def toKwh(Q):
    return [x / 1000.0 for x in Q]

def min_and_index(*sequence):
    min_value = min(sequence)
    index = sequence.index(min_value)
    return min_value, index

def negatives_to_zero(values):
    return [0.0 if v < 0.0 else v for v in values]

def calc_gamma(Q_i, Q_s, Q_T, Q_V):
    """
    Wärmeeintrag/-verlust Verhältnis. Ratio of gains and losses of a zone
    :param Q_i: internal gains
    :param Q_s: solar gains
    :param Q_T: transmission heat losses
    :param Q_V: ventilation heat losses
    :return: float. ratio of gains and losses
    """
    return (Q_i + Q_s) / (Q_T + Q_V)


# usage of heat gains (Ausnutzungsgrad für Wärmegewinne), eta_g
def calc_eta_g(gamma, tau, cooling=False):
    """
    Ausnutzungsgrad für Wärmegewinne/-verluste. A variable that describes thermal losses/gains of a zone
    :param gamma: Wärmeeintrag/-verlust Verhältnis. Ratio between gains and losses (Q_i + Q_s) / (Q_T + Q_V)
    :param tau: Zeitkonstante. Time constant of the zone, describing thermal latency (C * A) / ( H_T + H_V). Or coefficients from SIA 2024
    :param cooling: Boolean to indicate whether this function should return eta_g for the cooling case. Default is False
    :return: float. Value for the degree of thermal losses/gains of a zone
    """
    if gamma < 0.0:
        eta_g = 1.0
    elif gamma == 1.0:
        a = 1.0 + tau / 15.0
        if cooling:
            eta_g = a / (a + 1.0)
        else:
            eta_g = a / (a + 1.0)
    else:
        a = 1.0 + tau / 15.0
        if cooling:
            eta_g = (1.0 - gamma ** (-a)) / (1.0 - gamma ** (-(a + 1.0)))
        else:
            eta_g = (1.0 - gamma ** a) / (1.0 - gamma ** (a + 1.0))
    return eta_g


def calculate_Q_s(run_obstr, tree_obstr, tree_unobstr,
                       g_value, g_value_total,
                       setpoint,
                       win_areas,
                       hourly=False): 
    tree = tree_obstr
    if run_obstr == False:
        tree = tree_unobstr
    
    Q_array = []
    for i in range(tree.BranchCount):
        row = []
        win_area = win_areas[i] 
        branch = tree.Branch(i)
        # for j in range(len(branch)): # DEBUG
        for j in range(branch.Count):
            irrad = branch[j] / win_area   # calculating per W/m2 for shading control
            if irrad > setpoint:
                irrad *= g_value_total
            else:
                irrad *= g_value
            row.append(irrad * win_area)    # calculating back to total irradiance of entire surface
        
        if hourly:
            Q_array.append(row)
        else:
            Q_array.append(get_monthly_sum(row))
    
    return Q_array

# UTILS

def get_monthly_sum(hourly):
    monthly = []
    for month in range(MONTHS_PER_YEAR):
        start = int(HOURS_PER_DAY * sum(DAYS_PER_MONTH[0:month]))
        end = int(HOURS_PER_DAY * sum(DAYS_PER_MONTH[0:month + 1]))
        monthly.append(sum(hourly[start:end]))
    return monthly

def get_monthly_avg(hourly):
    monthly = []
    for month in range(MONTHS_PER_YEAR):
        start = int(HOURS_PER_DAY * sum(DAYS_PER_MONTH[0:month]))
        end = int(HOURS_PER_DAY * sum(DAYS_PER_MONTH[0:month + 1]))
        monthly.append(sum(hourly[start:end]) / (end-start))
    return monthly

def transpose_jagged_2D_array(array):
    transposed_array = []
    len_d1 = len(array)
    len_d2 = len(array[0])

    for i in range(len_d2):
        transposed_row = []
        for j in range(len_d1):
            transposed_row.append(array[j][i])
        transposed_array.append(transposed_row)

    return transposed_array

def get_hourly_schedules(room_schedules): 
    """
    Converts the encoded schedules for occupancy, devices, lighting, and setpoints into hourly schedules.
    :param: The schedules JSON.
    :returns: Hourly schedules for occupancy (P), devices (A), lighting (L), and setpoints (S)
    """ 
    P_schedule = room_schedules['OccupancySchedule']
    A_schedule = room_schedules['DeviceSchedule']
    L_schedule = room_schedules['LightingSchedule']
    S_schedule = room_schedules['SetpointSchedule']

    YEAR_INTIAL_WEEKDAY = 0  # indexed on 1
    # HOUR_DAY_START = 8 # inclusive, from SIA 2024 corrected from 7h
    # HOUR_DAY_END = 19 # exclusive, from SIA 2024 corrected from 18h

    # Other asserts are captured in schema validation
    assert 365 - room_schedules['DaysOffPerWeek'] * 52 == room_schedules['DaysUsedPerYear']

    # def get_daily_lighting_occupied():
    #     # light hours = occupancy hours when light hours > 0
    #     n_day = L_schedule['HoursPerDay']
    #     n_night = L_schedule['HoursPerNight']

    #     # Night hours (when lighting is activated during night)
    #     # are filled first from 19h to 24h, 
    #     # and, if extra, starting from 7h and moving backwards to 0h
    #     # The index tracks until when in the evening the night hours
    #     idx_night_start = HOURS_PER_DAY - 1 \
    #         if n_night > HOURS_PER_DAY - HOUR_DAY_END \
    #         else HOUR_DAY_END + n_night - 1
        
    #     L_daily_occupied = [0.0] * 24
    #     for hour, occupancy in reversed(list(enumerate(P_schedule['DailyProfile']))):
    #         if occupancy > 0.0: # if occupied
    #             if hour >= HOUR_DAY_START - 1 and hour < HOUR_DAY_END-1: # if during day
    #                 if n_day > 0: # if lighting hours left to allocate
    #                     L_daily_occupied[hour] = 1.0
    #                     n_day -= 1
    #             else: # during night
    #                 if n_night > 0 and hour < idx_night_start: # if lighting hours left to allocate
    #                     L_daily_occupied[hour] = 1.0
    #                     n_night -= 1
    #     return L_daily_occupied
    
    P_daily_occupied = P_schedule['DailyProfile']
    A_daily_occupied = A_schedule['DailyProfile']
    L_daily_occupied = L_schedule['DailyProfile']
    S_daily_occupied = S_schedule['DailyProfile']
    
    P_daily_unoccupied  = [P_schedule['Default']] * HOURS_PER_DAY
    A_daily_unoccupied  = [A_schedule['Default']] * HOURS_PER_DAY
    L_daily_unoccupied  = [L_schedule['Default']] * HOURS_PER_DAY
    S_daily_unoccupied  = [S_schedule['Default']] * HOURS_PER_DAY
    
    weekdays_on = DAYS_PER_WEEK - room_schedules['DaysOffPerWeek']
    weekday = YEAR_INTIAL_WEEKDAY
    # TODO assert props, expand daily profiles and yearly to hourly timeseries
    P_hourly = []
    A_hourly = []
    L_hourly = []
    S_hourly = []
    
    for month, month_days in enumerate(DAYS_PER_MONTH):
        # TODO assumes days off / holidays all at once rather 
        # than peppered through month. More appropriate for schools / summer / winter
        # but not really for national holidays...
        days_on = int(month_days * room_schedules['YearlyProfile'][month])

        for day in range(month_days):
            if weekday == DAYS_PER_WEEK:
                weekday = 0
            skip = weekday >= weekdays_on or day > days_on

            if skip: # unoccupied
                P_hourly.extend(P_daily_unoccupied)
                A_hourly.extend(A_daily_unoccupied)
                L_hourly.extend(L_daily_unoccupied) 
                S_hourly.extend(S_daily_unoccupied)
            else: # occupied
                P_hourly.extend(P_daily_occupied)
                A_hourly.extend(A_daily_occupied)
                L_hourly.extend(L_daily_occupied)      
                S_hourly.extend(S_daily_occupied)

            weekday += 1


    return P_hourly, A_hourly, L_hourly, S_hourly        


if __name__ == "__main__":
    class Jagged:
        def __init__(self, data):
            self.data = data

    def test():
        room_properties = {
            "description": "1.2 Wohnen Einfamilienhaus",
            "Raumlufttemperatur Auslegung Heizen (Winter)": 21.0,
            "Raumlufttemperatur Auslegung Heizen (Winter) - Absenktemperatur": 19.0,
            "Nettogeschossflaeche": 20.0,
            "Thermische Gebaeudehuellflaeche": 38.0,
            "U-Wert Fenster": 1.2,
            "Waermeeintragsleistung Personen (bei 24.0 deg C, bzw. 70 W)": 1.4,
            "Jaehrliche Vollaststunden der Geraete": 1780.0,
            "U-Wert opake Bauteile": 0.2,
            "Jaehrliche Vollaststunden der Raumbeleuchtung": 1450.0,
            "Vollaststunden pro Jahr (Personen)": 4090.0,
            "Abminderungsfaktor fuer Fensterrahmen": 0.75,
            "Aussenluft-Volumenstrom durch Infiltration": 0.15,
            "Raumlufttemperatur Auslegung Kuehlung (Sommer)": 26.0,
            "Raumlufttemperatur Auslegung Kuehlung (Sommer) - Absenktemperatur": 28.0,
            "Glasanteil": 30.0,
            "Zeitkonstante": 164.0,
            "Waermeeintragsleistung der Raumbeleuchtung": 2.7,
            "Waermeeintragsleistung der Geraete": 8.0,
            "Gesamtenergiedurchlassgrad Verglasung": 0.5,
            "Temperatur-Aenderungsgrad der Waermerueckgewinnung": 0.7,
            "Aussenluft-Volumenstrom (pro NGF)": 0.6
        }
        
        room_schedules = {
            "RoomType": "1.1 Wohnen Mehrfamilienhaus", 
            "YearlyProfile": [
                0.8, 
                0.8, 
                0.8, 
                0.8, 
                0.8, 
                0.8, 
                0.8, 
                0.8, 
                0.8, 
                0.8, 
                0.8, 
                0.8
            ], 
            "DaysOffPerWeek": 0, 
            "DaysUsedPerYear": 365,
            "LightingSchedule": {
                "DailyProfile": [
                    1.0, 
                    1.0, 
                    1.0, 
                    1.0, 
                    1.0, 
                    1.0, 
                    0.6, 
                    0.4, 
                    0, 
                    0, 
                    0, 
                    0, 
                    0.8, 
                    0.4, 
                    0, 
                    0, 
                    0, 
                    0.4, 
                    0.8, 
                    0.8, 
                    0.8, 
                    1.0, 
                    1.0, 
                    1.0
                ], 
                "Default": 0.0
            }, 
            "OccupancySchedule": {
                "DailyProfile": [
                    1.0, 
                    1.0, 
                    1.0, 
                    1.0, 
                    1.0, 
                    1.0, 
                    0.6, 
                    0.4, 
                    0, 
                    0, 
                    0, 
                    0, 
                    0.8, 
                    0.4, 
                    0, 
                    0, 
                    0, 
                    0.4, 
                    0.8, 
                    0.8, 
                    0.8, 
                    1.0, 
                    1.0, 
                    1.0
                ], 
                "Default": 0.0
            }, 
            "DeviceSchedule": {
                "DailyProfile": [
                    0.1, 
                    0.1, 
                    0.1, 
                    0.1, 
                    0.1, 
                    0.2, 
                    0.8, 
                    0.2, 
                    0.1, 
                    0.1, 
                    0.1, 
                    0.1, 
                    0.8, 
                    0.2, 
                    0.1, 
                    0.1, 
                    0.1, 
                    0.2, 
                    0.8, 
                    1.0, 
                    0.2, 
                    0.2, 
                    0.2, 
                    0.1
                ], 
                "Default": 0.1
            },
             "SetpointSchedule": {
                "DailyProfile": [
                    0.5,
                    0.5,
                    0.5,
                    0.5,
                    0.5,
                    0.5,
                    1.0,
                    1.0,
                    1.0,
                    1.0,
                    1.0,
                    1.0,
                    1.0,
                    1.0,
                    1.0,
                    1.0,
                    1.0,
                    1.0,
                    1.0,
                    1.0,
                    1.0,
                    0.5,
                    0.5,
                    0.5 
                ], 
                "Default": 0.0
            }
        }

        floor_area = 200.0
        T_e = [0.416398, 1.714286, 6.138038, 8.964167, 14.281048, 17.462361, 18.399328, 18.784946, 13.954167, 9.874059, 3.974583, 1.593683]
        T_i = [21.0] * 12
        setpoints_ub = [20,21,22,23,24,25,25,24,23,22,21,20]
        setpoints_lb = [18] * 12
        surface_areas = [30, 30, 3.0, 3.0, 3.0, 536.124969 - 69]
        surface_type = ["transp", "transp", "transp", "transp", "transp", "opaque"]
        Q_s_per_surface = [0.0] * len(surface_type)
        Q_s_per_surface[0] = [15.93572, 28.137958, 52.534591, 70.864124, 97.429731, 100.659248, 110.715495, 89.630934, 64.212227, 38.79425, 19.025089, 11.624501]
        Q_s_per_surface[1] = [23.174573, 39.025397, 68.999793, 88.159866, 101.53745, 109.179217, 119.64447, 98.66428, 77.103732, 44.753735, 21.723197, 17.185115]
        Q_s_per_surface[2] = [11.164155, 18.686334, 29.798874, 45.721346, 57.51364, 65.652511, 66.630836, 51.430892, 35.616327, 23.692149, 12.46553, 9.476936]
        Q_s_per_surface[3] = [46.316597, 61.478404, 90.694507, 87.846535, 91.278904, 83.993872, 93.773866, 97.520832, 92.037561, 70.833123, 42.180446, 29.584221]
        Q_s_per_surface[4] = [22.257355, 38.413927, 72.508876, 100.603912, 138.930607, 144.043764, 155.043357, 126.633178, 88.618052, 52.984679, 25.594113, 16.571932]
        Q_s_per_surface[5] = [0.0] * 12
        for i in range(len(Q_s_per_surface)):
            Q_s_per_surface[i] = [x * 1000 for x in Q_s_per_surface[i]]  # converting from kWh/m2 into Wh/m2
        # Q_s_per_surface = list(map(list, zip(*Q_s_per_surface)))  # transposing
        # jaggeddata = Jagged(Q_s_per_surface)
        import datatree as dt
        srf_irrad_obstr_tree = dt.DataTree([Q_s_per_surface[5]])
        srf_irrad_unobstr_tree = dt.DataTree(Q_s_per_surface[0:5])
        # jaggeddata = dt.DataTree(Q_s_per_surface)
        g_value = 0.5
        g_value_total = 0.14
        setpoint_shading = 200 # W/m2
        
        
        # ONLY FOR DEBUG hallucinate hourly T_e, setpoints
        if (__debug__):
            import datatree as dt
            srf_irrad_obstr_tree_tmp = dt.DataTree([[]]*srf_irrad_obstr_tree.BranchCount)
            srf_irrad_unobstr_tree_tmp = dt.DataTree([[]]*srf_irrad_unobstr_tree.BranchCount)
            
            setpoints_lb_tmp = []
            setpoints_ub_tmp = []
            T_e_tmp = []
            bs_multiplier = list(range(-6,6))+list(range(-6,6))[::-1]
            
            for i in range(MONTHS_PER_YEAR):
                setpoints_lb_day = [setpoints_lb[i] + a*0.1 for a in bs_multiplier]
                setpoints_ub_day = [setpoints_ub[i] + a*0.1 for a in bs_multiplier]
                T_e_day = [T_e[i] + a for a in bs_multiplier]
                
                srf_irrad_obstr_tree_day = [[srf_irrad_obstr_tree.Branch(j)[i]/DAYS_PER_MONTH[i] + 100*a for a in bs_multiplier] for j in range(srf_irrad_obstr_tree_tmp.BranchCount)]
                srf_irrad_unobstr_tree_day = [[srf_irrad_unobstr_tree.Branch(j)[i]/DAYS_PER_MONTH[i] + 100*a for a in bs_multiplier] for j in range(srf_irrad_unobstr_tree_tmp.BranchCount)]
                for _ in range(DAYS_PER_MONTH[i]):
                    setpoints_lb_tmp.extend(setpoints_lb_day)
                    setpoints_ub_tmp.extend(setpoints_ub_day)
                    T_e_tmp.extend(T_e_day)
                    
                    for i, b in enumerate(srf_irrad_obstr_tree_tmp.Branches):
                        b.extend(srf_irrad_obstr_tree_day[i])
                    for i, b in enumerate(srf_irrad_unobstr_tree_tmp.Branches):
                        b.extend(srf_irrad_unobstr_tree_day[i])
            setpoints_lb = setpoints_lb_tmp
            setpoints_ub = setpoints_ub_tmp
            T_e = T_e_tmp
            
            srf_irrad_obstr_tree = srf_irrad_obstr_tree_tmp
            srf_irrad_unobstr_tree = srf_irrad_unobstr_tree_tmp
            del T_e_tmp, setpoints_lb_tmp, setpoints_ub_tmp
            del srf_irrad_obstr_tree_tmp
            del srf_irrad_unobstr_tree_tmp
        

        [Q_Heat, Q_Cool, Q_Elec, Q_T, Q_V, Q_i, Q_s, _, _, _] = main(room_properties, room_schedules, floor_area,
                                                                  T_e, 
                                                                  setpoints_ub, setpoints_lb, 
                                                                  surface_areas, surface_type, 
                                                                  srf_irrad_obstr_tree, srf_irrad_unobstr_tree,
                                                                  g_value, g_value_total,
                                                                  setpoint_shading,
                                                                  False, hourly=True, use_adaptive_comfort=False)
        print(Q_Heat)
        print(Q_Cool)
        print(Q_Elec)
        print(Q_T)
        print(Q_V)
        print(Q_i)
        print(Q_s)


    test()
