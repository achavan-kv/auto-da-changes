using System;
using System.Collections;
using System.Collections.Specialized;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using STL.Common.Constants.ColumnNames;
using STL.Common.Constants.TableNames;
using STL.Common.Static;
namespace STL.PL
{
    /// <summary>
    /// Lists the status code history for an account. The current status
    /// of the account is shown. The user may update the account to a new
    /// status code.
    /// </summary>
    public class StatusCode : CommonForm
    {
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnClear;
        private STL.PL.AccountTextBox txtAccountNo;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private STL.PL.AccountTextBox txtCurrentCode;
        private Crownwood.Magic.Menus.MenuCommand menuFile;
        private System.Windows.Forms.Button btnLoad;
        private new string Error = "";
        private System.Windows.Forms.DataGrid dgStatusHistory;
        private decimal balance = 0;
        bool status = true;
        private System.Windows.Forms.Label manualBDW;
        private System.Windows.Forms.ComboBox drpStatus;
        string newStatus = "";
        private bool loading = false;
        private Crownwood.Magic.Menus.MenuCommand menuExit;
        private Crownwood.Magic.Menus.MenuCommand menuHelp;
        private Crownwood.Magic.Menus.MenuCommand menuLaunchHelp;
        private System.Windows.Forms.Label label45;
        private System.Windows.Forms.Label label54;
        public System.Windows.Forms.TextBox txtBalance;
        public System.Windows.Forms.TextBox txtArrears;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

