using System;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using STL.Common.Static;

namespace STL.PL
{
    public partial class DeleteVouchers : CommonForm
    {
        private Control _toEnable = null;
        bool suspendEvents = false;
        string OldUser = "";
        string OldPassword = "";
        string[] OldRoles = null;
        public bool Authorised = false;
        public int AuthorisedBy = 0;
        public bool deleted = false;
        string OldName = "";
        new string Error = "";

        public DeleteVouchers(Form root, Form parent, Control toEnable)
        {
            InitializeComponent();
            FormRoot = root;
            FormParent = parent;

            this.Name = parent.Name;
            _toEnable = toEnable;
            HashMenus(toEnable);

            dtEndDate.Value = DateTime.Now.AddMonths(-2);
        }

        private void HashMenus(Control toEnable)
        {
            dynamicMenus = new Hashtable();
            dynamicMenus[this.Name + ":" + toEnable.Name] = toEnable;
        }


        private void txtPassword_Validating(object sender, CancelEventArgs e)
        {
            if (txtUser.Text.Length != 0 && !suspendEvents)
            {
                try
                {
                    Wait();
                    var userId = StaticDataManager.ControlPermissionPasswordCheck(txtUser.Text, txtPassword.Text, this.Name, _toEnable.Name);
                    if (userId.HasValue)
                    {

                        Authorised = btnAuthorise.Enabled = _toEnable.Enabled;
                        if (Authorised)
                        {
                            AuthorisedBy = userId.Value;
                            suspendEvents = true;
                            btnAuthorise.Focus();
                            suspendEvents = false;
                        }
                    }
                    StopWait();
                }
                catch (Exception ex)
                {
                    Catch(ex, Function);
                }
            }
        }

        private void txtPassword_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
                txtPassword_Validating(null, null);
        }

        private void txtUser_Validating(object sender, CancelEventArgs e)
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

        private void btnAuthorise_Click(object sender, EventArgs e)
        {
            lEndDate.Enabled = true;
            dtEndDate.Enabled = true;
            btnDelete.Enabled = true;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                Wait();
                CustomerManager.DeletePrizeVouchers(dtEndDate.Value, "", false, out Error);
                if (Error.Length > 0)
                    ShowError(Error);
                else
                    deleted = true;
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
            finally
            {
                StopWait();
                Close();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}