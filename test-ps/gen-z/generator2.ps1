param(
    [string]$Path = ".",
    [string]$TaskFile = "Replication-Task.csv"
)

$hdr = @("Folder", "Entry", "Instance" )
$hargs = @()

for ($i = 1; $i -le 36; $i++)
{
    $hargs += "Arg$i"
}
$hdr += $hargs


$task = Get-Content "$Path\$TaskFile" | ConvertFrom-Csv -Header $hdr

$repcfg = $task[0]
$ProjectPath = $repcfg.Folder


# Create a descriptor of the replicating block
# collect names of input/output of the replicating block
$fargs = @{}
foreach ($arg in $hargs) {
    if ($repcfg.$arg -ne $null) {
        $fargs[$arg] = $repcfg.$arg
    }
}
$ReplicationBlock = @{ Fx = $repcfg.Instance; Args = $fargs } 

# Make a structure
$Structure = @{}
# Check whether the pairs (Folder + Entry) are unique
$Entries = @{}

Function ConvertFx {
    param($subtask, $fargs, $itype)
    $newsbt = @{}
    foreach ($arg in $fargs.Keys) {
        $ref = $fargs.$arg
        $newsbt[$ref] = $subtask.$arg 
    }
    return @{ Instance = @{ Name = $subtask.Instance; IType = $itype } ; Params = $newsbt }
}

foreach ($et in $task[1..$task.Count]) {
    if ($entries[$et.Entry] -ne $null) {
       if (-not $et.Folder -in $entries[$et.Entry]) { 
        Write-Warning ("The entry function must be unique! Please check for: {0} in {1}" -f $et.Entry, $et.Folder)
       } else {
        # collect the entries
        $ref = $et.Entry
        if ($ref -ne $null) {
            if ($Structure[$ref] -ne $null) {
                $Structure[$ref].units += ConvertFx -subtask $et -fargs $fargs       
            } else {
                $Structure[$ref] = @{Folder = $et.Folder ; units = @(ConvertFx -subtask $et -fargs $fargs -itype $repcfg.Instance )}
            }
        }
       }   
    } 
    else  {
       $entries[$et.Entry] = @($et.Folder)
    }
}

# Save the collected data

$SourceDir = "$Path\Source"
$PreDir = "Pre"
$PostDir = "Post"

if (-not (Test-Path "$SourceDir")) { New-Item -ItemType Directory $SourceDir }
if (-not (Test-Path "$SourceDir\$PreDir" )) { New-Item -ItemType Directory -Path $SourceDir -Name $PreDir }
if (-not (Test-Path "$SourceDir\$PostDir" )) { New-Item -ItemType Directory -Path $SourceDir -Name $PostDir } 

$str = $Structure | ConvertTo-Json -Depth 9 

if (-not (Test-Path "$SourceDir\structure.json" )) 
	{ $str | Set-Content -Path "$SourceDir\structure.json" -Encoding UTF8 }
else 
	{ 
		Write-Warning ("The structure file is already exist!")
		# May be I can make a set of structure file as structure-0.json .. structure-N.json 
	}