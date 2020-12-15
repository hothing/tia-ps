$source = @"
using System.Management.Automation;  // Windows PowerShell assembly.
namespace PumCmdlets
{
  // Declare the class as a cmdlet and specify the
  // appropriate verb and noun for the cmdlet name.
  [Cmdlet(VerbsCommon.Get, "TestName")]
  public class GetNameCmdlet : Cmdlet
  {
    // Declare the parameters for the cmdlet.
    [Parameter(Mandatory=true)]
    public string Name
    {
      get { return name; }
      set { name = value; }
    }
    private string name;

    // Override the ProcessRecord method to process
    // the supplied user name and write out a
    // greeting to the user by calling the WriteObject
    // method.
    protected override void ProcessRecord()
    {
      WriteObject("Hello " + name + "!");
    }
  }
}
"@

New-Module -Name TestModule -Cmdlet 'Get-TestName' -ScriptBlock {
    $res = Add-Type -TypeDefinition $source -PassThru
    } | Import-Module

# Sad. It doesn't work...