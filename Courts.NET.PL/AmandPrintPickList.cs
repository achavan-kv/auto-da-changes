using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using STL.Common;
using System.Data;
using System.Web.Services.Protocols;
using STL.PL.WS2;
using STL.Common.Constants.AccountTypes;
using STL.Common.Constants.ColumnNames;
using STL.Common.Constants.TableNames;
using System.Collections.Specialized;
using System.Xml;
using System.Text;
using STL.Common.Constants.Elements;
using STL.Common.Constants.Tags;
using Crownwood.Magic.Menus;
using STL.Common.Constants.ItemTypes;
using STL.Common.Static;
using STL.Common.Constants.StoreInfo;

//	jec 68107	check for null branch numbers

namespace STL.PL
{
	/// <summary>
	/// Summary description for AmendPrintPickList.
	/// </summary>
	public class AmandPrintPickList : CommonForm
	{
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.DataGrid dgPicklistDetails;
		private System.Windows.Forms.DataGrid dgOrderDetails;
		private System.Windows.Forms.NumericUpDown numDelNoteNo;
		private System.Windows.Forms.ComboBox drpPickList;
		private short branchcode = Convert.ToInt16( Config.BranchCode);
		private string errorTxt;
		private System.Windows.Forms.Button btnReprint;
		private System.Windows.Forms.Button btnAdditions;
		private System.Windows.Forms.Button btnRemoveItem;
		private System.Windows.Forms.Button btnRemovePicklist;
		private System.Windows.Forms.Button btnAddItem;
		private System.Windows.Forms.ComboBox drpBranchNo;
		private bool disableAdd = false;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Button btnLoadDelOrder;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Button btnLoadTransPickList;
		private System.Windows.Forms.ComboBox drpTransPickList;
		bool orderPicklist = true;
		short delNoteBranch = 0;
		private System.Windows.Forms.ErrorProvider errorProvider1;
        private System.Windows.Forms.Button btnLoadPickList;
        private IContainer components;

		public AmandPrintPickList(Form root, Form parent)
		{
			InitializeData(root,parent);
		}
		public AmandPrintPickList(Form root, Form parent, string pickListNo, int branchNo)
		{
			InitializeData(root,parent);
			drpPickList.Text = pickListNo;
			drpBranchNo.Text = branchNo.ToString();
			loadPicklistDetails (branchNo, Convert.ToInt32(pickListNo),0);

		}
		private void InitializeData (Form root, Form parent)
		{
			InitializeComponent();

			FormRoot = root;
			FormParent = parent;
			LoadStaticData();
            loadPickListdropdown(Convert.ToInt16(Config.BranchCode));
            LoadTransPickLists(Convert.ToInt16(Config.BranchCode));
			dynamicMenus = new Hashtable();
			HashMenus();
			ApplyRoleRestrictions();
			disableButtons();
		}
		private void disableButtons()
		{
			btnAddItem.Enabled = false;
			btnAdditions.Enabled = false;
			btnRemoveItem.Enabled = false;
			btnRemovePicklist.Enabled = false;
			btnReprint.Enabled = false;
		}
		private void enableButtons(bool add, bool remove, bool print)
		{
			
			if(remove)
			{
				btnRemoveItem.Enabled = true;
				btnRemovePicklist.Enabled = true;
			}
			btnReprint.Enabled = true;
			btnAdditions.Enabled = true;
		}
		private void LoadStaticData()
		{
			XmlUtilities xml = new XmlUtilities();
			XmlDocument dropDowns = new XmlDocument();
			dropDowns.LoadXml("<DROP_DOWNS></DROP_DOWNS>");
				
			if(StaticData.Tables[TN.BranchNumber]==null)
				dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns,TN.BranchNumber, null));
				
			if (dropDowns.DocumentElement.ChildNodes.Count > 0)
			{
				DataSet ds = StaticDataManager.GetDropDownData(dropDowns.DocumentElement, out errorTxt);
				if (errorTxt.Length > 0)
					ShowError(errorTxt);
				else
				{
					foreach (DataTable dt in ds.Tables)
					{
						StaticData.Tables[dt.TableName] = dt;
					}
				}
			}

			StringCollection branchNos = new StringCollection(); 	
			foreach(DataRow row in ((DataTable)StaticData.Tables[TN.BranchNumber]).Rows)
			{
				branchNos.Add(Convert.ToString(row["branchno"]));
			}

			drpBranchNo.DataSource = branchNos;
			drpBranchNo.Text = Config.BranchCode;
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
		private void HashMenus()
		{
			dynamicMenus[this.Name+":btnReprint"] = this.btnReprint; 
		}

