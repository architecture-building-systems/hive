# coding=utf-8
"""
Simple air source heat pump calculation according to Energy and Climate Systems lecture FS 2019
Q = E * COP

input:
    E = electricity [kWh]
    COP = coefficient of performance [-]

output:
    Q = thermal energy [kWh]
"""


def main(E, COP):
    return E * COP
