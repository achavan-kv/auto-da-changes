using System;
using System.Collections.Generic;
using System.Text;

namespace Blue.Cosacs.Shared
{
    public class InstResolutionResult
    {
        public InstallationResolution Resolution { get; set; }
        public List<InstallationSparePart> SpareParts { get; set; }
    }
}
