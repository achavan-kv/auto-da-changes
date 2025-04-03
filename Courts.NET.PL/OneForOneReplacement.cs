using mshtml;
using System;
using System.Data;
using System.Drawing;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;
using System.Web.Services.Protocols;
using System.Xml;
using STL.Common;
using STL.Common.Static;
using STL.Common.Constants.ColumnNames;
using STL.Common.Constants.TableNames;
using STL.Common.Constants.Elements;
using STL.Common.Constants.Enums;
using STL.PL.WS1;
using STL.PL.WS3;
using STL.PL.WS5;
using AxSHDocVw;
using STL.Common.Constants.AccountTypes;

namespace STL.PL
{
	/// <summary>
	/// Popup prompt used when replacing a warranty on a Cash and Go account.
	/// The details of the product being replaced are shown, along with a
	/// description of whether the warranty will remain in place, will be
	/// terminated or is already expired.
	/// </summary>
	public class OneForOneReplacement : CommonForm
	{
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.TextBox txtProductCode;
		private System.Windows.Forms.TextBox txtProductDescription;
		private System.Windows.Forms.TextBox txtModel;
		private System.Windows.Forms.ComboBox drpReturnReason;
		private System.Windows.Forms.DateTimePicker dtReturnDate;
		private System.Windows.Forms.Button btnSave;
		private string AccountNo = "";
		private int BuffNo = 0;
		private new string Error="";
        private Boolean _readonly = true;
		private DateTime dtTransactionDate = DateTime.Today;
		private OneForOneTimePeriod TimePeriod = OneForOneTimePeriod.IRPeriod1;
		private System.Windows.Forms.ErrorProvider errorProvider1;
		private System.Windows.Forms.TextBox txtBranchNo;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.GroupBox gbDetails;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button btnNotes;
		private System.Windows.Forms.GroupBox gbNotes;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.TextBox txtNotes;
		private System.Windows.Forms.TextBox txtReturnItem;
		private int NewBuffNo = 0;
		private InstantReplacementDetails instantReplacement = null;

		private bool valid = true;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.ComboBox drpRetStockLocn;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox txtQty;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.NumericUpDown nmRetQty;
		private int salesPersonNo = 0;
		private string salesPersonName = "";
        private int _warrantyLength = 0;
        private Label label10;
        private TextBox txtTimePeriod;
        private IContainer components;
        private string AcctType = string.Empty;         //#18435
        private int ItemId = 0; //#18482

		public bool ReadOnly 
		{
			get{return _readonly;}
			set{_readonly = value;}
		}

		public OneForOneReplacement(TranslationDummy d)
		{
			InitializeComponent();
		}

		public OneForOneReplacement(string AccountNo, string ProductCode, 
			string Model, string ProductDescription, 
			DateTime dtTransactionDate, string BranchNo,
			int buffNo, decimal price, decimal quantity,
			decimal orderValue, string warrantyNo,
			decimal taxRate, 
			Form root, Form parent,
			string contractno, int empeeNoSale,
            int itemId,int warrantyID, string accttype = "") //#18435 //IP - 29/07/11 - RI - #4429
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			TranslateControls();

			FormRoot = root;
			FormParent = parent;

			instantReplacement = new InstantReplacementDetails(
                ProductCode, warrantyNo,
				buffNo, 0, "", "", 0, 
				price, quantity, orderValue, 
				//OneForOneTimePeriod.IRPeriod1,
				0, taxRate, contractno, Convert.ToInt16(BranchNo),
                itemId, warrantyID, 0);    

			instantReplacement.ElapsedMonths = ElapsedMonths(dtTransactionDate, DateTime.Today);
			instantReplacement.Description = ProductDescription;

