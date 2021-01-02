using System;
using System.Text;
using System.IO;
using System.Reflection;

using System.Management.Automation;
using Siemens.Engineering;

namespace TiaCmdlet
{
    [Cmdlet(VerbsCommon.Get, "TiaInstances")]
    [OutputType(typeof(TiaPortalProcess[]))]
    public class GetTiaInstances : Cmdlet
    {
        private bool getOnlyFirst = false;
        [Parameter(Mandatory = false,
            Position = 1,
            HelpMessage = "TIA Portal mode: start with or without GUI")]
        public SwitchParameter First
        {
            get { return getOnlyFirst; }
            set { getOnlyFirst = value; }
        }
        protected override void ProcessRecord()
        {
            base.ProcessRecord();

            if (getOnlyFirst) { WriteObject(TiaPortal.GetProcesses()[0]); }            
            else
            {
                foreach (TiaPortalProcess tpx in TiaPortal.GetProcesses())
                {
                    WriteObject(tpx);
                }
            }  
        }
    }
}
