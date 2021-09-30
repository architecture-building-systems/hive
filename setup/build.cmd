@REM Script to build all solutions and create installer

@REM Base directories, assumes hive.wiki, honey_badger and hive share same root.
set HIVE_BUILD_DIR=%~dp0
set GIT_DIR=%HIVE_BUILD_DIR%..\..
SET HIVE_DIR=%HIVE_BUILD_DIR%..
SET HONEY_BADGER_DIR=%GIT_DIR%\honey-badger
SET HIVE_WIKI_DIR=%GIT_DIR%\hive.wiki

@REM Change these paths accordingly
@REM Requires python 2.7, likely from conda env.
SET PYTHON=python
SET IPY=%PROGRAMFILES%\IronPython 2.7\ipy.exe
SET HB=%HONEY_BADGER_DIR%\honey-badger.py
SET HIVE_WIKI_PY= %HIVE_WIKI_DIR%\hbwiki.py
SET MSBUILD=%PROGRAMFILES(x86)%\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild.exe
SET MAKENSIS=%PROGRAMFILES(X86)%\NSIS\makensis.exe

CD %HIVE_BUILD_DIR%

@REM Build Hive.Core
echo Building Hive.Core...
cd %HIVE_DIR%\src\Hive.Core\
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
cd %HIVE_DIR%\src\Hive.IO\Hive.IO\
%PYTHON% Building\compile_sia2024_json.py
%PYTHON% Building\generate_sia2024_schedules_json.py
%PYTHON% Building\compile_surface_tech_json.py
"%MSBUILD%" Hive.IO.csproj /p:PreBuildEvent="" /p:PostBuildEvent=""
echo ...Done

@REM Build Installer
echo Building Installer Setup_Hive.exe...
cd %HIVE_BUILD_DIR%
"%MAKENSIS%" hive.nsi
echo ...Done

@REM Generating Component Documentation for Wiki
echo Generating Component Documentation for Wiki...
cd %HIVE_DIR%\src\Hive.Core\
"%IPY%" "%HIVE_WIKI_PY%" epw_reader\Hive.Core.epw_reader.json
"%IPY%" "%HIVE_WIKI_PY%" sia380\Hive.Core.sia380.json
"%IPY%" "%HIVE_WIKI_PY%" solar\Hive.Core.solar.json
"%IPY%" "%HIVE_WIKI_PY%" solar_tech\Hive.Core.solar_tech.json
"%IPY%" "%HIVE_WIKI_PY%" combustion\Hive.Core.combustion.json
"%IPY%" "%HIVE_WIKI_PY%" cooling\Hive.Core.cooling.json
"%IPY%" "%HIVE_WIKI_PY%" heatpumps\Hive.Core.heatpumps.json

cd %HIVE_BUILD_DIR%
echo ...Done

pause