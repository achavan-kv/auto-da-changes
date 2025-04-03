using System;
using System.Data;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using STL.Common;
using STL.Common.Static;
using STL.Common.Constants.Giro;
using STL.Common.Constants.TableNames;
using STL.Common.Constants.ColumnNames;
using System.Xml;
using Crownwood.Magic.Menus;




namespace STL.PL
{
	/// <summary>
	/// Direct Debit Extra Payments
	/// This screen will list all direct debit accounts with an active mandate that
	/// are in arrears. When the user has confirmed consent with the customer, the
	/// user will tick the relevant row in the tablefield. The user will be able to
	/// enter a payment amount against a row that has been ticked. All rows that
	/// have been ticked will be included in the next payment file to be submitted
	/// to the bank. Until the next payment file is created the user will be able
	/// to amend the rows to change the amounts and to add or remove the ticks.
	/// Once a payment file has been created, the ticked rows will be removed from
	/// the list so that they cannot be amended after submission.
	///
	/// The user will only be able to enter a payment amount on a row that has been
	/// ticked. If a tick is removed then the screen will clear this amount.
	///
	/// The initial population of the tablefield will set the 'Giro Pending' column
	/// according to the stored data. When the 'Extra Payment' column is changed,
	/// the frame will update the corresponding 'Giro Pending' amount.
	/// </summary>
	public class DDPaymentExtra : CommonForm
	{
        private new string Error = "";

		//
		// BL details for this form
		//
		private DataSet _extraPaymentSet = null;
		private int _consentIndex;
		private bool _userChanged = false;
		private int _tickClickIndex = -1;

		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.Button btnSave;
		private System.Windows.Forms.Button btnClearAll;
		private System.Windows.Forms.Button btnExit;
		private System.Windows.Forms.GroupBox gbPaymentExtra;
		private System.Windows.Forms.DataGrid dgPaymentList;
		private System.Windows.Forms.ImageList imageList1;
		private Crownwood.Magic.Menus.MenuCommand menuFile;
		private Crownwood.Magic.Menus.MenuCommand menuHelp;
		private Crownwood.Magic.Menus.MenuCommand menuAbout;
		private Crownwood.Magic.Menus.MenuCommand menuExit;
		private System.ComponentModel.IContainer components;

