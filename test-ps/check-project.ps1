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
param (
    [Parameter(Mandatory = $true,
            HelpMessage = 'The TIA Project absolute (full) path for test',
            Position = 0)][String]$ProjectPath = "C:\Projects\TIATest\TIAtest.ap15_1"
)

# check available TIA API
$tiaref = "HKLM:\SOFTWARE\Siemens\Automation\Openness\15.1\PublicAPI\15.1.0.0"
$asmname = "Siemens.Engineering"

$TIA_ok = $false

# Try to get the registry item
$ri = Get-ItemProperty -Path $tiaref -Name $asmname -ErrorAction SilentlyContinue -ErrorVariable ec
if ($ri -ne $null) {
    $TIA_ok = $true
    $asm = $ri.$asmname    
} else {     
  	Write-Error "Error: TIA Openness V15.1 seems to be not installed"
    Exit 
}

if ($TIA_ok) {
    $res = [Reflection.Assembly]::LoadFrom($asm)
    $tp = New-Object Siemens.Engineering.TiaPortal(0) # 0 = without GUI

    
    # Trying to open and then close the project
    # NB1: The argument 'sourcePath' cannot be a relative path."
    $prj = $tp.Projects.Open($ProjectPath)
    if ($prj -ne $null) {
        Write-Host "Project exist and can be used"
        $prj.Close()
    }    
    # End work
    $tp.Dispose()
}