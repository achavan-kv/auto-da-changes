using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using STL.Common;

namespace STL.PL
{
    public partial class ViewSMSPopup : CommonForm
    {
        string text = "";
        public ViewSMSPopup(string text)
        {
            InitializeComponent();
            this.text = text;
        }

        private void ViewSMSPopup_Load(object sender, EventArgs e)
        {
            txtText.Text = text;
            txtText.SelectionStart = txtText.Text.Length; //To unselect the highlighted text
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
                this.Close();
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
