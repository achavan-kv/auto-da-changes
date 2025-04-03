using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Web.Services.Protocols;
using System.Data;
using STL.Common;
using STL.PL.WS1;
using STL.PL.WS2;
using STL.PL.WS3;
using STL.PL.WS4;
using STL.PL.WS5;
using STL.Common.Static;
using STL.Common.Constants.TableNames;
using STL.Common.Constants.ColumnNames;

using System.Xml;

namespace STL.PL
{
	/// <summary>
	/// Allows a scheduled delivery to be deleted from a schedule.
	/// This might be necessary when cancelling an account or when
	/// revising an account.
	/// </summary>
	public class ScheduleOverride : CommonForm
	{
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.TextBox txtUser;
		private System.Windows.Forms.TextBox txtPassword;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button btnAuthorise;
		private System.Windows.Forms.Button btnDelete;
		private new string Error = "";
		private System.Windows.Forms.ErrorProvider errorProvider1;
		private System.Windows.Forms.ColumnHeader colBranch;
		private System.Windows.Forms.ColumnHeader colNumber;
		private System.Windows.Forms.ColumnHeader colQty;
		private System.Windows.Forms.Splitter splitter1;
		public System.Windows.Forms.RichTextBox rtxtInstructions;
		private string _instructions = "";
		private double _toRemove = 0;
		private bool _cancelRemaining = true;
		private string _accountNo = "";
		private int _agreementNo = 0;
		private string _itemNo = "";
        private int _itemID = 0;                                    //IP/NM - 18/05/11 -CR1212 - #3627 
		private short _location = 0;
		private string _olduser = "";
        public bool deleted = false;

		public string OldUser
		{
			get{return _olduser;}
			set{_olduser = value;}
		}
		private string _oldpass = "";
		private System.Windows.Forms.ListView lvSchedules;
	
		public string OldPassword
		{
			get{return _oldpass;}
			set{_oldpass = value;}
		}
		private string[] _oldroles = null;
		public string[] OldRoles
		{
			get {return _oldroles;}
			set {_oldroles = value;}
		}

		string OldName = "";

		private short buffBranchNo = 0;
		private int buffNo = 0;
		bool isDotNetWarehouse = false;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public ScheduleOverride(TranslationDummy d)
		{
			InitializeComponent();
		}

        private bool _resetHoldProp;

		public ScheduleOverride(
			bool cancelRemaining,
			string accountNo, 
			int agreementNo,
            int itemID,                     //IP/NM - 18/05/11 -CR1212 - #3627 
			short location, 
			double toRemove,
			Form root,
			Form parent,
            bool resetHoldProp)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			dynamicMenus = new Hashtable();
			HashMenus();
			/* Force  authorisation even if already authorised 
			 * ApplyRoleRestrictions();
			 */
			TranslateControls();

			FormRoot = root;
			FormParent = parent;

			_cancelRemaining = cancelRemaining;
			_accountNo = accountNo;
			_agreementNo = agreementNo;
			//_itemNo = itemNo;
            _itemID = itemID;               //IP/NM - 18/05/11 -CR1212 - #3627         
			_location = location;

			txtUser.Enabled = btnAuthorise.Enabled = (bool)Country[CountryParameterNames.DelNoteCancel];
			txtPassword.Enabled = btnAuthorise.Enabled = (bool)Country[CountryParameterNames.DelNoteCancel];
			btnDelete.Enabled = !(bool)Country[CountryParameterNames.DelNoteCancel];