		//
		// Constructors
		//
		public DDPaymentExtra(TranslationDummy d)
		{
			InitializeComponent();
			menuMain = new Crownwood.Magic.Menus.MenuControl();
			menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {menuFile});
			menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {menuHelp});
		}

		public DDPaymentExtra(Form root, Form parent)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			// Set up
			FormRoot = root;
			FormParent = parent;
			menuMain = new Crownwood.Magic.Menus.MenuControl();
			menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {menuFile});
			menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {menuHelp});
			toolTip1.SetToolTip(this.dgPaymentList, GetResource("TT_TICKAMOUNT"));
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(DDPaymentExtra));
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.gbPaymentExtra = new System.Windows.Forms.GroupBox();
			this.btnExit = new System.Windows.Forms.Button();
			this.btnClearAll = new System.Windows.Forms.Button();
			this.btnSave = new System.Windows.Forms.Button();
			this.dgPaymentList = new System.Windows.Forms.DataGrid();
			this.imageList1 = new System.Windows.Forms.ImageList(this.components);
			this.menuFile = new Crownwood.Magic.Menus.MenuCommand();
			this.menuExit = new Crownwood.Magic.Menus.MenuCommand();
			this.menuHelp = new Crownwood.Magic.Menus.MenuCommand();
			this.menuAbout = new Crownwood.Magic.Menus.MenuCommand();
			this.gbPaymentExtra.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dgPaymentList)).BeginInit();
			this.SuspendLayout();
			// 
			// gbPaymentExtra
			// 
			this.gbPaymentExtra.BackColor = System.Drawing.SystemColors.Control;
			this.gbPaymentExtra.Controls.AddRange(new System.Windows.Forms.Control[] {
																						 this.btnExit,
																						 this.btnClearAll,
																						 this.btnSave,
																						 this.dgPaymentList});
			this.gbPaymentExtra.Location = new System.Drawing.Point(8, 0);
			this.gbPaymentExtra.Name = "gbPaymentExtra";
			this.gbPaymentExtra.Size = new System.Drawing.Size(776, 472);
			this.gbPaymentExtra.TabIndex = 0;
			this.gbPaymentExtra.TabStop = false;
			// 
			// btnExit
			// 
			this.btnExit.Location = new System.Drawing.Point(496, 424);
			this.btnExit.Name = "btnExit";
			this.btnExit.Size = new System.Drawing.Size(88, 24);
			this.btnExit.TabIndex = 3;
			this.btnExit.Text = "Exit";
			this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
			// 
			// btnClearAll
			// 
			this.btnClearAll.Location = new System.Drawing.Point(344, 424);
			this.btnClearAll.Name = "btnClearAll";
			this.btnClearAll.Size = new System.Drawing.Size(88, 24);
			this.btnClearAll.TabIndex = 2;
			this.btnClearAll.Text = "Clear All";
			this.btnClearAll.Click += new System.EventHandler(this.btnClearAll_Click);
			// 
			// btnSave
			// 
			this.btnSave.Location = new System.Drawing.Point(192, 424);
			this.btnSave.Name = "btnSave";
			this.btnSave.Size = new System.Drawing.Size(88, 24);
			this.btnSave.TabIndex = 1;
			this.btnSave.Text = "Save";
			this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
			// 
			// dgPaymentList
			// 
			this.dgPaymentList.CaptionText = "Accounts in Arrears or with Extra Payments Pending";
			this.dgPaymentList.DataMember = "";
			this.dgPaymentList.HeaderForeColor = System.Drawing.SystemColors.ControlText;
			this.dgPaymentList.Location = new System.Drawing.Point(24, 16);
			this.dgPaymentList.Name = "dgPaymentList";
			this.dgPaymentList.Size = new System.Drawing.Size(728, 400);
			this.dgPaymentList.TabIndex = 0;
			this.dgPaymentList.TabStop = false;
			this.dgPaymentList.MouseUp += new System.Windows.Forms.MouseEventHandler(this.dgPaymentList_MouseUp);
			this.dgPaymentList.CurrentCellChanged += new System.EventHandler(this.dgPaymentList_CurrentCellChanged);
			// 
			// imageList1
			// 
			this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
			this.imageList1.ImageSize = new System.Drawing.Size(16, 16);
			this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
			this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// menuFile
			// 
			this.menuFile.Description = "MenuItem";
			this.menuFile.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {
																							this.menuExit});
			this.menuFile.Text = "&File";
			// 
			// menuExit
			// 
			this.menuExit.Description = "MenuItem";
			this.menuExit.Text = "Exit";
			this.menuExit.Click += new System.EventHandler(this.menuExit_Click);
			// 
			// menuHelp
			// 
			this.menuHelp.Description = "MenuItem";
			this.menuHelp.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {
																							this.menuAbout});
			this.menuHelp.Text = "&Help";
			// 
			// menuAbout
			// 
			this.menuAbout.Description = "About Extra Payments";
			this.menuAbout.Text = "About Extra Payments";
			this.menuAbout.Click += new System.EventHandler(this.menuAbout_Click);
			// 
			// DDPaymentExtra
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.BackColor = System.Drawing.SystemColors.ControlDark;
			this.ClientSize = new System.Drawing.Size(792, 477);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.gbPaymentExtra});
			this.Name = "DDPaymentExtra";
			this.Text = "Giro Extra Payments";
			this.Load += new System.EventHandler(this.DDPaymentExtra_Load);
			this.gbPaymentExtra.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dgPaymentList)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion

		//
		// Local Routines
		//

		private void DDPaymentExtra_Load(object sender, System.EventArgs e)
		{
			this.LoadDetails();
		}

		private void LoadDetails()
		{
			try
			{
				Wait();
				Function = "Extra Payment LoadDetails";
				_userChanged = false;
				_tickClickIndex = -1;

				this._extraPaymentSet = null;
				this._extraPaymentSet = PaymentManager.GetDDPaymentExtraList(Config.CountryCode, out Error);

				if (Error.Length > 0)
				{
					ShowError(Error);
				}
				else 
				{
					int rowCount = 0;
					dgPaymentList.DataSource = null;
					dgPaymentList.ResetText();

					if (this._extraPaymentSet != null)
					{
						// Add a validation event
						_extraPaymentSet.Tables[TN.DDPaymentExtra].ColumnChanging += new DataColumnChangeEventHandler(this.ExtraPayment_ColumnChanging);

						// Load the data grid
						_extraPaymentSet.Tables[TN.DDPaymentExtra].DefaultView.AllowNew = false;
						dgPaymentList.DataSource = _extraPaymentSet.Tables[TN.DDPaymentExtra].DefaultView; 
						this._consentIndex = 13;

						// Add an unbound stand-alone icon column to tick consent
						this._extraPaymentSet.Tables[TN.DDPaymentExtra].DefaultView.Table.Columns.Add("TestIcon");

						if (dgPaymentList.TableStyles.Count == 0)
						{
							DataGridTableStyle tabStyle = new DataGridTableStyle();
							tabStyle.MappingName = _extraPaymentSet.Tables[TN.DDPaymentExtra].TableName;

							AddColumnStyle(CN.MandateId, tabStyle, 0, true, "", "", HorizontalAlignment.Left);
							AddColumnStyle(CN.PaymentId, tabStyle, 0, true, "", "", HorizontalAlignment.Left);
							AddColumnStyle(CN.OrigMonth, tabStyle, 0, true, "", "", HorizontalAlignment.Left);
							AddColumnStyle(CN.Consent, tabStyle, 0, true, "", "", HorizontalAlignment.Left);
							AddColumnStyle(CN.CurDDPending, tabStyle, 0, true, "", "", HorizontalAlignment.Left);
							AddColumnStyle(CN.CurAmount, tabStyle, 0, true, "", "", HorizontalAlignment.Left);
							AddColumnStyle(CN.CurConsent, tabStyle, 0, true, "", "", HorizontalAlignment.Left);

							AddColumnStyle(CN.AccountNumber, tabStyle, 90, true, GetResource("T_ACCOUNTNO"), "", HorizontalAlignment.Left);
							AddColumnStyle(CN.CustomerName, tabStyle, 200, true, GetResource("T_CUSTOMERNAME"), "", HorizontalAlignment.Left);
							AddColumnStyle(CN.OutstBal, tabStyle, 90, true, GetResource("T_BALANCE1"), DecimalPlaces, HorizontalAlignment.Right);
							AddColumnStyle(CN.Arrears, tabStyle, 90, true, GetResource("T_ARREARS"), DecimalPlaces, HorizontalAlignment.Right);
							AddColumnStyle(CN.DDPending, tabStyle, 90, true, GetResource("T_DDPENDING"), DecimalPlaces, HorizontalAlignment.Right);
							AddColumnStyle(CN.Amount, tabStyle, 90, false, GetResource("T_AMOUNT1"), DecimalPlaces, HorizontalAlignment.Right);

							// Icon column to tick consent for extra payments
							DataGridIconColumn iconColumn = new DataGridIconColumn(imageList1.Images[0], imageList1.Images[1], CN.Consent, "0", "1");
							iconColumn.HeaderText = "";
							iconColumn.MappingName = "TestIcon";
							iconColumn.Width = imageList1.Images[0].Size.Width;
							tabStyle.GridColumnStyles.Add(iconColumn);

							dgPaymentList.TableStyles.Add(tabStyle);
						}
						rowCount = _extraPaymentSet.Tables[TN.DDPaymentExtra].Rows.Count;
					}
					((MainForm)this.FormRoot).statusBar1.Text = rowCount.ToString() + GetResource("M_ACCOUNTSLISTED");
				}
			}
			catch(Exception ex)
			{
				Catch(ex, Function);
			}
			finally
			{
				_userChanged = true;
				StopWait();
				Function = "End of Extra Payment LoadDetails";
			}
		}    // End of LoadDetails


		/// <summary>
		/// Recalculate the amount pending.
		/// 'CurDDPending' and 'CurAmount' represent the values stored on
		/// the DB. 'Amount' is the new amount entered by the user.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="newAmount"></param>
		private void CalcDDPending(int index, decimal newAmount)
		{
			DataView paymentListView = this._extraPaymentSet.Tables[TN.DDPaymentExtra].DefaultView;
			DataRowView paymentRow = paymentListView[index];

			paymentRow[CN.Amount] = newAmount;

			paymentRow[CN.DDPending] =
				(decimal)paymentRow[CN.CurDDPending] -
				(decimal)paymentRow[CN.CurAmount] +
				(decimal)paymentRow[CN.Amount];
        
			if ((decimal)paymentRow[CN.DDPending] < 0.01M)
				paymentRow[CN.DDPending] = 0;
		}     /* End of CalcDDPending */


		// Copy the current amount to the user entered amount
		private void RestoreAmount(int index)
		{
			DataView paymentListView = this._extraPaymentSet.Tables[TN.DDPaymentExtra].DefaultView;
			DataRowView paymentRow = paymentListView[index];

			// Do not overwrite a non-zero amount already entered by the user
			if ((decimal)paymentRow[CN.Amount] == 0)
				paymentRow[CN.Amount] = (decimal)paymentRow[CN.CurAmount];

			this.CalcDDPending(index, (decimal)paymentRow[CN.Amount]);
		}     /* End of RestoreAmount */

		private bool ValidMoneyField(string moneyField, out decimal moneyValue)
		{
			// Check a blank or zero money value entered
			moneyValue = 0.0M;
			moneyField = moneyField.Trim();
			if (!IsStrictMoney(moneyField))
			{
				ShowInfo("M_NUMERIC");
				return false;
			}

			// Reformat
			moneyValue = MoneyStrToDecimal(moneyField);
			moneyField = moneyValue.ToString(DecimalPlaces);

			return true;
		}  // End of ValidMoneyField


		/// <summary>
		/// The tick box will never take focus.
		/// The Extra Payment Amount can only take focus if the row is ticked.
		/// So the user cannot tab onto the tick box nor past the end of a row.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="changeTick"></param>
		private void SetAmountFocus(int index, bool changeTick)
		{
			DataView paymentListView = this._extraPaymentSet.Tables[TN.DDPaymentExtra].DefaultView;
			DataRowView paymentRow = paymentListView[index];

			if (changeTick)
			{
				paymentRow[CN.Consent] = !(Convert.ToBoolean(paymentRow[CN.Consent]));
				if (!(Convert.ToBoolean(paymentRow[CN.Consent])))
				{
					this.CalcDDPending(index, 0);
				}
				else
					this.RestoreAmount(index);
			}
			if (Convert.ToBoolean(paymentRow[CN.Consent]))
			{
				// Move the focus to this Extra Payment Amount
				dgPaymentList.CurrentCell = new DataGridCell(index, _consentIndex-1);
			}
			else
			{
				// Move the focus to the previous column
				dgPaymentList.CurrentCell = new DataGridCell(index, _consentIndex-2);
			}
		}

		//
		// Form Events
		//
		private void btnSave_Click(object sender, System.EventArgs e)
		{
			try
			{
				Function = "Save Extra Payments";
				Wait();

				// Save the payment list
				bool partSave = false;
				((MainForm)this.FormRoot).statusBar1.Text = GetResource("M_SAVINGPAYMENTS");
				string acctNo = "";
				string customerName = "";
				do
				{
					this._extraPaymentSet = PaymentManager.SaveDDPaymentExtraList(this._extraPaymentSet, out acctNo, out customerName, out Error);
					if (Error.Length > 0)
						ShowError(Error);
					else
					{
						// Check whether an account and customer has been returned.
						// If they have then this row could not be saved because it
						// has been changed by another session. The user will be
						// prompted and ask if the rest of the list should be saved.
						if (acctNo != "")
						{
							partSave = true;
							if (DialogResult.No == ShowInfo("M_EXTRAPAYMENTSESSION",
								new object[] {acctNo, customerName}, MessageBoxButtons.YesNo))
								break;
						}
					}
				} while (acctNo != "");

				if (partSave) ShowInfo("M_PARTSAVELIST", new object[] {GetResource("T_EXTRAPAYMENTS")});
				// Reload the list
				this.LoadDetails();
			}
			catch(Exception ex)
			{
				Catch(ex, Function);
			}
			finally
			{
				StopWait();
				Function = "End of Save Extra Payments";
			}
		}

		private void btnClearAll_Click(object sender, System.EventArgs e)
		{
			try
			{
				Function = "Clear Extra Payments";
				Wait();

				int index = 0;
				foreach (DataRow paymentRow in this._extraPaymentSet.Tables[TN.DDPaymentExtra].Rows)
				{
					paymentRow[CN.Consent] = 0;
					paymentRow[CN.Amount] = 0;
					this.CalcDDPending(index,0);
					index++;
				}

			}
			catch(Exception ex)
			{
				Catch(ex, Function);
			}
			finally
			{
				StopWait();
				Function = "End of Clear Extra Payments";
			}
		}

		private void btnExit_Click(object sender, System.EventArgs e)
		{
			try
			{
				Function = "Exit Extra Payments";
				Wait();
				CloseTab();
			}
			catch(Exception ex)
			{
				Catch(ex, Function);
			}
			finally
			{
				StopWait();
				Function = "End of Exit Extra Payments";
			}
		}

		private void menuExit_Click(object sender, System.EventArgs e)
		{
			this.btnExit_Click(sender, e);
		}

		protected void ExtraPayment_ColumnChanging(object sender, System.Data.DataColumnChangeEventArgs e)
		{
			// Validate the Extra Payment amount
			try
			{
				Function = "ExtraPayment_ColumnChanging";

				if (_userChanged && e.Column.ColumnName == CN.Amount)
				{
					Wait();
					_userChanged = false;

					// Get the current row
					DataRow curPayment = e.Row;

					// Only change a ticked row
					if (Convert.ToBoolean(curPayment[CN.Consent]))
					{
						// Validate a number has been entered
						decimal newAmount = 0.0M;
						if (!ValidMoneyField(e.ProposedValue.ToString(), out newAmount))
						{
							dgPaymentList.CurrentCell = new DataGridCell(dgPaymentList.CurrentCell.RowNumber, dgPaymentList.CurrentCell.ColumnNumber);
							_userChanged = true;
							return;
						}

						// Validate the Extra Payment is between zero and the Outstanding Balance
						if (newAmount < 0 || newAmount > (decimal)curPayment[CN.OutstBal])
						{
							ShowInfo("M_VALIDEXTRA");
							e.ProposedValue = (decimal)curPayment[CN.Amount];
							dgPaymentList.CurrentCell = new DataGridCell(dgPaymentList.CurrentCell.RowNumber, _consentIndex-1);
							_userChanged = true;
							return;
						}

						// Write back the rounded value
						e.ProposedValue = newAmount;

						// Recalculate the Giro Pending
						this.CalcDDPending(dgPaymentList.CurrentCell.RowNumber, newAmount);
					}
					_userChanged = true;
				}
			}
			catch(Exception ex)
			{
				Catch(ex, Function);
			}
			finally
			{
				StopWait();
				Function = "End of ExtraPayment_ColumnChanging";
			}
	
		}

		private void dgPaymentList_CurrentCellChanged(object sender, System.EventArgs e)
		{
			try
			{
				Function = "dgPaymentList_CurrentCellChanged";

				if (_userChanged)
				{
					Wait();
					_userChanged = false;
					this._tickClickIndex = -1;

					int index = dgPaymentList.CurrentRowIndex;
					if (index >= 0)
					{
						if (dgPaymentList.CurrentCell.ColumnNumber == this._consentIndex-1)
						{
							// The focus is on the Extra Payment amount but this cannot
							// be changed if the row is not ticked, in which case move
							// the focus to the previous column.
							this.SetAmountFocus(index, false);
						}
						else if (dgPaymentList.CurrentCell.ColumnNumber == this._consentIndex)
						{
							// The focus is on the consent tick column but we don't know
							// whether the user tabbed here or clicked with the mouse.
							// So move the focus to a column on the row,
							// but first record the row number for the MouseUp event.
							this._tickClickIndex = index;
							this.SetAmountFocus(index, false);
						}
					}
					_userChanged = true;
				}
			}
			catch(Exception ex)
			{
				Catch(ex, Function);
			}
			finally
			{
				StopWait();
				Function = "End of dgPaymentList_CurrentCellChanged";
			}
		}

		private void dgPaymentList_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			try
			{
				Function = "dgPaymentList_MouseUp";

				if (_userChanged)
				{
					Wait();
					_userChanged = false;

					if (e.Button == MouseButtons.Right)
					{
						DataGrid ctl = (DataGrid)sender;

						MenuCommand m1 = new MenuCommand(GetResource("P_ACCOUNT_DETAILS"));
						m1.Click += new System.EventHandler(this.OnAccountDetails);

						PopupMenu popup = new PopupMenu();
						popup.Animate = Animate.Yes;
						popup.AnimateStyle = Animation.SlideHorVerPositive;

						popup.MenuCommands.AddRange(new MenuCommand[] {m1});
						MenuCommand selected = popup.TrackPopup(ctl.PointToScreen(new Point(e.X, e.Y)));
					}
					else
					{

						if (_tickClickIndex >= 0)
						{
							// The dgPaymentList_CurrentCellChanged event has fired first
							// and that moved the focus to another column in case the user
							// tabbed to the tick box. However, this event means the user
							// clicked the tick, so now the focus moves back to the amount
							// if it is enabled.
							this.SetAmountFocus(_tickClickIndex, true);
							_tickClickIndex = -1;
						}
						else
						{
							int index = dgPaymentList.CurrentRowIndex;
							if (index >= 0)
							{
								if (dgPaymentList.CurrentCell.ColumnNumber == this._consentIndex)
								{
									// The user has clicked on the same tick box again so the
									// dgPaymentList_CurrentCellChanged event has not fired.
									this.SetAmountFocus(index, true);
								}
							}
						}
					}
					_userChanged = true;
				}
			}
			catch(Exception ex)
			{
				Catch(ex, Function);
			}
			finally
			{
				StopWait();
				Function = "End of dgPaymentList_MouseUp";
			}
		}

		private void OnAccountDetails(object sender, System.EventArgs e)
		{
			try
			{
				Function = "OnAccountDetails";
				int index = dgPaymentList.CurrentRowIndex;

				if (index >= 0)
				{
					DataView paymentListView = this._extraPaymentSet.Tables[TN.DDPaymentExtra].DefaultView;
					DataRowView paymentRow = paymentListView[index];
					string acctNo = (string)paymentRow[CN.AccountNumber];

					if (acctNo.Length != 0)
					{
						AccountDetails details = new AccountDetails(acctNo.Replace("-",""), FormRoot, this);
						((MainForm)this.FormRoot).AddTabPage(details,7);
					}
					else
						ShowInfo("M_NOACCOUNT");
				}
			}
			catch(Exception ex)
			{
				Catch(ex, Function);
			}
		}


		private void menuAbout_Click(object sender, System.EventArgs e)
		{
			ShowInfo("M_ABOUTEXTRAPAYMENTS");
		}


	}
}
