import os
import csv
import json

RESOURCES_DIR = os.path.abspath(os.path.join(os.path.dirname(__file__), "..", "Resources")) 
OUT_DIR = os.path.dirname(__file__)

def compile_tree_schedules_to_json():
    """
    Read in csv files with data about tree schedules (located in hive/src/Hive.IO/Hive.IO/Resources)
    and save them to a json format.
    """
    
    JSON_OUT = "tree_schedules.json"
    JSON_PATH = os.path.join(OUT_DIR, JSON_OUT)

    filename = "tree_schedules.csv"
    
    print("Compiling tree schedules...")
    data = dict()
    
    file_path = os.path.join(RESOURCES_DIR, filename)
    assert os.path.exists(file_path), "File does not exist: " + file_path
    
    with open(file_path, "r") as fp:
        reader = csv.DictReader(fp)

        out = [{
            "walnut": row["walnut"],
            "sycamore": row["sycamore"], 
            "birch": row["birch"], 
            "mallow": row["mallow"],
            "oak": row["oak"],
            "maple": row["maple"],
            "willow": row["willow"],
            "dogwood": row["dogwood"],
            "katsura": row["katsura"],
            "fruiting": row["fruiting"],
            "witch_hazel": row["witch_hazel"],
            "staff_vine": row["staff_vine"],
            "olive": row["olive"],
            "viburnum": row["viburnum"],
            "bamboo": row["bamboo"],
            "conifer": row["conifer"],
            } 
        for row in reader]

        keys = out[0].keys()
        ref_out = {key: [float(i[key]) for i in out] for key in keys}

    data = ref_out
    
    with open(JSON_PATH, "w") as fp:
        json.dump(data, fp, indent=4, encoding="utf8")
    
    print("Done. Saved at {0}".format(JSON_PATH))

if __name__ == '__main__':
    compile_tree_schedules_to_json()
