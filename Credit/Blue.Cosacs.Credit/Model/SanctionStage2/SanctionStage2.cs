using System.Collections.Generic;

namespace Blue.Cosacs.Credit.Model.SanctionStage2
{
    public class SanctionStage2
    {
        public int Id { get; set; }
        public string Applicant1Name { get; set; }
        public string Applicant2Name { get; set; }
        public int Stage { get; set; }
        public string ApplicationType { get; set; }
        public Applicant1 Applicant1 { get; set; }
        public List<ProposalReference> References { get; set; }
        public int MonthsInCurrentAddress { get; set; }
        public string CurrentResidentialStatus { get; set; }
        public bool Filled { get; set; }
        public bool FilledSelfEmployed { get; set; }
        public bool IsApplicant2 { get; set; }
    }
}
