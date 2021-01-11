using System;
using System.Management.Automation;
using Siemens.Engineering;

namespace TiaCmdlet
{
    [Cmdlet(VerbsCommon.Get, "TiaProject")]
    public class GetTiaProject : PSCmdlet
    {
        private TiaPortal tp = null;

        private string projectName = null;

        [Parameter(Mandatory = true,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true,
            Position = 0,
            HelpMessage = "TIA Portal  object")]
        public TiaPortal Portal
        {
            get { return tp; }
            set { tp = value; }
        }

        [Parameter(Mandatory = false,
            Position = 1,
            HelpMessage = "TIA project name")]
        public string Name
        {
            get { return projectName; }
            set { projectName = value; }
        }

        protected override void ProcessRecord()
        {
            base.ProcessRecord();
            if (projectName != null) 
            {
                foreach (Project p in tp.Projects)
                {
                    // TODO: regular expression or wildcards
                    if (String.Compare(p.Name, projectName) == 0)
                    {
                        WriteObject(p);
                    }
                }
            }
            else
            {
                foreach (Project p in tp.Projects)
                {
                    WriteObject(p);
                }
            }            
        }
    }
}
