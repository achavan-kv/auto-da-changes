using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using STL.Common;
using STL.Common.Constants.TableNames;
using STL.Common.Constants.ColumnNames;
using System.Xml;
using System.Data;
using STL.Common.Static;
using Crownwood.Magic.Menus;

namespace STL.PL
{
	/// <summary>
	/// Allows a cheque to be returned. The cheque has to be found based
	/// either on the cheque number or the bank account number. Once
	/// the cheque is listed the transaction can be reversed.
	/// </summary>
	public class ReturnCheque : CommonForm
	{
		//private Crownwood.Magic.Menus.MenuControl menuMain;
		private Crownwood.Magic.Menus.MenuCommand menuFile;
		private Crownwood.Magic.Menus.MenuCommand menuExit;
		private System.Windows.Forms.GroupBox gbSearchCriteria;
		private System.Windows.Forms.TextBox txtChequeNo;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ComboBox drpBankCode;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox txtBankAcctNo;
		private System.Windows.Forms.DateTimePicker dtTo;
		private System.Windows.Forms.DateTimePicker dtFrom;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Button btnLoad;
		private System.Windows.Forms.Button btnExit;
		private System.Windows.Forms.Button btnClear;
		private System.Windows.Forms.GroupBox gbResult;
		private System.Windows.Forms.DataGrid dgResults;
		private new string Error = "";
		private System.Windows.Forms.ErrorProvider errors;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public ReturnCheque(TranslationDummy d)
		{
			InitializeComponent();
			menuMain = new Crownwood.Magic.Menus.MenuControl();
			menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {menuFile});
		}

		public ReturnCheque(Form root, Form parent)
		{
			InitializeComponent();
			menuMain = new Crownwood.Magic.Menus.MenuControl();
			menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {menuFile});
			FormRoot = root;
			FormParent = parent;
			LoadStaticData();

			dtFrom.Value = new DateTime(DateTime.Now.Year, 
				DateTime.Now.Month,
				DateTime.Now.Day, 0,0,0);

