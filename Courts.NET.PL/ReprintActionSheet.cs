using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Collections.Specialized;
using System.Web.Services.Protocols;
using System.Xml;
using System.Data;
using STL.PL.WS5;
using STL.PL.WS1;
using STL.PL.WS2;
using STL.Common.Constants.TableNames;
using STL.Common.Constants.TabPageNames;
using STL.Common.Constants.ColumnNames;
using STL.Common.Static;
using STL.Common;
using Crownwood.Magic.Menus;
using mshtml;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using Blue.Cosacs.Shared;

namespace STL.PL
{
	/// <summary>
	/// Allows the date of allocation to be cleared for a debt collector.
	/// The action sheet can then be re-printed. All accounts are listed
	/// for a debt collector, and individual accounts or the entire batch
	/// can be selected for re-printing.
	/// </summary>
	public class ReprintActionSheet : CommonForm
	{
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.ComboBox drpEmpType;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.ComboBox drpEmpName;
		private System.Windows.Forms.GroupBox groupBox2;
		private Crownwood.Magic.Menus.MenuCommand menuFile;
		private string error = "";
		private bool staticLoaded = false;
		private bool empTypesLoaded = false;
		private System.Windows.Forms.DataGrid dgAllocations;
		private System.Windows.Forms.DataGrid dgDetails;
		private System.Windows.Forms.Button btnPrintBatch;
		private System.Windows.Forms.Button btnPrintAccounts;
		private System.Windows.Forms.Button btnClear;
		private System.Windows.Forms.ComboBox drpBranch;
		private System.Windows.Forms.Label label5;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public ReprintActionSheet(TranslationDummy d)
		{
			InitializeComponent();
		}

		public ReprintActionSheet(Form root, Form parent)
		{
			InitializeComponent();
			FormRoot = root;
			FormParent = parent;

			InitialiseStaticData();

			menuMain = new Crownwood.Magic.Menus.MenuControl();
			menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {menuFile});
		}


		private void InitialiseStaticData()	
		{		
			try
			{
				Function = "BStaticDataManager::GetDropDownData";
				XmlUtilities xml = new XmlUtilities();
				XmlDocument dropDowns = new XmlDocument();
				dropDowns.LoadXml("<DROP_DOWNS></DROP_DOWNS>");
			
				StringCollection branchNos = new StringCollection(); 	
				StringCollection empTypes = new StringCollection();
				empTypes.Add("Staff Types");

				if(StaticData.Tables[TN.EmployeeTypes]==null)
					dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns,TN.EmployeeTypes,
						new string[] {"ET1", "L"}));    

