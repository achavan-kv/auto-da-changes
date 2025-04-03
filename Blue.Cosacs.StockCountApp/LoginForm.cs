using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Blue.Cosacs.StockCountApp
{
    public partial class LoginForm : Form
    {
        private UserRepository _userRepo;

        public LoginForm()
        {
            InitializeComponent();
            _userRepo = new UserRepository();
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            var previousUser = _userRepo.Get();
            if (previousUser != null)
            {
                this.UIThread(() => { 
                    textBoxUser.Text = previousUser.Id;
                    textBoxPassword.Focus();
                });
            }
            else
            {
                this.UIThread(() => textBoxUser.Focus());                
            }            
        }

        private void buttonLogin_Click(object sender, EventArgs e)
        {
            ShowLoader();
            try
            {
                if (RequestManager.Login(textBoxUser.Text, textBoxPassword.Text))
                {
                    _userRepo.Save(textBoxUser.Text); 
                    this.UIThreadInvoke(() =>
                    {
                        this.DialogResult = DialogResult.Yes;
                        this.Close();
                    });
                }
                else
                {
                    this.UIThread(() => MessageBox.Show("Invalid login. Please check your details and try again."));
                }
            }
            catch (Exception ex)
            {
                this.UIThread(() =>MessageBox.Show("An error occurred while logging in - " + ex.Message));
            }
            HideLoader();
        }

        private void CheckEnterKeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
            {
                buttonLogin_Click(sender, e);
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.UIThreadInvoke(() =>
            {
                this.DialogResult = DialogResult.No;
                this.Close();
            });
        }

        private void ShowLoader()
        {
            this.UIThread(()=> 
            {
                labelLoading.Visible = true;
                labelLoading.Refresh();
            });
        }

        private void HideLoader()
        {
            this.UIThread(() =>
            { 
                labelLoading.Visible = false;
            });
        }         
    }
}
