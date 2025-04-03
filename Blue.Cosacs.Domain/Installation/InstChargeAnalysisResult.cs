using System;
using System.Collections.Generic;
using System.Text;

namespace Blue.Cosacs.Shared
{
    public class InstChargeAnalysisResult
    {
        public int InstNo { get; set; }
        public string BreakDownText { get; set; }
        public string BreakDownCode { get; set; }
        public decimal Electrical { get; set; }
        public decimal Furniture { get; set; }
    }
}
