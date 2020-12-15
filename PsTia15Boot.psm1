<#
 .Synopsis
 The commandlet gets a list of available version of TIA Openness

 .Description
 The registry key HKLM:\SOFTWARE\Siemens\Automation\Openness\ is checking for presence,
 and then collect available version of TIA Openess

 .Example
 Get-TiaVersion

#>

function Get-TiaVersion {
[CmdletBinding()]
[OutputType([string[]])]
param()
    # Try to get the registry item
    $TIA_ok = $false
    $tiaref0 = "HKLM:\SOFTWARE\Siemens\Automation\Openness\"
    $asmname = "Siemens.Engineering"

	# Try to get the registry item
	$TIA_ok = $false
	# Find major installed version of TIA Openness
	$ref0 = Get-ChildItem -Path $tiaref0 | Split-Path -Leaf
    if ($ref0 -eq $null) { 
		Write-Error -Message "The registry record for TIA Openness is not found" -Category ResourceUnavailable -ErrorAction Stop
	}
	return $ref0
}


<#
 .Synopsis
 The commandlet finds a defined version of TIA Openness and return a path to .Net assembly

 .Description
 The registry key HKLM:\SOFTWARE\Siemens\Automation\Openness\ is checking for presence,
 and then a major version of TIA is selecting and then a requested version of API is getting.

 .Example
 Get-TiaOpenness

#>

function Get-TiaOpenness {
[CmdletBinding()]
param(
	[Parameter(Mandatory=$true, ValueFromPipeline=$true)]
    [string]$tiaversion = "15.1"
)
    # Try to get the registry item
    $TIA_ok = $false
    $tiaref0 = "HKLM:\SOFTWARE\Siemens\Automation\Openness\"
    $asmname = "Siemens.Engineering"

	# Try to get the registry item
	$TIA_ok = $false
	# Find major installed version of TIA Openness
	$ref0 = Get-ChildItem -Path $tiaref0 | Sort-Object | Select-Object -last 1
    if ($ref0 -eq $null) { 
		Write-Error -Message "The registry record for TIA Openness is not found" -Category ResourceUnavailable -ErrorAction Stop
	}
	# Select an apropriate version
	$ref1 = Get-Item -Path ($ref0.PSPath + "\PublicAPI") | Get-ChildItem  | where-Object { $_.Name.ToString() -like "*\$tiaversion.?.?" }
    if ($ref1 -eq $null) { 
		Write-Error -Message "The registry record for TIA Openness is not found" -Category ResourceUnavailable -ErrorAction Stop
	}
	
    # Find and load the assmebly    
	$ri = Get-ItemProperty -Path $ref1.PSPath -Name $asmname -ErrorAction SilentlyContinue -ErrorVariable ec
    if ($ri -ne $null) {
        $TIA_ok = $true
        $asm = $ri.$asmname    
    } else {     
  	    Write-Error "Error: TIA Openness seems to be not installed" -Category ResourceUnavailable -ErrorAction Stop
        Exit 
    }
	return $asm
}

<#
 .Synopsis
 The commandlet finds a TIA Openness by TIA vesrion and API version and then return a path to .Net assembly

 .Description
 The registry key HKLM:\SOFTWARE\Siemens\Automation\Openness\ is checking for presence the requested TIA version,
 and then selects API version and the return it.

 .Example
 Get-TiaOpenness2 -Version "15.0"

 .Example
 Get-TiaOpenness2 -Version "15.0"

#>

function Get-TiaOpenness2 {
[CmdletBinding()]
param(
	[Parameter(	Mandatory=$true, 
				ValueFromPipeline=$true, 
				HelpMessage = 'TIA Portal version (major)',
				Position = 0)]
    [string]$Version = "15.0"
	
	[Parameter(	Mandatory=$false, 
				HelpMessage = 'TIA Openess API Version (minor)',
				Position = 1)]
    [string]$ApiVersion = "15.1"
)
    # Try to get the registry item
    $TIA_ok = $false
    $tiaref0 = "HKLM:\SOFTWARE\Siemens\Automation\Openness\"
    $asmname = "Siemens.Engineering"

	# Try to get the registry item
	$TIA_ok = $false
	# Find a requested installed version of TIA Openness
	$ref0 = Get-ChildItem -Path ($tiaref0.PSPath + $Version)
    if ($ref0 -eq $null) { 
		Write-Error -Message "The registry record for TIA Openness is not found" -Category ResourceUnavailable -ErrorAction Stop
	}
	# Select an apropriate version
	if ($ApiVersion) {
		$ref1 = Get-Item -Path ($ref0.PSPath + "\PublicAPI") | 
				Get-ChildItem  | 
				where-Object { $_.Name.ToString() -like "*\$ApiVersion.?.?" }
		if ($ref1 -eq $null) { 
			Write-Error -Message "The registry record for TIA Openness is not found" -Category ResourceUnavailable -ErrorAction Stop
		}
	}
	else {
		$ref1 = Get-Item -Path ($ref0.PSPath + "\PublicAPI") | 
				Get-ChildItem  |
				Sort-Object |
				Select-Object -Last 1
		if ($ref1 -eq $null) { 
			Write-Error -Message "The registry record for TIA Openness is not found" -Category ResourceUnavailable -ErrorAction Stop
		}
	}
	
    # Find and load the assmebly    
	$ri = Get-ItemProperty -Path $ref1.PSPath -Name $asmname -ErrorAction SilentlyContinue -ErrorVariable ec
    if ($ri -ne $null) {
        $TIA_ok = $true
        $asm = $ri.$asmname    
    } else {     
  	    Write-Error "Error: TIA Openness seems to be not installed" -Category ResourceUnavailable -ErrorAction Stop
        Exit 
    }
	return $asm
}

<#
 .Synopsis
 
 .Description
 
 .Parameter Path
 
 .Example
 
 .Example
 #>
function Test-TiaProjectVersion
{
    [OutputType([bool])]
    param([Parameter(Mandatory=$true, ValueFromPipeline=$true)][string]$Path=".")
	if (-not (Split-Path $Path -IsAbsolute)) {
		$Path = Join-Path -Path (Split-Path $Path -Resolve) -ChildPath  $Path -Resolve
	}
	$extmap = @{ "ap14" = "14.0", "ap15" = "15.0", "ap15_1" = "15.1", "ap16" = "16.0" }
	
	if (Test-Path -Path $Path -PathType Leaf) {
		$filename = $Path	
	} 
	else {
		#TODO: should use an installed TIA version, not any known
		$fap = Get-ChildItem -Path $Path -Include ".ap*"
		$fap | Select-Object Extention | Foreach-Object { if ($extmap[$_]) {res = 1} } 
	}
	$ext = $Path.Split(".")[-1]
	$ver = $extmap[$ext]
	if ($ver -eq $null) {
		$res = 1
	}
    return 0
}

Export-ModuleMember -Function Find-TiaOpenness
Export-ModuleMember -Function Test-TiaProjectVersion