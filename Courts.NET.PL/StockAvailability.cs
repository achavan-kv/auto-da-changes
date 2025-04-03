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

namespace STL.PL
{
	/// <summary>
	/// Popup prompt to list outstanding purchase orders for items out of stock.
	/// This is used when adding an out of stock item to a customer order. The 
	/// purchase orders can be reviewed and the required delivery date agreed with
	/// the customer for when the stock is expected to be available.
	/// Alternatively the out of stock item can be cancelled so that it is not
	/// added to the customer order.
	/// </summary>
	public class StockAvailability : CommonForm
	{
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.DataGrid dgItems;
        private System.Windows.Forms.Label lTimeRequired;
		public System.Windows.Forms.DateTimePicker dtDeliveryRequired;
		private System.Windows.Forms.Label lDeliveryRequired;
		private System.Windows.Forms.CheckBox chxAlignDates;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnContinue;
		private Crownwood.Magic.Menus.MenuCommand menuFile;
		public bool stockPurchased = false;
        public string purchaseOrderNumber = "";
        public ComboBox cbTime;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public StockAvailability(TranslationDummy d)
		{
			InitializeComponent();
		}

		public StockAvailability(DataView purchaseOrders, System.Windows.Forms.Form par, Form root)
		{
			InitializeComponent();

			this.FormParent = par;
			this.FormRoot = root;

			dgItems.DataSource = purchaseOrders;

			DataGridTableStyle tabStyle = new DataGridTableStyle();
			tabStyle.MappingName = purchaseOrders.Table.TableName;
			dgItems.TableStyles.Add(tabStyle);
				
			tabStyle.GridColumnStyles[CN.ItemNo].Width = 60;
			tabStyle.GridColumnStyles[CN.ItemNo].HeaderText = GetResource("T_ITEMNO");

			tabStyle.GridColumnStyles[CN.ReceiptDate].Width = 100;
			tabStyle.GridColumnStyles[CN.ReceiptDate].HeaderText = GetResource("T_RECEIPTDATE");

			tabStyle.GridColumnStyles[CN.SupplierNo].Width = 100;
			tabStyle.GridColumnStyles[CN.SupplierNo].HeaderText = GetResource("T_SUPPLIERCODE");

			tabStyle.GridColumnStyles[CN.Quantity].Width = 90;
			tabStyle.GridColumnStyles[CN.Quantity].HeaderText = GetResource("T_QUANTITYONORDER");

			tabStyle.GridColumnStyles[CN.StockLocn].Width = 100;
			tabStyle.GridColumnStyles[CN.StockLocn].HeaderText = GetResource("T_STOCKLOCN");

            tabStyle.GridColumnStyles[CN.PurchaseOrderNumber].Width = 0;

			dtDeliveryRequired.Value = DateTime.Today;
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cbTime = new System.Windows.Forms.ComboBox();
            this.btnContinue = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.chxAlignDates = new System.Windows.Forms.CheckBox();
            this.lTimeRequired = new System.Windows.Forms.Label();
            this.dtDeliveryRequired = new System.Windows.Forms.DateTimePicker();
            this.lDeliveryRequired = new System.Windows.Forms.Label();
            this.dgItems = new System.Windows.Forms.DataGrid();
            this.menuFile = new Crownwood.Magic.Menus.MenuCommand();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgItems)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cbTime);
            this.groupBox1.Controls.Add(this.btnContinue);
            this.groupBox1.Controls.Add(this.btnCancel);
            this.groupBox1.Controls.Add(this.chxAlignDates);
            this.groupBox1.Controls.Add(this.lTimeRequired);
            this.groupBox1.Controls.Add(this.dtDeliveryRequired);
            this.groupBox1.Controls.Add(this.lDeliveryRequired);
            this.groupBox1.Location = new System.Drawing.Point(32, 176);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(416, 160);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            // 
            // cbTime
            // 
            this.cbTime.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbTime.FormattingEnabled = true;
            this.cbTime.Items.AddRange(new object[] {
            "AM",
            "PM"});
            this.cbTime.Location = new System.Drawing.Point(261, 40);
            this.cbTime.Name = "cbTime";
            this.cbTime.Size = new System.Drawing.Size(59, 21);
            this.cbTime.TabIndex = 69;
            this.cbTime.SelectedIndexChanged += new System.EventHandler(this.cbTime_SelectedIndexChanged);
            // 
            // btnContinue
            // 
            this.btnContinue.Location = new System.Drawing.Point(104, 120);
            this.btnContinue.Name = "btnContinue";
            this.btnContinue.Size = new System.Drawing.Size(75, 23);
            this.btnContinue.TabIndex = 68;
            this.btnContinue.Text = "Continue";
            this.btnContinue.Click += new System.EventHandler(this.btnContinue_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(240, 120);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 67;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // chxAlignDates
            // 
            this.chxAlignDates.Location = new System.Drawing.Point(88, 80);
            this.chxAlignDates.Name = "chxAlignDates";
            this.chxAlignDates.Size = new System.Drawing.Size(232, 24);
            this.chxAlignDates.TabIndex = 34;
            this.chxAlignDates.Text = "Align existing items planned delivery date";
            this.chxAlignDates.CheckedChanged += new System.EventHandler(this.chxAlignDates_CheckedChanged);
            // 
            // lTimeRequired
            // 
            this.lTimeRequired.Location = new System.Drawing.Point(256, 24);
            this.lTimeRequired.Name = "lTimeRequired";
            this.lTimeRequired.Size = new System.Drawing.Size(32, 16);
            this.lTimeRequired.TabIndex = 33;
            this.lTimeRequired.Text = "Time:";
            // 
            // dtDeliveryRequired
            // 
            this.dtDeliveryRequired.CustomFormat = "ddd dd MMM yyyy";
            this.dtDeliveryRequired.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtDeliveryRequired.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.dtDeliveryRequired.Location = new System.Drawing.Point(88, 40);
            this.dtDeliveryRequired.Name = "dtDeliveryRequired";
            this.dtDeliveryRequired.Size = new System.Drawing.Size(112, 20);
            this.dtDeliveryRequired.TabIndex = 30;
            this.dtDeliveryRequired.Value = new System.DateTime(1900, 1, 1, 0, 0, 0, 0);
            // 
            // lDeliveryRequired
            // 
            this.lDeliveryRequired.Location = new System.Drawing.Point(88, 24);
            this.lDeliveryRequired.Name = "lDeliveryRequired";
            this.lDeliveryRequired.Size = new System.Drawing.Size(112, 16);
            this.lDeliveryRequired.TabIndex = 32;
            this.lDeliveryRequired.Text = "Delivery Required:";
            // 
            // dgItems
            // 
            this.dgItems.CaptionText = "Purchase Orders";
            this.dgItems.DataMember = "";
            this.dgItems.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgItems.Location = new System.Drawing.Point(32, 8);
            this.dgItems.Name = "dgItems";
            this.dgItems.ReadOnly = true;
            this.dgItems.Size = new System.Drawing.Size(416, 160);
            this.dgItems.TabIndex = 7;
            this.dgItems.MouseUp += new System.Windows.Forms.MouseEventHandler(this.dgItems_MouseUp);
            // 
            // menuFile
            // 
            this.menuFile.Description = "MenuItem";
            this.menuFile.Text = "&File";
            // 
            // StockAvailability
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(488, 349);
            this.ControlBox = false;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.dgItems);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "StockAvailability";
            this.Text = "Stock Availability Dates";
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgItems)).EndInit();
            this.ResumeLayout(false);

		}
		#endregion

		private void btnContinue_Click(object sender, System.EventArgs e)
		{
			try
			{
				if(FormParent.GetType().Name == "NewAccount")
				{
					string loc = ((NewAccount)this.FormParent).Location;
					((NewAccount)this.FormParent).ClearItemDetails();
					((NewAccount)this.FormParent).Location = loc;
					((NewAccount)FormParent).drpLocation_Validating(this, new CancelEventArgs());
					((NewAccount)this.FormParent).PurchaseOrder = true;

					if(chxAlignDates.Checked)
						((NewAccount)this.FormParent).AlignedDelDate = dtDeliveryRequired.Value;
				}
			}
			catch(Exception ex)
			{
				Catch(ex, Function);
			}
			finally
			{
				Close();
			}		
		}

		private void dgItems_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			try
			{
				if(dgItems.CurrentRowIndex>=0)
				{
					dgItems.Select(dgItems.CurrentCell.RowNumber);
					
					if(FormParent.GetType().Name == "NewAccount")
					{
						((NewAccount)this.FormParent).Location = Convert.ToString(dgItems[dgItems.CurrentRowIndex,4]);
					}

					DateTime delDate = Convert.ToDateTime(dgItems[dgItems.CurrentRowIndex,1]);
					dtDeliveryRequired.Value = delDate.AddDays(Convert.ToInt32(Country[CountryParameterNames.DefaultDelDays]));
                    purchaseOrderNumber = (string)dgItems[dgItems.CurrentRowIndex, 5];
				}
			}
			catch(Exception ex)
			{
				Catch(ex, Function);
			}
		}

		private void chxAlignDates_CheckedChanged(object sender, System.EventArgs e)
		{
			if(FormParent.GetType().Name == "NewAccount")
				((NewAccount)this.FormParent).AlignDates = chxAlignDates.Checked;
		}

		private void btnCancel_Click(object sender, System.EventArgs e)
		{
			if(FormParent.GetType().Name == "NewAccount")
			{
				((NewAccount)this.FormParent).PurchaseOrder = false;
				((NewAccount)this.FormParent).AlignDates = false;
				
				Close();
			}
		}

        //IP - 28/05/12 - #10225
        private void cbTime_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Convert.ToString(cbTime.SelectedItem) == "AM")
            {
                dtDeliveryRequired.Value = dtDeliveryRequired.Value;

            }
            else
            {
                dtDeliveryRequired.Value = dtDeliveryRequired.Value.AddHours(12);
            }
        }
	}
}
