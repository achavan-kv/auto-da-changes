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
	public partial class TelAction_LEG : CommonForm
    {
        private DataTable m_dtLegalDetails;
        private DialogResult m_buttonClicked = DialogResult.None;

        public DialogResult ButtonClicked
        {
            get
            {
                return m_buttonClicked;
            }
        }
        
        public TelAction_LEG(DataTable dtLegalDetails)
		{
            m_dtLegalDetails = dtLegalDetails;
            InitializeComponent();
		}

        private void TelAction_LEG_Load(object sender, EventArgs e)
        {
            try
            {
                if (m_dtLegalDetails.Rows.Count == 0)
                    ClearControls();
                else
                {
                    DataRow dr = m_dtLegalDetails.Rows[0];
                    txtSolicitorNo.Text = dr[CN.CMSolicitorNo] != DBNull.Value ? dr[CN.CMSolicitorNo].ToString() : "";
                    dtpAucDate.Value = dr[CN.CMAuctionDate] != DBNull.Value ? Convert.ToDateTime(dr[CN.CMAuctionDate]) : new DateTime(1900,1,1);
                    txtAucProceed.Text = dr[CN.CMAuctionProceeds] != DBNull.Value ? MoneyStrToDecimal(dr[CN.CMAuctionProceeds].ToString()).ToString() : "";
                    txtAucAmount.Text = dr[CN.CMAuctionAmount] != DBNull.Value ? MoneyStrToDecimal(dr[CN.CMAuctionAmount].ToString()).ToString() : "";
                    dtpMentionDate.Value = dr[CN.CMMentionDate] != DBNull.Value ? Convert.ToDateTime(dr[CN.CMMentionDate]) : new DateTime(1900,1,1);
                    txtMentionCost.Text = dr[CN.CMMentionCost] != DBNull.Value ? MoneyStrToDecimal(dr[CN.CMMentionCost].ToString()).ToString() : "";
                    dtpDefaultedDate.Value = dr[CN.CMDefaultedDate] != DBNull.Value ? Convert.ToDateTime(dr[CN.CMDefaultedDate]) : new DateTime(1900,1,1);
                    txtCourtAmount.Text = dr[CN.CMCourtAmount] != DBNull.Value ? MoneyStrToDecimal(dr[CN.CMCourtAmount].ToString()).ToString() : "";
                    dtpCourtDate.Value = dr[CN.CMCourtDate] != DBNull.Value ? Convert.ToDateTime(dr[CN.CMCourtDate]) : new DateTime(1900,1,1);
                    txtCourtDeposit.Text = dr[CN.CMCourtDeposit] != DBNull.Value ? MoneyStrToDecimal(dr[CN.CMCourtDeposit].ToString()).ToString() : "";
                    txtRemittance.Text = dr[CN.CMPaymentRemittance] != DBNull.Value ? MoneyStrToDecimal(dr[CN.CMPaymentRemittance].ToString()).ToString() : "";
                    dtpLegalAtchDate.Value = dr[CN.CMLegalAttachmentDate] != DBNull.Value ? Convert.ToDateTime(dr[CN.CMLegalAttachmentDate]) : new DateTime(1900,1,1);
                    dtpLegalIniDate.Value = dr[CN.CMLegalInitiatedDate] != DBNull.Value ? Convert.ToDateTime(dr[CN.CMLegalInitiatedDate]) : new DateTime(1900,1,1);
                    txtUserNotes.Text = dr[CN.CMUserNotes] != DBNull.Value ? dr[CN.CMUserNotes].ToString() : "";
                    txtJudgement.Text = dr[CN.CMJudgement] != DBNull.Value ? dr[CN.CMJudgement].ToString() : "";
                    chkCaseClosed.Checked = dr[CN.CMCaseClosed] != DBNull.Value ? Convert.ToBoolean(dr[CN.CMCaseClosed]) : false;
                }
            }
            catch(Exception ex)
			{
                Catch(ex, "TelAction_LEG_Load");
			}
        }

        //private void btnOK_Click(object sender, EventArgs e)
        //{
        //    //if (ValidateFields() == false)
        //    //    return;

        //    //try
        //    //{
        //    //    DataRow dr;
        //    //    if (m_dtLegalDetails.Rows.Count == 0)
        //    //    {
        //    //        dr = m_dtLegalDetails.NewRow();
        //    //        m_dtLegalDetails.Rows.Add(dr);
        //    //    }
        //    //    else
        //    //    {
        //    //        dr = m_dtLegalDetails.Rows[0];
        //    //    }

        //    //    //---------------------------------------------------------
        //    //    dr[CN.CMSolicitorNo] = txtSolicitorNo.Text;

        //    //    if (dtpAucDate.Value.ToShortDateString() == "01/01/1900")
        //    //        dr[CN.CMAuctionDate] = DBNull.Value; 
        //    //    else
        //    //        dr[CN.CMAuctionDate] = dtpAucDate.Value.ToShortDateString();

        //    //    if (txtAucProceed.Text.Trim() == "")
        //    //        dr[CN.CMAuctionProceeds] = DBNull.Value; 
        //    //    else
        //    //        dr[CN.CMAuctionProceeds] = MoneyStrToDecimal(txtAucProceed.Text.Trim());

        //    //    if (txtAucAmount.Text.Trim() == "")
        //    //        dr[CN.CMAuctionAmount] = DBNull.Value;
        //    //    else
        //    //        dr[CN.CMAuctionAmount] = MoneyStrToDecimal(txtAucAmount.Text.Trim());

        //    //    if (dtpMentionDate.Value.ToShortDateString() == "01/01/1900")
        //    //        dr[CN.CMMentionDate] = DBNull.Value;
        //    //    else
        //    //        dr[CN.CMMentionDate] = dtpMentionDate.Value.ToShortDateString();

        //    //    if (txtMentionCost.Text.Trim() == "")
        //    //        dr[CN.CMMentionCost] = DBNull.Value;
        //    //    else
        //    //        dr[CN.CMMentionCost] = MoneyStrToDecimal(txtMentionCost.Text.Trim());

        //    //    if (dtpDefaultedDate.Value.ToShortDateString() == "01/01/1900")
        //    //        dr[CN.CMDefaultedDate] = DBNull.Value;
        //    //    else
        //    //        dr[CN.CMDefaultedDate] = dtpDefaultedDate.Value.ToShortDateString();

        //    //    if (txtCourtAmount.Text.Trim() == "")
        //    //        dr[CN.CMCourtAmount] = DBNull.Value;
        //    //    else
        //    //        dr[CN.CMCourtAmount] = MoneyStrToDecimal(txtCourtAmount.Text.Trim());

        //    //    if (dtpCourtDate.Value.ToShortDateString() == "01/01/1900")
        //    //        dr[CN.CMCourtDate] = DBNull.Value;
        //    //    else
        //    //        dr[CN.CMCourtDate] = dtpCourtDate.Value.ToShortDateString();

        //    //    if (txtCourtDeposit.Text.Trim() == "")
        //    //        dr[CN.CMCourtDeposit] = DBNull.Value;
        //    //    else
        //    //        dr[CN.CMCourtDeposit] = MoneyStrToDecimal(txtCourtDeposit.Text.Trim());

        //    //    if (txtRemittance.Text.Trim() == "")
        //    //        dr[CN.CMPaymentRemittance] = DBNull.Value;
        //    //    else
        //    //        dr[CN.CMPaymentRemittance] = MoneyStrToDecimal(txtRemittance.Text.Trim());

        //    //    if (dtpLegalAtchDate.Value.ToShortDateString() == "01/01/1900")
        //    //        dr[CN.CMLegalAttachmentDate] = DBNull.Value;
        //    //    else
        //    //        dr[CN.CMLegalAttachmentDate] = dtpLegalAtchDate.Value.ToShortDateString();

        //    //    if (dtpLegalIniDate.Value.ToShortDateString() == "01/01/1900")
        //    //        dr[CN.CMLegalInitiatedDate] = DBNull.Value;
        //    //    else
        //    //        dr[CN.CMLegalInitiatedDate] = dtpLegalIniDate.Value.ToShortDateString();

        //    //    dr[CN.CMUserNotes] = txtUserNotes.Text.Trim();
        //    //    dr[CN.CMJudgement] = txtJudgement.Text.Trim();
        //    //    dr[CN.CMCaseClosed] = chkCaseClosed.Checked;
        //    //    //---------------------------------------------------------
        //    //    m_dtLegalDetails.AcceptChanges();
        //    //}
        //    //catch (Exception ex)
        //    //{
        //    //    Catch(ex, "btnOK_Click");
        //    //}

        //    //this.Close();
        //}

        private void btnCancel_Click(object sender, EventArgs e)
        {
            m_dtLegalDetails.RejectChanges();
            m_buttonClicked = DialogResult.Cancel;
            this.Close();            
        }

        #region -- DateTimePicker Null Value Trick-----------------
        //---------------------------------------------------------
        //These stuff should be handled and moved to a separate proper custom-control class
        private void dtpCourtDate_ValueChanged(object sender, EventArgs e)
        {
            dtpCourtDate_Hider.Visible = (dtpCourtDate.Value.Date == Date.blankDate);
        }

        private void dtpAucDate_ValueChanged(object sender, EventArgs e)
        {
            dtpAucDate_Hider.Visible = (dtpAucDate.Value.Date == Date.blankDate);
        }

        private void dtpMentionDate_ValueChanged(object sender, EventArgs e)
        {
            dtpMentionDate_Hider.Visible = (dtpMentionDate.Value.Date == Date.blankDate);
        }

        private void dtpLegalAtchDate_ValueChanged(object sender, EventArgs e)
        {
            dtpLegalAtchDate_Hider.Visible = (dtpLegalAtchDate.Value.Date == Date.blankDate);
        }

        private void dtpDefaultedDate_ValueChanged(object sender, EventArgs e)
        {
            dtpDefaultedDate_Hider.Visible = (dtpDefaultedDate.Value.Date == Date.blankDate);
        }

        private void dtpLegalIniDate_ValueChanged(object sender, EventArgs e)
        {
            dtpLegalIniDate_Hider.Visible = (dtpLegalIniDate.Value.Date == Date.blankDate);
        }

        private void dtpLegalIniDate_Enter(object sender, EventArgs e)
        {
            if(dtpLegalIniDate.Value == Date.blankDate)
                dtpLegalIniDate.Value = DateTime.Today;
            dtpLegalIniDate_Hider.Visible = false;
        }

        private void dtpAucDate_Enter(object sender, EventArgs e)
        {
            if(dtpAucDate.Value == Date.blankDate)
                dtpAucDate.Value = DateTime.Today;
            dtpAucDate_Hider.Visible = false;
        }

        private void dtpMentionDate_Enter(object sender, EventArgs e)
        {
            if(dtpMentionDate.Value == Date.blankDate)
                dtpMentionDate.Value = DateTime.Today;
            dtpMentionDate_Hider.Visible = false;
        }

        private void dtpDefaultedDate_Enter(object sender, EventArgs e)
        {
            if(dtpDefaultedDate.Value == Date.blankDate)
                dtpDefaultedDate.Value = DateTime.Today;
            dtpDefaultedDate_Hider.Visible = false;
        }

        private void dtpCourtDate_Enter(object sender, EventArgs e)
        {
            if(dtpCourtDate.Value == Date.blankDate)
                dtpCourtDate.Value = DateTime.Today;
            dtpCourtDate_Hider.Visible = false;            
        }

        private void dtpLegalAtchDate_Enter(object sender, EventArgs e)
        {
            if (dtpLegalAtchDate.Value == Date.blankDate)
                dtpLegalAtchDate.Value = DateTime.Today;
            dtpLegalAtchDate_Hider.Visible = false;
        }

        private void dtpAucDate_Hider_Click(object sender, EventArgs e)
        {
            if (dtpAucDate.Value == Date.blankDate)
                dtpAucDate.Value = DateTime.Today;
            dtpAucDate_Hider.Visible = false;
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

        private void ClearControls()
        {
            foreach (Control control in gbMain.Controls)
            {
                if (control is TextBox)
                    (control as TextBox).Text = "";
                else if (control is DateTimePicker)
                    (control as DateTimePicker).Value = new DateTime(1900,1,1);
                else if (control is CheckBox)
                    (control as CheckBox).Checked = false;
            }
        }

        private bool ValidateFields()
        {
            //---Clear Error Provider---------------------------
            errProvider.SetError(txtSolicitorNo, "");
            errProvider.SetError(dtpLegalIniDate, "");
            errProvider.SetError(txtAucProceed, "");
            errProvider.SetError(txtMentionCost, "");
            errProvider.SetError(txtCourtAmount, "");
            errProvider.SetError(txtCourtDeposit, "");
            errProvider.SetError(txtRemittance, "");
            errProvider.SetError(txtAucAmount, "");
            //--------------------------------------------------

            bool rtnValue = true;

            //-----Checking for mandatory fields---------------- 
            if (txtSolicitorNo.Text.Trim() == "")
            {
                rtnValue = false;
                errProvider.SetError(txtSolicitorNo, GetResource("M_ENTERMANDATORY"));
            }
            if (dtpLegalIniDate.Value == Date.blankDate)
            {
                rtnValue = false;
                errProvider.SetError(dtpLegalIniDate, GetResource("M_ENTERMANDATORY"));
            }
            //--------------------------------------------------


            //-----Checking for correct values in the fields----
            double dblValue = 0;

            if (txtAucProceed.Text.Trim() != "")
            {
                try
                {
                    dblValue = Double.Parse(txtAucProceed.Text.Trim());
                }
                catch
                {
                    rtnValue = false;
                    errProvider.SetError(txtAucProceed, GetResource("M_NUMERICVALUE"));
                }
            }
            if (txtAucAmount.Text.Trim() != "")
            {
                try
                {
                    dblValue = Double.Parse(txtAucAmount.Text.Trim());
                }
                catch
                {
                    rtnValue = false;
                    errProvider.SetError(txtAucAmount, GetResource("M_NUMERICVALUE"));
                }
            }
            if (txtMentionCost.Text.Trim() != "")
            {
                try
                {
                    dblValue = Double.Parse(txtMentionCost.Text.Trim());
                }
                catch
                {
                    rtnValue = false;
                    errProvider.SetError(txtMentionCost, GetResource("M_NUMERICVALUE"));
                }
            }
            if (txtCourtAmount.Text.Trim() != "")
            {
                try
                {
                    dblValue = Double.Parse(txtCourtAmount.Text.Trim());
                }
                catch
                {
                    rtnValue = false;
                    errProvider.SetError(txtCourtAmount, GetResource("M_NUMERICVALUE"));
                }
            }
            if (txtCourtDeposit.Text.Trim() != "")
            {
                try
                {
                    dblValue = Double.Parse(txtCourtDeposit.Text.Trim());
                }
                catch
                {
                    rtnValue = false;
                    errProvider.SetError(txtCourtDeposit, GetResource("M_NUMERICVALUE"));
                }
            }
            if (txtRemittance.Text.Trim() != "")
            {
                try
                {
                    dblValue = Double.Parse(txtRemittance.Text.Trim());
                }
                catch
                {
                    rtnValue = false;
                    errProvider.SetError(txtRemittance, GetResource("M_NUMERICVALUE"));
                }
            }
            //--------------------------------------------------

            return rtnValue;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (ValidateFields() == false)
                return;

            try
            {
                DataRow dr;
                if (m_dtLegalDetails.Rows.Count == 0)
                {
                    dr = m_dtLegalDetails.NewRow();
                    m_dtLegalDetails.Rows.Add(dr);
                }
                else
                {
                    dr = m_dtLegalDetails.Rows[0];
                }

                //---------------------------------------------------------
                dr[CN.CMSolicitorNo] = txtSolicitorNo.Text;

                if (dtpAucDate.Value.ToShortDateString() == "01/01/1900")
                    dr[CN.CMAuctionDate] = DBNull.Value; 
                else
                    dr[CN.CMAuctionDate] = dtpAucDate.Value.ToShortDateString();

                if (txtAucProceed.Text.Trim() == "")
                    dr[CN.CMAuctionProceeds] = DBNull.Value; 
                else
                    dr[CN.CMAuctionProceeds] = MoneyStrToDecimal(txtAucProceed.Text.Trim());

                if (txtAucAmount.Text.Trim() == "")
                    dr[CN.CMAuctionAmount] = DBNull.Value;
                else
                    dr[CN.CMAuctionAmount] = MoneyStrToDecimal(txtAucAmount.Text.Trim());

                if (dtpMentionDate.Value.ToShortDateString() == "01/01/1900")
                    dr[CN.CMMentionDate] = DBNull.Value;
                else
                    dr[CN.CMMentionDate] = dtpMentionDate.Value.ToShortDateString();

                if (txtMentionCost.Text.Trim() == "")
                    dr[CN.CMMentionCost] = DBNull.Value;
                else
                    dr[CN.CMMentionCost] = MoneyStrToDecimal(txtMentionCost.Text.Trim());

                if (dtpDefaultedDate.Value.ToShortDateString() == "01/01/1900")
                    dr[CN.CMDefaultedDate] = DBNull.Value;
                else
                    dr[CN.CMDefaultedDate] = dtpDefaultedDate.Value.ToShortDateString();

                if (txtCourtAmount.Text.Trim() == "")
                    dr[CN.CMCourtAmount] = DBNull.Value;
                else
                    dr[CN.CMCourtAmount] = MoneyStrToDecimal(txtCourtAmount.Text.Trim());

                if (dtpCourtDate.Value.ToShortDateString() == "01/01/1900")
                    dr[CN.CMCourtDate] = DBNull.Value;
                else
                    dr[CN.CMCourtDate] = dtpCourtDate.Value.ToShortDateString();

                if (txtCourtDeposit.Text.Trim() == "")
                    dr[CN.CMCourtDeposit] = DBNull.Value;
                else
                    dr[CN.CMCourtDeposit] = MoneyStrToDecimal(txtCourtDeposit.Text.Trim());

                if (txtRemittance.Text.Trim() == "")
                    dr[CN.CMPaymentRemittance] = DBNull.Value;
                else
                    dr[CN.CMPaymentRemittance] = MoneyStrToDecimal(txtRemittance.Text.Trim());

                if (dtpLegalAtchDate.Value.ToShortDateString() == "01/01/1900")
                    dr[CN.CMLegalAttachmentDate] = DBNull.Value;
                else
                    dr[CN.CMLegalAttachmentDate] = dtpLegalAtchDate.Value.ToShortDateString();

                if (dtpLegalIniDate.Value.ToShortDateString() == "01/01/1900")
                    dr[CN.CMLegalInitiatedDate] = DBNull.Value;
                else
                    dr[CN.CMLegalInitiatedDate] = dtpLegalIniDate.Value.ToShortDateString();

                dr[CN.CMUserNotes] = txtUserNotes.Text.Trim();
                dr[CN.CMJudgement] = txtJudgement.Text.Trim();
                dr[CN.CMCaseClosed] = chkCaseClosed.Checked;
                //---------------------------------------------------------
                
                m_dtLegalDetails.AcceptChanges();  
                m_buttonClicked = DialogResult.OK;                              
            }
            catch (Exception ex)
            {
                Catch(ex, "btnSave_Click");
            }
            
            this.Close();         
        } 
	}
}

