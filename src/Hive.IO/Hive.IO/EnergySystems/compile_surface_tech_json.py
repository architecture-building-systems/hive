from __future__ import print_function, with_statement

import os
import csv
import json

RESOURCES_DIR = os.path.abspath(os.path.join(os.path.dirname(__file__), "..", "Resources")) 
OUT_DIR = os.path.dirname(__file__)

def compile_surface_tech_to_json():
    """
    Read in csv files with data about solar tech (located in hive/src/Hive.IO/Hive.IO/Resources)
    and save them to a json format as a single file. This file will be added as a resource to 
    `Hive.IO.dll` that can then be read with `Hive.IO.EnergySystems.SurfaceBasedTech`.
    """
    
    JSON_OUT = "surface_tech_module_types.json"
    JSON_PATH = os.path.join(OUT_DIR, JSON_OUT)

    CSV_FILES = ["pv_efficiency.csv", "st_efficiency.csv"] # not yet supported: "pvt_efficiency.csv"
    CATEGORIES = ["Photovoltaic (PV)", "Solar Thermal (ST)"] # not yet supported:"Photovoltaic and Thermal (PVT)"

    HEADERS = {
        "Type": str,
        "electric efficiency": float,
        "thermal efficiency": float,
        "cost per m2": float,
        "cost per kWp": float,
        "life cycle GHG factor kgCO2eq per m2": float,
        "life cycle GHG factor kgCO2eq per kWp": float,
        "Lifetime": float,
        "Description": str
    }

    HEADERS_MAP = {
        "Type": "Name",
        "electric efficiency": "ElectricEfficiency",
        "thermal efficiency": "ThermalEfficiency",
        "cost per m2": "SpecificCapitalCost",
        "cost per kWp": None,
        "life cycle GHG factor kgCO2eq per m2": "SpecificEmbodiedEmissions",
        "life cycle GHG factor kgCO2eq per kWp": None,
        "Lifetime": "Lifetime", 
        "Description": "Description"
    }
    
    print("Compiling Surface Tech Module Types...")
    data = dict()
    
    for filename, category in zip(CSV_FILES, CATEGORIES):
        file_path = os.path.join(RESOURCES_DIR, filename)
        assert os.path.exists(file_path), "File does not exist: " + file_path
        
        records = []
        with open(file_path, "r") as fp:
            reader = csv.DictReader(fp)
            csv_headers = set(reader.fieldnames)
            assert (all(h in HEADERS.keys() for h in csv_headers)), "Missing headers in '" + filename + "' : " + str(set(HEADERS.keys()) - csv_headers)
            for row in reader:
                record = dict()
                for csv_header, json_header in HEADERS_MAP.items():
                    if not json_header: continue
                    try:
                        record[json_header] = HEADERS[csv_header](row[csv_header])
                    except ValueError:
                        record[json_header] = 0.0
                
                records.append(record)
        data[category] = records
    
    with open(JSON_PATH, "w") as fp:
        json.dump(data, fp, indent=4, encoding="utf8")
    
    print("Done. Saved at {0}".format(JSON_PATH))

def compile_emitters_to_json():
    pass

def compile_conversion_tech_to_json():
    pass

if __name__ == '__main__':
    compile_surface_tech_to_json()
    compile_emitters_to_json()
    compile_conversion_tech_to_json()
