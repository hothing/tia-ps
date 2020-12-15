param(
    [string]$Path = ".",
    [string]$Project = "..",
    [string]$StructureFile = "structure-prj.json"
)

#Check the project
$prj = Split-Path -Path "$Project\*.ap15_1" -Leaf -Resolve
if ($prj.Length -ge 1) 
{
    
} 
else 
{
    Write-Error "TIA Project was not found"
} 
