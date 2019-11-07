"""
Implements predefined default values from SIA 2028:2010 Klimadaten for SIA 380.1 calculations.

"""


def default_values(station):
    I = 0.0
    I_E = 0.0
    I_S = 0.0
    I_W = 0.0
    I_N = 0.0
    theta_e = 0.0
    return I, I_E, I_S, I_W, I_N, theta_e


if __name__ == '__main__':
    values = default_values("Zürich-Kloten")
    for i in range(len(values)):
        print(values[i])