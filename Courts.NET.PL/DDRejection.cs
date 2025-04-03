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
	/// Direct Debit Reject Actions
	/// This screen will list all direct debit accounts with a rejected payment.
	/// For each account the user can choose to not re-present; re-present or
	/// cancel the mandate. Each payment selected to re-present will be included
	/// in the next re-presentation file to be submitted to the bank. Mandates
	/// to be cancelled will be cancelled by the next Giro Housekeeping EOD run.
	/// Until then the user will be able to amend the rejection option.
	///
	/// Once an EOD run has completed and a re-presentation file has been created,
	/// the cancelled and re-presentation rows will no longer appear in the list.
	/// </summary>
	public class DDRejection : CommonForm
	{
		private new string Error = "";

		//
		// BL details for this form
		//
		private bool _userChanged = false;
		private DataSet _rejectionSet = null;
		private int _actionIndex;

		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.Button btnSave;
		private System.Windows.Forms.Button btnExit;
		private System.Windows.Forms.GroupBox gbRejection;
		private System.Windows.Forms.DataGrid dgPaymentList;
		private System.Windows.Forms.ImageList imageList1;
		private System.Windows.Forms.ComboBox drpAction;
		private System.Windows.Forms.Button btnMandate;
		private Crownwood.Magic.Menus.MenuCommand menuFile;
		private Crownwood.Magic.Menus.MenuCommand menuExit;
		private Crownwood.Magic.Menus.MenuCommand menuAbout;
		private Crownwood.Magic.Menus.MenuCommand menuHelp;
		private System.ComponentModel.IContainer components;

		//
		// Constructors
		//
		public DDRejection(TranslationDummy d)
		{
			InitializeComponent();
			menuMain = new Crownwood.Magic.Menus.MenuControl();
			menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {menuFile});
			menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {menuHelp});
		}

		public DDRejection(Form root, Form parent)
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
			toolTip1.SetToolTip(this.dgPaymentList, GetResource("TT_CLICKACTION"));
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
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.gbRejection = new System.Windows.Forms.GroupBox();
			this.drpAction = new System.Windows.Forms.ComboBox();
			this.btnExit = new System.Windows.Forms.Button();
			this.btnMandate = new System.Windows.Forms.Button();
			this.btnSave = new System.Windows.Forms.Button();
			this.dgPaymentList = new System.Windows.Forms.DataGrid();
			this.imageList1 = new System.Windows.Forms.ImageList(this.components);
			this.menuFile = new Crownwood.Magic.Menus.MenuCommand();
			this.menuExit = new Crownwood.Magic.Menus.MenuCommand();
			this.menuAbout = new Crownwood.Magic.Menus.MenuCommand();
			this.menuHelp = new Crownwood.Magic.Menus.MenuCommand();
			this.gbRejection.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dgPaymentList)).BeginInit();
			this.SuspendLayout();
			// 
			// gbRejection
			// 
			this.gbRejection.BackColor = System.Drawing.SystemColors.Control;
			this.gbRejection.Controls.AddRange(new System.Windows.Forms.Control[] {
																					  this.drpAction,
																					  this.btnExit,
																					  this.btnMandate,
																					  this.btnSave,
																					  this.dgPaymentList});
			this.gbRejection.Location = new System.Drawing.Point(8, 0);
			this.gbRejection.Name = "gbRejection";
			this.gbRejection.Size = new System.Drawing.Size(776, 472);
			this.gbRejection.TabIndex = 0;
			this.gbRejection.TabStop = false;
			// 
			// drpAction
			// 
			this.drpAction.Enabled = false;
			this.drpAction.Location = new System.Drawing.Point(640, 424);
			this.drpAction.Name = "drpAction";
			this.drpAction.Size = new System.Drawing.Size(121, 21);
			this.drpAction.TabIndex = 4;
			this.drpAction.Text = "comboBox1";
			this.drpAction.Visible = false;
			this.drpAction.SelectionChangeCommitted += new System.EventHandler(this.drpAction_SelectionChangeCommitted);
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
			// btnMandate
			// 
			this.btnMandate.Location = new System.Drawing.Point(344, 424);
			this.btnMandate.Name = "btnMandate";
			this.btnMandate.Size = new System.Drawing.Size(88, 24);
			this.btnMandate.TabIndex = 2;
			this.btnMandate.Text = "Mandate";
			this.btnMandate.Click += new System.EventHandler(this.btnMandate_Click);
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
			this.dgPaymentList.CaptionText = "Rejected Payments pending a Reject Action";
			this.dgPaymentList.DataMember = "";
			this.dgPaymentList.HeaderForeColor = System.Drawing.SystemColors.ControlText;
			this.dgPaymentList.Location = new System.Drawing.Point(48, 16);
			this.dgPaymentList.Name = "dgPaymentList";
			this.dgPaymentList.ReadOnly = true;
			this.dgPaymentList.Size = new System.Drawing.Size(672, 400);
			this.dgPaymentList.TabIndex = 0;
			this.dgPaymentList.MouseUp += new System.Windows.Forms.MouseEventHandler(this.dgPaymentList_MouseUp);
			this.dgPaymentList.CurrentCellChanged += new System.EventHandler(this.dgPaymentList_CurrentCellChanged);
			// 
			// imageList1
			// 
			this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
			this.imageList1.ImageSize = new System.Drawing.Size(16, 16);
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
			// menuAbout
			// 
			this.menuAbout.Description = "About Extra Payments";
			this.menuAbout.Text = "About Rejections";
			this.menuAbout.Click += new System.EventHandler(this.menuAbout_Click);
			// 
			// menuHelp
			// 
			this.menuHelp.Description = "MenuItem";
			this.menuHelp.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {
																							this.menuAbout});
			this.menuHelp.Text = "&Help";
			// 
			// DDRejection
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.BackColor = System.Drawing.SystemColors.ControlDark;
			this.ClientSize = new System.Drawing.Size(792, 477);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.gbRejection});
			this.Name = "DDRejection";
			this.Text = "Giro Reject Actions";
			this.Load += new System.EventHandler(this.DDRejection_Load);
			this.gbRejection.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dgPaymentList)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion

		//
		// Local Routines
		//

		private void DDRejection_Load(object sender, System.EventArgs e)
		{
			this.LoadStaticData();
			this.LoadDetails();
		}

		private void LoadStaticData()
		{
			XmlUtilities xml = new XmlUtilities();
			XmlDocument dropDowns = new XmlDocument();
			dropDowns.LoadXml("<DROP_DOWNS></DROP_DOWNS>");

			if (StaticData.Tables[TN.DDRejection] == null)
				dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns,TN.DDRejection, new string[]{"GRA", "L"}));

			if (dropDowns.DocumentElement.ChildNodes.Count > 0)
			{
				DataSet ds = StaticDataManager.GetDropDownData(dropDowns.DocumentElement, out Error);
				if (Error.Length > 0)
					ShowError(Error);
				else
				{
					foreach (DataTable dt in ds.Tables)
					{
						StaticData.Tables[dt.TableName] = dt;
					}
				}
			}			

			drpAction.DataSource = (DataTable)StaticData.Tables[TN.DDRejection];
			drpAction.ValueMember = CN.Code;
			drpAction.DisplayMember = CN.CodeDescription;
		}

		private void LoadDetails()
		{
			try
			{
				Wait();
				Function = "Rejection LoadDetails";
				this._userChanged = false;

				this._rejectionSet = null;
				this._rejectionSet = PaymentManager.GetDDRejectionList(Config.CountryCode, out Error);

				if (Error.Length > 0)
				{
					ShowError(Error);
				}
				else
				{
					// Load the data grid
					int rowCount = 0;
					dgPaymentList.DataSource = null;
					dgPaymentList.ResetText();

					if (this._rejectionSet != null)
					{
						dgPaymentList.DataSource = _rejectionSet.Tables[TN.DDRejection].DefaultView; 
						this._actionIndex = 11;

						if (dgPaymentList.TableStyles.Count == 0)
						{
							DataGridTableStyle tabStyle = new DataGridTableStyle();
							tabStyle.MappingName = _rejectionSet.Tables[TN.DDRejection].TableName;

							AddColumnStyle(CN.MandateId, tabStyle, 0, true, "", "", HorizontalAlignment.Left);
							AddColumnStyle(CN.PaymentId, tabStyle, 0, true, "", "", HorizontalAlignment.Left);
							AddColumnStyle(CN.PaymentType, tabStyle, 0, true, "", "", HorizontalAlignment.Left);
							AddColumnStyle(CN.OrigMonth, tabStyle, 0, true, "", "", HorizontalAlignment.Left);
							AddColumnStyle(CN.RejectAction, tabStyle, 0, true, "", "", HorizontalAlignment.Left);
							AddColumnStyle(CN.CurRejectAction, tabStyle, 0, true, "", "", HorizontalAlignment.Left);

							AddColumnStyle(CN.AccountNumber, tabStyle, 90, true, GetResource("T_ACCOUNTNO"), "", HorizontalAlignment.Left);
							AddColumnStyle(CN.CustomerName, tabStyle, 200, true, GetResource("T_CUSTOMERNAME"), "", HorizontalAlignment.Left);
							AddColumnStyle(CN.MonthName, tabStyle, 50, true, GetResource("T_MONTH"), "", HorizontalAlignment.Left);
							AddColumnStyle(CN.DateEffective, tabStyle, 80, true, GetResource("T_EFFECTIVE"), "", HorizontalAlignment.Left);
							AddColumnStyle(CN.Amount, tabStyle, 90, true, GetResource("T_AMOUNT1"), DecimalPlaces, HorizontalAlignment.Right);
							AddColumnStyle(CN.RejectActionStr, tabStyle, 120, true, GetResource("T_ACTION"), "", HorizontalAlignment.Left);

							dgPaymentList.TableStyles.Add(tabStyle);
						}
					}
					rowCount = _rejectionSet.Tables[TN.DDRejection].Rows.Count;
					((MainForm)this.FormRoot).statusBar1.Text = rowCount.ToString() + GetResource("M_ACCOUNTSLISTED");
				}
			}
			catch(Exception ex)
			{
				Catch(ex, Function);
			}
			finally
			{
				this._userChanged = true;
				StopWait();
				Function = "End of Rejection LoadDetails";
			}
		}    // End of LoadDetails




		private void ShowOption (bool enable, string action)
		{
			if (enable)
			{
				int left = dgPaymentList.GetCurrentCellBounds().Left;
				int top = dgPaymentList.GetCurrentCellBounds().Top;
				int width = dgPaymentList.GetCurrentCellBounds().Width;

				drpAction.Left = left + dgPaymentList.Left;
				drpAction.Top = top + dgPaymentList.Top;
				drpAction.Width = width;
				int i = drpAction.FindStringExact(action);
				drpAction.SelectedIndex = i >= 0 ? i : 0;									
			}

			drpAction.Visible = enable;
			drpAction.Enabled = enable;
		}


		//
		// Form Events
		//
		private void btnSave_Click(object sender, System.EventArgs e)
		{
			try
			{
				Function = "Save Reject Asctions";
				Wait();

				// Save the payment list
				bool partSave = false;
				((MainForm)this.FormRoot).statusBar1.Text = GetResource("M_SAVINGPAYMENTS");
				string acctNo = "";
				string customerName = "";
				do
				{
					this._rejectionSet = PaymentManager.SaveDDRejectionList(this._rejectionSet, out acctNo, out customerName, out Error);
					if (Error.Length > 0)
						ShowError(Error);
					else
					{
						// Check whether an account and customer has been returned.
						// If they have then this row could not be saved because it
						// has been changed by another session. THe user will be
						// prompted and ask if the rest of the list should be saved.
						if (acctNo != "")
						{
							partSave = true;
							if (DialogResult.No == ShowInfo("M_REJECTIONSESSION",
								new object[] {acctNo, customerName}, MessageBoxButtons.YesNo))
								break;
						}
					}
				} while (acctNo != "");

				if (partSave) ShowInfo("M_PARTSAVELIST", new object[] {GetResource("T_REJECTIONS")});
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
				Function = "End of Save Reject Actions";
			}
		}

		private void btnExit_Click(object sender, System.EventArgs e)
		{
			try
			{
				Function = "Exit Reject Actions";
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
				Function = "End of Exit Reject Actions";
			}
		}

		private void menuExit_Click(object sender, System.EventArgs e)
		{
			this.btnExit_Click(sender, e);
		}

		private void dgPaymentList_CurrentCellChanged(object sender, System.EventArgs e)
		{
			Function = "dgPaymentList_CurrentCellChanged";
			try
			{
				if (_userChanged)
				{
					Wait();
					_userChanged = false;

					int index = dgPaymentList.CurrentRowIndex;
					if (index >= 0)
					{
						DataView paymentListView = (DataView)dgPaymentList.DataSource;
						DataRowView paymentRow = paymentListView[index];

						if (dgPaymentList.CurrentCell.ColumnNumber == this._actionIndex)
						{
							// The focus is on the reject action column
							this.ShowOption(true, (string)paymentRow[CN.RejectActionStr]);
						}
						else
						{
							// Hide the option list
							this.ShowOption(false, "");
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


		private void drpAction_SelectionChangeCommitted(object sender, System.EventArgs e)
		{
			Function = "drpAction_SelectionChangeCommitted";
			try
			{
				if (_userChanged)
				{
					Wait();
					_userChanged = false;

					int index = dgPaymentList.CurrentRowIndex;
					if (index >= 0)
					{
						DataView paymentListView = this._rejectionSet.Tables[TN.DDRejection].DefaultView;
						DataRowView paymentRow = paymentListView[index];
						paymentRow[CN.RejectAction] = drpAction.SelectedValue;
						paymentRow[CN.RejectActionStr] = drpAction.Text;
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
				Function = "End of drpAction_SelectedIndexChanged";
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
						MenuCommand m2 = new MenuCommand(GetResource("P_MANDATE_DETAILS"));
						m1.Click += new System.EventHandler(this.OnAccountDetails);
						m2.Click += new System.EventHandler(this.btnMandate_Click);

						PopupMenu popup = new PopupMenu();
						popup.Animate = Animate.Yes;
						popup.AnimateStyle = Animation.SlideHorVerPositive;

						popup.MenuCommands.AddRange(new MenuCommand[] {m1,m2});
						MenuCommand selected = popup.TrackPopup(ctl.PointToScreen(new Point(e.X, e.Y)));
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
					DataView paymentListView = this._rejectionSet.Tables[TN.DDRejection].DefaultView;
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

		private void btnMandate_Click(object sender, System.EventArgs e)
		{
			try
			{
				Function = "btnMandate_Click";
				int index = dgPaymentList.CurrentRowIndex;

				if (index >= 0)
				{
					DataView paymentListView = this._rejectionSet.Tables[TN.DDRejection].DefaultView;
					DataRowView paymentRow = paymentListView[index];
					string acctNo = (string)paymentRow[CN.AccountNumber];

					if (acctNo.Length != 0)
					{
						DDMandate details = new DDMandate(FormRoot, this, 0, acctNo.Replace("-",""), "", "", false);
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
			ShowInfo("M_ABOUTREJECTIONS");
		}


	}
}
