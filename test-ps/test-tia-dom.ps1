$ProjectPath = "C:\Users\simatic\Documents\Automation\01_272_FGKs_V15.1\01_272_FGKs_V15.1.ap15_1"
# Export path
$exppath = "C:\Users\simatic\Documents\TIAps\data\"

# Internal variables
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
    $tp = New-Object Siemens.Engineering.TiaPortal(1) # 0 = without GUI

    
    # Trying to open and then close the project
    # NB1: The argument 'sourcePath' cannot be a relative path."
    $prj = $tp.Projects.Open($ProjectPath)
    Write-Information "The project has been opened"
    if ($prj -ne $null) {
        Import-Module .\PsTia15PlcCmdlet.dll
        # Iterate over the project items: devices and groups
        foreach ($dev in $prj.Devices) {
            Write-Host ("Device name : {0}" -f ($dev.Name))
        }
        <#
        $prj.Devices[0] - usually it is CPU assmeble (not itself)
        $prj.Devices[0].DeviceItems - the CPU assemble componets which include CPU itself
        I do not see a robust way to identify the CPU
        The way to find CPU: iterate over the device item list and check a property "Classification" which should be "CPU"
        #>
        # Find first CPU
        $devplc = $prj.Devices[0].DeviceItems | where-object -Property Classification -eq "CPU"
        # get the program blocks
        $sc = ($devplc | Get-SoftwareContainer)
        # Ungrouped blocks
        foreach ($blk in $sc.Software.BlockGroup.Blocks) {Write-Host ("Block name : {0}" -f $blk.Name) }
        # Export all blocks in groups of max.level 2        
        foreach ($grp in $sc.Software.BlockGroup.Groups) {
            Write-Host ("Group : {0}" -f $grp.Name)

            foreach ($blk in $grp.Blocks) {
                Write-Host ("Block name : {0}" -f $blk.Name) 
                    $expfilename = ("{0}\{1}_{2}_{3}.xml" -f $exppath, $grp.Name, $blk.Name, $blk.ProgrammingLanguage).Replace("\\","\")
                    Write-Host "Export to : $expfilename"
                    $blk.Export($expfilename, 0)
            }
        }
        # Import block into the group
        #$sc.Software.BlockGroup.Blocks.Import(".", 0)
        $prj.Close()
        Write-Information "The project has been closed"
    }    
    # End work
    $tp.Dispose()
}