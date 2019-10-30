
import json
import GhPython
import System
import hblib_4b5ec46e_cafa_4a72_ac7b_fb839ce3c618 as hblib
import Grasshopper

BADGER_CONFIG = json.loads('''{
    "description": "A simplistic implementation of SIA 380.1", 
    "name": "sia380", 
    "include-install": [], 
    "id": "4b5ec46e-cafa-4a72-ac7b-fb839ce3c618", 
    "components": [
        {
            "subcategory": "demand", 
            "inputs": [
                {
                    "description": "Zeitkonstante des Geb\u00e4udes [h]", 
                    "nick-name": "\u03c4", 
                    "default": null, 
                    "name": "tau", 
                    "access": "item", 
                    "type": "float"
                }, 
                {
                    "description": "Aussenlufttemperatur", 
                    "nick-name": "\u03b8_e", 
                    "default": null, 
                    "name": "theta_e", 
                    "access": "item", 
                    "type": "float"
                }, 
                {
                    "description": "Raumlufttemperatur", 
                    "nick-name": "\u03b8_i", 
                    "default": null, 
                    "name": "theta_i", 
                    "access": "item", 
                    "type": "float"
                }, 
                {
                    "description": "L\u00e4nge der Berechnungsperiode [h]", 
                    "nick-name": "t", 
                    "default": null, 
                    "name": "t", 
                    "access": "item", 
                    "type": "float"
                }, 
                {
                    "description": "Aussenwandfl\u00e4che (opak) [m2]", 
                    "nick-name": "A_op", 
                    "default": null, 
                    "name": "A_op", 
                    "access": "item", 
                    "type": "float"
                }, 
                {
                    "description": "Fensterfl\u00e4che [m2]", 
                    "nick-name": "A_w", 
                    "default": null, 
                    "name": "A_w", 
                    "access": "item", 
                    "type": "float"
                }, 
                {
                    "description": "W\u00e4rmedurchgangskoeffizient Aussenwand [W/m2K]", 
                    "nick-name": "U_op", 
                    "default": null, 
                    "name": "U_op", 
                    "access": "item", 
                    "type": "float"
                }, 
                {
                    "description": "W\u00e4rmedurchgangskoeffizient Fenster [W/m2K]", 
                    "nick-name": "U_w", 
                    "default": null, 
                    "name": "U_w", 
                    "access": "item", 
                    "type": "float"
                }, 
                {
                    "description": "Aussenluft-Volumenstrom [m3/h]", 
                    "nick-name": "V\u0307_e", 
                    "default": null, 
                    "name": "Vdot_e", 
                    "access": "item", 
                    "type": "float"
                }, 
                {
                    "description": "Aussenluft-Volumenstrom durch Infiltration [m3/h]", 
                    "nick-name": "V\u0307_inf", 
                    "default": null, 
                    "name": "Vdot_inf", 
                    "access": "item", 
                    "type": "float"
                }, 
                {
                    "description": "Nutzungsgrad der W\u00e4rmer\u00fcckgewinnung [-]", 
                    "nick-name": "\u03b7_rec", 
                    "default": null, 
                    "name": "eta_rec", 
                    "access": "item", 
                    "type": "float"
                }, 
                {
                    "description": "W\u00e4rmeabgabe Personen [W]", 
                    "nick-name": "\u03c6_P", 
                    "default": null, 
                    "name": "phi_P", 
                    "access": "item", 
                    "type": "float"
                }, 
                {
                    "description": "W\u00e4rmeabgabe Beleuchtung [W]", 
                    "nick-name": "\u03c6_B", 
                    "default": null, 
                    "name": "phi_B", 
                    "access": "item", 
                    "type": "float"
                }, 
                {
                    "description": "W\u00e4rmeabgabe Ger\u00e4te [W]", 
                    "nick-name": "\u03c6_G", 
                    "default": null, 
                    "name": "phi_G", 
                    "access": "item", 
                    "type": "float"
                }, 
                {
                    "description": "Vollaststunden Personen [h]", 
                    "nick-name": "t_P", 
                    "default": null, 
                    "name": "t_P", 
                    "access": "item", 
                    "type": "float"
                }, 
                {
                    "description": "Vollaststunden Beleuchtung [h]", 
                    "nick-name": "t_B", 
                    "default": null, 
                    "name": "t_B", 
                    "access": "item", 
                    "type": "float"
                }, 
                {
                    "description": "Vollaststunden Ger\u00e4ter [h]", 
                    "nick-name": "t_G", 
                    "default": null, 
                    "name": "t_G", 
                    "access": "item", 
                    "type": "float"
                }, 
                {
                    "description": "g-Wert [-]", 
                    "nick-name": "g", 
                    "default": null, 
                    "name": "g", 
                    "access": "item", 
                    "type": "float"
                }, 
                {
                    "description": "Reduktionsfaktor solare W\u00e4rmeeintr\u00e4ge [-]", 
                    "nick-name": "f_sh", 
                    "default": null, 
                    "name": "f_sh", 
                    "access": "item", 
                    "type": "float"
                }, 
                {
                    "description": "Solare Strahlung [Wh/m2]", 
                    "nick-name": "I", 
                    "default": null, 
                    "name": "I", 
                    "access": "item", 
                    "type": "float"
                }
            ], 
            "description": "SIA 380.1 calculation for a single month", 
            "outputs": [
                {
                    "nick-name": "Q_H", 
                    "type": "float", 
                    "name": "Q_H", 
                    "description": "Heizw\u00e4rmebedarf [Wh]"
                }, 
                {
                    "nick-name": "Q_T", 
                    "type": "float", 
                    "name": "Q_T", 
                    "description": "Transmissionsw\u00e4rmeverluste [Wh]"
                }, 
                {
                    "nick-name": "Q_V", 
                    "type": "float", 
                    "name": "Q_V", 
                    "description": "L\u00fcftungsw\u00e4remeverluste [Wh]"
                }, 
                {
                    "nick-name": "\u03b7_g", 
                    "type": "float", 
                    "name": "eta_g", 
                    "description": "Ausnutzungsgrad f\u00fcr W\u00e4rmegewinne [-]"
                }, 
                {
                    "nick-name": "Q_i", 
                    "type": "float", 
                    "name": "Q_i", 
                    "description": "Interne W\u00e4rmeeintr\u00e4ge [Wh]"
                }, 
                {
                    "nick-name": "Q_s", 
                    "type": "float", 
                    "name": "Q_s", 
                    "description": "Solare W\u00e4rmeeintr\u00e4ge [Wh]"
                }
            ], 
            "name": "SIA 380.1", 
            "id": "6bbb1401-7fb5-4219-ab56-e806b4ec3113", 
            "main-module": "sia380", 
            "category": "[hive]", 
            "class-name": "Sia380", 
            "abbreviation": "sia380", 
            "main-function": "monthly"
        }
    ], 
    "version": "0.1", 
    "author": "daren-thomas", 
    "include-files": [
        "sia380.py"
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

