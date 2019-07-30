
import json
import GhPython
import System
import hblib
import Grasshopper

BADGER_CONFIG = json.loads('''{
    "description": "A simplistic implementation of SIA 2024", 
    "name": "sia2024", 
    "id": "f5cd6a0b-c7f9-42c9-b58c-045640dfe65d", 
    "components": [
        {
            "subcategory": "demand", 
            "inputs": [
                {
                    "description": "Use case", 
                    "nick-name": "use", 
                    "default": "mfh", 
                    "name": "use_case", 
                    "type": "string"
                }, 
                {
                    "description": "Area in m2", 
                    "nick-name": "area", 
                    "default": 100.0, 
                    "name": "area", 
                    "type": "float"
                }, 
                {
                    "description": "Time period", 
                    "nick-name": "month", 
                    "default": 1, 
                    "name": "month", 
                    "type": "integer"
                }
            ], 
            "description": "Default values for SIA 380.1", 
            "outputs": [
                {
                    "nick-name": "tau - time constant [h]", 
                    "type": "float", 
                    "name": "tau", 
                    "description": "Zeitkonstante des Geb\u00e4udes [h]"
                }, 
                {
                    "nick-name": "\u03b8_e", 
                    "type": "float", 
                    "name": "theta_e", 
                    "description": "Aussenlufttemperatur"
                }, 
                {
                    "nick-name": "\u03b8_i", 
                    "type": "float", 
                    "name": "theta_i", 
                    "description": "Raumlufttemperatur"
                }, 
                {
                    "nick-name": "t", 
                    "type": "float", 
                    "name": "t", 
                    "description": "L\u00e4nge der Berechnungsperiode [h]"
                }, 
                {
                    "nick-name": "A_op", 
                    "type": "float", 
                    "name": "A_op", 
                    "description": "Aussenwandfl\u00e4che (opak) [m2]"
                }, 
                {
                    "nick-name": "A_w", 
                    "type": "float", 
                    "name": "A_w", 
                    "description": "Fensterfl\u00e4che [m2]"
                }, 
                {
                    "nick-name": "U_op - U value opaque walls [W/(m2K)]", 
                    "type": "float", 
                    "name": "U_op", 
                    "description": "W\u00e4rmedurchgangskoeffizient Aussenwand [W/m2K]"
                }, 
                {
                    "nick-name": "U_w - U value windows [W/(m2K)]", 
                    "type": "float", 
                    "name": "U_w", 
                    "description": "W\u00e4rmedurchgangskoeffizient Fenster [W/m2K]"
                }, 
                {
                    "nick-name": "V\u0307_e - exterior air volume flow", 
                    "type": "float", 
                    "name": "Vdot_e", 
                    "description": "Aussenluft-Volumenstrom [m3/h]"
                }, 
                {
                    "nick-name": "V\u0307_inf - infiltration air volume flow", 
                    "type": "float", 
                    "name": "Vdot_inf", 
                    "description": "Aussenluft-Volumenstrom durch Infiltration [m3/h]"
                }, 
                {
                    "nick-name": "\u03b7_rec", 
                    "type": "float", 
                    "name": "eta_rec", 
                    "description": "Nutzungsgrad der W\u00e4rmer\u00fcckgewinnung [-]"
                }, 
                {
                    "nick-name": "\u03c6_P - heat release People [W]", 
                    "type": "float", 
                    "name": "phi_P", 
                    "description": "W\u00e4rmeabgabe Personen [W]"
                }, 
                {
                    "nick-name": "\u03c6_B - heat release Lightning [W]", 
                    "type": "float", 
                    "name": "phi_B", 
                    "description": "W\u00e4rmeabgabe Beleuchtung [W]"
                }, 
                {
                    "nick-name": "\u03c6_G - heat release Appliances [W]", 
                    "type": "float", 
                    "name": "phi_G", 
                    "description": "W\u00e4rmeabgabe Ger\u00e4te [W]"
                }, 
                {
                    "nick-name": "t_P - full load hours People [h]", 
                    "type": "float", 
                    "name": "t_P", 
                    "description": "Vollaststunden Personen [h]"
                }, 
                {
                    "nick-name": "t_B - full load hours Lightning [h]", 
                    "type": "float", 
                    "name": "t_B", 
                    "description": "Vollaststunden Beleuchtung [h]"
                }, 
                {
                    "nick-name": "t_G - full load hours Appliances [h]", 
                    "type": "float", 
                    "name": "t_G", 
                    "description": "Vollaststunden Ger\u00e4ter [h]"
                }, 
                {
                    "nick-name": "g - g-Value windows [-]", 
                    "type": "float", 
                    "name": "g", 
                    "description": "g-Wert [-]"
                }, 
                {
                    "nick-name": "f_sh", 
                    "type": "float", 
                    "name": "f_sh", 
                    "description": "Reduktionsfaktor solare W\u00e4rmeeintr\u00e4ge [-]"
                }, 
                {
                    "nick-name": "I", 
                    "type": "float", 
                    "name": "I", 
                    "description": "Solare Strahlung [Wh/m2]"
                }
            ], 
            "name": "SIA 2024", 
            "id": "99dd97f1-b633-44e9-866c-6b6dc3bb8ae0", 
            "main-module": "sia2024", 
            "category": "[hive]", 
            "class-name": "Sia2024", 
            "abbreviation": "sia2024", 
            "main-function": "default_values"
        }
    ], 
    "version": "0.1", 
    "author": "saso-basic", 
    "include-files": [
        "sia2024.py"
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