			dtTo.Value = new DateTime(DateTime.Now.Year, 
				DateTime.Now.Month,
				DateTime.Now.Day, 23,59,59);
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
			this.menuFile = new Crownwood.Magic.Menus.MenuCommand();
			this.menuExit = new Crownwood.Magic.Menus.MenuCommand();
			this.gbSearchCriteria = new System.Windows.Forms.GroupBox();
			this.btnLoad = new System.Windows.Forms.Button();
			this.btnExit = new System.Windows.Forms.Button();
			this.btnClear = new System.Windows.Forms.Button();
			this.dtTo = new System.Windows.Forms.DateTimePicker();
			this.dtFrom = new System.Windows.Forms.DateTimePicker();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.txtBankAcctNo = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.drpBankCode = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.txtChequeNo = new System.Windows.Forms.TextBox();
			this.gbResult = new System.Windows.Forms.GroupBox();
			this.dgResults = new System.Windows.Forms.DataGrid();
			this.errors = new System.Windows.Forms.ErrorProvider();
			this.gbSearchCriteria.SuspendLayout();
			this.gbResult.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dgResults)).BeginInit();
			this.SuspendLayout();
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
			this.menuExit.Text = "E&xit";
			this.menuExit.Click += new System.EventHandler(this.menuExit_Click);
			// 
			// gbSearchCriteria
			// 
			this.gbSearchCriteria.BackColor = System.Drawing.SystemColors.Control;
			this.gbSearchCriteria.Controls.AddRange(new System.Windows.Forms.Control[] {
																						   this.btnLoad,
																						   this.btnExit,
																						   this.btnClear,
																						   this.dtTo,
																						   this.dtFrom,
																						   this.label4,
																						   this.label5,
																						   this.label3,
																						   this.txtBankAcctNo,
																						   this.label2,
																						   this.drpBankCode,
																						   this.label1,
																						   this.txtChequeNo});
			this.gbSearchCriteria.Location = new System.Drawing.Point(8, 0);
			this.gbSearchCriteria.Name = "gbSearchCriteria";
			this.gbSearchCriteria.Size = new System.Drawing.Size(776, 136);
			this.gbSearchCriteria.TabIndex = 0;
			this.gbSearchCriteria.TabStop = false;
			this.gbSearchCriteria.Text = "Search Criteria";
			// 
			// btnLoad
			// 
			this.errors.SetIconAlignment(this.btnLoad, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
			this.btnLoad.Location = new System.Drawing.Point(712, 17);
			this.btnLoad.Name = "btnLoad";
			this.btnLoad.Size = new System.Drawing.Size(48, 23);
			this.btnLoad.TabIndex = 5;
			this.btnLoad.Text = "Load";
			this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
			// 
			// btnExit
			// 
			this.btnExit.Location = new System.Drawing.Point(712, 97);
			this.btnExit.Name = "btnExit";
			this.btnExit.Size = new System.Drawing.Size(48, 23);
			this.btnExit.TabIndex = 7;
			this.btnExit.Text = "Exit";
			this.btnExit.Click += new System.EventHandler(this.menuExit_Click);
			// 
			// btnClear
			// 
			this.btnClear.Location = new System.Drawing.Point(712, 57);
			this.btnClear.Name = "btnClear";
			this.btnClear.Size = new System.Drawing.Size(48, 23);
			this.btnClear.TabIndex = 6;
			this.btnClear.Text = "Clear";
			this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
			// 
			// dtTo
			// 
			this.dtTo.CustomFormat = "ddd dd MMM yyyy HH:mm";
			this.dtTo.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
			this.dtTo.Location = new System.Drawing.Point(200, 88);
			this.dtTo.Name = "dtTo";
			this.dtTo.Size = new System.Drawing.Size(144, 20);
			this.dtTo.TabIndex = 4;
			this.dtTo.Tag = "";
			this.dtTo.Value = new System.DateTime(2002, 5, 8, 14, 5, 24, 830);
			// 
			// dtFrom
			// 
			this.dtFrom.CustomFormat = "ddd dd MMM yyyy HH:mm";
			this.dtFrom.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
			this.dtFrom.Location = new System.Drawing.Point(32, 88);
			this.dtFrom.Name = "dtFrom";
			this.dtFrom.Size = new System.Drawing.Size(144, 20);
			this.dtFrom.TabIndex = 3;
			this.dtFrom.Tag = "";
			this.dtFrom.Value = new System.DateTime(2002, 5, 8, 14, 5, 24, 830);
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(200, 72);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(32, 16);
			this.label4.TabIndex = 9;
			this.label4.Text = "To:";
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(32, 72);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(40, 16);
			this.label5.TabIndex = 8;
			this.label5.Text = "From:";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(376, 24);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(102, 16);
			this.label3.TabIndex = 5;
			this.label3.Text = "Bank Account No:";
			// 
			// txtBankAcctNo
			// 
			this.txtBankAcctNo.Location = new System.Drawing.Point(376, 40);
			this.txtBankAcctNo.MaxLength = 20;
			this.txtBankAcctNo.Name = "txtBankAcctNo";
			this.txtBankAcctNo.Size = new System.Drawing.Size(128, 20);
			this.txtBankAcctNo.TabIndex = 2;
			this.txtBankAcctNo.Text = "";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(200, 24);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(64, 16);
			this.label2.TabIndex = 3;
			this.label2.Text = "Bank Code:";
			// 
			// drpBankCode
			// 
			this.drpBankCode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.drpBankCode.Location = new System.Drawing.Point(200, 40);
			this.drpBankCode.Name = "drpBankCode";
			this.drpBankCode.Size = new System.Drawing.Size(144, 21);
			this.drpBankCode.TabIndex = 1;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(32, 24);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(64, 16);
			this.label1.TabIndex = 1;
			this.label1.Text = "Cheque No:";
			// 
			// txtChequeNo
			// 
			this.txtChequeNo.Location = new System.Drawing.Point(32, 40);
			this.txtChequeNo.MaxLength = 16;
			this.txtChequeNo.Name = "txtChequeNo";
			this.txtChequeNo.Size = new System.Drawing.Size(128, 20);
			this.txtChequeNo.TabIndex = 0;
			this.txtChequeNo.Text = "";
			// 
			// gbResult
			// 
			this.gbResult.BackColor = System.Drawing.SystemColors.Control;
			this.gbResult.Controls.AddRange(new System.Windows.Forms.Control[] {
																				   this.dgResults});
			this.gbResult.Location = new System.Drawing.Point(8, 136);
			this.gbResult.Name = "gbResult";
			this.gbResult.Size = new System.Drawing.Size(776, 336);
			this.gbResult.TabIndex = 1;
			this.gbResult.TabStop = false;
			this.gbResult.Text = "Results";
			// 
			// dgResults
			// 
			this.dgResults.CaptionVisible = false;
			this.dgResults.DataMember = "";
			this.dgResults.HeaderForeColor = System.Drawing.SystemColors.ControlText;
			this.dgResults.Location = new System.Drawing.Point(64, 44);
			this.dgResults.Name = "dgResults";
			this.dgResults.ReadOnly = true;
			this.dgResults.Size = new System.Drawing.Size(648, 248);
			this.dgResults.TabIndex = 1;
			this.dgResults.TabStop = false;
			this.dgResults.MouseUp += new System.Windows.Forms.MouseEventHandler(this.dgResults_MouseUp);
			// 
			// ReturnCheque
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.BackColor = System.Drawing.SystemColors.ControlDark;
			this.ClientSize = new System.Drawing.Size(792, 477);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.gbResult,
																		  this.gbSearchCriteria});
			this.Name = "ReturnCheque";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Cheque Return";
			this.gbSearchCriteria.ResumeLayout(false);
			this.gbResult.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dgResults)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion

		private void menuExit_Click(object sender, System.EventArgs e)
		{
			CloseTab();
		}

		private void dgResults_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			int index = dgResults.CurrentRowIndex;

			if(index>=0)
			{
				dgResults.Select(dgResults.CurrentCell.RowNumber);

				if (e.Button == MouseButtons.Right)
				{
					DataGrid ctl = (DataGrid)sender;

					MenuCommand m1 = new MenuCommand(GetResource("P_REVERSETRANSACTION"));
					m1.Click += new System.EventHandler(this.OnReverseTransaction);

					PopupMenu popup = new PopupMenu();
					popup.Animate = Animate.Yes;
					popup.AnimateStyle = Animation.SlideHorVerPositive;

					popup.MenuCommands.Add(m1);
					MenuCommand selected = popup.TrackPopup(ctl.PointToScreen(new Point(e.X, e.Y)));
				}
			}
		}

		private void OnReverseTransaction(object sender, System.EventArgs e)
		{
			try
			{
				Wait();

				int i = dgResults.CurrentRowIndex;

				PaymentManager.ReverseCheque(	(string)((DataView)dgResults.DataSource)[i][CN.AccountNumber],
					(string)((DataView)dgResults.DataSource)[i][CN.ChequeNo],
					(string)((DataView)dgResults.DataSource)[i][CN.BankCode],
					(string)((DataView)dgResults.DataSource)[i][CN.BankAccountNo2],
					(decimal)((DataView)dgResults.DataSource)[i][CN.TransValue],
					(short)((DataView)dgResults.DataSource)[i][CN.PayMethod],
					(short)((DataView)dgResults.DataSource)[i][CN.BranchNo],
					(string)((DataView)dgResults.DataSource)[i][CN.TransTypeCode],
					(DateTime)((DataView)dgResults.DataSource)[i][CN.DateTrans],
					(int)((DataView)dgResults.DataSource)[i][CN.TransRefNo],
					Config.CountryCode,
					out Error );
					
				if(Error.Length > 0)
					ShowError(Error);
				else
					btnLoad_Click(null, null);

			}
			catch(Exception ex)
			{
				Catch(ex, "OnReverseTransaction");
			}
			finally
			{
				StopWait();
			}
		}

		private void LoadStaticData()
		{
			try
			{
				Wait();
		
				XmlUtilities xml = new XmlUtilities();
				XmlDocument dropDowns = new XmlDocument();
				dropDowns.LoadXml("<DROP_DOWNS></DROP_DOWNS>");

				if(StaticData.Tables[TN.Bank]==null)
					dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns,TN.Bank, null));

				if(dropDowns.DocumentElement.ChildNodes.Count>0)
				{
					DataSet ds = StaticDataManager.GetDropDownData(dropDowns.DocumentElement, out Error);
					if(Error.Length>0)
						ShowError(Error);
					else
					{
						foreach(DataTable dt in ds.Tables)
							StaticData.Tables[dt.TableName] = dt;
					}
				}			
				drpBankCode.DataSource = (DataTable)StaticData.Tables[TN.Bank];
				drpBankCode.DisplayMember = CN.BankName;
			}
			catch(Exception ex)
			{
				Catch(ex, "LoadStaticData");
			}
			finally
			{
				StopWait();
			}
		}

		private void btnLoad_Click(object sender, System.EventArgs e)
		{
			try
			{
				Wait();

				if(!(txtChequeNo.Text.Length>0 || txtBankAcctNo.Text.Length>0))
					errors.SetError(btnLoad, GetResource("M_ENTERCHEQUNO"));
				else
				{
					errors.SetError(btnLoad, "");

					DataSet ds	 = PaymentManager.GetChequeDetails(txtChequeNo.Text,
						(string)((DataRowView)drpBankCode.SelectedItem)[CN.BankCode],
						txtBankAcctNo.Text,
						dtFrom.Value,
						dtTo.Value,
						out Error);
					if(Error.Length>0)
						ShowError(Error);
					else
					{
						dgResults.DataSource = ds.Tables[0].DefaultView;

						if(dgResults.TableStyles.Count==0)
						{
							DataGridTableStyle tabStyle = new DataGridTableStyle();
							tabStyle.MappingName = ds.Tables[0].DefaultView.Table.TableName;
							dgResults.TableStyles.Add(tabStyle);
			
							tabStyle.GridColumnStyles[CN.AccountNumber].HeaderText = GetResource("T_ACCTNO");
							tabStyle.GridColumnStyles[CN.AccountNumber].Width = 120;
							tabStyle.GridColumnStyles[CN.BankName].HeaderText = GetResource("T_BANKNAME");
							tabStyle.GridColumnStyles[CN.BankName].Width = 120;
							tabStyle.GridColumnStyles[CN.BankAccountNo2].HeaderText = GetResource("T_BANKACCTNO");
							tabStyle.GridColumnStyles[CN.BankAccountNo2].Width = 120;
							tabStyle.GridColumnStyles[CN.ChequeNo].HeaderText = GetResource("T_CHEQUENO");
							tabStyle.GridColumnStyles[CN.TransValue].HeaderText = GetResource("T_LOCALAMOUNT");
							tabStyle.GridColumnStyles[CN.TransValue].Alignment = HorizontalAlignment.Right;
							((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.TransValue]).Format = DecimalPlaces;
							tabStyle.GridColumnStyles[CN.CodeDescript].HeaderText = GetResource("T_PAYMETHOD");
							tabStyle.GridColumnStyles[CN.CodeDescript].Width = 100;
							tabStyle.GridColumnStyles[CN.PayMethod].Width = 0;
							tabStyle.GridColumnStyles[CN.BankCode].Width = 0;
							tabStyle.GridColumnStyles[CN.BranchNo].Width = 0;
							tabStyle.GridColumnStyles[CN.TransTypeCode].Width = 0;
							tabStyle.GridColumnStyles[CN.DateTrans].Width = 0;
							tabStyle.GridColumnStyles[CN.TransRefNo].Width = 0;
						}
						((MainForm)FormRoot).statusBar1.Text = ((DataView)dgResults.DataSource).Count.ToString() + " Rows returned";
					}
				}
			}
			catch(Exception ex)
			{
				Catch(ex, "btnLoad_Click");
			}
			finally
			{
				StopWait();
			}
		}

		private void btnClear_Click(object sender, System.EventArgs e)
		{
			try
			{
				Wait();

				dtFrom.Value = new DateTime(DateTime.Now.Year, 
					DateTime.Now.Month,
					DateTime.Now.Day, 0,0,0);

				dtTo.Value = new DateTime(DateTime.Now.Year, 
					DateTime.Now.Month,
					DateTime.Now.Day, 23,59,59);

				txtChequeNo.Text = "";
				drpBankCode.SelectedIndex = 0;
				txtBankAcctNo.Text = "";

				if(dgResults.DataSource!=null)
					((DataView)dgResults.DataSource).Table.Clear();
			}
			catch(Exception ex)
			{
				Catch(ex, "btnTotal_Click");
			}
			finally
			{
				StopWait();
			}			
		}
	}
}
