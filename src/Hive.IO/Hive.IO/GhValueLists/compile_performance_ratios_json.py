import os
import csv
import json

RESOURCES_DIR = os.path.abspath(os.path.join(os.path.dirname(__file__), "..", "Resources")) 
OUT_DIR = os.path.dirname(__file__)

def compile_performace_ratios_to_json():
    """
    Read in csv files with data about performance ratios (located in hive/src/Hive.IO/Hive.IO/Resources)
    and save them to a json format.
    """
    
    JSON_OUT = "performance_ratios.json"
    JSON_PATH = os.path.join(OUT_DIR, JSON_OUT)

    filename = "performance_ratio.csv"

    HEADERS = {
        "Scenario": str,
        "performance ratio": float
    }

    HEADERS_MAP = {
        "Scenario": "Scenario",
        "performance ratio": "PerformanceRatio"
    }
    
    print("Compiling Performance Ratios...")
    data = dict()
    
    file_path = os.path.join(RESOURCES_DIR, filename)
    assert os.path.exists(file_path), "File does not exist: " + file_path
    
    records = []
    with open(file_path, "r") as fp:
        reader = csv.DictReader(fp)
        csv_headers = set(reader.fieldnames)
        assert (all(h in csv_headers for h in HEADERS.keys())), "Missing headers in '" + filename + "' : " + str(set(HEADERS.keys()) - csv_headers)
        for row in reader:
            record = dict()
            for csv_header, json_header in HEADERS_MAP.items():
                if not json_header: continue
                try:
                    record[json_header] = HEADERS[csv_header](row[csv_header])
                except ValueError:
                    record[json_header] = 0.0
            
            records.append(record)
    data = records
    
    with open(JSON_PATH, "w") as fp:
        json.dump(data, fp, indent=4, encoding="utf8")
    
    print("Done. Saved at {0}".format(JSON_PATH))

if __name__ == '__main__':
    compile_performace_ratios_to_json()
