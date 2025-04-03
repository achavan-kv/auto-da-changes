using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web.Services.Protocols;
using System.Windows.Forms;
using System.Xml;
using AxSHDocVw;
using BBSL.Libraries.General;
using BBSL.Libraries.Printing;
using BBSL.Libraries.Printing.PrintDocuments;
using Blue.Cosacs.Shared;
using mshtml;
using STL.Common;
using STL.Common.Constants.AccountTypes;
using STL.Common.Constants.ColumnNames;
using STL.Common.Constants.Elements;
using STL.Common.Constants.FTransaction;
using STL.Common.Constants.ItemTypes;
using STL.Common.Constants.TableNames;
using STL.Common.Constants.Tags;
using STL.Common.Constants.Values;
using STL.Common.Printing.AgreementPrinting;
using STL.Common.Printing.CustomerCard;
using STL.Common.ServiceRequest;
using STL.Common.Static;
using STL.DAL;
using STL.PL.Utils;
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

//i think this form is to small, we could more non reusable code here
namespace STL.PL
{
    /// <summary>
    /// All of the forms in the Presentation Layer (PL) project are derived
    /// from this base class that contains many common functions including:
    ///  - User Permissions
    ///  - Error Logging
    ///  - Field Validation
    ///  - Field Formatting
    ///  - Language Translation
    ///  - Printing
    /// </summary>
    /// 
    public partial class CommonForm : System.Windows.Forms.Form
    {
        private Control authCashTill = null;
        private Thread _dataThread;
        public Thread DataThread
        {
            get { return _dataThread; }
            set { _dataThread = value; }
        }
        private static ResourceManager _rm;
        public static ResourceManager ResMan
        {
            get { return _rm; }
            set { _rm = value; }
        }
        private bool ReprintInvoiceOption = false;
        private string _function = "";
        protected bool _hasdatachanged = false;
        protected XmlUtilities xml;
        public Hashtable dynamicMenus;
        protected string Error = "";
        bool _closed = true;
        protected bool loaded = false;
        public DataSet CachedData = null;
        private string _postHeader = "Content-Type: application/x-www-form-urlencoded\n";
        private Exception threadError;
        private string threadFunction;
        public string PostHeader
        {
            get { return _postHeader; }
        }

        new public bool Closed
        {
            get { return _closed; }
            set { _closed = value; }
        }

        private bool _thermalprinting = false;

        public Crownwood.Magic.Menus.MenuControl menuMain;

        private System.Windows.Forms.Form _parent;


        public System.Windows.Forms.Form FormParent
        {
            get { return _parent; }
            set { _parent = value; }
        }
        private System.Windows.Forms.Form _root;
        public System.Windows.Forms.Form FormRoot
        {
            get { return _root; }
            set { _root = value; }
        }

        private readonly string rpOldModel = "290";
        private readonly string rpNewModel = "295";

        private PrintDialog printDlg = null;            //IP - 10/05/12 - #9609 - CR8520

        #region Web Service Proxy properties
        /// <summary>
        /// These are properties which can be used by any form inheriting 
        /// from CommonForm. It the FormRoot property has been set it
        /// will point to the MainForm and the Web Service proxy object
        /// properties on the main form will be used. This is because we
        /// only really want to create one set of proxy objects rather than
        /// one per form. However, it is possible that the FormRoot 
        /// property may not have been set, in this case, we must create 
        /// a proxy object for this form. This requires an empty try 
        /// catch block to prevent exceptions when viewing the forms in 
        /// design view.
        /// </summary>

        private WEODManager _eodManager = null;
        [Browsable(false)]
        public WEODManager EodManager
        {
            get
            {
                if (this.FormRoot != null)
                    return ((MainForm)FormRoot).RootEodManager;
                else
                {
                    try
                    {
                        if (_eodManager == null)
                            _eodManager = new WEODManager(true);
                        else
                            _eodManager.Setup();
                    }
                    catch { }
                    return _eodManager;
                }
            }
        }

        private WStaticDataManager _staticDataManager = null;
        [Browsable(false)]
        public WStaticDataManager StaticDataManager
        {
            get
            {
                if (this.FormRoot != null)
                    return ((MainForm)FormRoot).RootStaticDataManager;
                else
                {
                    try
                    {
                        if (_staticDataManager == null)
                            _staticDataManager = new WStaticDataManager(true);
                        else
                            _staticDataManager.Setup();
                    }
                    catch { }
                    return _staticDataManager;
                }
            }
        }

        private WLogin _login = null;
        [Browsable(false)]
        public WLogin Login
        {
            get
            {
                if (this.FormRoot != null)
                    return ((MainForm)FormRoot).RootLogin;
                else
                {
                    try
                    {
                        if (_login == null)
                            _login = new WLogin(true);
                        else
                            _login.Setup();
                    }
                    catch { }
                    return _login;
                }
            }
        }

        private WAccountManager _accountManager = null;
        [Browsable(false)]
        public WAccountManager AccountManager
        {
            get
            {
                if (this.FormRoot != null)
                    return ((MainForm)FormRoot).RootAccountManager;
                else
                {
                    try
                    {
                        if (_accountManager == null)
                            _accountManager = new WAccountManager(true);
                        else
                            _accountManager.Setup();
                    }
                    catch { }
                    return _accountManager;
                }
            }
        }

        private WCustomerManager _customerManager = null;
        [Browsable(false)]
        public WCustomerManager CustomerManager
        {
            get
            {
                if (this.FormRoot != null)
                    return ((MainForm)FormRoot).RootCustomerManager;
                else
                {
                    try
                    {
                        if (_customerManager == null)
                            _customerManager = new WCustomerManager(true);
                        else
                            _customerManager.Setup();
                    }
                    catch { }
                    return _customerManager;
                }
            }
        }