        public StatusCode(TranslationDummy d)
        {
            InitializeComponent();
            menuMain = new Crownwood.Magic.Menus.MenuControl();
            menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] { menuFile });
        }

        public StatusCode(Form root, Form parent)
        {
            loading = true;
            InitializeComponent();
            menuMain = new Crownwood.Magic.Menus.MenuControl();
            menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] { menuFile, menuHelp });

            FormRoot = root;
            FormParent = parent;

            dynamicMenus = new Hashtable();
            HashMenus();
            ApplyRoleRestrictions();

            StringCollection s = new StringCollection();

            if (manualBDW.Enabled)
                s.AddRange(new string[] { "1", "2", "3", "4", "5", "6", "7", "8", "9", "S" });
            else
                s.AddRange(new string[] { "1", "2", "3", "4", "5", "9", "S" });

            drpStatus.DataSource = s;
            loading = false;

            txtBalance.BackColor = SystemColors.Window;
            txtArrears.BackColor = SystemColors.Window;
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StatusCode));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label45 = new System.Windows.Forms.Label();
            this.txtBalance = new System.Windows.Forms.TextBox();
            this.txtArrears = new System.Windows.Forms.TextBox();
            this.label54 = new System.Windows.Forms.Label();
            this.drpStatus = new System.Windows.Forms.ComboBox();
            this.manualBDW = new System.Windows.Forms.Label();
            this.btnLoad = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtCurrentCode = new STL.PL.AccountTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtAccountNo = new STL.PL.AccountTextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.dgStatusHistory = new System.Windows.Forms.DataGrid();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.menuFile = new Crownwood.Magic.Menus.MenuCommand();
            this.menuExit = new Crownwood.Magic.Menus.MenuCommand();
            this.menuHelp = new Crownwood.Magic.Menus.MenuCommand();
            this.menuLaunchHelp = new Crownwood.Magic.Menus.MenuCommand();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgStatusHistory)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.SystemColors.Control;
            this.groupBox1.Controls.Add(this.label45);
            this.groupBox1.Controls.Add(this.txtBalance);
            this.groupBox1.Controls.Add(this.txtArrears);
            this.groupBox1.Controls.Add(this.label54);
            this.groupBox1.Controls.Add(this.drpStatus);
            this.groupBox1.Controls.Add(this.btnLoad);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.txtCurrentCode);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.txtAccountNo);
            this.groupBox1.Controls.Add(this.groupBox2);
            this.groupBox1.Location = new System.Drawing.Point(8, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(776, 472);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            // 
            // label45
            // 
            this.label45.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label45.Location = new System.Drawing.Point(8, 288);
            this.label45.Name = "label45";
            this.label45.Size = new System.Drawing.Size(112, 16);
            this.label45.TabIndex = 42;
            this.label45.Text = "Outstanding Balance";
            this.label45.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // txtBalance
            // 
            this.txtBalance.Location = new System.Drawing.Point(136, 288);
            this.txtBalance.Name = "txtBalance";
            this.txtBalance.ReadOnly = true;
            this.txtBalance.Size = new System.Drawing.Size(88, 20);
            this.txtBalance.TabIndex = 40;
            // 
            // txtArrears
            // 
            this.txtArrears.Location = new System.Drawing.Point(136, 360);
            this.txtArrears.Name = "txtArrears";
            this.txtArrears.ReadOnly = true;
            this.txtArrears.Size = new System.Drawing.Size(88, 20);
            this.txtArrears.TabIndex = 41;
            // 
            // label54
            // 
            this.label54.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label54.Location = new System.Drawing.Point(72, 360);
            this.label54.Name = "label54";
            this.label54.Size = new System.Drawing.Size(48, 16);
            this.label54.TabIndex = 43;
            this.label54.Text = "Arrears";
            this.label54.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // drpStatus
            // 
            this.drpStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpStatus.Location = new System.Drawing.Point(136, 216);
            this.drpStatus.Name = "drpStatus";
            this.drpStatus.Size = new System.Drawing.Size(48, 21);
            this.drpStatus.TabIndex = 39;
            this.drpStatus.SelectedIndexChanged += new System.EventHandler(this.drpStatus_SelectedIndexChanged);
            // 
            // manualBDW
            // 
            this.manualBDW.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.manualBDW.Enabled = false;
            this.manualBDW.Location = new System.Drawing.Point(69, 118);
            this.manualBDW.Name = "manualBDW";
            this.manualBDW.Size = new System.Drawing.Size(48, 16);
            this.manualBDW.TabIndex = 38;
            this.manualBDW.Text = "label1";
            this.manualBDW.Visible = false;
            // 
            // btnLoad
            // 
            this.btnLoad.Image = ((System.Drawing.Image)(resources.GetObject("btnLoad.Image")));
            this.btnLoad.Location = new System.Drawing.Point(264, 88);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(32, 32);
            this.btnLoad.TabIndex = 16;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(32, 224);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(96, 16);
            this.label3.TabIndex = 7;
            this.label3.Text = "New Status Code:";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(24, 160);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(112, 16);
            this.label2.TabIndex = 5;
            this.label2.Text = "Current Status Code:";
            // 
            // txtCurrentCode
            // 
            this.txtCurrentCode.BackColor = System.Drawing.SystemColors.Window;
            this.txtCurrentCode.Location = new System.Drawing.Point(136, 152);
            this.txtCurrentCode.MaxLength = 20;
            this.txtCurrentCode.Name = "txtCurrentCode";
            this.txtCurrentCode.PreventPaste = false;
            this.txtCurrentCode.ReadOnly = true;
            this.txtCurrentCode.Size = new System.Drawing.Size(40, 20);
            this.txtCurrentCode.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(64, 104);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 16);
            this.label1.TabIndex = 3;
            this.label1.Text = "Account No:";
            // 
            // txtAccountNo
            // 
            this.txtAccountNo.BackColor = System.Drawing.SystemColors.Window;
            this.txtAccountNo.Location = new System.Drawing.Point(136, 96);
            this.txtAccountNo.MaxLength = 20;
            this.txtAccountNo.Name = "txtAccountNo";
            this.txtAccountNo.PreventPaste = false;
            this.txtAccountNo.Size = new System.Drawing.Size(96, 20);
            this.txtAccountNo.TabIndex = 2;
            this.txtAccountNo.Text = "000-0000-0000-0";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.dgStatusHistory);
            this.groupBox2.Controls.Add(this.btnSave);
            this.groupBox2.Controls.Add(this.btnClear);
            this.groupBox2.Controls.Add(this.manualBDW);
            this.groupBox2.Location = new System.Drawing.Point(320, 24);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(360, 424);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Status History";
            // 
            // dgStatusHistory
            // 
            this.dgStatusHistory.DataMember = "";
            this.dgStatusHistory.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgStatusHistory.Location = new System.Drawing.Point(24, 24);
            this.dgStatusHistory.Name = "dgStatusHistory";
            this.dgStatusHistory.Size = new System.Drawing.Size(312, 360);
            this.dgStatusHistory.TabIndex = 0;
            // 
            // btnSave
            // 
            this.btnSave.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnSave.BackgroundImage")));
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnSave.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnSave.Location = new System.Drawing.Point(128, 392);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(24, 24);
            this.btnSave.TabIndex = 36;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(168, 392);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(75, 23);
            this.btnClear.TabIndex = 3;
            this.btnClear.Text = "&Clear";
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // menuFile
            // 
            this.menuFile.Description = "MenuItem";
            this.menuFile.Text = "&File";
            // 
            // menuExit
            // 
            this.menuExit.Description = "MenuItem";
            this.menuExit.Text = "About This Screen";
            // 
            // menuHelp
            // 
            this.menuHelp.Description = "MenuItem";
            this.menuHelp.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {
            this.menuLaunchHelp});
            this.menuHelp.Text = "&Help";
            // 
            // menuLaunchHelp
            // 
            this.menuLaunchHelp.Description = "MenuItem";
            this.menuLaunchHelp.Text = "&About This Screen";
            this.menuLaunchHelp.Click += new System.EventHandler(this.menuLaunchHelp_Click);
            // 
            // StatusCode
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.SystemColors.ControlDark;
            this.ClientSize = new System.Drawing.Size(792, 477);
            this.Controls.Add(this.groupBox1);
            this.Name = "StatusCode";
            this.Text = "Status Code Maintenance";
            this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.StatusCode_HelpRequested);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgStatusHistory)).EndInit();
            this.ResumeLayout(false);

        }
        #endregion

        private void HashMenus()
        {
            dynamicMenus[this.Name + ":manualBDW"] = this.manualBDW;
        }

        private void btnSave_Click(object sender, System.EventArgs e)
        {
            try
            {
                Wait();
                string category = "BDM";
                status = true;
                drpStatus_SelectedIndexChanged(this, null);

                if (status)
                {
                    if (newStatus == "6")
                    {
                        ManualWriteOff mwo = new ManualWriteOff(category);
                        mwo.ShowDialog();

                        if (mwo.reasonCode.Length > 0)
                        {
                            AccountManager.SavePending(txtAccountNo.UnformattedText, 0,
                                mwo.reasonCode, 0, Credential.UserId, out Error);
                            if (Error.Length > 0)
                                ShowError(Error);
                            else
                                btnLoad_Click(this, null);
                        }
                    }
                    else
                    {
                        AccountManager.UpdateStatus(txtAccountNo.UnformattedText, newStatus, out Error);

                        if (Error.Length > 0)
                            ShowError(Error);
                        else
                            btnLoad_Click(this, null);
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

        private void btnLoad_Click(object sender, System.EventArgs e)
        {
            //IP - CoSACS Improvement - pass the account number entered into the LoadStatusData method.
            string acctNo = txtAccountNo.Text;

            //IP - CoSACS Improvement - Moved the code into the 'LoadStatusData(string accountNo)' method
            //as this is also called from the AccountSearch class.
            LoadStatusData(acctNo);

        }

        private void btnClear_Click(object sender, System.EventArgs e)
        {
            dgStatusHistory.DataSource = null;
            txtCurrentCode.Text = "";
            txtAccountNo.Text = "000-0000-0000-0";
            drpStatus.SelectedIndex = 0;
            txtBalance.Text = (0).ToString(DecimalPlaces);
            txtArrears.Text = (0).ToString(DecimalPlaces);
        }

        private void drpStatus_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (!loading)
            {
                newStatus = (string)drpStatus.SelectedItem;

                if (txtCurrentCode.Text == newStatus)
                {
                    status = false;
                    ShowInfo("M_INVALIDCODE");
                }

                if ((txtCurrentCode.Text == "1" || txtCurrentCode.Text == "2" ||
                    txtCurrentCode.Text == "3" || txtCurrentCode.Text == "4" ||
                    txtCurrentCode.Text == "9") &&
                    newStatus != "1" && newStatus != "2" &&
                    newStatus != "3" && newStatus != "4" &&
                    newStatus != "5" && newStatus != "S" &&
                    newStatus != "9" && newStatus != "8")
                {
                    status = false;
                    ShowInfo("M_INVALIDCODE");
                }

                if (balance != 0 && newStatus == "S")
                {
                    status = false;
                    ShowInfo("M_BALANCECHECK");
                }

                if ((txtCurrentCode.Text == "5") && newStatus != "1" &&
                    newStatus != "2" && newStatus != "3" &&
                    newStatus != "4" && newStatus != "S" &&
                    newStatus != "6" && newStatus != "7" &&
                    newStatus != "8")
                {
                    status = false;
                    ShowInfo("M_INVALIDCODE");
                }

                if ((txtCurrentCode.Text == "6" || txtCurrentCode.Text == "7") &&
                    newStatus != "5" && newStatus != "6" &&
                    newStatus != "7" && newStatus != "S" &&
                    newStatus != "8")
                {
                    status = false;
                    ShowInfo("M_INVALIDCODE");
                }
            }
        }

        private void StatusCode_HelpRequested(object sender, System.Windows.Forms.HelpEventArgs hlpevent)
        {
            string fileName = this.Name + ".htm";
            LaunchHelp(fileName);
        }

        private void menuLaunchHelp_Click(object sender, System.EventArgs e)
        {
            StatusCode_HelpRequested(this, null);
        }

        //IP - 31/07/08 - CoSACS Improvement - moved code from 'private void btnLoad_Click(object sender, System.EventArgs e)'
        //into the below method.
        public void LoadStatusData(string accountNo)
        {
            DataSet ds = null;
            DataSet details = null;
            loading = true;
            DateTime Datedel = DateTime.MinValue;

            try
            {
                Wait();

                //IP - CoSACS Improvement - if an account number has been selected from the 
                //'Account Search' screen then once passed into this routine set it to the 
                //account number field.
                if (accountNo != "000-0000-0000-0" && txtAccountNo.Text != accountNo)
                {
                    txtAccountNo.Text = accountNo;
                }

                string serviceAccount = ""; string serviceStatus = "";                                                      //IP - 26/03/12 - #8842 - LW73943 - Merged from current
                int serviceRequestNo = 0;                                                                                   //IP - 26/03/12 - #8842 - LW73943 - Merged from current

                //IP - 18/07/08 - (CoSACS Improvement) If the account number field does not have an account number entered
                //then display the 'Account Search' screen.
                if (accountNo == "000-0000-0000-0")
                {
                    AccountSearch acctSearch = new AccountSearch();
                    acctSearch.Details = false;
                    acctSearch.FormParent = this;
                    acctSearch.FormRoot = this.FormRoot;
                    ((MainForm)this.FormRoot).AddTabPage(acctSearch, 9);
                }
                //IP - 18/07/08 - (CoSACS Improvement) else an account number has been entered.
                else
                {
                    details = AccountManager.GetAccountDetails(txtAccountNo.UnformattedText, out Error);
                    if (Error.Length > 0)
                    {
                        ShowError(Error);
                    }
                    //else
                    else if (details.Tables[TN.AccountDetails].Rows.Count > 0) //IP - 18/07/08 - (CoSACS Improvement) an account number that exists has been entered.
                    {
                        foreach (DataTable dt in details.Tables)
                            if (dt.TableName == TN.AccountDetails)
                                foreach (DataRow row in dt.Rows)
                                {
                                    txtCurrentCode.Text = (string)row[CN.AccountStatus2];

                                    if (DBNull.Value != row[CN.OutstandingBalance2])
                                    {
                                        balance = (decimal)row[CN.OutstandingBalance2];
                                        txtBalance.Text = balance.ToString(DecimalPlaces);
                                    }
                                    else
                                    {
                                        balance = 0;
                                        txtBalance.Text = balance.ToString(DecimalPlaces);
                                    }

                                    if (DBNull.Value != row[CN.Arrears])
                                        txtArrears.Text = ((decimal)row[CN.Arrears]).ToString(DecimalPlaces);
                                    else
                                        txtArrears.Text = (0).ToString(DecimalPlaces);

                                    if (row[CN.DateDel] != DBNull.Value)
                                        Datedel = Convert.ToDateTime(row[CN.DateDel]);

                                    serviceAccount = Convert.ToString(row[CN.ServiceChargeAcctNo]);                                 //IP - 26/03/12 - #8842 - LW73943 - Merged from current
                                    serviceStatus = Convert.ToString(row["State"]);                                                //#19773 //IP - 26/03/12 - #8842 - LW73943 - Merged from current
                                    //serviceRequestNo = Convert.ToInt32(row[CN.ServiceRequestNo]);                                 //IP - 26/03/12 - #8842 - LW73943 - Merged from current
                                    if (DBNull.Value != row[CN.ServiceRequestNo])                                                   //IP - 10/04/12 - #9878 
                                    {
                                        serviceRequestNo = Convert.ToInt32(row[CN.ServiceRequestNo]);
                                    }

                                }
                        //}

                        StringCollection s = new StringCollection();
                        if (manualBDW.Enabled)
                            s.AddRange(new string[] { "1", "2", "3", "4", "5", "6", "7", "8", "9", "S" });
                        else
                            s.AddRange(new string[] { "1", "2", "3", "4", "5", "9", "S" });

                        if (balance <= 0)
                        {
                            s.Remove("4");
                            s.Remove("5");
                            s.Remove("6");
                        }

                        ((MainForm)this.FormRoot).statusBar1.Text = string.Empty;

                        if (serviceAccount == "")                                                                                    //IP - 26/03/12 - #8842 - LW73943 - Merged from current
                        {
                            if ((Datedel == DateTime.MinValue || Datedel.ToShortDateString() == "01/01/1900") && balance == 0)
                            {
                                s.Remove("S");
                                ((MainForm)this.FormRoot).statusBar1.Text = "Note Account not delivered - need to cancel rather than settle";

                            }
                        }
                        else
                        {
                            //if (serviceStatus != "C")                                                                               //IP - 26/03/12 - #8842 - LW73943 - Merged from current
                            if (serviceStatus != "C" && serviceStatus != string.Empty)                                                //IP - 10/04/12 - #9878
                            {
                                s.Remove("S");
                                ((MainForm)this.FormRoot).statusBar1.Text = "Service request " + Convert.ToString(serviceRequestNo) + " not closed";

                            }
                        }


                        s.Remove(txtCurrentCode.Text);

                        drpStatus.DataSource = null;
                        drpStatus.DataSource = s;

                        ds = AccountManager.GetStatusForAccount(txtAccountNo.UnformattedText, out Error);

                        if (Error.Length > 0)
                            ShowError(Error);
                        else
                        {
                            if (ds != null)
                            {
                                dgStatusHistory.DataSource = ds.Tables["Table1"].DefaultView;

                                if (dgStatusHistory.TableStyles.Count == 0)
                                {
                                    DataGridTableStyle tabStyle = new DataGridTableStyle();
                                    tabStyle.MappingName = ds.Tables["Table1"].TableName;
                                    dgStatusHistory.TableStyles.Add(tabStyle);

                                    tabStyle.GridColumnStyles[CN.acctno].Width = 0;

                                    tabStyle.GridColumnStyles[CN.DateStatChge].Width = 100;
                                    tabStyle.GridColumnStyles[CN.DateStatChge].ReadOnly = true;
                                    tabStyle.GridColumnStyles[CN.DateStatChge].HeaderText = GetResource("T_DATECHANGED");

                                    tabStyle.GridColumnStyles[CN.EmpeeNoStat].Width = 70;
                                    tabStyle.GridColumnStyles[CN.EmpeeNoStat].ReadOnly = true;
                                    tabStyle.GridColumnStyles[CN.EmpeeNoStat].HeaderText = GetResource("T_CHANGEDBY");

                                    tabStyle.GridColumnStyles[CN.StatusCode].Width = 70;
                                    tabStyle.GridColumnStyles[CN.StatusCode].ReadOnly = true;
                                    tabStyle.GridColumnStyles[CN.StatusCode].HeaderText = GetResource("T_STATUSCODE");
                                }
                            }
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
                loading = false;
                StopWait();
            }
        }
    }
}
