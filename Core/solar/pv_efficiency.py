# -*- coding: utf-8 -*-
"""
The data is (should be) based on the lecture Energie und Klimasysteme  II,
Erneuerbare Energieerzeugung am Gebäude, FS 2019
"""
from __future__ import print_function


def get_pv_efficiency(pv_efficiency):
    return "\n".join("{key}: {value}".format(key=key, value=value)
                     for key, value in pv_efficiency.items())