        private void loadPickListdropdown(short branch)
		{
			try
			{
				Function = "AmandPrintPickList::loadPickListdropdown";
                drpPickList.DataSource = null;
							
				StringCollection pickList = new StringCollection();
				char pickListScreen =  'P';
                DataSet ds = AccountManager.LoadAvailablePicklists(branch, pickListScreen, out errorTxt);
				if (errorTxt.Length > 0)
					ShowError(errorTxt);
				else
				{
					foreach(DataRow row in ((DataTable)ds.Tables[TN.Deliveries]).Rows)
					{
						string str = row.ItemArray[0].ToString();
						pickList.Add(str.ToUpper());
					}
					drpPickList.DataSource = pickList;
				}


			}
			catch(Exception ex)
			{
				Catch(ex, Function);
			}
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AmandPrintPickList));
            this.dgPicklistDetails = new System.Windows.Forms.DataGrid();
            this.dgOrderDetails = new System.Windows.Forms.DataGrid();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.btnAdditions = new System.Windows.Forms.Button();
            this.btnRemoveItem = new System.Windows.Forms.Button();
            this.btnRemovePicklist = new System.Windows.Forms.Button();
            this.btnAddItem = new System.Windows.Forms.Button();
            this.btnReprint = new System.Windows.Forms.Button();
            this.numDelNoteNo = new System.Windows.Forms.NumericUpDown();
            this.drpPickList = new System.Windows.Forms.ComboBox();
            this.drpBranchNo = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.drpTransPickList = new System.Windows.Forms.ComboBox();
            this.btnLoadTransPickList = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.btnLoadDelOrder = new System.Windows.Forms.Button();
            this.btnLoadPickList = new System.Windows.Forms.Button();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.dgPicklistDetails)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgOrderDetails)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numDelNoteNo)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // dgPicklistDetails
            // 
            this.dgPicklistDetails.CaptionText = "PickList Details";
            this.dgPicklistDetails.DataMember = "";
            this.dgPicklistDetails.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgPicklistDetails.Location = new System.Drawing.Point(24, 304);
            this.dgPicklistDetails.Name = "dgPicklistDetails";
            this.dgPicklistDetails.ReadOnly = true;
            this.dgPicklistDetails.Size = new System.Drawing.Size(744, 160);
            this.dgPicklistDetails.TabIndex = 10;
            // 
            // dgOrderDetails
            // 
            this.dgOrderDetails.CaptionText = "Order Detail";
            this.dgOrderDetails.DataMember = "";
            this.dgOrderDetails.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgOrderDetails.Location = new System.Drawing.Point(24, 160);
            this.dgOrderDetails.Name = "dgOrderDetails";
            this.dgOrderDetails.ReadOnly = true;
            this.dgOrderDetails.Size = new System.Drawing.Size(656, 136);
            this.dgOrderDetails.TabIndex = 11;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(110, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(40, 16);
            this.label1.TabIndex = 20;
            this.label1.Text = "Branch";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(64, 58);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(88, 16);
            this.label2.TabIndex = 21;
            this.label2.Text = "PickList Number";
            this.label2.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(336, 58);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(136, 16);
            this.label3.TabIndex = 22;
            this.label3.Text = "Delivery/Collection Note # ";
            this.label3.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // btnAdditions
            // 
            this.btnAdditions.Location = new System.Drawing.Point(696, 24);
            this.btnAdditions.Name = "btnAdditions";
            this.btnAdditions.Size = new System.Drawing.Size(80, 32);
            this.btnAdditions.TabIndex = 23;
            this.btnAdditions.Text = "Print Amendments";
            this.btnAdditions.Click += new System.EventHandler(this.btnAdditions_Click);
            // 
            // btnRemoveItem
            // 
            this.btnRemoveItem.Location = new System.Drawing.Point(696, 184);
            this.btnRemoveItem.Name = "btnRemoveItem";
            this.btnRemoveItem.Size = new System.Drawing.Size(80, 48);
            this.btnRemoveItem.TabIndex = 24;
            this.btnRemoveItem.Text = "Remove Items  from Picklist";
            this.btnRemoveItem.Click += new System.EventHandler(this.btnRemoveItem_Click);
            // 
            // btnRemovePicklist
            // 
            this.btnRemovePicklist.Location = new System.Drawing.Point(696, 144);
            this.btnRemovePicklist.Name = "btnRemovePicklist";
            this.btnRemovePicklist.Size = new System.Drawing.Size(80, 32);
            this.btnRemovePicklist.TabIndex = 25;
            this.btnRemovePicklist.Text = "Remove Picklist";
            this.btnRemovePicklist.Click += new System.EventHandler(this.btnRemovePicklist_Click);
            // 
            // btnAddItem
            // 
            this.btnAddItem.Enabled = false;
            this.btnAddItem.Location = new System.Drawing.Point(696, 104);
            this.btnAddItem.Name = "btnAddItem";
            this.btnAddItem.Size = new System.Drawing.Size(80, 32);
            this.btnAddItem.TabIndex = 26;
            this.btnAddItem.Text = "Add Item to Picklist";
            this.btnAddItem.Click += new System.EventHandler(this.btnAddItem_Click);
            // 
            // btnReprint
            // 
            this.btnReprint.Enabled = false;
            this.btnReprint.Location = new System.Drawing.Point(696, 64);
            this.btnReprint.Name = "btnReprint";
            this.btnReprint.Size = new System.Drawing.Size(80, 32);
            this.btnReprint.TabIndex = 27;
            this.btnReprint.Text = "Re-Print Picklist";
            this.btnReprint.Visible = false;
            this.btnReprint.Click += new System.EventHandler(this.btnReprint_Click);
            // 
            // numDelNoteNo
            // 
            this.numDelNoteNo.Location = new System.Drawing.Point(480, 56);
            this.numDelNoteNo.Maximum = new decimal(new int[] {
            -727379969,
            232,
            0,
            0});
            this.numDelNoteNo.Name = "numDelNoteNo";
            this.numDelNoteNo.Size = new System.Drawing.Size(104, 20);
            this.numDelNoteNo.TabIndex = 18;
            // 
            // drpPickList
            // 
            this.drpPickList.Location = new System.Drawing.Point(160, 56);
            this.drpPickList.Name = "drpPickList";
            this.drpPickList.Size = new System.Drawing.Size(104, 21);
            this.drpPickList.TabIndex = 28;
            // 
            // drpBranchNo
            // 
            this.drpBranchNo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpBranchNo.Location = new System.Drawing.Point(160, 16);
            this.drpBranchNo.Name = "drpBranchNo";
            this.drpBranchNo.Size = new System.Drawing.Size(104, 21);
            this.drpBranchNo.TabIndex = 35;
            this.drpBranchNo.SelectedIndexChanged += new System.EventHandler(this.drpBranchNo_SelectedIndexChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.drpTransPickList);
            this.groupBox1.Controls.Add(this.btnLoadTransPickList);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.btnLoadDelOrder);
            this.groupBox1.Controls.Add(this.btnLoadPickList);
            this.groupBox1.Controls.Add(this.drpPickList);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.drpBranchNo);
            this.groupBox1.Controls.Add(this.numDelNoteNo);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(24, 8);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(656, 144);
            this.groupBox1.TabIndex = 36;
            this.groupBox1.TabStop = false;
            // 
            // drpTransPickList
            // 
            this.drpTransPickList.Location = new System.Drawing.Point(160, 104);
            this.drpTransPickList.Name = "drpTransPickList";
            this.drpTransPickList.Size = new System.Drawing.Size(104, 21);
            this.drpTransPickList.TabIndex = 44;
            // 
            // btnLoadTransPickList
            // 
            this.btnLoadTransPickList.Image = ((System.Drawing.Image)(resources.GetObject("btnLoadTransPickList.Image")));
            this.btnLoadTransPickList.Location = new System.Drawing.Point(280, 96);
            this.btnLoadTransPickList.Name = "btnLoadTransPickList";
            this.btnLoadTransPickList.Size = new System.Drawing.Size(32, 32);
            this.btnLoadTransPickList.TabIndex = 43;
            this.btnLoadTransPickList.Click += new System.EventHandler(this.btnLoadTransPickList_Click);
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(8, 106);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(144, 16);
            this.label5.TabIndex = 39;
            this.label5.Text = "Transport PickList Number";
            this.label5.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // btnLoadDelOrder
            // 
            this.btnLoadDelOrder.Image = ((System.Drawing.Image)(resources.GetObject("btnLoadDelOrder.Image")));
            this.btnLoadDelOrder.Location = new System.Drawing.Point(600, 48);
            this.btnLoadDelOrder.Name = "btnLoadDelOrder";
            this.btnLoadDelOrder.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.btnLoadDelOrder.Size = new System.Drawing.Size(32, 32);
            this.btnLoadDelOrder.TabIndex = 37;
            this.btnLoadDelOrder.Click += new System.EventHandler(this.btnLoadDelOrder_Click);
            // 
            // btnLoadPickList
            // 
            this.btnLoadPickList.Image = ((System.Drawing.Image)(resources.GetObject("btnLoadPickList.Image")));
            this.btnLoadPickList.Location = new System.Drawing.Point(280, 48);
            this.btnLoadPickList.Name = "btnLoadPickList";
            this.btnLoadPickList.Size = new System.Drawing.Size(32, 32);
            this.btnLoadPickList.TabIndex = 36;
            this.btnLoadPickList.Click += new System.EventHandler(this.btnLoadPickList_Click);
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // AmandPrintPickList
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(792, 470);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnReprint);
            this.Controls.Add(this.btnAddItem);
            this.Controls.Add(this.btnRemovePicklist);
            this.Controls.Add(this.btnRemoveItem);
            this.Controls.Add(this.btnAdditions);
            this.Controls.Add(this.dgOrderDetails);
            this.Controls.Add(this.dgPicklistDetails);
            this.Name = "AmandPrintPickList";
            this.Text = "Amend / Print Picklist";
            ((System.ComponentModel.ISupportInitialize)(this.dgPicklistDetails)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgOrderDetails)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numDelNoteNo)).EndInit();
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);

		}
		#endregion

		private void loadPicklistDetails (int filter, int pickListNo, int buffNo)
		{
			disableButtons();
			dgPicklistDetails.DataSource = null;
			dgPicklistDetails.TableStyles.Clear();
			DataSet ds = AccountManager.GetPickListSchedule(filter, pickListNo,buffNo, out errorTxt );
			DataView dvPicklist = ds.Tables[TN.Schedules].DefaultView;
			int loadNo = 0;

			dgPicklistDetails.DataSource = dvPicklist;

			DataGridTableStyle tabStyle = new DataGridTableStyle();
			tabStyle.MappingName = dvPicklist.Table.TableName;
			dgPicklistDetails.TableStyles.Add(tabStyle);


            tabStyle.GridColumnStyles[CN.BuffBranchNo].Width = 0;
            tabStyle.GridColumnStyles[CN.DelNoteBranch].Width = 90;
            tabStyle.GridColumnStyles[CN.DelNoteBranch].HeaderText = GetResource("T_DELNOTEBRANCH");
            tabStyle.GridColumnStyles[CN.BuffNo].Width = 100;
			tabStyle.GridColumnStyles[CN.BuffNo].HeaderText = GetResource("T_DELNOTENUMBER");			
			tabStyle.GridColumnStyles[CN.acctno].Width = 140;
			tabStyle.GridColumnStyles[CN.acctno].HeaderText = GetResource("T_ACCTNO");
			tabStyle.GridColumnStyles[CN.AgrmtNo].Width = 0;
			tabStyle.GridColumnStyles[CN.ItemNo].Width = 90;
			tabStyle.GridColumnStyles[CN.ItemNo].HeaderText = GetResource("T_ITEMNO");
			tabStyle.GridColumnStyles[CN.Quantity].Width = 0;
			tabStyle.GridColumnStyles[CN.StockLocn].Width = 100;
			tabStyle.GridColumnStyles[CN.StockLocn].HeaderText = GetResource("T_STOCKLOCN");
			tabStyle.GridColumnStyles[CN.DateDelPlan].Width = 100;
			tabStyle.GridColumnStyles[CN.DateDelPlan].HeaderText = GetResource("T_DATEDELPLAN");
			tabStyle.GridColumnStyles[CN.LoadNo].Width = 90;
			tabStyle.GridColumnStyles[CN.LoadNo].HeaderText = GetResource("T_LOADNO");
            tabStyle.GridColumnStyles[CN.ItemId].Width = 0;

			tabStyle.GridColumnStyles[CN.PicklistNumber].Width = 0;

			bool print =  false;
			bool add = false;
			bool remove =  false;
			if (dvPicklist.Count > 0)
			{
				loadNo  = Convert.ToInt32(((DataView)dgPicklistDetails.DataSource)[0][CN.LoadNo].ToString());
				print = true;
				if (loadNo == 0)
				{
					remove= true;
				}

			}
			if (loadNo == 0 && disableAdd == false)
			{
				add = true;
			}
			enableButtons(add,remove,print);

		}
		private void loadDeliveryNoteDetails(int stockLocn, int buffNo)
		{
			dgOrderDetails.DataSource = null;
			dgOrderDetails.TableStyles.Clear();
			DataSet ds = AccountManager.Schedule_GetByBuffNo(stockLocn, buffNo, out errorTxt );
			DataView dvOrderDetails = ds.Tables[TN.Schedules].DefaultView;
            
			
			dgOrderDetails.DataSource = dvOrderDetails;
            
			DataGridTableStyle tabStyle = new DataGridTableStyle();
			tabStyle.MappingName = dvOrderDetails.Table.TableName;
			dgOrderDetails.TableStyles.Add(tabStyle);


			tabStyle.GridColumnStyles[CN.DelNoteBranch].Width = 90;
            tabStyle.GridColumnStyles[CN.DelNoteBranch].HeaderText = GetResource("T_DELNOTEBRANCH");
			tabStyle.GridColumnStyles[CN.BuffNo].Width = 60;
			tabStyle.GridColumnStyles[CN.BuffNo].HeaderText = GetResource("T_DELNOTENUMBER");			
			tabStyle.GridColumnStyles[CN.acctno].Width = 100;
			tabStyle.GridColumnStyles[CN.acctno].HeaderText = GetResource("T_ACCTNO");
			tabStyle.GridColumnStyles[CN.AgrmtNo].Width = 0;
			tabStyle.GridColumnStyles[CN.DelOrColl].Width = 0;
			tabStyle.GridColumnStyles[CN.ItemNo].Width = 100;
			tabStyle.GridColumnStyles[CN.ItemNo].HeaderText = GetResource("T_ITEMNO");
			tabStyle.GridColumnStyles[CN.Quantity].Width = 0;
			tabStyle.GridColumnStyles[CN.StockLocn].Width = 100;
			tabStyle.GridColumnStyles[CN.StockLocn].HeaderText = GetResource("T_STOCKLOCN");
			tabStyle.GridColumnStyles[CN.DateDelPlan].Width = 100;
			tabStyle.GridColumnStyles[CN.DateDelPlan].HeaderText = GetResource("T_DATEDELPLAN");
			tabStyle.GridColumnStyles[CN.RetStockLocn].Width = 0;
			tabStyle.GridColumnStyles[CN.RetItemNo].Width = 0;
			tabStyle.GridColumnStyles[CN.RetVal].Width = 0;
			tabStyle.GridColumnStyles[CN.VanNo].Width = 0;
			tabStyle.GridColumnStyles[CN.LoadNo].Width = 60;
			tabStyle.GridColumnStyles[CN.LoadNo].HeaderText = GetResource("T_LOADNO");
			tabStyle.GridColumnStyles[CN.ItemType].Width = 0;
			tabStyle.GridColumnStyles[CN.DelType].Width = 0;
			tabStyle.GridColumnStyles[CN.PicklistNumber].Width = 0;
            tabStyle.GridColumnStyles[CN.BuffBranchNo].Width = 0;

			int loadNo = 0;
			int picklistNo = 0;

			if (dvOrderDetails.Count >  0)
			{
				loadNo  = Convert.ToInt32(((DataView)dgOrderDetails.DataSource)[0][CN.LoadNo].ToString());
				if ((string)((DataView)dgOrderDetails.DataSource)[0][CN.PicklistNumber].ToString() !="0")
				{ //load details of picklist from this order if picklist number not 0.
					drpPickList.Text = (string)((DataView)dgOrderDetails.DataSource)[0][CN.PicklistNumber].ToString();
					picklistNo = Convert.ToInt32( (string)((DataView)dgOrderDetails.DataSource)[0][CN.PicklistNumber].ToString());
					loadPicklistDetails (stockLocn, picklistNo, buffNo); //reload details of picklist this item is on.
				}

				if (loadNo > 0 ) //already on delivery schedule so cannot be added to picklist
				{
					disableAdd = true;
				}
				else 
					disableAdd= false;
			}

            //IP - 30/11/2007 - UAT(224)
            bool PickListHasRows = false;
            DataView dvPickList = dgPicklistDetails.DataSource as DataView;
            //Set 'PickListHasRows' to true if rows are returned in the 'dgPicklistDetails' datagrid.
            if (dvPickList != null)
            {
                PickListHasRows = dvPickList.Count > 0;
            }

			if (ds.Tables[TN.Schedules].Rows.Count >0 && disableAdd==false  && PickListHasRows) //add means that this picklist is not already printed and so can be added to
			{
				btnAddItem.Enabled=true;
			}
			else
			{
				btnAddItem.Enabled=false;
			}

		}

		private void btnAdditions_Click(object sender, System.EventArgs e)
		{
         //Create a string that contains the courts branch numbers CR903
         StringBuilder sb = new StringBuilder();
         foreach (DataRow row in ((DataTable)StaticData.Tables[TN.BranchNumber]).Rows)
         {
            if (row[CN.StoreType].ToString() == StoreType.Courts)
            {
               sb.Append(row[CN.BranchNo].ToString());
               sb.Append("|");
            }
         }

			int picklistNo = 0;
			if(orderPicklist)
				picklistNo = Convert.ToInt32(drpPickList.Text);
			else
				picklistNo = Convert.ToInt32(drpTransPickList.Text);

			PrintPickList(picklistNo, true, false, Convert.ToInt16(drpBranchNo.Text), delNoteBranch, orderPicklist,sb.ToString());
		}

		private void btnReprint_Click(object sender, System.EventArgs e)
		{
         //Create a string that contains the courts branch numbers CR903
         StringBuilder sb = new StringBuilder();
         foreach (DataRow row in ((DataTable)StaticData.Tables[TN.BranchNumber]).Rows)
         {
            if (row[CN.StoreType].ToString() == StoreType.Courts)
            {
               sb.Append(row[CN.BranchNo].ToString());
               sb.Append("|");
            }
         }

			int picklistNo = 0;
			if(orderPicklist)
				picklistNo = Convert.ToInt32(drpPickList.Text);
			else
				picklistNo = Convert.ToInt32(drpTransPickList.Text);

		    PrintPickList(picklistNo, false, true, Convert.ToInt16(drpBranchNo.Text), delNoteBranch, orderPicklist,sb.ToString());
			
			/* AA - was hoping to clear these details after printing the picklist
			 * but it seems that this lot was clearing details from the picklist in any case
			 
			  dgPicklistDetails.DataSource = null;
			dgPicklistDetails.TableStyles.Clear();
			dgOrderDetails.DataSource = null;
			dgOrderDetails.TableStyles.Clear();*/

		
		}

		private void btnAddItem_Click(object sender, System.EventArgs e)
		{
            try
            {
                Wait();

                string picklistNo = drpPickList.Text;
                string picklistBranchNo = Convert.ToInt32(drpBranchNo.Text).ToString();

                DataTable updatePick = setupDataTable();
                DataView DNItems = (DataView)dgOrderDetails.DataSource;
                DataSet ds = new DataSet();

                if (DNItems.Count > 0)
                {
                    string accountNo = (string)((DataView)dgOrderDetails.DataSource)[0][CN.acctno];
                    string buffBranchNo = (string)((DataView)dgOrderDetails.DataSource)[0][CN.BuffBranchNo].ToString();
                    string buffNo = (string)((DataView)dgOrderDetails.DataSource)[0][CN.BuffNo].ToString();
                    string stockLocn = (string)((DataView)dgOrderDetails.DataSource)[0][CN.StockLocn].ToString();
                    string retStockLocn = (string)((DataView)dgOrderDetails.DataSource)[0][CN.RetStockLocn].ToString();

                    DataRow row = updatePick.NewRow();
                    row[CN.acctno] = accountNo;
                    row[CN.BuffBranchNo] = buffBranchNo;
                    row[CN.StockLocn] = Convert.ToInt16(retStockLocn) > 0 ? retStockLocn : stockLocn;
                    row[CN.BuffNo] = buffNo;
                    row[CN.ItemId] = "0";
                    row[CN.PicklistNumber] = picklistNo;
                    row["PicklistBranchNumber"] = picklistBranchNo;
                    row[CN.Released] = true;

                    updatePick.Rows.Add(row);

                    ds.Tables.Add(updatePick);
                    AccountManager.UpdateScheduleForPicklist(ds, Convert.ToInt32(picklistBranchNo), "O", out errorTxt); 

                    if (errorTxt.Length > 0)
                        ShowError(errorTxt);
                    else
                    {
                        loadPicklistDetails(Convert.ToInt32(picklistBranchNo), Convert.ToInt32(drpPickList.Text), Convert.ToInt32(0));
                        btnAddItem.Enabled = false;
                        dgOrderDetails.TableStyles.Clear();
                        dgOrderDetails.DataSource = null;
                        numDelNoteNo.Value = 0;
                    }
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

		private void btnRemovePicklist_Click(object sender, System.EventArgs e)
		{
			string picklistNo =  "0";
			string picklistBranchNo = "0";
			DataTable updatePick = setupDataTable();
			//DataView DNItems = (DataView)dgOrderDetails.DataSource;
			DataSet ds = new DataSet();
			string accountNo = "";
			string buffBranchNo = "";
			string buffNo = "";
			string itemId = "";
			string stockLocn = "";

			foreach(DataRowView addedItems in (DataView)dgPicklistDetails.DataSource)
			{
				accountNo = (string)addedItems [CN.acctno];
				buffBranchNo =  (string)addedItems[CN.BuffBranchNo].ToString();
				buffNo = (string)addedItems[CN.BuffNo].ToString();
				itemId =  (string)addedItems[CN.ItemId].ToString();
				stockLocn = (string)addedItems[CN.StockLocn].ToString();
				DataRow row = updatePick.NewRow();
				row[CN.acctno] = accountNo;
				row[CN.BuffBranchNo] = buffBranchNo;
				row[CN.StockLocn] = stockLocn;
				row[CN.BuffNo] = buffNo;
				row[CN.ItemId] = itemId;
				row[CN.PicklistNumber] = picklistNo;
				row["PicklistBranchNumber"] = picklistBranchNo;
				row[CN.Released] = true;
					
				updatePick.Rows.Add(row);
			}
			ds.Tables.Add(updatePick);
			AccountManager.UpdateScheduleForPicklist (ds,Convert.ToInt32(picklistBranchNo), "O", out errorTxt);
			loadPicklistDetails(Convert.ToInt32(stockLocn), Convert.ToInt32(drpPickList.Text), Convert.ToInt32(0));				
		
		}


		private void btnRemoveItem_Click(object sender, System.EventArgs e)
		{
			string picklistNo =  "0";
			string buffBranchNo="0";
			string picklistBranchNo = "0";
			DataTable updatePick =setupDataTable();
			DataView DNItems = (DataView)dgPicklistDetails.DataSource;
			DataSet ds = new DataSet();
			if (DNItems.Count > 0)
            {
			    int count = DNItems.Count;
					          
				for (int i = count-1; i >=0 ; i--)
				{
					                                         
					if (dgPicklistDetails.IsSelected(i))
					{
						string accountNo = (string)DNItems[i][CN.acctno];
						buffBranchNo =  (string)DNItems[i][CN.BuffBranchNo].ToString();
						string buffNo = (string)DNItems[i][CN.BuffNo].ToString();
						string stockLocn = (string)DNItems[i][CN.StockLocn].ToString();
						
                        DataRow row = updatePick.NewRow();
						row[CN.acctno] = accountNo;
						row[CN.BuffBranchNo] = buffBranchNo;
						row[CN.StockLocn] = stockLocn;
						row[CN.BuffNo] = buffNo;
						row[CN.ItemId] = "0";
						row[CN.PicklistNumber] = picklistNo;
						row["PicklistBranchNumber"] = picklistBranchNo;
						row[CN.Released] = true;
							
						updatePick.Rows.Add(row);
					}   
				
			    }
			    ds.Tables.Add(updatePick);
			    AccountManager.UpdateScheduleForPicklist (ds,Convert.ToInt32( picklistBranchNo), "O", out errorTxt);
				loadPicklistDetails(Convert.ToInt32(drpBranchNo.Text), Convert.ToInt32(drpPickList.Text), Convert.ToInt32(0));				
			}  
		
		}
		private DataTable setupDataTable()
		{
			DataTable dt =new DataTable(TN.Schedules);
			dt.Columns.AddRange(new DataColumn[]{new DataColumn(CN.acctno), 
													new DataColumn(CN.BuffBranchNo),
													new DataColumn(CN.StockLocn), 
													new DataColumn(CN.BuffNo),
													new DataColumn(CN.ItemId),
													new DataColumn(CN.PicklistNumber),
													new DataColumn("PicklistBranchNumber"),
													new DataColumn(CN.Released)});
			return dt;
		}

		private void btnLoadPickList_Click(object sender, System.EventArgs e)
		{
			int branchNo = Convert.ToInt32(drpBranchNo.Text);

			if(drpPickList.Text==""  || !IsStrictNumeric(drpPickList.Text))	//jec 68107
				errorProvider1.SetError(drpPickList, GetResource("M_NUMERIC"));
			else
			{
				errorProvider1.SetError(drpPickList, "");
				int pickListNo = Convert.ToInt32( drpPickList.Text);
				loadPicklistDetails(branchNo, pickListNo, 0);
				orderPicklist = true;
			}

		}

		private void btnLoadDelOrder_Click(object sender, System.EventArgs e)
		{
			int branchNo = Convert.ToInt32(drpBranchNo.Text);
			int buffNo = Convert.ToInt32(numDelNoteNo.Value);
			loadDeliveryNoteDetails(branchNo, buffNo);
			orderPicklist = true;
			//loadPicklistDetails(branchNo, 0, buffNo);
		}

		private void LoadTransPickLists(short branch)
		{
			try
			{
				Function = "AmendPrintPickList::LoadTransPickLists";
                drpTransPickList.DataSource = null;
							
				StringCollection pickList = new StringCollection();

                DataSet ds = AccountManager.LoadAvailableTransPicklists(branch, out errorTxt);
				if (errorTxt.Length > 0)
					ShowError(errorTxt);
				else
				{
					foreach(DataRow row in ((DataTable)ds.Tables[TN.Deliveries]).Rows)
					{
						string str = row.ItemArray[0].ToString();
						pickList.Add(str.ToUpper());
					}
					drpTransPickList.DataSource = pickList;
				}
			}
			catch(Exception ex)
			{
				Catch(ex, Function);
			}
		}

		private void btnLoadTransPickList_Click(object sender, System.EventArgs e)
		{
			try
			{
				Wait();
				if(drpTransPickList.Text==""  || !IsStrictNumeric(drpTransPickList.Text)) //jec 68107
					errorProvider1.SetError(drpTransPickList, GetResource("M_NUMERIC"));
				else
				{
					errorProvider1.SetError(drpTransPickList, "");
					short branchNo = Convert.ToInt16(drpBranchNo.Text);
					int pickListNo = Convert.ToInt32(drpTransPickList.Text);
			
					disableButtons();
					dgPicklistDetails.DataSource = null;
					dgPicklistDetails.TableStyles.Clear();
				
					DataSet ds = AccountManager.GetTransPickListDetails(branchNo, pickListNo, out errorTxt );
					DataView dvPicklist = ds.Tables[TN.Schedules].DefaultView;

					dgPicklistDetails.DataSource = dvPicklist;

					DataGridTableStyle tabStyle = new DataGridTableStyle();
					tabStyle.MappingName = dvPicklist.Table.TableName;
					dgPicklistDetails.TableStyles.Add(tabStyle);

					tabStyle.GridColumnStyles[CN.BuffBranchNo].Width = 80;
					tabStyle.GridColumnStyles[CN.BuffBranchNo].HeaderText = GetResource("T_DELNOTEBRANCH");
					tabStyle.GridColumnStyles[CN.BuffNo].Width = 100;
					tabStyle.GridColumnStyles[CN.BuffNo].HeaderText = GetResource("T_DELNOTENUMBER");			
					tabStyle.GridColumnStyles[CN.acctno].Width = 140;
					tabStyle.GridColumnStyles[CN.acctno].HeaderText = GetResource("T_ACCTNO");
					tabStyle.GridColumnStyles[CN.AgrmtNo].Width = 0;
					tabStyle.GridColumnStyles[CN.ItemNo].Width = 90;
					tabStyle.GridColumnStyles[CN.ItemNo].HeaderText = GetResource("T_ITEMNO");
					tabStyle.GridColumnStyles[CN.Quantity].Width = 0;
					tabStyle.GridColumnStyles[CN.StockLocn].Width = 100;
					tabStyle.GridColumnStyles[CN.StockLocn].HeaderText = GetResource("T_STOCKLOCN");
					tabStyle.GridColumnStyles[CN.DateDelPlan].Width = 100;
					tabStyle.GridColumnStyles[CN.DateDelPlan].HeaderText = GetResource("T_DATEDELPLAN");
					tabStyle.GridColumnStyles[CN.LoadNo].Width = 90;
					tabStyle.GridColumnStyles[CN.LoadNo].HeaderText = GetResource("T_LOADNO");

					tabStyle.GridColumnStyles[CN.PicklistNumber].Width = 0;
					tabStyle.GridColumnStyles[CN.DelNoteBranch].Width = 0;
                    tabStyle.GridColumnStyles[CN.ItemId].Width = 0;

					bool print = false;
					if(dvPicklist.Count > 0)
					{
						print = true;
						delNoteBranch = Convert.ToInt16(dvPicklist[0][CN.DelNoteBranch]);
					}
					enableButtons(false, false, print);
					orderPicklist = false;
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

        private void drpBranchNo_SelectedIndexChanged(object sender, EventArgs e)
        {
            loadPickListdropdown(Convert.ToInt16(drpBranchNo.Text));
            LoadTransPickLists(Convert.ToInt16(drpBranchNo.Text));
        }
	}
}
