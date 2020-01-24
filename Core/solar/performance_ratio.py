# -*- coding: utf-8 -*-
"""
The data is (should be) based on the lecture Energie und Klimasysteme  II,
Erneuerbare Energieerzeugung am Gebäude, FS 2019, Folie 30,
"""


def get_performance_ratio(performance_scenario):
    return "\n".join("{key}: {value}".format(key=key, value=value)
                     for key, value in performance_scenario.items())
