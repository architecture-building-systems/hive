# MOTHERCELL

The Mothercell is the main Grasshopper component of Hive. 

It shall be a cluster of Hive components.
It shall serve as a GUI.
It shall be the main interface for inputs and outputs.

It shall be...

...the Mothercell.


## GUI and IO
GUI is the GHCluster, which can be opened to fiddle with the individual grasshopper components.

In order to get Rhino information into the Mothercell, we build some IO grassshopper components. They create Hive.IO objects, like Hive.IO.EnergySystems.PV, that are all fed into the Mothercell. The idea here is that the inputs are flexible, i.e. just throw in as many .IO objects as you want. The Mothercell should then take care of identifying the objects (easy, because they all have specific classes) and feed them into the right components inside the cluster. So much for the idea...

Input: Will only have geometry, and parameters (reference efficiencies, constructions, ...), but no simulations/calculations (irradiance and time resolved efficiencies etc happen inside the Mothercell).

![Hive Architecture](https://github.com/architecture-building-systems/hive/blob/master/repository_files/HiveIOGUI.png)
