﻿using System;
using System.Text;
using System.IO;
using System.Reflection;

using System.Management.Automation;
using Siemens.Engineering;

namespace TiaCmdlet
{
    [Cmdlet(VerbsCommon.Open, "TiaProject")]
    [OutputType(typeof(Project))]
    public class OpenTiaProject : PSCmdlet
    {
        private TiaPortal tp = null;

        private string projectPath = null;

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
            ParameterSetName = "GivenProject",
            HelpMessage = "TIA project path")]
        public string Path
        {
            get { return projectPath; }
            set { projectPath = value; }
        }
        protected override void ProcessRecord()
        {
            base.ProcessRecord();
            if (projectPath != null) 
            {
                Project prj = tp.Projects.Open(new System.IO.FileInfo(projectPath));
                WriteObject(prj);
            } 
            else
            {
                foreach (Project prj in tp.Projects) 
                {
                    WriteObject(prj);
                }
            }
        }
    }
}
