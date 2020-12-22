cd /d %~dp0

@del "..\bin\PublicAPI\AddIn\Loader\Siemens.Engineering.AddIn.Loader.x32.exe.adr"
@del "..\bin\PublicAPI\AddIn\Loader\Siemens.Engineering.AddIn.Loader.x64.exe.adr"

@copy "..\bin\PublicAPI\AddIn\Loader\Siemens.Engineering.AddIn.Loader.x32.exe.s_adr" "..\bin\PublicAPI\AddIn\Loader\Siemens.Engineering.AddIn.Loader.x32.exe.adr"
@copy "..\bin\PublicAPI\AddIn\Loader\Siemens.Engineering.AddIn.Loader.x64.exe.s_adr" "..\bin\PublicAPI\AddIn\Loader\Siemens.Engineering.AddIn.Loader.x64.exe.adr"