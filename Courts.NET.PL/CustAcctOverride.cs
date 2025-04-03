using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using STL.Common;
using STL.PL.WS1;
using STL.PL.WS2;
using System.Web.Services.Protocols;
using STL.Common.Static;

namespace STL.PL
{
    /// <summary>
    /// An authorisation popup to allow an account change its association
    /// with one customer to a different customer. An employee with a user
    /// role that allows customer account override authorisation can enter
    /// employee number and password to authorise the update.
    /// </summary>
    public class CustAcctOverride : CommonForm
    {
        private System.Windows.Forms.GroupBox grpWarning;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox grpOverride;
        private System.Windows.Forms.Button btnAuthorise;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.TextBox txtUser;
        private new string Error = "";
        private string _olduser = "";
        public string OldUser
        {
            get { return _olduser; }
            set { _olduser = value; }
        }
        private string _oldpass = "";

        public string OldPassword
        {
            get { return _oldpass; }
            set { _oldpass = value; }
        }
        private string[] _oldroles = null;
        private System.Windows.Forms.Label txtAccountNumber;
        private System.Windows.Forms.Label txtCustomerID;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label txtNewCustID;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private IContainer components;

        public string[] OldRoles
        {
            get { return _oldroles; }
            set { _oldroles = value; }
        }

        public CustAcctOverride(TranslationDummy d)
        {
            InitializeComponent();
        }

