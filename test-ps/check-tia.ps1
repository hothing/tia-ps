<#PSScriptInfo

.VERSION 1.0

.GUID 57ee66e4-3ac1-5bae-87de-a85e13d7f001

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
The script check that TIA Portal and Openness are available 

.DESCRIPTION 

.PARAMETER CheckVersion

#> 
param (
    [Parameter(Mandatory = $true,
            HelpMessage = 'The TIA Portal version',
            Position = 0)][String]$CheckVersion = "15.1"
)

# check available TIA API
#$tiaref15_1 = "HKLM:\SOFTWARE\Siemens\Automation\Openness\15.1\PublicAPI\15.1.0.0"
#$tiaref14_sp1 = "HKLM:\SOFTWARE\Siemens\Automation\Openness\14.0\PublicAPI\14.0.1.0"
$tiaref = "HKLM:\SOFTWARE\Siemens\Automation\Openness\$CheckVersion\PublicAPI\$CheckVersion.0.0"
<#
It seems that lowest version for TIA Openes should be a 14.0.1 aka 14SP1.
If you look to the APIs supported by TIA V15.1 you will see it.
#>

$asmname = "Siemens.Engineering"

$TIA_ok = $false

# Try to get the registry item
$ri = Get-ItemProperty -Path $tiaref -Name $asmname -ErrorAction SilentlyContinue -ErrorVariable ec
if ($ri -ne $null) {
    $TIA_ok = $true
    $asm = $ri.$asmname    
} else {     
  	Write-Error "Error: TIA Openness seems to be not installed"
    Exit 
}

if ($TIA_ok) {
    try {
        $res = [Reflection.Assembly]::LoadFrom($asm)
        $tp = New-Object Siemens.Engineering.TiaPortal(0) # 0 = without GUI
        # End work
        $tp.Dispose()
    } 
    catch {
        Write-Error "TIA Openness is not operable"
        Exit
    }
    Write-Host "TIA Openness is operable"
}