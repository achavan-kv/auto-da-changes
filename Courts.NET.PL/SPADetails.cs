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
	/// <summary>
	/// Lists the SPA history for the account, showing the allocation,
	/// the date allocated and the date of expiry.
	/// </summary>
	public class SPADetails : CommonForm
	{
        private System.Windows.Forms.GroupBox gbSPAHistory;
        private System.Windows.Forms.TextBox txtAccountNo;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnExit;
        private new string Error = "";
        private System.Windows.Forms.DataGrid dgSpaHistory;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public SPADetails(string acctNo)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
            txtAccountNo.Text = acctNo;
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.gbSPAHistory = new System.Windows.Forms.GroupBox();
            this.dgSpaHistory = new System.Windows.Forms.DataGrid();
            this.txtAccountNo = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btnExit = new System.Windows.Forms.Button();
            this.gbSPAHistory.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgSpaHistory)).BeginInit();
            this.SuspendLayout();
            // 
            // gbSPAHistory
            // 
            this.gbSPAHistory.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                                       this.dgSpaHistory});
            this.gbSPAHistory.Location = new System.Drawing.Point(8, 48);
            this.gbSPAHistory.Name = "gbSPAHistory";
            this.gbSPAHistory.Size = new System.Drawing.Size(616, 248);
            this.gbSPAHistory.TabIndex = 0;
            this.gbSPAHistory.TabStop = false;
            this.gbSPAHistory.Text = "SPA History";
            // 
            // dgSpaHistory
            // 
            this.dgSpaHistory.DataMember = "";
            this.dgSpaHistory.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgSpaHistory.Location = new System.Drawing.Point(8, 24);
            this.dgSpaHistory.Name = "dgSpaHistory";
            this.dgSpaHistory.ReadOnly = true;
            this.dgSpaHistory.Size = new System.Drawing.Size(600, 216);
            this.dgSpaHistory.TabIndex = 0;
            // 
            // txtAccountNo
            // 
            this.txtAccountNo.BackColor = System.Drawing.SystemColors.Control;
            this.txtAccountNo.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.txtAccountNo.ForeColor = System.Drawing.SystemColors.WindowText;
            this.txtAccountNo.Location = new System.Drawing.Point(120, 24);
            this.txtAccountNo.Name = "txtAccountNo";
            this.txtAccountNo.ReadOnly = true;
            this.txtAccountNo.Size = new System.Drawing.Size(104, 20);
            this.txtAccountNo.TabIndex = 20;
            this.txtAccountNo.Text = "000-0000-0000-0";
            // 
            // label4
            // 
            this.label4.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label4.Location = new System.Drawing.Point(16, 24);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(96, 18);
            this.label4.TabIndex = 18;
            this.label4.Text = "Account Number";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(568, 24);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(40, 23);
            this.btnExit.TabIndex = 33;
            this.btnExit.Text = "Exit";
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // SPADetails
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(632, 309);
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                          this.btnExit,
                                                                          this.txtAccountNo,
                                                                          this.label4,
                                                                          this.gbSPAHistory});
            this.Name = "SPADetails";
            this.Text = "SPA History";
            this.Load += new System.EventHandler(this.SPADetails_Load);
            this.gbSPAHistory.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgSpaHistory)).EndInit();
            this.ResumeLayout(false);

        }
		#endregion

        private void btnExit_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }

        private void SPADetails_Load(object sender, System.EventArgs e)
        {
            try
            {
                Wait();
                LoadSPAHistory();
            }
            catch(Exception ex)
            {
                Catch(ex, "SPADetails_Load");
            }
            finally
            {
                StopWait();
            }
        }

        private void LoadSPAHistory()
        {
            string acctNo = txtAccountNo.Text.Replace("-","");

            try
            {
                DataSet ds = AccountManager.GetSPAHistory(acctNo, out Error);

                if(Error.Length>0)
                {
                    ShowError(Error);
                }
                else
                {
                    dgSpaHistory.DataSource = ds.Tables[TN.SpaHistory].DefaultView; 
                    if(dgSpaHistory.TableStyles.Count==0)
                    {
                        DataGridTableStyle tabStyle = new DataGridTableStyle();
                        tabStyle.MappingName = ds.Tables[TN.SpaHistory].TableName;
                        dgSpaHistory.TableStyles.Add(tabStyle);
                        tabStyle.GridColumnStyles[CN.Code].Width = 0;
                        tabStyle.GridColumnStyles[CN.ReasonCodeDesc].Width = 120;
                        tabStyle.GridColumnStyles[CN.ReasonCodeDesc].HeaderText = GetResource("T_REASON");
                        tabStyle.GridColumnStyles[CN.AllocNo].Width = 0;
                        tabStyle.GridColumnStyles[CN.AcctNo].Width = 0;
                        tabStyle.GridColumnStyles[CN.ActionNo].Width = 60;
                        tabStyle.GridColumnStyles[CN.ActionNo].HeaderText = GetResource("T_ACTIONNO");                     
                        tabStyle.GridColumnStyles[CN.DateAdded].Width = 100;  // 68268 RD 06/06/06
                        tabStyle.GridColumnStyles[CN.DateAdded].HeaderText = GetResource("T_DATEADDED");
                        ((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.DateAdded]).Format = "dd/MM/yyyy HH:mm";
                        tabStyle.GridColumnStyles[CN.DateExpiry].Width = 80;
                        tabStyle.GridColumnStyles[CN.DateExpiry].HeaderText = GetResource("T_DATEEXPIRY");
                        tabStyle.GridColumnStyles[CN.SpaInstal].Width = 80;
                        tabStyle.GridColumnStyles[CN.SpaInstal].Alignment = HorizontalAlignment.Right;
                        tabStyle.GridColumnStyles[CN.SpaInstal].HeaderText = GetResource("T_INSTALLMENT") + " ";
                        ((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.SpaInstal]).Format = DecimalPlaces;
                        tabStyle.GridColumnStyles[CN.EmployeeNo].Width = 110;
                        tabStyle.GridColumnStyles[CN.EmployeeNo].HeaderText = GetResource("T_ALLOCATEDEMPLOYEE");
                        tabStyle.GridColumnStyles[CN.EmployeeNoSpa].Width = 0;
                    }
                }
            }
            catch(Exception ex)
            {
                Catch(ex, Function);
            }
        }
	}
}
