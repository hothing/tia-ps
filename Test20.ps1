$modPath = "C:\Users\simatic\source\repos\tia-ps\TiaPsCmdlet\TiaCmdlet\bin\Debug\net462\TiaCmdlet.dll" 
Import-Module $modPath


$DebugPreference = "Continue"

$tp = Get-TiaInstance

$prj = ($tp | Get-TiaProject)

$dev = ($prj | Get-TiaDeviceList -Path "G1" | where-object Name -eq -Value "D1_1")

$prg = Get-TiaProgram -Device $dev -Name "C1_1"

$x = Get-TiaPlcPartition -i $prg -n PROGRAM