using System;
using System.Data;
using System.Drawing;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Forms;
using System.Web.Services.Protocols;
using STL.PL.WS5;
using STL.Common.Constants.ColumnNames;
using STL.Common.Constants.TableNames;
using System.Xml;
using STL.Common;
using STL.Common.Static;
using Crownwood.Magic.Menus;


namespace STL.PL
{
	/// <summary>
	/// Summary description for BehaviouralResults.
	/// </summary>
	public class BehaviouralResults : CommonForm
	{
        private System.Windows.Forms.GroupBox groupBox1;
		private Crownwood.Magic.Menus.MenuCommand menuExit;
		private Crownwood.Magic.Menus.MenuCommand menuFile;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Button btnLoad;
		private System.Windows.Forms.ComboBox drpCode;
		private string err = "";
        //private DataSet dropDownDS; // = null;
        private System.Windows.Forms.Button btnApply;
		//private string category; // = "";
		//private bool staticLoaded; // = false;
        private ComboBox drpDates;
        private Label label1;
        public Button btnExcel;
        private BasicCustomerDetails customerScreen = null;
        private DataGridView dgAccounts;
        private Button btn_clear;
        private Button btn_selectall;
    
        public BasicCustomerDetails CustomerScreen
        {
            get { return customerScreen; }
            set { customerScreen = value; }
        }

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public BehaviouralResults(TranslationDummy d)
		{
			InitializeComponent();
			menuMain = new Crownwood.Magic.Menus.MenuControl();
			menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {menuFile});
		}

