using System;
using System.Data;
using System.Drawing;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Forms;
using System.Web.Services.Protocols;
using STL.PL.WS9;
using STL.Common.Constants.ColumnNames;
using STL.Common.Constants.TableNames;
using System.Xml;
using STL.Common;
using STL.Common.Static;
using Crownwood.Magic.Menus;

namespace STL.PL
{
	/// <summary>
	/// A popup used from the EOD Control screen to show details of
	/// errors or warnings for a particular EOD process and run number.
	/// </summary>
	public class InterfaceDetails : CommonForm
	{
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.DataGrid dgErrors;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox txtInterface;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox txtRunno;
		private string err = "";

		public InterfaceDetails(TranslationDummy d)
		{
			InitializeComponent();
		}

        public InterfaceDetails(string eodInterface, int runno, DateTime startdate, bool errors)        //jec 06/04/11
		{
			InitializeComponent();
			
			txtInterface.Text = eodInterface;
			txtRunno.Text = runno.ToString();

			txtInterface.BackColor = SystemColors.Window;
			txtRunno.BackColor = SystemColors.Window;

			if(errors)
                LoadErrors(eodInterface, runno, startdate);     //jec 06/04/11
			else
                LoadValues(eodInterface, runno);
		}

        private void LoadErrors(string eodInterface, int runno, DateTime startdate)
		{
			try
			{
                DataSet ds = EodManager.GetInterfaceError(eodInterface, runno, startdate, out err);     //jec 06/04/11
				if(err.Length > 0)
					ShowError(err);
				else
				{
					if(ds != null)
					{
						dgErrors.DataSource = ds.Tables["Table1"].DefaultView; 
				
						if(dgErrors.TableStyles.Count==0)
						{
							DataGridTableStyle tabStyle = new DataGridTableStyle();
							tabStyle.MappingName = ds.Tables["Table1"].TableName;
							dgErrors.TableStyles.Add(tabStyle);

							tabStyle.GridColumnStyles[CN.Severity].Width = 50;
							tabStyle.GridColumnStyles[CN.Severity].HeaderText = GetResource("P_SEVERITY");

							tabStyle.GridColumnStyles[CN.ErrorDate].Width = 80;
							tabStyle.GridColumnStyles[CN.ErrorDate].HeaderText = GetResource("P_ERRORDATE");

							tabStyle.GridColumnStyles[CN.ErrorText].Width = 450;
							tabStyle.GridColumnStyles[CN.ErrorText].HeaderText = GetResource("P_ERRORTEXT");

							tabStyle.GridColumnStyles[CN.Interface].Width = 0;
							tabStyle.GridColumnStyles[CN.Runno].Width = 0;
						}
					}
				}
			}
			catch(Exception ex)
			{
				Catch(ex, Function);
			}
		}

		private void LoadValues(string eodInterface, int runno)
		{
			try
			{
				DataSet ds = EodManager.GetInterfaceValue(eodInterface, runno, out err);
				if(err.Length > 0)
					ShowError(err);
				else
				{
					if(ds != null)
					{
						dgErrors.DataSource = ds.Tables["Table1"].DefaultView; 
				
						if(dgErrors.TableStyles.Count==0)
						{
							DataGridTableStyle tabStyle = new DataGridTableStyle();
							tabStyle.MappingName = ds.Tables["Table1"].TableName;
							dgErrors.TableStyles.Add(tabStyle);

							tabStyle.GridColumnStyles[CN.CountType1].Width = 100;
							tabStyle.GridColumnStyles[CN.CountType1].HeaderText = GetResource("T_COUNTTYPE1");

							tabStyle.GridColumnStyles[CN.CountType2].Width = 100;
							tabStyle.GridColumnStyles[CN.CountType2].HeaderText = GetResource("T_COUNTTYPE2");

							tabStyle.GridColumnStyles[CN.BranchNo].Width = 60;
							tabStyle.GridColumnStyles[CN.BranchNo].HeaderText = GetResource("T_BRANCH");

							tabStyle.GridColumnStyles[CN.CountValue].Width = 100;
							tabStyle.GridColumnStyles[CN.CountValue].HeaderText = GetResource("T_COUNTVALUE");

							tabStyle.GridColumnStyles[CN.Value].Width = 100;
							tabStyle.GridColumnStyles[CN.Value].HeaderText = GetResource("T_VALUE");
							tabStyle.GridColumnStyles[CN.Value].Alignment = HorizontalAlignment.Right;
							((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.Value]).Format = DecimalPlaces;

							tabStyle.GridColumnStyles[CN.Interface].Width = 0;
							tabStyle.GridColumnStyles[CN.Runno].Width = 0;
							tabStyle.GridColumnStyles[CN.AcctType].Width = 0;
						}
					}
				}
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
			this.dgErrors = new System.Windows.Forms.DataGrid();
			this.label1 = new System.Windows.Forms.Label();
			this.txtInterface = new System.Windows.Forms.TextBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.label2 = new System.Windows.Forms.Label();
			this.txtRunno = new System.Windows.Forms.TextBox();
			this.groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dgErrors)).BeginInit();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.AddRange(new System.Windows.Forms.Control[] {
																					this.dgErrors});
			this.groupBox1.Location = new System.Drawing.Point(24, 88);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(720, 272);
			this.groupBox1.TabIndex = 1;
			this.groupBox1.TabStop = false;
			// 
			// dgErrors
			// 
			this.dgErrors.DataMember = "";
			this.dgErrors.HeaderForeColor = System.Drawing.SystemColors.ControlText;
			this.dgErrors.Location = new System.Drawing.Point(24, 32);
			this.dgErrors.Name = "dgErrors";
			this.dgErrors.ReadOnly = true;
			this.dgErrors.Size = new System.Drawing.Size(672, 216);
			this.dgErrors.TabIndex = 0;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(144, 32);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(64, 16);
			this.label1.TabIndex = 4;
			this.label1.Text = "Interface:";
			// 
			// txtInterface
			// 
			this.txtInterface.Location = new System.Drawing.Point(216, 32);
			this.txtInterface.Name = "txtInterface";
			this.txtInterface.ReadOnly = true;
			this.txtInterface.Size = new System.Drawing.Size(112, 20);
			this.txtInterface.TabIndex = 3;
			this.txtInterface.Text = "";
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.AddRange(new System.Windows.Forms.Control[] {
																					this.label2,
																					this.txtRunno,
																					this.label1,
																					this.txtInterface});
			this.groupBox2.Location = new System.Drawing.Point(24, 8);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(720, 72);
			this.groupBox2.TabIndex = 5;
			this.groupBox2.TabStop = false;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(384, 32);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(64, 16);
			this.label2.TabIndex = 6;
			this.label2.Text = "Run No:";
			// 
			// txtRunno
			// 
			this.txtRunno.Location = new System.Drawing.Point(456, 32);
			this.txtRunno.Name = "txtRunno";
			this.txtRunno.ReadOnly = true;
			this.txtRunno.Size = new System.Drawing.Size(112, 20);
			this.txtRunno.TabIndex = 5;
			this.txtRunno.Text = "";
			// 
			// InterfaceDetails
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(776, 389);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.groupBox2,
																		  this.groupBox1});
			this.Name = "InterfaceDetails";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Interface Details";
			this.groupBox1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dgErrors)).EndInit();
			this.groupBox2.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion
	}
}
