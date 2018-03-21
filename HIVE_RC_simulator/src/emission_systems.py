# This comoponent contains a car-builder system of objects which define simple 
# emission systems.
#
# Nest: An educational plugin developed by the A/S chair at ETH Zurich
# This component is based on emission_system.py in the RC_BuildingSimulator github repository
# https://github.com/architecture-building-systems/RC_BuildingSimulator
#
# Authors: Prageeth Jayathissa <jayathissa@arch.ethz.ch>, Michael Fehr
# Converted into a grasshopper plugin by Justin Zarb <zarbj@student.ethz.ch>
#
# This file is part of Nest
#
# Licensing/Copywrite and liability comments go here.
# Copyright 2018, Architecture and Building Systems - ETH Zurich
# Licence: MIT

"""
Place this component in the grasshopper workspace so that other Nest components can access the emission systems object definitions
-
Provided by Nest 0.0.1
"""

ghenv.Component.Name = "Emission Systems"
ghenv.Component.NickName = 'EmissionSystems'
ghenv.Component.Message = 'VER 0.0.1\nFEB_21_2018'
ghenv.Component.IconDisplayMode = ghenv.Component.IconDisplayMode.application
ghenv.Component.Category = "Nest"
ghenv.Component.SubCategory = "0 | Core"

try: ghenv.Component.AdditionalHelpFromDocStrings = "2"
except: pass

import scriptcontext as sc
class EmissionDirector:

    """
    The director sets what Emission system is being used, and runs that set Emission system
    """

    builder = None

    # Sets what Emission system is used
    def set_builder(self, builder):
        #        self.__builder = builder
        self.builder = builder
    # Calcs the energy load of that system. This is the main() fu

    def calc_flows(self):

        # Director asks the builder to produce the system body. self.builder
        # is an instance of the class

        body = self.builder.heat_flows()

        return body


class EmissionSystemBase:

    """ 
    The base class in which systems are built from
    """

    def __init__(self, energy_demand):

        self.energy_demand = energy_demand


    def heat_flows(self): pass
    """
    determines the node where the heating/cooling system is active based on the system used
    Also determines the return and supply temperatures for the heating/cooling system
    """


class OldRadiators(EmissionSystemBase):
    """
    Old building with radiators and high supply temperature
    Heat is emitted to the air node
    """

    def heat_flows(self):
        flows = Flows()
        flows.phi_ia_plus = self.energy_demand
        flows.phi_st_plus = 0
        flows.phi_m_plus = 0

        flows.heating_supply_temperature = 65
        flows.heating_return_temperature = 45
        flows.cooling_supply_temperature = 12
        flows.cooling_return_temperature = 21

        return flows


class NewRadiators(EmissionSystemBase):
    """    
    Newer building with radiators and medium supply temperature
    Heat is emitted to the air node
    """

    def heat_flows(self):
        flows = Flows()
        flows.phi_ia_plus = self.energy_demand
        flows.phi_st_plus = 0
        flows.phi_m_plus = 0

        flows.heating_supply_temperature = 50
        flows.heating_return_temperature = 35
        flows.cooling_supply_temperature = 12
        flows.cooling_return_temperature = 21

        return flows

class ChilledBeams(EmissionSystemBase):
    """
    Chilled beams: identical to newRadiators but used for cooling
    Heat is emitted to the air node
    """

    def heat_flows(self):
        flows = Flows()
        flows.phi_ia_plus = self.energy_demand
        flows.phi_st_plus = 0
        flows.phi_m_plus = 0

        flows.heating_supply_temperature = 50
        flows.heating_return_temperature = 35
        flows.cooling_supply_temperature = 18
        flows.cooling_return_temperature = 21

        return flows


class AirConditioning(EmissionSystemBase):
    """
    All heat is given to the air via an AC-unit. HC input via the air node as in the ISO 13790 Annex C.
    supplyTemperature as with new radiators (assumption)
    Heat is emitted to the air node
    """

    def heat_flows(self):
        flows = Flows()
        flows.phi_ia_plus = self.energy_demand
        flows.phi_st_plus = 0
        flows.phi_m_plus = 0

        flows.heating_supply_temperature = 40
        flows.heating_return_temperature = 20
        flows.cooling_supply_temperature = 6
        flows.cooling_return_temperature = 15

        return flows

class FloorHeating(EmissionSystemBase):
    """
    All HC energy goes into the surface node, supplyTemperature low
    Heat is emitted to the surface node
    """

    def heat_flows(self):
        flows = Flows()
        flows.phi_ia_plus = 0
        flows.phi_st_plus = self.energy_demand
        flows.phi_m_plus = 0

        flows.heating_supply_temperature = 40
        flows.heating_return_temperature = 5
        flows.cooling_supply_temperature = 12
        flows.cooling_return_temperature = 21

        return flows

class TABS(EmissionSystemBase):
    """
    Thermally activated Building systems. HC energy input into bulk node. Supply Temperature low.
    Heat is emitted to the thermal mass node
    """

    def heat_flows(self):
        flows = Flows()
        flows.phi_ia_plus = 0
        flows.phi_st_plus = 0
        flows.phi_m_plus = self.energy_demand

        flows.heating_supply_temperature = 50
        flows.heating_return_temperature = 35
        flows.cooling_supply_temperature = 12
        flows.cooling_return_temperature = 21

        return flows


class Flows:
    """
    A base object to store output variables
    """

    phi_ia_plus = float("nan")
    phi_m_plus = float("nan")
    phi_st_plus = float("nan")

    heating_supply_temperature = float("nan")
    cooling_supply_temperature = float("nan")
    # return temperatures

sc.sticky["EmissionDirector"] = EmissionDirector
sc.sticky["OldRadiators"] = OldRadiators
sc.sticky["AirConditioning"] = AirConditioning
sc.sticky["NewRadiators"] = NewRadiators
sc.sticky["ChilledBeams"] = ChilledBeams
sc.sticky["FloorHeating"] = FloorHeating
sc.sticky["TABS"] = TABS

print 'Emission systems are go!'