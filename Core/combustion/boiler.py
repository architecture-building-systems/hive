"""
could be gas, oil, bio boiler
because its just efficiency times fuel [kWh]

arguments:
    - cost of carrier [CHF/kWh]
    - emmissions of carrier [kgCO2/kWheq.]
    - efficiency [-]
    - heating loads [kWh]

should return
    - cost [CHF]
    - carbon emissions [kgCO2eq.]
"""