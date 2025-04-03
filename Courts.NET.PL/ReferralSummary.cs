using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Xml;
using STL.Common.Constants.Tags;
using STL.Common.Constants.Elements;
using STL.Common.Constants.TableNames;
using STL.Common.Constants.ItemTypes;
using System.Threading;
using STL.Common.Constants.ColumnNames;

namespace STL.PL
{
	/// <summary>
	/// Common tab to show an account summary for the credit application,
	/// the agreement and the account history.  Used by the underwriter
	/// referral screen and the delivery authorisation screen.
	/// </summary>
	public class ReferralSummary : CommonForm
	{
		private Crownwood.Magic.Controls.TabControl tcSummary;
		private Crownwood.Magic.Controls.TabPage tpPersonalDetails;
		private System.Windows.Forms.Label label32;
		private System.Windows.Forms.TextBox txtOccupation;
		private System.Windows.Forms.Label label31;
		private System.Windows.Forms.TextBox txtExpenses;
		private STL.PL.DatePicker dtDateInCurrentEmp;
		private STL.PL.DatePicker dtDateInCurrentAddress;
		private System.Windows.Forms.Label label30;
		private System.Windows.Forms.TextBox txtResidentialStatus;
		private System.Windows.Forms.Label label29;
		private System.Windows.Forms.TextBox txtMaritalStatus;
		private System.Windows.Forms.Label label28;
		private System.Windows.Forms.TextBox txtIncomeAfterTax;
		private System.Windows.Forms.Label label25;
		private System.Windows.Forms.TextBox txtAge;
		private Crownwood.Magic.Controls.TabPage tpAgreement;
		private Crownwood.Magic.Controls.TabPage tpAccountHistory;
		private System.Windows.Forms.GroupBox gbLineItems;
		public System.Windows.Forms.DataGrid dgLineItems;
		private System.Windows.Forms.Splitter splitter1;
		public System.Windows.Forms.TreeView tvItems;
		private System.Windows.Forms.TextBox txtRepaymentPcent;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox txtTermsType;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label21;
		private System.Windows.Forms.Label label20;
		private System.Windows.Forms.TextBox txtFinalInstalment;
		private System.Windows.Forms.TextBox txtInstalment;
		private System.Windows.Forms.TextBox txtDeposit;
		private System.Windows.Forms.Label label18;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox txtAgreementTotal;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox txtAcctType;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox txtNoOfCurrent;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox txtNoOfSettled;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.TextBox txtWorstCurrentStatus;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.TextBox txtWorstSettledStatus;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.TextBox txtOutstandingBalance;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.TextBox txtTotalInstalmentValue;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.TextBox txtNoAccountsInArrears;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.TextBox txtLongestAgreement;
		private System.Windows.Forms.Label label13;
		private System.Windows.Forms.Label label14;
		private System.Windows.Forms.TextBox txtLargestAgreement;
		private System.Windows.Forms.Label label15;
		private System.Windows.Forms.TextBox txtFeesAndInterest;
		private System.Windows.Forms.Label label16;
		private System.Windows.Forms.TextBox txtCashAccountsSettled;
		private System.Windows.Forms.Label label17;
		private System.Windows.Forms.TextBox txtRepaymentPcentCurrent;
		private System.Windows.Forms.Label label19;
		private System.Windows.Forms.TextBox txtNoOfReturnedCheques;
		private System.Windows.Forms.Label label22;
		private System.Windows.Forms.TextBox txtValueOfAllArrears;
		private new string Error = "";
		XmlNode LineItems = null;
		XmlDocument itemDoc = null;
		private string AccountType = "";
		private DataTable itemsTable = null;
		private System.Windows.Forms.ImageList imageList1;
		private System.ComponentModel.IContainer components;
		private string AccountNo = "";
		private string CustomerID = "";
		private System.Windows.Forms.TextBox txtCreditLimit;
		private System.Windows.Forms.Label label23;
		private System.Windows.Forms.TextBox txtRFAccts;
		private System.Windows.Forms.Label label24;
		private System.Windows.Forms.Label label26;
		private System.Windows.Forms.TextBox txtSysRecommend;
        private Crownwood.Magic.Controls.TabPage tpUnderwriterDetails;
        private RichTextBox txtReferralNotes;
        private Label lblScore;
        private Label lblReferralNotes;
        private TextBox txtScore;
		private DateTime DateProp;

        UserRight viewUnderiterInfoTab = UserRight.Create("ViewUwInfoTab"); //IP - 15/03/11 - #3316 - CR1245


		public ReferralSummary(TranslationDummy d)
		{
			InitializeComponent();
		}

		public ReferralSummary()
		{
			InitializeComponent();
			SetBackColour(Controls);
            CheckUserRights(viewUnderiterInfoTab); //IP - 15/03/11 - #3316 - CR1245
		}


		public void LoadDetails(Form root, Form parent, string accountType, DataSet data, XmlNode lineItems)
		{	
			FormRoot = root;
			FormParent = parent;
			LineItems = lineItems;
			AccountType = accountType;
            Populate(data);
			BuildLineItems();

            //IP - 15/03/11 - #3316 - CR1245 - Do not display the tab if the user does not have the user right
            if (!viewUnderiterInfoTab.IsAllowed)
            {
                if (tcSummary.TabPages.Contains(tpUnderwriterDetails))
                {
                    tcSummary.TabPages.Remove(tpUnderwriterDetails);    
                }
            }
		}

