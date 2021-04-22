# coding=utf-8
"""
Monthly heating, cooling and electricity demand calculation according to SIA 380.
Equations from 'Interaktion Struktu, Klima und Wärmebedarf_HS20.xlsx', 19.10.2020

heating and cooling demand: SIA 380.1
thermal balance depends on individual surfaces, i.e. each building surface (both transparent and opaque) can have
individual proporties (U-value, g-value, infiltration, etc.)

temperature set-point must be an input, e.g. from an adaptive thermal comfort module

electricity demand: currently simply by using sqm and internal loads for lighting and equipment
"""

from __future__ import division
import math

import ghpythonlib.treehelpers as th

# Constants
MONTHS_PER_YEAR = 12
DAYS_PER_MONTH = [31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31]
DAYS_PER_YEAR = 365.0
HOURS_PER_DAY = 24
HOURS_PER_MONTH = [(HOURS_PER_DAY * days) for days in DAYS_PER_MONTH]   # length of calculation period (hours per month) [h]
HOURS_PER_YEAR = 8760  # no leap year    
SECONDS_PER_HOUR = 3600.0

def cleanDictForNaN(d):
    # a = d.values()
    # b = d.keys()
    for i in d:
        # if isinstance(d[i],str): continue
        if math.isnan(d[i]) or d[i] == "NaN":
            d[i] = 0.0

    return d


