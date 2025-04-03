using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using STL.Common;
using STL.Common.Constants.TableNames;
using STL.Common.Constants.Tags;
using STL.Common.Static;
//using STL.PL.SERVICE;
using STL.PL.WS1;
using STL.PL.WS2;
using STL.PL.Utils;
using Blue.Cosacs.Shared.Extensions;
//using STL.PL.Installation;

namespace STL.PL
{
	/// <summary>
	/// Search screen to list stock items that can be selected and added to a sale.
	/// A specific branch code must be entered and any part of the product description
	/// can be optionally entered. The products are then listed for these criteria.
	/// The list includes columns to show availability and stock on order.
	/// </summary>
	public class LocationStockEnquiry : CommonForm
    {
        #region Designer Region
        private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.CheckBox chxShowDeleted;
		private System.Windows.Forms.TextBox txtProdDesc;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.CheckBox chxShowInStock;
		private System.Windows.Forms.Button btnSearch;
		private System.Windows.Forms.Button btnSearchList;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox txtProdDesc1;
		private System.Windows.Forms.TextBox txtProductCode;
		private System.Windows.Forms.TextBox txtSupplierCode;
		private System.Windows.Forms.TextBox txtProdDesc2;
		private System.Windows.Forms.ErrorProvider errorProvider1;		
		private System.Windows.Forms.DataGrid dgItems;
		private System.Windows.Forms.TreeView tvUnitPrice;
		private System.Windows.Forms.CheckBox chxLimit;
		private System.Windows.Forms.Button btnEnter;
		private System.Windows.Forms.Button btnClear;
		//private Crownwood.Magic.Menus.MenuControl menuMain;
		private Crownwood.Magic.Menus.MenuCommand menuFile;
		private Crownwood.Magic.Menus.MenuCommand menuSearch;
		private Crownwood.Magic.Menus.MenuCommand menuSearchList;
		private Crownwood.Magic.Menus.MenuCommand menuClear;
		private Crownwood.Magic.Menus.MenuCommand menuExit;
        private System.Windows.Forms.ComboBox drpLocation;
        #endregion
        private IContainer components;

        private new string Error = "";
        private DataSet byLocation;
		private short location=0;
		private int showDeleted=0;
		private int showInStock=0;
		private string productDesc="";
        private Label label8;
        private Label label7;
        private TextBox txtColour;
        private TextBox txtStyle;
		private bool limit=false;
		
		public LocationStockEnquiry(TranslationDummy d) 
            : this() { }

