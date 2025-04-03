using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using STL.PL.WS5;
using STL.Common;
using System.Web.Services.Protocols;
using STL.Common.Static;
using STL.Common.Constants.TableNames;
using STL.Common.Constants.ColumnNames;
namespace STL.PL
{
	/// <summary>
	/// Maintenance screen to translate the stock item descriptions to another
	/// language. Each description is held in a dictionary on the database with
	/// its stock item number and the corresponding translation.
	/// </summary>
	public class StockItemTranslations : CommonForm
	{
		//private System.Windows.Forms.GroupBox groupBox1;
		//private System.Windows.Forms.Label label1;
		//private System.Windows.Forms.ComboBox drpScreens;
		private System.Windows.Forms.DataGrid dgFields;

		private new string Error = "";
		private DataSet fields = null;
		private bool staticLoaded = true;
		//private Crownwood.Magic.Menus.MenuControl menuMain;
		private Crownwood.Magic.Menus.MenuCommand menuFile;
		private Crownwood.Magic.Menus.MenuCommand menuSave;
		private Crownwood.Magic.Menus.MenuCommand menuExit;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.TextBox txtItemno;
		private System.Windows.Forms.TextBox txtDescr1_en;
		private System.Windows.Forms.TextBox txtDescr2_en;
		private System.Windows.Forms.TextBox txtDescr1;
		private System.Windows.Forms.TextBox txtDescr2;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Button btnClear;
		private System.Windows.Forms.Button btnSearch;
		private System.Windows.Forms.Button btnExit;
		private System.Windows.Forms.Button btnSave; //false;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public StockItemTranslations(TranslationDummy d)
		{
			InitializeComponent();
			menuMain = new Crownwood.Magic.Menus.MenuControl();
			menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {menuFile});
		}