        private WClientLogging _clientLogging = null;
        [Browsable(false)]
        public WClientLogging ClientLogging
        {
            get
            {
                if (this.FormRoot != null)
                    return ((MainForm)FormRoot).RootClientLogging;
                else
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
                            _clientLogging.Url = Config.Url + "WClientLogging.asmx";
                        }
                    }
                    catch { }
                    return _clientLogging;
                }
            }
        }

        private WSystemConfig _systemConfig = null;
        [Browsable(false)]
        public WSystemConfig SystemConfig
        {
            get
            {
                if (this.FormRoot != null)
                    return ((MainForm)FormRoot).RootSystemConfig;
                else
                {
                    try
                    {
                        if (_systemConfig == null)
                            _systemConfig = new WSystemConfig(true);
                        else
                            _systemConfig.Setup();
                    }
                    catch { }
                    return _systemConfig;
                }
            }
        }

        private WCreditManager _creditManager = null;
        [Browsable(false)]
        public WCreditManager CreditManager
        {
            get
            {
                if (this.FormRoot != null)
                    return ((MainForm)FormRoot).RootCreditManager;
                else
                {
                    try
                    {
                        if (_creditManager == null)
                            _creditManager = new WCreditManager(true);
                        else
                            _creditManager.Setup();
                    }
                    catch { }
                    return _creditManager;
                }
            }
        }

        private WPaymentManager _paymentManager = null;
        [Browsable(false)]
        public WPaymentManager PaymentManager
        {
            get
            {
                if (this.FormRoot != null)
                    return ((MainForm)FormRoot).RootPaymentManager;
                else
                {
                    try
                    {
                        if (_paymentManager == null)
                            _paymentManager = new WPaymentManager(true);
                        else
                            _paymentManager.Setup();

                        _paymentManager.Timeout = 1800000;
                    }
                    catch { }
                    return _paymentManager;
                }
            }
        }

        private WSetData _setData = null;
        [Browsable(false)]
        public WSetData SetDataManager
        {
            get
            {
                if (this.FormRoot != null)
                    return ((MainForm)FormRoot).RootSetData;
                else
                {
                    try
                    {
                        if (_setData == null)
                            _setData = new WSetData(true);
                        else
                        {
                            _setData.Setup();
                        }
                    }
                    catch { }
                    return _setData;
                }
            }
        }

        private WServiceManager _serviceManager = null;
        [Browsable(false)]
        public WServiceManager ServiceManager
        {
            get
            {
                if (this.FormRoot != null)
                    return ((MainForm)FormRoot).RootServiceManager;
                else
                {
                    try
                    {
                        if (_serviceManager == null)
                            _serviceManager = new WServiceManager(true);
                        else
                            _serviceManager.Setup();
                    }
                    catch { }
                    return _serviceManager;
                }
            }
        }

        private WCollectionsManager _collectionsManager = null;
        [Browsable(false)]
        public WCollectionsManager CollectionsManager
        {
            get
            {
                if (this.FormRoot != null)
                    return ((MainForm)FormRoot).RootCollectionsManager;
                else
                {
                    try
                    {
                        if (_collectionsManager == null)
                            _collectionsManager = new WCollectionsManager(true);
                        else
                            _collectionsManager.Setup();
                    }
                    catch { }
                    return _collectionsManager;
                }
            }
        }

        private WPrinting _wprinting = null;
        [Browsable(false)]
        public WPrinting Printing
        {
            get
            {
                if (this.FormRoot != null)
                    return ((MainForm)FormRoot).RootPrintManager;
                else
                {
                    try
                    {
                        if (_wprinting == null)
                            _wprinting = new WPrinting(true);
                        else
                            _wprinting.Setup();
                    }
                    catch { }
                    return _wprinting;
                }
            }
        }

        private WStoreCard _wstorecard = null;
        [Browsable(false)]
        public WStoreCard StoreCardManager
        {
            get
            {
                if (this.FormRoot != null)
                    return ((MainForm)FormRoot).RootStoreCardManager;
                else
                {
                    try
                    {
                        if (_wstorecard == null)
                            _wstorecard = new WStoreCard(true);
                        else
                            _wstorecard.Setup();
                    }
                    catch { }
                    return _wstorecard;
                }
            }
        }

        private WInstallation _wInstallation = null;
        [Browsable(false)]
        public WInstallation InstallationManager
        {
            get
            {
                if (this.FormRoot != null)
                    return ((MainForm)FormRoot).RootInstallationManager;
                else
                {
                    try
                    {
                        if (_wInstallation == null)
                            _wInstallation = new WInstallation(true);
                        else
                            _wInstallation.Setup();
                    }
                    catch { }
                    return _wInstallation;
                }
            }
        }

        private WStock _wStock = null;
        [Browsable(false)]
        public WStock StockManager
        {
            get
            {
                if (this.FormRoot != null)
                    return ((MainForm)FormRoot).RootStockManager;
                else
                {
                    try
                    {
                        if (_wStock == null)
                            _wStock = new WStock(true);
                        else
                            _wStock.Setup();
                    }
                    catch { }
                    return _wStock;
                }
            }
        }
        #endregion

        private CountryParameterCollection _country = null;
        [Browsable(false)]
        public CountryParameterCollection Country
        {
            get
            {
                if (FormRoot != null)
                    return ((MainForm)FormRoot).RootCountry;
                else
                    return _country;
            }
        }

        protected string _helpFileUrl = Config.Url + "HelpFiles/";
        private PrintDialog printDialog1;

        public string HelpFileUrl
        {
            get { return _helpFileUrl; }
            set { _helpFileUrl = value; }
        }

        protected string _iconFileUrl = Config.Url + "Icons/";
        public string IconFileUrl
        {
            get { return _iconFileUrl; }
            set { _iconFileUrl = value; }
        }

        public DialogResult ShowError(string err, string caption = null, MessageBoxButtons buttons = MessageBoxButtons.OK)
        {
            return MessageBox.Show(this, err, caption ?? Messages.List["M_ERROR"].ToString(), buttons, MessageBoxIcon.Error);
        }

        public void ShowWarning(string warning)
        {
            MessageBox.Show(this, warning, (string)Messages.List["M_WARNING"], MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        //public DialogResult ShowInfo(string err)
        //{
        //    string msg = GetResource(err);
        //    return MessageBox.Show(this, msg, (string)Messages.List["M_INFORMATION"], MessageBoxButtons.OK, MessageBoxIcon.Information);
        //}

        public DialogResult ShowInfo(string err, MessageBoxButtons buttons = MessageBoxButtons.OK)
        {
            string msg = GetResource(err);
            return MessageBox.Show(this, msg, (string)Messages.List["M_INFORMATION"], buttons, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Second override for parameterised messages
        /// </summary>
        /// <param name="msgName"></param>
        /// <param name="parms"></param>
        public DialogResult ShowInfo(string msgName, object[] parms)
        {
            string msg = CommonForm.GetResource(msgName, parms);
            return MessageBox.Show(this, msg, (string)Messages.List["M_INFORMATION"], MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public DialogResult ShowInfo(string msgName, object[] parms, MessageBoxButtons buttons)
        {
            string msg = GetResource(msgName, parms);
            return MessageBox.Show(this, msg, (string)Messages.List["M_INFORMATION"], buttons, MessageBoxIcon.Information);
        }

        public static string GetResource(string msgName, params object[] parms)
        {
            //if this is an english culture use the string in the resource file
            //otherwise look up the string in the dictionary
            string complete = String.Format((string)Messages.List[msgName], parms);
            //if(Config.Culture.IndexOf("en-")!=-1)
            //	complete = String.Format((string)Messages.List[msgName], parms);
            //else
            //{
            string trans = String.Format(Translate(msgName), parms);
            complete = trans == msgName ? complete : trans;	//in case there is no translation
            //}

            string[] lines = complete.Split('\\');
            string msg = "";
            foreach (string s in lines)
                msg += s + "\n";
            msg = msg.Substring(0, msg.Length - 1);
            return msg;
        }

        [DebuggerStepThrough]
        public static string GetResource(string msgName)
        {
            try
            {
                string complete = (string)Messages.List[msgName];
                //if(Config.Culture.IndexOf("en-")!=-1)
                //	complete = (string)Messages.List[msgName];
                //else
                //{
                string trans = Translate(msgName);
                complete = trans == msgName ? complete : trans;	//in case there is no translation
                //}
                string[] lines = null;

                if (complete != null)
                {
                    lines = complete.Split('\\');
                }

                string msg = "";
                if (lines != null)
                {
                    foreach (string s in lines)
                        msg += s + "\n";
                    msg = msg.Substring(0, msg.Length - 1);
                }
                return msg;
            }
            catch { return ""; }
        }

        public string Function
        {
            get { return _function; }
            set { _function = value; }
        }

        public string Finished
        {
            get
            {
                return " - FINISHED.";
            }
        }


        /// <summary>
        /// Sets the cursor to the hourglass;
        /// </summary>
        public void Wait()
        {
            Cursor.Current = Cursors.WaitCursor;
        }

        /// <summary>
        /// Sets the cursor to the default pointer
        /// </summary>
        public void StopWait()
        {
            Cursor.Current = Cursors.Default;
        }

        /// <summary>
        /// Logs messages in the server message log
        /// </summary>
        /// <param name="message">the message string to log</param>
        /// <param name="user">the user to log the message for</param>
        public void logMessage(string message, string user, STL.PL.WS4.EventLogEntryType type)
        {
            ClientLogging.logMessage(message, user, type);
        }

        /// <summary>
        /// Logs an exception in the server log file
        /// Will use logging level set in the web.config file
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="user"></param>
        public void logException(Exception ex, string user, string function)
        {
            //string[] msgs = new String[] {"Message: " + ex.Message,
            //                                 "Source: " + ex.Source,
            //                                 "Stack Trace: " + ex.StackTrace};
            //ClientLogging.logMessages(msgs, Credential.UserId.ToString(), STL.PL.WS4.EventLogEntryType.Error);
            MainForm.HandleException(this, ex);
        }

        /// <summary>
        /// Error handling to perform when catching a non-SOAP
        /// i.e. client side error
        /// Logs the exception in the server log 		
        /// <param name="ex">the exception object thrown</param>
        /// <param name="function">the function the error occurred in</param>
        /// </summary>
        //[Obsolete("Do not use this method any more!", false)]
        public void Catch(Exception ex, string function)
        {
            if (ex.Message.Contains("System.Security.SecurityException: Authentication failed for employee"))
            {
                return; // Hack hack hack 
            }

            if (ex.Message.Contains("Client Server versions do not match"))
            {
                MessageBox.Show("New CoSaCS version required. Please restart to update CoSaCS.", "New CoSaCS version required!", MessageBoxButtons.OK, MessageBoxIcon.Stop); // Hack hack hack 
                MainForm.Current.SetUpgrade();
                return;
            }

            try
            {
                /* save a screen shot to the app directory at the time of the error */
                string location = Assembly.GetExecutingAssembly().Location;
                string path = location.Substring(0, location.ToLower().IndexOf("courts.net.pl.exe"));
                path += "ErrorScreenShot.jpg";

                ScreenCapture sc = new ScreenCapture();
                sc.CaptureWindowToFile(Handle, path, System.Drawing.Imaging.ImageFormat.Jpeg);

                if (!(ex is SoapException))	/* if it's a SOAP exception it will already have been logged */
                    logException(ex, Credential.UserId.ToString(), function);
            }
            catch //(Exception e)
            {
                //Not much point in telling the user that the message logging has failed
                //MessageBox.Show("Service: LogMessage\n\nMessage: " + e.Message, "An error has occurred!");
            }
            finally
            {
                this.threadError = ex;
                this.threadFunction = function;
                var threadDelegate = new ThreadStart(this.ShowSoapError);
                var thread = new Thread(threadDelegate);
                thread.SetApartmentState(ApartmentState.STA);
                thread.Start();
                thread.Join();
            }
        }

        private void ShowSoapError()
        {
            var s = new SoapErrorDisplay(threadError); //, threadFunction);
            s.ShowDialog();
        }

        private void InitializeComponent()
        {
            this.printDialog1 = new System.Windows.Forms.PrintDialog();
            this.SuspendLayout();
            // 
            // printDialog1
            // 
            this.printDialog1.UseEXDialog = true;
            // 
            // CommonForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(792, 573);
            this.KeyPreview = true;
            this.Name = "CommonForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Load += new System.EventHandler(this.CommonForm_Load);
            this.ResumeLayout(false);

        }

        public CommonForm()
        {
            xml = new XmlUtilities();
            InitializeComponent();

            //if (!RecieptTitlesInitialised)
            //    InitialiseRecieptTitles();
        }

        /// <summary>
        /// CommonKeyPress
        /// should trap whether data changed updating _hasdatachanged
        /// </summary>
        /// 
        public void CommonKeyPress(object sender, KeyPressEventArgs e)
        {
            this._hasdatachanged = true;
        }

        /// <summary>
        /// Event handler that needs to be hooked up to various 'changed' events
        /// for the various controls on the form so that the fact that data has 
        /// changed gets recorded via _hasdatachanged.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void CommonDataChanged(object sender, EventArgs e)
        {
            this._hasdatachanged = true;
        }
        public string StripCurrency(string field, CultureInfo c)
        {
            /* this method needs to cope with all possible negative currency patterns */
            var num = c.NumberFormat;
            string separator = num.CurrencyGroupSeparator;
            string currency = num.CurrencySymbol;
            field = field.Replace(currency, "");
            field = field.Replace(separator, "");
            //int i = 0;

            switch (num.CurrencyNegativePattern)
            {
                case 0:
                    field = field.Replace("(", "-").Replace(")", "");	/* ($n) */
                    break;
                case 3:
                    field = field.IndexOf("-") == -1 ? field : "-" + field.Replace("-", "");	/* $n-	*/
                    break;
                case 4:
                    field = field.Replace("(", "-").Replace(")", "");	/* (n$) */
                    break;
                case 6:
                    field = field = field.IndexOf("-") == -1 ? field : "-" + field.Replace("-", "");					/* n-$  */
                    break;
                case 7:
                    field = "-" + field.Replace("-", "");					/* n$-	*/
                    break;
                case 10:
                    field = field = field.IndexOf("-") == -1 ? field : "-" + field.Replace("-", "");					/* n $-	*/
                    break;
                case 11:
                    field = field = field.IndexOf("-") == -1 ? field : "-" + field.Replace("-", "");					/* $ n-	*/
                    break;
                case 13:
                    field = field = field.IndexOf("-") == -1 ? field : "-" + field.Replace("-", "");					/* n- $ */
                    break;
                case 14:
                    field = field.Replace("(", "-").Replace(")", "");	/* ($ n) */
                    break;
                case 15:
                    field = field.Replace("(", "-").Replace(")", "");	/* (n $) */
                    break;
                default:
                    break;
            }
            return field.Trim();
        }

        public string StripCurrency(string field)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo(Config.Culture);
            return this.StripCurrency(field, Thread.CurrentThread.CurrentCulture);
        }



        protected void ResetMenus()
        {
            try
            {
                foreach (object o in dynamicMenus.Values)
                {
                    try
                    {
                        if (((Crownwood.Magic.Menus.MenuCommand)o).Text != "&Transactions")
                        {

                            ((Crownwood.Magic.Menus.MenuCommand)o).Visible = false;
                            ((Crownwood.Magic.Menus.MenuCommand)o).Enabled = false;
                        }
                    }
                    catch (InvalidCastException)
                    {
                        ((Control)o).Visible = false;
                        ((Control)o).Enabled = false;
                    }
                }
            }
            catch
            {
            }
        }

        protected DataSet menus = null;

        private void RoleRestrictionsThread()
        {
            try
            {
                Wait();
                Function = "RoleRestrictionsThread";

                menus = StaticDataManager.GetDynamicMenus(Credential.UserId, this.Name, out Error);
                if (Error.Length > 0)
                    ShowError(Error);
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
            finally
            {
                StopWait();
                Function = "End of RoleRestrictionsThread";
            }
        }

        public void ApplyRoleRestrictions()
        {
            Function = "ApplyRoleRestrictions()";
            try
            {
                menus = null;

                //Load up the list of items to be manipulated for this user and this screen
                Thread data = new Thread(new ThreadStart(RoleRestrictionsThread));
                data.Start();
                data.Join();

                if (menus == null || menus.Tables.Count == 0)
                    return;

                foreach (DataRow row in menus.Tables[0].Rows)
                {
                    string key = String.Format("{0}:{1}", row["Screen"], row["Control"]);
                    var o = dynamicMenus[key];

                    if (o == null)
                        continue;

                    if (o is Crownwood.Magic.Menus.MenuCommand)
                    {
                        (o as Crownwood.Magic.Menus.MenuCommand).Visible = Convert.ToBoolean(row["Visible"]);
                        (o as Crownwood.Magic.Menus.MenuCommand).Enabled = Convert.ToBoolean(row["Enabled"]);
                    }
                    else if (o is ToolBarButton)
                    {
                        (o as ToolBarButton).Visible = Convert.ToBoolean(row["Visible"]);
                        (o as ToolBarButton).Enabled = Convert.ToBoolean(row["Enabled"]);
                    }
                    else if (o is Control)
                    {
                        (o as Control).Visible = Convert.ToBoolean(row["Visible"]);
                        (o as Control).Enabled = Convert.ToBoolean(row["Enabled"]);
                    }
                    else if (o is UserRight)
                    {
                        (o as UserRight).IsAllowed = Convert.ToBoolean(row["Enabled"]);
                    }

                    //if (o != null)
                    //{
                    //    switch ((o.GetType()).Name)
                    //    {
                    //        case "MenuCommand": ((Crownwood.Magic.Menus.MenuCommand)o).Visible = Convert.ToBoolean(row["Visible"]);
                    //            ((Crownwood.Magic.Menus.MenuCommand)o).Enabled = Convert.ToBoolean(row["Enabled"]);
                    //            break;
                    //        case "ToolBarButton": ((ToolBarButton)o).Visible = Convert.ToBoolean(row["Visible"]);
                    //            ((ToolBarButton)o).Enabled = Convert.ToBoolean(row["Enabled"]);
                    //            break;
                    //        default: ((Control)o).Visible = Convert.ToBoolean(row["Visible"]);
                    //            ((Control)o).Enabled = Convert.ToBoolean(row["Enabled"]);
                    //            break;
                    //    }
                    //}
                }
            }
            catch (Exception ex)
            {
                Catch(ex, "ApplyRoleRestrictions");
                //logMessage(ex, Credential.UserId.ToString(), STL.PL.WS4.EventLogEntryType.Error);
            }
        }

        public bool CheckForRolePermission(string permission)
        {
            Function = "CheckForRolePermission()";
            try
            {
                //Load up the list of items to be manipulated for this
                //user and this screen
                menus = null;
                Thread data = new Thread(new ThreadStart(RoleRestrictionsThread));
                data.Start();
                data.Join();

                if (menus != null)
                {
                    foreach (DataTable tab in menus.Tables)
                    {
                        foreach (DataRow row in tab.Rows)
                        {
                            string key = (string)row["Screen"] + ":" + (string)row["Control"];
                            if (key == permission)
                            {
                                return (Convert.ToInt32(row["Enabled"]) > 0);
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Catch(ex, "CheckForRolePermissions");
                //logMessage(ex.Message, Credential.UserId.ToString(), STL.PL.WS4.EventLogEntryType.Error);
            }

            return false;
        }
        public static string FormatAccountNo(string unformatted)
        {
            var i = 0;
            var temp = string.Empty;
            foreach (char letter in unformatted)
            {
                temp += letter;
                switch (i)
                {
                    case 2:
                        temp += "-";
                        break;
                    case 6:
                        temp += "-";
                        break;
                    case 10:
                        temp += "-";
                        break;
                    default: break;
                }
                i++;
            }
            return temp;
        }

        public static void FormatAccountNo(ref string unformatted)
        {
            int i = 0;
            var temp = string.Empty;
            foreach (char letter in unformatted)
            {
                temp += letter;
                switch (i)
                {
                    case 2:
                        temp += "-";
                        break;
                    case 6:
                        temp += "-";
                        break;
                    case 10:
                        temp += "-";
                        break;
                    default: break;
                }
                i++;
            }
            unformatted = temp;
        }

        public static void FormatCreditCardNo(ref string unformatted)
        {
            int i = 0;
            string temp = null;
            foreach (char letter in unformatted)
            {
                temp += letter;
                switch (i)
                {
                    case 3:
                        temp += "-";
                        break;
                    case 7:
                        temp += "-";
                        break;
                    case 11:
                        temp += "-";
                        break;
                    default: break;
                }
                i++;
            }
            unformatted = temp;
        }

        public void ProgressStart()
        {
            ((MainForm)this.FormRoot).ProgressStart();
        }

        public void ProgressEnd()
        {
            ((MainForm)this.FormRoot).ProgressEnd();
        }

        public void CloseTab()
        {
            ((MainForm)this.FormRoot).MainTabControl_ClosePressed(this, new EventArgs());
        }

        public void CloseTab(CommonForm page)
        {
            try
            {
                Crownwood.Magic.Controls.TabPage del = null;
                foreach (Crownwood.Magic.Controls.TabPage tp in ((MainForm)page.FormRoot).MainTabControl.TabPages)
                {
                    if (tp.Control == page)
                    {
                        del = tp;
                        break;
                    }
                }
                ((MainForm)page.FormRoot).MainTabControl.TabPages.Remove(del);
            }
            catch
            {
                return;
            }
        }

        //This method will be run whenever a screen is closed
        //if special validation is required the method should
        //be overriden in the child class to implement the 
        //required behaviour
        public virtual bool ConfirmClose()
        {
            return true;
        }

        //I think someone already invented the wheel
        //because there is already an int.parse/tryparse
        public bool IsNumeric(string text)
        {
            //Regex reg = new Regex("^[0-9-+]*$");
            // (M49,M104) DSR 10/4/03 - Parse a whole number
            Regex reg = new Regex("((^[+-][0-9]+$)|(^[0-9]*$))");
            return reg.IsMatch(text);
        }

        //how a string can be positive?
        public bool IsPositive(string text)
        {
            // Parse a whole positive number
            Regex reg = new Regex("(^[0-9]*$)");
            return reg.IsMatch(text);
        }

        public bool IsStrictNumeric(string text)
        {
            //Regex reg = new Regex("^[0-9.]*$");
            // (M49,M104) DSR 10/4/03 - Parse a decimal number
            // JPJ - decimal separator is not always '.' sometimes it is ',' (indonesia)
            string decimalPoint = Thread.CurrentThread.CurrentCulture.NumberFormat.CurrencyDecimalSeparator;
            //Regex reg = new Regex("(^([+-][0-9]*|[0-9]*)\\.[0-9]+$)|(^([+-][0-9]*|[0-9]*)$)");
            Regex reg = new Regex("(^([+-][0-9]*|[0-9]*)\\" + decimalPoint + "[0-9]+$)|(^([+-][0-9]*|[0-9]*)$)");
            return reg.IsMatch(text);
        }

        public bool IsStrictMoney(string moneyString)
        {
            // Validate for a numeric allowing currency symbol and blank
            moneyString = StripCurrency(moneyString);
            if (moneyString.Trim().Length > 0)
            {
                return IsStrictNumeric(moneyString);
            }
            else
            {
                return true;
            }
        }

        public decimal MoneyStrToDecimal(string moneyString)
        {
            return MoneyStrToDecimal(moneyString, DecimalPlaces);
        }

        //wow this is really...bad
        public decimal MoneyStrToDecimal(string moneyString, string decimalPlaces)
        {
            // Overload to allow a different number of decimal places to the Country format
            // Convert to decimal allowing currency symbol and blank
            moneyString = StripCurrency(moneyString.Trim());
            if (IsStrictNumeric(moneyString) && moneyString.Trim().Length > 0)
            {
                decimal moneyValue = Convert.ToDecimal(moneyString);
                // Reformat again in case the user entered more decimal places than the currency format
                string moneyReformatted = moneyValue.ToString(decimalPlaces);
                return Convert.ToDecimal(StripCurrency(moneyReformatted));
            }
            else
            {
                // Return zero instead of blank
                return 0.0M;
            }
        }

        //a bool passed by ref? that is a whole new level of...i can't say it here
        private DataTable MergePaymentTransactions(DataTable transactions, decimal balance, ref bool updateRow)
        {
            //need to go through the datatable and combine all transactions
            //from the same date of the same transaction type
            // The sort order is the reverse of the final print order so that the 
            // balance can be calculated backwards.

            DataTable transCopy = transactions.Copy();

            foreach (DataRow r in transCopy.Rows)
                r[CN.DateTrans] = ((DateTime)r[CN.DateTrans]).Date;

            DataView transView = new DataView(transCopy, "", CN.DateTrans + " DESC," + CN.TransTypeCode + " DESC", DataViewRowState.CurrentRows);
            transView.RowFilter = CN.TransPrinted + " = 'N'";

            //Copy the transactions table and use it to hold the merged transactions. Need
            //to add a balance column to the table
            DataTable merged = transactions.Clone();
            merged.Columns.Add(new DataColumn(CN.Balance, Type.GetType("System.Decimal")));

            //Create a table of merged transactions
            DataRow prevRow = null;
            foreach (DataRowView drv in transView)
            {
                if (((DateTime)drv[CN.DateTrans]).Date == DateTime.Today)
                    updateRow = false;

                if (prevRow == null ||
                    (((DateTime)prevRow[CN.DateTrans]).Date != ((DateTime)drv[CN.DateTrans]).Date ||		//this is a new row
                    (string)prevRow[CN.TransTypeCode] != (string)drv[CN.TransTypeCode]))
                {
                    DataRow row = merged.NewRow();
                    row[CN.acctno] = drv[CN.acctno];
                    row[CN.DateTrans] = drv[CN.DateTrans];
                    row[CN.TransRefNo] = drv[CN.TransRefNo];
                    row[CN.TransTypeCode] = drv[CN.TransTypeCode];
                    row[CN.TransValue] = drv[CN.TransValue];
                    if (_thermalprinting) // need agreement total for thermal printing. 
                    {
                        row[CN.AgreementTotal] = drv[CN.AgreementTotal];
                    }
                    row[CN.Balance] = balance;
                    merged.Rows.Add(row);
                    prevRow = row;
                }
                else		//same date and trans type therefore merge
                {
                    //sum the quantities
                    prevRow[CN.TransValue] = (decimal)prevRow[CN.TransValue] + (decimal)drv[CN.TransValue];
                }
                balance -= (decimal)drv[CN.TransValue];			//subtract TransValue from the final balance
            }
            return merged;
        }

        protected void FilterTermsType(ref DataView termsTypes, bool affinity, string acctType, string scoringBand, string storeType, bool isLoan)
        {
            // Optionally filter Terms Types for Affinity and always filter for the account type selected.
            // Include Tier1/2 Privilege Club terms if eligible.
            string newFilter = "isactive = TRUE AND ";

            if (!(bool)Country[CountryParameterNames.AffinityStockSales])
            {
                // Affinity filter
                if (affinity)
                    newFilter += "Affinity = 'Y' AND ";
                else
                    newFilter += "Affinity = 'N' AND ";
            }

            //CR903 Filter on store type
            newFilter += "(" + CN.StoreType + " = '" + storeType + "' OR " + CN.StoreType + " = 'A') AND";

            //CR906 filter on isLoan
            newFilter += "(isLoan = " + isLoan + ") AND";

            // Account Type filter
            if (AT.IsCreditType(acctType)
                && acctType != AT.HP
                && acctType != AT.ReadyFinance)
            {
                // Old credit account types will now use HP terms types
                newFilter += " AccountType = '" + AT.HP + "'";
            }
            else
            {
                newFilter += " AccountType = '" + acctType + "'";
            }

            // Score band filter
            // This can be an empty string, in which case use "A"
            if ((bool)Country[CountryParameterNames.TermsTypeBandEnabled] && (scoringBand == String.Empty))
            {
                scoringBand = Country[CountryParameterNames.TermsTypeBandDefault].ToString();
            }

            newFilter += " AND Band = '" + scoringBand + "'";

            // Privilege Club filter
            //if (customerPClubCode != PCCustCodes.Tier1) newFilter += " AND PClubTier1 = 'N'";
            //if (customerPClubCode != PCCustCodes.Tier2) newFilter += " AND PClubTier2 = 'N'";

            termsTypes.RowFilter = "(" + newFilter + ") or termstype = 'Terms Types'";

            // string[] distinct = { "termstype" };
            // DataTable dt = new DataTable();
            //dt = termsTypes.ToTable(true,distinct);
            //termsTypes = dt.DefaultView;           
        }

        //This overloaded method is no longer required 04/10/2007 JH

        protected void FilterTermsType(ref ComboBox termsTypes, bool affinity, string acctType, string scoringBand, string storeType, bool isLoan)
        {
            // Overloaded to filter terms type in a combo box
            // Get the current setting
            string curTermsType = termsTypes.Text;
            DataView termsTypeView = termsTypes.DataSource as DataView;
            this.FilterTermsType(ref termsTypeView, affinity, acctType, scoringBand, storeType, isLoan);

            // Make sure the TermsType has not changed if it is still available
            int index = termsTypes.FindStringExact(curTermsType);
            termsTypes.SelectedIndex = (index != -1) ? index : 0;
        }


        protected void SetDeposit(CheckBox cbDeposit, TextBox txtDeposit,
            decimal defaultDeposit, bool depositIsPercentage, ref decimal newDeposit,
            decimal subTotal, bool subTotalShown, bool Override)
        {
            decimal minDeposit = 0;
            txtDeposit.Enabled = cbDeposit.Checked;

            if (!Override)
            {
                txtDeposit.Enabled = true;
                cbDeposit.Enabled = true;

                if (cbDeposit.Checked && defaultDeposit > 0)
                {
                    // The terms type has a default deposit
                    if (depositIsPercentage)
                    {
                        // The default deposit is a percentage of the subtotal
                        minDeposit = subTotal * (defaultDeposit / 100);
                        minDeposit = Decimal.Round(minDeposit, 2); //SC 69334 - 9/11/07
                    }
                    else
                    {
                        // The default deposit is a fixed amount
                        minDeposit = defaultDeposit;
                    }

                    // Make sure the deposit is not less then the min
                    newDeposit = (newDeposit < minDeposit) ? minDeposit : newDeposit;
                    if (subTotalShown)
                    {
                        // Make sure the deposit is not more than the cash price
                        // (Note: cannot use agr total here because it varies with a different deposit)
                        newDeposit = (newDeposit > subTotal) ? subTotal : newDeposit;
                    }
                }
                else if (!cbDeposit.Checked)
                {
                    // The terms type does not have a deposit
                    newDeposit = 0;
                }



                txtDeposit.Text = newDeposit.ToString(DecimalPlaces);
            }
            else
            {
                newDeposit = 0;
                txtDeposit.Text = "0";
                txtDeposit.Enabled = false;
                cbDeposit.Enabled = false;
            }
        }


        protected void SetTermsType(ComboBox drpTermsType, NumericUpDown numPaymentHolidays,
            CheckBox cbDeposit, TextBox txtDeposit, NumericUpDown txtNoMonths, ComboBox drpLengths,
            bool loadAccount, decimal subTotal, bool subTotalShown, ref decimal defaultDeposit,
            ref bool depositIsPercentage, ref decimal newDeposit, decimal instalNo, bool checkdepositwaiver)
        {
            if (drpTermsType.SelectedIndex != 0)
            {
                // Extract the terms type code
                string terms = drpTermsType.Text;
                terms = terms.Substring(0, terms.IndexOf("-") - 1);

                //  check for DBNull first!!
                //numPaymentHolidays.Enabled = (bool)((DataView)drpTermsType.DataSource)[drpTermsType.SelectedIndex][CN.PaymentHoliday];
                object paymentHolidays = ((DataView)drpTermsType.DataSource)[drpTermsType.SelectedIndex][CN.PaymentHoliday];
                numPaymentHolidays.Enabled =
                    paymentHolidays != DBNull.Value ? (bool)paymentHolidays : false;


                if (!numPaymentHolidays.Enabled)
                    numPaymentHolidays.Value = 0;

                DataSet ds = StaticDataManager.LoadTermsTypeDetails(terms, out Error);
                if (Error.Length > 0)
                    ShowError(Error);
                else
                {
                    if (ds.Tables[TN.TermsType].Rows.Count > 0)
                    {
                        // Default Deposit
                        defaultDeposit = Convert.ToDecimal(ds.Tables[TN.TermsType].Rows[0][CN.DefaultDeposit]);
                        depositIsPercentage = (bool)(ds.Tables[TN.TermsType].Rows[0][CN.DepositIsPercentage]);
                        cbDeposit.Enabled = true; // UAT Issue 174 RD/DR 29/09/05(this._defaultDeposit > 0);
                        // Don't tick the deposit when opening for revision with a zero deposit
                        cbDeposit.Checked = ((defaultDeposit > 0 || newDeposit > 0) && !(loadAccount && newDeposit == 0));
                        if (!loadAccount)
                        {
                            // Only calculate deposit if the account is not being loaded for revision
                            SetDeposit(cbDeposit, txtDeposit, defaultDeposit, depositIsPercentage,
                                ref newDeposit, subTotal, subTotalShown, checkdepositwaiver);
                        }


                        // Default Term
                        txtNoMonths.Minimum = (int)ds.Tables[TN.TermsType].Rows[0]["MinTerm"];
                        txtNoMonths.Maximum = (int)ds.Tables[TN.TermsType].Rows[0]["MaxTerm"];
                        int defaultTerm = (int)ds.Tables[TN.TermsType].Rows[0]["DefaultTerm"];
                        // Don't use the default number of months when revising
                        if (!loadAccount)
                            instalNo = defaultTerm;
                        else if (instalNo < txtNoMonths.Minimum)
                            instalNo = txtNoMonths.Minimum;
                        else if (instalNo > txtNoMonths.Maximum)
                            instalNo = txtNoMonths.Maximum;

                        if (ds.Tables[TN.TermsTypeLength].Rows.Count > 0)
                        {
                            // Display the length list instead of the number of months
                            drpLengths.DataSource = (DataTable)ds.Tables[TN.TermsTypeLength];
                            drpLengths.DisplayMember = CN.Length;
                            int i = drpLengths.FindStringExact(instalNo.ToString());
                            // If the instalNo is not in the list for some reason then try to use the default
                            if (i == -1) i = drpLengths.FindStringExact(defaultTerm.ToString());
                            drpLengths.SelectedIndex = (i != -1) ? i : 0;
                            drpLengths.Visible = true;
                            drpLengths.Enabled = true;
                            txtNoMonths.Visible = false;
                            txtNoMonths.Enabled = false;
                            // Copy the selected length to the number of months
                            // This should fire the txtNoMonths_ValueChanged event
                            txtNoMonths.Value = Convert.ToDecimal(drpLengths.Text);
                        }
                        else
                        {
                            txtNoMonths.Value = instalNo;
                            drpLengths.Visible = false;
                            drpLengths.Enabled = false;
                            txtNoMonths.Visible = true;
                            txtNoMonths.Enabled = true;
                        }
                    }


                }
            }
            else
            {
                numPaymentHolidays.Enabled = false;
                numPaymentHolidays.Value = 0;
                defaultDeposit = 0;
                depositIsPercentage = false;
                cbDeposit.Enabled = false;
                drpLengths.Visible = false;
                drpLengths.Enabled = false;
                txtNoMonths.Visible = true;
                txtNoMonths.Enabled = false;
                txtDeposit.Enabled = false;
            }
        }

        public void PrintPaymentCardDueDate(string customerID, string accountNo)
        {
            DataSet ds = null;
            DataTable payment = null;
            ReceiptPrinter rp = null;
            try
            {
                Function = "PrintPaymentCard";
                Wait();

                ds = AccountManager.GetPaymentCardDetails(customerID, accountNo, Config.BranchCode, out Error);
                if (Error.Length > 0)
                    ShowError(Error);
                else
                {
                    foreach (DataTable dt in ds.Tables)		//get the right datatable
                        if (dt.TableName == TN.PaymentCard) //what about ds.Tables[TN.PaymentCard]
                            payment = dt;

                    if (payment != null)
                    {
                        DataRow pay = null;
                        if (payment.Rows.Count > 0)			//get the first (and only) row
                            pay = payment.Rows[0];

                        if (pay != null)
                        {
                            if (pay[CN.DateDel] != DBNull.Value)
                            {
                                rp = new ReceiptPrinter(this);

                                rp.OpenPrinter();
                                rp.Invert();

                                rp.Card = (string)pay[CN.NewPCType];
                                switch (rp.Card)
                                {
                                    case "S":
                                        rp.Feed("\x6", 6);
                                        break;
                                    case "L":
                                        rp.Feed("\x14", 14);
                                        break;
                                    default:
                                        break;
                                }
                                rp.PrintString(((DateTime)pay[CN.DateFirst]).Day.ToString());
                                rp.Feed("\x2", 2);
                                rp.PrintString("      ");
                                rp.Feed("\x2", 2);
                                rp.PrintString(((decimal)pay[CN.InstalAmount]).ToString(DecimalPlaces));
                                rp.Feed("\x2", 2);
                                rp.PrintString("      ");
                                rp.Feed("\x2", 2);
                                rp.PrintString("      ");
                                rp.Feed("\x1", 1);
                                rp.PrintString("      ");
                                rp.PrintString("      ");
                                rp.PrintString("      ");
                                rp.PrintString("      ");
                                rp.PrintString("      ");

                                rp.Release();
                                Thread.Sleep(1000);//our code is so fast that we have to introduce a delay, otherwise the user would not be able to follow our amazing program
                            }
                        }
                    }
                    else
                    {
                        ShowInfo("M_NOPAYMENTCARDINFO");
                    }
                }
            }
            catch (SlipPrinterException ex)
            {
                if (ex.Message != "Cancel")
                    Catch((Exception)ex, Function);
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
            finally
            {
                StopWait();
                if (rp != null) rp.ClosePrinter();
            }
        }

        private ReceiptPrinter GetReceiptPrinter()
        {
            ReceiptPrinter rp = null;
            bool retry = true;
            while (retry)
            {
                try
                {
                    rp = new ReceiptPrinter(this);

                    rp.OpenPrinter();

                    retry = false;

                }
                catch (SlipPrinterException)
                {
                    rp = null;
                    //if (ex.Message != "Cancel")
                    //   Catch((Exception)ex, Function);
                    //5.1 uat168 rdb 21/11/07 replaced original code with code payment screen to
                    ((MainForm)this.FormRoot).statusBar1.Text = GetResource("M_SLIPNOCONNECT");
                    DialogResult userRequest = ShowInfo("M_SLIPCONNECT", MessageBoxButtons.AbortRetryIgnore);
                    if (userRequest == DialogResult.Abort || userRequest == DialogResult.Ignore)
                    {
                        retry = false;
                    }
                    else
                    {
                        retry = true;
                    }
                }
            }
            return rp;
        }

        public void PrintPaymentCard(string customerID, string accountNo)
        {
            DataSet ds = null;
            DataTable payment = null;
            ReceiptPrinter rp = null;
            try
            {
                Function = "PrintPaymentCard";
                Wait();

                ds = AccountManager.GetPaymentCardDetails(customerID, accountNo, Config.BranchCode, out Error);
                if (Error.Length > 0)
                    ShowError(Error);
                else
                {
                    foreach (DataTable dt in ds.Tables)		//get the right datatable
                        if (dt.TableName == TN.PaymentCard)
                            payment = dt;

                    if (payment != null)
                    {
                        DataRow pay = null;
                        if (payment.Rows.Count > 0)			//get the first (and only) row
                            pay = payment.Rows[0];

                        if (pay != null)
                        {
                            rp = GetReceiptPrinter();
                            if (rp != null)
                            {
                                rp.Invert();

                                rp.Card = (string)pay[CN.NewPCType];

                                if ((string)pay[CN.AcctType] == AT.ReadyFinance &&
                                    (bool)Country[CountryParameterNames.PrintRFCreditLimit])
                                {
                                    switch (rp.Card)
                                    {
                                        case "S":
                                            rp.Feed("\x2", 2);
                                            break;
                                        case "L":
                                            rp.Feed("\x10", 10);
                                            break;
                                        default:
                                            break;
                                    }
                                    rp.PrintString(((decimal)pay[CN.CreditLimit]).ToString(DecimalPlaces));
                                    rp.Feed("\x1", 1);
                                    rp.PrintString("RF Credit Limit");
                                    rp.Feed("\x1", 1);
                                }
                                else
                                {
                                    switch (rp.Card)
                                    {
                                        case "S":
                                            rp.Feed("\x6", 6);
                                            break;
                                        case "L":
                                            rp.Feed("\x14", 14);
                                            break;
                                        default:
                                            break;
                                    }
                                }

                                /* change this to never print out due date at this stage 
                                if(pay[CN.DateFirst]==DBNull.Value ||
                                    (DateTime)pay[CN.DateFirst]==DateTime.MinValue.AddYears(1899))
                                    rp.PrintString("      ");
                                else
                                    rp.PrintString(((DateTime)pay[CN.DateFirst]).Day.ToString());
                                */
                                rp.PrintString("      ");
                                rp.Feed("\x2", 2);
                                rp.PrintString(((decimal)pay[CN.Deposit]).ToString(DecimalPlaces));
                                rp.Feed("\x2", 2);

                                /* never print installment 
                                if(pay[CN.DateDel]!=DBNull.Value)
                                    rp.PrintString(((decimal)pay[CN.InstalAmount]).ToString(DecimalPlaces));	
                                else
                                    rp.PrintString("      ");
                                */
                                rp.PrintString("      ");
                                rp.Feed("\x2", 2);
                                rp.PrintString(((decimal)pay[CN.AgreementTotal]).ToString(DecimalPlaces));
                                rp.Feed("\x2", 2);
                                rp.PrintString(accountNo);
                                rp.Feed("\x1", 1);
                                rp.PrintString((string)pay[CN.PostCode]);
                                rp.PrintString((string)pay[CN.Address3]);
                                rp.PrintString((string)pay[CN.Address2]);
                                rp.PrintString((string)pay[CN.Address1]);
                                rp.PrintString((string)pay[CN.Title] + " " + (string)pay[CN.LastName]);

                                rp.Release();
                                Thread.Sleep(1000);
                            }
                        }
                    }
                    else
                    {
                        ShowInfo("M_NOPAYMENTCARDINFO");
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
                if (rp != null) rp.ClosePrinter();
            }
        }

        #region CR 1086 - Themal Printing

        BBSL.Printing.IPrinter printer;
        public BBSL.Printing.IPrinter Printer
        {
            get
            {
                if (printer == null)
                    printer = new BBSL.Printing.EPSONPrinter(Config.ThermalPrinterName);
                return printer;
            }
        }

        #region Initialisation and Configuration

        protected bool ThermalPrintingEnabled
        { get { return Config.ThermalPrintingEnabled; } }

        protected bool PrintAutomaticMiniStatement
        {
            get
            {
                return Country.GetCountryParameterValue<bool>(CountryParameterNames.Printing.PrintAutomaticMiniStatement);
            }
        }

        public void InitialisePrinting()
        {
            Country.GetCountryParameterValueException += delegate (Exception ex) { this.Invoke(new MethodInvoker(delegate { Catch(ex, ""); })); };

            #region Base

            PrintContent.BusinessTitle.Value = Country.GetCountryParameterValue<string>(CountryParameterNames.Printing.BusinessTitle);
            PrintContent.BusinessRegNo.Value = Country.GetCountryParameterValue<string>(CountryParameterNames.Printing.BusinessRegNo);
            PrintContent.BusinessRegNo.ShouldBePrinted = Country.GetCountryParameterValue<bool>(CountryParameterNames.Printing.DisplayBusinessRegistrationNumber);
            PrintContent.BusinessRegNoLabel.Value = Country.GetCountryParameterValue<string>(CountryParameterNames.Printing.BusinessRegistrationLabel);
            PrintContent.BusinessRegNoLabel.ShouldBePrinted = Country.GetCountryParameterValue<bool>(CountryParameterNames.Printing.DisplayBusinessRegistrationLabel);
            PrintContent.MinimumHeight = Country.GetCountryParameterValue<Int32>(CountryParameterNames.Printing.ReceiptMinimumHeight);

            PrintContent.PrinterName = Config.ThermalPrinterName;
            PrintContent.CurrencyFormat = DecimalPlaces;
            PrintContent.Culture = Config.Culture;
            PrintContent.BusinessTaxNo.Value = Country.GetCountryParameterValue<string>(CountryParameterNames.Printing.TaxNumber);
            //PrintContent.BusinessTaxNo.ShouldBePrinted = Country.GetCountryParameterValue<bool>(CountryParameterNames.Printing.DisplayBusinessTaxNumber);
            PrintContent.BusinessTaxNoLabel.Value = Country.GetCountryParameterValue<string>(CountryParameterNames.Printing.TaxNumberLabel);
            PrintContent.BusinessTaxNoLabel.ShouldBePrinted = Country.GetCountryParameterValue<bool>(CountryParameterNames.Printing.DisplayTaxNumberLabel);

            try
            {
                PrintContent.DateTimeFormat =
                    (DateTimeFormats)Enum.Parse(typeof(DateTimeFormats), Country.GetCountryParameterValue<string>(CountryParameterNames.Printing.ReceiptDateFormat), true);
            }
            catch (Exception) { }

            ThreadPool.QueueUserWorkItem(delegate
            {
                DataRow branchAddress = null;
                DataSet ds = AccountManager.GetBranchAddress(Convert.ToInt32(Config.BranchCode), 1, out Error);
                foreach (DataTable dt in ds.Tables)
                    if (dt.TableName == "BranchDetails")
                        foreach (DataRow r in dt.Rows)
                            branchAddress = r;
                if (branchAddress != null)
                {
                    PrintContent.Store.Value = (string)branchAddress["BranchName"];
                    PrintContent.StoreAddress1.Value = (string)branchAddress["BranchAddr1"];
                    PrintContent.StoreAddress2.Value = (string)branchAddress["BranchAddr2"];
                    PrintContent.StoreAddress3.Value = (string)branchAddress["BranchAddr3"];
                }
            });

            #endregion

            #region Receipt

            BBSL.Libraries.Printing.PrintDocuments.Receipt.TitleSalesPerson = GetResource("T_SALESPERSON");
            BBSL.Libraries.Printing.PrintDocuments.Receipt.TitleCashierID = GetResource("T_CASHIER");
            BBSL.Libraries.Printing.PrintDocuments.Receipt.TitleAmountTendered = GetResource("T_AMOUNTTENDERED");
            BBSL.Libraries.Printing.PrintDocuments.Receipt.TitleChangeGiven = GetResource("T_CHANGEGIVEN");
            BBSL.Libraries.Printing.PrintDocuments.Receipt.TitleMoreRewardsNo = GetResource("T_MOREREWARDSNO");
            BBSL.Libraries.Printing.PrintDocuments.Receipt.TitleInvoiceNo = GetResource("T_INVOICENO");
            BBSL.Libraries.Printing.PrintDocuments.Receipt.TitlePayMethod = GetResource("T_PAYMETHOD");
            BBSL.Libraries.Printing.PrintDocuments.Receipt.TitleBranchCode = GetResource("T_BRANCH");
            BBSL.Libraries.Printing.PrintDocuments.Receipt.TitleAccountNo = GetResource("T_ACCOUNTNO");
            BBSL.Libraries.Printing.PrintDocuments.Receipt.TitleAccountBalance = GetResource("T_ACCOUNTBALANCE");
            BBSL.Libraries.Printing.PrintDocuments.Receipt.TitleAvailableSpend = GetResource("T_AVAILABLESPEND2");
            BBSL.Libraries.Printing.PrintDocuments.Receipt.TitleDeferredTermsAmount = GetResource("T_DEFERREDTERMSAMOUNT");
            BBSL.Libraries.Printing.PrintDocuments.Receipt.TitleSignatureText = Country.GetCountryParameterValue<string>(CountryParameterNames.Printing.CashNGoSignatureText);
            BBSL.Libraries.Printing.PrintDocuments.Receipt.TitleCustomerName = GetResource("T_CUSTOMERNAME");
            BBSL.Libraries.Printing.PrintDocuments.Receipt.TitleCustomerAddress = GetResource("T_CUSTOMERADDRESS");
            BBSL.Libraries.Printing.PrintDocuments.Receipt.TitleDatePrinted = GetResource("T_DATEPRINTED2");                           //IP - 09/05/12 - #9608 - CR8520
            BBSL.Libraries.Printing.PrintDocuments.Receipt.TitleReprintDate = GetResource("T_DATEREPRINTED");                         //IP - 09/05/12 - #9608 - CR8520

            if (Config.CountryCode == "C")
            {
                BBSL.Libraries.Printing.PrintDocuments.Receipt.TitleAmount = "Mt";
                BBSL.Libraries.Printing.PrintDocuments.Receipt.TitleTotalAmount = "Mt";
                BBSL.Libraries.Printing.PrintDocuments.Receipt.TitleTax = Country.GetCountryParameterValue<string>(CountryParameterNames.TaxName);
                BBSL.Libraries.Printing.PrintDocuments.Receipt.TitleSubTotal = "Sous Total";
            }
            else if (Config.CountryCode == "Q")
            {
                BBSL.Libraries.Printing.PrintDocuments.Receipt.TitleTotalAmount = GetResource("T_AMT");
                BBSL.Libraries.Printing.PrintDocuments.Receipt.TitleAmount = GetResource("T_AMT");
                BBSL.Libraries.Printing.PrintDocuments.Receipt.TitleTax = "OB";//Country.GetCountryParameterValue<string>(CountryParameterNames.TaxName);
                BBSL.Libraries.Printing.PrintDocuments.Receipt.TitleSubTotal = GetResource("T_SUBTOTAL");

                ///TODO - ASH
                //BBSL.Libraries.Printing.PrintDocuments.Receipt.TitleTotalAmount = GetResource("T_AMT");
                //BBSL.Libraries.Printing.PrintDocuments.Receipt.TitleAmount = GetResource("T_AMT");
                BBSL.Libraries.Printing.PrintDocuments.Receipt.TitleLuxTax = "LUX"; //Country.GetCountryParameterValue<string>(CountryParameterNames.TaxName ) +
                //BBSL.Libraries.Printing.PrintDocuments.Receipt.TitleSubTotal = GetResource("T_SUBTOTAL");
            }
            else
            {
                BBSL.Libraries.Printing.PrintDocuments.Receipt.TitleTotalAmount = GetResource("T_AMT");
                BBSL.Libraries.Printing.PrintDocuments.Receipt.TitleAmount = GetResource("T_AMT");
                BBSL.Libraries.Printing.PrintDocuments.Receipt.TitleTax = Country.GetCountryParameterValue<string>(CountryParameterNames.TaxName);
                BBSL.Libraries.Printing.PrintDocuments.Receipt.TitleSubTotal = GetResource("T_SUBTOTAL");



            }

            switch (Config.CountryCode)
            {
                case "C":
                    BBSL.Libraries.Printing.PrintDocuments.Receipt.TitleTaxInvoice = "FACTURE TVA";
                    BBSL.Libraries.Printing.PrintDocuments.Receipt.TitleSalesPerson = "Vendeur";
                    break;
                case "M":
                    BBSL.Libraries.Printing.PrintDocuments.Receipt.TitleTaxInvoice = "INVOICE";
                    break;
                default:
                    BBSL.Libraries.Printing.PrintDocuments.Receipt.TitleTaxInvoice = "Ord/Invoice No";
                    break;
            }
            //CR 2018-13
            BBSL.Libraries.Printing.PrintDocuments.Receipt.TitleInvoiceDate = "Invoice Date:";

            switch (Config.CountryCode)
            {
                case "C":
                    BBSL.Libraries.Printing.PrintDocuments.Receipt.TitleTotalAmount = "Mt Total";
                    BBSL.Libraries.Printing.PrintDocuments.Receipt.TitleInvoiceTotal = "TTC";
                    break;
                default:
                    BBSL.Libraries.Printing.PrintDocuments.Receipt.TitleTotalAmount = GetResource("T_TOTALAMT");
                    BBSL.Libraries.Printing.PrintDocuments.Receipt.TitleInvoiceTotal = GetResource("T_INVOICETOTAL");
                    break;
            }

            #endregion

            #region Payment Receipt

            PaymentReceipt.TitleAccountBalance = GetResource("T_ACCOUNTBALANCE");
            PaymentReceipt.TitleAccountNo = GetResource("T_ACCOUNTNUMBER");
            PaymentReceipt.TitleAmount = GetResource("T_AMOUNT1");
            PaymentReceipt.TitleBranchCode = GetResource("T_BRANCH");
            PaymentReceipt.TitleCustomerName = GetResource("T_CUSTOMERNAME");
            PaymentReceipt.TitleStoreAddress = GetResource("T_BRANCHADDRESS");
            PaymentReceipt.TitleTransactionNo = GetResource("T_TRANSACTIONNO");
            PaymentReceipt.TitleType = GetResource("T_TRANSACTIONTYPE");
            PaymentReceipt.TitleTransactionDate = GetResource("T_TRANSACTIONDATE");
            PaymentReceipt.TitleSalesPerson = GetResource("T_SALESPERSON");
            PaymentReceipt.TitleCashierID = GetResource("T_CASHIEID");
            PaymentReceipt.Footer.Value = Country.GetCountryParameterValue<string>(CountryParameterNames.Printing.PaymentFooter);

            #endregion

            #region Payment Statement

            PaymentStatement.TitleRemainingBalance = GetResource("T_ACCOUNTBALANCE");
            PaymentStatement.TitleAccountNo = GetResource("T_ACCOUNTNUMBER");
            PaymentStatement.TitleAmountPaid = GetResource("T_AMOUNT1");
            PaymentStatement.TitleCustomerName = GetResource("T_CUSTOMERNAME");
            PaymentStatement.TitleAgreementTotal = GetResource("T_AGREETOTAL");
            PaymentStatement.TitleAvailableSpend = GetResource("T_AVAILABLESPEND2");
            PaymentStatement.TitleCashierID = GetResource("T_CASHIEID");
            PaymentStatement.TitleCreditLimit = GetResource("T_CREDITLIMIT");
            PaymentStatement.TitleCustomerAddress = GetResource("T_CUSTOMERADDRESS");
            PaymentStatement.TitleDatePaid = GetResource("T_DATEPAID");
            PaymentStatement.TitlePayments = GetResource("T_PAYMENTS");
            PaymentStatement.TitlePaymentType = GetResource("T_TYPE");
            PaymentStatement.TitleRemainingBalance = GetResource("T_BALANCE1");
            PaymentStatement.TitleReferenceNo = GetResource("T_REFNO");
            PaymentStatement.TitlePaymentTotal = GetResource("T_PAYMENTTOTAL");
            PaymentStatement.TitleBalanceTotal = GetResource("T_BALANCETOTAL");

            #endregion

            #region Mini Statement

            MiniStatement.TitleAccountNo = GetResource("T_ACCTNO");
            MiniStatement.TitleAgreementTotal = GetResource("T_AGREETOTAL");
            MiniStatement.TitleAmountPaid = GetResource("T_AMT");
            MiniStatement.TitleAvailableSpend = GetResource("T_AVAILABLESPEND2");
            MiniStatement.TitleCashierID = GetResource("T_CASHIEID");
            MiniStatement.TitleCreditLimit = GetResource("T_CREDITLIMIT");
            MiniStatement.TitleCustomerAddress = GetResource("T_CUSTOMERADDRESS");
            MiniStatement.TitleCustomerName = GetResource("T_CUSTOMERNAME");
            MiniStatement.TitleDatePaid = GetResource("T_DATE");
            MiniStatement.TitlePaymentType = GetResource("T_TYPE");
            MiniStatement.TitleReferenceNo = GetResource("T_REFNO1");
            MiniStatement.TitleRemainingBalance = GetResource("T_BALANCE2");
            MiniStatement.TitlePayments = GetResource("T_TRANSACTIONS1");
            MiniStatement.TitleDateFrom = GetResource("T_DATEFROM");
            MiniStatement.TitleDateTo = GetResource("T_DATETO");

            #endregion

            #region Store Card Receipt
            StoreCardReceipt.TitleCustomerName = GetResource("T_CUSTOMERNAME");
            StoreCardReceipt.TitleCustomerAddress = GetResource("T_CUSTOMERADDRESS");
            StoreCardReceipt.TitleMachineName = GetResource("T_MACHINENAME");
            StoreCardReceipt.TitleInvoiceNumber = GetResource("T_INVOICE_NUMBER");
            StoreCardReceipt.TitleReceiptNumber = GetResource("T_RECEIPTNUMBER");
            StoreCardReceipt.TitleStoreCardName = GetResource("T_STORECARDNAME");
            StoreCardReceipt.TitleStoreCardNumber = GetResource("T_STORECARDNUMBER");
            StoreCardReceipt.TitleStoreCardExpiryDate = GetResource("T_EXPIRYDATE");
            StoreCardReceipt.TitleStoreCardLimit = GetResource("T_STORECARDLIMIT");
            StoreCardReceipt.TitleStoreCardAvailable = GetResource("T_STORECARDAVAILABLE");
            StoreCardReceipt.TitleAmountPaid = GetResource("T_AMOUNT1");
            StoreCardReceipt.TitleTransactionDate = GetResource("T_TRANSACTIONDATE");
            StoreCardReceipt.TitleSignatureText = GetResource("T_CUSTSIGNATURE");
            StoreCardReceipt.TitleSignatureText = Country.GetCountryParameterValue<string>(CountryParameterNames.Printing.SCardReceiptSignatureText);

            #endregion
        }

        void ApplyCountryParametersForCashNGo(ref BBSL.Libraries.Printing.PrintDocuments.Receipt receipt)
        {
            receipt.Title.Value = Country.GetCountryParameterValue<string>(CountryParameterNames.Printing.CashNGoReceiptTitle);
            receipt.Title.ShouldBePrinted = Country.GetCountryParameterValue<bool>(CountryParameterNames.Printing.DisplayCashNGoReceiptTitle);
            receipt.Footer.Value = Country.GetCountryParameterValue<string>(CountryParameterNames.Printing.CashNGoFooter);
            receipt.Footer.ShouldBePrinted = Country.GetCountryParameterValue<bool>(CountryParameterNames.Printing.CashNGoDisplayFooter);
            receipt.IncludeSignatureStrip = Country.GetCountryParameterValue<bool>(CountryParameterNames.Printing.CashNGoDisplaySignature);

            PrintContent.BusinessTaxNo.ShouldBePrinted = Country.GetCountryParameterValue<bool>(CountryParameterNames.Printing.CashNGoDisplayVATNumber);
        }
        void ApplyCountryParametersForTaxInvoice(ref BBSL.Libraries.Printing.PrintDocuments.Receipt receipt)
        {
            receipt.Title.Value = Country.GetCountryParameterValue<string>(CountryParameterNames.Printing.TaxInvoiceTitle);
            receipt.Title.ShouldBePrinted = Country.GetCountryParameterValue<bool>(CountryParameterNames.Printing.DisplayTaxInvoiceTitle);
            receipt.Footer.Value = Country.GetCountryParameterValue<string>(CountryParameterNames.Printing.TaxInvoiceFooter);
            receipt.Footer.ShouldBePrinted = Country.GetCountryParameterValue<bool>(CountryParameterNames.Printing.TaxInvoiceDisplayFooter);

            PrintContent.BusinessTaxNo.ShouldBePrinted = Country.GetCountryParameterValue<bool>(CountryParameterNames.Printing.TaxInvoiceDisplayVATNumber);
        }
        void ApplyCountryParametersForPayment(ref BBSL.Libraries.Printing.PrintDocuments.PaymentStatement paymentStatement)
        {
            paymentStatement.Title.Value = Country.GetCountryParameterValue<string>(CountryParameterNames.Printing.PaymentReceiptTitle);
            paymentStatement.Title.ShouldBePrinted = Country.GetCountryParameterValue<bool>(CountryParameterNames.Printing.DisplayPaymentReceiptTitle);
            paymentStatement.Footer.Value = Country.GetCountryParameterValue<string>(CountryParameterNames.Printing.PaymentFooter);
            paymentStatement.Footer.ShouldBePrinted = Country.GetCountryParameterValue<bool>(CountryParameterNames.Printing.PaymentReceiptDisplayFooter);

            PrintContent.BusinessTaxNo.ShouldBePrinted = Country.GetCountryParameterValue<bool>(CountryParameterNames.Printing.PaymentReceiptDisplayVATNumber);
        }

        //IP - 11/01/10 - Store Card
        void ApplyCountryParametersForPayment(ref BBSL.Libraries.Printing.PrintDocuments.StoreCardReceipt storeCardReceipt)
        {
            storeCardReceipt.Title.Value = Country.GetCountryParameterValue<string>(CountryParameterNames.Printing.SCardReceiptTitle);
            storeCardReceipt.Title.ShouldBePrinted = Country.GetCountryParameterValue<bool>(CountryParameterNames.Printing.SCardReceiptDisplayTitle);
            storeCardReceipt.Footer.Value = Country.GetCountryParameterValue<string>(CountryParameterNames.Printing.SCardReceiptFooter);
            storeCardReceipt.Footer.ShouldBePrinted = Country.GetCountryParameterValue<bool>(CountryParameterNames.Printing.SCardReceiptDisplayFooter);
            storeCardReceipt.IncludeSignatureStrip = Country.GetCountryParameterValue<bool>(CountryParameterNames.Printing.SCardReceiptDisplaySignature);

        }

        #endregion

        #region Decision Making - Which printer to use
        /// <summary>
        /// IP - 09/05/12 - #9609 - CR8520
        /// </summary>
        public void NewPrintTaxInvoicePaidAndTakenBulk(DataSet payMethodSet)
        {
            this.PrintReceiptBulk(payMethodSet);
        }

        public void NewPrintTaxInvoice(string accountNo,
                                        int agreementNo,
                                        string accountType,
                                        string customerID,
                                        bool paidAndTaken,
                                        bool collection,
                                        InstantReplacementDetails replacement,
                                        decimal paid,
                                        decimal change,
                                        XmlNode lineItems,
                                        int buffNo,
                                        AxWebBrowser browser,
                                        ref int noPrints,
                                        bool creditNote,
                                        bool multiple,
                                        int salesPerson,
                                        short paymentMethod,
                                        DataSet payMethodSet,
                                        bool taxExempt,
                                        string salesPersonName = "",                      //IP - 17/05/12 - #9447 - CR1239
                                        string cashierName = "",                          //IP - 17/05/12 - #9447 - CR1239
                                        int cashierID = 0                                //IP - 17/05/12 - #9447 - CR1239
                                        , int versionNo = 0, bool ReprintInvoice = false)
        {
            ReprintInvoiceOption = ReprintInvoice;
            if (creditNote) //Always print a credit note on the laser printer
            {
                browser.TabIndex = noPrints++;
                this.LaserPrintTaxInvoice(browser,
                    accountNo,
                    agreementNo,
                    accountType,
                    customerID,
                    paidAndTaken,
                    collection,
                    lineItems,
                    buffNo,
                    creditNote,
                    multiple);
                return;
            }

            if (paidAndTaken && ReprintInvoiceOption == false)
            {

                this.PrintReceipt(customerID: customerID,
                                  accountNo: accountNo,
                                  collection: collection,
                                  replacement: replacement,
                                  lineItems: lineItems,
                                  paid: paid,
                                  change: change,
                                  sender: this,
                                  buffNo: buffNo,
                                  taxExempt: taxExempt,
                                  salesPerson: salesPerson,
                                  paidAndTaken: paidAndTaken,
                                  paymentMethod: paymentMethod,
                                  payMethodSet: payMethodSet,
                                  salesPersonName: salesPersonName,                 //IP - 17/05/12 - #9447 - CR1239
                                  cashierName: cashierName,                         //IP - 17/05/12 - #9447 - CR1239
                                  cashierID: cashierID);                            //IP - 17/05/12 - #9447 - CR1239

                ThreadPool.QueueUserWorkItem(new WaitCallback(delegate
                {
                    // Audit the Tax Invoice print if it is a re-print
                    AccountManager.AuditReprint(accountNo, agreementNo, DocumentType.TaxInvoice);
                }));
            }
            else
            {
                bool useReceiptPrinter = false;
                bool useLaserPrinter = false;

                switch ((string)Country[CountryParameterNames.TaxInvType])
                {

                    case "0":
                        if (GetType().Name == "Payment")		//IP - 18/11/10 - LW 72856 - Merged from 5.1
                            useReceiptPrinter = true;
                        break;
                    case "1":
                        useLaserPrinter = true;
                        break;
                    case "2":
                        useReceiptPrinter = true;
                        break;
                    default: //useReceiptPrinter = true;
                        break;
                }

                if (useLaserPrinter)
                {
                    browser.TabIndex = noPrints++;
                    this.LaserPrintTaxInvoice(browser,
                        accountNo,
                        agreementNo,
                        accountType,
                        customerID,
                        paidAndTaken,
                        collection,
                        lineItems,
                        buffNo,
                        creditNote,
                        multiple
                        , versionNo
                        , ReprintInvoice);
                }
                else if (useReceiptPrinter)
                {
                    taxExempt = AccountManager.IsTaxExempt(accountNo, agreementNo.ToString(), out Error);

                    if (Error.Length > 0)
                        ShowError(Error);
                    else
                    {
                        if (ThermalPrintingEnabled)
                        {
                            if (ReprintInvoiceOption == true)
                            {
                                ThermalPrintTaxInvoice
                                    (
                                        accountNo,
                                        agreementNo,
                                        accountType,
                                        customerID,
                                        paidAndTaken,
                                        collection,
                                        lineItems,
                                        buffNo,
                                        creditNote,
                                        multiple,
                                        taxExempt     //#18112
                                        , versionNo, ReprintInvoice
                                    );

                            }
                            else
                            {
                                (new Thread(new ThreadStart(delegate
                                {
                                    ThermalPrintTaxInvoice
                                        (
                                            accountNo,
                                            agreementNo,
                                            accountType,
                                            customerID,
                                            paidAndTaken,
                                            collection,
                                            lineItems,
                                            buffNo,
                                            creditNote,
                                            multiple,
                                            taxExempt     //#18112
                                            , versionNo, ReprintInvoice
                                        );
                                }))).Start();
                            }
                        }
                        else
                        {
                            SlipPrinterPrintReceipt(customerID,
                                accountNo,
                                collection,
                                replacement,
                                lineItems, paid,
                                change,
                                this,
                                buffNo,
                                taxExempt,
                                salesPerson,
                                paidAndTaken,
                                paymentMethod,
                                payMethodSet);
                        }

                        //this.PrintReceipt(customerID, accountNo,
                        //    collection,
                        //    replacement,
                        //    lineItems,
                        //    paid,
                        //    change,
                        //    this,
                        //    buffNo,
                        //    taxExempt,
                        //    salesPerson,
                        //    paidAndTaken,
                        //    paymentMethod,
                        //    payMethodSet);

                        ThreadPool.QueueUserWorkItem(new WaitCallback(delegate
                        {
                            // Audit the Tax Invoice print if it is a re-print
                            AccountManager.AuditReprint(accountNo, agreementNo, DocumentType.TaxInvoice);
                        }));
                    }
                }
            }

            //if (paidAndTaken)
            //{
            //    switch ((string)Country[CountryParameterNames.TaxInvType])
            //    {
            //        case "4": laser = true;
            //            break;
            //        default: receipt = true;
            //            break;
            //    }
            //}
            //else
            //{
            //    switch ((string)Country[CountryParameterNames.TaxInvType])
            //    {
            //        case "0": if (GetType().Name == "Payment")		/* nonsense */
            //                receipt = true;
            //            break;
            //        case "2": receipt = true;
            //            break;
            //        default: laser = true;
            //            break;
            //    }
            //}

            //if (laser || creditNote)
            //{
            //    browser.TabIndex = noPrints++;
            //    this.PrintTaxInvoice(browser,
            //        accountNo,
            //        agreementNo,
            //        accountType,
            //        customerID,
            //        paidAndTaken,
            //        collection,
            //        lineItems,
            //        buffNo,
            //        creditNote,
            //        multiple);
            //}

            //if (receipt && !creditNote)
            //{
            //    //if (this.SlipPrinterOK(true))
            //    {
            //        /* determine value of taxExempt property */

            //        if (!paidAndTaken) // If tax exempt hasn't been ticked then check account. SC 70687 11/2/09
            //        {
            //            taxExempt = AccountManager.IsTaxExempt(accountNo, agreementNo.ToString(), out Error);
            //        }


            //        if (Error.Length > 0)
            //            ShowError(Error);
            //        else
            //        {
            //            this.PrintReceipt(customerID, accountNo,
            //                collection,
            //                replacement,
            //                lineItems,
            //                paid,
            //                change,
            //                this,
            //                buffNo,
            //                taxExempt,
            //                salesPerson,
            //                paidAndTaken,
            //                paymentMethod,
            //                payMethodSet);

            //            // Audit the Tax Invoice print if it is a re-print
            //            AccountManager.AuditReprint(accountNo, agreementNo, DocumentType.TaxInvoice);
            //        }
            //    }
            //}
        }

        public void NewPrintPaymentReceipt(DataTable transactions,
                                                decimal finalBalance,
                                                decimal creditLimit,
                                                decimal availableSpend,
                                                string customerID,
                                                string customerName,
                                                string accountNo,
                                                short paymentMethod,
                                                bool combinedRF,
                                                string accountType,                          //IP - 21/05/12 - #10146
                                                bool printMiniStat = false)                  //IP - 18/05/12 - #9445 - CR1239
        {
            if (ThermalPrintingEnabled)
            {
                (new Thread(new ThreadStart(delegate
                {
                    ThermalPrintPaymentReceipt
                            (
                                transactions,
                                finalBalance,
                                customerID,
                                customerName,
                                creditLimit,
                                availableSpend,
                                accountNo,
                                paymentMethod,
                                combinedRF,
                                accountType,                                   //IP - 21/05/12 - #10146
                                printMiniStat                                //IP - 18/05/12 - #9445 - CR1239
                            );
                }))).Start();

            }
            else
            {
                SlipPrinterPrintPaymentReceipt(transactions, customerName, accountNo, finalBalance, paymentMethod);
            }
        }

        public void PrintPaymentReceipt(DataTable transactions,
                                        string customerName,
                                        string accountNo,
                                        decimal finalBalance,
                                        short paymentMethod)
        {
            if (ThermalPrintingEnabled)
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(delegate
                {
                    ThermalPrintPaymentReceiptOldStyle
                        (
                            transactions,
                            customerName,
                            accountNo,
                            finalBalance,
                            paymentMethod
                        );
                }));
            }
            else
            {
                SlipPrinterPrintPaymentReceipt(transactions, customerName, accountNo, finalBalance, paymentMethod);
            }
        }


        //IP - 12/10/11 - #3921 - CR1232
        public void NewPrintCashLoanReceipt(string customerName, string acctNo, DateTime transDate, string type, decimal amtDisbursed, int transNo, decimal outstBal, DateTime firstInstalDate, string disbursementType)
        {
            if (ThermalPrintingEnabled)
            {
                (new Thread(new ThreadStart(delegate
                {
                    ThermalPrintCashLoanReceipt
                            (
                                customerName, acctNo, transDate, type, amtDisbursed, transNo, outstBal, firstInstalDate, disbursementType
                            );
                }))).Start();

            }
            else
            {
                SlipPrinterPrintCashLoanReceipt(customerName, acctNo, transDate, type, amtDisbursed, transNo, outstBal, firstInstalDate, disbursementType);
            }
        }

        //IP - 22/12/11 - #8811 - CR1234
        public void NewPrintCashierDisbursementReceipt(DataTable cashierDisbursements,
                                               string cashier)
        {
            if (ThermalPrintingEnabled)
            {
                (new Thread(new ThreadStart(delegate
                {
                    ThermalPrintDisbursementReceipt
                            (
                               cashierDisbursements,
                               cashier
                            );
                }))).Start();

            }

        }

        //IP - 12/10/11 - #3921 - CR1232
        void SlipPrinterPrintCashLoanReceipt(string customerName, string acctNo, DateTime transDate, string type, decimal amtDisbursed, int transNo, decimal outstBal, DateTime firstInstalDate, string disbursementType)
        {
            if (this.SlipPrinterOK(true))
            {
                ReceiptPrinter rp = null;
                try
                {
                    Wait();
                    Function = "SlipPrinterPrintCashLoanReceipt";
                    DateTime recent = DateTime.Today;

                    rp = new ReceiptPrinter(this);
                    rp.OpenPrinter();


                    // Currently have to reopen printer to refresh paper out sensor
                    rp.ReOpenPrinter();

                    rp.Feed("\x2", 2);
                    rp.Narrow();
                    rp.Init();
                    rp.LineSpacing = 15;

                    bool openDrawer = Config.CashDrawerID.Length > 0 || (bool)Country[CountryParameterNames.OpenCashDrawerForCredit];

                    if (openDrawer)
                        rp.OpenDrawer();

                    string printLine = "";
                    printLine += customerName + Environment.NewLine;
                    printLine += Environment.NewLine;
                    printLine += GetResource("T_ACCOUNTNUMBER") + ": " + acctNo + Environment.NewLine;
                    printLine += GetResource("T_DISBURSEMENTTYPE") + ": " + disbursementType + Environment.NewLine;
                    printLine += GetResource("T_TRANSACTIONDATE") + ": ";
                    printLine += transDate.Day.ToString().PadLeft(2, '0');
                    printLine += transDate.Month.ToString().PadLeft(2, '0');
                    printLine += transDate.Year.ToString().Substring(2, 2) + Environment.NewLine;
                    printLine += GetResource("T_TYPE") + ": " + type + Environment.NewLine;
                    printLine += (GetResource("T_AMOUNTDISBURSED")).PadRight(12, ' ');
                    printLine += amtDisbursed.ToString(DecimalPlaces.Replace("C", "F")).PadLeft(1, ' ') + Environment.NewLine;
                    printLine += GetResource("T_TRANSACTIONNO") + ": " + transNo.ToString().PadLeft(6, '0') + Environment.NewLine;
                    printLine += (GetResource("T_BALANCE")).PadRight(12, ' ');
                    if (outstBal < 0)
                        printLine += (-outstBal).ToString(DecimalPlaces.Replace("C", "F")).PadLeft(20, ' ') + GetResource("T_CR");
                    else
                        printLine += outstBal.ToString(DecimalPlaces.Replace("C", "F")).PadLeft(20, ' ');
                    printLine += Environment.NewLine;
                    printLine += Environment.NewLine;
                    printLine += GetResource("T_BRANCH") + ": " + Config.BranchCode;
                    rp.PrintString(printLine);

                    rp.Release();
                    Thread.Sleep(1000);

                }
                catch (SlipPrinterException ex)
                {
                    if (ex.Message != "Cancel")
                        Catch((Exception)ex, Function);
                }
                catch (Exception ex)
                {
                    Catch(ex, Function);
                }
                finally
                {
                    StopWait();
                    if (rp != null) rp.ClosePrinter();
                }
            }
        }

        /// <summary>
        /// //IP - 09/05/12 - #9609 - CR8520
        /// </summary>
        public void PrintReceiptBulk(DataSet payMethodSet)
        {

            //ThreadPool.QueueUserWorkItem(new WaitCallback(delegate        //IP - 16/05/12 - #9609 - CR8520 - caused exception when printing
            //{
            //  //IP - 10/05/12 - #9609 - CR8520
            using (var receipt = new BBSL.Libraries.Printing.PrintDocuments.ReceiptBulk(payMethodSet))
            {
                receipt.BeforePrintPage = (i) => PrintReceiptBulkOne(payMethodSet, receipt, i);

                printDialog1.Document = receipt;

                var result = printDialog1.ShowDialog();

                if (result == DialogResult.OK)
                {
                    receipt.Print();
                }

            }
            //}));
        }

        //IP - 11/05/12 - #9609 - CR8520
        private void PrintReceiptBulkOne(DataSet payMethodSet, ReceiptBulk receipt, int i)
        {
            var dv = new DataView(payMethodSet.Tables[TN.PayMethodList]);       //DataView of Paymethods used on invoice
            var payments = new DataTable();
            var transactionsSet = new DataSet();

            var accountNo = Convert.ToString(payMethodSet.Tables[TN.Accounts].Rows[i][CN.acctno]);
            var buffNo = Convert.ToInt32(payMethodSet.Tables[TN.Accounts].Rows[i][CN.BuffNo]);
            var salesPerson = Convert.ToInt32(payMethodSet.Tables[TN.Accounts].Rows[i][CN.EmpeeNoSale]);
            var salesPersonName = Convert.ToString(payMethodSet.Tables[TN.Accounts].Rows[i][CN.SalesPersonName]);               //IP - 17/05/12 - #9447 - CR1239    
            var cashierEmpeeNo = Convert.ToInt32(payMethodSet.Tables[TN.Accounts].Rows[i][CN.CashierEmpeeNo]);                  //IP - 17/05/12 - #9447 - CR1239  
            var cashierName = Convert.ToString(payMethodSet.Tables[TN.Accounts].Rows[i][CN.CashierName]);                       //IP - 17/05/12 - #9447 - CR1239  
            var taxExempt = Convert.ToBoolean(payMethodSet.Tables[TN.Accounts].Rows[i][CN.TaxExempt]);                          //IP - 21/05/12 - #10144 - CR1239
            var change = Convert.ToDecimal(payMethodSet.Tables[TN.Accounts].Rows[i][CN.Change]);                                //IP - 21/05/12 - #10145 - CR1239
            var lineItems = AccountManager.GetLineItems(accountNo, buffNo, AT.Special, Config.CountryCode, Convert.ToInt16(Config.BranchCode), out Error);

            dv.RowFilter = CN.acctno + "= '" + accountNo + "' and " + CN.AgrmtNo + "= '" + buffNo + "'";

            payments = dv.ToTable();

            var payMethod = payments.Rows.Count > 1 ? Convert.ToInt16(0) : Convert.ToInt16(payments.Rows[0][CN.PayMethod]);

            transactionsSet.Tables.Add(payments);
            payments.TableName = TN.PayMethodList;

            this.ThermalPrintReceipt(customerID: "PAID & TAKEN",
                                     accountNo: accountNo,
                                     collection: false,
                                     replacement: null,
                                     lineItems: lineItems,
                                     paid: 0,
                                     change: change,
                                     reprint: true,
                                     buffNo: buffNo,
                                     taxExempt: taxExempt,
                                     salesPerson: salesPerson,
                                     paidAndTaken: true,
                                     paymentMethod: payMethod,
                                     payMethodSet: transactionsSet,
                                     isBulk: true,
                                     receipt: receipt,
                                     salesPersonName: salesPersonName,                                                              //IP - 17/05/12 - #9447 - CR1239 
                                     cashierName: cashierName,                                                                      //IP - 17/05/12 - #9447 - CR1239 
                                     cashierID: cashierEmpeeNo);                                                                    //IP - 17/05/12 - #9447 - CR1239 

        }

        public void PrintReceipt(string customerID,
                                    string accountNo,
                                    bool collection,
                                    InstantReplacementDetails replacement,
                                    XmlNode lineItems, decimal paid,
                                    decimal change,
                                    CommonForm sender,
                                    int buffNo,
                                    bool taxExempt,
                                    int salesPerson,
                                    bool paidAndTaken,
                                    short paymentMethod,
                                    DataSet payMethodSet,                                        //IP - 09/05/12 - #9609 - CR8520
                                    string salesPersonName = "",                                //IP - 17/05/12 - #9447 - CR1239
                                    string cashierName = "",                                    //IP - 17/05/12 - #9447 - CR1239
                                    int cashierID = 0)                                          //IP - 17/05/12 - #9447 - CR1239
        {
            if (ThermalPrintingEnabled)
            {
                var dsPayMethod = payMethodSet.Copy();

                ThreadPool.QueueUserWorkItem(new WaitCallback(delegate
                {
                    ThermalPrintReceipt
                        (
                        customerID: customerID,
                        accountNo: accountNo,
                        collection: collection,
                        replacement: replacement,
                        lineItems: lineItems,
                        paid: paid,
                        change: change,
                        reprint: (sender.Name == "SearchCashAndGo"),          //IP - 11/05/12 - #9609 - CR8520
                        buffNo: buffNo,
                        taxExempt: taxExempt,
                        salesPerson: salesPerson,
                        paidAndTaken: paidAndTaken,
                        paymentMethod: paymentMethod,
                        payMethodSet: dsPayMethod,
                        salesPersonName: salesPersonName,                   //IP - 17/05/12 - #9447 - CR1239
                        cashierName: cashierName,                           //IP - 17/05/12 - #9447 - CR1239
                        cashierID: cashierID                                //IP - 17/05/12 - #9447 - CR1239
                        );
                }));
            }
            else
            {
                SlipPrinterPrintReceipt(customerID,
                    accountNo,
                    collection,
                    replacement,
                    lineItems, paid,
                    change,
                    sender,
                    buffNo,
                    taxExempt,
                    salesPerson,
                    paidAndTaken,
                    paymentMethod,
                    payMethodSet);
            }
        }

        #endregion

        #region Slip Printer

        public int PrintPaymentCardTransactions(DataTable transactions,
                                                        int startLine,
                                                        decimal finalBalance,
                                                        string customerID,
                                                        string accountNo,
                                                        short paymentMethod,
                                                        bool paymentMade)
        {
            ReceiptPrinter rp = null;
            bool tooLong = false;
            try
            {
                Wait();
                Function = "SlipPrinterPrintPaymentCardTransactions";

                if (this.SlipPrinterOK(true))
                {
                    decimal balance = finalBalance;
                    bool updateRow = true;

                    DataTable merged = MergePaymentTransactions(transactions, balance, ref updateRow);

                    DataView transView = new DataView(transactions, "", CN.DateTrans + " DESC," + CN.TransTypeCode + " DESC", DataViewRowState.CurrentRows);
                    transView.RowFilter = CN.TransPrinted + " = 'N'";

                    //if there were no transaction for the current date then we need to add a dummy 
                    //row to the merged table
                    if (updateRow)
                    {
                        DataRow dummy = merged.NewRow();
                        dummy[CN.acctno] = "dummy";		//so we can tell we don't to update db for this row
                        dummy[CN.DateTrans] = DateTime.Today;
                        dummy[CN.TransRefNo] = 0;
                        dummy[CN.TransTypeCode] = TransType.Update;
                        dummy[CN.TransValue] = 0;
                        dummy[CN.Balance] = finalBalance;
                        merged.Rows.Add(dummy);
                    }

                    int cardType = AccountManager.GetPaymentCardType(Convert.ToInt16(Config.BranchCode),
                        StaticDataManager.GetServerDate(),
                        out Error);

                    //fire up the receipt printer and go to the right start line
                    rp = new ReceiptPrinter(this);
                    if (cardType == 0)
                        rp.Card = CardType.typeShort;
                    else
                        rp.Card = CardType.typeLong;

                    rp.OpenPrinter();
                    rp.Init();
                    rp.ReverseFeed("\x4", startLine);
                    rp.Narrow();
                    rp.LineSpacing = 15;

                    if (Config.ReceiptPrinterModel == rpNewModel)
                        rp.Feed(startLine - 1);

                    //69378 If a payment is being taken then the cash drawer should open
                    if (paymentMade)
                    {
                        bool openDrawer = Config.CashDrawerID.Length > 0 &&
                           ((paymentMethod != 3 && paymentMethod != 4) || (bool)Country[CountryParameterNames.OpenCashDrawerForCredit]);

                        if (openDrawer)
                            rp.OpenDrawer();
                    }
                    /* Print the transactions in the merged table
                     * use a view to reverse the order
                     * Before a transaction is printed, update the fintrans table to "Y"
                     * if there is a problem printing the transaction, then the fintrans
                     * must be updated to reflect that the transaction was not successfully 
                     * printed  - there is still a small chance that the second compensating update
                     * might fail but this is acceptable
                     */
                    // DSR 9/4/03 TransTypeCode now sorted ASCENDING to print REB after PAY
                    DataView mergedView = new DataView(merged, "", CN.DateTrans + " ASC, " + CN.TransTypeCode + " ASC", DataViewRowState.CurrentRows);
                    foreach (DataRowView drv in mergedView)
                    {
                        switch (cardType)
                        {
                            case 0:
                                tooLong = startLine > CardType.rowsShort;
                                break;
                            case 1:
                                tooLong = startLine > CardType.rowsLong;
                                break;
                            default:
                                break;
                        }

                        if (tooLong)
                        {
                            startLine = 1;
                            rp.Release();
                            if (!(DialogResult.OK == ShowInfo("M_PRINTNEWPAYMENTCARD", MessageBoxButtons.OKCancel)))
                                throw new SlipPrinterException("Cancel");
                            else
                            {
                                rp.ClosePrinter();
                                this.PrintPaymentCard(customerID, accountNo);
                                if (!(DialogResult.OK == ShowInfo("M_REINSERTPAYMENTCARD", MessageBoxButtons.OKCancel)))
                                    throw new SlipPrinterException("Cancel");
                                else
                                {
                                    rp.OpenPrinter();
                                    rp.Init();
                                    rp.ReverseFeed("\x4", startLine);
                                    rp.Narrow();
                                    rp.LineSpacing = 15;
                                    rp.Feed(startLine - 1);
                                }
                            }
                        }

                        transView.RowFilter =
                            CN.DateTrans + " >= '" + ((DateTime)drv[CN.DateTrans]).Date + "' AND " +
                            CN.DateTrans + " <  '" + ((DateTime)drv[CN.DateTrans]).Date.AddDays(1) + "' AND " +
                            CN.TransTypeCode + " = '" + (string)drv[CN.TransTypeCode] + "'";

                        foreach (DataRowView r in transView)
                        {
                            //update fintrans for these record(s)
                            AccountManager.SetPaymentCardPrinted((string)r[CN.acctno],
                                (int)r[CN.TransRefNo],
                                (DateTime)r[CN.DateTrans],
                                "Y",
                                startLine,
                                out Error);
                            if (Error.Length > 0)
                                ShowError(Error);
                        }

                        try
                        {
                            string str = "";
                            string printLine = "";
                            printLine += ((DateTime)drv[CN.DateTrans]).Day.ToString().PadLeft(2, '0');
                            printLine += ((DateTime)drv[CN.DateTrans]).Month.ToString().PadLeft(2, '0');
                            printLine += ((DateTime)drv[CN.DateTrans]).Year.ToString().Substring(2, 2);
                            printLine += "  ";
                            printLine += ((int)drv[CN.TransRefNo]).ToString().PadLeft(6, '0');
                            printLine += "  ";
                            printLine += (string)drv[CN.TransTypeCode];
                            printLine += "  ";

                            decimal val = (decimal)drv[CN.TransValue];
                            if (Math.Abs(val) >= 100000)
                            {
                                val = Math.Round(val, 0);
                                str = val.ToString().Replace("-", "");
                            }
                            else
                                str = val.ToString(DecimalPlaces.Replace("C", "F")).Replace("-", "");
                            printLine += str.PadLeft(8, ' ');
                            //printLine += (((decimal)drv[CN.TransValue]).ToString(DecimalPlaces.Replace("C","F"))).Replace("-","").PadLeft(8, ' ');

                            printLine += " ";

                            val = (decimal)drv[CN.Balance];
                            if (Math.Abs(val) >= 100000)
                            {
                                val = Math.Round(val, 0);
                                str = val.ToString().Replace("-", "");
                            }
                            else
                                str = val.ToString(DecimalPlaces.Replace("C", "F")).Replace("-", "");

                            printLine += str.PadLeft(8, ' ');
                            //printLine += (((decimal)drv[CN.Balance]).ToString(DecimalPlaces.Replace("C","F"))).PadLeft(8, ' ');

                            printLine += "  ";
                            printLine += (startLine.ToString()).PadLeft(2, '0');

                            rp.PrintString(false, printLine);
                            startLine++;

                            if (Config.ReceiptPrinterModel == rpOldModel)
                                Thread.Sleep(1000);
                        }
                        catch (SlipPrinterException ex)
                        {
                            //error printing therefore reset fintrans
                            foreach (DataRowView r in transView)
                            {
                                //update fintrans for these record(s)
                                AccountManager.SetPaymentCardPrinted((string)r[CN.acctno],
                                    (int)r[CN.TransRefNo],
                                    (DateTime)r[CN.DateTrans],
                                    "N",
                                    startLine - 1,
                                    out Error);
                                if (Error.Length > 0)
                                    ShowError(Error);
                            }
                            throw ex;	//pass the exception on
                        }
                    }
                    rp.Release();
                    Thread.Sleep(1000);
                }
            }
            catch (SlipPrinterException ex)
            {
                if (ex.Message != "Cancel")
                    Catch((Exception)ex, Function);
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
            finally
            {
                StopWait();
                if (rp != null)
                {
                    rp.ClosePrinter();
                }
            }
            return startLine;
        }

        void SlipPrinterPrintPaymentReceipt(DataTable transactions,
                                            string customerName,
                                            string accountNo,
                                            decimal finalBalance,
                                            short paymentMethod)
        {
            if (this.SlipPrinterOK(true))
            {
                ReceiptPrinter rp = null;
                try
                {
                    Wait();
                    Function = "SlipPrinterPrintPaymentReceipt";
                    bool updateRow = true;
                    DateTime recent = DateTime.Today;
                    DataTable merged = MergePaymentTransactions(transactions, finalBalance, ref updateRow);

                    rp = new ReceiptPrinter(this);
                    rp.OpenPrinter();

                    // DSR 9/4/03 TransTypeCode now sorted ASCENDING to print REB after PAY
                    DataView mergedView = new DataView(merged, "", CN.DateTrans + " ASC, " + CN.TransTypeCode + " ASC", DataViewRowState.CurrentRows);
                    foreach (DataRowView row in mergedView)
                    //foreach(DataRow row in merged.Rows)
                    {
                        // Currently have to reopen printer to refresh paper out sensor
                        rp.ReOpenPrinter();

                        rp.Feed("\x2", 2);
                        rp.Narrow();
                        rp.Init();
                        rp.LineSpacing = 15;

                        bool openDrawer = Config.CashDrawerID.Length > 0 &&
                            ((paymentMethod != 3 && paymentMethod != 4) || (bool)Country[CountryParameterNames.OpenCashDrawerForCredit]);

                        if (openDrawer)
                            rp.OpenDrawer();

                        decimal amount = (decimal)row[CN.TransValue];
                        amount = amount < 0 ? -amount : amount;
                        decimal balance = (decimal)row[CN.Balance];

                        string printLine = "";
                        printLine += customerName + Environment.NewLine;
                        printLine += Environment.NewLine;
                        printLine += GetResource("T_ACCOUNTNUMBER") + ": " + accountNo + Environment.NewLine;
                        printLine += GetResource("T_TRANSACTIONDATE") + ": ";
                        printLine += ((DateTime)row[CN.DateTrans]).Day.ToString().PadLeft(2, '0');
                        printLine += ((DateTime)row[CN.DateTrans]).Month.ToString().PadLeft(2, '0');
                        printLine += ((DateTime)row[CN.DateTrans]).Year.ToString().Substring(2, 2) + Environment.NewLine;
                        printLine += GetResource("T_TYPE") + ": " + (string)row[CN.TransTypeCode] + Environment.NewLine;
                        printLine += (GetResource("T_AMOUNT")).PadRight(12, ' ');
                        printLine += amount.ToString(DecimalPlaces.Replace("C", "F")).PadLeft(20, ' ') + Environment.NewLine;
                        printLine += GetResource("T_TRANSACTIONNO") + ": " + ((int)row[CN.TransRefNo]).ToString().PadLeft(6, '0') + Environment.NewLine;
                        printLine += (GetResource("T_BALANCE")).PadRight(12, ' ');
                        if (balance < 0)
                            printLine += (-balance).ToString(DecimalPlaces.Replace("C", "F")).PadLeft(20, ' ') + GetResource("T_CR");
                        else
                            printLine += balance.ToString(DecimalPlaces.Replace("C", "F")).PadLeft(20, ' ');
                        printLine += Environment.NewLine;
                        printLine += Environment.NewLine;
                        printLine += GetResource("T_BRANCH") + ": " + Config.BranchCode;
                        rp.PrintString(printLine);

                        rp.Release();
                        Thread.Sleep(1000);
                    }
                }
                catch (SlipPrinterException ex)
                {
                    if (ex.Message != "Cancel")
                        Catch((Exception)ex, Function);
                }
                catch (Exception ex)
                {
                    Catch(ex, Function);
                }
                finally
                {
                    StopWait();
                    if (rp != null) rp.ClosePrinter();
                }
            }
        }

        void SlipPrinterPrintReceipt(string customerID,
                                     string accountNo,
                                     bool collection,
                                     InstantReplacementDetails replacement,
                                     XmlNode lineItems, decimal paid,
                                     decimal change,
                                     CommonForm sender,
                                     int buffNo,
                                     bool taxExempt,
                                     int salesPerson,
                                     bool paidAndTaken,
                                     short paymentMethod,
                                     DataSet payMethodSet)
        {

            if (this.SlipPrinterOK(true))
            {
                ReceiptPrinter rp = null;
                try
                {
                    Function = "SlipPrinterPrintReceipt";
                    Wait();
                    //uat(5.2)-907, 4.3 merge ------
                    //LW - 71722 To avoid printing the receipt if it's got no items --------------
                    int lineItemCount = 0;
                    foreach (XmlNode item in lineItems.ChildNodes)
                    {
                        if (Convert.ToInt32(item.Attributes[Tags.ItemId].Value) != StockItemCache.Get(StockItemKeys.STAX) && item.Attributes[Tags.Quantity].Value != "0")
                            lineItemCount++;

                        XmlNode related = item.SelectSingleNode(Elements.RelatedItem);
                        foreach (XmlNode child in related.ChildNodes)
                        {
                            if (Convert.ToInt32(child.Attributes[Tags.ItemId].Value) != StockItemCache.Get(StockItemKeys.STAX) &&
                                child.Attributes[Tags.Quantity].Value != "0")
                                lineItemCount++;
                        }
                    }

                    if (lineItemCount == 0)
                    {
                        return;
                    }
                    //-----------------------------------------------------------------------------

                    string head1 = "";
                    string head2 = "";
                    string head3 = "";
                    string titleLine = "";
                    //int result = 0;
                    string textLine = "";
                    DataRow branchAddress = null;
                    decimal totalAmount = 0;
                    decimal totalTax = 0;
                    decimal invoiceTotal = 0;
                    string invoice = "";
                    string salesperson = GetResource("T_SALESPERSON");
                    bool isReplacement = replacement != null;
                    //string amounts = "";
                    int serialNo = 0;
                    bool status = true;

                    string barcode = null;

                    if (Config.CountryCode == "C")
                    {
                        head1 = "Mt";
                        head2 = (string)Country[CountryParameterNames.TaxName];
                        head3 = "Sous Total";
                    }
                    else
                    {
                        head1 = GetResource("T_AMT");
                        head2 = (string)Country[CountryParameterNames.TaxName];
                        head3 = GetResource("T_SUBTOTAL");
                    }
                    titleLine = head1.PadRight(11, ' ') + head2.PadRight(11, ' ') + head3.PadRight(11, ' ');

                    rp = new ReceiptPrinter(sender);
                    rp.OpenPrinter();
                    rp.Feed("\x2", 2);
                    rp.Narrow();
                    rp.Init();

                    bool openDrawer = Config.CashDrawerID.Length > 0 &&
                        ((paymentMethod != 3 && paymentMethod != 4) || (bool)Country[CountryParameterNames.OpenCashDrawerForCredit]);

                    if (openDrawer)
                        rp.OpenDrawer();

                    DataSet ds = AccountManager.GetBranchAddress(Convert.ToInt32(Config.BranchCode), 1, out Error);
                    foreach (DataTable dt in ds.Tables)
                        if (dt.TableName == "BranchDetails")
                            foreach (DataRow r in dt.Rows)
                                branchAddress = r;
                    if (branchAddress != null)
                    {
                        if (buffNo == 0)
                            buffNo = Convert.ToInt32((string)branchAddress["BuffNo"]);

                        switch (Config.CountryCode)
                        {
                            case "C":
                                invoice = "FACTURE TVA: ";
                                salesperson = "Vendeur: ";
                                break;
                            case "M":
                                invoice = "INVOICE    : ";
                                break;
                            default:
                                invoice = GetResource("T_TAXINVOICE");
                                break;
                        }
                        if (isReplacement)
                            textLine = GetResource("T_REPLACEMENTINVOICE") + Config.BranchCode + "/" + buffNo.ToString();
                        else
                        {
                            if (collection)
                                textLine = GetResource("T_RETURNINVOICE") + Config.BranchCode + "/" + buffNo.ToString();
                            else
                            {
                                if (paidAndTaken)
                                {
                                    textLine = invoice + Config.BranchCode + "/" + buffNo.ToString();
                                    barcode = Config.BranchCode + "/" + buffNo.ToString();
                                }
                                else
                                    textLine = invoice + accountNo;
                            }
                        }

                        serialNo = Convert.ToInt32(Config.BranchCode + "000000") + Convert.ToInt32(branchAddress["Hissn"]);
                        rp.PrintString(textLine);

                        if (paidAndTaken)
                            rp.PrintString(salesperson + salesPerson.ToString());

                        rp.PrintString(GetResource("T_INVOICENO") + serialNo);
                        if ((bool)Country[CountryParameterNames.LoyaltyCard])
                        {
                            string morerewardsno = CustomerManager.GetMoreRewardsNo(customerID, out Error);
                            if (Error.Length > 0)
                                ShowError(Error);
                            else
                                rp.PrintString(GetResource("T_MOREREWARDSNO") + morerewardsno);
                        }
                        rp.PrintString(DateTime.Today.ToShortDateString());
                        rp.PrintString((string)branchAddress["BranchName"]);
                        rp.PrintString((string)branchAddress["BranchAddr1"]);
                        rp.PrintString((string)branchAddress["BranchAddr2"]);
                        rp.PrintString((string)branchAddress["BranchAddr3"]);

                        if (isReplacement)
                        {
                            textLine = "-" + replacement.Quantity.ToString() +
                                "\t" + replacement.ItemNo + " " +
                                replacement.Description +
                                Environment.NewLine +
                                titleLine +
                                Environment.NewLine +
                                Math.Round(-replacement.OrderValue, 2).ToString().PadRight(11, ' ');
                            if (replacement.TaxAmount == Convert.ToDecimal(0) && Config.CountryCode == "Z")
                                textLine = textLine + "Zero Rated".PadRight(11, ' ');
                            else
                                textLine = textLine + Math.Round(-replacement.TaxAmount, 2).ToString().PadRight(11, ' ');

                            textLine = textLine + Math.Round(-(replacement.OrderValue + replacement.TaxAmount), 2).ToString().PadRight(11, ' ') +
                            Environment.NewLine +
                                Environment.NewLine;
                            rp.PrintString(textLine);

                            totalAmount -= replacement.OrderValue;
                            totalTax -= replacement.TaxAmount;
                            invoiceTotal -= (replacement.OrderValue + replacement.TaxAmount);
                        }

                        //now print the line items
                        int lines = 0;
                        foreach (XmlNode item in lineItems.ChildNodes)
                        {
                            PrintLineItem(rp, item, titleLine, taxExempt, ref totalAmount, ref totalTax, ref invoiceTotal, ref lines);
                            if (lines >= 5)
                            {
                                rp.Release();
                                Thread.Sleep(1000);

                                rp.ReOpenPrinter();

                                while (rp.SlpEmpty)
                                {
                                    DialogResult userRequest = ShowInfo("M_SLIPPAPER", MessageBoxButtons.AbortRetryIgnore);
                                    if (userRequest == DialogResult.Abort)
                                    {
                                        status = false;
                                        break;
                                    }
                                    else if (userRequest == DialogResult.Ignore)
                                    {
                                        status = false;
                                        break;
                                    }
                                }
                                lines = 0;
                                if (!status)
                                    break;
                            }
                        }

                        if (status)
                        {
                            switch (Config.CountryCode)
                            {
                                case "C":
                                    head1 = "Mt Total";
                                    head3 = "TTC";
                                    break;
                                default:
                                    head1 = GetResource("T_TOTALAMT");
                                    head3 = GetResource("T_INVOICETOTAL");
                                    break;
                            }

                            if (collection)
                                head3 = GetResource("T_REFTOTAL");

                            titleLine = head1.PadRight(11, ' ') + head2.PadRight(11, ' ') + head3.PadRight(11, ' ');
                            rp.PrintString(titleLine);
                            string totals = totalAmount.ToString(Format).PadRight(11, ' ');
                            if (totalTax == Convert.ToDecimal(0) && Config.CountryCode == "Z")
                            {
                                totals += "Zero Rated".PadRight(11, ' ');
                            }
                            else
                            {
                                totals += totalTax.ToString(Format).PadRight(11, ' ');
                            }
                            totals += invoiceTotal.ToString(Format).PadRight(11, ' ');
                            rp.PrintString(totals);

                            if (paidAndTaken)
                            {
                                rp.PrintString("                      -------------");
                                if (collection)
                                    rp.PrintString("      " + GetResource("T_REFTOTAL") + ":" + paid.ToString(Format).PadLeft(17, ' '));
                                else
                                    rp.PrintString("      " + GetResource("T_AMOUNTTENDERED") + paid.ToString(Format).PadLeft(11, ' '));
                                if (!collection)
                                {
                                    rp.PrintString("                      -------------");
                                    rp.PrintString("      " + GetResource("T_CHANGEGIVEN") + "  " + change.ToString(Format).PadLeft(11, ' '));
                                }
                                rp.PrintString("                      -------------");

                                textLine = "";
                                foreach (DataTable dt in payMethodSet.Tables)
                                {
                                    foreach (DataRow r in dt.Rows)
                                    {
                                        textLine += (string)r[CN.CodeDescription] + "\t" +
                                            Convert.ToDecimal(r[CN.Value]).ToString(Format) +
                                            Environment.NewLine +
                                            "            ";
                                    }
                                }

                                rp.PrintString(Environment.NewLine + "Pay Method: " + textLine);

                            }
                            rp.Release();
                            Thread.Sleep(1000);
                        }
                    }
                }
                catch (SlipPrinterException ex)
                {
                    if (ex.Message != "Cancel")
                        Catch((Exception)ex, Function);
                }
                catch (Exception ex)
                {
                    Catch(ex, Function);
                }
                finally
                {
                    StopWait();
                    if (rp != null)
                        rp.ClosePrinter();
                }
            }
        }

        #endregion

        #region Thermal Printer

        void ThermalPrintPaymentReceipt(DataTable transactions,
                                                decimal finalBalance,
                                                string customerID,
                                                string customerName,
                                                decimal creditLimit,
                                                decimal availableSpend,
                                                string accountNo,
                                                short paymentMethod,
                                                bool combinedRF,
                                                string accountType,                                 //IP - 21/05/12 - #10146
                                                bool printMiniStat = false                          //IP - 18/05/12 - #9445 - CR1239
            )
        {
            PaymentStatement paymentStatement = new PaymentStatement();
            ApplyCountryParametersForPayment(ref paymentStatement);
            paymentStatement.DocumentName = paymentStatement.Title.Value + " : " + customerName;

            DataSet ds = AccountManager.GetPrintingInfoForCL((string)transactions.Rows[0][CN.AcctNo]);
            if (ds != null)
            {
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        paymentStatement.BusinessTitle.Value = Convert.ToString(ds.Tables[0].Rows[0]["BusinessTitle"]);
                        paymentStatement.BusinessTaxNo.Value = Convert.ToString(ds.Tables[0].Rows[0]["BusinessTaxNo"]);
                    }
                }
            }

            try
            {
                Wait();
                Function = "ThermalPrintPaymentCardTransactions";

                decimal balance = finalBalance;
                bool updateRow = true;
                _thermalprinting = true; // thermal printing needs agreement total which is in the transactions DataTable
                DataTable merged = MergePaymentTransactions(transactions, balance, ref updateRow);

                DataView transView = new DataView(transactions, "", CN.DateTrans + " DESC," + CN.TransTypeCode + " DESC", DataViewRowState.CurrentRows);
                //transView.RowFilter = CN.TransPrinted + " = 'N'";

                //if there were no transaction for the current date then we need to add a dummy 
                //row to the merged table
                if (updateRow)
                {
                    DataRow dummy = merged.NewRow();
                    dummy[CN.acctno] = "dummy";		//so we can tell we don't to update db for this row
                    dummy[CN.DateTrans] = DateTime.Today;
                    dummy[CN.TransRefNo] = 0;
                    dummy[CN.TransTypeCode] = TransType.Update;
                    dummy[CN.TransValue] = 0;
                    dummy[CN.Balance] = finalBalance;
                    merged.Rows.Add(dummy);
                }

                if (combinedRF)
                    merged = transactions;

                bool openDrawer = Config.CashDrawerID.Length > 0 &&
                   ((paymentMethod != 3 && paymentMethod != 4) || Country.GetCountryParameterValue<bool>(CountryParameterNames.OpenCashDrawerForCredit));

                if (openDrawer)
                    Printer.OpenDrawer();

                paymentStatement.Date.Value = DateTime.Now;
                paymentStatement.CustomerName.Value = customerName;
                DataSet customerAddress = CustomerManager.GetCustomerAddresses(customerID, out Error);
                foreach (DataTable dt in customerAddress.Tables)
                    foreach (DataRow row in dt.Rows)
                    {
                        if (((string)row[CN.AddressType]).Trim() == "H")
                        {
                            paymentStatement.CustomerAddress1.Value = row[CN.Address1].ToString();
                            paymentStatement.CustomerAddress2.Value = row[CN.Address2].ToString();
                            paymentStatement.CustomerAddress3.Value = row[CN.Address3].ToString();
                            paymentStatement.CustomerAddress4.Value = row[CN.PostCode].ToString();
                        }
                    }

                if (accountType.Trim() == AT.ReadyFinance)                             //IP - 21/05/12 - #10146
                {
                    paymentStatement.AvailableSpend.Value = availableSpend.ToPositive();
                    paymentStatement.CreditLimit.Value = creditLimit.ToPositive();
                }
                else
                {
                    paymentStatement.AvailableSpend.ShouldBePrinted = false;
                    paymentStatement.CreditLimit.ShouldBePrinted = false;
                }

                List<BBSL.Libraries.Printing.Transaction> payments = new List<BBSL.Libraries.Printing.Transaction>();

                decimal balanceTotal = 0,
                        paymentTotal = 0;

                /* Print the transactions in the merged table
                 * use a view to reverse the order
                 * Before a transaction is printed, update the fintrans table to "Y"
                 * if there is a problem printing the transaction, then the fintrans
                 * must be updated to reflect that the transaction was not successfully 
                 * printed  - there is still a small chance that the second compensating update
                 * might fail but this is acceptable
                 */
                // DSR 9/4/03 TransTypeCode now sorted ASCENDING to print REB after PAY
                DataView mergedView = new DataView(merged, "",
                                                    String.Format("{0} ASC, {1} ASC, {2} ASC", CN.AcctNo, CN.DateTrans, CN.TransTypeCode),
                                                    DataViewRowState.CurrentRows);
                var lastAcctNo = "";
                var index = 0;
                List<string> accounts = new List<string>();
                bool PostwriteoffTransaction = false;
                foreach (DataRowView drv in mergedView)
                {
                    index++;

                    BBSL.Libraries.Printing.Transaction payment = new BBSL.Libraries.Printing.Transaction();

                    payment.AccountNo = drv[CN.AcctNo].ToString();
                    payment.Amount = drv[CN.TransValue].ToDecimal();
                    payment.AgreementTotal = drv[CN.AgreementTotal].ToDecimal().ToPositive();
                    payment.PaymentType = drv[CN.TransTypeCode].ToString();
                    paymentStatement.ReferenceNo.Value = drv[CN.TransRefNo].ToString();

                    //if (combinedRF)
                    //{
                    BBSL.Libraries.Printing.BBSCustomer customer =
                        AccountManager.GetAccountStatementLastTransactionsLocal(payment.AccountNo, 1);
                    if (customer.Accounts.Count > 0 && customer.Accounts[0].Transactions.Count > 0)
                        payment.RemainingBalance = customer.Accounts[0].Transactions[0].RemainingBalance;

                    if (customer.Accounts[0].AccountNo.Substring(3, 1) == "5" && payment.RemainingBalance <= 0) // its a write off account so lets use the Bad Debt Wo true. 
                    {
                        payment.RemainingBalance = finalBalance;
                        PostwriteoffTransaction = true; // don't print statement for written off account
                    }
                    //}
                    //else
                    //    payment.RemainingBalance = drv[CN.Balance].ToDecimal().ToPositive();

                    payments.Add(payment);

                    if (!accounts.Contains(payment.AccountNo))
                    {
                        accounts.Add(payment.AccountNo);
                    }

                    if (payment.PaymentType.ToUpper() == "PAY")  //Not sure is this the correct way of doing
                        paymentTotal += payment.Amount.ToPositive();

                    if (lastAcctNo != payment.AccountNo)
                    {
                        balanceTotal += payment.RemainingBalance.ToPositive();

                        UpdateRemainingBalance(payments, lastAcctNo);

                        lastAcctNo = payment.AccountNo;
                    }

                    if (index == mergedView.Count) //If last record
                    {
                        UpdateRemainingBalance(payments, lastAcctNo);
                    }
                }

                paymentStatement.Payments.Value = payments;
                paymentStatement.PaymentTotal.Value = paymentTotal;
                paymentStatement.BalanceTotal.Value = balanceTotal;

                paymentStatement.Print();

                //if (PrintAutomaticMiniStatement && !PostwriteoffTransaction)
                if (printMiniStat && !PostwriteoffTransaction)                               //IP - 18/05/12 - #9445 - CR1239
                {
                    int noOfTransactions = Country.GetCountryParameterValue<int>(CountryParameterNames.Printing.TransactionsForAutomaticMiniStatement);
                    foreach (string accountno in accounts)
                    {
                        Statements statements = new Statements();
                        statements.PrintStatementForAccount(accountno, noOfTransactions, AccountManager, Country, false);
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
            }
        }

        void ThermalPrintPaymentReceiptOldStyle(DataTable transactions,
                                        string customerName,
                                        string accountNo,
                                        decimal finalBalance,
                                        short paymentMethod)
        {
            PaymentReceipt paymentReceipt = new PaymentReceipt();
            paymentReceipt.DocumentName = paymentReceipt.Title.Value + " : " + customerName;

            DataSet ds = AccountManager.GetPrintingInfoForCL((string)transactions.Rows[0][CN.AcctNo]);
            if (ds != null)
            {
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        paymentReceipt.BusinessTitle.Value = Convert.ToString(ds.Tables[0].Rows[0]["BusinessTitle"]);
                        paymentReceipt.BusinessTaxNo.Value = Convert.ToString(ds.Tables[0].Rows[0]["BusinessTaxNo"]);
                    }
                }
            }

            try
            {
                Wait();
                Function = "ThermalPrintPaymentReceipt";
                bool updateRow = true;
                DateTime recent = DateTime.Today;
                DataTable merged = MergePaymentTransactions(transactions, finalBalance, ref updateRow);
                bool openDrawer = Config.CashDrawerID.Length > 0 &&
                    ((paymentMethod != 3 && paymentMethod != 4) || Country.GetCountryParameterValue<bool>(CountryParameterNames.OpenCashDrawerForCredit));

                if (openDrawer)
                    Printer.OpenDrawer();

                // DSR 9/4/03 TransTypeCode now sorted ASCENDING to print REB after PAY
                DataView mergedView = new DataView(merged, "", CN.DateTrans + " ASC, " + CN.TransTypeCode + " ASC", DataViewRowState.CurrentRows);
                foreach (DataRowView row in mergedView)
                {
                    decimal amount = (decimal)row[CN.TransValue];
                    amount = amount < 0 ? -amount : amount;
                    decimal balance = (decimal)row[CN.Balance];

                    if (balance < 0)
                        paymentReceipt.AccountBalance.Value = -balance;
                    else
                        paymentReceipt.AccountBalance.Value = balance;

                    paymentReceipt.AccountNo.Value = accountNo;
                    paymentReceipt.Amount.Value = amount;
                    paymentReceipt.CustomerName.Value = customerName;
                    paymentReceipt.CustomerTitle.Value = "";
                    paymentReceipt.BranchCode.Value = Config.BranchCode;
                    paymentReceipt.TransactionDate.Value = (DateTime)row[CN.DateTrans];
                    paymentReceipt.TransactionNo.Value = ((int)row[CN.TransRefNo]).ToString();
                    paymentReceipt.Type.Value = (string)row[CN.TransTypeCode];
                    paymentReceipt.SalesPerson.Value = Credential.UserId.ToString();
                    paymentReceipt.Title.Value = Country.GetCountryParameterValue<string>(CountryParameterNames.Printing.PaymentReceiptTitle);

                    paymentReceipt.Print();
                }
            }
            catch (SlipPrinterException ex)
            {
                if (ex.Message != "Cancel")
                    Catch((Exception)ex, Function);
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
            finally
            {
                StopWait();
                paymentReceipt.Dispose();
            }

        }
        //BCX : This is used for LUX tax for curacao 
        void ThermalPrintReceipt(string customerID,
                                string accountNo,
                                bool collection,
                                InstantReplacementDetails replacement,
                                XmlNode lineItems,
                                decimal paid,
                                decimal change,
                                bool reprint,
                                int buffNo,
                                bool taxExempt,
                                int salesPerson,
                                bool paidAndTaken,
                                short paymentMethod,
                                DataSet payMethodSet,
                                bool isBulk = false,
                                BBSL.Libraries.Printing.PrintDocuments.Receipt receipt = null,
                                string salesPersonName = "",                              //IP - 17/05/12 - #9447 - CR1239
                                string cashierName = "",                                  //IP - 17/05/12 - #9447 - CR1239
                                int cashierID = 0)                                        //IP - 17/05/12 - #9447 - CR1239
        {

            if (isBulk && receipt == null)                                                          //IP - 11/05/12 - #9609 - CR8520
                throw new ArgumentException("receipt cannot be null for bulk printing");

            if (!isBulk)                                                                            //IP - 11/05/12 - #9609 - CR8520
                receipt = new BBSL.Libraries.Printing.PrintDocuments.Receipt();

            bool titlesHaveChangedFromDefault = false;
            //BBSL.Libraries.Printing.PrintDocuments.Receipt receipt = new BBSL.Libraries.Printing.PrintDocuments.Receipt();    //IP - 11/05/12 - #9609 - CR8520
            ApplyCountryParametersForCashNGo(ref receipt);

            receipt.DocumentName = receipt.Title.Value;

            try
            {
                Function = "ThermalPrintReceipt";
                Wait();

                //IP - 11/05/12 - #9611 - CR8520
                if (paidAndTaken && reprint == false)
                {

                    if (collection)            //IP - 16/05/12 - #10129 - Audit Cash and Go Return print
                    {
                        AccountManager.PrintAuditCashAndGo(accountNo, buffNo, salesPerson, AuditType.CashAndGoReturnPrint, taxExempt, change, paymentMethod); //IP - 22/05/12 - #10156 - Added paymentMethod
                    }
                    else
                    {

                        AccountManager.PrintAuditCashAndGo(accountNo, buffNo, salesPerson, AuditType.CashAndGoPrint, taxExempt, change, paymentMethod);     //IP - 22/05/12 - #10156 
                    }
                }


                //IP - 09/05/12 - #9608 - CR8520
                if (reprint)
                {
                    //IP - 11/05/12 - #9611
                    if (!isBulk)
                    {
                        AccountManager.PrintAuditCashAndGo(accountNo, buffNo, salesPerson, AuditType.CashAndGoSingleReprint, taxExempt, change, paymentMethod); //IP - 22/05/12 - #10156 - Added paymentMethod
                    }
                    else
                    {
                        AccountManager.PrintAuditCashAndGo(accountNo, buffNo, salesPerson, AuditType.CashAndGoBulkReprint, taxExempt, change, paymentMethod);    //IP - 22/05/12 - #10156 
                    }

                    receipt.Reprint = true;
                    receipt.ReprintDate.Value = DateTime.Now;
                    receipt.Title.Value = "REPRINT" + " " + receipt.Title.Value;

                    receipt.CashierID.Value = Convert.ToString(cashierID) + " " + cashierName;                                      //IP - 17/05/12 - #9447 - CR1239
                }
                else
                {
                    receipt.CashierID.Value = Credential.UserId.ToString() + " " + Credential.Name;                          //IP - 17/05/12 - #9447 - CR1239
                }

                receipt.SalesPerson.Value = salesPerson.ToString() + " " + salesPersonName;                     //IP - 17/05/12 - #9447 - CR1239
                receipt.CountryCode.Value = Convert.ToString(Country[CountryParameterNames.CountryCode]);

                //uat(5.2)-907, 4.3 merge ------
                //LW - 71722 To avoid printing the receipt if it's got no items --------------
                if (lineItems == null)
                    return;
                int lineItemCount = 0;
                foreach (XmlNode item in lineItems.ChildNodes)
                {
                    if (item.Attributes[Tags.Code].Value != "STAX" && item.Attributes[Tags.Quantity].Value != "0")
                        lineItemCount++;

                    XmlNode related = item.SelectSingleNode(Elements.RelatedItem);
                    foreach (XmlNode child in related.ChildNodes)
                    {
                        if (child.Attributes[Tags.Code].Value != "STAX" && child.Attributes[Tags.Quantity].Value != "0")
                            lineItemCount++;
                    }
                }

                if (lineItemCount == 0)
                {
                    return;
                }

                string titleLine = "";
                string textLine = "";
                decimal totalAmount = 0;
                decimal totalTax = 0;
                decimal AdditioaltotalTax = 0;
                decimal invoiceTotal = 0;
                decimal OtherCurrecnyinvoiceTotal = 0;
                string OtherCurrencyName = "";
                bool isOtherCorruncyActive = false;
                bool isReplacement = replacement != null;

                string barcode = null;

                string err, branchHissn = "";

                {
                    DataRow branchAddress = null;
                    DataSet ds = AccountManager.GetBranchAddress(Convert.ToInt32(Config.BranchCode), 1, out err);
                    foreach (DataTable dt in ds.Tables)
                        if (dt.TableName == "BranchDetails")
                            foreach (DataRow r in dt.Rows)
                                branchAddress = r;

                    if (branchAddress != null)
                    {
                        if (buffNo == 0)
                            buffNo = branchAddress["BuffNo"].ToInt32();

                        branchHissn = branchAddress["Hissn"].ToString();
                    }
                }

                //IP - 09/05/12 - #9608 - CR8520
                if (reprint)
                {
                    receipt.Date.Value = Convert.ToDateTime(payMethodSet.Tables[TN.PayMethodList].Compute("max(DateTrans)", string.Empty));
                }
                else
                {
                    receipt.Date.Value = DateTime.Now;
                }

                if (!isBulk && !reprint)                //IP - 11/05/12 - #9609 - CR8520
                {
                    bool openDrawer = Config.CashDrawerID.Length > 0 &&
                        ((paymentMethod != 3 && paymentMethod != 4) || Country.GetCountryParameterValue<bool>(CountryParameterNames.OpenCashDrawerForCredit));

                    if (openDrawer)
                        Printer.OpenDrawer();
                }

                if (buffNo == 0)
                {
                    DataRow branchAddress = null;
                    DataSet ds = AccountManager.GetBranchAddress(Convert.ToInt32(Config.BranchCode), 1, out Error);
                    foreach (DataTable dt in ds.Tables)
                        if (dt.TableName == "BranchDetails")
                            foreach (DataRow r in dt.Rows)
                                branchAddress = r;
                    if (branchAddress != null)
                        buffNo = branchAddress["BuffNo"].ToInt32();
                }

                if (isReplacement)
                {
                    BBSL.Libraries.Printing.PrintDocuments.Receipt.TitleTaxInvoice = GetResource("T_REPLACEMENTINVOICE");
                    titlesHaveChangedFromDefault = true;
                    receipt.TaxInvoice.Value = Config.BranchCode + "/" + buffNo.ToString();
                }
                else
                {
                    if (collection)
                    {
                        BBSL.Libraries.Printing.PrintDocuments.Receipt.TitleTaxInvoice = GetResource("T_RETURNINVOICE");
                        titlesHaveChangedFromDefault = true;
                        receipt.TaxInvoice.Value = Config.BranchCode + "/" + buffNo.ToString();
                    }
                    else
                    {
                        if (paidAndTaken)
                        {
                            receipt.TaxInvoice.Value = Config.BranchCode + "/" + buffNo.ToString();
                            barcode = Config.BranchCode + "/" + buffNo.ToString();
                        }
                        else
                            receipt.TaxInvoice.Value = accountNo;
                    }
                }

                if (!paidAndTaken)
                    receipt.AccountNo.Value = accountNo;

                //receipt.InvoiceNo.Value = (Convert.ToInt32(Config.BranchCode + "000000") + Convert.ToInt32(branchHissn)).ToString();      //IP - 09/05/12 - #9610 - CR8520 - not required to be printed

                //receipt.DocumentName = receipt.Title.Value + " : " + receipt.InvoiceNo.Value;
                receipt.DocumentName = receipt.Title.Value;                                                                                 //IP - 09/05/12 - #9610 - CR8520 

                if (Country.GetCountryParameterValue<bool>(CountryParameterNames.LoyaltyCard))
                {
                    string morerewardsno = CustomerManager.GetMoreRewardsNo(customerID, out Error);
                    if (Error.Length > 0)
                        ShowError(Error);
                    else
                        receipt.MoreRewardsNo.Value = morerewardsno;
                }

                if (isReplacement)
                {
                    textLine = "-" + replacement.Quantity.ToString() +
                        "\t" + replacement.ItemNo + " " +
                        replacement.Description +
                        Environment.NewLine +
                        titleLine +
                        Environment.NewLine +
                        Math.Round(-replacement.OrderValue, 2).ToString().PadRight(11, ' ') +
                        Math.Round(-replacement.TaxAmount, 2).ToString().PadRight(11, ' ') +
                        Math.Round(-(replacement.OrderValue + replacement.TaxAmount), 2).ToString().PadRight(11, ' ') +
                        Environment.NewLine +
                        Environment.NewLine;

                    totalAmount -= replacement.OrderValue;
                    totalTax -= replacement.TaxAmount;
                    invoiceTotal -= (replacement.OrderValue + replacement.TaxAmount);

                }

                List<Product> products = new List<Product>();
                products = GetProducts(lineItems.ChildNodes, taxExempt, ref totalAmount, ref totalTax, ref AdditioaltotalTax, ref invoiceTotal, ref products, ref OtherCurrecnyinvoiceTotal, ref OtherCurrencyName, ref isOtherCorruncyActive);
                receipt.Products.Value = products;

                if (collection)
                {
                    BBSL.Libraries.Printing.PrintDocuments.Receipt.TitleInvoiceTotal = GetResource("T_REFTOTAL");
                    titlesHaveChangedFromDefault = true;
                }

                receipt.SubTotal.Value = totalAmount;
                receipt.TotalTax.Value = totalTax;
                receipt.Total.Value = invoiceTotal;
                receipt.AdditionalTotalTax.Value = AdditioaltotalTax;
                //CR20181014 
                receipt.OtherCurrencyTotal.Value = OtherCurrecnyinvoiceTotal;
                receipt.OtherCurrencyName.Value = OtherCurrencyName;
                receipt.isOtherCorruncyActive.Value = isOtherCorruncyActive;

                if (collection)
                {
                    BBSL.Libraries.Printing.PrintDocuments.Receipt.TitleAmountTendered = GetResource("T_REFTOTAL");
                    titlesHaveChangedFromDefault = true;
                }

                if (payMethodSet != null)
                {
                    if (isBulk)                             //IP - 21/05/12 - #10148 
                    {
                        receipt.AmountTendered.Value = 0;
                        receipt.ChangeGiven.Value = 0;
                    }

                    foreach (DataTable dt in payMethodSet.Tables)
                    {
                        if (dt.TableName != TN.Accounts)                //IP - 10/05/12 - #9609 - CR8520
                        {
                            foreach (DataRow r in dt.Rows)
                            {
                                decimal value = r[CN.Value].ToDecimal();
                                receipt.AmountTendered.Value = receipt.AmountTendered.Value + value ?? value;
                                receipt.ChangeGiven.Value = change;

                            }
                        }
                    }
                }

                receipt.PayMethod.Value = (Enum.Parse(typeof(PayMethods), paymentMethod.ToString())).ToString();
                receipt.payMethodSet = payMethodSet;
                //receipt.SalesPerson.Value = salesPerson.ToString();
                //receipt.CashierID.Value = Credential.User;
                //receipt.CashierName.Value = Credential.Name;

                int noOfCopies = Country.GetCountryParameterValue<int>(CountryParameterNames.Printing.CashNGoNoOfCopies);

                if (!isBulk)                //IP - 11/05/12 - #9609 - CR8520
                {
                    if (noOfCopies > 1)
                    {
                        receipt.AdditionalText.Value = Country.GetCountryParameterValue<string>(CountryParameterNames.Printing.CashNGoOrigianlText);
                        receipt.Print();
                        noOfCopies--;

                        do
                        {
                            receipt.AdditionalText.Value = Country.GetCountryParameterValue<string>(CountryParameterNames.Printing.CashNGoCopyText);
                            receipt.Print();

                        } while (noOfCopies-- > 1);
                    }
                    else receipt.Print();
                }
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
            finally
            {
                StopWait();
                if (titlesHaveChangedFromDefault)
                    InitialisePrinting();

                if (!isBulk)
                    receipt.Dispose();
            }
        }

        //IP - 13/10/11 - #3921 - CR1232
        void ThermalPrintCashLoanReceipt(string customerName, string acctNo, DateTime transDate, string type, decimal amtDisbursed, int transNo, decimal outstBal, DateTime firstInstalDate, string disbursementType)
        {
            DataSet ds = AccountManager.GetPrintingInfoForCL(acctNo);
            if (ds != null)
            {
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        PrintContent.BusinessTitle.Value = Convert.ToString(ds.Tables[0].Rows[0]["BusinessTitle"]);
                        PrintContent.BusinessTaxNo.Value = Convert.ToString(ds.Tables[0].Rows[0]["BusinessTaxNo"]);
                    }
                }
            }

            CashLoanReceipt clr = new CashLoanReceipt();
            clr.Title = new BBSL.Libraries.Printing.PrintDataWrapper<string>("Cash Loan Disbursement", true);

            BBSL.Libraries.Printing.PrintDocuments.CashLoanReceipt.TitleCustomerName = "Customer Name:";
            BBSL.Libraries.Printing.PrintDocuments.CashLoanReceipt.TitleAccountNo = "Account Number:";
            BBSL.Libraries.Printing.PrintDocuments.CashLoanReceipt.TitleTransactionDate = "Transaction Date:";
            BBSL.Libraries.Printing.PrintDocuments.CashLoanReceipt.TitleType = "Type:";
            BBSL.Libraries.Printing.PrintDocuments.CashLoanReceipt.TitleAmountDisbursed = "Amount Disbursed:";
            BBSL.Libraries.Printing.PrintDocuments.CashLoanReceipt.TitleTransactionNo = "Transaction No:";
            BBSL.Libraries.Printing.PrintDocuments.CashLoanReceipt.TitleAccountBalance = "Balance:";
            BBSL.Libraries.Printing.PrintDocuments.CashLoanReceipt.TitleFirstPaymentDate = "First Payment Date";
            BBSL.Libraries.Printing.PrintDocuments.CashLoanReceipt.TitleDisbursementType = "Disbursement Type";

            clr.CustomerName = new BBSL.Libraries.Printing.PrintDataWrapper<string>(customerName, true);
            clr.AccountNo = new BBSL.Libraries.Printing.PrintDataWrapper<string>(acctNo, true);
            clr.TransactionDate = new BBSL.Libraries.Printing.PrintDataWrapper<DateTime>(transDate, true);
            clr.Type = new BBSL.Libraries.Printing.PrintDataWrapper<string>(type, true);
            clr.AmountDisbursed = new BBSL.Libraries.Printing.PrintDataWrapper<decimal?>(amtDisbursed, true);
            clr.TransactionNo = new BBSL.Libraries.Printing.PrintDataWrapper<string>(Convert.ToString(transNo), true);
            clr.AccountBalance = new BBSL.Libraries.Printing.PrintDataWrapper<decimal?>(outstBal, true); ;
            clr.FirstPaymentDate = new BBSL.Libraries.Printing.PrintDataWrapper<DateTime>(firstInstalDate, true);
            clr.DisbursementType = new BBSL.Libraries.Printing.PrintDataWrapper<string>(disbursementType, true);

            clr.Print();
        }

        //IP - 22/12/11 - #8811 - CR1234    
        void ThermalPrintDisbursementReceipt(DataTable cashierDisbursements,
                                            string cashier)
        {
            DisbursementReceipt disbursementReceipt = new DisbursementReceipt();

            try
            {
                Wait();
                Function = "ThermalPrintDisbursements";

                BBSL.Libraries.Printing.PrintDocuments.DisbursementReceipt.TitleCashier = "Cashier:";
                //BBSL.Libraries.Printing.PrintDocuments.DisbursementReceipt.TitleDisbursementDate = "Date of Disbursement:";
                BBSL.Libraries.Printing.PrintDocuments.DisbursementReceipt.TitleDisbursementDate = "Date Disbursed:";                   //IP - 18/05/12 - #10141

                BBSL.Libraries.Printing.PrintDocuments.DisbursementReceipt.TitleDisbursement = "Disbursement Type";
                BBSL.Libraries.Printing.PrintDocuments.DisbursementReceipt.TitlePayMethod = "Pay Method";
                BBSL.Libraries.Printing.PrintDocuments.DisbursementReceipt.TitleValue = "Value";
                BBSL.Libraries.Printing.PrintDocuments.DisbursementReceipt.TitleReference = "Reference";
                BBSL.Libraries.Printing.PrintDocuments.DisbursementReceipt.TitleSignatureText = "Employee Signature";

                disbursementReceipt.Cashier = new BBSL.Libraries.Printing.PrintDataWrapper<string>(cashier, true);
                disbursementReceipt.DisbursementDate = new BBSL.Libraries.Printing.PrintDataWrapper<DateTime>(DateTime.Now, true);


                List<BBSL.Libraries.Printing.Disbursement> disbursements = new List<BBSL.Libraries.Printing.Disbursement>();

                foreach (DataRow dr in cashierDisbursements.Rows)
                {

                    BBSL.Libraries.Printing.Disbursement disbursement = new BBSL.Libraries.Printing.Disbursement();

                    disbursement.DisbursementType = Convert.ToString(dr[CN.CodeDescription]);
                    disbursement.PayMethod = Convert.ToString(dr[CN.PayMethodDescription]);
                    disbursement.Value = Convert.ToDecimal(dr[CN.Value]);
                    disbursement.Reference = Convert.ToString(dr[CN.Reference]);

                    disbursements.Add(disbursement);
                }

                disbursementReceipt.Disbursements.Value = disbursements;

                disbursementReceipt.Print();

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

        void ThermalPrintTaxInvoice(string accountNo,
                                    int agreementNo,
                                    string accountType,
                                    string customerID,
                                    bool paidAndTaken,
                                    bool collection,
                                    XmlNode lineItems,
                                    int buffNo,
                                    bool creditNote,
                                    bool multiple,
                                    bool taxExempt   //#18112
                                    , int versionNo = 0, bool ReprintInvoice = false)
        {
            BBSL.Libraries.Printing.PrintDocuments.Receipt receipt = new BBSL.Libraries.Printing.PrintDocuments.Receipt();
            ApplyCountryParametersForTaxInvoice(ref receipt);
            if (ReprintInvoice == true)
            {
                receipt.Title.Value += " - Reprint Copy";
            }

            try
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(delegate
                {
                    try
                    {
                        // Audit the Tax Invoice print if it is a re-print
                        AccountManager.AuditReprint(accountNo, agreementNo, DocumentType.TaxInvoice);
                    }
                    catch (Exception) { }
                }));

                string err, branchHissn = "";

                {
                    DataRow branchAddress = null;
                    DataSet ds = AccountManager.GetBranchAddress(Convert.ToInt32(Config.BranchCode), 1, out err);
                    foreach (DataTable dt in ds.Tables)
                        if (dt.TableName == "BranchDetails")
                            foreach (DataRow r in dt.Rows)
                                branchAddress = r;

                    if (branchAddress != null)
                    {
                        if (buffNo == 0)
                            buffNo = branchAddress["BuffNo"].ToInt32();

                        branchHissn = branchAddress["Hissn"].ToString();
                    }
                }

                //BOC CR 2018-13
                DataSet dsInvoiceDetails = new DataSet();
                dsInvoiceDetails = AccountManager.GetInvoiceDetails(accountNo, Convert.ToString(agreementNo), "");
                DataTable dtInvoiceDetails = dsInvoiceDetails.Tables["InvoiceDetails"];
                DataTable dtInvoicePaymentDetails = dsInvoiceDetails.Tables["InvoicePaymentDetails"];

                DataRow drInvDet = dtInvoiceDetails.Rows[0];
                string agreemtInvNum = Convert.ToString(drInvDet["agreementinvoicenumber"]);
                string salesman = Convert.ToString(drInvDet["soldbyID"]) + " - " + Convert.ToString(drInvDet["soldByName"]);
                string cashier = Convert.ToString(drInvDet["createdByID"]) + " - " + Convert.ToString(drInvDet["createdByName"]);
                string countryName = Convert.ToString(Country[CountryParameterNames.CountryName]);
                char taxInvoicePrinted = Convert.ToChar(drInvDet["TaxInvoicePrinted"]);
                string regNo = Convert.ToString(Country["BusinessRegNo"]);
                DateTime dtInvDate = DateTime.Now;
                string version_no = string.Empty;
                if (ReprintInvoice == true)
                {
                    version_no = Convert.ToString(versionNo);
                }
                else
                {
                    version_no = Convert.ToString(drInvDet["inv_version_no"]);
                }
                //EOC CR 2018-13

                DataSet accountDetailsSet = AccountManager.GetAccountDetails(accountNo, out err);
                foreach (DataTable accountDetailsTable in accountDetailsSet.Tables)
                    if (accountDetailsTable.TableName == TN.AccountDetails && accountDetailsTable.Rows.Count > 0)
                    {
                        if (accountType.Trim() != AT.ReadyFinance)                              //IP - 18/05/12 - #9446 - CR1239 - only display for RF accounts
                        {
                            receipt.AccountBalance.ShouldBePrinted = false;
                            receipt.AvailableSpend.ShouldBePrinted = false;
                        }
                        else
                        {
                            receipt.AccountBalance.Value = accountDetailsTable.Rows[0][CN.OutstandingBalance2].ToDecimal();
                            receipt.AvailableSpend.Value = accountDetailsTable.Rows[0][CN.AvailableSpend].ToDecimal();
                        }
                        dtInvDate = Convert.ToDateTime(accountDetailsTable.Rows[0]["Account Opened"]);
                    }

                DataSet custdetails = new DataSet();
                if (ReprintInvoice == true && agreementNo != 1)
                {
                    //retrieve the basic customer details for Web
                    custdetails = CustomerManager.GetBasicCustomerDetails_Web(Convert.ToString(agreementNo), out err);
                    dtInvDate = Convert.ToDateTime(drInvDet["createdOn"]);
                }
                else
                {
                    custdetails = CustomerManager.GetBasicCustomerDetails(customerID, accountNo, "H", out err);
                }

                BBSL.Libraries.Printing.BBSCustomer customer = new BBSL.Libraries.Printing.BBSCustomer();

                Dictionary<string, List<string>> addresses = new Dictionary<string, List<string>>();

                if (custdetails != null)
                {
                    foreach (DataTable dt in custdetails.Tables)
                    {
                        switch (dt.TableName)
                        {
                            case "BasicDetails":
                                {
                                    foreach (DataRow row in dt.Rows)
                                        customer.Name = row[CN.FirstName].ToString() + " " + row[CN.LastName].ToString();
                                }
                                break;
                            case TN.CustomerAddresses:
                                {
                                    foreach (DataRow row in dt.Rows)
                                    {
                                        List<string> address = new List<string>(3);
                                        address.Add(row[Address.Address1].ToString());
                                        address.Add(row[Address.Address2].ToString());
                                        address.Add(row[Address.Address3].ToString());
                                        address.Add(row[Address.PostCode].ToString());

                                        addresses.Add(row[Address.AddressType].ToString().ToUpper().Trim(), address);
                                    }
                                }
                                break;
                            case TN.Customer:
                                {
                                    if (dt.Rows.Count > 0)
                                    {
                                        DataRow row = dt.Rows[0];
                                        customer.Name = row[CN.FirstName].ToString() + " " + row[CN.LastName].ToString();
                                        List<string> address = new List<string>(3);
                                        address.Add(Convert.ToString(row["AddressLine1"]));
                                        address.Add(Convert.ToString(row["AddressLine2"].ToString()));
                                        address.Add(Convert.ToString(row["AddressLine3"].ToString()));
                                        address.Add(Convert.ToString(row["PostCode"].ToString()));

                                        addresses.Add("H", address);
                                    }
                                }
                                break;
                            default:
                                break;
                        }
                    }
                }

                System.Action<List<String>> assignCustomerAddress = delegate (List<String> address)
                {
                    customer.Address1 = address[0];
                    customer.Address2 = address[1];
                    customer.Address3 = address[2];
                    customer.Address4 = address[3];
                };

                //if (addresses.ContainsKey(Address.Home) && addresses[Address.Home].GetRange(0, 2).TrueForAll((str) => !string.IsNullOrEmpty(str.Trim())))
                if (addresses.ContainsKey(Address.Home))
                {
                    assignCustomerAddress(addresses[Address.Home]);
                }
                //else if (addresses.ContainsKey(Address.Work) && addresses[Address.Work].GetRange(0, 2).TrueForAll((str) => !string.IsNullOrEmpty(str.Trim())))
                else if (addresses.ContainsKey(Address.Work))
                {
                    assignCustomerAddress(addresses[Address.Work]);
                }
                //else if (addresses.ContainsKey(Address.Delivery) && addresses[Address.Delivery].GetRange(0, 2).TrueForAll((str) => !string.IsNullOrEmpty(str.Trim())))
                else if (addresses.ContainsKey(Address.Delivery))
                {
                    assignCustomerAddress(addresses[Address.Delivery]);
                }

                receipt.DocumentName = receipt.Title.Value + " : " + customer.Name;

                receipt.Customer.Value = customer;
                receipt.AccountNo.Value = accountNo;
                if (agreemtInvNum != string.Empty && agreemtInvNum.Length > 3)
                {
                    if (agreementNo == 1)
                    {
                        receipt.TaxInvoice.Value = agreemtInvNum.Insert(3, "-") + "-" + version_no;
                    }
                    else
                    {
                        receipt.TaxInvoice.Value = agreemtInvNum.Insert(3, "-");
                    }
                }
                else
                {
                    receipt.TaxInvoice.Value = Config.BranchCode + "/" + buffNo.ToString();
                }

                //receipt.InvoiceNo.Value = ((Config.BranchCode + "000000").ToInt32() + branchHissn.ToInt32()).ToString();            //IP - 09/05/12 - #9610 - CR8520 
                receipt.Date.Value = DateTime.Now;
                //receipt.CashierID.Value = Credential.User;
                receipt.CashierID.Value = Credential.UserId.ToString() + " " + Credential.Name;   //IP - 17/05/12 - #9447 - CR1239
                receipt.BranchCode.Value = Config.BranchCode;
                receipt.PayMethod.ShouldBePrinted = false;                      //IP - 12/09/11 - #8128 - UAT64 - Paymethods not required to be printed on Tax Invoice

                //BOC CR 2018-13
                receipt.SalesPerson.Value = salesman;
                if (dtInvDate != null)
                    receipt.InvoiceDate.Value = dtInvDate.ToString("dd-MMM-yyyy");
                string currencySymbol = (string)Country[CountryParameterNames.CurrencySymbolForPrint];
                receipt.CurrencySymbol.Value = currencySymbol;
                receipt.CountryCode.Value = Convert.ToString(Country[CountryParameterNames.CountryCode]);
                //BOC CR 2018-13

                if (ReprintInvoice == true)
                {
                    if (agreementNo == 1)//Win Sales Report
                    {
                        receipt.PayMethod.ShouldBePrinted = false;
                    }
                    else
                    {//Web Sales Report
                        if (dtInvoicePaymentDetails.Rows.Count > 0)
                        {
                            receipt.PayMethod.ShouldBePrinted = true;
                            DataSet payMethodSet = new DataSet();
                            DataTable dtPayMethodSet = new DataTable(STL.Common.Constants.TableNames.TN.PayMethodList);
                            dtPayMethodSet.Columns.Add(CN.Value);
                            dtPayMethodSet.Columns.Add(CN.CodeDescription);
                            foreach (DataRow dr in dtInvoicePaymentDetails.Rows)
                            {
                                DataRow drNew = dtPayMethodSet.NewRow();
                                drNew[CN.CodeDescription] = dr["payMethod"];
                                drNew[CN.Value] = dr["amount"];
                                dtPayMethodSet.Rows.Add(drNew);
                            }
                            payMethodSet.Tables.Add(dtPayMethodSet);
                            receipt.payMethodSet = payMethodSet;
                        }
                    }
                    receipt.BranchCode.Value = agreemtInvNum.Substring(0, 3);
                }
                else
                {
                    if (paidAndTaken)                                               //IP - 23/09/11 - RI - #8236 - Fixed error caused by fix to LW73769
                    {
                        receipt.PayMethod.ShouldBePrinted = true;
                    }
                    else
                    {
                        receipt.PayMethod.ShouldBePrinted = false;
                    }
                }

                int branchCode = Convert.ToInt32(Config.BranchCode);

                decimal totalTax = 0, AdditionaltotalTax = 0, invoiceTotal = 0, totalAmount = 0, OtherCurrencytotalAmount = 0;
                bool isOtherCorruncyActive = false;
                string OtherCurrencyName = "";
                List<Product> products = new List<Product>();
                XmlNode xmlNodeProducts = null;
                if (ReprintInvoice == true)
                {
                    if (lineItems == null)
                    {
                        if (agreementNo == 1)
                        {
                            xmlNodeProducts = AccountManager.GetLineItemsWithVersion(accountNo, agreementNo, accountType, Config.CountryCode, Convert.ToInt16(Config.BranchCode), versionNo, out Error);
                        }
                        else
                        {
                            xmlNodeProducts = AccountManager.GetSalesOrderLineItems(accountNo, agreementNo, accountType, Config.CountryCode, Convert.ToInt16(Config.BranchCode), agreemtInvNum, out Error);
                        }
                        //xmlNodeProducts = AccountManager.GetLineItemsWithVersion(accountNo, agreementNo, accountType, Config.CountryCode, (short)branchCode, versionNo, out err);
                    }
                    else
                    {
                        xmlNodeProducts = lineItems;
                    }
                }
                else
                {
                    xmlNodeProducts = AccountManager.GetLineItems(accountNo, agreementNo, accountType, Config.CountryCode, (short)branchCode, out err);
                }
                if (xmlNodeProducts != null)
                    products = GetProducts(xmlNodeProducts.ChildNodes, taxExempt, ref totalAmount, ref totalTax, ref AdditionaltotalTax, ref invoiceTotal, ref products, ref OtherCurrencytotalAmount, ref OtherCurrencyName, ref isOtherCorruncyActive, agreementNo);  //#18112

                // Extract the deffered terms amount/service charge, then remove it from products list
                Product tmpProduct = null;
                foreach (Product product in products)
                    if (product.ProductCode == "DT")
                    {
                        tmpProduct = product;
                        receipt.DeferredTermsAmount.Value = product.TotalPrice;
                        break;
                    }
                products.Remove(tmpProduct);

                receipt.Products.Value = products;
                receipt.SubTotal.Value = totalAmount;
                receipt.TotalTax.Value = totalTax;
                receipt.Total.Value = invoiceTotal;
                receipt.AdditionalTotalTax.Value = AdditionaltotalTax; //BCX: This is used for LUX tax for curacao
                receipt.OtherCurrencyTotal.Value = OtherCurrencytotalAmount;
                receipt.OtherCurrencyName.Value = OtherCurrencyName;
                receipt.isOtherCorruncyActive.Value = isOtherCorruncyActive;
                receipt.Print();
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
            finally
            {
                StopWait();
                receipt.Dispose();
            }
        }

        private static void UpdateRemainingBalance(List<BBSL.Libraries.Printing.Transaction> payments, string lastAcctNo)
        {
            var lastAcctPayments = payments.FindAll(p => p.AccountNo == lastAcctNo);

            var paymentSubTotal = 0.0m;
            foreach (var p in lastAcctPayments)
            {
                paymentSubTotal += p.Amount;
            }

            var runningBalance = lastAcctPayments.Count > 0 ? lastAcctPayments[0].RemainingBalance : 0.0m;
            runningBalance += paymentSubTotal.ToPositive();
            foreach (var p in lastAcctPayments)
            {
                runningBalance = runningBalance + p.Amount;
                p.RemainingBalance = runningBalance;
                p.Amount = p.Amount.ToPositive();
            }
        }

        //private static void UpdateRemainingBalanceList(List<BBSL.Libraries.Printing.Transaction> payments, string lastAcctNo)
        //{
        //    var lastAcctPayments = payments.FindAll(p => p.AccountNo == lastAcctNo);

        //    var paymentSubTotal = 0.0m;
        //    foreach (var p in lastAcctPayments)
        //    {
        //        paymentSubTotal += p.Amount;
        //    }

        //    var runningBalance = lastAcctPayments.Count > 0 ? lastAcctPayments[0].RemainingBalance : 0.0m;
        //    runningBalance += paymentSubTotal.ToPositive();
        //    foreach (var p in lastAcctPayments)
        //    {
        //        runningBalance = runningBalance + p.Amount;
        //        p.RemainingBalance = runningBalance;
        //        p.Amount = p.Amount.ToPositive();
        //    }
        //}

        #endregion

        #region Laser Printer

        private void LaserPrintTaxInvoice(AxSHDocVw.AxWebBrowser b,
                                            string accountNo,
                                            int agreementNo,
                                            string accountType,
                                            string customerID,
                                            bool paidAndTaken,
                                            bool collection,
                                            XmlNode lineItems,
                                            int buffNo,
                                            bool creditNote,
                                            bool multiple, int versionNo = 0, bool ReprintInvoice = false)
        {

            object Zero = 0;
            object EmptyString = "";
            string url = "";

            /* make sure ampersands are properly encoded specifically
             * to cater for the PAID & TAKEN customer */
            customerID = customerID.Replace("&", "%26");

            string queryString = "customerID=" + customerID + "&" +
                "acctNo=" + accountNo + "&" +
                "accountType=" + accountType + "&" +
                "culture=" + Config.Culture + "&" +
                "country=" + Config.CountryCode + "&" +
                "branch=" + Config.BranchCode + "&" +
                "buffno=" + buffNo.ToString() + "&" +
                 "creditNote=" + creditNote.ToString() + "&" +
                 "multiple=" + multiple.ToString() + "&" +
                 "versionNo=" + versionNo.ToString() + "&" +//
                 "ReprintInvoice=" + ReprintInvoice + "&" +//
                 "agrmtno=" + agreementNo + "&" +//
                "user=" + Credential.UserId.ToString() + "&" +
                "IsProofofPurchase=false";

            /* if this is the paid and taken account then we must pass the 
             * lineItems back which means we need to use HTTP POST because 
             * of the limit on querystring length */
            if (paidAndTaken)
            {
                /* various characters in the xml will be automatically escaped
                 * e.g. " becomes &quot; This leaves ampersands in the querystring 
                 * which must be encoded or they will cause problems */
                string xml = lineItems.OuterXml;
                xml = xml.Replace("&", "%26");
                queryString += "&collection=" + collection.ToString();
                queryString += "&lineItems=" + xml;
                url = Config.Url + "WPaidAndTakenTaxInvoice.aspx";
                object postData = EncodePostData(queryString);
                object headers = PostHeader;
                b.Navigate(url, ref Zero, ref EmptyString, ref postData, ref headers);
            }
            else
            {
                url = Config.Url + "WTaxInvoice.aspx?" + queryString;
                b.Navigate(url, ref Zero, ref EmptyString, ref EmptyString, ref EmptyString);
            }

            //Commenting this since we want to audit the print only when printing is finished completely. This code is in WTaxInvoice.aspx
            // Audit the Tax Invoice print if it is a re-print
            //AccountManager.AuditReprint(accountNo, agreementNo, DocumentType.TaxInvoice);            
        }

        #endregion

        /// <summary>
        /// Extracts product information from an XmlNodeList, makes <see cref="STL.PL.Printing.Product"/> 
        /// out of them and returns them in a List Product.
        /// totalAmount, totalTax & invoiceTotal will contain the correct values when this method returns
        /// </summary>
        /// <param name="items"></param>
        /// <param name="taxExempt"></param>
        /// <param name="totalAmount"></param>
        /// <param name="totalTax"></param>
        /// <param name="invoiceTotal"></param>
        /// <param name="products"></param>
        /// <returns></returns>
        private List<Product> GetProducts(XmlNodeList items, bool taxExempt, ref decimal totalAmount, ref decimal totalTax, ref decimal AdditioaltotalTax, ref decimal invoiceTotal, ref List<Product> products, ref decimal OtherCurrencyinvoiceTotal, ref string OtherCurrencyName, ref bool isOtherCorruncyActive, int agreementNo = 0)
        {
            string _error = "";
            foreach (XmlNode item in items)
            {
                Product product = new Product();

                decimal taxRate = 0;
                decimal itemValue = 0;
                decimal ActualitemValue = 0;
                decimal AdditionaltaxRate = 0;

                string termsType = "";
                if (this is NewAccount)
                    termsType = ((NewAccount)this).TermsType;

                if (item.Attributes[Tags.Code].Value != "STAX" &&
                    item.Attributes[Tags.Quantity].Value != "0" &&
                    (item.Attributes[Tags.Code].Value != "DT" ||		/* don't display DT for indonesia */
                    Config.CountryCode != "I"))
                {
                    if (item.Attributes[Tags.Type].Value == IT.Component)
                    {
                        //PrintKitLineItem(rp, item, header, taxExempt, ref totalAmount, ref totalTax, ref invoiceTotal, ref lines);
                    }
                    else
                    {
                        //The foll has been commented since this was calculating incorrect tax value
                        //taxRate = Convert.ToDecimal(item.Attributes[Tags.TaxRate].Value) * 100; //BCX : This is used for LUX tax for curacao 
                        taxRate = Convert.ToDecimal(item.Attributes[Tags.TaxRate].Value); //BCX : This is used for LUX tax for curacao 
                        product.Quantity = Convert.ToInt32(item.Attributes[Tags.Quantity].Value);
                        if (ReprintInvoiceOption)//for Reprint
                        {
                            if (product.Quantity < 0)//for Return Case
                            {
                                if (agreementNo != 1 && string.IsNullOrEmpty(item.Attributes[Tags.ParentItemId].Value) == false)
                                {//Web case for Warranty Items
                                    itemValue = Convert.ToDecimal(item.Attributes[Tags.UnitPrice].Value) * -1;
                                }
                                else
                                {
                                    itemValue = Convert.ToDecimal(item.Attributes[Tags.Value].Value) * -1;
                                }
                            }
                            else
                            {
                                if (agreementNo != 1 && string.IsNullOrEmpty(item.Attributes[Tags.ParentItemId].Value) == false)
                                {//Web case for Warranty Items
                                    itemValue = Convert.ToDecimal(item.Attributes[Tags.UnitPrice].Value);
                                }
                                else
                                {
                                    itemValue = Convert.ToDecimal(item.Attributes[Tags.Value].Value);
                                }
                            }
                        }
                        else
                        {
                            itemValue = Convert.ToDecimal(item.Attributes[Tags.Value].Value);
                        }
                        if (item.Attributes[Tags.AdditionalTaxRates].Value != null)
                        {
                            if (item.Attributes[Tags.AdditionalTaxRates].Value != null)
                            {
                                AdditionaltaxRate = Convert.ToDecimal(item.Attributes[Tags.AdditionalTaxRates].Value) * 100;
                            }
                            else
                            {
                                AdditionaltaxRate = 0;
                            }
                        }
                        else
                        {
                            AdditionaltaxRate = 0;
                        }


                        //IP - 23/09/11 - RI - #8236 - CR8201

                        if (Convert.ToString(Country[CountryParameterNames.RIInterfaceOptions]) != "FACT")
                        {
                            product.Description = item.Attributes[Tags.Description1].Value + " " + item.Attributes[Tags.Description2].Value + " " + item.Attributes[Tags.Brand].Value + " " + item.Attributes[Tags.Style].Value;
                        }
                        else
                        {
                            product.Description = item.Attributes[Tags.Description1].Value + " " + item.Attributes[Tags.Description2].Value;
                        }

                        //BCX: This is used for LUX tax for curacao
                        ActualitemValue = itemValue;
                        product.ProductCode = item.Attributes[Tags.Code].Value;
                        product.TaxAmount = AdditionaltaxRate > 0 ? 0 : CalculateTaxAmount(taxRate, taxExempt, ref itemValue);
                        product.TaxPercent = AdditionaltaxRate > 0 ? 0 : taxRate;
                        //itemValue = ActualitemValue;                    
                        product.AdditionalTaxAmount = CalculateTaxAmount(AdditionaltaxRate, taxExempt, ref ActualitemValue);
                        product.AdditionalTaxPercent = AdditionaltaxRate;
                        if (agreementNo != 1 && !ReprintInvoiceOption) // 6714210 : Incorrect Invoice Total - Web Case Fix
                        {
                            itemValue = ActualitemValue - product.TaxAmount - product.AdditionalTaxAmount;
                        }
                        // This condition is used for Inclusive tax type country 
                        // where extra tax need to consider for total and unit price.
                        if (product.TaxAmount == 0 && product.AdditionalTaxAmount > 0)
                        {
                            product.TotalPrice = product.TaxAmount + product.AdditionalTaxAmount + ActualitemValue;
                            product.UnitPrice = ActualitemValue;
                        }
                        else
                        {
                            product.TotalPrice = product.TaxAmount + product.AdditionalTaxAmount + itemValue;
                            product.UnitPrice = itemValue;
                        }


                        //if (termsType == "WC" && (item.Attributes[Tags.Type].Value == IT.Warranty ||
                        //    item.Attributes[Tags.Type].Value == IT.KitWarranty))
                        //rp.PrintString("Warranty To Pay");
                        totalAmount += itemValue;
                        totalTax += product.TaxAmount;
                        AdditioaltotalTax += product.AdditionalTaxAmount;
                        invoiceTotal += product.TotalPrice;

                        //CR201801014-TaxInvoice Amount displayed in Exchange Rate
                        DataSet ds = StaticDataManager.GetCountryMaintenanceParameters(Config.CountryCode, out Error);
                        DataRow[] drSelected = ds.Tables[0].Select("CodeName = 'InvoiceGuilder'");
                        isOtherCorruncyActive = Convert.ToBoolean(drSelected[0].ItemArray[4].ToString());

                        DataSet ExchangeRateSet = PaymentManager.GetExchangeRates(out _error);
                        DataTable ExchangeRateTable = ExchangeRateSet.Tables[TN.ExchangeRates];
                        if (ExchangeRateTable != null)
                        {
                            OtherCurrencyinvoiceTotal += (product.TotalPrice / Convert.ToDecimal(ExchangeRateTable.Rows[0]["rate"]));
                            OtherCurrencyName = ExchangeRateTable.Rows[0][1].ToString();
                        }
                        if (!product.IsEmpty)
                            products.Add(product);

                        XmlNode related = item.SelectSingleNode(Elements.RelatedItem);
                        if (related.ChildNodes.Count > 0)
                            products = GetProducts(related.ChildNodes, taxExempt, ref totalAmount, ref totalTax, ref AdditioaltotalTax, ref invoiceTotal, ref products, ref OtherCurrencyinvoiceTotal, ref OtherCurrencyName, ref isOtherCorruncyActive, agreementNo);
                        //CR201801014 - End
                    }
                }
            }

            return products;
        }

        #endregion

        protected void OpenCashDrawer(bool requiresAuth)
        {
            ///this is the default reason used for the logging in opening.
            string reason = "STAR";
            bool authorised = !requiresAuth;

            if (Config.CashDrawerID.Length > 0)
            {
                if (requiresAuth)
                {
                    authCashTill = new Control();
                    authCashTill.Enabled = false;
                    authCashTill.Visible = false;
                    authCashTill.Name = "authCashTill";
                    AuthorisePrompt auth = new AuthorisePrompt(this, authCashTill, GetResource("M_CASHTILLAUTH"));
                    auth.ShowDialog();
                    authorised = auth.Authorised;
                    if (authorised)
                    {
                        CashTillOpen cto = new CashTillOpen((DataTable)StaticData.Tables[TN.CashDrawerReasons]);
                        cto.ShowDialog();
                        reason = cto.Reason;
                    }
                }

                if (authorised)
                {
                    if (SlipPrinterOK(true))
                    {
                        ReceiptPrinter rp = new ReceiptPrinter(this);
                        rp.OpenPrinter();
                        rp.OpenDrawer();
                        rp.ClosePrinter();

                        ///Only log the cash drawer opening if a reason code has been provided
                        if (reason.Length > 0)
                            SaveCashDrawerOpen(Credential.UserId, reason);
                    }
                }
            }
        }

        protected void SaveCashDrawerOpen(int user, string reason)
        {
            PaymentManager.SaveCashDrawerOpen(user, reason, Config.CashDrawerID, out Error);
            if (Error.Length > 0)
                ShowError(Error);
        }

        private decimal CalculateTaxAmount(decimal taxRate, bool taxExempt, ref decimal orderValue)
        {
            decimal taxamt = 0;

            if (!taxExempt)
            {
                //order values held tax exclusive
                //if(Country.TaxType == "I" && Country.AgreementTaxType == "E")
                if ((string)Country[CountryParameterNames.AgreementTaxType] == "E")
                    taxamt = (orderValue * taxRate) / 100;

                //order values held tax inclusive
                //if(Country.TaxType == "E" && Country.AgreementTaxType == "I")
                if ((string)Country[CountryParameterNames.AgreementTaxType] == "I")
                {
                    taxamt = (orderValue * taxRate) / (100 + taxRate);
                    orderValue -= Math.Round(taxamt, 2);
                }
            }
            return Math.Round(taxamt, 2);
        }

        private void PrintLineItem(ReceiptPrinter rp, XmlNode item, string header, bool taxExempt, ref decimal totalAmount, ref decimal totalTax, ref decimal invoiceTotal, ref int lines)
        {
            string text = "";
            string amounts = "";
            decimal tax = 0;
            decimal taxRate = 0;
            decimal unitPrice = 0;
            decimal quantity = 0;
            decimal itemValue = 0;
            lines++;

            string termsType = "";
            if (this is NewAccount)
                termsType = ((NewAccount)this).TermsType;

            rp.Init();
            rp.Narrow();

            Thread.Sleep(1000);

            if (item.Attributes[Tags.Code].Value != "STAX" &&
                item.Attributes[Tags.Quantity].Value != "0" &&
                (item.Attributes[Tags.Code].Value != "DT" ||		/* don't display DT for indonesia */
                Config.CountryCode != "I"))
            {
                if (item.Attributes[Tags.Type].Value == IT.Kit)
                {
                    PrintKitLineItem(rp, item, header, taxExempt, ref totalAmount, ref totalTax, ref invoiceTotal, ref lines);
                }
                else
                {
                    quantity = Convert.ToDecimal(item.Attributes[Tags.Quantity].Value);
                    taxRate = Convert.ToDecimal(item.Attributes[Tags.TaxRate].Value);
                    unitPrice = Convert.ToDecimal(item.Attributes[Tags.UnitPrice].Value);
                    itemValue = Convert.ToDecimal(item.Attributes[Tags.Value].Value);

                    text = item.Attributes[Tags.Quantity].Value;
                    text += "\t" + item.Attributes[Tags.Code].Value;

                    //IP - 23/09/11 - RI - #8237 - CR8201
                    if (Convert.ToString(Country[CountryParameterNames.RIInterfaceOptions]) != "FACT")
                    {
                        text += " " + item.Attributes[Tags.Description1].Value + " " + item.Attributes[Tags.Brand].Value + " " + item.Attributes[Tags.Style].Value;
                    }
                    else
                    {
                        text += " " + item.Attributes[Tags.Description1].Value;
                    }

                    text += "\n\t" + item.Attributes[Tags.Description2].Value;
                    if (item.Attributes[Tags.ContractNumber].Value.Length > 0)
                        text += "\n\t(" + item.Attributes[Tags.ContractNumber].Value + ")";
                    rp.PrintString(text);
                    rp.PrintString(header);
                    tax = CalculateTaxAmount(taxRate, taxExempt, ref itemValue);

                    amounts = itemValue.ToString(Format).PadRight(11, ' ');
                    if (tax == Convert.ToDecimal(0) && Config.CountryCode == "Z")
                        amounts += "Zero Rated".PadRight(11, ' ');
                    else
                        amounts += tax.ToString(Format).PadRight(11, ' ');
                    amounts += (tax + itemValue).ToString(Format).PadRight(11, ' ');
                    rp.PrintString(amounts);
                    if (termsType == "WC" && (item.Attributes[Tags.Type].Value == IT.Warranty ||
                        item.Attributes[Tags.Type].Value == IT.KitWarranty))
                        rp.PrintString("Warranty To Pay");
                    rp.Feed("\x1", 1);

                    totalAmount += itemValue;
                    totalTax += tax;
                    invoiceTotal += itemValue + tax;

                    //recurse
                    XmlNode related = item.SelectSingleNode(Elements.RelatedItem);
                    foreach (XmlNode child in related.ChildNodes)
                        PrintLineItem(rp, child, header, taxExempt, ref totalAmount, ref totalTax, ref invoiceTotal, ref lines);
                }
            }
        }

        // 5.1 uat138 rdb 6/12/07 calculate kit item tax individuallt per item to avoid rounding differences
        // with server figure
        private decimal CalculateKitTax(XmlNode related, bool taxExempt, ref decimal orderValue)
        {
            decimal taxRate;
            decimal itemValue;
            decimal taxamt = 0;
            decimal taxTotal = 0;

            if (!taxExempt)
            {
                foreach (XmlNode child in related.ChildNodes)
                {
                    taxRate = Convert.ToDecimal(child.Attributes[Tags.TaxRate].Value);
                    itemValue = Convert.ToDecimal(child.Attributes[Tags.Value].Value);

                    //order values held tax exclusive
                    //if(Country.TaxType == "I" && Country.AgreementTaxType == "E")
                    if ((string)Country[CountryParameterNames.AgreementTaxType] == "E")
                        taxamt = (itemValue * taxRate) / 100;

                    //order values held tax inclusive
                    //if(Country.TaxType == "E" && Country.AgreementTaxType == "I")
                    if ((string)Country[CountryParameterNames.AgreementTaxType] == "I")
                    {
                        taxamt = (itemValue * taxRate) / (100 + taxRate);
                        orderValue -= Math.Round(taxamt, 2);
                    }

                    taxamt = Math.Round(taxamt, 2);

                    taxTotal += taxamt;

                }
            }


            return taxTotal;
        }

        private void PrintKitLineItem(ReceiptPrinter rp, XmlNode item, string header, bool taxExempt, ref decimal totalAmount, ref decimal totalTax, ref decimal invoiceTotal, ref int lines)
        {
            string text = "";
            string amounts = "";
            decimal tax = 0;
            decimal taxRate = 0;
            decimal unitPrice = 0;
            decimal quantity = 0;
            decimal itemValue = 0;

            Thread.Sleep(1000);

            quantity = Convert.ToDecimal(item.Attributes[Tags.Quantity].Value);
            taxRate = Convert.ToDecimal(item.Attributes[Tags.TaxRate].Value);
            unitPrice = Convert.ToDecimal(item.Attributes[Tags.UnitPrice].Value);
            itemValue = Convert.ToDecimal(item.Attributes[Tags.Value].Value);

            text = item.Attributes[Tags.Quantity].Value;
            text += "\t" + item.Attributes[Tags.Code].Value;

            //IP - 23/09/11 - RI - #8237 - CR8201
            if (Convert.ToString(Country[CountryParameterNames.RIInterfaceOptions]) != "FACT")
            {
                text += " " + item.Attributes[Tags.Description1].Value + " " + item.Attributes[Tags.Brand].Value + " " + item.Attributes[Tags.Style].Value;
            }
            else
            {
                text += " " + item.Attributes[Tags.Description1].Value;
            }

            rp.PrintString(text);
            rp.PrintString(header);
            // when calculating tax on a kit item rounding errors occur if you take tax on whole value of kit
            // we need to look at each item and calculate tax individually then total to match server
            if (item.Attributes[Tags.Type].Value == IT.Kit)
            {
                XmlNode kitReleated = item.SelectSingleNode(Elements.RelatedItem);
                tax = CalculateKitTax(kitReleated, taxExempt, ref itemValue);


            }
            else
            {
                tax = CalculateTaxAmount(taxRate, taxExempt, ref itemValue);
            }
            amounts = itemValue.ToString(Format).PadRight(11, ' ');
            if (tax == Convert.ToDecimal(0) && Config.CountryCode == "Z")
                amounts += "Zero Rated".PadRight(11, ' ');
            else
                amounts += tax.ToString(Format).PadRight(11, ' ');
            amounts += Math.Round(tax + itemValue, 2).ToString().PadRight(11, ' ');
            rp.PrintString(amounts);

            XmlNode related = item.SelectSingleNode(Elements.RelatedItem);
            foreach (XmlNode comp in related.ChildNodes)
            {
                Thread.Sleep(1000);
                text = comp.Attributes[Tags.Quantity].Value;
                text += "\t" + comp.Attributes[Tags.Code].Value;

                //IP - 23/09/11 - RI - #8237 - CR8201
                if (Convert.ToString(Country[CountryParameterNames.RIInterfaceOptions]) != "FACT")
                {
                    text += " " + comp.Attributes[Tags.Description1].Value + " " + comp.Attributes[Tags.Brand].Value + " " + comp.Attributes[Tags.Style].Value;
                }
                else
                {
                    text += " " + comp.Attributes[Tags.Description1].Value;
                }

                if (comp.Attributes[Tags.ContractNumber].Value.Length > 0)
                    text += "\n\t(" + comp.Attributes[Tags.ContractNumber].Value + ")";
                rp.PrintString(text);
                XmlNode node = comp.SelectSingleNode(Elements.RelatedItem);
                foreach (XmlNode grandChild in node.ChildNodes)
                    PrintLineItem(rp, grandChild, header, taxExempt, ref totalAmount, ref totalTax, ref invoiceTotal, ref lines);
            }
            totalAmount += itemValue;
            totalTax += tax;
            invoiceTotal += itemValue + tax;
        }


        public void TranslateControls()
        {
            //if(Config.Culture.IndexOf("en-")==-1)
            //{
            this.Text = Translate(this.Text);
            TranslateControls(this.Controls);
            //}
        }

        public void TranslateMenus(Crownwood.Magic.Collections.MenuCommandCollection menus)
        {
            //if(Config.Culture.IndexOf("en-")==-1)
            //{
            if (menus != null)
            {
                foreach (Crownwood.Magic.Menus.MenuCommand m in menus)
                {
                    m.Text = Translate(m.Text);
                    if (m.MenuCommands.Count > 0)
                        TranslateMenus(m.MenuCommands);
                }
            }
            //}
        }


        /// <summary>
        /// Method to loop through the controls on a form and translate them into the 
        /// language represented by the current culture setting. Will be called by each 
        /// page constructor but can also be called at other times (i.e. when the 
        /// culture is changed
        /// </summary>
        public void TranslateControls(System.Windows.Forms.Control.ControlCollection controls)
        {
            foreach (Control c in controls)
            {

                switch ((c.GetType()).Name)
                {
                    case "DataGrid":
                        if (((DataGrid)c).CaptionText.Length != 0)
                            ((DataGrid)c).CaptionText = Translate(((DataGrid)c).CaptionText);
                        break;
                    case "TabPage":
                        //try
                        //{
                        if (c is Crownwood.Magic.Controls.TabPage) // don't do anything if using normal tab...
                        {
                            if (((Crownwood.Magic.Controls.TabPage)c).Title.Length != 0)
                                ((Crownwood.Magic.Controls.TabPage)c).Title = Translate(((Crownwood.Magic.Controls.TabPage)c).Title);
                        }
                        //}

                        //catch
                        //{

                        //}
                        if (c.Controls.Count > 0)
                            TranslateControls(c.Controls);

                        break;
                    case "ComboBox":		//Do not need translating
                        break;
                    case "TextBox":			//Do not need translating
                        break;
                    case "AccountTextBox":	//Do not need translating
                        break;
                    case "DateTimePicker":	//Do not need translating
                        break;
                    case "MenuControl":
                        if (((Crownwood.Magic.Menus.MenuControl)c).MenuCommands.Count > 0)
                            TranslateMenus(((Crownwood.Magic.Menus.MenuControl)c).MenuCommands);
                        break;
                    default:
                        if (c.Text.Length != 0)
                            c.Text = Translate(c.Text);
                        if (c.Controls.Count > 0)
                            TranslateControls(c.Controls);
                        break;
                }
            }
        }

        private static string Translate(string text)
        {
            //look up the string in the hash table which corresponds to the current 
            //culture and return the result
            object translation = null;
            Hashtable dictionary = (Hashtable)StaticData.Dictionaries[Config.Culture];
            if (dictionary != null)
            {
                translation = dictionary[text];
                text = translation == null ? text : translation.ToString();
            }
            return text;
        }

        private void LoadDictionaryThread()
        {
            try
            {
                Wait();
                Function = "LoadDictionaryThread";

                Hashtable ht = new Hashtable();
                /* Disable loading the translation from the server 
                DataSet ds = StaticDataManager.GetDictionary(Config.Culture, out Error);
                if (Error.Length > 0)
                    ShowError(Error);
                else
                {
                    foreach (DataTable dt in ds.Tables)
                        foreach (DataRow r in dt.Rows)
                            ht[r[CN.English]] = r[CN.Translation];*/
                StaticData.Dictionaries[Config.Culture] = ht;
                /*}*/
            }

            //IP - 18/08/08 - CoSACS Improvement
            //Added a catch block to catch a soap exception, when the server connection or database setup is incorrect.
            //Message box should be displayed informing the user to check their settings and they will be unable to log into CoSACS.
            catch (SoapException ex)
            {
                if (ex.Message.Contains("Cannot open database"))
                {
                    MessageBox.Show("Please check your server connection and database setup", "Alert", MessageBoxButtons.OK);

                    MainForm mainFrm = this as MainForm;

                    if (mainFrm != null)
                        mainFrm.grpLogInCourts.Enabled = false;

                }
                else
                    Catch(ex, Function);
            }
            //catch	url may not have been set
            //{
            //    //Nothing we can do about this, translation will not be 
            //    //possible until the webservice url has been set
            //}
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
            finally
            {
                StopWait();
                Function = "End of LoadDictionaryThread";
            }
        }

        public void LoadDictionary()
        {
            try
            {
                //Don't need to translate into english
                //if(Config.Culture.IndexOf("en-")==-1)
                //{
                if (StaticData.Dictionaries[Config.Culture] == null)
                {
                    //Thread data = new Thread(new ThreadStart(LoadDictionaryThread));
                    //data.Start();
                    //data.Join();
                    LoadDictionaryThread();
                }
                //}
            }
            catch (WebException)	//url may not have been set
            {
                //Nothing we can do about this, translation will not be 
                //possible until the webservice url has been set
            }
        }

        public void LoadDictionary(string culture)
        {
            try
            {
                //Don't need to translate into english
                //if(culture.IndexOf("en-")==-1)
                //{
                if (StaticData.Dictionaries[culture] == null)
                {
                    Hashtable ht = new Hashtable();
                    //DataSet ds = StaticDataManager.GetDictionary(culture, out Error);
                    if (Error.Length > 0)
                        ShowError(Error);
                    else
                    {
                        //foreach (DataTable dt in ds.Tables)
                        //    foreach (DataRow r in dt.Rows)
                        //        ht[r[CN.English]] = r[CN.Translation];
                        StaticData.Dictionaries[culture] = ht;
                    }
                }
                //}
            }
            catch (WebException)	//url may not have been set
            {
                //Nothing we can do about this, translation will not be 
                //possible until the webservice url has been set
            }
        }

        //Quick bubble sort to arrange the cultures into order
        public void BubbleSort(CultureInfo[] a)
        {
            for (int i = 0; i < a.Length; i++)
            {
                for (int j = a.Length - 1; j > i; j--)
                {
                    if (0 <= a[j - 1].DisplayName.CompareTo(a[j].DisplayName))
                    {
                        CultureInfo T = a[j - 1];
                        a[j - 1] = a[j];
                        a[j] = T;
                    }
                }
            }
        }

        //this is so wrong in so many ways
        public void ClearControls(System.Windows.Forms.Control.ControlCollection controls)
        {
            foreach (Control c in controls)
            {
                string type = c.GetType().ToString();
                switch (type)
                {
                    case "System.Windows.Forms.TextBox":
                        c.Text = "";
                        break;
                    case "System.Windows.Forms.RichTextBox":
                        c.Text = "";
                        break;
                    case "System.Windows.Forms.NumericUpDown":
                        ((NumericUpDown)c).Value = 0;
                        break;
                    case "System.Windows.Forms.ComboBox":
                        if (((ComboBox)c).Items.Count > 0)
                            ((ComboBox)c).SelectedIndex = 0;
                        break;
                    case "System.Windows.Forms.DataGrid":
                        ((DataGrid)c).DataSource = null;
                        break;
                    case "System.Windows.Forms.CheckBox":
                        ((CheckBox)c).Checked = false;
                        break;
                    case "System.Windows.Forms.DateTimePicker":
                        ((DateTimePicker)c).Value = DateTime.Today;
                        break;
                    case "STL.PL.AccountTextBox":
                        c.Text = "000-0000-0000-0";
                        break;
                    case "System.Windows.Forms.DataGridView":
                        ((DataGridView)c).DataSource = null;
                        break;
                    case "STL.PL.CustomControl.WatermarkTextBox": // Address Standardization CR2019 - 025
                        c.Text = string.Empty;
                        break;
                    case "STL.PL.CustomControl.AutoSuggestCombo": // Address Standardization CR2019 - 025
                        if (((ComboBox)c).Items.Count > 0)
                            ((ComboBox)c).SelectedIndex = -1;
                        ((ComboBox)c).Text = string.Empty;
                        break;
                    default:
                        break;
                }
                ClearControls(c.Controls);
            }
        }

        public StringCollection ListLabels()
        {
            StringCollection s = new StringCollection();
            s.Add(this.Text);
            ListLabels(this.Controls, ref s);

            //have to force the case to MainForm in this case otherwise it
            //picks up the inherited version which is null. Clumsy and hateful,
            //but easier than puting it right properly
            if ((this.GetType()).Name == "MainForm")
            {
                ListMenus(((MainForm)this).menuMain.MenuCommands, ref s);
            }
            else
            {
                if (this.menuMain != null)
                    ListMenus(this.menuMain.MenuCommands, ref s);
            }

            return s;
        }

        public void ListMenus(Crownwood.Magic.Collections.MenuCommandCollection menus, ref StringCollection s)
        {
            foreach (Crownwood.Magic.Menus.MenuCommand m in menus)
            {
                if (m.Text.Length != 0)
                    s.Add(m.Text);
                if (m.MenuCommands.Count > 0)
                    ListMenus(m.MenuCommands, ref s);
            }
        }

        public void ListLabels(System.Windows.Forms.Control.ControlCollection controls, ref StringCollection s)
        {
            foreach (Control c in controls)
            {

                switch ((c.GetType()).Name)
                {
                    case "DataGrid":
                        if (((DataGrid)c).CaptionText != "")
                            s.Add(((DataGrid)c).CaptionText);
                        break;
                    case "TabPage":
                        if (((Crownwood.Magic.Controls.TabPage)c).Title != "")
                            s.Add(((Crownwood.Magic.Controls.TabPage)c).Title);
                        break;
                    case "ComboBox":		//Do not need translating
                        break;
                    case "TextBox":			//Do not need translating
                        break;
                    case "AccountTextBox":	//Do not need translating
                        break;
                    case "DateTimePicker":	//Do not need translating
                        break;
                    default:
                        if (c.Text != "")
                            s.Add(c.Text);
                        break;
                }
                if (c.Controls.Count > 0)
                    ListLabels(c.Controls, ref s);
            }
        }

        public AxSHDocVw.AxWebBrowser[] CreateBrowserArray(int numBrowsers)
        {
            ((MainForm)FormRoot).PrinterStat.Clear();

            AxSHDocVw.AxWebBrowser[] browsers = new AxSHDocVw.AxWebBrowser[numBrowsers];

            for (int i = 0; i < browsers.Length; i++)
            {
                browsers[i] = new AxSHDocVw.AxWebBrowser();
                this.Controls.Add(browsers[i]);
                browsers[i].ContainingControl = this;
                browsers[i].Enabled = true;
                browsers[i].Location = new System.Drawing.Point(100, 100);
                browsers[i].Size = new System.Drawing.Size(5, 5);
                browsers[i].TabIndex = i;
                browsers[i].DocumentComplete += new AxSHDocVw.DWebBrowserEvents2_DocumentCompleteEventHandler(this.DocumentComplete);
                browsers[i].Tag = true;
                //browsers[i].CreateControl(); // = true;
                var handle = browsers[i].Handle; // get the handle to force the control creation
                Debug.Assert(browsers[i].Created, "Browser should be created...");
            }

            /* if there is only one browser then we don't need to use the icons so
             * we need to flag the browser (using the Tag property so the document 
             * complete event can tell what to do */
            if (numBrowsers == 1)
                browsers[0].Tag = false;

            ((MainForm)FormRoot)._browsersLoaded = -1;
            ((MainForm)FormRoot).statusBar1.Text = "";
            return browsers;
        }

        private void DocumentComplete(object sender, AxSHDocVw.DWebBrowserEvents2_DocumentCompleteEvent e)
        {
            AxWebBrowser browser = (AxSHDocVw.AxWebBrowser)sender;

            if (((bool)Country[CountryParameterNames.PrintToolBar]) ||
                browser.LocationURL.Contains("DeliverySchedule.aspx") ||
                browser.LocationURL.Contains("DeliveryNote2.aspx"))
            {
                if ((bool)browser.Tag)
                    ((MainForm)FormRoot).PrinterStat.AddButton(browser);
                else
                {
                    IHTMLDocument2 HTMLDocument = (IHTMLDocument2)browser.Document;
                    HTMLDocument.title = "";
                    HTMLDocument.execCommand("Print", false, null);
                }
            }
            else
            {
                DocumentPrinting docPrinting = new DocumentPrinting((AxSHDocVw.AxWebBrowser)sender, (MainForm)FormRoot, (CommonForm)FormParent);
                Thread docThread = new Thread(new ThreadStart(docPrinting.DocumentCompleteThread));
                docThread.Start();
                this.Cursor = Cursors.Default;
            }
        }

        protected void PrintAgreementDocuments(string accountNo,
            string accountType,
            string customerID,
            bool paidAndTaken,
            bool collection,
            decimal paid,
            decimal change,
            XmlNode lineItems,
            int agreementNo,
            Form from,
            bool multiple,
            int salesPerson,
            short paymentMethod,
            bool spaPrint = false,          //UAT1012 jec
            bool printSchedule = false)        //UAT1012 jec
        {
            int noContracts = 0;
            int noPrints = 0;
            int noBrowsers = 4;

            //#13716 - CR12949 - Print Ready Assist Contracts
            ReadyAssistContracts(accountNo, agreementNo, accountType);

            if (((bool)Country[CountryParameterNames.PrintScheduleOfPayments] && spaPrint == false) || ((bool)Country[CountryParameterNames.PrintSPAscheduleOfPayments] && spaPrint == true))    //UAT1012 jec
                noBrowsers++;

            if (accountType == AT.ReadyFinance)
                noBrowsers++;

            XmlNodeList contracts = null;
            if (lineItems != null)
            {
                //string xpath = "//"+Elements.Item+"[@"+Tags.Type+"='"+IT.Warranty+"' and @"+Tags.Quantity+" > 0]//"+Elements.ContractNo;
                string xpath = "//" + Elements.Item + "[(@" + Tags.Type + "='" + IT.Warranty + "' or @" + Tags.Type + "='" + IT.KitWarranty + "') and @" + Tags.Quantity + " != '0']";
                contracts = lineItems.SelectNodes(xpath);
                noContracts = contracts.Count;
            }


            ((MainForm)FormRoot).browsers = ((CommonForm)FormRoot).CreateBrowserArray(noBrowsers + noContracts);

            noBrowsers = 0;

            if (accountType != AT.Cash)
            {
                if (lineItems != null)  //#19620
                {
                    PrintAgreement(((MainForm)FormRoot).browsers[noBrowsers++], accountNo, agreementNo, accountType, customerID, ref noPrints, multiple);
                }

            }

            if (accountType == AT.ReadyFinance &&
                !(from is NewAccount))
                PrintRFTerms(((MainForm)FormRoot).browsers[noBrowsers++], accountNo, customerID, ref noPrints);

            if (lineItems != null)  //#19620
            {
                NewPrintTaxInvoice(accountNo, agreementNo, accountType, customerID,
                paidAndTaken, collection, null, paid, change,
                lineItems, 0, ((MainForm)FormRoot).browsers[noBrowsers++], ref noPrints, false, multiple, salesPerson, paymentMethod, null, false);
            }


            PrintRFSummary(((MainForm)FormRoot).browsers[noBrowsers++], accountNo, customerID, accountType, ref noPrints);

            PrintScheduleOfPayments(((MainForm)FormRoot).browsers[noBrowsers++], accountNo, customerID, accountType, ref noPrints, spaPrint, printSchedule);    //UAT1012 jec

            if (contracts != null && (bool)Country[CountryParameterNames.PrintWarrantyContract])
            {
                PrintWarrantyContracts(((MainForm)FormRoot).browsers, accountNo, customerID, agreementNo, contracts, ref noPrints, noBrowsers, multiple);
            }


        }

        //CR906 12/09/07 rdb cash loans
        protected void PrintCashLoanDocuments(string accountNo,
            string accountType,
            string customerID,
            bool paidAndTaken,
            bool collection,
            decimal paid,
            decimal change,
            XmlNode lineItems,
            int agreementNo,
            Form from,
            bool multiple,
            int salesPerson,
            short paymentMethod)
        {
            int noContracts = 0;
            int noPrints = 0;
            int noBrowsers = 4;

            if ((bool)Country[CountryParameterNames.PrintScheduleOfPayments])
                noBrowsers++;

            if (accountType == AT.ReadyFinance)
                noBrowsers++;

            XmlNodeList contracts = null;
            if (lineItems != null)
            {
                //string xpath = "//"+Elements.Item+"[@"+Tags.Type+"='"+IT.Warranty+"' and @"+Tags.Quantity+" > 0]//"+Elements.ContractNo;
                string xpath = "//" + Elements.Item + "[(@" + Tags.Type + "='" + IT.Warranty + "' or @" + Tags.Type + "='" + IT.KitWarranty + "') and @" + Tags.Quantity + " != '0']";
                contracts = lineItems.SelectNodes(xpath);
                noContracts = contracts.Count;
            }


            ((MainForm)FormRoot).browsers = ((CommonForm)FormRoot).CreateBrowserArray(noBrowsers + noContracts);

            noBrowsers = 0;
            PrintCashLoan(((MainForm)FormRoot).browsers[noBrowsers++], accountNo, agreementNo, accountType, customerID, ref noPrints, multiple);

            if (accountType == AT.ReadyFinance &&
                !(from is NewAccount))
                PrintRFTerms(((MainForm)FormRoot).browsers[noBrowsers++], accountNo, customerID, ref noPrints);

            bool taxExempt = AccountManager.IsTaxExempt(accountNo, agreementNo.ToString(), out Error);
            NewPrintTaxInvoice(accountNo, agreementNo, accountType, customerID,
                paidAndTaken, collection, null, paid, change,
                lineItems, 0, ((MainForm)FormRoot).browsers[noBrowsers++], ref noPrints, false, multiple, salesPerson, paymentMethod, null, taxExempt);

            PrintRFSummary(((MainForm)FormRoot).browsers[noBrowsers++], accountNo, customerID, accountType, ref noPrints);

            PrintScheduleOfPayments(((MainForm)FormRoot).browsers[noBrowsers++], accountNo, customerID, accountType, ref noPrints);

            if (contracts != null && (bool)Country[CountryParameterNames.PrintWarrantyContract])
            {
                PrintWarrantyContracts(((MainForm)FormRoot).browsers, accountNo, customerID, agreementNo, contracts, ref noPrints, noBrowsers, multiple);
            }

            //#13716 - CR12949 - Print Ready Assist Contracts
            ReadyAssistContracts(accountNo, agreementNo, accountType);

        }


        //#13716 - CR12949 - Print Ready Assist Contracts
        protected void ReadyAssistContracts(string accountNo, int agreementNo, string accountType)
        {

            XmlNode refreshedLineItems = AccountManager.GetLineItems(accountNo, agreementNo, accountType, Config.CountryCode, Convert.ToInt16(Config.BranchCode), out Error);

            if (refreshedLineItems != null)  //#19620
            {
                XmlNodeList readyAssist = refreshedLineItems.SelectNodes("//Item[@ReadyAssist != 'False' and @Quantity != 0]");

                //var readyAssistContractSet = AccountManager.IsReadyAssistContractDateSet(accountNo, agreementNo, out Error);  //#18630 - CR15594

                if (readyAssist != null && readyAssist.Count != 0) //&& readyAssistContractSet)  
                {
                    PrintReadyAssistContracts(readyAssist);
                }
            }

        }

        protected void PrintWarrantyContracts(AxWebBrowser[] browsers, string accountNo, string customerID, int agreementNo, XmlNodeList contracts, ref int noPrints, int index, bool multiple)
        {
            object Zero = 0;
            object EmptyString = "";
            bool isIR = false;

            foreach (XmlNode c in contracts)
            {
                AccountManager.IsItemInstantReplacement(Convert.ToInt32(c.Attributes[Tags.ItemId].Value),
                                                        Convert.ToInt16(c.Attributes[Tags.Location].Value),
                                                        out isIR, out Error);
                if (Error.Length > 0)
                    ShowError(Error);

                string warrantyno = c.Attributes[Tags.Code].Value;
                //Regex reg = new Regex("^19.*2$");	/* ^ (starts with) 19 (literal) .(any character) *(zero or more) 2$ (ends with 2)	*/
                if (warrantyno.Length > 0)      //#18393

                //!reg.IsMatch(warrantyno) )		/* don't print if immediate replacement */
                //    &&  !isIR)
                {
                    browsers[index].TabIndex = noPrints++;
                    string url = Config.Url + "WWarrantyContract.aspx?" +
                        "customerID=" + customerID + "&" +
                        "accountNo=" + accountNo + "&" +
                        "culture=" + Config.Culture + "&" +
                        "countryCode=" + Config.CountryCode + "&" +
                        "agreementNo=" + agreementNo.ToString() + "&" +
                        "contractNo=" + c.Attributes[Tags.ContractNumber].Value + "&" +
                        "multiple=" + multiple.ToString() + "&" +
                        "warrantyType=" + c.Attributes[Tags.WarrantyType].Value;
                    browsers[index].Navigate(url, ref Zero, ref EmptyString, ref EmptyString, ref EmptyString);
                    index++;

                    // Audit the Warranty print if it is a re-print
                    AccountManager.AuditReprint(accountNo, agreementNo, DocumentType.Warranty);
                }
            }
        }

        //#13716 - CR12949
        protected void PrintReadyAssistContracts(XmlNodeList readyAssist)
        {
            StringBuilder sb = new StringBuilder();

            foreach (XmlNode r in readyAssist)
            {
                if (sb.Length > 0)
                {
                    sb.Append(',');
                }

                sb.Append(Convert.ToString(r.Attributes[Tags.LineItemId].Value));

            }

            LaunchWebBrowser(String.Format("Contracts/PrintContracts?lineItemIds={0}", sb.ToString()));

        }

        protected void PrintElectronicBankTransferSheet(string acctNo)
        {
            LaunchWebBrowser(String.Format("CashLoan/PrintElectronicBankTransferSheet?acctNo={0}", acctNo));
        }

        //#13716 - CR12949
        private void LaunchWebBrowser(string url)
        {
            var browser = CreateBrowserArray(1);
            object x = "";
            browser[0].Navigate(Config.Url + url, ref x, ref x, ref x, ref x);
        }

        private void PrintRFSummary(AxSHDocVw.AxWebBrowser b, string accountNo, string customerID,
            string accountType, ref int noPrints)
        {
            if (accountType == AT.ReadyFinance && (bool)Country[CountryParameterNames.NoRFSummary])
            {
                b.TabIndex = noPrints++;
                object Zero = 0;
                object EmptyString = "";
                string url = Config.Url + "WRFSummary.aspx?" +
                    "customerID=" + customerID + "&" +
                    "accountNo=" + accountNo + "&" +
                    "culture=" + Config.Culture + "&" +
                        "countryCode=" + Config.CountryCode + "&" +
                        "storeType=" + Config.StoreType;
                b.Navigate(url, ref Zero, ref EmptyString, ref EmptyString, ref EmptyString);
            }
        }

        private void PrintScheduleOfPayments(AxSHDocVw.AxWebBrowser b,
            string accountNo,
            string customerID,
            string accountType,
            ref int noPrints,
            bool spaPrint = false,      //UAT1012 jec
            bool printSchedule = false)        //UAT1012 jec
        {
            if (((bool)Country[CountryParameterNames.PrintScheduleOfPayments] && spaPrint == false)     //UAT1012 jec
                && AT.IsCreditType(accountType))
            {
                b.TabIndex = noPrints++;
                object Zero = 0;
                object EmptyString = "";
                string url = Config.Url + "WScheduleOfPayments.aspx?" +
                    CN.CustomerID + "=" + customerID + "&" +
                    CN.AccountNumber + "=" + accountNo + "&" +
                    CN.Culture + "=" + Config.Culture + "&" +
                    CN.AccountType + "=" + accountType + "&" +
                    "countryCode=" + Config.CountryCode;
                b.Navigate(url, ref Zero, ref EmptyString, ref EmptyString, ref EmptyString);
            }

            if (((bool)Country[CountryParameterNames.PrintSPAscheduleOfPayments] && spaPrint == true && printSchedule == true)    //UAT1012 jec
                && AT.IsCreditType(accountType))
            {
                b.TabIndex = noPrints++;
                object Zero = 0;
                object EmptyString = "";
                string url = Config.Url + "WScheduleOfPayments.aspx?" +
                    CN.CustomerID + "=" + customerID + "&" +
                    CN.AccountNumber + "=" + accountNo + "&" +
                    CN.Culture + "=" + Config.Culture + "&" +
                    CN.AccountType + "=" + accountType + "&" +
                    "countryCode=" + Config.CountryCode;
                b.Navigate(url, ref Zero, ref EmptyString, ref EmptyString, ref EmptyString);
            }
        }

        protected void PrintArrScheduleOfPayments(AxSHDocVw.AxWebBrowser b,
          string accountNo,
          string customerID,
          string accountType,
          ref int noPrints,
          char period,
          decimal arrangementAmount,
          int noOfInstalments,
          decimal instalmentAmount,
          decimal oddPaymentAmount,
          DateTime firstPaymentDate,
          int remainInstals,
          decimal highInstalAmount)
        {
            if ((bool)Country[CountryParameterNames.PrintSPAscheduleOfPayments] &&      //UAT1012 jec
                AT.IsCreditType(accountType))
            {
                b.TabIndex = noPrints++;
                object Zero = 0;
                object EmptyString = "";
                string url = Config.Url + "WScheduleOfArrangementPayments.aspx?" +
                    CN.CustomerID + "=" + customerID + "&" +
                    CN.AccountNumber + "=" + accountNo + "&" +
                    CN.Culture + "=" + Config.Culture + "&" +
                    CN.AccountType + "=" + accountType + "&" +
                    "p=" + period.ToString() + "&" +
                    "aa=" + arrangementAmount.ToString() + "&" +
                    "i=" + noOfInstalments.ToString() + "&" +
                    "ia=" + instalmentAmount.ToString() + "&" +
                    "fi=" + oddPaymentAmount.ToString() + "&" +
                    "fpd=" + firstPaymentDate.ToString() + "&" +
                    "ri=" + remainInstals.ToString() + "&" +
                    "hi=" + highInstalAmount.ToString() + "&" +
                    "countryCode=" + Config.CountryCode;
                b.Navigate(url, ref Zero, ref EmptyString, ref EmptyString, ref EmptyString);
            }
        }

        protected void PrintFoodLoss(AxSHDocVw.AxWebBrowser b,
          string accountNo,
          string customerID,
          string serviceRequestNo)
        {

            object Zero = 0;
            object EmptyString = "";
            string url = Config.Url + "WFoodLoss.aspx?" +
                CN.CustomerID + "=" + customerID + "&" +
                CN.AccountNumber + "=" + accountNo + "&" +
                CN.ServiceRequestNo + "=" + serviceRequestNo + "&" +
                CN.Culture + "=" + Config.Culture + "&" +
                CN.CountryCode + "=" + Config.CountryCode + "&" +
                "User=" + Credential.UserId.ToString();
            b.Navigate(url, ref Zero, ref EmptyString, ref EmptyString, ref EmptyString);

        }


        protected void PrintStatementOfAccount(AxSHDocVw.AxWebBrowser b,
            string accountNo,
            string customerID,
            string accountType)
        {
            object Zero = 0;
            object EmptyString = "";
            string url = Config.Url + "WStatementOfAccount.aspx?" +
                CN.CustomerID + "=" + customerID + "&" +
                CN.AccountNumber + "=" + accountNo + "&" +
                CN.Culture + "=" + Config.Culture + "&" +
                CN.AccountType + "=" + accountType + "&" +
                "countryCode=" + Config.CountryCode;
            b.Navigate(url, ref Zero, ref EmptyString, ref EmptyString, ref EmptyString);
        }

        protected void PrintContractNos(AxSHDocVw.AxWebBrowser b, decimal number)
        {
            object Zero = 0;
            object EmptyString = "";
            string url = Config.Url + "WContractNos.aspx?" +
                CN.BranchNo + "=" + Config.BranchCode + "&" +
                CN.ContractNo + "=" + number.ToString();
            b.Navigate(url, ref Zero, ref EmptyString, ref EmptyString, ref EmptyString);
        }

        protected void PrintAccountNos(AxSHDocVw.AxWebBrowser b, decimal number, string accountType)
        {
            object Zero = 0;
            object EmptyString = "";
            string url = Config.Url + "WAccountNos.aspx?" +
                CN.BranchNo + "=" + Config.BranchCode + "&" +
                CN.AccountType + "=" + accountType + "&" +
                CN.AccountNo + "=" + number.ToString();
            b.Navigate(url, ref Zero, ref EmptyString, ref EmptyString, ref EmptyString);
        }

        protected bool SlipPrinterOK(bool checkPaper)
        {
            bool status = false;

            while (true)
            {
                try
                {
                    //if (Config.UseThermalPrinter)
                    //    if (ReceiptPrinter_MSPOS.IsPrinterAvailable)
                    //        return true;

                    ((MainForm)this.FormRoot).statusBar1.Text = GetResource("M_SLIPCHECK");
                    ReceiptPrinter rp = new ReceiptPrinter(this);

                    rp.OpenPrinter();

                    if (checkPaper && rp.SlpEmpty)
                    {
                        rp.ClosePrinter();
                        ((MainForm)this.FormRoot).statusBar1.Text = GetResource("M_SLIPPAPEROUT");
                        DialogResult userRequest = ShowInfo("M_SLIPPAPER", MessageBoxButtons.AbortRetryIgnore);
                        if (userRequest == DialogResult.Abort)
                        {
                            status = false;
                            break;
                        }
                        else if (userRequest == DialogResult.Ignore)
                        {
                            status = false;
                            break;
                        }
                    }
                    else
                    {
                        rp.ClosePrinter();
                        ((MainForm)this.FormRoot).statusBar1.Text = GetResource("M_SLIPOK");
                        status = true;
                        break;
                    }
                }
                catch
                {
                    ((MainForm)this.FormRoot).statusBar1.Text = GetResource("M_SLIPNOCONNECT");
                    DialogResult userRequest = ShowInfo("M_SLIPCONNECT", MessageBoxButtons.AbortRetryIgnore);
                    if (userRequest == DialogResult.Abort)
                    {
                        status = false;
                        break;
                    }
                    else
                    {
                        if (userRequest == DialogResult.Ignore)
                        {
                            status = false;
                            break;
                        }
                    }
                }
            }
            //status = true; //remove remove !!!!
            return status;
        }

        protected void PrintCollectionNotes(DataGrid grid, bool printAll)
        {
            object Zero = 0;
            object EmptyString = "";
            string url = "";
            string queryString = "";
            string lastAcctNo = "";

            XmlDocument deliveryNotes = new XmlDocument();
            deliveryNotes.LoadXml("<COLLECTIONNOTES/>");

            DataView dv = (DataView)grid.DataSource;
            int count = dv.Count;

            ((MainForm)FormRoot).browsers = ((CommonForm)FormRoot).CreateBrowserArray(1);
            //AxSHDocVw.AxWebBrowser[] browsers = CreateBrowserArray(1);

            for (int i = count - 1; i >= 0; i--)
            {
                if (grid.IsSelected(i) || printAll)		/* create a DeliveryNoteRequest node for 
														 * each selected row	*/
                {
                    if (lastAcctNo.Length == 0 || lastAcctNo != (string)dv[i][CN.acctno])
                    {
                        XmlDocument d = new XmlDocument();
                        d.LoadXml(XMLTemplates.CollectionNoteRequestXML);
                        XmlNode dn = d.DocumentElement;

                        dn.SelectSingleNode("ACCTNO").InnerText = (string)dv[i][CN.acctno];
                        if (dv[i]["RetStockLocn"] == DBNull.Value)
                            dn.SelectSingleNode("BRANCHNO").InnerText = Convert.ToString(dv[i]["StockLocn"]);
                        else
                            dn.SelectSingleNode("BRANCHNO").InnerText = Convert.ToString(dv[i]["RetStockLocn"]);
                        dn.SelectSingleNode("USER").InnerText = Credential.UserId.ToString();
                        dn.SelectSingleNode("CUSTOMERID").InnerText = (string)dv[i]["CustID"];
                        dn.SelectSingleNode("BUFFNO").InnerText = Convert.ToString(dv[i][CN.BuffNo]);
                        dn.SelectSingleNode("COLLDATE").InnerText = ((DateTime)dv[i][CN.DatePlanDel]).ToString();
                        dn.SelectSingleNode("CULTURE").InnerText = Config.Culture;
                        dn.SelectSingleNode("DELADDRESS").InnerText = (string)dv[i]["DeliveryAddress"];

                        XmlNode li = dn.SelectSingleNode("LINEITEMS");
                        for (int j = count - 1; j >= 0; j--)
                        {
                            if ((string)dv[j][CN.acctno] == (string)dv[i][CN.acctno])
                            {
                                XmlDocument c = new XmlDocument();
                                c.LoadXml(XMLTemplates.CollectionItemRequestXML);
                                XmlNode ci = c.DocumentElement;

                                decimal price = Math.Abs(Convert.ToDecimal(dv[j]["Quantity"])) * Convert.ToDecimal(dv[j]["Price"]);

                                ci.SelectSingleNode("QUANTITY").InnerText = Convert.ToString(dv[j]["Quantity"]);
                                ci.SelectSingleNode("ITEMNO").InnerText = (string)dv[j]["ItemNo"];
                                ci.SelectSingleNode("DESC1").InnerText = (string)dv[j]["ItemDescr1"];
                                ci.SelectSingleNode("DESC2").InnerText = (string)dv[j]["ItemDescr2"];
                                ci.SelectSingleNode("PRICE").InnerText = price.ToString();
                                ci.SelectSingleNode("SUPPLIER").InnerText = (string)dv[j][CN.Supplier];  //IP - 09/02/10 - CR1048 (Ref:3.1.2) Merged - Malaysia Enhancements (CR1072)
                                ci.SelectSingleNode("NOTES").InnerText = (string)dv[j]["Notes"];
                                if (dv[j]["GRTNotes"] != DBNull.Value)
                                {
                                    ci.SelectSingleNode("GRTNOTES").InnerText = (string)dv[j]["GRTNotes"];
                                }
                                ci = d.ImportNode(ci, true);
                                li.AppendChild(ci);
                            }
                        }

                        dn = deliveryNotes.ImportNode(dn, true);
                        deliveryNotes.DocumentElement.AppendChild(dn);

                        lastAcctNo = (string)dv[i][CN.acctno];
                    }
                }
            }

            for (int i = count - 1; i >= 0; i--)
            {
                if (grid.IsSelected(i) || printAll)
                    dv[i][CN.acctno] = "";
            }

            for (int i = count - 1; i >= 0; i--)
            {
                if ((string)dv[i][CN.acctno] == "")
                    dv.Delete(i);
            }

            queryString = "collectionNotes=" + deliveryNotes.DocumentElement.OuterXml;
            queryString = queryString.Replace("&", "%26");
            queryString += "&countryCode=" + Config.CountryCode;
            url = Config.Url + "WCollectionNote.aspx";
            object postData = EncodePostData(queryString);
            object headers = PostHeader;
            ((MainForm)FormRoot).browsers[0].Navigate(url, ref Zero, ref EmptyString, ref postData, ref headers);
        }

        public void TaxInvoicePrint(DataView dv)
        {
            ((MainForm)FormRoot).browsers = ((CommonForm)FormRoot).CreateBrowserArray(1);

            foreach (DataRowView row in dv)
            {
                PrintTaxInvoice(((MainForm)FormRoot).browsers[0], row["AcctNo"].ToString());
            }
        }

        private void PrintTaxInvoice(AxSHDocVw.AxWebBrowser b, string accountNo)
        {

            object Zero = 0;
            object EmptyString = "";

            string queryString = "customerID=&" +
                 "acctNo=" + accountNo + "&" +
                 "accountType=&" +
                 "culture=" + Config.Culture + "&" +
                 "country=" + Config.CountryCode + "&" +
                 "branch=" + Config.BranchCode + "&" +
                 "buffno=&" +
                 "creditNote=false&" +
                  "multiple=false&" +
                 "user=" + Credential.UserId.ToString() + "&" +
                 "IsProofofPurchase=true";



            string url = Config.Url + "WTaxInvoice.aspx?" + queryString;
            b.Navigate(url, ref Zero, ref EmptyString, ref EmptyString, ref EmptyString);

        }

        public void PrintRFTerms(AxSHDocVw.AxWebBrowser b, string accountNo, string customerID, ref int noPrints)
        {
            if ((bool)Country[CountryParameterNames.NoRFDetails])
            {
                b.TabIndex = noPrints++;
                object Zero = 0;
                object EmptyString = "";
                string url = Config.Url + "WRFTerms.aspx?" +
                    CN.CustomerID + "=" + customerID + "&" +
                    CN.AccountNo + "=" + accountNo + "&" +
                    CN.Culture + "=" + Config.Culture + "&" +
                    "countryCode=" + Config.CountryCode + "&" +
                    "storeType=" + Config.StoreType;                                            //#19687 
                b.Navigate(url, ref Zero, ref EmptyString, ref EmptyString, ref EmptyString);
            }
        }

        protected void PrintCashierTotalsSummary(AxSHDocVw.AxWebBrowser b,
            short branchNo,
            DateTime dateFrom,
            DateTime dateTo)
        {
            object Zero = 0;
            object EmptyString = "";
            string url = Config.Url + "WCashierTotalsSummary.aspx?" +
                CN.BranchNo + "=" + branchNo.ToString() + "&" +
                CN.DateFrom + "=" + dateFrom.ToString() + "&" +
                CN.DateTo + "=" + dateTo.ToString() + "&" +
                CN.Culture + "=" + Config.Culture;
            b.Navigate(url, ref Zero, ref EmptyString, ref EmptyString, ref EmptyString);
        }

        protected void PrintCashierTotals(AxSHDocVw.AxWebBrowser b,
            bool subTotal,
            short branchNo,
            int employeeNo,
            string employeeName,
            DateTime dateFrom,
            DateTime dateTo,
            bool listCheques,
            int cashierID)
        {
            object Zero = 0;
            object EmptyString = "";
            string url = Config.Url + "WCashierTotals.aspx?" +
                "branchNo=" + branchNo.ToString() + "&" +
                "employeeNo=" + employeeNo.ToString() + "&" +
                "dateFrom=" + dateFrom.ToString() + "&" +
                "dateTo=" + dateTo.ToString() + "&" +
                "subTotal=" + subTotal.ToString() + "&" +
                "culture=" + Config.Culture + "&" +
                "listCheques=" + listCheques.ToString() + "&" +
                "employeeName=" + employeeName + "&" +
                "countryCode=" + Config.CountryCode + "&" +
                "cashierID=" + cashierID.ToString();
            b.Navigate(url, ref Zero, ref EmptyString, ref EmptyString, ref EmptyString);
        }

        private void PrintCashLoan(AxSHDocVw.AxWebBrowser b, string accountNo, int agreementNo, string accountType, string customerID, ref int noPrints, bool multiple)
        {
            if (accountType != AT.Cash)	/*don't print agreement for cash accounts */
            {
                //construct the query string from the datarow passed in
                b.TabIndex = noPrints++;
                object Zero = 0;
                object EmptyString = "";
                string url = Config.Url + "WCashLoan.aspx?" +
                    "customerID=" + customerID + "&" +
                    "accountNo=" + accountNo + "&" +
                    "accountType=" + accountType + "&" +
                    "culture=" + Config.Culture + "&" +
                    "countryCode=" + Config.CountryCode + "&" +
                    "multiple=" + multiple.ToString() + "&" +
                    "branch=" + Config.BranchCode.ToString();                   //#19425 - CR18938

                b.Navigate(url, ref Zero, ref EmptyString, ref EmptyString, ref EmptyString);

                // Audit the Agreement print if it is a re-print
                //AccountManager.AuditReprint(accountNo, agreementNo, DocumentType.CashLoan);
            }
        }

        public object EncodePostData(string queryString)
        {
            byte[] postBytes = System.Text.UTF8Encoding.UTF8.GetBytes(queryString);
            return (object)postBytes;
        }

        public void PrintAcctNoOnCheque(string accountNo)
        {
            ReceiptPrinter rp = null;
            try
            {
                Wait();
                rp = new ReceiptPrinter(this);
                rp.OpenPrinter();
                rp.Init();
                rp.LineSpacing = 15;
                rp.PrintString(accountNo + Environment.NewLine + DateTime.Now.ToString());
                rp.Release();
            }
            catch (SlipPrinterException ex)
            {
                if (ex.Message != "Cancel")
                    Catch((Exception)ex, Function);
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
            finally
            {
                StopWait();
                if (rp != null) rp.ClosePrinter();
            }
        }

        protected void AddColumnStyle(string columnName, DataGridTableStyle tabStyle,
            int width, bool readOnly, string headerText, string format,
            HorizontalAlignment alignment)
        {
            DataGridTextBoxColumn aColumnTextColumn = new DataGridTextBoxColumn();
            aColumnTextColumn.MappingName = columnName;
            aColumnTextColumn.Width = width;
            aColumnTextColumn.ReadOnly = readOnly;
            aColumnTextColumn.HeaderText = headerText;
            aColumnTextColumn.Format = format;
            aColumnTextColumn.Alignment = alignment;
            tabStyle.GridColumnStyles.Add(aColumnTextColumn);
        }

        protected void PrintJournalEnquiry(AxSHDocVw.AxWebBrowser b, DateTime dateFirst,
            DateTime dateLast, int firstRef, int lastRef, int empNo, int branch, int combination)
        {
            object Zero = 0;
            object EmptyString = "";
            string url = Config.Url + "WJournalEnquiry.aspx?" +
                "dateFirst=" + dateFirst.ToString() + "&" +
                "dateLast=" + dateLast.ToString() + "&" +
                "firstRef=" + firstRef.ToString() + "&" +
                "lastRef=" + lastRef.ToString() + "&" +
                "empNo=" + empNo.ToString() + "&" +
                "branch=" + branch.ToString() + "&" +
                "combination=" + combination.ToString() + "&" +
                "culture=" + Config.Culture + "&" +
                "countryCode=" + Config.CountryCode;

            b.Navigate(url, ref Zero, ref EmptyString, ref EmptyString, ref EmptyString);
        }

        //why the decimal places is a string?
        //is it like two instead of 2?
        [Browsable(false)]
        public string DecimalPlaces
        {
            get
            {
                if (Country == null)
                    return "";
                else
                    return (string)Country[CountryParameterNames.DecimalPlaces];
            }
        }

        //what is it going to format?
        [Browsable(false)]
        public string Format
        {
            get
            {
                if (Country == null)
                    return "";
                else
                {
                    string dp = (string)Country[CountryParameterNames.DecimalPlaces];
                    dp = dp.Remove(0, 1);
                    dp = dp.Insert(0, "F");
                    return dp;
                }
            }
        }

        /// <summary>
        /// To set multiple grid columns to be readonly we use a bitwise
        /// mask and set the bits which correspond to the column numbers
        /// we want to make readonly. 
        /// </summary>
        protected int SetReadOnlyMask(int columnNumber, int currentMask)
        {
            double binary = 2;
            double power = Convert.ToDouble(columnNumber - 1);
            int result = Convert.ToInt32(Math.Pow(binary, power));
            return currentMask | result;
        }

        protected int SetReadOnlyMask(int[] columnNumbers, int currentMask)
        {
            foreach (int n in columnNumbers)
            {
                currentMask = SetReadOnlyMask(n, currentMask);
            }
            return currentMask;
        }

        protected void MoneyField_Leave(object sender, System.EventArgs e)
        {
            try
            {
                Wait();

                ((TextBox)sender).Text = MoneyStrToDecimal(((TextBox)sender).Text).ToString(DecimalPlaces);
            }
            catch (Exception ex)
            {
                Catch(ex, "MoneyField_Leave");
            }
            finally
            {
                StopWait();
            }
        }

        /// <summary>
        /// Add the appropriate event handler to the passed control to detect the KeyPress
        /// event - used to detect unsaved changes on forms.
        /// </summary>
        /// <param name="singleControl">The single Control to add the KeyPressed event handler to</param>
        protected void AddKeyPressedEventHandler(Control singleControl)
        {
            if (singleControl is TextBox)
            {
                ((TextBox)singleControl).TextChanged += new System.EventHandler(this.CommonDataChanged);
            }
            if (singleControl is RadioButton)
            {
                ((RadioButton)singleControl).CheckedChanged += new System.EventHandler(this.CommonDataChanged);
            }
            if (singleControl is CheckBox)
            {
                ((CheckBox)singleControl).CheckedChanged += new System.EventHandler(this.CommonDataChanged);
            }
            if (singleControl is ComboBox)
            {
                ((ComboBox)singleControl).SelectionChangeCommitted += new System.EventHandler(this.CommonDataChanged);
            }
            if (singleControl is NumericUpDown)
            {
                ((NumericUpDown)singleControl).ValueChanged += new System.EventHandler(this.CommonDataChanged);
            }
            if (singleControl is ListBox)
            {
                ((ListBox)singleControl).SelectedValueChanged += new System.EventHandler(this.CommonDataChanged);
            }
            if (singleControl is DateTimePicker)
            {
                ((DateTimePicker)singleControl).ValueChanged += new System.EventHandler(this.CommonDataChanged);
            }
        }

        //Recursively set up event handlers to detect the KeyPress event for all
        //relevant controls on the form...
        protected void AddKeyPressedEventHandlers(Control control)
        {//are you kidding? what is all this code for? 
            foreach (Control c in control.Controls)
            {
                if (c is TextBox)
                {
                    ((TextBox)c).TextChanged += new System.EventHandler(this.CommonDataChanged);
                }
                if (c is RadioButton)
                {
                    ((RadioButton)c).CheckedChanged += new System.EventHandler(this.CommonDataChanged);
                }
                if (c is CheckBox)
                {
                    ((CheckBox)c).CheckedChanged += new System.EventHandler(this.CommonDataChanged);
                }
                if (c is ComboBox)
                {
                    ((ComboBox)c).SelectionChangeCommitted += new System.EventHandler(this.CommonDataChanged);
                }
                if (c is NumericUpDown)
                {
                    ((NumericUpDown)c).ValueChanged += new System.EventHandler(this.CommonDataChanged);
                }
                if (c is ListBox)
                {
                    ((ListBox)c).SelectedValueChanged += new System.EventHandler(this.CommonDataChanged);
                }
                if (c is DateTimePicker)
                {
                    ((DateTimePicker)c).ValueChanged += new System.EventHandler(this.CommonDataChanged);
                }

                AddKeyPressedEventHandlers(c);
            }
        }

        /// <summary>
        /// Sends relevant data to WPickList.aspx. Ammended to include courts branches CR903
        /// </summary>
        /// <param name="pickListNo"></param>
        /// <param name="additions"></param>
        /// <param name="reprint"></param>
        /// <param name="branchNo"></param>
        /// <param name="delNoteBranch"></param>
        /// <param name="orderPicklist"></param>
        /// <param name="courtsBranches"></param>
        protected void PrintPickList(int pickListNo, bool additions, bool reprint,
              short branchNo, short delNoteBranch, bool orderPicklist, string courtsBranches)
        {
            ((MainForm)FormRoot).browsers = ((CommonForm)FormRoot).CreateBrowserArray(1);

            string branchName = "";
            short isAddition = Convert.ToInt16(additions);
            short isReprint = Convert.ToInt16(reprint);
            short isOrderPicklist = Convert.ToInt16(orderPicklist);

            DataSet ds = AccountManager.GetBranchAddress(Convert.ToInt32(Config.BranchCode), 1, out Error);
            foreach (DataTable dt in ds.Tables)
                if (dt.TableName == TN.BranchDetails)
                    foreach (DataRow r in dt.Rows)
                        branchName = (string)r[CN.BranchName];

            object Zero = 0;
            object EmptyString = "";
            string url = Config.Url + "WPickList.aspx?" +
                "pickListNo=" + pickListNo.ToString() + "&" +
                "branchName=" + branchName + "&" +
                "branchNo=" + branchNo.ToString() + "&" +
                "delNoteBranch=" + delNoteBranch.ToString() + "&" +
                "empeeNo=" + Credential.UserId.ToString() + "&" +
                "isAddition=" + isAddition.ToString() + "&" +
                "isReprint=" + isReprint.ToString() + "&" +
                "isOrderPicklist=" + isOrderPicklist.ToString() + "&" +
                "culture=" + Config.Culture + "&" +
                   "countryCode=" + Config.CountryCode + "&" +
                   "courtsBranches=" + courtsBranches;

            ((MainForm)FormRoot).browsers[0].Navigate(url, ref Zero, ref EmptyString, ref EmptyString, ref EmptyString);
        }

        public void LaunchHelp(string fileName)
        {
            string helpPath = HelpFileUrl + fileName;
            string localPath = Application.StartupPath + "\\" + fileName;

            // Reload the help if it is over a day old
            FileInfo localFile = new FileInfo(localPath);
            if (localFile.LastWriteTime < DateTime.Today)


                Help.ShowHelp(this, helpPath);
            //Help.ShowHelp(this, localPath);
        }


        public bool LoadServerIcon(string fileName, out string localPath)
        {
            string serverPath = IconFileUrl + fileName;
            localPath = Application.StartupPath + "\\" + fileName;

            try
            {
                // Reload the icon if it is over a day old
                FileInfo localFile = new FileInfo(localPath);
                if (localFile.LastWriteTime < DateTime.Today)
                {
                    string myStringWebResource = null;
                    // Create a new WebClient instance.
                    WebClient myWebClient = new WebClient();
                    myStringWebResource = serverPath;
                    // Download the Web resource and save it into the current filesystem folder.
                    myWebClient.DownloadFile(myStringWebResource, fileName);
                }
            }
            catch
            {
                // URL not found because Icon not available on the server
            }

            return File.Exists(localPath);
        }
        /// <summary>
        /// Downloads photo or image of customer signature. Returns whether files exists. Wants "p" for photo else signature
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="fileType"></param>
        /// <param name="localPath"></param>
        /// <returns></returns>
        public bool LoadPhoto(string fileName, string fileType, out string localPath)
        {
            DTransfer Trans = new DTransfer();
            //            string fullpath 

            string serverPath = String.Empty;
            if (fileType == "p")
            {
                serverPath = (string)Country[CountryParameterNames.PhotoDirectory];
            }
            else
            {
                serverPath = (string)Country[CountryParameterNames.SignatureDirectory];
            }
            localPath = Application.StartupPath + "\\" + fileName;

            //try
            //{
            FileInfo localFile = new FileInfo(localPath);
            // Remove file
            if (File.Exists(localFile.FullName))
            {
                File.Delete(localFile.FullName);
            }
            // Create a new WebClient instance.
            WebClient myWebClient = new WebClient();
            var myStringWebResource = serverPath;
            //WUpdater WS = new WUpdater(true);
            //DTransfer Transfer = new DTransfer();
            // Download the photograph and save it into the current filesystem folder.
            if (serverPath != String.Empty)
            {
                if (!File.Exists(localFile.FullName))
                {
                    try
                    {

                        //WS.DownloadFile(serverPath);
                        myWebClient.DownloadFile(myStringWebResource + "/" + fileName, localFile.FullName);
                        // Transfer.WriteBinarFile(WS.DownloadFile(serverPath + @"\" + fileName), localFile.FullName);

                    }
                    catch
                    {
                        return false;
                    }
                }
            }
            else
            {
                throw new STLException(GetResource("M_INVALIDPHOTODIRECTORY"));
            }
            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //    //throw new STLException(GetResource("M_INVALIDPHOTODIRECTORY"));
            //}

            return File.Exists(localPath);
        }
        //todo fix this ... //fix what? the only possible fix is delete EVERYTHING
        public void UploadPhoto(string fileName, string path, string fileType)
        {
            string serverPath = String.Empty;
            if (fileType == "p")
            {
                serverPath = (string)Country[CountryParameterNames.PhotoDirectory];
            }
            else
            {
                serverPath = (string)Country[CountryParameterNames.SignatureDirectory];
            }

            string localPath = path;

            try
            {
                FileInfo localFile = new FileInfo(localPath);

                // Create a new WebClient instance.
                WebClient wc = new WebClient();

                // Upload the photograph and save it into the server directory.
                if (serverPath != String.Empty)
                {
                    FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);

                    byte[] picture = new byte[fs.Length];

                    fs.Read(picture, 0, Convert.ToInt32(fs.Length));

                    fs.Close();

                    NetworkCredential cred = new NetworkCredential();

                    wc.Credentials = CredentialCache.DefaultCredentials;
                    wc.UploadData(serverPath + "/" + fileName, "PUT", picture);
                }
                else
                {
                    throw new STLException(GetResource("M_INVALIDPHOTODIRECTORY"));
                }

            }
            catch (Exception ex)
            {
                // URL not found
                throw ex;
            }

            //return File.Exists(localPath);
        }

        protected void PrintDeliverySchedule(AxSHDocVw.AxWebBrowser b, DataGrid grid)
        {
            object Zero = 0;
            object EmptyString = "";
            string url = "";
            string queryString = "";
            XmlDocument deliverySchedules = new XmlDocument();
            deliverySchedules.LoadXml("<DELIVERYSCHEDULES/>");

            DataView dv = (DataView)grid.DataSource;
            int count = dv.Count;

            for (int i = count - 1; i >= 0; i--)
            {
                // create a DeliveryNoteRequest node for each selected row
                if (grid.IsSelected(i))
                {
                    XmlDocument d = new XmlDocument();
                    d.LoadXml(XMLTemplates.DeliveryScheduleRequest);
                    XmlNode ds = d.DocumentElement;

                    ds.SelectSingleNode("LOADNO").InnerText = Convert.ToString(dv[i][CN.LoadNo]);
                    ds.SelectSingleNode("BRANCHNO").InnerText = Convert.ToString(dv[i][CN.BranchNo]);
                    ds.SelectSingleNode("DATEDEL").InnerText = ((DateTime)dv[i][CN.DateDel]).ToString();
                    ds.SelectSingleNode("TRUCKID").InnerText = (string)dv[i][CN.TruckID];
                    if (dv[i][CN.CarrierNumber].ToString().Trim() != "")
                        ds.SelectSingleNode("TRUCKID").InnerText += " (" + dv[i][CN.CarrierNumber].ToString() + ")";
                    ds.SelectSingleNode("DRIVERNAME").InnerText = (string)dv[i][CN.DriverName];
                    ds.SelectSingleNode("PRINTED").InnerText = Convert.ToString(dv[i][CN.Printed]);
                    ds.SelectSingleNode("CULTURE").InnerText = Config.Culture;

                    ds = deliverySchedules.ImportNode(ds, true);
                    deliverySchedules.DocumentElement.AppendChild(ds);
                }
            }

            if (deliverySchedules.DocumentElement.ChildNodes.Count > 0)
            {
                queryString = "deliverySchedules=" + deliverySchedules.DocumentElement.OuterXml;
                queryString = queryString.Replace("&", "%26");
                queryString += "&countryCode=" + Config.CountryCode;
                url = Config.Url + "WDeliverySchedule.aspx";
                object postData = EncodePostData(queryString);
                object headers = PostHeader;
                b.Navigate(url, ref Zero, ref EmptyString, ref postData, ref headers);
            }
        }

        private void SetCulture()
        {
            try
            {
                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(Config.Culture);
            }
            catch
            {
                if (Config.Culture.ToLower() == "en-cb")
                    Config.Culture = "en-029";
                else if (Config.Culture.ToLower() == "en-029")
                    Config.Culture = "en-CB";

                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(Config.Culture);
            }
        }

        protected void PrintDeliveryDocuments(DataGrid grid)
        {
            int noBrowsers = 2;

            ((MainForm)FormRoot).browsers = ((CommonForm)FormRoot).CreateBrowserArray(noBrowsers);

            noBrowsers = 0;
            PrintDeliverySchedule(((MainForm)FormRoot).browsers[noBrowsers++], grid);
            PrintDelNotesOnSchedule(((MainForm)FormRoot).browsers[noBrowsers++], grid);
        }
        protected void PrintRebateReport(DataGrid grid)
        {
            object Zero = 0;
            object EmptyString = "";
            string url = "";
            string queryString = "";

            ((MainForm)FormRoot).browsers = ((CommonForm)FormRoot).CreateBrowserArray(1);

            XmlDocument wholeThing = CreateReportADoc(grid);

            queryString = "rebateReport=" + wholeThing.DocumentElement.OuterXml;
            queryString = queryString.Replace("&", "%26");
            queryString += "&countryCode=" + Config.CountryCode;
            url = Config.Url + "WRebateForecast.aspx";
            object postData = EncodePostData(queryString);
            object headers = PostHeader;
            ((MainForm)FormRoot).browsers[0].Navigate(url, ref Zero, ref EmptyString, ref postData, ref headers);
        }
        protected void PrintRebateTab2(DataGrid grid)
        {
            object Zero = 0;
            object EmptyString = "";
            string url = "";
            string queryString = "";

            ((MainForm)FormRoot).browsers = ((CommonForm)FormRoot).CreateBrowserArray(1);

            XmlDocument wholeThing = CreateReportCDoc(grid);

            queryString = "rebateReport=" + wholeThing.DocumentElement.OuterXml;
            queryString = queryString.Replace("&", "%26");
            queryString += "&countryCode=" + Config.CountryCode;
            url = Config.Url + "WRebateForecast2.aspx";
            object postData = EncodePostData(queryString);
            object headers = PostHeader;
            ((MainForm)FormRoot).browsers[0].Navigate(url, ref Zero, ref EmptyString, ref postData, ref headers);
        }
        private XmlDocument CreateReportADoc(DataGrid grid)
        {
            XmlDocument doc = new XmlDocument();

            doc.LoadXml("<REBATEFORECAST/>");

            XmlNode culture = doc.CreateElement("CULTURE");
            culture.InnerText = Config.Culture;
            doc.DocumentElement.AppendChild(culture);

            XmlNode title = doc.CreateElement("TITLE");
            doc.DocumentElement.AppendChild(title);
            title.InnerText = grid.CaptionText;

            XmlNode header = doc.CreateElement("HEADER");
            doc.DocumentElement.AppendChild(header);

            XmlNode level = doc.CreateElement("LEVEL");
            XmlNode p1 = doc.CreateElement("P1");
            XmlNode p2 = doc.CreateElement("P2");
            XmlNode p3 = doc.CreateElement("P3");
            XmlNode p4 = doc.CreateElement("P4");
            XmlNode p5 = doc.CreateElement("P5");
            XmlNode p6 = doc.CreateElement("P6");
            XmlNode p7 = doc.CreateElement("P7");
            XmlNode p8 = doc.CreateElement("P8");
            XmlNode p9 = doc.CreateElement("P9");
            XmlNode p10 = doc.CreateElement("P10");
            XmlNode p11 = doc.CreateElement("P11");
            XmlNode p12 = doc.CreateElement("P12");

            level.InnerText = "Arrears Level";
            p1.InnerText = grid.TableStyles[0].GridColumnStyles[CN.P1].HeaderText;
            p2.InnerText = grid.TableStyles[0].GridColumnStyles[CN.P2].HeaderText;
            p3.InnerText = grid.TableStyles[0].GridColumnStyles[CN.P3].HeaderText;
            p4.InnerText = grid.TableStyles[0].GridColumnStyles[CN.P4].HeaderText;
            p5.InnerText = grid.TableStyles[0].GridColumnStyles[CN.P5].HeaderText;
            p6.InnerText = grid.TableStyles[0].GridColumnStyles[CN.P6].HeaderText;
            p7.InnerText = grid.TableStyles[0].GridColumnStyles[CN.P7].HeaderText;
            p8.InnerText = grid.TableStyles[0].GridColumnStyles[CN.P8].HeaderText;
            p9.InnerText = grid.TableStyles[0].GridColumnStyles[CN.P9].HeaderText;
            p10.InnerText = grid.TableStyles[0].GridColumnStyles[CN.P10].HeaderText;
            p11.InnerText = grid.TableStyles[0].GridColumnStyles[CN.P11].HeaderText;
            p12.InnerText = grid.TableStyles[0].GridColumnStyles[CN.P12].HeaderText;

            header.AppendChild(level);
            header.AppendChild(p1);
            header.AppendChild(p2);
            header.AppendChild(p3);
            header.AppendChild(p4);
            header.AppendChild(p5);
            header.AppendChild(p6);
            header.AppendChild(p7);
            header.AppendChild(p8);
            header.AppendChild(p9);
            header.AppendChild(p10);
            header.AppendChild(p11);
            header.AppendChild(p12);

            XmlNode data = doc.CreateElement("DATA");
            doc.DocumentElement.AppendChild(data);
            XmlNode footer = doc.CreateElement("FOOTER");
            doc.DocumentElement.AppendChild(footer);

            DataView dv = (DataView)grid.DataSource;

            for (int i = 0; i < dv.Count; i++)
            {
                XmlNode row = doc.CreateElement("ROW");
                data.AppendChild(row);

                string levelStr = (string)dv[i][CN.ArrearsLevel];

                level = doc.CreateElement("LEVEL");
                p1 = doc.CreateElement("P1");
                p2 = doc.CreateElement("P2");
                p3 = doc.CreateElement("P3");
                p4 = doc.CreateElement("P4");
                p5 = doc.CreateElement("P5");
                p6 = doc.CreateElement("P6");
                p7 = doc.CreateElement("P7");
                p8 = doc.CreateElement("P8");
                p9 = doc.CreateElement("P9");
                p10 = doc.CreateElement("P10");
                p11 = doc.CreateElement("P11");
                p12 = doc.CreateElement("P12");

                level.InnerText = (string)dv[i][CN.ArrearsLevel];
                p1.InnerText = Convert.ToDecimal(dv[i][CN.P1]).ToString(DecimalPlaces);
                p2.InnerText = Convert.ToDecimal(dv[i][CN.P2]).ToString(DecimalPlaces);
                p3.InnerText = Convert.ToDecimal(dv[i][CN.P3]).ToString(DecimalPlaces);
                p4.InnerText = Convert.ToDecimal(dv[i][CN.P4]).ToString(DecimalPlaces);
                p5.InnerText = Convert.ToDecimal(dv[i][CN.P5]).ToString(DecimalPlaces);
                p6.InnerText = Convert.ToDecimal(dv[i][CN.P6]).ToString(DecimalPlaces);
                p7.InnerText = Convert.ToDecimal(dv[i][CN.P7]).ToString(DecimalPlaces);
                p8.InnerText = Convert.ToDecimal(dv[i][CN.P8]).ToString(DecimalPlaces);
                p9.InnerText = Convert.ToDecimal(dv[i][CN.P9]).ToString(DecimalPlaces);
                p10.InnerText = Convert.ToDecimal(dv[i][CN.P10]).ToString(DecimalPlaces);
                p11.InnerText = Convert.ToDecimal(dv[i][CN.P11]).ToString(DecimalPlaces);
                p12.InnerText = Convert.ToDecimal(dv[i][CN.P12]).ToString(DecimalPlaces);

                if (levelStr != "TOTALS")
                {
                    row.AppendChild(level);
                    row.AppendChild(p1);
                    row.AppendChild(p2);
                    row.AppendChild(p3);
                    row.AppendChild(p4);
                    row.AppendChild(p5);
                    row.AppendChild(p6);
                    row.AppendChild(p7);
                    row.AppendChild(p8);
                    row.AppendChild(p9);
                    row.AppendChild(p10);
                    row.AppendChild(p11);
                    row.AppendChild(p12);
                }

                if (levelStr == "TOTALS")
                {
                    footer.AppendChild(level);
                    footer.AppendChild(p1);
                    footer.AppendChild(p2);
                    footer.AppendChild(p3);
                    footer.AppendChild(p4);
                    footer.AppendChild(p5);
                    footer.AppendChild(p6);
                    footer.AppendChild(p7);
                    footer.AppendChild(p8);
                    footer.AppendChild(p9);
                    footer.AppendChild(p10);
                    footer.AppendChild(p11);
                    footer.AppendChild(p12);
                }
            }
            return doc;
        }

        private XmlDocument CreateReportCDoc(DataGrid grid)
        {
            XmlDocument doc = new XmlDocument();

            doc.LoadXml("<REBATEFORECAST/>");

            XmlNode culture = doc.CreateElement("CULTURE");
            culture.InnerText = Config.Culture;
            doc.DocumentElement.AppendChild(culture);

            XmlNode title = doc.CreateElement("TITLE");
            doc.DocumentElement.AppendChild(title);
            title.InnerText = grid.CaptionText;

            XmlNode header = doc.CreateElement("HEADER");
            doc.DocumentElement.AppendChild(header);

            XmlNode col1 = doc.CreateElement("COL1");
            XmlNode col2 = doc.CreateElement("COL2");
            XmlNode col3 = doc.CreateElement("COL3");
            XmlNode col4 = doc.CreateElement("COL4");
            XmlNode col5 = doc.CreateElement("COL5");
            XmlNode col6 = doc.CreateElement("COL6");
            XmlNode col7 = doc.CreateElement("COL7");
            XmlNode col8 = doc.CreateElement("COL8");
            XmlNode col9 = doc.CreateElement("COL9");
            XmlNode col10 = doc.CreateElement("COL10");
            XmlNode col11 = doc.CreateElement("COL11");

            col1.InnerText = grid.TableStyles[0].GridColumnStyles[CN.PeriodEnd].HeaderText;
            col2.InnerText = grid.TableStyles[0].GridColumnStyles[CN.Forecast].HeaderText;
            col3.InnerText = grid.TableStyles[0].GridColumnStyles[CN.Revised].HeaderText;
            col4.InnerText = grid.TableStyles[0].GridColumnStyles[CN.DueDate].HeaderText;
            col5.InnerText = grid.TableStyles[0].GridColumnStyles[CN.AgreementDueDate].HeaderText;
            col6.InnerText = grid.TableStyles[0].GridColumnStyles[CN.Settled].HeaderText;
            col7.InnerText = grid.TableStyles[0].GridColumnStyles[CN.OutstBal].HeaderText;
            col8.InnerText = grid.TableStyles[0].GridColumnStyles[CN.DateLastChanged].HeaderText;
            col9.InnerText = grid.TableStyles[0].GridColumnStyles[CN.Threshold].HeaderText;
            col10.InnerText = grid.TableStyles[0].GridColumnStyles[CN.Unaccounted].HeaderText;
            col11.InnerText = grid.TableStyles[0].GridColumnStyles[CN.Actual].HeaderText;

            header.AppendChild(col1);
            header.AppendChild(col2);
            header.AppendChild(col3);
            header.AppendChild(col4);
            header.AppendChild(col5);
            header.AppendChild(col6);
            header.AppendChild(col7);
            header.AppendChild(col8);
            header.AppendChild(col9);
            header.AppendChild(col10);
            header.AppendChild(col11);

            XmlNode data = doc.CreateElement("DATA");
            doc.DocumentElement.AppendChild(data);

            DataView dv = (DataView)grid.DataSource;

            for (int i = 0; i < dv.Count; i++)
            {
                XmlNode row = doc.CreateElement("ROW");
                data.AppendChild(row);

                col1 = doc.CreateElement("COL1");
                col2 = doc.CreateElement("COL2");
                col3 = doc.CreateElement("COL3");
                col4 = doc.CreateElement("COL4");
                col5 = doc.CreateElement("COL5");
                col6 = doc.CreateElement("COL6");
                col7 = doc.CreateElement("COL7");
                col8 = doc.CreateElement("COL8");
                col9 = doc.CreateElement("COL9");
                col10 = doc.CreateElement("COL10");
                col11 = doc.CreateElement("COL11");

                col1.InnerText = Convert.ToDateTime(dv[i][CN.PeriodEnd]).ToString("dd/MM/yyy");
                col2.InnerText = Convert.ToDecimal(dv[i][CN.Forecast]).ToString(DecimalPlaces);
                col3.InnerText = Convert.ToDecimal(dv[i][CN.Revised]).ToString(DecimalPlaces);
                col4.InnerText = Convert.ToDecimal(dv[i][CN.DueDate]).ToString(DecimalPlaces);
                col5.InnerText = Convert.ToDecimal(dv[i][CN.AgreementDueDate]).ToString(DecimalPlaces);
                col6.InnerText = Convert.ToDecimal(dv[i][CN.Settled]).ToString(DecimalPlaces);
                col7.InnerText = Convert.ToDecimal(dv[i][CN.OutstBal]).ToString(DecimalPlaces);
                col8.InnerText = Convert.ToDecimal(dv[i][CN.DateLastChanged]).ToString(DecimalPlaces);
                col9.InnerText = Convert.ToDecimal(dv[i][CN.Threshold]).ToString(DecimalPlaces);
                col10.InnerText = Convert.ToDecimal(dv[i][CN.Unaccounted]).ToString(DecimalPlaces);
                col11.InnerText = Convert.ToDecimal(dv[i][CN.Actual]).ToString(DecimalPlaces);

                row.AppendChild(col1);
                row.AppendChild(col2);
                row.AppendChild(col3);
                row.AppendChild(col4);
                row.AppendChild(col5);
                row.AppendChild(col6);
                row.AppendChild(col7);
                row.AppendChild(col8);
                row.AppendChild(col9);
                row.AppendChild(col10);
                row.AppendChild(col11);
            }
            return doc;
        }

        protected void PrintBailiffCommissionTransactions(DataGrid grid, int empeeNo,
            string empeeName)
        {
            object Zero = 0;
            object EmptyString = "";
            string url = "";
            string queryString = "";

            XmlDocument doc = null;

            doc = LoadDocument(grid, empeeNo, empeeName);

            ((MainForm)FormRoot).browsers = ((CommonForm)FormRoot).CreateBrowserArray(1);

            queryString = "commissionTransactions=" + doc.DocumentElement.OuterXml;
            queryString = queryString.Replace("&", "%26");
            queryString += "&countryCode=" + Config.CountryCode;
            url = Config.Url + "WCommissionTransactions.aspx";
            object postData = EncodePostData(queryString);
            object headers = PostHeader;
            ((MainForm)FormRoot).browsers[0].Navigate(url, ref Zero, ref EmptyString, ref postData, ref headers);
        }

        protected void PrintCommissionPayment(DataGrid staffGrid, DataGrid transactionGrid,
            int empeeNo, string empeeName, decimal commValue)
        {
            object Zero = 0;
            object EmptyString = "";
            string url = "";
            string queryString = "";

            XmlDocument doc = null;

            doc = LoadDocument(transactionGrid, empeeNo, empeeName);

            XmlNode footer = doc.CreateElement("FOOTER");
            doc.DocumentElement.AppendChild(footer);

            XmlNode empName = doc.CreateElement("EMPNAME");
            XmlNode valueNode = doc.CreateElement("COMMNVALUE");
            XmlNode user = doc.CreateElement("USER");

            empName.InnerText = empeeName;
            //valueNode.InnerText = commValue.ToString();
            valueNode.InnerText = commValue.ToString(DecimalPlaces); //IP - 06/05/08 - v5.1 UAT(330) 
            user.InnerText = Credential.Name;

            footer.AppendChild(empName);
            footer.AppendChild(valueNode);
            footer.AppendChild(user);

            DataView dv = (DataView)staffGrid.DataSource;
            int count = dv.Count;

            for (int i = count - 1; i >= 0; i--)
            {
                if (staffGrid.IsSelected(i))
                    dv[i][CN.EmployeeName] = "";
            }

            for (int i = count - 1; i >= 0; i--)
            {
                if ((string)dv[i][CN.EmployeeName] == "")
                {
                    dv.Delete(i);
                }
            }

            ((MainForm)FormRoot).browsers = ((CommonForm)FormRoot).CreateBrowserArray(1);

            queryString = "commissionPayment=" + doc.DocumentElement.OuterXml;
            queryString = queryString.Replace("&", "%26");
            queryString += "&countryCode=" + Config.CountryCode;
            queryString += "&rePrint=" + (0).ToString();
            url = Config.Url + "WCommissionPayment.aspx";
            object postData = EncodePostData(queryString);
            object headers = PostHeader;
            ((MainForm)FormRoot).browsers[0].Navigate(url, ref Zero, ref EmptyString, ref postData, ref headers);
        }

        private XmlDocument LoadDocument(DataGrid grid, int empeeNo, string empeeName)
        {
            int numTrans = 0;
            int transPerPage = 35;

            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<COMMISSION/>");

            XmlNode header = doc.CreateElement("HEADER");
            doc.DocumentElement.AppendChild(header);
            XmlNode bailiff = doc.CreateElement("BAILIFF");
            XmlNode datePrinted = doc.CreateElement("DATE");
            bailiff.InnerText = empeeName + " (" + empeeNo.ToString() + ")";
            datePrinted.InnerText = DateTime.Now.ToLongDateString();

            header.AppendChild(bailiff);
            header.AppendChild(datePrinted);

            DataView dv = (DataView)grid.DataSource;
            int count = dv.Count;

            XmlNode transactions = doc.CreateElement("TRANSACTIONS");
            doc.DocumentElement.AppendChild(transactions);

            for (int i = count - 1; i >= 0; i--)
            {
                numTrans++;
                XmlNode transaction = doc.CreateElement("TRANSACTION");
                transactions.AppendChild(transaction);

                XmlNode acctNo = doc.CreateElement("ACCTNO");
                XmlNode dateTrans = doc.CreateElement("DATETRANS");
                XmlNode commValue = doc.CreateElement("VALUE");
                XmlNode cheque = doc.CreateElement("CHEQUE");
                XmlNode status = doc.CreateElement("STATUS");
                XmlNode type = doc.CreateElement("TYPE");
                XmlNode amount = doc.CreateElement("AMOUNT");

                DateTime dt = Convert.ToDateTime(dv[i][CN.DateTrans]);

                acctNo.InnerText = (string)dv[i][CN.AcctNo];
                dateTrans.InnerText = (Convert.ToDateTime(dv[i][CN.DateTrans])).ToShortDateString();
                commValue.InnerText = Convert.ToDecimal(dv[i][CN.TransValue]).ToString(DecimalPlaces);
                cheque.InnerText = (string)dv[i][CN.ChequeColln];
                status.InnerText = (string)dv[i][CN.Status];
                type.InnerText = (string)dv[i][CN.Code];
                amount.InnerText = Convert.ToDecimal(dv[i][CN.ActionValue]).ToString(DecimalPlaces);

                transaction.AppendChild(acctNo);
                transaction.AppendChild(dateTrans);
                transaction.AppendChild(commValue);
                transaction.AppendChild(cheque);
                transaction.AppendChild(status);
                transaction.AppendChild(type);
                transaction.AppendChild(amount);

                if (numTrans == transPerPage)
                {
                    XmlNode pb = doc.CreateElement("PB");
                    transaction.AppendChild(pb);
                    numTrans = 0;
                }
            }

            return doc;
        }

        protected void RePrintBailiffCommission(DataGrid grid)
        {
            object Zero = 0;
            object EmptyString = "";
            string url = "";
            string queryString = "";
            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<COMMISSIONS/>");

            DataView dv = (DataView)grid.DataSource;

            for (int i = dv.Count - 1; i >= 0; i--)
            {
                if (grid.IsSelected(i) && Convert.ToDecimal(dv[i][CN.LstCommn]) > 0)
                {
                    XmlNode commission = doc.CreateElement("COMMISSION");
                    doc.DocumentElement.AppendChild(commission);

                    XmlNode last = doc.CreateElement("LAST");
                    commission.AppendChild(last);

                    XmlNode details = doc.CreateElement("DETAILS");
                    commission.AppendChild(details);

                    XmlNode bailiff = doc.CreateElement("BAILIFF");
                    XmlNode amount = doc.CreateElement("AMOUNT");
                    XmlNode empName = doc.CreateElement("USER");

                    bailiff.InnerText = (string)dv[i][CN.EmployeeName] + " (" + Convert.ToInt32(dv[i][CN.EmployeeNo]).ToString() + ")";
                    amount.InnerText = Convert.ToDecimal(dv[i][CN.LstCommn]).ToString(DecimalPlaces); ;
                    empName.InnerText = Credential.Name;

                    details.AppendChild(bailiff);
                    details.AppendChild(amount);
                    details.AppendChild(empName);
                }
            }

            for (int i = dv.Count - 1; i >= 0; i--)
            {
                if (grid.IsSelected(i) && Convert.ToDecimal(dv[i][CN.LstCommn]) > 0)
                    dv[i][CN.EmployeeName] = "";
            }

            for (int i = dv.Count - 1; i >= 0; i--)
            {
                if ((string)dv[i][CN.EmployeeName] == "")
                {
                    dv.Delete(i);
                }
            }

            if (doc.DocumentElement.ChildNodes.Count > 0)
            {
                ((MainForm)FormRoot).browsers = ((CommonForm)FormRoot).CreateBrowserArray(1);

                queryString = "commissionPayment=" + doc.DocumentElement.OuterXml;
                queryString = queryString.Replace("&", "%26");
                queryString += "&countryCode=" + Config.CountryCode;
                queryString += "&rePrint=" + (1).ToString();
                url = Config.Url + "WCommissionPayment.aspx";
                object postData = EncodePostData(queryString);
                object headers = PostHeader;
                ((MainForm)FormRoot).browsers[0].Navigate(url, ref Zero, ref EmptyString, ref postData, ref headers);
            }
        }

        public void PrintPrizeVouchers(string accountNo, decimal cashPrice, int buffNo, DateTime dateIssued,
                                        bool reprint, bool additional)
        {
            if (!reprint && !additional)
            {
                CustomerManager.IssuePrizeVouchers(accountNo, cashPrice, buffNo, out dateIssued, out Error);
                if (Error.Length > 0)
                    ShowError(Error);
            }

            if (dateIssued != Date.blankDate)
            {
                ((MainForm)FormRoot).browsers = ((CommonForm)FormRoot).CreateBrowserArray(1);
                ((MainForm)FormRoot).browsers[0].Tag = true;

                object Zero = 0;
                object EmptyString = "";
                string url = Config.Url + "WPrizeVoucher.aspx?" +
                    "acctNo" + "=" + accountNo + "&" +
                    "dateIssued" + "=" + dateIssued.ToString() + "&" +
                    "buffNo" + "=" + buffNo.ToString() + "&" +
                    "reprint" + "=" + Convert.ToInt16(reprint).ToString() + "&" +
                    "additional" + "=" + Convert.ToInt16(reprint).ToString() + "&" +
                    "culture" + "=" + Config.Culture + "&" +
                    "countryCode=" + Config.CountryCode;

                ((MainForm)FormRoot).browsers[0].Navigate(url, ref Zero, ref EmptyString, ref EmptyString, ref EmptyString);
            }
        }

        protected bool ValidateMandatoryText(ErrorProvider ep, params TextBox[] tb)
        {
            bool ret = true;
            foreach (TextBox t in tb)
            {
                if (t.Text.Trim().Equals(""))
                {
                    ep.SetError(t, GetResource("M_ENTERMANDATORY"));
                    ret = false;
                    break;
                }
            }
            return ret;
        }

        protected string FullServiceStatusText(string srStatus)
        {
            string fullStatusText;
            switch (srStatus)
            {
                case ServiceStatus.Allocation:
                    fullStatusText = ServiceStatusText.Allocation;
                    break;
                case ServiceStatus.BERReplacement:
                    fullStatusText = ServiceStatusText.BERReplacement;
                    break;
                case ServiceStatus.Closed:
                    fullStatusText = ServiceStatusText.Closed;
                    break;
                case ServiceStatus.CommentUpdate:
                    fullStatusText = ServiceStatusText.CommentUpdate;
                    break;
                case ServiceStatus.Deposit:
                    fullStatusText = ServiceStatusText.Deposit;
                    break;
                case ServiceStatus.Estimate:
                    fullStatusText = ServiceStatusText.Estimate;
                    break;
                case ServiceStatus.New:
                    fullStatusText = ServiceStatusText.New;
                    break;
                case ServiceStatus.Resolution:
                    fullStatusText = ServiceStatusText.Resolution;
                    break;
                //CR 949/958 New status 'To Be Allocated'
                case ServiceStatus.ToBeAllocated:
                    fullStatusText = ServiceStatusText.ToBeAllocated;
                    break;
                case ServiceStatus.TechnicianAllocated: //CR 1024 (NM 23/04/2009)
                    fullStatusText = ServiceStatusText.TechnicianAllocated;
                    break;
                case ServiceStatus.AllocatedToSupplier: //CR 1024 (NM 23/04/2009)
                    fullStatusText = ServiceStatusText.AllocatedToSupplier;
                    break;
                default:
                    //fullStatusText = String.Empty;
                    fullStatusText = srStatus;      //CR1030 jec 24/01/11 default to input value (srStatus sometimes passed as ServiceStatusText.value not ServiceStatus.value)
                    break;
            }
            return fullStatusText;
        }

        protected string FullServiceTypeText(string srStatus)
        {
            string fullStatusText;
            switch (srStatus)
            {
                case ServiceType.Courts:
                    fullStatusText = ServiceTypeText.Courts;
                    break;
                case ServiceType.NonCourts:
                    fullStatusText = ServiceTypeText.NonCourts;
                    break;
                case ServiceType.Stock:
                    fullStatusText = ServiceTypeText.Stock;
                    break;
                default:
                    fullStatusText = String.Empty;
                    break;
            }
            return fullStatusText;
        }

        public void CheckStyleSheet(string stylesheet)
        {
            DateTime lastwrite = DateTime.Now;
            double size = 0;
            string path = "";

            try
            {
                // Check if styles dir exists.
                DirectoryInfo styledir = new DirectoryInfo(System.Windows.Forms.Application.StartupPath + @"\Stylesheets\");
                if (!styledir.Exists)
                {
                    styledir.Create();
                }

                // Check if delivery stylesheets exists or is valid
                string localstylepath = System.Windows.Forms.Application.StartupPath + @"\Stylesheets\" + stylesheet;
                FileInfo deliveryfile = new FileInfo(localstylepath);

                Printing.GetFileInfo(ref lastwrite, ref size, ref path, stylesheet, Config.CountryCode);

                if (!deliveryfile.Exists || deliveryfile.Length != size || deliveryfile.LastWriteTime != lastwrite)
                {
                    STL.DAL.DTransfer T = new STL.DAL.DTransfer();
                    T.WriteBinarFile(Printing.DownloadFile(path), localstylepath);
                    deliveryfile.LastWriteTime = lastwrite;
                }

                // Check if  stylesheets css exists or is valid
                string localcsspath = System.Windows.Forms.Application.StartupPath + @"\Stylesheets\" + "styles.css";
                FileInfo cssfile = new FileInfo(localcsspath);

                Printing.GetFileInfo(ref lastwrite, ref size, ref path, "styles.css", Config.CountryCode);

                if (!cssfile.Exists || cssfile.Length != size || cssfile.LastWriteTime != lastwrite)
                {
                    STL.DAL.DTransfer T = new STL.DAL.DTransfer();
                    T.WriteBinarFile(Printing.DownloadFile(path), localcsspath);
                    cssfile.LastWriteTime = lastwrite;
                }

            }
            catch
            {
                throw;
            }
        }

        //IP - 11/01/11 - Store Card
        public void NewPrintStoreCardReceipt(string customerName, string custAddr1, string custAddr2, string custAddr3, string custAddr4, string invoiceNo, int receiptNo,
            decimal amtPaid, StoreCardValidated storecard)
        {
            storecard.StoreCardAvailable = storecard.StoreCardAvailable - amtPaid;

            ThreadPool.QueueUserWorkItem(new WaitCallback(delegate
            {


                int noOfCopies = Country.GetCountryParameterValue<int>(CountryParameterNames.Printing.SCardReceiptCopies);

                bool customerCopy = true;
                do
                {
                    if (ThermalPrintingEnabled)
                    {
                        PrintStoreCardReceipt(customerName, custAddr1, custAddr2, custAddr3, custAddr4, invoiceNo, receiptNo, amtPaid, storecard, customerCopy);
                    }
                    else
                    {
                        SlipPrinterPrintStoreCardReceipt(customerName, custAddr1, custAddr2, custAddr3, custAddr4, invoiceNo, receiptNo, amtPaid, storecard, customerCopy);
                    }
                    noOfCopies--;
                    customerCopy = false;
                } while (noOfCopies >= 1);



            }));

        }

        //IP - 07/01/11 - Store Card - Print Store Card Receipt
        public void PrintStoreCardReceipt(string customerName,
            string custAddr1, string custAddr2, string custAddr3, string custAddr4, string invoiceNo, int receiptNo, decimal amtPaid,
            StoreCardValidated storecard, bool customerCopy)
        {
            StoreCardReceipt scr = new StoreCardReceipt();
            ApplyCountryParametersForPayment(ref scr);

            scr.DocumentName = scr.Title.Value;
            if (customerCopy)
                scr.Title.Value += " Customer Copy";
            else
                scr.Title.Value += " Merchant Copy";
            scr.CustomerName.Value = customerName;
            scr.CustomerAddress1.Value = custAddr1;
            scr.CustomerAddress2.Value = custAddr2;
            scr.CustomerAddress3.Value = custAddr3;
            scr.CustomerAddress4.Value = custAddr4;
            scr.MachineName.Value = Environment.MachineName;
            scr.InvoiceNumber.Value = invoiceNo;
            scr.ReceiptNumber.Value = receiptNo;
            scr.StoreCardName.Value = storecard.Name;
            scr.StoreCardNumber.Value = "XXXX-XXXX-XXXX-" + storecard.CardNo.ToString().Substring(storecard.CardNo.ToString().Length - 4, 4);
            scr.StoreCardExpiryDate.Value = string.Format("{0:MM/yy}", storecard.ExpDate);
            scr.StoreCardLimit.Value = storecard.StoreCardLimit;
            scr.StoreCardAvailableSpend.Value = storecard.StoreCardAvailable;
            scr.TransactionDate.Value = System.DateTime.Now;
            scr.AmountPaid.Value = amtPaid;

            scr.Print();

        }



        void SlipPrinterPrintStoreCardReceipt(string customerName,
            string custAddr1, string custAddr2, string custAddr3, string custAddr4, string invoiceNo, int receiptNo, decimal amtPaid, StoreCardValidated storecard, bool CustomerCopy)
        {

            if (this.SlipPrinterOK(true))
            {
                ReceiptPrinter rp = null;
                try
                {
                    Function = "SlipPrinterPrintReceipt";
                    Wait();
                    //uat(5.2)-907, 4.3 merge ------
                    //LW - 71722 To avoid printing the receipt if it's got no items --------------

                    rp = new ReceiptPrinter(this);
                    rp.OpenPrinter();
                    rp.Feed("\x2", 2);
                    rp.Narrow();
                    rp.Init();
                    string textLine = "Store Card Receipt";
                    if (CustomerCopy)
                        textLine += " Customer Copy";
                    else
                        textLine += " Merchant Copy";
                    rp.PrintString(textLine);

                    textLine = "Customer Name " + customerName;
                    rp.PrintString(textLine);

                    textLine = "Address       " + custAddr1;
                    rp.PrintString(textLine);

                    textLine = "              " + custAddr2;
                    rp.PrintString(textLine);

                    textLine = "              " + custAddr3;
                    rp.PrintString(textLine);

                    textLine = "              " + custAddr4;
                    rp.PrintString(textLine);


                    textLine = "Machine Name  " + Environment.MachineName;
                    rp.PrintString(textLine);

                    textLine = "Invoice No.   " + invoiceNo;
                    rp.PrintString(textLine);

                    textLine = "Receipt No.   " + receiptNo.ToString();
                    rp.PrintString(textLine);

                    rp.PrintString("");

                    textLine = "Card Name     " + storecard.Name;
                    rp.PrintString(textLine);

                    textLine = "Number        " + storecard.CardNo.ToString().Substring(storecard.CardNo.ToString().Length - 4, 4);
                    rp.PrintString(textLine);

                    textLine = "Expiry Date   " + storecard.ExpDate.ToShortDateString();
                    rp.PrintString(textLine);


                    textLine = "Date          " + DateTime.Now.ToString();
                    rp.PrintString(textLine);

                    textLine = "Amount        " + amtPaid.ToString("n2");
                    rp.PrintString(textLine);

                    textLine = "Card Limit    " + Convert.ToDecimal(storecard.StoreCardLimit).ToString("n2");
                    rp.PrintString(textLine);

                    textLine = "Available     " + Convert.ToDecimal(storecard.StoreCardAvailable).ToString("n2");
                    rp.PrintString(textLine);


                }
                catch (SlipPrinterException ex)
                {
                    if (ex.Message != "Cancel")
                        Catch((Exception)ex, Function);
                }
                catch (Exception ex)
                {
                    Catch(ex, Function);
                }
                finally
                {
                    StopWait();
                    if (rp != null)
                        rp.ClosePrinter();
                }
            }
        }


        protected void PrintInstallationBooking(string installationNos)
        {
            var queryString = String.Format("installationNos={0}&countryCode={1}",
                                            (installationNos ?? "").Replace("&", "%26"),
                                            Config.CountryCode);

            object Zero = 0;
            object EmptyString = "";
            object postData = EncodePostData(queryString);
            object headers = PostHeader;
            var url = Config.Url + "WInstallationBookingPrint.aspx";

            ((MainForm)FormRoot).browsers = ((CommonForm)FormRoot).CreateBrowserArray(1);
            ((MainForm)FormRoot).browsers[0].Navigate(url, ref Zero, ref EmptyString, ref postData, ref headers);
        }

        public string ToDecimalString(string str)
        {
            return Convert.ToDecimal(str).ToString(DecimalPlaces);
        }

        protected void CheckUserRights(params Control[] controls)
        {
            if (controls == null)
                return;

            dynamicMenus = new Hashtable();

            foreach (var control in controls)
            {
                var key = String.Format("{0}:{1}", this.Name, control.Name);
                dynamicMenus[key] = control;
            }

            ApplyRoleRestrictions();
        }

        protected void CheckUserRights(params Tuple<string, Control>[] nameControlPairs)
        {
            if (nameControlPairs == null)
                return;

            dynamicMenus = new Hashtable();

            foreach (var nameControlPair in nameControlPairs)
            {
                var key = String.Format("{0}:{1}", this.Name, nameControlPair.Item1);
                dynamicMenus[key] = nameControlPair.Item2;
            }

            ApplyRoleRestrictions();
        }

        protected void CheckUserRights(params UserRight[] rights)
        {
            if (rights == null)
                return;

            dynamicMenus = new Hashtable();

            foreach (var right in rights)
            {
                var key = String.Format("{0}:{1}", this.Name, right.Name);
                dynamicMenus[key] = right;
            }

            ApplyRoleRestrictions();
        }

        protected bool ConfirmStaticDataLoaded(params Tuple<string, string[]>[] parameters)
        {
            var docDropDowns = new XmlDocument();
            docDropDowns.LoadXml("<DROP_DOWNS></DROP_DOWNS>");

            var xml = new XmlUtilities();

            foreach (var parameter in parameters)
                if (StaticData.Tables[parameter.Item1] == null)
                    docDropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(docDropDowns, parameter.Item1, parameter.Item2));

            if (docDropDowns.DocumentElement.ChildNodes.Count == 0)
                return true;

            var ds = StaticDataManager.GetDropDownData(docDropDowns.DocumentElement, out Error);

            if (Error.Length > 0)
                return false;

            foreach (DataTable dt in ds.Tables)
                StaticData.Tables[dt.TableName] = dt;

            return true;
        }

        public class UserRight
        {
            private string name;

            private UserRight(string name)
            {
                this.name = name;
            }

            public bool IsAllowed { get; internal set; }

            [EditorBrowsable(EditorBrowsableState.Never)]
            [Browsable(false)]
            internal protected string Name
            {
                get { return name; }
            }

            public static UserRight Create(string name)
            {
                return new UserRight(name);
            }
        }

        private void CommonForm_Load(object sender, EventArgs e)
        {

        }
    }

    public delegate void RecordIDHandler<T>(object sender, RecordIDEventArgs<T> args);

    public class RecordIDEventArgs<T> : EventArgs
    {
        public RecordIDEventArgs(T recordid)
        {
            this.RecordID = recordid;
        }

        public T RecordID
        {
            get;
            set;
        }
    }

    public class ServerConnectException : ApplicationException
    {
        public ServerConnectException()
        {
        }

        public ServerConnectException(string msg)
            : base(msg)
        {
        }
    }


}

