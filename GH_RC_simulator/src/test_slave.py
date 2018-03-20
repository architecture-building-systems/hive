# This component confirms that the model outputs match predefined values
#
# Nest: A energy simulation plugin developed by the A/S chair at ETH Zurich
#
# This component is based on tests\testRCmodel.py (accessed 2/22/2018) 
# in the RC_BuildingSimulator Github repository:
# https://github.com/architecture-building-systems/RC_BuildingSimulator
# Documentation is available on the project wiki.
#
# Author: Justin Zarb <zarbj@student.ethz.ch>
#
# This file is part of Nest
#
# Licensing/Copyright and liability comments go here.
# <Copyright 2018, Architecture and Building Systems - ETH Zurich>
# <Licence: MIT>

"""
Use this component to run standard tests on the RC model within the GH environment.
This test ensures that the grasshopper component returns the same results as the python model.
-
Provided by Nest 0.0.1
    
    Args:
        mass_temperature: The mass node temperature for the hour simulated
        lighting_demand: lighting energy demand for the given time step
        heating_demand: heating energy demand required to maintain the heating setpoint temperature defined in the Zone.
        heating_sys_electricity: Heating electricity consumption
        cooling_sys_electricity: Cooling electricity consumption
        cop: coefficient of performance of the heating/cooling system for this hour
        run: index of test to run
    Returns:
        out: Message for whether test is passed or not
        test_passed: True if all tests have been passed.
"""

ghenv.Component.Name = "Unit Test Slave"
ghenv.Component.NickName = 'unit_test_slave'
ghenv.Component.Message = 'VER 0.0.1\nFEB_28_2018'
ghenv.Component.IconDisplayMode = ghenv.Component.IconDisplayMode.application
ghenv.Component.Category = "Nest"
ghenv.Component.SubCategory = "Simulation"
#compatibleNestVersion = VER 0.0.1\nFEB_21_2018
try: ghenv.Component.AdditionalHelpFromDocStrings = "3"
except: pass

import scriptcontext as sc

# Initialise results
results = {'mass_temperature':mass_temperature,
           'lighting_demand':lighting_demand,
           'heating_sys_electricity':heating_sys_electricity,
           'cooling_sys_electricity':cooling_sys_electricity,
           'energy_demand':energy_demand,
           'cop':cop}

def is_equal(a,b):
    try:
        assert a==round(b,2)
        return True
    except:
        return False

def run_test(test_number):
    # Create a truth dictionary based on whether each value matches
    test = {}
    for k in sc.sticky['expected_results'].keys():
        try:
            assert sc.sticky['expected_results'][k][test_number] is not None
            test[k] = (is_equal(sc.sticky['expected_results'][k][test_number],results[k]))
        except (KeyError,AssertionError): # there is no 'name' key in the results dictionary
            pass
    
    # if the test passes, just say it passed, otherwise return a breakdown of the test.
    test_passed = reduce(lambda x, y: x * y, test.values(), 1)
    if test_passed:
        return sc.sticky['expected_results']['name'][test_number]+' passed', True
    else: 
        message = [sc.sticky['expected_results']['name'][test_number]+' failed']
        for key in test.keys():
            value = str(round(results[key],2))+'/'+str(sc.sticky['expected_results'][key][test_number])
            token = 'passed' if test[key] else value
            message.append(key+':'+token)
        return message, False

x,test_passed = run_test(run)

# record result
if 'recL' in locals():
    if not (run in tests_run):
        tests_run.append(run)
        recL.append(x)
        
    # reset when slider goes to zero
    if run == 0:
        tests_run = [0]
        recL = [run_test(0)[0]]

else:
    recL = []
    tests_run=[]


# Print results
for test in recL:
    if type(test) is str:
        print test
    elif type(test) is list:
        print test[0]
        for t in range(1,len(test)):
            print '  ',test[t]

