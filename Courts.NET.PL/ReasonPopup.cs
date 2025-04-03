using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using STL.Common;
using STL.Common.Constants.ColumnNames;
using System.Web.Services.Protocols;
using STL.Common.Static;
using System.Collections.Specialized;
using System.Xml;
using STL.Common.Constants.TableNames;
using STL.Common.Constants.AccountTypes;
using STL.Common.Constants.ScreenModes;




namespace STL.PL
{
	/// <summary>
	/// A common popup prompt to request a reason to be entered for the user
	/// action being performed. A message and a title for the reason field
	/// is passed to this popup. Validation prevents the user continuing until
	/// a reason has been entered or until the action is cancelled.
	/// </summary>
	public class ReasonPopup : CommonForm
	{
		private new string Error = "";
		private string _windowTitle;
		private string _msgText;
		private string _reasonTableName;
		private string _reasonType;
		private string _reasonCode = "";
		public string reasonCode
		{
			get
			{
				return (this._reasonCode);
			}
		}

		private string _reasonText = "";
		public string reasonText
		{
			get
			{
				return (this._reasonText);
			}
		}
		private string _reason = "";
		public string reason
		{
			get
			{
				return (this._reason);
			}
		}


		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.ComboBox drpReason;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Label lTitle;
		private System.Windows.Forms.Label lMsgText;
		private System.Windows.Forms.ErrorProvider errorProvider1;
		private System.Windows.Forms.Button btnOK;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		/// <summary>
		/// Translation constructor
		/// </summary>
		/// <param name="d"></param>
		public ReasonPopup(TranslationDummy d)
		{
			InitializeComponent();
		}		

		public ReasonPopup(string windowTitle, string msgText, string reasonTableName, string reasonType, Form root, Form parent)
		{
			this._windowTitle = windowTitle;
			this._msgText = msgText;
			this._reasonTableName = reasonTableName;
			this._reasonType = reasonType;

			InitializeComponent();
			FormRoot = root;
			FormParent = parent;
			//TranslateControls();
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
			this.btnOK = new System.Windows.Forms.Button();
			this.lMsgText = new System.Windows.Forms.Label();
			this.btnCancel = new System.Windows.Forms.Button();
			this.lTitle = new System.Windows.Forms.Label();
			this.drpReason = new System.Windows.Forms.ComboBox();
			this.errorProvider1 = new System.Windows.Forms.ErrorProvider();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.BackColor = System.Drawing.SystemColors.Control;
			this.groupBox1.Controls.AddRange(new System.Windows.Forms.Control[] {
																					this.btnOK,
																					this.lMsgText,
																					this.btnCancel,
																					this.lTitle,
																					this.drpReason});
			this.groupBox1.Location = new System.Drawing.Point(8, 0);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(560, 176);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			// 
			// btnOK
			// 
			this.btnOK.Location = new System.Drawing.Point(112, 136);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(104, 23);
			this.btnOK.TabIndex = 67;
			this.btnOK.Text = "OK";
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// lMsgText
			// 
			this.lMsgText.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.lMsgText.Location = new System.Drawing.Point(16, 24);
			this.lMsgText.Name = "lMsgText";
			this.lMsgText.Size = new System.Drawing.Size(528, 48);
			this.lMsgText.TabIndex = 66;
			this.lMsgText.Text = "Message Text";
			this.lMsgText.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// btnCancel
			// 
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(344, 136);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(104, 23);
			this.btnCancel.TabIndex = 64;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// lTitle
			// 
			this.lTitle.Location = new System.Drawing.Point(40, 88);
			this.lTitle.Name = "lTitle";
			this.lTitle.Size = new System.Drawing.Size(72, 16);
			this.lTitle.TabIndex = 63;
			this.lTitle.Text = "Reason";
			this.lTitle.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// drpReason
			// 
			this.drpReason.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.drpReason.DropDownWidth = 152;
			this.drpReason.Location = new System.Drawing.Point(120, 88);
			this.drpReason.Name = "drpReason";
			this.drpReason.Size = new System.Drawing.Size(344, 21);
			this.drpReason.TabIndex = 60;
			// 
			// errorProvider1
			// 
			this.errorProvider1.DataMember = null;
			// 
			// ReasonPopup
			// 
            this.AutoScaleMode = AutoScaleMode.None;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.BackColor = System.Drawing.SystemColors.ControlDark;
			this.ClientSize = new System.Drawing.Size(574, 179);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.groupBox1});
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ReasonPopup";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Reason Pop-up";
			this.Load += new System.EventHandler(this.ReasonPopup_Load);
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void ReasonPopup_Load(object sender, System.EventArgs e)
		{
			this.Text = _windowTitle;
			this.lMsgText.Text = _msgText;

			// Load Reason Code drop down
			StringCollection reasons = new StringCollection();
			reasons.Add("");

			XmlUtilities xml = new XmlUtilities();
			XmlDocument dropDowns = new XmlDocument();
			dropDowns.LoadXml("<DROP_DOWNS></DROP_DOWNS>");
			dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, _reasonTableName, new string[]{_reasonType, "L"}));

			DataSet ds = StaticDataManager.GetDropDownData(dropDowns.DocumentElement, out Error);
			if(Error.Length > 0)
				ShowError(Error);
			else
			{
				foreach(DataTable dt in ds.Tables)
					StaticData.Tables[dt.TableName] = dt;

				foreach(DataRow row in ((DataTable)StaticData.Tables[_reasonTableName]).Rows)
				{
					string str = (string)row.ItemArray[0]+" : "+(string)row.ItemArray[1];
					reasons.Add(str.ToUpper());
				}
				drpReason.DataSource = reasons;
			}
		}

		private void btnOK_Click(object sender, System.EventArgs e)
		{
			try
			{
				Wait();
				Function = "btnOK_Click";
				int index = this.drpReason.Text.IndexOf(":");
				if (index < 0)
				{
					this.errorProvider1.SetError(this.drpReason, GetResource("M_ENTERMANDATORY"));
				}
				else
				{
					this.errorProvider1.SetError(this.drpReason, "");
					this._reasonCode = this.drpReason.Text.Substring(0, index - 1);
					this._reasonText = this.drpReason.Text.Substring(index + 2);
					this._reason = this.drpReason.Text;
					this.Close();
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

		private void btnCancel_Click(object sender, System.EventArgs e)
		{
			try
			{
				Wait();
				Function = "btnCancel_Click";
				this._reasonCode = "";
				this._reasonText = "";
				this._reason = "";
				this.Close();
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

	}
}
