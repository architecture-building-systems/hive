# Setup

## Install Hive (Windows)

Simply run Setup_Hive.exe.

## Create Installer (Windows)

[build.cmd](build.cmd) rebuilds the installer by:

- Building the [Hive.IO](../src/Hive.IO) C# Grasshopper components / assemblies
- Building the [Hive.Core](../src/Hive.Core) Python Grasshopper components / assemblies via honey-badger
- Run Python compiling scripts to serialise data sources into JSONs
- Create [the installer](Setup_Hive.exe) using NSIS and based on the [hive.nsi](hive.nsi) script
- Regenerating the Wiki pages for the hive repository using a python script.

Before running build.cmd, you need:

- An activated `venv` or `conda` environment with python 2.7 the libararies listed in the [pip requirements file](requirements.txt). Below is a sample of the commands you will need (in a conda prompt!). [Also see this guide for conda setup](https://docs.conda.io/projects/conda/en/latest/user-guide/getting-started.html). 

  ```sh
  cd \path\to\here
  conda create -n hive
  conda activate hive
  conda install python=2.7
  conda install pip
  pip install -r requirements.txt
  ```

- Clone the [Honey Badger](https://github.com/architecture-building-systems/honey-badger) repository, as we need the python script.
  ```git
  git clone https://github.com/architecture-building-systems/honey-badger.git
  ```
  - honey-badger-runtime.dll is included here and can be [downloaded from here](https://github.com/architecture-building-systems/honey-badger/blob/master/honey-badger-runtime/bin/honey-badger-runtime.dll)
- MSBuild (installed with Visual Studio, assumes v2019 Community)
- [IronPython 2.7.8](https://github.com/IronLanguages/ironpython2/releases/tag/ipy-2.7.8) (standalone installation, v2.7.8 for Rhino 6 / 7 compatibility, see honey badger repo readme for details)
- [NSIS](https://nsis.sourceforge.io/Download)
- [Solar model](https://github.com/christophwaibel/GH_Solar_V2) (the `dll` and `gha` are already deposited here in Hive, but the repo is where you could get the newest version)

Once you are ready:

1. Check / Update paths in `build.cmd`for repository / exe locations
2. Open a conda prompt and activate your environment for hive (with python 2.7)

3. Navigate to and run `build.cmd`. Check for errors in the console output.
4. Run the installer with `start Setup_Hive.exe`

Example
```sh
conda activate hive
build.cmd
# ...some output...
start Setup_Hive.exe
```

You now have your latest changes of hive installed!