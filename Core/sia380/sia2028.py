"""
Implements predefined default values from SIA 2028:2010 Klimadaten for SIA 380.1 calculations.
"""
from __future__ import division
from __future__ import print_function


def default_values(station, month):
    # Data from Table 2, SIA 2028:2010 Klimadaten
    # irradiation in MJ
    # MJ * 1000.0 / 3.6 = Wh
    station_data = {
        "Zurich-Kloten": {'theta_e': [0.2, 1.3, 5.4, 8.5, 13.6, 16.5, 18.7, 18.5, 14.0, 9.7, 4.1, 1.7, 9.4],
                          'I': [102.0, 167.0, 313.0, 425.0, 546.0, 583.0, 603.0, 525.0, 355.0, 209.0, 106.0, 80.0],
                          'I_E': [67.0, 109.0, 190.0, 244.0, 303.0, 321.0, 335.0, 297.0, 194.0, 107.0, 60.0, 48.0],
                          'I_S': [163.0, 235.0, 316.0, 298.0, 295.0, 277.0, 303.0, 337.0, 311.0, 244.0, 148.0, 123.0],
                          'I_W': [78.0, 123.0, 196.0, 236.0, 297.0, 311.0, 332.0, 297.0, 218.0, 142.0, 73.0, 56.0],
                          'I_N': [43.0, 65.0, 96.0, 119.0, 163.0, 184.0, 182.0, 142.0, 98.0, 67.0, 39.0, 32.0]},
        "Zurich-MeteoSchweiz": {'theta_e': [0.4, 1.6, 5.5, 8.4, 13.4, 16.2, 18.4, 18.4, 14.0, 9.9, 4.2, 1.8, 9.4],
                                'I': [104.0, 165.0, 311.0, 417.0, 536.0, 570.0, 595.0, 522.0, 355.0, 214.0, 109.0, 80.0],
                                'I_E': [67.0, 109.0, 185.0, 233.0, 281.0, 295.0, 311.0, 287.0, 192.0, 112.0, 62.0, 51.0],
                                'I_S': [177.0, 235.0, 313.0, 290.0, 284.0, 270.0, 297.0, 332.0, 311.0, 254.0, 158.0, 137.0],
                                'I_W': [80.0, 123.0, 198.0, 231.0, 287.0, 303.0, 327.0, 295.0, 218.0, 142.0, 75.0, 59.0],
                                'I_N': [43.0, 63.0, 94.0, 111.0, 150.0, 166.0, 166.0, 134.0, 91.0, 62.0, 39.0, 32.0]}
    }

    MJ_to_Wh = 1000.0 / 3.6
    I = station_data[station]['I'][month - 1] * MJ_to_Wh
    I_E = station_data[station]['I_E'][month - 1] * MJ_to_Wh
    I_S = station_data[station]['I_S'][month - 1] * MJ_to_Wh
    I_W = station_data[station]['I_W'][month - 1] * MJ_to_Wh
    I_N = station_data[station]['I_N'][month - 1] * MJ_to_Wh
    theta_e = station_data[station]['theta_e'][month - 1]
    return I, I_E, I_S, I_W, I_N, theta_e


if __name__ == '__main__':
    values = default_values("Zuerich-Kloten")
    for i in range(len(values)):
        print(values[i])
