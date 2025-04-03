using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using BBSL.Libraries.Printing.PrintDocuments;
using BBSL.Libraries.Printing;
using STL.Common;
using STL.Common.Static;

namespace STL.PL
{
    public partial class StatementsForm : CommonForm
    {
        Crownwood.Magic.Menus.MenuCommand menuFile = new Crownwood.Magic.Menus.MenuCommand();
        Crownwood.Magic.Menus.MenuCommand menuExit = new Crownwood.Magic.Menus.MenuCommand();
        Statements statements = new Statements();

        public StatementsForm(Form root)
        {
            FormRoot = root;

            //
            // Required for Windows Form Designer support
            //
            MyInitializeComponent();
            ApplyRoleRestrictions();
            statements.AccountNotFoundEvent += AccountNotFoundEventHandler;
            statements.CustomerNotFoundEvent += CustomerNotFoundEventHandler;
            statements.ExceptionEvent += ExceptionEventHandler;
            statements.LotsOfTransactionsEvent += LotsOfTransactionsEventHandler;
        }
        public StatementsForm(Form root, string accountNo)
            : this(root)
        {
            textBoxAccountNo.Text = accountNo;
        }
        public StatementsForm(string customerID, Form root)
            : this(root)
        {
            textBoxCustomerID.Text = customerID;
        }
        public StatementsForm(string customerID, string accountNo, Form root)
            : this(root)
        {
            textBoxCustomerID.Text = customerID;
            textBoxAccountNo.Text = accountNo;
        }
        void MyInitializeComponent()
        {
            InitializeComponent();
            this.menuFile.Description = "MenuItem";
            this.menuFile.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {
            this.menuExit});
            this.menuFile.Text = "&File";
            this.menuExit.Description = "MenuItem";
            this.menuExit.Text = "E&xit";
            this.menuExit.Click += new System.EventHandler(this.btnClose_Click);            
            menuMain = new Crownwood.Magic.Menus.MenuControl();
            menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {menuFile});
            //dateFromPrintStatementForAccount.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month - 1, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
            dateFromPrintStatementForAccount.Value = DateTime.Now.AddMonths(-1);  //IP - 12/01/12 - #9402
            dateToPrintStatementForAccount.Value = DateTime.Now;
            //dateFromPrintStatementForCustomer.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month - 1, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
            dateFromPrintStatementForCustomer.Value = DateTime.Now.AddMonths(-1); //IP - 12/01/12 - #9402
            dateToPrintStatementForCustomer.Value = DateTime.Now;
            comboBoxAccountsFilter.SelectedIndex = 0;
        }
        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
            statements.AccountNotFoundEvent -= AccountNotFoundEventHandler;
            statements.CustomerNotFoundEvent -= CustomerNotFoundEventHandler;
            statements.ExceptionEvent -= ExceptionEventHandler;
            statements.LotsOfTransactionsEvent -= LotsOfTransactionsEventHandler;
            Function = "";
        }

        void btnClose_Click(object sender, System.EventArgs e)
        {
            CloseTab();
        }

        void buttonPrintStatementForAccount_Click(object sender, EventArgs e)
        {
            Function = "buttonPrintStatementForAccount_Click";
            try
            {
                statements.PrintStatementForAccount
                    (
                        textBoxAccountNo.Text, 
                        dateFromPrintStatementForAccount.Value, 
                        dateToPrintStatementForAccount.Value,
                        AccountManager, 
                        Country,
                        false
                    );
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
        }

        private void buttonPreviewPrintStatementForAccount_Click(object sender, EventArgs e)
        {
            Function = "buttonPreviewPrintStatementForAccount_Click";
            try
            {
                statements.PrintStatementForAccount
                    (
                        textBoxAccountNo.Text,
                        dateFromPrintStatementForAccount.Value,
                        dateToPrintStatementForAccount.Value,
                        AccountManager,
                        Country,
                        true
                    );
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
        }

        private void buttonPrintStatementForCustomer_Click(object sender, EventArgs e)
        {
            Function = "buttonPrintStatementForCustomer_Click";
            try
            {
                statements.PrintStatementForCustomer
                    (
                        textBoxCustomerID.Text, 
                        dateFromPrintStatementForCustomer.Value, 
                        dateToPrintStatementForCustomer.Value, 
                        AccountManager, Country,
                        onlyHolderAccounts,
                        false
                    );
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
        }

        private void buttonPreviewPrintStatementForCustomer_Click(object sender, EventArgs e)
        {
            Function = "buttonPrintStatementForCustomer_Click";
            try
            {
                statements.PrintStatementForCustomer
                    (
                        textBoxCustomerID.Text,
                        dateFromPrintStatementForCustomer.Value,
                        dateToPrintStatementForCustomer.Value,
                        AccountManager, Country,
                        onlyHolderAccounts,
                        true
                    );
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
        }

        void AccountNotFoundEventHandler(String error)
        {
            this.Invoke(new MethodInvoker(delegate
            {
                MessageBox.Show
                    (
                        "Account Not Found\n" + error,
                        "Statements", 
                        MessageBoxButtons.OK, 
                        MessageBoxIcon.Exclamation
                    );
                textBoxAccountNo.SelectAll();
                textBoxAccountNo.Focus();
            }));
        }
        void CustomerNotFoundEventHandler(String error)
        {
            this.Invoke(new MethodInvoker(delegate
            {
                MessageBox.Show
                    (
                        "Customer Not Found\n" + error, 
                        "Statements", 
                        MessageBoxButtons.OK, 
                        MessageBoxIcon.Exclamation
                    );
                textBoxCustomerID.SelectAll();
                textBoxCustomerID.Focus();
            }));
        }
        void ExceptionEventHandler(Exception ex)
        {
            this.Invoke(new MethodInvoker(delegate
            {
                Catch(ex, Function);
            }));
        }
        void LotsOfTransactionsEventHandler(int noOfTransactions)
        {
            this.Invoke(new MethodInvoker(delegate
            {
                DialogResult dialogResult = MessageBox.Show
                    (
                        "There are " + noOfTransactions + " transactions to print.\n\nDo you want to continue?",
                        "Statements",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning
                    );

                if (dialogResult == DialogResult.No)
                    statements.Print = false;
            }));
        }

        private void textBox_TextChanged(object sender, EventArgs e)
        {
            TextBox textBox = (TextBox)sender;

            if (textBox == textBoxAccountNo)
            {
                buttonPrintStatementForAccount.Enabled = 
                    buttonPreviewPrintStatementForAccount.Enabled = !string.IsNullOrEmpty(textBoxAccountNo.Text);
            }
            else if (textBox == textBoxCustomerID)
            {
                buttonPrintStatementForCustomer.Enabled = 
                    buttonPreviewPrintStatementForCustomer.Enabled = !string.IsNullOrEmpty(textBoxCustomerID.Text);
            }
        }

        bool onlyHolderAccounts
        {
            get
            {
                return comboBoxAccountsFilter.SelectedIndex == 0;
            }
        }
    }
}