		private void Populate(DataSet details)
		{
			foreach (DataRow row in details.Tables[TN.ReferralData].Rows)
			{
				/* Personal Details */
				txtAge.Text = Convert.ToString(row[CN.Age]);
				txtMaritalStatus.Text = (string)row[CN.MaritalStatus];
				txtResidentialStatus.Text = (string)row[CN.ResidentialStatus];
				txtOccupation.Text = (string)row[CN.Occupation];
				txtExpenses.Text = ((decimal)row[CN.Expenses]).ToString(DecimalPlaces);

				if((DateTime)row[CN.DateInAddress] > DatePicker.MinValue)
				{
					dtDateInCurrentAddress.DateFrom = DateTime.Today;
					dtDateInCurrentAddress.Value = (DateTime)row[CN.DateInAddress];
				}

				if((DateTime)row[CN.DateEmployed] > DatePicker.MinValue)
				{
					dtDateInCurrentEmp.DateFrom = DateTime.Today;
					dtDateInCurrentEmp.Value = (DateTime)row[CN.DateEmployed];
				}

				txtIncomeAfterTax.Text = ((decimal)row[CN.Income]).ToString(DecimalPlaces);

				/* Agreement Details */
				txtRepaymentPcent.Text = ((decimal)row[CN.RepaymentPcent]).ToString("F2") + "%";
				txtAcctType.Text = AccountType;
				txtTermsType.Text = (string)row[CN.TermsType];
				txtDeposit.Text = ((decimal)row[CN.Deposit]).ToString(DecimalPlaces);
				txtInstalment.Text = ((decimal)row[CN.MonthlyInstal]).ToString(DecimalPlaces);
				txtFinalInstalment.Text = ((decimal)row[CN.FinalInstal]).ToString(DecimalPlaces);
				txtAgreementTotal.Text = ((decimal)row[CN.AgreementTotal]).ToString(DecimalPlaces);

				/* Account History */
				txtValueOfAllArrears.Text = ((decimal)row[CN.ValueOfArrears]).ToString(DecimalPlaces);
				txtOutstandingBalance.Text = ((decimal)row[CN.OutstBal]).ToString(DecimalPlaces);
				txtLargestAgreement.Text = ((decimal)row[CN.LargestAgreement]).ToString(DecimalPlaces);
				txtFeesAndInterest.Text = ((decimal)row[CN.FeesAndInterest]).ToString(DecimalPlaces);
				txtTotalInstalmentValue.Text = ((decimal)row[CN.TotalCurrentInstalments]).ToString(DecimalPlaces);
				txtNoOfCurrent.Text = Convert.ToString(row[CN.NumCurrent]);
				txtNoOfSettled.Text = Convert.ToString(row[CN.NumSettled]);
				txtNoAccountsInArrears.Text = Convert.ToString(row[CN.NumInArrears]);
				txtCashAccountsSettled.Text = Convert.ToString(row[CN.NumCashSettled]);
				txtNoOfReturnedCheques.Text = Convert.ToString(row[CN.NumReturnedCheques]);
				txtWorstCurrentStatus.Text = (string)row[CN.WorstCurrent];
				txtWorstSettledStatus.Text = (string)row[CN.WorstSettled];
				txtLongestAgreement.Text = Convert.ToString(row[CN.LongestAgreement]);
				txtRepaymentPcentCurrent.Text = ((decimal)row[CN.RepaymentPcentCurrent]).ToString("F2") + "%";
				txtCreditLimit.Text = ((decimal)row[CN.CreditLimit]).ToString(DecimalPlaces);
				txtRFAccts.Text = Convert.ToString(row[CN.NumRFAccounts]);
				txtSysRecommend.Text = (string)row[CN.SysRecommend];

                /* Underwriter Information */
                txtScore.Text = Convert.ToString(row[CN.Score]);    //IP - 15/03/11 - #3316 - CR1245
                txtReferralNotes.Text = Convert.ToString(row[CN.PropNotes]);    //IP - 15/03/11 - #3316 - CR1245
			}
		}

		public void LoadDetails(Form root, Form parent, string accountNo, string customerID, DateTime dateProp, string accountType)
		{
			FormRoot = root;
			FormParent = parent;
			AccountType = accountType;
			AccountNo = accountNo;
			CustomerID = customerID;
			DateProp = dateProp;		
			LineItems = null;

			/* retrieve all the data and populate the controls */
			DataSet details = CreditManager.GetReferralSummaryData(AccountNo, CustomerID, AccountType, DateProp, out LineItems, out Error);
			if(Error.Length>0)
				ShowError(Error);
			else
			{
				Populate(details);
				BuildLineItems();
			}					
		}

		private void BuildLineItems()
		{
			itemDoc = new XmlDocument();
			itemDoc.LoadXml("<ITEMS></ITEMS>");

			//initialise the XML document and the tree view
			if(LineItems != null)
			{
				LineItems = itemDoc.ImportNode(LineItems, true);
				itemDoc.ReplaceChild(LineItems, itemDoc.DocumentElement);
			}

			if(itemDoc.DocumentElement.HasChildNodes)
			{
				populateTable();
			}
		}

		private void populateTable()
		{
			//Set up the datagrid columns
			if(itemsTable == null)
			{
				//Create the table to hold the Line items
				itemsTable = new DataTable("Items");
				DataColumn[] key = new DataColumn[3];

				itemsTable.Columns.AddRange(new DataColumn[]{ new DataColumn("ProductCode"),
														 new DataColumn("ProductDescription"),
														 new DataColumn("StockLocation"),
														 new DataColumn("QuantityOrdered"),
														 new DataColumn("UnitPrice"),
														 new DataColumn("Value"),
														 new DataColumn("DeliveredQuantity"),
														 new DataColumn("DateDelivered"),
														 new DataColumn("DatePlanDel"),
														 new DataColumn("DateReqDel"),
														 new DataColumn("TimeReqDel"),
														 new DataColumn("DelNoteBranch"),
														 new DataColumn("Notes"),
														 new DataColumn("ContractNo"),
                                                         new DataColumn ("ItemID")});           //IP - 20/05/11 - CR1212 - RI - #3627

				//key[0] = itemsTable.Columns["ProductCode"];
                key[0] = itemsTable.Columns["ItemID"];                                           //IP - 20/05/11 - CR1212 - RI - #3627
				key[1] = itemsTable.Columns["StockLocation"];
				key[2] = itemsTable.Columns["ContractNo"];
				itemsTable.PrimaryKey = key;
			}

			dgLineItems.DataSource = itemsTable.DefaultView;

			if(dgLineItems.TableStyles.Count == 0)
			{
				DataGridTableStyle tabStyle = new DataGridTableStyle();
				tabStyle.MappingName = itemsTable.TableName;	

				AddColumnStyle("ProductCode", tabStyle, 90, true, GetResource("T_PRODCODE"), "", HorizontalAlignment.Left);
				AddColumnStyle("ProductDescription", tabStyle, 200, true, GetResource("T_DESCRIPTION"), "", HorizontalAlignment.Left);
				AddColumnStyle("StockLocation", tabStyle, 75, true, GetResource("T_STOCKLOCN"), "", HorizontalAlignment.Left);
				AddColumnStyle("QuantityOrdered", tabStyle, 40, true, GetResource("T_QUANTITY"), "", HorizontalAlignment.Left);
				AddColumnStyle("UnitPrice", tabStyle, 70, true, GetResource("T_UNITPRICE"), DecimalPlaces, HorizontalAlignment.Right);
				AddColumnStyle("Value", tabStyle, 70, true, GetResource("T_VALUE"), DecimalPlaces, HorizontalAlignment.Right);
				AddColumnStyle("DeliveredQuantity", tabStyle, 70, true, GetResource("T_DELIVEREDQTY"), "", HorizontalAlignment.Left);
				AddColumnStyle("DateDelivered", tabStyle, 70, true, GetResource("T_DELIVERYDATE"), "", HorizontalAlignment.Left);
				AddColumnStyle("DatePlanDel", tabStyle, 150, true, GetResource("T_DATEDELPLAN"), "", HorizontalAlignment.Left);
				AddColumnStyle("DateReqDel", tabStyle, 150, true, GetResource("T_REQDELDATE"), "", HorizontalAlignment.Left);
				AddColumnStyle("TimeReqDel", tabStyle, 150, true, GetResource("T_REQDELTIME"), "", HorizontalAlignment.Left);
				AddColumnStyle("DelNoteBranch", tabStyle, 100, true, GetResource("T_DELNOTEBRANCH"), "", HorizontalAlignment.Left);
				AddColumnStyle("Notes", tabStyle, 150, true, GetResource("T_NOTES"), "", HorizontalAlignment.Left);
				AddColumnStyle("ContractNo", tabStyle, 100, true, GetResource("T_CONTRACTNO"), "", HorizontalAlignment.Left);
                AddColumnStyle("ItemID", tabStyle, 0, true, GetResource("T_ITEMID"), "", HorizontalAlignment.Left);        //IP - 20/05/11 - CR1212 - RI - #3627
					
				dgLineItems.TableStyles.Add(tabStyle);	
			}

			itemsTable.Clear();
			tvItems.Nodes.Clear();
			tvItems.Nodes.Add(new TreeNode("Account"));

			double subTotal = 0;
			populateTable(itemDoc.DocumentElement, tvItems.Nodes[0], ref subTotal);

			tvItems.Nodes[0].Expand();		
		}

