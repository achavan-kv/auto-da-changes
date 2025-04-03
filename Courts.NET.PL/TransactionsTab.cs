using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace STL.PL
{
	/// <summary>
	/// Summary description for TransactionsTab.
	/// </summary>
	public class TransactionsTab : CommonUserControl
	{
		private System.Windows.Forms.GroupBox gbTransactions;
		public System.Windows.Forms.DataGrid dgTransactions;
		private System.Windows.Forms.Panel panel5;
		public System.Windows.Forms.TextBox txtTotalInterest;
		private System.Windows.Forms.Label label45;
		private System.Windows.Forms.Label label54;
		public System.Windows.Forms.TextBox txtTotalAdmin;
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public TransactionsTab(TranslationDummy d)
		{
			InitializeComponent();
		}
		public TransactionsTab()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			TranslateControls();

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

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.gbTransactions = new System.Windows.Forms.GroupBox();
			this.dgTransactions = new System.Windows.Forms.DataGrid();
			this.panel5 = new System.Windows.Forms.Panel();
			this.txtTotalInterest = new System.Windows.Forms.TextBox();
			this.label45 = new System.Windows.Forms.Label();
			this.label54 = new System.Windows.Forms.Label();
			this.txtTotalAdmin = new System.Windows.Forms.TextBox();
			this.gbTransactions.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dgTransactions)).BeginInit();
			this.panel5.SuspendLayout();
			this.SuspendLayout();
			// 
			// gbTransactions
			// 
			this.gbTransactions.Controls.AddRange(new System.Windows.Forms.Control[] {
																						 this.dgTransactions,
																						 this.panel5});
			this.gbTransactions.Location = new System.Drawing.Point(0, -8);
			this.gbTransactions.Name = "gbTransactions";
			this.gbTransactions.Size = new System.Drawing.Size(768, 302);
			this.gbTransactions.TabIndex = 0;
			this.gbTransactions.TabStop = false;
			// 
			// dgTransactions
			// 
			this.dgTransactions.CaptionText = "Transactions";
			this.dgTransactions.DataMember = "";
			this.dgTransactions.HeaderForeColor = System.Drawing.SystemColors.ControlText;
			this.dgTransactions.Location = new System.Drawing.Point(8, 16);
			this.dgTransactions.Name = "dgTransactions";
			this.dgTransactions.ReadOnly = true;
			this.dgTransactions.Size = new System.Drawing.Size(752, 224);
			this.dgTransactions.TabIndex = 0;
			this.dgTransactions.TabStop = false;
			// 
			// panel5
			// 
			this.panel5.Controls.AddRange(new System.Windows.Forms.Control[] {
																				 this.txtTotalInterest,
																				 this.label45,
																				 this.label54,
																				 this.txtTotalAdmin});
			this.panel5.Location = new System.Drawing.Point(8, 240);
			this.panel5.Name = "panel5";
			this.panel5.Size = new System.Drawing.Size(752, 56);
			this.panel5.TabIndex = 8;
			// 
			// txtTotalInterest
			// 
			this.txtTotalInterest.Location = new System.Drawing.Point(184, 32);
			this.txtTotalInterest.Name = "txtTotalInterest";
			this.txtTotalInterest.Size = new System.Drawing.Size(88, 20);
			this.txtTotalInterest.TabIndex = 3;
			this.txtTotalInterest.Text = "";
			// 
			// label45
			// 
			this.label45.Location = new System.Drawing.Point(40, 16);
			this.label45.Name = "label45";
			this.label45.Size = new System.Drawing.Size(100, 16);
			this.label45.TabIndex = 4;
			this.label45.Text = "Total Admin Fees";
			this.label45.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label54
			// 
			this.label54.Location = new System.Drawing.Point(176, 16);
			this.label54.Name = "label54";
			this.label54.Size = new System.Drawing.Size(120, 16);
			this.label54.TabIndex = 5;
			this.label54.Text = "Total Interest Charged";
			this.label54.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// txtTotalAdmin
			// 
			this.txtTotalAdmin.Location = new System.Drawing.Point(48, 32);
			this.txtTotalAdmin.Name = "txtTotalAdmin";
			this.txtTotalAdmin.Size = new System.Drawing.Size(88, 20);
			this.txtTotalAdmin.TabIndex = 2;
			this.txtTotalAdmin.Text = "";
			// 
			// TransactionsTab
			// 
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.gbTransactions});
			this.Name = "TransactionsTab";
			this.Size = new System.Drawing.Size(768, 302);
			this.gbTransactions.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dgTransactions)).EndInit();
			this.panel5.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion
	}
}
