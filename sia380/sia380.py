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

    :param tau: Zeitkonstante des Gebäudes [h]
    :param theta_i: Aussenlufttemperatur [°C]
    :param theta_e: Raumlufttemperatur [°C]
    :param t: hours in month [h] (de: Länge der Berechnungsperiode)
    :param A_op: Aussenwandfläche (opak) [m2]
    :param A_w: Window area [m2] (de: Fensterfläche)
    :param U_op: Wärmedurchgangskoeffizient Aussenwand [W/m2K]
    :param U_w: Wärmedurchgangskoeffizient Fenster [W/m2K]
    :param Vdot_e: Aussenluft-Volumenstrom [m3/h]
    :param Vdot_inf: Aussenluft-Volumenstrom durch Infiltration [m3/h]
    :param eta_rec: Nutzungsgrad der Wärmerückgewinnung [-]
    :param phi_P: Wärmeabgabe Personen [W]
    :param phi_B: Wärmeabgabe Beleuchtung [W]
    :param phi_G: Wärmeabgabe Geräte [W]
    :param t_P: Vollaststunden Personen [h]
    :param t_B: Vollaststunden Beleuchtung [h]
    :param t_G: Vollaststunden Geräter [h]
    :param g: g-Wert [-]
    :param f_sh: Reduktionsfaktor solare Wärmeeinträge [-]
    :param I: Solare Strahlung [Wh/m2]
    :return: Q_H (Heizwärmebedarf [Wh]), Q_T (Transmissionswärmeverluste [Wh]),
    Q_V (Lüftungswäremeverluste [Wh]), eta_g (Ausnutzungsgrad für Wärmegewinne [-]),
    Q_i (Interne Wärmeeinträge [Wh]), Q_s (Solare Wärmeeinträge [Wh])
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
        'Q_H': [761489.8378, 248954.823, 12189.7451, 543.7569, 0.0375, 0.0, 0.0, 0.0, 0.0745, 622.9969, 383494.0403, 850056.911],
        'Q_T': [1555704.0, 1322899.2, 1153497.6, 888624.0, 523627.2, 330480.0, 189720.0, 197308.8, 499392.0, 804412.8, 1197072.0, 1464638.4],
        'Q_V': [766413.0, 651722.4, 568267.2, 437778.0, 257963.4, 162810.0, 93465.0, 97203.6, 246024.0, 396291.6, 589734.0, 721549.8],
        'eta_g': [0.9996, 0.9825, 0.8194, 0.6726, 0.3787, 0.2493, 0.1362, 0.1427, 0.3962, 0.6833, 0.9961, 0.9999],
        'Q_i': [541260.0, 488880.0, 541260.0, 523800.0, 541260.0, 523800.0, 541260.0, 541260.0, 523800.0, 541260.0, 523800.0, 541260.0],
        'Q_s': [1020000.0, 1267500.0, 1545000.0, 1447500.0, 1522500.0, 1455000.0, 1537500.0, 1522500.0, 1357500.0, 1215000.0, 885000.0, 795000.0]
    }
    output_names = ['Q_H', 'Q_T', 'Q_V', 'eta_g', 'Q_i', 'Q_s']

    for month in range(12):  # months in year
        inputs_for_month = {k: monthly_inputs[k][month] for k in monthly_inputs.keys()}
        result = monthly(**inputs_for_month)
        results_for_month = dict(zip(output_names, result))
        for output in output_names:
            assert isclose(expected_outputs[output][month], results_for_month[output]), "Expected {output}[{month}] to be {a}, got {b}".format(
                output=output, month=month, b=results_for_month[output], a=expected_outputs[output][month])


if __name__ == '__main__':
    test()
