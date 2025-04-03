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
	/// Common popup to list a set of information so that the user can
	/// review the members of a data set. Originally used to list the days
	/// of the week for a Delivery Area, so that the user can select
	/// a suitable delivery day for a Delivery Area. Note that this
	/// popup is only informative and does not return a selected item.
	/// In the case of days of the week it can show the suitable days
	/// of a week but cannot determine the actual date.
	/// </summary>
	public class ListInfoPopup : CommonForm
	{
		private bool _resultIgnore = false;
		public bool resultIgnore
		{
			get {return _resultIgnore;}
			set {_resultIgnore = value;}
		}


		private new string Error = "";
		private string _windowTitle;
		private string _msgText;
		private string _setName = "";
		private string _tableName = "";
		private DataTable _setTable;
		private DataView _setView;

		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label lMsgText;
		private System.Windows.Forms.DataGrid dgList;
		private System.Windows.Forms.Button btnChange;
		private System.Windows.Forms.Button btnIgnore;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		/// <summary>
		/// Translation constructor
		/// </summary>
		/// <param name="d"></param>
		public ListInfoPopup(TranslationDummy d)
		{
			InitializeComponent();
		}		

		public ListInfoPopup(string windowTitle, string msgText, string setName,
			string tableName, Form root, Form parent)
		{
			this._windowTitle = windowTitle;
			this._msgText = msgText;
			this._setName = setName;
			this._tableName = tableName;

			InitializeComponent();
			FormRoot = root;
			FormParent = parent;
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

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.btnIgnore = new System.Windows.Forms.Button();
			this.dgList = new System.Windows.Forms.DataGrid();
			this.btnChange = new System.Windows.Forms.Button();
			this.lMsgText = new System.Windows.Forms.Label();
			this.groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dgList)).BeginInit();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.BackColor = System.Drawing.SystemColors.Control;
			this.groupBox1.Controls.AddRange(new System.Windows.Forms.Control[] {
																					this.btnIgnore,
																					this.dgList,
																					this.btnChange,
																					this.lMsgText});
			this.groupBox1.Location = new System.Drawing.Point(8, 0);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(512, 384);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			// 
			// btnIgnore
			// 
			this.btnIgnore.Location = new System.Drawing.Point(288, 344);
			this.btnIgnore.Name = "btnIgnore";
			this.btnIgnore.Size = new System.Drawing.Size(112, 23);
			this.btnIgnore.TabIndex = 69;
			this.btnIgnore.Text = "Ignore";
			this.btnIgnore.Click += new System.EventHandler(this.btnIgnore_Click);
			// 
			// dgList
			// 
			this.dgList.DataMember = "";
			this.dgList.HeaderForeColor = System.Drawing.SystemColors.ControlText;
			this.dgList.Location = new System.Drawing.Point(72, 112);
			this.dgList.Name = "dgList";
			this.dgList.ReadOnly = true;
			this.dgList.Size = new System.Drawing.Size(376, 216);
			this.dgList.TabIndex = 68;
			// 
			// btnChange
			// 
			this.btnChange.Location = new System.Drawing.Point(112, 344);
			this.btnChange.Name = "btnChange";
			this.btnChange.Size = new System.Drawing.Size(112, 23);
			this.btnChange.TabIndex = 67;
			this.btnChange.Text = "Change Delivery";
			this.btnChange.Click += new System.EventHandler(this.btnChange_Click);
			// 
			// lMsgText
			// 
			this.lMsgText.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.lMsgText.Location = new System.Drawing.Point(16, 24);
			this.lMsgText.Name = "lMsgText";
			this.lMsgText.Size = new System.Drawing.Size(488, 72);
			this.lMsgText.TabIndex = 66;
			this.lMsgText.Text = "Message Text";
			this.lMsgText.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// ListInfoPopup
			// 
            this.AutoScaleMode = AutoScaleMode.None;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.BackColor = System.Drawing.SystemColors.ControlDark;
			this.ClientSize = new System.Drawing.Size(526, 387);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.groupBox1});
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ListInfoPopup";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "ListInfoPopup";
			this.Load += new System.EventHandler(this.ListInfoPopup_Load);
			this.groupBox1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dgList)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion

		private void ListInfoPopup_Load(object sender, System.EventArgs e)
		{
			this.Text = _windowTitle;
			this.lMsgText.Text = _msgText;
			this.dgList.CaptionText = this._setName;

			// Load the descriptions for the set 
			XmlUtilities xml = new XmlUtilities();
			XmlDocument dropDowns = new XmlDocument();
			dropDowns.LoadXml("<DROP_DOWNS></DROP_DOWNS>");
				
			if(StaticData.Tables[TN.DeliveryArea]==null)
				dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns,TN.DeliveryArea, new string[]{"DDY", "L"}));

			if (dropDowns.DocumentElement.ChildNodes.Count > 0)
			{
				DataSet ds = StaticDataManager.GetDropDownData(dropDowns.DocumentElement, out Error);
				if (Error.Length > 0)
					ShowError(Error);
				else
				{
					foreach (DataTable dt in ds.Tables)
					{
						StaticData.Tables[dt.TableName] = dt;
					}
				}
			}

			// Load this set
			DataSet deliveryAreaSet = SetDataManager.GetSetDetailsForSetName(this._setName, this._tableName, out this.Error);
			if (Error.Length > 0)
				ShowError(Error);
			else
			{
				_setTable = deliveryAreaSet.Tables[0];
				_setTable.Columns.Add(CN.Description);

				// Add a description for each code
				foreach (DataRow row in _setTable.Rows)
				{
					foreach (DataRow descRow in ((DataTable)StaticData.Tables[this._tableName]).Rows)
					{
						if ((string)descRow[CN.Code] == (string)row[CN.Code])
						{
							row[CN.Description] = (string)descRow[CN.CodeDescript];
						}
					}
				}

				_setView = new DataView(_setTable);

				if (dgList.TableStyles.Count == 0)
				{
					DataGridTableStyle tabStyle = new DataGridTableStyle();
					tabStyle.MappingName = _setView.Table.TableName;

					dgList.TableStyles.Clear();
					dgList.TableStyles.Add(tabStyle);
					dgList.DataSource = _setView;

					// Initially hide all columns
					for (int i = 0; i < _setTable.Columns.Count; i++)
					{
						tabStyle.GridColumnStyles[i].Width = 0;
					}

					// Show the columns we want
					tabStyle.GridColumnStyles[CN.Description].Width = 80;
					tabStyle.GridColumnStyles[CN.Description].ReadOnly = true;
					tabStyle.GridColumnStyles[CN.Description].HeaderText = string.Empty;

                    tabStyle.GridColumnStyles[CN.CodeDescription].Width = 200;
                    tabStyle.GridColumnStyles[CN.CodeDescription].ReadOnly = true;
                    tabStyle.GridColumnStyles[CN.CodeDescription].HeaderText = string.Empty;
				}
                _setView.Sort = CN.Code + " ASC ";
			}
		}


		private void btnChange_Click(object sender, System.EventArgs e)
		{
			try
			{
				Wait();
				Function = "btnChange_Click";
				this._resultIgnore = false;
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

		private void btnIgnore_Click(object sender, System.EventArgs e)
		{
			try
			{
				Wait();
				Function = "btnIgnore_Click";
				this._resultIgnore = true;
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
