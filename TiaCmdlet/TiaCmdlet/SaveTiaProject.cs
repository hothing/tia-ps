using System;
using System.Text;
using System.IO;
using System.Reflection;

using System.Management.Automation;
using Siemens.Engineering;

namespace TiaCmdlet
{
    [Cmdlet(VerbsData.Save, "TiaProject")]
    public class SaveTiaProject : PSCmdlet
    {
        private TiaPortal tp = null;

        private string projectPath = null;

        private Project project = null;

        [Parameter(Mandatory = true,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true,
            Position = 0,
            ParameterSetName = "RefDirect",
            HelpMessage = "TIA Project")]
        public Project Project
        {
            get { return project; }
            set { project = value; }
        }

        [Parameter(Mandatory = true,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true,
            Position = 0,
            ParameterSetName = "RefByName",
            HelpMessage = "TIA Portal  object")]
        public TiaPortal Portal
        {
            get { return tp; }
            set { tp = value; }
        }

        [Parameter(Mandatory = true,
            Position = 1,
            ParameterSetName = "RefByName",
            HelpMessage = "TIA project path")]
        public string Path
        {
            get { return projectPath; }
            set { projectPath = value; }
        }
        protected override void ProcessRecord()
        {
            base.ProcessRecord();
            if (ParameterSetName == "RefByName")
            {
                foreach (Project p in tp.Projects)
                {
                    if (String.Compare( p.Path.Name , projectPath) == 0) { project = p; }
                }                
            }
            if (project != null) { project.Save();  }
        }
    }
}
