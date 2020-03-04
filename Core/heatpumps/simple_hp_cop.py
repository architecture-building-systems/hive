# coding=utf-8
"""
Simple COP calculation for heat pumps, from lectures Energy and Climate Systems FS 2019
COP_W = eta ( T_1 / (T_1 - T_2))

arguments:
    - eta : efficiency HP
    - T_1 : temperature warm reservoir [K]
    - T_2 : temperature cold reservoir [K]

output:
    - COP_W : coefficient of performance [-]
"""


def main(eta, T_1, T_2):
    return eta * (T_1 / (T_1 - T_2))
