Import-Module .\PsTia15PlcCmdlet.dll

<#
 .Synopsis
 Start TIA Portal (only if appropriate version of TIA Openness is installed) 

 .Description
 The script tries to start TIA Portal. It checks a registry to find the path to a Tia.Net assembly.

 .Parameter WithoutGui
 This parameter says to start headless TIA

 .Example
 Start-TiaPortal

 .Example
 Start-TiaPortal -WithoutGui
 #>

function Start-TiaPortal {
param(
    [switch]$WithoutGui = $false
)
    # Try to get the registry item
    $TIA_ok = $false
    $tiaref0 = "HKLM:\SOFTWARE\Siemens\Automation\Openness\"
    $tiaversion = "15.1"
	$asmname = "Siemens.Engineering"

	# Try to get the registry item
	$TIA_ok = $false
	# Find major installed version of TIA Openness
	$ref0 = Get-ChildItem -Path $tiaref0 | Sort-Object | Select-Object -last 1
    if ($ref0 -eq $null) { Write-Error -Message "The registry record for TIA Openes is not found" -Category ResourceUnavailable -ErrorAction Stop}
	# Select an apropriate version
	$ref1 = Get-Item -Path ($ref0.PSPath + "\PublicAPI") | Get-ChildItem  | where-Object { $_.Name.ToString() -like "*\$tiaversion.?.?" }
    if ($ref1 -eq $null) { Write-Error -Message "The registry record for TIA Openes is not found" -Category ResourceUnavailable -ErrorAction Stop}
	
    # Find and load the assmebly    
	$ri = Get-ItemProperty -Path $ref1.PSPath -Name $asmname -ErrorAction SilentlyContinue -ErrorVariable ec
    if ($ri -ne $null) {
        $TIA_ok = $true
        $asm = $ri.$asmname    
    } else {     
  	    Write-Error "Error: TIA Openness seems to be not installed" -Category ResourceUnavailable -ErrorAction Stop
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
        Write-Error "TIA Openness is not operable" -Category ProtocolError -ErrorAction Stop
        Exit
    }
}

<#
 .Synopsis
 
 .Description
 
 .Parameter WithoutGui
 
 .Example
 
 .Example
 #>
function Test-ProjectVesrion
{
    [OutputType([int32])]
    param([Parameter(Mandatory=$true, ValueFromPipeline=$true)][string]$ProjectPath=".")
    return 0
}


<#
 .Synopsis
 
 .Description
 
 .Parameter Path
 
 .Parameter Name
 
 .Example
 
 .Example
 #>
function Create-TiaProject
{
    param(
        [Parameter(Mandatory=$true, ValueFromPipeline=$true)][string]$tiap,
        [Parameter(Mandatory=$true)][string]$Path=".",
        [Parameter(Mandatory=$true)][string]$Name
        )
    if ($tiap -eq $null) { Write-Error "TIA Openness is not operable" -Category ProtocolError -ErrorAction Stop }
    $r0 = Split-Path -Path $Path -IsAbsolute
    if (-not $r0) { $Path = Join-Path -Path (Split-Path $Path -Resolve) -ChildPath  $Path -Resolve}
    return $tiap.Projects.Create($Path, $Name) 
}

<#
 .Synopsis
 
 .Description
 
 .Parameter Path
 
 .Example
 
 .Example
 #>
function Open-TiaProject
{
    param(
        [Parameter(Mandatory=$true, ValueFromPipeline=$true)][string]$tiap,
        [Parameter(Mandatory=$true)][string]$Path="."
        )
    if ($tiap -eq $null) { Write-Error "TIA Openness is not operable" -Category ProtocolError -ErrorAction Stop }
    $r0 = Split-Path -Path $Path -IsAbsolute
    if (-not $r0) { $Path = Join-Path -Path (Split-Path $Path -Resolve) -ChildPath  $Path -Resolve}
    return $tiap.Projects.Open($Path)
}

<#
 .Synopsis
 
 .Description
 
 .Parameter WithoutGui
 
 .Example
 
 .Example
 #>
function Close-TiaProject
{
    param(
        [Parameter(Mandatory=$true, ValueFromPipeline=$true)][string]$tiap,
        [Parameter(Mandatory=$true)][string]$ProjectPath="."
        )
    if ($tiap -eq $null) { Write-Error "TIA Openness is not operable" -Category ProtocolError -ErrorAction Stop }
    $r0 = Split-Path -Path $ProjectPath -IsAbsolute
    if (-not $r0) { $ProjectPath = Join-Path -Path (Split-Path $ProjectPath -Resolve) -ChildPath  $ProjectPath -Resolve}
    #TODO: close the project by specified path
    #return $tiap.Open($ProjectPath) 
}

enum DeviceType {
    Any = 0
    PLC = 1
    RemoteIO = 2
}

<#
 .Synopsis
 
 .Description
 
 .Parameter Project

 .Parameter Name
 
 .Example
 
 .Example
 #>
function Get-TiaDevice
{
    [CmdletBinding(DefaultParameterSetName = 'ByName')]
    param(
        [Parameter(Mandatory=$true, ValueFromPipeline=$true)]$Project,
        
        [Parameter(Mandatory=$true, ParameterSetName = 'ByName')]
        [Parameter(Mandatory=$true, ParameterSetName = 'ByNameAll')][string]$Name="*",

        [Parameter(Mandatory=$true, ParameterSetName = 'ByClass')]
        [Parameter(Mandatory=$true, ParameterSetName = 'ByClassAll')][string]$Class="CPU",

        [Parameter(Mandatory = $true, ParameterSetName = 'ByNameAll')]
        [Parameter(Mandatory = $true, ParameterSetName = 'ByClassAll')]
        [switch]$All
        )
    begin {
        if ($tiap -eq $null) { Write-Error "TIA Openness is not operable" -Category ProtocolError -ErrorAction Stop }
        $devs = $Project.Devices[0].DeviceItems 
    }
    process {
        #TODO: get the device by specified name
        #return $Project.Devices[0].DeviceItems | where-object -Property Classification -eq "CPU"
        if ($Class)
        {
            if ($All) 
            {
                return $devs | where-object -Property Classification -imatch $Class
            } 
            else 
            {
                return $devs | where-object -Property Classification -eq $Class
            }
        }
        else {
            if ($All) 
            {
                return $devs | where-object -Property Name -imatch $Name
            } 
            else 
            {
                return $devs | where-object -Property Name -eq $Name
            }
        }
    }
}

Export-ModuleMember -Function Start-TiaPortal
Export-ModuleMember -Function Open-TiaProject
Export-ModuleMember -Function Close-TiaProject
Export-ModuleMember -Function Get-TiaDevice
Export-ModuleMember -Alias DeviceType