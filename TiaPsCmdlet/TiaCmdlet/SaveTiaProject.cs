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

        private Project project = null;

        private string projectName = null;
                
        private string projectNewPath = null;
                
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

        [Parameter(Mandatory = false,
            Position = 1,
            ParameterSetName = "RefByName",
            HelpMessage = "TIA project name")]
        public string Name
        {
            get { return projectName; }
            set { projectName = value; }
        }
                
        [Parameter(Mandatory = false,
           Position = 1,
           HelpMessage = "TIA project new path")]
        public string NewPath
        {
            get { return projectNewPath; }
            set { projectNewPath = value; }
        }

        private void SelectDefaultProject()
        {
            if (project == null) {
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
        private void Save()        
        {
            if (project != null) { project.Save(); }
        }
                
        private void SaveAs()
        {           
            if (project != null) {
                if (projectNewPath != null)
                {
                    project.SaveAs(new System.IO.DirectoryInfo(projectNewPath));
                } else
                {
                    // TODO: raise Exception
                }
                 
            }
        }

        protected override void ProcessRecord()
        {
            base.ProcessRecord();
            if (ParameterSetName == "RefByName")
            {
                if (projectName == null)
                {
                    SelectDefaultProject();
                } else
                {
                    SelectProject();
                }                
            }

            if (projectNewPath == null)
            {
                Save();
            }
            else
            {
                SaveAs();
            }
        }
    }
}
