using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using AxSHDocVw;
using STL.Common;
using STL.Common.Constants.Categories;
using STL.Common.Constants.StoreInfo;
using STL.Common.Constants.TableNames;
using STL.Common.Static;
using STL.EPOS;
using STL.PL.CashLoan;
using STL.PL.Collections;
//using STL.PL.SERVICE;
using STL.PL.StoreCard;
using STL.PL.Tallyman;
using STL.PL.Warehouse;
//JC CR1072
//CR1232 
// #10229 
using STL.PL.WS1;
using STL.PL.WS10;
using STL.PL.WS11;
using STL.PL.WS12;
using STL.PL.WS2;
using STL.PL.WS3;
using STL.PL.WS4;
using STL.PL.WS5;
using STL.PL.WS6;
using STL.PL.WS7;
using STL.PL.WS8;
using STL.PL.WS9;
using STL.PL.WSInstallation;
using STL.PL.WSPrinting;
using STL.PL.WSStock;
using STL.PL.WSStoreCard;
using STL.PL.SERVICE;


namespace STL.PL
{
	/// <summary>
	/// The startup form for the application. This form displays the application
	/// wallpaper and the login prompt. Static data is loaded here to be used by
	/// the various drop down fields in the application. All of the main menu options
	/// are displayed and invoked from this form. The menus available are controlled
	/// by the user permissions for the user role logged in.
	/// </summary>
	public partial class MainForm : CommonForm
	{
		public NewAccount newAcct;
		private new string Error = "";
		private System.Windows.Forms.ErrorProvider errorProvider1;
		public System.Windows.Forms.StatusBar statusBar1;
		public Crownwood.Magic.Controls.TabControl MainTabControl;
		private Crownwood.Magic.Controls.TabPage tpLogIn;
		public new Crownwood.Magic.Menus.MenuControl menuMain;
		private Crownwood.Magic.Menus.MenuCommand menuFile;
		private Crownwood.Magic.Menus.MenuCommand menuLogOff;
		private Crownwood.Magic.Menus.MenuCommand menuExit;
		private Crownwood.Magic.Menus.MenuCommand menuConfig;
		private Crownwood.Magic.Menus.MenuCommand menuVersion;
		private Crownwood.Magic.Menus.MenuCommand menuChangePassword;
		private Crownwood.Magic.Menus.MenuCommand menuAccount;
		private Crownwood.Magic.Menus.MenuCommand menuCustomer;
		private Crownwood.Magic.Menus.MenuCommand menuSysMaint;
		private Crownwood.Magic.Menus.MenuCommand menuCollections;
		private Crownwood.Magic.Menus.MenuCommand menuNewAccount;
		private Crownwood.Magic.Menus.MenuCommand menuManualSale;
		private Crownwood.Magic.Menus.MenuCommand menuAccountDetails;
		private Crownwood.Magic.Menus.MenuCommand menuAccountRevise;
		private Crownwood.Magic.Menus.MenuCommand menuAddAcctCodes;
		private Crownwood.Magic.Menus.MenuCommand menuCustomerSearch;
		private Crownwood.Magic.Menus.MenuCommand menuAddCustCodes;
		private Crownwood.Magic.Menus.MenuCommand menuCustomiseMenus;
		private Crownwood.Magic.Menus.MenuCommand menuMandatory;
		public STL.PL.SanctionStatus tbSanction;
		private Crownwood.Magic.Menus.MenuCommand menuNewCustomer;
		private Crownwood.Magic.Menus.MenuCommand menuScoringRules;
		private System.ComponentModel.IContainer components;
		private Crownwood.Magic.Menus.MenuCommand menuScoringMatrix;
		private Crownwood.Magic.Menus.MenuCommand menuDepenSpendFactor;
        private Crownwood.Magic.Menus.MenuCommand menuApplicantSpendFactor;
		private Crownwood.Magic.Menus.MenuCommand menuCredit;
		private Crownwood.Magic.Menus.MenuCommand menuIncomplete;
		public System.Windows.Forms.ImageList menuIcons;
		public System.Windows.Forms.PictureBox pbSplash;
		public System.Windows.Forms.GroupBox grpLogIn;
		private System.Windows.Forms.TextBox txtPassword;
		private System.Windows.Forms.TextBox txtUser;
		private AppUpdater.AppUpdater appUpdater1;
		private Crownwood.Magic.Menus.MenuCommand menuAuthoriseDelivery;
		private Crownwood.Magic.Menus.MenuCommand menuPaidAndTaken;
		private Crownwood.Magic.Menus.MenuCommand menuGoodsReturn;
		private Crownwood.Magic.Menus.MenuCommand menuTranslation;

		private Crownwood.Magic.Menus.MenuCommand menuTransaction;
		private Crownwood.Magic.Menus.MenuCommand menuPayment;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private Crownwood.Magic.Menus.MenuCommand menuWarehouse;
		//private Crownwood.Magic.Menus.MenuCommand menuImmediateDelivery;
		private Crownwood.Magic.Menus.MenuCommand menuScreenTranslation;
		public System.Windows.Forms.PictureBox pbDownloading;
		public System.Windows.Forms.Label lDownloading;
		private Crownwood.Magic.Menus.MenuCommand menuFollowUp;
		private Crownwood.Magic.Menus.MenuCommand menuTelephoneAction;
		private Crownwood.Magic.Menus.MenuCommand menuOneForOneReplacement;
		private Crownwood.Magic.Menus.MenuCommand menuCodeMaintenance;
		private Crownwood.Magic.Menus.MenuCommand menuEmployeeMaintenance;
		private Crownwood.Magic.Menus.MenuCommand menuBranch;
		//private Crownwood.Magic.Menus.MenuCommand menuRePrintDeliveryNote;
		private Crownwood.Magic.Menus.MenuCommand menuSearchCashAndGo;
		//private Thread updates = null;
		private LocationStockEnquiry _locationStockEnquiry = null;
		private Crownwood.Magic.Menus.MenuCommand menuImageManagement;
		private Crownwood.Magic.Menus.MenuCommand menuCashier;
		private Crownwood.Magic.Menus.MenuCommand menuTransferTransaction;
		private Crownwood.Magic.Menus.MenuCommand menuJournal;
		private Crownwood.Magic.Menus.MenuCommand menuCorrection;
		private Crownwood.Magic.Menus.MenuCommand menuReturnCheque;
		private Crownwood.Magic.Menus.MenuCommand menuCancel;
		private Crownwood.Magic.Menus.MenuCommand menuGeneralTransactions;
		private System.Data.SqlClient.SqlCommand sqlCommand1;
		private Crownwood.Magic.Menus.MenuCommand menuDDMandate;
		private Crownwood.Magic.Menus.MenuCommand menuTransTypeMaintenance;
		private Crownwood.Magic.Menus.MenuCommand menuLocateHelp;
		private Crownwood.Magic.Menus.MenuCommand menuLocationEnquiry;
		private Crownwood.Magic.Menus.MenuCommand menuProductEnquiry;
		private Crownwood.Magic.Menus.MenuCommand menuWOReview;
		private Crownwood.Magic.Menus.MenuCommand menuExchangeRates;
		public STL.PL.PrinterStatus PrinterStat;
		private Crownwood.Magic.Menus.MenuCommand menuSellGiftVoucher;
		private Crownwood.Magic.Menus.MenuCommand menuCommand2;
		private Crownwood.Magic.Menus.MenuCommand menuStatusCode;
		private Crownwood.Magic.Menus.MenuCommand menuEOD;
		private Crownwood.Magic.Menus.MenuCommand menuCountryMaintenance;
		public SanctionStage1 sanctionStage1 = null;
		private Crownwood.Magic.Menus.MenuCommand menuCashAndGoReturn;
		private Crownwood.Magic.Menus.MenuCommand menuUpdateDateDue;
		private Crownwood.Magic.Menus.MenuCommand menuDisbursements;
		private Crownwood.Magic.Menus.MenuCommand menuDeposits;
		private Crownwood.Magic.Menus.MenuCommand menuTermsType;
        private Crownwood.Magic.Menus.MenuCommand menuMmiMatrix;
        private Crownwood.Magic.Menus.MenuCommand menuDeliveryArea;
		private Crownwood.Magic.Menus.MenuCommand menuChangeCustID;
		private Crownwood.Magic.Menus.MenuCommand menuNumberGeneration;
		private Crownwood.Magic.Menus.MenuCommand menuReverseCancel;
		private Crownwood.Magic.Menus.MenuCommand menuCashierByBranch;
		private Crownwood.Magic.Menus.MenuCommand menuWarrantyRenewals;
		private Crownwood.Magic.Menus.MenuCommand menuFinTransQuery;
		private Crownwood.Magic.Menus.MenuCommand menuTempReceiptInvest;
		private Crownwood.Magic.Menus.MenuCommand menuBailiffReview;
		private Crownwood.Magic.Menus.MenuCommand menuReports;
		private Crownwood.Magic.Menus.MenuCommand menuSumryUpdControl;
		private Crownwood.Magic.Menus.MenuCommand menuDDPaymentExtra;
		private Crownwood.Magic.Menus.MenuCommand menuDDRejection;
		//private Crownwood.Magic.Menus.MenuCommand menuOrdersForDelivery;
		private Crownwood.Magic.Menus.MenuCommand menuReprintActionSheet;
		private Crownwood.Magic.Menus.MenuCommand menuRedeliverReposs;
		//private Crownwood.Magic.Menus.MenuCommand menuDeliveryNotification;
		//private Crownwood.Magic.Menus.MenuCommand menuDelSchedule;
		private Crownwood.Magic.Menus.MenuCommand menuRebateCalculation;
		//private Crownwood.Magic.Menus.MenuCommand menuPrintDelSched;
		//private Crownwood.Magic.Menus.MenuCommand menuTransportMaint;
		private Crownwood.Magic.Menus.MenuCommand menuUnpaidAccounts;
		//private Crownwood.Magic.Menus.MenuCommand menuAmendReprintPicklist;
		private Crownwood.Magic.Menus.MenuCommand menuAcctNoCtrl;
		private Crownwood.Magic.Menus.MenuCommand menuCancelCollectionNotes;
		private Crownwood.Magic.Menus.MenuCommand menuRebateReport;
		private Crownwood.Magic.Menus.MenuCommand menuCommand3;
		private Crownwood.Magic.Menus.MenuCommand menuMonitorBookings;
		//private Crownwood.Magic.Menus.MenuCommand menuMonitorDeliveries;
		private Crownwood.Magic.Menus.MenuCommand menuCommissionMaint;
		private Crownwood.Magic.Menus.MenuCommand menuCalcBailCommission;
		private Crownwood.Magic.Menus.MenuCommand menuReprintBailCommn;
		private Crownwood.Magic.Menus.MenuCommand menuInterfaceReport;
		//private Crownwood.Magic.Menus.MenuCommand menuTransportSchedule;
		private Crownwood.Magic.Menus.MenuCommand menuChangeOrderDetails;
		//private Crownwood.Magic.Menus.MenuCommand menuScheduleChanges;
		private Crownwood.Magic.Menus.MenuCommand menuEODInterface;
		private Crownwood.Magic.Menus.MenuCommand menuCustomerMailing;
		private Crownwood.Magic.Menus.MenuCommand menuPaymentFileDefn;
		private Crownwood.Magic.Menus.MenuCommand menuTTMatrix;
		//private Crownwood.Magic.Menus.MenuCommand menuService;
		//private Crownwood.Magic.Menus.MenuCommand menuServiceRequest;
		//private Crownwood.Magic.Menus.MenuCommand menuServiceRequestSearch;
		//private Crownwood.Magic.Menus.MenuCommand menuServiceRequestAudit;
		//private Crownwood.Magic.Menus.MenuCommand menuServiceTechMaintenance;
		//private Crownwood.Magic.Menus.MenuCommand menuServiceTechDiary;
		//private Crownwood.Magic.Menus.MenuCommand menuServicePriceMatrix;
		//private Crownwood.Magic.Menus.MenuCommand menuServiceChargeToAuthorisation;
		//private Crownwood.Magic.Menus.MenuCommand menuServiceTechPayment;
		//private Crownwood.Magic.Menus.MenuCommand menuServiceOutstanding;
		//private Crownwood.Magic.Menus.MenuCommand menuServiceBatchPrint;
		private Crownwood.Magic.Menus.MenuCommand menuPrizeVouchers;
		private Crownwood.Magic.Menus.MenuCommand menuCommissionsSetUp;
		//private Crownwood.Magic.Menus.MenuCommand menuServiceProgressReport;
		//private Crownwood.Magic.Menus.MenuCommand menuServiceFailureReport;
		//private Crownwood.Magic.Menus.MenuCommand menuServiceClaimsReport;
		private Crownwood.Magic.Menus.MenuCommand menuInstantReplacement;
		private Crownwood.Magic.Menus.MenuCommand menuWarrantyReport;
		private Crownwood.Magic.Menus.MenuCommand menuFactoringReports;
		private Crownwood.Magic.Menus.MenuCommand menuCommissionReport;
		private Crownwood.Magic.Menus.MenuCommand menuAbout;
		private Crownwood.Magic.Menus.MenuCommand menuEPOS;
		private Crownwood.Magic.Menus.MenuCommand menuStrategyConfiguration;
		private Crownwood.Magic.Menus.MenuCommand menuWorkLists;
		private Crownwood.Magic.Menus.MenuCommand menuSMS;
		private Crownwood.Magic.Menus.MenuCommand menuCommissionEnquiry;
		private Crownwood.Magic.Menus.MenuCommand menuAddBank;
		private Crownwood.Magic.Menus.MenuCommand menuProvisions;
		private Crownwood.Magic.Menus.MenuCommand menuAccountStatus;
		private Crownwood.Magic.Menus.MenuCommand menuServiceCustomerInteraction;
		private Crownwood.Magic.Menus.MenuCommand menuViewStoreCard;
		private Crownwood.Magic.Menus.MenuCommand menuLetterMerge;
		private Crownwood.Magic.Menus.MenuCommand menuZoneAutomation;
		private Crownwood.Magic.Menus.MenuCommand menuTallymanExtract;
		private Control openTill = null;
		private Crownwood.Magic.Menus.MenuCommand menuStatements;
		private Crownwood.Magic.Menus.MenuCommand menuStoreCardRateSetup;
		private Crownwood.Magic.Menus.MenuCommand menuNonStock;
		private Crownwood.Magic.Menus.MenuCommand menuBehavioural;
		private Crownwood.Magic.Menus.MenuCommand menuPendingInstallations;
		private Crownwood.Magic.Menus.MenuCommand menuInstBookingPrint;
		private Crownwood.Magic.Menus.MenuCommand menuAuthoriseIC;
		private Crownwood.Magic.Menus.MenuCommand menuInstManagement;
		private Crownwood.Magic.Menus.MenuCommand menuAllEODTasks;
		private Crownwood.Magic.Menus.MenuCommand menuScoring;
		private Crownwood.Magic.Menus.MenuCommand menuSpendFactor;
		private Crownwood.Magic.Menus.MenuCommand menuTestMode;
		//private WinDriver.Server.Windows.Wingman wingman;
		private PictureBox progress;
		private Crownwood.Magic.Menus.MenuCommand menuCashLoanApplication;
		public GroupBox grpLogInCourts;
		private Label label5;
		private Label label6;
		private TextBox txtPasswordCourts;
		private TextBox txtUserCourts;
		private ResourceManager resources;
		private Crownwood.Magic.Menus.MenuCommand menuFailedDeliveriesCollections;
		private Crownwood.Magic.Menus.MenuCommand menuStoreCardBatchPrint;
		private Crownwood.Magic.Menus.MenuCommand menuService;
		private Crownwood.Magic.Menus.MenuCommand menuBERRep;
    private Crownwood.Magic.Menus.MenuCommand menuDuplicateCustomers;
    private Crownwood.Magic.Menus.MenuCommand menuSalesCommEnquiry;
    private Crownwood.Magic.Menus.MenuCommand menuSalesCommBranchEnquiry;
    private Crownwood.Magic.Menus.MenuCommand menuCashLoanBankTransfer;
    private Crownwood.Magic.Menus.MenuCommand menuOnlineProductSearch;
        private Crownwood.Magic.Menus.MenuCommand RePrintInvoice;
        public static bool upgrading = false;

		public LocationStockEnquiry StockEnquiryByLocation
		{
			get
			{
				if (_locationStockEnquiry == null)
					_locationStockEnquiry = new LocationStockEnquiry();
				return _locationStockEnquiry;
			}
		}

		public MainForm(TranslationDummy d)
		{
			InitializeComponent();
		}


		public static MainForm Current { get; private set; }

		public MainForm()
			: base()
		{
			//
			// Required for Windows Form Designer support
			//	

			Current = this;

			try
			{
				dynamicMenus = new Hashtable();
				InitializeComponent();

				resources = new ResourceManager("STL.PL.Properties.Resources", Assembly.GetExecutingAssembly());
				if (String.IsNullOrEmpty(Properties.Settings.Default.Branch))
				{
					this.menuConfig.Enabled = true;
					this.menuConfig.Visible = true;
					ConfigMaintenance config = new ConfigMaintenance(true);
					config.FormRoot = this;
					config.FormParent = this;
					config.ShowDialog();
				}
				else
				{
					var Settings = Properties.Settings.Default;
					Config.BranchCode = String.IsNullOrEmpty(Settings.Branch) ? Config.BranchCode : Settings.Branch;
					Config.SplashImage = Settings.SplashImage;
					Config.CashDrawerID = Settings.CashDrawerID;
					Config.ReceiptPrinterModel = Settings.ReceiptPrinterModel;
					Config.ThermalPrintingEnabled = Settings.ThermalPrinterEnabled;
					Config.ThermalPrinterName = Settings.PrinterName;
					Config.Culture = Settings.Culture;
					CultureInfo current = new CultureInfo(Settings.Culture);
				}
				Thread.CurrentThread.CurrentCulture = new CultureInfo(Config.Culture);
				Thread.CurrentThread.CurrentUICulture = new CultureInfo(Config.Culture);

				Config.Version = Assembly.GetExecutingAssembly().GetName().Version.ToString();

				// hook up global async service call event handlers
				Blue.Cosacs.Shared.Services.Client.Begin += OnAsyncServiceStart;
				Blue.Cosacs.Shared.Services.Client.Success += OnAsyncServiceEnd;
				Blue.Cosacs.Shared.Services.Client.Exception += OnAsyncServiceException;

				// add progress bar
				progress = new PictureBox();
				progress.Anchor = AnchorStyles.Bottom | AnchorStyles.Right | AnchorStyles.Top;
				progress.Image = (System.Drawing.Image)resources.GetObject("ajax_squares");
				progress.SizeMode = PictureBoxSizeMode.CenterImage;
				progress.Height = 11;
				progress.Width = 43;
				progress.Top = 5;
				progress.Left = 730;
				progress.Visible = false;
				this.statusBar1.Controls.Add(progress);


				this.FormRoot = this;
				this.FormParent = this;

				CountryParameterCollection.CountryParamNotFound += new EventHandler<CountryParamEventArgs>(CountryParamNotFoundAlert);


				LoadDictionary();
				TranslateControls();
				TranslateMenus(menuMain.MenuCommands);

				appUpdater1.UpdateUrl = Config.Url + "UpdateVersion.xml";

				HashMenus();
				tpLogIn.Tag = this.menuMain;

				this.menuExit.Click += new System.EventHandler(this.menuExit_Click);
				this.MainTabControl.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
				this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;

				//Wire up an event handler to intercept application exit
				Application.ApplicationExit += new System.EventHandler(this.OnApplicationExit);

				//if (Config.TestKey == "815691")
				//{
				//    // Always enable Config App when testing
				//    this.menuConfig.Enabled = true;
				//    this.menuConfig.Visible = true;
				//}
				this.Text = String.Format("CoSaCS [{0}]", Config.Server);

				tbSanction.Common.FormRoot = this;


				//var resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
				this.Icon = ((System.Drawing.Icon)(resources.GetObject("CosacsIcon")));
				grpLogIn.Visible = true;
				SplashSetup(true);





				//CR903 If this is a Courts branch then show the splash image
				/*if (Config.StoreType == StoreType.Courts)
				{
					pbSplash.Visible = true;
					grpLogIn.Visible = false;
					grpLogInCourts.Visible = true;
				}
				else
				{
					pbSplash.Visible = false;
					grpLogIn.Visible = true;
				grpLogInCourts.Visible = false;
				}*/

			}
			catch (Exception ex)
			{
				if (ex is ServerConnectException)
					throw;
				MessageBox.Show("There is a problem with CoSACS configuration:" + Environment.NewLine + ex.Message.ToString(), "CoSACS configuration Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}


			//Remove any customer photos that were not deleted on closing down the app
			string path = Application.StartupPath;
			foreach (var file in new System.IO.DirectoryInfo(path).GetFiles("*.jpg"))
				if (file.Name != "ErrorScreenShot.jpg")
					Delete(file);

			foreach (var file in new System.IO.DirectoryInfo(path).GetFiles("*.tmp"))
				Delete(file);


		}

		private void MenuOverride()
		{
			if (!Credential.HasPermission(Blue.Cosacs.Shared.CosacsPermissionEnum.CustomerSearch) && menuCustomer.MenuCommands.IndexOf(menuCustomerSearch) > 0)
				menuCustomer.MenuCommands.Remove(menuCustomerSearch);

			if ((!Credential.HasPermission(Blue.Cosacs.Shared.CosacsPermissionEnum.CashLoan) && !Credential.HasPermission(Blue.Cosacs.Shared.CosacsPermissionEnum.CashLoanDisbursement)) && menuCustomer.MenuCommands.IndexOf(menuCashLoanApplication) > 0)  //#14910
				menuCustomer.MenuCommands.Remove(menuCashLoanApplication);

			if (!Credential.HasPermission(Blue.Cosacs.Shared.CosacsPermissionEnum.CodeMaintenance) && menuCustomer.MenuCommands.IndexOf(menuCodeMaintenance) > 0)
				menuSysMaint.MenuCommands.Remove(menuCodeMaintenance);

			if (!Credential.HasPermission(Blue.Cosacs.Shared.CosacsPermissionEnum.LegacyCashAndGo) && menuCustomer.MenuCommands.IndexOf(menuCashAndGoReturn) > 0)
				menuAccount.MenuCommands.Remove(menuCashAndGoReturn);

			//#12385
			if (!Credential.HasPermission(Blue.Cosacs.Shared.CosacsPermissionEnum.StoreCardBatchPrint) && menuAllEODTasks.MenuCommands.IndexOf(menuStoreCardBatchPrint) > 0)
				menuAllEODTasks.MenuCommands.Remove(menuStoreCardBatchPrint);

			//if (!Credential.HasPermission(Blue.Cosacs.Shared.CosacsPermissionEnum.ServiceBatchPrint) && menuCustomer.MenuCommands.IndexOf(menuServiceBatchPrint) > 0)
			//    menuService.MenuCommands.Remove(menuServiceBatchPrint);

            //#19422 - CR17976
            if (!Credential.HasPermission(Blue.Cosacs.Shared.CosacsPermissionEnum.DuplicateCustomers) && menuCustomer.MenuCommands.IndexOf(menuDuplicateCustomers) > 0)
                menuCustomer.MenuCommands.Remove(menuDuplicateCustomers);

            if (!Credential.HasPermission(Blue.Cosacs.Shared.CosacsPermissionEnum.SalesCommEnquiryCSR)
                    && !Credential.HasPermission(Blue.Cosacs.Shared.CosacsPermissionEnum.SalesCommissionBranchEnquiry)
                        && menuReports.MenuCommands.IndexOf(menuSalesCommEnquiry) > 0)
                menuReports.MenuCommands.Remove(menuSalesCommEnquiry);

           if(!Credential.HasPermission(Blue.Cosacs.Shared.CosacsPermissionEnum.SalesCommissionBranchEnquiry)
                && menuReports.MenuCommands.IndexOf(menuSalesCommBranchEnquiry) > 0)
                menuReports.MenuCommands.Remove(menuSalesCommBranchEnquiry);

           if ((!Credential.HasPermission(Blue.Cosacs.Shared.CosacsPermissionEnum.CashLoanDisbursementRecordBankTransfer)) && menuCustomer.MenuCommands.IndexOf(menuCashLoanBankTransfer) > 0)
               menuCustomer.MenuCommands.Remove(menuCashLoanBankTransfer);

		}


		private void Delete(FileInfo file)
		{
			try
			{
				file.Delete();
			}
			catch
			{ }
		}

		private void OnAsyncServiceStart<TRequest>(TRequest request)
		{
			lock (this)
			{
				if (asyncCallsInProgressCount < 0)
					asyncCallsInProgressCount = 0;
				asyncCallsInProgressCount++;
				ProgressStart();
			}
		}

		private int asyncCallsInProgressCount = 0;

		private void OnAsyncServiceException<TRequest>(TRequest request, Exception ex, bool handled)
		{
			OnAsyncServiceEnd(request);
			if (!handled)
				Application_ThreadException(this, new ThreadExceptionEventArgs(ex));
		}

		private void OnAsyncServiceEnd<TRequest>(TRequest request)
		{
			DecrementProcessCounter();
		}

		private void DecrementProcessCounter()
		{
			lock (this)
			{
				asyncCallsInProgressCount--;
				if (asyncCallsInProgressCount < 0)
					asyncCallsInProgressCount = 0;
				if (asyncCallsInProgressCount == 0)
				{
					BeginInvoke(new InvokeDelegate(() => ProgressEnd()));
				}
			}
		}

		public void SetUpgrade()
		{
			upgrading = true;
		}


		public delegate void InvokeDelegate();

		public new void ProgressStart()
		{
			this.progress.Visible = true;
			//this.activef = Cursors.AppStarting;
		}

		public new void ProgressEnd()
		{
			this.progress.Visible = false;
			//Cursor = Cursors.Default;
		}

		private void SplashAfterLogin()
		{
			if (string.IsNullOrEmpty(Config.SplashImage))
				pbSplash.Dock = DockStyle.Fill;
		}

		private void SplashAfterLogout()
		{
			if (string.IsNullOrEmpty(Config.SplashImage))
			{
				pbSplash.Height = 250;
				pbSplash.Dock = DockStyle.Bottom;
			}
		}

		public void SplashSetup()
		{
			SplashSetup(grpLogIn.Visible);
		}

		public void SplashSetup(bool beforeLogin)
		{
			pbSplash.Visible = true;
			pbSplash.BorderStyle = System.Windows.Forms.BorderStyle.None;

			const string whitePanelName = "whitePanel";
			Panel whitePanel;

			if (!string.IsNullOrEmpty(Config.SplashImage))
			{
				pbSplash.Image = new Bitmap(Config.SplashImage);
				pbSplash.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
				pbSplash.Dock = DockStyle.Fill;

				grpLogIn.Top = 160;
				grpLogIn.BackColor = Color.Transparent;

				if (tpLogIn.Controls.ContainsKey(whitePanelName))
				{
					whitePanel = (Panel)tpLogIn.Controls[whitePanelName];
					whitePanel.Visible = false;
				}
			}
			else
			{
				// var resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
				pbSplash.Image = (System.Drawing.Image)resources.GetObject("CosacsSplash");
				pbSplash.BackColor = Color.White;
				pbSplash.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
				pbSplash.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
				//pbSplash.Height = 250; 
				//pbSplash.Dock = DockStyle.Bottom;

				grpLogIn.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom;
				grpLogIn.BackColor = Color.White;
				grpLogIn.Top = 75;

				if (tpLogIn.Controls.ContainsKey(whitePanelName))
				{
					whitePanel = (Panel)tpLogIn.Controls[whitePanelName];
					whitePanel.Visible = true;
				}
				else
				{
					whitePanel = new Panel { Name = whitePanelName };
					whitePanel.BackColor = Color.White;
					whitePanel.Dock = DockStyle.Fill;
					tpLogIn.Controls.Add(whitePanel);
				}
			}

			if (beforeLogin)
				SplashAfterLogout();
			else
				SplashAfterLogin();
		}

		private void CountryParamNotFoundAlert(object sender, CountryParamEventArgs e)
		{
			MessageBox.Show("Warning! The country parameter " + e.CountryParamName + " was not setup." + Environment.NewLine +
							"Please contact CoSACS support before using system.", "Database Setup Error!", MessageBoxButtons.OK);
		}

		/// <summary>
		/// kick off an asynchronous web service call to load all the required
		/// static data in the background. This should slightly improve 
		/// perceived performance in loading new screens for the first time 
		/// </summary>
		private void InitialiseStaticData()
		{
			try
			{
				Wait();

				AsyncCallback cb = new AsyncCallback(StaticDataCallback);

				XmlUtilities xml = new XmlUtilities();
				XmlDocument dropDowns = new XmlDocument();
				dropDowns.LoadXml("<DROP_DOWNS></DROP_DOWNS>");

				if (StaticData.Tables[TN.TermsType] == null)
					dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.TermsType, new string[] { Config.CountryCode }));

				if (StaticData.Tables[TN.SourceOfAttraction] == null)
					dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.SourceOfAttraction, null));

