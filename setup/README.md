# Setup

## Install Hive (Windows)

Simply run `Setup_Hive.exe`.

## Create Installer (Windows)

[build.cmd](build.cmd) rebuilds the installer by:

- Building the [Hive.IO](../src/Hive.IO) C# Grasshopper components / assemblies
- Running Python compiling scripts to serialise data sources into JSONs
- Creating [the installer](Setup_Hive.exe) using NSIS and based on the [hive.nsi](hive.nsi) script
- Regenerating the Wiki pages for the hive repository using a python script (note this is done in a separate repository called hive.wiki!).


### Requirements

Before running build.cmd, you need:

- An activated `venv` or `conda` environment with python 2.7 and the libararies listed in the [pip requirements file](requirements.txt). Below is a sample of the commands you will need (in a conda prompt!). [Also see this guide for conda setup](https://docs.conda.io/projects/conda/en/latest/user-guide/getting-started.html). 

  ```sh
  conda create -n hive
  conda activate hive
  conda install python=2.7
  conda install pip
  cd \path\to\hive\setup
  pip install -r requirements.txt
  ```

- MSBuild (installed with Visual Studio, assumes v2019 Community)
- [NSIS](https://nsis.sourceforge.io/Download)
- [Solar model](https://github.com/christophwaibel/GH_Solar_V2) (the `dll` and `gha` are already deposited here in Hive, but the repo is where you could get the newest version)
- If you want to regenerate / update the wiki pages, you will need to clone the hive.wiki repo:
  ```git
  git clone https://github.com/architecture-building-systems/hive.wiki.git
  ```

### Building
Once you are ready:

1. Check / Update paths in `build.cmd` for repository / exe locations on your machine
2. Open a conda prompt and activate your environment for hive (with python 2.7)
3. Run `build.cmd`. Check for errors in the console output.
4. Run the installer with `start Setup_Hive.exe`

Example
```sh
conda activate hive
build.cmd
# ...some output...
start Setup_Hive.exe
```

You now have your latest changes of hive installed!

### Troubleshooting

- python not recognised on your machine
  - make sure you are in a conda prompt (if using conda), have activated your environment and that it has python 2.7 installed!
  - sometimes you might have to install python 2.7 separately on your machine, but an environment should suffice.
- "couldn't find path" kind of errors when trying to compile python components
  - check the paths for IronPython and for the honey-badger.py script
- hive wiki script runs properly but no changes on wiki page
  - remember you need to push changes on the hive.wiki repo **separately** from the main hive repo.