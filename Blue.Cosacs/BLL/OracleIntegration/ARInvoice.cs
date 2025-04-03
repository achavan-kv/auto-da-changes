using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Data.SqlClient;
using System.Data;
using STL.DAL;
using STL.Common;

/// <summary>
/// This is Oracle Sales Invoice Interface
/// </summary>
///
namespace STL.BLL.OracleIntegration
{
    public class ARInvoice
    {
        public ARInvoiceHeader InvoiceHeader;

        public ARInvoiceLine[] InvoiceLine;
        //public ArrayList InvoiceLine;


        public static ARInvoice[] Populate()
        {
            ArrayList invoices = new ArrayList();

            DataSet Invoicedata = new DataSet();
            // Get Invoices
            DOracleIntegration Oi = new DOracleIntegration();
            Invoicedata = Oi.GetInvoicedata();

            string prevAccount ="";
            string prevInvoice = "";

            ARInvoice CurrInv = new ARInvoice(); 
            ArrayList CurrInvoiceLine=null;
            foreach (DataRow data in Invoicedata.Tables[0].Rows)
            {
                                
                // check for same account/invoice
                if (prevAccount != data["acctno"].ToString() ||
                        prevInvoice != data["invoicereference"].ToString())
                {
                    if (prevAccount != string.Empty)
                        CurrInv.InvoiceLine = (ARInvoiceLine[])CurrInvoiceLine.ToArray(typeof(ARInvoiceLine));

                    CurrInvoiceLine  = new ArrayList();
                    CurrInv = new ARInvoice();
                    CurrInv.InvoiceHeader = new ARInvoiceHeader();

                    prevAccount = data["acctno"].ToString();
                    prevInvoice = data["invoicereference"].ToString();

                    CurrInv.InvoiceHeader.TranType = data["TranType"].ToString();
                    CurrInv.InvoiceHeader.TranClass = data["TranClass"].ToString();
                    CurrInv.InvoiceHeader.TranDate = Convert.ToDateTime(data["TranDate"].ToString());
                    CurrInv.InvoiceHeader.GLDate = Convert.ToDateTime(data["GLDate"].ToString());
                    CurrInv.InvoiceHeader.DelDate = Convert.ToDateTime(data["DelDate"].ToString());     // jec 29/10/08
                    CurrInv.InvoiceHeader.empeenosale = Convert.ToInt32(data["empeenosale"].ToString());
                    CurrInv.InvoiceHeader.invoicereference = data["invoicereference"].ToString();
                    CurrInv.InvoiceHeader.CredInvRef = data["CredInvRef"].ToString();
                    CurrInv.InvoiceHeader.PayTerm = Convert.ToInt32(data["PayTerm"].ToString());
                    CurrInv.InvoiceHeader.SalesPerson = data["SalesPerson"].ToString();
                    CurrInv.InvoiceHeader.CustomerId = data["CustomerId"].ToString();
                    CurrInv.InvoiceHeader.AcctNo = data["acctno"].ToString();
                    CurrInv.InvoiceHeader.BillAdrRef = Convert.ToInt32(data["BillAdrRef"].ToString());
                    CurrInv.InvoiceHeader.ShipAdrRef = Convert.ToInt32(data["ShipAdrRef"].ToString());
                    CurrInv.InvoiceHeader.BranchNo = Convert.ToInt32(data["BranchNo"].ToString());
                    CurrInv.InvoiceHeader.RunNo = Convert.ToInt32(data["RunNo"].ToString());
                    //CurrInv.InvoiceLine = new ArrayList();
                    // Add Invoice
                    invoices.Add(CurrInv);

                }

                // Line Details
                               
                ARInvoiceLine InvLine = new ARInvoiceLine();
                
                //Populate Invoice line
                InvLine.itemno = data["itemno"].ToString();
                InvLine.lineDescription = data["lineDescription"].ToString();
                InvLine.UOM = data["UOM"].ToString();
                InvLine.Quantity = Convert.ToDouble(data["Quantity"].ToString());   //70646 
                InvLine.UnitPrice = Convert.ToDecimal(data["UnitPrice"].ToString());
                InvLine.LineValue = Convert.ToDecimal(data["LineValue"].ToString());
                InvLine.TaxFlag = data["TaxFlag"].ToString();
                InvLine.TaxCode = data["TaxCode"].ToString();
                InvLine.TaxRate = Convert.ToDecimal(data["TaxRate"].ToString());
                InvLine.RetItemNo = data["RetItemNo"].ToString();   // jec 29/10/08
                InvLine.LineRef = Convert.ToInt32(data["LineRef"].ToString());
                InvLine.AccountCode = data["AccountCode"].ToString();

                // Add line to invoice
                CurrInvoiceLine.Add(InvLine);                

            }
            // Add last invoice line
            if (prevAccount != string.Empty)
                CurrInv.InvoiceLine = (ARInvoiceLine[])CurrInvoiceLine.ToArray(typeof(ARInvoiceLine));

            return (ARInvoice[]) invoices.ToArray(typeof(ARInvoice));

        }


    }
}

