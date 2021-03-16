using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

using System.Management.Automation;
using Siemens.Engineering;
using Siemens.Engineering.HW;
using Siemens.Engineering.SW;

namespace TiaCmdlet
{
    [Cmdlet(VerbsData.Import, "TiaPlcBlock")]
    public class ImportTiaPlcBlock : PSCmdlet
    {
        private Siemens.Engineering.SW.Blocks.PlcBlockComposition block_group = null;
       
        private string path = null;

        private string file_name = null;

        private ImportOptions options = ImportOptions.None;

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
            HelpMessage = "TIA Plc Block group")]
        [Alias("i")]
        public Siemens.Engineering.SW.Blocks.PlcBlockComposition InputObject
        {
            get { return block_group; }
            set { block_group = value; }
        }
        
        /// <summary>
        /// Gets or sets the path
        /// </summary>
        [Parameter(Mandatory = false,
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
        [Parameter(Mandatory = true,
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
            HelpMessage = "Import options")]
        [Alias("o")]
        public ImportOptions Options
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
            if (block_group != null)
            {
                string qfn = getFullFileName();
                if (file_name != null)
                {
                    try 
                    {
                        IList<Siemens.Engineering.SW.Blocks.PlcBlock> blocks = block_group.Import(new FileInfo(qfn), options);
                        if (pass_thru)
                        {
                            foreach (var block in blocks)
                            {
                                WriteObject(block);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        WriteError(new ErrorRecord(e, $"the file '{qfn}' cannot be imported", ErrorCategory.InvalidOperation, block_group));
                    }                   
                }
                else
                {
                    WriteError(new ErrorRecord(new ArgumentNullException(), $"the file name is not given", ErrorCategory.InvalidArgument, null));
                }
            }
            else
            {
                WriteError(new ErrorRecord(new ArgumentNullException(), $"the block group is not given", ErrorCategory.InvalidArgument, null));
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
