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

# map the csv header names to Sia2024Record names...
header_map = {
    "description": "RoomType",
    "Zeitkonstante": "Zeitkonstante"
}

def main():
    file_template = "200728_SIA2024_Raumdaten_{quality}.csv"
    folder = os.path.abspath(os.path.join(os.path.dirname(__file__), "..", "..", "..", "Hive.Core", "sia380"))
    for quality in {"Bestand", "Standardwert", "Zielwert"}:
        file_path = os.path.join(folder, file_template.format(quality=quality))
        assert os.path.exists(file_path)
        with open(file_path, "r") as fp:
            reader = csv.DictReader(fp)
            print(reader.fieldnames)
        


if __name__ == '__main__':
    main()