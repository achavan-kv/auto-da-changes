using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using STL.BLL;
using STL.DAL;
using STL.Common;
using STL.Common.Constants.TableNames;
using System.Xml;
using STL.Common.Constants.ColumnNames;
using System.Configuration;
using System.Data.SqlClient;
using System.Threading;
using System.Globalization;
using STL.Common.Printing.AgreementPrinting;
using STL.Common.Constants.FTransaction;

namespace STL.WS
{
    public partial class WPrizeVoucher : CommonWebPage
    {
        string accountNo = "";
        int buffNo = 0;

        protected void Page_Load(object sender, EventArgs e)
        {
            SqlConnection conn = null;
          

            try
            {
                conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {

                            /* Extract deliveryNotes document from the QueryString */
                            accountNo = Request["acctNo"];
                            buffNo = Convert.ToInt32(Request["buffNo"]);
                            bool additional = Convert.ToBoolean(Convert.ToInt16(Request["additional"]));
                            bool reprint = Convert.ToBoolean(Convert.ToInt16(Request["reprint"]));
                            string culture = Request["culture"];
                            string countryCode = Request["countryCode"];

                            //Thread.CurrentThread.CurrentCulture = new CultureInfo(culture);
                            base.SetCulture();

                            DateTime dateIssued = Convert.ToDateTime(Request["dateIssued"]);

                            //Build the XML document
                            PrizeVoucherXML pxml = new PrizeVoucherXML(countryCode);
                            pxml.Load("<PRIZEVOUCHERS/>");

                            DCustomer cust = new DCustomer();

                            if (reprint)
                            {
                                decimal cashPrice = 0;
                                int voucherID = 0;
                                int numVouchers = 0;
                                cust.VoidPrizeVouchers(conn, trans, accountNo, dateIssued, buffNo,
                                                        out cashPrice, out voucherID, out numVouchers);

                                for (int i = 1; i <= numVouchers; i++)
                                {
                                    cust.SavePrizeVouchers(conn, trans, voucherID);
                                }
                            }

                            DataTable dtVouchers = cust.GetPrizeVouchers(conn, trans, accountNo, dateIssued, buffNo);

                            LoadDocument(pxml, dtVouchers);

                            cust.SetPrizeVouchersPrinted(conn, trans, accountNo, dateIssued, buffNo);

                            Response.Write(pxml.Transform());
                            trans.Commit();
                            break;
                        }
                    }
                    catch (SqlException ex)
                    {
                        CatchDeadlock(ex, conn);
                    }
                } while (retries <= maxRetries);
            }
            catch (Exception ex)
            {
                logException(ex, Function);
                Response.Write(ex.Message);
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
            }
        }

        private void LoadDocument(PrizeVoucherXML pxml, DataTable dtVouchers)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<VOUCHERS/>");

            int numVouchers = 0;

            foreach (DataRow row in dtVouchers.Rows)
            {
                numVouchers++;
                string nodeName = "VOUCHER" + numVouchers.ToString();

                XmlNode voucher = doc.CreateElement(nodeName);
                doc.DocumentElement.AppendChild(voucher);

                XmlNode acctNo = doc.CreateElement("ACCOUNTNO");
                XmlNode voucherNo = doc.CreateElement("VOUCHERNO");

                if(buffNo == 0)
                    acctNo.InnerText = accountNo;
                else
                    acctNo.InnerText = accountNo.Substring(0, 3) + "/" + buffNo.ToString();

                voucherNo.InnerText = row[CN.VoucherNo].ToString() + " : " + DateTime.Now.ToShortDateString() + " " + 
                                      DateTime.Now.ToShortTimeString();

                voucher.AppendChild(acctNo);
                voucher.AppendChild(voucherNo);

                if (numVouchers == Document.DeliveryScheduleItems)
                {
                    if (numVouchers < dtVouchers.Rows.Count)
                    {
                        XmlNode pb = doc.CreateElement("PB");
                        doc.DocumentElement.AppendChild(pb);
                    }
                    
                    pxml.ImportNode(doc.DocumentElement, true);
                    doc.LoadXml("<VOUCHERS/>");

                    numVouchers = 0;
                }
            }

            if (numVouchers > 0)
            {
                int remainder = Document.DeliveryScheduleItems - numVouchers;
                for (int i = 1; i <= remainder; i++)
                {
                    numVouchers++;
                    string nodeName = "VOUCHER" + numVouchers.ToString();

                    XmlNode voucher = doc.CreateElement(nodeName);
                    doc.DocumentElement.AppendChild(voucher);

                    XmlNode acctNo = doc.CreateElement("ACCOUNTNO");
                    XmlNode voucherNo = doc.CreateElement("VOUCHERNO");

                    acctNo.InnerText = "VOID";
                    voucherNo.InnerText = "VOID";

                    voucher.AppendChild(acctNo);
                    voucher.AppendChild(voucherNo);
                }

                pxml.ImportNode(doc.DocumentElement, true);
            }
        }
    }
}