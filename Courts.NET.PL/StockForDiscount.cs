using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using STL.Common;
using System.Data;
using System.Web.Services.Protocols;
using System.Xml;
using STL.PL.WS1;
using STL.PL.WS2;
using STL.PL.WS3;
using STL.PL.WS4;
using STL.PL.WS5;
using STL.Common.Constants.Tags;
using STL.Common.Constants.ItemTypes;
using STL.Common.Constants.Elements;

namespace STL.PL
{
	/// <summary>
	/// Popup prompt to list stock items currently on a customer order.
	/// This is used when a discount is added to an order. This popup
	/// allows the user to select a stock item from the list and link
	/// the discount to that stock item.
	/// </summary>
	public class StockForDiscount : CommonForm
	{
		private DataTable stock;
		private DataView stockView;
		private System.Windows.Forms.DataGrid dgStock;
		private string _key = "";
		private System.Windows.Forms.Button btnLink;
		private System.Windows.Forms.Button btnClose;
		public int Count = 0;
        private Button btnLinkToAll;
        public bool addDiscount = true;                         //IP - 07/07/11 - CR1212 - RI - #3817
	
		public string Key
		{
			get
			{
				return _key;
			}
			set
			{
				_key = value;
			}
		}
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public StockForDiscount(TranslationDummy d)
		{
			InitializeComponent();
		}

        public StockForDiscount(XmlDocument itemDoc, System.Windows.Forms.Form par, string filter, bool accountWideDiscount)    //IP - 31/05/11 - CR1212 - RI - #2315
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			TranslateControls();
			this.FormParent = par;

            if (accountWideDiscount)         //IP - 31/05/11 - CR1212 - RI - #2315
            {
                btnLinkToAll.Visible = true;
                btnLink.Visible = false;
            }   

