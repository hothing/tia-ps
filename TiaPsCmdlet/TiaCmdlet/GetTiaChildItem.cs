using System;
using System.Management.Automation;
using Siemens.Engineering;

namespace TiaCmdlet
{
    [Cmdlet(VerbsCommon.Get, "TiaChildItem")]
    public class GetTiaChildItem : PSCmdlet
    {
        private Project _project = null;

        private string _itemName = null;

        private WildcardPattern _nameMatch = null;
        
        private String[] _path;

        private readonly char[] _itemsSeparator = { '/' };

        #region Command parameters

        /// <summary>
        /// Gets or sets the TIA project object.
        /// </summary>
        [Parameter(Mandatory = true,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true,
            Position = 0,
            HelpMessage = "TIA Project")]
        public Project Project
        {
            get { return _project; }
            set { _project = value; }
        }

        /// <summary>
        /// Gets or sets the device group name.
        /// </summary>
        [Parameter(Mandatory = false,
            Position = 1,
            HelpMessage = "TIA project name")]
        public string Name
        {
            get { return _itemName; }
            set { _itemName = value; }
        }
        #endregion Command parameters

        #region command code

        #region internal commands
        private void WriteEachDevice()
        {
            foreach (Siemens.Engineering.HW.Device dev in _project.Devices)
            {
                WriteObject(dev);
            }
        }

        private void WriteMatchedDevice()
        {
            if (_nameMatch != null)
            {
                foreach (Siemens.Engineering.HW.Device dev in _project.Devices)
                {
                    if (_nameMatch.IsMatch(dev.Name))
                    {
                        WriteObject(dev);
                    }
                }
            }            
        }

        private void WriteDeviceGroup()
        {
            if ((_itemName == null) || (String.Compare(_itemName, ".") == 0))
            {
                WriteObject(_project.Devices);                
            }
            else
            {
                var dg = _project.DeviceGroups.Find(_itemName);
                WriteObject(dg);
            }
        }
        #endregion internal commands

        protected override void BeginProcessing()
        {
            base.BeginProcessing();
            if (WildcardPattern.ContainsWildcardCharacters(_itemName))
            {
                _nameMatch = new WildcardPattern(_itemName);
            }
            //pathComponents = itemName.Split(itemsSeparator, -1,  StringSplitOptions.RemoveEmptyEntries);            
        }
        protected override void ProcessRecord()
        {
            base.ProcessRecord();
            if (_nameMatch != null)
            {
                WriteMatchedDevice();
            }
            else
            {
                if ((_path != null) && (_path.Length > 1))
                {
                    // TODO: Recursively go down throw 'DeviceUserGroupComposition'
                } 
                else
                {
                    WriteDeviceGroup();
                }                
            }
        }

        protected override void EndProcessing()
        {
            base.EndProcessing();
            _nameMatch = null;
            _path = null;
        }
        #endregion command code
    }
}
