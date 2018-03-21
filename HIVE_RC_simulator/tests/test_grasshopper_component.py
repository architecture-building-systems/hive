"""
Aims:
1. Compare the results generated in the ghpython simulation to a python-run model, for the same inputs
2. embed the tests from testRCmodel into grasshopper. (should auto-update to match python unit tests)

Methodology:
1. load inputs and outputs from grasshopper

2. Run simulation using inputs generated in grasshopper, over the period specified in grasshopper

- default ghpython model should be the same as the python model.
- make sure that all the inputs are the same
    - internal gains, solar gains (probably need to be taken from grasshopper)
- assert that the simulation outputs are also the same.
"""
import os, sys


import unittest
import pandas as pd

# Load grasshopper results and combine them into a single data frame
gh_result = pd.read_csv('grasshopper_result.csv')
gh_attr = pd.read_csv('grasshopper_attributes.csv', delimiter=':', index_col=0).to_dict()['Value']
print (gh_attr)

for k,v in gh_attr.items():
    try:
        v=float(v)
    except ValueError:
        pass

print (type(gh_attr['room_depth']))

# Todo: Figure out a better way to do this
sys.path.insert(0, os.path.abspath(
    os.path.join(os.path.dirname(__file__), '..', '..')))
from building_physics import Building  # Importing Building Class


class TestSimulation(unittest.TestCase):


    def test_gh_results(self):
        # This should be the default rc zone, or a replication of the zone in grasshopper.
        TestZone = Building(window_area=gh_attr['window_area'],
                          external_envelope_area= gh_attr['external_envelope_area'],
                          room_depth=gh_attr['room_depth'],
                          room_width=gh_attr['room_width'],
                          room_height=gh_attr['room_height'],
                          lighting_load=gh_attr['lighting_load'],
                          lighting_control=gh_attr['lighting_control'],
                          lighting_utilisation_factor=gh_attr['lighting_utilisation_factor'],
                          lighting_maintenance_factor=gh_attr['lighting_maintenance_factor'],
                          u_walls=gh_attr['u_walls'],
                          u_windows=gh_attr['u_windows'],
                          ach_vent=gh_attr['ach_vent'],
                          ach_infl=gh_attr['ach_infl'],
                          ventilation_efficiency=gh_attr['ventilation_efficiency'],
                          thermal_capacitance_per_floor_area=gh_attr['thermal_capacitance_per_floor_area'],
                          t_set_heating=gh_attr['t_set_heating'],
                          t_set_cooling=gh_attr['t_set_cooling'],
                          max_cooling_energy_per_floor_area=gh_attr['max_cooling_energy_per_floor_area'],
                          max_heating_energy_per_floor_area=gh_attr['max_heating_energy_per_floor_area'],
                          heating_supply_system=gh_attr['heating_supply_system'],
                          cooling_supply_system=gh_attr['cooling_supply_system'],
                          heating_emission_system=gh_attr['heating_emission_system'],
                          cooling_emission_system=gh_attr['cooling_emission_system'])
        t_m_prev = 20

        # Empty Lists for Storing Data to Plot
        ElectricityOut = []
        HeatingDemand = []  # Energy required by the zone
        HeatingEnergy = []  # Energy required by the supply system to provide HeatingDemand
        CoolingDemand = []  # Energy surplus of the zone
        CoolingEnergy = []  # Energy required by the supply system to get rid of CoolingDemand
        IndoorAir = []

        for h in range(0,len(gh_result)):
            internal_gains = gh_result.loc[h,'internal_gains']
            solar_gains = gh_result.loc[h,'solar_gains']
            t_out = gh_result.loc[h, 'outdoor_air_temperature']
            ill = gh_result.loc[h, 'ill']
            occupancy = gh_result.loc[h,'occupancy']

            TestZone.solve_building_energy(internal_gains, solar_gains, t_out, t_m_prev)
            TestZone.solve_building_lighting(ill, occupancy)

            ElectricityOut.append(TestZone.electricity_out)
            HeatingDemand.append(TestZone.heating_demand)
            CoolingDemand.append(TestZone.cooling_demand)
            IndoorAir.append(TestZone.t_air)

        indoor_air_round = [round(x,2) for x in list(gh_result.indoor_air_temperature)]
        IndoorAir_round = [round(x,2) for x in IndoorAir]
        for i, j in zip(indoor_air_round, IndoorAir_round):
            print(i,j)

        self.assertListEqual(indoor_air_round, IndoorAir_round)



if __name__ == '__main__':
    unittest.main()