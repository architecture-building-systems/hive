# Setup

## Install Hive (Windows)

Simply run Setup_Hive.exe.

## Create Installer (Windows)

Use build.cmd to rebuild Hive.IO, Hive.Core and then create the installer (based on the hive.nsi script)

### Requirements

- MSBuild (installed with Visual Studio, assums v2019 Community)
- [Honey Badger](https://github.com/architecture-building-systems/honey-badger)
- [IronPython 2.7.8](https://github.com/IronLanguages/ironpython2/releases/tag/ipy-2.7.8) (standalone installation, v2.7.8 for Rhino 6 / 7 compatibility, see honey badger repo readme for details)
- [NSIS](https://nsis.sourceforge.io/Download)
