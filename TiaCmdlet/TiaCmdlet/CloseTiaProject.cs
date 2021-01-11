using System;
using System.Text;
using System.IO;
using System.Reflection;

using System.Management.Automation;
using Siemens.Engineering;

namespace TiaCmdlet
{
    [Cmdlet(VerbsCommon.Close, "TiaProject")]
    public class CloseTiaProject : PSCmdlet
    {
        private TiaPortal tp = null;

        private string projectName = null;

        private Project project = null;

        [Parameter(Mandatory = true,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true,
            Position = 0,
            ParameterSetName = "CloseDirect",
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
            ParameterSetName = "CloseByName",
            HelpMessage = "TIA Portal  object")]
        public TiaPortal Portal
        {
            get { return tp; }
            set { tp = value; }
        }

        [Parameter(Mandatory = false,
            Position = 1,
            ParameterSetName = "CloseByName",
            HelpMessage = "TIA project path")]
        public string Name
        {
            get { return projectName; }
            set { projectName = value; }
        }

        private void SelectDefaultProject()
        {
            if (project == null)
            {
                var tpe = tp.Projects.GetEnumerator();
                if (tpe.MoveNext()) { project = tpe.Current; }
            }
        }

        private void SelectProject()
        {
            foreach (Project p in tp.Projects)
            {
                if (String.Compare(p.Name, projectName) == 0) { project = p; }
            }
        }
        protected override void ProcessRecord()
        {
            base.ProcessRecord();
            if (ParameterSetName == "CloseByName")
            {
               if (projectName != null)
               {
                    SelectProject();
               } 
               else
               {
                    SelectDefaultProject();
               }               
            }
            if (project != null) { project.Close();  }
        }
    }
}
