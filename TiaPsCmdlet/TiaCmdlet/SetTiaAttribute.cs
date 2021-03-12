using System;
using System.Management.Automation;
using Siemens.Engineering;

namespace TiaCmdlet
{
    [Cmdlet(VerbsCommon.Set, "TiaAttribute")]
    public class SetTiaAttribute : PSCmdlet
    {

        private Siemens.Engineering.IEngineeringObject iobj = null;
        
        private string attr_name = null;

        private Object attr_value = null;

        private bool pass_thru = false;

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
            Position = 1,
            ParameterSetName = "Attribute",
            HelpMessage = "Attribute of device")]
        [Alias("t")]
        public string Attribute
        {
            get { return attr_name; }
            set { attr_name = value; }
        }

        /// <summary>
        /// Gets or sets a device attribute value
        /// </summary>
        [Parameter(Mandatory = true,
            Position = 2,
            ParameterSetName = "Attribute",
            HelpMessage = "Value of attribute")]
        [Alias("v")]
        public Object Value
        {
            get { return attr_value; }
            set { attr_value = value; }
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
                    try {
                        iobj.SetAttribute(attr_name, attr_value);
                        if (pass_thru) { WriteObject(iobj); }                         
                    }
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
