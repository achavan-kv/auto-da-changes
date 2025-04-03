using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using STL.PL.Collections.CollectionsClasses;
using STL.Common.Constants.ColumnNames;

namespace STL.PL.Collections
{
    public partial class SMSSetup : CommonForm
    {
        private string error = "";
       StrategyConfigPopulation stratConfig = new StrategyConfigPopulation();
       WorkListData workListData = new WorkListData();
       DataTable dtSMS = new DataTable();

        const string customerName = " {CN} ";
        const string accountNo = " {AN} ";
        const string arrears = " {AR} ";
        const string dueDate = " {DD} ";
        const string instalment = " {IN} ";
        const string arrears_and_fee = " {AF} ";
        const string ptpInstalment = " {PT} ";      //UAT1043 jec 08/07/10
        const string ptpDueDate = " {PD} ";         //UAT1043 jec 08/07/10
        const string balIncCharges = " {BIC} ";         //CR1084 jec 13/08/10
        const string balExcCharges = " {BEC} ";         //CR1084 jec 13/08/10
        const string totFeeCharges = " {FC} ";         //CR1084 jec 13/08/10
        const string fees = " {FEE} ";         //CR1084 jec 13/08/10
        const string interest = " {INT} ";         //CR1084 jec 13/08/10
        const string adminFee = " {ADM} ";         //CR1084 jec 13/08/10

        public SMSSetup(Form root, Form parent)
        {
            InitializeComponent();
            
            FormRoot = root;
            FormParent = parent;
        }

        private void btnName_Click(object sender, EventArgs e)
        {
            rtxtSMS.Text += customerName;
            rtxtSMS.Focus();
            rtxtSMS.Select(rtxtSMS.Text.Length, 0);
        }

        private void btnAccountNo_Click(object sender, EventArgs e)
        {
            rtxtSMS.Text += accountNo;
            rtxtSMS.Focus();
            rtxtSMS.Select(rtxtSMS.Text.Length, 0);
        }

        private void btnArrears_Click(object sender, EventArgs e)
        {
            rtxtSMS.Text += arrears;
            rtxtSMS.Focus();
            rtxtSMS.Select(rtxtSMS.Text.Length, 0);
        }

        private void btnArrearsPlusFee_Click(object sender, EventArgs e)
        {
            rtxtSMS.Text += arrears_and_fee;
            rtxtSMS.Focus();
            rtxtSMS.Select(rtxtSMS.Text.Length, 0);
        }

        private void btnDueDate_Click(object sender, EventArgs e)
        {
            rtxtSMS.Text += dueDate;
            rtxtSMS.Focus();
            rtxtSMS.Select(rtxtSMS.Text.Length, 0);
        }

        private void btnInstalment_Click(object sender, EventArgs e)
        {
            rtxtSMS.Text += instalment;
            rtxtSMS.Focus();
            rtxtSMS.Select(rtxtSMS.Text.Length, 0);
        }

        //UAT1043 jec 08/07/10
        private void btnPTPInstal_Click(object sender, EventArgs e)
        {
            rtxtSMS.Text += ptpInstalment;
            rtxtSMS.Focus();
            rtxtSMS.Select(rtxtSMS.Text.Length, 0);
        }

