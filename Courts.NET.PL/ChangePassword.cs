using System;
using System.Windows.Forms;
using STL.Common.Static;

namespace STL.PL
{
    public class ChangePassword : CommonForm
    {
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtNewPassword;
        private System.Windows.Forms.TextBox txtConfirm;
        private System.Windows.Forms.TextBox txtOldPassword;
        private System.Windows.Forms.TextBox txtEmpNo;
        private System.Windows.Forms.TextBox txtEmpName;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.ErrorProvider errorProvider1;

        private System.ComponentModel.Container components = null;

        private string oldPassword = "";
        bool passwordCheck = true;
        bool saveRecord = true;
        bool oldPasswordCheck = true;
        private System.Windows.Forms.Button button1;
        private string err = "";
        public bool status = false;

        public ChangePassword(TranslationDummy d)
        {
            InitializeComponent();
        }

        public ChangePassword(Form root, Form parent, string password, string name, string empeeNo)
        {
            InitializeComponent();

            FormRoot = root;
            FormParent = parent;
            txtEmpNo.Text = empeeNo;
            txtEmpName.Text = name;
            oldPassword = password;

            txtOldPassword.Enabled = password.Length > 0;
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
            System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(ChangePassword));
            this.txtNewPassword = new System.Windows.Forms.TextBox();
            this.txtConfirm = new System.Windows.Forms.TextBox();
            this.txtOldPassword = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtEmpNo = new System.Windows.Forms.TextBox();
            this.txtEmpName = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.button1 = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtNewPassword
            // 
            this.txtNewPassword.Location = new System.Drawing.Point(152, 72);
            this.txtNewPassword.Name = "txtNewPassword";
            this.txtNewPassword.PasswordChar = '*';
            this.txtNewPassword.Size = new System.Drawing.Size(80, 20);
            this.txtNewPassword.TabIndex = 2;
            this.txtNewPassword.Text = "";
            // 
            // txtConfirm
            // 
            this.txtConfirm.Location = new System.Drawing.Point(152, 112);
            this.txtConfirm.Name = "txtConfirm";
            this.txtConfirm.PasswordChar = '*';
            this.txtConfirm.Size = new System.Drawing.Size(80, 20);
            this.txtConfirm.TabIndex = 3;
            this.txtConfirm.Text = "";
            this.txtConfirm.Leave += new System.EventHandler(this.txtConfirm_Leave);
            // 
            // txtOldPassword
            // 
            this.txtOldPassword.Location = new System.Drawing.Point(152, 32);
            this.txtOldPassword.Name = "txtOldPassword";
            this.txtOldPassword.PasswordChar = '*';
            this.txtOldPassword.Size = new System.Drawing.Size(80, 20);
            this.txtOldPassword.TabIndex = 1;
            this.txtOldPassword.Text = "";
            this.txtOldPassword.Leave += new System.EventHandler(this.txtOldPassword_Leave);
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(8, 112);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(128, 16);
            this.label3.TabIndex = 92;
            this.label3.Text = "Confirm New Password";
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(8, 72);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(80, 16);
            this.label1.TabIndex = 93;
            this.label1.Text = "New Password";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(8, 32);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(80, 16);
            this.label2.TabIndex = 94;
            this.label2.Text = "Old Password";
            // 
            // txtEmpNo
            // 
            this.txtEmpNo.Location = new System.Drawing.Point(104, 16);
            this.txtEmpNo.Name = "txtEmpNo";
            this.txtEmpNo.ReadOnly = true;
            this.txtEmpNo.Size = new System.Drawing.Size(80, 20);
            this.txtEmpNo.TabIndex = 95;
            this.txtEmpNo.TabStop = false;
            this.txtEmpNo.Text = "";
            // 
            // txtEmpName
            // 
            this.txtEmpName.Location = new System.Drawing.Point(104, 48);
            this.txtEmpName.Name = "txtEmpName";
            this.txtEmpName.ReadOnly = true;
            this.txtEmpName.Size = new System.Drawing.Size(168, 20);
            this.txtEmpName.TabIndex = 96;
            this.txtEmpName.TabStop = false;
            this.txtEmpName.Text = "";
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(8, 16);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(80, 16);
            this.label4.TabIndex = 97;
            this.label4.Text = "Employee No";
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(8, 48);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(80, 16);
            this.label5.TabIndex = 98;
            this.label5.Text = "Name";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.AddRange(new System.Windows.Forms.Control[] {
																					this.button1,
																					this.btnSave,
																					this.label2,
																					this.txtConfirm,
																					this.label3,
																					this.label1,
																					this.txtNewPassword,
																					this.txtOldPassword});
            this.groupBox1.Location = new System.Drawing.Point(0, 88);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(344, 152);
            this.groupBox1.TabIndex = 99;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Details";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(272, 40);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(56, 24);
            this.button1.TabIndex = 96;
            this.button1.Text = "Clear";
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnSave
            // 
            this.btnSave.BackgroundImage = ((System.Drawing.Bitmap)(resources.GetObject("btnSave.BackgroundImage")));
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnSave.Location = new System.Drawing.Point(288, 96);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(24, 24);
            this.btnSave.TabIndex = 95;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // errorProvider1
            // 
            this.errorProvider1.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
            this.errorProvider1.DataMember = null;
            // 
            // ChangePassword
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(352, 253);
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.label5,
																		  this.label4,
																		  this.txtEmpName,
																		  this.txtEmpNo,
																		  this.groupBox1});
            this.Name = "ChangePassword";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "ChangePassword";
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion

        private void txtOldPassword_Leave(object sender, System.EventArgs e)
        {
            //if (txtOldPassword.Text.ToLower() != oldPassword.ToLower())
            //{
            //    ShowInfo("M_OLDPASSWORDCHECKFAILED");
            //    oldPasswordCheck = false;
            //    button1_Click(this, null);
            //    txtOldPassword.Focus();
            //}
            //else
            //    oldPasswordCheck = true;
        }

        private void txtConfirm_Leave(object sender, System.EventArgs e)
        {
            //if (oldPasswordCheck)
            //{
            //    if (txtNewPassword.Text.ToLower() != txtConfirm.Text.ToLower()) //WHY case insensitive ????
            //    {
            //        ShowInfo("M_PASSWORDCONFIRMMISMATCH");
            //        passwordCheck = false;
            //        button1_Click(this, null);
            //        txtOldPassword.Focus();
            //    }
            //    else
            //        passwordCheck = true;
            //}
        }

        //private struct ValidatePasswordResult
        //{
        //    public bool IsValid
        //    {
        //        get;
        //        set;
        //    }

        //    public string ErrorMessage
        //    {
        //        get;
        //        set;
        //    }
        //}

        //private ValidatePasswordResult ValidatePassword()
        //{
        //    var returnValue = new ValidatePasswordResult();

        //    var userValidator = new Blue.Admin.UserPasswordValidation();

        //    if (userValidator.Validate(this.txtEmpNo.Text, this.txtOldPassword.Text).IsValid)
        //    {
        //        if (txtNewPassword.Text != txtConfirm.Text)
        //        {
        //            this.errorProvider1.SetError(this.txtNewPassword, "The passwords you entered did not match");
        //        }
        //        else
        //        {
        //            var changePassword = new new Blue.Admin.ChangePassword();
        //        }
        //    }

        //    return returnValue;
        //}

        private void btnSave_Click(object sender, System.EventArgs e)
        {
            try
            {
                this.errorProvider1.Clear();
                if (txtNewPassword.Text != txtConfirm.Text)
                {
                    this.errorProvider1.SetError(this.txtNewPassword, "The passwords you entered did not match");
                }
                else
                {
                    var result = this.Login.SavePassword(this.txtNewPassword.Text, this.txtOldPassword.Text, this.txtEmpNo.Text, Credential.User);

                    if (result.IsValid)
                    {
                        Credential.Password = txtNewPassword.Text.ToLower();
                        Credential.User = txtEmpNo.Text;
                        status = true;
                        Close();
                    }
                    else
                    {
                        this.errorProvider1.SetError(this.txtOldPassword, result.ErrorMessage);
                    }
                }

            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
        }

        private void button1_Click(object sender, System.EventArgs e)
        {
            txtOldPassword.Text = "";
            txtNewPassword.Text = "";
            txtConfirm.Text = "";
        }
    }
}
