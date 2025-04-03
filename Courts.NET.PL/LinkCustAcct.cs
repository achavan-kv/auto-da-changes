using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Web.Services.Protocols;
using STL.Common.Constants.AccountTypes;
using STL.Common.Constants.ColumnNames;
using STL.Common.Static;
using STL.Common.Constants.TableNames;
using System.Data;

namespace STL.PL
{
	/// <summary>
	/// A popup prompt to request an account number to be linked to a customer.
	/// </summary>
	public class LinkCustAcct : CommonForm
	{
		private STL.PL.AccountTextBox txtAccountNo;
		private System.Windows.Forms.Button btnLink;
		private System.Windows.Forms.Label label1;
		private string CustomerID = "";
		private new string Error = "";
		private System.Windows.Forms.ErrorProvider errorProvider1;
		private bool rescore = false;
        public ComboBox drpRelationship;
        private IContainer components;
    
		public bool Rescore
		{
			get{return rescore;}
        }

		public LinkCustAcct(string customerID, Form root, Form parent)
		{
			InitializeComponent();
			TranslateControls();
            drpRelationship.DataSource = (DataTable)StaticData.Tables[TN.CustomerRelationship];
            drpRelationship.DisplayMember = CN.CodeDescription;
            FormRoot = root;
			FormParent = parent;
			CustomerID = customerID;
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LinkCustAcct));
            this.txtAccountNo = new STL.PL.AccountTextBox();
            this.btnLink = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.drpRelationship = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // txtAccountNo
            // 
            this.txtAccountNo.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtAccountNo.Location = new System.Drawing.Point(24, 56);
            this.txtAccountNo.Name = "txtAccountNo";
            this.txtAccountNo.PreventPaste = false;
            this.txtAccountNo.Size = new System.Drawing.Size(104, 23);
            this.txtAccountNo.TabIndex = 0;
            this.txtAccountNo.TabStop = false;
            this.txtAccountNo.Text = "000-0000-0000-0";
            // 
            // btnLink
            // 
            this.btnLink.Image = ((System.Drawing.Image)(resources.GetObject("btnLink.Image")));
            this.btnLink.ImageAlign = System.Drawing.ContentAlignment.BottomRight;
            this.btnLink.Location = new System.Drawing.Point(307, 56);
            this.btnLink.Name = "btnLink";
            this.btnLink.Size = new System.Drawing.Size(24, 23);
            this.btnLink.TabIndex = 1;
            this.btnLink.Click += new System.EventHandler(this.btnLink_Click);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(24, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(176, 32);
            this.label1.TabIndex = 2;
            this.label1.Text = "Enter account number to link this customer.";
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            this.errorProvider1.DataMember = "";
            // 
            // drpRelationship
            // 
            this.drpRelationship.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpRelationship.ItemHeight = 13;
            this.drpRelationship.Location = new System.Drawing.Point(151, 56);
            this.drpRelationship.Name = "drpRelationship";
            this.drpRelationship.Size = new System.Drawing.Size(126, 21);
            this.drpRelationship.TabIndex = 3;
            // 
            // LinkCustAcct
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(361, 95);
            this.Controls.Add(this.drpRelationship);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnLink);
            this.Controls.Add(this.txtAccountNo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LinkCustAcct";
            this.ShowInTaskbar = false;
            this.Text = "Link Customer To Account";
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		private void btnLink_Click(object sender, System.EventArgs e)
		{
			try
			{
				Wait();
                string Relationship = (string)((DataRowView)drpRelationship.SelectedItem)["code"];

				string oldCustomer = AccountManager.GetLinkedCustomerIDbyType(txtAccountNo.Text.Replace("-",""),Relationship, out Error);
				if(Error.Length>0)
					ShowError(Error);
				else
				{
					/* account may already be linked to another customer */
					if(oldCustomer.Length>0)
					{
						if(oldCustomer == CustomerID)	/* or the same customer */
						{
							errorProvider1.SetError(txtAccountNo, GetResource("M_ALREADYLINKED"));
						}
						else
						{
							errorProvider1.SetError(txtAccountNo, "");
							CustAcctOverride over = new CustAcctOverride(txtAccountNo.Text.Replace("-",""),
								oldCustomer, 
								CustomerID, this);
							over.ShowDialog();
							Close();
						}						
					}
					else
					{
						bool locked = AccountManager.LockAccount(txtAccountNo.Text.Replace("-",""),
													Credential.UserId.ToString(), out Error);
						if(Error.Length>0)
							ShowError(Error);
						else
						{
							if(locked)
							{
								rescore = false;
								bool exists = AccountManager.AddCustomerToAccount(txtAccountNo.Text.Replace("-",""),
																	CustomerID,
																	Relationship,// usually holder
																	"",	0,	/* account type unknown */
																	out rescore,
																	out Error);
								if(Error.Length>0)
									ShowError(Error);
								else
								{
									if(!exists)
										errorProvider1.SetError(txtAccountNo, GetResource("M_NOSUCHACCOUNT"));
									else
										errorProvider1.SetError(txtAccountNo, "");

									AccountManager.UnlockAccount(txtAccountNo.Text.Replace("-",""), 
										Credential.UserId, out Error);
									if(Error.Length>0)
										ShowError(Error);
									else
									{
										locked = false;
										if(exists)
											Close();
									}
								}
							}
						}
					}
				}
			}
			catch(Exception ex)
			{
				Catch(ex, "btnLink_Click");
			}
			finally
			{
				StopWait();
			}
		}
	}
}
