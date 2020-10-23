REM use honey-badger to build all the Hive.Core components and install them to Libraries

REM set up some environment variables to make this easier
REM (this assumes the IronPython installation folder and also the path to honey-badger.py)
SET IPY=C:\Program Files\IronPython 2.7\ipy.exe
SET HB=%USERPROFILE%\Documents\GitHub\honey-badger\honey-badger.py

"%IPY%" "%HB%" -i epw_reader\Hive.Core.epw_reader.json
"%IPY%" "%HB%" -i sia380\Hive.Core.sia380.json
"%IPY%" "%HB%" -i solar\Hive.Core.solar.json
"%IPY%" "%HB%" -i solar_tech\Hive.Core.solar_tech.json
"%IPY%" "%HB%" -i combustion\Hive.Core.combustion.json
"%IPY%" "%HB%" -i cooling\Hive.Core.cooling.json
"%IPY%" "%HB%" -i heatpumps\Hive.Core.heatpumps.json

pause