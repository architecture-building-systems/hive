# coding=utf-8
"""
Implements predefined default values from SIA 2024:2015 for SIA 380.1 calculations.

"""


def default_values(use_case, type, area, month):
    """

    :param use_case:
    :param type:
    :param area:
    :param month:
    :return:
    """

    """
    tau     Zeitkonstante des Gebäudes [h]
    theta_e Aussenlufttemperatur
    theta_i Raumlufttemperatur
    t       Länge der Berechnungsperiode [h]
    A_op    Aussenwandfläche (opak) [m2]            !!!!!! A_{th} in sia2024 - thermische Gebäudehüllfläche 
    A_w     Fensterfläche [m2]                      !!!!!! f_g in sia2024 - Glasanteil in [%]
    U_op    Wärmedurchgangskoeffizient Aussenwand [W/m2K]
    U_w     Wärmedurchgangskoeffizient Fenster [W/m2K]
    Vdot_e  Aussenluft-Volumenstrom [m3/h]
    Vdot_inf Aussenluft-Volumenstrom durch Infiltration [m3/h]
    eta_rec Nutzungsgrad der Wärmerückgewinnung [-]
    phi_P   Wärmeabgabe Personen [W]
    phi_L   Wärmeabgabe Beleuchtung [W]
    phi_A   Wärmeabgabe Geräte [W]
    t_P     Vollaststunden Personen [h]
    t_L     Vollaststunden Beleuchtung [h]
    t_A     Vollaststunden Geräte [h]
    g       g-Wert [-]
    f_sh    Reduktionsfaktor solare Wärmeeinträge [-]
    I       description": "Solare Strahlung [Wh/m2]
    """

    dayspermonth = [31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31]

    # Data according to testcase in sia380.py
    # but this should be variable...
    theta_e = [0.5, 1.7, 5.8, 8.9, 14.1, 16.5, 18.5, 18.4, 14.2, 10.4, 4.7, 1.7]    # ambient air temperature [°C]
    t = [744.0, 672.0, 744.0, 720.0, 744.0, 720.0, 744.0, 744.0, 720.0, 744.0, 720.0, 744.0]    # length of calculation period (hours per month) [h]

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
        - 3.2   office:     small office space (Einzel,- Gruppenbüro)               HNF2
        - ...   ...:
        - 4.1   school:     school room (Schulzimmer)                               HNF5
        - ...   ...:
        _____________________________________________________________________________________
    """
    # how do you know: A_op, A_w? must be from rhino geometry!
    # following data implements standard values of SIA 2024:2015 - table 2, p.10 (Standardwert)
    use_case_data = {
        "mfh": {'tau': 182.0,
                'theta_i': 26.0,
                'A_op': 26.0,
                'A_w': 26.0 * 0.3, # 30%... west window?! sia2024
                'U_op': 0.2,
                'U_w': 1.2,
                'Vdot_e': 1.0,
                'Vdot_inf': 0.15,
                'eta_rec': 0.75,
                'phi_P': 2.3,
                'phi_L': 2.7,
                'phi_A': 8.0,
                't_P': [4090.0] * 12,
                't_L': [1450.0] * 12,
                't_A': [1780.0] * 12,
                'g': 0.5},
        "efh": {'tau': 164.0,
                'theta_i': 26.0,
                'A_op': 20.0,
                'A_w': 38.0,
                'U_op': 0.2,
                'U_w': 1.2,
                'Vdot_e': 0.6,
                'Vdot_inf': 0.15,
                'eta_rec': 0.75,
                'phi_P': 1.4,
                'phi_L': 2.7,
                'phi_A': 8.0,
                't_P': [4090.0] * 12,
                't_L': [1450.0] * 12,
                't_A': [1780.0] * 12,
                'g': 0.5},
        "office": {'tau': 117.0,
                   'theta_i': 26.0,
                   'A_op': 36.0,
                   'A_w': 42.0,
                   'U_op': 0.2,
                   'U_w': 1.2,
                   'Vdot_e': 2.6,
                   'Vdot_inf': 0.15,
                   'eta_rec': 0.75,
                   'phi_P': 5.0,
                   'phi_L': 15.9,
                   'phi_A': 7.0,
                   't_P': [4090.0] * 12,
                   't_L': [1450.0] * 12,
                   't_A': [1780.0] * 12,
                   'g': 0.5},
        "school": {'tau': 72.0,
                   'theta_i': 26.0,
                   'A_op': 70.0,
                   'A_w': 95.0,
                   'U_op': 0.2,
                   'U_w': 1.2,
                   'Vdot_e': 8.3,
                   'Vdot_inf': 0.15,
                   'eta_rec': 0.75,
                   'phi_P': 23.3,
                   'phi_L': 14.0,
                   'phi_A': 4.0,
                   't_P': [4090.0] * 12,
                   't_L': [1450.0] * 12,
                   't_A': [1780.0] * 12,
                   'g': 0.5}
    }

    # following data implements ideal values of SIA 2024:2015 - table 2, p.10 (Zielwert)
    use_case_data_ideal = {
        "mfh": {'tau': 258.0, #
                'theta_i': 26.0,
                'A_op': 26.0,
                'A_w': 26.0 * 0.3,
                'U_op': 0.1,
                'U_w': 0.9,
                'Vdot_e': 1.0,
                'Vdot_inf': 0.15,
                'eta_rec': 0.8,
                'phi_P': 2.3,
                'phi_L': 1.7,
                'phi_A': 4.0,
                't_P': [4090.0] * 12,
                't_L': [1110.0] * 12,
                't_A': [1780.0] * 12,
                'g': 0.5},
        "efh": {'tau': 164.0,
                'theta_i': 26.0,
                'A_op': 20.0,
                'A_w': 38.0,
                'U_op': 0.2,
                'U_w': 1.2,
                'Vdot_e': 0.6,
                'Vdot_inf': 0.15,
                'eta_rec': 0.75,
                'phi_P': 1.4,
                'phi_L': 2.7,
                'phi_A': 8.0,
                't_P': [4090.0] * 12,
                't_L': [1450.0] * 12,
                't_A': [1780.0] * 12,
                'g': 0.5},
        "office": {'tau': 117.0,
                   'theta_i': 26.0,
                   'A_op': 36.0,
                   'A_w': 42.0,
                   'U_op': 0.2,
                   'U_w': 1.2,
                   'Vdot_e': 2.6,
                   'Vdot_inf': 0.15,
                   'eta_rec': 0.75,
                   'phi_P': 5.0,
                   'phi_L': 15.9,
                   'phi_A': 7.0,
                   't_P': [4090.0] * 12,
                   't_L': [1450.0] * 12,
                   't_A': [1780.0] * 12,
                   'g': 0.5},
        "school": {'tau': 72.0,
                   'theta_i': 26.0,
                   'A_op': 70.0,
                   'A_w': 95.0,
                   'U_op': 0.2,
                   'U_w': 1.2,
                   'Vdot_e': 8.3,
                   'Vdot_inf': 0.15,
                   'eta_rec': 0.75,
                   'phi_P': 23.3,
                   'phi_L': 14.0,
                   'phi_A': 4.0,
                   't_P': [4090.0] * 12,
                   't_L': [1450.0] * 12,
                   't_A': [1780.0] * 12,
                   'g': 0.5}
    }

    # following data implements values for existing buildings of SIA 2024:2015 - table 2, p.10 (Bestand)
    use_case_data_existing = {
        "mfh": {'tau': 68.0,
                'theta_i': 26.0,
                'A_op': 26.0,
                'A_w': 26.0 * 0.3,
                'U_op': 0.8,
                'U_w': 1.5,
                'Vdot_e': 1.0,
                'Vdot_inf': 0.3,
                'eta_rec': 0.0,
                'phi_P': 2.3,
                'phi_L': 2.7,
                'phi_A': 10.0,
                't_P': [4090.0] * 12,
                't_L': [1450.0] * 12,
                't_A': [1780.0] * 12,
                'g': 0.65},
        "efh": {'tau': 164.0,
                'theta_i': 26.0,
                'A_op': 20.0,
                'A_w': 38.0,
                'U_op': 0.2,
                'U_w': 1.2,
                'Vdot_e': 0.6,
                'Vdot_inf': 0.15,
                'eta_rec': 0.75,
                'phi_P': 1.4,
                'phi_L': 2.7,
                'phi_A': 8.0,
                't_P': [4090.0] * 12,
                't_L': [1450.0] * 12,
                't_A': [1780.0] * 12,
                'g': 0.5},
        "office": {'tau': 117.0,
                   'theta_i': 26.0,
                   'A_op': 36.0,
                   'A_w': 42.0,
                   'U_op': 0.2,
                   'U_w': 1.2,
                   'Vdot_e': 2.6,
                   'Vdot_inf': 0.15,
                   'eta_rec': 0.75,
                   'phi_P': 5.0,
                   'phi_L': 15.9,
                   'phi_A': 7.0,
                   't_P': [4090.0] * 12,
                   't_L': [1450.0] * 12,
                   't_A': [1780.0] * 12,
                   'g': 0.5},
        "school": {'tau': 72.0,
                   'theta_i': 26.0,
                   'A_op': 70.0,
                   'A_w': 95.0,
                   'U_op': 0.2,
                   'U_w': 1.2,
                   'Vdot_e': 8.3,
                   'Vdot_inf': 0.15,
                   'eta_rec': 0.75,
                   'phi_P': 23.3,
                   'phi_L': 14.0,
                   'phi_A': 4.0,
                   't_P': [4090.0] * 12,
                   't_L': [1450.0] * 12,
                   't_A': [1780.0] * 12,
                   'g': 0.5}
    }

    if type == "standard":
        tmp = use_case_data[use_case]
    elif type == "ideal":
        tmp = use_case_data_ideal[use_case]
    else:
        tmp = use_case_data_existing[use_case]

    I = [37778.0, 46944.0, 57222.0, 53611.0, 56389.0, 53889.0, 56944.0, 56389.0, 50278.0, 45000.0, 32778.0, 29444.0] # sia380.py; where is that from?
    f_sh = 0.9  # sia2024, p.12, 1.3.1.9 Reduktion solare Wärmeeinträge

    # transforming yearly sia2024 data to monthly
    for i in range(len(dayspermonth)):
        tmp['t_P'][i] *= dayspermonth[i] / 365.0
        tmp['t_L'][i] *= dayspermonth[i] / 365.0
        tmp['t_A'][i] *= dayspermonth[i] / 365.0

    return tmp['tau'], theta_e[month - 1], tmp['theta_i'], t[month - 1], tmp['A_op'], tmp['A_w'], tmp['U_op'], \
           tmp['U_w'], tmp['Vdot_e'] * area, tmp['Vdot_inf'] * area, tmp['eta_rec'], tmp['phi_P'] * area, \
           tmp['phi_L'] * area, tmp['phi_A'] * area, tmp['t_P'][month - 1], tmp['t_L'][month - 1], \
           tmp['t_A'][month - 1], tmp['g'], f_sh, I[month - 1]


if __name__ == '__main__':
    values = default_values("mfh", "standard", 500.0, 1)
    for i in range(len(values)):
        print(values[i])