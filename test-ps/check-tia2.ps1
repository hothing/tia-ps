$tiaref0 = "HKLM:\SOFTWARE\Siemens\Automation\Openness\"
$tiaversion = "15.1"
$asmname = "Siemens.Engineering"

# Try to get the registry item
$TIA_ok = $false
# Find major installed version of TIA Openness
$ref0 = Get-ChildItem -Path $tiaref0 | Sort-Object | Select-Object -last 1
# Select an apropriate version
$ref1 = Get-Item -Path ($ref0.PSPath + "\PublicAPI") | Get-ChildItem  | where-Object { $_.Name.ToString() -like "*\$tiaversion.?.?" }

# Find and load the assmebly    
$ri = Get-ItemProperty -Path $ref1.PSPath -Name $asmname -ErrorAction SilentlyContinue -ErrorVariable ec
if ($ri -ne $null) {
    $TIA_ok = $true
    $asm = $ri.$asmname    
} else {     
  	Write-Error "Error: TIA Openness seems to be not installed"
    Exit 
}

try {
    $res = [Reflection.Assembly]::LoadFrom($asm)
    $TiaMode = 1 # with GUI
    if ($WithoutGui) { 
        $TiaMode = 0 # without GUI 
    }
    $tp = New-Object Siemens.Engineering.TiaPortal($TiaMode) 
} 
Catch {
    Write-Error "TIA Openness is not operable"
    Exit
}

function Get-DevComposition {
    param([Parameter(Mandatory=$true, ValueFromPipeline=$true)]$device, [int]$Id = 0)
    if ($device.DeviceItems -ne $null) {
        return $device.DeviceItems[$Id]
    } else {
        return $null
    }
}

function Plug-DevItem {
    param([Parameter(Mandatory=$true, ValueFromPipeline=$true)]$device, [string]$OrderNum, [string]$Name, [int]$Position)
    if ($device -ne $null) {
        if ($device.CanPlugNew("OrderNumber:$ordernum", $name, $position)) {
            $device.PlugNew("OrderNumber:$ordernum", $name, $position)
        }
        return $device
    } else {
        return $null
    }
}

function Test-DevItemPluggable {
    param([Parameter(Mandatory=$true, ValueFromPipeline=$true)]$device, [string]$OrderNum, [string]$Name, [int]$Position)
    if ($device -ne $null) {        
        return $device.CanPlugNew("OrderNumber:$ordernum", $name, $position)
    } else {
        return $false
    }
}

# Some stupid tasks 
# Task 1: create an empty project
$tp.Projects.Create("C:\Users\Administrator\Documents\Automation", "Test2")
# Task 2a: add a PLC unit *empty
$dev1 = $tp.Projects[0].Devices.Create("System:Device.S71500", "PLC_1")
# Task 2b: make the items of unit

if ((Test-DevItemPluggable -device $dev1 -OrderNum "6ES7 590-1***0-0AA0" -Name "Rack_1" -Position 0) -ne $null) {
    # insert the rack
    $dev1 | Plug-DevItem -OrderNum "6ES7 590-1***0-0AA0" -Name "Rack_1" -Position 0
    # insert CPU into the rack  
    $dev1 | Get-DevComposition | Plug-DevItem -OrderNum "6ES7 515-2AM02-0AB0/V2.8" -Name "-A100" -Position 1
}
# Task 3: add PLC unit with CPU
$dev2 = $tp.Projects[0].Devices.CreateWithItem("OrderNumber:6ES7 517-3AP00-0AB0/V2.8", "-A200", "PLC_2")
# Task 3b: add IO-module(s)
$dev2 | Get-DevComposition |
    Plug-DevItem -OrderNum "6ES7 523-1BL00-0AA0/V1.0" -Name "-A202" -Position 2 |
    Plug-DevItem -OrderNum "6ES7 534-7QE00-0AB0/V1.0" -Name "-A203" -Position 3 
# Task 4: add ET200SP/PN and connect to the PLC_1
# TypeIdentifier : System:Device.ET200SP
$dev3 = $tp.Projects[0].Devices.CreateWithItem("OrderNumber:6ES7 155-6AU01-0BN0/V4.1", "-A300", "RIO_1")
$dev3 | Get-DevComposition |
    Plug-DevItem -OrderNum "6ES7 131-6BH01-0BA0/V0.0" -Name "-A301" -Position 1 |
    Plug-DevItem -OrderNum "6ES7 132-6BF00-0CA0/V2.0" -Name "-A302" -Position 2 

# Task 5: Crate a PROFINET network
##$net1 = $tp.Projects[0].Subnets.Create("System:Subnet.Ethernet", "PROFINET_1")
# Task 6: connect PLC_1 and ET200SP/PN to the PROFINET network