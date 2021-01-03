using System;
using System.Text;
using System.IO;
using System.Reflection;

using System.Management.Automation;
using Siemens.Engineering;

namespace TiaCmdlet
{
    [Cmdlet(VerbsCommon.New, "TiaInstance")]
    [OutputType(typeof(TiaPortal))]
    public class NewTiaInstance : PSCmdlet
    {
        private bool mode = false;
        [Parameter(Mandatory = false,
            Position = 1,
            HelpMessage = "TIA Portal mode: start with or without GUI")]
        public SwitchParameter WithGui
        {
            get { return mode; }
            set { mode = value; }
        }

        protected override void ProcessRecord()
        {
            base.ProcessRecord();
            TiaPortalMode pmode = (mode ? TiaPortalMode.WithUserInterface : TiaPortalMode.WithoutUserInterface);
            TiaPortal tp = new TiaPortal(pmode);
            WriteObject(tp);
        }
    }
}
