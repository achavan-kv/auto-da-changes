using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using STL.Common.Constants.Values;

namespace STL.PL
{
    /// <summary>
    /// This popup prompt to warn the user to set action in order to sale the item 
    /// when MMI threshold exceeding the monthly instalment for customer.
    /// The user also has the option to refer the account.
    /// </summary>
    public class SaleAction : CommonForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;
        private Label lblTermLength;
        private Label lblDeposit;
        private Label lblReduceAgree;
        private Label lblReasonReferal;
        public RichTextBox rtxtNewReferralNotes;
        private Button btnGoToRefer;
        private Button btnCancelRefer;
        private Button btnModify;
        private Button btnRefer;
        private Button btnCancel;

        private bool m_modify = false;
        private bool m_refer = false;
        private bool m_closeSale = false;
        //private bool _isMonthToExtendApplicable = false;
        //private bool _isAmountToDepositApplicable = false;
        //private bool _isAmountToReduceApplicable = false;
        private bool _isCashLoanDetails = false;
        private int _totalMonth;
        private int _monthToExtend;
        private decimal _amountToDeposit;
        private decimal _amountToReduce;


        public bool Modify
        {
            get
            {
                return m_modify;
            }
            set
            {
                m_modify = value;
            }
        }
        public bool Refer
        {
            get
            {
                return m_refer;
            }
            set
            {
                m_refer = value;
            }
        }

        public bool CloseSale
        {
            get
            {
                return m_closeSale;
            }
            set
            {
                m_closeSale = value;
            }
        }

        public int TotalMonth
        {
            get
            {
                return _totalMonth;
            }
            set
            {
                _totalMonth = value;
            }
        }

        public int MonthToExtend
        {
            get
            {
                return _monthToExtend;
            }
            set
            {
                _monthToExtend = value;
            }
        }

        public decimal AmountToDeposit
        {
            get
            {
                return _amountToDeposit;
            }
            set
            {
                _amountToDeposit = value;
            }
        }

        public decimal AmountToReduce
        {
            get
            {
                return _amountToReduce;
            }
            set
            {
                _amountToReduce = value;
            }
        }

        //public bool IsMonthToExtendApplicable
        //{
        //    get
        //    {
        //        return _isMonthToExtendApplicable;
        //    }
        //    set
        //    {
        //        _isMonthToExtendApplicable = value;
        //    }
        //}

        //public bool IsAmountToDepositApplicable
        //{
        //    get
        //    {
        //        return _isAmountToDepositApplicable;
        //    }
        //    set
        //    {
        //        _isAmountToDepositApplicable = value;
        //    }
        //}

        //public bool IsAmountToReduceApplicable
        //{
        //    get
        //    {
        //        return _isAmountToReduceApplicable;
        //    }
        //    set
        //    {
        //        _isAmountToReduceApplicable = value;
        //    }
        //}

        public bool IsCashLoanDetails
        {
            get
            {
                return _isCashLoanDetails;
            }
            set
            {
                _isCashLoanDetails = value;
            }
        }

        public SaleAction(TranslationDummy d)
        {
            InitializeComponent();
        }

        public SaleAction()
        {
            InitializeComponent();
            SetControlsForReferal(false);
            SetControlPosition(false);
        }

        public void SetControlsForNotes()
        {
            int noteNum = 0;
            if (TotalMonth <= VL.MaxInstallmentTerms)
                this.lblTermLength.Text = String.Format("{0}) Extend the term length (by choosing different Term) by {1} {2}.", ++noteNum, MonthToExtend, MonthToExtend > 1 ? "months" : "month");
            else
                this.lblTermLength.Text = String.Format("{0}) Extend the term length (max {1} months allowed).", ++noteNum, VL.MaxInstallmentTerms);

            if (IsCashLoanDetails)
            {
                this.lblDeposit.Visible = false;
                this.lblReduceAgree.Text = String.Format("{0}) Reduce the loan value by ${1} amount.", ++noteNum, AmountToReduce);
            }
            else
            {
                this.lblDeposit.Text = String.Format("{0}) Customer must pay deposit of ${1} amount.", ++noteNum, AmountToDeposit);
                this.lblReduceAgree.Text = String.Format("{0}) Reduce the agreement value by ${1} amount.", ++noteNum, AmountToReduce);
            }
        }

