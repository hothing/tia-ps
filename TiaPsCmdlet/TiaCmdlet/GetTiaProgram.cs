using System;
using System.Management.Automation;
using Siemens.Engineering;

namespace TiaCmdlet
{
    [Cmdlet(VerbsCommon.Get, "TiaProgram")]
    public class GetTiaProgram : PSCmdlet
    {

        private Siemens.Engineering.HW.DeviceItem device_item = null;

        private Siemens.Engineering.HW.Device device = null;

        private string module_name = null;

        #region Command parameters

        /// <summary>
        /// Gets or sets the device module
        /// </summary>
        [Parameter(
        Position = 0,
        Mandatory = true,
        ParameterSetName = "Module",
        ValueFromPipeline = true,
        ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty]
        [Alias("m")]
        public Siemens.Engineering.HW.DeviceItem Module
        {
            get { return this.device_item; }
            set { this.device_item = value; }
        }

        /// <summary>
        /// Gets or sets the device module
        /// </summary>
        [Parameter(
        Position = 0,
        Mandatory = true,
        ParameterSetName = "Device",
        ValueFromPipeline = true,
        ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty]
        [Alias("i")]
        public Siemens.Engineering.HW.Device Device
        {
            get { return this.device; }
            set { this.device = value; }
        }

        /// <summary>
        /// Gets or sets a device attribute
        /// </summary>
        [Parameter(Mandatory = true,
            Position = 1,
            ParameterSetName = "Device",
            HelpMessage = "Device module name")]
        [Alias("n")]
        public string Name
        {
            get { return module_name; }
            set { module_name = value; }
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
            if (ParameterSetName.Equals("Device"))
            {
                if (device != null)
                {
                    if (module_name != null)
                    {
                        foreach (Siemens.Engineering.HW.DeviceItem deviceItem in device.DeviceItems)
                        {
                            if (deviceItem.Name.Equals(module_name)) { device_item = deviceItem; break; }
                        }
                    }
                }
            }
            if (device_item != null)
            {
                var container = ((Siemens.Engineering.IEngineeringServiceProvider)device_item).GetService<Siemens.Engineering.HW.Features.SoftwareContainer>();
                if (container != null)
                {
                    WriteObject(container.Software);
                }
                else
                {
                    //WriteWarning("The modules doesn't have a software container");
                    WriteError(new ErrorRecord(new ItemNotFoundException("container"), "The module doesn't have a software container", ErrorCategory.ObjectNotFound, device_item));
                }
                
            }
            else {
                //WriteWarning("The module not found");
                WriteError(new ErrorRecord(new ItemNotFoundException("module"), $"The module {module_name} is not found", ErrorCategory.ObjectNotFound, device));
            }
        }

        protected override void EndProcessing()
        {
            base.EndProcessing();
        }
        #endregion command code
    }
}
