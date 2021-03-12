using System;
using System.Management.Automation;
using Siemens.Engineering;

namespace TiaCmdlet
{
    [Cmdlet(VerbsCommon.Set, "TiaDevice")]
    public class SetTiaDevice : PSCmdlet
    {

        private Siemens.Engineering.HW.Device device = null;

        private string author = null;

        private string comment = null;

        private string name = null;

        #region Command parameters

        /// <summary>
        /// Gets or sets the device
        /// </summary>
        [Parameter(Mandatory = true,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true,
            Position = 0,
            HelpMessage = "TIA Device")]
        [Alias("i")]
        public Siemens.Engineering.HW.Device Device
        {
            get { return device; }
            set { device = value; }
        }
                
        /// <summary>
        /// Gets or sets a device name
        /// </summary>
        [Parameter(Mandatory = false,
            Position = 1,
            ParameterSetName = "RefName",
            HelpMessage = "Device name")]
        [Alias("n")]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        /// <summary>
        /// Gets or sets an author
        /// </summary>
        [Parameter(Mandatory = false,
            Position = 2,
            ParameterSetName = "RefAuthor",
            HelpMessage = "Device insertion author")]
        [Alias("a")]
        public string Author
        {
            get { return author; }
            set { author = value; }
        }

        /// <summary>
        /// Gets or sets a comment
        /// </summary>
        [Parameter(Mandatory = false,
            Position = 2,
            ParameterSetName = "RefComment",
            HelpMessage = "Device comment")]
        [Alias("c")]
        public string Comment
        {
            get { return comment; }
            set { comment = value; }
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
            if (device != null)
            {
                if ((name != null) && (name.Length > 0)) { device.Name = name; }
                if (author != null) {
                    if (author.Length >= 3) { device.SetAttribute("Author", author); }
                    else { WriteWarning("Is '{author}' not too small?"); }                         
                }
                if (comment != null)
                {
                    device.SetAttribute("Comment", comment);                   
                }                
            }
        }

        protected override void EndProcessing()
        {
            base.EndProcessing();
        }
        #endregion command code
    }
}
