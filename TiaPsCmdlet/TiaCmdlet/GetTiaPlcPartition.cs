using System;
using System.Management.Automation;
//using Siemens.Engineering;

namespace TiaCmdlet
{
    

    [Cmdlet(VerbsCommon.Get, "TiaPlcPartition")]
    public class GetTiaPlcPartition : PSCmdlet
    {

        private Siemens.Engineering.SW.PlcSoftware software = null;

        private string partName = null;

        #region Command parameters

        /// <summary>
        /// Gets or sets the plc group
        /// </summary>
        [Parameter(
        Position = 0,
        Mandatory = true,
        ValueFromPipeline = true)]
        [ValidateNotNullOrEmpty]
        [Alias("i")]
        public Siemens.Engineering.SW.PlcSoftware InputObject
        {
            get { return this.software; }
            set { this.software = value; }
        }

        /// <summary>
        /// Gets or sets a partition name
        /// </summary>
        [Parameter(
            Position = 1,
            Mandatory = true,
            HelpMessage = "a partition name")]
        [ValidateNotNullOrEmpty]
        [Alias("n")]
        public string Name
        {
            get { return partName; }
            set { partName = value; }
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
            if (String.Compare(partName, "PROGRAM") == 0)
            {
                WriteObject(software.BlockGroup);
            } else if (String.Compare(partName, "SOURCE") == 0)
            {
                WriteObject(software.ExternalSourceGroup);
            }
            else if (String.Compare(partName, "TAGS") == 0)
            {
                WriteObject(software.TagTableGroup);
            }
            else if (String.Compare(partName, "TECHNOLOGY") == 0)
            {
                WriteObject(software.TechnologicalObjectGroup);
            }
            else if (String.Compare(partName, "TYPES") == 0)
            {
                WriteObject(software.TypeGroup);
            }
            else if (String.Compare(partName, "FORCE") == 0)
            {
                WriteObject(software.WatchAndForceTableGroup);
            }
        }

        protected override void EndProcessing()
        {
            base.EndProcessing();
        }
        #endregion command code
    }
}
