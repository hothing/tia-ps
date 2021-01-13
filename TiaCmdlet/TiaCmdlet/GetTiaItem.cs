using System;
using System.Management.Automation;
using Siemens.Engineering;

namespace TiaCmdlet
{
    [Cmdlet(VerbsCommon.Get, "TiaItem")]
    public class GetTiaItem : PSCmdlet
    {
        private Project project = null;

        private string itemName = null;

        private bool _recurse = false;

        private WildcardPattern nameMatch = null;

        private char[] itemsSeparator = {'/'};

        private String[] pathComponents;

        #region Command parameters

        [Parameter(Mandatory = true,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true,
            Position = 0,
            HelpMessage = "TIA Project")]
        public Project Project
        {
            get { return project; }
            set { project = value; }
        }

        [Parameter(Mandatory = false,
            Position = 1,
            HelpMessage = "TIA project name")]
        public string Name
        {
            get { return itemName; }
            set { itemName = value; }
        }

        /// <summary>
        /// Gets or sets the recurse switch.
        /// </summary>
        [Parameter]
        [Alias("s")]
        public SwitchParameter Recurse
        {
            get
            {
                return _recurse;
            }

            set
            {
                _recurse = value;
            }
        }
        #endregion Command parameters

        #region command code

        #region internal commands
        private void WriteEachDevice()
        {
            foreach (Siemens.Engineering.HW.Device dev in project.Devices)
            {
                WriteObject(dev);
            }
        }

        private void WriteMatchedDevice()
        {
            if (nameMatch != null)
            {
                foreach (Siemens.Engineering.HW.Device dev in project.Devices)
                {
                    if (nameMatch.IsMatch(dev.Name))
                    {
                        WriteObject(dev);
                    }
                }
            }            
        }

        private void WriteDeviceGroup()
        {
            if ((itemName == null) || (String.Compare(itemName, ".") == 0))
            {
                WriteObject(project.Devices);                
            }
            else
            {
                var dg = project.DeviceGroups.Find(itemName);
                WriteObject(dg);
            }
        }
        #endregion internal commands

        protected override void BeginProcessing()
        {
            base.BeginProcessing();
            if (WildcardPattern.ContainsWildcardCharacters(itemName))
            {
                nameMatch = new WildcardPattern(itemName);
            }
            //pathComponents = itemName.Split(itemsSeparator, -1,  StringSplitOptions.RemoveEmptyEntries);            
        }
        protected override void ProcessRecord()
        {
            base.ProcessRecord();
            if (nameMatch != null)
            {
                WriteMatchedDevice();
            }
            else
            {
                if ((pathComponents != null) && (pathComponents.Length > 1))
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
            nameMatch = null;
            pathComponents = null;
        }
        #endregion command code
    }
}
