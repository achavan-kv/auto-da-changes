using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using STL.Common.Constants.TableNames;
using STL.Common.Constants.ColumnNames;
using STL.PL.WS2;
using STL.Common.Static;
using System.Web.Services.Protocols;
using System.Xml;
using System.Threading;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using STL.Common;
namespace STL.PL
{
	/// <summary>
	/// Lists accounts for a customer. The user may select an individual
	/// account and enter a new due date.
	/// </summary>
	public class UpdateDateDue : CommonForm
	{
		private Crownwood.Magic.Menus.MenuCommand menuFile;
		private Crownwood.Magic.Menus.MenuCommand menuExit;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.GroupBox groupBox2;
		private STL.PL.AccountTextBox txtAccountNo;
		private System.Windows.Forms.Button btnSave;
		private System.Windows.Forms.DataGrid dgAccounts;
		private System.Windows.Forms.TextBox txtCosacsDate;
		private System.Windows.Forms.TextBox txtName;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label1;
		private string error = "";
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.DateTimePicker dtDateFirst;
		private DataView accounts;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.NumericUpDown txtDay;
		private System.Windows.Forms.Button btnClear;
		private System.Windows.Forms.Label lAuthorise;
		private bool status = true;
		private Crownwood.Magic.Menus.MenuCommand menuHelp;
		private Crownwood.Magic.Menus.MenuCommand menuLaunchHelp;
		private System.Windows.Forms.Button btnSearchAccount;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		
		private System.ComponentModel.Container components = null;

		public UpdateDateDue(TranslationDummy d)
		{
			InitializeComponent();
			menuMain = new Crownwood.Magic.Menus.MenuControl();
			menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {menuFile, menuHelp});
		}

