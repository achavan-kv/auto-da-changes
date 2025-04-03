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
    }

    public class CustomerCreditSummaryDetails
    {
        public string extUId { get; set; }
        public string customerName { get; set; }
        public string[] dueDate { get; set; }
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
        public string id { get; set; }
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

    public class CreateCashaccount
    {
        public string accountNumber { get; set; }
    }

    public class CreateRFaccount
    {
        public string accountNumber { get; set; }
    }

    public class Commissions
    {
        public string ResourceType { get; set; }
        public string Source { get; set; }
        public DateTime Date { get; set; }
        public string acctno { get; set; }
        public string checkoutId { get; set; }
        public List<CommissionsList> CommissionsList { get; set; }
    }

    public class CommissionsList
    {
        public string OrderId { get; set; }
        public string EmployeeNo { get; set; }
        public string ItemText { get; set; }
        public string CommissionType { get; set; }
        public string ItemNo { get; set; }
        public int StockLocn { get; set; }
        public decimal NetCommissionValue { get; set; }
        public decimal CommissionPercent { get; set; }
        public decimal CommissionAmount { get; set; }
    }

}
