using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;
using Crownwood.Magic.Menus;
using STL.Common;
using STL.Common.Constants.ColumnNames;
using STL.Common.Constants.TableNames;
using STL.Common.Static;

namespace STL.PL
{
    /// <summary>
    /// When an employee takes a payment the amount of the payment is added to
    /// their cash till to be totalled at the end of the day. These totals
    /// then become available to deposit either to the safe or to the bank.
    /// This screen lists the employees for the selected branch. An employee
    /// can be selected to list a breakdown of the amounts available for deposit
    /// for each payment method. The user can assign these amounts to be deposited
    /// to the safe or to the bank. Once all the amounts have been deposited then
    /// the cashier session ends. A new cashier session will be started the next
    /// time the employee takes a payment or when this screen is used to take a
    /// float from the safe. At any time during a session money can be transferred
    /// between the safe and the cashier to either increase the float or to deposit
    /// money back to the safe.
    /// </summary>
    public class CashierDeposits : CommonForm
    {
        private new string Error = "";
        private int BranchIndex = -1;
        private short PreviousBranchNo = Convert.ToInt16(Config.BranchCode);
        private DataView Deposits = null;
        private int CurrentCashier = 0;
        private bool CurrentCashierTotalled = false;
        private bool DepositIsOutstanding = false;
        // CR696 Deposit types are now set in the BL per branch per paymethod
        //private string DefaultDepositType = "";
        private bool BlockColChange = false;
        bool _closing = false;
        private bool Unlock = false;

        private Crownwood.Magic.Menus.MenuCommand menuHelp;
        private Crownwood.Magic.Menus.MenuCommand menuCashierDepositsHelp;
        private Crownwood.Magic.Menus.MenuCommand menuFile;
        private Crownwood.Magic.Menus.MenuCommand menuSave;
        private Crownwood.Magic.Menus.MenuCommand menuExit;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox drpBranchNo;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.DataGrid dgEmployee;
        private System.Windows.Forms.GroupBox gbDeposits;
        private System.Windows.Forms.DataGrid dgDeposits;
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

