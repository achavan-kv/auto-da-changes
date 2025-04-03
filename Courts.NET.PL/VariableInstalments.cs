using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using STL.Common.Constants.ColumnNames;

namespace STL.PL
{
	/// <summary>
	/// Popup prompt to list the variable instalments for a terms type.
	/// This popup is used when a customer order is being created or 
	/// revised. When a terms type is selected for a hire purchase or
	/// Ready Finance account that has variable instalments then the list
	/// of variable instalments can be reviewed. 
	/// </summary>
	public class VariableInstalments : CommonForm
	{
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.DataGrid dgVariable;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public VariableInstalments(DataView dvVariable, Form root, Form parent)
		{
			InitializeComponent();

			FormRoot = root;
			FormParent = parent;

			dgVariable.DataSource = dvVariable;

			DataGridTableStyle tabStyle = new DataGridTableStyle();
			tabStyle.MappingName = dvVariable.Table.TableName;
			dgVariable.TableStyles.Add(tabStyle);
				
			tabStyle.GridColumnStyles[CN.AcctNo].Width = 0;
			tabStyle.GridColumnStyles[CN.InstalOrder].Width = 0;
			tabStyle.GridColumnStyles[CN.ServiceCharge].Width = 0;

			tabStyle.GridColumnStyles[CN.DateFrom].Width = 85;
			tabStyle.GridColumnStyles[CN.DateFrom].HeaderText = GetResource("T_DATEFROM");

			tabStyle.GridColumnStyles[CN.Instalment2].Width = 85;
			tabStyle.GridColumnStyles[CN.Instalment2].HeaderText = GetResource("T_INSTALLMENT");
			((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.Instalment2]).Format = DecimalPlaces;

			tabStyle.GridColumnStyles[CN.InstalmentNumber].Width = 115;
			tabStyle.GridColumnStyles[CN.InstalmentNumber].HeaderText = GetResource("T_INSTALNO");
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
			this.dgVariable = new System.Windows.Forms.DataGrid();
			this.groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dgVariable)).BeginInit();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.AddRange(new System.Windows.Forms.Control[] {
																					this.dgVariable});
			this.groupBox1.Location = new System.Drawing.Point(8, 8);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(424, 200);
			this.groupBox1.TabIndex = 9;
			this.groupBox1.TabStop = false;
			// 
			// dgVariable
			// 
			this.dgVariable.CaptionText = "Variable Instalment Details";
			this.dgVariable.DataMember = "";
			this.dgVariable.HeaderForeColor = System.Drawing.SystemColors.ControlText;
			this.dgVariable.Location = new System.Drawing.Point(48, 32);
			this.dgVariable.Name = "dgVariable";
			this.dgVariable.ReadOnly = true;
			this.dgVariable.Size = new System.Drawing.Size(328, 136);
			this.dgVariable.TabIndex = 9;
			// 
			// VariableInstalments
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(448, 229);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.groupBox1});
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "VariableInstalments";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Variable Instalments";
			this.groupBox1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dgVariable)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion
	}
}
