using STL.Common;
using STL.Common.Constants.ColumnNames;
using STL.Common.Constants.TableNames;
using STL.Common.Static;
using STL.PL.WS5;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Web.Services.Protocols;
using System.Windows.Forms;
using System.Xml;
using System.Data.SqlClient;
using AxSHDocVw;
using STL.Common.Printing.AgreementPrinting;
using System.Web.UI.WebControls;

namespace STL.PL
{
    public partial class ReprintInvoice : CommonForm
    {
        //private DataTable _cancelReasons;
        private string error = "";
        private int searchClicked = 0;
        private DataSet accounts;
        private int BranchNo = 0;
        private DateTime InvoiceDateFrom = DateTime.Today;
        private DateTime InvoiceDateTo = DateTime.Today;
        private string InvoiceNo = String.Empty;
        private string accountNo = String.Empty;
        private new string Error = String.Empty;
        private string accountType = String.Empty;
        private DataView acctView;
        private Crownwood.Magic.Menus.MenuCommand menuFile;
        //public XmlDocument itemDoc;
        private string _custid = "";
        private bool _cashAndGo;
        int PageSize = 5;


        //public XmlNode LineItems
        //{
        //    get { return itemDoc.DocumentElement; }
        //}
        AxWebBrowser browser = new AxWebBrowser();
        public XmlDocument itemDoc;
        private bool _paidAndTaken = false;
        public bool PaidAndTaken
        {
            get { return _paidAndTaken; }
            set
            {
                _paidAndTaken = value;
            }
        }
        public XmlNode LineItems
        {
            get { return itemDoc.DocumentElement; }
        }

        public string CustomerID
        {
            get { return _custid; }
            set
            {
                //uat117
                if (_custid == String.Empty && value == "PAID & TAKEN")
                {
                    _cashAndGo = true;
                }
                _custid = value;
            }
        }

