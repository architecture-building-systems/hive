REM use honey-badger to build all the Hive.Core components and install them to Libraries

REM set up some environment variables to make this easier
REM (this assumes the IronPython installation folder and also the path to honey-badger.py)
SET IPY=C:\Program Files\IronPython 2.7\ipy.exe
SET HB=%USERPROFILE%\Documents\GitHub\honey-badger\honey-badger.py

"%IPY%" "%HB%" -i combustion\combustion.json
"%IPY%" "%HB%" -i cooling\cooling.json
"%IPY%" "%HB%" -i epw_reader\epw_reader.json
"%IPY%" "%HB%" -i heatpumps\heatpumps.json
"%IPY%" "%HB%" -i sia380\sia380.json
"%IPY%" "%HB%" -i solar\solar.json
"%IPY%" "%HB%" -i solar_tech\solar_tech.json