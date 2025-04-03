using System;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using STL.Common.Static;

namespace STL.PL
{
    /// <summary>
    /// A common authorisation popup. This is used from a main screen
    /// where additional authorisation is required, such as selling an
    /// out of stock item or manually entering a fee on a payment.
    /// A different employee with a user role that allows authorisation
    /// can enter their employee number and password.
    /// </summary>
    public class AuthorisePrompt : CommonForm
    {
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.TextBox txtUser;
        private System.Windows.Forms.Button btnCancel;
        public System.Windows.Forms.Button btnAuthorise;
        private System.Windows.Forms.Label lExplanation;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private IContainer components;
        private Control _toEnable = null;


        public bool Authorised = false;
        public int AuthorisedBy = 0;
        bool suspendEvents = false;
        private bool buttonClicked = false;

        public AuthorisePrompt(TranslationDummy d)
        {
            InitializeComponent();
        }

        public AuthorisePrompt(Form screen, Control toEnable, string explanation)
        {
            InitializeComponent();

            this.Name = screen.Name;
            _toEnable = toEnable;
            HashMenus(toEnable);
            lExplanation.Text = explanation;
            this.AuthorisedBy = Credential.UserId;
        }

        private void HashMenus(Control toEnable)
        {
            dynamicMenus = new Hashtable();
            dynamicMenus[this.Name + ":" + toEnable.Name] = toEnable;
        }

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AuthorisePrompt));
            this.btnAuthorise = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.txtUser = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.lExplanation = new System.Windows.Forms.Label();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // btnAuthorise
            // 
            this.btnAuthorise.Location = new System.Drawing.Point(119, 114);
            this.btnAuthorise.Name = "btnAuthorise";
            this.btnAuthorise.Size = new System.Drawing.Size(66, 23);
            this.btnAuthorise.TabIndex = 2;
            this.btnAuthorise.Text = "Authorise";
            this.btnAuthorise.Click += new System.EventHandler(this.btnAuthorise_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(189, 114);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(66, 23);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // txtPassword
            // 
            this.txtPassword.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.txtPassword.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.txtPassword.Location = new System.Drawing.Point(121, 88);
            this.txtPassword.MaxLength = 30;
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.PasswordChar = '*';
            this.txtPassword.Size = new System.Drawing.Size(134, 20);
            this.txtPassword.TabIndex = 1;
            this.txtPassword.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtPassword_KeyPress);
            // 
            // txtUser
            // 
            this.txtUser.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.txtUser.Location = new System.Drawing.Point(121, 64);
            this.txtUser.MaxLength = 30;
            this.txtUser.Name = "txtUser";
            this.txtUser.Size = new System.Drawing.Size(134, 20);
            this.txtUser.TabIndex = 0;
            this.txtUser.Tag = "";
            this.txtUser.Validating += new System.ComponentModel.CancelEventHandler(this.txtUser_Validating);
            // 
            // label3
            // 
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.label3.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label3.Location = new System.Drawing.Point(17, 90);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(101, 16);
            this.label3.TabIndex = 5;
            this.label3.Text = "Password";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label4
            // 
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.label4.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label4.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label4.Location = new System.Drawing.Point(17, 66);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(101, 16);
            this.label4.TabIndex = 4;
            this.label4.Text = "Employee Number";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lExplanation
            // 
            this.lExplanation.AutoEllipsis = true;
            this.lExplanation.BackColor = System.Drawing.SystemColors.Info;
            this.lExplanation.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lExplanation.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lExplanation.Location = new System.Drawing.Point(-1, -1);
            this.lExplanation.Name = "lExplanation";
            this.lExplanation.Padding = new System.Windows.Forms.Padding(4);
            this.lExplanation.Size = new System.Drawing.Size(302, 58);
            this.lExplanation.TabIndex = 0;
            this.lExplanation.Text = "Authorisation Required";
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // AuthorisePrompt
            // 
            this.AcceptButton = this.btnAuthorise;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.SystemColors.Control;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(300, 142);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnAuthorise);
            this.Controls.Add(this.txtPassword);
            this.Controls.Add(this.txtUser);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.lExplanation);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AuthorisePrompt";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Authorisation Required";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.AuthorisePrompt_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

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

        private void btnCancel_Click(object sender, System.EventArgs e)
        {
            Authorised = false;
            AuthorisedBy = 0;
            Close();
        }

        private void btnAuthorise_Click(object sender, System.EventArgs e)
        {
            if (txtUser.Text.Length == 0)
                return;


            try
            {
                Wait();


                // The CoSACS Employee No will be returned in case the user entered a FACT Employee No

                var userId = StaticDataManager.ControlPermissionPasswordCheck(txtUser.Text, txtPassword.Text, this.Name, _toEnable.Name);

                if (!userId.HasValue)
                {
                    FailedLogin("Login password incorrect or user does not have permission.");
                }
                else
                {
                    buttonClicked = true;

                    // Make sure we use the CoSACS Employee No

                    AuthorisedBy = userId.Value;
                    Authorised = true;
                    Close();
                }

                StopWait();
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
        }

        private void FailedLogin(string message)
        {
            txtPassword.Text = "";
            ShowError(message);
        }

   
        private void txtPassword_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
                btnAuthorise_Click(null, null);
        }

        private void AuthorisePrompt_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (buttonClicked == false)
            {
                Authorised = false;
                AuthorisedBy = 0;
            }
        }
    }
}
