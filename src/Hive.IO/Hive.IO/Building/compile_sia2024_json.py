from __future__ import print_function, with_statement

import os
import csv
import json

# map the csv header names to Sia2024Record names...
# (try to keep them as similar as possible)
header_map = {    
    "Zeitkonstante": "RoomConstant",
    "Raumlufttemperatur Auslegung Kuehlung (Sommer)": "CoolingSetpoint",
    "Raumlufttemperatur Auslegung Kuehlung (Sommer) - Absenktemperatur": "CoolingSetback",
    "Raumlufttemperatur Auslegung Heizen (Winter)": "HeatingSetpoint",
    "Raumlufttemperatur Auslegung Heizen (Winter) - Absenktemperatur": "HeatingSetback",
    "Nettogeschossflaeche": "FloorArea",
    "Thermische Gebaeudehuellflaeche": "EnvelopeArea",
    "Glasanteil": "GlazingRatio",
    "U-Wert opake Bauteile": "UValueOpaque",
    "U-Wert Fenster": "UValueTransparent",
    "Gesamtenergiedurchlassgrad Verglasung": "GValue",
    "Gesamtenergiedurchlassgrad Verglasung und Sonnenschutz": "GValueTotal",
	"Strahlungsleistung fuer Betaetigung Sonnenschutz": "ShadingSetpoint",
    "Abminderungsfaktor fuer Fensterrahmen": "WindowFrameReduction",
    "Aussenluft-Volumenstrom (pro NGF)": "AirChangeRate",
    "Aussenluft-Volumenstrom durch Infiltration": "Infiltration",
    "Temperatur-Aenderungsgrad der Waermerueckgewinnung": "HeatRecovery",
    "Waermeeintragsleistung Personen (bei 24.0 deg C, bzw. 70 W)": "OccupantLoads",
    "Waermeeintragsleistung der Raumbeleuchtung": "LightingLoads",
    "Waermeeintragsleistung der Geraete": "EquipmentLoads",
    "Vollaststunden pro Jahr (Personen)": "OccupantYearlyHours",
    "Jaehrliche Vollaststunden der Raumbeleuchtung": "LightingYearlyHours",
    "Jaehrliche Vollaststunden der Geraete": "EquipmentYearlyHours",
    "Kosten opake Bauteile": "OpaqueCost",
    "Kosten transparente Bauteile": "TransparentCost",
    "Emissionen opake Bauteile": "OpaqueEmissions",
    "Emissionen transparente Bauteile": "TransparentEmissions"
}


# map the use type based on the first integer in the "description" of the csv files
# FIXME: there might be a more official list for this...
building_use_type_map = {
    1: "Wohnen",
    2: "Hotel",
    3: "Buero",
    4: "Schule",
    5: "Verkauf",
    6: "Restaurant",
    7: "Halle",
    8: "Spital",
    9: "Werkstatt",
    10: "Lager",
    11: "Sport",
    12: "Diverses"
}

def room_properties():
    """
    Read in these three files (located in hive/src/Hive.IO/Hive.IO/Resources)

    - 200108_SIA2024_Raumdaten_Bestand.csv
    - 200108_SIA2024_Raumdaten_Standardwert.csv
    - 200108_SIA2024_Raumdaten_Zielwert.csv

    and save them to a json format as a single file (adding a field "Quality" and "BuildingUseType"). This file
    will be added as a resource to `Hive.IO.dll` that can then be read with `Hive.IO.Building.Sia2024Record`.
    """
    
    file_template = "201008_SIA2024_Raumdaten_{quality}.csv"
    folder = os.path.abspath(os.path.join(os.path.dirname(__file__), "..", "Resources")) 
    #folder = "C:\\users\\chwaibel\\documents\\github\\Hive\\src\\Hive.IO\\Hive.IO\\Resources"
    records = []
    for quality in {"Bestand", "Standardwert", "Zielwert"}:
        file_path = os.path.join(folder, file_template.format(quality=quality))
        assert os.path.exists(file_path)
        with open(file_path, "r") as fp:
            reader = csv.DictReader(fp)
            float_fields = set(reader.fieldnames) - {"description"}
            assert (all(f in header_map.keys() for f in float_fields))
            for row in reader:
                building_use_type = building_use_type_map[int(row["description"].split(".")[0])]
                record = {"Quality": quality, "BuildingUseType": building_use_type, "RoomType": row["description"]}
                for f in float_fields:
                    try:
                        record[header_map[f]] = float(row[f])
                    except ValueError:
                        record[header_map[f]] = 0.0
                records.append(record)
    out_file = os.path.join(os.path.dirname(__file__), "sia2024_room_data.json")
    with open(out_file, "w") as fp:
        json.dump(records, fp, indent=4, encoding="utf8")
        
if __name__ == '__main__':
    room_properties()
