using System;
using System.Text;
using System.IO;
using System.Reflection;

using System.Management.Automation;
using Siemens.Engineering;

namespace TiaCmdlet
{
    [Cmdlet(VerbsCommon.Get, "TiaInstance")]
    [OutputType(typeof(TiaPortal))]
    public class GetTiaInstance : Cmdlet
    {
        private int index = -1;
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
            if (index < 0) { index = 0; }
            var tpl = TiaPortal.GetProcesses();
            if (index < tpl.Count)
            {
                var tproc = TiaPortal.GetProcesses()[index];
                var prj = tproc.Attach();
                WriteObject(prj);
            }            
        }
    }
}
