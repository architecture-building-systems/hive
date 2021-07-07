@REM Script to build all solutions and create installer

@REM Change these paths accordingly
@REM Requires python 2.7, likely from conda env.
SET PYTHON=python
SET IPY=%PROGRAMFILES%\IronPython 2.7\ipy.exe
SET HB=C:\Users\Christoph\Documents\GitHub\honey-badger\honey-badger.py
SET MSBUILD=%PROGRAMFILES(x86)%\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild.exe
SET MAKENSIS=%PROGRAMFILES(X86)%\NSIS\makensis.exe

SET HIVEWIKIPY=C:\Users\Christoph\Documents\GitHub\hive.wiki\hbwiki.py

@REM Build Hive.Core
echo Building Hive.Core...
cd ..\src\Hive.Core\
"%IPY%" "%HB%" epw_reader\Hive.Core.epw_reader.json
"%IPY%" "%HB%" sia380\Hive.Core.sia380.json
"%IPY%" "%HB%" solar\Hive.Core.solar.json
"%IPY%" "%HB%" solar_tech\Hive.Core.solar_tech.json
"%IPY%" "%HB%" combustion\Hive.Core.combustion.json
"%IPY%" "%HB%" cooling\Hive.Core.cooling.json
"%IPY%" "%HB%" heatpumps\Hive.Core.heatpumps.json
echo ...Done

@REM Build Hive.IO (only the main csproj, not tests)
echo Building Hive.IO...
cd ..\Hive.IO\Hive.IO\
%PYTHON% Building\compile_sia2024_json.py
%PYTHON% Building\generate_sia2024_schedules_json.py
"%MSBUILD%" Hive.IO.csproj /p:PreBuildEvent="" /p:PostBuildEvent=""
cd ..\..\..\
echo ...Done

@REM Build Installer
echo Building Installer Setup_Hive.exe...
cd setup\
"%MAKENSIS%" hive.nsi
echo ...Done

@REM Generating Component Documentation for Wiki
echo Generating Component Documentation for Wiki...
cd ..\src\Hive.Core\
"%IPY%" "%HIVEWIKIPY%" epw_reader\Hive.Core.epw_reader.json
"%IPY%" "%HIVEWIKIPY%" sia380\Hive.Core.sia380.json
"%IPY%" "%HIVEWIKIPY%" solar\Hive.Core.solar.json
"%IPY%" "%HIVEWIKIPY%" solar_tech\Hive.Core.solar_tech.json
"%IPY%" "%HIVEWIKIPY%" combustion\Hive.Core.combustion.json
"%IPY%" "%HIVEWIKIPY%" cooling\Hive.Core.cooling.json
"%IPY%" "%HIVEWIKIPY%" heatpumps\Hive.Core.heatpumps.json

cd ..\..\setup\
echo ...Done

pause