﻿using System;
using System.Management.Automation;
using Siemens.Engineering;

namespace TiaCmdlet
{
    [Cmdlet(VerbsCommon.Get, "TiaDevices")]
    public class GetTiaDevices : PSCmdlet
    {
        private Project project = null;
               
        private WildcardPattern nameMatch = null;

        private char[] pathDelimeter = { '/', '.'};

        private string path = null;

        private Boolean enumActive = true;

        private Boolean enumPassive = true;

        private Boolean recursive = false;

        #region Command parameters

        /// <summary>
        /// Gets or sets the project to refer the root DeviceGroup(s)
        /// </summary>
        [Parameter(Mandatory = true,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true,
            ParameterSetName = "RefRoot",
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
            Position = 2,
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
            Position = 3,
            HelpMessage = "Device group path")]
        [Alias("f")]
        public string Path
        {
            get { return path; }
            set { path = value; }
        }

        /// <summary>
        /// The enumeration mode: all devices
        /// </summary>
        [Parameter(Mandatory = false,
            Position = 4,
            HelpMessage = "The enumeration mode: all devices")]
        [Alias("l")]
        public SwitchParameter All  {
            get { return enumActive && enumPassive; }
            set { enumActive = value; enumPassive = enumActive; }
        }

        /// <summary>
        /// The enumeration mode: only active devices such as PC, PLC, NetworkHost
        /// </summary>
        [Parameter(Mandatory = false,
            Position = 4,
            HelpMessage = "The enumeration mode: only active devices such as PC, PLC, NetworkHost")]
        [Alias("a")]
        public SwitchParameter Active
        {
            get { return enumActive && !enumPassive; }
            set { enumActive = value; enumPassive = false; }
        }

        /// <summary>
        /// The enumeration mode: only passive devices (any what is not active)
        /// </summary>
        [Parameter(Mandatory = false,
            Position = 4,
            HelpMessage = "The enumeration mode: only passive devices (any what is not active)")]
        [Alias("p")]
        public SwitchParameter Passive
        {
            get { return enumPassive; }
            set { enumPassive = value; enumActive = false; }
        }

        /// <summary>
        /// The enumeration mode: only passive devices (any what is not active)
        /// </summary>
        [Parameter(Mandatory = false,
            Position = 4,
            HelpMessage = "The enumeration mode: only passive devices (any what is not active)")]
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
                if (enumActive && enumPassive)
                {
                    WriteObject(dev);
                } 
                else
                {
                    bool isPassive = dev.IsGsd;
                    if (!isPassive) {
                        foreach (Siemens.Engineering.HW.DeviceItem di in dev.Items)
                        {
                            var attr = di.GetAttribute("Classification");
                            //NB: I use a trick: if the attribute Çlassification' is null then a deviceitem is passive
                            isPassive = isPassive && ( attr == null);
                        }
                    }
                    if (enumActive && !enumPassive && !isPassive) 
                        { WriteObject(dev); }
                    else if (!enumActive && enumPassive && isPassive)
                        { WriteObject(dev); }
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
            if (!enumActive && !enumPassive)
            {
                enumActive = true;
                enumPassive = true;
            }
        }
        protected override void ProcessRecord()
        {
            base.ProcessRecord();
            if (path == null)
            {
                if (!recursive)
                {
                    WriteDeviceList(project.Devices);
                    WriteDeviceList(project.UngroupedDevicesGroup.Devices);
                }
                else
                {
                    UserGroupsTraverse(project.DeviceGroups);                    
                }
            }
            else
            {
                string[] ugnames = path.Split(pathDelimeter, 0, StringSplitOptions.RemoveEmptyEntries);
                Siemens.Engineering.HW.DeviceUserGroupComposition ugc = project.DeviceGroups;
                Siemens.Engineering.HW.DeviceUserGroup ug = null;

                foreach (String gn in ugnames)
                {
                    if (ugc != null)
                    {
                        WriteDebug($"the group {gn} is finding");
                        ug = ugc.Find(gn);
                        if (ug != null)
                        {
                            ugc = ug.Groups;
                            WriteDebug("the group {0} is found");
                        }
                        else
                        {
                            ThrowTerminatingError(new ErrorRecord(new ItemNotFoundException(), "the specified group does not exist", ErrorCategory.InvalidArgument, gn));
                            break;                            
                        }
                    }
                    else
                    {
                        ThrowTerminatingError(new ErrorRecord(new ItemNotFoundException(), "the specified group does not exist", ErrorCategory.InvalidArgument, gn));
                        break;
                    }
                }
                if (ug != null) { WriteDeviceList(ug.Devices); }       
                else { WriteWarning("The user group is empty. Internal error."); }
            }
        }

        protected override void EndProcessing()
        {
            base.EndProcessing();
            nameMatch = null;
        }
        #endregion command code
    }
}