        private void SetControlsForReferal(bool showReferal)
        {
            lblTermLength.Visible = !showReferal;
            lblDeposit.Visible = !showReferal && !IsCashLoanDetails;
            lblReduceAgree.Visible = !showReferal;
            btnModify.Visible = !showReferal;
            btnRefer.Visible = !showReferal;
            btnCancel.Visible = !showReferal;

            lblReasonReferal.Visible = showReferal;
            rtxtNewReferralNotes.Visible = showReferal;
            btnGoToRefer.Visible = showReferal;
            btnCancelRefer.Visible = showReferal;
        }

        private void SetControlPosition(bool showReferal)
        {
            int newPositionY = 0;
            int verSpaceBetCtrlBtn = 30;
            int verSpaceBetCtrls = 22;
            if (!showReferal)
            {
                newPositionY = lblReduceAgree.Location.Y + verSpaceBetCtrlBtn;
                btnModify.Location = new Point(btnModify.Location.X, newPositionY);
                btnRefer.Location = new Point(btnRefer.Location.X, newPositionY);
                btnCancel.Location = new Point(btnCancel.Location.X, newPositionY);
            }
            else
            {
                lblReasonReferal.Location = lblTermLength.Location;
                rtxtNewReferralNotes.Location = new Point(lblReasonReferal.Location.X, lblReasonReferal.Location.Y + verSpaceBetCtrls);
                newPositionY = rtxtNewReferralNotes.Location.Y + rtxtNewReferralNotes.Height;
                btnGoToRefer.Location = new Point(btnGoToRefer.Location.X, newPositionY);
                btnCancelRefer.Location = new Point(btnCancelRefer.Location.X, newPositionY);
            }

            this.Size = new System.Drawing.Size(450, 200);
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.rtxtNewReferralNotes = new System.Windows.Forms.RichTextBox();
            this.lblTermLength = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnRefer = new System.Windows.Forms.Button();
            this.btnModify = new System.Windows.Forms.Button();
            this.lblDeposit = new System.Windows.Forms.Label();
            this.lblReduceAgree = new System.Windows.Forms.Label();
            this.btnGoToRefer = new System.Windows.Forms.Button();
            this.lblReasonReferal = new System.Windows.Forms.Label();
            this.btnCancelRefer = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // rtxtNewReferralNotes
            // 
            this.rtxtNewReferralNotes.Location = new System.Drawing.Point(41, 96);
            this.rtxtNewReferralNotes.MaxLength = 1000;
            this.rtxtNewReferralNotes.Name = "rtxtNewReferralNotes";
            this.rtxtNewReferralNotes.Size = new System.Drawing.Size(359, 70);
            this.rtxtNewReferralNotes.TabIndex = 41;
            this.rtxtNewReferralNotes.Text = "";
            this.rtxtNewReferralNotes.Visible = false;
            // 
            // lblTermLength
            // 
            this.lblTermLength.AutoSize = true;
            this.lblTermLength.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.lblTermLength.Location = new System.Drawing.Point(25, 15);
            this.lblTermLength.Name = "lblTermLength";
            this.lblTermLength.Size = new System.Drawing.Size(395, 13);
            this.lblTermLength.TabIndex = 42;
            this.lblTermLength.Text = "1) Extend the term length (by choosing different Term) by #ExtendMonths# months";
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(280, 223);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(48, 24);
            this.btnCancel.TabIndex = 44;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnRefer
            // 
            this.btnRefer.Location = new System.Drawing.Point(200, 223);
            this.btnRefer.Name = "btnRefer";
            this.btnRefer.Size = new System.Drawing.Size(48, 24);
            this.btnRefer.TabIndex = 43;
            this.btnRefer.Text = "Refer";
            this.btnRefer.Click += new System.EventHandler(this.btnRefer_Click);
            // 
            // btnModify
            // 
            this.btnModify.Location = new System.Drawing.Point(120, 224);
            this.btnModify.Name = "btnModify";
            this.btnModify.Size = new System.Drawing.Size(48, 24);
            this.btnModify.TabIndex = 45;
            this.btnModify.Text = "Modify";
            this.btnModify.Click += new System.EventHandler(this.btnModify_Click);
            // 
            // lblDeposit
            // 
            this.lblDeposit.AutoSize = true;
            this.lblDeposit.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.lblDeposit.Location = new System.Drawing.Point(25, 37);
            this.lblDeposit.Name = "lblDeposit";
            this.lblDeposit.Size = new System.Drawing.Size(296, 13);
            this.lblDeposit.TabIndex = 46;
            this.lblDeposit.Text = "2) Customer must pay deposit of $#DepositeAmount# amount";
            // 
            // lblReduceAgree
            // 
            this.lblReduceAgree.AutoSize = true;
            this.lblReduceAgree.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.lblReduceAgree.Location = new System.Drawing.Point(25, 59);
            this.lblReduceAgree.Name = "lblReduceAgree";
            this.lblReduceAgree.Size = new System.Drawing.Size(307, 13);
            this.lblReduceAgree.TabIndex = 47;
            this.lblReduceAgree.Text = "3) Reduce the Agreement value by $#ReduceAmount# amount";
            // 
            // btnGoToRefer
            // 
            this.btnGoToRefer.Location = new System.Drawing.Point(120, 191);
            this.btnGoToRefer.Name = "btnGoToRefer";
            this.btnGoToRefer.Size = new System.Drawing.Size(78, 24);
            this.btnGoToRefer.TabIndex = 48;
            this.btnGoToRefer.Text = "Go To Refer";
            this.btnGoToRefer.Visible = false;
            this.btnGoToRefer.Click += new System.EventHandler(this.btnGoToRefer_Click);
            // 
            // lblReasonReferal
            // 
            this.lblReasonReferal.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.lblReasonReferal.Location = new System.Drawing.Point(41, 79);
            this.lblReasonReferal.Name = "lblReasonReferal";
            this.lblReasonReferal.Size = new System.Drawing.Size(380, 13);
            this.lblReasonReferal.TabIndex = 49;
            this.lblReasonReferal.Text = "Reason For Referal";
            this.lblReasonReferal.Visible = false;
            // 
            // btnCancelRefer
            // 
            this.btnCancelRefer.Location = new System.Drawing.Point(250, 191);
            this.btnCancelRefer.Name = "btnCancelRefer";
            this.btnCancelRefer.Size = new System.Drawing.Size(78, 24);
            this.btnCancelRefer.TabIndex = 50;
            this.btnCancelRefer.Text = "Cancel Refer";
            this.btnCancelRefer.Visible = false;
            this.btnCancelRefer.Click += new System.EventHandler(this.btnCancelRefer_Click);
            // 
            // SaleAction
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(434, 261);
            this.Controls.Add(this.btnCancelRefer);
            this.Controls.Add(this.lblReasonReferal);
            this.Controls.Add(this.btnGoToRefer);
            this.Controls.Add(this.lblReduceAgree);
            this.Controls.Add(this.lblDeposit);
            this.Controls.Add(this.btnModify);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnRefer);
            this.Controls.Add(this.lblTermLength);
            this.Controls.Add(this.rtxtNewReferralNotes);
            this.Name = "SaleAction";
            this.Text = "MMI Validation";
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        private void btnModify_Click(object sender, EventArgs e)
        {
            Modify = true;
            Refer = false;
            CloseSale = false;
            Close();
        }

        private void btnRefer_Click(object sender, System.EventArgs e)
        {
            SetControlsForReferal(true);
            SetControlPosition(true);
        }

        private void btnGoToRefer_Click(object sender, EventArgs e)
        {
            Modify = false;
            Refer = true;
            CloseSale = false;
            Close();
        }

        private void btnCancelRefer_Click(object sender, EventArgs e)
        {
            SetControlsForReferal(false);
            SetControlPosition(false);
        }

        private void btnCancel_Click(object sender, System.EventArgs e)
        {
            if (DialogResult.Yes == ShowInfo("M_CONFIRMSALECLOSEWIHOUTSAVE", MessageBoxButtons.YesNo))
            {
                Modify = false;
                Refer = false;
                CloseSale = true;
                Close();
            }
        }
    }
}
