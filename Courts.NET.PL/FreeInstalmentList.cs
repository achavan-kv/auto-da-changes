using System;
using System.Data;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using STL.Common;
using STL.Common.Static;
using STL.Common.Constants.ColumnNames;
using STL.Common.Constants.TableNames;
using Crownwood.Magic.Menus;
using STL.Common.Constants.EmployeeTypes;
using STL.Common.Constants.FTransaction;




namespace STL.PL
{
	/// <summary>
	/// Popup prompt used by the Payment screen to list free instalments due
	/// to a customer. This is only offered to customers qualified as Privilege
	/// Club Tier2 for each account in good status for the required minimum
	/// number of instalments.
	/// The screen will list each account with the value of the free instalment
	/// due. The user can tick each account to either take a free instalment
	/// or to take a Gift Voucher of equivalent value.
	/// </summary>
	public class FreeInstalmentList : CommonForm
	{
		private new string Error = "";
		private DataTable _freeList = null;

		private System.Windows.Forms.DataGrid dgFreeInstalmentList;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.ImageList imageList1;
		private System.Windows.Forms.Label lCustomerName;
		private System.Windows.Forms.TextBox txtCustId;
		private System.Windows.Forms.TextBox txtCustomerName;
		private System.Windows.Forms.Label lCustomerId;
		private System.ComponentModel.IContainer components;
		private System.Windows.Forms.Button btnGiftVoucher;
        private ErrorProvider errorProvider1;
		private System.Windows.Forms.Button btnFreeInstalment;


		public FreeInstalmentList(Form root, Form parent)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			FormRoot = root;
			FormParent = parent;
		}

		public FreeInstalmentList(string piCustId, string piCustomerName, DataTable piFreeList, Form root, Form parent)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// After InitializeComponent call
			//
			FormRoot = root;
			FormParent = parent;