        public CustAcctOverride(string accountNo, string custid, string newCustId, System.Windows.Forms.Form par)
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();
            dynamicMenus = new Hashtable();
            ApplyRoleRestrictions();
            this.FormParent = par;
            txtAccountNumber.Text = accountNo;
            txtCustomerID.Text = custid;
            txtNewCustID.Text = newCustId;
            TranslateControls();
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CustAcctOverride));
            this.grpWarning = new System.Windows.Forms.GroupBox();
            this.txtNewCustID = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.txtCustomerID = new System.Windows.Forms.Label();
            this.txtAccountNumber = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.grpOverride = new System.Windows.Forms.GroupBox();
            this.btnAuthorise = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.txtUser = new System.Windows.Forms.TextBox();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.grpWarning.SuspendLayout();
            this.grpOverride.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // grpWarning
            // 
            this.grpWarning.BackColor = System.Drawing.SystemColors.Control;
            this.grpWarning.Controls.Add(this.txtNewCustID);
            this.grpWarning.Controls.Add(this.label5);
            this.grpWarning.Controls.Add(this.txtCustomerID);
            this.grpWarning.Controls.Add(this.txtAccountNumber);
            this.grpWarning.Controls.Add(this.label2);
            this.grpWarning.Controls.Add(this.label1);
            this.grpWarning.Controls.Add(this.richTextBox1);
            this.grpWarning.Location = new System.Drawing.Point(8, 8);
            this.grpWarning.Name = "grpWarning";
            this.grpWarning.Size = new System.Drawing.Size(240, 208);
            this.grpWarning.TabIndex = 0;
            this.grpWarning.TabStop = false;
            this.grpWarning.Text = "Warning";
            // 
            // txtNewCustID
            // 
            this.txtNewCustID.BackColor = System.Drawing.SystemColors.Window;
            this.txtNewCustID.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtNewCustID.ForeColor = System.Drawing.SystemColors.Highlight;
            this.txtNewCustID.Location = new System.Drawing.Point(40, 160);
            this.txtNewCustID.Name = "txtNewCustID";
            this.txtNewCustID.Size = new System.Drawing.Size(160, 16);
            this.txtNewCustID.TabIndex = 7;
            this.txtNewCustID.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label5
            // 
            this.label5.BackColor = System.Drawing.SystemColors.Window;
            this.label5.Location = new System.Drawing.Point(48, 136);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(144, 24);
            this.label5.TabIndex = 6;
            this.label5.Text = "you are trying to associate it with Customer ID:";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txtCustomerID
            // 
            this.txtCustomerID.BackColor = System.Drawing.SystemColors.Window;
            this.txtCustomerID.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCustomerID.ForeColor = System.Drawing.SystemColors.Highlight;
            this.txtCustomerID.Location = new System.Drawing.Point(40, 104);
            this.txtCustomerID.Name = "txtCustomerID";
            this.txtCustomerID.Size = new System.Drawing.Size(160, 16);
            this.txtCustomerID.TabIndex = 5;
            this.txtCustomerID.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txtAccountNumber
            // 
            this.txtAccountNumber.BackColor = System.Drawing.SystemColors.Window;
            this.txtAccountNumber.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtAccountNumber.ForeColor = System.Drawing.SystemColors.Highlight;
            this.txtAccountNumber.Location = new System.Drawing.Point(56, 48);
            this.txtAccountNumber.Name = "txtAccountNumber";
            this.txtAccountNumber.Size = new System.Drawing.Size(128, 16);
            this.txtAccountNumber.TabIndex = 4;
            this.txtAccountNumber.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.SystemColors.Window;
            this.label2.Location = new System.Drawing.Point(48, 80);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(144, 24);
            this.label2.TabIndex = 3;
            this.label2.Text = "already associated with Customer ID:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.SystemColors.Window;
            this.label1.Location = new System.Drawing.Point(88, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 16);
            this.label1.TabIndex = 1;
            this.label1.Text = "Account No:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // richTextBox1
            // 
            this.richTextBox1.Location = new System.Drawing.Point(8, 16);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.ReadOnly = true;
            this.richTextBox1.Size = new System.Drawing.Size(224, 184);
            this.richTextBox1.TabIndex = 0;
            this.richTextBox1.TabStop = false;
            this.richTextBox1.Text = "";
            // 
            // grpOverride
            // 
            this.grpOverride.BackColor = System.Drawing.SystemColors.Control;
            this.grpOverride.Controls.Add(this.btnAuthorise);
            this.grpOverride.Controls.Add(this.label3);
            this.grpOverride.Controls.Add(this.label4);
            this.grpOverride.Controls.Add(this.txtPassword);
            this.grpOverride.Controls.Add(this.txtUser);
            this.grpOverride.Location = new System.Drawing.Point(8, 224);
            this.grpOverride.Name = "grpOverride";
            this.grpOverride.Size = new System.Drawing.Size(240, 112);
            this.grpOverride.TabIndex = 1;
            this.grpOverride.TabStop = false;
            this.grpOverride.Text = "Override";
            // 
            // btnAuthorise
            // 
            this.btnAuthorise.Location = new System.Drawing.Point(159, 78);
            this.btnAuthorise.Name = "btnAuthorise";
            this.btnAuthorise.Size = new System.Drawing.Size(64, 23);
            this.btnAuthorise.TabIndex = 2;
            this.btnAuthorise.Text = "Override";
            this.btnAuthorise.Click += new System.EventHandler(this.btnAuthorise_Click);
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(24, 64);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(56, 16);
            this.label3.TabIndex = 9;
            this.label3.Text = "Password:";
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(24, 24);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(104, 16);
            this.label4.TabIndex = 8;
            this.label4.Text = "Employee Number:";
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(24, 80);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.PasswordChar = '*';
            this.txtPassword.Size = new System.Drawing.Size(100, 20);
            this.txtPassword.TabIndex = 1;
            // 
            // txtUser
            // 
            this.txtUser.Location = new System.Drawing.Point(24, 40);
            this.txtUser.Name = "txtUser";
            this.txtUser.Size = new System.Drawing.Size(100, 20);
            this.txtUser.TabIndex = 0;
            this.txtUser.Validating += new System.ComponentModel.CancelEventHandler(this.txtUser_Validating);
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // CustAcctOverride
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.SystemColors.ControlDark;
            this.ClientSize = new System.Drawing.Size(256, 341);
            this.Controls.Add(this.grpOverride);
            this.Controls.Add(this.grpWarning);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "CustAcctOverride";
            this.ShowInTaskbar = false;
            this.Text = "Link Customer To Account";
            this.Closing += new System.ComponentModel.CancelEventHandler(this.CustAcctOverride_Closing);
            this.grpWarning.ResumeLayout(false);
            this.grpOverride.ResumeLayout(false);
            this.grpOverride.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);

        }
        #endregion

        private void btnAuthorise_Click(object sender, System.EventArgs e)
		{
			Function = "BLogin::IsValid()";
			if(txtUser.Text.Length!=0)
			{
				try
				{
					Wait();

                    if (!StaticDataManager.ControlPermissionPasswordCheck(txtUser.Text, txtPassword.Text, this.Name, "lAuthorise").HasValue)
                    {
                        MessageBox.Show("User is not authorised or password is incorrect.", "User not Authorised", MessageBoxButtons.OK);
                    }
                    else
                    {
                        AddCustomerToAccount();
                    }
				}
				catch(Exception ex)
				{
					Catch(ex, Function);
				}
				finally
				{
					StopWait();
				}
			}
		}

        /// <summary>
        /// Delete any existing customer links and then establish the new one
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddCustomerToAccount()
        {
            Function = "WAccountManager::AddCustomerToAccount()";

            try
            {
                Wait();

                bool locked = AccountManager.LockAccount(txtAccountNumber.Text.Replace("-", ""),
                        Credential.UserId.ToString(), out Error);
                if (Error.Length > 0)
                    ShowError(Error);
                else
                {
                    if (locked)
                    {
                        bool rescore = false;
                        AccountManager.AddCustomerToAccount(txtAccountNumber.Text.Replace("-", ""),
                                                            txtNewCustID.Text,
                                                            "H",
                                                            "", 0,
                                                            out rescore,
                                                            out Error);
                        if (Error.Length > 0)
                            ShowError(Error);
                        else
                        {
                            AccountManager.UnlockAccount(txtAccountNumber.Text.Replace("-", ""),
                                                            Credential.UserId, out Error);
                            if (Error.Length > 0)
                                ShowError(Error);
                            else
                            {
                                // CR-02 Implement Audit on Customer account linking-2/07/2020   
                                AccountManager.AuditCustomerAccountLinking(txtAccountNumber.Text.Replace("-", ""), txtCustomerID.Text,
                                                              txtNewCustID.Text, txtUser.Text, out Error);
                                Close();
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
                StopWait();
            }
        }

        private void CustAcctOverride_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
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
                //e.Cancel = true;
                txtUser.Focus();
                txtUser.Select(0, txtUser.Text.Length);
                errorProvider1.SetError(txtUser, ex.Message);
            }
        }

    }
}
