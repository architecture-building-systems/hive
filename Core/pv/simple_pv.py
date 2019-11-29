# coding=utf-8
"""
Implements a simplified PV calculation base on the formula:

E = Gh * OF * eta * PR * A

for inputs:

- Gh - Global horizontal irradiation [kWh/m2]
- OF - Orientation factor [%] (Default Value: 100%)
- eta - PV module efficiency [%] (Default Value: 18%)
- PR - Performance ratio [%] (Default Value: 75%)
- A - PV area[m2] (Default Value: 100 m2)

and outputs:

- E - PV electricity generation [kWh]
"""


def simple_pv(Gh, OF, eta, PR, A):
    return Gh * OF * eta * PR * A