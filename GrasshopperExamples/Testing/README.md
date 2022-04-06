Test files for debugging, prototyping and development.

- `Solar_Unobstructed_HiveCore.gh`: Unobstructed solar model, but with all Hive inputs (Building, Environment, EnSys) and the MainDistributor.

- `Hive_Results.gh`: Tests the results visualizer and the results outputter. No simulations.
- `Hive_ParametricInputs.gh`: Tests the parametric input components for Building, Environment and EnergySystems. No simulations.
- `Hive_EnvironmentAndEpwReader.gh`: Tests the Hive environment component and the EPW reader. No simulations. 
- `Hive_Form_EnergySystems.gh`: Tests the Hive EnergySystems input component. Also to test the WinForm. No simulations.
- `Hive_Form_Building.gh`: Tests the Hive Building input component. Also to test the WinForm. No simulations.
- `Hive_MainDistributor.gh`: Tests the main Hive distributor (the one that takes all inputs). No simulations.
- `Hive_BuildingGeometries.gh`: Tests different geometries/shapes for the Hive Building input component.

- `Core_Conversion_Emitter.gh`: Tests the conversion and emitter components, from Inputs to Hive Core. No simulations. 
- `Core_DemandHourly.gh`: Tests the hourly demand simulation, derived of monthly SIA 380.1 demand. RESULTS NOT VALIDATED. 