			// Copy the data in case this popup is cancelled
			this._freeList = piFreeList;
			this.txtCustId.Text = piCustId;
			this.txtCustomerName.Text = piCustomerName;
			this.LoadData();
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
            this.components = new System.ComponentModel.Container();
            this.dgFreeInstalmentList = new System.Windows.Forms.DataGrid();
            this.btnGiftVoucher = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.lCustomerName = new System.Windows.Forms.Label();
            this.txtCustId = new System.Windows.Forms.TextBox();
            this.txtCustomerName = new System.Windows.Forms.TextBox();
            this.lCustomerId = new System.Windows.Forms.Label();
            this.btnFreeInstalment = new System.Windows.Forms.Button();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.dgFreeInstalmentList)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // dgFreeInstalmentList
            // 
            this.dgFreeInstalmentList.CaptionText = "Free Instalment List";
            this.dgFreeInstalmentList.DataMember = "";
            this.dgFreeInstalmentList.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgFreeInstalmentList.Location = new System.Drawing.Point(8, 40);
            this.dgFreeInstalmentList.Name = "dgFreeInstalmentList";
            this.dgFreeInstalmentList.Size = new System.Drawing.Size(648, 216);
            this.dgFreeInstalmentList.TabIndex = 0;
            this.dgFreeInstalmentList.TabStop = false;
            // 
            // btnGiftVoucher
            // 
            this.btnGiftVoucher.Location = new System.Drawing.Point(124, 262);
            this.btnGiftVoucher.Name = "btnGiftVoucher";
            this.btnGiftVoucher.Size = new System.Drawing.Size(112, 23);
            this.btnGiftVoucher.TabIndex = 1;
            this.btnGiftVoucher.Text = "Gift Voucher";
            this.btnGiftVoucher.Click += new System.EventHandler(this.btnGiftVoucher_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(430, 262);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(112, 23);
            this.button2.TabIndex = 2;
            this.button2.Text = "Cancel";
            this.button2.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // imageList1
            // 
            this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imageList1.ImageSize = new System.Drawing.Size(16, 16);
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // lCustomerName
            // 
            this.lCustomerName.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lCustomerName.Location = new System.Drawing.Point(232, 8);
            this.lCustomerName.Name = "lCustomerName";
            this.lCustomerName.Size = new System.Drawing.Size(88, 16);
            this.lCustomerName.TabIndex = 10;
            this.lCustomerName.Text = "Customer Name";
            this.lCustomerName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtCustId
            // 
            this.txtCustId.BackColor = System.Drawing.SystemColors.Window;
            this.txtCustId.Location = new System.Drawing.Point(88, 8);
            this.txtCustId.MaxLength = 20;
            this.txtCustId.Name = "txtCustId";
            this.txtCustId.ReadOnly = true;
            this.txtCustId.Size = new System.Drawing.Size(120, 20);
            this.txtCustId.TabIndex = 9;
            // 
            // txtCustomerName
            // 
            this.txtCustomerName.BackColor = System.Drawing.SystemColors.Window;
            this.txtCustomerName.Location = new System.Drawing.Point(328, 8);
            this.txtCustomerName.MaxLength = 80;
            this.txtCustomerName.Name = "txtCustomerName";
            this.txtCustomerName.ReadOnly = true;
            this.txtCustomerName.Size = new System.Drawing.Size(312, 20);
            this.txtCustomerName.TabIndex = 7;
            this.txtCustomerName.TabStop = false;
            // 
            // lCustomerId
            // 
            this.lCustomerId.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lCustomerId.Location = new System.Drawing.Point(8, 8);
            this.lCustomerId.Name = "lCustomerId";
            this.lCustomerId.Size = new System.Drawing.Size(72, 16);
            this.lCustomerId.TabIndex = 8;
            this.lCustomerId.Text = "Customer ID";
            this.lCustomerId.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnFreeInstalment
            // 
            this.btnFreeInstalment.Location = new System.Drawing.Point(277, 262);
            this.btnFreeInstalment.Name = "btnFreeInstalment";
            this.btnFreeInstalment.Size = new System.Drawing.Size(112, 23);
            this.btnFreeInstalment.TabIndex = 11;
            this.btnFreeInstalment.Text = "Free Instalment";
            this.btnFreeInstalment.Click += new System.EventHandler(this.btnFreeInstalment_Click);
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // FreeInstalmentList
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(666, 293);
            this.Controls.Add(this.btnFreeInstalment);
            this.Controls.Add(this.lCustomerName);
            this.Controls.Add(this.txtCustId);
            this.Controls.Add(this.txtCustomerName);
            this.Controls.Add(this.lCustomerId);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.btnGiftVoucher);
            this.Controls.Add(this.dgFreeInstalmentList);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FreeInstalmentList";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "CoSACS Payment";
            this.Load += new System.EventHandler(this.FreeInstalmentList_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgFreeInstalmentList)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		// Local Procedures

		private void AddColumnStyle(string columnName, DataGridTableStyle tabStyle,
			int width, bool readOnly, string headerText, string format)
		{
			DataGridTextBoxColumn aColumnTextColumn = new DataGridTextBoxColumn();
			aColumnTextColumn.MappingName = columnName;
			aColumnTextColumn.Width = width;
			aColumnTextColumn.ReadOnly = readOnly;
			aColumnTextColumn.HeaderText = headerText;
			aColumnTextColumn.Format = format;
			tabStyle.GridColumnStyles.Add(aColumnTextColumn);
		}

		private void LoadData ()
		{
			//
			// Display the list of Customer Free Instalments
			//
			try
			{
				Function = "Free Instalment List Screen: Load Data";
				Wait();

				((MainForm)this.FormRoot).statusBar1.Text = this._freeList.Rows.Count + GetResource("M_ACCOUNTSLISTED");

				// Create a view for the DataGrid
				DataView FreeInstalmentListView = new DataView(this._freeList);
				FreeInstalmentListView.AllowNew = false;
				FreeInstalmentListView.Sort = CN.FreeInstalment + " DESC ";
				dgFreeInstalmentList.CausesValidation = false;
				dgFreeInstalmentList.DataSource = FreeInstalmentListView;

				if (dgFreeInstalmentList.TableStyles.Count == 0)
				{
					// Create the table style for the DataGrid
					DataGridTableStyle tabStyle = new DataGridTableStyle();
					tabStyle.MappingName = FreeInstalmentListView.Table.TableName;

					//
					// Style each column that needs to be displayed
					//

					// Normal columns
					AddColumnStyle(CN.AccountNo,			tabStyle, 100, true,  GetResource("T_ACCOUNTNO"),	"");
					AddColumnStyle(CN.OutstandingBalance,	tabStyle,  90, true,  GetResource("T_OUTBAL"),		DecimalPlaces);
					AddColumnStyle(CN.Rebate,				tabStyle,  90, true,  GetResource("T_REBATE"),		DecimalPlaces);
					AddColumnStyle(CN.SettlementFigure,		tabStyle,  90, true,  GetResource("T_SETTLEMENT"),	DecimalPlaces);
					AddColumnStyle(CN.InstalAmount,			tabStyle,  90, true,  GetResource("T_INSTALLMENT"),	DecimalPlaces);
					AddColumnStyle(CN.Status,				tabStyle,  30, true,  GetResource("T_STATUS"),		"");
					AddColumnStyle(CN.FreeInstalment,		tabStyle,  90, true,  GetResource("T_AMOUNT"),		DecimalPlaces);
					AddColumnStyle(CN.ToDelete,				tabStyle,  0,  true,  "",							"");

					dgFreeInstalmentList.TableStyles.Clear();
					dgFreeInstalmentList.TableStyles.Add(tabStyle);
					dgFreeInstalmentList.DataSource = FreeInstalmentListView;
				}
			}
			catch(Exception ex)
			{
				Catch(ex, Function);
			}
			finally
			{
				StopWait();
			}
		}

        // Events

        private void FreeInstalmentList_Load(object sender, EventArgs e)
        {
            if ((string)Country[CountryParameterNames.CountryCode] == "S")
            {
                // Disable the free instalment option for Singapore
                this.btnFreeInstalment.Enabled = false;
                this.btnFreeInstalment.Visible = false;
            }
        }

		private void btnFreeInstalment_Click(object sender, System.EventArgs e)
		{
			try
			{
				Function = "Free Instalment List Screen: Free Instalment button";
				Wait();

                this.errorProvider1.SetError(this.btnFreeInstalment, "");
                this.errorProvider1.SetError(this.btnGiftVoucher, "");

				DataView freeInstalmentListView = (DataView)this.dgFreeInstalmentList.DataSource;

                bool isSelected = false;
				for (int i = 0; i < freeInstalmentListView.Count; i++)
				{
					DataRowView curRow = freeInstalmentListView[i];
					if (dgFreeInstalmentList.IsSelected(i))
					{
                        isSelected = true;
						// Pay a free instalment on this account
						PaymentManager.WriteFreeInstalment(
							(string)curRow[CN.AccountNo],
							Convert.ToInt16(Config.BranchCode),
							Convert.ToDecimal(curRow[CN.FreeInstalment]),
							Config.CountryCode,
							out Error);

						if (Error.Length > 0)
							ShowError(Error);
						else
						{
							// Remove this row later
							curRow[CN.ToDelete] = "Y";
						}
					}
					else
						curRow[CN.ToDelete] = "N";
				}

                if (!isSelected)
                {
                    this.errorProvider1.SetError(this.btnFreeInstalment, GetResource("M_SELECTROWS"));
                }
                else
                {
                    // Remove the rows that were paid
                    for (int i = freeInstalmentListView.Count - 1; i >= 0; i--)
                    {
                        DataRowView curRow = freeInstalmentListView[i];
                        if ((string)curRow[CN.ToDelete] == "Y") curRow.Delete();
                    }

                    freeInstalmentListView.Table.AcceptChanges();
                    if (freeInstalmentListView.Count == 0) Close();
                }
			}
			catch(Exception ex)
			{
				Catch(ex, Function);
			}
			finally
			{
				StopWait();
			}
		}


		private void btnGiftVoucher_Click(object sender, System.EventArgs e)
		{
			try
			{
				Function = "Free Instalment List Screen: Gift Voucher button";
				Wait();

                this.errorProvider1.SetError(this.btnFreeInstalment, "");
                this.errorProvider1.SetError(this.btnGiftVoucher, "");

				DataView freeInstalmentListView = (DataView)this.dgFreeInstalmentList.DataSource;
				int totalVoucherAmount = 0;

				DataSet accountSet = new DataSet();
				DataTable accountList = new DataTable(TN.AccountNumbers);
				accountList.Columns.Add(CN.AccountNo);
				accountList.Columns.Add(CN.FreeInstalment, Type.GetType("System.Decimal"));
				accountSet.Tables.Add(accountList);

                bool isSelected = false;
				for (int i = 0; i < freeInstalmentListView.Count; i++)
				{
					DataRowView curRow = freeInstalmentListView[i];
					if (dgFreeInstalmentList.IsSelected(i))
					{
                        isSelected = true;
						// Total the amount for the Gift Voucher
						totalVoucherAmount += Convert.ToInt32(curRow[CN.FreeInstalment]);
						// Add this acctno to the list
						DataRow newRow = accountList.NewRow();
						newRow[CN.AccountNo] = (string)curRow[CN.AccountNo];
						newRow[CN.FreeInstalment] = (decimal)curRow[CN.FreeInstalment];
						accountList.Rows.Add(newRow);
						// Remove this row later
						curRow[CN.ToDelete] = "Y";
					}
					else
						curRow[CN.ToDelete] = "N";
				}

                if (!isSelected)
                {
                    this.errorProvider1.SetError(this.btnGiftVoucher, GetResource("M_SELECTROWS"));
                }
                else
                {
                    if (totalVoucherAmount >= 0.01M)
                    {
                        // Call the Gift Voucher popup
                        GiftVoucher GiftVoucherPopup = new GiftVoucher(this, FormRoot, totalVoucherAmount, accountSet);
                        GiftVoucherPopup.ShowDialog();

                        if (GiftVoucherPopup.soldAmount == totalVoucherAmount)
                        {
                            // Remove the rows that were included in the Gift Voucher
                            for (int i = freeInstalmentListView.Count - 1; i >= 0; i--)
                            {
                                DataRowView curRow = freeInstalmentListView[i];
                                if ((string)curRow[CN.ToDelete] == "Y") curRow.Delete();
                            }

                            freeInstalmentListView.Table.AcceptChanges();
                            if (freeInstalmentListView.Count == 0) Close();
                        }
                    }
                }
			}
			catch(Exception ex)
			{
				Catch(ex, Function);
			}
			finally
			{
				StopWait();
			}
		}


		private void btnCancel_Click(object sender, System.EventArgs e)
		{
			try
			{
				Function = "Free Instalment List Screen: Cancel button";
				Wait();
				Close();
			}
			catch(Exception ex)
			{
				Catch(ex, Function);
			}
			finally
			{
				StopWait();
			}
		}


	}
}
