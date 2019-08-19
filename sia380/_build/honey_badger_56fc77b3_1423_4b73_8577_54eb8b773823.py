
import json
import GhPython
import System
import hblib
import Grasshopper

BADGER_CONFIG = json.loads('''{
    "description": "A simplistic implementation of Electricity and DHW demand", 
    "name": "EDandDHW", 
    "id": "56fc77b3-1423-4b73-8577-54eb8b773823", 
    "components": [
        {
            "subcategory": "demand", 
            "inputs": [
                {
                    "description": "NFA total (m^2)", 
                    "nick-name": "NFA total", 
                    "default": 250.90000000000001, 
                    "name": "NFA total", 
                    "type": "float"
                }, 
                {
                    "description": "NFA laundry (m^2)", 
                    "nick-name": "NFA laundry", 
                    "default": 2.7000000000000002, 
                    "name": "NFA laundry", 
                    "type": "float"
                }, 
                {
                    "description": "NFA adjacent rooms (incl. Cellar) (m^2)", 
                    "nick-name": "NFA adjacent rooms", 
                    "default": 12.1, 
                    "name": "NFA adjacent rooms", 
                    "type": "float"
                }, 
                {
                    "description": "Annual electricity demand of EA equipment (kWh/m^2)", 
                    "nick-name": "Annual elect. demand", 
                    "default": 14, 
                    "name": "Annual elect. demand", 
                    "type": "float"
                }, 
                {
                    "description": "Annual electricity requirement lighting EL+L,Ac (kWh/m^2)", 
                    "nick-name": "Annual elect. req. lighting", 
                    "default": 4, 
                    "name": "Annual elect. req. lighting", 
                    "type": "float"
                }, 
                {
                    "description": "Annual electricity requirement simple ventilation Ev (kWh/m^2)", 
                    "nick-name": "Annual elect. req. vent.", 
                    "default": 1, 
                    "name": "Annual elect. req. vent.", 
                    "type": "float"
                }, 
                {
                    "description": "Washing equipment (kWh/m^2)", 
                    "nick-name": "Washing equipment", 
                    "default": 34, 
                    "name": "Washing equipment", 
                    "type": "float"
                }, 
                {
                    "description": "Washing light (kWh/m^2)", 
                    "nick-name": "Washing light", 
                    "default": 17, 
                    "name": "Washing light", 
                    "type": "float"
                }, 
                {
                    "description": "Washing simple ventilation (kWh/m^2)", 
                    "nick-name": "Washing simple vent.", 
                    "default": 3, 
                    "name": "Washing simple vent.", 
                    "type": "float"
                }, 
                {
                    "description": "Next room equipment (kWh/m^2)", 
                    "nick-name": "Next room equipment", 
                    "default": 0, 
                    "name": "Next room equipment", 
                    "type": "float"
                }, 
                {
                    "description": "Lighting in adjoining room (kWh/m^2)", 
                    "nick-name": "Lighting in adjoining room", 
                    "default": 13, 
                    "name": "Lighting in adjoining room", 
                    "type": "float"
                }, 
                {
                    "description": "Next room simple ventilation (kWh/m^2)", 
                    "nick-name": "Next room simple vent.", 
                    "default": 0.40000000000000002, 
                    "name": "Next room simple vent.", 
                    "type": "float"
                }, 
                {
                    "description": "Annual DHW demand (kWh/m^2)", 
                    "nick-name": "Annual DHW demand", 
                    "default": 13.5, 
                    "name": "Annual DHW demand", 
                    "type": "float"
                }, 
                {
                    "description": "Washing (kWh/m^2)", 
                    "nick-name": "Washing", 
                    "default": 0, 
                    "name": "Washing", 
                    "type": "float"
                }, 
                {
                    "description": "Adjacent room (kWh/m^2)", 
                    "nick-name": "Adjacent room", 
                    "default": 0, 
                    "name": "Adjacent room", 
                    "type": "float"
                }
            ], 
            "description": "Electricity and DHW demand calculation", 
            "outputs": [
                {
                    "nick-name": "Total electricity demand (kWh/a)", 
                    "type": "float", 
                    "name": "Total Annual Electricity Demand", 
                    "description": "Total Annual Electricity Demand (kWh/a)"
                }, 
                {
                    "nick-name": "Total DHW demand (kWh/a)", 
                    "type": "float", 
                    "name": "Total Annual DHW Demand", 
                    "description": "Total Annual DHW Demand (kWh/a)"
                }
            ], 
            "name": "Electricity and DHW demand", 
            "id": "80230b7e-cae1-4998-9483-f7ac1279d1be", 
            "main-module": "EDandDHW", 
            "category": "[hive]", 
            "class-name": "EDandDHW", 
            "abbreviation": "Electricity and DHW demand", 
            "main-function": "ed_and_dhw"
        }
    ], 
    "version": "0.1", 
    "author": "saso-basic", 
    "include-files": [
        "EDandDHW.py"
    ]
}''')

for component in BADGER_CONFIG['components']:
    # dynamically create subclasses of ``component`` for each component in the badger file
    globals()[component['class-name']] = type(component['class-name'], (hblib.get_base_class(component),), {})


class AssemblyInfo(GhPython.Assemblies.PythonAssemblyInfo):
    def get_AssemblyName(self):
        return BADGER_CONFIG['name']

    def get_AssemblyDescription(self):
        return BADGER_CONFIG['description']

    def get_AssemblyVersion(self):
        return BADGER_CONFIG['version']

    def get_AuthorName(self):
        return BADGER_CONFIG['author']

    def get_Id(self):
        return System.Guid(BADGER_CONFIG['id'])

