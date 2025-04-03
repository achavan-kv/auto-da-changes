namespace Blue.Cosacs.Credit.Model.SanctionStage2
{
    public class Applicant1
    {
        public string PreviousAddress { get; set; }
        public string LandlordName { get; set; }
        public string LandlordPhone { get; set; }
        public string EmployerName { get; set; }
        public string EmployerAddress { get; set; }
        public string EmployerDepartment { get; set; }
        public string EmployerWorkplacePhone { get; set; }

        public Applicant1()
        {
        }

        internal Applicant1(Credit.Proposal proposal)
        {
            this.PreviousAddress = proposal.PreviousAddress;
            this.LandlordName = proposal.LandlordName;
            this.LandlordPhone = proposal.LandlordPhone;
            this.EmployerName = proposal.EmployerName;
            this.EmployerAddress = proposal.EmployerAddress;
            this.EmployerDepartment = proposal.EmployerDepartment;
            this.EmployerWorkplacePhone = proposal.EmployerWorkplacePhone;
        }
    }
}
