# coding=utf-8

from __future__ import division
import math
import json


def main(description, room_constant, cooling_setpoint, heating_setpoint, floor_area, envelope_area, glazing_ratio,
         u_value_opaque, u_value_transparent, g_value, windows_frame_reduction,
         air_change_rate, infiltration, heat_recovery,
         occupant_loads, lighting_loads, equipment_loads,
         occupant_yearly_hours, lighting_yearly_hours, equipment_yearly_hours,
         opaque_cost, transparent_cost,
         opaque_emissions, transparent_emissions):
    """
    Creates a dictionary according to our Hive sia2024 naming convention
    :param description: 'description'. Must be from a pre-defined list
    :param room_constant: 'Zeitkonstante', integer, in hours?
    :param cooling_setpoint: 'Raumlufttemperatur Auslegung Kuehlung (Sommer)', float, in deg. C
    :param heating_setpoint: 'Raumlufttemperatur Auslegung Heizen (Winter)', float, in deg. C
    :param floor_area: 'Nettogeschossflaeche', float, in square meters
    :param envelope_area: 'Thermische Gebaeudehuellflaeche', float, in square meters
    :param glazing_ratio: 'Glasanteil', float, in %
    :param u_value_opaque: 'U-Wert opake Bauteile', float, in W/m^2K
    :param u_value_transparent: 'U-Wert Fenster', float, in W/m^2K
    :param g_value: 'Gesamtenergiedurchlassgrad Verglasung', float, factor from 0 - 1
    :param windows_frame_reduction: 'Abminderungsfaktor fuer Fensterrahmen', float, factor from 0 to 1?
    :param air_change_rate: 'Aussenluft-Volumenstrom (pro NGF)', float
    :param infiltration: 'Aussenluft-Volumenstrom durch Infiltration', float
    :param heat_recovery: 'Temperatur-Aenderungsgrad der Waermerueckgewinnung', float
    :param occupant_loads: 'Waermeeintragsleistung Personen (bei 24.0 deg C, bzw. 70 W)', float
    :param lighting_loads: 'Waermeeintragsleistung der Raumbeleuchtung', float
    :param equipment_loads: 'Waermeeintragsleistung der Geraete', float
    :param occupant_yearly_hours: 'Vollaststunden pro Jahr (Personen)', integer, in hours
    :param lighting_yearly_hours: 'Jaehrliche Vollaststunden der Raumbeleuchtung', integer, in hours
    :param equipment_yearly_hours: 'Jaehrliche Vollaststunden der Geraete', integer, in hours
    :param opaque_cost: 'Kosten opake Bauteile', float, CHF per square meters
    :param transparent_cost: 'Kosten transparente Bauteile', float, CHF per square meters
    :param opaque_emissions: 'Emissionen opake Bauteile', float, kgCO2/m^2
    :param transparent_emissions: 'Emissionen transparente Bauteile', float, kgCO2/m^2
    :return: Returns a dictionary
    """

    sia_room = {}
    sia_room["description"] = description
    sia_room["Zeitkonstante"] = room_constant
    sia_room["Raumlufttemperatur Auslegung Kuehlung (Sommer)"] = cooling_setpoint
    sia_room["Raumlufttemperatur Auslegung Heizen (Winter)"] = heating_setpoint
    sia_room["Nettogeschossflaeche"] = floor_area
    sia_room["Thermische Gebaeudehuellflaeche"] = envelope_area
    sia_room["Glasanteil"] = glazing_ratio
    sia_room["U-Wert opake Bauteile"] = u_value_opaque
    sia_room["U-Wert Fenster"] = u_value_transparent
    sia_room["Gesamtenergiedurchlassgrad Verglasung"] = g_value
    sia_room["Abminderungsfaktor fuer Fensterrahmen"] = windows_frame_reduction
    sia_room["Aussenluft-Volumenstrom (pro NGF)"] = air_change_rate
    sia_room["Aussenluft-Volumenstrom durch Infiltration"] = infiltration
    sia_room["Temperatur-Aenderungsgrad der Waermerueckgewinnung"] = heat_recovery
    sia_room["Waermeeintragsleistung Personen (bei 24.0 deg C, bzw. 70 W)"] = occupant_loads
    sia_room["Waermeeintragsleistung der Raumbeleuchtung"] = lighting_loads
    sia_room["Waermeeintragsleistung der Geraete"] = equipment_loads
    sia_room["Vollaststunden pro Jahr (Personen)"] = occupant_yearly_hours
    sia_room["Jaehrliche Vollaststunden der Raumbeleuchtung"] = lighting_yearly_hours
    sia_room["Jaehrliche Vollaststunden der Geraete"] = equipment_yearly_hours
    sia_room["Kosten opake Bauteile"] = opaque_cost
    sia_room["Kosten transparente Bauteile"] = transparent_cost
    sia_room["Emissionen opake Bauteile"] = opaque_emissions
    sia_room["Emissionen transparente Bauteile"] = transparent_emissions

    return json.dumps(sia_room)
