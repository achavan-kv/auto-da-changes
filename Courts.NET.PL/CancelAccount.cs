using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;
using System.Collections.Specialized;
using STL.Common.Constants.TableNames;
using STL.Common.Constants.ColumnNames;
using STL.PL.WS5;
using System.Data;
using STL.Common.Static;
using STL.Common;
using System.Web.Services.Protocols;


namespace STL.PL
{
	/// <summary>
	/// Manual cancellation of a customer account. A reason and user
	/// notes can be entered for the account to be cancelled.
	/// </summary>
	public class CancelAccount : CommonForm
	{
		private System.Windows.Forms.GroupBox groupBox1;
		private STL.PL.AccountTextBox txtAccountNo;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ComboBox drpReason;
		private System.Windows.Forms.Button btnClear;
        private new string Error = "";
		string acctNo = "";
		private System.Windows.Forms.Label label2;
		public System.Windows.Forms.StatusBar statusBar;
		private System.Windows.Forms.TextBox txtNotes;
		private System.Windows.Forms.Label lNotes;

        private XmlUtilities xml = new XmlUtilities();
        private XmlDocument dropDowns = new XmlDocument();
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
        private TextBox txtMonths;
        private Label lblmonths;
        private Label lblcontact;

		private bool _reverse = false;
		public bool Reverse 
		{
			get{return _reverse;}
			set
			{
				_reverse = value;

				if(_reverse)
					btnClear.Text = "Reverse Cancelled Account";
				else
					btnClear.Text = "Cancel Account";
			}
		}

		public CancelAccount(TranslationDummy d)
		{
			InitializeComponent();

		}

		public CancelAccount(Form root, Form parent, bool isReverse)
		{
			InitializeComponent();

			FormRoot = root;
			FormParent = parent;

			Reverse = isReverse;

			LoadStatic();
		}

