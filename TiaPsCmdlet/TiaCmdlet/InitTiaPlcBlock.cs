using System;
using System.Management.Automation;
using Siemens.Engineering;

namespace TiaCmdlet
{
    [Cmdlet(VerbsData.Initialize, "TiaPlcBlock")]
    public class InitializeTiaPlcBlock : PSCmdlet
    {

        private Siemens.Engineering.SW.PlcSoftware program = null;

        private Siemens.Engineering.SW.Blocks.PlcBlock block = null;
        
        #region Command parameters

        /// <summary>
        /// Gets or sets the device module
        /// </summary>
        [Parameter(
        Position = 0,
        Mandatory = true,
        ParameterSetName = "Software",
        ValueFromPipeline = true,
        ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty]
        [Alias("m")]
        public Siemens.Engineering.SW.PlcSoftware Software
        {
            get { return this.program; }
            set { this.program = value; }
        }

        /// <summary>
        /// Gets or sets the device module
        /// </summary>
        [Parameter(
        Position = 0,
        Mandatory = true,
        ParameterSetName = "Block",
        ValueFromPipeline = true,
        ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty]
        [Alias("i")]
        public Siemens.Engineering.SW.Blocks.PlcBlock Block
        {
            get { return this.block; }
            set { this.block = value; }
        }

        #endregion Command parameters

        #region command code

        #region internal commands

        #endregion internal commands

        protected override void BeginProcessing()
        {
            base.BeginProcessing();            
        }
        protected override void ProcessRecord()
        {
            base.ProcessRecord();
            Siemens.Engineering.Compiler.ICompilable compiler = null;
            if (ParameterSetName.Equals("Software"))
            {
                if (program != null)
                {
                    compiler = program.GetService<Siemens.Engineering.Compiler.ICompilable>();
                }
            }
            else if (ParameterSetName.Equals("Block"))
            {
                if (block != null)
                {
                     compiler = block.GetService<Siemens.Engineering.Compiler.ICompilable>();                   
                }
            }
            else {
                WriteError(new ErrorRecord(new InvalidPowerShellStateException(), $"Parameterset is wrong", ErrorCategory.InvalidArgument, null));
            }
            if (compiler != null)
            {
                var result = compiler.Compile();
                WriteObject(result);
            }
            else
            {
                //FIXME: change the exception type
                WriteError(new ErrorRecord(new Exception("TIA Compiler"), $"The object is not compilable", ErrorCategory.InvalidArgument, null));
            }
        }

        protected override void EndProcessing()
        {
            base.EndProcessing();
        }
        #endregion command code
    }
}