			try
			{
				Wait();
				Function = "BAccountManager::GetScheduledDeliveriesForItem()";
				_instructions = "Select row(s) to delete. Quantity deleted must be at least " + toRemove + " to proceed. Enter appropriate user credentials, authorise, then delete."; 
				rtxtInstructions.Text = _instructions;
				_toRemove = toRemove;

                DataSet ds = AccountManager.GetScheduledDeliveriesForItem(accountNo, agreementNo, itemID, location, out Error);      //IP/NM - 18/05/11 -CR1212 - #3627  
				if(Error.Length>0)
					ShowError(Error);
				else
				{
					foreach(DataRow row in ds.Tables["Schedules"].Rows)
					{
						lvSchedules.Items.Add(new ListViewItem(new string[] {row["Branch"].ToString(), row["Number"].ToString(), row["Item Qty"].ToString()}));

						buffBranchNo = Convert.ToInt16(row["Branch"]);
						buffNo = (int)(row["Number"]);
					}
					isDotNetWarehouse = AccountManager.IsDotNetWarehouse(Convert.ToInt16(Config.BranchCode), out Error);
					if(Error.Length>0)
						ShowError(Error);
				}

                _resetHoldProp = resetHoldProp;

                // 5.1 uat74 rdb delete iif not useful elsewhere
                // if parent is NewAccount we can get itemDoc
                //NewAccount parNewAcc = parent as NewAccount;
                //_resetHoldProp = true;    
                //if (parNewAcc != null)

                //{
                //    XmlDocument itemsDoc = parNewAcc.itemDoc;

                //    int totalQuantityDelivered = 0;
                //    int totalQuantiyUndelivered = 0;
                //    XmlNodeList items = itemsDoc.SelectNodes("/item");
                //    foreach(XmlNode item in items)
                //    {
                //        //todo get item type
                //        int quantity = Convert.ToInt32(item.Attributes["Quantity"]);
                //        int quantityDelivered = Convert.ToInt32(item.Attributes["QuantityDelivered"]);
                //        totalQuantityDelivered += quantity;
                //        totalQuantiyUndelivered += (quantity - quantityDelivered);

                //    }
                //    if (totalQuantityDelivered > 0 && totalQuantiyUndelivered == 0)
                //    {
                //        _resetHoldProp = false;  
                //    }
                //}

				StopWait();
			}
			catch(Exception ex)
			{
				Catch(ex, Function);
			}
		}