        public ReprintInvoice(Form root, Form parent) //Changes For Hide Show
        {
            InitializeComponent();
            showsubmenu( root,  parent);
            LoadStaticData();
        }
        public void showsubmenu(Form root, Form parent)
        {
            try
            {
                menuMain = new Crownwood.Magic.Menus.MenuControl();
                menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] { menuFile});
                FormRoot = root;
                FormParent = parent;
                ApplyRoleRestrictions();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        private void LoadStaticData()
        {
            try
            {
                XmlUtilities xml = new XmlUtilities();
                XmlDocument dropDowns = new XmlDocument();
                dropDowns.LoadXml("<DROP_DOWNS></DROP_DOWNS>");
                StringCollection branchNos = new StringCollection();
                if (StaticData.Tables[TN.BranchNumber] == null)
                    dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.BranchNumber, null));
                if (dropDowns.DocumentElement.ChildNodes.Count > 0)
                {
                    DataSet ds = StaticDataManager.GetDropDownData(dropDowns.DocumentElement, out error);
                    if (error.Length > 0)
                        ShowError(error);
                    else
                    {
                        foreach (DataTable dt in ds.Tables)
                        {
                            StaticData.Tables[dt.TableName] = dt;
                        }
                    }
                }
                //Now customise the dropdowns.. 
                foreach (DataRow row in ((DataTable)StaticData.Tables[TN.BranchNumber]).Rows)
                {
                    branchNos.Add(Convert.ToString(row["branchno"]));
                }
                drpBranchNo.DataSource = branchNos;
                drpBranchNo.Text = Config.BranchCode;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        private void btnSearch_Click(object sender, EventArgs e)
        {
            Function = "btnSearch_Click";
            try
            {
                Wait();
                if (searchClicked > 0 && accounts != null)
                {
                    accounts.Clear();
                    accounts.Dispose();
                }

                Search();
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
            finally
            {
                StopWait();
            }
        }
        private void Search()
        {
            try
            {
                BranchNo = Convert.ToInt16(drpBranchNo.SelectedValue);
                InvoiceDateFrom = Convert.ToDateTime(dtDateFrom.Text.Trim());
                InvoiceDateTo = Convert.ToDateTime(dtDateTo.Text.Trim());
                //Pass Null if no value is passed for InvoiceNo
                if (txtOrdInvoiceNo.Text.Trim() == string.Empty)
                {
                    InvoiceNo = null;
                }
                else
                {
                    InvoiceNo = txtOrdInvoiceNo.Text.Trim().Replace("-", String.Empty); //Changes For Replace -
                    InvoiceNo = InvoiceNo.Substring(0, 14);  //Changes For Replace -
                }
                //Pass Null if no value is passed for Accountno
                accountNo = txtAccountNumber.Text.Replace("-", String.Empty);
                if (accountNo == "000000000000")
                {
                    accountNo = null;
                }
                else
                {
                    accountNo = txtAccountNumber.Text.Replace("-", String.Empty);
                }
                BindData();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void BindData()
        {
            try
            {
                accounts = AccountManager.InvoiceAccountsSearch(BranchNo, InvoiceDateFrom, InvoiceDateTo, InvoiceNo, accountNo);
                var dt = accounts.Tables[TN.Accounts];
                acctView = new DataView(dt);
                if (acctView.Count > 0)
                {
                    dgAccounts.DataSource = acctView;
                    dgAccounts.Columns[0].Width = 60;
                    dgAccounts.Columns[1].Width = 90;
                    dgAccounts.Columns[2].Width = 100;
                    dgAccounts.Columns[3].Width = 130;
                    dgAccounts.Columns[4].Width = 130;
                    //dgAccounts.Columns[5].Width = 200;
                    dgAccounts.Columns[1].ReadOnly = true;
                    dgAccounts.Columns[2].ReadOnly = true;
                    dgAccounts.Columns[3].ReadOnly = true;
                    dgAccounts.Columns[4].ReadOnly = true;
                    dgAccounts.Columns[5].ReadOnly = true;
                    dgAccounts.Columns[6].ReadOnly = true;


                    this.dgAccounts.Columns["AgreementNo"].Visible = false;
                    this.dgAccounts.Columns["AccountType"].Visible = false;
                    this.dgAccounts.Columns["Customer Id"].Visible = false;
                    chkTick.Visible = true;
                }
                else
                {
                    MessageBox.Show("No Record Found For this search !!");
                    dgAccounts.DataSource = null;
                }

                if (Error.Length > 0)
                    ShowError(Error);
            }
            catch (SoapException ex)
            {
                Catch(ex, Function);
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
            finally
            {



            }
        }

        private void chkSelectall_CheckedChanged(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dgAccounts.Rows)
            {
                DataGridViewCheckBoxCell chk = (DataGridViewCheckBoxCell)row.Cells[0];
                if (chkSelectall.Checked == true)
                {
                    chk.Value = true;
                }
                else
                {
                    chk.Value = false;
                }
                //chk.Value = !(chk.Value == null ? false : (bool)chk.Value); //because chk.Value is initialy null
            }



        }
        private void btnClear_Click(object sender, EventArgs e)
        {
            try
            {
                //drpBranchNo.SelectedIndex = 0;
                drpBranchNo.Text = Config.BranchCode;
                dtDateFrom.Value = DateTime.Today;
                dtDateTo.Value = DateTime.Today;
                txtOrdInvoiceNo.Text = " ";
                txtAccountNumber.Text = "000-0000-0000-0";
                chkSelectall.Checked = false;
                dgAccounts.DataSource = null;
                ((MainForm)FormRoot).browsers = ((CommonForm)FormRoot).CreateBrowserArray(0);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.CloseTab();
            ((MainForm)FormRoot).browsers = ((CommonForm)FormRoot).CreateBrowserArray(0);
        }

        private void txtOrdInvoiceNo_TextChanged(object sender, EventArgs e)
        {
            //if (System.Text.RegularExpressions.Regex.IsMatch(txtOrdInvoiceNo.Text, "[^0-9]"))
            //{
            //    txtOrdInvoiceNo.Text = String.Empty;
            //    MessageBox.Show("Please enter only numbers.");

            //}

        }
        public static bool IsAllDigits(string s)
        {
            if (s == "000-00000000000")
            {
                s = s.Replace("-", "");
            }
            foreach (char c in s)
            {
                if (!Char.IsDigit(c))
                    return false;
            }
            return true;
        }

        private void ReprintInvoice_Load(object sender, EventArgs e)
        {
            dtDateFrom.Value = DateTime.Today;
            dtDateTo.Value = DateTime.Today;
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                WS2.WAccountManager wAccountManager = new WS2.WAccountManager();
                int noPrints = 0;
                XmlNode lineItems;
                AxWebBrowser[] browsers = CreateBrowserArray(10);
                int branchno;
                String invoiceno;
                String acctno;
                String customerid;
                string accountType;
                int agreementNo;
                //int buffno = 1;
                //bool paidAndTaken = false;
                string agreementinvoicenumber = string.Empty;
                bool collection = false;
                //bool creditNote = true;
                bool multiple = false;
                bool ReprintInvoice = true;
                int i = 0;
                int invVersion = 0;
                //int noBrowsers = 4;
                foreach (DataGridViewRow row in dgAccounts.Rows)
                {
                    if (Convert.ToBoolean(row.Cells[0].Value) == true)
                    {
                        browser.TabIndex = noPrints++;
                        branchno = Convert.ToInt16(row.Cells[1].Value);
                        invoiceno = Convert.ToString(row.Cells[3].Value);
                        acctno = Convert.ToString(row.Cells[4].Value);
                        agreementNo = Convert.ToInt32(row.Cells[7].Value);
                        accountType = Convert.ToString(row.Cells[8].Value);
                        customerid = Convert.ToString(row.Cells[9].Value);
                        if (invoiceno.Length > 0)
                        {
                            if (invoiceno.Contains("-"))
                            {
                                agreementinvoicenumber = invoiceno.Remove(invoiceno.IndexOf("-"));
                                invVersion = Convert.ToInt32(invoiceno.Substring(invoiceno.LastIndexOf('-') + 1));
                            }
                            else
                            {
                                agreementinvoicenumber = invoiceno;
                                invVersion = 1;
                            }
                        }
                        //To Give option to Print only 20 Invoice once
                        if (i < 10)
                        {
                            if (agreementNo == 1)
                            {
                                lineItems = AccountManager.GetLineItemsWithVersion(acctno, agreementNo, accountType, Config.CountryCode, Convert.ToInt16(Config.BranchCode), invVersion, out Error);
                            }
                            else
                            {
                                lineItems = AccountManager.GetSalesOrderLineItems(acctno, agreementNo, accountType, Config.CountryCode, Convert.ToInt16(Config.BranchCode), agreementinvoicenumber, out Error);
                            }
                            //LaserPrintTaxInvoice(browsers[i], acctno, agreementNo, accountType, customerid, PaidAndTaken, collection, lineItems, buffno, creditNote, multiple, invVersion, ReprintInvoice = true);
                            if (lineItems != null)  //#19620
                            {
                                if (agreementNo == 1)
                                {
                                    PaidAndTaken = false;
                                }
                                else
                                {
                                    PaidAndTaken = true;
                                }

                                bool taxExempt = AccountManager.IsTaxExempt(acctno, agreementNo.ToString(), out Error);
                                NewPrintTaxInvoice(acctno, agreementNo, accountType, customerid,
                                PaidAndTaken, collection, null, 0, 0,
                                lineItems, 0, browsers[i], ref noPrints, false, multiple, 0, 0, null, taxExempt, "", "", 0, invVersion, ReprintInvoice);
                            }
                        }
                        else
                        {
                            MessageBox.Show("You can print only 10 Invoices.");
                            return;
                        }
                        Thread.Sleep(1000);
                        i++;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //#region Laser Printer        
        //public void LaserPrintTaxInvoice(AxSHDocVw.AxWebBrowser b,
        //                                    string accountNo,
        //                                    int agreementNo,
        //                                    string accountType,
        //                                    string customerID,
        //                                    bool paidAndTaken,
        //                                    bool collection,
        //                                    XmlNode lineItems,
        //                                    int buffNo,
        //                                    bool creditNote,
        //                                    bool multiple,
        //                                    int versionNo, bool ReprintInvoice = true)

        //{

        //    object Zero = 0;
        //    object EmptyString = "";
        //    string url = "";

        //    /* make sure ampersands are properly encoded specifically
        //     * to cater for the PAID & TAKEN customer */
        //    customerID = customerID.Replace("&", "%26");

        //    string queryString = "customerID=" + customerID + "&" +
        //        "acctNo=" + accountNo + "&" +
        //        "accountType=" + accountType + "&" +
        //        "culture=" + Config.Culture + "&" +
        //        "country=" + Config.CountryCode + "&" +
        //        "branch=" + Config.BranchCode + "&" +
        //        "buffno=" + buffNo.ToString() + "&" +
        //         "creditNote=" + creditNote.ToString() + "&" +
        //         "multiple=" + multiple.ToString() + "&" +
        //         "versionNo=" + versionNo.ToString() + "&" +
        //         "ReprintInvoice=" + ReprintInvoice + "&" +
        //        "user=" + Credential.UserId.ToString() + "&" +
        //        "IsProofofPurchase=false";

        //    /* if this is the paid and taken account then we must pass the 
        //     * lineItems back which means we need to use HTTP POST because 
        //     * of the limit on querystring length */
        //    if (agreementNo != 1)
        //    {
        //        /* various characters in the xml will be automatically escaped
        //         * e.g. " becomes &quot; This leaves ampersands in the querystring 
        //         * which must be encoded or they will cause problems */
        //        string xml = lineItems.OuterXml;
        //        xml = xml.Replace("&", "%26");
        //        queryString += "&collection=" + collection.ToString();
        //        queryString += "&lineItems=" + xml;
        //        url = Config.Url + "WPaidAndTakenTaxInvoice.aspx";
        //        object postData = EncodePostData(queryString);
        //        object headers = PostHeader;
        //        b.Navigate(url, ref Zero, ref EmptyString, ref postData, ref headers);
        //    }
        //    else
        //    {
        //        url = Config.Url + "WTaxInvoice.aspx?" + queryString;
        //        b.Navigate(url, ref Zero, ref EmptyString, ref EmptyString, ref EmptyString);
        //    }
        //    // Audit the Tax Invoice print if it is a re-print
        //    AccountManager.AuditReprint(accountNo, agreementNo, DocumentType.TaxInvoice);
        //}

        //#endregion

        private void menuFile_Click(object sender, EventArgs e)
        {

        }
        private void menuExit_Click(object sender, EventArgs e)
        {
            this.CloseTab();
        }

        private void txtOrdInvoiceNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsLetter(e.KeyChar)) && !(char.IsNumber(e.KeyChar)) && !(char.IsControl(e.KeyChar)) && !(char.IsWhiteSpace(e.KeyChar)))
            {
                e.Handled = true;
            }
        }

        private void txtOrdInvoiceNo_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void ReprintInvoice_Leave(object sender, EventArgs e)
        {
            ((MainForm)FormRoot).browsers = ((CommonForm)FormRoot).CreateBrowserArray(0);
        }

      
    }
}
