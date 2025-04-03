using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using STL.Common;
using STL.Common.Constants.ColumnNames;
using STL.Common.Static;
using System.Data;
using System.Data.SqlClient;
using STL.Common.Constants.TableNames;
using System.Xml;

namespace STL.PL
{
	/// <summary>
	/// Common user control to list the cashier totals by each payment method.
	/// Used by Cashier Disbursments and Cashier Totals.
	/// </summary>
	public class CashierTotalsBreakdownS : CommonForm
	{
		private System.Windows.Forms.DataGrid dgBreakdown;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private int _id = 0;
        private new string Error = "";

		public CashierTotalsBreakdownS(int id)
		{
			InitializeComponent();
			_id = id;			
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
			this.dgBreakdown = new System.Windows.Forms.DataGrid();
			((System.ComponentModel.ISupportInitialize)(this.dgBreakdown)).BeginInit();
			this.SuspendLayout();
			// 
			// dgBreakdown
			// 
			this.dgBreakdown.CaptionVisible = false;
			this.dgBreakdown.DataMember = "";
			this.dgBreakdown.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dgBreakdown.HeaderForeColor = System.Drawing.SystemColors.ControlText;
			this.dgBreakdown.Name = "dgBreakdown";
			this.dgBreakdown.ReadOnly = true;
			this.dgBreakdown.Size = new System.Drawing.Size(546, 160);
			this.dgBreakdown.TabIndex = 1;
			this.dgBreakdown.TabStop = false;
			// 
			// CashierTotalsBreakdown
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(546, 160);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.dgBreakdown});
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "CashierTotalsBreakdown";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Cashier Totals By Pay Method";
			this.Load += new System.EventHandler(this.CashierTotalsBreakdown_Load);
			((System.ComponentModel.ISupportInitialize)(this.dgBreakdown)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion

		private void CashierTotalsBreakdown_Load(object sender, System.EventArgs e)
		{
			try
			{
				Wait();
				DataSet ds = PaymentManager.GetCashierTotalsBreakdown(_id, out Error);
				if(Error.Length>0)
					ShowError(Error);
				else
				{
					DataView dv = ds.Tables[0].DefaultView;

					/* make sure we have a row for every pay method */
					foreach (DataRow r in ((DataTable)StaticData.Tables[TN.PayMethod]).Rows)
					{
						dv.RowFilter = CN.CodeDescription + " = '" + (string)r[CN.CodeDescription] + "'";
						if(dv.Count == 0)
						{
							DataRow newRow = dv.Table.NewRow();
							newRow[CN.CodeDescription] = r[CN.CodeDescription];
							newRow[CN.SystemTotal] = 0;
							newRow[CN.UserTotal] = 0;
							newRow[CN.Deposit] = 0;
							newRow[CN.Difference] = 0;
							newRow[CN.Reason] = "";
							dv.Table.Rows.Add(newRow);
						}
					}
					dv.RowFilter = "";

					dgBreakdown.DataSource = dv;
					dgBreakdown.TableStyles.Clear();

					DataGridTableStyle tabStyle = new DataGridTableStyle();
					tabStyle.MappingName = dv.Table.TableName;				

					AddColumnStyle(CN.CodeDescription, tabStyle, 80, true, GetResource("T_PAYMETHOD"), "", HorizontalAlignment.Left);
					AddColumnStyle(CN.UserTotal, tabStyle, 80, true, GetResource("T_USERVALUE"), DecimalPlaces, HorizontalAlignment.Right);
					AddColumnStyle(CN.SystemTotal, tabStyle, 80, true, GetResource("T_SYSTEMVALUE"), DecimalPlaces, HorizontalAlignment.Right);					
					AddColumnStyle(CN.Deposit, tabStyle, 80, true, GetResource("T_DEPOSIT"), DecimalPlaces, HorizontalAlignment.Right);
					AddColumnStyle(CN.Difference, tabStyle, 80, true, GetResource("T_DIFFERENCE"), DecimalPlaces, HorizontalAlignment.Right);
					AddColumnStyle(CN.Reason, tabStyle, 105, true, GetResource("T_REASON"), "", HorizontalAlignment.Left);
					dgBreakdown.TableStyles.Add(tabStyle);
				}
			}
			catch(Exception ex)
			{
				Catch(ex, "CashierTotalsBreakdown_Load");
			}
			finally
			{
				StopWait();
			}
		}

        //private void LoadStaticData()
        //{
        //    XmlUtilities xml = new XmlUtilities();
        //    XmlDocument dropDowns = new XmlDocument();
        //    dropDowns.LoadXml("<DROP_DOWNS></DROP_DOWNS>");

        //    if(StaticData.Tables[TN.PayMethod]==null)
        //        dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns,TN.PayMethod, new string[]{"FPM", "L"}));
			
        //    if(dropDowns.DocumentElement.ChildNodes.Count>0)
        //    {
        //        DataSet ds = StaticDataManager.GetDropDownData(dropDowns.DocumentElement, out Error);
        //        if(Error.Length>0)
        //            ShowError(Error);
        //        else
        //        {
        //            foreach(DataTable dt in ds.Tables)
        //                StaticData.Tables[dt.TableName] = dt;
        //        }
        //    }
        //}
	}
}
