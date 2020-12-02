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
import sys
import ghpythonlib.treehelpers as th

def cleanDictForNaN(d):
    # a = d.values()
    # b = d.keys()
    for i in d:
        if math.isnan(d[i]) or d[i] == "NaN":
            d[i] = 0.0

    return d


def main(room_properties, floor_area, T_e, setpoints_ub, setpoints_lb, surface_areas, surface_type,
         srf_irrad_obstr_tree, srf_irrad_unobstr_tree, g_value, g_value_total, setpoint_shading, run_obstructed_simulation):
    '''
    Computes monthly heating, cooling and electricity demand for a thermal zone, based on SIA 380.1
    :param room_properties: room properties in json format
    :param floor_area: Floor area of the room in m2
    :param T_e: monthly average ambient air temperature in degree Celsius
    :param T_i: Temperature setpoints
    :param setpoints_ub: Upper bound for temperature setpoints
    :param setpoints_lb: Lower bound for temperature setpoints
    :param surface_areas: building surface areas that are used for fabric gains/losses computation
    :param surface_type: indicator if this surface is transparent or not. if yes, it will be assigned the transparent construction from room properties. 'opaque' for opaque or 'transp' for transparent
    :param surface_irradiance: monthly solar irradiation in W/m2 per building surface. Jagged array [months_per_year][num_srfs]. Must be in correct order with the next 2 parameters 'surface_areas' and 'surface_is_transparent'
    :return: Monthly cooling, heating and electricity loads for a thermal zone
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
    hours_per_day = 24
    months_per_year = 12
    days_per_month = [31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31]
    t = [(hours_per_day * days) for days in days_per_month]   # length of calculation period (hours per month) [h]

    # read room properties from sia2024
    room_properties = cleanDictForNaN(room_properties)

    # f_sh = 0.9  # sia2024, p.12, 1.3.1.9 Reduktion solare Wärmeeinträge
    # theta_i_summer = room_properties["Raumlufttemperatur Auslegung Kuehlung (Sommer)"]
    # theta_i_winter = room_properties["Raumlufttemperatur Auslegung Heizen (Winter)"]
    # g = room_properties["Gesamtenergiedurchlassgrad Verglasung"]
    tau = room_properties["Zeitkonstante"]
    U_op = room_properties["U-Wert opake Bauteile"]
    U_w = room_properties["U-Wert Fenster"]
    Vdot_e_spec = room_properties["Aussenluft-Volumenstrom (pro NGF)"]
    Vdot_inf_spec = room_properties["Aussenluft-Volumenstrom durch Infiltration"]
    eta_rec = room_properties["Temperatur-Aenderungsgrad der Waermerueckgewinnung"]
    Phi_P = room_properties["Waermeeintragsleistung Personen (bei 24.0 deg C, bzw. 70 W)"]
    Phi_L = room_properties["Waermeeintragsleistung der Raumbeleuchtung"]
    Phi_A = room_properties["Waermeeintragsleistung der Geraete"]
    t_P = [room_properties["Vollaststunden pro Jahr (Personen)"]] * 12
    t_L = [room_properties["Jaehrliche Vollaststunden der Raumbeleuchtung"]] * 12
    t_A = [room_properties["Jaehrliche Vollaststunden der Geraete"]] * 12

    # transforming daily sia2024 data to monthly
    for i in range(len(days_per_month)):
        t_P[i] *= days_per_month[i] / 365.0
        t_L[i] *= days_per_month[i] / 365.0
        t_A[i] *= days_per_month[i] / 365.0

    # formatting the grasshopper tree that contains solar irradiation time series for each window
    # could be changed later to also include solar irradiation on opaque surfaces...
    # ...would need to be adapted in the 'for surface in range(num_surfaces):' loop as well then
    win_areas = [x for (x, y) in zip(surface_areas, surface_type) if y != "opaque"]
    Q_s_jagged = None
    if (srf_irrad_obstr_tree.Branch(0).Count == 0 and srf_irrad_unobstr_tree.BranchCount == 0):
        Q_s_per_surface = [[0.0] for x in range(months_per_year)]
    else:
        Q_s_jagged = tree_to_jagged_monthly(run_obstructed_simulation, srf_irrad_obstr_tree, srf_irrad_unobstr_tree,
                                            g_value, g_value_total, setpoint_shading, win_areas)
        Q_s_per_surface = transpose_jagged_2D_array(Q_s_jagged)

    # assign room properties to individual surfaces
    #    surface_type = ["opaque", "opaque", "transp", "transp"]
    #    surface_areas = [44.0, 62.3, 4.0, 5.2]
    num_surfaces = len(surface_type)

    # calculations from Illias Excel sheet:
    # preAllocate arrays. its a bit faster than .append (https://levelup.gitconnected.com/faster-lists-in-python-4c4287502f0a)
    Q_i = [0.0] * months_per_year
    Q_s = [0.0] * months_per_year
    Q_i_out = [0.0] * months_per_year
    Q_s_out = [0.0] * months_per_year
    Q_V_out = [0.0] * months_per_year
    Q_T_out = [0.0] * months_per_year
    QT_opaque_out = [0.0] * months_per_year
    QT_transparent_out = [0.0] * months_per_year

    Q_Heat = [0.0] * months_per_year
    Q_Cool = [0.0] * months_per_year
    Q_Elec = [0.0] * months_per_year

    Phi_P_tot = Phi_P * floor_area
    Phi_L_tot = Phi_L * floor_area
    Phi_A_tot = Phi_A * floor_area

    # For each month, compute gains and losses of the zone
    # for some variables, we compute upper and lower bounds (subscripts ub & lb)
    # furthermore, cooling and heating demand requires us to compute certain variables differently.
    # See equation sheet 'EK1_Formelsammlung_HS20.pdf'
    for month in range(months_per_year):
        # External air flowrate (thermisch wirksamer Aussenluftvolumenstrom)
        Vdot_e = Vdot_e_spec * floor_area
        Vdot_inf = Vdot_inf_spec * floor_area
        Vdot_th = Vdot_e * (1.0 - eta_rec) + Vdot_inf
        Vdot_th_no_heat_recovery = Vdot_e + Vdot_inf

        # Ventilation heat loss coefficient (Lüftungs-Wärmetransferkoeffizient), H_V
        H_V = Vdot_th/3600.0 * rho * c_p
        H_V_no_heat_recovery = Vdot_th_no_heat_recovery/3600.0 * rho * c_p

        # Ventilation losses (Lüftungswärmeverluste), Q_V = H_V * (T_i - T_e) * t
        # we compute with and without heat recovery, ub and lb, and take the respectively best (lowest demand)
        # this assumes, the ventilation system operates ideally and does not, for example, keep warm air in summer
        Q_V_ub_heating = H_V * (setpoints_ub[month] - T_e[month]) * t[month]
        Q_V_lb_heating = H_V * (setpoints_lb[month] - T_e[month]) * t[month]
        Q_V_ub_cooling = H_V * (setpoints_ub[month] - T_e[month]) * t[month]
        Q_V_lb_cooling = H_V * (setpoints_lb[month] - T_e[month]) * t[month]
        Q_V_ub_heating_no_heat_recovery = H_V_no_heat_recovery * (setpoints_ub[month] - T_e[month]) * t[month]
        Q_V_lb_heating_no_heat_recovery = H_V_no_heat_recovery * (setpoints_lb[month] - T_e[month]) * t[month]
        Q_V_ub_cooling_no_heat_recovery = H_V_no_heat_recovery * (setpoints_ub[month] - T_e[month]) * t[month]
        Q_V_lb_cooling_no_heat_recovery = H_V_no_heat_recovery * (setpoints_lb[month] - T_e[month]) * t[month]

        # Internal loads (interne Wärmeeinträge)
        Q_i[month] = Phi_P_tot * t_P[month] + Phi_L_tot * t_L[month] + Phi_A_tot * t_A[month]
        Q_T_per_surfaces_this_month_ub = [0.0] * num_surfaces
        Q_T_per_surfaces_this_month_lb = [0.0] * num_surfaces
        QT_op_per_srf_this_month_ub = []
        QT_op_per_srf_this_month_lb = []
        QT_tr_per_srf_this_month_ub = []
        QT_tr_per_srf_this_month_lb = []

        # transmission heat losses, Q_T, per surface, both transparent and opaque
        for surface in range(num_surfaces):
            # Transmission heat transfer coefficient (Transmissions-Wärmetransferkoeffizient), H_T, (PER SURFACE)
            if(surface_type[surface] == "opaque"):
                H_T = surface_areas[surface] * U_op
                QT_op_per_srf_this_month_ub.append(H_T * (setpoints_ub[month] - T_e[month]) * t[month])
                QT_op_per_srf_this_month_lb.append(H_T * (setpoints_lb[month] - T_e[month]) * t[month])
            else:
                H_T = surface_areas[surface] * U_w
                QT_tr_per_srf_this_month_ub.append(H_T * (setpoints_ub[month] - T_e[month]) * t[month])
                QT_tr_per_srf_this_month_lb.append(H_T * (setpoints_lb[month] - T_e[month]) * t[month])

            # Transmission losses (Transmissionswärmeverluste), Q_T, (PER SURFACE, because function of H_T)
            Q_T_per_surfaces_this_month_ub[surface] = H_T * (setpoints_ub[month] - T_e[month]) * t[month]
            Q_T_per_surfaces_this_month_lb[surface] = H_T * (setpoints_lb[month] - T_e[month]) * t[month]

        Q_T_ub = sum(Q_T_per_surfaces_this_month_ub)
        Q_T_lb = sum(Q_T_per_surfaces_this_month_lb)

        # solar gains (solare Wärmeeinträge), Q_s, (PER SURFACE)
        # unobstructed or obstructed, both using SolarModel.dll and GHSolar.gha
        Q_s[month] = sum(Q_s_per_surface[month])    # currently, only transparent surfaces

        # Heatgains/-losses ratio (Wärmeeintrag/-verlust Verhältnis), gamma
        # calculating for different cases, heating / cooling, upper / lower bounds, with / without heat recovery
        gamma_ub_heating = calc_gamma(Q_i[month], Q_s[month], Q_T_ub, Q_V_ub_heating)
        gamma_lb_heating = calc_gamma(Q_i[month], Q_s[month], Q_T_lb, Q_V_lb_heating)
        gamma_ub_cooling = calc_gamma(Q_i[month], Q_s[month], Q_T_ub, Q_V_ub_cooling)
        gamma_lb_cooling = calc_gamma(Q_i[month], Q_s[month], Q_T_lb, Q_V_lb_cooling)
        gamma_ub_heating_no_hr = calc_gamma(Q_i[month], Q_s[month], Q_T_ub, Q_V_ub_heating_no_heat_recovery)
        gamma_lb_heating_no_hr = calc_gamma(Q_i[month], Q_s[month], Q_T_lb, Q_V_lb_heating_no_heat_recovery)
        gamma_ub_cooling_no_hr = calc_gamma(Q_i[month], Q_s[month], Q_T_ub, Q_V_ub_cooling_no_heat_recovery)
        gamma_lb_cooling_no_hr = calc_gamma(Q_i[month], Q_s[month], Q_T_lb, Q_V_lb_cooling_no_heat_recovery)

        # usage of heat gains (Ausnutzungsgrad für Wärmegewinne), eta_g
        eta_g_lb_heating = calc_eta_g(gamma_lb_heating, tau, False)
        eta_g_ub_heating = calc_eta_g(gamma_ub_heating, tau, False)
        eta_g_lb_cooling = calc_eta_g(gamma_lb_cooling, tau, True)
        eta_g_ub_cooling = calc_eta_g(gamma_ub_cooling, tau, True)
        eta_g_lb_heating_no_hr = calc_eta_g(gamma_lb_heating_no_hr, tau)
        eta_g_ub_heating_no_hr = calc_eta_g(gamma_ub_heating_no_hr, tau)
        eta_g_lb_cooling_no_hr = calc_eta_g(gamma_lb_cooling_no_hr, tau, True)
        eta_g_ub_cooling_no_hr = calc_eta_g(gamma_ub_cooling_no_hr, tau, True)

        # heating demand (Heizwärmebedarf), Q_H
        # calculating different cases, lower/upper bounds (lb/ub), with/without heat recovery (hr)
        Q_H_ub = Q_T_ub + Q_V_ub_heating - eta_g_ub_heating * (Q_i[month] + Q_s[month])
        Q_H_lb = Q_T_lb + Q_V_lb_heating - eta_g_lb_heating * (Q_i[month] + Q_s[month])
        Q_H_ub_no_hr = Q_T_ub + Q_V_ub_heating_no_heat_recovery - eta_g_ub_heating_no_hr * (Q_i[month] + Q_s[month])
        Q_H_lb_no_hr = Q_T_lb + Q_V_lb_heating_no_heat_recovery - eta_g_lb_heating_no_hr * (Q_i[month] + Q_s[month])
        Q_H_ub, Q_H_lb, Q_H_ub_no_hr, Q_H_lb_no_hr = negatives_to_zero([Q_H_ub, Q_H_lb, Q_H_ub_no_hr, Q_H_lb_no_hr])

        # cooling demand (Kältebedarf), Q_K
        # calculating different cases, lower/upper bounds (lb/ub), with/without heat recovery (hr)
        Q_K_lb = Q_i[month] + Q_s[month] - eta_g_lb_cooling * (Q_T_lb + Q_V_lb_cooling)
        Q_K_ub = Q_i[month] + Q_s[month] - eta_g_ub_cooling * (Q_T_ub + Q_V_ub_cooling)
        Q_K_lb_no_hr = Q_i[month] + Q_s[month] - eta_g_lb_cooling_no_hr * (Q_T_lb + Q_V_lb_cooling_no_heat_recovery)
        Q_K_ub_no_hr = Q_i[month] + Q_s[month] - eta_g_ub_cooling_no_hr * (Q_T_ub + Q_V_ub_cooling_no_heat_recovery)
        Q_K_lb, Q_K_ub, Q_K_lb_no_hr, Q_K_ub_no_hr = negatives_to_zero([Q_K_lb, Q_K_ub, Q_K_lb_no_hr, Q_K_ub_no_hr])

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
            Q_Cool[month] = Q_K
            Q_Heat[month] = 0.0
        else:
            demand = Q_H
            Q_Cool[month] = 0.0
            Q_Heat[month] = Q_H

        Q_Elec[month] = Phi_L_tot * t_L[month] + Phi_A_tot * t_A[month]   # lighting and utility loads. simplification, because utility and lighting have efficiencies (inefficiencies are heat loads). I would need to know that to get full electricity loads


        # Q_i, Q_s are * with eta_rec in heating case
        # Q_T, Q_V are * with eta_rec in cooling case
        sum_QT_op_lb = sum(QT_op_per_srf_this_month_lb) # index 0 or 2
        sum_QT_op_ub = sum(QT_op_per_srf_this_month_ub) # index 1 or 3
        sum_QT_tr_lb = sum(QT_tr_per_srf_this_month_lb)
        sum_QT_tr_ub = sum(QT_tr_per_srf_this_month_ub)

        Q_T_list = [Q_T_lb, Q_T_ub, Q_T_lb, Q_T_ub]
        QT_opaque_list = [sum_QT_op_lb, sum_QT_op_ub, sum_QT_op_lb, sum_QT_op_ub]
        QT_transparent_list = [sum_QT_tr_lb, sum_QT_tr_ub, sum_QT_tr_lb, sum_QT_tr_ub]
        Q_V_heating_list = [Q_V_lb_heating, Q_V_ub_heating, Q_V_lb_heating_no_heat_recovery, Q_V_ub_heating_no_heat_recovery]
        Q_V_cooling_list = [Q_V_lb_cooling, Q_V_ub_cooling, Q_V_lb_cooling_no_heat_recovery, Q_V_ub_cooling_no_heat_recovery]
        eta_rec_heating_list = [eta_g_lb_heating, eta_g_ub_heating, eta_g_lb_heating_no_hr, eta_g_ub_heating_no_hr]
        eta_rec_cooling_list = [eta_g_lb_cooling, eta_g_ub_cooling, eta_g_lb_cooling_no_hr, eta_g_ub_cooling_no_hr]

        if demand < 0:  # cooling
            Q_T_out[month] = Q_T_list[Q_K_index] * eta_rec_cooling_list[Q_K_index]
            QT_opaque_out[month] = QT_opaque_list[Q_K_index] * eta_rec_cooling_list[Q_K_index]
            QT_transparent_out[month] = QT_transparent_list[Q_K_index] * eta_rec_cooling_list[Q_K_index]
            Q_V_out[month] = Q_V_cooling_list[Q_K_index] * eta_rec_cooling_list[Q_K_index]
            Q_i_out[month] = Q_i[month]
            Q_s_out[month] = Q_s[month]
        else:   # heating
            Q_T_out[month] = Q_T_list[Q_H_index]
            QT_opaque_out[month] = QT_opaque_list[Q_H_index]
            QT_transparent_out[month] = QT_transparent_list[Q_H_index]
            Q_V_out[month] = Q_V_heating_list[Q_H_index]
            Q_i_out[month] = Q_i[month] * eta_rec_heating_list[Q_H_index]
            Q_s_out[month] = Q_s[month] * eta_rec_heating_list[Q_H_index]


    if Q_s_jagged != None:
        Q_s_tree = th.list_to_tree(Q_s_jagged, source=[0, 0])   # import ghpythonlib.treehelpers as th
    else:
        Q_s_tree = None

    tokWh = 1000.0
    return [x / tokWh for x in Q_Heat], [x / tokWh for x in Q_Cool], [x / tokWh for x in Q_Elec], \
           [x / tokWh for x in Q_T_out], [x / tokWh for x in Q_V_out], [x / tokWh for x in Q_i_out], \
           [x / tokWh for x in Q_s_out], [x / tokWh for x in QT_opaque_out], [x / tokWh for x in QT_transparent_out], Q_s_tree


def min_and_index(*sequence):
    min_value = min(sequence)
    index = sequence.index(min_value)
    return min_value, index


def negatives_to_zero(values):
    new_values = []
    for value in values:
        if value < 0.0:
            new_values.append(0.0)
        else:
            new_values.append(value)
    return new_values


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
    else:
        a = 1.0 + tau / 15.0
        if cooling:
            eta_g = (1.0 - gamma ** (-a)) / (1.0 - gamma ** (-(a+1.0)))
        else:
            eta_g = (1.0 - gamma ** a) / (1.0 - gamma ** (a + 1.0))
    return eta_g


def tree_to_jagged_monthly(run_obstr, tree_obstr, tree_unobstr, g_value, g_value_total, setpoint, win_areas):
    # sia2024: 200 W/m2. but maybe should be if srf_irrad > 200 W/m2 && Troom > 22?
    #
    # c# code
    # private void RunScript(DataTree < double > G, double g, ref object Q_s)
    # {
    #       int winCount = G.BranchCount;
    #       double[][] Q_array = new double[12][];
    #       for (int j = 0; j < G.Branch(0).Count; j++)
    #           Q_array[j] = new double[G.BranchCount];
    #       DataTree < double > doubleTree = new DataTree < double > ();
    #       for (int i = 0; i < G.BranchCount; i++)
    #       {
    #           for (int j = 0; j < G.Branch(i).Count; ++j) // should be 12, 1 for each month of the year
    #           {
    #               GH_Path path = new GH_Path(i, j);
    #               double irrad = G.Branch(i)[j] * g;
    #               doubleTree.Add(irrad, path);
    #
    #               Q_array[j][i] = irrad * 1000;
    #            }
    #       }
    #       jagged jag = new jagged();
    #       jag.data = Q_array;
    #
    #       Q_s = jag;
    # }
    #
    # public struct jagged
    # {
    #     public double[][] data;
    # }
    def get_monthly_average(annual_timeseries):
        dayspermonth = [31.0, 28.0, 31.0, 30.0, 31.0, 30.0, 31.0, 31.0, 30.0, 31.0, 30.0, 31.0]
        months = 12
        hour_per_day = 24
        monthly_list = []
        for month in range(months):
            start_hour = int(hour_per_day * sum(dayspermonth[0:month]))
            end_hour = int(hour_per_day * sum(dayspermonth[0:month + 1]))
            monthly_list.append(sum(annual_timeseries[start_hour:end_hour]))
        return monthly_list

    tree = tree_obstr
    if run_obstr == False:
        tree = tree_unobstr

    Q_array = []
    for i in range(tree.BranchCount):
        row = []
        for j in range(tree.Branch(i).Count):
            irrad = tree.Branch(i)[j] / win_areas[i]    # calculating per W/m2 for shading control
            if irrad > setpoint:
                irrad *= g_value_total
            else:
                irrad *= g_value
            row.append(irrad * win_areas[i])    # calculating back to total irradiance of entire surface
        monthly_row = get_monthly_average(row)
        Q_array.append(monthly_row)

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
        setpoints_ub = [25] * 12
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
        Q_s_per_surface = list(map(list, zip(*Q_s_per_surface)))  # transposing
        jaggeddata = Jagged(Q_s_per_surface)

        [Q_Heat, Q_Cool, Q_Elec, Q_T, Q_V, Q_i, Q_s, _, _] = main(room_properties, floor_area, T_e, T_i, setpoints_ub, setpoints_lb, surface_areas, surface_type, jaggeddata)
        print(Q_Heat)
        print(Q_Cool)
        print(Q_Elec)
        print(Q_T)
        print(Q_V)
        print(Q_i)
        print(Q_s)


    test()