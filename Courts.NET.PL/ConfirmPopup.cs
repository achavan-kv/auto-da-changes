using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace STL.PL
{
    public partial class ConfirmPopup : Form
    {
        
        /// <summary>
        /// Generic Info Popup Screen - you supply ConfirmText and then the User enters data in TextBox
        /// Returns 0 for OK and 1 for Cancel -- By default Return =1;
        /// </summary>
        public bool Return = false;
        public ConfirmPopup(string ConfirmText, out string TextResult, out bool Return)
        {
            InitializeComponent();
            this.Text = ConfirmText;
            TextResult = this.textBox.Text;
            Return = false;
        }
        

        private void buttonOK_Click(object sender, EventArgs e)
        {
            Return = true;
            Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Return = false;
            Close();
        }

        private void textBox_TextChanged(object sender, EventArgs e)
        {   // ensure a valid entry made and remove leading/embedded blanks
            if (textBox.Text.Trim().Length != 0)
            {
                buttonCancel.Enabled = true;
                buttonOK.Enabled = true;
                textBox.Text = textBox.Text.Trim();
                textBox.Text = textBox.Text.Replace(" ", "");
            }
            else
            {
                buttonCancel.Enabled = false;
                buttonOK.Enabled = false;
            }   
        }
    
    }
}