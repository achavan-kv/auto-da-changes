using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using STL.Common;
using System.Data;
using System.Web.Services.Protocols;
using STL.PL.WS1;
using STL.PL.WS2;
using STL.PL.WS3;
using STL.PL.WS4;
using STL.PL.WS5;
using STL.Common.Constants.AccountTypes;
using STL.Common.Constants.ColumnNames;
using System.Collections.Specialized;
using System.Xml;
using STL.Common.Constants.Elements;
using STL.Common.Constants.Tags;
using Crownwood.Magic.Menus;
using STL.Common.Constants.ItemTypes;
using STL.Common.Static;
using STL.Common.Services.Model;
using System.Collections.Generic;

namespace STL.PL
{
	/// <summary>
	/// Popup prompt to list warranties associated with a stock item.
	/// This popup will automatically list the associated warranties
	/// as the user enters items onto an order. The required warranty
	/// can be picked and added to the order.
	/// </summary>
	public class RelatedProductWarranties : CommonForm
    {
		private System.Windows.Forms.ErrorProvider errorProvider1;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnBuy;
		private System.Windows.Forms.Label message;
		private DataTable _contracts = null;
		private new string Error;
		private double _qty = 0;
		public double Quantity 
		{
			get{return _qty;}
			set{_qty = value;}
		}
		//int oldCurrentRow;
		//int oldCurrentCol;
		//bool valid;
		short _location;
		string AccountNo;
		int AgreementNo = 1;
		string AccountType = "";
		XmlNode node = null;
		new XmlUtilities xml;
		private System.Windows.Forms.DataGrid dgContracts;
		private System.Windows.Forms.Button btnRemove;
		private System.Windows.Forms.Button btnEnter;
		private Control allowCashAndGoCredit;
		private Control allowNewAccountCredit;
		private bool _manualCDV = false;
        private bool ManualSale = false;
        private DataGridView dgWarranties;
        private IContainer components;

		public RelatedProductWarranties(TranslationDummy d)
		{
			InitializeComponent();
		}

        //private void HashMenus()
        //{
        //    dynamicMenus = new Hashtable();
        //    dynamicMenus[this.Name+":allowCashAndGoCredit"] = this.allowCashAndGoCredit; 
        //    dynamicMenus[this.Name+":allowNewAccountCredit"] = this.allowNewAccountCredit; 
        //}

        public RelatedProductWarranties(IList<WarrantyItemXml> warranties, double quantity, XmlNode currentItem, short location, string accountNo, int agreementNo, bool manualSale, System.Windows.Forms.Form par, Form root, string acctType)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			TranslateControls();

			this.FormParent = par;
			this.FormRoot = root;
			this.Quantity = quantity;
			this._location = location;
			this.node = currentItem;
			this.AccountNo = accountNo;
			this.AgreementNo = agreementNo;
			this.AccountType = acctType;
			this.ManualSale = manualSale;
			

			this._manualCDV = AccountManager.ManualCDVExists(this.AccountNo, out Error);
			if(Error.Length > 0) ShowError(Error);

			xml = new XmlUtilities();

            dgWarranties.DataSource = warranties;

            DataGridTableStyle tabStyle = new DataGridTableStyle();
			
            dgWarranties.DataSource = warranties;
            dgWarranties.Columns["Id"].Visible = false;
            dgWarranties.Columns["BranchForDeliveryNote"].Visible = false;
            dgWarranties.Columns["DeliveryTime"].Visible = false;
            dgWarranties.Columns["DeliveryDate"].Visible = false;
            dgWarranties.Columns["Quantity"].Visible = false;
            dgWarranties.Columns["Description"].Width = 150;
            dgWarranties.Columns["Description"].HeaderText = GetResource("T_DESCRIPTION");
            dgWarranties.Columns["Length"].Width = 50;
            dgWarranties.Columns["Length"].HeaderText = GetResource("T_DURATION");
            dgWarranties.Columns["Location"].Visible = false;
            dgWarranties.Columns["Value"].Visible = false;

            //#19628
            if (dgWarranties.Columns.Contains("IsFree"))
            {
                dgWarranties.Columns["IsFree"].Visible = false;
            }

            dgWarranties.Columns["Code"].HeaderText = "Warranty Code";
            dgWarranties.Columns["ContractNumber"].Visible = false;

			_contracts = new DataTable();
			_contracts.Columns.AddRange(new DataColumn[]{new DataColumn(CN.ContractNo), new DataColumn(CN.ReadOnly)});
			dgContracts.DataSource = _contracts.DefaultView;
			_contracts.DefaultView.AllowDelete = false;
			_contracts.DefaultView.AllowNew = false;
			_contracts.DefaultView.AllowEdit = !((bool)Country[CountryParameterNames.AutomaticWarrantyNo]) || ManualSale || this._manualCDV || (AccountType == AT.ReadyFinance);
            