		public UpdateDateDue(Form root, Form parent)
		{
			InitializeComponent();
			dtDateFirst.Value = DateTime.Today;
			menuMain = new Crownwood.Magic.Menus.MenuControl();
			menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {menuFile, menuHelp});
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UpdateDateDue));
            this.menuFile = new Crownwood.Magic.Menus.MenuCommand();
            this.menuExit = new Crownwood.Magic.Menus.MenuCommand();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnSearchAccount = new System.Windows.Forms.Button();
            this.lAuthorise = new System.Windows.Forms.Label();
            this.btnClear = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.txtAccountNo = new STL.PL.AccountTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtCosacsDate = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtDay = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.dtDateFirst = new System.Windows.Forms.DateTimePicker();
            this.btnSave = new System.Windows.Forms.Button();
            this.dgAccounts = new System.Windows.Forms.DataGrid();
            this.menuHelp = new Crownwood.Magic.Menus.MenuCommand();
            this.menuLaunchHelp = new Crownwood.Magic.Menus.MenuCommand();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtDay)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgAccounts)).BeginInit();
            this.SuspendLayout();
            // 
            // menuFile
            // 
            this.menuFile.Description = "MenuItem";
            this.menuFile.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {
            this.menuExit});
            this.menuFile.Text = "&File";
            // 
            // menuExit
            // 
            this.menuExit.Description = "MenuItem";
            this.menuExit.Text = "E&xit";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnSearchAccount);
            this.groupBox1.Controls.Add(this.lAuthorise);
            this.groupBox1.Controls.Add(this.btnClear);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.txtName);
            this.groupBox1.Controls.Add(this.txtAccountNo);
            this.groupBox1.Location = new System.Drawing.Point(24, 24);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(736, 120);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            // 
            // btnSearchAccount
            // 
            this.btnSearchAccount.Image = ((System.Drawing.Image)(resources.GetObject("btnSearchAccount.Image")));
            this.btnSearchAccount.Location = new System.Drawing.Point(256, 48);
            this.btnSearchAccount.Name = "btnSearchAccount";
            this.btnSearchAccount.Size = new System.Drawing.Size(32, 32);
            this.btnSearchAccount.TabIndex = 57;
            this.btnSearchAccount.Click += new System.EventHandler(this.btnSearchAccount_Click);
            // 
            // lAuthorise
            // 
            this.lAuthorise.Enabled = false;
            this.lAuthorise.Location = new System.Drawing.Point(24, 24);
            this.lAuthorise.Name = "lAuthorise";
            this.lAuthorise.Size = new System.Drawing.Size(16, 16);
            this.lAuthorise.TabIndex = 56;
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(624, 56);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(75, 23);
            this.btnClear.TabIndex = 55;
            this.btnClear.Text = "&Clear";
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(352, 40);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(40, 16);
            this.label4.TabIndex = 46;
            this.label4.Text = "Name:";
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(352, 56);
            this.txtName.MaxLength = 60;
            this.txtName.Name = "txtName";
            this.txtName.ReadOnly = true;
            this.txtName.Size = new System.Drawing.Size(176, 20);
            this.txtName.TabIndex = 43;
            this.txtName.TabStop = false;
            // 
            // txtAccountNo
            // 
            this.txtAccountNo.Location = new System.Drawing.Point(104, 56);
            this.txtAccountNo.Name = "txtAccountNo";
            this.txtAccountNo.Size = new System.Drawing.Size(94, 20);
            this.txtAccountNo.TabIndex = 1;
            this.txtAccountNo.Text = "000-0000-0000-0";
            this.txtAccountNo.Leave += new System.EventHandler(this.txtAccountNo_Leave);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(504, 128);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(128, 16);
            this.label1.TabIndex = 47;
            this.label1.Text = "CoSACS Delivery Date:";
            // 
            // txtCosacsDate
            // 
            this.txtCosacsDate.Location = new System.Drawing.Point(504, 144);
            this.txtCosacsDate.Name = "txtCosacsDate";
            this.txtCosacsDate.ReadOnly = true;
            this.txtCosacsDate.Size = new System.Drawing.Size(104, 20);
            this.txtCosacsDate.TabIndex = 11;
            this.txtCosacsDate.TabStop = false;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.txtDay);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.dtDateFirst);
            this.groupBox2.Controls.Add(this.btnSave);
            this.groupBox2.Controls.Add(this.dgAccounts);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.txtCosacsDate);
            this.groupBox2.Location = new System.Drawing.Point(24, 160);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(736, 224);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(648, 40);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 16);
            this.label2.TabIndex = 58;
            this.label2.Text = "Day Due:";
            // 
            // txtDay
            // 
            this.txtDay.Enabled = false;
            this.txtDay.Location = new System.Drawing.Point(648, 56);
            this.txtDay.Maximum = new decimal(new int[] {
            31,
            0,
            0,
            0});
            this.txtDay.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.txtDay.Name = "txtDay";
            this.txtDay.Size = new System.Drawing.Size(48, 20);
            this.txtDay.TabIndex = 4;
            this.txtDay.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.txtDay.ValueChanged += new System.EventHandler(this.txtDay_ValueChanged);
            this.txtDay.TextChanged += new System.EventHandler(this.txtDay_TextChanged);
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(504, 40);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(56, 16);
            this.label3.TabIndex = 56;
            this.label3.Text = "Date First:";
            // 
            // dtDateFirst
            // 
            this.dtDateFirst.CustomFormat = "ddd dd MMM yyyy ";
            this.dtDateFirst.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtDateFirst.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.dtDateFirst.Location = new System.Drawing.Point(504, 56);
            this.dtDateFirst.Name = "dtDateFirst";
            this.dtDateFirst.Size = new System.Drawing.Size(120, 20);
            this.dtDateFirst.TabIndex = 3;
            this.dtDateFirst.Value = new System.DateTime(2002, 5, 8, 0, 0, 0, 0);
            this.dtDateFirst.CloseUp += new System.EventHandler(this.dtDateFirst_CloseUp);
            this.dtDateFirst.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.dtDateFirst_KeyPress);
            // 
            // btnSave
            // 
            this.btnSave.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnSave.BackgroundImage")));
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnSave.Location = new System.Drawing.Point(656, 136);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(24, 24);
            this.btnSave.TabIndex = 5;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // dgAccounts
            // 
            this.dgAccounts.DataMember = "";
            this.dgAccounts.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgAccounts.Location = new System.Drawing.Point(16, 24);
            this.dgAccounts.Name = "dgAccounts";
            this.dgAccounts.ReadOnly = true;
            this.dgAccounts.Size = new System.Drawing.Size(456, 184);
            this.dgAccounts.TabIndex = 0;
            this.dgAccounts.TabStop = false;
            this.dgAccounts.MouseUp += new System.Windows.Forms.MouseEventHandler(this.dgAccounts_MouseUp);
            // 
            // menuHelp
            // 
            this.menuHelp.Description = "MenuItem";
            this.menuHelp.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {
            this.menuLaunchHelp});
            this.menuHelp.Text = "&Help";
            // 
            // menuLaunchHelp
            // 
            this.menuLaunchHelp.Description = "MenuItem";
            this.menuLaunchHelp.Text = "&About This Screen";
            this.menuLaunchHelp.Click += new System.EventHandler(this.menuLaunchHelp_Click);
            // 
            // UpdateDateDue
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(792, 477);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "UpdateDateDue";
            this.Text = "Update Date Due";
            this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.UpdateDateDue_HelpRequested);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtDay)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgAccounts)).EndInit();
            this.ResumeLayout(false);

		}
		#endregion

		private void btnSearchAccount_Click(object sender, System.EventArgs e)
		{
			try
			{
				Wait();
				DataSet ds = AccountManager.GetInstalmentAccounts(txtAccountNo.UnformattedText, out error);
				
				if(error.Length > 0)
					ShowError(error);
				else
				{
					if(ds != null)
					{
                        

                        ds.Tables["Table1"].Columns.Add(new DataColumn((CN.Month), Type.GetType("System.DateTime"))); //Store the original month
                        foreach (DataRow dr in ds.Tables["Table1"].Rows)
                        {
                            dr[CN.Month] = dr[CN.DateFirst];
                        }

						accounts = ds.Tables["Table1"].DefaultView;
						dgAccounts.DataSource = accounts;

						if(dgAccounts.TableStyles.Count==0)
						{
							DataGridTableStyle tabStyle = new DataGridTableStyle();
							tabStyle.MappingName = ds.Tables["Table1"].TableName;
							dgAccounts.TableStyles.Add(tabStyle);

							tabStyle.GridColumnStyles[CN.AcctNo].Width = 90;
							tabStyle.GridColumnStyles[CN.AcctNo].HeaderText = GetResource("T_ACCTNO");
							
							tabStyle.GridColumnStyles[CN.DateFirst].Width = 90;
							tabStyle.GridColumnStyles[CN.DateFirst].HeaderText = GetResource("T_DATEFIRST");

							tabStyle.GridColumnStyles[CN.OutstBal].Width = 120;
							tabStyle.GridColumnStyles[CN.OutstBal].HeaderText = GetResource("T_OUTBAL");
							tabStyle.GridColumnStyles[CN.OutstBal].Alignment = HorizontalAlignment.Left;
							((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.OutstBal]).Format = DecimalPlaces;


							tabStyle.GridColumnStyles[CN.Type].Width = 70;
							tabStyle.GridColumnStyles[CN.Type].HeaderText = GetResource("T_TYPE");

							tabStyle.GridColumnStyles[CN.AgreementDate].Width = 0;
							tabStyle.GridColumnStyles[CN.CustomerName].Width = 0;
							tabStyle.GridColumnStyles[CN.DateDel].Width = 0;
                            tabStyle.GridColumnStyles[CN.Month].Width = 0;
						}

						if (ds.Tables.Count > 0)
							if (ds.Tables[0].Rows.Count > 0)
							{
								txtName.Text = (string)ds.Tables[0].Rows[0][CN.CustomerName];
								dtDateFirst.Value = Convert.ToDateTime(ds.Tables[0].Rows[0][CN.DateFirst]);
								txtCosacsDate.Text = ((DateTime)ds.Tables[0].Rows[0][CN.DateDel]).ToShortDateString();
                                
                                // enable/disable 
								if((string)ds.Tables[0].Rows[0][CN.Type] == "R")
                                {
									txtDay.Enabled = true;
                                }
								else
                                {
									txtDay.Enabled = false;
                                }
                                // this will trigger event "txtDay_ValueChanged" which checks "txtDay.Enabled" 
                                txtDay.Value = Convert.ToInt16(dtDateFirst.Value.Day);
                                dgAccounts.CurrentRowIndex = 0;
                                //dgAccounts_MouseUp(null, null);
							}
					}
				}
			}
			catch(Exception ex)
			{
				Catch(ex, "btnSearchAccount_Click");
			}		
			finally
			{
				StopWait();
			}
		}

		private void btnSave_Click(object sender, System.EventArgs e)
		{
			try
			{
				Wait();

				if(accounts.Table.DataSet.HasChanges(DataRowState.Modified))
				{
					DataSet ds = new DataSet();
					ds.Tables.Add(accounts.Table.GetChanges(DataRowState.Modified));

					AccountManager.UpdateDateFirst(ds, out error);
					if(error.Length>0)
						ShowError(error);
					else
					{
						accounts.Table.AcceptChanges();
						btnClear_Click(this, null);
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

		private void dgAccounts_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			try
			{
				if(dgAccounts.DataSource != null)
				{
					if(((DataView)dgAccounts.DataSource).Count > 0)
					{
						if(e.Button == MouseButtons.Left)
						{
							Wait();
							int index = dgAccounts.CurrentRowIndex;

							if(index >= 0)
							{
								DataView d = (DataView)dgAccounts.DataSource;
								dtDateFirst.Value = Convert.ToDateTime(d[index][CN.DateFirst]);
                                
                                if ((string)d[index][CN.Type] == "R")
                                {
                                    txtDay.Enabled = true;
                                }
                                else
                                {
                                    txtDay.Enabled = false;
                                }
                                // set up Due Day here !!
                                // this will trigger event "txtDay_ValueChanged" which checks "txtDay.Enabled"
                                txtDay.Value = Convert.ToInt16(dtDateFirst.Value.Day);

								txtCosacsDate.Text = ((DateTime)d[index][CN.DateDel]).ToShortDateString();
							}
						}
					}
				}
			}
			catch(Exception ex)
			{
				Catch(ex, "dgDeliveredItems_Click");
			}
			finally
			{
			}

		}
        
		private void txtDay_ValueChanged(object sender, System.EventArgs e)
		{
            //int i = 0;

			if(dgAccounts.DataSource != null)
			{
				foreach(DataRowView row in (DataView)dgAccounts.DataSource)
				{
                    // only update datefirst with due day if "R" type and Dueday enabled
                    if ((string)row[CN.Type] == "R" & txtDay.Enabled)
                    {
                        DateTime oldDate = (DateTime)row[CN.Month];
                        Int16 newDay = Convert.ToInt16(txtDay.Value);
                        DateTime newDateStart;
                        
                        //68693 Due date can now go up to 31 added this additional logic to handle it
                        if (DateTime.DaysInMonth(oldDate.Year, oldDate.Month) < newDay)
                        {
                            newDateStart = new DateTime(
                                oldDate.Year,
                                oldDate.Month + 1,
                                newDay - DateTime.DaysInMonth(oldDate.Year, oldDate.Month));
                        }
                        else
                            newDateStart = new DateTime(oldDate.Year, oldDate.Month, newDay);


                        row[CN.DateFirst] = newDateStart;
                         
                    }
                    // set datefirst dropdown to selected row value
                    //if (dgAccounts.IsSelected(i))
                    //{
                    //    dtDateFirst.Value = Convert.ToDateTime(row[CN.DateFirst]);
                    //}
                    //i++;
				}
                dtDateFirst.Value = Convert.ToDateTime(dgAccounts[dgAccounts.CurrentRowIndex, 1]);
            }
            
            
		}

		private void txtDay_TextChanged(object sender, System.EventArgs e)
		{
            //if (Convert.ToInt16(txtDay.Value) > txtDay.Maximum)
            //    txtDay.Value = txtDay.Maximum;

            //if (Convert.ToInt16(txtDay.Value) < txtDay.Minimum)
            //    txtDay.Value = txtDay.Minimum;

            //txtDay_ValueChanged(this, null);
		}

		private void btnClear_Click(object sender, System.EventArgs e)
		{
			dgAccounts.DataSource = null;
			txtAccountNo.Text = "000-0000-0000-0";
			txtName.Text = "";
			dtDateFirst.Value = DateTime.Today;			
			txtDay.Enabled = false;
			txtDay.Value = 1;
			txtCosacsDate.Text = "";
		}

		private void dtDateFirst_CloseUp(object sender, System.EventArgs e)
		{
			try
			{
				if(dgAccounts.DataSource != null)
				{
					if(((DataView)dgAccounts.DataSource).Count > 0)
					{
						DataView d = (DataView)dgAccounts.DataSource;
						int index = dgAccounts.CurrentRowIndex;
						
						if(index >= 0)
						{
							if(dtDateFirst.Value < (DateTime)d[index][CN.DateDel])
							{
								ShowInfo("M_DATEDUEINVALID");
								status = false;
							}
							else
								status = true;

							if(status)
							{
								TimeSpan ts = dtDateFirst.Value - (DateTime)d[index][CN.DateDel];
								if(ts.Days > 56) // 2 months
								{
									AuthorisePrompt ap = new AuthorisePrompt(this, lAuthorise, GetResource("M_DATEDUEAUTH"));
									ap.ShowDialog();
									if (ap.Authorised)
									{
										status = true;
									}
									else
									{
										status = false;
									}
								}
							}

                            if (status)
                            {
                                d[index][CN.DateFirst] = dtDateFirst.Value;
                                d[index][CN.Month] = dtDateFirst.Value;
                                // set up Due Day 
                                txtDay.Value = Convert.ToInt16(dtDateFirst.Value.Day);
                            }
                            else
                                dtDateFirst.Value = Convert.ToDateTime(d[index][CN.DateFirst]);
						}
					}
				}
			}
			catch(Exception ex)
			{
				Catch(ex, "dtDateFirst_Leave");
			}				
		}

		private void txtAccountNo_Leave(object sender, System.EventArgs e)
		{
			try
			{
				Wait();
				this.btnSearchAccount_Click(sender, e);
			}
			catch(Exception ex)
			{
				Catch(ex, "txtAccountNo_Leave");
			}		
			finally
			{
				StopWait();
			}
		}

		private void dtDateFirst_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
		{
			if(e.KeyChar == (char)13)
				dtDateFirst_CloseUp(this,null);	
		}

		private void UpdateDateDue_HelpRequested(object sender, System.Windows.Forms.HelpEventArgs hlpevent)
		{
			string fileName = this.Name + ".htm";
			LaunchHelp(fileName);
		}

		private void menuLaunchHelp_Click(object sender, System.EventArgs e)
		{
			UpdateDateDue_HelpRequested(this, null);
		}

	}
}