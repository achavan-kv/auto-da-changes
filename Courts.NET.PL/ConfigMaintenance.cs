using System;
using System.Drawing.Printing;
using System.Globalization;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using BBSL.Libraries.Printing.PrintDocuments;
using Blue.Cosacs.Shared.Services;
using Blue.Cosacs.Shared.Services.CosacsConfig;
using STL.Common.Static;

namespace STL.PL
{
    /// <summary>
    /// Configuration parameters for the CoSACS application. Normally only
    /// used after a new client installation to set the country code and
    /// branch code for example.
    /// </summary>
    /// 

    public class ConfigMaintenance : CommonForm
    {
        Properties.Settings Settings;
        private MaskedTextBox txtBranch;
        private bool newInstall = false;

        public ConfigMaintenance()
        {
            Settings = Properties.Settings.Default;
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            menuMain = new Crownwood.Magic.Menus.MenuControl();
            menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] { menuFile });

            loadConfig();
            //highlite the server portion of the web service url which must be 
            //changed
            txtUrl.Select(7, txtUrl.Text.IndexOf("/", 7) - 7);
            //TranslateControls();
        }

        public ConfigMaintenance(bool newInstall)
            : this()
        {
            this.newInstall = newInstall;
        }

        public ConfigMaintenance(TranslationDummy d)
        {
            Settings = Properties.Settings.Default;
            InitializeComponent();
            menuMain = new Crownwood.Magic.Menus.MenuControl();
            menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] { menuFile });
        }
        private Crownwood.Magic.Menus.MenuCommand menuFile;
        private Crownwood.Magic.Menus.MenuCommand menuSave;
        private Crownwood.Magic.Menus.MenuCommand menuExit;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtUrl;
        private System.Windows.Forms.TextBox txtCountry;
        //private Crownwood.Magic.Menus.MenuControl menuMain;
        //private readonly string serviceUrlTag = "WebServiceURL";
        //private readonly string countryCode = "CountryCode";
        //private readonly string branchCode = "BranchCode";
        //private readonly string updatePath = "UpdatePath";
        //private readonly string splashImage = "SplashImage";
        //private readonly string cashDrawerID = "CashDrawerID";
        //private readonly string receiptprintermodel = "ReceiptPrinterModel";
        //private readonly string recieptPrinterName = "RecieptPrinterName";

        private System.Windows.Forms.ImageList menuIcons;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox drpCulture;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox txtSplash;
        private System.Windows.Forms.ErrorProvider error;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtCashDrawerID;
        private System.Windows.Forms.ComboBox drpRPModel;
        private System.Windows.Forms.Label label7;
        private Button btnSave;
        private Button btnBrowseImage;
        private CheckBox chkEnablePosPrinter;
        private Label lblRecieptPrinterName;
        private ComboBox cmboPrinters;        
        private System.ComponentModel.IContainer components;




        private void loadConfig()
        {
            CultureInfo[] a = CultureInfo.GetCultures(CultureTypes.SpecificCultures);
            BubbleSort(a);
            drpCulture.DataSource = a;
            drpCulture.DisplayMember = "DisplayName";
            CultureInfo current = new CultureInfo(String.IsNullOrEmpty(Settings.Culture) ? Config.Culture : Settings.Culture);
            drpCulture.SelectedIndex = drpCulture.FindStringExact(current.DisplayName);

            //if(Config.Url.Length==0)
            //{
            //    txtUrl.Text = ConfigurationManager.AppSettings[serviceUrlTag];
            //    txtCountry.Text = ConfigurationManager.AppSettings[countryCode];
            //    txtBranch.Text = ConfigurationManager.AppSettings[branchCode];
            //    //txtUpdates.Text = ConfigurationSettings.AppSettings[updatePath];
            //    txtSplash.Text = ConfigurationManager.AppSettings[splashImage];
            //    txtCashDrawerID.Text = ConfigurationManager.AppSettings[cashDrawerID];
            //    drpRPModel.SelectedIndex = drpRPModel.FindStringExact(ConfigurationManager.AppSettings[receiptprintermodel]);

            //    chkEnablePosPrinter.Checked = Boolean.Parse(ConfigurationManager.AppSettings[MSpos]);

            //}
            //else
            //{
            txtUrl.Text = Config.Url;
            txtCountry.Text = Config.CountryCode;
            txtBranch.Text = Config.BranchCode = String.IsNullOrEmpty(Settings.Branch) ? Config.BranchCode : Settings.Branch;
            //txtUpdates.Text = Config.UpdatePath;
            txtSplash.Text = Config.SplashImage = Settings.SplashImage;
            txtCashDrawerID.Text = Config.CashDrawerID = Settings.CashDrawerID;
            drpRPModel.SelectedIndex = drpRPModel.FindStringExact(Settings.ReceiptPrinterModel);
            Config.ReceiptPrinterModel = Settings.ReceiptPrinterModel;

            chkEnablePosPrinter.Checked = Config.ThermalPrintingEnabled = Settings.ThermalPrinterEnabled;
            //}

            foreach (string strPrinter in PrinterSettings.InstalledPrinters)
                cmboPrinters.Items.Add(strPrinter);

            cmboPrinters.SelectedItem = Config.ThermalPrinterName = Settings.PrinterName;
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

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigMaintenance));
            this.menuFile = new Crownwood.Magic.Menus.MenuCommand();
            this.menuSave = new Crownwood.Magic.Menus.MenuCommand();
            this.menuIcons = new System.Windows.Forms.ImageList(this.components);
            this.menuExit = new Crownwood.Magic.Menus.MenuCommand();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnBrowseImage = new System.Windows.Forms.Button();
            this.txtSplash = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtBranch = new System.Windows.Forms.MaskedTextBox();
            this.cmboPrinters = new System.Windows.Forms.ComboBox();
            this.chkEnablePosPrinter = new System.Windows.Forms.CheckBox();
            this.lblRecieptPrinterName = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.drpRPModel = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtCashDrawerID = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.drpCulture = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtUrl = new System.Windows.Forms.TextBox();
            this.txtCountry = new System.Windows.Forms.TextBox();
            this.error = new System.Windows.Forms.ErrorProvider(this.components);
            this.groupBox4.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.error)).BeginInit();
            this.SuspendLayout();
            // 
            // menuFile
            // 
            this.menuFile.Description = "MenuItem";
            this.menuFile.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {
            this.menuSave,
            this.menuExit});
            this.menuFile.Text = "&File";
            // 
            // menuSave
            // 
            this.menuSave.Description = "MenuItem";
            this.menuSave.ImageIndex = 1;
            this.menuSave.ImageList = this.menuIcons;
            this.menuSave.Text = "&Save";
            this.menuSave.Click += new System.EventHandler(this.menuSave_Click);
            // 
            // menuIcons
            // 
            this.menuIcons.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.menuIcons.ImageSize = new System.Drawing.Size(16, 16);
            this.menuIcons.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // menuExit
            // 
            this.menuExit.Description = "MenuItem";
            this.menuExit.ImageIndex = 0;
            this.menuExit.ImageList = this.menuIcons;
            this.menuExit.Text = "E&xit";
            this.menuExit.Click += new System.EventHandler(this.menuExit_Click);
            // 
            // groupBox4
            // 
            this.groupBox4.BackColor = System.Drawing.SystemColors.Control;
            this.groupBox4.Controls.Add(this.btnSave);
            this.groupBox4.Controls.Add(this.groupBox2);
            this.groupBox4.Controls.Add(this.label1);
            this.groupBox4.Controls.Add(this.groupBox1);
            this.groupBox4.Location = new System.Drawing.Point(8, 0);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(776, 472);
            this.groupBox4.TabIndex = 3;
            this.groupBox4.TabStop = false;
            // 
            // btnSave
            // 
            this.btnSave.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnSave.BackgroundImage")));
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnSave.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnSave.Location = new System.Drawing.Point(711, 47);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(25, 25);
            this.btnSave.TabIndex = 40;
            this.btnSave.Click += new System.EventHandler(this.menuSave_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.BackColor = System.Drawing.SystemColors.Control;
            this.groupBox2.Controls.Add(this.btnBrowseImage);
            this.groupBox2.Controls.Add(this.txtSplash);
            this.groupBox2.Location = new System.Drawing.Point(112, 344);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(568, 96);
            this.groupBox2.TabIndex = 39;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Splash Image Location:";
            // 
            // btnBrowseImage
            // 
            this.btnBrowseImage.Image = ((System.Drawing.Image)(resources.GetObject("btnBrowseImage.Image")));
            this.btnBrowseImage.Location = new System.Drawing.Point(470, 36);
            this.btnBrowseImage.Name = "btnBrowseImage";
            this.btnBrowseImage.Size = new System.Drawing.Size(32, 32);
            this.btnBrowseImage.TabIndex = 6;
            this.btnBrowseImage.Click += new System.EventHandler(this.btnBrowseImage_Click);
            // 
            // txtSplash
            // 
            this.txtSplash.Location = new System.Drawing.Point(72, 48);
            this.txtSplash.Name = "txtSplash";
            this.txtSplash.Size = new System.Drawing.Size(328, 20);
            this.txtSplash.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(168, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(448, 48);
            this.label1.TabIndex = 37;
            this.label1.Text = "Enter/ amend config file values and then save. Application need not be restarted " +
    "for new values to take effect.";
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.SystemColors.Control;
            this.groupBox1.Controls.Add(this.txtBranch);
            this.groupBox1.Controls.Add(this.cmboPrinters);
            this.groupBox1.Controls.Add(this.chkEnablePosPrinter);
            this.groupBox1.Controls.Add(this.lblRecieptPrinterName);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.drpRPModel);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.txtCashDrawerID);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.drpCulture);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.txtUrl);
            this.groupBox1.Controls.Add(this.txtCountry);
            this.groupBox1.Location = new System.Drawing.Point(112, 72);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(568, 256);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Config Variables";
            // 
            // txtBranch
            // 
            this.txtBranch.Location = new System.Drawing.Point(187, 88);
            this.txtBranch.Mask = "0000";
            this.txtBranch.Name = "txtBranch";
            this.txtBranch.Size = new System.Drawing.Size(76, 20);
            this.txtBranch.TabIndex = 19;
            // 
            // cmboPrinters
            // 
            this.cmboPrinters.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmboPrinters.Enabled = false;
            this.cmboPrinters.Location = new System.Drawing.Point(288, 144);
            this.cmboPrinters.Name = "cmboPrinters";
            this.cmboPrinters.Size = new System.Drawing.Size(176, 21);
            this.cmboPrinters.TabIndex = 18;
            // 
            // chkEnablePosPrinter
            // 
            this.chkEnablePosPrinter.AutoSize = true;
            this.chkEnablePosPrinter.Location = new System.Drawing.Point(288, 108);
            this.chkEnablePosPrinter.Name = "chkEnablePosPrinter";
            this.chkEnablePosPrinter.Size = new System.Drawing.Size(138, 17);
            this.chkEnablePosPrinter.TabIndex = 16;
            this.chkEnablePosPrinter.Text = "Enable Thermal Printing";
            this.chkEnablePosPrinter.UseVisualStyleBackColor = true;
            this.chkEnablePosPrinter.CheckedChanged += new System.EventHandler(this.chkEnablePosPrinter_CheckedChanged);
            // 
            // lblRecieptPrinterName
            // 
            this.lblRecieptPrinterName.Enabled = false;
            this.lblRecieptPrinterName.Location = new System.Drawing.Point(285, 128);
            this.lblRecieptPrinterName.Name = "lblRecieptPrinterName";
            this.lblRecieptPrinterName.Size = new System.Drawing.Size(112, 16);
            this.lblRecieptPrinterName.TabIndex = 15;
            this.lblRecieptPrinterName.Text = "Printer Name";
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(288, 184);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(120, 16);
            this.label7.TabIndex = 10;
            this.label7.Text = "Receipt Printer Model";
            // 
            // drpRPModel
            // 
            this.drpRPModel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpRPModel.Items.AddRange(new object[] {
            "295",
            "290"});
            this.drpRPModel.Location = new System.Drawing.Point(288, 200);
            this.drpRPModel.Name = "drpRPModel";
            this.drpRPModel.Size = new System.Drawing.Size(104, 21);
            this.drpRPModel.TabIndex = 9;
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(72, 184);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(100, 16);
            this.label6.TabIndex = 8;
            this.label6.Text = "Cash Drawer ID";
            // 
            // txtCashDrawerID
            // 
            this.txtCashDrawerID.Location = new System.Drawing.Point(72, 200);
            this.txtCashDrawerID.Name = "txtCashDrawerID";
            this.txtCashDrawerID.Size = new System.Drawing.Size(176, 20);
            this.txtCashDrawerID.TabIndex = 7;
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(72, 128);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(100, 16);
            this.label5.TabIndex = 6;
            this.label5.Text = "Culture Setting";
            // 
            // drpCulture
            // 
            this.drpCulture.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpCulture.Location = new System.Drawing.Point(72, 144);
            this.drpCulture.Name = "drpCulture";
            this.drpCulture.Size = new System.Drawing.Size(176, 21);
            this.drpCulture.TabIndex = 3;
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(184, 72);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(100, 16);
            this.label4.TabIndex = 4;
            this.label4.Text = "Branch Code";
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(72, 72);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(100, 16);
            this.label3.TabIndex = 2;
            this.label3.Text = "Country Code";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(72, 24);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(100, 16);
            this.label2.TabIndex = 1;
            this.label2.Text = "Web Service URL";
            // 
            // txtUrl
            // 
            this.txtUrl.Location = new System.Drawing.Point(72, 40);
            this.txtUrl.Name = "txtUrl";
            this.txtUrl.Size = new System.Drawing.Size(400, 20);
            this.txtUrl.TabIndex = 0;
            // 
            // txtCountry
            // 
            this.txtCountry.Enabled = false;
            this.txtCountry.Location = new System.Drawing.Point(72, 88);
            this.txtCountry.Name = "txtCountry";
            this.txtCountry.Size = new System.Drawing.Size(88, 20);
            this.txtCountry.TabIndex = 1;
            // 
            // error
            // 
            this.error.ContainerControl = this;
            this.error.DataMember = "";
            // 
            // ConfigMaintenance
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.SystemColors.ControlDark;
            this.ClientSize = new System.Drawing.Size(792, 476);
            this.Controls.Add(this.groupBox4);
            this.Name = "ConfigMaintenance";
            this.Text = "CoSACS Config File Maintenance";
            this.groupBox4.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.error)).EndInit();
            this.ResumeLayout(false);

        }
        #endregion

        private void menuExit_Click(object sender, System.EventArgs e)
        {
            CloseTab();
        }

        private void menuSave_Click(object sender, System.EventArgs e)
        {

            txtBranch.Text = txtBranch.Text.Replace(" ", "");       // #9994 

            if (String.IsNullOrEmpty(txtBranch.Text.Trim()))
            {
                error.SetError(txtBranch, "The branch selected does not exist.");
                return;
            }

            if (!newInstall && Config.Connected)
            {
                Client.Call(new CheckBranchRequest()
                                {
                                    BranchNo = txtBranch.Text
                                },
                                response =>
                                {
                                    if (response.BranchExists)
                                    {
                                        Save();
                                        error.SetError(txtBranch, "");
                                    }
                                    else
                                        error.SetError(txtBranch, "The branch selected does not exist.");
                                },
                                this);
            }
            else
                Save();

        }

        private void Save()
        {
            string Error = string.Empty;

            try
            {
                Wait();

                if (drpCulture.SelectedIndex == -1)
                {
                    error.SetError(drpCulture, "Please choose a culture");
                    return;
                }
                else
                    error.Clear();

                Config.BranchCode = Settings.Branch = txtBranch.Text;
                Config.CountryCode = txtCountry.Text.ToUpper();
                Config.Url = txtUrl.Text;
                Config.CashDrawerID = Settings.CashDrawerID = txtCashDrawerID.Text;
                Config.ReceiptPrinterModel = Settings.ReceiptPrinterModel = (string)drpRPModel.SelectedItem;
                Config.ThermalPrinterName = Settings.PrinterName = cmboPrinters.SelectedItem != null ? cmboPrinters.SelectedItem.ToString() : null;// txtThermalPrinterName.Text;
                PrintContent.PrinterName = Config.ThermalPrinterName;
                Config.ThermalPrintingEnabled = Settings.ThermalPrinterEnabled = chkEnablePosPrinter.Checked;

                //if (!Config.NewInstall)
                //{
                //    Config.StoreType = AccountManager.GetStoreType(Convert.ToInt16(Config.BranchCode), out Error);
                //    if (Error.Length > 0)
                //    {
                //        ShowError(Error);
                //    }
                //}


                Config.Culture = Settings.Culture = ((CultureInfo)drpCulture.SelectedItem).Name;
                LoadDictionary();

                //Translate the mainform
                //Crownwood.Magic.Menus.MenuControl m = (Crownwood.Magic.Menus.MenuControl)((MainForm)FormRoot).MainTabControl.TabPages[0].Tag;
                //TranslateMenus(m.MenuCommands);
                //TranslateControls(((MainForm)FormRoot).Controls);

                Thread.CurrentThread.CurrentCulture = new CultureInfo(Config.Culture);
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(Config.Culture);

                /*
                Thread.CurrentThread.CurrentCulture.DateTimeFormat = (new CultureInfo("en-GB")).DateTimeFormat;
                */

                Config.SplashImage = Settings.SplashImage = txtSplash.Text;
                //if(File.Exists(Config.SplashImage))
                //{
                //    Bitmap bg = new Bitmap(Config.SplashImage);
                //    ((MainForm)FormRoot).grpLogIn.BackgroundImage = bg; 
                //    ((MainForm)FormRoot).pbSplash.Image = bg;
                //}
                //Config.UpdatePath = txtUpdates.Text;
                Config.Save(Assembly.GetExecutingAssembly());
                Settings.Save();

                if (Credential.User != null)
                    ((MainForm)FormRoot).Text = GetResource("M_TITLETEXT", new object[] { Credential.UserId.ToString(), Credential.Name, Config.BranchCode, Config.Server });
                Config.NewInstall = false;
                CloseTab();
                this.Close();
                ((MainForm)FormRoot).SplashSetup();
            }
            catch (Exception ex)
            {
                Catch(ex, "ConfigSave");
            }
            finally
            {
                StopWait();
            }
        }

        //private void ConfigMaintenance_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        //{
        //    Save();
        //}

        /*
        private void btnBrowse_Click(object sender, System.EventArgs e)
        {
            try
            {
                Function = "btnBrowse_Click";
                OpenFileDialog open = new OpenFileDialog();
                open.Filter = "xml files (*.xml)|*.xml|All files (*.*)|*.*" ;

                //open.InitialDirectory = "";
                if(open.ShowDialog() == DialogResult.OK)
                {
                    txtUpdates.Text = open.FileName;
                }
            }
            catch(Exception ex)
            {
                Catch(ex, Function);
            }
        }
        */

        private void btnBrowseImage_Click(object sender, System.EventArgs e)
        {
            try
            {
                Function = "menuSplash_Click";

                OpenFileDialog open = new OpenFileDialog();
                open.Filter = "jpegs(*.jpg)|*.jpg|bitmaps(*.bmp)|*.bmp|All files(*.*)|*.*";
                open.Title = "Select Image";

                if (open.ShowDialog() == DialogResult.OK)
                {
                    txtSplash.Text = open.FileName;
                }
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
        }

        private void chkEnablePosPrinter_CheckedChanged(object sender, EventArgs e)
        {
            lblRecieptPrinterName.Enabled = chkEnablePosPrinter.Checked;
            cmboPrinters.Enabled = chkEnablePosPrinter.Checked;
        }

        
    }
}
