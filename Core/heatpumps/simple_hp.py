# coding=utf-8
"""
Simple heat pump calculation according to Energy and Climate Systems lecture FS 2019
E = Q / COP

input:
    Q = heating loads [kWh]
    COP = coefficient of performance [-]

output:
    E = electricity loads [kWh]
"""


def main(Q, COP):
    return Q / COP
