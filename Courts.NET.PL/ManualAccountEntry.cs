using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;
using System.Collections.Specialized;
using STL.Common.Constants.TableNames;
using STL.Common.Constants.ColumnNames;
using STL.PL.WS2;
using System.Data;
using STL.Common.Static;
using STL.Common;
using System.Web.Services.Protocols;
using STL.Common.Constants.AccountTypes;

namespace STL.PL
{
	/// <summary>
	/// Popup prompt to request a manual account number to be entered. A list
	/// of new account numbers is always kept in reserve so that sales can
	/// continue in the event of a system failure. These accounts are subsequently
	/// entered into the system from a paper copy. In this case the reserved
	/// account numbers must be manually entered instead of being automatically
	/// generated by the new sales order screen.
	/// </summary>
	public class ManualAccountEntry : CommonForm
	{
		private string _accountType = "";
		public STL.PL.AccountTextBox txtAccountNo;
		public System.Windows.Forms.StatusBar statusBar;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.ErrorProvider errorProvider1;

		public bool valid = false;
		private System.Windows.Forms.Label lManualTitle;
		private string err = "";

		public ManualAccountEntry(TranslationDummy d)
		{
			InitializeComponent();
		}

		public ManualAccountEntry()
		{
			InitializeComponent();
		}

		public ManualAccountEntry(string accountType)
		{
			InitializeComponent();

			// Set up the field title for this type of manual account number
			this._accountType = accountType;
			string acctTypeName = "";
			switch (accountType)
			{
				case AT.ReadyFinance :
					acctTypeName = GetResource("T_ACTTYPERF");
					break;
				case AT.HP :
					acctTypeName = GetResource("T_ACTTYPEHP");
					break;
				case AT.Cash :
					acctTypeName = GetResource("T_ACTTYPECASH");
					break;
				default :
					break;
			}

			this.lManualTitle.Text = GetResource("M_MANUALACCTTYPE", new object[] {acctTypeName});
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
			this.txtAccountNo = new STL.PL.AccountTextBox();
			this.lManualTitle = new System.Windows.Forms.Label();
			this.statusBar = new System.Windows.Forms.StatusBar();
			this.errorProvider1 = new System.Windows.Forms.ErrorProvider();
			this.SuspendLayout();
			// 
			// txtAccountNo
			// 
			this.txtAccountNo.Location = new System.Drawing.Point(88, 72);
			this.txtAccountNo.Name = "txtAccountNo";
			this.txtAccountNo.Size = new System.Drawing.Size(94, 20);
			this.txtAccountNo.TabIndex = 1;
			this.txtAccountNo.Text = "000-0000-0000-0";
			this.txtAccountNo.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtAccountNo_KeyPress);
			// 
			// lManualTitle
			// 
			this.lManualTitle.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.lManualTitle.Location = new System.Drawing.Point(24, 32);
			this.lManualTitle.Name = "lManualTitle";
			this.lManualTitle.Size = new System.Drawing.Size(224, 16);
			this.lManualTitle.TabIndex = 25;
			this.lManualTitle.Text = "Please Enter Manual Account Number";
			// 
			// statusBar
			// 
			this.statusBar.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.statusBar.Location = new System.Drawing.Point(0, 133);
			this.statusBar.Name = "statusBar";
			this.statusBar.Size = new System.Drawing.Size(272, 16);
			this.statusBar.TabIndex = 26;
			// 
			// ManualAccountEntry
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(272, 149);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.statusBar,
																		  this.lManualTitle,
																		  this.txtAccountNo});
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ManualAccountEntry";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Manual Entry";
			this.ResumeLayout(false);

		}
		#endregion

		private void txtAccountNo_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
		{
			try
			{ 
				Wait();
				
				if(e.KeyChar == (char)13)
				{
					string acctNo = txtAccountNo.Text.Replace("-","");

					AccountManager.ValidateAccountNumber(acctNo, Config.CountryCode, this._accountType, out err);

					if(err.Length>0)
					{
						errorProvider1.SetError(txtAccountNo, err);
						txtAccountNo.Text = "000-0000-0000-0";
						txtAccountNo.Select(0, txtAccountNo.Text.Length);	
						this.statusBar.Text = "Invalid Account Number";
						valid = false;
					}
					else
					{
						errorProvider1.SetError(txtAccountNo, "");
						this.statusBar.Text = "Account Number Valid";
						valid = true;
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
	}
}
