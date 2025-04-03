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
	/// Controls the End Of Day batch processes to be run from the .NET
	/// server. Each batch process can be enabled or disabled and the 
	/// results of the last run reviewed.
	/// </summary>
	public class EODControl : CommonForm
	{
		private Crownwood.Magic.Menus.MenuCommand menuExit;
		private Crownwood.Magic.Menus.MenuCommand menuFile;
		private string err = "";
		private DataSet controls = null;
		private System.Windows.Forms.GroupBox grpEOD;
		private Crownwood.Magic.Controls.TabControl tcEOD;
		private Crownwood.Magic.Controls.TabPage tpControl;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.DataGrid dgControl;
		private Crownwood.Magic.Controls.TabPage tpResults;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.DataGrid dgResults;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox txtInterface;
		private System.Windows.Forms.Button btnSave;
		bool status = true;
		private System.Windows.Forms.Button btnLoad;
		
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public EODControl(TranslationDummy d)
		{
			InitializeComponent();
			menuMain = new Crownwood.Magic.Menus.MenuControl();
			menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {menuFile});
		}

		public EODControl(Form root, Form parent)
		{
			InitializeComponent();
			menuMain = new Crownwood.Magic.Menus.MenuControl();
			menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {menuFile});

			FormRoot = root;
			FormParent = parent;
			LoadData();
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(EODControl));
			this.menuExit = new Crownwood.Magic.Menus.MenuCommand();
			this.menuFile = new Crownwood.Magic.Menus.MenuCommand();
			this.grpEOD = new System.Windows.Forms.GroupBox();
			this.tcEOD = new Crownwood.Magic.Controls.TabControl();
			this.tpControl = new Crownwood.Magic.Controls.TabPage();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.btnSave = new System.Windows.Forms.Button();
			this.dgControl = new System.Windows.Forms.DataGrid();
			this.tpResults = new Crownwood.Magic.Controls.TabPage();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.label1 = new System.Windows.Forms.Label();
			this.txtInterface = new System.Windows.Forms.TextBox();
			this.dgResults = new System.Windows.Forms.DataGrid();
			this.btnLoad = new System.Windows.Forms.Button();
			this.grpEOD.SuspendLayout();
			this.tcEOD.SuspendLayout();
			this.tpControl.SuspendLayout();
			this.groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dgControl)).BeginInit();
			this.tpResults.SuspendLayout();
			this.groupBox2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dgResults)).BeginInit();
			this.SuspendLayout();
			// 
			// menuExit
			// 
			this.menuExit.Description = "MenuItem";
			this.menuExit.Text = "E&xit";
			// 
			// menuFile
			// 
			this.menuFile.Description = "MenuItem";
			this.menuFile.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {
																							this.menuExit});
			this.menuFile.Text = "&File";
			// 
			// grpEOD
			// 
			this.grpEOD.Controls.AddRange(new System.Windows.Forms.Control[] {
																				 this.tcEOD});
			this.grpEOD.Location = new System.Drawing.Point(8, 8);
			this.grpEOD.Name = "grpEOD";
			this.grpEOD.Size = new System.Drawing.Size(768, 456);
			this.grpEOD.TabIndex = 0;
			this.grpEOD.TabStop = false;
			// 
			// tcEOD
			// 
			this.tcEOD.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.World, ((System.Byte)(0)));
			this.tcEOD.IDEPixelArea = true;
			this.tcEOD.Location = new System.Drawing.Point(24, 24);
			this.tcEOD.Name = "tcEOD";
			this.tcEOD.PositionTop = true;
			this.tcEOD.SelectedIndex = 0;
			this.tcEOD.SelectedTab = this.tpControl;
			this.tcEOD.Size = new System.Drawing.Size(720, 416);
			this.tcEOD.TabIndex = 7;
			this.tcEOD.TabPages.AddRange(new Crownwood.Magic.Controls.TabPage[] {
																					this.tpControl,
																					this.tpResults});
			this.tcEOD.SelectionChanged += new System.EventHandler(this.tcEOD_SelectionChanged);
			// 
			// tpControl
			// 
			this.tpControl.Controls.AddRange(new System.Windows.Forms.Control[] {
																					this.groupBox1});
			this.tpControl.Location = new System.Drawing.Point(0, 25);
			this.tpControl.Name = "tpControl";
			this.tpControl.Size = new System.Drawing.Size(720, 391);
			this.tpControl.TabIndex = 0;
			this.tpControl.Title = "Control";
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.AddRange(new System.Windows.Forms.Control[] {
																					this.btnLoad,
																					this.btnSave,
																					this.dgControl});
			this.groupBox1.Location = new System.Drawing.Point(24, 16);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(672, 360);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			// 
			// btnSave
			// 
			this.btnSave.BackgroundImage = ((System.Drawing.Bitmap)(resources.GetObject("btnSave.BackgroundImage")));
			this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.btnSave.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.btnSave.Location = new System.Drawing.Point(352, 328);
			this.btnSave.Name = "btnSave";
			this.btnSave.Size = new System.Drawing.Size(24, 24);
			this.btnSave.TabIndex = 37;
			this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
			// 
			// dgControl
			// 
			this.dgControl.DataMember = "";
			this.dgControl.HeaderForeColor = System.Drawing.SystemColors.ControlText;
			this.dgControl.Location = new System.Drawing.Point(16, 24);
			this.dgControl.Name = "dgControl";
			this.dgControl.Size = new System.Drawing.Size(640, 288);
			this.dgControl.TabIndex = 0;
			// 
			// tpResults
			// 
			this.tpResults.Controls.AddRange(new System.Windows.Forms.Control[] {
																					this.groupBox2});
			this.tpResults.Location = new System.Drawing.Point(0, 25);
			this.tpResults.Name = "tpResults";
			this.tpResults.Selected = false;
			this.tpResults.Size = new System.Drawing.Size(720, 391);
			this.tpResults.TabIndex = 1;
			this.tpResults.Title = "View Results";
			this.tpResults.Visible = false;
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.AddRange(new System.Windows.Forms.Control[] {
																					this.label1,
																					this.txtInterface,
																					this.dgResults});
			this.groupBox2.Location = new System.Drawing.Point(24, 16);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(672, 360);
			this.groupBox2.TabIndex = 1;
			this.groupBox2.TabStop = false;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(224, 32);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(64, 16);
			this.label1.TabIndex = 2;
			this.label1.Text = "Interface:";
			// 
			// txtInterface
			// 
			this.txtInterface.Location = new System.Drawing.Point(296, 32);
			this.txtInterface.Name = "txtInterface";
			this.txtInterface.Size = new System.Drawing.Size(112, 20);
			this.txtInterface.TabIndex = 1;
			this.txtInterface.Text = "";
			// 
			// dgResults
			// 
			this.dgResults.DataMember = "";
			this.dgResults.HeaderForeColor = System.Drawing.SystemColors.ControlText;
			this.dgResults.Location = new System.Drawing.Point(56, 72);
			this.dgResults.Name = "dgResults";
			this.dgResults.ReadOnly = true;
			this.dgResults.Size = new System.Drawing.Size(560, 272);
			this.dgResults.TabIndex = 0;
			this.dgResults.MouseUp += new System.Windows.Forms.MouseEventHandler(this.dgResults_MouseUp);
			// 
			// btnLoad
			// 
			this.btnLoad.Image = ((System.Drawing.Bitmap)(resources.GetObject("btnLoad.Image")));
			this.btnLoad.Location = new System.Drawing.Point(264, 320);
			this.btnLoad.Name = "btnLoad";
			this.btnLoad.Size = new System.Drawing.Size(32, 32);
			this.btnLoad.TabIndex = 38;
			this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
			// 
			// EODControl
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(792, 477);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.grpEOD});
			this.Name = "EODControl";
			this.Text = "EOD Control";
			this.grpEOD.ResumeLayout(false);
			this.tcEOD.ResumeLayout(false);
			this.tpControl.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dgControl)).EndInit();
			this.tpResults.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dgResults)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion


		private void tcEOD_SelectionChanged(object sender, System.EventArgs e)
		{
			switch(tcEOD.SelectedTab.Name)
			{
				case "tpResults":
					LoadResults();
					break;
				default:
					break;
			}
		}

		private void LoadResults()
		{
			string eodInterface = "";

			try
			{
				int index = dgControl.CurrentRowIndex;

				if(index >= 0)
				{
					DataView dv = (DataView)dgControl.DataSource;
					eodInterface = (string)dv[index][GetResource("T_INTERFACE")];
					txtInterface.Text = eodInterface;

                    DataSet ds = EodManager.GetInterfaceControl(eodInterface, "", true, out err);       //UAT1010 jec 09/07/10
					if(err.Length > 0)
						ShowError(err);
					else
					{
						if(ds != null)
						{
							dgResults.DataSource = ds.Tables["Table1"].DefaultView; 
					
							if(dgResults.TableStyles.Count==0)
							{
								DataGridTableStyle tabStyle = new DataGridTableStyle();
								tabStyle.MappingName = ds.Tables["Table1"].TableName;
								dgResults.TableStyles.Add(tabStyle);

								tabStyle.GridColumnStyles[CN.Interface].Width = 90;
								tabStyle.GridColumnStyles[CN.Interface].HeaderText = GetResource("T_INTERFACE");

								tabStyle.GridColumnStyles[CN.Runno].Width = 75;
								tabStyle.GridColumnStyles[CN.Runno].HeaderText = GetResource("T_RUNNO");

								tabStyle.GridColumnStyles[CN.DateStart].Width = 70;
								tabStyle.GridColumnStyles[CN.DateStart].HeaderText = GetResource("T_DATESTART");

								tabStyle.GridColumnStyles[CN.DateFinish].Width = 130;
								tabStyle.GridColumnStyles[CN.DateFinish].HeaderText = GetResource("T_DATEFINISH");

								tabStyle.GridColumnStyles[CN.Result].Width = 100;
								tabStyle.GridColumnStyles[CN.Result].HeaderText = GetResource("T_RESULT");
							}
						}
					}
				}
			}
			catch(Exception ex)
			{
				Catch(ex, Function);
			}
		}

		private void dgResults_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Right)
			{
				if(dgResults.CurrentRowIndex >= 0)
				{
					DataGrid ctl = (DataGrid)sender;							

					MenuCommand m1 = new MenuCommand(GetResource("P_VIEWERRORS"));
					MenuCommand m2 = new MenuCommand(GetResource("P_VIEWVALUES"));

					m1.Click += new System.EventHandler(this.OnViewErrors);
					m2.Click += new System.EventHandler(this.OnViewValues);

					PopupMenu popup = new PopupMenu(); 
					popup.MenuCommands.AddRange(new MenuCommand[] {m1,m2});
					MenuCommand selected = popup.TrackPopup(ctl.PointToScreen(new Point(e.X, e.Y)));					
				}
			}
		}

		private void OnViewErrors(object sender, System.EventArgs e)
		{
			string eodInterface = "";
			int runno = 0;

			try
			{
				int index = dgResults.CurrentRowIndex;

				if(index >= 0)
				{
					DataView dv = (DataView)dgResults.DataSource;
					eodInterface = (string)dv[index][CN.Interface];
					runno = (int)dv[index][CN.Runno];
                    var startdate=Convert.ToDateTime(dv[index][CN.DateStart]);        //jec 06/04/11
                    InterfaceDetails id = new InterfaceDetails(eodInterface, runno, startdate, true);   //jec 06/04/11
					id.ShowDialog();
				}
			}
			catch(Exception ex)
			{
				Catch(ex, Function);
			}
		}

		private void OnViewValues(object sender, System.EventArgs e)
		{
			string eodInterface = "";
			int runno = 0;

			try
			{
				int index = dgResults.CurrentRowIndex;

				if(index >= 0)
				{
					DataView dv = (DataView)dgResults.DataSource;
					eodInterface = (string)dv[index][CN.Interface];
					runno = (int)dv[index][CN.Runno];
                    var startdate = Convert.ToDateTime(dv[index][CN.DateStart]);        //jec 06/04/11
                    InterfaceDetails id = new InterfaceDetails(eodInterface, runno, startdate, false);      //jec 06/04/11
					id.ShowDialog();
				}
			}
			catch(Exception ex)
			{
				Catch(ex, Function);
			}
		}

		private void btnSave_Click(object sender, System.EventArgs e)
		{
			DataSet changes = null;

			try
			{
				
				Function = "btnSave_Click()";
				Wait();

				if(status)
				{
					if (controls.HasChanges(DataRowState.Modified))	//have any records been modified
						changes = controls.GetChanges(DataRowState.Modified);
				
					if (controls.HasChanges(DataRowState.Modified))
					{

						EodManager.EODControlUpdate(changes, out err);
						if(err.Length>0)
							ShowError(err);
						else 
						{
							dgControl.TableStyles.Clear();
							dgControl.DataSource = null;
						}
					}
				}
				else
					ShowInfo("M_EODCONTROL");
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

		protected void OnColumnChange(object sender, System.Data.DataColumnChangeEventArgs e)
		{
			try
			{
				string str = e.ProposedValue.ToString();

				str = str.ToUpper();

				if(str == "Y" || str == "N")
				{
					e.ProposedValue = str.ToUpper();
					status = true;
				}
				else 
				{
					ShowInfo("M_EODCONTROL");
					status = false;
				}
			}
			catch(Exception ex)
			{
				Catch(ex, Function);
			}
		}

		private void LoadData()
		{
			try
			{
				Wait();

				controls = EodManager.GetEODControl(out err);
				if(err.Length > 0)
					ShowError(err);
				else
				{
					if(controls != null)
					{
						DataView dv = new DataView(controls.Tables["Table1"]);

						dv.AllowNew = false;
						dv.AllowEdit = true;
						dv.AllowDelete = false;
						dgControl.DataSource = dv;

						DataGridTableStyle tabStyle = new DataGridTableStyle();
						tabStyle.MappingName = dv.Table.TableName;
						dgControl.TableStyles.Add(tabStyle);
						tabStyle.GridColumnStyles[CN.Description].Width = 430;
						tabStyle.GridColumnStyles[CN.Description].ReadOnly = true;
						tabStyle.GridColumnStyles[CN.Description].HeaderText = GetResource("T_DESCRIPTION");
			
						tabStyle.GridColumnStyles[CN.DoDefault].Width = 80;
						tabStyle.GridColumnStyles[CN.DoDefault].HeaderText = GetResource("T_DODEFAULT");

						tabStyle.GridColumnStyles[CN.DoNextRun].Width = 80;
						tabStyle.GridColumnStyles[CN.DoNextRun].HeaderText = GetResource("T_DONEXTRUN");

						tabStyle.GridColumnStyles[CN.Interface].Width = 0;

						dv.Table.ColumnChanging += new DataColumnChangeEventHandler(this.OnColumnChange);
					}
				}
				StopWait();
			}
			catch(Exception ex)
			{
				Catch(ex, Function);
			}
		}

		private void btnLoad_Click(object sender, System.EventArgs e)
		{
			LoadData();
		}
	}
}
