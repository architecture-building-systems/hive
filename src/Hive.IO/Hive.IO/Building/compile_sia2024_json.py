"""
Read in these three files (located in hive/Hive.Core/sia380)

- 200728_SIA2024_Raumdaten_Bestand.csv
- 200728_SIA2024_Raumdaten_Standardwert.csv
- 200728_SIA2024_Raumdaten_Zielwert.csv

and save them to a json format as a single file (adding a field "Quality" and "BuildingUseType"). This file
will be added as a resource to `Hive.IO.dll` that can then be read with `Hive.IO.Building.Sia2024Record`.
"""
from __future__ import print_function

import os
import csv
import json

# map the csv header names to Sia2024Record names...
# (try to keep them as similar as possible)
header_map = {    
    "Zeitkonstante": "Zeitkonstante",
    "Raumlufttemperatur Auslegung Kuehlung (Sommer)": "RaumlufttemperaturAuslegungKuehlen",
    "Raumlufttemperatur Auslegung Heizen (Winter)": "RaumlufttemperaturAuslegungHeizen",
    "Nettogeschossflaeche": "Nettogeschossflaeche",
    "Thermische Gebaeudehuellflaeche": "ThermischeGebaeudehuellflaeche",
    "Glasanteil": "Glasanteil",
    "U-Wert opake Bauteile": "UWertOpakeBauteile",
    "U-Wert Fenster": "UWertFenster",
    "Abminderungsfaktor fuer Fensterrahmen": "AbminderungsfaktorFuerFensterrahmen",
    "Aussenluft-Volumenstrom (pro NGF)": "AussenluftVolumenstrom",
    "Aussenluft-Volumenstrom durch Infiltration": "AussenluftVolumenstromDurchInfiltration",
    "Temperatur-Aenderungsgrad der Waermerueckgewinnung": "TemperaturAenderungsgradDerWaermerueckgewinnung",
    "Waermeeintragsleistung Personen (bei 24.0 deg C, bzw. 70 W)": "WaermeeintragsleistungPersonen",
    "Waermeeintragsleistung der Raumbeleuchtung": "WaermeeintragsleistungRaumbeleuchtung",
    "Waermeeintragsleistung der Geraete": "WaermeeintragsleistungGeraete",
    "Vollaststunden pro Jahr (Personen)": "VollaststundenPersonen",
    "Jaehrliche Vollaststunden der Raumbeleuchtung": "VollaststundenRaumbeleuchtung",
    "Jaehrliche Vollaststunden der Geraete": "VollaststundenGeraete",
    "Gesamtenergiedurchlassgrad Verglasung": "GesamtenergiedurchlasgradVerglasung",
    "Kosten opake Bauteile": "KostenOpakeBauteile",
    "Kosten transparente Bauteile": "KostenTransparenteBauteile",
    "Emissionen opake Bauteile": "EmissionenOpakeBauteile",
    "Emissionen transparente Bauteile": "EmissionenTransparenteBauteile"
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

def main():
    file_template = "200728_SIA2024_Raumdaten_{quality}.csv"
    folder = os.path.abspath(os.path.join(os.path.dirname(__file__), "..", "..", "..", "Hive.Core", "sia380"))
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
                        record[header_map[f]] = None
                records.append(record)
    out_file = os.path.join(os.path.dirname(__file__), "sia2024_room_data.json")
    with open(out_file, "w") as fp:
        json.dump(records, fp, indent=4, encoding="utf8")

                
        


if __name__ == '__main__':
    main()