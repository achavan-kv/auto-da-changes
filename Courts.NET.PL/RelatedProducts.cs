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
using STL.Common.Constants.TableNames;
using System.Collections.Specialized;
using System.Xml;
using STL.Common.Constants.Elements;
using STL.Common.Constants.Tags;
using Crownwood.Magic.Menus;
using STL.Common.Constants.ItemTypes;
using STL.Common.Static;
using STL.Common.Services;
using STL.Common.Services.Model;
using System.Collections.Generic;
using Blue.Cosacs.Shared;

namespace STL.PL
{
	/// <summary>
	/// Popup prompt to list stock items associated with another stock item.
	/// Certain stock items are defined to be associated with other stock item
	/// codes that start with a certain prefix. This popup will automatically
	/// list the associated stock items as the user enters items onto an order.
	/// The user can select one or more of the associated items to add to the order.
	/// </summary>
	public class RelatedProducts : CommonForm
	{
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Button btnEnter;
		private System.Windows.Forms.DataGrid dgSelectedItems;
        private System.Windows.Forms.Button btnCancel;
		private DataTable _selectedItems = null;
		private string AccountType = "";
        
		private short _parentlocn = 0;
		private string _parentkey = "";
		private bool _dutyfree = false;
		private bool _taxexempt = false;
		private string _deliverydate = "";
		private string _deliverytime = "";
		private string _delnotebranch = "";
		private DataTable _contractnos = null;
		private string _warranty = "";
		private string _accountno = "";
		private int _agreementno = 0;
		private bool _allowsupashield = false;
		private decimal price = 0;
        private string spiffFilter = "";

		private new string Error = "";

		public short ParentLocn 
		{
			get{return _parentlocn;}
			set{_parentlocn = value;}
		}
		
		public string ParentKey 
		{
			get{return _parentkey;}
			set{_parentkey = value;}
		}
		
		public bool DutyFree 
		{
			get{return _dutyfree;}
			set{_dutyfree = value;}
		}
		
		public bool TaxExempt 
		{
			get{return _taxexempt;}
			set{_taxexempt = value;}
		}

		public string DeliveryDate 
		{
			get{return _deliverydate;}
			set{_deliverydate = value;}
		}
		
		public string DeliveryTime 
		{
			get{return _deliverytime;}
			set{_deliverytime = value;}
		}
		
		public string DelNoteBranch 
		{
			get{return _delnotebranch;}
			set{_delnotebranch = value;}
		}

		public DataTable ContractNos
		{
			get{return _contractnos;}
			set{_contractnos = value;}
		}

		public string Warranty
		{
			set{_warranty = value;}
			get{return _warranty;}
		}

		public string AccountNo
		{
			set{_accountno = value;}
			get{return _accountno;}
		}

		public int AgreementNo
		{
			set{_agreementno = value;}
			get{return _agreementno;}
		}

		public bool AllowSupaShield
		{
			get{return _allowsupashield;}
			set{_allowsupashield = value;}
		}

        private string _itemno = "";
        public string ItemNo
        {
            get { return _itemno; }
            set { _itemno = value; }
        }

        public int ItemId { get; set; }

		private XmlNode related;

		public XmlDocument originalDoc = new XmlDocument();

        public bool RelatedProductsChosen { get; set; }
		private double _qty = 0;
		private System.Windows.Forms.NumericUpDown numQuantity;
		private System.Windows.Forms.Label labQuantity;
	
		public double Quantity 
		{
			get{return _qty;}
			set{_qty = value;}
		}

		public Hashtable SelectedRItems = new Hashtable();  
		XmlNode node = null;
		//XmlUtilities xml = null;

		private System.Windows.Forms.Button btnProceed;
        private TextBox txtValue;
        private Label lblvalue;
        private ErrorProvider errorProvider1;
        private DataGridView dgRelatedItems;
        private Label lAuthorise;
        private Button btnRemove;
        private IContainer components;

