# coding=utf-8
"""
Implements a simplified version of the SIA 380.1 calculations as a first test of the honey-badger system for
creating GHPython components.

In order to keep the code as similar to the mathematical formulas used in the standard, we abandon some PEP8 principles
of variable naming.
"""

from __future__ import division

def isclose(a, b, rel_tol=1e-09, abs_tol=0.0):
    # from here: https://stackoverflow.com/a/33024979/2260
    return abs(a-b) <= max(rel_tol * max(abs(a), abs(b)), abs_tol)


def calc_eta_g(Q_T, Q_V, gamma, tau):
    """FIXME: ask Illias how this works."""
    if (Q_T + Q_V) < 0.0:
        return 0.0
    else:
        if isclose(gamma, 1.0):
            return (1.0 + tau / 15.0) / (2.0 + tau / 15.0)
        else:
            return (1.0 - gamma ** (1.0 + tau / 15.0)) / (1.0 - gamma ** (2.0 + tau / 15.0))


def monthly(tau, theta_i, theta_e, t, A_op, A_w, U_op, U_w, Vdot_e, Vdot_inf, eta_rec, phi_P, phi_B, phi_G, t_P, t_B,
            t_G, g, f_sh, I):
    """

    :param tau:
    :param theta_i:
    :param theta_e:
    :param t: hours in month [h] (de: Länge der Berechnungsperiode)
    :param A_op:
    :param A_w: Window area [m2] (de: Fensterfläche)
    :param U_op:
    :param U_w:
    :param Vdot_e:
    :param Vdot_inf:
    :param eta_rec:
    :param phi_P:
    :param phi_B:
    :param phi_G:
    :param t_P:
    :param t_B:
    :param t_G:
    :param g:
    :param f_sh:
    :param I:
    :return:
    """
    # some constants:
    c_a = 1005.0  # specific heat capacity of air [J/kg*K] (de: Spezifische Wärmekapazität Luft)
    rho_a = 1.2  # density of air [kg/m3] (de: Dichte Luft)
    SECONDS_PER_HOUR = 3600.0  # seconds per hour [-]

    Vdot_th = (Vdot_e * (1 - eta_rec) + Vdot_inf) / SECONDS_PER_HOUR  # [m3/s]
    H_V = Vdot_th * rho_a * c_a
    Q_V = H_V * (theta_i - theta_e) * t
    A_g = A_w  # simplification?!
    Q_s = A_g * g * f_sh * I
    Q_i = phi_P * t_P + phi_B * t_B + phi_G * t_G
    H_T = A_op * U_op + A_w * U_w
    Q_T = H_T * (theta_i - theta_e) * t
    gamma = (Q_i + Q_s) / (Q_T + Q_V)
    eta_g = calc_eta_g(Q_T, Q_V, gamma, tau)
    Q_H = Q_T + Q_V - eta_g * (Q_i + Q_s)
    return Q_H, Q_T, Q_V, eta_g, Q_i, Q_s


