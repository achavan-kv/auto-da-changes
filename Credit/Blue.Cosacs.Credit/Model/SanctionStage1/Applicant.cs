using System;

namespace Blue.Cosacs.Credit.Model.SanctionStage1
{
    public class Applicant
    {
        public DateTime? DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string EthnicGroup { get; set; }
        public string Nationality { get; set; }

        public DateTime? DateInCurrentAddress { get; set; }
        public string CurrentResidentialStatus { get; set; }
        public string PropertyType { get; set; }
        public int? RentAmountPerMonth { get; set; }
        public int? NoOfDependants { get; set; }

        public DateTime? CurrentEmploymentDate { get; set; }
        public string EmploymentStatus { get; set; }
        public string Occupation { get; set; }
        public string PayFrequency { get; set; }

        public int? PayAmount { get; set; }
        public int? MonthlyIncome { get; set; }
        public string AdditionalIncomeFrequency { get; set; }
        public int? AdditionalIncomeAmount { get; set; }

        public int? UtilitiesFees { get; set; }
        public int? LoansOrCreditCardsFees { get; set; }
        public int? SchoolFees { get; set; }
        public int? OtherFees { get; set; }

        public bool? HasCreditCard { get; set; }
        public string PaymentMethod { get; set; }
        public string BankName { get; set; }
        public string AccountType { get; set; }

        public DateTime? DateBankAccountOpened { get; set; }
        public string AccountNumber { get; set; }
        public int? AccountBranch { get; set; }

        public string MaritalStatus { get; set; }

        public Applicant()
        {
        }

        internal Applicant(Credit.Proposal proposal)
        {
            this.DateOfBirth = proposal.DateOfBirth;
            this.Gender = proposal.Gender;
            this.EthnicGroup = proposal.EthnicGroup;
            this.Nationality = proposal.Nationality;
            this.DateInCurrentAddress = proposal.DateInCurrentAddress;
            this.CurrentResidentialStatus = proposal.CurrentResidentialStatus;
            this.PropertyType = proposal.PropertyType;
            this.RentAmountPerMonth = proposal.RentAmountPerMonth;
            this.CurrentEmploymentDate = proposal.CurrentEmploymentDate;
            this.EmploymentStatus = proposal.EmploymentStatus;
            this.Occupation = proposal.Occupation;
            this.PayFrequency = proposal.PayFrequency;
            this.PayAmount = proposal.PayAmount;
            this.MonthlyIncome = proposal.MonthlyIncome;
            this.AdditionalIncomeFrequency = proposal.AdditionalIncomeFrequency;
            this.AdditionalIncomeAmount = proposal.AdditionalIncomeAmount;
            this.UtilitiesFees = proposal.UtilitiesFees;
            this.LoansOrCreditCardsFees = proposal.LoansOrCreditCardsFees;
            this.SchoolFees = proposal.SchoolFees;
            this.OtherFees = proposal.OtherFees;
            this.HasCreditCard = proposal.HasCreditCard;
            this.PaymentMethod = proposal.PaymentMethod;
            this.DateBankAccountOpened = proposal.DateBankAccountOpened;
            this.BankName = proposal.BankName;
            this.AccountType = proposal.AccountType;
            this.AccountNumber = proposal.AccountNumber;
            this.AccountBranch = proposal.AccountBranch;
            this.NoOfDependants = proposal.NoOfDependants;
            this.MaritalStatus = proposal.MaritalStatus;
        }

        internal Applicant(Credit.ProposalApplicant2 proposal)
        {
            this.DateOfBirth = proposal.DateOfBirth;
            this.Gender = proposal.Gender;
            this.EthnicGroup = proposal.EthnicGroup;
            this.Nationality = proposal.Nationality;
            this.DateInCurrentAddress = proposal.DateInCurrentAddress;
            this.CurrentResidentialStatus = proposal.CurrentResidentialStatus;
            this.PropertyType = proposal.PropertyType;
            this.RentAmountPerMonth = proposal.RentAmountPerMonth;
            this.CurrentEmploymentDate = proposal.CurrentEmploymentDate;
            this.EmploymentStatus = proposal.EmploymentStatus;
            this.Occupation = proposal.Occupation;
            this.PayFrequency = proposal.PayFrequency;
            this.PayAmount = proposal.PayAmount;
            this.MonthlyIncome = proposal.MonthlyIncome;
            this.AdditionalIncomeFrequency = proposal.AdditionalIncomeFrequency;
            this.AdditionalIncomeAmount = proposal.AdditionalIncomeAmount;
            this.UtilitiesFees = proposal.UtilitiesFees;
            this.LoansOrCreditCardsFees = proposal.LoansOrCreditCardsFees;
            this.SchoolFees = proposal.SchoolFees;
            this.OtherFees = proposal.OtherFees;
            this.HasCreditCard = proposal.HasCreditCard;
            this.PaymentMethod = proposal.PaymentMethod;
            this.DateBankAccountOpened = proposal.DateBankAccountOpened;
            this.BankName = proposal.BankName;
            this.AccountType = proposal.AccountType;
            this.AccountNumber = proposal.AccountNumber;
            this.AccountBranch = proposal.AccountBranch;
            this.NoOfDependants = proposal.NoOfDependants;
            this.MaritalStatus = proposal.MaritalStatus;
        }
    }
}