				if(StaticData.Tables[TN.BranchNumber] == null)
					dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns,TN.BranchNumber, null));
				
				if(dropDowns.DocumentElement.ChildNodes.Count>0)
				{
					DataSet ds = StaticDataManager.GetDropDownData(dropDowns.DocumentElement, out error);
					if(error.Length>0)
						ShowError(error);
					else
					{
						foreach(DataTable dt in ds.Tables)
							StaticData.Tables[dt.TableName] = dt;
					}
				}

			     foreach (DataRow row in CollectionsManager.RolesGet((int)CosacsPermissionEnum.Bailiff).Tables[0].Rows)
            {
                string str = string.Format("{0} : {1}", row[0], row[1]);
						// Only show employee types with 'reference' column set
						empTypes.Add(str.ToUpper());
				}
				drpEmpType.DataSource = empTypes;

				foreach(DataRow row in ((DataTable)StaticData.Tables[TN.BranchNumber]).Rows)
				{
					branchNos.Add(Convert.ToString(row["branchno"]));
				}
				drpBranch.DataSource = branchNos;
				drpBranch.Text = Config.BranchCode;

				staticLoaded = true;
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

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.drpBranch = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnPrintBatch = new System.Windows.Forms.Button();
            this.btnPrintAccounts = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.drpEmpType = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.drpEmpName = new System.Windows.Forms.ComboBox();
            this.dgDetails = new System.Windows.Forms.DataGrid();
            this.dgAllocations = new System.Windows.Forms.DataGrid();
            this.menuFile = new Crownwood.Magic.Menus.MenuCommand();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgDetails)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgAllocations)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.drpBranch);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.groupBox2);
            this.groupBox1.Controls.Add(this.drpEmpType);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.drpEmpName);
            this.groupBox1.Controls.Add(this.dgDetails);
            this.groupBox1.Controls.Add(this.dgAllocations);
            this.groupBox1.Location = new System.Drawing.Point(16, 16);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(760, 440);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            // 
            // drpBranch
            // 
            this.drpBranch.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpBranch.Location = new System.Drawing.Point(88, 40);
            this.drpBranch.Name = "drpBranch";
            this.drpBranch.Size = new System.Drawing.Size(48, 21);
            this.drpBranch.TabIndex = 88;
            this.drpBranch.SelectedIndexChanged += new System.EventHandler(this.drpBranch_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(88, 24);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(40, 16);
            this.label5.TabIndex = 89;
            this.label5.Text = "Branch";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnPrintBatch);
            this.groupBox2.Controls.Add(this.btnPrintAccounts);
            this.groupBox2.Controls.Add(this.btnClear);
            this.groupBox2.Location = new System.Drawing.Point(512, 136);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(136, 176);
            this.groupBox2.TabIndex = 77;
            this.groupBox2.TabStop = false;
            // 
            // btnPrintBatch
            // 
            this.btnPrintBatch.Location = new System.Drawing.Point(24, 24);
            this.btnPrintBatch.Name = "btnPrintBatch";
            this.btnPrintBatch.Size = new System.Drawing.Size(88, 35);
            this.btnPrintBatch.TabIndex = 4;
            this.btnPrintBatch.Text = "Update Batch For Re-Print";
            this.btnPrintBatch.Click += new System.EventHandler(this.btnPrintBatch_Click);
            // 
            // btnPrintAccounts
            // 
            this.btnPrintAccounts.Location = new System.Drawing.Point(24, 77);
            this.btnPrintAccounts.Name = "btnPrintAccounts";
            this.btnPrintAccounts.Size = new System.Drawing.Size(88, 35);
            this.btnPrintAccounts.TabIndex = 5;
            this.btnPrintAccounts.Text = "Update A/C\'s For Re-Print";
            this.btnPrintAccounts.Click += new System.EventHandler(this.btnPrintAccounts_Click);
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(24, 128);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(88, 35);
            this.btnClear.TabIndex = 6;
            this.btnClear.Text = "Clear";
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // drpEmpType
            // 
            this.drpEmpType.ItemHeight = 13;
            this.drpEmpType.Location = new System.Drawing.Point(184, 40);
            this.drpEmpType.Name = "drpEmpType";
            this.drpEmpType.Size = new System.Drawing.Size(176, 21);
            this.drpEmpType.TabIndex = 73;
            this.drpEmpType.SelectedIndexChanged += new System.EventHandler(this.drpEmpType_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.SystemColors.Control;
            this.label2.Location = new System.Drawing.Point(184, 24);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(96, 16);
            this.label2.TabIndex = 75;
            this.label2.Text = "Employee Type";
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(400, 24);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(88, 16);
            this.label4.TabIndex = 76;
            this.label4.Text = "Employee Name";
            // 
            // drpEmpName
            // 
            this.drpEmpName.Enabled = false;
            this.drpEmpName.ItemHeight = 13;
            this.drpEmpName.Location = new System.Drawing.Point(400, 40);
            this.drpEmpName.Name = "drpEmpName";
            this.drpEmpName.Size = new System.Drawing.Size(176, 21);
            this.drpEmpName.TabIndex = 74;
            this.drpEmpName.SelectedIndexChanged += new System.EventHandler(this.drpEmpName_SelectedIndexChanged);
            // 
            // dgDetails
            // 
            this.dgDetails.CaptionText = "Customer Details";
            this.dgDetails.DataMember = "";
            this.dgDetails.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgDetails.Location = new System.Drawing.Point(88, 264);
            this.dgDetails.Name = "dgDetails";
            this.dgDetails.ReadOnly = true;
            this.dgDetails.Size = new System.Drawing.Size(320, 152);
            this.dgDetails.TabIndex = 1;
            this.dgDetails.MouseUp += new System.Windows.Forms.MouseEventHandler(this.dgDetails_MouseUp);
            // 
            // dgAllocations
            // 
            this.dgAllocations.CaptionText = "Allocation Details";
            this.dgAllocations.DataMember = "";
            this.dgAllocations.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgAllocations.Location = new System.Drawing.Point(88, 88);
            this.dgAllocations.Name = "dgAllocations";
            this.dgAllocations.ReadOnly = true;
            this.dgAllocations.Size = new System.Drawing.Size(320, 152);
            this.dgAllocations.TabIndex = 0;
            this.dgAllocations.MouseUp += new System.Windows.Forms.MouseEventHandler(this.dgAllocations_MouseUp);
            // 
            // menuFile
            // 
            this.menuFile.Description = "MenuItem";
            this.menuFile.Text = "&File";
            // 
            // ReprintActionSheet
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(792, 477);
            this.Controls.Add(this.groupBox1);
            this.Name = "ReprintActionSheet";
            this.Text = "Re-Print Debt Collectors Action Sheet";
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgDetails)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgAllocations)).EndInit();
            this.ResumeLayout(false);

		}
		#endregion

		private void drpEmpType_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			string empType;
			string empTypeStr;
			string empTitle;
			DataSet ds = null;
			int branchNo = 0;

			try
			{
				Wait();
				if(staticLoaded && drpEmpType.SelectedIndex != 0)
				{
					empTypesLoaded = false;

					empTypeStr = (string)drpEmpType.SelectedItem;
					int index = empTypeStr.IndexOf(":");
					empType = empTypeStr.Substring(0, index - 1);
					
					int len = empTypeStr.Length - 1;
					empTitle = empTypeStr.Substring(index + 1, len - index);

					StringCollection staff = new StringCollection();
					staff.Add(empTitle);

					branchNo = Convert.ToInt32(drpBranch.Text);;

					ds = Login.GetSalesStaffByType(empType, branchNo, out error);

					if(error.Length>0)
						ShowError(error);
					else
					{
						foreach(DataTable dt in ds.Tables)
						{
							if(dt.TableName==TN.SalesStaff)
							{
								foreach(DataRow row in dt.Rows)
								{
									string str = Convert.ToString(row.ItemArray[0]) + " : "+(string)row.ItemArray[1];
									staff.Add(str.ToUpper());
								}
							}
						}
						drpEmpName.DataSource = staff;
						drpEmpName.Enabled = true;
						empTypesLoaded = true;

                        //IP - 24/04/08 - UAT(301) - If a different 'Emmployee Type' is selected
                        //from the 'Employee Type' drop down, then need to clear the details
                        //displayed on both grids.

                        dgAllocations.DataSource = null;
                        dgDetails.DataSource = null;
					}
				}
				else if (drpEmpType.SelectedIndex == 0)
				{
					drpEmpName.DataSource = null;
					drpEmpName.Enabled = false;
					drpEmpName.Text = "";
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

		private void drpEmpName_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			try
			{
				Wait();
				if(empTypesLoaded && drpEmpType.SelectedIndex != 0)
				{
					DataSet ds = AccountManager.LoadAllocationsForReprint(SelectedEmpeeNo(), out error);
					if(error.Length>0)
						ShowError(error);
					else
					{
						dgAllocations.DataSource = ds.Tables[TN.Allocations].DefaultView;
                        dgDetails.DataSource = null;    // clear Detail grid        (jec 28/03/07)

						if(((DataView)dgAllocations.DataSource).Count > 0 && dgAllocations.TableStyles.Count==0)
						{
							DataGridTableStyle tabStyle = new DataGridTableStyle();
							tabStyle.MappingName = ds.Tables[TN.Allocations].TableName;
							dgAllocations.TableStyles.Add(tabStyle);

							tabStyle.GridColumnStyles[CN.EmployeeNo].Width = 0;

							tabStyle.GridColumnStyles[CN.DateAlloc].Width = 110;
							tabStyle.GridColumnStyles[CN.DateAlloc].HeaderText = GetResource("T_DATEALLOC");

							tabStyle.GridColumnStyles[CN.Total].Width = 154; //IP
							tabStyle.GridColumnStyles[CN.Total].HeaderText = GetResource("T_NUMALLOCATED");
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
			}
		}

		private int SelectedEmpeeNo()
		{
			int empeeNo = 0;
			if (drpEmpName.DataSource != null && drpEmpName.SelectedIndex > 0)
			{
				int index = ((string)drpEmpName.SelectedItem).IndexOf(":");
				string empeeNoStr = ((string)drpEmpName.SelectedItem).Substring(0, index - 1);
				empeeNo = Convert.ToInt32(empeeNoStr);
			}
			return empeeNo;
		}

		private void dgAllocations_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			try
			{
				Wait();


                // uat301 rdb 17/06/08 detect if grid was sorted
                DataGrid sourceGrid = (DataGrid)sender;

                // Perform hit-test
                DataGrid.HitTestInfo hitTestInfo = sourceGrid.HitTest(e.X, e.Y);

                // Check if a column header was clicked
                if (hitTestInfo.Type == DataGrid.HitTestType.ColumnHeader)
                {
                    //dgAllocations.CurrentRowIndex = -1;
                    dgDetails.DataSource = null;
                }
                else
                {


                    


                    dgDetails.TableStyles.Clear();

                    DataView dv = (DataView)dgAllocations.DataSource;
                    int i = dgAllocations.CurrentRowIndex;

                    if (i >= 0)
                    {
                        DataSet ds = AccountManager.LoadAllocationDetails((int)dv[i][CN.EmployeeNo], (DateTime)dv[i][CN.DateAlloc], out error);
                        if (error.Length > 0)
                            ShowError(error);
                        else
                        {
                            dgDetails.DataSource = ds.Tables[TN.Allocations].DefaultView;

                            if (((DataView)dgDetails.DataSource).Count > 0 && dgDetails.TableStyles.Count == 0)
                            {
                                DataGridTableStyle tabStyle = new DataGridTableStyle();
                                tabStyle.MappingName = ds.Tables[TN.Allocations].TableName;
                                dgDetails.TableStyles.Add(tabStyle);

                                tabStyle.GridColumnStyles[CN.EmployeeNo].Width = 0;
                                tabStyle.GridColumnStyles[CN.DateAlloc].Width = 0;

                                tabStyle.GridColumnStyles[CN.AcctNo].Width = 90;
                                tabStyle.GridColumnStyles[CN.AcctNo].HeaderText = GetResource("T_ACCTNO");

                                tabStyle.GridColumnStyles[CN.CustomerName].Width = 174; //IP - 10/06/08 - Format screen
                                tabStyle.GridColumnStyles[CN.CustomerName].HeaderText = GetResource("T_CUSTOMERNAME");
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
			}
		}

		private void btnPrintBatch_Click(object sender, System.EventArgs e)
		{
			if(dgAllocations.DataSource != null)
				UpdateForReprint(dgAllocations, true);
		}

		private void btnPrintAccounts_Click(object sender, System.EventArgs e)
		{
			if(dgDetails.DataSource != null)
				UpdateForReprint(dgDetails, false);
		}

		private void btnClear_Click(object sender, System.EventArgs e)
		{
			staticLoaded = false;
			empTypesLoaded = false;
			dgAllocations.DataSource = null;
			dgDetails.DataSource = null;
			drpEmpType.DataSource = null;
			drpEmpName.DataSource = null;
			InitialiseStaticData();
		}

		private void UpdateForReprint(DataGrid grid, bool batch)
		{
			try
			{
				Wait();
				string acctNo = "";
				DataView dv = (DataView)grid.DataSource;
				int count = dv.Count;

				for (int i = count-1; i >=0 ; i--)
				{
					if(grid.IsSelected(i))
					{
						if(batch)
							acctNo = "";
						else
							acctNo = (string)dv[i][CN.AcctNo];

						AccountManager.UpdateAllocForReprint(acctNo,(int)dv[i][CN.EmployeeNo], 
															(DateTime)dv[i][CN.DateAlloc], 
															batch, out error);
						if(error.Length > 0)
							ShowError(error);
						else
							dv[i][CN.EmployeeNo]= -1;

					}
				}

				for (int i = count-1; i >=0 ; i--)
				{
					if ((int)dv[i][CN.EmployeeNo] == -1)
						dv.Delete(i); 
				}

                //IP - 24/04/08 - UAT(301) Once the row in the dgAllocations grid has been removed 
                //clear the details from the dgDetails grid.

                dgDetails.DataSource = null;
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

		private void dgDetails_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if(dgDetails.CurrentRowIndex >= 0)
			{
				dgDetails.Select(dgDetails.CurrentCell.RowNumber);

				if (e.Button == MouseButtons.Right)
				{					
					DataGrid ctl = (DataGrid)sender;							

					MenuCommand m1 = new MenuCommand(GetResource("P_ACCOUNT_DETAILS"));

					m1.Click += new System.EventHandler(this.OnAccountDetails);

					PopupMenu popup = new PopupMenu(); 
					popup.MenuCommands.AddRange(new MenuCommand[] {m1});

					MenuCommand selected = popup.TrackPopup(ctl.PointToScreen(new Point(e.X, e.Y)));					
				}
			}
		}

		private void OnAccountDetails(object sender, System.EventArgs e)
		{
			try
			{
				Function = "OnAccountDetails";
				int index = dgDetails.CurrentRowIndex;

				if(index>=0)
				{
					string acctNo = (string)dgDetails[index, 0];
					AccountDetails details = new AccountDetails(acctNo, FormRoot, this);
					((MainForm)this.FormRoot).AddTabPage(details,7);
				}
			}
			catch(Exception ex)
			{
				Catch(ex, Function);
			}
		}

        private void drpBranch_SelectedIndexChanged(object sender, EventArgs e)
        {
            //IP - 24/04/08 - UAT(301) - When a different branch has been selected
            //load the employee names for the selected type for the selected branch.

            drpEmpType_SelectedIndexChanged(null, null);
        }
	}
}
