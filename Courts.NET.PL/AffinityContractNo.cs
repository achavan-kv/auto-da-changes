using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using STL.Common.Constants.ColumnNames;
using System.Xml;
using STL.Common.Constants.Tags;

namespace STL.PL
{
	/// <summary>
	/// Summary description for CashTillOpen.
	/// </summary>
	public class AffinityContractNo : CommonForm
	{
		private System.Windows.Forms.Label label1;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.TextBox txtContractNo;
		private XmlNode Item = null;
        private new string Error = "";
		private System.Windows.Forms.ErrorProvider errors;

		private string _reason = "";
		public string Reason 
		{
			get{return _reason;}
		}

		public AffinityContractNo(TranslationDummy d)
		{
			InitializeComponent();
		}

		public AffinityContractNo(Form root, Form parent, XmlNode item)
		{
			InitializeComponent();

			FormRoot = root;
			FormParent = parent; 
			Item = item;			
			txtContractNo.Text = Item.Attributes[Tags.ContractNumber].Value;
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
			this.label1 = new System.Windows.Forms.Label();
			this.btnOK = new System.Windows.Forms.Button();
			this.txtContractNo = new System.Windows.Forms.TextBox();
			this.errors = new System.Windows.Forms.ErrorProvider();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(24, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(160, 32);
			this.label1.TabIndex = 1;
			this.label1.Text = "Please enter a unique contract number for this affinity item";
			// 
			// btnOK
			// 
			this.btnOK.Location = new System.Drawing.Point(80, 104);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(40, 23);
			this.btnOK.TabIndex = 2;
			this.btnOK.Text = "OK";
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// txtContractNo
			// 
			this.txtContractNo.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
			this.txtContractNo.Location = new System.Drawing.Point(32, 64);
			this.txtContractNo.MaxLength = 10;
			this.txtContractNo.Name = "txtContractNo";
			this.txtContractNo.Size = new System.Drawing.Size(128, 20);
			this.txtContractNo.TabIndex = 3;
			this.txtContractNo.Text = "";
			// 
			// AffinityContractNo
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(200, 141);
			this.ControlBox = false;
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.txtContractNo,
																		  this.btnOK,
																		  this.label1});
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "AffinityContractNo";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Affinity Contract Number";
			this.ResumeLayout(false);

		}
		#endregion


		private void btnOK_Click(object sender, System.EventArgs e)
		{
			try
			{
				Wait();	
				bool close = true;
				string msg = "";

				txtContractNo.Text = txtContractNo.Text.Trim();

				string xpath = "//Item[@Type='Affinity' and @ContractNumber='"+txtContractNo.Text+"']";
				XmlNode duplicate = Item.OwnerDocument.DocumentElement.SelectSingleNode(xpath);

				if(txtContractNo.Text.Length==0)
				{
					msg = GetResource("M_ENTERMANDATORY");
					close = false;
				}
				else
				{
					if(duplicate != null)
					{
						if(Item != duplicate)
						{
							msg = GetResource("M_INUSEONTHISACCOUNT", new object[] {txtContractNo.Text});
							close = false;
						}
					}
					else
					{
						Item.Attributes[Tags.ContractNumber].Value = txtContractNo.Text;

						/* check the database and make sure this contract no
						* hasn't been used on another account */
						bool unique = false;
						AccountManager.AffinityContractNoUnique(((NewAccount)FormParent).AccountNo, ((NewAccount)FormParent).AgreementNo, txtContractNo.Text, out unique, out Error);
						if(Error.Length>0)
							ShowError(Error);
						else
						{
							if(!unique)
							{
								msg = GetResource("M_INUSEONOTHERACCOUNT", new object[] {txtContractNo.Text});
								close = false;							
							}
						}
					}
				}
				
				if(close)
				{
					errors.SetError(txtContractNo, "");
					Close();
				}
				else
					errors.SetError(txtContractNo, msg);

			}
			catch(Exception ex)
			{
				Catch(ex, "btnOK_Click");
			}
			finally
			{
				StopWait();
			}			
		}
	}
}
