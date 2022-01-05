using System;
using System.Management.Automation;
using Siemens.Engineering.SW.Blocks;

namespace TiaCmdlet
{
    public enum ProgramItemType { FcBlock, FbBlock, DbBlock, IDbBlock, Group }

    [Cmdlet(VerbsCommon.New, "TiaProgramItem")]
    public class NewTiaProgramItem : PSCmdlet
    {

        #region Command parameters

        private PlcBlockComposition blocks;

        private PlcBlockUserGroupComposition groups;

        private PlcSystemBlockGroupComposition sysGroups;

        private string[] name;

        private string[] path;

        private ProgramItemType itype;

        private string plang;

        /// <summary>
        /// Gets or sets the blocks container
        /// </summary>
        [Parameter(Position = 0,
            Mandatory = true,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true,
            HelpMessage = "TIA Plc Blocks container")]
        [Alias("i")]
        public PlcBlockComposition Blocks
        {
            get { return blocks; }
            set { blocks = value; }
        }

        /// <summary>
        /// Gets or sets the blocks group
        /// </summary>
        [Parameter(Position = 1,
            Mandatory = false,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true,
            HelpMessage = "TIA Plc blocks group")]
        [Alias("g")]
        public PlcBlockUserGroupComposition Groups
        {
            get { return groups; }
            set { groups = value; }
        }

        /// <summary>
        /// Gets or sets the additional enumeration area in 'SystemGroups' 
        /// </summary>
        [Parameter(Position = 2,
            Mandatory = true,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true,
            HelpMessage = "Plc system blocks group")]
        [Alias("sg")]
        public PlcSystemBlockGroupComposition SystemBlockGroups
        {
            get { return sysGroups; }
            set { sysGroups = value; }
        }

        /// <summary>
        /// Gets or sets the block(s) name(s)
        /// </summary>
        [Parameter(Position = 3,
            Mandatory = true,
            HelpMessage = "Plc block name(s)")]
        [Alias("n")]
        public string[] Name
        {
            get { return name; }
            set { name = value; }
        }

        /// <summary>
        /// Gets or sets the block(s) storage path
        /// </summary>
        [Parameter(Position = 4,
            Mandatory = false,
            HelpMessage = "Plc block(s) path")]
        [Alias("p")]
        public string[] Path
        {
            get { return path; }
            set { path = value; }
        }


        /// <summary>
        /// Gets or sets the block(s) type
        /// </summary>
        [Parameter(Position = 5,
            Mandatory = true,
            HelpMessage = "Plc block(s) type")]
        [Alias("t")]
        public ProgramItemType Type
        {
            get { return itype; }
            set { itype = value; }
        }

        /// <summary>
        /// Gets or sets the block(s) programming language
        /// </summary>
        [Parameter(Position = 6,
            Mandatory = false,
            HelpMessage = "Plc block(s) programming language")]
        [Alias("l")]
        public string Language
        {
            get { return plang; }
            set { plang = value; }
        }

        #endregion

        protected override void BeginProcessing()
        {
            base.BeginProcessing();
        }

        protected override void ProcessRecord()
        {
            base.ProcessRecord();
            if (blocks != null) WriteObject(1);
            if (groups != null) WriteObject(2);
            if (sysGroups != null) WriteObject(3);
        }

        protected override void EndProcessing()
        {
            base.EndProcessing();
        }
    }
}
