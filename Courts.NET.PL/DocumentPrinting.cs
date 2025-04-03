using System;
using mshtml;
using AxSHDocVw;
using System.Windows.Forms;
using System.Threading;

namespace STL.PL
{
	/// <summary>
	/// Contains common functionality for document printing. This includes
	/// documents such as Agreement Terms, Delivery Note, Tax Invoice and
	/// Cashier Totals etc.
	/// </summary>
	public class DocumentPrinting : CommonForm
	{
		private int _printOrder = -1;
		private AxWebBrowser _browser = null;
		private MainForm _main = null;
		private CommonForm _parent = null;
        delegate void SetTextCallback(string text);
        //private string statBar = "";

		public DocumentPrinting(AxWebBrowser browser, MainForm main, CommonForm parent)
		{
			_printOrder = browser.TabIndex;
			_browser = browser;
			_main = main;
			_parent = parent;
		}

		public void DocumentCompleteThread()
		{
			/* make sure threads complete in the right order */
			//Random r = new Random();
			//Thread.Sleep(r.Next(1000));

			while (! BrowsersLoaded(_printOrder))
				Thread.Sleep(500);			
		}

		private bool BrowsersLoaded(int loaded)
		{
			bool status = false;
			lock(this)
			{
				if(loaded == _main._browsersLoaded + 1)
				{
					string webPage = _browser.LocationURL;

					/* look up the page loaded in the resourse table so it can be translated */
                   
					if (webPage.Contains("Agreement.aspx"))
                        webPage = GetResource("M_AGREEMENT");
                    else if (webPage.Contains("ActionSheet.aspx"))
                        webPage = GetResource("M_ACTIONSHEET");
                    else if (webPage.Contains("CollectionNote.aspx"))
                        webPage = GetResource("M_COLLECTIONNOTE");
                    else if (webPage.Contains("DeliveryNote.aspx"))
                        webPage = GetResource("M_DELIVERYNOTE");
                    else if (webPage.Contains("FaultyProductNote.aspx"))
                        webPage = GetResource("M_FAULTYPRODUCT");
                    else if (webPage.Contains("OneForOneReplacementNote.aspx"))
                        webPage = GetResource("M_ONEFORONEREPLACEMENT");
                    else if (webPage.Contains("PaidAndTakenTaxInvoice.aspx"))
                        webPage = GetResource("M_TAXINVOICE");
                    else if (webPage.Contains("RFSummary.aspx"))
                        webPage = GetResource("M_RFSUMMARY");
                    else if (webPage.Contains("TaxInvoice.aspx"))
                        webPage = GetResource("M_TAXINVOICE");
                    else if (webPage.Contains("WarrantyContract.aspx"))
                        webPage = GetResource("M_WARRANTYCONTRACT");
                    else if (webPage.Contains("RFTerms.aspx"))
                        webPage = GetResource("M_RFTERMS");
                    else if (webPage.Contains("CashierTotals.aspx"))
                        webPage = GetResource("M_CASHIERTOTALS");
                    else if (webPage.Contains("ScheduleOfPayments.aspx"))
                        webPage = GetResource("M_SCHEDULEOFPAYMENTS");
					
                    string statBar = webPage + " Loaded, ";			
                    SetText(statBar);
                    
    				IHTMLDocument2 HTMLDocument = (IHTMLDocument2) _browser.Document;
					HTMLDocument.title = "";
					HTMLDocument.execCommand("Print", false, null);					

					_main._browsersLoaded = loaded;
					status = true;
				}
			}
			return status;
		}

        private void SetText(string text)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (this._main.statusBar1.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetText);
                this._main.statusBar1.Invoke(d, new object[] { text });
            }
            else
            {
                this._main.statusBar1.Text += text;
            }
        }
	}
}