			tabStyle = new DataGridTableStyle();
			tabStyle.MappingName = _contracts.TableName;

			AddColumnStyle(CN.ReadOnly,tabStyle, 0,true, "", "", HorizontalAlignment.Left);
			
			DataGridEditColumn aColumnEditColumn = new DataGridEditColumn(CN.ReadOnly, "Y");
			aColumnEditColumn.MappingName = CN.ContractNo;
			aColumnEditColumn.HeaderText = GetResource("T_CONTRACTNO");
			aColumnEditColumn.Width = 136;
			aColumnEditColumn.ReadOnly = false;
			aColumnEditColumn.NullText = "";
			tabStyle.GridColumnStyles.Add(aColumnEditColumn); 

			dgContracts.TableStyles.Add(tabStyle);
			
			GetExistingContracts(0);			

			message.Text = GetResource("M_ADDWARRANTY", new Object[] { quantity });
		}

		private void GetExistingContracts(int index)
		{
			string warrItem = ((IList<WarrantyItemXml>)dgWarranties.DataSource)[index].Code;        // #15642
			string xpath = Elements.RelatedItem+"/"+Elements.Item+"[@"+ 
							Tags.Code+"='"+warrItem+"' and @Quantity != '0']";
			XmlNodeList contracts = node.SelectNodes(xpath);
			if(contracts!=null)
			{
				foreach (XmlNode child in contracts)
				{
					DataRow r = _contracts.NewRow();
					r[CN.ContractNo] = child.Attributes[Tags.ContractNumber].Value;
					
					if(AccountType != AT.ReadyFinance)
						r[CN.ReadOnly] = "Y";

					_contracts.Rows.Add(r);
				}
			}
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
            this.allowCashAndGoCredit = new System.Windows.Forms.Control();
            this.allowNewAccountCredit = new System.Windows.Forms.Control();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnRemove = new System.Windows.Forms.Button();
            this.btnEnter = new System.Windows.Forms.Button();
            this.dgContracts = new System.Windows.Forms.DataGrid();
            this.message = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnBuy = new System.Windows.Forms.Button();
            this.dgWarranties = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgContracts)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgWarranties)).BeginInit();
            this.SuspendLayout();
            // 
            // allowCashAndGoCredit
            // 
            this.allowCashAndGoCredit.Enabled = false;
            this.allowCashAndGoCredit.Location = new System.Drawing.Point(0, 0);
            this.allowCashAndGoCredit.Name = "allowCashAndGoCredit";
            this.allowCashAndGoCredit.Size = new System.Drawing.Size(0, 0);
            this.allowCashAndGoCredit.TabIndex = 0;
            // 
            // allowNewAccountCredit
            // 
            this.allowNewAccountCredit.Enabled = false;
            this.allowNewAccountCredit.Location = new System.Drawing.Point(0, 0);
            this.allowNewAccountCredit.Name = "allowNewAccountCredit";
            this.allowNewAccountCredit.Size = new System.Drawing.Size(0, 0);
            this.allowNewAccountCredit.TabIndex = 0;
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnRemove);
            this.groupBox1.Controls.Add(this.btnEnter);
            this.groupBox1.Controls.Add(this.dgContracts);
            this.groupBox1.Controls.Add(this.message);
            this.groupBox1.Controls.Add(this.btnCancel);
            this.groupBox1.Controls.Add(this.btnBuy);
            this.groupBox1.Location = new System.Drawing.Point(8, 152);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(320, 160);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            // 
            // btnRemove
            // 
            this.btnRemove.BackColor = System.Drawing.Color.SlateBlue;
            this.btnRemove.Font = new System.Drawing.Font("Arial Narrow", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRemove.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.btnRemove.Image = global::STL.PL.Properties.Resources.Minus;
            this.btnRemove.Location = new System.Drawing.Point(216, 80);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(20, 20);
            this.btnRemove.TabIndex = 23;
            this.btnRemove.UseVisualStyleBackColor = false;
            this.btnRemove.Click += new System.EventHandler(this.DeleteContract_Click);
            // 
            // btnEnter
            // 
            this.btnEnter.BackColor = System.Drawing.Color.SlateBlue;
            this.btnEnter.Font = new System.Drawing.Font("Arial Narrow", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnEnter.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.btnEnter.Image = global::STL.PL.Properties.Resources.plus;
            this.btnEnter.Location = new System.Drawing.Point(216, 56);
            this.btnEnter.Name = "btnEnter";
            this.btnEnter.Size = new System.Drawing.Size(20, 20);
            this.btnEnter.TabIndex = 22;
            this.btnEnter.UseVisualStyleBackColor = false;
            this.btnEnter.Click += new System.EventHandler(this.btnEnter_Click);
            // 
            // dgContracts
            // 
            this.dgContracts.CaptionVisible = false;
            this.dgContracts.DataMember = "";
            this.dgContracts.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgContracts.Location = new System.Drawing.Point(16, 56);
            this.dgContracts.Name = "dgContracts";
            this.dgContracts.Size = new System.Drawing.Size(184, 96);
            this.dgContracts.TabIndex = 11;
            this.dgContracts.MouseUp += new System.Windows.Forms.MouseEventHandler(this.dgContracts_MouseUp);
            // 
            // message
            // 
            this.message.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.message.Location = new System.Drawing.Point(16, 8);
            this.message.Name = "message";
            this.message.Size = new System.Drawing.Size(192, 40);
            this.message.TabIndex = 9;
            this.message.Text = "Enter a contract number for each item purchased.";
            // 
            // btnCancel
            // 
            this.btnCancel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnCancel.Location = new System.Drawing.Point(248, 88);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(56, 23);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnBuy
            // 
            this.btnBuy.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnBuy.Location = new System.Drawing.Point(248, 56);
            this.btnBuy.Name = "btnBuy";
            this.btnBuy.Size = new System.Drawing.Size(56, 23);
            this.btnBuy.TabIndex = 6;
            this.btnBuy.Text = "Buy";
            this.btnBuy.Click += new System.EventHandler(this.btnBuy_Click);
            // 
            // dgWarranties
            // 
            this.dgWarranties.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgWarranties.Location = new System.Drawing.Point(24, 31);
            this.dgWarranties.Name = "dgWarranties";
            this.dgWarranties.Size = new System.Drawing.Size(288, 115);
            this.dgWarranties.TabIndex = 7;
            this.dgWarranties.Click += new System.EventHandler(this.dgWarranties_Click);
            // 
            // RelatedProductWarranties
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(338, 314);
            this.ControlBox = false;
            this.Controls.Add(this.dgWarranties);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "RelatedProductWarranties";
            this.ShowInTaskbar = false;
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgContracts)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgWarranties)).EndInit();
            this.ResumeLayout(false);

		}
		#endregion

		private void btnCancel_Click(object sender, System.EventArgs e)
		{
			Close();
		}

		private void btnBuy_Click(object sender, System.EventArgs e)
		{
			bool valid = true;
			int count = 0;

			//Validate all contract no's so they can't save if there are errors
			foreach(DataRowView row in (DataView)dgContracts.DataSource)
			{
				valid = ValidateContracts(row["contractno"]);
				count++;
			}

			if(valid && count>0)
			{
				/* Don't need to check schedules for warranties, they will 
				 * be automatically collected if necessary */
				errorProvider1.SetError(dgContracts, "");
                ((RelatedProducts)this.FormParent).Warranty = Convert.ToString(dgWarranties.CurrentRow.Cells["Code"].Value);
				((RelatedProducts)this.FormParent).ContractNos = _contracts;
                ((RelatedProducts)this.FormParent).ItemId = Convert.ToInt32(dgWarranties.CurrentRow.Cells["Id"].Value); 
				Close();
			}
		}

		private bool ValidateContracts(object txt)
			// Amended check on contract number to allow 10 digit no
		{
			string msg = "";
			bool valid = true;
			string text = "";

			//make sure it's not DBNull
			if(txt == DBNull.Value)
			{
				valid = false;
				msg = "You must enter a contract number";
			}
			else
			{
				text = (string)txt;
			}

			//make sure it's not too long
			if(valid)
			{
				if(text.Length>10)
				{
					valid = false;
					msg = "Contract number must be less than 11 characters.";
				}
			}

			//Make sure there aren't too many
			if(valid)
			{
				if(_contracts.DefaultView.Count>Quantity)
				{
					valid = false;
					msg = "You have entered too many contract numbers.";
				}
			}

			if(valid)
			{
				/* if the contract no has not been automatically generated
				 * then we need to make sure it's unique */
				if(!(bool)Country[CountryParameterNames.AutomaticWarrantyNo])
				{
					/* 1) check the items XmlDocument and see if there
					 * are any contract nodes with this contract number 
					 * 2) if found see if it's for the same item and stocklocn 
					 * if it is then that's OK, otherwise it's not unique */

					//string xpath = "//"+Elements.ContractNo+"[@"+Tags.ContractNumber+" = '"+text+"']";

					string xpath = "//Item[@ContractNumber='"+text+"']";
					XmlNode duplicate = ((RelatedProducts)FormParent).originalDoc.DocumentElement.SelectSingleNode(xpath);; //((NewAccount)FormParent).LineItems.SelectSingleNode(xpath);

					if(duplicate != null)
					{
                        string waritemno = dgWarranties["waritemno", dgWarranties.CurrentRow.Index].ToString(); 
						if((Convert.ToInt16(duplicate.Attributes[Tags.Location].Value) != this._location ||
							duplicate.Attributes[Tags.Code].Value != waritemno) &&
							duplicate.Attributes[Tags.Quantity].Value != "0")
						{
							/* this is a duplicate and not allowed */
							valid = false;
							msg = "Contract number "+text+" used elsewhere on this account";
						}
					}

					if(valid)
					{
						/* check the database and make sure this contract no
						 * hasn't been used on another account */
						bool unique = false;
						AccountManager.ContractNoUnique(this.AccountNo, AgreementNo, text, out unique, out Error);
						if(Error.Length>0)
							ShowError(Error);
						else
						{
							if(!unique)
							{
								valid = false;
								msg = "Contract number "+text+" is used on another account";
							}
						}
					}
				}
			}

			if(!valid)
			{
				dgContracts.CurrentCell = new DataGridCell(0, 0);
				errorProvider1.SetError(dgContracts, msg);
			}
			else
			{
				errorProvider1.SetError(dgContracts, "");
			}
			return valid;
		}

		private void dgWarranties_Click(object sender, System.EventArgs e)
		{
			
			if(dgWarranties.CurrentRow.Index >= 0)
			{
				_contracts.Clear();
				this.GetExistingContracts(dgWarranties.CurrentRow.Index);		
			}
		}

		private void dgContracts_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			int index = dgContracts.CurrentRowIndex;
			if(index>=0)	//may be empty
			{
				if (e.Button == MouseButtons.Right)
				{
					DataGrid ctl = (DataGrid)sender;

					MenuCommand m1 = new MenuCommand(GetResource("P_DELETE"));
					m1.Click += new System.EventHandler(this.DeleteContract_Click);

					PopupMenu popup = new PopupMenu();
					popup.Animate = Animate.Yes;
					popup.AnimateStyle = Animation.SlideHorVerPositive;
					popup.MenuCommands.Add(m1);
					MenuCommand selected = popup.TrackPopup(ctl.PointToScreen(new Point(e.X, e.Y)));
				}
			}
		}

		private void DeleteContract_Click(object sender, System.EventArgs e)
		{
			try
			{
				int index = dgContracts.CurrentRowIndex;
				int index2 = dgWarranties.CurrentRow.Index;

				if(index>=0)
				{
					if((string)((DataView)dgContracts.DataSource)[index][CN.ReadOnly] == "Y")
					{
						/* must set the associated item qty to zero otherwise the warranty will
						 * not be collected (if necessary) */
						string warrItem = (string)((DataView)dgWarranties.DataSource)[index2]["waritemno"];
						string contractNo = (string)((DataView)dgContracts.DataSource)[index][CN.ContractNo];
						//string xpath = "RelatedItems/Item[@Code='"+warrItem+"' and @Quantity!='0' and @ContractNumber='"+contractNo+"']";
						string xpath = "//Item[@Code='"+warrItem+"' and @Quantity!='0' and @ContractNumber='"+contractNo+"']";

						//XmlNodeList contracts = node.SelectNodes(xpath);	/* there should be at most one */
						XmlNodeList contracts = ((RelatedProducts)FormParent).originalDoc.DocumentElement.SelectNodes(xpath);
						if(contracts!=null)
						{
							foreach (XmlNode child in contracts)
								child.Attributes[Tags.Quantity].Value = "0";
						}						 
					}
					/* remove the actuall datagrid entry */
					_contracts.DefaultView.AllowDelete = true;
					_contracts.DefaultView[index].Delete();
					_contracts.DefaultView.AllowDelete = false;  
				}
			}
			catch(Exception ex)
			{
				Catch(ex, Function);
			}
		}

		/// <summary>
		/// Methos to populate automatic contract no baised on country paramater
		/// RD / JL - 10 December 2002
		/// </summary>
		private void btnEnter_Click(object sender, System.EventArgs e)
		{
			try
			{
				Function = "btnEnter_Click";
				Wait();

				if(_contracts.Rows.Count < Quantity)
				{
					DataRow row = _contracts.NewRow();
					row[CN.ReadOnly] = "N";
					if ((bool)Country[CountryParameterNames.AutomaticWarrantyNo])
					{
						string contractNo = AccountManager.AutoWarranty(Config.BranchCode, out Error);
						if(Error.Length>0)
							ShowError(Error);
						else
						{
							row[CN.ContractNo] = contractNo;	
							
							if(AccountType != AT.ReadyFinance && !ManualSale && !this._manualCDV)
								row[CN.ReadOnly] = "Y";
						}
					}
					_contracts.Rows.Add(row);
				}
			}
			catch(Exception ex)
			{
				Catch(ex, Function);
			}
			finally
			{
				StopWait();
				Function = "End of btnEnter_Click";
			}
		}
	}
}