		private void HashMenus()
		{
			dynamicMenus[this.Name+":btnDelete"] = this.btnDelete; 
			dynamicMenus[this.Name+":btnAuthorise"] = this.btnAuthorise; 
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(ScheduleOverride));
			this.btnAuthorise = new System.Windows.Forms.Button();
			this.txtPassword = new System.Windows.Forms.TextBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.rtxtInstructions = new System.Windows.Forms.RichTextBox();
			this.splitter1 = new System.Windows.Forms.Splitter();
			this.lvSchedules = new System.Windows.Forms.ListView();
			this.colBranch = new System.Windows.Forms.ColumnHeader();
			this.colNumber = new System.Windows.Forms.ColumnHeader();
			this.colQty = new System.Windows.Forms.ColumnHeader();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.btnDelete = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.txtUser = new System.Windows.Forms.TextBox();
			this.errorProvider1 = new System.Windows.Forms.ErrorProvider();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// btnAuthorise
			// 
			this.btnAuthorise.Location = new System.Drawing.Point(176, 40);
			this.btnAuthorise.Name = "btnAuthorise";
			this.btnAuthorise.Size = new System.Drawing.Size(64, 23);
			this.btnAuthorise.TabIndex = 4;
			this.btnAuthorise.Text = "Authorise";
			this.btnAuthorise.Click += new System.EventHandler(this.btnAuthorise_Click);
			// 
			// txtPassword
			// 
			this.txtPassword.Location = new System.Drawing.Point(24, 80);
			this.txtPassword.Name = "txtPassword";
			this.txtPassword.PasswordChar = '*';
			this.txtPassword.TabIndex = 1;
			this.txtPassword.Text = "";
			// 
			// groupBox1
			// 
			this.groupBox1.BackColor = System.Drawing.SystemColors.Control;
			this.groupBox1.Controls.AddRange(new System.Windows.Forms.Control[] {
																					this.rtxtInstructions,
																					this.splitter1,
																					this.lvSchedules});
			this.groupBox1.Location = new System.Drawing.Point(8, 8);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(280, 176);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Scheduled Deliveries";
			// 
			// rtxtInstructions
			// 
			this.rtxtInstructions.Dock = System.Windows.Forms.DockStyle.Fill;
			this.rtxtInstructions.Location = new System.Drawing.Point(3, 115);
			this.rtxtInstructions.Name = "rtxtInstructions";
			this.rtxtInstructions.Size = new System.Drawing.Size(274, 58);
			this.rtxtInstructions.TabIndex = 2;
			this.rtxtInstructions.Text = "";
			// 
			// splitter1
			// 
			this.splitter1.Dock = System.Windows.Forms.DockStyle.Top;
			this.splitter1.Location = new System.Drawing.Point(3, 112);
			this.splitter1.Name = "splitter1";
			this.splitter1.Size = new System.Drawing.Size(274, 3);
			this.splitter1.TabIndex = 1;
			this.splitter1.TabStop = false;
			// 
			// lvSchedules
			// 
			this.lvSchedules.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
																						  this.colBranch,
																						  this.colNumber,
																						  this.colQty});
			this.lvSchedules.Dock = System.Windows.Forms.DockStyle.Top;
			this.lvSchedules.FullRowSelect = true;
			this.lvSchedules.GridLines = true;
			this.lvSchedules.Location = new System.Drawing.Point(3, 16);
			this.lvSchedules.Name = "lvSchedules";
			this.lvSchedules.Size = new System.Drawing.Size(274, 96);
			this.lvSchedules.TabIndex = 0;
			this.lvSchedules.View = System.Windows.Forms.View.Details;
			// 
			// colBranch
			// 
			this.colBranch.Text = "Branch";
			this.colBranch.Width = 92;
			// 
			// colNumber
			// 
			this.colNumber.Text = "Number";
			this.colNumber.Width = 90;
			// 
			// colQty
			// 
			this.colQty.Text = "Item Qty";
			this.colQty.Width = 88;
			// 
			// groupBox2
			// 
			this.groupBox2.BackColor = System.Drawing.SystemColors.Control;
			this.groupBox2.Controls.AddRange(new System.Windows.Forms.Control[] {
																					this.btnDelete,
																					this.btnAuthorise,
																					this.label2,
																					this.label1,
																					this.txtPassword,
																					this.txtUser});
			this.groupBox2.Location = new System.Drawing.Point(8, 192);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(280, 120);
			this.groupBox2.TabIndex = 1;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Authorise";
			// 
			// btnDelete
			// 
			this.btnDelete.Enabled = false;
			this.btnDelete.Location = new System.Drawing.Point(176, 80);
			this.btnDelete.Name = "btnDelete";
			this.btnDelete.Size = new System.Drawing.Size(64, 23);
			this.btnDelete.TabIndex = 5;
			this.btnDelete.Text = "Delete";
			this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(24, 64);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(56, 16);
			this.label2.TabIndex = 3;
			this.label2.Text = "Password:";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(24, 24);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(104, 16);
			this.label1.TabIndex = 2;
			this.label1.Text = "Employee Number:";
			// 
			// txtUser
			// 
			this.txtUser.Location = new System.Drawing.Point(24, 40);
			this.txtUser.Name = "txtUser";
			this.txtUser.TabIndex = 0;
			this.txtUser.Text = "";
			this.txtUser.Validating += new System.ComponentModel.CancelEventHandler(this.txtUser_Validating);
			// 
			// errorProvider1
			// 
			this.errorProvider1.DataMember = null;
			// 
			// ScheduleOverride
			// 
            this.AutoScaleMode = AutoScaleMode.None;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.AutoScroll = true;
			this.BackColor = System.Drawing.SystemColors.ControlDark;
			this.ClientSize = new System.Drawing.Size(296, 317);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.groupBox2,
																		  this.groupBox1});
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "ScheduleOverride";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Item Schedule Override";
			this.groupBox1.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void txtUser_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			try
			{
				txtUser.Text = txtUser.Text.Trim();
				if(txtUser.Text.Length > 0)
				{
					errorProvider1.SetError(txtUser, "");
				}
				else
				{
					txtUser.Focus();
					txtUser.Select(0, txtUser.Text.Length);
					errorProvider1.SetError(txtUser, GetResource("M_ENTERMANDATORY"));
				}
			}
			catch(Exception ex)
			{
				//e.Cancel = true;
				txtUser.Focus();
				txtUser.Select(0, txtUser.Text.Length);
				errorProvider1.SetError(txtUser, ex.Message);
			}
		}

		private void btnAuthorise_Click(object sender, System.EventArgs e)
		{
			Function = "BLogin::IsValid()";
			if(txtUser.Text.Length!=0)
			{
				try
				{
					Wait();
					OldUser = Credential.User;
					OldPassword = Credential.Password;
					OldRoles = Credential.Roles;
					OldName = Credential.Name;

					Credential.User = txtUser.Text;
					Credential.Password = txtPassword.Text;
					string empeeNo = "";
					string name = "";

					// The CoSACS Employee No will be returned
					// in case the user entered a FACT Employee No
                    Credential.Roles = Login.IsValid(DateTime.Now, out empeeNo, out name, out Error);
					if(Error.Length>0)
					{
						txtUser.Text = "";
						txtPassword.Text = "";
						Credential.User = OldUser;
						Credential.Password = OldPassword;
						Credential.Roles = OldRoles;
						ShowError(Error);						
					}
					else
					{
						// Make sure we use the CoSACS Employee No
						Credential.User = empeeNo;
						Credential.Name = name;
						ApplyRoleRestrictions();
						if(btnDelete.Enabled==false)
						{
							rtxtInstructions.Text = "User entered is not authorised to delete schedules.";
							rtxtInstructions.ForeColor = Color.Red;
						}
						else
						{
							rtxtInstructions.Text = _instructions;
							rtxtInstructions.ForeColor = Color.Black;
						}
						txtUser.Enabled = btnAuthorise.Enabled;
						txtPassword.Enabled = btnAuthorise.Enabled;

						Credential.User = OldUser;
						Credential.Password = OldPassword;
						Credential.Roles = OldRoles;
						Credential.Name = OldName;
					}
					StopWait();
				}
				catch(Exception ex)
				{
					Catch(ex, Function);
				}
			}
		}

		/// <summary>
		/// when the delete button is clicked check to make sure that enough qty 
		/// has been deleted
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnDelete_Click(object sender, System.EventArgs e)
		{
			Function = "btnDelete_Click";
			double totalQty = 0;

			Wait();
			try		//create a dataset of all the selected schedules to delete
			{
				DataSet ds = new DataSet();
				DataTable dt = new DataTable("Schedules");
				dt.Columns.Add(new DataColumn("AccountNo", System.Type.GetType("System.String")));
				dt.Columns.Add(new DataColumn("AgreementNo", System.Type.GetType("System.Int32")));
				dt.Columns.Add(new DataColumn("ItemNo", System.Type.GetType("System.String")));
				dt.Columns.Add(new DataColumn("Location", System.Type.GetType("System.Int16")));
				dt.Columns.Add(new DataColumn("Branch", System.Type.GetType("System.Int16")));
				dt.Columns.Add(new DataColumn("Number", System.Type.GetType("System.Int32")));
				dt.Columns.Add(new DataColumn("Quantity", System.Type.GetType("System.Double")));
                dt.Columns.Add(new DataColumn("QtyRemoved", System.Type.GetType("System.Double"))); //IP/JC - 03/03/10 - CR1072 - Malaysia 3PL 
                dt.Columns.Add(new DataColumn("ItemID", System.Type.GetType("System.Int32")));      //IP - 08/06/11 - CR1212 - RI


				foreach(int i in lvSchedules.SelectedIndices)
				{
					DataRow row = dt.NewRow();
					row["AccountNo"] = _accountNo;
					row["AgreementNo"] = _agreementNo;
					row["ItemNo"] = _itemNo;
					row["Location"] = _location;
					row["Branch"] = Convert.ToInt16(lvSchedules.Items[i].SubItems[0].Text);
					row["Number"] = Convert.ToInt32(lvSchedules.Items[i].SubItems[1].Text);
					row["Quantity"] = Convert.ToDouble(lvSchedules.Items[i].SubItems[2].Text);
                    row["QtyRemoved"] = _toRemove; //IP/JC - 03/03/10 - CR1072 - Malaysia 3PL 
                    row["ItemID"] = _itemID;       //IP - 08/06/11 - CR1212 - RI
					totalQty += Convert.ToDouble(lvSchedules.Items[i].SubItems[2].Text);
					dt.Rows.Add(row);
				}
				ds.Tables.Add(dt);

				if(totalQty < _toRemove)
				{
					rtxtInstructions.Text = "Quantity selected is only " + totalQty + ". Quantity required is " + _toRemove +".";
					rtxtInstructions.ForeColor = Color.Red;
				}
				else
				{
                    // 5.1 uat74 - rdb - added resetHoldProp param (awaiting delivery authorisation) was always reset to 'Y'
                    // dont do this if we have delivered some line items
                    // and there are no items to deliver

					//Go ahead and delete the selected schedules
					AccountManager.DeleteDeliverySchedules(ds,_resetHoldProp, out Error);
					if(Error.Length>0)
					{
						//((NewAccount)this.FormParent).ScheduledQtyDeleted = 0;
						ShowError(Error);
					}
					else
					{
						//Need to signal calling screen that schedules have been deleted
						
						if(FormParent!=null)
						{
							if(FormParent.GetType().Name == "NewAccount")
							{
								((NewAccount)this.FormParent).ScheduledQtyDeleted = _toRemove;
							}
								
							// UAT 13 - Cancel Account will call this screen to delete each item in turn
							/*if (_cancelRemaining)
							{
								// Delete remaining items for this Del Note, effectively 
								// cancelling the Del Note.  UAT Issue 54.
								//DataSet schedules = AccountManager.GetScheduledForAccount(_accountNo, false, out Error);
								DataSet schedules = AccountManager.Schedule_GetByBuffNo(buffBranchNo, buffNo, out Error);
								schedules.Tables[TN.Schedules].DefaultView.RowFilter = "Quantity >= 1";
								AccountManager.CancelDeliveryNote(_accountNo, schedules, isDotNetWarehouse, out Error);
							}*/
						}
                        deleted = true;
						Close();
					}
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
	}
}
