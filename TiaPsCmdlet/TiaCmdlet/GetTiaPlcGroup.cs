using System;
using System.Linq;
using System.Management.Automation;
using Siemens.Engineering;

namespace TiaCmdlet
{
    [Cmdlet(VerbsCommon.Get, "TiaPlcGroup")]
    public class GetTiaPlcGroup : PSCmdlet
    {

        private Siemens.Engineering.SW.PlcSoftware software = null;

        private string path = null;

        private char[] pathDelimeter = { '/', '.' };

        private Boolean inSystemGroup = false;

        #region Command parameters

        /// <summary>
        /// Gets or sets the plc group
        /// </summary>
        [Parameter(
        Position = 0,
        Mandatory = true,
        ValueFromPipeline = true,
        ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty]
        [Alias("i")]
        public Siemens.Engineering.SW.PlcSoftware InputObject
        {
            get { return this.software; }
            set { this.software = value; }
        }

        /// <summary>
        /// Gets or sets a group path in the program folders
        /// </summary>
        [Parameter(Mandatory = true,
            HelpMessage = "a group path in the program folders")]
        [Alias("n")]
        public string Path
        {
            get { return path; }
            set { path = value; }
        }

        /// <summary>
        /// Gets or sets the path delimeter
        /// </summary>
        [Parameter(Mandatory = false,
            HelpMessage = "Path delimeter")]
        [Alias("d")]
        public string Delimeter
        {
            get { return pathDelimeter.ToString(); }
            set { pathDelimeter = value.ToCharArray(); }
        }

        /// <summary>
        /// Gets or sets the system blocks
        /// </summary>
        [Parameter(Mandatory = false,
            HelpMessage = "Include the system blocks")]
        [Alias("sg")]
        public SwitchParameter SystemGroup
        {
            get { return inSystemGroup; }
            set { inSystemGroup = value; }
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
            if (!inSystemGroup)
            {
                if (
                    (path == null) 
                    || (path.Equals(pathDelimeter[0].ToString()))
                    || (path.Equals(pathDelimeter[1].ToString()))
                    )
                {
                    WriteObject(software.BlockGroup.Blocks);
                }
                else 
                {
                    string[] ugnames = path.Split(pathDelimeter, StringSplitOptions.RemoveEmptyEntries);
                    WriteDebug($"path is {path}");
                    Siemens.Engineering.SW.Blocks.PlcBlockUserGroupComposition ugc = software.BlockGroup.Groups;
                    Siemens.Engineering.SW.Blocks.PlcBlockUserGroup ug = null;
                    if (ugnames.Length > 0)
                    {
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
                    if (ug != null)
                    {
                        WriteObject(ug.Blocks);
                    }
                    else { WriteWarning("The user group is empty or doesn't exist."); }
                }                
            } 
            else
            {
                if (
                   (path == null)
                   || (path.Equals(pathDelimeter[0].ToString()))
                   || (path.Equals(pathDelimeter[1].ToString()))
                   )
                {
                    var sbg = software.BlockGroup.SystemBlockGroups.First();
                    if (sbg != null)
                    {
                        WriteObject(sbg.Blocks);
                    }                    
                }
                else
                {
                    string[] ugnames = path.Split(pathDelimeter, StringSplitOptions.RemoveEmptyEntries);
                    WriteDebug($"path is {path}");
                    Siemens.Engineering.SW.Blocks.PlcSystemBlockGroupComposition ugc = software.BlockGroup.SystemBlockGroups;
                    Siemens.Engineering.SW.Blocks.PlcSystemBlockGroup ug = null;
                    if (ugnames.Length > 0)
                    {
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
                    if (ug != null)
                    {
                        WriteObject(ug.Blocks);
                    }
                    else { WriteWarning("The user group is empty or doesn't exist."); }
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
