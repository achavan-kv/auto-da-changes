using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Symbol.Barcode2;

namespace Blue.Cosacs.StockCountApp
{
    public partial class SettingsForm : Form
    {
        private readonly SettingsRepository _repo;

        public SettingsForm()
        {            
            InitializeComponent();            
            _repo = new SettingsRepository();  
        }              

        private void buttonConfirm_Click(object sender, EventArgs e)
        {
            Save();
        }

        private void CheckEnterKeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {                
                Save();
            }
        }

        private void buttonBack_Click(object sender, EventArgs e)
        {
            if (IsValid()) 
            {
                this.DialogResult = DialogResult.Yes;
                this.Close();
            }
        }

        private void Save()
        {
            ShowLoader();
            if (IsValid())
            {
                Settings.AuthHost = "http://" + textBoxHost.Text;
                _repo.Save(Settings.AuthHost);                        
            }
            HideLoader();
        } 

        private void textBoxHost_TextChanged(object sender, EventArgs e)
        {
            Validate(textBoxHost);
        }    

        private void Validate(TextBox textBox)
        {
            if (string.IsNullOrEmpty(textBox.Text))
            {
                this.UIThread(() =>
                {
                    textBox.BackColor = Color.FromArgb(254, 214, 209);
                });
            }
            else
            {
                this.UIThread(() =>
                {
                    textBox.BackColor = Color.White;
                });
            }
        }

        private bool IsValid()
        {
            return !string.IsNullOrEmpty(textBoxHost.Text);
        }
        
        private void ShowLoader()
        {
            this.UIThread(() => { 
                labelLoading.Visible = true;
                labelLoading.Refresh();
            });            
        }

        private void HideLoader()
        {
            this.UIThread(() =>
            {
                labelLoading.Visible = false;
                labelLoading.Refresh();
            });                
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            var settings = _repo.Get();
            if (settings != null)
            {             
                textBoxHost.Text = settings.Host.Replace("http://","");
            }

            Validate(textBoxHost);            
        }    
    }
}