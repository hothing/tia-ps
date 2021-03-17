using System;
using System.Linq;
using System.Management.Automation;
using Siemens.Engineering;
using Siemens.Engineering.HW;
using Siemens.Engineering.SW;

namespace TiaCmdlet
{
    [Cmdlet(VerbsCommon.Get, "TiaPlcBlock")]
    public class GetTiaPlcBlock : PSCmdlet
    {
        private PlcSoftware program = null;
               
        private WildcardPattern nameMatch = null;

        private WildcardPattern includeGroup = null;

        private WildcardPattern excludeGroup = null;

        private char[] pathDelimeter = { '/', '.'};

        private string path = null;

        private string filter = null;

        private Boolean recursive = false;

        private Boolean includeSystemGroup = false;

        #region Command parameters

        /// <summary>
        /// Gets or sets the project to refer the root DeviceGroup(s)
        /// </summary>
        [Parameter(Mandatory = true,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true,
            Position = 0,
            HelpMessage = "TIA Plc Program")]
        [Alias("i")]
        public PlcSoftware Program
        {
            get { return program; }
            set { program = value; }
        }
                
        /// <summary>
        /// Gets or sets the path delimeter
        /// </summary>
        [Parameter(Mandatory = false,
            ParameterSetName = "withPath",
            HelpMessage = "Device group path delimeter")]
        [Alias("d")]
        public string Delimeter
        {
            get { return pathDelimeter.ToString(); }
            set { pathDelimeter = value.ToCharArray(); }
        }

        /// <summary>
        /// Gets or sets the path
        /// </summary>
        [Parameter(Mandatory = false,
            ParameterSetName = "withPath",
            HelpMessage = "Block group path")]
        [Alias("p")]
        public string Path
        {
            get { return path; }
            set { path = value; }
        }

        /// <summary>
        /// Gets or sets the path
        /// </summary>
        [Parameter(Mandatory = false,
            HelpMessage = "Block filter")]
        [Alias("f")]
        public string Filter
        {
            get { return filter; }
            set { filter = value; nameMatch = new WildcardPattern(filter); }
        }

        /// <summary>
        /// Gets or sets the 'recursive' enumeration method 
        /// </summary>
        [Parameter(Mandatory = false,
            HelpMessage = "Traverse all blocks recursive thru all groups")]
        [Alias("r")]
        public SwitchParameter Recursive
        {
            get { return recursive; }
            set { recursive = value; }
        }

        /// <summary>
        /// Gets or sets the system blocks
        /// </summary>
        [Parameter(Mandatory = false,
            HelpMessage = "Include the system blocks")]
        [Alias("sg")]
        public SwitchParameter IncludeSystemGroup
        {
            get { return includeSystemGroup; }
            set { includeSystemGroup = value; }
        }

        #endregion Command parameters

        #region command code

        #region internal commands
        private void WriteBlockList(Siemens.Engineering.SW.Blocks.PlcBlockComposition bc)
        {
            foreach (Siemens.Engineering.SW.Blocks.PlcBlock blk in bc)
            {
                if (nameMatch != null)
                {
                    if (nameMatch.IsMatch(blk.Name)) { WriteObject(blk); }                    
                }
                else
                {
                    WriteObject(blk);
                }
            }            
        }

        private void UserGroupsTraverse(Siemens.Engineering.SW.Blocks.PlcBlockUserGroupComposition ugc)
        {
            foreach (var ug in ugc)
            {
                if (ug != null) {
                    WriteBlockList(ug.Blocks);
                    UserGroupsTraverse(ug.Groups);
                }
            }
        }

        private void SysGroupsTraverse(Siemens.Engineering.SW.Blocks.PlcSystemBlockGroupComposition ugc)
        {
            foreach (var ug in ugc)
            {
                if (ug != null)
                {
                    WriteBlockList(ug.Blocks);
                    SysGroupsTraverse(ug.Groups);
                }
            }
        }
        #endregion internal commands

        protected override void BeginProcessing()
        {
            base.BeginProcessing();           
        }
        protected override void ProcessRecord()
        {
            base.ProcessRecord();
            if (
                    (path == null)
                    || (path.Equals(pathDelimeter[0].ToString()))
                    || (path.Equals(pathDelimeter[1].ToString()))
               )
            {
                WriteBlockList(program.BlockGroup.Blocks);
                               
                if (recursive)
                {
                    UserGroupsTraverse(program.BlockGroup.Groups);                    
                }

                if (includeSystemGroup)
                {
                    if (recursive) { SysGroupsTraverse(program.BlockGroup.SystemBlockGroups); }
                    else {
                        var bx = program.BlockGroup.SystemBlockGroups.First();
                        WriteBlockList(bx.Blocks);  
                    }
                }
            }
            else
            {
                string[] ugnames = path.Split(pathDelimeter, StringSplitOptions.RemoveEmptyEntries);
                WriteDebug($"path is {path}");
                Siemens.Engineering.SW.Blocks.PlcBlockUserGroupComposition ugc = program.BlockGroup.Groups;
                Siemens.Engineering.SW.Blocks.PlcBlockUserGroup ug = null;
                if (ugnames.Length > 0) { 
                    WriteDebug($"the root group is {ugnames[0]}");
                    foreach (String gn in ugnames)
                    {
                        if (ugc != null)
                        {
                            WriteDebug($"the group {gn} is finding");
                            ug = ugc.Find(gn);
                            if (ug != null)
                            {
                                ugc = ug.Groups;
                                WriteDebug($"the group {gn} is found");
                            }
                            else
                            {
                                ThrowTerminatingError(new ErrorRecord(new ItemNotFoundException(), $"the specified group '{gn}' does not exist", ErrorCategory.InvalidArgument, path));
                                break;
                            }
                        }
                        else
                        {
                            ThrowTerminatingError(new ErrorRecord(new ItemNotFoundException(), $"the specified group does '{gn}' not exist", ErrorCategory.InvalidArgument, path));
                            break;
                        }
                    }
                }
                else 
                {
                    WriteDebug($"the single group is {path}");
                    ug = ugc.Find(path);                    
                }
                if (ug != null) {
                    WriteBlockList(ug.Blocks);
                    if (recursive) 
                    {
                        UserGroupsTraverse(ug.Groups); 
                    }                    
                }       
                else { WriteWarning("The user group is empty or doesn't exist."); }
            }
        }

        protected override void EndProcessing()
        {
            base.EndProcessing();
        }
        #endregion command code
    }
}
