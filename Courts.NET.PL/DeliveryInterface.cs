using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using STL.Common;
using System.Data;
using System.Web.Services.Protocols;
using STL.PL.WS9;
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
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;


namespace STL.PL
{
	/// <summary>
	/// Summary description for DeliveryInterface.
	/// </summary>
	public class DeliveryInterface : CommonForm
	{
		private System.Windows.Forms.GroupBox groupBox4;
		private System.Windows.Forms.Label label27;
		private System.Windows.Forms.TextBox txtCFDelTotal;
		private System.Windows.Forms.DataGrid dgBranch;
		private System.Windows.Forms.DataGrid dgDelDetails;
		private string branchFilter = "";
		private string err = "";
		public System.Windows.Forms.Button btnExcel;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public DeliveryInterface(Form root, Form parent, int runNo, int branchNo,bool liveDatabase)
		{
			FormRoot = root;
			FormParent = parent;
			InitializeComponent();

			if(branchNo > 0)
				branchFilter = CN.BranchNo + " = " + branchNo.ToString(); 

			LoadTotals(runNo,liveDatabase);
			txtCFDelTotal.BackColor = SystemColors.Window;
		}

		private void LoadTotals(int runNo,bool liveDatabase)
		{
			decimal delTotal = 0;
			decimal total = 0;

			try
			{
				DataSet ds = AccountManager.SUCBGetDelTotals(runNo, liveDatabase ,out delTotal, out err);
				if(err.Length > 0)
					ShowError(err);
				else
				{
					if(ds != null)
					{
						dgBranch.DataSource = ds.Tables[TN.Deliveries].DefaultView; 
				
						if(branchFilter.Length > 0)
							((DataView)dgBranch.DataSource).RowFilter = branchFilter;

						if(dgBranch.TableStyles.Count == 0)
						{
							DataGridTableStyle tabStyle = new DataGridTableStyle();
							tabStyle.MappingName = ds.Tables[TN.Deliveries].TableName;
							dgBranch.TableStyles.Add(tabStyle);

							tabStyle.GridColumnStyles[CN.BranchNo].Width = 70;
							tabStyle.GridColumnStyles[CN.BranchNo].HeaderText = GetResource("T_BRANCH");

							tabStyle.GridColumnStyles[CN.TransValue].Width = 100;
							tabStyle.GridColumnStyles[CN.TransValue].HeaderText = GetResource("T_VALUE");
							((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.TransValue]).Format = DecimalPlaces;

                            //tabStyle.GridColumnStyles[CN.Runno].Width = 0;                                            //IP - 20/02/12 - #9423 - CR8262
						}

						foreach(DataTable dt in ds.Tables)
						{
							foreach(DataRow row in dt.Rows)
							{
								if(DBNull.Value!=row[CN.TransValue])
									total += ((decimal)row[CN.TransValue]);
						
								txtCFDelTotal.Text = total.ToString(DecimalPlaces);
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DeliveryInterface));
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.label27 = new System.Windows.Forms.Label();
            this.txtCFDelTotal = new System.Windows.Forms.TextBox();
            this.dgBranch = new System.Windows.Forms.DataGrid();
            this.dgDelDetails = new System.Windows.Forms.DataGrid();
            this.btnExcel = new System.Windows.Forms.Button();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgBranch)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgDelDetails)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.label27);
            this.groupBox4.Controls.Add(this.txtCFDelTotal);
            this.groupBox4.Location = new System.Drawing.Point(8, 8);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(272, 64);
            this.groupBox4.TabIndex = 30;
            this.groupBox4.TabStop = false;
            // 
            // label27
            // 
            this.label27.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World);
            this.label27.Location = new System.Drawing.Point(24, 32);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(80, 16);
            this.label27.TabIndex = 26;
            this.label27.Text = "Delivery Total";
            this.label27.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // txtCFDelTotal
            // 
            this.txtCFDelTotal.Location = new System.Drawing.Point(112, 29);
            this.txtCFDelTotal.Name = "txtCFDelTotal";
            this.txtCFDelTotal.ReadOnly = true;
            this.txtCFDelTotal.Size = new System.Drawing.Size(112, 20);
            this.txtCFDelTotal.TabIndex = 23;
            // 
            // dgBranch
            // 
            this.dgBranch.CaptionText = "Branch Totals";
            this.dgBranch.DataMember = "";
            this.dgBranch.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgBranch.Location = new System.Drawing.Point(8, 88);
            this.dgBranch.Name = "dgBranch";
            this.dgBranch.ReadOnly = true;
            this.dgBranch.Size = new System.Drawing.Size(280, 152);
            this.dgBranch.TabIndex = 31;
            this.dgBranch.Navigate += new System.Windows.Forms.NavigateEventHandler(this.dgBranch_Navigate);
            this.dgBranch.MouseUp += new System.Windows.Forms.MouseEventHandler(this.dgBranch_MouseUp);
            // 
            // dgDelDetails
            // 
            this.dgDelDetails.CaptionText = "Delivery Details";
            this.dgDelDetails.DataMember = "";
            this.dgDelDetails.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgDelDetails.Location = new System.Drawing.Point(8, 248);
            this.dgDelDetails.Name = "dgDelDetails";
            this.dgDelDetails.ReadOnly = true;
            this.dgDelDetails.Size = new System.Drawing.Size(704, 208);
            this.dgDelDetails.TabIndex = 32;
            // 
            // btnExcel
            // 
            this.btnExcel.Enabled = false;
            this.btnExcel.Image = ((System.Drawing.Image)(resources.GetObject("btnExcel.Image")));
            this.btnExcel.Location = new System.Drawing.Point(712, 248);
            this.btnExcel.Name = "btnExcel";
            this.btnExcel.Size = new System.Drawing.Size(32, 32);
            this.btnExcel.TabIndex = 57;
            this.btnExcel.Click += new System.EventHandler(this.btnExcel_Click);
            // 
            // DeliveryInterface
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(792, 470);
            this.Controls.Add(this.btnExcel);
            this.Controls.Add(this.dgDelDetails);
            this.Controls.Add(this.dgBranch);
            this.Controls.Add(this.groupBox4);
            this.Name = "DeliveryInterface";
            this.Text = "Delivery Interface";
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgBranch)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgDelDetails)).EndInit();
            this.ResumeLayout(false);

		}
		#endregion

		private void dgBranch_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			try
			{
				Wait();

				int index = dgBranch.CurrentRowIndex;
				if(index >= 0)
				{
                    //int runno = 0;                                        //IP - 20/02/12 - #9423 - CR8262
                    var datetrans = string.Empty;                           //IP - 20/02/12 - #9423 - CR8262

					string branchNo = "";

					DataView dv = (DataView)dgBranch.DataSource;
                    //runno = (int)dv[index][CN.Runno];                     //IP - 20/02/12 - #9423 - CR8262
                    datetrans = Convert.ToString(dv[index][CN.TransactionDate2]);    //IP - 20/02/12 - #9423 - CR8262
					branchNo = (string)dv[index][CN.BranchNo];
					branchNo += "%";

                    //DataSet ds = AccountManager.SUCBGetDelDetails(runno, branchNo, out err);
                    DataSet ds = AccountManager.SUCBGetDelDetails(datetrans, branchNo, out err);        //IP - 20/02/12 - #9423 - CR8262

					if(err.Length > 0)
						ShowError(err);
					else
					{
						if(ds != null)
						{
							btnExcel.Enabled = ds.Tables[TN.Deliveries].Rows.Count > 0;
							dgDelDetails.DataSource = ds.Tables[TN.Deliveries].DefaultView; 
				
							if(dgDelDetails.TableStyles.Count==0)
							{
								DataGridTableStyle tabStyle = new DataGridTableStyle();
								tabStyle.MappingName = ds.Tables[TN.Deliveries].TableName;
								dgDelDetails.TableStyles.Add(tabStyle);

								tabStyle.GridColumnStyles[CN.AcctNo].Width = 85;
								tabStyle.GridColumnStyles[CN.AcctNo].HeaderText = GetResource("T_ACCOUNTNO");

								tabStyle.GridColumnStyles[CN.ItemNo].Width = 90;
								tabStyle.GridColumnStyles[CN.ItemNo].HeaderText = GetResource("T_PRODCODE");
						
								tabStyle.GridColumnStyles[CN.StockLocn].Width = 80;
								tabStyle.GridColumnStyles[CN.StockLocn].HeaderText = GetResource("T_STOCKLOCN");

								tabStyle.GridColumnStyles[CN.TransValue].Width = 120;
								tabStyle.GridColumnStyles[CN.TransValue].HeaderText = GetResource("T_VALUE");
								tabStyle.GridColumnStyles[CN.TransValue].Alignment = HorizontalAlignment.Right;
								((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.TransValue]).Format = DecimalPlaces;

								tabStyle.GridColumnStyles[CN.ContractNo].Width = 90;
								tabStyle.GridColumnStyles[CN.ContractNo].HeaderText = GetResource("T_CONTRACTNO");

								tabStyle.GridColumnStyles[CN.RetItemNo].Width = 90;
								tabStyle.GridColumnStyles[CN.RetItemNo].HeaderText = GetResource("T_RETITEM");

								tabStyle.GridColumnStyles[CN.RetStockLocn].Width = 100;
								tabStyle.GridColumnStyles[CN.RetStockLocn].HeaderText = GetResource("T_RETLOCN");

                                tabStyle.GridColumnStyles[CN.StoreCard].Width = 100;                                                        //IP - 20/02/12 - #9423 - CR8262
                                tabStyle.GridColumnStyles[CN.StoreCard].HeaderText = GetResource("T_STORECARD");
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

		private void btnExcel_Click(object sender, System.EventArgs e)
		{
			try
			{
				Wait();

				/* save the current data grid contents to a CSV */
				string comma = ",";
				string path = "";
				DataView dv = (DataView)dgDelDetails.DataSource;

				SaveFileDialog save = new SaveFileDialog();
				save.Filter = "csv files (*.csv)|*.csv|All files (*.*)|*.*" ;
				save.Title = "Save Delivery Details";
				save.CreatePrompt = true;

				if(save.ShowDialog() == DialogResult.OK)
				{
					path = save.FileName;
					FileInfo fi = new FileInfo(path);
					FileStream fs = fi.OpenWrite();
		
					string line = GetResource("T_ACCOUNTNO") + comma +
						GetResource("T_PRODCODE") + comma +
						GetResource("T_STOCKLOCN") + comma +
                        GetResource("T_BUFFNO") + comma +
                        GetResource("T_QUANTITY") + comma +
						GetResource("T_VALUE") + comma +
						GetResource("T_CONTRACTNO") + comma +
						GetResource("T_RETITEM") + comma +
						GetResource("T_RETLOCN") + comma +
                        GetResource("T_STORECARD") + comma +                                    //IP - 20/02/12 - #9423 - CR8262
						Environment.NewLine + Environment.NewLine;	
					byte[] blob = System.Text.Encoding.UTF8.GetBytes(line);
					fs.Write(blob,0,blob.Length);
		
					foreach(DataRowView row in dv)
					{					
						line =	"'"+row[CN.AcctNo]+"'" + comma +
							Convert.ToString(row[CN.ItemNo]) + comma +
							Convert.ToString(row[CN.StockLocn]) + comma +
                            Convert.ToString(row[CN.BuffNo]) + comma +
                            Convert.ToString(row[CN.Quantity]) + comma +
							((decimal)row[CN.TransValue]).ToString(DecimalPlaces).Replace(",","") + comma +
							Convert.ToString(row[CN.ContractNo]) + comma +
							Convert.ToString(row[CN.RetItemNo]) + comma +
							Convert.ToString(row[CN.RetStockLocn]) + comma +
                            Convert.ToString(row[CN.StoreCard]) + comma +                       //IP - 20/02/12 - #9423 - CR8262
							Environment.NewLine;	

						blob = System.Text.Encoding.UTF8.GetBytes(line);
						fs.Write(blob,0,blob.Length);
					}
					fs.Close();						

					/* attempt to launch Excel. May get a COMException if Excel is not 
						* installed */
					try
					{
						/* we have to use Reflection to call the Open method because 
							* the methods have different argument lists for the 
							* different versions of Excel - JJ */
						object[] args = null;
						Excel.Application excel = new Excel.Application();

						if(excel.Version == "10.0")	/* Excel2002 */
							args = new object[]{path, 0, true, 2, "", "", true, Excel.XlPlatform.xlWindows, comma, true, false, 0, true, false, false };
						else
							args = new object[]{path, 0, true, 2, "", "", true, Excel.XlPlatform.xlWindows, comma, true, false, 0, true };

						/* Retrieve the Workbooks property */
						object wbs = excel.GetType().InvokeMember("Workbooks", BindingFlags.Public|BindingFlags.GetField|BindingFlags.GetProperty, null, excel, new Object[] {});

						/* call the Open method */
						object wb = wbs.GetType().InvokeMember("Open", BindingFlags.Public | BindingFlags.InvokeMethod, null, wbs, args);

						excel.Visible = true;
					}
					catch(COMException)
					{
						/*change back slashes to forward slashes so the path doesn't
							* get split into multiple lines */
						ShowInfo("M_EXCELNOTFOUND", new object[] {path.Replace("\\", "/")});
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

        private void dgBranch_Navigate(object sender, NavigateEventArgs ne)
        {

        }
	}
}
