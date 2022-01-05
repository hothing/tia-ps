$modPath = "C:\Users\simatic\source\repos\tia-ps\TiaPsCmdlet\TiaCmdlet\bin\Debug\net462\TiaCmdlet.dll" 
Import-Module $modPath



function findNetHost($deviceItem, $hostlist)
{
    Write-Debug "[findNetHost] check the $($deviceItem.Name)"
    Write-Debug "[findNetHost]  $($hostlist.GetType())"
    try { 
        $ift = $deviceItem.GetAttribute("InterfaceType")
        if ($ift -ne $null)  
        {
            Write-Debug "[findNetHost] found"
            $hostlist = $hostlist + @(,$deviceItem)            
        }        
    }
    catch
    {
        Write-Debug "[findNetHost] exception $_"
    }

    foreach ($di in $deviceItem.Items)
    {
        $hostlist = findNetHost -deviceItem $di -hostlist $hostlist            
    }
    Write-Debug "[findNetHost] done with the $($deviceItem.Name)"
    $hostlist
}


function getNetInterface($deviceItem)
{
    $netif = $deviceItem.GetService
}

$DebugPreference = "Continue"

$tp = Get-TiaInstance

$prj = ($tp | Get-TiaProject)

$dev = ($prj | Get-TiaDeviceList -Path "G1" | where-object Name -eq -Value "D1_1")

$dhost = findNetHost $dev.Items @()

$gsm = $dhost.GetType().GetMethod("GetService")

$methodCall = $gsm.MakeGenericMethod([Siemens.Engineering.HW.Features.NetworkInterface])

$netif = $methodCall.Invoke($dhost, $null)

$subnet = $netif.Nodes[0].ConnectedSubnet

$subnet.Nodes | % { $_.GetAttribute("Address") }

$ien = New-Object 'System.Collections.Generic.List``1[System.String]'
$ien.Add("Address")
$ien.Add("SubnetMask")

$subnet.Nodes | % { $x = $_.GetAttributes($ien); [PSCustomObject]@{ Address = $x[0]; SubnetMask = $x[1]} } | ft
