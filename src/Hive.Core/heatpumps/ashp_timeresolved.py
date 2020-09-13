# coding=utf-8
"""
Calulating time resolved COP of Air-source heat pump and the electricity demand, given heating load

Source: 10.1016/j.apenergy.2019.03.177  Eq. (A.7)

COP_{HP,t} = pi_{HP,1} * exp[pi_{HP,2} * (T_supply - T_{amb,t}) + pi_{HP,3} * exp[pi_{HP,4} * (T_supply - T_{amb,t})]
where T_supply is the supply temperature, T_{amb,t} is the ambient air temperature and pi_HP are parameters
depending on the type of the HP.

arguments:
    - pi_HP,1,2,3,4 [-]
    - T_supply [°C]
    - T_amb,t [°C]
    - Q_th [kW]

output:
    - x_el,t [kW]
"""
import math

def main(Q_th, T_supply, T_amb, pi_1, pi_2, pi_3, pi_4):
    kelvin = 273.15
    T_supply_kelvin = T_supply + kelvin
    horizon = len(T_amb) if len(T_amb) < len(Q_th) else len(Q_th)
    COP = [0] * horizon
    x_el = [0] * horizon
    for i in range(horizon):
        T_amb_kelvin = T_amb[i] + kelvin
        COP[i] = pi_1 * math.exp(pi_2 * (T_supply_kelvin - T_amb_kelvin)) + pi_3 * math.exp(pi_4 * (T_supply_kelvin - T_amb_kelvin))
        x_el[i] = Q_th[i] / COP[i]
    return x_el, COP
