using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unicomer.Cosacs.Model
{
    public class TransactionModel
    {
    }
    public class ValidateUserAccountsModel
    {
        public UserAccountsModel _UserAccountsModel { get; set; }
        public string Message { get; set; }
    }

    public class UserAccountsModel
    {
        public string extUId { get; set; }
        public string totalCreditDue { get; set; }
        public string totalCreditLine { get; set; }
        public string monthlyDue { get; set; }
        public string amountDueNow { get; set; }
        public string monthlyPaymentCapacity { get; set; }
        public DueDates[] dueDates { get; set; }
    }

    public class DueDates
    {
        public Int64 date { get; set; }
        public string invoiceNumber { get; set; }
        public decimal amount { get; set; }
        public decimal totalRemainingAmount { get; set; }
        public string status { get; set; }
        public int days { get; set; }
    }

    //public class ListUserAccounts
    //{
    //    public string CustId { get; set; }
    //    //public string outstandingBalance { get; set; }
    //    public string TotalCreditDue { get; set; }
    //    public string DueDate { get; set; }
    //    public string CreditLimit { get; set; }
    //    public string CreditAvailable { get; set; }
    //    public string MonthlyDue { get; set; }
    //    public string AmountDueNow { get; set; }

    //}

    public class DueDateList
    {

        public DateTime DueDate { get; set; }

    }

    public class CreditApp
    {
        public string accountNumber { get; set; }
        //public string MessageTitle { get; set; }
        //public string Message { get; set; }
        //public string CredtAppStatus { get; set; }
        //public string CustName { get; set; }
        //public string CustID { get; set; }
        public MailTemplate MailContent { get; set; }
    }

    public class CreditAnswerModel
    {
        /// <summary>
        /// Customer Id
        /// </summary>
        public string CustId { get; set; }
        public DateTime Q1 { get; set; }//DOB
        public string Q2 { get; set; }//Title
        public string Q3 { get; set; }//Address type//Cancel
        public string Q4 { get; set; }//Address 1
        public string Q5 { get; set; }//Address 2
        public string Q6 { get; set; }//Address 3
        public string Q7 { get; set; }//Delivery Area
        public DateTime Q8 { get; set; }//DateIn
        public char Q9 { get; set; }//MaritalStatus
        public int Q10 { get; set; }//Dependants
        public string Q11 { get; set; }// Nationality
        public string Q12 { get; set; }//Occupation
        public char Q13 { get; set; }//PayFrequency
        public string Q14 { get; set; }//TelephoneNumber
        public DateTime Q15 { get; set; }//CurrentEmploymentStartDdate
        public DateTime Q16 { get; set; }//PrevEmploymentStartDate
        public char Q17 { get; set; }//EmployeeStatus
        public DateTime Q18 { get; set; }//DateInPrevAddress
        public decimal Q19 { get; set; }//NetIncome//Total Net Income
        public decimal Q20 { get; set; }//AdditionalIncome
        public decimal Q21 { get; set; }//Utilities
        public decimal Q22 { get; set; }//AdditionalExpenditure1
        public decimal Q23 { get; set; }//AdditionalExpenditure2
        public string Q24 { get; set; }//BankDetail
        public DateTime Q25 { get; set; }//BankAccountOpenDate
        public string Q26 { get; set; }//BankAccountType
        public decimal Q27 { get; set; }//MiscellaneousExpense
        public string Q28 { get; set; }//Reference1_FirstName
        public string Q29 { get; set; }//Reference1_LastName
        public string Q30 { get; set; }//Reference1_Relationship
        public string Q31 { get; set; }//Reference1_HomeAddress
        public string Q32 { get; set; }//Reference1_HomeTelephone
        public DateTime Q33 { get; set; }//Reference1_DateChecked
        public string Q34 { get; set; }//Reference1_Comments
        public string Q35 { get; set; }//Reference2_FirstName
        public string Q36 { get; set; }//Reference2_LastName
        public string Q37 { get; set; }//Reference2_Relationship
        public string Q38 { get; set; }//Reference2_HomeAddress
        public string Q39 { get; set; }//Reference2_HomeTelephone
        public string Q40 { get; set; }//Reference2_DateChecked
        public string Q41 { get; set; }//Reference2_Comments
        public byte[] Q42 { get; set; }//Document_IdProof
        public byte[] Q43 { get; set; }//Document_AddressProof
        public byte[] Q44 { get; set; }//Document_IncomeProof

        public string Q45 { get; set; }// Not available
        public string Q46 { get; set; }//Sex/Customer--Done
        public string Q47 { get; set; }//Employer name/Proposal--Stage 2--Done
        public string Q48 { get; set; }//add type='W'/Cust Address-- Done
        public string Q49 { get; set; }//resstatus/Custaddress.--Done
        public int Q50 { get; set; }//yrs current addr/Proposal --Done
        public string Q51 { get; set; }//Comment3/ Total Living Expense--Done
        public string Q52 { get; set; }//Comment2/Total Loan Expense--Done
        public string Q53 { get; set; }//FileType_IdProof
        public string Q54 { get; set; }//FileType_AddressProof
        public string Q55 { get; set; }//FileType_IncomeProof
        public byte[] Q56 { get; set; }//Id Proof back side image
        public string Q57 { get; set; }//File Type Id Proof back side image
        public string Q58 { get; set; }//CCardNo1 - proposal
        public decimal Q59 { get; set; }//RF CreditLimit
        public DateTime Q60 { get; set; }//TP yrs current addr/Proposal --Done

    }

    public class CountryMaintenanceModel
    {
        public string Message { get; set; }
        public string StatusCode { get; set; }
        public DataSet DtCountryMaintenance { get; set; }
    }

    public class GetContract
    {
        public string CustId { get; set; }
        //public string ExtCustId { get; set; }
        public string accountNumber { get; set; }
        public int dayOfMonth { get; set; }
        public frequency frecuency { get; set; }
    }

    public class GetTPContractAccount
    {
        public string accountNumber { get; set; }

    }

    public enum frequency
    {
        ONCE = 1,
        TWICE = 2
    }

    public class ContractResult
    {
        public byte[] contract { get; set; }
        public Signatures[] signatures { get; set; }
    }

    public class Signatures
    {
        public string title { get; set; }
        public string id { get; set; }
    }

    public class ContractDetails
    {
        public CustomerContractDetails CustomerContractDetail { get; set; }
        public string Message { get; set; }
    }

    public class CustomerContractDetails
    {
        public string Name { get; set; }
        public string YPUserID { get; set; }
        public string CustID { get; set; }
        public string AccountNumber { get; set; }
        public string Address { get; set; }
        public string PropDate { get; set; }
        public string PropMonth { get; set; }
        public string PropYear { get; set; }
        public string PurchaseAmount { get; set; }
        public string InstallmentAmount { get; set; }
        public string DueDay { get; set; }
    }

    public class CustomerCreditSummaryDetails
    {
        public string extUId { get; set; }
        public string customerName { get; set; }
        public List<Int64> dueDate { get; set; }
        public string totalOutstandingAmount { get; set; }
        public string monthlyOutstandingAmount { get; set; }
        public string creditAvailable { get; set; }

    }

    public class CustomerCreditSummary
    {
        public CustomerCreditSummaryDetails _CustomerCreditSummary { get; set; }
        public string Message { get; set; }
    }

    public class Answer
    {
        public string answer { get; set; }
        public string filetype { get; set; }
        public string questionId { get; set; }
    }

    public class AnswerModel
    {
        public string custId { get; set; }
        public List<Answer> answers { get; set; }
    }

    public class CreditDcoumentStatus
    {
        public string custId { get; set; }
        public string accountNumber { get; set; }
        public string status { get; set; }
        public string email { get; set; }
    }
    public class CDcoumentStatus
    {
        public CreditDcoumentStatus _CreditDcoumentStatus { get; set; }
        //public string Message { get; set; }
    }

    public class MailTemplate
    {
        public string To { get; set; }
        public string Cc { get; set; }
        public string Subject { get; set; }
        public string body { get; set; }

    }

    public class TPTransactionConfirm
    {
        [Required]
        public decimal loanAmount { get; set; }
        [Required]
        public int numberOfInstallments { get; set; }
        [Required]
        public string storeId { get; set; }
    }

    public class UpdateTransactionBody
    {

        public string status { get; set; }
        public string reason { get; set; }
        public string userPortalId { get; set; }
        public string storeId { get; set; }
    }

    public class UpdateTransactionQueryString
    {
        public string acctNo { get; set; }
        public string custId { get; set; }


    }

    public class UpdateTransactionResult
    {
        public bool success { get; set; }
        public string messages { get; set; }
        public MailTemplate MailContent { get; set; }
    }
    public class CutomerContract
    {
        public string Custid { get; set; }
        public string AccountNumber { get; set; }
        public string EmailId { get; set; }
        public string FilePath { get; set; }
        public string MailBody { get; set; }
        public string MailSubject { get; set; }
    }

    public class CreditInformation
    {
        public string CustId { get; set; }
        public string accountNumber { get; set; }
        public int dayOfMonth { get; set; }
    }

    public class ContractNotificationStatus
    {
        public List<MailNotificationstatusList> MailNotificationstatus { get; set; }
    }

    public class MailNotificationstatusList
    {
        public string Custid { get; set; }
        public string AccountNumber { get; set; }
        public string status { get; set; }
    }

}