		private void populateTable(XmlNode relatedItems, TreeNode tvNode, ref double sub)
		{
			Function = "populateTable";
			string itemType = "";
			double qty = 0;

			//outer loop iterates through <item> tags
			foreach(XmlNode item in relatedItems.ChildNodes)
			{
				if(item.NodeType == XmlNodeType.Element)
				{
					TreeNode tvChild = new TreeNode();
					tvChild.Tag = item.Attributes[Tags.Key].Value;
					DataRow row = itemsTable.NewRow();
					bool showRow = true;
					
					itemType = item.Attributes[Tags.Type].Value;
					qty = Convert.ToDouble(item.Attributes[Tags.Quantity].Value);
					
					//tvChild.Text = itemType;
                    tvChild.Text = item.Attributes[Tags.FreeGift].Value.ToUpper().Trim() == "TRUE" ? "FreeGift" : itemType; //IP - 21/04/10 - UAT(53) UAT5.2 - Merged from 4.3
					tvChild.ImageIndex=1;
					tvChild.SelectedImageIndex=1;
					
					if(itemType==IT.Stock||itemType==IT.Component)
					{
						tvChild.ImageIndex=0;
						tvChild.SelectedImageIndex=0;
					}
					//if(itemType==IT.Discount||itemType==IT.KitDiscount)
                    if (itemType == IT.Discount || itemType == IT.KitDiscount || item.Attributes[Tags.FreeGift].Value.ToUpper().Trim() == "TRUE") //IP - 21/04/10 - UAT(53) UAT5.2 - Merged from 4.3
					{
						tvChild.ImageIndex=2;
						tvChild.SelectedImageIndex=2;
					}
					if(itemType==IT.Warranty)
					{
						tvChild.ImageIndex=3;
						tvChild.SelectedImageIndex=3;
					}
					if(itemType==IT.Kit || qty <= 0)
					{
						showRow = false;
					}

					row["ProductCode"] = item.Attributes[Tags.Code].Value;
                    row["ItemId"] = item.Attributes[Tags.ItemId].Value;
					row["ProductDescription"] = item.Attributes[Tags.Description1].Value;
					row["StockLocation"] = item.Attributes[Tags.Location].Value;
					row["QuantityOrdered"] = item.Attributes[Tags.Quantity].Value;
					row["UnitPrice"] = item.Attributes[Tags.UnitPrice].Value;
					row["DeliveredQuantity"] = item.Attributes[Tags.DeliveredQuantity].Value;
					row["DateDelivered"] = item.Attributes[Tags.DateDelivered].Value;
					row["DatePlanDel"] = Convert.ToDateTime(item.Attributes[Tags.PlannedDeliveryDate].Value).ToString("dd/MM/yyy");
					row["DateReqDel"] = Convert.ToDateTime(item.Attributes[Tags.DeliveryDate].Value).ToString("dd/MM/yyy");
					row["TimeReqDel"] = item.Attributes[Tags.DeliveryTime].Value;
					row["DelNoteBranch"] = item.Attributes[Tags.BranchForDeliveryNote].Value;
					row["Notes"] = item.Attributes[Tags.ColourTrim].Value;
					row["ContractNo"] = item.Attributes[Tags.ContractNumber].Value;
                    row[CN.ItemId] = item.Attributes[Tags.ItemId].Value;                            //IP - 20/05/11 - CR1212 - RI - #3627

					if(showRow)
					{
						row["Value"] = item.Attributes[Tags.Value].Value;
						sub += Convert.ToDouble(StripCurrency(item.Attributes[Tags.Value].Value));
					}

					foreach(XmlNode child in item.ChildNodes)
						if(child.NodeType==XmlNodeType.Element&&child.Name==Elements.RelatedItem)
						{
							if(child.HasChildNodes)
								populateTable(child, tvChild, ref sub);
						}
					
					if(qty > 0)
						tvNode.Nodes.Add(tvChild);
					
					if(showRow)
						itemsTable.Rows.Add(row);
				}
			}
			Function = "End of populateTable";
		}

