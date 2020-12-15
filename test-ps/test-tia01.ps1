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