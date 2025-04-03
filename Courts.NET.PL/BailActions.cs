using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using STL.PL.WS2;
using System.Data;
using System.Web.Services.Protocols;
using Crownwood.Magic.Menus;


namespace STL.PL
{
	/// <summary>
	/// Summary description for BailActions.
	/// </summary>
	public class BailActions : CommonForm
	{
		private System.Windows.Forms.DataGrid dgActions;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.TextBox textNotes;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Button btnReturn;
        private new string Error = "";

		public BailActions(TranslationDummy d)
		{
			InitializeComponent();
		}

		public BailActions()
		{
			InitializeComponent();
		}

		public BailActions(string acctNo, Form root, Form parent)
		{
			InitializeComponent();
			FormRoot = root;
			FormParent = parent;

			DataSet ds = null;
			
			try
			{
				ds = AccountManager.GetBailActions(acctNo, out Error);

				dgActions.DataSource = ds.Tables["BailActions"].DefaultView; 
				if(dgActions.TableStyles.Count==0)
				{
					DataGridTableStyle tabStyle = new DataGridTableStyle();
					tabStyle.MappingName = ds.Tables["BailActions"].TableName;
					dgActions.TableStyles.Add(tabStyle);

					tabStyle.GridColumnStyles["AllocNo"].Width = 0;
					tabStyle.GridColumnStyles["AmtCommPaidOn"].Width = 0;
					tabStyle.GridColumnStyles["Notes"].Width = 0;
					tabStyle.GridColumnStyles["CommissionDays"].Width = 0;

					tabStyle.GridColumnStyles["Acctno"].Width = 90;
					tabStyle.GridColumnStyles["Acctno"].HeaderText = GetResource("T_ACCTNO");

					tabStyle.GridColumnStyles["ActionNo"].Width = 60;
					tabStyle.GridColumnStyles["ActionNo"].HeaderText = GetResource("T_ACTIONNO");

					tabStyle.GridColumnStyles["EmpeeNo"].Width = 120;
					tabStyle.GridColumnStyles["EmpeeNo"].HeaderText = GetResource("T_ALLOCATEDEMPLOYEE");

					tabStyle.GridColumnStyles["DateAdded"].Width = 100;  // 68268 RD 06/06/06
					tabStyle.GridColumnStyles["DateAdded"].HeaderText = GetResource("T_DATEADDED");
                    ((DataGridTextBoxColumn)tabStyle.GridColumnStyles["DateAdded"]).Format = "dd/MM/yyyy HH:mm";

					tabStyle.GridColumnStyles["Code"].Width = 40;
					tabStyle.GridColumnStyles["Code"].HeaderText = GetResource("T_CODE");

					tabStyle.GridColumnStyles["ActionValue"].Width = 80;
					tabStyle.GridColumnStyles["ActionValue"].Alignment = HorizontalAlignment.Right;
					tabStyle.GridColumnStyles["ActionValue"].HeaderText = GetResource("T_ACTIONVALUE");

					tabStyle.GridColumnStyles["DateDue"].Width = 70;
					tabStyle.GridColumnStyles["DateDue"].HeaderText = GetResource("T_DATEDUE");

					tabStyle.GridColumnStyles["AddedBy"].Width = 60;
					tabStyle.GridColumnStyles["AddedBy"].HeaderText = GetResource("T_ADDEDBY");

					dgActions_CurrentCellChanged(this, null);
				}
                foreach (DataRow row in ds.Tables["BailActions"].Rows)
                {
                    if (row["DateDue"].ToString().IndexOf("1900") > 0)
                    {
                        row["DateDue"] = string.Empty;
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
			this.dgActions = new System.Windows.Forms.DataGrid();
			this.textNotes = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.btnReturn = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.dgActions)).BeginInit();
			this.SuspendLayout();
			// 
			// dgActions
			// 
			this.dgActions.CaptionText = "Follow Up Actions";
			this.dgActions.DataMember = "";
			this.dgActions.HeaderForeColor = System.Drawing.SystemColors.ControlText;
			this.dgActions.Location = new System.Drawing.Point(8, 24);
			this.dgActions.Name = "dgActions";
			this.dgActions.ReadOnly = true;
			this.dgActions.Size = new System.Drawing.Size(560, 232);
			this.dgActions.TabIndex = 0;
			this.dgActions.CurrentCellChanged += new System.EventHandler(this.dgActions_CurrentCellChanged);
			// 
			// textNotes
			// 
			this.textNotes.Location = new System.Drawing.Point(8, 288);
			this.textNotes.MaxLength = 700;
			this.textNotes.Multiline = true;
			this.textNotes.Name = "textNotes";
			this.textNotes.Size = new System.Drawing.Size(560, 136);
			this.textNotes.TabIndex = 71;
			this.textNotes.Text = "";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(8, 272);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(40, 16);
			this.label3.TabIndex = 72;
			this.label3.Text = "Notes";
			// 
			// btnReturn
			// 
			this.btnReturn.Location = new System.Drawing.Point(248, 440);
			this.btnReturn.Name = "btnReturn";
			this.btnReturn.TabIndex = 74;
			this.btnReturn.Text = "Return";
			this.btnReturn.Click += new System.EventHandler(this.btnReturn_Click);
			// 
			// BailActions
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(584, 477);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.btnReturn,
																		  this.label3,
																		  this.textNotes,
																		  this.dgActions});
			this.Name = "BailActions";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "BailActions";
			((System.ComponentModel.ISupportInitialize)(this.dgActions)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion

		private void dgActions_CurrentCellChanged(object sender, System.EventArgs e)
		{
			try
			{
				int index = dgActions.CurrentRowIndex;

				if(index >= 0)
				{
					DataView dv = (DataView)dgActions.DataSource;
					
					if(dv[index]["Notes"] == DBNull.Value)
						textNotes.Text = "";
					else
						textNotes.Text = (string)dv[index]["Notes"];
				}
			}
			catch(Exception ex)
			{
				Catch(ex, Function);
			}
		}

		private void btnReturn_Click(object sender, System.EventArgs e)
		{
			Close();
		}
	}
}
