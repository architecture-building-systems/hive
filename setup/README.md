# Setup

## Install Hive (Windows)

Simply run Setup_Hive.exe.

## Create Installer (Windows)

Use build.cmd to rebuild Hive.IO, Hive.Core and then create the installer (Setup_Hive.exe) (based on the hive.nsi script)

### Requirements

To run build.cmd you need:

- An activated `venv` or `conda` environment setup that includes python 2.7 (for running compiling JSON as data sources). [See this guide for conda setup](https://docs.conda.io/projects/conda/en/latest/user-guide/getting-started.html)
- MSBuild (installed with Visual Studio, assumes v2019 Community)
- Clone the [Honey Badger](https://github.com/architecture-building-systems/honey-badger) repository, as we need the python script.
  - honey-badger-runtime.dll is included here and can be [downloaded from here](https://github.com/architecture-building-systems/honey-badger/blob/master/honey-badger-runtime/bin/honey-badger-runtime.dll)
- [IronPython 2.7.8](https://github.com/IronLanguages/ironpython2/releases/tag/ipy-2.7.8) (standalone installation, v2.7.8 for Rhino 6 / 7 compatibility, see honey badger repo readme for details)
- [NSIS](https://nsis.sourceforge.io/Download)
- [Solar model](https://github.com/christophwaibel/GH_Solar_V2) (the `dll` and `gha` are already deposited here in Hive, but the repo is where you could get the newest version)

## Updating Hive

1. Open a conda prompt and activate your environment for hive (with python 2.7)
2. Navigate to and run `build.cmd`. Check for errors in the console output.
3. Run the installer with `start Setup_Hive.exe`

You now have your latest changes of hive installed!