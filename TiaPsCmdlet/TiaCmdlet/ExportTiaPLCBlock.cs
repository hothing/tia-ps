using System;
using System.Linq;
using System.IO;
using System.Management.Automation;
using Siemens.Engineering;
using Siemens.Engineering.HW;
using Siemens.Engineering.SW;

namespace TiaCmdlet
{
    [Cmdlet(VerbsData.Export, "TiaPlcBlock")]
    public class ExportTiaPlcBlock : PSCmdlet
    {
        private Siemens.Engineering.SW.Blocks.PlcBlock block = null;
               
        private string path = null;

        private string file_name = null;

        private ExportOptions options = ExportOptions.None;

        private bool pass_thru;

        private string cdir = null;

        #region Command parameters

        /// <summary>
        /// Gets or sets the project to refer the root DeviceGroup(s)
        /// </summary>
        [Parameter(Mandatory = true,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true,
            Position = 0,
            HelpMessage = "TIA Project")]
        [Alias("i")]
        public Siemens.Engineering.SW.Blocks.PlcBlock InputObject
        {
            get { return block; }
            set { block = value; }
        }
        
        /// <summary>
        /// Gets or sets the path
        /// </summary>
        [Parameter(Mandatory = false,
            ParameterSetName = "withPath",
            HelpMessage = "XML-file directory path")]
        [Alias("p")]
        public string Path
        {
            get { return path; }
            set { path = value; }
        }

        /// <summary>
        /// Gets or sets the path
        /// </summary>
        [Parameter(Mandatory = false,
            HelpMessage = "XML-file name")]
        [Alias("f")]
        public string Name
        {
            get { return file_name; }
            set { file_name = value; }
        }

        /// <summary>
        /// Gets or sets the export options 
        /// </summary>
        [Parameter(Mandatory = false,
            HelpMessage = "Export options")]
        [Alias("o")]
        public ExportOptions Options
        {
            get { return options; }
            set { options = value; }
        }

        /// <summary>
        /// Gets or sets a 'PassThru'mode
        /// </summary>
        [Parameter(Mandatory = false,
            HelpMessage = "Returns an object representing the item with which you are working. By default, this cmdlet does not generate any output.")]
        public SwitchParameter PassThru
        {
            get { return pass_thru; }
            set { pass_thru = value; }
        }
        #endregion Command parameters

        #region command code

        #region internal commands

        protected string getFullFileName()
        {
            string fqp = file_name;
            if (file_name != null) {
                if ((path == null) || (path.Equals("")))
                {
                    fqp = System.IO.Path.GetFullPath(file_name);
                }
                else
                {
                    if (System.IO.Path.IsPathRooted(path))
                    {
                        fqp = System.IO.Path.Combine(path, file_name);
                    }
                    else
                    {
                        var fp = System.IO.Path.GetFullPath(path);
                        fqp = System.IO.Path.Combine(fp, file_name);
                    }
                }
            }
            return fqp;
        }
        #endregion internal commands

        protected override void BeginProcessing()
        {
            base.BeginProcessing();
            this.cdir = Directory.GetCurrentDirectory();
            Directory.SetCurrentDirectory(this.SessionState.Path.CurrentFileSystemLocation.Path);
        }
        protected override void ProcessRecord()
        {
            base.ProcessRecord();
            if (block != null)
            {
                if (file_name == null) { file_name = block.Name + ".xml"; }
                block.Export(new FileInfo(getFullFileName()), options);
                if (pass_thru) { WriteObject(block); }
            }
            else
            {
                WriteError(new ErrorRecord(new ArgumentNullException(), $"the block is not given", ErrorCategory.InvalidArgument, block));
            }
        }

        protected override void EndProcessing()
        {
            base.EndProcessing();
            Directory.SetCurrentDirectory(this.cdir);
        }
        #endregion command code
    }
}
