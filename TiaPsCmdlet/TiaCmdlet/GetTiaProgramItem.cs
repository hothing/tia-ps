using System;
using System.Linq;
using System.Management.Automation;
using Siemens.Engineering.SW.Blocks;

namespace TiaCmdlet
{
    [Cmdlet(VerbsCommon.Get, "TiaProgramItem")]
    public class GetTiaProgramItem : PSCmdlet
    {
        private PlcBlockComposition blocks;

        private PlcBlockUserGroupComposition groups;

        private PlcSystemBlockGroupComposition sysGroups;

        private Utils.ExtendedWildcardPattern filter = null;

        private string includePattern;

        private string excludePattern;

        private char pathDelimeter = '/';

        private string path = null;
                
        private bool recursive = false;

        private bool contentIsGroup = false;

        #region Command parameters

        /// <summary>
        /// Gets or sets the blocks container
        /// </summary>
        [Parameter(Position = 0,
            Mandatory = true,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true,
            HelpMessage = "Plc Blocks container")]
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
            HelpMessage = "Plc blocks group")]
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
        /// Gets or sets the path
        /// </summary>
        [Parameter(Position = 3,
            Mandatory = false,
            HelpMessage = "Block group path")]
        [Alias("p")]
        public string Path
        {
            get { return path; }
            set { path = value; }
        }

        /// <summary>
        /// Gets or sets the 'recursive' enumeration method 
        /// </summary>
        [Parameter(Position = 4,
            Mandatory = false,
            HelpMessage = "Traverse all blocks recursive thru all groups")]
        [Alias("r")]
        public SwitchParameter Recursive
        {
            get { return recursive; }
            set { recursive = value; }
        }

        /// <summary>
        /// Gets or sets the content type
        /// </summary>
        [Parameter(Position = 5,
            Mandatory = false,
            HelpMessage = "Content type")]
        [Alias("ct")]
        public SwitchParameter ContentIsGroup
        {
            get { return contentIsGroup; }
            set { contentIsGroup = value; }
        }

        /// <summary>
        /// Gets or sets the path delimeter
        /// </summary>
        [Parameter(Position = 6,
            Mandatory = false,
            ParameterSetName = "withPath",
            HelpMessage = "A group path delimeter")]
        [Alias("d")]
        public char Delimeter
        {
            get { return pathDelimeter; }
            set { pathDelimeter = value; }
        }

        /// <summary>
        /// Gets or sets the include filter pattern
        /// </summary>
        [Parameter(Mandatory = false,
            HelpMessage = "include filter pattern")]
        [Alias("inc")]
        public string Include
        {
            get { return includePattern; }
            set { includePattern = value; }
        }

        /// <summary>
        /// Gets or sets the exclude filter pattern
        /// </summary>
        [Parameter(Mandatory = false,
            HelpMessage = "exclude filter pattern")]
        [Alias("exc")]
        public string Exclude
        {
            get { return excludePattern; }
            set { excludePattern = value; }
        }

        #endregion Command parameters

        #region command code

        #region internal commands

        private void WriteBlockList(PlcBlockComposition plcBlocks)
        {
            WriteDebug("WriteBlockList/1");
            foreach (PlcBlock blk in plcBlocks)
            {
                WriteObject(blk);
            }
        }

        private void WriteBlockList(string path, PlcBlockComposition plcBlocks)
        {
            WriteDebug("WriteBlockList/2");
            if (filter != null)
            {
                var npath = path + pathDelimeter;
                foreach (PlcBlock blk in plcBlocks)
                {
                    var opath = npath + blk.Name;
                    WriteDebug($"[WriteBlockList] matching the {opath} with {includePattern} and with {excludePattern}");
                    if (filter.IsMatch(opath)) { WriteObject(blk); }
                }
            }
            else
            {
                WriteBlockList(plcBlocks);
            }
        }

        private void WriteGroupList(PlcBlockGroup plcGroup)
        {
            WriteDebug($"WriteGroupList/1 for {plcGroup?.Name}");
            foreach (PlcBlockGroup grp in plcGroup?.Groups)
            {
                WriteObject(grp);
            }
        }

        private void WriteGroupList(string path, PlcBlockGroup plcGroup)
        {
            WriteDebug($"WriteGroupList/2 for {plcGroup?.Name}");
            if (filter != null)
            {
                var npath = path + pathDelimeter;
                foreach (PlcBlockGroup grp in plcGroup?.Groups)
                {
                    var opath = npath + grp.Name;
                    WriteDebug($"[WriteGroupList] matching the {opath} with {includePattern} and with {excludePattern}");
                    if (filter.IsMatch(opath)) { WriteObject(grp); }
                }                   
            }
            else
            {
                WriteGroupList(plcGroup);
            }
        }

        private void WriteGroupList(PlcSystemBlockGroup plcGroup)
        {
            WriteDebug($"WriteGroupList/3 for {plcGroup?.Name}");
            foreach (PlcSystemBlockGroup grp in plcGroup?.Groups)
            {
                WriteObject(grp);
            }
        }

        private void WriteGroupList(string path, PlcSystemBlockGroup plcGroup)
        {
            WriteDebug($"WriteGroupList/4 for {plcGroup?.Name}");
            if (filter != null)
            {
                var npath = path + pathDelimeter;
                foreach (PlcSystemBlockGroup grp in plcGroup?.Groups)
                {
                    var opath = npath + grp.Name;
                    WriteDebug($"[WriteGroupList] matching the {opath} with {includePattern} and with {excludePattern}"); 
                    if (filter.IsMatch(opath)) { WriteObject(grp); }
                }
            }
            else
            {
                WriteGroupList(plcGroup);
            }
        }

        #region Select/Traverese User Groups


        private void TraverseUserGroups(PlcBlockUserGroupComposition ugc)
        {
            WriteDebug("[TraverseUserGroups]");
            foreach (var ug in ugc)
            {
                if (ug != null) {
                    if (!contentIsGroup) { WriteBlockList(ug.Blocks); }
                    else { WriteObject(ug); }
                    if (recursive) TraverseUserGroups(ug.Groups);
                }
            }
        }


        private void SelectInUserGroups(string path, PlcBlockUserGroupComposition ugc)
        {
            WriteDebug($"[SelectInUserGroups] for {path}");
            if (filter != null)
            {
                var npath = path + pathDelimeter;
                foreach (var ug in ugc)
                {
                    if (ug != null)
                    {
                        var opath = npath + ug.Name;
                        WriteDebug($"[SelectInUserGroups] select from the {opath}");
                        {
                            if (!contentIsGroup)
                            {
                                WriteBlockList(opath, ug.Blocks);
                            }
                            else
                            {
                                WriteObject(ug);
                                //WriteGroupList(opath, ug);
                            }
                        }
                        if (recursive) SelectInUserGroups(opath, ug.Groups);
                    }
                }
            }          
            else
            {
                TraverseUserGroups(ugc);
            }
        }

        private PlcBlockGroup GetUserGroup(string path, PlcBlockUserGroupComposition root)
        {
            PlcBlockUserGroupComposition ugc = root;
            PlcBlockGroup grp = null;
            char[] sep = { pathDelimeter };
            string[] ugnames = path.Split(sep, StringSplitOptions.RemoveEmptyEntries);
            WriteDebug($"[GetUserGroup] path is {path}");
            if (ugnames.Length > 0)
            {
                WriteDebug($"[GetUserGroup] root group is {ugnames[0]}");
                foreach (String gn in ugnames)
                {
                    if (ugc != null)
                    {
                        WriteDebug($"[GetUserGroup] the group {gn} is finding");
                        grp = ugc.Find(gn);
                        if (grp != null)
                        {
                            ugc = grp.Groups;
                            WriteDebug($"[GetUserGroup] the group {gn} is found");
                        }
                        else
                        {
                            WriteDebug($"[GetUserGroup] the group {gn} is not found");
                            break;
                        }
                    }
                    else
                    {
                        grp = null;
                        break;
                    }
                }
                return grp;
            }
            return null;
        }
        #endregion

        #region Select/Traverese System Groups

        private void TraverseSysGroups(PlcSystemBlockGroupComposition sgc)
        {
            foreach (var sg in sgc)
            {
                if (sg != null)
                {
                    if (!contentIsGroup)
                    {
                        WriteBlockList(sg.Blocks);
                    }
                    else
                    {
                        WriteGroupList(sg);
                    }
                    if (recursive) TraverseSysGroups(sg.Groups);
                }
            }
        }

        private void SelectInSysGroups(string path, PlcSystemBlockGroupComposition sgc)
        {
            if (filter != null)
            {
                var npath = path + pathDelimeter;
                foreach (var sg in sgc)
                {
                    if (sg != null)
                    {
                        var opath = npath + sg.Name;
                        WriteDebug($"[SelectInSysGroups] select from the {opath}"); 
                        {
                            if (!contentIsGroup)
                            {
                                WriteBlockList(opath, sg.Blocks);
                            }
                            else
                            {
                                WriteGroupList(opath, sg);
                            }
                        }
                        if (recursive) SelectInSysGroups(opath, sg.Groups);
                    }
                }
            }
            else
            {
                TraverseSysGroups(sgc);
            }
        }

        private PlcSystemBlockGroup GetSysGroup(string path, PlcSystemBlockGroupComposition root)
        {
            char[] sep = { pathDelimeter };
            string[] ugnames = path.Split(sep, StringSplitOptions.RemoveEmptyEntries);
            PlcSystemBlockGroupComposition ugc = root;
            PlcSystemBlockGroup grp = null;

            WriteDebug($"[GetSysGroup] path is {path}");
            if (ugnames.Length > 0)
            {
                WriteDebug($"[GetSysGroup] root group is {ugnames[0]}");
                foreach (String gn in ugnames)
                {
                    if (ugc != null)
                    {
                        WriteDebug($"[GetSysGroup] group {gn} is finding");
                        grp = ugc.Find(gn);
                        if (grp != null)
                        {
                            ugc = grp.Groups;
                            WriteDebug($"[GetSysGroup] group {gn} is found");
                        }
                        else
                        {
                            WriteDebug($"[GetSysGroup] group {gn} is not found");
                            break;
                        }
                    }
                    else
                    {
                        grp = null;
                        break;
                    }
                }
                return grp;
            }
            return null;
        }
        #endregion

        #endregion internal commands

        protected override void BeginProcessing()
        {
            base.BeginProcessing();
                        
            // filter parameters handling
            if (includePattern != null)
            {
                if (!includePattern.StartsWith("*")) includePattern = $"{pathDelimeter}{includePattern}";
                filter = new Utils.ExtendedWildcardPattern(includePattern, "", WildcardOptions.CultureInvariant);
                if (excludePattern != null)
                {
                    if (!excludePattern.StartsWith("*")) excludePattern = $"{pathDelimeter}{excludePattern}";
                    filter = new Utils.ExtendedWildcardPattern(includePattern, excludePattern, WildcardOptions.CultureInvariant);
                }
            } else if (excludePattern != null)
            {
                if (!excludePattern.StartsWith("*")) excludePattern = $"{pathDelimeter}{excludePattern}";
                filter = new Utils.ExtendedWildcardPattern("*", excludePattern, WildcardOptions.CultureInvariant);
            }
            WriteDebug($"includePattern: {includePattern}");
            WriteDebug($"excludePattern: {excludePattern}");
        }

        protected override void ProcessRecord()
        {
            base.ProcessRecord();
            var root_path = pathDelimeter.ToString();
            bool useRoot = true;
            PlcBlockGroup ug;
            // NB: path is absolute, any level or object can be selected                
            PlcBlockComposition ublocks = blocks;
            PlcBlockUserGroupComposition ugc = groups;

            if ((path == null) || (path.Equals(pathDelimeter.ToString())))
            {
                path = "";
            }
            else
            {
                useRoot = false;
            }
            if (useRoot)
            {
                if (!contentIsGroup) {
                    WriteBlockList(path, ublocks);
                    if (recursive) { SelectInUserGroups(path, ugc); }
                }
                else
                {
                    SelectInUserGroups(path, ugc);
                }                        
            }
            else
            {
                ug = GetUserGroup(path, ugc);
                if (ug != null)
                {
                    if (!contentIsGroup)
                    {
                        WriteBlockList(path, ug.Blocks);
                        if (recursive) { SelectInUserGroups(path, ug.Groups); }
                    }
                    else
                    {
                        SelectInUserGroups(path, ug.Groups);
                    }
                }
                else
                {
                    WriteDebug($"The path '{path}' is not found in the user program");
                }
            }
            if (sysGroups != null)
            {   
                if (useRoot)
                {
                    path = "";
                    SelectInSysGroups(path, sysGroups);
                }
                else
                {
                    PlcSystemBlockGroup sg;
                    sg = GetSysGroup(path, sysGroups);
                    if (sg != null)
                    {
                        if (!contentIsGroup) 
                        {
                            WriteBlockList(path, sg.Blocks);
                            if (recursive) { SelectInSysGroups(path, sg.Groups); }
                        }
                        else
                        {
                            SelectInSysGroups(path, sg.Groups);
                        }
                    }
                    else
                    {
                        WriteDebug($"The path '{path}' is not found in the system resources");
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
