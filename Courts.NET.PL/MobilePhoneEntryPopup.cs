using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace STL.PL
{
    public partial class MobilePhoneEntryPopup : Form
    {
        private bool _ok = false;
        public bool OK
        {
            get { return _ok; }
        }
        public string Mobile1
        {
            get { return txtMobile1.Text; } 
            set { 
                txtMobile1.Text = value; 
            } 
        }
        
        public string Mobile2
        {
            get { return txtMobile2.Text; }
            set { txtMobile2.Text = value; }
        }
        
        public string Mobile3
        {
            get { return txtMobile3.Text; }
            set { txtMobile3.Text = value; } 
        }

        public MobilePhoneEntryPopup(bool readOnly)
        {
            InitializeComponent();
            txtMobile1.ReadOnly = readOnly;
            txtMobile2.ReadOnly = readOnly;
            txtMobile3.ReadOnly = readOnly;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            this._ok = true;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this._ok = false;
            this.Close();
        }
    }
}