		private void SetBackColour(System.Windows.Forms.Control.ControlCollection controls)
		{
			foreach(Control c in controls)
			{
				if( c is TextBox )
					c.BackColor = SystemColors.Window;
				if(c.Controls.Count > 0)
					SetBackColour(c.Controls);
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ReferralSummary));
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.tcSummary = new Crownwood.Magic.Controls.TabControl();
            this.tpUnderwriterDetails = new Crownwood.Magic.Controls.TabPage();
            this.tpPersonalDetails = new Crownwood.Magic.Controls.TabPage();
            this.label26 = new System.Windows.Forms.Label();
            this.txtSysRecommend = new System.Windows.Forms.TextBox();
            this.label32 = new System.Windows.Forms.Label();
            this.txtOccupation = new System.Windows.Forms.TextBox();
            this.label31 = new System.Windows.Forms.Label();
            this.txtExpenses = new System.Windows.Forms.TextBox();
            this.label30 = new System.Windows.Forms.Label();
            this.txtResidentialStatus = new System.Windows.Forms.TextBox();
            this.label29 = new System.Windows.Forms.Label();
            this.txtMaritalStatus = new System.Windows.Forms.TextBox();
            this.label28 = new System.Windows.Forms.Label();
            this.txtIncomeAfterTax = new System.Windows.Forms.TextBox();
            this.label25 = new System.Windows.Forms.Label();
            this.txtAge = new System.Windows.Forms.TextBox();
            this.tpAgreement = new Crownwood.Magic.Controls.TabPage();
            this.txtRFAccts = new System.Windows.Forms.TextBox();
            this.label24 = new System.Windows.Forms.Label();
            this.txtCreditLimit = new System.Windows.Forms.TextBox();
            this.label23 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtAcctType = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtAgreementTotal = new System.Windows.Forms.TextBox();
            this.label21 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.txtFinalInstalment = new System.Windows.Forms.TextBox();
            this.txtInstalment = new System.Windows.Forms.TextBox();
            this.txtDeposit = new System.Windows.Forms.TextBox();
            this.label18 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtTermsType = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtRepaymentPcent = new System.Windows.Forms.TextBox();
            this.gbLineItems = new System.Windows.Forms.GroupBox();
            this.dgLineItems = new System.Windows.Forms.DataGrid();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.tvItems = new System.Windows.Forms.TreeView();
            this.tpAccountHistory = new Crownwood.Magic.Controls.TabPage();
            this.label22 = new System.Windows.Forms.Label();
            this.txtValueOfAllArrears = new System.Windows.Forms.TextBox();
            this.label19 = new System.Windows.Forms.Label();
            this.txtNoOfReturnedCheques = new System.Windows.Forms.TextBox();
            this.label17 = new System.Windows.Forms.Label();
            this.txtRepaymentPcentCurrent = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.txtCashAccountsSettled = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.txtFeesAndInterest = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.txtLargestAgreement = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.txtLongestAgreement = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.txtNoAccountsInArrears = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.txtTotalInstalmentValue = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.txtOutstandingBalance = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.txtWorstSettledStatus = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtWorstCurrentStatus = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtNoOfSettled = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtNoOfCurrent = new System.Windows.Forms.TextBox();
            this.txtReferralNotes = new System.Windows.Forms.RichTextBox();
            this.lblReferralNotes = new System.Windows.Forms.Label();
            this.lblScore = new System.Windows.Forms.Label();
            this.dtDateInCurrentEmp = new STL.PL.DatePicker();
            this.dtDateInCurrentAddress = new STL.PL.DatePicker();
            this.txtScore = new System.Windows.Forms.TextBox();
            this.tcSummary.SuspendLayout();
            this.tpUnderwriterDetails.SuspendLayout();
            this.tpPersonalDetails.SuspendLayout();
            this.tpAgreement.SuspendLayout();
            this.gbLineItems.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgLineItems)).BeginInit();
            this.tpAccountHistory.SuspendLayout();
            this.SuspendLayout();
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "");
            this.imageList1.Images.SetKeyName(1, "");
            this.imageList1.Images.SetKeyName(2, "");
            this.imageList1.Images.SetKeyName(3, "");
            this.imageList1.Images.SetKeyName(4, "");
            // 
            // tcSummary
            // 
            this.tcSummary.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tcSummary.IDEPixelArea = true;
            this.tcSummary.Location = new System.Drawing.Point(0, 0);
            this.tcSummary.Name = "tcSummary";
            this.tcSummary.PositionTop = true;
            this.tcSummary.SelectedIndex = 3;
            this.tcSummary.SelectedTab = this.tpUnderwriterDetails;
            this.tcSummary.Size = new System.Drawing.Size(762, 381);
            this.tcSummary.TabIndex = 2;
            this.tcSummary.TabPages.AddRange(new Crownwood.Magic.Controls.TabPage[] {
            this.tpPersonalDetails,
            this.tpAgreement,
            this.tpAccountHistory,
            this.tpUnderwriterDetails});
            // 
            // tpUnderwriterDetails
            // 
            this.tpUnderwriterDetails.Controls.Add(this.txtScore);
            this.tpUnderwriterDetails.Controls.Add(this.lblScore);
            this.tpUnderwriterDetails.Controls.Add(this.lblReferralNotes);
            this.tpUnderwriterDetails.Controls.Add(this.txtReferralNotes);
            this.tpUnderwriterDetails.Location = new System.Drawing.Point(0, 25);
            this.tpUnderwriterDetails.Name = "tpUnderwriterDetails";
            this.tpUnderwriterDetails.Size = new System.Drawing.Size(762, 356);
            this.tpUnderwriterDetails.TabIndex = 7;
            this.tpUnderwriterDetails.Title = "Underwiter Information";
            // 
            // tpPersonalDetails
            // 
            this.tpPersonalDetails.Controls.Add(this.label26);
            this.tpPersonalDetails.Controls.Add(this.txtSysRecommend);
            this.tpPersonalDetails.Controls.Add(this.label32);
            this.tpPersonalDetails.Controls.Add(this.txtOccupation);
            this.tpPersonalDetails.Controls.Add(this.label31);
            this.tpPersonalDetails.Controls.Add(this.txtExpenses);
            this.tpPersonalDetails.Controls.Add(this.label30);
            this.tpPersonalDetails.Controls.Add(this.txtResidentialStatus);
            this.tpPersonalDetails.Controls.Add(this.label29);
            this.tpPersonalDetails.Controls.Add(this.txtMaritalStatus);
            this.tpPersonalDetails.Controls.Add(this.label28);
            this.tpPersonalDetails.Controls.Add(this.txtIncomeAfterTax);
            this.tpPersonalDetails.Controls.Add(this.label25);
            this.tpPersonalDetails.Controls.Add(this.txtAge);
            this.tpPersonalDetails.Controls.Add(this.dtDateInCurrentEmp);
            this.tpPersonalDetails.Controls.Add(this.dtDateInCurrentAddress);
            this.tpPersonalDetails.Location = new System.Drawing.Point(0, 25);
            this.tpPersonalDetails.Name = "tpPersonalDetails";
            this.tpPersonalDetails.Selected = false;
            this.tpPersonalDetails.Size = new System.Drawing.Size(762, 356);
            this.tpPersonalDetails.TabIndex = 3;
            this.tpPersonalDetails.Title = "Personal Details / System Recommendation";
            // 
            // label26
            // 
            this.label26.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label26.Location = new System.Drawing.Point(80, 48);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(136, 16);
            this.label26.TabIndex = 48;
            this.label26.Text = "System Recommendation:";
            // 
            // txtSysRecommend
            // 
            this.txtSysRecommend.Location = new System.Drawing.Point(80, 64);
            this.txtSysRecommend.MaxLength = 2;
            this.txtSysRecommend.Name = "txtSysRecommend";
            this.txtSysRecommend.ReadOnly = true;
            this.txtSysRecommend.Size = new System.Drawing.Size(48, 23);
            this.txtSysRecommend.TabIndex = 47;
            // 
            // label32
            // 
            this.label32.Location = new System.Drawing.Point(80, 144);
            this.label32.Name = "label32";
            this.label32.Size = new System.Drawing.Size(95, 16);
            this.label32.TabIndex = 26;
            this.label32.Text = "Occupation";
            // 
            // txtOccupation
            // 
            this.txtOccupation.Location = new System.Drawing.Point(80, 160);
            this.txtOccupation.Name = "txtOccupation";
            this.txtOccupation.ReadOnly = true;
            this.txtOccupation.Size = new System.Drawing.Size(88, 23);
            this.txtOccupation.TabIndex = 25;
            // 
            // label31
            // 
            this.label31.Location = new System.Drawing.Point(232, 144);
            this.label31.Name = "label31";
            this.label31.Size = new System.Drawing.Size(95, 16);
            this.label31.TabIndex = 24;
            this.label31.Text = "Expenses";
            // 
            // txtExpenses
            // 
            this.txtExpenses.Location = new System.Drawing.Point(232, 160);
            this.txtExpenses.Name = "txtExpenses";
            this.txtExpenses.ReadOnly = true;
            this.txtExpenses.Size = new System.Drawing.Size(88, 23);
            this.txtExpenses.TabIndex = 23;
            this.txtExpenses.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label30
            // 
            this.label30.Location = new System.Drawing.Point(456, 48);
            this.label30.Name = "label30";
            this.label30.Size = new System.Drawing.Size(95, 16);
            this.label30.TabIndex = 7;
            this.label30.Text = "Residential Status";
            // 
            // txtResidentialStatus
            // 
            this.txtResidentialStatus.Location = new System.Drawing.Point(456, 64);
            this.txtResidentialStatus.Name = "txtResidentialStatus";
            this.txtResidentialStatus.ReadOnly = true;
            this.txtResidentialStatus.Size = new System.Drawing.Size(88, 23);
            this.txtResidentialStatus.TabIndex = 6;
            // 
            // label29
            // 
            this.label29.Location = new System.Drawing.Point(344, 48);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(95, 16);
            this.label29.TabIndex = 5;
            this.label29.Text = "Marital Status";
            // 
            // txtMaritalStatus
            // 
            this.txtMaritalStatus.Location = new System.Drawing.Point(344, 64);
            this.txtMaritalStatus.Name = "txtMaritalStatus";
            this.txtMaritalStatus.ReadOnly = true;
            this.txtMaritalStatus.Size = new System.Drawing.Size(88, 23);
            this.txtMaritalStatus.TabIndex = 4;
            // 
            // label28
            // 
            this.label28.Location = new System.Drawing.Point(344, 144);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(95, 16);
            this.label28.TabIndex = 3;
            this.label28.Text = "Income After Tax";
            // 
            // txtIncomeAfterTax
            // 
            this.txtIncomeAfterTax.Location = new System.Drawing.Point(344, 160);
            this.txtIncomeAfterTax.Name = "txtIncomeAfterTax";
            this.txtIncomeAfterTax.ReadOnly = true;
            this.txtIncomeAfterTax.Size = new System.Drawing.Size(88, 23);
            this.txtIncomeAfterTax.TabIndex = 2;
            this.txtIncomeAfterTax.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label25
            // 
            this.label25.Location = new System.Drawing.Point(232, 48);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(32, 16);
            this.label25.TabIndex = 1;
            this.label25.Text = "Age";
            // 
            // txtAge
            // 
            this.txtAge.Location = new System.Drawing.Point(232, 64);
            this.txtAge.Name = "txtAge";
            this.txtAge.ReadOnly = true;
            this.txtAge.Size = new System.Drawing.Size(40, 23);
            this.txtAge.TabIndex = 0;
            // 
            // tpAgreement
            // 
            this.tpAgreement.Controls.Add(this.txtRFAccts);
            this.tpAgreement.Controls.Add(this.label24);
            this.tpAgreement.Controls.Add(this.txtCreditLimit);
            this.tpAgreement.Controls.Add(this.label23);
            this.tpAgreement.Controls.Add(this.label4);
            this.tpAgreement.Controls.Add(this.txtAcctType);
            this.tpAgreement.Controls.Add(this.label3);
            this.tpAgreement.Controls.Add(this.txtAgreementTotal);
            this.tpAgreement.Controls.Add(this.label21);
            this.tpAgreement.Controls.Add(this.label20);
            this.tpAgreement.Controls.Add(this.txtFinalInstalment);
            this.tpAgreement.Controls.Add(this.txtInstalment);
            this.tpAgreement.Controls.Add(this.txtDeposit);
            this.tpAgreement.Controls.Add(this.label18);
            this.tpAgreement.Controls.Add(this.label2);
            this.tpAgreement.Controls.Add(this.txtTermsType);
            this.tpAgreement.Controls.Add(this.label1);
            this.tpAgreement.Controls.Add(this.txtRepaymentPcent);
            this.tpAgreement.Controls.Add(this.gbLineItems);
            this.tpAgreement.Location = new System.Drawing.Point(0, 25);
            this.tpAgreement.Name = "tpAgreement";
            this.tpAgreement.Selected = false;
            this.tpAgreement.Size = new System.Drawing.Size(762, 356);
            this.tpAgreement.TabIndex = 6;
            this.tpAgreement.Title = "Agreement Details";
            this.tpAgreement.Visible = false;
            // 
            // txtRFAccts
            // 
            this.txtRFAccts.Location = new System.Drawing.Point(120, 128);
            this.txtRFAccts.Name = "txtRFAccts";
            this.txtRFAccts.ReadOnly = true;
            this.txtRFAccts.Size = new System.Drawing.Size(55, 23);
            this.txtRFAccts.TabIndex = 43;
            this.txtRFAccts.Text = "0";
            this.txtRFAccts.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label24
            // 
            this.label24.Location = new System.Drawing.Point(120, 112);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(120, 16);
            this.label24.TabIndex = 42;
            this.label24.Text = "No. of RF Accounts:";
            // 
            // txtCreditLimit
            // 
            this.txtCreditLimit.Location = new System.Drawing.Point(32, 128);
            this.txtCreditLimit.Name = "txtCreditLimit";
            this.txtCreditLimit.ReadOnly = true;
            this.txtCreditLimit.Size = new System.Drawing.Size(80, 23);
            this.txtCreditLimit.TabIndex = 41;
            this.txtCreditLimit.Text = "0";
            this.txtCreditLimit.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label23
            // 
            this.label23.Location = new System.Drawing.Point(32, 112);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(80, 16);
            this.label23.TabIndex = 40;
            this.label23.Text = "Credit Limit:";
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(120, 56);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(72, 16);
            this.label4.TabIndex = 39;
            this.label4.Text = "Account Type";
            // 
            // txtAcctType
            // 
            this.txtAcctType.Location = new System.Drawing.Point(120, 72);
            this.txtAcctType.Name = "txtAcctType";
            this.txtAcctType.ReadOnly = true;
            this.txtAcctType.Size = new System.Drawing.Size(48, 23);
            this.txtAcctType.TabIndex = 38;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(648, 56);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(88, 16);
            this.label3.TabIndex = 36;
            this.label3.Text = "Agreement Total";
            // 
            // txtAgreementTotal
            // 
            this.txtAgreementTotal.Location = new System.Drawing.Point(648, 72);
            this.txtAgreementTotal.Name = "txtAgreementTotal";
            this.txtAgreementTotal.ReadOnly = true;
            this.txtAgreementTotal.Size = new System.Drawing.Size(80, 23);
            this.txtAgreementTotal.TabIndex = 37;
            this.txtAgreementTotal.TabStop = false;
            this.txtAgreementTotal.Text = "0";
            this.txtAgreementTotal.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label21
            // 
            this.label21.Location = new System.Drawing.Point(544, 56);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(88, 16);
            this.label21.TabIndex = 34;
            this.label21.Text = "Final Instalment";
            // 
            // label20
            // 
            this.label20.Location = new System.Drawing.Point(440, 56);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(64, 16);
            this.label20.TabIndex = 33;
            this.label20.Text = "Instalment";
            // 
            // txtFinalInstalment
            // 
            this.txtFinalInstalment.Location = new System.Drawing.Point(544, 72);
            this.txtFinalInstalment.Name = "txtFinalInstalment";
            this.txtFinalInstalment.ReadOnly = true;
            this.txtFinalInstalment.Size = new System.Drawing.Size(80, 23);
            this.txtFinalInstalment.TabIndex = 35;
            this.txtFinalInstalment.TabStop = false;
            this.txtFinalInstalment.Text = "0";
            this.txtFinalInstalment.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // txtInstalment
            // 
            this.txtInstalment.Location = new System.Drawing.Point(440, 72);
            this.txtInstalment.Name = "txtInstalment";
            this.txtInstalment.ReadOnly = true;
            this.txtInstalment.Size = new System.Drawing.Size(80, 23);
            this.txtInstalment.TabIndex = 32;
            this.txtInstalment.TabStop = false;
            this.txtInstalment.Text = "0";
            this.txtInstalment.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // txtDeposit
            // 
            this.txtDeposit.Location = new System.Drawing.Point(336, 72);
            this.txtDeposit.Name = "txtDeposit";
            this.txtDeposit.ReadOnly = true;
            this.txtDeposit.Size = new System.Drawing.Size(80, 23);
            this.txtDeposit.TabIndex = 31;
            this.txtDeposit.Text = "0";
            this.txtDeposit.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label18
            // 
            this.label18.Location = new System.Drawing.Point(336, 56);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(48, 16);
            this.label18.TabIndex = 30;
            this.label18.Text = "Deposit:";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(192, 56);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(72, 16);
            this.label2.TabIndex = 7;
            this.label2.Text = "Terms Type";
            // 
            // txtTermsType
            // 
            this.txtTermsType.Location = new System.Drawing.Point(192, 72);
            this.txtTermsType.Name = "txtTermsType";
            this.txtTermsType.ReadOnly = true;
            this.txtTermsType.Size = new System.Drawing.Size(128, 23);
            this.txtTermsType.TabIndex = 6;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(32, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(96, 40);
            this.label1.TabIndex = 5;
            this.label1.Text = "Repayment as % of net Disposable Income";
            // 
            // txtRepaymentPcent
            // 
            this.txtRepaymentPcent.Location = new System.Drawing.Point(32, 72);
            this.txtRepaymentPcent.Name = "txtRepaymentPcent";
            this.txtRepaymentPcent.ReadOnly = true;
            this.txtRepaymentPcent.Size = new System.Drawing.Size(48, 23);
            this.txtRepaymentPcent.TabIndex = 4;
            // 
            // gbLineItems
            // 
            this.gbLineItems.Controls.Add(this.dgLineItems);
            this.gbLineItems.Controls.Add(this.splitter1);
            this.gbLineItems.Controls.Add(this.tvItems);
            this.gbLineItems.Location = new System.Drawing.Point(24, 168);
            this.gbLineItems.Name = "gbLineItems";
            this.gbLineItems.Size = new System.Drawing.Size(712, 176);
            this.gbLineItems.TabIndex = 3;
            this.gbLineItems.TabStop = false;
            this.gbLineItems.Text = "Line Items";
            // 
            // dgLineItems
            // 
            this.dgLineItems.CaptionText = "Line Items";
            this.dgLineItems.DataMember = "";
            this.dgLineItems.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgLineItems.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgLineItems.Location = new System.Drawing.Point(127, 19);
            this.dgLineItems.Name = "dgLineItems";
            this.dgLineItems.ReadOnly = true;
            this.dgLineItems.Size = new System.Drawing.Size(582, 154);
            this.dgLineItems.TabIndex = 2;
            // 
            // splitter1
            // 
            this.splitter1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.splitter1.Location = new System.Drawing.Point(124, 19);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(3, 154);
            this.splitter1.TabIndex = 1;
            this.splitter1.TabStop = false;
            // 
            // tvItems
            // 
            this.tvItems.Dock = System.Windows.Forms.DockStyle.Left;
            this.tvItems.ImageIndex = 0;
            this.tvItems.ImageList = this.imageList1;
            this.tvItems.Indent = 19;
            this.tvItems.ItemHeight = 17;
            this.tvItems.Location = new System.Drawing.Point(3, 19);
            this.tvItems.Name = "tvItems";
            this.tvItems.SelectedImageIndex = 0;
            this.tvItems.Size = new System.Drawing.Size(121, 154);
            this.tvItems.TabIndex = 0;
            this.tvItems.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvItems_AfterSelect);
            // 
            // tpAccountHistory
            // 
            this.tpAccountHistory.Controls.Add(this.label22);
            this.tpAccountHistory.Controls.Add(this.txtValueOfAllArrears);
            this.tpAccountHistory.Controls.Add(this.label19);
            this.tpAccountHistory.Controls.Add(this.txtNoOfReturnedCheques);
            this.tpAccountHistory.Controls.Add(this.label17);
            this.tpAccountHistory.Controls.Add(this.txtRepaymentPcentCurrent);
            this.tpAccountHistory.Controls.Add(this.label16);
            this.tpAccountHistory.Controls.Add(this.txtCashAccountsSettled);
            this.tpAccountHistory.Controls.Add(this.label15);
            this.tpAccountHistory.Controls.Add(this.txtFeesAndInterest);
            this.tpAccountHistory.Controls.Add(this.label14);
            this.tpAccountHistory.Controls.Add(this.txtLargestAgreement);
            this.tpAccountHistory.Controls.Add(this.label13);
            this.tpAccountHistory.Controls.Add(this.label12);
            this.tpAccountHistory.Controls.Add(this.txtLongestAgreement);
            this.tpAccountHistory.Controls.Add(this.label11);
            this.tpAccountHistory.Controls.Add(this.txtNoAccountsInArrears);
            this.tpAccountHistory.Controls.Add(this.label10);
            this.tpAccountHistory.Controls.Add(this.txtTotalInstalmentValue);
            this.tpAccountHistory.Controls.Add(this.label9);
            this.tpAccountHistory.Controls.Add(this.txtOutstandingBalance);
            this.tpAccountHistory.Controls.Add(this.label8);
            this.tpAccountHistory.Controls.Add(this.txtWorstSettledStatus);
            this.tpAccountHistory.Controls.Add(this.label7);
            this.tpAccountHistory.Controls.Add(this.txtWorstCurrentStatus);
            this.tpAccountHistory.Controls.Add(this.label6);
            this.tpAccountHistory.Controls.Add(this.txtNoOfSettled);
            this.tpAccountHistory.Controls.Add(this.label5);
            this.tpAccountHistory.Controls.Add(this.txtNoOfCurrent);
            this.tpAccountHistory.Location = new System.Drawing.Point(0, 25);
            this.tpAccountHistory.Name = "tpAccountHistory";
            this.tpAccountHistory.Selected = false;
            this.tpAccountHistory.Size = new System.Drawing.Size(762, 356);
            this.tpAccountHistory.TabIndex = 5;
            this.tpAccountHistory.Title = "Account History";
            this.tpAccountHistory.Visible = false;
            // 
            // label22
            // 
            this.label22.Location = new System.Drawing.Point(56, 48);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(88, 24);
            this.label22.TabIndex = 63;
            this.label22.Text = "Value of all arrears";
            // 
            // txtValueOfAllArrears
            // 
            this.txtValueOfAllArrears.Location = new System.Drawing.Point(56, 80);
            this.txtValueOfAllArrears.Name = "txtValueOfAllArrears";
            this.txtValueOfAllArrears.ReadOnly = true;
            this.txtValueOfAllArrears.Size = new System.Drawing.Size(80, 23);
            this.txtValueOfAllArrears.TabIndex = 64;
            this.txtValueOfAllArrears.TabStop = false;
            this.txtValueOfAllArrears.Text = "0";
            this.txtValueOfAllArrears.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label19
            // 
            this.label19.Location = new System.Drawing.Point(56, 248);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(88, 24);
            this.label19.TabIndex = 61;
            this.label19.Text = "No. of returned cheques";
            // 
            // txtNoOfReturnedCheques
            // 
            this.txtNoOfReturnedCheques.Location = new System.Drawing.Point(56, 280);
            this.txtNoOfReturnedCheques.Name = "txtNoOfReturnedCheques";
            this.txtNoOfReturnedCheques.ReadOnly = true;
            this.txtNoOfReturnedCheques.Size = new System.Drawing.Size(64, 23);
            this.txtNoOfReturnedCheques.TabIndex = 62;
            this.txtNoOfReturnedCheques.TabStop = false;
            this.txtNoOfReturnedCheques.Text = "0";
            this.txtNoOfReturnedCheques.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label17
            // 
            this.label17.Location = new System.Drawing.Point(592, 216);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(88, 56);
            this.label17.TabIndex = 59;
            this.label17.Text = "Repayment as % of net disposable for current accounts";
            // 
            // txtRepaymentPcentCurrent
            // 
            this.txtRepaymentPcentCurrent.Location = new System.Drawing.Point(592, 280);
            this.txtRepaymentPcentCurrent.Name = "txtRepaymentPcentCurrent";
            this.txtRepaymentPcentCurrent.ReadOnly = true;
            this.txtRepaymentPcentCurrent.Size = new System.Drawing.Size(80, 23);
            this.txtRepaymentPcentCurrent.TabIndex = 60;
            this.txtRepaymentPcentCurrent.TabStop = false;
            this.txtRepaymentPcentCurrent.Text = "0";
            this.txtRepaymentPcentCurrent.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label16
            // 
            this.label16.Location = new System.Drawing.Point(448, 144);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(88, 24);
            this.label16.TabIndex = 57;
            this.label16.Text = "No. of cash accounts settled";
            // 
            // txtCashAccountsSettled
            // 
            this.txtCashAccountsSettled.Location = new System.Drawing.Point(448, 176);
            this.txtCashAccountsSettled.Name = "txtCashAccountsSettled";
            this.txtCashAccountsSettled.ReadOnly = true;
            this.txtCashAccountsSettled.Size = new System.Drawing.Size(56, 23);
            this.txtCashAccountsSettled.TabIndex = 58;
            this.txtCashAccountsSettled.TabStop = false;
            this.txtCashAccountsSettled.Text = "0";
            this.txtCashAccountsSettled.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label15
            // 
            this.label15.Location = new System.Drawing.Point(448, 48);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(104, 24);
            this.label15.TabIndex = 55;
            this.label15.Text = "Fees && Interest since last acct opened";
            // 
            // txtFeesAndInterest
            // 
            this.txtFeesAndInterest.Location = new System.Drawing.Point(448, 80);
            this.txtFeesAndInterest.Name = "txtFeesAndInterest";
            this.txtFeesAndInterest.ReadOnly = true;
            this.txtFeesAndInterest.Size = new System.Drawing.Size(80, 23);
            this.txtFeesAndInterest.TabIndex = 56;
            this.txtFeesAndInterest.TabStop = false;
            this.txtFeesAndInterest.Text = "0";
            this.txtFeesAndInterest.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label14
            // 
            this.label14.Location = new System.Drawing.Point(320, 48);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(88, 24);
            this.label14.TabIndex = 53;
            this.label14.Text = "Largest Agreement";
            // 
            // txtLargestAgreement
            // 
            this.txtLargestAgreement.Location = new System.Drawing.Point(320, 80);
            this.txtLargestAgreement.Name = "txtLargestAgreement";
            this.txtLargestAgreement.ReadOnly = true;
            this.txtLargestAgreement.Size = new System.Drawing.Size(80, 23);
            this.txtLargestAgreement.TabIndex = 54;
            this.txtLargestAgreement.TabStop = false;
            this.txtLargestAgreement.Text = "0";
            this.txtLargestAgreement.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label13
            // 
            this.label13.Location = new System.Drawing.Point(496, 288);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(48, 16);
            this.label13.TabIndex = 52;
            this.label13.Text = "months";
            // 
            // label12
            // 
            this.label12.Location = new System.Drawing.Point(448, 248);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(72, 24);
            this.label12.TabIndex = 50;
            this.label12.Text = "Longest Agreement";
            // 
            // txtLongestAgreement
            // 
            this.txtLongestAgreement.Location = new System.Drawing.Point(448, 280);
            this.txtLongestAgreement.Name = "txtLongestAgreement";
            this.txtLongestAgreement.ReadOnly = true;
            this.txtLongestAgreement.Size = new System.Drawing.Size(40, 23);
            this.txtLongestAgreement.TabIndex = 51;
            this.txtLongestAgreement.TabStop = false;
            this.txtLongestAgreement.Text = "0";
            this.txtLongestAgreement.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label11
            // 
            this.label11.Location = new System.Drawing.Point(320, 144);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(88, 24);
            this.label11.TabIndex = 48;
            this.label11.Text = "No. of accounts in arrears";
            // 
            // txtNoAccountsInArrears
            // 
            this.txtNoAccountsInArrears.Location = new System.Drawing.Point(320, 176);
            this.txtNoAccountsInArrears.Name = "txtNoAccountsInArrears";
            this.txtNoAccountsInArrears.ReadOnly = true;
            this.txtNoAccountsInArrears.Size = new System.Drawing.Size(80, 23);
            this.txtNoAccountsInArrears.TabIndex = 49;
            this.txtNoAccountsInArrears.TabStop = false;
            this.txtNoAccountsInArrears.Text = "0";
            this.txtNoAccountsInArrears.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label10
            // 
            this.label10.Location = new System.Drawing.Point(592, 48);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(88, 24);
            this.label10.TabIndex = 46;
            this.label10.Text = "Total Instalment value of current";
            // 
            // txtTotalInstalmentValue
            // 
            this.txtTotalInstalmentValue.Location = new System.Drawing.Point(592, 80);
            this.txtTotalInstalmentValue.Name = "txtTotalInstalmentValue";
            this.txtTotalInstalmentValue.ReadOnly = true;
            this.txtTotalInstalmentValue.Size = new System.Drawing.Size(80, 23);
            this.txtTotalInstalmentValue.TabIndex = 47;
            this.txtTotalInstalmentValue.TabStop = false;
            this.txtTotalInstalmentValue.Text = "0";
            this.txtTotalInstalmentValue.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label9
            // 
            this.label9.Location = new System.Drawing.Point(184, 48);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(88, 24);
            this.label9.TabIndex = 44;
            this.label9.Text = "Outstanding Balance";
            // 
            // txtOutstandingBalance
            // 
            this.txtOutstandingBalance.Location = new System.Drawing.Point(184, 80);
            this.txtOutstandingBalance.Name = "txtOutstandingBalance";
            this.txtOutstandingBalance.ReadOnly = true;
            this.txtOutstandingBalance.Size = new System.Drawing.Size(80, 23);
            this.txtOutstandingBalance.TabIndex = 45;
            this.txtOutstandingBalance.TabStop = false;
            this.txtOutstandingBalance.Text = "0";
            this.txtOutstandingBalance.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label8
            // 
            this.label8.Location = new System.Drawing.Point(320, 248);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(88, 24);
            this.label8.TabIndex = 42;
            this.label8.Text = "Worst settled status ever";
            // 
            // txtWorstSettledStatus
            // 
            this.txtWorstSettledStatus.Location = new System.Drawing.Point(320, 280);
            this.txtWorstSettledStatus.Name = "txtWorstSettledStatus";
            this.txtWorstSettledStatus.ReadOnly = true;
            this.txtWorstSettledStatus.Size = new System.Drawing.Size(56, 23);
            this.txtWorstSettledStatus.TabIndex = 43;
            this.txtWorstSettledStatus.TabStop = false;
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(184, 248);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(88, 24);
            this.label7.TabIndex = 40;
            this.label7.Text = "Worst current status ever";
            // 
            // txtWorstCurrentStatus
            // 
            this.txtWorstCurrentStatus.Location = new System.Drawing.Point(184, 280);
            this.txtWorstCurrentStatus.Name = "txtWorstCurrentStatus";
            this.txtWorstCurrentStatus.ReadOnly = true;
            this.txtWorstCurrentStatus.Size = new System.Drawing.Size(55, 23);
            this.txtWorstCurrentStatus.TabIndex = 41;
            this.txtWorstCurrentStatus.TabStop = false;
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(184, 144);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(55, 24);
            this.label6.TabIndex = 38;
            this.label6.Text = "No. of Settled";
            // 
            // txtNoOfSettled
            // 
            this.txtNoOfSettled.Location = new System.Drawing.Point(184, 176);
            this.txtNoOfSettled.Name = "txtNoOfSettled";
            this.txtNoOfSettled.ReadOnly = true;
            this.txtNoOfSettled.Size = new System.Drawing.Size(55, 23);
            this.txtNoOfSettled.TabIndex = 39;
            this.txtNoOfSettled.TabStop = false;
            this.txtNoOfSettled.Text = "0";
            this.txtNoOfSettled.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(56, 144);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(55, 24);
            this.label5.TabIndex = 36;
            this.label5.Text = "No. of Current";
            // 
            // txtNoOfCurrent
            // 
            this.txtNoOfCurrent.Location = new System.Drawing.Point(56, 176);
            this.txtNoOfCurrent.Name = "txtNoOfCurrent";
            this.txtNoOfCurrent.ReadOnly = true;
            this.txtNoOfCurrent.Size = new System.Drawing.Size(55, 23);
            this.txtNoOfCurrent.TabIndex = 37;
            this.txtNoOfCurrent.TabStop = false;
            this.txtNoOfCurrent.Text = "0";
            this.txtNoOfCurrent.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // txtReferralNotes
            // 
            this.txtReferralNotes.BackColor = System.Drawing.SystemColors.Control;
            this.txtReferralNotes.Location = new System.Drawing.Point(27, 120);
            this.txtReferralNotes.Name = "txtReferralNotes";
            this.txtReferralNotes.Size = new System.Drawing.Size(483, 169);
            this.txtReferralNotes.TabIndex = 0;
            this.txtReferralNotes.Text = "";
            // 
            // lblReferralNotes
            // 
            this.lblReferralNotes.AutoSize = true;
            this.lblReferralNotes.Location = new System.Drawing.Point(24, 88);
            this.lblReferralNotes.Name = "lblReferralNotes";
            this.lblReferralNotes.Size = new System.Drawing.Size(103, 15);
            this.lblReferralNotes.TabIndex = 1;
            this.lblReferralNotes.Text = "Underwriter Notes";
            // 
            // lblScore
            // 
            this.lblScore.AutoSize = true;
            this.lblScore.Location = new System.Drawing.Point(24, 22);
            this.lblScore.Name = "lblScore";
            this.lblScore.Size = new System.Drawing.Size(36, 15);
            this.lblScore.TabIndex = 3;
            this.lblScore.Text = "Score";
            // 
            // dtDateInCurrentEmp
            // 
            this.dtDateInCurrentEmp.DateFrom = new System.DateTime(2002, 11, 19, 0, 0, 0, 0);
            this.dtDateInCurrentEmp.Enabled = false;
            this.dtDateInCurrentEmp.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.dtDateInCurrentEmp.Label = "Curr. Emp. started:";
            this.dtDateInCurrentEmp.LinkedBias = false;
            this.dtDateInCurrentEmp.LinkedComboBox = null;
            this.dtDateInCurrentEmp.LinkedDatePicker = null;
            this.dtDateInCurrentEmp.LinkedLabel = null;
            this.dtDateInCurrentEmp.Location = new System.Drawing.Point(424, 240);
            this.dtDateInCurrentEmp.Months = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.dtDateInCurrentEmp.Name = "dtDateInCurrentEmp";
            this.dtDateInCurrentEmp.Size = new System.Drawing.Size(256, 56);
            this.dtDateInCurrentEmp.TabIndex = 22;
            this.dtDateInCurrentEmp.Tag = "dtCurrEmpStart1";
            this.dtDateInCurrentEmp.Value = new System.DateTime(2002, 11, 19, 0, 0, 0, 0);
            this.dtDateInCurrentEmp.Years = new decimal(new int[] {
            0,
            0,
            0,
            0});
            // 
            // dtDateInCurrentAddress
            // 
            this.dtDateInCurrentAddress.DateFrom = new System.DateTime(2002, 11, 19, 0, 0, 0, 0);
            this.dtDateInCurrentAddress.Enabled = false;
            this.dtDateInCurrentAddress.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.dtDateInCurrentAddress.Label = "Date In Curr Address:";
            this.dtDateInCurrentAddress.LinkedBias = true;
            this.dtDateInCurrentAddress.LinkedComboBox = null;
            this.dtDateInCurrentAddress.LinkedDatePicker = null;
            this.dtDateInCurrentAddress.LinkedLabel = null;
            this.dtDateInCurrentAddress.Location = new System.Drawing.Point(72, 240);
            this.dtDateInCurrentAddress.Months = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.dtDateInCurrentAddress.Name = "dtDateInCurrentAddress";
            this.dtDateInCurrentAddress.Size = new System.Drawing.Size(256, 56);
            this.dtDateInCurrentAddress.TabIndex = 21;
            this.dtDateInCurrentAddress.Tag = "dtDateInCurrentAddress1";
            this.dtDateInCurrentAddress.Value = new System.DateTime(2002, 11, 19, 0, 0, 0, 0);
            this.dtDateInCurrentAddress.Years = new decimal(new int[] {
            0,
            0,
            0,
            0});
            // 
            // txtScore
            // 
            this.txtScore.Location = new System.Drawing.Point(27, 52);
            this.txtScore.Name = "txtScore";
            this.txtScore.ReadOnly = true;
            this.txtScore.Size = new System.Drawing.Size(88, 23);
            this.txtScore.TabIndex = 7;
            // 
            // ReferralSummary
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(762, 381);
            this.Controls.Add(this.tcSummary);
            this.Name = "ReferralSummary";
            this.Text = "Referral Summary";
            this.tcSummary.ResumeLayout(false);
            this.tpUnderwriterDetails.ResumeLayout(false);
            this.tpUnderwriterDetails.PerformLayout();
            this.tpPersonalDetails.ResumeLayout(false);
            this.tpPersonalDetails.PerformLayout();
            this.tpAgreement.ResumeLayout(false);
            this.tpAgreement.PerformLayout();
            this.gbLineItems.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgLineItems)).EndInit();
            this.tpAccountHistory.ResumeLayout(false);
            this.tpAccountHistory.PerformLayout();
            this.ResumeLayout(false);

		}
		#endregion

		private void tvItems_AfterSelect(object sender, System.Windows.Forms.TreeViewEventArgs e)
		{
			try
			{
				if((string)e.Node.Tag != null)
				{
					string key = (string)e.Node.Tag;
                    //string product = key.Substring(0, key.IndexOf("|"));
                    string itemID = key.Substring(0, key.IndexOf("|"));                                 //IP - 20/05/11 - CR1212 - RI - #3627
					string location = key.Substring(key.IndexOf("|")+1, key.Length-(key.IndexOf("|")+1));

					int index=0;
					foreach(DataRowView row in itemsTable.DefaultView)
					{	
                        //if((string)row.Row["ProductCode"]==product && (string)row.Row["StockLocation"]==location)
                        if ((string)row.Row[CN.ItemId] == itemID && (string)row.Row["StockLocation"] == location)     //IP - 20/05/11 - CR1212 - RI - #3627
						{
							dgLineItems.Select(index);
							dgLineItems.CurrentRowIndex = index;
						}
						else
						{
							dgLineItems.UnSelect(index);
						}
						index++;
					}
				}
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}
	}
}
