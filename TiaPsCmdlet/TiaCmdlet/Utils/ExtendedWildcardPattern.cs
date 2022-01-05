using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;

namespace TiaCmdlet.Utils
{
    class ExtendedWildcardPattern
    {
        private readonly WildcardPattern includeFilter = null;

        private readonly WildcardPattern excludeFilter = null;

        public ExtendedWildcardPattern(string includePattern, string excludePattern)
        {
            includeFilter = new WildcardPattern(includePattern);
            excludeFilter = new WildcardPattern(excludePattern);
        }

        public ExtendedWildcardPattern(string includePattern, string excludePattern, WildcardOptions options)
        {
            includeFilter = new WildcardPattern(includePattern, options);
            excludeFilter = new WildcardPattern(excludePattern, options);
        }

        public bool IsMatch(string str)
        {
            return includeFilter.IsMatch(str) && !excludeFilter.IsMatch(str);
        }
    }
}
