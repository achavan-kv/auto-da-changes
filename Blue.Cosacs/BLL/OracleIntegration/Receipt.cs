using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Data.SqlClient;
using System.Data;
using STL.DAL;
using STL.Common;

/// <summary>
/// This is Oracle Receipt Interface
/// </summary>
///
namespace STL.BLL.OracleIntegration
{
    public class Receipt
    {
        public string ReceiptNo = " ";
        public DateTime ReceiptDate;
        public string CurrencyCode = " ";
        public string Custid = " ";
        public string Acctno = " ";
        public decimal ReceiptAmount;
        public string InvoiceReference = " ";
        public decimal AppliedAmount;
        public string CosacsUser = " ";
        public string PayMethod = " ";
        public string TranType = " ";
        public string Chq_CredCard = " ";
        public string Bankname = " ";
        public int RunNo;
        public string OracleReceiptNo = " ";        // 70514 jec



        public static Receipt[] Populate()
        {
            ArrayList receipts = new ArrayList();

            DataSet Receiptdata = new DataSet();
            // Get Receipts
            DOracleIntegration Oi = new DOracleIntegration();
            Receiptdata = Oi.GetReceiptdata();

            Receipt CurrReceipt = new Receipt();

            foreach (DataRow data in Receiptdata.Tables[0].Rows)
            {
                CurrReceipt = new Receipt();

                CurrReceipt.ReceiptNo = data["ReceiptNo"].ToString();
                CurrReceipt.ReceiptDate = Convert.ToDateTime(data["ReceiptDate"].ToString());
                CurrReceipt.CurrencyCode = data["CurrencyCode"].ToString();
                CurrReceipt.Custid = data["Custid"].ToString();
                CurrReceipt.Acctno = data["Acctno"].ToString();
                CurrReceipt.ReceiptAmount = Convert.ToDecimal(data["ReceiptAmount"].ToString());
                CurrReceipt.InvoiceReference = data["InvoiceReference"].ToString();
                CurrReceipt.AppliedAmount = Convert.ToDecimal(data["AppliedAmount"].ToString());
                CurrReceipt.CosacsUser = data["CosacsUser"].ToString();
                CurrReceipt.PayMethod = data["PayMethod"].ToString();
                CurrReceipt.TranType = data["TranType"].ToString();
                CurrReceipt.Chq_CredCard = data["Chq_CredCard"].ToString();
                CurrReceipt.Bankname = data["Bankname"].ToString();
                CurrReceipt.RunNo = Convert.ToInt32(data["RunNo"].ToString());
                CurrReceipt.OracleReceiptNo = data["OracleReceiptNo"].ToString();

                // Add Receipt
                receipts.Add(CurrReceipt);

            }

            return (Receipt[]) receipts.ToArray(typeof(Receipt));
        }
    }
}

