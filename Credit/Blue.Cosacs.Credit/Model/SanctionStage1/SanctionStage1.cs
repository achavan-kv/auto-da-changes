using System.Collections.Generic;
namespace Blue.Cosacs.Credit.Model.SanctionStage1
{
    public class SanctionStage1
    {
        public int Id { get; set; }
        public bool IsApplicant2 { get; set; }
        public bool Filled { get; set; }
        public string ApplicationType { get; set; }
        public string Applicant1Name { get; set; }
        public string Applicant2Name { get; set; }
        public int Stage { get; set; }
        public Applicant Applicant { get; set; }
        public List<EmploymentHistory> EmploymentHistory { get; set; }
    }
}