		public StockItemTranslations(MainForm root)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			menuMain = new Crownwood.Magic.Menus.MenuControl();
			menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {menuFile});
			//TranslateControls();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			this.FormRoot = root;
			this.FormParent = root;
			//LoadData();
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(StockItemTranslations));
			this.dgFields = new System.Windows.Forms.DataGrid();
			this.menuFile = new Crownwood.Magic.Menus.MenuCommand();
			this.menuSave = new Crownwood.Magic.Menus.MenuCommand();
			this.menuExit = new Crownwood.Magic.Menus.MenuCommand();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.btnSave = new System.Windows.Forms.Button();
			this.btnExit = new System.Windows.Forms.Button();
			this.btnClear = new System.Windows.Forms.Button();
			this.btnSearch = new System.Windows.Forms.Button();
			this.label6 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.txtDescr2 = new System.Windows.Forms.TextBox();
			this.txtDescr1 = new System.Windows.Forms.TextBox();
			this.txtDescr2_en = new System.Windows.Forms.TextBox();
			this.txtDescr1_en = new System.Windows.Forms.TextBox();
			this.txtItemno = new System.Windows.Forms.TextBox();
			((System.ComponentModel.ISupportInitialize)(this.dgFields)).BeginInit();
			this.groupBox2.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.SuspendLayout();
			// 
			// dgFields
			// 
			this.dgFields.CaptionText = "Stock Item Translations";
			this.dgFields.DataMember = "";
			this.dgFields.HeaderForeColor = System.Drawing.SystemColors.ControlText;
			this.dgFields.Location = new System.Drawing.Point(16, 96);
			this.dgFields.Name = "dgFields";
			this.dgFields.Size = new System.Drawing.Size(744, 352);
			this.dgFields.TabIndex = 8;
			this.dgFields.MouseUp += new System.Windows.Forms.MouseEventHandler(this.dgFields_MouseUp);
			// 
			// menuFile
			// 
			this.menuFile.Description = "MenuItem";
			this.menuFile.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {
																							this.menuSave,
																							this.menuExit});
			this.menuFile.Text = "&File";
			// 
			// menuSave
			// 
			this.menuSave.Description = "MenuItem";
			this.menuSave.Text = "&Save";
			this.menuSave.Click += new System.EventHandler(this.btnSave_Click);
			// 
			// menuExit
			// 
			this.menuExit.Description = "MenuItem";
			this.menuExit.Text = "E&xit";
			this.menuExit.Click += new System.EventHandler(this.menuExit_Click);
			// 
			// groupBox2
			// 
			this.groupBox2.BackColor = System.Drawing.SystemColors.Control;
			this.groupBox2.Controls.AddRange(new System.Windows.Forms.Control[] {
																					this.groupBox3,
																					this.dgFields});
			this.groupBox2.Location = new System.Drawing.Point(8, 0);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(776, 472);
			this.groupBox2.TabIndex = 9;
			this.groupBox2.TabStop = false;
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.AddRange(new System.Windows.Forms.Control[] {
																					this.btnSave,
																					this.btnExit,
																					this.btnClear,
																					this.btnSearch,
																					this.label6,
																					this.label5,
																					this.label4,
																					this.label3,
																					this.label2,
																					this.txtDescr2,
																					this.txtDescr1,
																					this.txtDescr2_en,
																					this.txtDescr1_en,
																					this.txtItemno});
			this.groupBox3.Location = new System.Drawing.Point(16, 16);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(744, 80);
			this.groupBox3.TabIndex = 9;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Search Criteria";
			// 
			// btnSave
			// 
			this.btnSave.BackgroundImage = ((System.Drawing.Bitmap)(resources.GetObject("btnSave.BackgroundImage")));
			this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.btnSave.Location = new System.Drawing.Point(656, 48);
			this.btnSave.Name = "btnSave";
			this.btnSave.Size = new System.Drawing.Size(24, 24);
			this.btnSave.TabIndex = 37;
			this.btnSave.Click += new System.EventHandler(this.btnSave_Click_1);
			// 
			// btnExit
			// 
			this.btnExit.Location = new System.Drawing.Point(688, 48);
			this.btnExit.Name = "btnExit";
			this.btnExit.Size = new System.Drawing.Size(48, 23);
			this.btnExit.TabIndex = 7;
			this.btnExit.Text = "Exit";
			this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
			// 
			// btnClear
			// 
			this.btnClear.Location = new System.Drawing.Point(688, 16);
			this.btnClear.Name = "btnClear";
			this.btnClear.Size = new System.Drawing.Size(48, 23);
			this.btnClear.TabIndex = 6;
			this.btnClear.Text = "Clear";
			this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
			// 
			// btnSearch
			// 
			this.btnSearch.Image = ((System.Drawing.Bitmap)(resources.GetObject("btnSearch.Image")));
			this.btnSearch.Location = new System.Drawing.Point(608, 16);
			this.btnSearch.Name = "btnSearch";
			this.btnSearch.Size = new System.Drawing.Size(72, 24);
			this.btnSearch.TabIndex = 5;
			this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(192, 40);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(72, 23);
			this.label6.TabIndex = 9;
			this.label6.Text = "Translation 2";
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(192, 16);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(72, 23);
			this.label5.TabIndex = 8;
			this.label5.Text = "Translation 1";
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(400, 16);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(72, 23);
			this.label4.TabIndex = 7;
			this.label4.Text = "Description 1";
			this.label4.Click += new System.EventHandler(this.label4_Click);
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(400, 40);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(72, 23);
			this.label3.TabIndex = 6;
			this.label3.Text = "Description 2";
			this.label3.Click += new System.EventHandler(this.label3_Click);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(16, 16);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(48, 23);
			this.label2.TabIndex = 5;
			this.label2.Text = "Item No";
			this.label2.Click += new System.EventHandler(this.label2_Click);
			// 
			// txtDescr2
			// 
			this.txtDescr2.Location = new System.Drawing.Point(264, 40);
			this.txtDescr2.Name = "txtDescr2";
			this.txtDescr2.TabIndex = 3;
			this.txtDescr2.Text = "";
			// 
			// txtDescr1
			// 
			this.txtDescr1.Location = new System.Drawing.Point(264, 16);
			this.txtDescr1.Name = "txtDescr1";
			this.txtDescr1.TabIndex = 1;
			this.txtDescr1.Text = "";
			this.txtDescr1.TextChanged += new System.EventHandler(this.txtDescr1_TextChanged);
			// 
			// txtDescr2_en
			// 
			this.txtDescr2_en.Location = new System.Drawing.Point(472, 40);
			this.txtDescr2_en.Name = "txtDescr2_en";
			this.txtDescr2_en.TabIndex = 4;
			this.txtDescr2_en.Text = "";
			// 
			// txtDescr1_en
			// 
			this.txtDescr1_en.Location = new System.Drawing.Point(472, 16);
			this.txtDescr1_en.Name = "txtDescr1_en";
			this.txtDescr1_en.TabIndex = 2;
			this.txtDescr1_en.Text = "";
			this.txtDescr1_en.TextChanged += new System.EventHandler(this.txtDescr1_en_TextChanged);
			// 
			// txtItemno
			// 
			this.txtItemno.Location = new System.Drawing.Point(64, 16);
			this.txtItemno.Name = "txtItemno";
			this.txtItemno.TabIndex = 0;
			this.txtItemno.Text = "";
			// 
			// StockItemTranslations
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.BackColor = System.Drawing.SystemColors.ControlDark;
			this.ClientSize = new System.Drawing.Size(792, 477);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.groupBox2});
			this.Name = "StockItemTranslations";
			this.Text = "Stock ItemTranslation";
			((System.ComponentModel.ISupportInitialize)(this.dgFields)).EndInit();
			this.groupBox2.ResumeLayout(false);
			this.groupBox3.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// Loads StockItemTranslations that have not been translated.
		/// </summary>
		private void LoadData()
		{
			int status = 0;
			DataView dv = null;
            
			try
			{
				Wait();
				Function = "StockItemTranslations()";

				if(staticLoaded)
				{
					dgFields.DataSource = null;
					dgFields.ResetText();

					status = StaticDataManager.GetStockItemTranslations(txtItemno.Text, txtDescr1_en.Text, txtDescr1.Text, txtDescr2_en.Text, txtDescr2.Text, out fields, out Error);

					if(Error.Length>0)
						ShowError(Error);
					else
					{
						if(fields!=null)
						{
							((MainForm)this.FormRoot).statusBar1.Text = fields.Tables[0].Rows.Count + " Row(s) returned.";

							dv = new DataView(fields.Tables[0]);
							dv.AllowNew = false;
							dgFields.DataSource = dv;

							if(dgFields.TableStyles.Count==0)
							{
								DataGridTableStyle tabStyle = new DataGridTableStyle();
								tabStyle.MappingName = dv.Table.TableName;
								dgFields.TableStyles.Add(tabStyle);

								tabStyle.GridColumnStyles[CN.ItemNo].Width = 50;
								tabStyle.GridColumnStyles[CN.ItemNo].ReadOnly = true;
								tabStyle.GridColumnStyles[CN.ItemNo].HeaderText = GetResource("T_ITEMNO");

								tabStyle.GridColumnStyles[CN.descr1_en].Width = 200;
								tabStyle.GridColumnStyles[CN.descr1_en].NullText = "";
								tabStyle.GridColumnStyles[CN.descr1_en].ReadOnly = true;
								tabStyle.GridColumnStyles[CN.descr1_en].HeaderText = GetResource("T_ENGLISHDESC1");

								tabStyle.GridColumnStyles[CN.descr1].Width = 200;
								tabStyle.GridColumnStyles[CN.descr1].NullText = "";
								tabStyle.GridColumnStyles[CN.descr1].ReadOnly = false;
								tabStyle.GridColumnStyles[CN.descr1].HeaderText = GetResource("T_TRANS1");

								tabStyle.GridColumnStyles[CN.descr2_en].Width = 250;
								tabStyle.GridColumnStyles[CN.descr2_en].NullText = "";
								tabStyle.GridColumnStyles[CN.descr2_en].ReadOnly = true;
								tabStyle.GridColumnStyles[CN.descr2_en].HeaderText = GetResource("T_ENGLISHDESC2");

								tabStyle.GridColumnStyles[CN.descr2].Width = 250;
								tabStyle.GridColumnStyles[CN.descr2].NullText = "";
								tabStyle.GridColumnStyles[CN.descr2].ReadOnly = false;
								tabStyle.GridColumnStyles[CN.descr2].HeaderText = GetResource("T_TRANS2");

								//((DataGridBoolColumn)tabStyle.GridColumnStyles["enabled"]).AllowNull = false;
								//((DataGridBoolColumn)tabStyle.GridColumnStyles["visible"]).AllowNull = false;
								//((DataGridBoolColumn)tabStyle.GridColumnStyles["mandatory"]).AllowNull = false;
							}
						}
						else
						{
							((MainForm)this.FormRoot).statusBar1.Text = "0 Row(s) returned.";
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

		private void SaveUpdates()
		{
			try
			{
				Function = "SaveUpdates()";
				Wait();

				if(fields.HasChanges(DataRowState.Modified))	//have any records been modified
				{
					//Extract just the records that have changed to cut down traffic
					DataSet changes = fields.GetChanges(DataRowState.Modified);
					StaticDataManager.SaveStockItemTranslations(changes, out Error);
					if(Error.Length>0)
						ShowError(Error);
					else
						fields.AcceptChanges();		//commit changes to the dataset
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

		private void btnSave_Click(object sender, System.EventArgs e)
		{
			SaveUpdates();
		}

		private void menuExit_Click(object sender, System.EventArgs e)
		{
			CloseTab();
		}

        //private void panel1_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        //{
		
        //}

		private void txtDescr1_en_TextChanged(object sender, System.EventArgs e)
		{
		
		}

		private void txtDescr1_TextChanged(object sender, System.EventArgs e)
		{
		
		}

		private void label2_Click(object sender, System.EventArgs e)
		{
		
		}

		private void label4_Click(object sender, System.EventArgs e)
		{
		
		}

		private void label3_Click(object sender, System.EventArgs e)
		{
		
		}

		private void btnClear_Click(object sender, System.EventArgs e)
		{
			this.txtItemno.Text = "";
			this.txtDescr1.Text = "";
			this.txtDescr2.Text = "";
			this.txtDescr1_en.Text = "";
			this.txtDescr2_en.Text = "";
		}

		private void btnExit_Click(object sender, System.EventArgs e)
		{
			CloseTab();
		}

		private void btnSearch_Click(object sender, System.EventArgs e)
		{
			LoadData();		
		}

		private void btnSave_Click_1(object sender, System.EventArgs e)
		{
			SaveUpdates();
		}

		/// <summary>
		/// KEF - Added so fields are populatedwhen details in grid are selected
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void dgFields_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			Function = "dgFields_MouseUp";
			
			try
			{
				Wait();
				int index = dgFields.CurrentRowIndex;
				if(index>=0)
				{
					txtItemno.Text = dgFields[index, 0] as string;
					txtDescr1.Text = dgFields[index, 2] as string;
					txtDescr2.Text = dgFields[index, 4] as string;
					txtDescr1_en.Text = dgFields[index, 1] as string;
					txtDescr2_en.Text = dgFields[index, 3] as string;
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
