# coding=utf-8
"""
Time resolved Solar Thermal Collector

Eq. (A.11) from 10.1016/j.apenergy.2019.03.177
found in: 10.1016/j.apenergy.2016.07.055
"""


def main(inlet_temp, ambient_temp, FRtaualpha, FRUL, irradiance, surface_area):
    """
    Calculate heating energy generated from a solar thermal collector
    :param inlet_temp: Inlet temperature into the collector [°C], time series
    :param ambient_temp: Ambient air temperature at the collector [°C], time series
    :param FRtaualpha: Optical efficiency [-], constant
    :param FRUL: Heat loss coefficient [W/m2K], constant
    :param irradiance: Irradiance on the collector [W/m2], time series
    :param surface_area: Surface area of the solar thermal collector [m2]
    :returns: heating energy [kWh] time resolved, efficiency [-] time resolved
    """

    horizon = len(inlet_temp)
    if horizon > len(ambient_temp):
        horizon = len(ambient_temp)
    if horizon > len(irradiance):
        horizon = len(irradiance)

    eta = [0.0] * horizon
    heating = [0.0] * horizon

    for i in range(horizon):
        if (irradiance[i] > 0.0):
            eta_temp = FRtaualpha - ((FRUL * (inlet_temp[i] - ambient_temp[i])) / irradiance[i])
            eta[i] = max(0, eta_temp)
        else:
            eta[i] = 0.0
        heating[i] = (irradiance[i] * eta[i] * surface_area) / 1000.0

    return heating, eta


if __name__ == '__main__':
    inlet_temp = [10.0, 4.0]
    ambient_temp = [-5.0, 2.0]
    FRtaualpha = 0.68
    FRUL = 4.9
    irradiance = [890.0, 12.0]
    surface_area= 1.0

    heating, eta = main(inlet_temp, ambient_temp, FRtaualpha, FRUL, irradiance, surface_area)
    print(heating)
