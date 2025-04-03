using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using STL.Common.Constants.ColumnNames;
using STL.Common.Static;
using STL.Common.Constants.TableNames;

namespace STL.PL
{
    public partial class BankMaintenance : CommonForm
    {
        private string error = string.Empty;
        DataView dvBank = new DataView();

        public BankMaintenance(Form root, Form parent)
        {
            FormRoot = root;
            FormParent = parent;

            InitializeComponent();
            LoadStaticData();
        }

        //Exit the Bank Maintenance screen.
        private void btnExit_Click(object sender, EventArgs e)
        {
            CloseTab();
        }

        //Clearing bank address details
        private void btnClear_Click(object sender, EventArgs e)
        {
            Clear();  
        }

        //Loading bank data
        private void LoadStaticData()
        {
            try
            {
                DataSet bankSet = new DataSet();
                bankSet = StaticDataManager.GetBankDetails(out error);

                if (error.Length > 0)
                {
                    ShowError(error);
                }
                else
                {
                    dvBank = bankSet.Tables[TN.Bank].DefaultView;
                    drpBankName.DataSource = dvBank;
                    drpBankName.DisplayMember = CN.BankName;
                    drpBankName.ValueMember = CN.BankCode;
                }

            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
        }

        //Looping through data view to load bank address details
        private void drpBankName_SelectionChangeCommitted(object sender, EventArgs e)
        {
            foreach(DataRowView row in dvBank)
            {
                if (drpBankName.SelectedValue.ToString() == (string)row[CN.BankCode])
                {
                    txtBankCode.Text = (string)row[CN.BankCode];
                    txtBankName.Text = (string)row[CN.BankName];
                    txtBankAddress1.Text = (string)row[CN.BankAddress1];
                    txtBankAddress2.Text = (string)row[CN.BankAddress2];
                    txtBankAddress3.Text = (string)row[CN.BankAddress3];
                    txtPostCode.Text = (string)row[CN.BankPostCode];
                }
            }
        }

       
        /// <summary>
        /// Click event to save new or update existing Bank details
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, EventArgs e)
        {

            bool valid = true;

            try
            {
                Wait();

                if (txtBankCode.Text.Equals(string.Empty))
                {
                    errorProvider1.SetError(txtBankCode, GetResource("M_ENTERMANDATORY"));
                    valid = false;
                }

                if (txtBankName.Text.Equals(string.Empty))
                {
                    errorProvider1.SetError(txtBankName, GetResource("M_ENTERMANDATORY"));
                    valid = false;
                }

                if (valid)
                {
      
                    StaticDataManager.UpdateBank(this.txtBankCode.Text, this.txtBankName.Text, this.txtBankAddress1.Text,
                        this.txtBankAddress2.Text, this.txtBankAddress3.Text, this.txtPostCode.Text, out error);

                    if (Error.Length > 0)
                    {
                        ShowError(Error);
                    }
                    else
                    {
                        Clear();
                        LoadStaticData();
                    }
                }
            }
            catch (Exception ex)
            {
                Catch(ex, "btnSave_Click");
            }
            finally
            {
                StopWait();
            }

        }
        
        /// <summary>
        /// Click event to delete Bank details.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {

                Wait();

                    StaticDataManager.DeleteBank(this.txtBankCode.Text, out error);

                    if (Error.Length > 0)
                    {
                        ShowError(Error);
                    }
                    else
                    {
                        Clear();
                        LoadStaticData();
                    }
              
            }
            catch (Exception ex)
            {
                Catch(ex, "btnDelete_Click");
            }
            finally
            {
                StopWait();
            }

        }

        /// <summary>
        /// Clears all text boxes
        /// </summary>
        private void Clear()
        {
            txtBankCode.Text = string.Empty;
            txtBankName.Text = string.Empty;
            txtBankAddress1.Text = string.Empty;
            txtBankAddress2.Text = string.Empty;
            txtBankAddress3.Text = string.Empty;
            txtPostCode.Text = string.Empty;
        }

        private void txtBankCode_Leave(object sender, EventArgs e)
        {

            try
            {
                Wait();

                bool bankLoaded = false;
                foreach (DataRowView row in dvBank)
                {
                    if (txtBankCode.Text.ToUpper() == ((string)row[CN.BankCode]).ToUpper())
                    {
                        bankLoaded = true;
                        drpBankName.Text = (string)row[CN.BankName];
                        txtBankName.Text = (string)row[CN.BankName];
                        txtBankAddress1.Text = (string)row[CN.BankAddress1];
                        txtBankAddress2.Text = (string)row[CN.BankAddress2];
                        txtBankAddress3.Text = (string)row[CN.BankAddress3];
                        txtPostCode.Text = (string)row[CN.BankPostCode];
                    }
                }

                if (!bankLoaded && txtBankCode.Text!= string.Empty)
                {
                    txtBankName.Text = string.Empty;
                    txtBankAddress1.Text = string.Empty;
                    txtBankAddress2.Text = string.Empty;
                    txtBankAddress3.Text = string.Empty;
                    txtPostCode.Text = string.Empty;

                    MessageBox.Show(this, "Bank not found!", "warning");
                    
                }
     
            }
            catch (Exception ex)
            {
                Catch(ex, "txtBankCode_Leave");
            }
            finally
            {
                StopWait();
            }
            
        }

        private void txtBankCode_TextChanged(object sender, EventArgs e)
        {
            errorProvider1.SetError(this.txtBankCode, "");
        }

        private void txtBankName_TextChanged(object sender, EventArgs e)
        {
            errorProvider1.SetError(this.txtBankName, "");
        }

    }
}