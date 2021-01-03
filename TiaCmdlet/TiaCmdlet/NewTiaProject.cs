using System;
using System.Text;
using System.IO;
using System.Reflection;

using System.Management.Automation;
using Siemens.Engineering;

namespace TiaCmdlet
{
    [Cmdlet(VerbsCommon.New, "TiaProject")]
    [OutputType(typeof(Project))]
    public class NewTiaProject : PSCmdlet
    {
        private TiaPortal tp = null;

        private string projectPath = null;

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

        [Parameter(Mandatory = true,
            Position = 1,
            ParameterSetName = "GivenProject",
            HelpMessage = "TIA project path")]
        public string Path
        {
            get { return projectPath; }
            set { projectPath = value; }
        }

        [Parameter(Mandatory = false,
            Position = 2,
            ParameterSetName = "GivenProject",
            HelpMessage = "TIA project name")]
        public string Name
        {
            get { return projectName; }
            set { projectName = value; }
        }
        protected override void BeginProcessing()
        {
            // TODO: extract default name from the project path
            if (projectName == null) { projectName = "default.ap16"; }            
        }

        protected override void ProcessRecord()
        {
            base.ProcessRecord();
            if (projectPath != null) 
            {
                Project prj = tp.Projects.Create(new System.IO.DirectoryInfo(projectPath), projectName );
                WriteObject(prj);
            }
        }
    }
}
