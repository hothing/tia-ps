using System;
using System.Text;
using System.IO;
using System.Reflection;

using System.Management.Automation;
using Siemens.Engineering;

namespace TiaCmdlet
{
    [Cmdlet(VerbsCommon.Get, "TiaInstance")]
    [OutputType(typeof(Object[]))]
    public class GetTiaInstance : Cmdlet
    {
        protected override void ProcessRecord()
        {
            base.ProcessRecord();

            foreach (TiaPortalProcess tpx in TiaPortal.GetProcesses())
            {
                WriteObject(tpx); // This is what actually "returns" output.
            }            
        }
    }
}