			this.AccountNo = AccountNo;
			this.BuffNo = buffNo;
			this.txtProductCode.Text = txtReturnItem.Text = ProductCode;
			this.txtModel.Text = Model;
			this.txtProductDescription.Text = ProductDescription;
			this.dtReturnDate.Value = DateTime.Today;
			this.dtTransactionDate = dtTransactionDate;
			this.txtBranchNo.Text = BranchNo;
			this.txtQty.Text = quantity.ToString();
			this.nmRetQty.Value = this.nmRetQty.Maximum = quantity;
			this.salesPersonNo = empeeNoSale;
            this.AcctType = accttype;               //#18435
            this.ItemId = itemId;                   //#18482

			txtProductCode.BackColor = SystemColors.Window;
			txtBranchNo.BackColor = SystemColors.Window;
			txtModel.BackColor = SystemColors.Window;
			txtProductDescription.BackColor = SystemColors.Window;
			//txtTimePeriod.BackColor = SystemColors.Window;
			txtReturnItem.ForeColor = SystemColors.Highlight;
			txtQty.BackColor = SystemColors.Window;

			loadStatic();

			// Get the warranty length from the warranty band
            DataSet ds = AccountManager.GetProductWarranties(
				itemId,
				Convert.ToInt16(BranchNo), 
				0,
				"",
				false,
				out Error);
            if (Error.Length > 0)
                ShowError(Error);
            else
                foreach (DataTable dt in ds.Tables)
                {
                    if (dt.TableName == TN.Warranties)
                    {
                        if (dt.Rows.Count > 0)
                        {
                            this._warrantyLength = Convert.ToInt32(dt.Rows[0][CN.Duration]);
                        }
                    }
                }
			//CheckWarranty();
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OneForOneReplacement));
            this.gbDetails = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.nmRetQty = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.drpRetStockLocn = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtReturnItem = new System.Windows.Forms.TextBox();
            this.drpReturnReason = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtQty = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtModel = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtProductDescription = new System.Windows.Forms.TextBox();
            this.txtProductCode = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.btnNotes = new System.Windows.Forms.Button();
            this.label11 = new System.Windows.Forms.Label();
            this.txtBranchNo = new System.Windows.Forms.TextBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.dtReturnDate = new System.Windows.Forms.DateTimePicker();
            this.label8 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.txtTimePeriod = new System.Windows.Forms.TextBox();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.gbNotes = new System.Windows.Forms.GroupBox();
            this.button1 = new System.Windows.Forms.Button();
            this.txtNotes = new System.Windows.Forms.TextBox();
            this.gbDetails.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nmRetQty)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.gbNotes.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbDetails
            // 
            this.gbDetails.BackColor = System.Drawing.SystemColors.Control;
            this.gbDetails.Controls.Add(this.groupBox2);
            this.gbDetails.Controls.Add(this.groupBox1);
            this.gbDetails.Controls.Add(this.btnNotes);
            this.gbDetails.Controls.Add(this.label11);
            this.gbDetails.Controls.Add(this.txtBranchNo);
            this.gbDetails.Controls.Add(this.btnSave);
            this.gbDetails.Controls.Add(this.dtReturnDate);
            this.gbDetails.Controls.Add(this.label8);
            this.gbDetails.Controls.Add(this.label10);
            this.gbDetails.Controls.Add(this.txtTimePeriod);
            this.gbDetails.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbDetails.Location = new System.Drawing.Point(0, 0);
            this.gbDetails.Name = "gbDetails";
            this.gbDetails.Size = new System.Drawing.Size(536, 261);
            this.gbDetails.TabIndex = 37;
            this.gbDetails.TabStop = false;
            this.gbDetails.Text = "Faulty Product Notes";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.nmRetQty);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.drpRetStockLocn);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.txtReturnItem);
            this.groupBox2.Controls.Add(this.drpReturnReason);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Location = new System.Drawing.Point(16, 168);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(504, 80);
            this.groupBox2.TabIndex = 43;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Return Details";
            // 
            // nmRetQty
            // 
            this.nmRetQty.Location = new System.Drawing.Point(136, 40);
            this.nmRetQty.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nmRetQty.Name = "nmRetQty";
            this.nmRetQty.Size = new System.Drawing.Size(48, 20);
            this.nmRetQty.TabIndex = 45;
            this.nmRetQty.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(136, 24);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(48, 16);
            this.label5.TabIndex = 44;
            this.label5.Text = "Quantity:";
            // 
            // drpRetStockLocn
            // 
            this.drpRetStockLocn.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpRetStockLocn.ItemHeight = 13;
            this.drpRetStockLocn.Location = new System.Drawing.Point(200, 40);
            this.drpRetStockLocn.Name = "drpRetStockLocn";
            this.drpRetStockLocn.Size = new System.Drawing.Size(112, 21);
            this.drpRetStockLocn.TabIndex = 42;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(200, 24);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(120, 16);
            this.label3.TabIndex = 43;
            this.label3.Text = "Return Stock Location:";
            // 
            // txtReturnItem
            // 
            this.txtReturnItem.Location = new System.Drawing.Point(16, 40);
            this.txtReturnItem.MaxLength = 8;
            this.txtReturnItem.Name = "txtReturnItem";
            this.txtReturnItem.Size = new System.Drawing.Size(104, 20);
            this.txtReturnItem.TabIndex = 0;
            this.txtReturnItem.Validating += new System.ComponentModel.CancelEventHandler(this.txtReturnItem_Validating);
            // 
            // drpReturnReason
            // 
            this.drpReturnReason.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpReturnReason.ItemHeight = 13;
            this.drpReturnReason.Location = new System.Drawing.Point(328, 40);
            this.drpReturnReason.Name = "drpReturnReason";
            this.drpReturnReason.Size = new System.Drawing.Size(160, 21);
            this.drpReturnReason.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(16, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(120, 16);
            this.label1.TabIndex = 41;
            this.label1.Text = "Return Product Code:";
            // 
            // label9
            // 
            this.label9.Location = new System.Drawing.Point(328, 24);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(100, 16);
            this.label9.TabIndex = 4;
            this.label9.Text = "Return Reason:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.txtQty);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.txtModel);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.txtProductDescription);
            this.groupBox1.Controls.Add(this.txtProductCode);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Location = new System.Drawing.Point(16, 80);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(504, 80);
            this.groupBox1.TabIndex = 42;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Product Details";
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(136, 22);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(48, 16);
            this.label4.TabIndex = 8;
            this.label4.Text = "Quantity:";
            // 
            // txtQty
            // 
            this.txtQty.Location = new System.Drawing.Point(136, 40);
            this.txtQty.Name = "txtQty";
            this.txtQty.ReadOnly = true;
            this.txtQty.Size = new System.Drawing.Size(40, 20);
            this.txtQty.TabIndex = 9;
            this.txtQty.TabStop = false;
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(200, 24);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(100, 16);
            this.label7.TabIndex = 2;
            this.label7.Text = "Model:";
            // 
            // txtModel
            // 
            this.txtModel.Location = new System.Drawing.Point(200, 40);
            this.txtModel.Name = "txtModel";
            this.txtModel.ReadOnly = true;
            this.txtModel.Size = new System.Drawing.Size(112, 20);
            this.txtModel.TabIndex = 7;
            this.txtModel.TabStop = false;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(16, 24);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(100, 16);
            this.label2.TabIndex = 0;
            this.label2.Text = "Product Code:";
            // 
            // txtProductDescription
            // 
            this.txtProductDescription.Location = new System.Drawing.Point(328, 40);
            this.txtProductDescription.Name = "txtProductDescription";
            this.txtProductDescription.ReadOnly = true;
            this.txtProductDescription.Size = new System.Drawing.Size(160, 20);
            this.txtProductDescription.TabIndex = 6;
            this.txtProductDescription.TabStop = false;
            // 
            // txtProductCode
            // 
            this.txtProductCode.Location = new System.Drawing.Point(16, 40);
            this.txtProductCode.Name = "txtProductCode";
            this.txtProductCode.ReadOnly = true;
            this.txtProductCode.Size = new System.Drawing.Size(104, 20);
            this.txtProductCode.TabIndex = 5;
            this.txtProductCode.TabStop = false;
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(328, 24);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(118, 16);
            this.label6.TabIndex = 1;
            this.label6.Text = "Product Description:";
            // 
            // btnNotes
            // 
            this.btnNotes.Image = ((System.Drawing.Image)(resources.GetObject("btnNotes.Image")));
            this.btnNotes.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnNotes.Location = new System.Drawing.Point(456, 16);
            this.btnNotes.Name = "btnNotes";
            this.btnNotes.Size = new System.Drawing.Size(24, 24);
            this.btnNotes.TabIndex = 2;
            this.btnNotes.Click += new System.EventHandler(this.btnNotes_Click);
            // 
            // label11
            // 
            this.label11.Location = new System.Drawing.Point(16, 32);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(64, 16);
            this.label11.TabIndex = 40;
            this.label11.Text = "Branch No:";
            // 
            // txtBranchNo
            // 
            this.txtBranchNo.Location = new System.Drawing.Point(16, 48);
            this.txtBranchNo.Name = "txtBranchNo";
            this.txtBranchNo.ReadOnly = true;
            this.txtBranchNo.Size = new System.Drawing.Size(56, 20);
            this.txtBranchNo.TabIndex = 39;
            this.txtBranchNo.TabStop = false;
            // 
            // btnSave
            // 
            this.btnSave.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnSave.BackgroundImage")));
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnSave.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnSave.Location = new System.Drawing.Point(488, 16);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(24, 24);
            this.btnSave.TabIndex = 3;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // dtReturnDate
            // 
            this.dtReturnDate.CustomFormat = "ddd dd MMM yyyy";
            this.dtReturnDate.Enabled = false;
            this.dtReturnDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtReturnDate.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.dtReturnDate.Location = new System.Drawing.Point(96, 48);
            this.dtReturnDate.Name = "dtReturnDate";
            this.dtReturnDate.Size = new System.Drawing.Size(112, 20);
            this.dtReturnDate.TabIndex = 31;
            this.dtReturnDate.TabStop = false;
            this.dtReturnDate.Value = new System.DateTime(1900, 1, 1, 0, 0, 0, 0);
            // 
            // label8
            // 
            this.label8.Location = new System.Drawing.Point(96, 32);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(100, 16);
            this.label8.TabIndex = 3;
            this.label8.Text = "Return Date:";
            // 
            // label10
            // 
            this.label10.Location = new System.Drawing.Point(232, 32);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(144, 16);
            this.label10.TabIndex = 37;
            this.label10.Text = "One for One Time Period:";
            this.label10.Visible = false;
            // 
            // txtTimePeriod
            // 
            this.txtTimePeriod.Location = new System.Drawing.Point(232, 48);
            this.txtTimePeriod.Name = "txtTimePeriod";
            this.txtTimePeriod.ReadOnly = true;
            this.txtTimePeriod.Size = new System.Drawing.Size(288, 20);
            this.txtTimePeriod.TabIndex = 38;
            this.txtTimePeriod.TabStop = false;
            this.txtTimePeriod.Visible = false;
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            this.errorProvider1.DataMember = "";
            this.errorProvider1.Icon = ((System.Drawing.Icon)(resources.GetObject("errorProvider1.Icon")));
            // 
            // gbNotes
            // 
            this.gbNotes.BackColor = System.Drawing.SystemColors.ControlLight;
            this.gbNotes.Controls.Add(this.button1);
            this.gbNotes.Controls.Add(this.txtNotes);
            this.gbNotes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbNotes.Location = new System.Drawing.Point(0, 0);
            this.gbNotes.Name = "gbNotes";
            this.gbNotes.Size = new System.Drawing.Size(536, 261);
            this.gbNotes.TabIndex = 38;
            this.gbNotes.TabStop = false;
            this.gbNotes.Text = "Service Notes";
            // 
            // button1
            // 
            this.button1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button1.BackgroundImage")));
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.button1.Location = new System.Drawing.Point(488, 16);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(24, 24);
            this.button1.TabIndex = 37;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // txtNotes
            // 
            this.txtNotes.Location = new System.Drawing.Point(16, 32);
            this.txtNotes.Multiline = true;
            this.txtNotes.Name = "txtNotes";
            this.txtNotes.Size = new System.Drawing.Size(456, 200);
            this.txtNotes.TabIndex = 38;
            // 
            // OneForOneReplacement
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.SystemColors.ControlDark;
            this.ClientSize = new System.Drawing.Size(536, 261);
            this.ControlBox = false;
            this.Controls.Add(this.gbDetails);
            this.Controls.Add(this.gbNotes);
            this.Name = "OneForOneReplacement";
            this.Text = "Instant Replacement";
            this.gbDetails.ResumeLayout(false);
            this.gbDetails.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nmRetQty)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.gbNotes.ResumeLayout(false);
            this.gbNotes.PerformLayout();
            this.ResumeLayout(false);

		}
		#endregion

		private void LoadStaticThread()
		{
			try
			{
				Wait();
				Function = "LoadStaticThread";
			
				XmlUtilities xml = new XmlUtilities();
				XmlDocument dropDowns = new XmlDocument();
				dropDowns.LoadXml("<DROP_DOWNS></DROP_DOWNS>");

				if(StaticData.Tables["code"]==null)
					dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.ReturnReasons, new string[] {"FPN", "L"}));
				if(StaticData.Tables[TN.BranchNumber]==null)
					dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns,TN.BranchNumber, null));
				
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
			
				drpRetStockLocn.DataSource = (DataTable)StaticData.Tables[TN.BranchNumber];
				drpRetStockLocn.DisplayMember = CN.BranchNo;
			}
			catch(Exception ex)
			{
				Catch(ex, Function);
			}
			finally
			{
				StopWait();
				Function = "End of LoadStaticThread";
			}
		}

		private void loadStatic()
		{
			Function = "LoadStatic";

			Thread dataThread = new Thread(new ThreadStart(LoadStaticThread));
			dataThread.Start();
			dataThread.Join();			

			drpReturnReason.DataSource = ((DataTable)StaticData.Tables[TN.ReturnReasons]).DefaultView;
			drpReturnReason.DisplayMember = CN.CodeDescript;

			short sl = StaticDataManager.GetBranchServiceLocation(Convert.ToInt16(txtBranchNo.Text), out Error);
			
			foreach(DataRow r in ((DataTable)drpRetStockLocn.DataSource).Rows)
			{
				if((short)r[CN.BranchNo] == sl)
					drpRetStockLocn.SelectedItem = r;
			}
			
			drpRetStockLocn.SelectedText = sl.ToString();

			salesPersonName = Login.GetEmployeeName(salesPersonNo, out Error);
						
			Function = "End of LoadStatic";
		}		

        //private void CheckWarranty() 
        //{
         

            ////DateTime endPeriod1 = this.dtTransactionDate.AddDays(Convert.ToDouble(Country[CountryParameterNames.IRPeriod1]));
            ////DateTime endPeriod2 = endPeriod1.AddMonths(Convert.ToInt32(Country[CountryParameterNames.IRPeriod2]));
            ////DateTime endPeriod3 = endPeriod2.AddMonths(Convert.ToInt32(Country[CountryParameterNames.IRPeriod3]));
            ////DateTime endPeriod4 = this.dtTransactionDate.AddYears(this._warrantyLength);
	
			// The warranty could expire before the other periods so check this first
            //if (DateTime.Today >= endPeriod4)
            //{
            //    // The warranty has expired
            //    this.TimePeriod = OneForOneTimePeriod.WarrantyExpired;
            //    this.txtTimePeriod.Text = GetResource("M_IRPERIOD5");
            //    this.gbDetails.Enabled = false;
            //}
            //else if (DateTime.Today < endPeriod1)
            //{
            //    // In period 1
            //    this.TimePeriod = OneForOneTimePeriod.IRPeriod1;
            //    this.txtTimePeriod.Text = GetResource("M_IRPERIOD1");
            //}
            //else if (DateTime.Today < endPeriod2)
            //{
            //    // In period 2
            //    this.TimePeriod = OneForOneTimePeriod.IRPeriod2;
            //    this.txtTimePeriod.Text = GetResource("M_IRPERIOD2");
            //}
            //else if (DateTime.Today < endPeriod3)
            //{
            //    // In period 3
            //    this.TimePeriod = OneForOneTimePeriod.IRPeriod3;
            //    this.txtTimePeriod.Text = GetResource("M_IRPERIOD3");
            //}
            //else
            //{
            //    // In period 4
            //    this.TimePeriod = OneForOneTimePeriod.IRPeriod4;
            //    this.txtTimePeriod.Text = GetResource("M_IRPERIOD4");
            //}
            //instantReplacement.TimePeriod = this.TimePeriod;
		//}

		private void PrintNotes(int months)
		{
			((MainForm)FormRoot).browsers = ((CommonForm)FormRoot).CreateBrowserArray(1);

			object Zero = 0;
			object EmptyString = "";
			string url = Config.Url + "WOneForOneReplacementNote.aspx?"
				+ CN.acctno + "=" + (string)this.AccountNo + "&"
				+ CN.BranchNo + "=" + (string)this.txtBranchNo.Text + "&"
				+ CN.ItemNo + "=" + (string)this.txtProductCode.Text + "&"
				+ CN.ItemDescr1 + "=" + (string)this.txtProductDescription.Text + "&"
				+ CN.Reason + "=" + (string)this.drpReturnReason.Text + "&"
				+ Elements.DateReturn + "=" + (string)this.dtReturnDate.Value.ToString() + "&"
				//+ Elements.OneForOneTimePeriod + "=" + (string)this.txtTimePeriod.Text + "&"
				+ "user=" + salesPersonNo.ToString() + "&"
				+ "userName=" + salesPersonName + "&"
				+ "culture=" + Config.Culture+ "&"
				+ CN.Notes + "=" + txtNotes.Text+"&"
				+ "countryCode=" + Config.CountryCode;

			((MainForm)FormRoot).browsers[0].Navigate(url, ref Zero, ref EmptyString, ref EmptyString, ref EmptyString);

		}

		private int ElapsedMonths(DateTime from, DateTime to)
		{
			int y = to.Year - from.Year;
			int m = to.Month - from.Month;
			if(m<0)
			{
				y--;
				m += 12;
			}
			return y*12+m;
		}

		private bool Save()
		{
			bool valid = true;

			/* save the details to the ProductFaults table */
			int y = DateTime.Today.Year - dtTransactionDate.Year;
			int m = DateTime.Today.Month - dtTransactionDate.Month;
			if(m<0)
			{
				y--;
				m += 12;
			}
			int months = y*12+m;

			NewBuffNo = AccountManager.SaveProductFaults(AccountNo, BuffNo, 
								txtProductCode.Text, txtReturnItem.Text,
								txtNotes.Text, 
								(string)((DataView)drpReturnReason.DataSource)[drpReturnReason.SelectedIndex][CN.Code],
								dtReturnDate.Value, Convert.ToInt16(months), Convert.ToInt16(txtBranchNo.Text), out Error);
			if(Error.Length>0)
			{
				valid = false;
				ShowError(Error);
			}
			else
			{
				instantReplacement.NewBuffNo = NewBuffNo;
				instantReplacement.ReturnItemNo = txtReturnItem.Text;
				instantReplacement.ReturnStockLocn = Convert.ToInt16(drpRetStockLocn.Text);
				instantReplacement.ReturnReason = (string)((DataView)drpReturnReason.DataSource)[drpReturnReason.SelectedIndex][CN.Code];
				instantReplacement.Quantity = nmRetQty.Value;
				//instantReplacement.OrderValue = instantReplacement.Quantity * instantReplacement.Price;
				
				if((string)Country[CountryParameterNames.AgreementTaxType]=="E")
				{
					instantReplacement.TaxAmount = AccountManager.CalculateTaxAmount(Config.CountryCode,
													instantReplacement.Serialise(),
													false, 0, out Error);
				}
				
				PrintNotes(months);
			}

			return valid;
		}

		private void btnSave_Click(object sender, System.EventArgs e)
		{	
			Function = "btnSave_Click";

			try
			{
				Wait();
				if(valid)
				{
					if(!Save())
						ShowInfo("M_INVALIDFIELDS");
					else
					{
						/* launch the NewAccount screen in replacement mode */

						/* make sure the original branch is used */
						string acctno = AccountManager.GetPaidAndTakenAccount(AccountNo.Substring(0,3), out Error);
						
						//string acctno = AccountManager.GetPaidAndTakenAccount(Config.BranchCode, out Error);
						if(Error.Length>0)
							ShowError(Error);
						else
						{
							if(acctno.Length!=0)
							{
                                XmlNode replacementXml = null;
                                if (instantReplacement != null)
                                    replacementXml = instantReplacement.Serialise();

                                if (replacementXml != null && this.AccountNo != acctno && this.AcctType != AT.Special) //#18435 //#17290 - Only process collection here if Instant Replacement not on Cash & Go
                                {
                                    AccountManager.InstantReplacementCollection(replacementXml, this.AccountNo, Convert.ToInt16(txtBranchNo.Text), Config.CountryCode, out Error);
                                }


								NewAccount revise = new NewAccount(this.AccountNo, this.BuffNo, instantReplacement, false, FormRoot, FormRoot);
								revise.Replacement = true;
								
								/* this check is designed to catch WOC special accts but
								 * it will also be true if the branch has been changed
								 * i.e. one branches P&T acctno is not the same as anothers */
								if(this.AccountNo != acctno)
								{
									revise.PTWarrantyAccountNo = this.AccountNo;
								}
								revise.Text = GetResource("P_WARRANTY_REPLACE");
								if(revise.AccountLoaded)
								{
									((MainForm)FormRoot).AddTabPage(revise,24);
									revise.SupressEvents = false;

                                    //IP - 18/03/08 - (69630)
                                    //As this is a Cash & Go Instant Replacement, the 'Link to Customer Account' button,
                                    //'Customer Search' menu option 
                                    //do NOT need to be enabled. Previously, by clicking on these
                                    //multiple times would cause multiple GRT transactions to be posted to 
                                    //'fintrans'.
                                    revise.btnCustomerSearch.Enabled = false;
                                    revise.menuCustomerSearch.Enabled = false;
                                    revise.menuPrintReceipt.Enabled = true;
								}
							}
							else
								ShowInfo("M_NOPAIDANDTAKEN");
						}
						Close();
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
				Function = "End of btnSaveClick";
			}
		}

		private void button1_Click(object sender, System.EventArgs e)
		{
			gbNotes.SendToBack();
		}

		private void btnNotes_Click(object sender, System.EventArgs e)
		{
			gbDetails.SendToBack();
		}

		private void txtReturnItem_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			/* make sure that they have entered a valid stock item code */
			try
			{
				Wait();

				//int count = AccountManager.GetItemCount(txtReturnItem.Text, Convert.ToInt16(txtBranchNo.Text), out Error);
                int count = AccountManager.GetItemCount(this.ItemId, Convert.ToInt16(txtBranchNo.Text), out Error);      //#18482
				if(Error.Length > 0)
					ShowError(Error);
				else
				{
					if(count == 0)
					{
						errorProvider1.SetError(txtReturnItem, GetResource("M_RETSTOCKNOTPRESENT"));
						txtReturnItem.Focus();
						valid = false;
					}
					else
					{
						errorProvider1.SetError(txtReturnItem, "");
						valid = true;
					}
				}
			}
			catch(Exception ex)
			{
				Catch(ex, "txtReturnItem_Validating");
			}
		}
	}
}
