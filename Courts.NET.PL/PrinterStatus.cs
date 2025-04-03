using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using STL.Common;
using AxSHDocVw;
using mshtml;
using Blue.Cosacs.Shared;

namespace STL.PL
{
    /// <summary>
    /// Popup prompt to allow the user to enter the printer name, type and
    /// paper source for the laser printer.
    /// </summary>
    public class PrinterStatus : CommonUserControl
    {
        private System.Windows.Forms.ImageList images;
        private System.Windows.Forms.ToolBar toolBar;
        private System.ComponentModel.IContainer components;
        private int buttonWidth = 30;

        public PrinterStatus()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();
        }

        public void Clear()
        {
            toolBar.Buttons.Clear();
            Width = 0;
        }

        public void AddButton(AxWebBrowser browser)
        {
            int index = 0;
            string webPage = browser.LocationURL;
            string locationURL = browser.LocationURL;

            int x = webPage.LastIndexOf("/") + 2;
            int y = webPage.IndexOf(".aspx");
            // x can be greater than y if a date is in the url i.e. 29/06/09
            if (x > y)
            {
                string temp = webPage.Split(new string[] { ".aspx" }, StringSplitOptions.RemoveEmptyEntries)[0];
                x = temp.LastIndexOf("/") + 2;
            }


            webPage = webPage.Substring(x, y - x);

            //If printing contract for a Free Warranty, the icon displayed on the status bar should be different
            if (webPage == "WarrantyContract")
            {
                var indexWarrantyTypeString = locationURL.IndexOf("warrantyType");
                var warrantyTypeString = locationURL.Substring(indexWarrantyTypeString, 14);
                var warrantyType = warrantyTypeString.Substring(warrantyTypeString.Length - 1, 1);

                if (warrantyType == WarrantyType.Free)
                {
                    webPage = webPage + "Free";
                } 

            }

            switch (webPage)
            {
                case "Agreement": webPage = GetResource("M_AGREEMENT");
                    index = 0;
                    break;
                case "ActionSheet": webPage = GetResource("M_ACTIONSHEET");
                    index = 6;
                    break;
                case "CollectionNote": webPage = GetResource("M_COLLECTIONNOTE");
                    index = 6;
                    break;
                case "DeliveryNote": webPage = GetResource("M_DELIVERYNOTE");
                    index = 6;
                    break;
                case "FaultyProductNote": webPage = GetResource("M_FAULTYPRODUCT");
                    index = 6;
                    break;
                case "OneForOneReplacementNote": webPage = GetResource("M_ONEFORONEREPLACEMENT");
                    index = 6;
                    break;
                case "PaidAndTakenTaxInvoice": webPage = GetResource("M_TAXINVOICE");
                    index = 4;
                    break;
                case "RFSummary": webPage = GetResource("M_RFSUMMARY");
                    index = 1;
                    break;
                case "TaxInvoice": webPage = GetResource("M_TAXINVOICE");
                    index = 4;
                    break;
                case "WarrantyContract": webPage = GetResource("M_WARRANTYCONTRACT");
                    index = 3;
                    break;
                case "WarrantyContractFree": webPage = GetResource("M_FREEWARRANTYCONTRACT");
                    index = 11;
                    break;
                case "RFTerms": webPage = GetResource("M_RFTERMS");
                    index = 2;
                    break;
                case "CashierTotals": webPage = GetResource("M_CASHIERTOTALS");
                    index = 6;
                    break;
                case "ScheduleOfPayments": webPage = GetResource("M_SCHEDULEOFPAYMENTS");
                    index = 5;
                    break;
                // Arrangement Schedule   - jec 09/06/09 Credit Collections changes
                case "ScheduleOfArrangementPayments": webPage = GetResource("M_SCHEDULEOFPAYMENTS");
                    index = 5;
                    break;
                case "DeliveryNote2": webPage = GetResource("M_DELIVERYNOTE");
                    index = 7;
                    break;
                case "DeliverySchedule": webPage = GetResource("M_TRANSPORTSCHEDULE");
                    index = 8;
                    break;
                case "PrizeVoucher": webPage = GetResource("M_PRIZEVOUCHER");
                    index = 9;
                    break;
                case "CashLoan": webPage = "Cash Loan";
                    index = 10;
                    break;
                default:
                    index = 6;
                    break;
            }

            ToolBarButton tb = new ToolBarButton();
            tb.ImageIndex = index;
            tb.Tag = browser;
            tb.ToolTipText = webPage;
            toolBar.Buttons.Add(tb);
            Width += buttonWidth;
        }

