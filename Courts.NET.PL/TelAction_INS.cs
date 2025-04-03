using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using STL.Common;
using System.Data;
using STL.Common.Constants.ColumnNames;
using STL.Common.Static;
using System.Xml;
using STL.Common.Constants.TableNames;
using STL.Common.Constants.AccountTypes;
using STL.Common.Constants.RateTypes;

namespace STL.PL
{
	public partial class TelAction_INS : CommonForm
	{
        //---Constants--------------------------------------
        const string INSTYPE_NULLVALUE = "NOT KNOWN";   //This will be used as default value for INSTYPE dropdown (cmbInsType)
        const string CLAIMTYPE_NULLVALUE = "NOT KNOWN"; //This will be used as default value for CLAIMTYPE dropdown (cmbClaimType)
        //--------------------------------------------------
        
        private DataTable m_dtInsuranceDetails;
        private DialogResult m_buttonClicked = DialogResult.None;

        public DialogResult ButtonClicked
        {
            get
            {
                return m_buttonClicked;
            }
        }

        public TelAction_INS(DataTable dtInsuranceDetails)
		{
            m_dtInsuranceDetails = dtInsuranceDetails;
            m_dtInsuranceDetails.AcceptChanges();
            InitializeComponent();
		}

        private void TelAction_INS_Load(object sender, EventArgs e)
        {
            try
            {
                PopulateComboBoxes();

                if (m_dtInsuranceDetails.Rows.Count == 0)
                    ClearControls();
                else
                {
                    DataRow dr = m_dtInsuranceDetails.Rows[0];  

                    cmbInsType.SelectedValue = dr[CN.CMInsType] != DBNull.Value ? dr[CN.CMInsType].ToString() : "";
                    dtpInitiatedDate.Value = dr[CN.CMInitiatedDate] != DBNull.Value ? Convert.ToDateTime(dr[CN.CMInitiatedDate]) : new DateTime(1900, 1, 1);
                    txtInsAmount.Text = dr[CN.CMInsAmount] != DBNull.Value ? MoneyStrToDecimal(dr[CN.CMInsAmount].ToString()).ToString() : "";
                    cmbClaimType.Text = dr[CN.CMFullOrPartClaim] != DBNull.Value ? dr[CN.CMFullOrPartClaim].ToString() : CLAIMTYPE_NULLVALUE;
                    txtUserNotes.Text = dr[CN.CMUserNotes] != DBNull.Value ? dr[CN.CMUserNotes].ToString() : "";
                    chkInsApproved.Checked = dr[CN.CMIsApproved] != DBNull.Value ? Convert.ToBoolean(dr[CN.CMIsApproved]) : false;
                }
            }
            catch (Exception ex)
            {
                Catch(ex, "TelAction_INS_Load");
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {       
            if (ValidateFields() == false)
                return;

            try
            {
                DataRow dr;
                if (m_dtInsuranceDetails.Rows.Count == 0)
                {
                    dr = m_dtInsuranceDetails.NewRow();
                    m_dtInsuranceDetails.Rows.Add(dr);
                }
                else
                {
                    dr = m_dtInsuranceDetails.Rows[0];
                }

                //---------------------------------------------------------
                if (cmbInsType.Text.Trim() == INSTYPE_NULLVALUE)
                    dr[CN.CMInsType] = DBNull.Value;
                else
                    dr[CN.CMInsType] = cmbInsType.SelectedValue.ToString();
                
                if(dtpInitiatedDate.Value.ToShortDateString() == "01/01/1900")
                    dr[CN.CMInitiatedDate] = DBNull.Value;
                else
                    dr[CN.CMInitiatedDate] = dtpInitiatedDate.Value.ToShortDateString();
                
                if(txtInsAmount.Text.Trim() == "")
                    dr[CN.CMInsAmount] = DBNull.Value;
                else
                    dr[CN.CMInsAmount] = MoneyStrToDecimal(txtInsAmount.Text.Trim());

                if (cmbClaimType.Text.Trim() == CLAIMTYPE_NULLVALUE)
                    dr[CN.CMFullOrPartClaim] = DBNull.Value;
                else
                    dr[CN.CMFullOrPartClaim] = cmbClaimType.Text.Trim();

                dr[CN.CMUserNotes]   = txtUserNotes.Text.Trim();
                dr[CN.CMIsApproved]  = chkInsApproved.Checked;
                //---------------------------------------------------------
                
                m_dtInsuranceDetails.AcceptChanges();
                m_buttonClicked = DialogResult.OK;
            }
            catch (Exception ex)
            {
                Catch(ex, "btnOK_Click");
            }

            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            m_dtInsuranceDetails.RejectChanges();
            m_buttonClicked = DialogResult.Cancel;
            this.Close();  
        }

        private void ClearControls()
        {
            foreach (Control control in gbMain.Controls)
            {
                if (control is TextBox)
                {
                    (control as TextBox).Text = "";
                }
                else if (control is DateTimePicker)
                {
                    (control as DateTimePicker).Value = new DateTime(1900, 1, 1);
                }
                else if (control is CheckBox)
                {
                    (control as CheckBox).Checked = false;
                }
                else if (control is ComboBox)
                {
                    if((control as ComboBox).Items.Count > 0)
                        (control as ComboBox).SelectedIndex = 0;
                }
            }
        }

        #region -- DateTimePicker Null Value Trick-----------------
        //---------------------------------------------------------
        //These stuff should be handled and moved to a separate proper custom-control class
        private void dtpInitiatedDate_ValueChanged(object sender, EventArgs e)
        {
            dtpInitiatedDate_Hider.Visible = (dtpInitiatedDate.Value.Date == Date.blankDate);
        }

        private void dtpInitiatedDate_Enter(object sender, EventArgs e)
        {
            dtpInitiatedDate_Hider.Visible = false;
            if(dtpInitiatedDate.Value == Date.blankDate)
                dtpInitiatedDate.Value = DateTime.Today;
        }

        private void dateTimePicker_KeyUp(object sender, KeyEventArgs e)
        {
            if (sender is DateTimePicker && e.KeyCode == Keys.Delete)
            {
                (sender as DateTimePicker).Value = new DateTime(1900, 1, 1);
            }
        }
        //---------------------------------------------------------
        # endregion

        private void PopulateComboBoxes()
        {
            cmbClaimType.Items.Add(CLAIMTYPE_NULLVALUE);
            cmbClaimType.Items.Add("FULL");
            cmbClaimType.Items.Add("PART");

            //cmbInsType.Items.Add(INSTYPE_NULLVALUE);
            
            cmbInsType.DataSource = (DataTable)StaticData.Tables[TN.InsuranceTypes];
            cmbInsType.DisplayMember = CN.CodeDescript;
            cmbInsType.ValueMember = CN.Code;
        }

        private bool ValidateFields()
        {
            //---Clear Error Provider---------------------------
            errProvider.SetError(cmbInsType, "");
            errProvider.SetError(dtpInitiatedDate, "");
            errProvider.SetError(txtInsAmount, "");
            //--------------------------------------------------

            bool rtnValue = true;

            //-----Checking for mandatory fields---------------- 
            if (cmbInsType.Text.Trim() == INSTYPE_NULLVALUE)
            {
                rtnValue = false;
                errProvider.SetError(cmbInsType, GetResource("M_SELECTMANDATORY"));
            }
            if (dtpInitiatedDate.Value == Date.blankDate)
            {
                rtnValue = false;
                errProvider.SetError(dtpInitiatedDate, GetResource("M_ENTERMANDATORY"));
            }
            //--------------------------------------------------
            
            //-----Checking for correct values in the fields----
            double dblValue = 0;

            if (txtInsAmount.Text.Trim() != "")
            {
                try
                {
                    dblValue = Double.Parse(txtInsAmount.Text.Trim());
                }
                catch
                {
                    rtnValue = false;
                    errProvider.SetError(txtInsAmount, GetResource("M_NUMERICVALUE"));
                }
            }
            //--------------------------------------------------

            return rtnValue;
        }
        
	}
}

