using System;
using System.Management.Automation;
using Siemens.Engineering;

namespace TiaCmdlet
{
    [Cmdlet(VerbsCommon.Get, "TiaDeviceList")]
    public class GetTiaDeviceList : PSCmdlet
    {
        private Project project = null;
               
        private WildcardPattern nameMatch = null;

        private char[] pathDelimeter = { '/', '.'};

        private string path = null;

        private string filter = null;

        private Boolean recursive = false;

        #region Command parameters

        /// <summary>
        /// Gets or sets the project to refer the root DeviceGroup(s)
        /// </summary>
        [Parameter(Mandatory = true,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true,
            Position = 0,
            HelpMessage = "TIA Project")]
        [Alias("i")]
        public Project Project
        {
            get { return project; }
            set { project = value; }
        }
                
        /// <summary>
        /// Gets or sets the path delimeter
        /// </summary>
        [Parameter(Mandatory = false,
            Position = 1,
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
            Position = 2,
            HelpMessage = "Device group path")]
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
            Position = 2,
            HelpMessage = "Device filter")]
        [Alias("f")]
        public string Filter
        {
            get { return filter; }
            set { filter = value; nameMatch = new WildcardPattern(filter); }
        }

        /// <summary>
        /// Gets or sets the path
        /// </summary>
        [Parameter(Mandatory = false,
            Position = 3,
            HelpMessage = "Traverse all devices recursive thru all groups")]
        [Alias("r")]
        public SwitchParameter Recursive
        {
            get { return recursive; }
            set { recursive = value; }
        }
        #endregion Command parameters

        #region command code

        #region internal commands
        private void WriteDeviceList(Siemens.Engineering.HW.DeviceComposition dc)
        {
            foreach (Siemens.Engineering.HW.Device dev in dc)
            {
                if (nameMatch != null)
                {
                    if (nameMatch.IsMatch(dev.Name)) { WriteObject(dev); }                    
                }
                else
                {
                    WriteObject(dev);
                }
            }            
        }

        private void UserGroupsTraverse(Siemens.Engineering.HW.DeviceUserGroupComposition ugc)
        {
            foreach (var ug in ugc)
            {
                if (ug != null) { 
                    WriteDeviceList(ug.Devices);
                    UserGroupsTraverse(ug.Groups);
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
            if (path == null)
            {
                WriteDeviceList(project.Devices);
                WriteDeviceList(project.UngroupedDevicesGroup.Devices);
                if (recursive)
                {
                    UserGroupsTraverse(project.DeviceGroups);                    
                }
            }
            else
            {
                string[] ugnames = path.Split(pathDelimeter, StringSplitOptions.RemoveEmptyEntries);
                WriteDebug($"path is {path}");
                Siemens.Engineering.HW.DeviceUserGroupComposition ugc = project.DeviceGroups;
                Siemens.Engineering.HW.DeviceUserGroup ug = null;
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
                    WriteDeviceList(ug.Devices);
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
