using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blue.Cosacs.Merchandising.Models.Ashley
{
    public class SaveNewAccountParameters
    {
        public string AccountNumber { get; set; }
        public short BranchNo { get; set; }
        public string AccountType { get; set; }
        public string CODFlag { get; set; }
        public int SalesPerson { get; set; }
        public string SOA { get; set; }
        public double AgreementTotal { get; set; }
        public double Deposit { get; set; }
        public double ServiceCharge { get; set; }
        public List<LineItemList> LineItems { get; set; }
        public string TermsType { get; set; }
        public string NewBand { get; set; }
        public string CountryCode { get; set; }
        public System.DateTime DateFirst { get; set; }
        public double InstalAmount { get; set; }
        public double FinalInstalment { get; set; }
        public string PaymentMethod { get; set; }
        public int Months { get; set; }
        public bool TaxExempt { get; set; }
        public bool DutyFree { get; set; }
        public decimal TaxAmount { get; set; }
        public bool Collection { get; set; }
        public string BankCode { get; set; }
        public string BankAcctNo { get; set; }
        public string ChequeNo { get; set; }
        public short PayMethod { get; set; }
        public System.Xml.XmlNode ItemDetails { get; set; }
        public System.Xml.XmlNode ReplacementXml { get; set; }
        public decimal DtTaxAmount { get; set; }
        public string LoyaltyCardNo { get; set; }
        public bool ReScore { get; set; }
        public decimal Tendered { get; set; }
        public decimal GiftVoucherValue { get; set; }
        public bool CourtsVoucher { get; set; }
        public string VoucherReference { get; set; }
        public int VoucherAuthorisedBy { get; set; }
        public string AccountNoCompany { get; set; }
        public short PromoBranch { get; set; }
        public short PaymentHolidays { get; set; }
        public System.Data.DataTable PayMethodSet { get; set; }
        public short DueDay { get; set; }
        public int ReturnAuthorisedBy { get; set; }
        public System.Data.DataTable WarrantyRenewalSet { get; set; }
        public System.Data.DataTable VariableRatesSet { get; set; }
        public int AgreementNo { get; set; }
        public string PropResult { get; set; }
        public System.DateTime DateProp { get; set; }
        public bool ResetPropResult { get; set; }
        public bool Autoda { get; set; }
        public int User { get; set; }
        public string StoreCardAcctNo { get; set; }
        public System.Nullable<long> StoreCardNumber { get; set; }
        public bool PaidAndTaken { get; set; }
        public bool HasInstallation { get; set; }
        public bool CustLinkRequired { get; set; }
        public bool CashAndGoReturn { get; set; }
        public string CollectionType { get; set; }
        public System.Data.DataTable WarrantyRefunds { get; set; }
        public bool ReadyAssist { get; set; }
        public System.Nullable<int> ReadyAssistTermLength { get; set; }
        public Int16 PrefDay { get; set; }
    }

}
