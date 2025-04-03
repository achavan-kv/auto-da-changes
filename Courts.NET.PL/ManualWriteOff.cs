using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using STL.Common.Constants.TableNames;
using STL.Common.Constants.ColumnNames;
using STL.Common.Static;
using STL.Common;
using System.Collections.Specialized;
using System.Web.Services.Protocols;
using System.Xml;
using System.Data;
using STL.PL.WS1;

namespace STL.PL
{
	/// <summary>
	/// List of reason codes for manual write off of an account.
	/// The user selects one code as the reason for the write off.
	/// </summary>
	public class ManualWriteOff : CommonForm
	{
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Button btnExit;
		private System.Windows.Forms.DataGrid dgCodes;
		private string err = "";
		public string reasonCode = "";
        private System.Windows.Forms.Button btnCancel;
        private ErrorProvider errorProvider1;
        private IContainer components;

		public ManualWriteOff(TranslationDummy d)
		{
			InitializeComponent();
		}

		public ManualWriteOff(string category)
		{
			InitializeComponent();
			LoadData(category);
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

		public void LoadData(string category)
		{

			XmlUtilities xml = new XmlUtilities();
			XmlDocument dropDowns = new XmlDocument();
			dropDowns.LoadXml("<DROP_DOWNS></DROP_DOWNS>");

			dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns,TN.ManualCodes, new string[]{category, "L"}));				

			if(dropDowns.DocumentElement.ChildNodes.Count>0)
			{
				DataSet ds = StaticDataManager.GetDropDownData(dropDowns.DocumentElement, out err);
				if(err.Length>0)
					ShowError(err);
				else
				{
					foreach(DataTable dt in ds.Tables)
						StaticData.Tables[dt.TableName] = dt;
				}
			}
			
			dgCodes.DataSource = ((DataTable)StaticData.Tables[TN.ManualCodes]).DefaultView;

			if(dgCodes.TableStyles.Count==0)
			{
				DataGridTableStyle tabStyle = new DataGridTableStyle();
				tabStyle.MappingName = ((DataTable)StaticData.Tables[TN.ManualCodes]).TableName;
				dgCodes.TableStyles.Add(tabStyle);

				tabStyle.GridColumnStyles[CN.Code].Width = 40;
				tabStyle.GridColumnStyles[CN.Code].HeaderText = GetResource("T_CODE");
				tabStyle.GridColumnStyles[CN.CodeDescript].Width = 160;
				tabStyle.GridColumnStyles[CN.CodeDescript].HeaderText = GetResource("T_CODEDSCRIPT");
			}

            //IP - 17/09/10 - Check that there are rows to select.
            if (((DataView)dgCodes.DataSource).Table.Rows.Count > 0)
            {
                errorProvider1.SetError(dgCodes, "");

                dgCodes.Select(0);
            }
            else
            {
                errorProvider1.SetError(dgCodes, "Please setup Codes for category BDD in Code Maintenance.");
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.dgCodes = new System.Windows.Forms.DataGrid();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgCodes)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnCancel);
            this.groupBox1.Controls.Add(this.btnExit);
            this.groupBox1.Controls.Add(this.dgCodes);
            this.groupBox1.Location = new System.Drawing.Point(16, 16);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(384, 272);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(224, 240);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(88, 240);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(75, 23);
            this.btnExit.TabIndex = 4;
            this.btnExit.Text = "OK";
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // dgCodes
            // 
            this.dgCodes.DataMember = "";
            this.dgCodes.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgCodes.Location = new System.Drawing.Point(32, 32);
            this.dgCodes.Name = "dgCodes";
            this.dgCodes.ReadOnly = true;
            this.dgCodes.Size = new System.Drawing.Size(328, 200);
            this.dgCodes.TabIndex = 0;
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // ManualWriteOff
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(424, 309);
            this.ControlBox = false;
            this.Controls.Add(this.groupBox1);
            this.Name = "ManualWriteOff";
            this.Text = "Manual Write OfF Codes";
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgCodes)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);

		}
		#endregion

		private void btnExit_Click(object sender, System.EventArgs e)
		{
			DataView dv = (DataView)dgCodes.DataSource;
			int count = dv.Count;

			for (int i = count-1; i >=0 ; i--)
			{
				if (dgCodes.IsSelected(i))
				{
					reasonCode = (string)dv[i][CN.Code];
					Close();
				}
			}
		}

		private void btnCancel_Click(object sender, System.EventArgs e)
		{
			Close();
		}
	}
}
