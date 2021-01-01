using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Management.Automation;
using Siemens.Engineering;


namespace TiaCmdlet
{
    [Cmdlet(VerbsCommon.New, "TiaInstance")]
    [OutputType(typeof(Object))]
    class NewTiaInstance : Cmdlet
    {
        private string projectPath;
        [Parameter(Mandatory = true,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true,
            Position = 0,
            HelpMessage = "TIA project path")]
        public string Path
        {
            get { return projectPath; }
            set { projectPath = value; }
        }

        private bool mode = false;
        [Parameter(Mandatory = false,
            Position = 1,
            HelpMessage = "TIA Portal mode: start with or without GUI")]
        public bool WithGui
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