def test():
    """Run monthly with the values from the Excel sheet Illias gave me and make sure we get the same results"""
    monthly_inputs = {
        'tau': [238.0, 238.0, 238.0, 238.0, 238.0, 238.0, 238.0, 238.0, 238.0, 238.0, 238.0, 238.0],
        'theta_i': [21.0, 21.0, 21.0, 21.0, 21.0, 21.0, 21.0, 21.0, 21.0, 21.0, 21.0, 21.0],
        'theta_e': [0.5, 1.7, 5.8, 8.9, 14.1, 16.5, 18.5, 18.4, 14.2, 10.4, 4.7, 1.7],
        't': [744.0, 672.0, 744.0, 720.0, 744.0, 720.0, 744.0, 744.0, 720.0, 744.0, 720.0, 744.0],
        'A_op': [300.0, 300.0, 300.0, 300.0, 300.0, 300.0, 300.0, 300.0, 300.0, 300.0, 300.0, 300.0],
        'A_w': [60.0, 60.0, 60.0, 60.0, 60.0, 60.0, 60.0, 60.0, 60.0, 60.0, 60.0, 60.0],
        'U_op': [0.18, 0.18, 0.18, 0.18, 0.18, 0.18, 0.18, 0.18, 0.18, 0.18, 0.18, 0.18],
        'U_w': [0.8, 0.8, 0.8, 0.8, 0.8, 0.8, 0.8, 0.8, 0.8, 0.8, 0.8, 0.8],
        'Vdot_e': [120.0, 120.0, 120.0, 120.0, 120.0, 120.0, 120.0, 120.0, 120.0, 120.0, 120.0, 120.0],
        'Vdot_inf': [30.0, 30.0, 30.0, 30.0, 30.0, 30.0, 30.0, 30.0, 30.0, 30.0, 30.0, 30.0],
        'eta_rec': [0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0],
        'phi_P': [280.0, 280.0, 280.0, 280.0, 280.0, 280.0, 280.0, 280.0, 280.0, 280.0, 280.0, 280.0],
        'phi_B': [540.0, 540.0, 540.0, 540.0, 540.0, 540.0, 540.0, 540.0, 540.0, 540.0, 540.0, 540.0],
        'phi_G': [1600.0, 1600.0, 1600.0, 1600.0, 1600.0, 1600.0, 1600.0, 1600.0, 1600.0, 1600.0, 1600.0, 1600.0],
        't_P': [434.0, 392.0, 434.0, 420.0, 434.0, 420.0, 434.0, 434.0, 420.0, 434.0, 420.0, 434.0],
        't_B': [217.0, 196.0, 217.0, 210.0, 217.0, 210.0, 217.0, 217.0, 210.0, 217.0, 210.0, 217.0],
        't_G': [189.1, 170.8, 189.1, 183.0, 189.1, 183.0, 189.1, 189.1, 183.0, 189.1, 183.0, 189.1],
        'g': [0.5, 0.5, 0.5, 0.5, 0.5, 0.5, 0.5, 0.5, 0.5, 0.5, 0.5, 0.5],
        'f_sh': [0.9, 0.9, 0.9, 0.9, 0.9, 0.9, 0.9, 0.9, 0.9, 0.9, 0.9, 0.9],
        'I': [37778.0, 46944.0, 57222.0, 53611.0, 56389.0, 53889.0, 56944.0, 56389.0, 50278.0, 45000.0, 32778.0, 29444.0]
    }
    expected_outputs = {
        'Q_H': [760000.0, 250000.0, 12000.0, 540.0, 0.037, 2.5e-05, 9.9e-10, 1.9e-09, 0.074, 620.0, 380000.0, 850000.0],
        'Q_T': [1600000.0, 1300000.0, 1200000.0, 890000.0, 520000.0, 330000.0, 190000.0, 200000.0, 500000.0, 800000.0, 1200000.0, 1500000.0],
        'Q_V': [770000.0, 650000.0, 570000.0, 440000.0, 260000.0, 160000.0, 93000.0, 97000.0, 250000.0, 400000.0, 590000.0, 720000.0],
        'eta_g': [1.0, 0.98, 0.82, 0.67, 0.38, 0.25, 0.14, 0.14, 0.4, 0.68, 1.0, 1.0],
        'Q_i': [540000.0, 490000.0, 540000.0, 520000.0, 540000.0, 520000.0, 540000.0, 540000.0, 520000.0, 540000.0, 520000.0, 540000.0],
        'Q_s': [1000000.0, 1300000.0, 1500000.0, 1400000.0, 1500000.0, 1500000.0, 1500000.0, 1500000.0, 1400000.0, 1200000.0, 890000.0, 800000.0]
    }
    output_names = ['Q_H', 'Q_T', 'Q_V', 'eta_g', 'Q_i', 'Q_s']

    for month in range(12):  # months in year
        inputs_for_month = {k: monthly_inputs[k][month] for k in monthly_inputs.keys()}
        result = monthly(**inputs_for_month)
        results_for_month = dict(zip(output_names, result))
        for output in output_names:
            assert isclose(expected_outputs[output][month], results_for_month[output]), "Expected {output}[{month}] to be {a}, got {b}".format(
                output=output, month=month, a=results_for_month[output], b=expected_outputs[output][month])


if __name__ == '__main__':
    test()
