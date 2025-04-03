using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using STL.Common.Static;

namespace STL.PL
{
    public partial class ManualReferral : CommonForm
    {
        private string custId = string.Empty;
        private string acctNo = string.Empty;
        private DateTime dateProp;
        decimal creditLimit = 0;
        protected new string Error = "";
        public bool refer = false; //IP - 23/03/11 - CR1245

        public ManualReferral(string customerID, string accountNo, DateTime dateProp, decimal creditLimit)
        {
            InitializeComponent();

            refer = false; //IP - 23/03/11 - CR1245

            this.custId = customerID;
            this.acctNo = accountNo;
            this.dateProp = dateProp;
            this.creditLimit = creditLimit;
            
        }

        private void btnSave_Click(object sender, EventArgs e)
        {

            if (referralNotes.Text.Length > 0) //IP - 18/03/11 - Make entering notes mandatory
            {
                refer = true; //IP - 23/03/11 - CR1245

                CreditManager.SaveManualReferralNotes(custId, acctNo, dateProp, referralNotes.Text, creditLimit, Config.CountryCode, out Error);

                if (Error.Length > 0)
                    ShowError(Error);
                else
                {
                    this.Close();
                }
            }
            else
            {
                errorProvider1.SetError(referralNotes, GetResource("M_ENTERMANDATORY"));
            }
        }


        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close(); //IP - 23/03/11 - CR1245
        }
    }
}
