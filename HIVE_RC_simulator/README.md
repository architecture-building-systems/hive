# **HIVE RC_simulator** 

## Folder Description
* **auxiliary**: weather files and occupancy schedules

* **clipper**: clipper.dll file necessary for shading calculations

* **examples**: All the grasshopper files being used for development. These will be streamlined into simple examples.

* **src**: python source code for all the HIVE user objects

* **userObjects**: Hive user objects which need to be copied in the grasshopper userObjects folder


## Installation

Hive is still under development, but early testers are welcome! Installation instructions:

 1. Copy all the files in the userObjects folder into your local userObjects folder (see Grasshopper's <special folders>).

 2. Copy clipper.dll (from the clipper folder) to C:\Program Files\Rhinoceros 5.0 (64-bit)\System\clipper_library.dll

 3. Open hive_main_development.gh (working file and currently a mess) or hive_basic_setup.gh (a cleaner version showing the workflow)