		public BehaviouralResults(Form root, Form parent)
        {
            InitializeComponent();
            menuMain = new Crownwood.Magic.Menus.MenuControl();
            menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] { menuFile });

            FormRoot = root;
            FormParent = parent;

            dynamicMenus = new Hashtable();
            HashMenus();
            ApplyRoleRestrictions();

            LoadStatic();
            bool parrallelscore = false; // if parallel run then don't apply
            if ((string)Country[CountryParameterNames.BehaviouralScorecard] == "P")
            {
                parrallelscore = true;
            }

            if ((bool)Country[CountryParameterNames.BehaveApplyEodImmediate] || parrallelscore)
            {
                btnApply.Enabled = false;
                //btnApplyAll.Visible = false;
            }
            else 
            {
                btnApply.Enabled = true;
                //btnApplyAll.Visible = true;
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BehaviouralResults));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btn_clear = new System.Windows.Forms.Button();
            this.btn_selectall = new System.Windows.Forms.Button();
            this.dgAccounts = new System.Windows.Forms.DataGridView();
            this.btnExcel = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.drpDates = new System.Windows.Forms.ComboBox();
            this.btnApply = new System.Windows.Forms.Button();
            this.btnLoad = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.drpCode = new System.Windows.Forms.ComboBox();
            this.menuExit = new Crownwood.Magic.Menus.MenuCommand();
            this.menuFile = new Crownwood.Magic.Menus.MenuCommand();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgAccounts)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.SystemColors.Control;
            this.groupBox1.Controls.Add(this.btn_clear);
            this.groupBox1.Controls.Add(this.btn_selectall);
            this.groupBox1.Controls.Add(this.dgAccounts);
            this.groupBox1.Controls.Add(this.btnExcel);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.drpDates);
            this.groupBox1.Controls.Add(this.btnApply);
            this.groupBox1.Controls.Add(this.btnLoad);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.drpCode);
            this.groupBox1.Location = new System.Drawing.Point(8, 2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(776, 472);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            // 
            // btn_clear
            // 
            this.btn_clear.Location = new System.Drawing.Point(525, 40);
            this.btn_clear.Name = "btn_clear";
            this.btn_clear.Size = new System.Drawing.Size(96, 24);
            this.btn_clear.TabIndex = 138;
            this.btn_clear.Text = "Clear selection";
            this.btn_clear.Click += new System.EventHandler(this.btn_clear_Click);
            // 
            // btn_selectall
            // 
            this.btn_selectall.Location = new System.Drawing.Point(442, 40);
            this.btn_selectall.Name = "btn_selectall";
            this.btn_selectall.Size = new System.Drawing.Size(77, 24);
            this.btn_selectall.TabIndex = 137;
            this.btn_selectall.Text = "Select all";
            this.btn_selectall.Click += new System.EventHandler(this.btn_selectall_Click);
            // 
            // dgAccounts
            // 
            this.dgAccounts.AllowUserToAddRows = false;
            this.dgAccounts.AllowUserToDeleteRows = false;
            this.dgAccounts.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgAccounts.Location = new System.Drawing.Point(0, 70);
            this.dgAccounts.Name = "dgAccounts";
            this.dgAccounts.Size = new System.Drawing.Size(776, 402);
            this.dgAccounts.TabIndex = 136;
            this.dgAccounts.MouseUp += new System.Windows.Forms.MouseEventHandler(this.dgAccounts_MouseUp);
            // 
            // btnExcel
            // 
            this.btnExcel.Image = ((System.Drawing.Image)(resources.GetObject("btnExcel.Image")));
            this.btnExcel.Location = new System.Drawing.Point(738, 32);
            this.btnExcel.Name = "btnExcel";
            this.btnExcel.Size = new System.Drawing.Size(32, 32);
            this.btnExcel.TabIndex = 135;
            this.btnExcel.Click += new System.EventHandler(this.btnExcel_Click);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(16, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(130, 18);
            this.label1.TabIndex = 99;
            this.label1.Text = "Run Date:";
            // 
            // drpDates
            // 
            this.drpDates.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpDates.Location = new System.Drawing.Point(152, 13);
            this.drpDates.Name = "drpDates";
            this.drpDates.Size = new System.Drawing.Size(178, 21);
            this.drpDates.TabIndex = 98;
            this.drpDates.SelectedIndexChanged += new System.EventHandler(this.drpDates_SelectedIndexChanged);
            // 
            // btnApply
            // 
            this.btnApply.Location = new System.Drawing.Point(644, 19);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(88, 45);
            this.btnApply.TabIndex = 93;
            this.btnApply.Text = "Approve Selected";
            this.btnApply.Click += new System.EventHandler(this.Apply_Click);
            // 
            // btnLoad
            // 
            this.btnLoad.Location = new System.Drawing.Point(348, 40);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(88, 24);
            this.btnLoad.TabIndex = 25;
            this.btnLoad.Text = "Load";
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(19, 43);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(91, 21);
            this.label3.TabIndex = 23;
            this.label3.Text = "Category";
            // 
            // drpCode
            // 
            this.drpCode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpCode.Location = new System.Drawing.Point(152, 43);
            this.drpCode.Name = "drpCode";
            this.drpCode.Size = new System.Drawing.Size(178, 21);
            this.drpCode.TabIndex = 22;
            this.drpCode.SelectedIndexChanged += new System.EventHandler(this.drpCode_SelectedIndexChanged);
            // 
            // menuExit
            // 
            this.menuExit.Description = "MenuItem";
            this.menuExit.Text = "E&xit";
            // 
            // menuFile
            // 
            this.menuFile.Description = "MenuItem";
            this.menuFile.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {
            this.menuExit});
            this.menuFile.Text = "&File";
            // 
            // BehaviouralResults
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(792, 477);
            this.Controls.Add(this.groupBox1);
            this.Name = "BehaviouralResults";
            this.Text = "Behavioural Rescore Results";
            this.Load += new System.EventHandler(this.BehaviouralResults_Load);
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgAccounts)).EndInit();
            this.ResumeLayout(false);

		}
		#endregion

		private void HashMenus()
		{
			dynamicMenus[this.Name+":btnAccept"] = this.btnApply; 
		}

		private void btnLoad_Click(object sender, System.EventArgs e)
		{
			DataSet ds = null;
		
            ((MainForm)this.FormRoot).statusBar1.Text = "";
			try
			{
				Wait();
                string err = "";
                
				// get the first word which we are using as a parameter
             
                    ds=CreditManager.LoadBSCustomers(drpCode.Text,Convert.ToInt32(drpDates.SelectedValue),out err );
                    ds.Tables[0].Columns.Add("Approve", System.Type.GetType("System.Boolean"));
                    dgAccounts.DataSource = ds.Tables[0].DefaultView;
                    dgAccounts.Columns["Approve"].DisplayIndex = 0;
              
                    foreach (DataGridViewColumn col in dgAccounts.Columns)
                    {
                        if (col.Name != "Approve")
                        {
                            col.ReadOnly = true;
                        }
                    }
					
                dgAccounts.AutoResizeColumns();

				StopWait();
			}
			catch(Exception ex)
			{
				Catch(ex, Function);
			}
		}

		private void LoadStatic()	
		{
            try
            {
                ((MainForm)this.FormRoot).statusBar1.Text = "";
                DataSet ds = EodManager.GetInterfaceControl("BHSRescore","", false, out err);
                if (err.Length == 0)
                    if (ds != null)
                    {   //dgInterface.DataSource = ds.Tables["Table1"].DefaultView;
                        //StringCollection RunDates = new StringCollection();

                        //foreach (DataRow row in (ds.Tables["Table1"]).Rows)
                        //{
                        //    RunDates.Add(Convert.ToString((row["runno"])) + " " + Convert.ToString((row["datestart"])));
                        //}
                        //drpDates.DataSource = RunDates;
                        drpDates.DataSource = ds.Tables["Table1"];
                        drpDates.DisplayMember = "datestart";
                        drpDates.ValueMember = "runno";
                    }

                StringCollection Results = new StringCollection();
                Results.Add("Rejected");
                Results.Add("Lower Limit");
                Results.Add("Higher Limit");
                Results.Add("Same Limit");
                Results.Add("ScoreBand Changed");
                //Results.Add("Blocked Credit"); -- Removing as don't required -they can always use a report to see blocked credit customers. 
                //Results.Add("Parallel Run");
                drpCode.DataSource = Results;

                dgAccounts.SelectionMode = DataGridViewSelectionMode.FullRowSelect;                 
                //staticLoaded = true;
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            };

		}

        //private void dgAccounts_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        //{
        //    ((MainForm)this.FormRoot).statusBar1.Text = "";
        //    if (e.Button == MouseButtons.Right)
        //    {
        //        //if(dgAccounts.CurrentRow>= 0)
        //        //{
        //            DataGridView ctl = (DataGridView)sender;							

        //            MenuCommand m1 = new MenuCommand("Scoring Information");
        //        //GetResource("P_ACCOUNT_DETAILS")
        //            m1.Click += new System.EventHandler(this.OnReferralDetails);

        //            PopupMenu popup = new PopupMenu(); 
        //            popup.MenuCommands.AddRange(new MenuCommand[] {m1});
        //            MenuCommand selected = popup.TrackPopup(ctl.PointToScreen(new Point(e.X, e.Y)));					
        //        //}
        //    }
        //}              
		
		private void OnReferralDetails(object sender, System.EventArgs e)
		{
			try
			{
                Function = "OnReferralDetails";
				int index = dgAccounts.CurrentRow.Index;
                
				if(index >= 0)
                {
                    int ColumnIndex = dgAccounts.Columns["CustomerId"].Index;
                    string CustID = dgAccounts.Rows[index].Cells[ColumnIndex].FormattedValue.ToString();
                    ColumnIndex = dgAccounts.Columns["AccountNo"].Index;
                    string AccountNo = dgAccounts.Rows[index].Cells[ColumnIndex].FormattedValue.ToString();
                    ColumnIndex = dgAccounts.Columns["Date Proposal"].Index;
                    DateTime DateProp = Convert.ToDateTime(dgAccounts.Rows[index].Cells[ColumnIndex].FormattedValue);
                    ColumnIndex = dgAccounts.Columns["AcctType"].Index;
                    string AcctType = dgAccounts.Rows[index].Cells[ColumnIndex].FormattedValue.ToString();

                    Referral refer = new Referral(false, CustID,
                                        DateProp,
                                        AccountNo,
                                        AcctType,
                                        STL.Common.Constants.ScreenModes.SM.View,
                                        FormRoot,
                                        null, customerScreen, true);
                            ((MainForm)FormRoot).AddTabPage(refer,20);
                    
				}

                
			}
			catch(Exception ex)
			{
				Catch(ex, Function);
			}
		}
        //apply to all selected
        //private void btnAccept_Click(object sender, System.EventArgs e)
        //{
        // Wait();
        // try
        // {
        //     ((MainForm)this.FormRoot).statusBar1.Text = "";
        //     int selectedRowCount = dgAccounts.Rows.GetRowCount(DataGridViewElementStates.Selected);

        //     ((MainForm)this.FormRoot).statusBar1.Text = "";

        //     for (int i = selectedRowCount - 1; i >= 0; i--)
        //        {

        //               int ColumnIndex = dgAccounts.Columns["CustomerId"].Index;
        //                string CustID = dgAccounts.SelectedRows[i].Cells[ColumnIndex].FormattedValue.ToString();
        //                //string CustID = dgAccounts.Rows[index].Cells[ColumnIndex].FormattedValue.ToString();
        //                ApplyRescore(CustID);
                        
        //            }


        //     ((MainForm)this.FormRoot).statusBar1.Text = "Selected Customers Scores marked for applying. Will be applied after Eod Job";
				
        
        //    }
        //    catch(Exception ex)
        //    {
        //        Catch(ex, Function);
        //    }	
        //    finally
        //    {
        //        StopWait();
        //    }
        //}
        private void ApplyRescore(string CustomerId)
        {

            string err="";
            try
            {
                ((MainForm)this.FormRoot).statusBar1.Text = "";
                
                CreditManager.ApplyBSRescore(CustomerId,Convert.ToInt32(drpDates.SelectedValue),out err );
             	if(err.Length>0)
				{
					ShowError(err);
					
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
		
        //private void btnClear_Click(object sender, System.EventArgs e)
        //{
        //    drpCode.SelectedIndex = 0;
        //    ((MainForm)this.FormRoot).statusBar1.Text = "";
        //    //dgAccounts.TableStyles.Clear();
        //    dgAccounts.DataSource = null;
        //}

		private void drpCode_SelectedIndexChanged(object sender, System.EventArgs e)
		{
            dgAccounts.DataSource = null;
            ((MainForm)this.FormRoot).statusBar1.Text = "";
	
		}

        private void BehaviouralResults_Load(object sender, EventArgs e)
        {

        }

        private void btnExcel_Click(object sender, EventArgs e)
        {
            string filePath = STL.PL.Utils.ReportUtils.CreateCSVFile(dgAccounts, "Save Report to Excel");

            if (filePath.Length.Equals(0))
                MessageBox.Show("Save Failed");

            try
            {
                STL.PL.Utils.ReportUtils.OpenExcelCSV(filePath);
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
        }

        //private void dgAccounts_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
        //{
        //    if (e.Button == MouseButtons.Right)
        //    {
        //        //if(dgAccounts.CurrentRow>= 0)
        //        //{
        //        DataGridView ctl = (DataGridView)sender;

        //        MenuCommand m1 = new MenuCommand("Underwriters Screen");
        //        //GetResource("P_ACCOUNT_DETAILS")
        //        m1.Click += new System.EventHandler(this.OnReferralDetails);

        //        PopupMenu popup = new PopupMenu();
        //        popup.MenuCommands.AddRange(new MenuCommand[] { m1 });
        //        MenuCommand selected = popup.TrackPopup(ctl.PointToScreen(new Point(e.X, e.Y)));
                


        //        //}
        //    }
        //}

        private void dgAccounts_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                //if(dgAccounts.CurrentRow>= 0)
                //{
                DataGridView ctl = (DataGridView)sender;

                MenuCommand m1 = new MenuCommand("Underwriters Screen");
                //GetResource("P_ACCOUNT_DETAILS")
                m1.Click += new System.EventHandler(this.OnReferralDetails);

                PopupMenu popup = new PopupMenu();
                popup.MenuCommands.AddRange(new MenuCommand[] { m1 });
                MenuCommand selected = popup.TrackPopup(ctl.PointToScreen(new Point(e.X, e.Y)));
                //}
            }

        }
        // Apply to all accounts
        private void Apply_Click(object sender, EventArgs e)
        {
            Wait();
            try
            {
                foreach (DataGridViewRow dgvr in dgAccounts.Rows)
                {
                    if (dgvr.Cells["Approve"].Value != DBNull.Value && Convert.ToBoolean(dgvr.Cells["Approve"].Value) == true)
                    {
                       //int ColumnIndex = dgAccounts.Columns["CustomerId"].Index;
                        string CustID = dgvr.Cells["CustomerId"].Value.ToString();
                        ApplyRescore(CustID);
                    }
                }
                ((MainForm)this.FormRoot).statusBar1.Text = " Scores marked for applying. Will be applied after Eod Job";



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

        private void drpDates_SelectedIndexChanged(object sender, EventArgs e)
        {

            dgAccounts.DataSource = null;

            ((MainForm)this.FormRoot).statusBar1.Text = "";
				
        }

       

        //private void dgAccounts_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        //{
        //    try
        //    {
        //        //bool apply;

        //        //if (e.ColumnIndex == ((DataView)dgAccounts.DataSource).Table.Columns.IndexOf("Approve") && e.RowIndex >= 0)
        //        //{
        //        //    if (dgAccounts[e.ColumnIndex, e.RowIndex].Value == DBNull.Value || Convert.ToBoolean(dgAccounts[e.ColumnIndex, e.RowIndex].Value) == false)
        //        //    {
        //        //        apply = true;
        //        //    }
        //        //    else
        //        //    {
        //        //        apply = false;
        //        //    }

        //        //    foreach (DataGridViewRow row in dgAccounts.SelectedRows)
        //        //    {
        //        //        row.Cells["Approve"].Value = apply;
        //        //    }
        //        //}
        //    }
        //    catch (Exception ex)
        //    {
        //        Catch(ex, Function);
        //    }
        //}

        private void btn_selectall_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dgAccounts.Rows)
            {
                row.Cells["Approve"].Value = true;
            }
        }

        private void btn_clear_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dgAccounts.Rows)
            {
                row.Cells["Approve"].Value = false;
            }
        }
   
	}
}

