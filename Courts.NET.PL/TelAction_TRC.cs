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
    public partial class TelAction_TRC : CommonForm
    {
        private DataTable m_dtTRCDetails;
        private DialogResult m_buttonClicked = DialogResult.None;

        public DialogResult ButtonClicked
        {
            get
            {
                return m_buttonClicked;
            }
        }


        public TelAction_TRC(DataTable dtTRCDetails)
        {
            m_dtTRCDetails = dtTRCDetails;
            m_dtTRCDetails.AcceptChanges();
            InitializeComponent();
        }

        private void TelAction_TRC_Load(object sender, EventArgs e)
        {
            try
            {
                if (m_dtTRCDetails.Rows.Count == 0)
                    ClearControls();
                else
                {
                    DataRow dr = m_dtTRCDetails.Rows[0];

                    dtpInitiatedDate.Value = dr[CN.CMTRCInitiatedDate] != DBNull.Value ? Convert.ToDateTime(dr[CN.CMTRCInitiatedDate]) : new DateTime(1900, 1, 1);
                    txtUserNotes.Text = dr[CN.CMUserNotes] != DBNull.Value ? dr[CN.CMUserNotes].ToString() : "";
                    chkResolved.Checked = dr[CN.CMIsResolved] != DBNull.Value ? Convert.ToBoolean(dr[CN.CMIsResolved]) : false;
                }
            }
            catch (Exception ex)
            {
                Catch(ex, "TelAction_TRC_Load");
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            m_dtTRCDetails.RejectChanges();
            m_buttonClicked = DialogResult.Cancel;
            this.Close(); 
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (ValidateFields() == false)
                return;

            try
            {
                DataRow dr;
                if (m_dtTRCDetails.Rows.Count == 0)
                {
                    dr = m_dtTRCDetails.NewRow();
                    m_dtTRCDetails.Rows.Add(dr);
                }
                else
                {
                    dr = m_dtTRCDetails.Rows[0];
                }

                //---------------------------------------------------------
                if (dtpInitiatedDate.Value.ToShortDateString() == "01/01/1900")
                    dr[CN.CMTRCInitiatedDate] = DBNull.Value;
                else
                    dr[CN.CMTRCInitiatedDate] = dtpInitiatedDate.Value.ToShortDateString();

                dr[CN.CMUserNotes] = txtUserNotes.Text.Trim();
                dr[CN.CMIsResolved] = chkResolved.Checked;
                //---------------------------------------------------------

                m_dtTRCDetails.AcceptChanges();
                m_buttonClicked = DialogResult.OK;
            }
            catch (Exception ex)
            {
                Catch(ex, "btnOK_Click");
            }

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