		public LocationStockEnquiry()
	    {
            InitializeComponent();
            menuMain = new Crownwood.Magic.Menus.MenuControl();
            menuMain.MenuCommands.AddRange(new [] { menuFile });
            drpLocation_GetBranches();
	    }

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LocationStockEnquiry));
            this.txtProdDesc1 = new System.Windows.Forms.TextBox();
            this.btnSearch = new System.Windows.Forms.Button();
            this.btnSearchList = new System.Windows.Forms.Button();
            this.chxShowInStock = new System.Windows.Forms.CheckBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtProductCode = new System.Windows.Forms.TextBox();
            this.txtProdDesc2 = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.tvUnitPrice = new System.Windows.Forms.TreeView();
            this.dgItems = new System.Windows.Forms.DataGrid();
            this.chxShowDeleted = new System.Windows.Forms.CheckBox();
            this.txtProdDesc = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.drpLocation = new System.Windows.Forms.ComboBox();
            this.btnEnter = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.chxLimit = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.txtColour = new System.Windows.Forms.TextBox();
            this.txtStyle = new System.Windows.Forms.TextBox();
            this.txtSupplierCode = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.menuFile = new Crownwood.Magic.Menus.MenuCommand();
            this.menuSearch = new Crownwood.Magic.Menus.MenuCommand();
            this.menuSearchList = new Crownwood.Magic.Menus.MenuCommand();
            this.menuClear = new Crownwood.Magic.Menus.MenuCommand();
            this.menuExit = new Crownwood.Magic.Menus.MenuCommand();
            ((System.ComponentModel.ISupportInitialize)(this.dgItems)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // txtProdDesc1
            // 
            this.txtProdDesc1.Location = new System.Drawing.Point(143, 40);
            this.txtProdDesc1.Name = "txtProdDesc1";
            this.txtProdDesc1.Size = new System.Drawing.Size(272, 20);
            this.txtProdDesc1.TabIndex = 9;
            // 
            // btnSearch
            // 
            this.btnSearch.Image = ((System.Drawing.Image)(resources.GetObject("btnSearch.Image")));
            this.btnSearch.Location = new System.Drawing.Point(576, 32);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(72, 24);
            this.btnSearch.TabIndex = 5;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // btnSearchList
            // 
            this.btnSearchList.Location = new System.Drawing.Point(576, 64);
            this.btnSearchList.Name = "btnSearchList";
            this.btnSearchList.Size = new System.Drawing.Size(75, 23);
            this.btnSearchList.TabIndex = 6;
            this.btnSearchList.Text = "search list";
            this.btnSearchList.Click += new System.EventHandler(this.btnSearchList_Click);
            // 
            // chxShowInStock
            // 
            this.chxShowInStock.Checked = true;
            this.chxShowInStock.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chxShowInStock.Location = new System.Drawing.Point(392, 72);
            this.chxShowInStock.Name = "chxShowInStock";
            this.chxShowInStock.Size = new System.Drawing.Size(128, 24);
            this.chxShowInStock.TabIndex = 4;
            this.chxShowInStock.Text = "Show items in stock";
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(143, 72);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(128, 16);
            this.label6.TabIndex = 3;
            this.label6.Text = "Product Description 2:";
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(24, 40);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(88, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "Stock Location:";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(160, 40);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(145, 16);
            this.label2.TabIndex = 2;
            this.label2.Text = "Description / Courts Code";
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(26, 24);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(88, 16);
            this.label3.TabIndex = 0;
            this.label3.Text = "Item No:";
            // 
            // txtProductCode
            // 
            this.txtProductCode.Location = new System.Drawing.Point(26, 40);
            this.txtProductCode.Name = "txtProductCode";
            this.txtProductCode.Size = new System.Drawing.Size(100, 20);
            this.txtProductCode.TabIndex = 7;
            // 
            // txtProdDesc2
            // 
            this.txtProdDesc2.Location = new System.Drawing.Point(143, 88);
            this.txtProdDesc2.Name = "txtProdDesc2";
            this.txtProdDesc2.Size = new System.Drawing.Size(272, 20);
            this.txtProdDesc2.TabIndex = 10;
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(143, 24);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(128, 16);
            this.label5.TabIndex = 2;
            this.label5.Text = "Product Description 1:";
            // 
            // tvUnitPrice
            // 
            this.tvUnitPrice.BackColor = System.Drawing.SystemColors.Control;
            this.tvUnitPrice.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tvUnitPrice.Location = new System.Drawing.Point(576, 32);
            this.tvUnitPrice.Name = "tvUnitPrice";
            this.tvUnitPrice.Size = new System.Drawing.Size(181, 80);
            this.tvUnitPrice.TabIndex = 11;
            // 
            // dgItems
            // 
            this.dgItems.CaptionText = "Stock Items";
            this.dgItems.DataMember = "";
            this.dgItems.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.errorProvider1.SetIconAlignment(this.dgItems, System.Windows.Forms.ErrorIconAlignment.TopRight);
            this.dgItems.Location = new System.Drawing.Point(8, 16);
            this.dgItems.Name = "dgItems";
            this.dgItems.ReadOnly = true;
            this.dgItems.Size = new System.Drawing.Size(760, 208);
            this.dgItems.TabIndex = 0;
            this.dgItems.MouseUp += new System.Windows.Forms.MouseEventHandler(this.dgItems_MouseUp);
            // 
            // chxShowDeleted
            // 
            this.chxShowDeleted.Location = new System.Drawing.Point(392, 48);
            this.chxShowDeleted.Name = "chxShowDeleted";
            this.chxShowDeleted.Size = new System.Drawing.Size(128, 24);
            this.chxShowDeleted.TabIndex = 3;
            this.chxShowDeleted.Text = "Show deleted items";
            // 
            // txtProdDesc
            // 
            this.txtProdDesc.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtProdDesc.Location = new System.Drawing.Point(160, 56);
            this.txtProdDesc.Name = "txtProdDesc";
            this.txtProdDesc.Size = new System.Drawing.Size(192, 20);
            this.txtProdDesc.TabIndex = 2;
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.SystemColors.Control;
            this.groupBox1.Controls.Add(this.drpLocation);
            this.groupBox1.Controls.Add(this.btnEnter);
            this.groupBox1.Controls.Add(this.btnClear);
            this.groupBox1.Controls.Add(this.chxLimit);
            this.groupBox1.Controls.Add(this.btnSearchList);
            this.groupBox1.Controls.Add(this.btnSearch);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.chxShowDeleted);
            this.groupBox1.Controls.Add(this.txtProdDesc);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.chxShowInStock);
            this.groupBox1.Location = new System.Drawing.Point(8, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(776, 112);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Search Criteria";
            // 
            // drpLocation
            // 
            this.drpLocation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpLocation.Location = new System.Drawing.Point(24, 56);
            this.drpLocation.Name = "drpLocation";
            this.drpLocation.Size = new System.Drawing.Size(72, 21);
            this.drpLocation.TabIndex = 14;
            // 
            // btnEnter
            // 
            this.btnEnter.Location = new System.Drawing.Point(712, 64);
            this.btnEnter.Name = "btnEnter";
            this.btnEnter.Size = new System.Drawing.Size(48, 23);
            this.btnEnter.TabIndex = 13;
            this.btnEnter.Text = "Exit";
            this.btnEnter.Click += new System.EventHandler(this.btnEnter_Click);
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(712, 32);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(48, 23);
            this.btnClear.TabIndex = 12;
            this.btnClear.Text = "Clear";
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // chxLimit
            // 
            this.chxLimit.Checked = true;
            this.chxLimit.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chxLimit.Location = new System.Drawing.Point(392, 24);
            this.chxLimit.Name = "chxLimit";
            this.chxLimit.Size = new System.Drawing.Size(104, 24);
            this.chxLimit.TabIndex = 9;
            this.chxLimit.Text = "view top 250";
            // 
            // groupBox2
            // 
            this.groupBox2.BackColor = System.Drawing.SystemColors.Control;
            this.groupBox2.Controls.Add(this.dgItems);
            this.groupBox2.Location = new System.Drawing.Point(8, 240);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(776, 232);
            this.groupBox2.TabIndex = 7;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Search Results";
            // 
            // groupBox3
            // 
            this.groupBox3.BackColor = System.Drawing.SystemColors.Control;
            this.groupBox3.Controls.Add(this.label8);
            this.groupBox3.Controls.Add(this.label7);
            this.groupBox3.Controls.Add(this.txtColour);
            this.groupBox3.Controls.Add(this.txtStyle);
            this.groupBox3.Controls.Add(this.tvUnitPrice);
            this.groupBox3.Controls.Add(this.txtProdDesc2);
            this.groupBox3.Controls.Add(this.txtProdDesc1);
            this.groupBox3.Controls.Add(this.txtSupplierCode);
            this.groupBox3.Controls.Add(this.txtProductCode);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Location = new System.Drawing.Point(8, 112);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(776, 128);
            this.groupBox3.TabIndex = 8;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Selected Product";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(437, 71);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(37, 13);
            this.label8.TabIndex = 15;
            this.label8.Text = "Colour";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(437, 23);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(64, 13);
            this.label7.TabIndex = 14;
            this.label7.Text = "Style/Model";
            // 
            // txtColour
            // 
            this.txtColour.Location = new System.Drawing.Point(437, 88);
            this.txtColour.Name = "txtColour";
            this.txtColour.Size = new System.Drawing.Size(100, 20);
            this.txtColour.TabIndex = 13;
            // 
            // txtStyle
            // 
            this.txtStyle.Location = new System.Drawing.Point(437, 40);
            this.txtStyle.Name = "txtStyle";
            this.txtStyle.Size = new System.Drawing.Size(100, 20);
            this.txtStyle.TabIndex = 12;
            // 
            // txtSupplierCode
            // 
            this.txtSupplierCode.Location = new System.Drawing.Point(26, 88);
            this.txtSupplierCode.Name = "txtSupplierCode";
            this.txtSupplierCode.Size = new System.Drawing.Size(100, 20);
            this.txtSupplierCode.TabIndex = 8;
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(26, 72);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(80, 16);
            this.label4.TabIndex = 1;
            this.label4.Text = "Supplier Code:";
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            this.errorProvider1.DataMember = "";
            // 
            // menuFile
            // 
            this.menuFile.Description = "MenuItem";
            this.menuFile.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {
            this.menuSearch,
            this.menuSearchList,
            this.menuClear,
            this.menuExit});
            this.menuFile.Text = "&File";
            // 
            // menuSearch
            // 
            this.menuSearch.Description = "MenuItem";
            this.menuSearch.Text = "&Search";
            this.menuSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // menuSearchList
            // 
            this.menuSearchList.Description = "MenuItem";
            this.menuSearchList.Text = "Search &List";
            this.menuSearchList.Click += new System.EventHandler(this.btnSearchList_Click);
            // 
            // menuClear
            // 
            this.menuClear.Description = "MenuItem";
            this.menuClear.Text = "&Clear";
            this.menuClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // menuExit
            // 
            this.menuExit.Description = "MenuItem";
            this.menuExit.Text = "E&xit";
            // 
            // LocationStockEnquiry
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.SystemColors.ControlDark;
            this.ClientSize = new System.Drawing.Size(792, 477);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "LocationStockEnquiry";
            this.Text = "Stock Enquiry By Location";
            ((System.ComponentModel.ISupportInitialize)(this.dgItems)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);

		}
		#endregion

		private void drpLocation_GetBranches()
		{
            Function = "drpLocation_GetBranches";
			
			try
			{
                if (drpLocation.Items.Count > 0)
                    return;

                StringCollection Branchnos = new StringCollection();

                //these are already loaded in the newaccount screen so don't need to load again
                foreach (DataRow row in ((DataTable)StaticData.Tables[TN.BranchNumber]).Rows)
                    Branchnos.Add(Convert.ToString(row["branchno"]));

                drpLocation.DataSource = Branchnos;
                errorProvider1.SetError(txtProductCode, "");
			}
			catch(NullReferenceException ex)
			{
				txtProductCode.Focus();
				txtProductCode.SelectAll();
				errorProvider1.SetError(txtProductCode, ex.Message);
			}
			catch(Exception ex)
			{
				Catch(ex, Function);
			}
		}

		private void SearchThread()
		{
			try
			{
				Wait();
				Function = "SearchThread";

                byLocation = AccountManager.GetStockByLocation(new GetStockByLocationRequest
                {
                    StockLocationNo = location,
                    BranchCode      = Convert.ToInt16(Config.BranchCode),
                    ShowDeleted     = showDeleted,
                    ShowAvailable   = showInStock,
                    ProductDesction = productDesc,
                    Limit           = limit
                }, out Error);

				if(Error.Length > 0)
					ShowError(Error);						
			}
			catch(Exception ex)
			{
				Catch(ex, Function);
			}
			finally
			{
				StopWait();
				Function = "End of SearchThread";
			}
		}

	    private void btnSearch_Click(object sender, System.EventArgs e)
		{
            try
            {
                Function = "BAccountManager::GetStockByLocation()";
                Wait();

                location = Convert.ToInt16(drpLocation.Text);

                showDeleted = Convert.ToInt32(chxShowDeleted.Checked);
                showInStock = Convert.ToInt32(chxShowInStock.Checked);
                productDesc = txtProdDesc.Text;  //Enclosed by double quotes for FTS Fulltext function
                limit = chxLimit.Checked;

                //Thread data = new Thread(new ThreadStart(SearchThread));
                //data.Start();
                //data.Join();
                //No point starting a thread and joining immediately, just extra overhead
                //The only function of this screen is to search products
                //So user's not gonna carry out a second search before the first search finishes

                SearchThread();

                if (byLocation != null)
                {
                    dgItems.DataSource = byLocation;
                    dgItems.DataMember = "ByLocation";
                    SetGridTableStyle();
                    ((MainForm)this.FormRoot).StatusBarText = String.Format("{0} rows returned", byLocation.Tables["ByLocation"].Rows.Count);
                }
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
            finally
            {
                StopWait();
            }
		}

        private void SetGridTableStyle()
        {
            if (dgItems.TableStyles.Count > 0)
                return;

            DataGridTableStyle tabStyle = new DataGridTableStyle();
            tabStyle.MappingName = byLocation.Tables[0].TableName;
            dgItems.TableStyles.Add(tabStyle);

            tabStyle.GridColumnStyles["Product Code"].Width = 76;
            tabStyle.GridColumnStyles["Product Code"].Alignment = HorizontalAlignment.Center;
            tabStyle.GridColumnStyles["Product Code"].HeaderText = GetResource("T_PRODCODE");

            tabStyle.GridColumnStyles["D"].Width = 80;
            tabStyle.GridColumnStyles["D"].HeaderText = GetResource("T_PRODSTATUS"); // UAT 770
            tabStyle.GridColumnStyles["Product Description 1"].Width = 165;
            tabStyle.GridColumnStyles["Product Description 1"].HeaderText = GetResource("T_PRODDESC");
            tabStyle.GridColumnStyles["Product Description 2"].Width = 165;
            tabStyle.GridColumnStyles["Product Description 2"].HeaderText = GetResource("T_PRODDESC2");
            tabStyle.GridColumnStyles["Actual Stock"].Width = 50;
            tabStyle.GridColumnStyles["Actual Stock"].Alignment = HorizontalAlignment.Right;
            tabStyle.GridColumnStyles["Actual Stock"].HeaderText = GetResource("T_ACTUALSTOCK");
            tabStyle.GridColumnStyles["Available Stock"].Width = 50;
            tabStyle.GridColumnStyles["Available Stock"].Alignment = HorizontalAlignment.Right;
            tabStyle.GridColumnStyles["Available Stock"].HeaderText = GetResource("T_AVAILSTOCK");
            tabStyle.GridColumnStyles["Damage Stock"].Width = 50;
            tabStyle.GridColumnStyles["Damage Stock"].Alignment = HorizontalAlignment.Right;
            tabStyle.GridColumnStyles["Damage Stock"].HeaderText = GetResource("T_DAMAGESTOCK");
            tabStyle.GridColumnStyles["Stock On Order"].Width = 50;
            tabStyle.GridColumnStyles["Stock On Order"].Alignment = HorizontalAlignment.Right;
            tabStyle.GridColumnStyles["Stock On Order"].HeaderText = GetResource("T_STOCKONORDER");
            //69647 Add a Deleted Product column to the data grid
            tabStyle.GridColumnStyles["deleted"].Width = 50;
            tabStyle.GridColumnStyles["deleted"].HeaderText = GetResource("T_DELETEDITEM");
            tabStyle.GridColumnStyles["deleted"].Alignment = HorizontalAlignment.Center;
            tabStyle.GridColumnStyles["Delivery Date"].Width = 76;
            tabStyle.GridColumnStyles["Delivery Date"].Alignment = HorizontalAlignment.Center;
            tabStyle.GridColumnStyles["Delivery Date"].HeaderText = GetResource("T_DELIVERYDATE");
            tabStyle.GridColumnStyles["Delivery Date"].NullText = "";
            tabStyle.GridColumnStyles["Supplier Code"].Width = 0;
            tabStyle.GridColumnStyles["Vendor EAN"].NullText = "";
            tabStyle.GridColumnStyles["ItemID"].Width = 0;
            tabStyle.GridColumnStyles["Brand"].Width = 76;            //RM - 06/10/11 - RI
            tabStyle.GridColumnStyles["Model"].Width = 76;                   //IP - 26/07/11 - RI
            tabStyle.GridColumnStyles["VendorStyle"].Width = 0;                 //IP - 27/07/11 - RI
            tabStyle.GridColumnStyles["Colour"].Width = 0;                      //IP - 27/07/11 - RI
        }

		/// <summary>
		/// Validates the branch number entered by attempting to 
		/// convert it to a short
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
	    /*	private void txtLocation_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			try
			{
				Convert.ToInt16(txtLocation.Text);
				errorProvider1.SetError(txtLocation, "");
			}
			catch(Exception ex)
			{
				//e.Cancel = true;
				txtLocation.Select(0, txtLocation.Text.Length);
				errorProvider1.SetError(txtLocation, ex.Message);
			}
		}*/

        //private void txtProdDesc_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        //{
        //    txtProdDesc.Text = txtProdDesc.Text.ToUpper();
        //}

		/// <summary>
		/// Scans the datagrid for a particular product description
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnSearchList_Click(object sender, System.EventArgs e)
		{
			Function = "btnSearchList_Click";
			try
			{
                if (dgItems.DataSource == null)
                    return;

                int rowCount = byLocation.Tables["ByLocation"].Rows.Count;

                for (int index = 0; index < rowCount; index++)
                {
                    string desc = (string)dgItems[index, 2];
                    if (desc.Contains(txtProdDesc.Text.Trim()))
                    {
                        dgItems.CurrentRowIndex = index;
                        dgItems.Select(index);
                        break;
                    }
                }
			}
			catch(Exception ex)
			{
				Catch(ex, Function);
			}			
		}

		private void btnClear_Click(object sender, System.EventArgs e)
		{
			txtProdDesc.Text = "";
			txtProdDesc1.Text = "";
			txtProdDesc2.Text = "";
			txtSupplierCode.Text = "";
			txtProductCode.Text = "";
            txtStyle.Text = "";
            txtColour.Text = "";

            tvUnitPrice.Nodes.Clear();

            if (!FormParent.IsDisposed && FormParent is NewAccount)
            {
                var parent = (FormParent as NewAccount);
                parent.ProductCode = String.Empty;
                parent.ItemID = null;
                parent.Location = String.Empty;
            }
            //else if (!FormParent.IsDisposed && FormParent is SR_ServiceRequest)
            //{
            //    var parent = (FormParent as SR_ServiceRequest);
            //    parent.productCode = String.Empty;
            //    parent.stockLocation = String.Empty;
            //}

            if (byLocation != null)
                byLocation.Clear();
		}

		private void dgItems_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			Function = "dgItems_Click";

            try
            {
                if (dgItems.CurrentRowIndex < 0)
                    return;

                dgItems.Select(dgItems.CurrentCell.RowNumber);

                Wait();

                string acctType = "H";
                if (!FormParent.IsDisposed && FormParent is NewAccount)
                {
                    var parent = (FormParent as NewAccount);
                    
                    parent.ProductCode = Convert.ToString(dgItems[dgItems.CurrentRowIndex, 0]);
                    //RM #8113 wrong column
                    parent.ItemID = Convert.ToInt32(dgItems[dgItems.CurrentRowIndex, 17]); //TODO : Replace the DataGrid with DataGridView then can access the columns by name
                    parent.Location = drpLocation.Text;
                    acctType = parent.AccountType;
                }
                //else if (!FormParent.IsDisposed && FormParent is SR_ServiceRequest)
                //{
                //    var parent = (FormParent as SR_ServiceRequest);
                //    parent.searchLocation = drpLocation.Text;
                //    parent.productCode = Convert.ToString(dgItems[dgItems.CurrentRowIndex, 0]);  //TODO need to pass in itemId
                //    parent.stockLocation = drpLocation.Text;
                    
                //    //parent.StockLocationChanged();
                //}
                //else if (!FormParent.IsDisposed && FormParent is InstManagement)  //IP - 15/02/11 - Sprint 5.10 - #3177
                //{
                //    var parent = (FormParent as InstManagement);
                //    var itemNo = Convert.ToString(dgItems[dgItems.CurrentRowIndex, 0]);
                //    var itemId = Convert.ToInt32(dgItems[dgItems.CurrentRowIndex, 17]); //RM #8113 wrong column
                //    var stockLocation = drpLocation.Text.TryParseInt16();

                //    parent.UpdateCourtsPart(itemNo, itemId, stockLocation);
                //}

                XmlNode currentItem = AccountManager.GetItemDetails(new GetItemDetailsRequest
                {
                    ProductCode = (string)dgItems[dgItems.CurrentRowIndex, 0],
                    StockLocationNo = drpLocation.Text.TryParseInt16(0).Value,
                    BranchCode = Config.BranchCode.TryParseInt16(0).Value,
                    AccountType = acctType,
                    CountryCode = Config.CountryCode,
                    PromoBranch = Convert.ToInt16(Config.BranchCode),
                    //RM #8113 wrong column
                    ItemID = Convert.ToInt32(dgItems[dgItems.CurrentRowIndex, 17] )     //CR1212 jec 21/04/11
                }, out Error);

                if (Error.Length > 0)
                    ShowError(Error);
                else
                {
                    txtProductCode.Text = currentItem.Attributes[Tags.Code].Value;
                    txtProdDesc1.Text = currentItem.Attributes[Tags.Description1].Value;
                    txtProdDesc2.Text = currentItem.Attributes[Tags.Description2].Value;
                    txtSupplierCode.Text = currentItem.Attributes[Tags.SupplierCode].Value;
                    //txtColour.Text = currentItem.Attributes[Tags.ColourName].Value;    //CR1212 jec 21/04/11  
                    txtColour.Text = Convert.ToString(((DataTable)((DataSet)dgItems.DataSource).Tables["ByLocation"]).Rows[dgItems.CurrentRowIndex]["Colour"]);     //IP - 27/01/11 - RI
                    //txtStyle.Text = currentItem.Attributes[Tags.Style].Value;          //CR1212 jec 21/04/11
                    //txtStyle.Text = Convert.ToString(((DataTable)((DataSet)dgItems.DataSource).Tables["ByLocation"]).Rows[dgItems.CurrentRowIndex]["VendorLongStyle"]) == string.Empty ? Convert.ToString(((DataTable)((DataSet)dgItems.DataSource).Tables["ByLocation"]).Rows[dgItems.CurrentRowIndex]["VendorStyle"]) : Convert.ToString(((DataTable)((DataSet)dgItems.DataSource).Tables["ByLocation"]).Rows[dgItems.CurrentRowIndex]["VendorLongStyle"]);
                    txtStyle.Text = Convert.ToString(((DataTable)((DataSet)dgItems.DataSource).Tables["ByLocation"]).Rows[dgItems.CurrentRowIndex]["Model"]); //IP - 30/08/11 - RI - #4623 - Always display vendor style long
                    //label7.Text = Convert.ToString(((DataTable)((DataSet)dgItems.DataSource).Tables["ByLocation"]).Rows[dgItems.CurrentRowIndex]["VendorLongStyle"]) == string.Empty ? "Style" : "Long Style";               //IP - 27/07/11 - RI
                    //label7.Text = "Long Style";               //IP - 230/08/11 - RI - #4623 - Always display Long Style
                    tvUnitPrice.Nodes.Clear();
                    
                    TreeNode node = new TreeNode("Unit Price");
                    node.Nodes.Add("Cash: " + ToDecimalString(currentItem.Attributes[Tags.CashPrice].Value));
                    node.Nodes.Add("Regular: " + ToDecimalString(currentItem.Attributes[Tags.HPPrice].Value));
                    node.Nodes.Add("Duty Free: " + ToDecimalString(currentItem.Attributes[Tags.DutyFreePrice].Value));
                    tvUnitPrice.Nodes.Add(node);
                    tvUnitPrice.ExpandAll();
                }
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
            finally
            {
                StopWait();
            }
		}

		public override bool ConfirmClose()
		{
			if(FormParent != null && FormParent is NewAccount)
			{
                var parent = (FormParent as NewAccount);

                string loc = parent.Location;
                parent.ClearItemDetails();
                parent.Location = loc;
                parent.drpLocation_Validating(this, new CancelEventArgs());
			}

			return true;
		}

		private void btnEnter_Click(object sender, System.EventArgs e)
		{
			CloseTab();
		}
	}
}
