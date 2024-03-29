# NSIS script for creating the Hive installer
SetCompressor /FINAL lzma

; include logic library
!include 'LogicLib.nsh'

; include the modern UI stuff
!include "MUI2.nsh"

# icon stuff
#!define MUI_ICON "hive.ico"

Name "Hive"

!define MUI_FILE "savefile"
!define MUI_BRANDINGTEXT "Hive"
CRCCheck On


OutFile "Setup_Hive.exe"


;--------------------------------
;Folder selection page

InstallDir "$APPDATA\Grasshopper\Libraries\Hive"

;Request application privileges for Windows Vista
RequestExecutionLevel user

;--------------------------------
;Interface Settings

!define MUI_ABORTWARNING

;--------------------------------
;Pages

!insertmacro MUI_PAGE_LICENSE "..\LICENSE"
!insertmacro MUI_PAGE_DIRECTORY
!insertmacro MUI_PAGE_COMPONENTS
!insertmacro MUI_PAGE_INSTFILES

!insertmacro MUI_UNPAGE_CONFIRM
!insertmacro MUI_UNPAGE_INSTFILES

;--------------------------------
;Languages

  !insertmacro MUI_LANGUAGE "English"

;--------------------------------
;Installer Sections

Section "Hive" Base_Installation_Section
    SectionIn RO  # this section is required
    SetOutPath "$INSTDIR"

    # first, delete any "old" files (some of them have been renamed...)
    Delete /REBOOTOK "$INSTDIR\Hive.Core.epw_reader.ghpy"
    Delete /REBOOTOK "$INSTDIR\GHSolar.gha"
    Delete /REBOOTOK "$INSTDIR\ProvingGround.Conduit.gha"
    Delete /REBOOTOK "$INSTDIR\Hive.IO.gha"
    Delete /REBOOTOK "$INSTDIR\Hive.ProvingGround.Conduit.gha"
    Delete /REBOOTOK "$INSTDIR\honey-badger-runtime.dll"
    Delete /REBOOTOK "$INSTDIR\Newtonsoft.Json.dll"
    Delete /REBOOTOK "$INSTDIR\OxyPlot.dll"
    Delete /REBOOTOK "$INSTDIR\OxyPlot.WindowsForms.dll"
    Delete /REBOOTOK "$INSTDIR\Hive.Core.sia380.ghpy"
    Delete /REBOOTOK "$INSTDIR\Hive.Core.sia380.gha"
    Delete /REBOOTOK "$INSTDIR\Hive.Core.solar.gha"
    Delete /REBOOTOK "$INSTDIR\Hive.Core.solar.ghpy"
    Delete /REBOOTOK "$INSTDIR\SolarModel.dll"
    Delete /REBOOTOK "$INSTDIR\Hive.Core.combustion.ghpy"
    Delete /REBOOTOK "$INSTDIR\Hive.Core.cooling.ghpy"   
    Delete /REBOOTOK "$INSTDIR\Hive.Core.heatpumps.ghpy"   
    Delete /REBOOTOK "$INSTDIR\Hive.Core.solar_tech.ghpy" 
    Delete /REBOOTOK "$INSTDIR\Hive.Core.solar_tech.gha"
    
    # Also delete files from previous install location (Appdata\Grasshopper\Libraries)
    Delete /REBOOTOK "$INSTDIR\..\Hive.Core.epw_reader.ghpy"
    Delete /REBOOTOK "$INSTDIR\..\Hive.IO.gha"
    Delete /REBOOTOK "$INSTDIR\..\honey-badger-runtime.dll"
    Delete /REBOOTOK "$INSTDIR\..\Newtonsoft.Json.dll"
    Delete /REBOOTOK "$INSTDIR\..\OxyPlot.dll"
    Delete /REBOOTOK "$INSTDIR\..\OxyPlot.WindowsForms.dll"
    Delete /REBOOTOK "$INSTDIR\..\Hive.Core.sia380.ghpy"
    Delete /REBOOTOK "$INSTDIR\..\Hive.Core.sia380.gha"
    Delete /REBOOTOK "$INSTDIR\..\Hive.Core.solar.gha"
    Delete /REBOOTOK "$INSTDIR\..\Hive.Core.solar.ghpy"
    Delete /REBOOTOK "$INSTDIR\..\SolarModel.dll"
    Delete /REBOOTOK "$INSTDIR\..\Hive.Core.combustion.ghpy"
    Delete /REBOOTOK "$INSTDIR\..\Hive.Core.cooling.ghpy"   
    Delete /REBOOTOK "$INSTDIR\..\Hive.Core.heatpumps.ghpy"   
    Delete /REBOOTOK "$INSTDIR\..\Hive.Core.solar_tech.ghpy"
    Delete /REBOOTOK "$INSTDIR\..\Hive.Core.solar_tech.gha"

    # next, copy the "new files"

    # Hive.IO and dependencies
    File /oname=Hive.IO.gha "..\src\Hive.IO\Hive.IO\bin\Hive.IO.dll"
    File "..\src\Hive.IO\Hive.IO\bin\Newtonsoft.Json.dll"
    File "..\src\Hive.IO\Hive.IO\bin\OxyPlot.dll"
    File "..\src\Hive.IO\Hive.IO\bin\OxyPlot.WindowsForms.dll"
    File "GHSolar.gha"
    File "SolarModel.dll"
    File "Hive.ProvingGround.Conduit.gha"
    
    #IfFileExists "$INSTDIR\..\ProvingGround.Conduit.gha" 0 file_not_found
    #goto end_of_block
    #file_not_found:
    #File "ProvingGround.Conduit.gha"
    #end_of_block:


    # Hive.Core and dependencies
    # File "..\src\Hive.Core\epw_reader\_build\Hive.Core.epw_reader.ghpy"
    # File "..\src\Hive.Core\sia380\_build\Hive.Core.sia380.ghpy"
    # File "..\src\Hive.Core\sia380\_build\Hive.Core.sia380.gha"
    # File "..\src\Hive.Core\solar\_build\Hive.Core.solar.ghpy"
    # File "..\src\Hive.Core\solar_tech\_build\Hive.Core.solar_tech.ghpy"    
    # File "..\src\Hive.Core\solar_tech\_build\Hive.Core.solar_tech.gha"
    # File "..\src\Hive.Core\combustion\_build\Hive.Core.combustion.ghpy"
    # File "..\src\Hive.Core\cooling\_build\Hive.Core.cooling.ghpy"
    # File "..\src\Hive.Core\heatpumps\_build\Hive.Core.heatpumps.ghpy"

    # File "honey-badger-runtime.dll"        

SectionEnd

Section "Conduit" Conduit_Installation_Section

SectionIn RO

IfFileExists "$INSTDIR\..\ProvingGround.Conduit.gha" 0 file_not_found
    goto end_of_block
    file_not_found:
    File "ProvingGround.Conduit.gha"
    end_of_block:

SectionEnd

LangString DESC_Section1 ${LANG_ENGLISH} "Installs the Hive plugin for Grasshopper."
LangString DESC_Section2 ${LANG_ENGLISH} "Installs the Conduit plugin by Proving Ground Apps. Visit https://apps.proving$\nground.io/conduit/ for more information. If Conduit is already installed at the default location, this installation will be skipped."

!insertmacro MUI_FUNCTION_DESCRIPTION_BEGIN
  !insertmacro MUI_DESCRIPTION_TEXT ${Base_Installation_Section} $(DESC_Section1)
  !insertmacro MUI_DESCRIPTION_TEXT ${Conduit_Installation_Section} $(DESC_Section2)
!insertmacro MUI_FUNCTION_DESCRIPTION_END