def main(room_properties, floor_area, T_e, setpoints_ub, setpoints_lb, surface_areas, surface_type,
         srf_irrad_obstr_tree, srf_irrad_unobstr_tree, g_value, g_value_total, setpoint_shading, run_obstructed_simulation, hourly):
    '''
    Computes monthly heating, cooling and electricity demand for a thermal zone, based on SIA 380.1
    :param room_properties: room properties in json format
    :param floor_area: Floor area of the room in m2
    :param T_e: monthly average or hourly ambient air temperature in degree Celsius
    :param setpoints_ub: Upper bound for temperature setpoints
    :param setpoints_lb: Lower bound for temperature setpoints
    :param surface_areas: building surface areas that are used for fabric gains/losses computation
    :param surface_type: indicator if this surface is transparent or not. if yes, it will be assigned the transparent construction from room properties. 'opaque' for opaque or 'transp' for transparent
    :param srf_irrad_obstr_tree: hourly solar irradiation in W/m2 per building surface. Jagged array [months_per_year][num_srfs] or [hours_per_year][num_srfs]. Must be in correct order with the next 2 parameters 'surface_areas' and 'surface_is_transparent'
    :param srf_irrad_unobstr_tree: hourly solar irradiation in W/m2 per building surface. Jagged array [months_per_year][num_srfs] or [hours_per_year][num_srfs]. Must be in correct order with the next 2 parameters 'surface_areas' and 'surface_is_transparent'
    :param g_value: G value of windows.
    :param g_value_total: G value total including sunscreen ('Sonnenschutz') of windows (TODO rename this param to g_value_shaded?)
    :param setpoint_shading: Shading setpoint for activating sunscreen of windows, in W/m^2
    :param run_obstructed_simulation: Boolean to indicate if an obstructed solar simulation is conducted. True if yes.
    :param hourly: Boolean to indicate if hourly values should be returned instead of monthly. True if yes.
   
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
   
    # IF setpoints given monthly for hourly simulation, they stay the same for entire month regardless of T_e.
    if hourly and len(setpoints_lb) == len(setpoints_ub) == MONTHS_PER_YEAR:  
        print("WARNING: The setpoints are provided monthly so will not vary on an hourly basis.")
        setpoints_ub_tmp = []
        setpoints_lb_tmp = []
        for i in range(MONTHS_PER_YEAR):
            setpoints_ub_tmp.extend([setpoints_ub[i]] * HOURS_PER_MONTH[i])
            setpoints_lb_tmp.extend([setpoints_lb[i]] * HOURS_PER_MONTH[i])
        setpoints_ub = setpoints_ub_tmp
        setpoints_lb = setpoints_lb_tmp
        del setpoints_ub_tmp
        del setpoints_lb_tmp
        
        # ONLY FOR DEBUG hallucinate hourly T_e
        # if (__debug__):
        #     import datatree as dt
        #     srf_irrad_obstr_tree_tmp = dt.DataTree([[]]*srf_irrad_obstr_tree.BranchCount)
        #     srf_irrad_unobstr_tree_tmp = dt.DataTree([[]]*srf_irrad_unobstr_tree.BranchCount)
            
        #     T_e_tmp = []
        #     bs_multiplier = list(range(-6,6))+list(range(-6,6))[::-1]
            
        #     for i in range(MONTHS_PER_YEAR):
        #         T_e_day = [T_e[i] + a for a in bs_multiplier]
        #         srf_irrad_obstr_tree_day = [[srf_irrad_obstr_tree.Branch(j)[i]/DAYS_PER_MONTH[i] + 100*a for a in bs_multiplier] for j in range(srf_irrad_obstr_tree_tmp.BranchCount)]
        #         srf_irrad_unobstr_tree_day = [[srf_irrad_unobstr_tree.Branch(j)[i]/DAYS_PER_MONTH[i] + 100*a for a in bs_multiplier] for j in range(srf_irrad_unobstr_tree_tmp.BranchCount)]
        #         for _ in range(DAYS_PER_MONTH[i]):
        #             T_e_tmp.extend(T_e_day)
        #             for i, b in enumerate(srf_irrad_obstr_tree_tmp.Branches):
        #                 b.extend(srf_irrad_obstr_tree_day[i])
        #             for i, b in enumerate(srf_irrad_unobstr_tree_tmp.Branches):
        #                 b.extend(srf_irrad_unobstr_tree_day[i])
        #     T_e = T_e_tmp
        #     srf_irrad_obstr_tree = srf_irrad_obstr_tree_tmp
        #     srf_irrad_unobstr_tree = srf_irrad_unobstr_tree_tmp
        #     del T_e_tmp
        #     del srf_irrad_obstr_tree_tmp
        #     del srf_irrad_unobstr_tree_tmp
    
    # Assert inputs monthly or hourly based on toggle
    input_size = HOURS_PER_YEAR if hourly else MONTHS_PER_YEAR
    assert len(setpoints_ub) == input_size, "Length of 'setpoints_ub' (Setpoints upper bound) was %d but should be %d." % (len(setpoints_ub), input_size)
    assert len(setpoints_lb) == input_size, "Length of 'setpoints_lb' (Setpoints lower bound) was %d but should be %d." % (len(setpoints_lb), input_size)
    assert len(T_e) == input_size, "Length of 'T_e' (Ambient temperature) was %d but should be %d." % (len(T_e), input_size)

    # read room properties from sia2024
    room_properties = cleanDictForNaN(room_properties)

    # f_sh = 0.9  # sia2024, p.12, 1.3.1.9 Reduktion solare Wärmeeinträge
    # theta_i_summer = room_properties["Raumlufttemperatur Auslegung Kuehlung (Sommer)"]
    # theta_i_winter = room_properties["Raumlufttemperatur Auslegung Heizen (Winter)"]
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
    if hourly:
        #TODO Determine based on SIA 2024 schedules from type
        t_P = [room_properties["Vollaststunden pro Jahr (Personen)"] / float(HOURS_PER_YEAR)] * HOURS_PER_YEAR
        t_L = [room_properties["Jaehrliche Vollaststunden der Raumbeleuchtung"] / float(HOURS_PER_YEAR)] * HOURS_PER_YEAR
        t_A = [room_properties["Jaehrliche Vollaststunden der Geraete"] / float(HOURS_PER_YEAR)] * HOURS_PER_YEAR
    else:
        t_P = [room_properties["Vollaststunden pro Jahr (Personen)"]] * MONTHS_PER_YEAR
        t_L = [room_properties["Jaehrliche Vollaststunden der Raumbeleuchtung"]] * MONTHS_PER_YEAR
        t_A = [room_properties["Jaehrliche Vollaststunden der Geraete"]] * MONTHS_PER_YEAR
        # transforming daily sia2024 data to monthly
        for i in range(len(DAYS_PER_MONTH)):
            t_P[i] *= DAYS_PER_MONTH[i] / DAYS_PER_YEAR
            t_L[i] *= DAYS_PER_MONTH[i] / DAYS_PER_YEAR
            t_A[i] *= DAYS_PER_MONTH[i] / DAYS_PER_YEAR

    time_range = HOURS_PER_YEAR if hourly else MONTHS_PER_YEAR  

    # formatting the grasshopper tree that contains solar irradiation time series for each window
    # could be changed later to also include solar irradiation on opaque surfaces...
    # ...would need to be adapted in the 'for surface in range(num_surfaces):' loop as well then
    win_areas = [x for (x, y) in zip(surface_areas, surface_type) if y != "opaque"]
    Q_s_tr_per_surface = None
    
    if (srf_irrad_obstr_tree.Branch(0).Count == 0 and srf_irrad_unobstr_tree.BranchCount == 0):
        num_surfaces_tr = len([s for s in surface_type if s=="transp"])
        Q_s_tr_per_surface = [[0.0]*num_surfaces_tr] * time_range
    else:
        Q_s_tr_per_surface = calculate_Q_s(run_obstructed_simulation, srf_irrad_obstr_tree, srf_irrad_unobstr_tree,
                                            g_value, g_value_total, 
                                            setpoint_shading, win_areas,
                                            hourly=hourly)
    
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

    Phi_P = Phi_P_per_m2 * floor_area
    Phi_L = Phi_L_per_m2 * floor_area
    Phi_A = Phi_A_per_m2 * floor_area
            
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

        # Ventilation losses (Lüftungswärmeverluste)
        # Q_V = H_V * (T_i - T_e) * t
        # we compute with and without heat recovery, ub and lb, and take the respectively best (lowest demand)
        # this assumes, the ventilation system operates ideally and does not, for example, keep warm air in summer
        Q_V_ub = H_V * (setpoints_ub[t] - T_e[t]) * num_timesteps
        Q_V_lb = H_V * (setpoints_lb[t] - T_e[t]) * num_timesteps
        Q_V_ub_no_heat_recovery = H_V_no_heat_recovery * (setpoints_ub[t] - T_e[t]) * num_timesteps
        Q_V_lb_no_heat_recovery = H_V_no_heat_recovery * (setpoints_lb[t] - T_e[t]) * num_timesteps

        # Internal loads (interne Wärmeeinträge)
        # Q_i = Phi_P * t_P + Phi_L * t_L + Phi_A * t_A
        Q_i[t] = Phi_P * t_P[t] + Phi_L * t_L[t] + Phi_A * t_A[t]

        # Transmission heat losses, Q_T, per surface, both transparent and opaque
        Q_T_ub = 0.0
        Q_T_lb = 0.0
        Q_T_op_per_surface_ub = []
        Q_T_op_per_surface_lb = []
        Q_T_tr_per_surface_ub = []
        Q_T_tr_per_surface_lb = []
        
        deltaT_ub = (setpoints_ub[t] - T_e[t]) * num_timesteps
        deltaT_lb = (setpoints_lb[t] - T_e[t]) * num_timesteps
            
        for surface in range(num_surfaces):
            # Transmission heat transfer coefficient (Transmissions-Wärmetransferkoeffizient), H_T, (PER SURFACE)            
            if surface_type[surface] == "opaque":
                H_T = surface_areas[surface] * U_value_opaque
                Q_T_op_per_surface_ub.append(H_T * deltaT_ub)
                Q_T_op_per_surface_lb.append(H_T * deltaT_lb)
            else:
                H_T = surface_areas[surface] * U_value_transparent
                Q_T_tr_per_surface_ub.append(H_T * deltaT_ub)
                Q_T_tr_per_surface_lb.append(H_T * deltaT_lb)

            # Transmission losses (Transmissionswärmeverluste), Q_T, (PER SURFACE, because function of H_T)
            Q_T_ub += H_T * deltaT_ub
            Q_T_lb += H_T * deltaT_lb
        
        # solar gains (solare Wärmeeinträge), Q_s, (PER SURFACE)
        # unobstructed or obstructed, both using SolarModel.dll and GHSolar.gha
        Q_s_tr = sum(Q_s_tr_per_surface[t])
        Q_s[t] = Q_s_tr   # currently, only transparent surfaces

        # Heatgains/-losses ratio (Wärmeeintrag/-verlust Verhältnis), gamma
        # calculating for different cases, heating / cooling, upper / lower bounds, with / without heat recovery
        gamma_ub = calc_gamma(Q_i[t], Q_s[t], Q_T_ub, Q_V_ub)
        gamma_lb = calc_gamma(Q_i[t], Q_s[t], Q_T_lb, Q_V_lb)
        gamma_ub_no_hr = calc_gamma(Q_i[t], Q_s[t], Q_T_ub, Q_V_ub_no_heat_recovery)
        gamma_lb_no_hr = calc_gamma(Q_i[t], Q_s[t], Q_T_lb, Q_V_lb_no_heat_recovery)

        # usage of heat gains (Ausnutzungsgrad für Wärmegewinne), eta_g
        eta_g_ub_heating = calc_eta_g(gamma_ub, tau)  # TODO why do we need this?
        eta_g_lb_heating = calc_eta_g(gamma_lb, tau)
        eta_g_ub_cooling = calc_eta_g(gamma_ub, tau, cooling=True) 
        eta_g_lb_cooling = calc_eta_g(gamma_lb, tau, cooling=True) # TODO why do we need this?
        eta_g_ub_heating_no_hr = calc_eta_g(gamma_ub_no_hr, tau) # TODO why do we need this?
        eta_g_lb_heating_no_hr = calc_eta_g(gamma_lb_no_hr, tau)
        eta_g_ub_cooling_no_hr = calc_eta_g(gamma_ub_no_hr, tau, cooling=True)
        eta_g_lb_cooling_no_hr = calc_eta_g(gamma_lb_no_hr, tau, cooling=True) # TODO why do we need this?
 
        # heating demand (Heizwärmebedarf), Q_H
        # Q_H = Q_T + Q_V - eta_g * (Q_i + Q_s)
        # calculating different cases, lower/upper bounds (lb/ub), with/without heat recovery (hr)
        Q_H_ub = Q_T_ub + Q_V_ub - eta_g_ub_heating * (Q_i[t] + Q_s[t]) # TODO why do we need this?
        Q_H_lb = Q_T_lb + Q_V_lb - eta_g_lb_heating * (Q_i[t] + Q_s[t])
        Q_H_ub_no_hr = Q_T_ub + Q_V_ub_no_heat_recovery - eta_g_ub_heating_no_hr * (Q_i[t] + Q_s[t]) # TODO why do we need this?
        Q_H_lb_no_hr = Q_T_lb + Q_V_lb_no_heat_recovery - eta_g_lb_heating_no_hr * (Q_i[t] + Q_s[t])
        Q_H_ub, Q_H_lb, Q_H_ub_no_hr, Q_H_lb_no_hr = negatives_to_zero([Q_H_ub, Q_H_lb, Q_H_ub_no_hr, Q_H_lb_no_hr])

        # cooling demand (Kältebedarf), Q_K
        # calculating different cases, lower/upper bounds (lb/ub), with/without heat recovery (hr)
        Q_K_ub = Q_i[t] + Q_s[t] - eta_g_ub_cooling * (Q_T_ub + Q_V_ub)
        Q_K_lb = Q_i[t] + Q_s[t] - eta_g_lb_cooling * (Q_T_lb + Q_V_lb) # TODO why do we need this?
        Q_K_ub_no_hr = Q_i[t] + Q_s[t] - eta_g_ub_cooling_no_hr * (Q_T_ub + Q_V_ub_no_heat_recovery)
        Q_K_lb_no_hr = Q_i[t] + Q_s[t] - eta_g_lb_cooling_no_hr * (Q_T_lb + Q_V_lb_no_heat_recovery) # TODO why do we need this?
        Q_K_ub, Q_K_lb, Q_K_ub_no_hr, Q_K_lb_no_hr = negatives_to_zero([Q_K_ub, Q_K_lb, Q_K_ub_no_hr, Q_K_lb_no_hr]) # TODO why do we need this?

        # take smaller value of both comfort set points and remember the index
        Q_H, Q_H_index = min_and_index(Q_H_lb, Q_H_ub, Q_H_lb_no_hr, Q_H_ub_no_hr)
        Q_K, Q_K_index = min_and_index(Q_K_lb, Q_K_ub, Q_K_lb_no_hr, Q_K_ub_no_hr)
        Q_K *= -1.0     # make cooling negative

        # either subtract heating from cooling, but then also account for that in losses/gains by subtracting those too
        # or just take the higher load of both and then take the corresponding losses/gains
        # demand = Q_K + Q_H  # sometimes we have both cooling and heating. so subtract one from another
        # if demand < 0:
        #     Q_Cool[month] = demand * -1
        #     Q_Heat[month] = 0
        # else:
        #     Q_Cool[month] = 0
        #     Q_Heat[month] = demand
        if abs(Q_K) > Q_H:
            demand = Q_K
            Q_Cool[t] = Q_K
            Q_Heat[t] = 0.0
        else:
            demand = Q_H
            Q_Cool[t] = 0.0
            Q_Heat[t] = Q_H

        # TODO lighting and utility loads. simplification, because utility and lighting have efficiencies (inefficiencies are heat loads). 
        # Need efficiency or electricity use separately
        Q_Elec[t] = Phi_L * t_L[t] + Phi_A * t_A[t]   

        # Q_i, Q_s are * with eta_rec in heating case
        # Q_T, Q_V are * with eta_rec in cooling case
        Q_T_op_lb = sum(Q_T_op_per_surface_lb) # index 0 or 2
        Q_T_op_ub = sum(Q_T_op_per_surface_ub) # index 1 or 3
        Q_T_tr_lb = sum(Q_T_tr_per_surface_lb)
        Q_T_tr_ub = sum(Q_T_tr_per_surface_ub)

        # Create lists in form lb, ub, lb_no_heat_recovery, ub_no_heat_recovery
        Q_T_list = [Q_T_lb, Q_T_ub, Q_T_lb, Q_T_ub]
        Q_T_opaque_list = [Q_T_op_lb, Q_T_op_ub, Q_T_op_lb, Q_T_op_ub]
        Q_T_transparent_list = [Q_T_tr_lb, Q_T_tr_ub, Q_T_tr_lb, Q_T_tr_ub]
        Q_V_heating_list = [Q_V_lb, Q_V_ub, Q_V_lb_no_heat_recovery, Q_V_ub_no_heat_recovery]
        Q_V_cooling_list = [Q_V_lb, Q_V_ub, Q_V_lb_no_heat_recovery, Q_V_ub_no_heat_recovery]
        eta_rec_heating_list = [eta_g_lb_heating, eta_g_ub_heating, eta_g_lb_heating_no_hr, eta_g_ub_heating_no_hr]
        eta_rec_cooling_list = [eta_g_lb_cooling, eta_g_ub_cooling, eta_g_lb_cooling_no_hr, eta_g_ub_cooling_no_hr]

        if demand < 0:  # cooling
            Q_T_out[t] = Q_T_list[Q_K_index] * eta_rec_cooling_list[Q_K_index]
            Q_T_opaque_out[t] = Q_T_opaque_list[Q_K_index] * eta_rec_cooling_list[Q_K_index]
            Q_T_transparent_out[t] = Q_T_transparent_list[Q_K_index] * eta_rec_cooling_list[Q_K_index]
            Q_V_out[t] = Q_V_cooling_list[Q_K_index] * eta_rec_cooling_list[Q_K_index]
            Q_i_out[t] = Q_i[t]
            Q_s_out[t] = Q_s[t]
        else:   # heating
            Q_T_out[t] = Q_T_list[Q_H_index]
            Q_T_opaque_out[t] = Q_T_opaque_list[Q_H_index]
            Q_T_transparent_out[t] = Q_T_transparent_list[Q_H_index]
            Q_V_out[t] = Q_V_heating_list[Q_H_index]
            Q_i_out[t] = Q_i[t] * eta_rec_heating_list[Q_H_index]
            Q_s_out[t] = Q_s[t] * eta_rec_heating_list[Q_H_index]


    if Q_s_tr_per_surface != None:
        Q_s_tr_tree = th.list_to_tree(Q_s_tr_per_surface, source=[0, 0])   # import ghpythonlib.treehelpers as th
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
    elif gamma == 1.0: # TODO check SIA 380 norms 3.5.6.2, probably rarely happens anyways?
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
    def get_monthly_sum(hourly_timeseries):
        monthly_timeseries = []
        for month in range(MONTHS_PER_YEAR):
            start_hour = int(HOURS_PER_DAY * sum(DAYS_PER_MONTH[0:month]))
            end_hour = int(HOURS_PER_DAY * sum(DAYS_PER_MONTH[0:month + 1]))
            monthly_timeseries.append(sum(hourly_timeseries[start_hour:end_hour]))
        return monthly_timeseries
  
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
        
    # Transpose to per timestep
    Q_array = transpose_jagged_2D_array(Q_array)
    
    return Q_array

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


if __name__ == "__main__":
    class Jagged:
        def __init__(self, data):
            self.data = data

    def test():
        room_properties = {
            "description": "1.2 Wohnen Einfamilienhaus",
            "Raumlufttemperatur Auslegung Heizen (Winter)": 21.0,
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
            "Glasanteil": 30.0,
            "Zeitkonstante": 164.0,
            "Waermeeintragsleistung der Raumbeleuchtung": 2.7,
            "Waermeeintragsleistung der Geraete": 8.0,
            "Gesamtenergiedurchlassgrad Verglasung": 0.5,
            "Temperatur-Aenderungsgrad der Waermerueckgewinnung": 0.7,
            "Aussenluft-Volumenstrom (pro NGF)": 0.6
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
        

        [Q_Heat, Q_Cool, Q_Elec, Q_T, Q_V, Q_i, Q_s, _, _] = main(room_properties, floor_area,
                                                                  T_e, 
                                                                  setpoints_ub, setpoints_lb, 
                                                                  surface_areas, surface_type, 
                                                                  srf_irrad_obstr_tree, srf_irrad_unobstr_tree,
                                                                  g_value, g_value_total,
                                                                  setpoint_shading,
                                                                  False, hourly=True)
        print(Q_Heat)
        print(Q_Cool)
        print(Q_Elec)
        print(Q_T)
        print(Q_V)
        print(Q_i)
        print(Q_s)


    test()
