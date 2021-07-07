# Setup

## Install Hive (Windows)

Simply run Setup_Hive.exe.

## Create Installer (Windows)

Use build.cmd to rebuild Hive.IO, Hive.Core and then create the installer (Setup_Hive.exe) (based on the hive.nsi script)

### Requirements

To run build.cmd you need:

- MSBuild (installed with Visual Studio, assums v2019 Community)
- [Honey Badger](https://github.com/architecture-building-systems/honey-badger)
  - honey-badger-runtime.dll is included here and can be [downloaded from here](https://github.com/architecture-building-systems/honey-badger/blob/master/honey-badger-runtime/bin/honey-badger-runtime.dll)
- [IronPython 2.7.8](https://github.com/IronLanguages/ironpython2/releases/tag/ipy-2.7.8) (standalone installation, v2.7.8 for Rhino 6 / 7 compatibility, see honey badger repo readme for details)
- [NSIS](https://nsis.sourceforge.io/Download)
- [Solar model](https://github.com/christophwaibel/GH_Solar_V2) (the `dll` and `gha` are already deposited here in Hive, but the repo is where you could get the newest version)
