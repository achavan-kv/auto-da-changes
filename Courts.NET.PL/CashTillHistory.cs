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
	/// Summary description for CashierTotalsBreakdown.
	/// </summary>
	public class CashTillHistory : CommonForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private DataTable _dtHistory;
		private System.Windows.Forms.DataGrid dgHistory;
		//private string Error = "";

		public CashTillHistory(DataTable history)
		{
			InitializeComponent();
			_dtHistory = history;			
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
			this.dgHistory = new System.Windows.Forms.DataGrid();
			((System.ComponentModel.ISupportInitialize)(this.dgHistory)).BeginInit();
			this.SuspendLayout();
			// 
			// dgHistory
			// 
			this.dgHistory.CaptionVisible = false;
			this.dgHistory.DataMember = "";
			this.dgHistory.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dgHistory.HeaderForeColor = System.Drawing.SystemColors.ControlText;
			this.dgHistory.Name = "dgHistory";
			this.dgHistory.ReadOnly = true;
			this.dgHistory.Size = new System.Drawing.Size(546, 160);
			this.dgHistory.TabIndex = 2;
			this.dgHistory.TabStop = false;
			// 
			// CashTillHistory
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(546, 160);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.dgHistory});
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "CashTillHistory";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Cash Till History";
			this.Load += new System.EventHandler(this.CashTillHistory_Load);
			((System.ComponentModel.ISupportInitialize)(this.dgHistory)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion

		private void CashTillHistory_Load(object sender, System.EventArgs e)
		{
			try
			{
				dgHistory.DataSource = _dtHistory.DefaultView;
				
				dgHistory.TableStyles.Clear();

				DataGridTableStyle tabStyle = new DataGridTableStyle();
				tabStyle.MappingName = _dtHistory.TableName;				

				AddColumnStyle(CN.EmployeeNo, tabStyle, 50, true, GetResource("T_EMPEENO"), "", HorizontalAlignment.Left);
				AddColumnStyle(CN.name, tabStyle, 100, true, GetResource("T_EMPEENAME"), "", HorizontalAlignment.Left);
				AddColumnStyle(CN.TimeOpen, tabStyle, 100, true, GetResource("T_TIMEOPEN"), "", HorizontalAlignment.Left);					
				AddColumnStyle(CN.Description, tabStyle, 150, true, GetResource("T_DESCRIPTION"), "", HorizontalAlignment.Left);
				AddColumnStyle(CN.TillID, tabStyle, 100, true, GetResource("T_TILLID"), "", HorizontalAlignment.Left);
				dgHistory.TableStyles.Add(tabStyle);
			}
			catch(Exception ex)
			{
				Catch(ex, "CashTillHistory_Load");
			}
			finally
			{
				StopWait();
			}
		}
	}
}
