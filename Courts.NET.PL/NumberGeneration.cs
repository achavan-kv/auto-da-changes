using System;
using System.Data;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using STL.Common.Constants.TableNames;
using STL.Common.Constants.ColumnNames;
using System.Xml;
using STL.Common;
using STL.Common.Static;



namespace STL.PL
{
	/// <summary>
	/// New account numbers and contract numbers can be generated here.
	/// A list of new account numbers and contract numbers is always kept in
	/// reserve so that sales can continue in the event of a system failure.
	/// These numbers are subsequently entered into the system from a paper copy,
	/// instead of being automatically generated.

	/// </summary>
	public class NumberGeneration : CommonForm
	{

		private string _error = "";
		private Crownwood.Magic.Menus.MenuCommand menuFile;
		private Crownwood.Magic.Menus.MenuCommand menuExit;
		private System.Windows.Forms.GroupBox gbMain;
		private System.Windows.Forms.GroupBox gbContractNo;
		public System.Windows.Forms.Button btnPrintContractNo;
		private System.Windows.Forms.NumericUpDown numContractNo;
		private System.Windows.Forms.GroupBox gbAccountNo;
		private System.Windows.Forms.Label lAccountType;
		private System.Windows.Forms.ComboBox drpAccountType;
		public System.Windows.Forms.Button btnPrintAccountNo;
		private System.Windows.Forms.NumericUpDown numAccountNo;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public NumberGeneration()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			menuMain = new Crownwood.Magic.Menus.MenuControl();
			menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {menuFile});
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(NumberGeneration));
			this.menuFile = new Crownwood.Magic.Menus.MenuCommand();
			this.menuExit = new Crownwood.Magic.Menus.MenuCommand();
			this.gbMain = new System.Windows.Forms.GroupBox();
			this.gbContractNo = new System.Windows.Forms.GroupBox();
			this.btnPrintContractNo = new System.Windows.Forms.Button();
			this.numContractNo = new System.Windows.Forms.NumericUpDown();
			this.gbAccountNo = new System.Windows.Forms.GroupBox();
			this.lAccountType = new System.Windows.Forms.Label();
			this.drpAccountType = new System.Windows.Forms.ComboBox();
			this.btnPrintAccountNo = new System.Windows.Forms.Button();
			this.numAccountNo = new System.Windows.Forms.NumericUpDown();
			this.gbMain.SuspendLayout();
			this.gbContractNo.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.numContractNo)).BeginInit();
			this.gbAccountNo.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.numAccountNo)).BeginInit();
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
			this.menuExit.Description = "Exit";
			this.menuExit.Text = "Exit";
			this.menuExit.Click += new System.EventHandler(this.menuExit_Click);
			// 
			// gbMain
			// 
			this.gbMain.Controls.AddRange(new System.Windows.Forms.Control[] {
																				 this.gbContractNo,
																				 this.gbAccountNo});
			this.gbMain.Location = new System.Drawing.Point(8, 0);
			this.gbMain.Name = "gbMain";
			this.gbMain.Size = new System.Drawing.Size(776, 472);
			this.gbMain.TabIndex = 1;
			this.gbMain.TabStop = false;
			// 
			// gbContractNo
			// 
			this.gbContractNo.Controls.AddRange(new System.Windows.Forms.Control[] {
																					   this.btnPrintContractNo,
																					   this.numContractNo});
			this.gbContractNo.Location = new System.Drawing.Point(200, 252);
			this.gbContractNo.Name = "gbContractNo";
			this.gbContractNo.Size = new System.Drawing.Size(360, 104);
			this.gbContractNo.TabIndex = 404;
			this.gbContractNo.TabStop = false;
			this.gbContractNo.Text = "Contract Numbers";
			// 
			// btnPrintContractNo
			// 
			this.btnPrintContractNo.Image = ((System.Drawing.Bitmap)(resources.GetObject("btnPrintContractNo.Image")));
			this.btnPrintContractNo.Location = new System.Drawing.Point(280, 40);
			this.btnPrintContractNo.Name = "btnPrintContractNo";
			this.btnPrintContractNo.Size = new System.Drawing.Size(36, 30);
			this.btnPrintContractNo.TabIndex = 420;
			this.btnPrintContractNo.Click += new System.EventHandler(this.btnPrintContractNos_Click);
			// 
			// numContractNo
			// 
			this.numContractNo.Location = new System.Drawing.Point(128, 48);
			this.numContractNo.Name = "numContractNo";
			this.numContractNo.Size = new System.Drawing.Size(96, 20);
			this.numContractNo.TabIndex = 410;
			// 
			// gbAccountNo
			// 
			this.gbAccountNo.Controls.AddRange(new System.Windows.Forms.Control[] {
																					  this.lAccountType,
																					  this.drpAccountType,
																					  this.btnPrintAccountNo,
																					  this.numAccountNo});
			this.gbAccountNo.Location = new System.Drawing.Point(200, 108);
			this.gbAccountNo.Name = "gbAccountNo";
			this.gbAccountNo.Size = new System.Drawing.Size(360, 104);
			this.gbAccountNo.TabIndex = 403;
			this.gbAccountNo.TabStop = false;
			this.gbAccountNo.Text = "Account Numbers";
			// 
			// lAccountType
			// 
			this.lAccountType.Location = new System.Drawing.Point(32, 48);
			this.lAccountType.Name = "lAccountType";
			this.lAccountType.Size = new System.Drawing.Size(88, 16);
			this.lAccountType.TabIndex = 0;
			this.lAccountType.Text = "Account Type";
			this.lAccountType.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// drpAccountType
			// 
			this.drpAccountType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.drpAccountType.ItemHeight = 13;
			this.drpAccountType.Location = new System.Drawing.Point(128, 48);
			this.drpAccountType.Name = "drpAccountType";
			this.drpAccountType.Size = new System.Drawing.Size(40, 21);
			this.drpAccountType.TabIndex = 310;
			// 
			// btnPrintAccountNo
			// 
			this.btnPrintAccountNo.Image = ((System.Drawing.Bitmap)(resources.GetObject("btnPrintAccountNo.Image")));
			this.btnPrintAccountNo.Location = new System.Drawing.Point(280, 40);
			this.btnPrintAccountNo.Name = "btnPrintAccountNo";
			this.btnPrintAccountNo.Size = new System.Drawing.Size(32, 32);
			this.btnPrintAccountNo.TabIndex = 330;
			this.btnPrintAccountNo.Click += new System.EventHandler(this.btnPrintAccountNos_Click);
			// 
			// numAccountNo
			// 
			this.numAccountNo.Location = new System.Drawing.Point(192, 48);
			this.numAccountNo.Name = "numAccountNo";
			this.numAccountNo.Size = new System.Drawing.Size(64, 20);
			this.numAccountNo.TabIndex = 320;
			// 
			// NumberGeneration
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(792, 477);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.gbMain});
			this.Name = "NumberGeneration";
			this.Text = "Number Generation";
			this.Load += new System.EventHandler(this.NumberGeneration_Load);
			this.gbMain.ResumeLayout(false);
			this.gbContractNo.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.numContractNo)).EndInit();
			this.gbAccountNo.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.numAccountNo)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion

		private void NumberGeneration_Load(object sender, System.EventArgs e)
		{
			LoadStaticData();
		}

		private void LoadStaticData()
		{
			XmlUtilities xml = new XmlUtilities();
			XmlDocument dropDowns = new XmlDocument();
			dropDowns.LoadXml("<DROP_DOWNS></DROP_DOWNS>");
				
			if(StaticData.Tables[TN.AccountType]==null)
				dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns,TN.AccountType, null));
				
			if (dropDowns.DocumentElement.ChildNodes.Count > 0)
			{
				DataSet ds = StaticDataManager.GetDropDownData(dropDowns.DocumentElement, out _error);
				if (_error.Length > 0)
					ShowError(_error);
				else
				{
					foreach (DataTable dt in ds.Tables)
					{
						StaticData.Tables[dt.TableName] = dt;
					}
				}
			}
			drpAccountType.DataSource = StaticData.Tables[TN.AccountType];
			drpAccountType.DisplayMember = CN.AcctCat;
		}

		private void btnPrintAccountNos_Click(object sender, System.EventArgs e)
		{
			try
			{
				Wait();
				if (numAccountNo.Value > 0)
				{
					((MainForm)this.FormRoot).statusBar1.Text = GetResource("M_NUMBERACCOUNTPRINT");
					string accountType = (string)((DataRowView)drpAccountType.SelectedItem)[CN.AcctCat];
					PrintAccountNos(CreateBrowserArray(1)[0], numAccountNo.Value, accountType);
					((MainForm)this.FormRoot).statusBar1.Text = GetResource("M_NUMBERACCOUNTPRINTED");
				}
			}
			catch(Exception ex)
			{
				Catch(ex, "btnPrintAccountNos_Click");
			}
			finally
			{
				if (numAccountNo.Value < 1) ((MainForm)this.FormRoot).statusBar1.Text = "";
				StopWait();
			}
		}

		private void btnPrintContractNos_Click(object sender, System.EventArgs e)
		{
			try
			{
				Wait();
				if (numContractNo.Value > 0)
				{
					((MainForm)this.FormRoot).statusBar1.Text = GetResource("M_NUMBERCONTRACTPRINT");
					PrintContractNos(CreateBrowserArray(1)[0], numContractNo.Value);
					((MainForm)this.FormRoot).statusBar1.Text = GetResource("M_NUMBERCONTRACTPRINTED");
				}

			}
			catch(Exception ex)
			{
				Catch(ex, "btnPrintContractNos_Click");
			}
			finally
			{
				if (numContractNo.Value < 1) ((MainForm)this.FormRoot).statusBar1.Text = "";
				StopWait();
			}		
		}

		private void menuExit_Click(object sender, System.EventArgs e)
		{
			try
			{
				Wait();
				CloseTab();
			}
			catch(Exception ex)
			{
				Catch(ex, "menuExit_Click");
			}
			finally
			{
				StopWait();
			}
		}

	}
}