        public CashierDeposits(TranslationDummy d)
        {
            InitializeComponent();
            menuMain = new Crownwood.Magic.Menus.MenuControl();
            menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] { menuFile, menuHelp });
        }

        public CashierDeposits(Form root, Form parent)
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            FormRoot = root;
            FormParent = parent;

            menuMain = new Crownwood.Magic.Menus.MenuControl();
            menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] { menuFile, menuHelp });
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
            this.menuFile = new Crownwood.Magic.Menus.MenuCommand();
            this.menuSave = new Crownwood.Magic.Menus.MenuCommand();
            this.menuExit = new Crownwood.Magic.Menus.MenuCommand();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.drpBranchNo = new System.Windows.Forms.ComboBox();
            this.dgEmployee = new System.Windows.Forms.DataGrid();
            this.gbDeposits = new System.Windows.Forms.GroupBox();
            this.dgDeposits = new System.Windows.Forms.DataGrid();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.menuHelp = new Crownwood.Magic.Menus.MenuCommand();
            this.menuCashierDepositsHelp = new Crownwood.Magic.Menus.MenuCommand();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgEmployee)).BeginInit();
            this.gbDeposits.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgDeposits)).BeginInit();
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
            this.menuSave.Text = "&Save";
            // 
            // menuExit
            // 
            this.menuExit.Description = "MenuItem";
            this.menuExit.Text = "E&xit";
            this.menuExit.Click += new System.EventHandler(this.menuExit_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.SystemColors.Control;
            this.groupBox1.Controls.Add(this.btnRefresh);
            this.groupBox1.Controls.Add(this.btnExit);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.drpBranchNo);
            this.groupBox1.Controls.Add(this.dgEmployee);
            this.groupBox1.Location = new System.Drawing.Point(8, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(776, 208);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Employees";
            // 
            // btnRefresh
            // 
            this.btnRefresh.Location = new System.Drawing.Point(544, 32);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(56, 23);
            this.btnRefresh.TabIndex = 1;
            this.btnRefresh.TabStop = false;
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(608, 32);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(56, 23);
            this.btnExit.TabIndex = 2;
            this.btnExit.TabStop = false;
            this.btnExit.Text = "Exit";
            this.btnExit.Click += new System.EventHandler(this.menuExit_Click);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(64, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(48, 16);
            this.label1.TabIndex = 2;
            this.label1.Text = "Branch";
            // 
            // drpBranchNo
            // 
            this.drpBranchNo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpBranchNo.Location = new System.Drawing.Point(67, 32);
            this.drpBranchNo.Name = "drpBranchNo";
            this.drpBranchNo.Size = new System.Drawing.Size(96, 21);
            this.drpBranchNo.TabIndex = 0;
            this.drpBranchNo.SelectedIndexChanged += new System.EventHandler(this.drpBranchNo_SelectedIndexChanged);
            // 
            // dgEmployee
            // 
            this.dgEmployee.DataMember = "";
            this.dgEmployee.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgEmployee.Location = new System.Drawing.Point(67, 64);
            this.dgEmployee.Name = "dgEmployee";
            this.dgEmployee.ReadOnly = true;
            this.dgEmployee.Size = new System.Drawing.Size(597, 128);
            this.dgEmployee.TabIndex = 0;
            this.dgEmployee.TabStop = false;
            this.dgEmployee.MouseUp += new System.Windows.Forms.MouseEventHandler(this.dgEmployee_MouseUp);
            // 
            // gbDeposits
            // 
            this.gbDeposits.BackColor = System.Drawing.SystemColors.Control;
            this.gbDeposits.Controls.Add(this.dgDeposits);
            this.gbDeposits.Controls.Add(this.btnClear);
            this.gbDeposits.Controls.Add(this.btnSave);
            this.gbDeposits.Location = new System.Drawing.Point(8, 208);
            this.gbDeposits.Name = "gbDeposits";
            this.gbDeposits.Size = new System.Drawing.Size(776, 264);
            this.gbDeposits.TabIndex = 1;
            this.gbDeposits.TabStop = false;
            this.gbDeposits.Text = "Deposits";
            // 
            // dgDeposits
            // 
            this.dgDeposits.CaptionVisible = false;
            this.dgDeposits.DataMember = "";
            this.dgDeposits.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgDeposits.Location = new System.Drawing.Point(67, 48);
            this.dgDeposits.Name = "dgDeposits";
            this.dgDeposits.Size = new System.Drawing.Size(597, 184);
            this.dgDeposits.TabIndex = 0;
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(608, 16);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(56, 23);
            this.btnClear.TabIndex = 2;
            this.btnClear.TabStop = false;
            this.btnClear.Text = "Clear";
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(544, 16);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(56, 23);
            this.btnSave.TabIndex = 1;
            this.btnSave.TabStop = false;
            this.btnSave.Text = "Save";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // menuHelp
            // 
            this.menuHelp.Description = "MenuItem";
            this.menuHelp.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {
            this.menuCashierDepositsHelp});
            this.menuHelp.Text = "&Help";
            // 
            // menuCashierDepositsHelp
            // 
            this.menuCashierDepositsHelp.Description = "MenuItem";
            this.menuCashierDepositsHelp.Text = "&About this Screen";
            this.menuCashierDepositsHelp.Click += new System.EventHandler(this.menuCashierDepositsHelp_Click);
            // 
            // CashierDeposits
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.SystemColors.ControlDark;
            this.ClientSize = new System.Drawing.Size(792, 477);
            this.Controls.Add(this.gbDeposits);
            this.Controls.Add(this.groupBox1);
            this.Name = "CashierDeposits";
            this.Text = "Cashier Deposits";
            this.Load += new System.EventHandler(this.NewCashierDeposits_Load);
            this.Enter += new System.EventHandler(this.CashierDeposits_Enter);
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgEmployee)).EndInit();
            this.gbDeposits.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgDeposits)).EndInit();
            this.ResumeLayout(false);

        }
        #endregion

        private bool LoadEmployees(bool reset)
        {
            short branchNo = (short)((DataRowView)drpBranchNo.SelectedItem)[CN.BranchNo];
            int curEmpIndex = dgEmployee.CurrentRowIndex;
            decimal curForDeposit = 0;
            decimal curTransferredToSafe = 0;
            decimal curAllocatedFloat = 0;
            string curHasTotalled = "";
            bool refreshed = false;

            if (!reset && curEmpIndex >= 0)
            {
                // FR67947 Keep current values to detect whether thay have changed
                DataRowView curEmpRow = ((DataView)dgEmployee.DataSource)[curEmpIndex];
                CurrentCashier = (int)curEmpRow[CN.EmployeeNo];
                curForDeposit = (decimal)curEmpRow[CN.ForDeposit];
                curTransferredToSafe = (decimal)curEmpRow[CN.TransferredToSafe];
                curAllocatedFloat = (decimal)curEmpRow[CN.AllocatedFloat];
                curHasTotalled = (string)curEmpRow[CN.HasTotalled];
            }

            DataSet ds = PaymentManager.GetCashierOutstandingIncome(branchNo, out Error);
            if (Error.Length > 0)
                ShowError(Error);
            else
            {
                dgEmployee.DataSource = null; //Prevent errors when loading data grids
                dgEmployee.DataSource = ds.Tables[TN.CashierOutstandingIncome].DefaultView;

                if (dgEmployee.TableStyles.Count == 0)
                {
                    DataGridTableStyle tabStyle = new DataGridTableStyle();
                    tabStyle.MappingName = TN.CashierOutstandingIncome;

                    AddColumnStyle(CN.EmployeeNo, tabStyle, 50, true, GetResource("T_EMPEENO"), "", HorizontalAlignment.Left);
                    AddColumnStyle(CN.EmployeeName, tabStyle, 140, true, GetResource("T_NAME"), "", HorizontalAlignment.Left);
                    AddColumnStyle(CN.ForDeposit, tabStyle, 100, true, GetResource("T_FORDEPOSIT"), DecimalPlaces, HorizontalAlignment.Right);
                    AddColumnStyle(CN.TransferredToSafe, tabStyle, 100, true, GetResource("T_TRANSFERREDTOSAFE"), DecimalPlaces, HorizontalAlignment.Right);
                    AddColumnStyle(CN.AllocatedFloat, tabStyle, 100, true, GetResource("T_ALLOCATEDFLOAT"), DecimalPlaces, HorizontalAlignment.Right);
                    AddColumnStyle(CN.HasTotalled, tabStyle, 45, true, GetResource("T_TOTALLED"), "", HorizontalAlignment.Left);

                    dgEmployee.TableStyles.Add(tabStyle);
                }

                if (!reset && curEmpIndex >= 0)
                {
                    // FR67947 Check the cashier totals have not changed (perhaps by the cashier logged in elsewhere)
                    int index = 0;
                    bool found = false;
                    while (!found && index < ((DataView)dgEmployee.DataSource).Count)
                    {
                        if ((int)((DataView)dgEmployee.DataSource)[index][CN.EmployeeNo] == CurrentCashier)
                        {
                            // Found the same Cashier
                            found = true;
                            dgEmployee.CurrentRowIndex = index;
                            CurrentCashierTotalled = "Y" == (string)((DataView)dgEmployee.DataSource)[index][CN.HasTotalled] ? true : false;

                            // Check whether the totals have changed
                            DataRowView curEmpRow = ((DataView)dgEmployee.DataSource)[index];
                            if (curForDeposit != (decimal)curEmpRow[CN.ForDeposit]
                                || curTransferredToSafe != (decimal)curEmpRow[CN.TransferredToSafe]
                                || curAllocatedFloat != (decimal)curEmpRow[CN.AllocatedFloat]
                                || curHasTotalled != (string)curEmpRow[CN.HasTotalled])
                            {
                                // Totals have changed
                                refreshed = true;
                                ShowWarning(GetResource("M_MUSTREFRESHCASHIER"));
                                this.dgEmployee_MouseUp(null, new System.Windows.Forms.MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));
                            }
                        }
                        index++;
                    }
                    if (!found) this.ResetCashier();
                }
                else
                    this.ResetCashier();
            }
            return !refreshed;
        }

        private void ResetCashier()
        {
            CurrentCashier = 0;
            CurrentCashierTotalled = false;
            DepositIsOutstanding = false;
            Deposits = null;
            dgDeposits.DataSource = null;
        }


        /// <summary>
        /// This handler will be fired when the screen tries to close. It will 
        /// make sure that the screen is unlocked. It will also then check 
        /// whether the cashier logged in has any outsdtanding deposits to make. 
        /// If they do, they will not be able to exit the screen until they have 
        /// saved them.
        /// </summary>
        /// <returns></returns>
        public override bool ConfirmClose()
        {
            bool status = true;
            try
            {
                Wait();
                _closing = true;

                if (Unlock)
                {
                    short branchNo = (short)((DataRowView)drpBranchNo.SelectedItem)[CN.BranchNo];
                    PaymentManager.UnLockDepositScreen(branchNo, out Error);
                    if (Error.Length > 0)
                    {
                        ShowError(Error);
                        status = false;
                    }
                }

                if (status && Unlock)
                {
                    status = ((MainForm)FormRoot).CheckCashierDeposits();
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
            return status;
        }

        /// <summary>
        /// this calls the stored procedure DN_CashierOutstandingIncomeGetSP which 
        /// works out the amount available for deposit for each cashier and for 
        /// the Safe. See the procedure for description of how it does what it 
        /// does
        /// </summary>
        private void NewCashierDeposits_Load(object sender, System.EventArgs e)
        {
            try
            {
                Wait();

                /* load up the list of branches */
                XmlUtilities xml = new XmlUtilities();
                XmlDocument dropDowns = new XmlDocument();
                dropDowns.LoadXml("<DROP_DOWNS></DROP_DOWNS>");

                if (StaticData.Tables[TN.BranchNumber] == null)
                    dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.BranchNumber, null));

                DataSet ds = StaticDataManager.GetDropDownData(dropDowns.DocumentElement, out Error);
                if (Error.Length > 0)
                    ShowError(Error);
                else
                {
                    foreach (DataTable dt in ds.Tables)
                        StaticData.Tables[dt.TableName] = dt;
                }

                drpBranchNo.DataSource = ((DataTable)StaticData.Tables[TN.BranchNumber]).DefaultView;
                drpBranchNo.DisplayMember = CN.BranchNo;

                int index = drpBranchNo.FindStringExact(Config.BranchCode);
                if (index >= 0)
                    drpBranchNo.SelectedIndex = index;

                if (Config.BranchCode != Convert.ToString(Country[CountryParameterNames.HOBranchNo]))
                    drpBranchNo.Enabled = false;

                LoadEmployees(true);
            }
            catch (Exception ex)
            {
                Catch(ex, "NewCashierDeposits_Load");
            }
            finally
            {
                StopWait();
                loaded = true;
            }
        }

        /// <summary>
        /// It's possible to change branches in this screen if you're logged in
        /// to the head office branch. When the branch is changed the screen
        /// must be unlocked for the old branch and locked for the new branch. 
        /// The employee totals will then be reloaded for the new branch.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void drpBranchNo_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            try
            {
                Wait();
                bool status = true;

                if (loaded && BranchIndex != drpBranchNo.SelectedIndex && !_closing)
                {
                    if (BranchIndex >= 0)
                    {
                        short oldBranch = (short)((DataView)drpBranchNo.DataSource)[BranchIndex][CN.BranchNo];
                        PaymentManager.UnLockDepositScreen(oldBranch, out Error);
                        if (Error.Length > 0)
                        {
                            ShowError(Error);
                            drpBranchNo.SelectedIndex = BranchIndex;
                            status = false;
                        }
                    }

                    if (status)
                    {
                        short branchNo = (short)((DataRowView)drpBranchNo.SelectedItem)[CN.BranchNo];
                        PaymentManager.LockDepositScreen(branchNo, out Error);
                        if (Error.Length > 0)
                        {
                            ShowError(Error);
                            _closing = true;
                            Unlock = false;
                            CloseTab();
                        }
                        else
                        {
                            Unlock = true;
                            LoadEmployees(true);
                            BranchIndex = drpBranchNo.SelectedIndex;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Catch(ex, "drpBranchNo_SelectedIndexChanged");
            }
            finally
            {
                StopWait();
            }
        }

        /// <summary>
        /// The refresh button will cause the totals data for the employees
        /// to be reloaded for the selected branch
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRefresh_Click(object sender, System.EventArgs e)
        {
            try
            {
                Wait();
                LoadEmployees(true);
            }
            catch (Exception ex)
            {
                Catch(ex, "btnRefresh_Click");
            }
            finally
            {
                StopWait();
            }
        }

        /// <summary>
        /// This event handler will be called when the user changes column in the lower
        /// data grid. It is used to validate that the user has entered something 
        /// valid. Validation ensures that the user cannot deposit more to the safe
        /// and the bank than they have available for deposit.
        /// 
        /// Must be careful here not to insert any code which will trigger this
        /// event handler otherwise you'll end up in a never ending loop. See the use
        /// of the BlockColChange variable below to avoid this.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void OnColumnChange(object sender, System.Data.DataColumnChangeEventArgs e)
        {
            try
            {
                Wait();

                if (!BlockColChange && Deposits != null)
                {
                    int index = dgDeposits.CurrentCell.RowNumber;
                    decimal forDeposit = (decimal)Deposits[index][CN.ForDeposit];
                    decimal minDeposit = 0;
                    decimal maxDeposit = 0;

                    if (forDeposit >= 0)
                    {
                        // Positive Deposit
                        minDeposit = 0;
                        maxDeposit = forDeposit;
                    }
                    else
                    {
                        // Negative Deposit
                        minDeposit = forDeposit;
                        maxDeposit = 0;
                    }

                    // Make sure that the total amount being deposited is
                    // never more than the amount available
                    if (Deposits.Table.Columns[CN.BankedValue].Ordinal == dgDeposits.CurrentCell.ColumnNumber)
                    {
                        e.ProposedValue = decimal.Round((decimal)e.ProposedValue, 2); //IP - 20/09/2007
                        decimal forBank = (decimal)e.ProposedValue;
                        decimal forSafe = 0;
                        bool canBankNeg = ((string)e.Row[CN.NegativeRef] == "1");
                        int empeeno = (int)Deposits[index][CN.EmployeeNo];

                        if (forBank > maxDeposit)
                            forBank = maxDeposit;
                        else if (forBank < 0 && !canBankNeg)
                        {
                            // CR732 Amendment - restrict this option using the reference field
                            forBank = 0;
                            ShowInfo("M_BANKDEPOSITMETHOD");
                        }
                        else if (forBank < minDeposit && CurrentCashierTotalled && DepositIsOutstanding)
                        {
                            // CR732 A negative deposit can be forBank which will create a float
                            // and start a new cashier session. However, this is not allowed
                            // while there are any outstanding totalled deposits waiting to go
                            // to the safe or bank.
                            forBank = minDeposit;
                            ShowInfo("M_MUSTDEPOSITOUTSTANDING");
                        }

                        // If this is not the safe then default the safe
                        // to the remaining amount for deposit
                        if (empeeno != -1) forSafe = forDeposit - forBank;

                        e.ProposedValue = forBank;

                        BlockColChange = true;
                        Deposits[index][CN.SafeValue] = forSafe;
                        BlockColChange = false;
                    }

                    else if (Deposits.Table.Columns[CN.SafeValue].Ordinal == dgDeposits.CurrentCell.ColumnNumber)
                    {
                        decimal forBank = (decimal)Deposits[index][CN.BankedValue];
                        e.ProposedValue = decimal.Round((decimal)e.ProposedValue, 2); //IP - 20/09/2007
                        decimal forSafe = (decimal)e.ProposedValue;

                        if (forBank + forSafe > maxDeposit)
                            forSafe = forDeposit - forBank;
                        else if (forBank + forSafe < minDeposit && CurrentCashierTotalled && DepositIsOutstanding)
                        {
                            // A negative deposit can be made to the safe which will create a float
                            // and start a new cashier session. However, this is not allowed
                            // while there are any outstanding totalled deposits waiting to go
                            // to the safe or bank.
                            forSafe = forDeposit - forBank;
                            ShowInfo("M_MUSTDEPOSITOUTSTANDING");
                        }

                        e.ProposedValue = forSafe;
                    }
                }
            }
            catch (Exception ex)
            {
                Catch(ex, "OnColumnChange");
            }
            finally
            {
                StopWait();
            }

        }

        /// <summary>
        /// Some countries will not allow a cashier to run cashier totals while 
        /// they have money in the safe which they have deposited there throughout 
        /// the day. This is controlled by a country parameter. If it is the case,
        /// any money in the safe must be given back to the cashier before they can
        /// run cashier totals. This is done by right clicking on the "transferred to 
        /// safe" column in the upper data grid and selectinf the menu option there.
        /// This will cause the method below to run which will effectively reverse
        /// any deposits that the cashier has made to the safe since they last totalled.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnReverseSafeDeposit(object sender, System.EventArgs e)
        {
            try
            {
                Wait();
                // Reload the employees to make sure we have the up to date safe amount
                if (LoadEmployees(false))
                {
                    decimal safeBalance = 0;
                    decimal safeReverse = 0;
                    foreach (DataRowView r in (DataView)dgEmployee.DataSource)
                    {
                        if ((int)r[CN.EmployeeNo] == -1)
                        {
                            // Found the Safe
                            safeBalance = (decimal)r[CN.ForDeposit];
                        }
                        if ((int)r[CN.EmployeeNo] == CurrentCashier)
                        {
                            // Found the Cashier to be reversed
                            safeReverse = (decimal)r[CN.TransferredToSafe];
                        }
                    }

                    if (safeReverse > safeBalance)
                    {
                        ShowInfo("M_EXCEEDSSAFEBALANCE", new object[] { safeBalance.ToString(DecimalPlaces), safeReverse.ToString(DecimalPlaces), ((decimal)(safeReverse - safeBalance)).ToString(DecimalPlaces) });
                    }
                    else
                    {
                        PaymentManager.ReverseSafeDeposits(CurrentCashier, out Error);
                        if (Error.Length > 0)
                            ShowError(Error);
                        else
                            btnRefresh_Click(null, null);
                    }
                }
            }
            catch (Exception ex)
            {
                Catch(ex, "OnReverseSafeDeposit");
            }
            finally
            {
                StopWait();
            }
        }

        /// <summary>
        /// This is the handler that runs when the user clicks on the upper data grid.
        /// If it is a right-click they will be shown the "Reverse Safe Transfers" 
        /// context menu.
        /// Otherwise the lower data grid will be populated with amounts available 
        /// for deposit broken down by payment method for the cashier selected. The data
        /// is provided by the DN_CashierOutstandingIncomeGetByPayMethodSP which 
        /// contains further explanatory comments. 
        /// Some fields may be prepopulated according to what is permissible and 
        /// available. For example, the user cannot deposit to the bank if they have
        /// not totalled yet. There are other more specific rules for credit and debit 
        /// card transactions which effect the properties of the fields in this 
        /// datagrid.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgEmployee_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            try
            {
                Wait();

                int index = dgEmployee.CurrentRowIndex;
                var totForDeposit = 0.00;       // #9700 jec 28/02/12

                if (index >= 0)
                {
                    if (e.Button == MouseButtons.Right)
                    {
                        DataGrid ctl = (DataGrid)sender;

                        MenuCommand m1 = new MenuCommand(GetResource("P_REVERSEDEPOSIT"));
                        m1.Click += new System.EventHandler(this.OnReverseSafeDeposit);

                        PopupMenu popup = new PopupMenu();
                        popup.Animate = Animate.Yes;
                        popup.AnimateStyle = Animation.SlideHorVerPositive;

                        popup.MenuCommands.AddRange(new MenuCommand[] { m1 });
                        MenuCommand selected = popup.TrackPopup(ctl.PointToScreen(new Point(e.X, e.Y)));
                    }
                    else
                    {
                        dgEmployee.Select(index);
                        CurrentCashier = (int)((DataView)dgEmployee.DataSource)[index][CN.EmployeeNo];
                        CurrentCashierTotalled = "Y" == (string)((DataView)dgEmployee.DataSource)[index][CN.HasTotalled] ? true : false;
                        DepositIsOutstanding = false;
                        short branchno = (short)((DataRowView)drpBranchNo.SelectedItem)[CN.BranchNo];

                        /* get the stuff from the cashieroutstandingincome view 
                         * broken down by paymethod */
                        DataSet ds = PaymentManager.GetCashierOutstandingIncomeByPayMethod(CurrentCashier, branchno, out Error);
                        if (Error.Length > 0)
                            ShowError(Error);
                        else
                        {
                            DateTime dateDeposit = StaticDataManager.GetServerDateTime();
                            DataView dv = ds.Tables[TN.CashierOutstandingIncome].DefaultView;

                            DataTable dt = new DataTable(TN.CashierOutstandingIncome);
                            dt.Columns.AddRange(new DataColumn[] {	 new DataColumn(CN.CodeDescription),
																	 new DataColumn(CN.ForDeposit, Type.GetType("System.Decimal")),															 
																	 new DataColumn(CN.BankedValue, Type.GetType("System.Decimal")),
																	 new DataColumn(CN.SafeValue, Type.GetType("System.Decimal")),
																	 new DataColumn(CN.Reference),
																	 new DataColumn(CN.PayMethod),
																	 new DataColumn(CN.ReadOnly, Type.GetType("System.Int32")),
																	 new DataColumn(CN.DateDeposit, Type.GetType("System.DateTime")),
																	 new DataColumn(CN.TransTypeCode),
																	 new DataColumn(CN.Runno, Type.GetType("System.Int32")),
																	 new DataColumn(CN.EmployeeNo, Type.GetType("System.Int32")),
																	 new DataColumn(CN.EmployeeNoEntered, Type.GetType("System.Int32")),
																	 new DataColumn(CN.BranchNo, Type.GetType("System.Int16")),
																	 new DataColumn(CN.IncludeInCashierTotals, Type.GetType("System.Int16")),
																	 new DataColumn(CN.NegativeRef) });

                            foreach (DataRow r in ((DataTable)StaticData.Tables[TN.PayMethod]).Rows)
                            {
                                //if (((string)r[CN.Code]).Trim() != "0")	/* ignore 'not applicable */
                                if (((string)r[CN.Code]).Trim() != "0" && (Convert.ToString(r[CN.Additional2]).Length < 2 || Convert.ToString(r[CN.Additional2]).Substring(1) != "0")) //IP - 03/01/12 - #8473 - CR1234 - second digit if (1) then show	/* ignore 'not applicable */
                                {
                                    DataRow row = dt.NewRow();
                                    row[CN.PayMethod] = r[CN.Code];
                                    row[CN.CodeDescription] = r[CN.CodeDescription];
                                    row[CN.NegativeRef] = r[CN.Reference];

                                    decimal forDeposit = 0;
                                    dv.RowFilter = CN.Code + " = '" + ((string)r[CN.Code]).Trim() + "'";
                                    foreach (DataRowView drv in dv)
                                        forDeposit += (decimal)drv[CN.ForDeposit];


                                    DepositIsOutstanding = (DepositIsOutstanding || forDeposit != 0);
                                    row[CN.ForDeposit] = forDeposit;
                                    totForDeposit += (double)forDeposit;        // #9700 jec 28/02/12  total amounts shown in screen

                                    // FR68244 Electronic Credit Card always deposits to bank
                                    bool electronic = ((string)r[CN.Code] == "3" && (bool)Country[CountryParameterNames.CreditCardsElectronic]);
                                    // FR68244 Electronic Debit Card always deposits to bank
                                    electronic = electronic || ((string)r[CN.Code] == "4" && (bool)Country[CountryParameterNames.DebitCardsElectronic]);
                                    // Safe always deposits to bank
                                    bool safeCashier = (CurrentCashier == -1);

                                    row[CN.BankedValue] = 0;
                                    row[CN.SafeValue] = 0;

                                    if (safeCashier)
                                    {
                                        // Can only deposit to the bank
                                        row[CN.BankedValue] = forDeposit;
                                        // The safe is read only
                                        int mask = SetReadOnlyMask(4, 0);
                                        row[CN.ReadOnly] = mask;
                                    }
                                    else if (electronic)
                                    {
                                        // Can only deposit to the bank after totalled
                                        if (CurrentCashierTotalled) row[CN.BankedValue] = forDeposit;
                                        // The safe is read only
                                        int mask = SetReadOnlyMask(4, 0);
                                        // The bank is read only too
                                        mask = SetReadOnlyMask(3, mask);
                                        row[CN.ReadOnly] = mask;
                                    }
                                    else if (CurrentCashierTotalled)
                                    {
                                        // The user chooses where to deposit
                                        row[CN.ReadOnly] = 0;
                                    }
                                    else
                                    {
                                        // Otherwise only deposit to the safe
                                        row[CN.SafeValue] = forDeposit;
                                        // The bank is read only
                                        row[CN.ReadOnly] = SetReadOnlyMask(3, 0);
                                    }


                                    row[CN.Reference] = "";
                                    row[CN.DateDeposit] = dateDeposit;
                                    row[CN.TransTypeCode] = "";
                                    row[CN.Runno] = 0;
                                    row[CN.EmployeeNo] = CurrentCashier;
                                    row[CN.EmployeeNoEntered] = Credential.UserId;
                                    row[CN.BranchNo] = (short)((DataRowView)drpBranchNo.SelectedItem)[CN.BranchNo];
                                    row[CN.IncludeInCashierTotals] = CurrentCashierTotalled ? 0 : 1;

                                    dt.Rows.Add(row);
                                }
                            }
                            dt.ColumnChanging += new DataColumnChangeEventHandler(this.OnColumnChange);
                            dt.AcceptChanges();

                            //((DataView)dgEmployee.DataSource)[index][CN.ForDeposit] = totForDeposit;        // #9700 jec 28/02/12 set employee deposit to value totaled

                            dt.DefaultView.AllowNew = false;
                            dgDeposits.DataSource = Deposits = dt.DefaultView;
                            dgDeposits.TableStyles.Clear();

                            DataGridTableStyle tabStyle = new DataGridTableStyle();
                            tabStyle.MappingName = TN.CashierOutstandingIncome;

                            AddColumnStyle(CN.CodeDescription, tabStyle, 120, true, GetResource("T_PAYMETHOD"), "", HorizontalAlignment.Left);
                            AddColumnStyle(CN.ForDeposit, tabStyle, 100, true, GetResource("T_FORDEPOSIT"), DecimalPlaces, HorizontalAlignment.Right);

                            DataGridMultipleEditColumn aColumnEditColumn;

                            aColumnEditColumn = new DataGridMultipleEditColumn(CN.ReadOnly, tabStyle.GridColumnStyles.Count + 1);
                            aColumnEditColumn.MappingName = CN.BankedValue;
                            aColumnEditColumn.HeaderText = GetResource("T_AMOUNTBANKED");
                            aColumnEditColumn.Width = 100;
                            aColumnEditColumn.ReadOnly = false;
                            aColumnEditColumn.Format = DecimalPlaces;
                            aColumnEditColumn.Alignment = HorizontalAlignment.Right;
                            tabStyle.GridColumnStyles.Add(aColumnEditColumn);

                            aColumnEditColumn = new DataGridMultipleEditColumn(CN.ReadOnly, tabStyle.GridColumnStyles.Count + 1);
                            aColumnEditColumn.MappingName = CN.SafeValue;
                            aColumnEditColumn.HeaderText = GetResource("T_AMOUNTTOSAFE");
                            aColumnEditColumn.Width = 100;
                            aColumnEditColumn.ReadOnly = false;
                            aColumnEditColumn.Format = DecimalPlaces;
                            aColumnEditColumn.Alignment = HorizontalAlignment.Right;
                            tabStyle.GridColumnStyles.Add(aColumnEditColumn);

                            aColumnEditColumn = new DataGridMultipleEditColumn(CN.ReadOnly, tabStyle.GridColumnStyles.Count + 1);
                            aColumnEditColumn.MappingName = CN.Reference;
                            //UAT264 rdb 17/04/08 changing reson to reference
                            //aColumnEditColumn.HeaderText = GetResource("T_REASON");
                            aColumnEditColumn.HeaderText = GetResource("T_REFERENCE");
                            aColumnEditColumn.Width = 100;
                            aColumnEditColumn.ReadOnly = false;
                            tabStyle.GridColumnStyles.Add(aColumnEditColumn);

                            dgDeposits.TableStyles.Add(tabStyle);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Catch(ex, "dgEmployee_MouseUp");
            }
            finally
            {
                StopWait();
            }
        }

        /// <summary>
        /// This will clear everything the user has entered in the lower data grid. 
        /// It will not clear the whole screen as you might think.
        /// Note that it uses the BlockColChange variable again to prevent
        /// unnecessary validation of the perceived column changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClear_Click(object sender, System.EventArgs e)
        {
            try
            {
                Wait();

                if (Deposits != null)
                {
                    BlockColChange = true;

                    foreach (DataRowView r in Deposits)
                    {
                        r[CN.BankedValue] = 0;
                        r[CN.SafeValue] = 0;
                        r[CN.TransTypeCode] = "";
                        r[CN.Reference] = "";
                    }
                    Deposits.Table.AcceptChanges();

                    BlockColChange = false;
                }
            }
            catch (Exception ex)
            {
                Catch(ex, "btnClear_Click");
            }
            finally
            {
                StopWait();
            }
        }

        /// <summary>
        /// This will execute when the save button is clicked. All it does is
        /// validate that a unique reference has been added to the deposit if required.
        /// The actual saving takes place in the BuildCashierDepositsDataSet method below.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, System.EventArgs e)
        {
            try
            {
                //RM - removing richards incorrect change
                //System.Collections.Generic.List<string> references = new System.Collections.Generic.List<string>();

                Wait();

                DataSet ds = new DataSet();

                if (Deposits != null)
                {
                    /* do some validation */
                    int i = 0;
                    bool missing = false;
                    decimal safeTotal = 0;
                    while (i < Deposits.Count && !missing)
                    {
                        if ((decimal)Deposits[i][CN.BankedValue] != 0)
                        {
                            if ((string)Deposits[i][CN.Reference] == "")
                                missing = true;
                            else if ((bool)Country[CountryParameterNames.DepositUniqueReference])
                            {
                                //RM removing richards incorrect change
                                // uat279 rdb add new check if references we are adding are unique
                                //RM removing richards incorrect change
                                // uat279 rdb add new check if references we are adding are unique
                                //string newRef = Deposits[i][CN.Reference].ToString();
                                //string found = references.Find(delegate(string a) { return a == newRef; });
                                //if (found != null)
                                //{
                                //    ShowError("A Unique Reference must be used for each item.");
                                //    return;
                                //}
                                //else
                                //{
                                //    references.Add(newRef);
                                //}

                                missing = !PaymentManager.IsDepositReferenceUnique((string)Deposits[i][CN.Reference], out Error);
                                if (Error.Length > 0)
                                {
                                    ShowError(Error);
                                    return;
                                }
                            }
                        }
                        // Sum the amount to or from the safe with currency conversion
                        safeTotal += PaymentManager.CalcExchangeAmount(System.Convert.ToInt16(Deposits[i][CN.PayMethod]), 0, (decimal)Deposits[i][CN.SafeValue], out Error);
                        i++;
                    }

                    if (missing)
                    {
                        ShowInfo("M_CASHIERDEPOSITMISSINGINFO");
                    }
                    else
                    {
                        // Reload the employees to make sure we have the up to date safe amount
                        if (LoadEmployees(false))
                        {
                            decimal safeBalance = 0;
                            foreach (DataRowView r in (DataView)dgEmployee.DataSource)
                            {
                                if ((int)r[CN.EmployeeNo] == -1)
                                {
                                    // Found the Safe
                                    safeBalance = (decimal)r[CN.ForDeposit];
                                    break;
                                }
                            }

                            //IP - Livewire 69087 (19/09/2007)
                            //Rounding the safeTotal (amount deposited to the safe, and the amount)
                            //taken from the safe to 2dp.
                            Decimal dec = decimal.Round(safeTotal, 2);
                            if (dec < 0 && safeBalance < -dec)
                            {
                                ShowInfo("M_EXCEEDSSAFEBALANCE", new object[] { safeBalance.ToString(DecimalPlaces), (-safeTotal).ToString(DecimalPlaces), ((decimal)(-safeTotal - safeBalance)).ToString(DecimalPlaces) });
                            }
                            else
                            {
                                short branchno = (short)((DataRowView)drpBranchNo.SelectedItem)[CN.BranchNo];
                                DataSet depositSet = new DataSet();
                                DataTable depositList = Deposits.Table;
                                depositList.TableName = TN.CashierOutstandingIncome;
                                depositSet.Tables.Add(depositList);
                                PaymentManager.SaveCashierDeposits(branchno, CurrentCashier, depositSet, out Error);
                                if (Error.Length > 0)
                                    ShowError(Error);
                                else
                                {
                                    // rdb uat265 18/04/08  get menus back
                                    ((MainForm)FormRoot).CheckCashierDeposits();

                                    btnRefresh_Click(null, null);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Catch(ex, "btnSave_Click");
            }
            finally
            {
                StopWait();
            }
        }

        private void menuExit_Click(object sender, System.EventArgs e)
        {
            CloseTab();
        }

        private void menuCashierDepositsHelp_Click(object sender, System.EventArgs e)
        {
            CashierDeposits_HelpRequested(this, null);
        }

        private void CashierDeposits_HelpRequested(object sender, System.Windows.Forms.HelpEventArgs hlpevent)
        {
            string fileName = "Cashier_Deposits_Screen.htm";
            LaunchHelp(fileName);
        }

        private void CashierDeposits_Enter(object sender, EventArgs e)
        {
            // Must refresh to know whether the cashier is totalled
            try
            {
                Wait();
                LoadEmployees(true);
            }
            catch (Exception ex)
            {
                Catch(ex, "CashierDeposits_Enter");
            }
            finally
            {
                StopWait();
            }
        }


    }
}

