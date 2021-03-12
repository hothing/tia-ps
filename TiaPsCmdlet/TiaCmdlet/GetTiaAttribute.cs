using System;
using System.Management.Automation;
using Siemens.Engineering;

namespace TiaCmdlet
{
    [Cmdlet(VerbsCommon.Get, "TiaAttribute")]
    public class GetTiaAttribute : PSCmdlet
    {

        private Siemens.Engineering.IEngineeringObject iobj = null;
        
        private string attr_name = null;
                
        #region Command parameters

        /// <summary>
        /// Gets or sets the device
        /// </summary>
        [Parameter(Mandatory = true,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true,
            Position = 0,
            HelpMessage = "TIA Engineering Object")]
        [Alias("i")]
        public Siemens.Engineering.IEngineeringObject Device
        {
            get { return iobj; }
            set { iobj = value; }
        }
        
        /// <summary>
        /// Gets or sets a device attribute
        /// </summary>
        [Parameter(Mandatory = true,
            Position = 0,
            ParameterSetName = "Attribute",
            HelpMessage = "Attribute of device")]
        [Alias("t")]
        public string Attribute
        {
            get { return attr_name; }
            set { attr_name = value; }
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
            if (iobj != null)
            {
                if (attr_name != null)
                {
                    try { WriteObject(iobj.GetAttribute(attr_name)); }
                    catch (Exception e)
                    {
                        WriteError(new ErrorRecord(e, "Attribute is not accesible", ErrorCategory.ObjectNotFound, attr_name));
                    }
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