		public CancelAccount(string acctNo, Form root, Form parent)
		{
			InitializeComponent();

			FormRoot = root;
			FormParent = parent;

			txtAccountNo.Text = acctNo;

			LoadStatic();
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
            this.lblcontact = new System.Windows.Forms.Label();
            this.lblmonths = new System.Windows.Forms.Label();
            this.txtMonths = new System.Windows.Forms.TextBox();
            this.lNotes = new System.Windows.Forms.Label();
            this.txtNotes = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnClear = new System.Windows.Forms.Button();
            this.drpReason = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtAccountNo = new STL.PL.AccountTextBox();
            this.statusBar = new System.Windows.Forms.StatusBar();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lblcontact);
            this.groupBox1.Controls.Add(this.lblmonths);
            this.groupBox1.Controls.Add(this.txtMonths);
            this.groupBox1.Controls.Add(this.lNotes);
            this.groupBox1.Controls.Add(this.txtNotes);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.btnClear);
            this.groupBox1.Controls.Add(this.drpReason);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.txtAccountNo);
            this.groupBox1.Location = new System.Drawing.Point(24, 32);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(344, 272);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            // 
            // lblcontact
            // 
            this.lblcontact.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblcontact.Location = new System.Drawing.Point(196, 96);
            this.lblcontact.Name = "lblcontact";
            this.lblcontact.Size = new System.Drawing.Size(96, 16);
            this.lblcontact.TabIndex = 27;
            this.lblcontact.Text = "Contact After";
            this.lblcontact.Visible = false;
            // 
            // lblmonths
            // 
            this.lblmonths.AutoSize = true;
            this.lblmonths.Location = new System.Drawing.Point(252, 115);
            this.lblmonths.Name = "lblmonths";
            this.lblmonths.Size = new System.Drawing.Size(41, 13);
            this.lblmonths.TabIndex = 26;
            this.lblmonths.Text = "months";
            this.lblmonths.Visible = false;
            // 
            // txtMonths
            // 
            this.txtMonths.Location = new System.Drawing.Point(199, 112);
            this.txtMonths.MaxLength = 2;
            this.txtMonths.Name = "txtMonths";
            this.txtMonths.ReadOnly = true;
            this.txtMonths.Size = new System.Drawing.Size(47, 20);
            this.txtMonths.TabIndex = 25;
            this.txtMonths.Visible = false;
            // 
            // lNotes
            // 
            this.lNotes.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lNotes.Location = new System.Drawing.Point(16, 152);
            this.lNotes.Name = "lNotes";
            this.lNotes.Size = new System.Drawing.Size(40, 16);
            this.lNotes.TabIndex = 23;
            this.lNotes.Text = "Notes:";
            // 
            // txtNotes
            // 
            this.txtNotes.Enabled = false;
            this.txtNotes.Location = new System.Drawing.Point(16, 168);
            this.txtNotes.MaxLength = 300;
            this.txtNotes.Multiline = true;
            this.txtNotes.Name = "txtNotes";
            this.txtNotes.Size = new System.Drawing.Size(312, 80);
            this.txtNotes.TabIndex = 22;
            // 
            // label2
            // 
            this.label2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label2.Location = new System.Drawing.Point(16, 96);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(136, 16);
            this.label2.TabIndex = 21;
            this.label2.Text = "Reason For Cancellation:";
            // 
            // btnClear
            // 
            this.btnClear.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnClear.Location = new System.Drawing.Point(184, 48);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(152, 23);
            this.btnClear.TabIndex = 20;
            this.btnClear.Text = "Cancel Account";
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // drpReason
            // 
            this.drpReason.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpReason.ItemHeight = 13;
            this.drpReason.Location = new System.Drawing.Point(16, 112);
            this.drpReason.Name = "drpReason";
            this.drpReason.Size = new System.Drawing.Size(160, 21);
            this.drpReason.TabIndex = 4;
            this.drpReason.SelectedIndexChanged += new System.EventHandler(this.drpReason_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label1.Location = new System.Drawing.Point(16, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(96, 16);
            this.label1.TabIndex = 3;
            this.label1.Text = "Account Number:";
            // 
            // txtAccountNo
            // 
            this.txtAccountNo.Location = new System.Drawing.Point(16, 48);
            this.txtAccountNo.Name = "txtAccountNo";
            this.txtAccountNo.PreventPaste = false;
            this.txtAccountNo.Size = new System.Drawing.Size(88, 20);
            this.txtAccountNo.TabIndex = 2;
            this.txtAccountNo.Tag = "ACCNO";
            this.txtAccountNo.Text = "000-0000-0000-0";
            this.txtAccountNo.TextChanged += new System.EventHandler(this.txtAccountNo_TextChanged);
            // 
            // statusBar
            // 
            this.statusBar.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.statusBar.Location = new System.Drawing.Point(0, 325);
            this.statusBar.Name = "statusBar";
            this.statusBar.Size = new System.Drawing.Size(392, 16);
            this.statusBar.TabIndex = 2;
            // 
            // CancelAccount
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(392, 341);
            this.Controls.Add(this.statusBar);
            this.Controls.Add(this.groupBox1);
            this.Name = "CancelAccount";
            this.Text = "Cancel Account";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

		}
		#endregion

		private void LoadStatic()	
		{		
			Function = "BStaticDataManager::GetDropDownData";
			
			dropDowns.LoadXml("<DROP_DOWNS></DROP_DOWNS>");

			string category = "";
            
                if (Reverse)
                    category = "RVC";
                else
                    category = "CN2";
            
            
            
			dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns,TN.CancelReasons, new string[]{category, "L"}));
        
            
            category = "STC";
            dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.STCancelReasons, new string[] { category, "L" }));

			if(dropDowns.DocumentElement.ChildNodes.Count>0)
			{
				DataSet ds = StaticDataManager.GetDropDownData(dropDowns.DocumentElement, out Error);
				if(Error.Length>0)
					ShowError(Error);
				else
				{
					foreach(DataTable dt in ds.Tables)
						StaticData.Tables[dt.TableName] = dt;
				}
			}
           if (txtAccountNo.Text.Replace("-", "").Substring(3, 1) != "9") //not storecard
    			drpReason.DataSource = (DataTable)StaticData.Tables[TN.CancelReasons];
           else 		
                drpReason.DataSource = (DataTable)StaticData.Tables[TN.STCancelReasons];
            drpReason.DisplayMember = CN.CodeDescription;

			if((bool)Country[CountryParameterNames.CancellationNotes])
				txtNotes.Enabled = true;
		}


		private void btnClear_Click(object sender, System.EventArgs e)
		{
			try
			{
				Wait();
				string custID = "";
				string code = "";
				decimal balance = 0;
				bool status = true;
				bool outstPayments = false;
				bool cancel = true;
				

				this.statusBar.Text = "";
				acctNo = txtAccountNo.Text.Replace("-","");
				
				code = (string)((DataRowView)drpReason.SelectedItem)[CN.Code];
				if(code == "")
				{
					status = false;
					ShowInfo("M_INVALIDCANCELCODE");
				}

				if(status)
				{
					if(!Reverse)
					{
						AccountManager.LockAccount(acctNo, Credential.UserId.ToString(), out Error);
                        if (Error.Length > 0)
                            ShowError(Error);
                        else
                        {
                            if (AccountManager.CheckSRAcct(acctNo))
                            {
                                MessageBox.Show("This is an Service Request account and cannot be cancelled.", "Service Request account cannot be cancelled", MessageBoxButtons.OK,MessageBoxIcon.Exclamation);
                            }
                            else
                            {
                                AccountManager.CheckAccountToCancel(acctNo, Config.CountryCode, ref balance, ref outstPayments, out Error);
                                if (Error.Length > 0)
                                    ShowError(Error);
                                else
                                {
                                    if (outstPayments)
                                    {
                                        OutstandingPayment op = new OutstandingPayment(FormRoot);
                                        op.ShowDialog();
                                        cancel = op.rbCancel.Checked;
                                    }
                                    if (cancel)
                                    {
                                        DataSet ds = AccountManager.GetScheduledForAccount(acctNo, true, out Error);
                                        if (Error.Length > 0)
                                            ShowError(Error);
                                        else
                                        {
                                            foreach (DataRow row in ds.Tables[0].Rows)
                                            {
                                                if ((double)row[CN.Quantity] > 0)
                                                {
                                                    ScheduleOverride sched = new ScheduleOverride(false, acctNo, 1, Convert.ToInt32(row[CN.ItemID]), //(string)row[CN.ItemNo], //IP - 26/07/11 - RI - Pass in ItemID
                                                        (short)row[CN.StockLocn],
                                                        (double)row[CN.Quantity], FormRoot, this, true);
                                                    sched.ShowDialog();


                                                    //if (!sched.deleted)
                                                    //{
                                                    //    cancel = false;
                                                    //    this.statusBar.Text = GetResource("M_CANCELSCHEDULE");
                                                    //}
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        Close();
                                    }

                                    if (cancel)
                                    {
                                        short ContactMonths=0;

                                        if (txtMonths.Text!="" && IsNumeric(txtMonths.Text))
                                            ContactMonths = Convert.ToInt16(txtMonths.Text);
                                        // 67729 RD/DR 19/01/06 Changing to get the branch no for sundry account from the account
                                        AccountManager.CancelAccount(acctNo, custID, Convert.ToInt16(acctNo.Substring(0, 3)),
                                            code, balance, Config.CountryCode, txtNotes.Text,ContactMonths, out Error);
                                        if (Error.Length > 0)
                                            ShowError(Error);
                                        else

                                        {
                                            this.statusBar.Text = GetResource("M_CANCELSUCCESSFUL");
                                            if (txtAccountNo.Text.Replace("-", "").Substring(3, 1) == "9") // Storecard Account so close 
                                                Close();
                                            txtAccountNo.Text = "000-0000-0000-0";
                                            drpReason.SelectedIndex = 0;
                                            txtNotes.Text = "";
                                            
                                        }
                                    }
                                }
                            }
                        }
					}
					else
					{
						bool isCancelled = AccountManager.IsCancelled(acctNo, out Error);
						if(Error.Length>0)
							ShowError(Error);
						else
						{
							if(isCancelled)
							{
								AccountManager.ReverseCancellation(acctNo, code, txtNotes.Text, out Error);
								if(Error.Length>0)
									ShowError(Error);
								else
								{
									this.statusBar.Text = GetResource("M_REVERSECANCELSUCCESSFUL");
									txtAccountNo.Text = "000-0000-0000-0";
									drpReason.SelectedIndex = 0;
									txtNotes.Text = "";
								}
							}
							else
								this.statusBar.Text = GetResource("M_NOTCANCELLED");
						}
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

		public override bool ConfirmClose()
		{
			bool status = true;
			try
			{
				Function = "ConfirmClose()";
				Wait();
				AccountManager.UnlockAccount(acctNo, Credential.UserId, out Error);
				if(Error.Length>0)
				{
					status = false;
					ShowError(Error);				
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

			return status;
		}

         

        private void drpReason_SelectedIndexChanged(object sender, EventArgs e)
        { 
            txtMonths.Text = (string)((DataRowView)drpReason.SelectedItem)[CN.Reference];
            if (txtMonths.Text == "")
            {
                lblcontact.Enabled = false; lblmonths.Enabled = false; txtMonths.Enabled = false;
            }
            else
            {
                lblcontact.Enabled = true; lblmonths.Enabled = true; txtMonths.Enabled = true;
            }

        }

        private void txtAccountNo_TextChanged(object sender, EventArgs e)
        {


            if (txtAccountNo.Text.Replace("-", "").Substring(3, 1) != "9" || Reverse) //not storecard
            {
                drpReason.DataSource = (DataTable)StaticData.Tables[TN.CancelReasons];
                txtMonths.Visible = false;
                lblmonths.Visible = false;
            }
            else 
            {
                
                drpReason.DataSource = (DataTable)StaticData.Tables[TN.STCancelReasons];
                txtMonths.Visible = true;
                lblmonths.Visible = true;
            }

        }

	}
}
