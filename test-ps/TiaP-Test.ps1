<#PSScriptInfo

.VERSION 1.0

.GUID 57ee66e4-3ac1-5bae-87de-a85e13d7f002

.AUTHOR barbos@inbox.ru

.COMPANYNAME Hothing Ltd

.COPYRIGHT GPL v3

.TAGS 

.LICENSEURI https://www.gnu.org/licenses/gpl-3.0.txt

.PROJECTURI 

.ICONURI 

.EXTERNALMODULEDEPENDENCIES 

.REQUIREDSCRIPTS 

.EXTERNALSCRIPTDEPENDENCIES 

.RELEASENOTES

#>

<# 
.SYNOPSIS
The script check an ability to open and close a TIA project 

.DESCRIPTION 

.PARAMETER ProjectPath

#>
<# 
param (
    [Parameter(Mandatory = $true,
            HelpMessage = 'The TIA Project absolute (full) path for test',
            Position = 0)][String]$ProjectPath = "C:\Projects\TIATest\TIAtest.ap15_1"
)
#>

$TiaVersionsAttr = @(
 # V14 SP1
 @{
    RegMainKey = "14.0"
    RegAPIKey = "14.0.0.0"
    NameExt = "ap14"
 }
 # V15.0
 @{
    RegMainKey = "15.0"
    RegAPIKey = "15.0.0.0"
    NameExt = "ap15"
 }
 
 # V15.1
 @{
    RegMainKey = "15.1"
    RegAPIKey = "15.1.0.0"
    NameExt = "ap15_1"
 }
 
 # V16.0
 @{
    RegMainKey = "16.0"
    RegAPIKey = "16.0.0.0"
    NameExt = "ap16"
 }
 
)

Function Init-TiaPortal 
{
    $TIA_ok = $false
    $t = 1
    $asmname = "Siemens.Engineering"

    while (($t -le $TiaVersionsAttr.Count) -and -not $TIA_ok) {
        $tiaref = "HKLM:\SOFTWARE\Siemens\Automation\Openness\{0}\PublicAPI\{1}" -f ($TiaVersionsAttr[$t]['RegMainKey'], $TiaVersionsAttr[$t]['RegApiKey'])
        # Try to get the registry item
        $ri = Get-ItemProperty -Path $tiaref -Name $asmname -ErrorAction SilentlyContinue -ErrorVariable ec
        if ($ri -ne $null) {
            $TIA_ok = $true
            $asm = $ri.$asmname    
        }
        $t = $t + 1
    }
    
    if ($TIA_ok) {
        $res = [Reflection.Assembly]::LoadFrom($asm)
        $tp = New-Object Siemens.Engineering.TiaPortal(0) # 0 = without GUI
    }
    $tp
}

Function Get-FullProjectName
{
}

Function Get-DeviceNameList
{
    param (
    [Parameter(Mandatory = $true,ValueFromPipeline=$true,
            HelpMessage = 'The device path',
            Position = 0)]$DevicePath
            )
    
}

Function Get-Device
{
    param (
    [Parameter(Mandatory = $true,ValueFromPipeline=$true,
            HelpMessage = 'The TIA Project reference',
            Position = 0)]$Project,
    [Parameter(Mandatory = $true,
            HelpMessage = 'The device name',
            Position = 1)]$DeviceName
            )
}

Function Get-Software
{
}