		public RelatedProducts()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		public RelatedProducts(DataView RelatedProducts, double quantity, XmlNode currentItem, 
					int ItemId, string acctType, System.Windows.Forms.Form par, Form root)
		{
			InitializeComponent();
			TranslateControls();

			this.FormParent = par;
			this.FormRoot = root;
			this.numQuantity.Value = Convert.ToInt32(quantity);
			this.node = currentItem;
			this.ItemId = ItemId;
			AccountType = acctType;

			originalDoc.LoadXml(((NewAccount)FormParent).itemDoc.DocumentElement.OuterXml);

			xml = new XmlUtilities();

			dgRelatedItems.DataSource = RelatedProducts;
            dgRelatedItems.Rows[0].Selected = true;
            dgRelatedItems.CurrentCell=dgRelatedItems.Rows[0].Cells[0];
			DataGridTableStyle tabStyle = new DataGridTableStyle();
			tabStyle.MappingName = RelatedProducts.Table.TableName;
			
            dgRelatedItems.AutoResizeColumns();
            
			_selectedItems = new DataTable();
			_selectedItems.Columns.AddRange(new DataColumn[]{new DataColumn(CN.ItemNo), new DataColumn(CN.Quantity), new DataColumn(CN.Value)});
			dgSelectedItems.DataSource = _selectedItems.DefaultView;
			_selectedItems.DefaultView.AllowDelete = false;
			_selectedItems.DefaultView.AllowNew = false;
			_selectedItems.DefaultView.AllowEdit = false;
            
			tabStyle = new DataGridTableStyle();
			tabStyle.MappingName = _selectedItems.TableName;

			AddColumnStyle(CN.ItemNo,tabStyle, 85,true, GetResource("T_ITEMNO"), "", HorizontalAlignment.Left);
			AddColumnStyle(CN.Quantity,tabStyle, 80,true, GetResource("T_QUANTITY"), "", HorizontalAlignment.Left);

			dgSelectedItems.TableStyles.Add(tabStyle);

            spiffFilter = BuildItemFilter();
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RelatedProducts));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnEnter = new System.Windows.Forms.Button();
            this.dgSelectedItems = new System.Windows.Forms.DataGrid();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnProceed = new System.Windows.Forms.Button();
            this.labQuantity = new System.Windows.Forms.Label();
            this.numQuantity = new System.Windows.Forms.NumericUpDown();
            this.txtValue = new System.Windows.Forms.TextBox();
            this.lblvalue = new System.Windows.Forms.Label();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.dgRelatedItems = new System.Windows.Forms.DataGridView();
            this.lAuthorise = new System.Windows.Forms.Label();
            this.btnRemove = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgSelectedItems)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numQuantity)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgRelatedItems)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnRemove);
            this.groupBox1.Controls.Add(this.btnEnter);
            this.groupBox1.Controls.Add(this.dgSelectedItems);
            this.groupBox1.Controls.Add(this.btnCancel);
            this.groupBox1.Controls.Add(this.btnProceed);
            this.groupBox1.Location = new System.Drawing.Point(8, 216);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(344, 128);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            // 
            // btnEnter
            // 
            this.btnEnter.BackColor = System.Drawing.Color.SlateBlue;
            this.btnEnter.Font = new System.Drawing.Font("Arial Narrow", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnEnter.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.btnEnter.Image = global::STL.PL.Properties.Resources.plus;
            this.btnEnter.Location = new System.Drawing.Point(240, 32);
            this.btnEnter.Name = "btnEnter";
            this.btnEnter.Size = new System.Drawing.Size(20, 20);
            this.btnEnter.TabIndex = 22;
            this.btnEnter.UseVisualStyleBackColor = false;
            this.btnEnter.Click += new System.EventHandler(this.btnEnter_Click);
            // 
            // dgSelectedItems
            // 
            this.dgSelectedItems.CaptionVisible = false;
            this.dgSelectedItems.DataMember = "";
            this.dgSelectedItems.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgSelectedItems.Location = new System.Drawing.Point(16, 16);
            this.dgSelectedItems.Name = "dgSelectedItems";
            this.dgSelectedItems.Size = new System.Drawing.Size(208, 104);
            this.dgSelectedItems.TabIndex = 11;
            // 
            // btnCancel
            // 
            this.btnCancel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnCancel.Location = new System.Drawing.Point(272, 64);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(56, 24);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnProceed
            // 
            this.btnProceed.Enabled = false;
            this.btnProceed.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnProceed.Location = new System.Drawing.Point(272, 32);
            this.btnProceed.Name = "btnProceed";
            this.btnProceed.Size = new System.Drawing.Size(56, 24);
            this.btnProceed.TabIndex = 6;
            this.btnProceed.Text = "Proceed";
            this.btnProceed.Click += new System.EventHandler(this.btnProceed_Click);
            // 
            // labQuantity
            // 
            this.labQuantity.Location = new System.Drawing.Point(66, 186);
            this.labQuantity.Name = "labQuantity";
            this.labQuantity.Size = new System.Drawing.Size(48, 16);
            this.labQuantity.TabIndex = 25;
            this.labQuantity.Text = "Quantity";
            // 
            // numQuantity
            // 
            this.numQuantity.Location = new System.Drawing.Point(120, 184);
            this.numQuantity.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numQuantity.Name = "numQuantity";
            this.numQuantity.Size = new System.Drawing.Size(32, 20);
            this.numQuantity.TabIndex = 24;
            this.numQuantity.Tag = "";
            this.numQuantity.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // txtValue
            // 
            this.txtValue.Location = new System.Drawing.Point(252, 182);
            this.txtValue.Name = "txtValue";
            this.txtValue.Size = new System.Drawing.Size(100, 20);
            this.txtValue.TabIndex = 26;
            this.txtValue.TextChanged += new System.EventHandler(this.txtValue_TextChanged);
            // 
            // lblvalue
            // 
            this.lblvalue.Location = new System.Drawing.Point(198, 184);
            this.lblvalue.Name = "lblvalue";
            this.lblvalue.Size = new System.Drawing.Size(48, 16);
            this.lblvalue.TabIndex = 27;
            this.lblvalue.Text = "Value";
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // dgRelatedItems
            // 
            this.dgRelatedItems.AllowUserToAddRows = false;
            this.dgRelatedItems.AllowUserToDeleteRows = false;
            this.dgRelatedItems.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgRelatedItems.Location = new System.Drawing.Point(6, 3);
            this.dgRelatedItems.Name = "dgRelatedItems";
            this.dgRelatedItems.ReadOnly = true;
            this.dgRelatedItems.Size = new System.Drawing.Size(446, 175);
            this.dgRelatedItems.TabIndex = 28;
            this.dgRelatedItems.CellEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgRelatedItems_RowEnter);
            this.dgRelatedItems.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgRelatedItems_RowEnter);
            // 
            // lAuthorise
            // 
            this.lAuthorise.AutoSize = true;
            this.lAuthorise.Enabled = false;
            this.lAuthorise.Location = new System.Drawing.Point(390, 323);
            this.lAuthorise.Name = "lAuthorise";
            this.lAuthorise.Size = new System.Drawing.Size(35, 13);
            this.lAuthorise.TabIndex = 29;
            this.lAuthorise.Text = "label1";
            this.lAuthorise.Visible = false;
            // 
            // btnRemove
            // 
            this.btnRemove.BackColor = System.Drawing.Color.SlateBlue;
            this.btnRemove.Font = new System.Drawing.Font("Arial Narrow", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRemove.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.btnRemove.Image = ((System.Drawing.Image)(resources.GetObject("btnRemove.Image")));
            this.btnRemove.Location = new System.Drawing.Point(240, 66);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(20, 20);
            this.btnRemove.TabIndex = 148;
            this.btnRemove.UseVisualStyleBackColor = false;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // RelatedProducts
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(464, 350);
            this.ControlBox = false;
            this.Controls.Add(this.lAuthorise);
            this.Controls.Add(this.dgRelatedItems);
            this.Controls.Add(this.lblvalue);
            this.Controls.Add(this.txtValue);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.numQuantity);
            this.Controls.Add(this.labQuantity);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "RelatedProducts";
            this.Text = "Related Products";
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgSelectedItems)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numQuantity)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgRelatedItems)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion	

		private void btnEnter_Click(object sender, System.EventArgs e)
		{
			try
			{
				Function = "btnEnter_Click";
				Wait();
                btnProceed.Enabled = true;
				bool isSelected = false;
				int index = dgRelatedItems.CurrentRow.Index;

                bool authorised = false;        //IP - 07/07/11 - RI - #4037

				if(index>=0)
				{
                    ItemNo = (string)((DataView)dgRelatedItems.DataSource)[index]["ProductCode"];
                    ItemId = (int)((DataView)dgRelatedItems.DataSource)[index][CN.ItemId];              // RI
                    var qtyAvailable = ((DataView)dgRelatedItems.DataSource)[index][CN.QtyAvailable];   //IP - 07/07/11 - RI - #4037

					foreach(DataRowView addedItems in (DataView)dgSelectedItems.DataSource)
					{
                        if (addedItems[CN.ItemNo].ToString() == ItemNo)
						{
                               
							isSelected = true;
							break;
						}
					}

                    if (Convert.ToInt32(qtyAvailable) <= 0)                             //IP - 07/07/11 - RI - #4037 - If out of stock display authorisation prompt
                    {
                        if ((bool)Country[CountryParameterNames.OutOfStockAuth])
                        {
                            AuthorisePrompt auth = new AuthorisePrompt(this, lAuthorise, GetResource("M_NOSTOCKAUTH"));
                            auth.ShowDialog();
                            authorised = auth.Authorised;
                        }
                        else
                            authorised = true;

                    }
                    else
                    {
                        authorised = true;
                    }

                    if (authorised)                                                     //IP - 07/07/11 - RI - #4037 
                    {
                        if (!isSelected)
                        {
                            ListSpiffs(ItemNo, this.ParentLocn);
                            RelatedProductsChosen = true;
                            DataRow row = _selectedItems.NewRow();
                            row[CN.ItemNo] = ItemNo;
                            row[CN.Quantity] = this.numQuantity.Value.ToString();

                            _selectedItems.Rows.Add(row);

                            decimal value;
                            decimal.TryParse(txtValue.Text, out value);

                            AddRelatedItems(ItemId, Convert.ToDouble(numQuantity.Value), value);        // RI

                            if (related.Attributes[Tags.CanAddWarranty].Value == Boolean.TrueString)
                            {
                                AddWarranty(Convert.ToInt32(related.Attributes[Tags.ItemId].Value), this.ParentLocn, Convert.ToDouble(this.numQuantity.Value), ItemNo,
                                        ((NewAccount)FormParent).PaidAndTaken, ((NewAccount)FormParent).AllowInstantReplacement.Enabled, true);  // #15642


                                AddFreeWarranty(ItemNo, this.ParentLocn.ToString(), related.Attributes[Tags.ItemId].Value.ToString() + "|" + ParentLocn.ToString(), Convert.ToDouble(this.numQuantity.Value), 0);
                            }
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
				Function = "End of btnEnter_Click";
			}		
		} 

		private void btnProceed_Click(object sender, System.EventArgs e)
		{
			((NewAccount)FormParent).itemDoc = originalDoc;
			Close();
		}

		private void btnCancel_Click(object sender, System.EventArgs e)
		{
			SelectedRItems.Clear();
			Close();
		}

		private bool AddRelatedItems(int itemid, double quantity, decimal nonstockprice)       // RI
		{
			bool status = true;
            string key = itemid + "|" + this.ParentLocn.ToString();  // RI

            related = AccountManager.GetItemDetails(new GetItemDetailsRequest
            {
                ItemID = itemid,                    // RI
                StockLocationNo = this.ParentLocn,
                BranchCode = this.ParentLocn,
                AccountType = ((NewAccount)FormParent).AccountType,
                CountryCode = Config.CountryCode,
                IsDutyFree = this.DutyFree,
                IsTaxExempt = this.TaxExempt,
                AccountNo = this.AccountNo,
                AgrmtNo = this.AgreementNo
            }, out Error);

			if(Error.Length>0)
				ShowError(Error);
			else
			{
             
				XmlNode parent = xml.findItem(originalDoc.DocumentElement, this.ParentKey);
	
				XmlNode clone = related.Clone();
				clone = originalDoc.ImportNode(clone, true);

                if (nonstockprice == 0)
                    price = Convert.ToDecimal(StripCurrency(related.Attributes[Tags.UnitPrice].Value));
                else
                {
                    price = nonstockprice;
                    //clone.Attributes[Tags.CashPrice].Value = nonstockprice.ToString();
                    //clone.Attributes[Tags.HPPrice].Value = nonstockprice.ToString();
                    clone.Attributes[Tags.UnitPrice].Value = nonstockprice.ToString();
	
                }
                clone.Attributes[Tags.Value].Value = Convert.ToString(price * Convert.ToDecimal(quantity));     //CR1030 #3087 jec 03/02/11
				clone.Attributes[Tags.Quantity].Value = quantity.ToString();
				clone.Attributes[Tags.DeliveryDate].Value = this.DeliveryDate;
				clone.Attributes[Tags.DeliveryTime].Value = this.DeliveryTime;
				clone.Attributes[Tags.BranchForDeliveryNote].Value = this.DelNoteBranch;
				clone.Attributes[Tags.DeliveryProcess].Value = parent.Attributes[Tags.DeliveryProcess].Value;
                clone.Attributes[Tags.SalesBrnNo].Value = Convert.ToString(Config.BranchCode);                  //#19585
						
				parent.SelectSingleNode("RelatedItems").AppendChild(clone);
			}
			return status;
		}

        private bool AddWarranty(int itemid, short location, double quantity, string productCode, bool PaidAndTaken, bool allowInstantReplacement, bool allowSupaShield)           // #15642
		{
			bool status = true;
            string key = itemid + "|" + location.ToString(); // RI
			this._hasdatachanged=true;
            bool isIR = false;

			if(status)
			{
				Function = "BACcountManager::GetProductWarranties()";
                var warranties = Services.GetService(STL.Common.Services.Services.ServiceTypes.CosacsWeb).GetWarranties(productCode, location.ToString());      // #15642

                var taxType = (string)Country[CountryParameterNames.TaxType];
                //var agreementTaxType = (string)Country[CountryParameterNames.AgreementTaxType];
                var taxRate = Convert.ToDecimal(Country[CountryParameterNames.TaxRate]);
                //NewAccount.ApplyTaxInTaxInclusiveCountries(warranties, taxType, agreementTaxType, taxRate);

                if (warranties.Items != null)
                {
                   //hack to make warranty prices work without changing the Web
                    foreach (WarrantyResult.Item warrantyItem in warranties.Items)
                    {
                        warrantyItem.price.RetailPrice = warrantyItem.price.TaxInclusivePriceChange;
                    }
                }
              
                if(warranties != null && warranties.Items !=null)       // #16342
                {
                    foreach (var r in warranties.Items)
                    {
                        AccountManager.IsItemInstantReplacement(r.warrantyLink.Id, location, out isIR, out Error);
                        if (Error.Length > 0)
                            ShowError(Error);

                        if (!PaidAndTaken)
                        {
                            if (isIR)
                            {
                                if ((AccountType == AT.Cash || AccountType == AT.ReadyFinance)
                                    && (bool)Country[CountryParameterNames.IRCashRF])
                                {
                                    r.Status = "Y";
                                }
                                else
                                    r.Status = "N";
                            }
                            else
                                r.Status = "Y";
                        }
                        else
                        {
                            if (allowInstantReplacement && allowSupaShield)
                                r.Status = "Y";
                            else
                            {
                                if (allowInstantReplacement)
                                {
                                    if (isIR)
                                        r.Status = "Y";
                                    else
                                        r.Status = "N";
                                }
                                if (allowSupaShield)
                                {
                                    if (isIR)
                                        r.Status = "N";
                                    else
                                        r.Status = "Y";
                                }
                            }
                        }
                    }
                    var availableWarranties = new List<WarrantyItemXml>();      // #15642
                    foreach (var r in warranties.Items)
                    {
                        if (r.Status == "Y" && !WarrantyType.IsFree(r.warrantyLink.TypeCode)) //#17883
                        {
                            availableWarranties.Add(r.ToItem());
                        }

                    }

				    if(Error.Length>0)
					    ShowError(Error);

				    else
				    {
                        //DataView warranties = ds.Tables["Warranties"].DefaultView;					

                        if (allowSupaShield)
					    {
                            if (availableWarranties.Count > 0)          // #15642
						    {
							    //Launch a pop-up holding the warranties for the related products
                                RelatedProductWarranties warr = new RelatedProductWarranties(availableWarranties, quantity, related,        // #15642
															    location, AccountNo, AgreementNo, ((NewAccount)FormParent).ManualSale,
															    this, this.FormRoot, AccountType);
							    warr.ShowDialog();		//launch as a modal dialog

							    if(Warranty.Length>0)	//a warranty was selected
                                {
                                
								    /* need to ammend this process to create a seperate line item
								     * for each contract each with a qty of 1 */
                                    //XmlNode warranty = AccountManager.GetItemDetails(new STL.PL.WS2.GetItemDetailsRequest
                                    //{
                                    //    ProductCode = Warranty,
                                    //    ItemID = ItemId,                    // RI
                                    //    StockLocationNo = location,
                                    //    BranchCode = Convert.ToInt16(Config.BranchCode),
                                    //    AccountType = AccountType,
                                    //    CountryCode = Config.CountryCode,
                                    //    IsDutyFree = DutyFree,
                                    //    IsTaxExempt = TaxExempt
                                    //}, out Error);

                                    //XmlNode parent = xml.findItem(originalDoc.DocumentElement, key);
                                    string xPathRelated = "//Item[@Key = '" + key + "' and @Quantity != 0]";            // #16343
                                    XmlNode parent = originalDoc.DocumentElement.SelectSingleNode(xPathRelated);        // #16343
                                    var selectedWarranties = new List<WarrantyItemXml>();

                                    foreach (var r in availableWarranties)
                                    {
                                        if (r.Id == this.ItemId)
                                        {
                                            for (var i = 0; i < this.ContractNos.Rows.Count; i++)
                                            {
                                                var w = r.Clone();
                                                w.ContractNumber = Convert.ToString(ContractNos.Rows[i]["contractno"]);
                                                w.Value = w.PromotionPrice.HasValue ? w.PromotionPrice.Value : w.RetailPrice;
                                                w.Quantity = 1;
                                                w.DeliveryDate = parent.Attributes[Tags.DeliveryDate].Value;
                                                w.DeliveryTime = parent.Attributes[Tags.DeliveryTime].Value;
                                                w.BranchForDeliveryNote = parent.Attributes[Tags.BranchForDeliveryNote].Value;
                                                w.Location = location;
                                                selectedWarranties.Add(w);
                                            }
                                        }
                                    }

								    if(Error.Length>0)
									    ShowError(Error);
								    else
								    {	
                                        if (selectedWarranties.Count > 0)	//a warranty was selected
                                        {
                                            //XmlNode parent = xml.findItem(originalDoc.DocumentElement, key);

                                            /* tag the warranty on to the parent item's related items node */
                                            //     parent.SelectSingleNode("RelatedItems").AppendChild(new XmlNode());
                                            foreach (var w in selectedWarranties)
                                            {
                                                string xPath = "//Item[@Type = 'Warranty' and @Code = '" + w.Code + "' and @Location = '" + w.Location.ToString() + "' and @ContractNumber = '" + w.ContractNumber + "']";
                                                foreach (XmlNode toDel in originalDoc.SelectNodes(xPath))
                                                {
                                                    /* only delete them if their parent is the current item */
                                                    if (toDel.ParentNode.ParentNode.Attributes[Tags.Key].Value == key ||
                                                        toDel.Attributes[Tags.Quantity].Value == "0")
                                                        toDel.ParentNode.RemoveChild(toDel); /* properly delete rather than set qty to 0 */
                                                }
                                                parent.SelectSingleNode("RelatedItems").AppendChild(parent.OwnerDocument.ImportNode(w.ToXml(), true));
                                            }
                                            //LiveWire 69185 set a boolean value to true so that it is known that a warranty has been selected
                                            ((NewAccount)FormParent).warrantySelected = true;

                                        }
                                        /* reset the warranty field */
                                        Warranty = "";
                                        ContractNos = null;
								    }
							    }
						    }
						    else
							    status = false;
					    }
				    }
			    }
            }
			return status;
		}

        private void AddFreeWarranty(string code, string location, string key, double itemQty, int FreeReplacementQty = 0)           // #17677 - FreeReplacementQty from GRT
        {
            var warranties = Services.GetService(STL.Common.Services.Services.ServiceTypes.CosacsWeb).GetWarranties(code, location, WarrantyType.Free);

            if (warranties.Items == null || warranties.Items.Count == 0)// || warranties.Items.Count != 0)
                return;

            // #16169 - check for existing free warranties
            var existingFree = 0;

            string xPath = "//Item[@Type = 'Warranty' and @WarrantyType ='" + WarrantyType.Free + "']"; //#17287 //#16277

            string xPathItem = "//Item[@Code = '" + code + "' and @Location = '" + location.ToString() + "']";

            //#16303
            XmlNode item = originalDoc.SelectSingleNode(xPathItem);
            string xPathRelated = "RelatedItems/Item[@Type = 'Warranty' and @WarrantyType ='" + WarrantyType.Free + "' and @Quantity != '0']"; //#17287
            XmlNodeList freeWarranties = item.SelectNodes(xPathRelated);

            if (freeWarranties != null && freeWarranties.Count > 0)
            {
                existingFree = freeWarranties.Count;
            }

            // #16169 - If item quantity reduced - set quantity of surplus free warranties to zero
            if (existingFree > itemQty)
            {
                foreach (XmlNode exists in freeWarranties)
                {
                    if (existingFree > itemQty)
                    {
                        if (exists.Attributes[Tags.Quantity].Value == "1")
                        {
                            exists.Attributes[Tags.Quantity].Value = "0";
                            existingFree--;
                        }
                    }
                }
            }

            var qty = FreeReplacementQty == 0 ? itemQty - existingFree : existingFree == 0 ? 1 : FreeReplacementQty - existingFree; //#18437  // #17677 - use Replacement qty if value passed in from GRT

            for (var i = 0; i < qty; i++)
            {
                XmlNode parent = xml.findItem(originalDoc.DocumentElement, key);

                string Error;
                string contract = AccountManager.AutoWarranty(location, out Error);
                if (!string.IsNullOrEmpty(Error))
                    throw new Exception("Can not load warranty contract number. " + Error);

                var warranty = warranties.Items[0].ToItem();
                warranty.ContractNumber = contract;
                warranty.Value = 0;
                warranty.Quantity = 1;
                warranty.DeliveryDate = parent.Attributes[Tags.DeliveryDate].Value;
                warranty.DeliveryTime = parent.Attributes[Tags.DeliveryTime].Value;
                warranty.BranchForDeliveryNote = parent.Attributes[Tags.BranchForDeliveryNote].Value;
                warranty.Location = Convert.ToInt32(location);

                xPath = "//Item[@Type = 'Warranty' and @Code = '" + warranty.Code + "' and @Location = '" + warranty.Location.ToString() + "' and @ContractNumber = '" + warranty.ContractNumber + "']";
                foreach (XmlNode toDel in originalDoc.SelectNodes(xPath))
                {
                    if (toDel.ParentNode.ParentNode.Attributes[Tags.Key].Value == key ||
                        toDel.Attributes[Tags.Quantity].Value == "0")
                        toDel.ParentNode.RemoveChild(toDel);
                }
                parent.SelectSingleNode("RelatedItems").AppendChild(parent.OwnerDocument.ImportNode(warranty.ToXml(), true));
            }
        }

        string BuildItemFilter()
        {
            string filter = "itemtext in (";

            foreach (DataRowView row in (DataView)dgRelatedItems.DataSource)
                filter += "'" + (string)row["ProductCode"] + "',";

            filter = filter.Substring(0, filter.Length - 1);

            filter += ")";

            return filter;
        }

        private void ListSpiffs(string productCode, short location)
        {
            try
            {
                Function = "ListSpiffs()";
                DataSet ds = AccountManager.GetSpiffs(productCode, location, ItemId, out Error);
                if (Error.Length > 0)
                    ShowError(Error);
                else
                {
                    DataView dvSpiffs = ds.Tables[TN.Spiffs].DefaultView;

                    if (dvSpiffs.Count > 0)
                    {
                        dvSpiffs.RowFilter = spiffFilter;

                        if (dvSpiffs.Count > 0)
                        {
                            //launch a pop-up listing spiffs for similar associated products
                            SpiffSelection ss = new SpiffSelection(this.FormRoot, this, AccountType, dvSpiffs);
                            ss.ShowDialog();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
        }
        
        private void txtValue_TextChanged(object sender, EventArgs e)
        {
            // Bug #3033 - CR 1030 
            btnEnter.Enabled = true;
            errorProvider1.Clear();

            if (!txtValue.Visible)
                return;

            if(txtValue.Text == string.Empty)
            {
                btnEnter.Enabled = false;
            }
            else if(IsNumeric(txtValue.Text.Replace(".", string.Empty)) == false)
            {
                errorProvider1.SetError(txtValue, "Non numeric values not allowed");
                btnEnter.Enabled = false;
            }
            else
            {
                decimal value;
                if(decimal.TryParse(txtValue.Text, out value) == false)
                {
                    errorProvider1.SetError(txtValue, "Non numeric values not allowed");
                    btnEnter.Enabled = false;
                }
                else if(value < 0)
                {
                    errorProvider1.SetError(txtValue, "Negative values not allowed");
                    btnEnter.Enabled = false;   
                }
            }          
        }

        private void dgRelatedItems_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            
            if (dgRelatedItems.CurrentCell != null)
            {
                int rowindex = dgRelatedItems.CurrentCell.RowIndex; 
            
                DataGridViewRow row = dgRelatedItems.Rows[rowindex];
                //CR1030 #3087 jec 03/02/11  - for cash account use cash price otherwise use credit price
                //if ((Convert.ToDecimal(row.Cells["CashPrice"].Value) > 0 && AccountType=="C")       
                //    || (Convert.ToDecimal(row.Cells["CreditPrice"].Value) > 0 && AccountType!="C"))
                if ((Convert.ToDecimal(row.Cells["CashPrice"].Value) > 0 && (AccountType == "C" || AccountType == "S"))      //IP - 08/09/11 - RI - #8123 - If this is a Cash & Go sale and there is a CashPrice use CashPrice
                || (Convert.ToDecimal(row.Cells["CreditPrice"].Value) > 0 && AccountType != "C"))
                { //normal stockitem 
                    txtValue.Visible = false;
                    txtValue.Text = "0.0";
                    lblvalue.Visible = false;
                    numQuantity.Enabled = true;
                }
                else // value controlled item
                {
                    txtValue.Visible = true;
                    txtValue_TextChanged(txtValue, null);
                    numQuantity.Value = 1;
                    numQuantity.Enabled = false;
                }
            }
        }

        //IP - 07/07/11 - RI - #4038
        private void btnRemove_Click(object sender, EventArgs e)
        {
            int index = dgSelectedItems.CurrentRowIndex;

            if (index >= 0)
            {
                ((DataView)dgSelectedItems.DataSource).AllowDelete = true;
                ((DataView)dgSelectedItems.DataSource).Delete(index);
                ((DataView)dgSelectedItems.DataSource).AllowDelete = false;
            }
        }
	}
}
