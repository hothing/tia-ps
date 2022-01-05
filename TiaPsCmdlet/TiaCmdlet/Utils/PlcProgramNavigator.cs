using System;
using System.Collections.Generic;
using System.Management.Automation;
using Siemens.Engineering.SW.Blocks;

namespace TiaCmdlet.Utils
{
    class PlcProgramNavigator
    {
        private object itemsContainer = null;

        private Utils.ExtendedWildcardPattern filter = null;

        private PlcBlockComposition blocks;

        private PlcBlockUserGroupComposition ugroups;

        private PlcSystemBlockGroupComposition sgroups;

        public PlcBlockUserGroup GetGroup(string path)
        {
            return null;
        }

        public PlcBlock GetBlock(string path)
        {
            return null;
        }


    }
}
