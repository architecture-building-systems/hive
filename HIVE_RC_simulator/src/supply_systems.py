# This comoponent contains a car-builder system of objects which define simple 
# emission systems.
#
# Nest: An educational plugin developed by the A/S chair at ETH Zurich
# This component is based on code in the RC_BuildingSimulator github repository
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
Place this component in the grasshopper workspace so that other Nest components can access the supply systems objects.
-
Provided by Nest 0.0.1
"""

ghenv.Component.Name = "Supply Systems"
ghenv.Component.NickName = 'SupplySystems'
ghenv.Component.Message = 'VER 0.0.1\nFEB_22_2018'
ghenv.Component.IconDisplayMode = ghenv.Component.IconDisplayMode.application
ghenv.Component.Category = "Nest"
ghenv.Component.SubCategory = "0 | Core"

try: ghenv.Component.AdditionalHelpFromDocStrings = "2"
except: pass

import scriptcontext as sc

# This is one layer of abstraction too many, however it is kept for future explansion of the supply system
class SupplyDirector:

    """
    The director sets what Supply system is being used, and runs that set Supply system
    """

    builder = None

    # Sets what building system is used
    def set_builder(self, builder):
        self.builder = builder

    # Calcs the energy load of that system. This is the main() function
    def calc_system(self):

        # Director asks the builder to produce the system body. self.builder
        # is an instance of the class

        body = self.builder.calc_loads()

        return body


class SupplySystemBase:

    """
     The base class in which Supply systems are built from 
    """

    def __init__(self, load, t_out, heating_supply_temperature, cooling_supply_temperature, has_heating_demand, has_cooling_demand):
        self.load = load  # Energy Demand of the building at that time step
        self.t_out = t_out  # Outdoor Air Temperature
        # Temperature required by the emission system
        self.heating_supply_temperature = heating_supply_temperature
        self.cooling_supply_temperature = cooling_supply_temperature
        self.has_heating_demand = has_heating_demand
        self.has_cooling_demand = has_cooling_demand

    def calc_loads(self): pass
    """
    Caculates the electricty / fossil fuel consumption of the set supply system
    If the system also generates electricity, then this is stored as electricity_out
    """


class OilBoilerOld(SupplySystemBase):
    """
    Old oil boiler with fuel efficiency of 63 percent (medium of range in report of semester project M. Fehr)
    No condensation, pilot light
    """

    def calc_loads(self):
        system = SupplyOut()
        system.fossils_in = self.load / 0.63
        system.electricity_in = 0
        system.electricity_out = 0
        return system


class OilBoilerMed(SupplySystemBase):
    """
    Classic oil boiler with fuel efficiency of 82 percent (medium of range in report of semester project M. Fehr)
    No condensation, but better nozzles etc.
    """

    def calc_loads(self):
        system = SupplyOut()
        system.fossils_in = self.load / 0.82
        system.electricity_in = 0
        system.electricity_out = 0
        return system


class OilBoilerNew(SupplySystemBase):
    """
    New oil boiler with fuel efficiency of 98 percent (value from report of semester project M. Fehr)
    Condensation boiler, latest generation
    """

    def calc_loads(self):
        system = SupplyOut()
        system.fossils_in = self.load / 0.98
        system.electricity_in = 0
        system.electricity_out = 0
        return system


class HeatPumpAir(SupplySystemBase):
    """
    BETA Version
    Air-Water heat pump. Outside Temperature as reservoir temperature.
    COP based off regression analysis of manufacturers data
    Source: "A review of domestic heat pumps, Iain Staffell, Dan Brett, Nigel Brandonc and Adam Hawkes"
    http://pubs.rsc.org/en/content/articlepdf/2012/ee/c2ee22653g

    TODO: Validate this methodology
    """

    def calc_loads(self):
        system = SupplyOut()

        if self.has_heating_demand:
            # determine the temperature difference, if negative, set to 0
            deltaT = max(0, self.heating_supply_temperature - self.t_out)
            # Eq (4) in Staggell et al.
            system.cop = 6.81 - 0.121 * deltaT + 0.000630 * deltaT**2
            system.electricity_in = self.load / system.cop

        elif self.has_cooling_demand:
            # determine the temperature difference, if negative, set to 0
            deltaT = max(0, self.t_out - self.cooling_supply_temperature)
            # Eq (4) in Staggell et al.
            system.cop = 6.81 - 0.121 * deltaT + 0.000630 * deltaT**2
            system.electricity_in = self.load / system.cop

        else:
            raise ValueError(
                'HeatPumpAir called although there is no heating/cooling demand')

        system.fossils_in = 0
        system.electricity_out = 0
        return system


class HeatPumpWater(SupplySystemBase):
    """"
    BETA Version
    Reservoir temperatures 7 degC (winter) and 12 degC (summer).
    Ground-Water heat pump. Outside Temperature as reservoir temperature.
    COP based off regression analysis of manufacturers data
    Source: "A review of domestic heat pumps, Iain Staffell, Dan Brett, Nigel Brandonc and Adam Hawkes"
    http://pubs.rsc.org/en/content/articlepdf/2012/ee/c2ee22653g

        # TODO: Validate this methodology
    """


    def calc_loads(self):
        system = SupplyOut()
        if self.has_heating_demand:
            deltaT = max(0, self.heating_supply_temperature - 7.0)
            # Eq (4) in Staggell et al.
            system.cop = 8.77 - 0.150 * deltaT + 0.000734 * deltaT**2
            system.electricity_in = self.load / system.cop

        elif self.has_cooling_demand:
            deltaT = max(0, 12.0 - self.cooling_supply_temperature)
            # Eq (4) in Staggell et al.
            system.cop = 8.77 - 0.150 * deltaT + 0.000734 * deltaT**2
            system.electricity_in = self.load / system.cop

        system.fossils_in = 0
        system.electricity_out = 0
        return system


class ElectricHeating(SupplySystemBase):
    """
    Straight forward electric heating. 100 percent conversion to heat.
    """

    def calc_loads(self):
        system = SupplyOut()
        system.electricity_in = self.load
        system.fossils_in = 0
        system.electricity_out = 0
        return system


class CHP(SupplySystemBase):
    """
    Combined heat and power unit with 60 percent thermal and 33 percent
    electrical fuel conversion. 93 percent overall
    """

    def calc_loads(self):
        system = SupplyOut()
        system.fossils_in = self.load / 0.6
        system.electricity_in = 0
        system.electricity_out = system.fossils_in * 0.33
        return system


class DirectHeater(SupplySystemBase):
    """
    Created by PJ to check accuracy against previous simulation
    """

    def calc_loads(self):
        system = SupplyOut()
        system.electricity_in = self.load
        system.fossils_in = 0
        system.electricity_out = 0
        return system


class DirectCooler(SupplySystemBase):
    """
    Created by PJ to check accuracy against previous simulation
    """

    def calc_loads(self):
        system = SupplyOut()
        system.electricity_in = self.load
        system.fossils_in = 0
        system.electricity_out = 0
        return system


class SupplyOut:
    """
    The System class which is used to output the final results
    """
    fossils_in = float("nan")
    electricity_in = float("nan")
    electricity_out = float("nan")
    cop = float("nan")


sc.sticky["OilBoilerOld"] = OilBoilerOld
sc.sticky["OilBoilerMed"] = OilBoilerMed
sc.sticky["OilBoilerNew"] = OilBoilerNew
sc.sticky["HeatPumpAir"] = HeatPumpAir
sc.sticky["HeatPumpWater"] = HeatPumpWater
sc.sticky["ElectricHeating"] = ElectricHeating
sc.sticky["CHP"] = CHP
sc.sticky["DirectHeater"] = DirectHeater
sc.sticky["DirectCooler"] = DirectCooler
sc.sticky["SupplyDirector"] = SupplyDirector
print 'Supply systems are go!'