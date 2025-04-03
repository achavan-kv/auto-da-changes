using System;
using System.Collections.Generic;
using System.Text;

namespace Blue.Cosacs.Shared
{
    public partial class Acct : Entity
    { 
        public Agreement Agreement { get; set; }
        public Instalplan InstalPlan { get; set; }
        public Proposal Proposal { get; set; }
        public CustAcct CustAcct { get; set; }
        public CustAddress HomeAddress { get; set; }
        public List<StoreCard> Storecards { get; set; }
        public StorecardPaymentDetails StoreCardPaymentDetails { get; set; }

        public List<FinTrans> FinTrans { get; set; }
        public List <ProposalFlag> ProposalFlags { get; set; }
        public List<InstantCreditFlag> InstantCreditFlags { get; set; }

        public bool DCCleared { get; set; }
        //public Customer Customer { get; set; }
        public bool DepositorFIOutstanding { get; set; }
        public bool DocControlCleared { get; set; }
        public decimal FirstIntalmentPayable { get; set; }

        public bool ICDepositFlagOustanding { get; set; }
        public bool ICInstalOutstanding { get; set; }
        public bool ICInstalFlagOutstanding { get; set; }
        
        public bool ICArrearsOutstanding { get; set; }
        public bool ICChequeOutstanding { get; set; }
        public bool ICChequeFlagOutstanding { get; set; }
        public bool ICReferralOutstanding { get; set; }
        public bool ICReferralFlagOutstanding { get; set; }

        public bool S1FlagOutstanding { get; set; }
        public bool S2FlagOutstanding { get; set; }
        public bool UWFlagOutstanding { get; set; }

        public bool ClearDeposit { get; set; }          //IP - 08/03/11 - #3288
        public bool ClearInstalment { get; set; }       //IP - 08/03/11 - #3288

        public float MaxCustArrearsLevel { get; set; }
        public float MaxCustLiveHistoricArrearsLevel { get; set; }
        public float MaxCustSettledArrearsLevel { get; set; }


        public decimal CustomerLiveMonthsHistory { get; set; }
        public decimal CustomerSettledMonthsHistory { get; set; }

        public float SpendUpliftPercentage { get; set; }


        public void PrintStatement(DateTime datefrom)
        {
            //to do determine if we are printing using stylesheets or something else...
            // Argument for stylesheet is that they can change lots of the text... 
            // Stylesheets have it I believe... 
        }

        //referring to the existing customer class wasn't working
        public class CustomerLite 
        {  
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Title { get; set; }
            public byte creditblocked { get; set; }
        }
       
        public CustomerLite CustomerLight = new CustomerLite();

        public class Parameters
        {
            public class Load
            {
                public Load()
                {
                    AcctNo  = "";
                    AcctGet = true;
                }

                public short SettledMonthsToConsider { get; set; }
                public int SettledArrearsMonthsToCheck { get; set; }
                public bool MonthsHistoryGet { get; set; }
                public bool MaxCustArrearsGet { get; set; }
                public bool InstantCreditFlagGet { get; set; }
                public string AcctNo { get; set; }
                public bool AcctGet { get; set; }
                public bool AgreementGet { get; set; }
                public bool FinTransGet { get; set; }
                public bool InstalPlanGet { get; set; }
                public bool StoreCardGet { get; set; }
                public bool ProposalGet { get; set; }
                public bool ProposalFlagsGet { get; set; }
                //public bool AcctCodeGet { get; set; }
                //public bool FinTransLiteGet { get; set; }
                public bool StoreCardPayDetailsGet { get; set; }
                public bool CustAcctHolderGet { get; set; }
                public bool CustomerGet { get; set; }
                public bool HomeAddressGet { get; set; }
            }
        }
    }

    public class SaveNewAccountCreditParameters
    {
        public string AccountNumber { get; set; }
        public string NewAccountNumber { get; set; }
        public string CustomerID { get; set; }
        public string CheckType { get; set; }
        public DateTime DateProp { get; set; }
        public string PropResult { get; set; }
        public int User { get; set; }
        public string AccountType { get; set; }
        public bool IsLoan { get; set; }
        public bool SPAPrint { get; set; }
        public decimal AgreementTotal { get; set; }
        public decimal Deposit { get; set; }
        public string TermsType { get; set; }
        public string Approved { get; set; }
        public DateTime DateAccountOpened { get; set; }
    }
     
    public partial class fintranswithBalancesVW  
    {
        public double dayswithBalance
        { get;           set;}
        public double BalanceExUnpaidInterest
        { get; set; }
            
    }
}