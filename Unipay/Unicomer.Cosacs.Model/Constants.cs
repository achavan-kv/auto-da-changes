using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unicomer.Cosacs.Model
{
    public class Constants
    {
        public const string CollectionNotNulllorEmptyMessage = "{PropertyName} field cannot be null or empty.";
        public const string CollectionIndexNotNulllorEmptyMessage = "{PropertyName} {CollectionIndex} is required.";
    }

    public struct TestResult
    {
        public const string Pass = "P";
        public const string Fail = "F";
        public const string Warning = "W";
    }

    public struct CreditApproval
    {
        //public string CustId { get; set; }
        public const string Q1 = "1001";//DOB
        //public const string Q2 = "1002";//Title
        //public const string Q3 = "1003";//Address type//Cancel
        public const string Q4 = "1004";//Address 1
        //public const string Q5 = "1005";//Address 2
        //public const string Q6 = "1006";//Address 3
        //public const string Q7 = "1007";//Delivery Area
        //public const string Q8 = "1008";//DateIn
        public const string Q9 = "1009";//MaritalStatus
        public const string Q10 = "1010";//Dependants
        public const string Q11 = "1011";// Nationality
        public const string Q12 = "1012";//Occupation
        public const string Q13 = "1013";//PayFrequency
        public const string Q14 = "1014";//TelephoneNumber
        public const string Q15 = "1015";//CurrentEmploymentStartDdate
        //public const string Q16 = "1016";//PrevEmploymentStartDate
        //public const string Q17 = "1017";//EmployeeStatus
        //public const string Q18 = "1018";//DateInPrevAddress
        public const string Q19 = "1019";//NetIncome//Total Net Income
        //public const string Q20 = "1020";//AdditionalIncome
        //public const string Q21 = "1021";//Utilities
        //public const string Q22 = "1022";//AdditionalExpenditure1
        //public const string Q23 = "1023";//AdditionalExpenditure2
        //public const string Q24 = "1024";//BankDetail
        //public const string Q25 = "1025";//BankAccountOpenDate
        //public const string Q26 = "1026";//BankAccountType
        //public const string Q27 = "1027";//MiscellaneousExpense
        public const string Q28 = "1028";//Reference1_FirstName
        //public const string Q29 = "1029";//Reference1_LastName
        //public const string Q30 = "1030";//Reference1_Relationship
        //public const string Q31 = "1031";//Reference1_HomeAddress
        //public const string Q32 = "1032";//Reference1_HomeTelephone
        //public const string Q33 = "1033";//Reference1_DateChecked
        //public const string Q34 = "1034";//Reference1_Comments
        //public const string Q35 = "1035";//Reference2_FirstName
        //public const string Q36 = "1036";//Reference2_LastName
        //public const string Q37 = "1037";//Reference2_Relationship
        //public const string Q38 = "1038";//Reference2_HomeAddress
        //public const string Q39 = "1039";//Reference2_HomeTelephone
        //public const string Q40 = "1040";//Reference2_DateChecked
        //public const string Q41 = "1041";//Reference2_Comments
        public const string Q42 = "1042";//Document_IdProof
        public const string Q43 = "1043";//Document_AddressProof
        public const string Q44 = "1044";//Document_IncomeProof
        //public const string Q45 = "1045";// Not available
        public const string Q46 = "1046";//Sex/Customer--Done
        public const string Q47 = "1047";//Employer name/Proposal--Stage 2--Done
        public const string Q48 = "1048";//add type='W'/Cust Address-- Done
        public const string Q49 = "1049";//resstatus/Custaddress.--Done
        public const string Q50 = "1050";//yrs current addr/Proposal --Done
        public const string Q51 = "1051";//Comment3/ Total Living Expense--Done
        public const string Q52 = "1052";//Comment2/Total Loan Expense--Done
        public const string Q53 = "1053";//Id Proof back side image
    }

    public struct UpdateTransactionConstants
    {
        public const string Accepted = "ACCEPTED";
        public const string Rejected = "REJECTED";
        public const string Reverted = "REVERTED";
    }
}
