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
	public partial class TelAction_FRD : CommonForm
	{
        private DataTable m_dtFraudDetails;
        private DialogResult m_buttonClicked = DialogResult.None;

        public DialogResult ButtonClicked
        {
            get
            {
                return m_buttonClicked;
            }
        }

        public TelAction_FRD(DataTable dtFraudDetails)
		{
            m_dtFraudDetails = dtFraudDetails;
            m_dtFraudDetails.AcceptChanges();
            InitializeComponent();
		}

        private void TelAction_FRD_Load(object sender, EventArgs e)
        {
            try
            {
                if (m_dtFraudDetails.Rows.Count == 0)
                    ClearControls();
                else
                {
                    DataRow dr = m_dtFraudDetails.Rows[0];

                    dtpInitiatedDate.Value  = dr[CN.CMFraudInitiatedDate] != DBNull.Value ? Convert.ToDateTime(dr[CN.CMFraudInitiatedDate]) : new DateTime(1900, 1, 1);
                    txtUserNotes.Text       = dr[CN.CMUserNotes] != DBNull.Value ? dr[CN.CMUserNotes].ToString() : "";
                    chkResolved.Checked = dr[CN.CMIsResolved] != DBNull.Value ? Convert.ToBoolean(dr[CN.CMIsResolved]) : false;
                }
            }
            catch (Exception ex)
            {
                Catch(ex, "TelAction_FRD_Load");
            }
        }
       
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (ValidateFields() == false)
                return;

            try
            {
                DataRow dr;
                if (m_dtFraudDetails.Rows.Count == 0)
                {
                    dr = m_dtFraudDetails.NewRow();
                    m_dtFraudDetails.Rows.Add(dr);
                }
                else
                {
                    dr = m_dtFraudDetails.Rows[0];
                }

                //---------------------------------------------------------
                if (dtpInitiatedDate.Value.ToShortDateString() == "01/01/1900")
                    dr[CN.CMFraudInitiatedDate] = DBNull.Value;
                else
                    dr[CN.CMFraudInitiatedDate] = dtpInitiatedDate.Value.ToShortDateString();

                dr[CN.CMUserNotes] = txtUserNotes.Text.Trim();
                dr[CN.CMIsResolved] = chkResolved.Checked;
                //---------------------------------------------------------

                m_dtFraudDetails.AcceptChanges();
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
            m_dtFraudDetails.RejectChanges();
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
            if (dtpInitiatedDate.Value == Date.blankDate)
                dtpInitiatedDate.Value = DateTime.Today;
            dtpInitiatedDate_Hider.Visible = false;
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

        private bool ValidateFields()
        {
            //---Clear Error Provider---------------------------
            errProvider.SetError(dtpInitiatedDate, "");
            //--------------------------------------------------

            bool rtnValue = true;

            //-----Checking for mandatory fields---------------- 
            if (dtpInitiatedDate.Value == Date.blankDate)
            {
                rtnValue = false;
                errProvider.SetError(dtpInitiatedDate, GetResource("M_ENTERMANDATORY"));
            }
            //--------------------------------------------------

            return rtnValue;
        }
       
	}
}
