# CORE

CORE is the folder containing all simulation engines and databases HIVE uses:

- solar simulation
- SIA380.1
- SIA 2024
- etc...


These components are in principle stand-alone. They can be build either traditionally in Visual Studio, or with [Honey-badger](https://github.com/architecture-building-systems/honey-badger).

They are all packaged together in the `Hive/Mothercell`.


## Projects currently in this repository:

### **RC_simulator** 

A native energy simulation toolset for Grasshopper! A series of user objects based on resistor-capacitor model in the [RC_BuildingSimulator](https://github.com/architecture-building-systems/RC_BuildingSimulator) repository. For tool documentation and developer's notes check the [project wiki](https://github.com/architecture-building-systems/hive/wiki/Hive_RC_simulator). Currently still under development.

**Demo** As a proof of concept of the tool, a parametric shading surface is applied to a window. The energy impact of the different shading length can be viewed in real time in Grasshopper. This can be used to optimize or compare geometries.

![Hourly radiation calculations](https://github.com/architecture-building-systems/hive/blob/master/repository_files/radiation1.mp4)

![Some shading tests](https://github.com/architecture-building-systems/hive/blob/master/repository_files/radiation2.mp4)

![How to set up your first demonstration](https://github.com/architecture-building-systems/hive/blob/master/repository_files/zone.mp4)

![Real-world application](https://github.com/architecture-building-systems/hive/blob/master/repository_files/case_study.mp4)

## Other projects in the pipeline:

* **Net-zero energy building assesment** (Laura Cowie): Workflow sets in Ladybug and Honeybee for assessing design options for a net-zero building refurbishment.

* **Panelling tool** (Stefan Caranovic): A panelling tool recently circulated within the chair. A possible enhancement of this tool would be to include PV panel visualisation (Linus Walker).