				if (StaticData.Tables[TN.SalesStaff] == null)
					dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.SalesStaff, new string[] { Config.BranchCode, "S" }));

				if (StaticData.Tables[TN.AllStaff] == null)
					dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.AllStaff, null));

				if (StaticData.Tables[TN.MethodOfPayment] == null)
					dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.MethodOfPayment, null));

				if (StaticData.Tables[TN.AccountType] == null)
					dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.AccountType, new string[] { Config.CountryCode, Config.BranchCode }));

				if (StaticData.Tables[TN.BranchNumber] == null)
					dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.BranchNumber, null));

				if (StaticData.Tables[TN.CustomerCodes] == null)
					dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.CustomerCodes, null));

				if (StaticData.Tables[TN.AccountCodes] == null)
					dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.AccountCodes, null));

				if (StaticData.Tables[TN.UserTypes] == null)
					dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.UserTypes, null));

				if (StaticData.Tables[TN.UserFunctions] == null)
					dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.UserFunctions, null));

				if (StaticData.Tables[TN.Title] == null)
					dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.Title, new string[] { "TTL", "L" }));

				if (StaticData.Tables[TN.CustomerRelationship] == null)
					dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.CustomerRelationship, new string[] { "LCT", "L" }));

				if (StaticData.Tables[TN.RefRelation] == null)
					dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.RefRelation, new string[] { "RL1", "L" }));

				if (StaticData.Tables[TN.AddressType] == null)
					dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.AddressType, null));

				if (StaticData.Tables[TN.IDSelection] == null)
					dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.IDSelection, new string[] { "IT1", "L" }));

				if (StaticData.Tables[TN.MaritalStatus] == null)
					dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.MaritalStatus, new string[] { "MS1", "L" }));

				if (StaticData.Tables[TN.EthnicGroup] == null)
					dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.EthnicGroup, new string[] { "EG1", "L" }));

				if (StaticData.Tables[TN.Nationality] == null)
					dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.Nationality, new string[] { "NA2", "L" }));

				if (StaticData.Tables[TN.PropertyType] == null)
					dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.PropertyType, new string[] { "PT1", "L" }));

				if (StaticData.Tables[TN.DeliveryArea] == null)
					dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.DeliveryArea, new string[] { "DDY", "L" }));

				if (StaticData.Tables[TN.ResidentialStatus] == null)
					dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.ResidentialStatus, new string[] { "RS1", "L" }));

				if (StaticData.Tables[TN.SanctionStages] == null)
					dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.SanctionStages,
						new string[] { "PH1", "L" }));

				if (StaticData.Tables[TN.EmploymentStatus] == null)
					dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.EmploymentStatus, new string[] { "ES1", "L" }));

				if (StaticData.Tables[TN.EmployeeTypes] == null)
					dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.EmployeeTypes,
						new string[] { "ET1", "L" }));

				if (StaticData.Tables[TN.AcctSelectionStatus] == null)
					dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.AcctSelectionStatus,
						new string[] { "ASS", "L" }));

				if (StaticData.Tables[TN.AcctSelectionLetter] == null)
					dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.AcctSelectionLetter,
						new string[] { "ASL", "L" }));

				if (StaticData.Tables[TN.Letter] == null)
				{
					dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.Letter,
						new string[] { "LT1", "LT2", "L" }));
				}

				if (StaticData.Tables[TN.AdhocLetter] == null)
					dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.AdhocLetter,
						new string[] { "LT2", "L" }));


				if (StaticData.Tables[TN.AcctSelectionArrears] == null)
					dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.AcctSelectionArrears,
						new string[] { "ASR", "L" }));

				if (StaticData.Tables[TN.AcctSelectionAction] == null)
					dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.AcctSelectionAction,
						new string[] { "ASA", "L" }));

				if (StaticData.Tables[TN.SelectionAction] == null)
					dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.SelectionAction,
						new string[] { "FUP", "L" }));

				if (StaticData.Tables[TN.AcctSelectionCodes] == null)
					dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.AcctSelectionCodes,
						new string[] { "ASC", "L" }));

				if (StaticData.Tables[TN.AcctSelectionAllocation] == null)
					dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.AcctSelectionAllocation,
						new string[] { "AST", "L" }));

				if (StaticData.Tables[TN.AcctSelectionPoints] == null)
					dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.AcctSelectionPoints,
						new string[] { "ASP", "L" }));

				if (StaticData.Tables[TN.Occupation] == null)
					dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.Occupation, new string[] { "WT1", "L" }));

				if (StaticData.Tables[TN.PayFrequency] == null)
					dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.PayFrequency, new string[] { "PF1", "L" }));

				if (StaticData.Tables[TN.Bank] == null)
					dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.Bank, null));

				if (StaticData.Tables[TN.BankAccountType] == null)
					dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.BankAccountType, new string[] { "BA2", "L" }));

				if (StaticData.Tables[TN.DDDueDate] == null)
					dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.DDDueDate, null));

				if (StaticData.Tables[TN.ReferralCodes] == null)
					dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.ReferralCodes, new string[] { "SN1", "L" }));

				if (StaticData.Tables[TN.Countries] == null)
					dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.Countries, new string[] { "CTY", "L" }));

				if (StaticData.Tables[TN.ProductCategories] == null)
					dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.ProductCategories, null));

				if (StaticData.Tables[TN.ProofOfAddress] == null)
					dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.ProofOfAddress, new string[] { "PAD", "L" }));

				if (StaticData.Tables[TN.ProofOfID] == null)
					dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.ProofOfID, new string[] { "PID", "L" }));

				if (StaticData.Tables[TN.ProofOfIncome] == null)
					dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.ProofOfIncome, new string[] { "PIN", "L" }));

				if (StaticData.Tables[TN.AccountFlags] == null)
					dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.AccountFlags, new string[] { "PH2", "L" }));

				if (StaticData.Tables[TN.InstantCreditFlags] == null)
					dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.InstantCreditFlags, new string[] { "ICF", "L" }));

				if (StaticData.Tables[TN.PayMethod] == null)
					dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.PayMethod, new string[] { "FPM", "L" }));

				if (StaticData.Tables[TN.CreditCard] == null)
					dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.CreditCard, new string[] { CAT.CreditCardType, "L" }));

				if (StaticData.Tables[TN.Actions] == null)
					dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.Actions,
						new string[] { "FUP", "L" }));

				if (StaticData.Tables[TN.Gender] == null)
					dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.Gender, new string[] { "GEN", "L" }));

				if (StaticData.Tables[TN.ReturnReasons] == null)
					dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.ReturnReasons, new string[] { "FPN", "L" }));

				if (StaticData.Tables[TN.Deposits] == null)
					dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.Deposits, null));

				if (StaticData.Tables[TN.NonDeposits] == null)
					dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.NonDeposits, null));

				if (StaticData.Tables[TN.CardPrintType] == null)
					dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.CardPrintType, new string[] { CAT.CardPrintType, "L" }));

				if (StaticData.Tables[TN.CorrectionReasons] == null)
					dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.CorrectionReasons, new string[] { "FT1", "FT2", "L" }));

				if (StaticData.Tables[TN.RefundReasons] == null)
					dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.RefundReasons, new string[] { "RF1", "RF2", "L" }));

				if (StaticData.Tables[TN.CancelReasons] == null)
					dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.CancelReasons, new string[] { "CN2", "L" }));

				if (StaticData.Tables[TN.GeneralTransactionReasons] == null)
					dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.GeneralTransactionReasons, new string[] { "RC1", "RC2", "L" }));

				if (StaticData.Tables[TN.GeneralTransactions] == null)
					dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.GeneralTransactions, null));

				if (StaticData.Tables[TN.TransferReasons] == null)
					dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.TransferReasons, new string[] { "XRS", "L" }));

				if (StaticData.Tables[TN.GiftVoucherOther] == null)
					dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.GiftVoucherOther, new string[] { "VCCO", "L" }));

				if (StaticData.Tables[TN.CountryParameterCategories] == null)
					dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.CountryParameterCategories, new string[] { "CMC", "L" }));

				if (StaticData.Tables[TN.TaxTypes] == null)
					dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.TaxTypes, new string[] { "TTC", "L" }));

				if (StaticData.Tables[TN.TaxInvoiceFormats] == null)
					dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.TaxInvoiceFormats, new string[] { "TIF", "L" }));

				if (StaticData.Tables[TN.ServicePercentFormats] == null)
					dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.ServicePercentFormats, new string[] { "SPF", "L" }));

				if (StaticData.Tables[TN.CashDrawerReasons] == null)
					dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.CashDrawerReasons, new string[] { "CTO", "L" }));

				if (StaticData.Tables[TN.CustomerIdFormats] == null)
					dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.CustomerIdFormats, new string[] { "ICN", "L" }));

				//CR 866 New tables 
				if (StaticData.Tables[TN.TransportTypes] == null)
					dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.TransportTypes, new string[] { "TPT", "L" }));

				if (StaticData.Tables[TN.EducationLevels] == null)
					dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.EducationLevels, new string[] { "EDU", "L" }));

				//CR 866b New tables 
				if (StaticData.Tables[TN.Industries] == null)
					dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.Industries, new string[] { "IND", "L" }));

				if (StaticData.Tables[TN.Organisations] == null)
					dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.Organisations, new string[] { "ORG", "L" }));

				if (StaticData.Tables[TN.JobTitles] == null)
					dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.JobTitles, new string[] { "JBT", "L" }));

				//End CR 866

				if (StaticData.Tables[TN.ScoreCards] == null) // SC CR 1034 22/02/10
					dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.ScoreCards, new string[] { "SCT", "L" }));  //CR1034 SC Load drop down for scorecard types. 

				if (StaticData.Tables[TN.DepositWaiver_Category] == null)
					dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.DepositWaiver_Category, new string[] { "WDC", "L" }));

				if (StaticData.Tables[TN.DepositWaiver_Level] == null)
					dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.DepositWaiver_Level, new string[] { "WDL", "L" }));

				if (StaticData.Tables[TN.DepositWaiver_Product] == null)
					dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.DepositWaiver_Product, new string[] { "WDP", "L" }));

				if (StaticData.Tables[TN.DepositWaiver_SubClass] == null)
					dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.DepositWaiver_SubClass, new string[] { "WDS", "L" }));   //IP - 27/07/11 - RI - #4415

				if (StaticData.Tables[TN.BandLimitChange] == null)
					dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.BandLimitChange, new string[] { "BLC", "L" }));

				if (StaticData.Tables[TN.TermsTypeBandList] == null)
					dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.TermsTypeBandList, null));
				// Reassign Technician reasons      jec 06/01/11
				if (StaticData.Tables[TN.ReassignReason] == null)
					dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.ReassignReason, new string[] { CAT.ReassignReason, "L" }));

				//IP - 06/07/11 - CR1254 - RI - #4167
				if (StaticData.Tables[TN.RIInterfaceOptions] == null)
					dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.RIInterfaceOptions, new string[] { "RIO", "L" }));

				//CR1232 jec 19/09/11
				if (StaticData.Tables[TN.CashLoanAccountTypes] == null)
					dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.CashLoanAccountTypes, new string[] { "CLAT", "L" }));

				if (Country[CountryParameterNames.LoyaltyScheme] != null && (bool)Country[CountryParameterNames.LoyaltyScheme])
				{
					if (LoyaltyDropStatic.LoyatlyDrop == null || LoyaltyDropStatic.LoyatlyDrop.Tables.Count == 0)
					{
						LoyaltyDropStatic.LoyatlyDrop = StaticDataManager.GetLoyaltyDropData();
					}
				}

				StaticDataManager.BeginGetDropDownData(dropDowns, cb, StaticDataManager);

				InitialisePrinting();

			}
			catch (Exception ex)
			{
				Catch(ex, "InitialiseStaticData");
			}
			finally
			{
				StopWait();
			}
		}

		private void StaticDataCallback(IAsyncResult ar)
		{
			try
			{
				Wait();

				WStaticDataManager sdm = (WStaticDataManager)ar.AsyncState;
				DataSet ds = sdm.EndGetDropDownData(ar, out Error);
				if (Error.Length > 0)
					ShowError(Error);
				else
				{
					foreach (DataTable dt in ds.Tables)
						StaticData.Tables[dt.TableName] = dt;
				}
			}
			catch (Exception ex)
			{
				Catch(ex, "StaticDataCallback");
			}
			finally
			{
				StopWait();
			}
		}

		/// <summary>
		/// If the application exits for any reason, we must release any account
		/// locks the user has as a precaution in case the application
		/// crashes (perish the thought).
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnApplicationExit(object sender, System.EventArgs e)
		{
			try
			{
				Closed = true;
				//Make sure that the credentials have been set, otherwise
				//we'll get a SOAPException
				if (Credential.User != null && Credential.Password != null)
				{
					AccountManager.UnlockAccount("", Credential.UserId, out Error);
					if (Error.Length > 0)
						ShowError(Error);

					AccountManager.UnlockItem(Credential.UserId, out Error);
					if (Error.Length > 0)
						ShowError(Error);

					Login.LogOff(Environment.MachineName, Credential.UserId, Credential.User);

					/* unlock the deposit screen for the logged in branch
					 * just in case the application has been closed when it's open */
					if (Config.BranchCode.Length > 0)
					{
						PaymentManager.UnLockDepositScreen(Convert.ToInt16(Config.BranchCode), out Error);
						if (Error.Length > 0)
							ShowError(Error);
					}
				}
				Config.SaveRecent(Assembly.GetExecutingAssembly());
			}
			catch (Exception ex)
			{
				Catch(ex, Function);
			}
		}

		private void HashMenus()
		{
			dynamicMenus[this.Name + ":menuConfig"] = this.menuConfig;
			dynamicMenus[this.Name + ":menuAccount"] = this.menuAccount;
			dynamicMenus[this.Name + ":menuNewAccount"] = this.menuNewAccount;
			dynamicMenus[this.Name + ":menuAccountDetails"] = this.menuAccountDetails;
			dynamicMenus[this.Name + ":menuAccountRevise"] = this.menuAccountRevise;
			dynamicMenus[this.Name + ":menuAddAcctCodes"] = this.menuAddAcctCodes;
			dynamicMenus[this.Name + ":menuCustomer"] = this.menuCustomer;
			dynamicMenus[this.Name + ":menuAddCustCodes"] = this.menuAddCustCodes;
			dynamicMenus[this.Name + ":menuCustomerSearch"] = this.menuCustomerSearch;
			dynamicMenus[this.Name + ":menuSysMaint"] = this.menuSysMaint;
			dynamicMenus[this.Name + ":menuCollections"] = this.menuCollections;
			dynamicMenus[this.Name + ":menuCustomiseMenus"] = this.menuCustomiseMenus;
			dynamicMenus[this.Name + ":menuManualSale"] = this.menuManualSale;
			dynamicMenus[this.Name + ":menuMandatory"] = this.menuMandatory;
			dynamicMenus[this.Name + ":menuNewCustomer"] = this.menuNewCustomer;
			dynamicMenus[this.Name + ":menuScoringMatrix"] = this.menuScoringMatrix;
			dynamicMenus[this.Name + ":menuDepenSpendFactor"] = this.menuDepenSpendFactor;
            dynamicMenus[this.Name + ":menuApplicantSpendFactor"] = this.menuApplicantSpendFactor;
			dynamicMenus[this.Name + ":menuScoringRules"] = this.menuScoringRules;
			dynamicMenus[this.Name + ":menuTTMatrix"] = this.menuTTMatrix;
			dynamicMenus[this.Name + ":menuCredit"] = this.menuCredit;
			dynamicMenus[this.Name + ":menuIncomplete"] = this.menuIncomplete;
			dynamicMenus[this.Name + ":menuAuthoriseDelivery"] = this.menuAuthoriseDelivery;
			dynamicMenus[this.Name + ":menuPaidAndTaken"] = this.menuPaidAndTaken;
			dynamicMenus[this.Name + ":menuTranslation"] = this.menuTranslation;
			dynamicMenus[this.Name + ":menuGoodsReturn"] = this.menuGoodsReturn;
			dynamicMenus[this.Name + ":menuAuthoriseIC"] = this.menuAuthoriseIC;
			dynamicMenus[this.Name + ":menuTransaction"] = this.menuTransaction;
			dynamicMenus[this.Name + ":menuPayment"] = this.menuPayment;
			//dynamicMenus[this.Name + ":menuPrintDeliveryNote"] = this.menuPrintDeliveryNote;
			dynamicMenus[this.Name + ":menuWarehouse"] = this.menuWarehouse;
			////dynamicMenus[this.Name + ":menuImmediateDelivery"]     = this.menuImmediateDelivery;
			//dynamicMenus[this.Name + ":menuDeliveryNotification"]  = this.menuDeliveryNotification;
			//dynamicMenus[this.Name + ":menuPrintCollectionNote"] = this.menuPrintCollectionNote;
			dynamicMenus[this.Name + ":menuScreenTranslation"] = this.menuScreenTranslation;
			dynamicMenus[this.Name + ":menuFollowUp"] = this.menuFollowUp;
			dynamicMenus[this.Name + ":menuTelephoneAction"] = this.menuTelephoneAction;
			dynamicMenus[this.Name + ":menuBailiffReview"] = this.menuBailiffReview;
			dynamicMenus[this.Name + ":menuSearchCashAndGo"] = this.menuSearchCashAndGo;
			dynamicMenus[this.Name + ":menuOneForOneReplacement"] = this.menuOneForOneReplacement;
			dynamicMenus[this.Name + ":menuCodeMaintenance"] = this.menuCodeMaintenance;
			dynamicMenus[this.Name + ":menuEmployeeMaintenance"] = this.menuEmployeeMaintenance;
			dynamicMenus[this.Name + ":menuBranch"] = this.menuBranch;
			//dynamicMenus[this.Name + ":menuRePrintDeliveryNote"]   = this.menuRePrintDeliveryNote;
			dynamicMenus[this.Name + ":menuImageManagement"] = this.menuImageManagement;
			dynamicMenus[this.Name + ":menuCashier"] = this.menuCashier;
			dynamicMenus[this.Name + ":menuTransferTransaction"] = this.menuTransferTransaction;
			dynamicMenus[this.Name + ":menuJournal"] = this.menuJournal;
			dynamicMenus[this.Name + ":menuCorrection"] = this.menuCorrection;
			dynamicMenus[this.Name + ":menuReturnCheque"] = this.menuReturnCheque;
			dynamicMenus[this.Name + ":menuGeneralTransactions"] = this.menuGeneralTransactions;
			dynamicMenus[this.Name + ":menuDDMandate"] = this.menuDDMandate;
			dynamicMenus[this.Name + ":menuDDRejection"] = this.menuDDRejection;
			dynamicMenus[this.Name + ":menuDDPaymentExtra"] = this.menuDDPaymentExtra;
			dynamicMenus[this.Name + ":menuCancel"] = this.menuCancel;
			dynamicMenus[this.Name + ":menuTransTypeMaintenance"] = this.menuTransTypeMaintenance;
			dynamicMenus[this.Name + ":menuLocationEnquiry"] = this.menuLocationEnquiry;
			dynamicMenus[this.Name + ":menuProductEnquiry"] = this.menuProductEnquiry;
			dynamicMenus[this.Name + ":menuStatusCode"] = this.menuStatusCode;
			dynamicMenus[this.Name + ":menuWOReview"] = this.menuWOReview;
			dynamicMenus[this.Name + ":menuExchangeRates"] = this.menuExchangeRates;
			dynamicMenus[this.Name + ":menuSellGiftVoucher"] = this.menuSellGiftVoucher;
			dynamicMenus[this.Name + ":menuDeposits"] = this.menuDeposits;
			dynamicMenus[this.Name + ":menuEOD"] = this.menuEOD;
			dynamicMenus[this.Name + ":menuCountryMaintenance"] = this.menuCountryMaintenance;
			dynamicMenus[this.Name + ":openTill"] = this.openTill;
			dynamicMenus[this.Name + ":menuCashAndGoReturn"] = this.menuCashAndGoReturn;
			dynamicMenus[this.Name + ":menuUpdateDateDue"] = this.menuUpdateDateDue;
			dynamicMenus[this.Name + ":menuDisbursements"] = this.menuDisbursements;
			dynamicMenus[this.Name + ":menuInterfaceReport"] = this.menuInterfaceReport;
			dynamicMenus[this.Name + ":menuTermsType"] = this.menuTermsType;
            dynamicMenus[this.Name + ":menuMmiMatrix"] = this.menuMmiMatrix;
            dynamicMenus[this.Name + ":menuDeliveryArea"] = this.menuDeliveryArea;
			dynamicMenus[this.Name + ":menuNumberGeneration"] = this.menuNumberGeneration;
			dynamicMenus[this.Name + ":menuMonitorBookings"] = this.menuMonitorBookings;
			dynamicMenus[this.Name + ":menuFinTransQuery"] = this.menuFinTransQuery;
			//dynamicMenus[this.Name + ":menuMonitorDeliveries"]     = this.menuMonitorDeliveries;
			dynamicMenus[this.Name + ":menuReverseCancel"] = this.menuReverseCancel;
			dynamicMenus[this.Name + ":menuCashierByBranch"] = this.menuCashierByBranch;
			dynamicMenus[this.Name + ":menuWarrantyRenewals"] = this.menuWarrantyRenewals;
			dynamicMenus[this.Name + ":menuReports"] = this.menuReports;
			//dynamicMenus[this.Name + ":menuOrdersForDelivery"]     = this.menuOrdersForDelivery;
			dynamicMenus[this.Name + ":menuReprintActionSheet"] = this.menuReprintActionSheet;
			dynamicMenus[this.Name + ":menuRedeliverReposs"] = this.menuRedeliverReposs;
			dynamicMenus[this.Name + ":menuTempReceiptInvest"] = this.menuTempReceiptInvest;
			dynamicMenus[this.Name + ":menuRebateCalculation"] = this.menuRebateCalculation;
			//dynamicMenus[this.Name + ":menuPrintDelSched"]         = this.menuPrintDelSched;
			//dynamicMenus[this.Name + ":menuDelSchedule"]           = this.menuDelSchedule;
			//dynamicMenus[this.Name + ":menuTransportMaint"]        = this.menuTransportMaint;
			dynamicMenus[this.Name + ":menuUnpaidAccounts"] = this.menuUnpaidAccounts;
			//dynamicMenus[this.Name + ":menuAmendReprintPicklist"]  = this.menuAmendReprintPicklist;
			dynamicMenus[this.Name + ":menuAcctNoCtrl"] = this.menuAcctNoCtrl;
			dynamicMenus[this.Name + ":menuCancelCollectionNotes"] = this.menuCancelCollectionNotes;
			dynamicMenus[this.Name + ":menuFactoringReports"] = this.menuFactoringReports;
			dynamicMenus[this.Name + ":menuRebateReport"] = this.menuRebateReport;
			dynamicMenus[this.Name + ":menuCommissionMaint"] = this.menuCommissionMaint;
			dynamicMenus[this.Name + ":menuCalcBailCommission"] = this.menuCalcBailCommission;
			dynamicMenus[this.Name + ":menuReprintBailCommn"] = this.menuReprintBailCommn;
			//dynamicMenus[this.Name + ":menuTransportSchedule"]     = this.menuTransportSchedule;
			dynamicMenus[this.Name + ":menuChangeOrderDetails"] = this.menuChangeOrderDetails;
			//dynamicMenus[this.Name + ":menuScheduleChanges"]       = this.menuScheduleChanges;
			dynamicMenus[this.Name + ":menuEODInterface"] = this.menuEODInterface;
			dynamicMenus[this.Name + ":menuCustomerMailing"] = this.menuCustomerMailing;
			dynamicMenus[this.Name + ":menuPaymentFileDefn"] = this.menuPaymentFileDefn;	//jec 
			dynamicMenus[this.Name + ":menuSumryUpdControl"] = this.menuSumryUpdControl;	//jec 68073
			dynamicMenus[this.Name + ":menuAccountStatus"] = this.menuAccountStatus; //IP - 25/04/08
			dynamicMenus[this.Name + ":menuProvisions"] = this.menuProvisions;
			dynamicMenus[this.Name + ":menuStoreCardRateSetup"] = this.menuStoreCardRateSetup;
			dynamicMenus[this.Name + ":menuViewStoreCard"] = this.menuViewStoreCard;
            dynamicMenus[this.Name + ":RePrintInvoice"] = this.RePrintInvoice;


            //dynamicMenus[this.Name + ":menuService"] = this.menuService;
            //dynamicMenus[this.Name + ":menuServiceRequest"] = this.menuServiceRequest;
            //dynamicMenus[this.Name + ":menuServiceSearch"] = this.menuServiceRequestSearch;
            ////dynamicMenus[this.Name + ":menuServiceRequestAudit"]        = this.menuServiceRequestAudit;
            dynamicMenus[this.Name + ":menuCustomerInteraction"] = this.menuServiceCustomerInteraction;
			//dynamicMenus[this.Name + ":menuTechMaintenance"] = this.menuServiceTechMaintenance;
			//dynamicMenus[this.Name + ":menuTechDiary"] = this.menuServiceTechDiary;
			//dynamicMenus[this.Name + ":menuServicePriceMatrix"] = this.menuServicePriceMatrix;
			//dynamicMenus[this.Name + ":menuServiceChargeToAuthorisation"] = this.menuServiceChargeToAuthorisation;
			//dynamicMenus[this.Name + ":menuTechPayment"] = this.menuServiceTechPayment;
			//dynamicMenus[this.Name + ":menuOutstanding"] = this.menuServiceOutstanding;
			//dynamicMenus[this.Name + ":menuBatchPrint"] = this.menuServiceBatchPrint;
			//dynamicMenus[this.Name + ":menuServiceProgressReport"] = this.menuServiceProgressReport;
			//dynamicMenus[this.Name + ":menuServiceFailureReport"] = this.menuServiceFailureReport;
			//dynamicMenus[this.Name + ":menuServiceClaimsReport"] = this.menuServiceClaimsReport;

			dynamicMenus[this.Name + ":menuPrizeVouchers"] = this.menuPrizeVouchers;
			dynamicMenus[this.Name + ":menuCommissionsSetUp"] = this.menuCommissionsSetUp; //cr36 jec 25/08/06
			dynamicMenus[this.Name + ":menuInstantReplacement"] = this.menuInstantReplacement;
			dynamicMenus[this.Name + ":menuCommissionEnquiry"] = this.menuCommissionEnquiry; //cr36 jec 17/10/06
			dynamicMenus[this.Name + ":menuCommissionReport"] = this.menuCommissionReport; // jec 12/06/08
			dynamicMenus[this.Name + ":menuEPOS"] = this.menuEPOS; // jec 13/06/08
			dynamicMenus[this.Name + ":menuWarrantyReport"] = this.menuWarrantyReport;
			dynamicMenus[this.Name + ":menuLocateHelp"] = this.menuLocateHelp;
			dynamicMenus[this.Name + ":menuAddBank"] = this.menuAddBank;
			dynamicMenus[this.Name + ":menuBehavioural"] = this.menuBehavioural;
			// CR852 Collections
			dynamicMenus[this.Name + ":menuSMS"] = this.menuSMS;
			dynamicMenus[this.Name + ":menuWorkLists"] = this.menuWorkLists;
			dynamicMenus[this.Name + ":menuStrategyConfiguration"] = this.menuStrategyConfiguration;
			dynamicMenus[this.Name + ":menuLetterMerge"] = this.menuLetterMerge; //NM CR976
			dynamicMenus[this.Name + ":menuZoneAutomation"] = this.menuZoneAutomation; //NM CR976
			dynamicMenus[this.Name + ":menuTallymanExtract"] = this.menuTallymanExtract; //JC CR1072
			dynamicMenus[this.Name + ":menuNonStock"] = this.menuNonStock; //jec CR1094 08/12/10
			//dynamicMenus[this.Name + ":menuWarrantyReturnCodes"] = this.menuWarrantyReturnCodes; //jec CR1094 15/12/10
			dynamicMenus[this.Name + ":menuPendingInstallations"] = this.menuPendingInstallations;
			dynamicMenus[this.Name + ":menuInstBookingPrint"] = this.menuInstBookingPrint;
			dynamicMenus[this.Name + ":menuInstManagement"] = this.menuInstManagement;
			dynamicMenus[this.Name + ":menuAllEODTasks"] = this.menuAllEODTasks;
			dynamicMenus[this.Name + ":menuScoring"] = this.menuScoring;
			dynamicMenus[this.Name + ":menuSpendFactor"] = this.menuSpendFactor;
			//dynamicMenus[this.Name + ":menuProductAssociations"] = this.menuProductAssociations;    // jec 15/06/11
			dynamicMenus[this.Name + ":menuTestMode"] = this.menuTestMode;                           // jec 26/07/11
			dynamicMenus[this.Name + ":menuCashLoanApplication"] = this.menuCashLoanApplication;                           // CR1232 jec 14/09/11
			dynamicMenus[this.Name + ":menuFailedDeliveriesCollections"] = this.menuFailedDeliveriesCollections;                           // #10221
			dynamicMenus[this.Name + ":menuService"] = this.menuService;
			dynamicMenus[this.Name + ":menuBERRep"] = this.menuBERRep;
		dynamicMenus[this.Name + ":menuOnlineProductSearch"] = this.menuOnlineProductSearch;                           // #13889
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

		static Mutex CosacsMutex;

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.menuFile = new Crownwood.Magic.Menus.MenuCommand();
            this.menuConfig = new Crownwood.Magic.Menus.MenuCommand();
            this.menuIcons = new System.Windows.Forms.ImageList(this.components);
            this.menuLogOff = new Crownwood.Magic.Menus.MenuCommand();
            this.menuVersion = new Crownwood.Magic.Menus.MenuCommand();
            this.menuExit = new Crownwood.Magic.Menus.MenuCommand();
            this.menuChangePassword = new Crownwood.Magic.Menus.MenuCommand();
            this.menuTestMode = new Crownwood.Magic.Menus.MenuCommand();
            this.menuAccount = new Crownwood.Magic.Menus.MenuCommand();
            this.menuNewAccount = new Crownwood.Magic.Menus.MenuCommand();
            this.menuManualSale = new Crownwood.Magic.Menus.MenuCommand();
            this.menuAccountDetails = new Crownwood.Magic.Menus.MenuCommand();
            this.menuAccountRevise = new Crownwood.Magic.Menus.MenuCommand();
            this.menuAccountStatus = new Crownwood.Magic.Menus.MenuCommand();
            this.menuAddAcctCodes = new Crownwood.Magic.Menus.MenuCommand();
            this.menuPaidAndTaken = new Crownwood.Magic.Menus.MenuCommand();
            this.menuSearchCashAndGo = new Crownwood.Magic.Menus.MenuCommand();
            this.menuCashAndGoReturn = new Crownwood.Magic.Menus.MenuCommand();
            this.menuNumberGeneration = new Crownwood.Magic.Menus.MenuCommand();
            this.menuWarrantyRenewals = new Crownwood.Magic.Menus.MenuCommand();
            this.menuUnpaidAccounts = new Crownwood.Magic.Menus.MenuCommand();
            this.menuChangeOrderDetails = new Crownwood.Magic.Menus.MenuCommand();
            this.menuInstantReplacement = new Crownwood.Magic.Menus.MenuCommand();
            this.menuEPOS = new Crownwood.Magic.Menus.MenuCommand();
            this.menuCustomer = new Crownwood.Magic.Menus.MenuCommand();
            this.menuCustomerSearch = new Crownwood.Magic.Menus.MenuCommand();
            this.menuAddCustCodes = new Crownwood.Magic.Menus.MenuCommand();
            this.menuNewCustomer = new Crownwood.Magic.Menus.MenuCommand();
            this.menuAuthoriseDelivery = new Crownwood.Magic.Menus.MenuCommand();
            this.menuAuthoriseIC = new Crownwood.Magic.Menus.MenuCommand();
            this.menuGoodsReturn = new Crownwood.Magic.Menus.MenuCommand();
            this.menuOneForOneReplacement = new Crownwood.Magic.Menus.MenuCommand();
            this.menuImageManagement = new Crownwood.Magic.Menus.MenuCommand();
            this.menuPrizeVouchers = new Crownwood.Magic.Menus.MenuCommand();
            this.menuServiceCustomerInteraction = new Crownwood.Magic.Menus.MenuCommand();
            this.menuViewStoreCard = new Crownwood.Magic.Menus.MenuCommand();
            this.menuCashLoanApplication = new Crownwood.Magic.Menus.MenuCommand();
            this.menuDuplicateCustomers = new Crownwood.Magic.Menus.MenuCommand();
            this.menuCashLoanBankTransfer = new Crownwood.Magic.Menus.MenuCommand();
            this.menuCredit = new Crownwood.Magic.Menus.MenuCommand();
            this.menuIncomplete = new Crownwood.Magic.Menus.MenuCommand();
            this.menuFollowUp = new Crownwood.Magic.Menus.MenuCommand();
            this.menuBailiffReview = new Crownwood.Magic.Menus.MenuCommand();
            this.menuTelephoneAction = new Crownwood.Magic.Menus.MenuCommand();
            this.menuCancel = new Crownwood.Magic.Menus.MenuCommand();
            this.menuDDMandate = new Crownwood.Magic.Menus.MenuCommand();
            this.menuDDRejection = new Crownwood.Magic.Menus.MenuCommand();
            this.menuDDPaymentExtra = new Crownwood.Magic.Menus.MenuCommand();
            this.menuWOReview = new Crownwood.Magic.Menus.MenuCommand();
            this.menuStatusCode = new Crownwood.Magic.Menus.MenuCommand();
            this.menuUpdateDateDue = new Crownwood.Magic.Menus.MenuCommand();
            this.menuReverseCancel = new Crownwood.Magic.Menus.MenuCommand();
            this.menuTempReceiptInvest = new Crownwood.Magic.Menus.MenuCommand();
            this.menuReprintActionSheet = new Crownwood.Magic.Menus.MenuCommand();
            this.menuCalcBailCommission = new Crownwood.Magic.Menus.MenuCommand();
            this.menuReprintBailCommn = new Crownwood.Magic.Menus.MenuCommand();
            this.menuTransaction = new Crownwood.Magic.Menus.MenuCommand();
            this.menuPayment = new Crownwood.Magic.Menus.MenuCommand();
            this.menuCashier = new Crownwood.Magic.Menus.MenuCommand();
            this.menuDeposits = new Crownwood.Magic.Menus.MenuCommand();
            this.menuDisbursements = new Crownwood.Magic.Menus.MenuCommand();
            this.menuTransferTransaction = new Crownwood.Magic.Menus.MenuCommand();
            this.RePrintInvoice = new Crownwood.Magic.Menus.MenuCommand();
            this.menuJournal = new Crownwood.Magic.Menus.MenuCommand();
            this.menuCorrection = new Crownwood.Magic.Menus.MenuCommand();
            this.menuReturnCheque = new Crownwood.Magic.Menus.MenuCommand();
            this.menuGeneralTransactions = new Crownwood.Magic.Menus.MenuCommand();
            this.menuSellGiftVoucher = new Crownwood.Magic.Menus.MenuCommand();
            this.menuCashierByBranch = new Crownwood.Magic.Menus.MenuCommand();
            this.menuFinTransQuery = new Crownwood.Magic.Menus.MenuCommand();
            this.menuWarehouse = new Crownwood.Magic.Menus.MenuCommand();
            this.menuLocationEnquiry = new Crownwood.Magic.Menus.MenuCommand();
            this.menuProductEnquiry = new Crownwood.Magic.Menus.MenuCommand();
            this.menuRedeliverReposs = new Crownwood.Magic.Menus.MenuCommand();
            this.menuCancelCollectionNotes = new Crownwood.Magic.Menus.MenuCommand();
            this.menuFailedDeliveriesCollections = new Crownwood.Magic.Menus.MenuCommand();
            this.menuOnlineProductSearch = new Crownwood.Magic.Menus.MenuCommand();
            this.menuSysMaint = new Crownwood.Magic.Menus.MenuCommand();
            this.menuBranch = new Crownwood.Magic.Menus.MenuCommand();
            this.menuCodeMaintenance = new Crownwood.Magic.Menus.MenuCommand();
            this.menuEmployeeMaintenance = new Crownwood.Magic.Menus.MenuCommand();
            this.menuCustomiseMenus = new Crownwood.Magic.Menus.MenuCommand();
            this.menuMandatory = new Crownwood.Magic.Menus.MenuCommand();
            this.menuScoring = new Crownwood.Magic.Menus.MenuCommand();
			this.menuSpendFactor = new Crownwood.Magic.Menus.MenuCommand();
            this.menuScoringRules = new Crownwood.Magic.Menus.MenuCommand();
            this.menuScoringMatrix = new Crownwood.Magic.Menus.MenuCommand();
			this.menuDepenSpendFactor = new Crownwood.Magic.Menus.MenuCommand();
            this.menuApplicantSpendFactor = new Crownwood.Magic.Menus.MenuCommand();
            this.menuTTMatrix = new Crownwood.Magic.Menus.MenuCommand();
            this.menuTranslation = new Crownwood.Magic.Menus.MenuCommand();
            this.menuScreenTranslation = new Crownwood.Magic.Menus.MenuCommand();
            this.menuTransTypeMaintenance = new Crownwood.Magic.Menus.MenuCommand();
            this.menuExchangeRates = new Crownwood.Magic.Menus.MenuCommand();
            this.menuCountryMaintenance = new Crownwood.Magic.Menus.MenuCommand();
            this.menuTermsType = new Crownwood.Magic.Menus.MenuCommand();
            this.menuMmiMatrix = new Crownwood.Magic.Menus.MenuCommand();
            this.menuProvisions = new Crownwood.Magic.Menus.MenuCommand();
            this.menuDeliveryArea = new Crownwood.Magic.Menus.MenuCommand();
            this.menuAcctNoCtrl = new Crownwood.Magic.Menus.MenuCommand();
            this.menuAllEODTasks = new Crownwood.Magic.Menus.MenuCommand();
            this.menuEOD = new Crownwood.Magic.Menus.MenuCommand();
            this.menuEODInterface = new Crownwood.Magic.Menus.MenuCommand();
            this.menuTallymanExtract = new Crownwood.Magic.Menus.MenuCommand();
            this.menuStoreCardBatchPrint = new Crownwood.Magic.Menus.MenuCommand();
            this.menuPaymentFileDefn = new Crownwood.Magic.Menus.MenuCommand();
            this.menuCommissionsSetUp = new Crownwood.Magic.Menus.MenuCommand();
            this.menuStoreCardRateSetup = new Crownwood.Magic.Menus.MenuCommand();
            this.menuAddBank = new Crownwood.Magic.Menus.MenuCommand();
            this.menuNonStock = new Crownwood.Magic.Menus.MenuCommand();
            this.menuCollections = new Crownwood.Magic.Menus.MenuCommand();
            this.menuCommissionMaint = new Crownwood.Magic.Menus.MenuCommand();
            this.menuLetterMerge = new Crownwood.Magic.Menus.MenuCommand();
            this.menuZoneAutomation = new Crownwood.Magic.Menus.MenuCommand();
            this.menuWorkLists = new Crownwood.Magic.Menus.MenuCommand();
            this.menuStrategyConfiguration = new Crownwood.Magic.Menus.MenuCommand();
            this.menuSMS = new Crownwood.Magic.Menus.MenuCommand();
            this.menuReports = new Crownwood.Magic.Menus.MenuCommand();
            this.menuBehavioural = new Crownwood.Magic.Menus.MenuCommand();
            this.menuCustomerMailing = new Crownwood.Magic.Menus.MenuCommand();
            this.menuFactoringReports = new Crownwood.Magic.Menus.MenuCommand();
            this.menuInterfaceReport = new Crownwood.Magic.Menus.MenuCommand();
            this.menuMonitorBookings = new Crownwood.Magic.Menus.MenuCommand();
            this.menuRebateCalculation = new Crownwood.Magic.Menus.MenuCommand();
            this.menuRebateReport = new Crownwood.Magic.Menus.MenuCommand();
            this.menuCommissionEnquiry = new Crownwood.Magic.Menus.MenuCommand();
            this.menuCommissionReport = new Crownwood.Magic.Menus.MenuCommand();
            this.menuSumryUpdControl = new Crownwood.Magic.Menus.MenuCommand();
            this.menuWarrantyReport = new Crownwood.Magic.Menus.MenuCommand();
            this.menuSalesCommEnquiry = new Crownwood.Magic.Menus.MenuCommand();
            this.menuSalesCommBranchEnquiry = new Crownwood.Magic.Menus.MenuCommand();
            this.menuLocateHelp = new Crownwood.Magic.Menus.MenuCommand();
            this.menuAbout = new Crownwood.Magic.Menus.MenuCommand();
            this.menuPendingInstallations = new Crownwood.Magic.Menus.MenuCommand();
            this.menuInstBookingPrint = new Crownwood.Magic.Menus.MenuCommand();
            this.menuInstManagement = new Crownwood.Magic.Menus.MenuCommand();
            this.menuStatements = new Crownwood.Magic.Menus.MenuCommand();
            this.sqlCommand1 = new System.Data.SqlClient.SqlCommand();
            this.menuCommand2 = new Crownwood.Magic.Menus.MenuCommand();
            this.openTill = new System.Windows.Forms.Control();
            this.menuCommand3 = new Crownwood.Magic.Menus.MenuCommand();
            this.appUpdater1 = new STL.AppUpdater.AppUpdater(this.components);
            this.pbDownloading = new System.Windows.Forms.PictureBox();
            this.lDownloading = new System.Windows.Forms.Label();
            this.menuMain = new Crownwood.Magic.Menus.MenuControl();
            this.menuService = new Crownwood.Magic.Menus.MenuCommand();
            this.menuBERRep = new Crownwood.Magic.Menus.MenuCommand();
            this.MainTabControl = new Crownwood.Magic.Controls.TabControl();
            this.tpLogIn = new Crownwood.Magic.Controls.TabPage();
            this.grpLogInCourts = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.txtPasswordCourts = new System.Windows.Forms.TextBox();
            this.txtUserCourts = new System.Windows.Forms.TextBox();
            this.grpLogIn = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.txtUser = new System.Windows.Forms.TextBox();
            this.pbSplash = new System.Windows.Forms.PictureBox();
            this.statusBar1 = new System.Windows.Forms.StatusBar();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.tbSanction = new STL.PL.SanctionStatus();
            this.PrinterStat = new STL.PL.PrinterStatus();
            ((System.ComponentModel.ISupportInitialize)(this.appUpdater1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbDownloading)).BeginInit();
            this.MainTabControl.SuspendLayout();
            this.tpLogIn.SuspendLayout();
            this.grpLogInCourts.SuspendLayout();
            this.grpLogIn.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbSplash)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // menuFile
            // 
            this.menuFile.Description = "MenuItem";
            this.menuFile.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {
            this.menuConfig,
            this.menuLogOff,
            this.menuVersion,
            this.menuExit,
            this.menuChangePassword,
            this.menuTestMode});
            this.menuFile.Text = "&File";
            // 
            // menuConfig
            // 
            this.menuConfig.Description = "MenuItem";
            this.menuConfig.Enabled = false;
            this.menuConfig.ImageIndex = 0;
            this.menuConfig.ImageList = this.menuIcons;
            this.menuConfig.Text = "Config &File Maintenance";
            this.menuConfig.Visible = false;
            this.menuConfig.Click += new System.EventHandler(this.menuConfig_Click);
            // 
            // menuIcons
            // 
            this.menuIcons.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("menuIcons.ImageStream")));
            this.menuIcons.TransparentColor = System.Drawing.Color.Transparent;
            this.menuIcons.Images.SetKeyName(0, "");
            this.menuIcons.Images.SetKeyName(1, "");
            this.menuIcons.Images.SetKeyName(2, "");
            this.menuIcons.Images.SetKeyName(3, "");
            this.menuIcons.Images.SetKeyName(4, "");
            this.menuIcons.Images.SetKeyName(5, "");
            this.menuIcons.Images.SetKeyName(6, "");
            this.menuIcons.Images.SetKeyName(7, "");
            this.menuIcons.Images.SetKeyName(8, "");
            this.menuIcons.Images.SetKeyName(9, "");
            this.menuIcons.Images.SetKeyName(10, "");
            this.menuIcons.Images.SetKeyName(11, "");
            this.menuIcons.Images.SetKeyName(12, "");
            this.menuIcons.Images.SetKeyName(13, "");
            this.menuIcons.Images.SetKeyName(14, "");
            this.menuIcons.Images.SetKeyName(15, "");
            this.menuIcons.Images.SetKeyName(16, "");
            this.menuIcons.Images.SetKeyName(17, "");
            this.menuIcons.Images.SetKeyName(18, "");
            this.menuIcons.Images.SetKeyName(19, "");
            this.menuIcons.Images.SetKeyName(20, "");
            this.menuIcons.Images.SetKeyName(21, "");
            this.menuIcons.Images.SetKeyName(22, "");
            this.menuIcons.Images.SetKeyName(23, "");
            this.menuIcons.Images.SetKeyName(24, "");
            this.menuIcons.Images.SetKeyName(25, "");
            this.menuIcons.Images.SetKeyName(26, "");
            this.menuIcons.Images.SetKeyName(27, "");
            this.menuIcons.Images.SetKeyName(28, "");
            this.menuIcons.Images.SetKeyName(29, "");
            this.menuIcons.Images.SetKeyName(30, "");
            this.menuIcons.Images.SetKeyName(31, "");
            this.menuIcons.Images.SetKeyName(32, "");
            this.menuIcons.Images.SetKeyName(33, "");
            this.menuIcons.Images.SetKeyName(34, "");
            this.menuIcons.Images.SetKeyName(35, "");
            this.menuIcons.Images.SetKeyName(36, "");
            // 
            // menuLogOff
            // 
            this.menuLogOff.Description = "MenuItem";
            this.menuLogOff.Enabled = false;
            this.menuLogOff.ImageIndex = 2;
            this.menuLogOff.ImageList = this.menuIcons;
            this.menuLogOff.Text = "&Log Off";
            this.menuLogOff.Click += new System.EventHandler(this.menuLogOff_Click);
            // 
            // menuVersion
            // 
            this.menuVersion.Description = "MenuItem";
            this.menuVersion.ImageIndex = 1;
            this.menuVersion.ImageList = this.menuIcons;
            this.menuVersion.Text = "&About Cosacs.NET";
            this.menuVersion.Click += new System.EventHandler(this.menuVersion_Click);
            // 
            // menuExit
            // 
            this.menuExit.Description = "MenuItem";
            this.menuExit.ImageIndex = 3;
            this.menuExit.ImageList = this.menuIcons;
            this.menuExit.Text = "E&xit";
            this.menuExit.Click += new System.EventHandler(this.menuExit_Click);
            // 
            // menuChangePassword
            // 
            this.menuChangePassword.Description = "MenuItem";
            this.menuChangePassword.Text = "Change &Password";
            this.menuChangePassword.Click += new System.EventHandler(this.menuChangePassword_Click_1);
            // 
            // menuTestMode
            // 
            this.menuTestMode.Description = "MenuItem";
            this.menuTestMode.Enabled = false;
            this.menuTestMode.ImageList = this.menuIcons;
            this.menuTestMode.Text = "Start TestMode";
            this.menuTestMode.Visible = false;
            this.menuTestMode.Click += new System.EventHandler(this.menuTestMode_Click);
            // 
            // menuAccount
            // 
            this.menuAccount.Description = "MenuItem";
            this.menuAccount.Enabled = false;
            this.menuAccount.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {
            this.menuNewAccount,
            this.menuManualSale,
            this.menuAccountDetails,
            this.menuAccountRevise,
            this.menuAccountStatus,
            this.menuAddAcctCodes,
            this.menuPaidAndTaken,
            this.menuSearchCashAndGo,
            this.menuCashAndGoReturn,
            this.menuNumberGeneration,
            this.menuWarrantyRenewals,
            this.menuUnpaidAccounts,
            this.menuChangeOrderDetails,
            this.menuInstantReplacement,
            this.menuEPOS});
            this.menuAccount.Text = "&Account";
            this.menuAccount.Visible = false;
            this.menuAccount.Click += new System.EventHandler(this.menuAccount_Click);
            // 
            // menuNewAccount
            // 
            this.menuNewAccount.Description = "MenuItem";
            this.menuNewAccount.Enabled = false;
            this.menuNewAccount.ImageIndex = 4;
            this.menuNewAccount.ImageList = this.menuIcons;
            this.menuNewAccount.Text = "&New Sales Order";
            this.menuNewAccount.Visible = false;
            this.menuNewAccount.Click += new System.EventHandler(this.menuNewAccount_Click);
            // 
            // menuManualSale
            // 
            this.menuManualSale.Description = "MenuItem";
            this.menuManualSale.Enabled = false;
            this.menuManualSale.ImageIndex = 5;
            this.menuManualSale.ImageList = this.menuIcons;
            this.menuManualSale.Text = "&Manual Sale";
            this.menuManualSale.Visible = false;
            this.menuManualSale.Click += new System.EventHandler(this.menuManualSale_Click);
            // 
            // menuAccountDetails
            // 
            this.menuAccountDetails.Description = "MenuItem";
            this.menuAccountDetails.Enabled = false;
            this.menuAccountDetails.ImageIndex = 7;
            this.menuAccountDetails.ImageList = this.menuIcons;
            this.menuAccountDetails.Text = "Account &Search";
            this.menuAccountDetails.Visible = false;
            this.menuAccountDetails.Click += new System.EventHandler(this.menuAccountDetails_Click);
            // 
            // menuAccountRevise
            // 
            this.menuAccountRevise.Description = "MenuItem";
            this.menuAccountRevise.Enabled = false;
            this.menuAccountRevise.ImageIndex = 6;
            this.menuAccountRevise.ImageList = this.menuIcons;
            this.menuAccountRevise.Text = "&Revise Account";
            this.menuAccountRevise.Visible = false;
            this.menuAccountRevise.Click += new System.EventHandler(this.menuAccountRevise_Click);
            // 
            // menuAccountStatus
            // 
            this.menuAccountStatus.Description = "menuAccountStatus";
            this.menuAccountStatus.Enabled = false;
            this.menuAccountStatus.Text = "&AccountStatus";
            this.menuAccountStatus.Visible = false;
            this.menuAccountStatus.Click += new System.EventHandler(this.menuAccountStatus_Click);
            // 
            // menuAddAcctCodes
            // 
            this.menuAddAcctCodes.Description = "MenuItem";
            this.menuAddAcctCodes.Enabled = false;
            this.menuAddAcctCodes.ImageIndex = 8;
            this.menuAddAcctCodes.ImageList = this.menuIcons;
            this.menuAddAcctCodes.Text = "Add &Codes To Account";
            this.menuAddAcctCodes.Visible = false;
            this.menuAddAcctCodes.Click += new System.EventHandler(this.menuAccountCodes_Click);
            // 
            // menuPaidAndTaken
            // 
            this.menuPaidAndTaken.Description = "MenuItem";
            this.menuPaidAndTaken.Enabled = false;
            this.menuPaidAndTaken.ImageIndex = 23;
            this.menuPaidAndTaken.ImageList = this.menuIcons;
            this.menuPaidAndTaken.Text = "Cash and &Go";
            this.menuPaidAndTaken.Visible = false;
            this.menuPaidAndTaken.Click += new System.EventHandler(this.menuPaidAndTaken_Click);
            // 
            // menuSearchCashAndGo
            // 
            this.menuSearchCashAndGo.Description = "MenuItem";
            this.menuSearchCashAndGo.Enabled = false;
            this.menuSearchCashAndGo.Text = "&Search Cash and Go";
            this.menuSearchCashAndGo.Visible = false;
            this.menuSearchCashAndGo.Click += new System.EventHandler(this.menuSearchCashAndGo_Click);
            // 
            // menuCashAndGoReturn
            // 
            this.menuCashAndGoReturn.Description = "MenuItem";
            this.menuCashAndGoReturn.Enabled = false;
            this.menuCashAndGoReturn.Text = "Legacy Cash And Go &Returns";
            this.menuCashAndGoReturn.Visible = false;
            this.menuCashAndGoReturn.Click += new System.EventHandler(this.menuCashAndGoReturn_Click);
            // 
            // menuNumberGeneration
            // 
            this.menuNumberGeneration.Description = "Number Generation";
            this.menuNumberGeneration.Enabled = false;
            this.menuNumberGeneration.Text = "Number Generation";
            this.menuNumberGeneration.Visible = false;
            this.menuNumberGeneration.Click += new System.EventHandler(this.menuNumberGeneration_Click);
            // 
            // menuWarrantyRenewals
            // 
            this.menuWarrantyRenewals.Description = "MenuItem";
            this.menuWarrantyRenewals.Enabled = false;
            this.menuWarrantyRenewals.Text = "Warranty Renewals ";
            this.menuWarrantyRenewals.Visible = false;
            this.menuWarrantyRenewals.Click += new System.EventHandler(this.menuWarrantyRenewals_Click);
            // 
            // menuUnpaidAccounts
            // 
            this.menuUnpaidAccounts.Description = "MenuItem";
            this.menuUnpaidAccounts.Enabled = false;
            this.menuUnpaidAccounts.Text = "&Unpaid Accounts";
            this.menuUnpaidAccounts.Visible = false;
            this.menuUnpaidAccounts.Click += new System.EventHandler(this.menuUnpaidAccounts_Click);
            // 
            // menuChangeOrderDetails
            // 
            this.menuChangeOrderDetails.Description = "MenuItem";
            this.menuChangeOrderDetails.Enabled = false;
            this.menuChangeOrderDetails.Text = "Change Order Details";
            this.menuChangeOrderDetails.Visible = false;
            this.menuChangeOrderDetails.Click += new System.EventHandler(this.menuChangeOrderDetails_Click);
            // 
            // menuInstantReplacement
            // 
            this.menuInstantReplacement.Description = "MenuItem";
            this.menuInstantReplacement.Enabled = false;
            this.menuInstantReplacement.Text = "Instant Replacement";
            this.menuInstantReplacement.Visible = false;
            this.menuInstantReplacement.Click += new System.EventHandler(this.menuInstantReplacement_Click);
            // 
            // menuEPOS
            // 
            this.menuEPOS.Description = "MenuItem";
            this.menuEPOS.Enabled = false;
            this.menuEPOS.Text = "EPOS";
            this.menuEPOS.Visible = false;
            this.menuEPOS.Click += new System.EventHandler(this.menuEPOS_Click);
            // 
            // menuCustomer
            // 
            this.menuCustomer.Description = "MenuItem";
            this.menuCustomer.Enabled = false;
            this.menuCustomer.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {
            this.menuCustomerSearch,
            this.menuAddCustCodes,
            this.menuNewCustomer,
            this.menuAuthoriseDelivery,
            this.menuAuthoriseIC,
            this.menuGoodsReturn,
            this.menuOneForOneReplacement,
            this.menuImageManagement,
            this.menuPrizeVouchers,
            this.menuServiceCustomerInteraction,
            this.menuViewStoreCard,
            this.menuCashLoanApplication,
            this.menuDuplicateCustomers,
            this.menuCashLoanBankTransfer});
            this.menuCustomer.Text = "&Customer";
            this.menuCustomer.Visible = false;
            // 
            // menuCustomerSearch
            // 
            this.menuCustomerSearch.Description = "MenuItem";
            this.menuCustomerSearch.Enabled = false;
            this.menuCustomerSearch.ImageIndex = 9;
            this.menuCustomerSearch.ImageList = this.menuIcons;
            this.menuCustomerSearch.Text = "&Customer Search";
            this.menuCustomerSearch.Visible = false;
            this.menuCustomerSearch.Click += new System.EventHandler(this.menuCustomerSearch_Click);
            // 
            // menuAddCustCodes
            // 
            this.menuAddCustCodes.Description = "MenuItem";
            this.menuAddCustCodes.Enabled = false;
            this.menuAddCustCodes.ImageIndex = 8;
            this.menuAddCustCodes.ImageList = this.menuIcons;
            this.menuAddCustCodes.Text = "&Add Codes To Customer";
            this.menuAddCustCodes.Visible = false;
            this.menuAddCustCodes.Click += new System.EventHandler(this.menuAccountCodes_Click);
            // 
            // menuNewCustomer
            // 
            this.menuNewCustomer.Description = "MenuItem";
            this.menuNewCustomer.Enabled = false;
            this.menuNewCustomer.ImageIndex = 10;
            this.menuNewCustomer.ImageList = this.menuIcons;
            this.menuNewCustomer.Text = "&New Customer";
            this.menuNewCustomer.Visible = false;
            this.menuNewCustomer.Click += new System.EventHandler(this.menuNewCustomer_Click);
            // 
            // menuAuthoriseDelivery
            // 
            this.menuAuthoriseDelivery.Description = "MenuItem";
            this.menuAuthoriseDelivery.Enabled = false;
            this.menuAuthoriseDelivery.ImageIndex = 25;
            this.menuAuthoriseDelivery.ImageList = this.menuIcons;
            this.menuAuthoriseDelivery.Text = "&Authorise Delivery";
            this.menuAuthoriseDelivery.Visible = false;
            this.menuAuthoriseDelivery.Click += new System.EventHandler(this.menuAuthoriseDelivery_Click);
            // 
            // menuAuthoriseIC
            // 
            this.menuAuthoriseIC.Description = "MenuItem";
            this.menuAuthoriseIC.Enabled = false;
            this.menuAuthoriseIC.ImageIndex = 25;
            this.menuAuthoriseIC.ImageList = this.menuIcons;
            this.menuAuthoriseIC.Text = "&Authorise Instant Credit";
            this.menuAuthoriseIC.Visible = false;
            this.menuAuthoriseIC.Click += new System.EventHandler(this.menuAuthoriseIC_Click);
            // 
            // menuGoodsReturn
            // 
            this.menuGoodsReturn.Description = "MenuItem";
            this.menuGoodsReturn.Enabled = false;
            this.menuGoodsReturn.Text = "&Goods Return";
            this.menuGoodsReturn.Visible = false;
            this.menuGoodsReturn.Click += new System.EventHandler(this.menuGoodsReturn_Click);
            // 
            // menuOneForOneReplacement
            // 
            this.menuOneForOneReplacement.Description = "MenuItem";
            this.menuOneForOneReplacement.Text = "One For One Replacement";
            this.menuOneForOneReplacement.Visible = false;
            this.menuOneForOneReplacement.Click += new System.EventHandler(this.meneOneForOneReplacement_Click);
            // 
            // menuImageManagement
            // 
            this.menuImageManagement.Description = "MenuItem";
            this.menuImageManagement.Enabled = false;
            this.menuImageManagement.ImageIndex = 28;
            this.menuImageManagement.ImageList = this.menuIcons;
            this.menuImageManagement.Text = "&Image Management";
            this.menuImageManagement.Visible = false;
            this.menuImageManagement.Click += new System.EventHandler(this.menuImageManagement_Click);
            // 
            // menuPrizeVouchers
            // 
            this.menuPrizeVouchers.Description = "MenuItem";
            this.menuPrizeVouchers.Enabled = false;
            this.menuPrizeVouchers.Text = "Prize Vouchers";
            this.menuPrizeVouchers.Visible = false;
            this.menuPrizeVouchers.Click += new System.EventHandler(this.menuPrizeVouchers_Click);
            // 
            // menuServiceCustomerInteraction
            // 
            this.menuServiceCustomerInteraction.Description = "MenuItem";
            this.menuServiceCustomerInteraction.Enabled = false;
            this.menuServiceCustomerInteraction.Text = "Customer Interaction";
            this.menuServiceCustomerInteraction.Visible = false;
            this.menuServiceCustomerInteraction.Click += new System.EventHandler(this.menuServiceCustomerInteraction_Click);
            // 
            // menuViewStoreCard
            // 
            this.menuViewStoreCard.Description = "MenuItem";
            this.menuViewStoreCard.Enabled = false;
            this.menuViewStoreCard.ImageIndex = 9;
            this.menuViewStoreCard.ImageList = this.menuIcons;
            this.menuViewStoreCard.Text = "&View StoreCard Details";
            this.menuViewStoreCard.Visible = false;
            this.menuViewStoreCard.Click += new System.EventHandler(this.menuViewStoreCard_Click);
            // 
            // menuCashLoanApplication
            // 
            this.menuCashLoanApplication.Description = "MenuItem";
            this.menuCashLoanApplication.Enabled = false;
            this.menuCashLoanApplication.Text = "Cash Loan";
            this.menuCashLoanApplication.Visible = false;
            this.menuCashLoanApplication.Click += new System.EventHandler(this.menuCashLoanApplication_Click);
            // 
            // menuDuplicateCustomers
            // 
            this.menuDuplicateCustomers.Description = "MenuItem";
            this.menuDuplicateCustomers.ImageIndex = 9;
            this.menuDuplicateCustomers.ImageList = this.menuIcons;
            this.menuDuplicateCustomers.Text = "Duplicate Customers";
            this.menuDuplicateCustomers.Click += new System.EventHandler(this.menuDuplicateCustomers_Click);
            // 
            // menuCashLoanBankTransfer
            // 
            this.menuCashLoanBankTransfer.Description = "Cash Loan Record Bank Transfer";
            this.menuCashLoanBankTransfer.Text = "Cash Loan Record Bank Transfer";
            this.menuCashLoanBankTransfer.Click += new System.EventHandler(this.menuCashLoanBankTransfer_Click);
            // 
            // menuCredit
            // 
            this.menuCredit.Description = "MenuItem";
            this.menuCredit.Enabled = false;
            this.menuCredit.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {
            this.menuIncomplete,
            this.menuFollowUp,
            this.menuBailiffReview,
            this.menuTelephoneAction,
            this.menuCancel,
            this.menuDDMandate,
            this.menuDDRejection,
            this.menuDDPaymentExtra,
            this.menuWOReview,
            this.menuStatusCode,
            this.menuUpdateDateDue,
            this.menuReverseCancel,
            this.menuTempReceiptInvest,
            this.menuReprintActionSheet,
            this.menuCalcBailCommission,
            this.menuReprintBailCommn});
            this.menuCredit.Text = "C&redit";
            this.menuCredit.Visible = false;
            this.menuCredit.Click += new System.EventHandler(this.menuCredit_Click);
            // 
            // menuIncomplete
            // 
            this.menuIncomplete.Description = "MenuItem";
            this.menuIncomplete.Enabled = false;
            this.menuIncomplete.ImageIndex = 15;
            this.menuIncomplete.ImageList = this.menuIcons;
            this.menuIncomplete.Text = "&Incomplete Credit Applications";
            this.menuIncomplete.Visible = false;
            this.menuIncomplete.Click += new System.EventHandler(this.menuIncomplete_Click);
            // 
            // menuFollowUp
            // 
            this.menuFollowUp.Description = "MenuItem";
            this.menuFollowUp.Enabled = false;
            this.menuFollowUp.Text = "Collection Account Analysis";
            this.menuFollowUp.Visible = false;
            this.menuFollowUp.Click += new System.EventHandler(this.menuFollowUp_Click);
            // 
            // menuBailiffReview
            // 
            this.menuBailiffReview.Description = "MenuItem";
            this.menuBailiffReview.Enabled = false;
            this.menuBailiffReview.ImageIndex = 29;
            this.menuBailiffReview.ImageList = this.menuIcons;
            this.menuBailiffReview.Text = "&Bailiff Review";
            this.menuBailiffReview.Visible = false;
            this.menuBailiffReview.Click += new System.EventHandler(this.menuBailiffReview_Click);
            // 
            // menuTelephoneAction
            // 
            this.menuTelephoneAction.Description = "MenuItem";
            this.menuTelephoneAction.Enabled = false;
            this.menuTelephoneAction.ImageIndex = 27;
            this.menuTelephoneAction.ImageList = this.menuIcons;
            this.menuTelephoneAction.Text = "&Telephone Action Screen";
            this.menuTelephoneAction.Visible = false;
            this.menuTelephoneAction.Click += new System.EventHandler(this.menuTelephoneAction_Click);
            // 
            // menuCancel
            // 
            this.menuCancel.Description = "MenuItem";
            this.menuCancel.Enabled = false;
            this.menuCancel.Text = "&Cancel Account";
            this.menuCancel.Visible = false;
            this.menuCancel.Click += new System.EventHandler(this.menuCancel_Click);
            // 
            // menuDDMandate
            // 
            this.menuDDMandate.Description = "MenuItem";
            this.menuDDMandate.Enabled = false;
            this.menuDDMandate.Text = "&Giro Mandate";
            this.menuDDMandate.Visible = false;
            this.menuDDMandate.Click += new System.EventHandler(this.menuDDMandate_Click);
            // 
            // menuDDRejection
            // 
            this.menuDDRejection.Description = "MenuItem";
            this.menuDDRejection.Text = "Giro Rejections";
            this.menuDDRejection.Click += new System.EventHandler(this.menuDDRejection_Click);
            // 
            // menuDDPaymentExtra
            // 
            this.menuDDPaymentExtra.Description = "MenuItem";
            this.menuDDPaymentExtra.Text = "Giro Extra Payments";
            this.menuDDPaymentExtra.Click += new System.EventHandler(this.menuDDPaymentExtra_Click);
            // 
            // menuWOReview
            // 
            this.menuWOReview.Description = "MenuItem";
            this.menuWOReview.Enabled = false;
            this.menuWOReview.Text = "&Write Off Review";
            this.menuWOReview.Visible = false;
            this.menuWOReview.Click += new System.EventHandler(this.menuWOReview_Click);
            // 
            // menuStatusCode
            // 
            this.menuStatusCode.Description = "MenuItem";
            this.menuStatusCode.Enabled = false;
            this.menuStatusCode.Text = "Status Code Maintenance";
            this.menuStatusCode.Visible = false;
            this.menuStatusCode.Click += new System.EventHandler(this.menuStatusCode_Click);
            // 
            // menuUpdateDateDue
            // 
            this.menuUpdateDateDue.Description = "MenuItem";
            this.menuUpdateDateDue.Enabled = false;
            this.menuUpdateDateDue.Text = "Update Date Due";
            this.menuUpdateDateDue.Visible = false;
            this.menuUpdateDateDue.Click += new System.EventHandler(this.menuUpdateDateDue_Click);
            // 
            // menuReverseCancel
            // 
            this.menuReverseCancel.Description = "MenuItem";
            this.menuReverseCancel.Enabled = false;
            this.menuReverseCancel.Text = "Reverse Cancelled Account";
            this.menuReverseCancel.Visible = false;
            this.menuReverseCancel.Click += new System.EventHandler(this.menuReverseCancel_Click);
            // 
            // menuTempReceiptInvest
            // 
            this.menuTempReceiptInvest.Description = "Temporary Receipts";
            this.menuTempReceiptInvest.Text = "Temporary Receipts";
            this.menuTempReceiptInvest.Click += new System.EventHandler(this.menuTempReceiptInvest_Click);
            // 
            // menuReprintActionSheet
            // 
            this.menuReprintActionSheet.Description = "MenuItem";
            this.menuReprintActionSheet.Enabled = false;
            this.menuReprintActionSheet.Text = "Re-Print Debt Collectors Action Sheet";
            this.menuReprintActionSheet.Visible = false;
            this.menuReprintActionSheet.Click += new System.EventHandler(this.menuReprintActionSheet_Click);
            // 
            // menuCalcBailCommission
            // 
            this.menuCalcBailCommission.Description = "MenuItem";
            this.menuCalcBailCommission.Enabled = false;
            this.menuCalcBailCommission.Text = "Calculate Bailiff Commission";
            this.menuCalcBailCommission.Visible = false;
            this.menuCalcBailCommission.Click += new System.EventHandler(this.menuCalcBailCommission_Click);
            // 
            // menuReprintBailCommn
            // 
            this.menuReprintBailCommn.Description = "MenuItem";
            this.menuReprintBailCommn.Enabled = false;
            this.menuReprintBailCommn.Text = "Reprint Bailiff Commission";
            this.menuReprintBailCommn.Visible = false;
            this.menuReprintBailCommn.Click += new System.EventHandler(this.menuReprintBailCommn_Click);
            // 
            // menuTransaction
            // 
            this.menuTransaction.Description = "MenuItem";
            this.menuTransaction.ImageIndex = 2;
            this.menuTransaction.ImageList = this.menuIcons;
            this.menuTransaction.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {
            this.menuPayment,
            this.menuCashier,
            this.menuDeposits,
            this.menuDisbursements,
            this.menuTransferTransaction,
            this.RePrintInvoice,
            this.menuJournal,
            this.menuCorrection,
            this.menuReturnCheque,
            this.menuGeneralTransactions,
            this.menuSellGiftVoucher,
            this.menuCashierByBranch,
            this.menuFinTransQuery});
            this.menuTransaction.Text = "&Transactions";
            // 
            // menuPayment
            // 
            this.menuPayment.Description = "MenuItem";
            this.menuPayment.Enabled = false;
            this.menuPayment.ImageIndex = 26;
            this.menuPayment.ImageList = this.menuIcons;
            this.menuPayment.Text = "&Payments";
            this.menuPayment.Visible = false;
            this.menuPayment.Click += new System.EventHandler(this.menuPayment_Click);
            // 
            // menuCashier
            // 
            this.menuCashier.Description = "MenuItem";
            this.menuCashier.Enabled = false;
            this.menuCashier.ImageIndex = 32;
            this.menuCashier.ImageList = this.menuIcons;
            this.menuCashier.Text = "&Cashier Totals";
            this.menuCashier.Visible = false;
            this.menuCashier.Click += new System.EventHandler(this.menuCashier_Click);
            // 
            // menuDeposits
            // 
            this.menuDeposits.Description = "MenuItem";
            this.menuDeposits.Enabled = false;
            this.menuDeposits.ImageIndex = 33;
            this.menuDeposits.ImageList = this.menuIcons;
            this.menuDeposits.Text = "Cashier Deposits";
            this.menuDeposits.Visible = false;
            this.menuDeposits.Click += new System.EventHandler(this.menuDeposits_Click);
            // 
            // menuDisbursements
            // 
            this.menuDisbursements.Description = "MenuItem";
            this.menuDisbursements.Enabled = false;
            this.menuDisbursements.Text = "Cashier &Disbursements";
            this.menuDisbursements.Visible = false;
            this.menuDisbursements.Click += new System.EventHandler(this.menuDisbursements_Click);
            // 
            // menuTransferTransaction
            // 
            this.menuTransferTransaction.Description = "MenuItem";
            this.menuTransferTransaction.Enabled = false;
            this.menuTransferTransaction.ImageIndex = 30;
            this.menuTransferTransaction.ImageList = this.menuIcons;
            this.menuTransferTransaction.Text = "&Transfer Transaction";
            this.menuTransferTransaction.Visible = false;
            this.menuTransferTransaction.Click += new System.EventHandler(this.menuTransferTransaction_Click);
            // 
            // RePrintInvoice
            // 
            this.RePrintInvoice.Description = "MenuItem";
            this.RePrintInvoice.Enabled = false;
            this.RePrintInvoice.ImageIndex = 9;
            this.RePrintInvoice.ImageList = this.menuIcons;
            this.RePrintInvoice.Text = "&Reprint Invoice";
            this.RePrintInvoice.Visible = false;
            this.RePrintInvoice.Click += new System.EventHandler(this.RePrintInvoice_Click);
            // 
            // menuJournal
            // 
            this.menuJournal.Description = "MenuItem";
            this.menuJournal.Enabled = false;
            this.menuJournal.Text = "Transaction &Journal Enquiry";
            this.menuJournal.Visible = false;
            this.menuJournal.Click += new System.EventHandler(this.menuJournal_Click);
            // 
            // menuCorrection
            // 
            this.menuCorrection.Description = "MenuItem";
            this.menuCorrection.Enabled = false;
            this.menuCorrection.Text = "&Refunds and Corrections";
            this.menuCorrection.Visible = false;
            this.menuCorrection.Click += new System.EventHandler(this.menuCorrection_Click);
            // 
            // menuReturnCheque
            // 
            this.menuReturnCheque.Description = "MenuItem";
            this.menuReturnCheque.Enabled = false;
            this.menuReturnCheque.Text = "&Cheque Return";
            this.menuReturnCheque.Visible = false;
            this.menuReturnCheque.Click += new System.EventHandler(this.menuReturnCheque_Click);
            // 
            // menuGeneralTransactions
            // 
            this.menuGeneralTransactions.Description = "MenuItem";
            this.menuGeneralTransactions.Enabled = false;
            this.menuGeneralTransactions.ImageIndex = 31;
            this.menuGeneralTransactions.ImageList = this.menuIcons;
            this.menuGeneralTransactions.Text = "&General Financial Transactions";
            this.menuGeneralTransactions.Visible = false;
            this.menuGeneralTransactions.Click += new System.EventHandler(this.menuGeneralTransactions_Click);
            // 
            // menuSellGiftVoucher
            // 
            this.menuSellGiftVoucher.Description = "MenuItem";
            this.menuSellGiftVoucher.Enabled = false;
            this.menuSellGiftVoucher.Text = "Sell Gift &Voucher";
            this.menuSellGiftVoucher.Visible = false;
            this.menuSellGiftVoucher.Click += new System.EventHandler(this.menuSellGiftVoucher_Click);
            // 
            // menuCashierByBranch
            // 
            this.menuCashierByBranch.Description = "MenuItem";
            this.menuCashierByBranch.Enabled = false;
            this.menuCashierByBranch.Text = "Overages and Shortages";
            this.menuCashierByBranch.Visible = false;
            this.menuCashierByBranch.Click += new System.EventHandler(this.menuCashierByBranch_Click);
            // 
            // menuFinTransQuery
            // 
            this.menuFinTransQuery.Description = "MenuItem";
            this.menuFinTransQuery.Enabled = false;
            this.menuFinTransQuery.Text = "Financial Transaction &Query";
            this.menuFinTransQuery.Visible = false;
            this.menuFinTransQuery.Click += new System.EventHandler(this.menuFinTransQuery_Click);
            // 
            // menuWarehouse
            // 
            this.menuWarehouse.Description = "MenuItem";
            this.menuWarehouse.Enabled = false;
            this.menuWarehouse.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {
            this.menuLocationEnquiry,
            this.menuProductEnquiry,
            this.menuRedeliverReposs,
            this.menuCancelCollectionNotes,
            this.menuFailedDeliveriesCollections,
            this.menuOnlineProductSearch});
            this.menuWarehouse.Text = "&Warehouse";
            this.menuWarehouse.Visible = false;
            // 
            // menuLocationEnquiry
            // 
            this.menuLocationEnquiry.Description = "MenuItem";
            this.menuLocationEnquiry.Enabled = false;
            this.menuLocationEnquiry.Text = "Stock Enquiry By &Location";
            this.menuLocationEnquiry.Visible = false;
            this.menuLocationEnquiry.Click += new System.EventHandler(this.menuLocationEnquiry_Click);
            // 
            // menuProductEnquiry
            // 
            this.menuProductEnquiry.Description = "MenuItem";
            this.menuProductEnquiry.Enabled = false;
            this.menuProductEnquiry.Text = "Stock Enquiry By &Product";
            this.menuProductEnquiry.Visible = false;
            this.menuProductEnquiry.Click += new System.EventHandler(this.menuProductEnquiry_Click);
            // 
            // menuRedeliverReposs
            // 
            this.menuRedeliverReposs.Description = "Redeliver After Repossession";
            this.menuRedeliverReposs.Enabled = false;
            this.menuRedeliverReposs.Text = "Redeliver After Repossession";
            this.menuRedeliverReposs.Click += new System.EventHandler(this.menuRedeliverReposs_Click);
            // 
            // menuCancelCollectionNotes
            // 
            this.menuCancelCollectionNotes.Description = "MenuItem";
            this.menuCancelCollectionNotes.Enabled = false;
            this.menuCancelCollectionNotes.Text = "Ca&ncel Collection Notes";
            this.menuCancelCollectionNotes.Visible = false;
            this.menuCancelCollectionNotes.Click += new System.EventHandler(this.menuCancelCollectionNotes_Click);
            // 
            // menuFailedDeliveriesCollections
            // 
            this.menuFailedDeliveriesCollections.Description = "Failed Deliveries & Collections";
            this.menuFailedDeliveriesCollections.Enabled = false;
            this.menuFailedDeliveriesCollections.Text = "Failed Deliveries and Collections";
            this.menuFailedDeliveriesCollections.Visible = false;
            this.menuFailedDeliveriesCollections.Click += new System.EventHandler(this.menuFailedDeliveriesCollections_Click);
            // 
            // menuOnlineProductSearch
            // 
            this.menuOnlineProductSearch.Description = "MenuItem";
            this.menuOnlineProductSearch.Enabled = false;
            this.menuOnlineProductSearch.Text = "Online Product Maintenance";
            this.menuOnlineProductSearch.Visible = false;
            this.menuOnlineProductSearch.Click += new System.EventHandler(this.menuOnlineProductSearch_Click);
            // 
            // menuSysMaint
            // 
            this.menuSysMaint.Description = "MenuItem";
            this.menuSysMaint.Enabled = false;
            this.menuSysMaint.ImageList = this.menuIcons;
            this.menuSysMaint.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {
            this.menuBranch,
            this.menuCodeMaintenance,
            this.menuEmployeeMaintenance,
            this.menuCustomiseMenus,
            this.menuMandatory,
            this.menuScoring,
			this.menuSpendFactor,
            this.menuTranslation,
            this.menuScreenTranslation,
            this.menuTransTypeMaintenance,
            this.menuExchangeRates,
            this.menuCountryMaintenance,
            this.menuTermsType,
            this.menuMmiMatrix,
            this.menuProvisions,
            this.menuDeliveryArea,
            this.menuAcctNoCtrl,
            this.menuAllEODTasks,
            this.menuPaymentFileDefn,
            this.menuCommissionsSetUp,
            this.menuStoreCardRateSetup,
            this.menuAddBank,
            this.menuNonStock});
            this.menuSysMaint.Text = "S&ystem Configuration";
            this.menuSysMaint.Visible = false;
            this.menuSysMaint.Click += new System.EventHandler(this.menuSysMaint_Click);
            // 
            // menuBranch
            // 
            this.menuBranch.Description = "MenuItem";
            this.menuBranch.Enabled = false;
            this.menuBranch.Text = "&Branch";
            this.menuBranch.Visible = false;
            this.menuBranch.Click += new System.EventHandler(this.menuBranch_Click);
            // 
            // menuCodeMaintenance
            // 
            this.menuCodeMaintenance.Description = "MenuItem";
            this.menuCodeMaintenance.Text = "Code Maintenance";
            this.menuCodeMaintenance.Click += new System.EventHandler(this.menuCodeMaintenance_Click);
            // 
            // menuEmployeeMaintenance
            // 
            this.menuEmployeeMaintenance.Description = "MenuItem";
            this.menuEmployeeMaintenance.Enabled = false;
            this.menuEmployeeMaintenance.Text = "Staff Maintenance";
            this.menuEmployeeMaintenance.Visible = false;
            this.menuEmployeeMaintenance.Click += new System.EventHandler(this.menuEmployeeMaintenance_Click);
            // 
            // menuCustomiseMenus
            // 
            this.menuCustomiseMenus.Description = "MenuItem";
            this.menuCustomiseMenus.Enabled = false;
            this.menuCustomiseMenus.Text = "&Customise Menus";
            this.menuCustomiseMenus.Visible = false;
            this.menuCustomiseMenus.Click += new System.EventHandler(this.menuCustomiseMenus_Click);
            // 
            // menuMandatory
            // 
            this.menuMandatory.Description = "MenuItem";
            this.menuMandatory.Enabled = false;
            this.menuMandatory.ImageIndex = 13;
            this.menuMandatory.ImageList = this.menuIcons;
            this.menuMandatory.Text = "Customise &Mandatory Fields";
            this.menuMandatory.Visible = false;
            this.menuMandatory.Click += new System.EventHandler(this.menuMandatory_Click);
            // 
            // menuScoring
            // 
            this.menuScoring.Description = "MenuItem";
            this.menuScoring.Enabled = false;
            this.menuScoring.ImageList = this.menuIcons;
            this.menuScoring.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {
            this.menuScoringRules,
            this.menuScoringMatrix,
            this.menuTTMatrix});
            this.menuScoring.Text = "&Scoring";
            this.menuScoring.Visible = false;
			//
            // Expenses
            // 
            this.menuSpendFactor.Description = "MenuItem";
            this.menuSpendFactor.Enabled = false;
            this.menuSpendFactor.ImageList = this.menuIcons;
            this.menuSpendFactor.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {
            this.menuDepenSpendFactor,
            this.menuApplicantSpendFactor});
            this.menuSpendFactor.Text = "&Spend Factor";
            this.menuSpendFactor.Visible = false;
            // 
            // menuScoringRules
            // 
            this.menuScoringRules.Description = "MenuItem";
            this.menuScoringRules.Enabled = false;
            this.menuScoringRules.ImageIndex = 12;
            this.menuScoringRules.ImageList = this.menuIcons;
            this.menuScoringRules.Text = "Customise &Scoring Rules";
            this.menuScoringRules.Visible = false;
            this.menuScoringRules.Click += new System.EventHandler(this.menuScoringRules_Click);
            // 
            // menuScoringMatrix
            // 
            this.menuScoringMatrix.Description = "MenuItem";
            this.menuScoringMatrix.Enabled = false;
            this.menuScoringMatrix.ImageIndex = 14;
            this.menuScoringMatrix.ImageList = this.menuIcons;
            this.menuScoringMatrix.Text = "Customise Credi&t Matrices";
            this.menuScoringMatrix.Visible = false;
            this.menuScoringMatrix.Click += new System.EventHandler(this.menuScoringMatrix_Click);
            // 
            // menuDepenSpendFactor Matrix
            // 
            this.menuDepenSpendFactor.Description = "MenuItem";
            this.menuDepenSpendFactor.Enabled = false;
            this.menuDepenSpendFactor.ImageIndex = 14;
            this.menuDepenSpendFactor.ImageList = this.menuIcons;
            this.menuDepenSpendFactor.Text = "Set Dependent Spend Factor";
            this.menuDepenSpendFactor.Visible = false;
            this.menuDepenSpendFactor.Click += new System.EventHandler(this.menuDependentSpendMatrix_Click);
            // 
            // menuApplicantSpendFactor Matrix
            // 
            this.menuApplicantSpendFactor.Description = "MenuItem";
            this.menuApplicantSpendFactor.Enabled = false;
            this.menuApplicantSpendFactor.ImageIndex = 14;
            this.menuApplicantSpendFactor.ImageList = this.menuIcons;
            this.menuApplicantSpendFactor.Text = "Set Applicant Spend Factor";
            this.menuApplicantSpendFactor.Visible = false;
            this.menuApplicantSpendFactor.Click += new System.EventHandler(this.menuApplicantSpendMatrix_Click);
			// 
            // menuTTMatrix
            // 
            this.menuTTMatrix.Description = "MenuItem";
            this.menuTTMatrix.Enabled = false;
            this.menuTTMatrix.Text = "Customise Sco&reband Matrix";
            this.menuTTMatrix.Visible = false;
            this.menuTTMatrix.Click += new System.EventHandler(this.menuTTMatrix_Click);
            // 
            // menuTranslation
            // 
            this.menuTranslation.Description = "MenuItem";
            this.menuTranslation.Enabled = false;
            this.menuTranslation.Text = "&Stock Item Translation";
            this.menuTranslation.Visible = false;
            this.menuTranslation.Click += new System.EventHandler(this.menuTranslation_Click);
            // 
            // menuScreenTranslation
            // 
            this.menuScreenTranslation.Description = "MenuItem";
            this.menuScreenTranslation.Enabled = false;
            this.menuScreenTranslation.Image = ((System.Drawing.Image)(resources.GetObject("menuScreenTranslation.Image")));
            this.menuScreenTranslation.Text = "&Screen Translation";
            this.menuScreenTranslation.Visible = false;
            this.menuScreenTranslation.Click += new System.EventHandler(this.menuScreenTranslation_Click);
            // 
            // menuTransTypeMaintenance
            // 
            this.menuTransTypeMaintenance.Description = "MenuItem";
            this.menuTransTypeMaintenance.Enabled = false;
            this.menuTransTypeMaintenance.Text = "Transaction Type Maintenance";
            this.menuTransTypeMaintenance.Visible = false;
            this.menuTransTypeMaintenance.Click += new System.EventHandler(this.menuTransTypeMaintenance_Click);
            // 
            // menuExchangeRates
            // 
            this.menuExchangeRates.Description = "MenuItem";
            this.menuExchangeRates.Enabled = false;
            this.menuExchangeRates.Text = "E&xchange Rate Maintenance";
            this.menuExchangeRates.Visible = false;
            this.menuExchangeRates.Click += new System.EventHandler(this.menuExchangeRates_Click);
            // 
            // menuCountryMaintenance
            // 
            this.menuCountryMaintenance.Description = "MenuItem";
            this.menuCountryMaintenance.Enabled = false;
            this.menuCountryMaintenance.Text = "C&ountry Maintenance";
            this.menuCountryMaintenance.Visible = false;
            this.menuCountryMaintenance.Click += new System.EventHandler(this.menuCountryMaintenance_Click);
            // 
            // menuTermsType
            // 
            this.menuTermsType.Description = "MenuItem";
            this.menuTermsType.Enabled = false;
            this.menuTermsType.Text = "Terms Type &Maintenance";
            this.menuTermsType.Visible = false;
            this.menuTermsType.Click += new System.EventHandler(this.menuTermsType_Click);
            // 
            // menuMmiMatrix
            // 
            this.menuMmiMatrix.Description = "MenuItem";
            this.menuMmiMatrix.Enabled = false;
            this.menuMmiMatrix.Text = "MMI Matrix";
            this.menuMmiMatrix.Visible = false;
            this.menuMmiMatrix.Click += new System.EventHandler(this.menuMmiMatrix_Click);
            // 
            // menuProvisions
            // 
            this.menuProvisions.Description = "Edit Provisions";
            this.menuProvisions.Text = "&Provisions";
            this.menuProvisions.Visible = false;
            this.menuProvisions.Click += new System.EventHandler(this.menuProvisions_Click);
            // 
            // menuDeliveryArea
            // 
            this.menuDeliveryArea.Description = "MenuItem";
            this.menuDeliveryArea.Enabled = false;
            this.menuDeliveryArea.Text = "&Delivery Area Maintenance";
            this.menuDeliveryArea.Visible = false;
            this.menuDeliveryArea.Click += new System.EventHandler(this.menuDeliveryArea_Click);
            // 
            // menuAcctNoCtrl
            // 
            this.menuAcctNoCtrl.Description = "MenuItem";
            this.menuAcctNoCtrl.Enabled = false;
            this.menuAcctNoCtrl.Text = "&Account Number Control";
            this.menuAcctNoCtrl.Visible = false;
            this.menuAcctNoCtrl.Click += new System.EventHandler(this.menuAcctNoCtrl_Click);
            // 
            // menuAllEODTasks
            // 
            this.menuAllEODTasks.Description = "MenuItem";
            this.menuAllEODTasks.Enabled = false;
            this.menuAllEODTasks.ImageList = this.menuIcons;
            this.menuAllEODTasks.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {
            this.menuEOD,
            this.menuEODInterface,
            this.menuTallymanExtract,
            this.menuStoreCardBatchPrint});
            this.menuAllEODTasks.Text = "&End of Day Tasks";
            this.menuAllEODTasks.Visible = false;
            // 
            // menuEOD
            // 
            this.menuEOD.Description = "MenuItem";
            this.menuEOD.Enabled = false;
            this.menuEOD.Text = "EOD Control";
            this.menuEOD.Visible = false;
            this.menuEOD.Click += new System.EventHandler(this.menuEOD_Click);
            // 
            // menuEODInterface
            // 
            this.menuEODInterface.Description = "MenuItem";
            this.menuEODInterface.Enabled = false;
            this.menuEODInterface.Text = "End Of Day";
            this.menuEODInterface.Visible = false;
            this.menuEODInterface.Click += new System.EventHandler(this.menuEODInterface_Click);
            // 
            // menuTallymanExtract
            // 
            this.menuTallymanExtract.Description = "MenuItem";
            this.menuTallymanExtract.Text = "Malaysia Interface";
            this.menuTallymanExtract.Click += new System.EventHandler(this.menuTallymanExtract_Click);
            // 
            // menuStoreCardBatchPrint
            // 
            this.menuStoreCardBatchPrint.Description = "MenuItem";
            this.menuStoreCardBatchPrint.Text = "Store Card Batch Print";
            this.menuStoreCardBatchPrint.Click += new System.EventHandler(this.menuStoreCardBatchPrint_Click);
            // 
            // menuPaymentFileDefn
            // 
            this.menuPaymentFileDefn.Description = "Payment File Definition";
            this.menuPaymentFileDefn.Enabled = false;
            this.menuPaymentFileDefn.Text = "Payment File Definition";
            this.menuPaymentFileDefn.Visible = false;
            this.menuPaymentFileDefn.Click += new System.EventHandler(this.menuPaymentFileDefn_Click);
            // 
            // menuCommissionsSetUp
            // 
            this.menuCommissionsSetUp.Description = "Sales Commission Maintenance";
            this.menuCommissionsSetUp.Enabled = false;
            this.menuCommissionsSetUp.Text = "Sales Commission Maintenance";
            this.menuCommissionsSetUp.Visible = false;
            this.menuCommissionsSetUp.Click += new System.EventHandler(this.menuCommissionsSetUp_Click);
            // 
            // menuStoreCardRateSetup
            // 
            this.menuStoreCardRateSetup.Description = "MenuItem";
            this.menuStoreCardRateSetup.Enabled = false;
            this.menuStoreCardRateSetup.Text = "Setup Storecard interest Rates";
            this.menuStoreCardRateSetup.Visible = false;
            this.menuStoreCardRateSetup.Click += new System.EventHandler(this.menuStoreCardRateSetup_Click);
            // 
            // menuAddBank
            // 
            this.menuAddBank.Description = "MenuItem";
            this.menuAddBank.Enabled = false;
            this.menuAddBank.Text = "Bank Maintenance";
            this.menuAddBank.Visible = false;
            this.menuAddBank.Click += new System.EventHandler(this.menuAddBank_Click);
            // 
            // menuNonStock
            // 
            this.menuNonStock.Description = "MenuItem";
            this.menuNonStock.Enabled = false;
            this.menuNonStock.Text = "Non Stock Maintenance";
            this.menuNonStock.Visible = false;
            this.menuNonStock.Click += new System.EventHandler(this.menuNonStock_Click);
            // 
            // menuCollections
            // 
            this.menuCollections.Description = "MenuItem";
            this.menuCollections.Enabled = false;
            this.menuCollections.ImageList = this.menuIcons;
            this.menuCollections.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {
            this.menuCommissionMaint,
            this.menuLetterMerge,
            this.menuZoneAutomation,
            this.menuWorkLists,
            this.menuStrategyConfiguration,
            this.menuSMS});
            this.menuCollections.Text = "C&ollections";
            this.menuCollections.Visible = false;
            // 
            // menuCommissionMaint
            // 
            this.menuCommissionMaint.Description = "Bailiff Commission Maintenance";
            this.menuCommissionMaint.Enabled = false;
            this.menuCommissionMaint.Text = "Collection Commission Maintenance";
            this.menuCommissionMaint.Visible = false;
            this.menuCommissionMaint.Click += new System.EventHandler(this.menuCommissionMaint_Click);
            // 
            // menuLetterMerge
            // 
            this.menuLetterMerge.Description = "MenuItem";
            this.menuLetterMerge.Enabled = false;
            this.menuLetterMerge.Text = "Letter Merge";
            this.menuLetterMerge.Visible = false;
            this.menuLetterMerge.Click += new System.EventHandler(this.menuLetterMerge_Click);
            // 
            // menuZoneAutomation
            // 
            this.menuZoneAutomation.Description = "MenuItem";
            this.menuZoneAutomation.Enabled = false;
            this.menuZoneAutomation.Text = "Zone && Automation Allocation";
            this.menuZoneAutomation.Visible = false;
            this.menuZoneAutomation.Click += new System.EventHandler(this.menuZoneAutomation_Click);
            // 
            // menuWorkLists
            // 
            this.menuWorkLists.Description = "MenuItem";
            this.menuWorkLists.Enabled = false;
            this.menuWorkLists.Text = "Work List Setup";
            this.menuWorkLists.Visible = false;
            this.menuWorkLists.Click += new System.EventHandler(this.menuWorkLists_Click);
            // 
            // menuStrategyConfiguration
            // 
            this.menuStrategyConfiguration.Description = "MenuItem";
            this.menuStrategyConfiguration.Enabled = false;
            this.menuStrategyConfiguration.Text = "Strategy Configuration";
            this.menuStrategyConfiguration.Visible = false;
            this.menuStrategyConfiguration.Click += new System.EventHandler(this.menuStrategyConfiguration_Click);
            // 
            // menuSMS
            // 
            this.menuSMS.Description = "MenuItem";
            this.menuSMS.Enabled = false;
            this.menuSMS.Text = "SMS Setup";
            this.menuSMS.Visible = false;
            this.menuSMS.Click += new System.EventHandler(this.menuSMS_Click);
            // 
            // menuReports
            // 
            this.menuReports.Description = "MenuItem";
            this.menuReports.Enabled = false;
            this.menuReports.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {
            this.menuBehavioural,
            this.menuCustomerMailing,
            this.menuFactoringReports,
            this.menuInterfaceReport,
            this.menuMonitorBookings,
            this.menuRebateCalculation,
            this.menuRebateReport,
            this.menuCommissionEnquiry,
            this.menuCommissionReport,
            this.menuSumryUpdControl,
            this.menuWarrantyReport,
            this.menuSalesCommEnquiry,
            this.menuSalesCommBranchEnquiry});
            this.menuReports.Text = "Reports";
            this.menuReports.Visible = false;
            // 
            // menuBehavioural
            // 
            this.menuBehavioural.Description = "Behavioural Scoring Results";
            this.menuBehavioural.Enabled = false;
            this.menuBehavioural.Text = "Behavioural Scoring Rescore";
            this.menuBehavioural.Visible = false;
            this.menuBehavioural.Click += new System.EventHandler(this.menuBehavioural_Click);
            // 
            // menuCustomerMailing
            // 
            this.menuCustomerMailing.Description = "CustomerMailing";
            this.menuCustomerMailing.Enabled = false;
            this.menuCustomerMailing.Text = "Customer Mailing";
            this.menuCustomerMailing.Visible = false;
            this.menuCustomerMailing.Click += new System.EventHandler(this.menuCustomerMailing_Click);
            // 
            // menuFactoringReports
            // 
            this.menuFactoringReports.Description = "MenuItem";
            this.menuFactoringReports.Enabled = false;
            this.menuFactoringReports.Text = "Factoring Reports";
            this.menuFactoringReports.Visible = false;
            this.menuFactoringReports.Click += new System.EventHandler(this.menuFactoringReports_Click);
            // 
            // menuInterfaceReport
            // 
            this.menuInterfaceReport.Description = "MenuItem";
            this.menuInterfaceReport.Enabled = false;
            this.menuInterfaceReport.Text = "&Financial Interface Report";
            this.menuInterfaceReport.Visible = false;
            this.menuInterfaceReport.Click += new System.EventHandler(this.menuInterfaceReport_Click);
            // 
            // menuMonitorBookings
            // 
            this.menuMonitorBookings.Description = "Monitor Bookings";
            this.menuMonitorBookings.Enabled = false;
            this.menuMonitorBookings.Text = "Monitor Bookings";
            this.menuMonitorBookings.Visible = false;
            this.menuMonitorBookings.Click += new System.EventHandler(this.menuMonitorBookings_Click);
            // 
            // menuRebateCalculation
            // 
            this.menuRebateCalculation.Description = "MenuItem";
            this.menuRebateCalculation.Enabled = false;
            this.menuRebateCalculation.Text = "Re&bate Calculation Frame";
            this.menuRebateCalculation.Visible = false;
            this.menuRebateCalculation.Click += new System.EventHandler(this.menuRebateCalculation_Click);
            // 
            // menuRebateReport
            // 
            this.menuRebateReport.Description = "MenuItem";
            this.menuRebateReport.Enabled = false;
            this.menuRebateReport.Text = "Rebate Forecasting Report";
            this.menuRebateReport.Visible = false;
            this.menuRebateReport.Click += new System.EventHandler(this.menuRebateReport_Click);
            // 
            // menuCommissionEnquiry
            // 
            this.menuCommissionEnquiry.Description = "Sales Commission Enquiry";
            this.menuCommissionEnquiry.Enabled = false;
            this.menuCommissionEnquiry.Text = "Sales Commission Enquiry";
            this.menuCommissionEnquiry.Visible = false;
            this.menuCommissionEnquiry.Click += new System.EventHandler(this.menuCommissionEnquiry_Click);
            // 
            // menuCommissionReport
            // 
            this.menuCommissionReport.Description = "Sales Commission Report";
            this.menuCommissionReport.Enabled = false;
            this.menuCommissionReport.Text = "Sales Commission Report";
            this.menuCommissionReport.Visible = false;
            this.menuCommissionReport.Click += new System.EventHandler(this.menuCommissionReport_Click);
            // 
            // menuSumryUpdControl
            // 
            this.menuSumryUpdControl.Description = "MenuItem";
            this.menuSumryUpdControl.Enabled = false;
            this.menuSumryUpdControl.Text = "Summary &Update Control Report";
            this.menuSumryUpdControl.Visible = false;
            this.menuSumryUpdControl.Click += new System.EventHandler(this.menuSumryUpdControl_Click);
            // 
            // menuWarrantyReport
            // 
            this.menuWarrantyReport.Description = "MenuItem";
            this.menuWarrantyReport.Enabled = false;
            this.menuWarrantyReport.Text = "Warranty Reporting";
            this.menuWarrantyReport.Visible = false;
            this.menuWarrantyReport.Click += new System.EventHandler(this.menuWarrantyReport_Click);
            // 
            // menuSalesCommEnquiry
            // 
            this.menuSalesCommEnquiry.Description = "MenuItem";
            this.menuSalesCommEnquiry.Text = "Sales Commission Enquiry";
            this.menuSalesCommEnquiry.Click += new System.EventHandler(this.menuSalesCommEnquiry_Click);
            // 
            // menuSalesCommBranchEnquiry
            // 
            this.menuSalesCommBranchEnquiry.Description = "MenuItem";
            this.menuSalesCommBranchEnquiry.Text = "Sales Commission Branch Enquiry";
            this.menuSalesCommBranchEnquiry.Click += new System.EventHandler(this.menuSalesCommBranchEnquiry_Click);
            // 
            // menuLocateHelp
            // 
            this.menuLocateHelp.Description = "MenuItem";
            this.menuLocateHelp.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {
            this.menuAbout});
            this.menuLocateHelp.Text = "&Help";
            // 
            // menuAbout
            // 
            this.menuAbout.Description = "MenuItem";
            this.menuAbout.Text = "&About Cosacs";
            this.menuAbout.Click += new System.EventHandler(this.menuAbout_Click);
            // 
            // menuPendingInstallations
            // 
            this.menuPendingInstallations.Description = "MenuItem";
            // 
            // menuInstBookingPrint
            // 
            this.menuInstBookingPrint.Description = "MenuItem";
            // 
            // menuInstManagement
            // 
            this.menuInstManagement.Description = "MenuItem";
            // 
            // menuStatements
            // 
            this.menuStatements.Description = "MenuItem";
            this.menuStatements.ImageIndex = 4;
            this.menuStatements.ImageList = this.menuIcons;
            this.menuStatements.Text = "&Statements";
            this.menuStatements.Click += new System.EventHandler(this.menuStatements_Click);
            // 
            // menuCommand2
            // 
            this.menuCommand2.Description = "MenuItem";
            // 
            // openTill
            // 
            this.openTill.Enabled = false;
            this.openTill.Location = new System.Drawing.Point(0, 0);
            this.openTill.Name = "openTill";
            this.openTill.Size = new System.Drawing.Size(0, 0);
            this.openTill.TabIndex = 0;
            this.openTill.Visible = false;
            // 
            // menuCommand3
            // 
            this.menuCommand3.Description = "MenuItem";
            // 
            // appUpdater1
            // 
            this.appUpdater1.ChangeDetectionMode = STL.AppUpdater.ChangeDetectionModes.ServerManifestCheck;
            this.appUpdater1.Poller.PollInterval = 1800;
            this.appUpdater1.ShowDefaultUI = true;
            // 
            // pbDownloading
            // 
            this.pbDownloading.BackColor = System.Drawing.Color.SlateBlue;
            this.pbDownloading.Image = ((System.Drawing.Image)(resources.GetObject("pbDownloading.Image")));
            this.pbDownloading.Location = new System.Drawing.Point(771, 532);
            this.pbDownloading.Name = "pbDownloading";
            this.pbDownloading.Size = new System.Drawing.Size(20, 20);
            this.pbDownloading.TabIndex = 11;
            this.pbDownloading.TabStop = false;
            this.pbDownloading.Visible = false;
            // 
            // lDownloading
            // 
            this.lDownloading.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lDownloading.Location = new System.Drawing.Point(639, 535);
            this.lDownloading.Name = "lDownloading";
            this.lDownloading.Size = new System.Drawing.Size(130, 23);
            this.lDownloading.TabIndex = 12;
            this.lDownloading.Text = "Downloading components...";
            this.lDownloading.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.lDownloading.Visible = false;
            // 
            // menuMain
            // 
            this.menuMain.Animate = Crownwood.Magic.Menus.Animate.Yes;
            this.menuMain.AnimateStyle = Crownwood.Magic.Menus.Animation.SlideHorVerPositive;
            this.menuMain.AnimateTime = 100;
            this.menuMain.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.menuMain.Direction = Crownwood.Magic.Common.Direction.Horizontal;
            this.menuMain.Dock = System.Windows.Forms.DockStyle.Top;
            this.menuMain.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World);
            this.menuMain.HighlightTextColor = System.Drawing.SystemColors.MenuText;
            this.menuMain.Location = new System.Drawing.Point(0, 0);
            this.menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {
            this.menuFile,
            this.menuAccount,
            this.menuCustomer,
            this.menuCredit,
            this.menuTransaction,
            this.menuService,
            this.menuWarehouse,
            this.menuSysMaint,
            this.menuCollections,
            this.menuReports,
            this.menuLocateHelp});
            this.menuMain.Name = "menuMain";
            this.menuMain.Size = new System.Drawing.Size(792, 24);
            this.menuMain.Style = Crownwood.Magic.Common.VisualStyle.IDE;
            this.menuMain.TabIndex = 5;
            this.menuMain.TabStop = false;
            this.menuMain.Text = "menuControl1";
            // 
            // menuService
            // 
            this.menuService.Description = "MenuItem";
            this.menuService.Enabled = false;
            this.menuService.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {
            this.menuBERRep});
            this.menuService.Text = "Service";
            this.menuService.Visible = false;
            // 
            // menuBERRep
            // 
            this.menuBERRep.Description = "MenuItem";
            this.menuBERRep.Enabled = false;
            this.menuBERRep.Text = "BER Replacement";
            this.menuBERRep.Visible = false;
            this.menuBERRep.Click += new System.EventHandler(this.menuBERRep_Click);
            // 
            // MainTabControl
            // 
            this.MainTabControl.BackColor = System.Drawing.SystemColors.ControlLight;
            this.MainTabControl.ButtonActiveColor = System.Drawing.SystemColors.Highlight;
            this.MainTabControl.ButtonInactiveColor = System.Drawing.SystemColors.ControlLight;
            this.MainTabControl.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World);
            this.MainTabControl.IDEPixelArea = true;
            this.MainTabControl.Location = new System.Drawing.Point(0, 24);
            this.MainTabControl.Name = "MainTabControl";
            this.MainTabControl.PositionTop = true;
            this.MainTabControl.SelectedIndex = 0;
            this.MainTabControl.SelectedTab = this.tpLogIn;
            this.MainTabControl.ShowArrows = true;
            this.MainTabControl.ShowClose = true;
            this.MainTabControl.Size = new System.Drawing.Size(792, 504);
            this.MainTabControl.TabIndex = 4;
            this.MainTabControl.TabPages.AddRange(new Crownwood.Magic.Controls.TabPage[] {
            this.tpLogIn});
            this.MainTabControl.TextInactiveColor = System.Drawing.SystemColors.ActiveCaption;
            this.MainTabControl.ClosePressed += new System.EventHandler(this.MainTabControl_ClosePressed);
            this.MainTabControl.SelectionChanged += new System.EventHandler(this.MainTabControl_SelectionChanged);
            // 
            // tpLogIn
            // 
            this.tpLogIn.BackColor = System.Drawing.SystemColors.Control;
            this.tpLogIn.Controls.Add(this.grpLogInCourts);
            this.tpLogIn.Controls.Add(this.grpLogIn);
            this.tpLogIn.Controls.Add(this.pbSplash);
            this.tpLogIn.Location = new System.Drawing.Point(0, 25);
            this.tpLogIn.Name = "tpLogIn";
            this.tpLogIn.Size = new System.Drawing.Size(792, 479);
            this.tpLogIn.TabIndex = 3;
            this.tpLogIn.Title = "Main";
            // 
            // grpLogInCourts
            // 
            this.grpLogInCourts.BackColor = System.Drawing.Color.Transparent;
            this.grpLogInCourts.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("grpLogInCourts.BackgroundImage")));
            this.grpLogInCourts.Controls.Add(this.label5);
            this.grpLogInCourts.Controls.Add(this.label6);
            this.grpLogInCourts.Controls.Add(this.txtPasswordCourts);
            this.grpLogInCourts.Controls.Add(this.txtUserCourts);
            this.grpLogInCourts.Enabled = false;
            this.grpLogInCourts.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.grpLogInCourts.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.grpLogInCourts.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.grpLogInCourts.Location = new System.Drawing.Point(278, 143);
            this.grpLogInCourts.Name = "grpLogInCourts";
            this.grpLogInCourts.Size = new System.Drawing.Size(240, 160);
            this.grpLogInCourts.TabIndex = 12;
            this.grpLogInCourts.TabStop = false;
            this.grpLogInCourts.Text = "Log In";
            this.grpLogInCourts.Visible = false;
            // 
            // label5
            // 
            this.label5.BackColor = System.Drawing.Color.Transparent;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.label5.Image = ((System.Drawing.Image)(resources.GetObject("label5.Image")));
            this.label5.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label5.Location = new System.Drawing.Point(56, 88);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(64, 16);
            this.label5.TabIndex = 3;
            this.label5.Text = "Password";
            // 
            // label6
            // 
            this.label6.BackColor = System.Drawing.Color.Transparent;
            this.label6.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.label6.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.label6.Image = ((System.Drawing.Image)(resources.GetObject("label6.Image")));
            this.label6.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label6.Location = new System.Drawing.Point(56, 24);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(104, 16);
            this.label6.TabIndex = 2;
            this.label6.Text = "Employee Number";
            // 
            // txtPasswordCourts
            // 
            this.txtPasswordCourts.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.txtPasswordCourts.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.txtPasswordCourts.Location = new System.Drawing.Point(56, 112);
            this.txtPasswordCourts.Name = "txtPasswordCourts";
            this.txtPasswordCourts.PasswordChar = '*';
            this.txtPasswordCourts.Size = new System.Drawing.Size(125, 20);
            this.txtPasswordCourts.TabIndex = 1;
            this.txtPasswordCourts.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtPasswordCourts_KeyPress);
            this.txtPasswordCourts.Validating += new System.ComponentModel.CancelEventHandler(this.txtPasswordCourts_Validating);
            // 
            // txtUserCourts
            // 
            this.txtUserCourts.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.txtUserCourts.Location = new System.Drawing.Point(56, 48);
            this.txtUserCourts.Name = "txtUserCourts";
            this.txtUserCourts.Size = new System.Drawing.Size(128, 20);
            this.txtUserCourts.TabIndex = 0;
            this.txtUserCourts.Tag = "";
            // 
            // grpLogIn
            // 
            this.grpLogIn.BackColor = System.Drawing.Color.Transparent;
            this.grpLogIn.Controls.Add(this.label2);
            this.grpLogIn.Controls.Add(this.label1);
            this.grpLogIn.Controls.Add(this.txtPassword);
            this.grpLogIn.Controls.Add(this.txtUser);
            this.grpLogIn.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.grpLogIn.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.grpLogIn.ForeColor = System.Drawing.Color.Black;
            this.grpLogIn.Location = new System.Drawing.Point(267, 126);
            this.grpLogIn.Name = "grpLogIn";
            this.grpLogIn.Size = new System.Drawing.Size(240, 160);
            this.grpLogIn.TabIndex = 10;
            this.grpLogIn.TabStop = false;
            this.grpLogIn.Text = "Log In";
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.label2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label2.Location = new System.Drawing.Point(56, 88);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(64, 16);
            this.label2.TabIndex = 3;
            this.label2.Text = "Password";
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label1.Location = new System.Drawing.Point(56, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(104, 16);
            this.label1.TabIndex = 2;
            this.label1.Text = "Employee Number";
            // 
            // txtPassword
            // 
            this.txtPassword.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.txtPassword.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.txtPassword.Location = new System.Drawing.Point(56, 112);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.PasswordChar = '*';
            this.txtPassword.Size = new System.Drawing.Size(125, 20);
            this.txtPassword.TabIndex = 1;
            this.txtPassword.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtPassword_KeyPress);
            this.txtPassword.Validating += new System.ComponentModel.CancelEventHandler(this.txtPassword_Validating);
            // 
            // txtUser
            // 
            this.txtUser.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.txtUser.Location = new System.Drawing.Point(56, 48);
            this.txtUser.Name = "txtUser";
            this.txtUser.Size = new System.Drawing.Size(128, 20);
            this.txtUser.TabIndex = 0;
            this.txtUser.Tag = "";
            this.txtUser.Validating += new System.ComponentModel.CancelEventHandler(this.txtUser_Validating);
            // 
            // pbSplash
            // 
            this.pbSplash.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pbSplash.Image = ((System.Drawing.Image)(resources.GetObject("pbSplash.Image")));
            this.pbSplash.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.pbSplash.Location = new System.Drawing.Point(0, 0);
            this.pbSplash.Name = "pbSplash";
            this.pbSplash.Size = new System.Drawing.Size(792, 493);
            this.pbSplash.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbSplash.TabIndex = 9;
            this.pbSplash.TabStop = false;
            this.pbSplash.Visible = false;
            // 
            // statusBar1
            // 
            this.statusBar1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.statusBar1.Location = new System.Drawing.Point(0, 533);
            this.statusBar1.Name = "statusBar1";
            this.statusBar1.Size = new System.Drawing.Size(792, 20);
            this.statusBar1.TabIndex = 1;
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            this.errorProvider1.Icon = ((System.Drawing.Icon)(resources.GetObject("errorProvider1.Icon")));
            // 
            // tbSanction
            // 
            this.tbSanction.AccountNo = "";
            this.tbSanction.AccountType = "";
            this.tbSanction.allowConversionToHP = false;
            this.tbSanction.CurrentStatus = "";
            this.tbSanction.CustomerID = "";
            this.tbSanction.CustomerScreen = null;
            this.tbSanction.DateProp = new System.DateTime(((long)(0)));
            this.tbSanction.HoldProp = false;
            this.tbSanction.Location = new System.Drawing.Point(682, 0);
            this.tbSanction.Name = "tbSanction";
            this.tbSanction.ScreenMode = "Edit";
            this.tbSanction.Settled = false;
            this.tbSanction.Size = new System.Drawing.Size(120, 24);
            this.tbSanction.TabIndex = 6;
            this.tbSanction.TabStop = false;
            this.tbSanction.Visible = false;
            // 
            // PrinterStat
            // 
            this.PrinterStat.Location = new System.Drawing.Point(0, 527);
            this.PrinterStat.Name = "PrinterStat";
            this.PrinterStat.Size = new System.Drawing.Size(0, 29);
            this.PrinterStat.TabIndex = 13;
            // 
            // MainForm
            // 
            this.ClientSize = new System.Drawing.Size(792, 553);
            this.Controls.Add(this.pbDownloading);
            this.Controls.Add(this.lDownloading);
            this.Controls.Add(this.tbSanction);
            this.Controls.Add(this.menuMain);
            this.Controls.Add(this.MainTabControl);
            this.Controls.Add(this.PrinterStat);
            this.Controls.Add(this.statusBar1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Cosacs.NET";
            this.Closing += new System.ComponentModel.CancelEventHandler(this.MainForm_Closing);
            this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.MainForm_HelpRequested);
            this.Enter += new System.EventHandler(this.MainForm_Enter);
            ((System.ComponentModel.ISupportInitialize)(this.appUpdater1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbDownloading)).EndInit();
            this.MainTabControl.ResumeLayout(false);
            this.tpLogIn.ResumeLayout(false);
            this.grpLogInCourts.ResumeLayout(false);
            this.grpLogInCourts.PerformLayout();
            this.grpLogIn.ResumeLayout(false);
            this.grpLogIn.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbSplash)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{

			//Check if CoSaCS is running.
			bool ObtainedOwnership = false;
			CosacsMutex = new Mutex(true, ConfigurationManager.AppSettings["WebServiceURL"], out ObtainedOwnership);

			if (!ObtainedOwnership)
				Environment.Exit(1);

			// Thread StartPoll = new Thread(new ThreadStart(Poll.StartPoll)); 
			try
			{
				// 18/06/08 rdb added config setting for NeoLoad proxy
				string proxyAddress = ConfigurationManager.AppSettings["ProxyAddress"];
				if (proxyAddress != null && proxyAddress.Trim() != string.Empty)
				{
					System.Net.WebProxy proxy = new System.Net.WebProxy(proxyAddress, false);
					System.Net.WebRequest.DefaultWebProxy = proxy;
				}

				STL.Common.Static.Config.Url = ConfigurationManager.AppSettings["WebServiceURL"];
				//#if !DEBUG
				//                    StartPoll.Start();
				//#endif

			}
			catch (Exception ex)
			{
				MessageBox.Show("There is a problem with CoSACS:" + Environment.NewLine + ex.Message.ToString(), "CoSACS Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}


			if (!Config.Read(Assembly.GetExecutingAssembly()))
			{
				MessageBox.Show(ResMan.GetString("M_CONFIGERROR"), ResMan.GetString("M_INFORMATION"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			else
			{
				CheckForIllegalCrossThreadCalls = false;


				//The following line gives the application a  nice XP look.
				Application.EnableVisualStyles();
				try
				{
					// hookup a global exception handler to catch and log all unhandled exceptions
					//AppDomain.CurrentDomain.UnhandledException += GlobalHandler;a
					AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
					Application.ThreadException += new ThreadExceptionEventHandler(Application_ThreadException);
					Application.Run(new MainForm());
				}
				catch (ServerConnectException ex)
				{
					if (ex.Message != "")
					{
						MessageBox.Show(ex.Message);
					}
					Application.Exit();
				}

			}

			//if (StartPoll.IsAlive)
			//{
			//    StartPoll.Abort();
			//}
		}

		static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			if (e.ExceptionObject is Exception)
				HandleException(sender, (Exception)e.ExceptionObject);
			else
			{
				const string s = "An unhandled exception has occurred. We apologise for any inconvenience. Please take screen shot and send to Cosacs support.\n";
				MessageBox.Show(s + e.ExceptionObject.ToString(), "Application Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		// private static void GlobalHandler(object sender, UnhandledExceptionEventArgs args)

		public static void HandleException(object sender, Exception e)
		{
			if (e.Message.Contains("Client Server versions do not match"))
			{
				MessageBox.Show("New CoSaCS version required. Please restart to update CoSaCS.", "New CoSaCS version required!", MessageBoxButtons.OK, MessageBoxIcon.Stop); // Hack hack hack 
				upgrading = true;
				return;
			}
			// build the log exception message
			var sb = new StringBuilder();
			sb.AppendLine(e.Message);
			sb.AppendLine(e.StackTrace);

			// first try to send the exception to the server
			var error = new WS1.Error(e);
			var handler = new ErrorPostHandler();
			handler.Proxy = new WS1.WLogin(true);
			handler.Message = sb;
			handler.Proxy.BeginException(error, handler.PostExceptionToServer, null);

			const string s = "An unhandled exception has occurred. We apologise for any inconvenience. Please take screen shot and send to Cosacs support.\n";
			MessageBox.Show(s + sb.ToString(), "Application Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

		}

		private static void Application_ThreadException(object sender, ThreadExceptionEventArgs args)
		{
			HandleException(sender, (Exception)args.Exception);
		}

		private class ErrorPostHandler
		{
			public WS1.WLogin Proxy { get; set; }
			public StringBuilder Message { get; set; }

			public void PostExceptionToServer(IAsyncResult result)
			{
				Proxy.EndException(result);
			}
		}

		public int _browsersLoaded = -1;

		public AxWebBrowser[] browsers = null;

		#region Web Service Proxy properties
		/// <summary>
		/// The web service proxy propoerties inherited from the common form will in
		/// turn point to these web service proxy properties stored on the main form.
		/// This way there should be only one set of web service proxies used for all 
		/// forms in the application, provided that each Form's FormRoot property
		/// is populated before any web service access is attempted. 
		/// </summary>
		private WEODManager _eodManager = null;
		[Browsable(false)]
		public WEODManager RootEodManager
		{
			get
			{
				try
				{
					if (_eodManager == null)
						_eodManager = new WEODManager(true);
					else
						_eodManager.Setup();
				}
				catch (Exception) { }
				return _eodManager;
			}
		}

		private WPrinting _wsprinting = null;
		[Browsable(false)]
		public WPrinting RootPrintManager
		{
			get
			{
				try
				{
					if (_wsprinting == null)
						_wsprinting = new WPrinting(true);
					else
						_wsprinting.Setup();
				}
				catch (Exception) { }
				return _wsprinting;
			}
		}

		private WStoreCard _wsstorecard = null;
		[Browsable(false)]
		public WStoreCard RootStoreCardManager
		{
			get
			{
				try
				{
					if (_wsstorecard == null)
						_wsstorecard = new WStoreCard(true);
					else
						_wsstorecard.Setup();
				}
				catch (Exception) { }
				return _wsstorecard;
			}
		}

		private WInstallation _wInstallation = null;
		[Browsable(false)]
		public WInstallation RootInstallationManager
		{
			get
			{
				try
				{
					if (_wInstallation == null)
						_wInstallation = new WInstallation(true);
					else
						_wInstallation.Setup();
				}
				catch (Exception) { }
				return _wInstallation;
			}
		}

		private WStock _wStock = null;
		[Browsable(false)]
		public WStock RootStockManager
		{
			get
			{
				try
				{
					if (_wStock == null)
						_wStock = new WStock(true);
					else
						_wStock.Setup();
				}
				catch (Exception) { }
				return _wStock;
			}
		}

		private WStaticDataManager _staticDataManager = null;
		[Browsable(false)]
		public WStaticDataManager RootStaticDataManager
		{
			get
			{
				try
				{
					if (_staticDataManager == null)
						_staticDataManager = new WStaticDataManager(true);
					else
						_staticDataManager.Setup();
				}
				catch (Exception) { }
				return _staticDataManager;
			}
		}

		private WLogin _login = null;
		[Browsable(false)]
		public WLogin RootLogin
		{
			get
			{
				try
				{
					if (_login == null)
						_login = new WLogin(true);
					else
						_login.Setup();
				}
				catch (Exception) { }
				return _login;
			}
		}

		private WAccountManager _accountManager = null;
		[Browsable(false)]
		public WAccountManager RootAccountManager
		{
			get
			{
				try
				{
					if (_accountManager == null)
						_accountManager = new WAccountManager(true);
					else
						_accountManager.Setup();
				}
				catch (Exception) { }
				return _accountManager;
			}
		}

		private WCustomerManager _customerManager = null;
		[Browsable(false)]
		public WCustomerManager RootCustomerManager
		{
			get
			{
				try
				{
					if (_customerManager == null)
						_customerManager = new WCustomerManager(true);
					else
						_customerManager.Setup();
				}
				catch (Exception) { }
				return _customerManager;
			}
		}

		private WClientLogging _clientLogging = null;
		[Browsable(false)]
		public WClientLogging RootClientLogging
		{
			get
			{
				try
				{
					if (_clientLogging == null)
						_clientLogging = new WClientLogging(true);
					else
					{
						_clientLogging.AuthenticationValue.User = Credential.User;
						_clientLogging.AuthenticationValue.Password = Credential.Password;
						_clientLogging.AuthenticationValue.Culture = Config.Culture;
						_clientLogging.AuthenticationValue.Country = Config.CountryCode;
						_clientLogging.Url = Config.Url + "WClientLogging.asmx";
					}
				}
				catch (Exception) { }
				return _clientLogging;
			}
		}

		private WSystemConfig _systemConfig = null;
		[Browsable(false)]
		public WSystemConfig RootSystemConfig
		{
			get
			{
				try
				{
					if (_systemConfig == null)
						_systemConfig = new WSystemConfig(true);
					else
						_systemConfig.Setup();
				}
				catch (Exception) { }
				return _systemConfig;
			}
		}

		private WCreditManager _creditManager = null;
		[Browsable(false)]
		public WCreditManager RootCreditManager
		{
			get
			{
				try
				{
					if (_creditManager == null)
						_creditManager = new WCreditManager(true);
					else
						_creditManager.Setup();
				}
				catch (Exception) { }
				return _creditManager;
			}
		}

		private WSetData _setData = null;
		[Browsable(false)]
		public WSetData RootSetData
		{
			get
			{
				try
				{
					if (_setData == null)
						_setData = new WSetData(true);
					else
						_setData.Setup();
				}
				catch (Exception) { }
				return _setData;
			}
		}

		private WServiceManager _serviceManager = null;
		[Browsable(false)]
		public WServiceManager RootServiceManager
		{
			get
			{
				try
				{
					if (_serviceManager == null)
						_serviceManager = new WServiceManager(true);
					else
						_serviceManager.Setup();
				}
				catch (Exception) { }
				return _serviceManager;
			}
		}

		private WCollectionsManager _collectionsManager = null;
		[Browsable(false)]
		public WCollectionsManager RootCollectionsManager
		{
			get
			{
				try
				{
					if (_collectionsManager == null)
						_collectionsManager = new WCollectionsManager(true);
					else
						_collectionsManager.Setup();
				}
				catch (Exception) { }
				return _collectionsManager;
			}
		}

		private WPaymentManager _paymentManager = null;

		private void MainForm_Enter(object sender, System.EventArgs e)
		{
			/* this is an attempt to solve the problem where the user is unable 
			 * to exit the application */
			this.OnControlRemoved(new ControlEventArgs(MainTabControl));
		}

		private void menuImageManagement_Click(object sender, System.EventArgs e)
		{
			ImageManagement i = new ImageManagement(this, this);
			AddTabPage(i, 28);
		}

		private void menuCashier_Click(object sender, System.EventArgs e)
		{
			CashierTotals ct = new CashierTotals(this, this);

			AddTabPage(ct);

			if (ct.tabClosed == true)       //#10400 - LW74981 - found error whilst fixing this issue.
			{
				ct.CloseTab();
			}
		}

		private void menuTransferTransaction_Click(object sender, System.EventArgs e)
		{
			TransferTransaction tt = new TransferTransaction(this, this);
			AddTabPage(tt);
		}

		private void menuJournal_Click(object sender, System.EventArgs e)
		{
			JournalEnquiry je = new JournalEnquiry(this, this);
			AddTabPage(je);
		}

		private void menuCorrection_Click(object sender, System.EventArgs e)
		{
			RefundAndCorrection c = new RefundAndCorrection(this, this);
			AddTabPage(c);
		}

		private void menuReturnCheque_Click(object sender, System.EventArgs e)
		{
			ReturnCheque rc = new ReturnCheque(this, this);
			AddTabPage(rc);
		}

		private void menuDisbursements_Click(object sender, System.EventArgs e)
		{
			CashierDisbursments cd = new CashierDisbursments(this, this);
			AddTabPage(cd);
		}

		private void menuDeposits_Click(object sender, System.EventArgs e)
		{
			CashierDeposits cd = new CashierDeposits(this, this);
			AddTabPage(cd);
		}

		public void ShowStatus(string message, int time = 10)
		{
			ThreadStart work = delegate
			{
				SetStatus(message, time);
			};
			new Thread(work).Start();
		}

		private void SetStatus(string message, int time)
		{
			statusBar1.Text = message;
			Thread.Sleep(new TimeSpan(0, 0, time));
			statusBar1.Text = "";
		}

		private void menuCancel_Click(object sender, System.EventArgs e)
		{
			CancelAccount ca = new CancelAccount(this, this, false);
			ca.ShowDialog();
		}

		private void menuDDMandate_Click(object sender, System.EventArgs e)
		{
			DDMandate dm = new DDMandate(this, this);
			AddTabPage(dm);
		}

		private void menuDDPaymentExtra_Click(object sender, System.EventArgs e)
		{
			DDPaymentExtra dp = new DDPaymentExtra(this, this);
			AddTabPage(dp);
		}

		private void menuDDRejection_Click(object sender, System.EventArgs e)
		{
			DDRejection dr = new DDRejection(this, this);
			AddTabPage(dr);
		}

		private void menuTransTypeMaintenance_Click(object sender, System.EventArgs e)
		{
			TransTypeMaint tt = new TransTypeMaint(this, this);
			tt.ShowDialog();
			//AddTabPage(tt);
		}

		private void menuChangePassword_Click_1(object sender, System.EventArgs e)
		{
			//68692  Password prompt would display and error if no user logged in.  Fixed [PC]
			if (Credential.User != null)        //68865 jec 14/3/07
			{
				ChangePassword cp = new ChangePassword(this, this, Credential.Password, Credential.Name, Credential.User);
				cp.ShowDialog();
			}
			else
				ShowInfo("M_PASSWORDCHANGEREQUIRESLOGIN");

		}

		private void menuLocationEnquiry_Click(object sender, System.EventArgs e)
		{
			StockEnquiryByLocation.FormParent = this;
			StockEnquiryByLocation.FormRoot = this;
			AddTabPage(StockEnquiryByLocation);
		}

		private void menuProductEnquiry_Click(object sender, System.EventArgs e)
		{
			CodeStockEnquiry stock = new CodeStockEnquiry();
			stock.FormParent = this;
			stock.FormRoot = this;
			AddTabPage(stock);
		}

		private void menuWOReview_Click(object sender, System.EventArgs e)
		{
			WriteOffReview wo = new WriteOffReview(this, this);
			AddTabPage(wo);
		}

		private void menuExchangeRates_Click(object sender, System.EventArgs e)
		{
			ExchangeRates er = new ExchangeRates(this, this);
			AddTabPage(er);
		}

		private void menuSellGiftVoucher_Click(object sender, System.EventArgs e)
		{
			GiftVoucher gv = new GiftVoucher(this, this, true);
			gv.ShowDialog();
		}

		private void menuStatusCode_Click(object sender, System.EventArgs e)
		{
			StatusCode stat = new StatusCode(this, this);
			AddTabPage(stat);
		}

		private void menuEOD_Click(object sender, System.EventArgs e)
		{
			EODControl eod = new EODControl(this, this);
			AddTabPage(eod);
		}

		private void menuCountryMaintenance_Click(object sender, System.EventArgs e)
		{
			CountryMaintenance cm = new CountryMaintenance(this, this);
			AddTabPage(cm);
		}

		[Browsable(false)]
		public WPaymentManager RootPaymentManager
		{
			get
			{
				try
				{
					if (_paymentManager == null)
						_paymentManager = new WPaymentManager(true);
					else
					{
						//_paymentManager.AuthenticationValue.User = Credential.User;
						//_paymentManager.AuthenticationValue.Password = Credential.Password;
						//_paymentManager.AuthenticationValue.Culture = Config.Culture;
						//_paymentManager.Url = Config.Url + "WPaymentManager.asmx";
						_paymentManager.Setup();  //# 12850
					}

					_paymentManager.Timeout = 1800000;
				}
				catch (Exception) { }
				return _paymentManager;
			}
		}
		#endregion

		private CountryParameterCollection _rootcountry = null;

		[Browsable(false)]
		public CountryParameterCollection RootCountry
		{
			get
			{
				if (_rootcountry == null)
				{
					DataSet ds = StaticDataManager.GetCountryMaintenanceParameters(Config.CountryCode, out Error);
					if (Error.Length > 0)
						ShowError(Error);
					else
					{
						_rootcountry = new CountryParameterCollection(ds.Tables[TN.CountryParameters]);
						StaticDataSingleton.Instance().Data["Country"] = _rootcountry;
						var dsCache = StaticDataManager.GetStcokItemCache(out Error);

						if (Error.Length > 0)
							ShowError(Error);
						else if (dsCache != null && dsCache.Tables.Count > 0)
						{
							StockItemCache.Invalidate(dsCache.Tables[0]);
						}
					}
				}
				return _rootcountry;
			}

			set { _rootcountry = value; }
		}

		private void MainTabControl_SelectionChanged(object sender, System.EventArgs e)
		{
			Crownwood.Magic.Controls.TabPage tp = MainTabControl.SelectedTab;
			this.Controls.Remove(menuMain);
			this.menuMain = (Crownwood.Magic.Menus.MenuControl)tp.Tag;
			this.Controls.Add(menuMain);
			statusBar1.Text = "";
		}

		public void MainTabControl_ClosePressed(object sender, System.EventArgs e)
		{
			Function = "MainTabControl_ClosePressed";
			try
			{
				if (MainTabControl.SelectedIndex > 0 &&
					MainTabControl.SelectedIndex < MainTabControl.TabPages.Count)
				{
					Crownwood.Magic.Controls.TabPage tp = MainTabControl.SelectedTab;
					if (tp != null)
					{
						if (((CommonForm)tp.Control).ConfirmClose())
						{
							MainTabControl.TabPages.Remove(tp);
							this.OnControlRemoved(new System.Windows.Forms.ControlEventArgs(tp));
							statusBar1.Text = "";

							//if (((CommonForm)tp.Control).Name == "SR_ServiceRequest")
							//{
							//    SR_ServiceRequest SR = new SR_ServiceRequest(this.FormRoot, this, "", "", "");
							//    SR.UnlockAccount();
							//    SR.Dispose();
							//}

							//Don't call this method if the tab being closed is LocationStockEnquiry
							if (((CommonForm)tp.Control).Name != "LocationStockEnquiry")
							{
								if ((tp.Control is Payment) == false || (tp.Control as Payment).AvoidControlDispose == false)
								{
									DisposeControls((CommonForm)tp.Control);
								}
								tp.Dispose();
							}

							if (tp.Control is NewAccount && ((NewAccount)tp.Control).MagStripeReader != null)
								((NewAccount)tp.Control).MagStripeReader.Close();

							if (tp.Control is Payment && ((Payment)tp.Control).MagStripeReader != null)
								((Payment)tp.Control).MagStripeReader.Close();
						}
					}

				}
			}
			catch (Exception ex)
			{
				Catch(ex, Function);
			}
		}

		private void DisposeControls(Control control)
		{
			// The following code has been commented out as some child controls
			// were disposed in the parent dispose following.       jec UAT559/529
			// May need to be reinstated with modifications if needed.

			// Dispose Parent 
			if (!control.IsDisposed)
			{
				control.Dispose();
			}
			GC.SuppressFinalize(control);
		}

		public void AddTabPage(CommonForm page)
		{
			Function = "AddTabPage";
			try
			{
				page.TranslateControls();
				if (page.menuMain != null)
					TranslateMenus(page.menuMain.MenuCommands);
				Crownwood.Magic.Controls.TabPage tp = new Crownwood.Magic.Controls.TabPage(page.Text);
				tp.Control = page;
				tp.Tag = page.menuMain;
				tp.Selected = true;
				MainTabControl.TabPages.Add(tp);
				statusBar1.Text = "";
			}
			catch (Exception ex)
			{
				Catch(ex, Function);
			}
		}

		public void AddTabPage(CommonForm page, int iconIndex)
		{
			Function = "AddTabPage";
			try
			{
				page.TranslateControls();
				if (page.menuMain != null)
					TranslateMenus(page.menuMain.MenuCommands);
				Crownwood.Magic.Controls.TabPage tp = new Crownwood.Magic.Controls.TabPage(page.Text);
				tp.Control = page;
				tp.Tag = page.menuMain;
				tp.Selected = true;
				tp.ImageList = menuIcons;
				tp.ImageIndex = iconIndex;
				MainTabControl.TabPages.Add(tp);
				statusBar1.Text = "";
			}
			catch (Exception ex)
			{
				Catch(ex, Function);
			}
		}

		private void LogIn()
		{
			Function = "LogIn";

			Wait();
			grpLogInCourts.Enabled = false;
			grpLogIn.Enabled = false;

			if (txtUser.Text.Length != 0 || txtUserCourts.Text.Length != 0)
			{
				statusBar1.Text = "Validating credentials";

				Credential.User = txtUser.Text;
				Credential.Password = txtPassword.Text;
				new ServerCheck().CheckConnection(Assembly.GetExecutingAssembly().GetName().Version.ToString(), Config.BranchCode, (changePassword, name) => PostLogIn(changePassword, name), (Exception) => ShowLoginError(Exception), () => EnableCredentials());
                

            }
		}

		private void EnableCredentials()
		{
			grpLogIn.Enabled = true;
			DecrementProcessCounter();
		}

		private void ShowLoginError(Exception e)
		{
			HandleException(this, e);
		}

		private void PostLogIn(bool changePassword, string userName)
		{
			try
			{
				//TimeSpan ts;

				// Make sure we use the CoSACS Employee No
				//Credential.User = empeeNo;
				this.menuChangePassword.Enabled = true;/* KEF 66566 Fix change password error */
				this.menuChangePassword.Visible = true;/* KEF 66566 Fix change password error */

				// valid = CheckConfig();

				InitialiseStaticData();

				statusBar1.Text = "Loading branch list";
				ValidateBranchNo();

				if (Country != null)
				{
					statusBar1.Text = "Loading country specific parameters";

					//GetCountryDefaults(Config.CountryCode);

					if (!(bool)Country[CountryParameterNames.SystemOpen])
					{
						ShowInfo("M_SYSTEMCLOSED");
						return;
					}

					//                            DateTime datePassChge = Login.GetDatePasswordChanged(Credential.UserId, out Error);

					//var ts = DateTime.Now - datePassChge;

					if (changePassword) // ts.Days > Convert.ToInt32(Country[CountryParameterNames.PasswordChangeDays]) || txtPassword.Text.Length == 0)
					{
						var cp = new ChangePassword(this, this, txtPassword.Text, Credential.Name, Credential.User);
						cp.ShowDialog();

						if (!cp.status)
						{
							MessageBox.Show(GetResource("M_PASSWORDCHANGEFAILED"));
							return;
						}
					}

					//CR903 If this is a Courts branch then show the splash image
					pbSplash.Visible = (Config.StoreType == StoreType.Courts);
				}

				menuLogOff.Enabled = String.IsNullOrEmpty(Credential.User);

				/* finished loading everything now set them as logged in */

				menuLogOff.Enabled = true;

				txtPassword.Text = "";
				txtPasswordCourts.Text = "";

				this.Text = GetResource("M_TITLETEXT", new object[] { Credential.User, Credential.Name, Config.BranchCode, Config.Server });

				this.menuConfig.Enabled = false;
				this.menuConfig.Visible = false;

				CheckCashierDeposits();
				statusBar1.Text = "";
				grpLogIn.Text = "Log In";
				grpLogIn.Visible = false;
				// grpLogInCourts.Visible = false;
				SplashAfterLogin();                
                if (Config.CashDrawerID.Length > 0 && openTill.Enabled && Credential.IsInRole("C"))
				{
					int empeeno = Login.CashTillOpenLoadEmployee(Credential.UserId, Config.CashDrawerID, out Error);
					if (Error.Length > 0)
					{
						ShowError(Error);
					}
					else
					{
						if (empeeno == 0)
						{
							OpenCashDrawer(false);
						}
					}
				}
			}
			catch (Exception ex)
			{
				Catch(ex, Function);
			}
			finally
			{
				StopWait();
				// TODO progress
				// progress.Visible = false;
			}
		}

		public bool CheckCashierDeposits()
		{
			bool mustDeposit = false;
			PaymentManager.CashierMustDeposit(Credential.UserId, out mustDeposit, out Error);
			if (Error.Length > 0)
				ShowError(Error);
			else
			{
				if (mustDeposit)
					ShowInfo("M_MUSTDEPOSIT");
			}

			if (mustDeposit)
			{
				ApplyRoleRestrictions();
				var cashierDepositsOpen = false;

				var pages = 0;
				foreach (Crownwood.Magic.Controls.TabPage tp in this.MainTabControl.TabPages)
				{ /* count the open pages other than mainform and cashier deposits */
					if (!(tp.Title == GetResource("Main") ||
						tp.Control is CashierDeposits))
					{
						pages++;
					}
					else
					{
						if (tp.Control is CashierDeposits)
							cashierDepositsOpen = true;
					}
				}

				Crownwood.Magic.Controls.TabPage[] tps = new Crownwood.Magic.Controls.TabPage[pages];
				int i = 0;
				foreach (Crownwood.Magic.Controls.TabPage tp in this.MainTabControl.TabPages)
				{
					if (!(tp.Title == GetResource("Main") ||
						tp.Control is CashierDeposits))
					{
						tps[i++] = tp;
					}
				}

				for (i = 0; i < tps.Length; i++)
					CloseTab((CommonForm)tps[i].Control);

				if (menuDeposits.Enabled && !cashierDepositsOpen)
					this.menuDeposits_Click(null, null);

				/* all open screens will have been closed apart from 
				 * cashierdeposits. Some screens may have accounts locked
				 * so we should unlock all accounts that this user has locked
				 * as a precaution */
				AccountManager.UnlockAccount("", Credential.UserId, out Error);
				if (Error.Length > 0)
					ShowError(Error);

				ResetMenus();
			}
			else
			{
				ApplyRoleRestrictions();
			}
			MenuOverride();
			return !mustDeposit;
		}

		private bool ValidateBranchNo()
		{
			bool valid = true;

			XmlUtilities xml = new XmlUtilities();
			XmlDocument dropDowns = new XmlDocument();
			dropDowns.LoadXml("<DROP_DOWNS></DROP_DOWNS>");

			if (StaticData.Tables[TN.BranchNumber] == null)
				dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.BranchNumber, null));
			if (dropDowns.DocumentElement.ChildNodes.Count > 0)
			{
				DataSet ds = StaticDataManager.GetDropDownData(dropDowns.DocumentElement, out Error);
				if (Error.Length > 0)
					ShowError(Error);
				else
				{
					foreach (DataTable dt in ds.Tables)
					{
						StaticData.Tables[dt.TableName] = dt;
					}
				}
			}
			//DataTable branches = (DataTable)StaticData.Tables[TN.BranchNumber];
			//branches.DefaultView.RowFilter = CN.BranchNo + " = " + Config.BranchCode;
			//if (branches.DefaultView.Count == 0)
			//    valid = false;
			//branches.DefaultView.RowFilter = "";

			return valid;
		}

		private void txtPassword_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (!Closed)
				this.LogIn();
		}


		private void menuNewAccount_Click(object sender, System.EventArgs e)
		{
			NewAccount page = new NewAccount(false, this, this);
			this.AddTabPage(page, 4);
		}

		private void menuStatements_Click(object sender, System.EventArgs e)
		{
			if (!Config.ThermalPrintingEnabled)
			{
				MessageBox.Show
				(
					"Thermal Printing is Not Enabled\n\n" +
					"This screen is not available without thermal printing. Please install a thermal printer and enable this on the Config File Maintenance screen to continue.",
					"Statements",
					MessageBoxButtons.OK,
					MessageBoxIcon.Exclamation
				);
			}
			else
			{
				StatementsForm page = new StatementsForm(this);
				this.AddTabPage(page, 4);
			}
		}
		public void OpenStatementsTab(string customerID)
		{
			if (!Config.ThermalPrintingEnabled)
			{
				MessageBox.Show
				(
					"Thermal Printing is Not Enabled\n\n" +
					"This screen is not available without thermal printing. Please install a thermal printer and enable this on the Config File Maintenance screen to continue.",
					"Statements",
					MessageBoxButtons.OK,
					MessageBoxIcon.Exclamation
				);
			}
			else
			{
				StatementsForm page = new StatementsForm(customerID, this);
				this.AddTabPage(page, 4);
			}
		}
		public void OpenStatementsTab(string accountNo, string customerID)
		{
			if (!Config.ThermalPrintingEnabled)
			{
				MessageBox.Show
				(
					"Thermal Printing is Not Enabled\n\n" +
					"This screen is not available without thermal printing. Please install a thermal printer and enable this on the Config File Maintenance screen to continue.",
					"Statements",
					MessageBoxButtons.OK,
					MessageBoxIcon.Exclamation
				);
			}
			else
			{
				StatementsForm page = new StatementsForm(customerID, accountNo, this);
				this.AddTabPage(page, 4);
			}
		}

		private void menuAccountRevise_Click(object sender, System.EventArgs e)
		{
			AccountSearch page = new AccountSearch();
			page.Revise = true;
			page.Details = true;
			page.FormParent = this;
			page.FormRoot = this;
			this.AddTabPage(page, 9);
		}

		private void menuAccountDetails_Click(object sender, System.EventArgs e)
		{
			AccountSearch page = new AccountSearch();
			page.Details = true;
			page.FormParent = this;
			page.FormRoot = this;
			this.AddTabPage(page, 9);
		}

		private void txtUser_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			try
			{
				txtUser.Text = txtUser.Text.Trim();
				if (txtUser.Text.Length > 0)
				{
					errorProvider1.SetError(txtUser, "");
				}
				else
				{
					txtUser.Focus();
					txtUser.Select(0, txtUser.Text.Length);
					errorProvider1.SetError(txtUser, GetResource("M_ENTERMANDATORY"));
				}
			}
			catch (Exception ex)
			{
				txtUser.Focus();
				txtUser.Select(0, txtUser.Text.Length);
				errorProvider1.SetError(txtUser, ex.Message);
			}
		}

		private void menuAccountCodes_Click(object sender, System.EventArgs e)
		{
			AddCustAcctCodes codes = new AddCustAcctCodes(false, "", "", "", "000-0000-0000-0");
			codes.FormRoot = this;
			codes.FormParent = this;
			AddTabPage(codes, 8);
		}

		private void menuExit_Click(object sender, System.EventArgs e)
		{
			Closed = true;
			Close();
		}

		private void menuLogOff_Click(object sender, System.EventArgs e)
		{
			try
			{
				if (MainTabControl.TabPages.Count > 1)
				{
					ShowInfo("M_CLOSEALLWINDOWS");
				}
				else
				{
					ResetMenus();
					//grpLogIn.Visible = true;
					SplashAfterLogout();
					txtUser.Text = "";
					txtPassword.Text = "";
					txtUserCourts.Text = "";
					txtPasswordCourts.Text = "";
					this.Text = "Cosacs.NET";
					menuTransaction.Visible = true;
					menuTransaction.Enabled = true;

					grpLogIn.Enabled = true;

					if (!upgrading)
					{
						AccountManager.UnlockAccount("", Credential.UserId, out Error);
						if (Error.Length > 0)
							ShowError(Error);

						AccountManager.UnlockItem(Credential.UserId, out Error);
						if (Error.Length > 0)
							ShowError(Error);

						Login.LogOff(Environment.MachineName, Credential.UserId, Credential.User);
					}
					//CR903 Display LogIn group Box according to branch type
					//if (Config.StoreType == StoreType.Courts)
					//{
					//    grpLogIn.Visible = false;
					//    grpLogInCourts.Visible = true;
					//    pbSplash.Visible = true;
					//    txtUserCourts.Focus();
					//}
					//else
					//{
					grpLogIn.Visible = true;
					//grpLogInCourts.Visible = false;
					//pbSplash.Visible = false;
					txtUser.Focus();
					//}
				}
			}
			catch (Exception ex)
			{
				Catch(ex, "menuLogOff_Click");
			}
		}

		private void menuCustomerSearch_Click(object sender, System.EventArgs e)
		{
			CustomerSearch cust = new CustomerSearch();
			cust.FormRoot = this;
			cust.FormParent = this;
			AddTabPage(cust, 9);
		}
        private void RePrintInvoice_Click(object sender, System.EventArgs e)
        {
            ReprintInvoice cust = new ReprintInvoice(this,this);
            cust.FormRoot = this;
            cust.FormParent = this;
            AddTabPage(cust, 9);

        }

        private void menuViewStoreCard_Click(object sender, System.EventArgs e)
		{
			var sc = new StoreCard_View(this);
			AddTabPage(sc, 9);
		}

		private void MainForm_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			//errorProvider1.Dispose();

			string path = Application.StartupPath;
			//First try deleting all the jpg's that have not already been deleted for customer photos
			System.IO.DirectoryInfo fi = new System.IO.DirectoryInfo(path);
			System.IO.FileInfo[] f = fi.GetFiles("*.jpg");
			int n = fi.GetFiles("*.jpg").Length;
			for (int i = 0; i < n; i++)
			{
				if (f[i].Name != "ErrorScreenShot.jpg")
				{
					try
					{
						f[i].Delete();
					}
					catch
					{
						//Try to delete the file. Won't delete if currently being used.
					}
				}
			}

			//Next delete any temp files created by the web cam that have not been used
			System.IO.FileInfo[] temp = fi.GetFiles("*.tmp");
			int m = fi.GetFiles("*.tmp").Length;
			for (int i = 0; i < m; i++)
			{
				try
				{
					temp[i].Delete();
				}
				catch
				{
					//Try to delete the file. Won't delete if currently being used.
				}
			}
		}

		private void menuCustomiseMenus_Click(object sender, System.EventArgs e)
		{
			MenuMaintenance menus = new MenuMaintenance(this, this);
			AddTabPage(menus);
		}

		private void menuManualSale_Click(object sender, System.EventArgs e)
		{
			NewAccount page = new NewAccount(true, this, this);
			page.Text = "Manual Sale";
			this.AddTabPage(page, 5);
		}

		private void menuMandatory_Click(object sender, System.EventArgs e)
		{
			MandatoryFields m = new MandatoryFields();
			m.FormRoot = this;
			m.FormParent = this;
			AddTabPage(m);
		}

		private void menuNewCustomer_Click(object sender, System.EventArgs e)
		{
			Function = "menuNewCustomer_Click";
			BasicCustomerDetails details = new BasicCustomerDetails(true, this, this);
			AddTabPage(details, 10);
			details.loaded = true;
		}

		private void menuScoringRules_Click(object sender, System.EventArgs e)
		{
			ScoringRules rules = new ScoringRules(this, this);
			AddTabPage(rules);
		}

		private void txtPassword_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
		{
			if (e.KeyChar == (char)13)
				this.LogIn();
		}

		private void menuConfig_Click(object sender, System.EventArgs e)
		{
			ConfigMaintenance config = new ConfigMaintenance();
			config.FormRoot = this;
			config.FormParent = this;
			AddTabPage(config, 0);

		}

		private void menuVersion_Click(object sender, System.EventArgs e)
		{
			About a = new About();
			a.ShowDialog();
		}

		private void menuScoringMatrix_Click(object sender, System.EventArgs e)
		{
			ScoringMatrix matrix = new ScoringMatrix(this, this);
			AddTabPage(matrix, 14);
		}

		private void menuTTMatrix_Click(object sender, System.EventArgs e)
		{
			ScoringbandMatrix ttMatrix = new ScoringbandMatrix(this, this);
			AddTabPage(ttMatrix, 14);
		}

		private void menuDependentSpendMatrix_Click(object sender, System.EventArgs e)
        {
            DependentExpenseCriteria depExpenseMatrix = new DependentExpenseCriteria(this);
            depExpenseMatrix.FormParent = this;
            AddTabPage(depExpenseMatrix);

        }

        private void menuApplicantSpendMatrix_Click(object sender, System.EventArgs e)
        {
            ApplicantExpenseCriteria appExpenseMatrix = new ApplicantExpenseCriteria(this);
            appExpenseMatrix.FormParent = this;
            AddTabPage(appExpenseMatrix);

        }
		private void menuIncomplete_Click(object sender, System.EventArgs e)
		{
			Incomplete i = new Incomplete(this, this);
			AddTabPage(i, 15);
		}

		private void menuAuthoriseDelivery_Click(object sender, System.EventArgs e)
		{
			DeliveryAuthorisation da = new DeliveryAuthorisation(this, this);
			da.FormRoot = this;
			da.FormParent = this;
			AddTabPage(da, 25);
		}

		private void menuAuthoriseIC_Click(object sender, System.EventArgs e)
		{
			ICAuthorisation da = new ICAuthorisation(this, this);
			da.FormRoot = this;
			da.FormParent = this;
			AddTabPage(da, 25);
		}

		private void menuCredit_Click(object sender, System.EventArgs e)
		{
			SearchCashAndGo search = new SearchCashAndGo(this, this);
			AddTabPage(search);
		}

		private void menuPaidAndTaken_Click(object sender, System.EventArgs e)
		{
			string acctNo = "";
			//find the paid and taken account for this branch and then 
			//launch revise account screen
			try
			{
				Function = "menuPaidAndTaken_Click";
				Wait();
				acctNo = AccountManager.GetPaidAndTakenAccount(Config.BranchCode, out Error);
				if (Error.Length > 0)
					ShowError(Error);
				else
				{
					if (acctNo.Length != 0)
					{
						NewAccount revise = new NewAccount(acctNo, 1, null, false, this, this);
						revise.Text = "Cash And Go";
						revise.btnPrint.Visible = false;
						if (revise.AccountLoaded)
						{
							AddTabPage(revise, 23);
							revise.SupressEvents = false;
						}
					}
					else
						ShowInfo("M_NOPAIDANDTAKEN");
				}
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

		private void menuTranslation_Click(object sender, System.EventArgs e)
		{
			StockItemTranslations s = new StockItemTranslations(this);
			s.FormParent = this;
			AddTabPage(s);
		}

		private void menuGoodsReturn_Click(object sender, System.EventArgs e)
		{
			GoodsReturn g = new GoodsReturn(this, this);
			g.FormRoot = this;
			g.FormParent = this;
			AddTabPage(g);
		}

		private void menuPayment_Click(object sender, System.EventArgs e)
		{
			Payment p = new Payment("", "", 0, this, this);
			AddTabPage(p, 26);
		}

		private void menuGeneralTransactions_Click(object sender, System.EventArgs e)
		{
			GeneralFinancialTransactions gft = new GeneralFinancialTransactions(this, this);
			AddTabPage(gft);
		}

		private void menuScreenTranslation_Click(object sender, System.EventArgs e)
		{
			Localisation l = new Localisation();
			l.FormRoot = this;
			l.FormParent = this;
			AddTabPage(l);
		}

		private void menuFollowUp_Click(object sender, System.EventArgs e)
		{
			//CR852 Collections
			//FollowUp fu = new FollowUp(this, this);
			FollowUp5_2 fu = new FollowUp5_2(this, this);
			AddTabPage(fu);
		}

		private void menuTelephoneAction_Click(object sender, System.EventArgs e)
		{
			//CR852 Collections
			//TelephoneAction ta = new TelephoneAction(this, this, true);
			TelephoneAction5_2 ta = new TelephoneAction5_2(this, this, true);
			AddTabPage(ta);
		}

		private void menuBailiffReview_Click(object sender, System.EventArgs e)
		{
			//CR852 Collections
			//TelephoneAction ta = new TelephoneAction(this, this, false);
			BailReview5_2 ta = new BailReview5_2(this, this, false);
			AddTabPage(ta);
		}

		private void menuSearchCashAndGo_Click(object sender, System.EventArgs e)
		{
			SearchCashAndGo search = new SearchCashAndGo(this, this);
			AddTabPage(search);
		}

		private void menuAccount_Click(object sender, System.EventArgs e)
		{

		}

		private void meneOneForOneReplacement_Click(object sender, System.EventArgs e)
		{
		}

		private void menuCodeMaintenance_Click(object sender, System.EventArgs e)
		{
			CodeMaintenance cm = new CodeMaintenance(this);
			AddTabPage(cm);
		}

		private void menuEmployeeMaintenance_Click(object sender, System.EventArgs e)
		{
			StaffMaintenance sm = new StaffMaintenance(this, this);
			AddTabPage(sm);
		}

		private void menuSysMaint_Click(object sender, System.EventArgs e)
		{

		}

		private void menuBranch_Click(object sender, System.EventArgs e)
		{
			Branch b = new Branch();
			b.FormRoot = this;
			b.FormParent = this;
			AddTabPage(b);

		}

		private void menuCashAndGoReturn_Click(object sender, System.EventArgs e)
		{
			try
			{
				Wait();

				string acctno = AccountManager.GetPaidAndTakenAccount(Config.BranchCode, out Error);
				if (Error.Length > 0)
					ShowError(Error);
				else
				{
					NewAccount revise = new NewAccount(acctno, 1, null, true, FormRoot, this);
					revise.Collection = true;
					revise.Text = GetResource("P_CASH_GO_RETURN");
					revise.btnPrint.Visible = false;
					if (revise.AccountLoaded)
					{
						((MainForm)FormRoot).AddTabPage(revise, 24);
						revise.SupressEvents = false;
					}
					revise.IsLegacyPTReturn = true; //LW 71731 - To allow warranties to be added to the lineitem grid //IP - 17/02/10 - CR1072 - LW 71731 - Cash&Go Fixes from 4.3
				}
			}
			catch (Exception ex)
			{
				Catch(ex, "menuCashAndGoReturn_Click");
			}
			finally
			{
				StopWait();
			}
		}

		private void menuUpdateDateDue_Click(object sender, System.EventArgs e)
		{
			UpdateDateDue udd = new UpdateDateDue(this, this);
			AddTabPage(udd);
		}

		private void menuInterfaceReport_Click(object sender, System.EventArgs e)
		{
			InterfaceBreakdown ib = new InterfaceBreakdown(this, this, true);
			AddTabPage(ib);
		}

		private void menuTermsType_Click(object sender, System.EventArgs e)
		{
			TermsType tt = new TermsType(this, this);
			AddTabPage(tt);
		}

        private void menuMmiMatrix_Click(object sender, System.EventArgs e)
        {
            MmiMatrix mm = new MmiMatrix(this, this);
            AddTabPage(mm);
        }
        

        private void menuDeliveryArea_Click(object sender, System.EventArgs e)
		{
			SetSelection selection = new SetSelection(GetResource("T_DELIVERYAREA"), 45, 193, 8, null, TN.TNameDeliveryArea, TN.DeliveryArea, true);
			selection.FormRoot = this.FormRoot;
			selection.FormParent = this;
			selection.Text = GetResource("T_DELIVERYAREA") + ' ' + GetResource("T_MAINTENANCE");
			AddTabPage(selection);
		}


		private void menuNumberGeneration_Click(object sender, System.EventArgs e)
		{
			NumberGeneration ng = new NumberGeneration();
			ng.FormRoot = this;
			ng.FormParent = this;
			AddTabPage(ng);
		}

		private void menuMonitorBookings_Click(object sender, System.EventArgs e)
		{
			MonitorBookings mb = new MonitorBookings(this, this);
			AddTabPage(mb);
		}

		private void menuReverseCancel_Click(object sender, System.EventArgs e)
		{
			CancelAccount reverse = new CancelAccount(this, this, true);
			reverse.Text = "Reverse Cancelled Account";
			reverse.ShowDialog();
		}

		private void menuCashierByBranch_Click(object sender, System.EventArgs e)
		{
			CashierByBranch cb = new CashierByBranch(this, this);
			AddTabPage(cb);
		}

		//private void menuMonitorDeliveries_Click(object sender, System.EventArgs e)
		//{
		//    MonitorDeliveries md = new MonitorDeliveries(this, this);
		//    AddTabPage(md);
		//}

		private void menuWarrantyRenewals_Click(object sender, System.EventArgs e)
		{
			WarrantyRenewals wr = new WarrantyRenewals(null, "", this.FormRoot, this);
			wr.ShowDialog();		//launch as a modal dialog

		}

		private void menuFinTransQuery_Click(object sender, System.EventArgs e)
		{
			FinTransQuery ft = new FinTransQuery();
			ft.FormRoot = this;
			ft.FormParent = this;
			AddTabPage(ft);
		}

		private void menuTempReceiptInvest_Click(object sender, System.EventArgs e)
		{
			TemporaryReceiptsDetails tr = new TemporaryReceiptsDetails(this.FormRoot, this);
			AddTabPage(tr);
		}

		private void menuSumryUpdControl_Click(object sender, System.EventArgs e)
		{
			SummaryReportControlMain scr = new SummaryReportControlMain(this, this);
			AddTabPage(scr);
		}


		private void menuReprintActionSheet_Click(object sender, System.EventArgs e)
		{
			ReprintActionSheet ra = new ReprintActionSheet(this, this);
			AddTabPage(ra);
		}

		private void menuRedeliverReposs_Click(object sender, System.EventArgs e)
		{
			RedeliveryAfterRepossesson rar = new RedeliveryAfterRepossesson(this.FormRoot, this);
			AddTabPage(rar);
		}

		private void menuRebateCalculation_Click(object sender, System.EventArgs e)
		{
			RebateCalculation rc = new RebateCalculation(this, this);
			AddTabPage(rc);
		}

		private void menuUnpaidAccounts_Click(object sender, System.EventArgs e)
		{
			UnpaidAccounts ua = new UnpaidAccounts(this, this);
			AddTabPage(ua);
			//AddTabPage clears out the status bar which is no good for this screen, put
			//the message back!
			ua.SetStatusBar();
		}

		private void menuAmendReprintPicklist_Click(object sender, System.EventArgs e)
		{
			AmandPrintPickList ap = new AmandPrintPickList(this.FormRoot, this);
			AddTabPage(ap);
		}

		private void menuAcctNoCtrl_Click(object sender, System.EventArgs e)
		{
			AccountNumberCtrl anc = new AccountNumberCtrl(this.FormRoot, this);
			AddTabPage(anc);
			//AddTabPage clears out the status bar which is no good for this screen, put
			//the message back!
			anc.SetStatusBar();
		}

		private void menuRebateReport_Click(object sender, System.EventArgs e)
		{
			RebateReport rr = new RebateReport(this, this);
			AddTabPage(rr);
		}

		private void menuCommissionMaint_Click(object sender, System.EventArgs e)
		{
			BailiffCommissionMaintenance cm = new BailiffCommissionMaintenance(this.FormRoot, this);
			AddTabPage(cm);
		}

		private void menuCalcBailCommission_Click(object sender, System.EventArgs e)
		{
			CalculateBailiffCommission cb = new CalculateBailiffCommission(this.FormRoot, this);
			AddTabPage(cb);
		}

		private void menuReprintBailCommn_Click(object sender, System.EventArgs e)
		{
			ReprintBailiffCommission rbc = new ReprintBailiffCommission(this.FormRoot, this);
			AddTabPage(rbc);

		}

		private void menuChangeOrderDetails_Click(object sender, System.EventArgs e)
		{
			ChangeItemLocation cl = new ChangeItemLocation(this.FormRoot, this);
			AddTabPage(cl);
		}

		private void menuEODInterface_Click(object sender, System.EventArgs e)
		{
			EODUserInterface eod = new EODUserInterface(this.FormRoot, this);
			AddTabPage(eod);
		}

		private void menuCustomerMailing_Click(object sender, System.EventArgs e)
		{
			CustomerMailing c = new CustomerMailing(this.FormRoot, this);
			//c.FormParent=this;
			//c.FormRoot=this;
			AddTabPage(c);

		}

		// Payment File Definition screen
		private void menuPaymentFileDefn_Click(object sender, System.EventArgs e)
		{
			PaymentFileDefn pd = new PaymentFileDefn(this.FormRoot, this);
			AddTabPage(pd);

		}

		private void menuServiceRequestAudit_Click(object sender, EventArgs e)
		{
			//UAT 367 - Service Request Audit screen is no longer to be seen in the Service menu

			//SR_Audit p = new SR_Audit(this.FormRoot, this, 2);
			//AddTabPage(p);
		}

		//private void menuServiceRequest_Click(object sender, EventArgs e)
		//{
		//    SR_ServiceRequest p = new SR_ServiceRequest(this.FormRoot, this, "", "", "");
		//    AddTabPage(p);
		//}

		//private void menuServiceRequestSearch_Click(object sender, EventArgs e)
		//{
		//    //CR 949/958
		//    SR_ServiceSearch p = new SR_ServiceSearch(this.FormRoot, this);
		//    //SR_ServiceSearch p = new SR_ServiceSearch();
		//    p.FormRoot = this;
		//    p.FormParent = this;
		//    AddTabPage(p);
		//}

		//private void menuServiceTechDiary_Click(object sender, EventArgs e)
		//{
		//    SR_Diary p = new SR_Diary(this, this);
		//    AddTabPage(p);
		//}

		//private void menuServiceTechMaintenance_Click(object sender, EventArgs e)
		//{
		//    SR_TechMaintenance p = new SR_TechMaintenance(this.FormRoot, this);
		//    AddTabPage(p);
		//}

		//private void menuServiceTechPayment_Click(object sender, EventArgs e)
		//{
		//    SR_TechPayment p = new SR_TechPayment(this.FormRoot, this);
		//    AddTabPage(p);
		//}

		private void menuCancelCollectionNotes_Click(object sender, System.EventArgs e)
		{
			AddTabPage(new CancelCollectionNotes(this.FormRoot, this));
		}

		private void menuServiceCustomerInteraction_Click(object sender, EventArgs e)
		{
			SR_CustomerInteraction p = new SR_CustomerInteraction();
			p.FormRoot = this;
			p.FormParent = this;
			AddTabPage(p);
		}

		//private void menuServicePriceMatrix_Click(object sender, EventArgs e)
		//{
		//    SR_PriceMatrix p = new SR_PriceMatrix(this.FormRoot, this);
		//    AddTabPage(p);
		//}

		//private void menuServiceOutstanding_Click(object sender, EventArgs e)
		//{
		//    SR_Outstanding p = new SR_Outstanding(this.FormRoot, this);
		//    AddTabPage(p);
		//}

		//private void menuServiceBatchPrint_Click(object sender, EventArgs e)
		//{
		//    SR_BatchPrint p = new SR_BatchPrint(this.FormRoot, this);
		//    AddTabPage(p);
		//}

		//private void menuServiceFailureReport_Click(object sender, EventArgs e)
		//{
		//    SR_FailureReport p = new SR_FailureReport(this.FormRoot, this);
		//    AddTabPage(p);
		//}

		//private void menuServiceProgressReport_Click(object sender, EventArgs e)
		//{
		//    SR_ProgressReport p = new SR_ProgressReport(this.FormRoot, this);
		//    AddTabPage(p);
		//}

		//private void menuServiceClaimsReport_Click(object sender, EventArgs e)
		//{
		//    SR_ClaimsReport p = new SR_ClaimsReport(this.FormRoot, this);
		//    AddTabPage(p);
		//}

		private void menuPrizeVouchers_Click(object sender, EventArgs e)
		{
			PrizeVoucher pv = new PrizeVoucher(this.FormRoot, this);
			AddTabPage(pv);
		}
		// Commissions Set Up CR36
		private void menuCommissionsSetUp_Click(object sender, EventArgs e)
		{
			SalesCommissionMaintenance cs = new SalesCommissionMaintenance(this.FormRoot, this);
			AddTabPage(cs);
		}

		private void menuInstantReplacement_Click(object sender, EventArgs e)
		{
			InstantReplacement ir = new InstantReplacement(this.FormRoot, this);
			AddTabPage(ir);

		}
		// Commissions Enquiry CR36
		private void menuCommissionEnquiry_Click(object sender, EventArgs e)
		{
			CommissionEnquiry ce = new CommissionEnquiry(this.FormRoot, this);
			AddTabPage(ce);
		}

		private void menuWarrantyReport_Click(object sender, EventArgs e)
		{
			WarrantyReporting wr = new WarrantyReporting(this.FormRoot, this);
			AddTabPage(wr);
		}

		private void menuFactoringReports_Click(object sender, EventArgs e)
		{
			FactoringReports FR = new FactoringReports(this.FormRoot, this);
			AddTabPage(FR);

		}

		private void menuCommissionReport_Click(object sender, EventArgs e)
		{
			CommissionsReport cr = new CommissionsReport(this.FormRoot, this);
			AddTabPage(cr);
		}

		//69520 Code not required
		//private void menuCommand4_Click(object sender, EventArgs e)
		//{
		//    FactoringReports FR = new FactoringReports(this.FormRoot, this);
		//    AddTabPage(FR);
		//}

		//private void txtUserCourts_Validating(object sender, CancelEventArgs e)
		//{
		//    try
		//    {
		//        txtUserCourts.Text = txtUserCourts.Text.Trim();
		//        if (txtUserCourts.Text.Length > 0)
		//        {
		//            errorProvider1.SetError(txtUserCourts, "");
		//        }
		//        else
		//        {
		//            txtUserCourts.Focus();
		//            txtUserCourts.Select(0, txtUserCourts.Text.Length);
		//            errorProvider1.SetError(txtUserCourts, GetResource("M_ENTERMANDATORY"));
		//        }
		//    }
		//    catch (Exception ex)
		//    {
		//        txtUserCourts.Focus();
		//        txtUserCourts.Select(0, txtUserCourts.Text.Length);
		//        errorProvider1.SetError(txtUserCourts, ex.Message);
		//    }
		//}

		private void txtPasswordCourts_Validating(object sender, CancelEventArgs e)
		{
			if (!Closed)
				this.LogIn();
		}

		private void txtPasswordCourts_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == (char)13)
				this.LogIn();
		}




		private void menuAbout_Click(object sender, EventArgs e)
		{
			string fileName = "Default.htm";
			try
			{
				LaunchHelp(fileName);
			}
			catch
			{
				MessageBox.Show("Help files are not currently installed in the correct directory.");
			}
		}

		private void MainForm_HelpRequested(object sender, HelpEventArgs hlpevent)
		{
			string fileName = "Default.htm";
			//In case file does not exist
			try
			{
				LaunchHelp(fileName);
			}
			catch
			{
				MessageBox.Show("Help files are not currently installed in the correct directory.");
			}
		}
		public string ToCommandLineArg(string str, string arg)
		{
			str = str ?? "";
			arg = arg ?? "";
			return " " + str.Trim() + "=" + arg.Replace(' ', '%');
		}

		private void menuEPOS_Click(object sender, EventArgs e)
		{
			//CR 1086 - Integrating EPOS touch screen with CoSACS
			try
			{
				Function = "menuEPOS_Click";
				Wait();

				Assembly executingAssembly = Assembly.GetExecutingAssembly();
				string assembly = executingAssembly.FullName.Substring(0, executingAssembly.FullName.IndexOf(","));

				Action<string> StartEpos =
					delegate(string EPOSexe)
					{
						ProcessStartInfo epos = new ProcessStartInfo(EPOSexe);
						epos.Arguments = ToCommandLineArg(EposParameters.Assembly, assembly);
						epos.Arguments += ToCommandLineArg(EposParameters.BranchCode, Config.BranchCode);
						epos.Arguments += ToCommandLineArg(EposParameters.CoutryCode, Config.CountryCode);
						epos.Arguments += ToCommandLineArg(EposParameters.Pword, Credential.Password);
						epos.Arguments += ToCommandLineArg(EposParameters.ReceiptPrinter, Config.ThermalPrinterName);
						epos.Arguments += ToCommandLineArg(EposParameters.Uname, Credential.User);
						epos.Arguments += ToCommandLineArg(EposParameters.WebService, Config.Url);

						Process.Start(epos);
					};

				try
				{
					StartEpos(Config.EPOSexe);
				}
				catch (Win32Exception ex)
				{
					bool tryAgain = false;

					do
					{
						DialogResult result =
							MessageBox.Show(
								ex.Message + "\n\nPress Retry to select the application",
								"Courts EPOS Application Start Error",
								MessageBoxButtons.RetryCancel,
								MessageBoxIcon.Error);

						if (result == DialogResult.Cancel)
							break;

						OpenFileDialog openFileDialog = new OpenFileDialog();
						openFileDialog.Multiselect = false;
						openFileDialog.Filter = "EPOS Application|Blue.Cosacs.EPOS.exe|All Files|*.exe";
						openFileDialog.Title = "Please select the Courts EPOS application";
						openFileDialog.InitialDirectory = Path.GetDirectoryName(Application.ExecutablePath);
						result = openFileDialog.ShowDialog(this);

						if (result == DialogResult.Cancel)
							break;
						else
						{
							Config.EPOSexe = openFileDialog.FileName;
							try
							{
								StartEpos(Config.EPOSexe);

								Config.Save(executingAssembly);
								tryAgain = false;
							}
							catch (Win32Exception exc)
							{
								MessageBox.Show(exc.Message);
								tryAgain = true;
							}
						}
					} while (tryAgain);
				}
			}
			catch (Exception ex)
			{
				Catch(ex, Function);
			}
			finally
			{
				StopWait();
			}

			//string acctNo = "";
			//try
			//{
			//    Function = "menuEPOS_Click";
			//    Wait();
			//    acctNo = AccountManager.GetPaidAndTakenAccount(Config.BranchCode, out Error);
			//    if (Error.Length > 0)
			//        ShowError(Error);
			//    else
			//    {
			//        if (acctNo.Length != 0)
			//        {
			//            EPOS epos = new EPOS(acctNo, 1, this, this);
			//            AddTabPage(epos);
			//        }
			//        else
			//            ShowInfo("M_NOPAIDANDTAKEN");
			//    }
			//}
			//catch (Exception ex)
			//{
			//    Catch(ex, Function);
			//}
			//finally
			//{
			//    StopWait();
			//}
		}

		private void menuStrategyConfiguration_Click(object sender, EventArgs e)
		{
			StrategyConfiguration sc = new StrategyConfiguration(this.FormRoot, this);
			AddTabPage(sc);
		}

		private void menuWorkLists_Click(object sender, EventArgs e)
		{
			WorkLists wl = new WorkLists(this.FormRoot, this);
			AddTabPage(wl);
		}

		private void menuSMS_Click(object sender, EventArgs e)
		{
			SMSSetup ss = new SMSSetup(this.FormRoot, this);
			AddTabPage(ss);
		}

		private void menuAddBank_Click(object sender, EventArgs e)
		{
			BankMaintenance bank = new BankMaintenance(this, this);
			AddTabPage(bank);

		}

		public void EnableAllMenus(bool enable)
		{
			menuAccount.Enabled = enable;
			menuCustomer.Enabled = enable;
			menuCredit.Enabled = enable;
			menuTransaction.Enabled = enable;
			//menuService.Enabled = enable;
			menuWarehouse.Enabled = enable;
			// Disabling SysMaint will cause lockout if EOD screen closed when System closed
			// menuSysMaint.Enabled = enable;        // jec 19/12/07 UAT 282 Do not disable SysMaint
			menuReports.Enabled = enable;
            RePrintInvoice.Enabled = enable;
            
        }

		//private void menuServiceChargeToAuthorisation_Click(object sender, EventArgs e)
		//{
		//    var p = new SR_ChargeToAuthorisation(this.FormRoot, this);
		//    AddTabPage(p);
		//}

		private void menuAccountStatus_Click(object sender, EventArgs e)
		{
			AccountStatus a = new AccountStatus(this, this);
			AddTabPage(a);
		}

		private void menuLetterMerge_Click(object sender, EventArgs e)
		{
			var lm = new LetterMerge(this, this);
			AddTabPage(lm);
		}

		private void menuZoneAutomation_Click(object sender, EventArgs e)
		{
			var za = new ZoneAutomatedAllocation(this, this);
			AddTabPage(za);
		}

		public string StatusBarText
		{
			get
			{
				return statusBar1.Text;
			}
			set
			{
				statusBar1.Text = value ?? "";
			}
		}

		private bool TabPageOpen<T>() where T : CommonForm
		{
			bool found = false;

			foreach (Crownwood.Magic.Controls.TabPage tp in ((MainForm)FormRoot).MainTabControl.TabPages)
			{
				if (tp.Control is T)
				{
					((MainForm)this.FormRoot).MainTabControl.SelectedTab = tp;
					found = true;
				}
			}

			return found;
		}

		private void menuTallymanExtract_Click(object sender, EventArgs e)
		{
			var te = new TallymanExtract();
			AddTabPage(te);
		}

		private void menuBehavioural_Click(object sender, EventArgs e)
		{
			var B = new BehaviouralResults(this, this);
			AddTabPage(B);

		}

		private void menuProvisions_Click(object sender, EventArgs e)
		{
			var pro = new Provisions(this);
			AddTabPage(pro);
		}

		private void menuStoreCardRateSetup_Click(object sender, EventArgs e)
		{
			var storesetup = new StoreCardRatesSetup(this, this);
			//StoreCardRatesSetup
			AddTabPage(storesetup);
		}

		private void menuNonStock_Click(object sender, EventArgs e)
		{
			var non = new NonStock(this, this);
			AddTabPage(non);
		}



		private void menuWarrantyReturnCodes_Click(object sender, EventArgs e)
		{
			var wrc = new WarrantyReturnCodes(this, this);
			AddTabPage(wrc);
		}

		//private void menuPendingInstallations_Click(object sender, EventArgs e)
		//{
		//    AddTabPage(new Installation.PendingInstallations(this, this));
		//}

		//private void menuInstBookingPrint_Click(object sender, EventArgs e)
		//{
		//    AddTabPage(new Installation.BookingPrint(this, this));
		//}

		//private void menuInstManagement_Click(object sender, EventArgs e)
		//{
		//    AddTabPage(new Installation.InstManagement(this, this));
		//}

		private void menuTestMode_Click(object sender, EventArgs e)
		{
			//wingman = null;                         // jec 26/07/11 fix error when reentering after closing
			//if (wingman == null)
			//    wingman = new WinDriver.Server.Windows.Wingman();
			//wingman.Show();
		}

		public TForm GetIfExists<TForm>(TForm form = null) where TForm : CommonForm
		{
			foreach (Crownwood.Magic.Controls.TabPage page in MainTabControl.TabPages)
				if (page.Control is TForm && (form == null || form == page.Control))
					return page.Control as TForm;

			return null;
		}

		public bool FocusIfExists<TForm>(TForm form = null) where TForm : CommonForm
		{
			foreach (Crownwood.Magic.Controls.TabPage page in MainTabControl.TabPages)
				if (page.Control is TForm && (form == null || form == page.Control))
				{
					page.Selected = true;
					return true;
				}

			return false;
		}

		private void menuProduct_Associations_Click(object sender, EventArgs e)
		{
			var pa = new Product_Associations(this, this);
			AddTabPage(pa);
		}

		private void menuCashLoanApplication_Click(object sender, EventArgs e)
		{
			var cl = new CashLoanApplication(this.FormRoot, this);
			AddTabPage(cl);
		}

		private void menuFailedDeliveriesCollections_Click(object sender, EventArgs e)
		{
			var fl = new FailedDeliveriesCollections(this.FormRoot, this);
			AddTabPage(fl);
		}

		private void menuStoreCardBatchPrint_Click(object sender, EventArgs e)
		{
			StoreCardBatchPrint SC = new StoreCardBatchPrint(this.FormRoot, this);
			AddTabPage(SC);
		}

		private void menuBERRep_Click(object sender, EventArgs e)
		{

			BERReplacements replacements = new BERReplacements(this.FormRoot, this);
			AddTabPage(replacements);
		
		}

	private void menuOnlineProductSearch_Click(object sender, EventArgs e)      // #13889
		{
		   // var ol = new OnlineProductSearch(this.FormRoot, this);
			OnlineProductSearch ol = new OnlineProductSearch();
			ol.FormRoot = this;
			ol.FormParent = this;
			AddTabPage(ol);

		   
		}

    private void menuDuplicateCustomers_Click(object sender, EventArgs e)
    {
        DuplicateCustomers cust = new DuplicateCustomers();
        cust.FormRoot = this;
        cust.FormParent = this;
        AddTabPage(cust, 9);
    }

    private void menuSalesCommEnquiry_Click(object sender, EventArgs e)
    {
        SalesCommissionEnquiry SalesCommEnquiry = new SalesCommissionEnquiry(this.FormRoot, this);
        SalesCommEnquiry.FormRoot = this;
        SalesCommEnquiry.FormParent = this;
        AddTabPage(SalesCommEnquiry);
       
    }

    private void menuSalesCommBranchEnquiry_Click(object sender, EventArgs e)
    {
        SalesCommissionBranchEnquiry SalesCommBranchEnquiry = new SalesCommissionBranchEnquiry(this.FormRoot, this);
        SalesCommBranchEnquiry.FormRoot = this;
        SalesCommBranchEnquiry.FormParent = this;
        AddTabPage(SalesCommBranchEnquiry);
    }

    private void menuCashLoanBankTransfer_Click(object sender, EventArgs e)
    {
        CashLoanRecordBankTransfer CashLoanBankTransfer = new CashLoanRecordBankTransfer();
        CashLoanBankTransfer.FormRoot = this;
        CashLoanBankTransfer.FormParent = this;
        AddTabPage(CashLoanBankTransfer);
    }



	}
}