        private void btnPTPDueDate_Click(object sender, EventArgs e)
        {
            rtxtSMS.Text += ptpDueDate;
            rtxtSMS.Focus();
            rtxtSMS.Select(rtxtSMS.Text.Length, 0);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                bool valid = true;

                errorProvider1.SetError(txtSMSName, "");
                errorProvider1.SetError(rtxtSMS, "");

                if (txtSMSName.Text.Length == 0 && txtSMSName.Enabled == true)
                {
                    errorProvider1.SetError(txtSMSName, GetResource("M_ENTERMANDATORY"));
                    valid = false;
                }
                else
                {
                    errorProvider1.SetError(txtSMSName, "");
                }


                if (txtDescription.Text.Length == 0 && txtDescription.Enabled == true)
                {
                    errorProvider1.SetError(txtDescription, GetResource("M_ENTERMANDATORY"));
                    valid = false;
                }
                else
                {
                    errorProvider1.SetError(txtDescription, "");
                }

                if (rtxtSMS.Text.Length == 0)
                {
                    errorProvider1.SetError(rtxtSMS, GetResource("M_ENTERMANDATORY"));
                    valid = false;
                }
                else
                {
                    errorProvider1.SetError(rtxtSMS, "");
                }

                if (valid)
                {
                   if (txtSMSName.Enabled == true)
                   {
                      CollectionsManager.SaveSMS(txtSMSName.Text, rtxtSMS.Text, txtDescription.Text, out Error);
                      LoadSMS(txtSMSName.Text);
                   }
                   else
                   {
                       CollectionsManager.SaveSMS(drpSMS.SelectedValue.ToString(), rtxtSMS.Text, txtDescription.Text, out Error);
                      LoadSMS(drpSMS.SelectedValue.ToString());
                   }

                    if (error.Length > 0)
                        ShowError(error);
                    else
                    {
                        txtSMSName.Text = "";
                        txtSMSName.Enabled = false;
                        txtDescription.Enabled = false;
                        //rtxtSMS.Text = "";
                        drpSMS.Enabled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
        }

       private void SMSSetup_Load(object sender, EventArgs e)
       {
          try
          {
             LoadSMS(String.Empty);
          }
          catch (Exception ex)
          {
             Catch(ex, Function);
          }
       }

       private void btnSMS_Click(object sender, EventArgs e)
       {
           txtSMSName.Enabled = true;
           txtSMSName.Text = String.Empty;
           txtSMSName.Focus();
           txtDescription.Enabled = true;
           txtDescription.Text = String.Empty;
           drpSMS.Enabled = false;
          rtxtSMS.Text = String.Empty;
       }

       private void LoadSMS(string sms)
       {
           dtSMS = stratConfig.GetSMS();

           drpSMS.DataSource = dtSMS;
           drpSMS.DisplayMember = CN.SMSName;
           drpSMS.ValueMember = CN.SMSName;

           //IP - 20/01/10 - UAT(972)
           if (drpSMS.Items.Count == 0)
           {
               btnDeleteSMS.Enabled = false;
           }
           else
           {
               btnDeleteSMS.Enabled = true;
           }


           if (dtSMS.Rows.Count > 0) //IP - 20/01/10 - UAT(972)
           {
               if (sms == String.Empty)
               {
                   rtxtSMS.Text = dtSMS.Rows[0][CN.SMSText].ToString();
                   txtDescription.Text = dtSMS.Rows[0][CN.Description].ToString();
               }
               else
               {
                   foreach (DataRow row in dtSMS.Rows)
                   {
                       if (row[CN.SMSName].ToString() == sms)
                       {
                           rtxtSMS.Text = row[CN.SMSText].ToString();
                           txtDescription.Text = row[CN.Description].ToString();
                       }
                   }
                   drpSMS.SelectedValue = sms;
               }
           }
       }

       private void drpSMS_SelectionChangeCommitted(object sender, EventArgs e)
       {
          LoadSMS(drpSMS.SelectedValue.ToString());
       }

       private void btnDeleteSMS_Click(object sender, EventArgs e)
       {
          Function = "btnDeleteSMS_Click";
          try
          {
             if (DialogResult.Yes == ShowInfo("M_DELETESMS", MessageBoxButtons.YesNo))
             {
                workListData.DeleteSMS(drpSMS.SelectedValue.ToString());

                //IP - 20/01/10 - UAT(972)
                txtDescription.Text = String.Empty;
                rtxtSMS.Text = String.Empty;

                LoadSMS(String.Empty);
             }
          }
          catch (Exception ex)
          {
             Catch(ex, Function);
          }
          finally
          {
             Function = "End of btnDeleteSMS_Click";
          }
       }

       private void txtSMSName_Leave(object sender, EventArgs e)
       {
           // Limit SMS code to 4 characters - its a code not description - UAT1007 jec 01/03/10
           if (txtSMSName.Text.Length > 4)
           {
               errorProvider1.SetError(txtSMSName, "SMS code must not exceed 4 characters");
           }
           else
           {
               errorProvider1.SetError(txtSMSName, "");
           }
       }

       private void btnBalIncChgs_Click(object sender, EventArgs e)
       {
           rtxtSMS.Text += balIncCharges;
           rtxtSMS.Focus();
           rtxtSMS.Select(rtxtSMS.Text.Length, 0);
       }

       private void btnBalExcChgs_Click(object sender, EventArgs e)
       {
           rtxtSMS.Text += balExcCharges;
           rtxtSMS.Focus();
           rtxtSMS.Select(rtxtSMS.Text.Length, 0);
       }

       private void btnFeeIncChgs_Click(object sender, EventArgs e)
       {
           rtxtSMS.Text += totFeeCharges;
           rtxtSMS.Focus();
           rtxtSMS.Select(rtxtSMS.Text.Length, 0);
       }

       private void btnFees_Click(object sender, EventArgs e)
       {
           rtxtSMS.Text += fees;
           rtxtSMS.Focus();
           rtxtSMS.Select(rtxtSMS.Text.Length, 0);
       }

       private void btnInterest_Click(object sender, EventArgs e)
       {
           rtxtSMS.Text += interest;
           rtxtSMS.Focus();
           rtxtSMS.Select(rtxtSMS.Text.Length, 0);
       }

       private void btnAdminFees_Click(object sender, EventArgs e)
       {
           rtxtSMS.Text += adminFee;
           rtxtSMS.Focus();
           rtxtSMS.Select(rtxtSMS.Text.Length, 0);
       }
              
    }
}