			try
			{
				//Set up the table to display the stock items
				stock = new DataTable("Stock");
				stockView = new DataView(stock);

				stock.Columns.Add(new DataColumn("ProductCode"));
				stock.Columns.Add(new DataColumn("ProductDescription"));
                stock.Columns.Add(new DataColumn("ItemId"));            // RI
				stock.Columns.Add(new DataColumn("StockLocation"));
				stock.Columns.Add(new DataColumn("Type"));
                stock.Columns.Add(new DataColumn("ProductCategory"));

				//Set up the table to display the stock items
				dgStock.DataSource = stockView;
				DataGridTableStyle tabStyle = new DataGridTableStyle();
				tabStyle.MappingName = stock.TableName;
				dgStock.TableStyles.Add(tabStyle);
			
				tabStyle.GridColumnStyles["ProductCode"].Width = 76;
				tabStyle.GridColumnStyles["ProductCode"].Alignment = HorizontalAlignment.Center;
				tabStyle.GridColumnStyles["ProductCode"].HeaderText = GetResource("T_PRODCODE");

				tabStyle.GridColumnStyles["ProductDescription"].Width = 235;
				tabStyle.GridColumnStyles["ProductDescription"].HeaderText = GetResource("T_PRODDESC");

				tabStyle.GridColumnStyles["StockLocation"].Width = 70;
				tabStyle.GridColumnStyles["StockLocation"].HeaderText = GetResource("T_STOCKLOCN");

				tabStyle.GridColumnStyles["Type"].Width = 0;
                tabStyle.GridColumnStyles["ProductCategory"].Width = 0;
                tabStyle.GridColumnStyles["ItemId"].Width = 0;              //IP - 01/06/11 - CR1212 - RI - Hide ItemID column

				//populate the table from the XML document
				populateTable(itemDoc.DocumentElement);
				stockView.RowFilter = "Type = '" + IT.Kit + "' or Type = '" + IT.Stock + "'" + filter;
                Count = stockView.Count;

                if (Count == 0)
                    addDiscount = false;                //IP - 07/06/11 - CR1212 - RI - #3817
			}
			catch(Exception ex)
			{
				Catch(ex, Function);
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

		/// <summary>
		/// This function will call itself recursively until all 
		/// item nodes in the XML document have been entered into 
		/// the table.
		/// For each child node of the node passed in (which will
		/// always be an <item></item> node) create a row for that item
		/// and, if it's related items node has children, call this function
		/// again passing in that node.
		/// Also build up the tree view in the same format as the document
		/// </summary>
		/// <param name="relatedItems"></param>
		private void populateTable(XmlNode relatedItems)
		{
			Function = "populateTable";

			//outer loop iterates through <item> tags
			foreach(XmlNode item in relatedItems.ChildNodes)
			{
				if(item.NodeType == XmlNodeType.Element)
				{
					if(Convert.ToDouble(item.Attributes[Tags.Quantity].Value)>0)
					{
						DataRow row = stock.NewRow();
						row["Type"] = item.Attributes[Tags.Type].Value;
						row["ProductCode"] = item.Attributes[Tags.Code].Value;
                        row["ItemId"] = item.Attributes[Tags.ItemId].Value;
						row["ProductDescription"] = item.Attributes[Tags.Description1].Value;
						row["StockLocation"] = item.Attributes[Tags.Location].Value;
                        row["ProductCategory"] = item.Attributes[Tags.ProductCategory].Value;

						foreach(XmlNode child in item.ChildNodes)
							if(child.NodeType==XmlNodeType.Element&&child.Name==Elements.RelatedItem)
							{
								if(child.HasChildNodes)
									populateTable(child);
							}
						stock.Rows.Add(row);
					}
				}
			}
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StockForDiscount));
            this.dgStock = new System.Windows.Forms.DataGrid();
            this.btnLink = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnLinkToAll = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgStock)).BeginInit();
            this.SuspendLayout();
            // 
            // dgStock
            // 
            this.dgStock.CaptionText = "Account Stock Items";
            this.dgStock.DataMember = "";
            this.dgStock.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgStock.Location = new System.Drawing.Point(8, 8);
            this.dgStock.Name = "dgStock";
            this.dgStock.ReadOnly = true;
            this.dgStock.Size = new System.Drawing.Size(424, 144);
            this.dgStock.TabIndex = 0;
            // 
            // btnLink
            // 
            this.btnLink.Location = new System.Drawing.Point(87, 158);
            this.btnLink.Name = "btnLink";
            this.btnLink.Size = new System.Drawing.Size(56, 23);
            this.btnLink.TabIndex = 1;
            this.btnLink.Text = "Link";
            this.btnLink.Click += new System.EventHandler(this.btnLink_Click);
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(312, 158);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(56, 23);
            this.btnClose.TabIndex = 2;
            this.btnClose.Text = "Close";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnLinkToAll
            // 
            this.btnLinkToAll.Location = new System.Drawing.Point(184, 158);
            this.btnLinkToAll.Name = "btnLinkToAll";
            this.btnLinkToAll.Size = new System.Drawing.Size(72, 23);
            this.btnLinkToAll.TabIndex = 3;
            this.btnLinkToAll.Text = "Link To All";
            this.btnLinkToAll.Visible = false;
            this.btnLinkToAll.Click += new System.EventHandler(this.btnLinkToAll_Click);
            // 
            // StockForDiscount
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(440, 189);
            this.ControlBox = false;
            this.Controls.Add(this.btnLinkToAll);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnLink);
            this.Controls.Add(this.dgStock);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "StockForDiscount";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            ((System.ComponentModel.ISupportInitialize)(this.dgStock)).EndInit();
            this.ResumeLayout(false);

		}
		#endregion

		private void btnLink_Click(object sender, System.EventArgs e)
		{
			Function = "btnLink_Click";
			try
			{
				int index = dgStock.CurrentRowIndex;
				if(index>=0)	//may be empty
				{
					//Store the key of the row clicked.
                    //Key = (string)dgStock[index, 0] + "|" + (string)dgStock[index, 2];  //TODO need to change the key with ItemId
                    Key = (string)dgStock[index, 2] + "|" + (string)dgStock[index, 3];  // RI
					((NewAccount)this.FormParent).LinkToKey = Key;
					Close();
				}
			}
			catch(Exception ex)
			{
				Catch(ex, Function);
			}
		}

		private void btnClose_Click(object sender, System.EventArgs e)
		{
            //((NewAccount)this.FormParent).LinkToKey = "";
            addDiscount = false;                                //IP - CR1212 - RI - #3817
			Close();
		}

        private void btnLinkToAll_Click(object sender, EventArgs e)
        {
            ((NewAccount)this.FormParent).LinkDiscToAll = true;

            this.Close();
    
        }
	}
}
