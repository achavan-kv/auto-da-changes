using System;
using System.Collections.Generic;
using System.Text;

namespace Blue.Cosacs.Shared.Services.StoreCard
{
    partial class ActivateRequest
    {
        public string ProofAddress { get; set; }
        public string ProofAddNotes { get; set; }
        public string ProofID { get; set; }
        public string ProofIDNotes { get; set; }
        public string SecurityQ { get; set; }
        public string SecurityA { get; set; }
        public int EmpeeNo { get; set; }
        public short BranchNo { get; set; }
        public long CardNumber { get; set; }
        public string Reason { get; set; }
        public DateTime Date { get; set; }
    }
    
    partial class ActivateResponse 
    { 
        // put your properties/fields here
        public StoreCardStatus StoreCardStatus  { get; set; }
        public string Empeename { get; set; }

    }
}
