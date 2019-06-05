# coding=utf-8
"""
Implements a simplified version of the SIA 380.1 calculations as a first test of the honey-badger system for
creating GHPython components.

In order to keep the code as similar to the mathematical formulas used in the standard, we abandon some PEP8 principles
of variable naming.
"""

from __future__ import division


def calc_eta_g(Q_T, Q_V, gamma, tau):
    if (Q_T + Q_V) < 0.0:
        return 0.0
    else:
        if gamma == 1.0:
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

    H_V = Vdot_th * rho_a * c_a
    Q_V = H_V * (theta_i - theta_e) * t
    A_g = A_w  # simplification
    Q_s = A_g * g * f_sh * I
    Q_i = phi_P * t_P + phi_P * t_B + phi_G * t_G
    H_T = A_op * U_op + A_w * U_w
    Q_T = H_T * (theta_i - theta_e) * t
    gamma = (Q_i + Q_s) / (Q_T + Q_V)
    eta_g = calc_eta_g(Q_T, Q_V, gamma, tau)
    Q_H = Q_T + Q_V - eta_g * (Q_i + Q_s)
    return Q_H, Q_T, Q_V, eta_g, Q_i, Q_s
