using System;
using System.Text;
using System.IO;
using System.Reflection;

using System.Management.Automation;
using Siemens.Engineering;

namespace TiaCmdlet
{
    [Cmdlet(VerbsCommon.Get, "TiaProcess")]
    [OutputType(typeof(TiaPortalProcess[]))]
    public class GetTiaProcess : Cmdlet
    {
        private int index = 0;
        [Parameter(Mandatory = false,
            Position = 0,
            HelpMessage = "TIA Portal instance id: 0 .. n")]
        public int Id
        {
            get { return index; }
            set { index = value; }
        }
        protected override void ProcessRecord()
        {
            base.ProcessRecord();
            if (index >= 0)
            {
                var tpl = TiaPortal.GetProcesses();
                if (index < tpl.Count)
                {
                    WriteObject(TiaPortal.GetProcesses()[index]);
                }
            }
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