        public void AddButton(AxWebBrowser browser, string webPage)
        {
            int index = 0;
            //string webPage = browser.LocationURL;

            //int x = webPage.LastIndexOf("/") + 2;
            //int y = webPage.IndexOf(".aspx");
            //// x can be greater than y if a date is in the url i.e. 29/06/09
            //if (x > y)
            //{
            //    string temp = webPage.Split(new string[] { ".aspx" }, StringSplitOptions.RemoveEmptyEntries)[0];
            //    x = temp.LastIndexOf("/") + 2;
            //}

            //webPage = webPage.Substring(x, y - x);

            switch (webPage)
            {
                case "Agreement": webPage = GetResource("M_AGREEMENT");
                    index = 0;
                    break;
                case "ActionSheet": webPage = GetResource("M_ACTIONSHEET");
                    index = 6;
                    break;
                case "CollectionNote": webPage = GetResource("M_COLLECTIONNOTE");
                    index = 6;
                    break;
                case "DeliveryNote": webPage = GetResource("M_DELIVERYNOTE");
                    index = 6;
                    break;
                case "FaultyProductNote": webPage = GetResource("M_FAULTYPRODUCT");
                    index = 6;
                    break;
                case "OneForOneReplacementNote": webPage = GetResource("M_ONEFORONEREPLACEMENT");
                    index = 6;
                    break;
                case "PaidAndTakenTaxInvoice": webPage = GetResource("M_TAXINVOICE");
                    index = 4;
                    break;
                case "RFSummary": webPage = GetResource("M_RFSUMMARY");
                    index = 1;
                    break;
                case "TaxInvoice": webPage = GetResource("M_TAXINVOICE");
                    index = 4;
                    break;
                case "WarrantyContract": webPage = GetResource("M_WARRANTYCONTRACT");
                    index = 3;
                    break;
                case "RFTerms": webPage = GetResource("M_RFTERMS");
                    index = 2;
                    break;
                case "CashierTotals": webPage = GetResource("M_CASHIERTOTALS");
                    index = 6;
                    break;
                case "ScheduleOfPayments": webPage = GetResource("M_SCHEDULEOFPAYMENTS");
                    index = 5;
                    break;
                // Arrangement Schedule   - jec 09/06/09 Credit Collections changes
                case "ScheduleOfArrangementPayments": webPage = GetResource("M_SCHEDULEOFPAYMENTS");
                    index = 5;
                    break;
                case "DeliveryNote2": webPage = GetResource("M_DELIVERYNOTE");
                    index = 7;
                    break;
                case "DeliverySchedule": webPage = GetResource("M_TRANSPORTSCHEDULE");
                    index = 8;
                    break;
                case "PrizeVoucher": webPage = GetResource("M_PRIZEVOUCHER");
                    index = 9;
                    break;
                case "CashLoan": webPage = "Cash Loan";
                    index = 10;
                    break;
                default:
                    index = 6;
                    break;
            }

            ToolBarButton tb = new ToolBarButton();
            tb.ImageIndex = index;
            tb.Tag = browser;
            tb.ToolTipText = webPage;
            toolBar.Buttons.Add(tb);
            Width += buttonWidth;
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PrinterStatus));
            this.toolBar = new System.Windows.Forms.ToolBar();
            this.images = new System.Windows.Forms.ImageList(this.components);
            this.SuspendLayout();
            // 
            // toolBar
            // 
            this.toolBar.ButtonSize = new System.Drawing.Size(24, 24);
            this.toolBar.Divider = false;
            this.toolBar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolBar.DropDownArrows = true;
            this.toolBar.ImageList = this.images;
            this.toolBar.Location = new System.Drawing.Point(0, 0);
            this.toolBar.Name = "toolBar";
            this.toolBar.ShowToolTips = true;
            this.toolBar.Size = new System.Drawing.Size(0, 28);
            this.toolBar.TabIndex = 0;
            this.toolBar.Wrappable = false;
            this.toolBar.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler(this.toolBar_ButtonClick);
            // 
            // images
            // 
            this.images.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("images.ImageStream")));
            this.images.TransparentColor = System.Drawing.Color.Transparent;
            this.images.Images.SetKeyName(0, "");
            this.images.Images.SetKeyName(1, "");
            this.images.Images.SetKeyName(2, "");
            this.images.Images.SetKeyName(3, "");
            this.images.Images.SetKeyName(4, "");
            this.images.Images.SetKeyName(5, "");
            this.images.Images.SetKeyName(6, "");
            this.images.Images.SetKeyName(7, "");
            this.images.Images.SetKeyName(8, "");
            this.images.Images.SetKeyName(9, "prize.gif");
            this.images.Images.SetKeyName(10, "0260967.jpg");
            this.images.Images.SetKeyName(11, "WarrantyContractFree.bmp");
            // 
            // PrinterStatus
            // 
            this.Controls.Add(this.toolBar);
            this.Name = "PrinterStatus";
            this.Size = new System.Drawing.Size(0, 26);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        private void toolBar_ButtonClick(object sender, System.Windows.Forms.ToolBarButtonClickEventArgs e)
        {
            ToolBarButton tb = e.Button;
            AxWebBrowser b = (AxWebBrowser)tb.Tag;

            toolBar.Buttons.Remove(tb);
            this.Width -= buttonWidth;

            IHTMLDocument2 HTMLDocument =
                (IHTMLDocument2)b.Document;
            HTMLDocument.title = "";
            HTMLDocument.execCommand("Print", false, null);
        }
    }
}
