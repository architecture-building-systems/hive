# coding=utf-8
"""
Implements predefined default values from SIA 2024:2015 for SIA 380.1 calculations.

"""


def default_values(room, area, month, season):
    """

    :param room: sia2024 room type. dictionary with all the properties
    :param area: room area
    :param month: month of the year
    :param season: winter or summer
    :return:
    """

    """
    tau     Zeitkonstante des Gebäudes [h]
    theta_e Aussenlufttemperatur
    theta_i Raumlufttemperatur
    t       Länge der Berechnungsperiode [h]
    A_th    Thermische Gebäudehüllfläche [m2] 
    A_w     Fensterfläche [m2]                      !!!!!! f_g in sia2024 - Glasanteil in [%]
    U_op    Wärmedurchgangskoeffizient Aussenwand [W/m2K]
    U_w     Wärmedurchgangskoeffizient Fenster [W/m2K]
    q_ve  Aussenluft-Volumenstrom [m3/m2h]
    q_vinf Aussenluft-Volumenstrom durch Infiltration [m3/m2h]
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

    summerstart, summerend = 3, 10
    dayspermonth = [31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31]

    # length of calculation period (hours per month) [h]
    t = [744.0, 672.0, 744.0, 720.0, 744.0, 720.0, 744.0, 744.0, 720.0, 744.0, 720.0, 744.0]

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

    f_sh = 0.9  # sia2024, p.12, 1.3.1.9 Reduktion solare Wärmeeinträge

    if area is None:
        area = room["Nettogeschossflaeche"]

    tau = room["Zeitkonstante"]
    if not season:
        if summerstart <= month <= summerend:
            theta_i = room["Raumlufttemperatur Auslegung Kuehlung (Sommer)"]
        else:
            theta_i = room["Raumlufttemperatur Auslegung Kuehlung (Winter)"]
    else:
        if season == "summer":
            theta_i = room["Raumlufttemperatur Auslegung Kuehlung (Sommer)"]
        else:
            theta_i = room["Raumlufttemperatur Auslegung Kuehlung (Winter)"]

    A_th = room["Thermische Gebaeudehuellflaeche"]
    A_w = A_th * room["Glasanteil"]
    U_op = room["U-Wert opake Bauteile"]
    U_w = room["U-Wert Fenster"]
    q_ve = room["Aussenluft-Volumenstrom (pro NGF)"]
    q_vinf = room["Aussenluft-Volumenstrom durch Infiltration"]
    eta_rec = room["Temperatur-Aenderungsgrad der Waermerueckgewinnung"]
    phi_P = room["Waermeeintragsleistung Personen (bei 24.0 deg C, bzw. 70 W)"]
    phi_L = room["Waermeeintragsleistung der Raumbeleuchtung"]
    phi_A = room["Waermeeintragsleistung der Geraete"]
    t_P = [room["Vollaststunden pro Jahr (Personen)"]] * 12
    t_L = [room["Jaehrliche Vollaststunden der Raumbeleuchtung"]] * 12
    t_A = [room["Jaehrliche Vollaststunden der Geraete"]] * 12
    g = room["Gesamtenergiedurchlassgrad Verglasung"]

    # transforming yearly sia2024 data to monthly
    for i in range(len(dayspermonth)):
        t_P[i] *= dayspermonth[i] / 365.0
        t_L[i] *= dayspermonth[i] / 365.0
        t_A[i] *= dayspermonth[i] / 365.0

    return tau, theta_i, t[month - 1], \
           A_th - A_w, A_w, U_op, U_w, \
           q_ve * area, q_vinf * area, eta_rec, \
           phi_P * area, phi_L * area, phi_A * area, \
           t_P[month - 1], t_L[month - 1], t_A[month - 1], \
           g, f_sh


if __name__ == '__main__':
    values = default_values("mfh", "standard", None, 1, "winter")
    for i in range(len(values)):
        print(values[i])