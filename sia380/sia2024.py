# coding=utf-8
"""
Implements predefined default values for SIA 380.1 calculations.

TODO: create dropdowns for input prameters?

Inputs:
Use case (Default value: Single family home)
A - Living area [m2]
month [-]

Outputs:
U_op - U value opaque walls [W/(m2K)]
U_w - U value windows [W/(m2K)]
tau - time constant [h]
Ve - Exterior air volume flow rate per square meter of living area [m3/h]
Vinf - Infiltration air volume flow rate per square meter of living area [m3/h]
eta_rec,theta - ventilation heat recovery efficiency [-]
Phi_Peop - Heat release People [W]
Phi_Light - Heat release Lightning [W]
Phi_Appl - Heat release Appliances [W]
t_Peop - full load hours People [h]
t_Light - full load hours Lightning [h]
t_Appl - full load hours Appliances [h]
g - g-Value windows [-]

"""


def default_values(use_case, area, month):
    use_case_data = {
        "mfh": {'tau': 182.0,
                'theta_i': 26.0,
                'theta_e': 0,
                't': 0,
                'A_op': 20.0,
                'A_w': 26.0,
                'U_op': 0.2,
                'U_w': 1.2,
                'Vdot_e': 1.0,
                'Vdot_inf': 0.15,
                'eta_rec': 0.75,
                'phi_P': 2.3,
                'phi_B': 2.7,
                'phi_G': 8.0,
                't_P': 4090.0,
                't_B': 1540.0,
                't_G': 1780.0,
                'g': 0.5,
                'f_sh': 84.0,
                'I': 0},
        "efh": {'tau': 164.0,
                'theta_i': 26.0,
                'theta_e': 0,
                't': 0,
                'A_op': 20.0,
                'A_w': 38.0,
                'U_op': 0.2,
                'U_w': 1.2,
                'Vdot_e': 0.6,
                'Vdot_inf': 0.15,
                'eta_rec': 0.75,
                'phi_P': 1.4,
                'phi_B': 2.7,
                'phi_G': 8.0,
                't_P': 4090.0,
                't_B': 1450.0,
                't_G': 1780.0,
                'g': 0.5,
                'f_sh': 71.0,
                'I': 0},
        "office": {'tau': 117.0,
                   'theta_i': 26.0,
                   'theta_e': 0,
                   't': 0,
                   'A_op': 36.0,
                   'A_w': 42.0,
                   'U_op': 0.2,
                   'U_w': 1.2,
                   'Vdot_e': 2.6,
                   'Vdot_inf': 0.15,
                   'eta_rec': 0.75,
                   'phi_P': 5.0,
                   'phi_B': 15.9,
                   'phi_G': 7.0,
                   't_P': 1500.0,
                   't_B': 1210.0,
                   't_G': 1930.0,
                   'g': 0.5,
                   'f_sh': 107.0,
                   'I': 0},
        "school": {'tau': 72.0,
                   'theta_i': 26.0,
                   'theta_e': 0,
                   't': 0,
                   'A_op': 70.0,
                   'A_w': 95.0,
                   'U_op': 0.2,
                   'U_w': 1.2,
                   'Vdot_e': 8.3,
                   'Vdot_inf': 0.15,
                   'eta_rec': 0.75,
                   'phi_P': 23.3,
                   'phi_B': 14.0,
                   'phi_G': 4.0,
                   't_P': 1390.0,
                   't_B': 1180.0,
                   't_G': 1770.0,
                   'g': 0.5,
                   'f_sh': 245.0,
                   'I': 0}
    }

    tmp = use_case_data[use_case]
    return tmp['tau'], tmp['theta_i'], tmp['theta_e'], tmp['t'], tmp['A_op'], tmp['A_w'], tmp['U_op'], tmp['U_w'], tmp[
        'Vdot_e'], tmp['Vdot_inf'], tmp['eta_rec'], tmp['phi_P'], tmp['phi_B'], tmp['phi_G'], tmp['t_P'], tmp['t_B'], \
           tmp['t_G'], tmp['g'], tmp['f_sh'], tmp['I']
