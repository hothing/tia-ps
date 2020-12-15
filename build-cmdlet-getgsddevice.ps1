$source = @"
using System.Management.Automation;  // Windows PowerShell assembly.
using Siemens.Engineering;
using Siemens.Engineering.HW;
using Siemens.Engineering.HW.Features;

namespace PsTia15
{
  // Declare the class as a cmdlet and specify the
  // appropriate verb and noun for the cmdlet name.
  [Cmdlet(VerbsCommon.Get, "TiaGSDDevice")]
  public class GetTiaGsdDeviceCmdlet : Cmdlet
  {  
    #region Parameters
    private Siemens.Engineering.HW.DeviceItem _device_item;

    [Parameter(
        Position = 0,
        Mandatory = true,
        ValueFromPipeline = true,
        ValueFromPipelineByPropertyName = true)]
    [ValidateNotNullOrEmpty]
    public Siemens.Engineering.HW.DeviceItem DeviceItem
    {
       get { return this._device_item; }
       set { this._device_item = value; }
    }
    #endregion Parameters

    protected GsdDevice GetGsdDevice () {
        return ((IEngineeringServiceProvider)_device_item).GetService<GsdDevice>();
    }

    #region Cmdlet Overrides
    protected override void BeginProcessing()
    {
        base.BeginProcessing();        
    }


    // Override the ProcessRecord method to process
    // the supplied user name and write out a
    // greeting to the user by calling the WriteObject
    // method.
    protected override void ProcessRecord()
    {
      WriteObject(GetGsdDevice());
    }
    #endregion Cmdlet Overrides
  }
}
"@

$tiaref = "HKLM:\SOFTWARE\Siemens\Automation\Openness\15.1\PublicAPI\15.1.0.0"
$asmname = "Siemens.Engineering"
$asmfilename = "PsTia15PlcCmdlet.dll"

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
       $act = Add-Type -TypeDefinition $source -ReferencedAssemblies $asm -OutputType Library -OutputAssembly $asmfilename
    } 
    catch {
        Write-Error "TIA Openness is not operable"
        Exit
    }
    Write-Host "TIA Openness is operable"
}

# Use the module
# Import-Module $asmname