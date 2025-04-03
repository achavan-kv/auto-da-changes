using System;
using System.Collections.Generic;
using System.Linq;

namespace Blue.Cosacs.Credit.Model
{
    public class Proposal // Proposal Search
    {
        public int Id { get; set; }
        public List<Address> Addresses { get; set; }
        public Applicant1 Applicant1 { get; set; }
        public Applicant2 Applicant2 { get; set; }
        public SanctionStage1.SanctionStage1 SanctionStage1 { get; set; }
        public SanctionStage2.SanctionStage2 SanctionStage2 { get; set; }
        public string CompulsoryIdType { get; set; }

        public int? MonthlyIncome { get; set; }
        public LandlordDetails LandlordDetails { get; set; }
        public List<ProposalReference> References { get; set; }
        public string Nationality { get; set; }
        public string Occupation { get; set; }
        public string ApplicationType { get; set; }
        public string CurrentResidentialStatus { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CustomerType { get; set; }
        public DateTime? CurrentEmploymentDate { get; set; }

        public bool IsMonthlyIncomeChanged(Proposal previous)
        {
            return previous.MonthlyIncome == MonthlyIncome;
        }

        public bool IsCurrentEmploymentDateChanged(Proposal previous)
        {
            return previous.CurrentEmploymentDate == CurrentEmploymentDate;
        }

        public bool IsAddressChanged(Proposal previous)
        {
            var previousHomeAddress = previous.Addresses.Where(p => p.AddressType == "Home").FirstOrDefault();
            var currentHomeAddress = Addresses.Where(p => p.AddressType == "Home").FirstOrDefault();

            if (previousHomeAddress != null && currentHomeAddress != null)
            {
                return !(previousHomeAddress.City.ToUpper().Trim() == currentHomeAddress.City.ToUpper().Trim() &&
                       previousHomeAddress.DeliveryArea.ToUpper().Trim() == currentHomeAddress.DeliveryArea.ToUpper().Trim() &&
                       previousHomeAddress.PostCode.ToUpper().Trim() == currentHomeAddress.PostCode.ToUpper().Trim() &&
                       previousHomeAddress.Line1.ToUpper().Trim() == currentHomeAddress.Line1.ToUpper().Trim() &&
                       previousHomeAddress.Line2.ToUpper().Trim() == currentHomeAddress.Line2.ToUpper().Trim());
            }

            return false;
        }
    }

    public class ProposalApplicant1
    {
        public int Id { get; set; }
        public int Stage { get; set; }
        public string Applicant1Name { get; set; }
        public string Applicant2Name { get; set; }
        public bool HasApplicant2 { get; set; }
        public List<Address> Addresses { get; set; }
        public Applicant1 Applicant1 { get; set; }

        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
    }

    public class ProposalApplicant2
    {
        public int Id { get; set; }
        public int Stage { get; set; }
        public string Applicant1Name { get; set; }
        public string Applicant2Name { get; set; }
        public string ApplicationType { get; set; }
        public Applicant2 Applicant2 { get; set; }
    }

    public class LandlordDetails
    {
        public string LandlordName { get; set; }
        public string LandlordPhone { get; set; }
    }
}
