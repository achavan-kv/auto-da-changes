using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using STL.Common;
using STL.PL.WS5;
using STL.PL.WS6;
using System.Data;
using System.Web.Services.Protocols;
using System.Xml;
using STL.Common.Constants.TableNames;
using STL.Common.Static;

namespace STL.PL
{
	/// <summary>
	/// Screen to maintain the permissions assigned to each user type. All users
	/// are defined as a certain type, such as cashier or system administrator.
	/// Depending upon the user type different functions are available in the application.
	/// A permission enables a system function that might be a main menu item
	/// or a single field or button on a particular screen. A permission can also
	/// control a group of functions, such as muliple menu items or a menu item
	/// and a button.
	/// All user types are listed next to all of the functions requiring permission.
	/// For the selected user type a tick is displayed next to each function that
	/// is permitted for that user type. The ticks can be updated to assign more
	/// or less permitted functions to the user type.
	/// </summary>
	public class MenuMaintenance : CommonForm
	{
		private System.Windows.Forms.GroupBox grpUserTypes;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.ColumnHeader userType;
		private System.Windows.Forms.ColumnHeader userDescription;
		private System.Windows.Forms.ColumnHeader function;
		private new string Error = "";
		private System.Windows.Forms.ListView lvUserTypes;
		private System.Windows.Forms.ListView lvFunctions;
		private System.Windows.Forms.Button btnSave;
		//private Crownwood.Magic.Menus.MenuControl menuMain;
		private Crownwood.Magic.Menus.MenuCommand menuFile;
		private Crownwood.Magic.Menus.MenuCommand menuSave;
		private Crownwood.Magic.Menus.MenuCommand menuExit;
		private System.Windows.Forms.Button btnExit;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public MenuMaintenance(TranslationDummy d)
		{
			InitializeComponent();
			menuMain = new Crownwood.Magic.Menus.MenuControl();
			menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {menuFile});
		}

		public MenuMaintenance(Form root, Form parent)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			FormRoot = root;
			FormParent = parent;
			menuMain = new Crownwood.Magic.Menus.MenuControl();
			menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {menuFile});
			//TranslateControls();

			//load up the user types
			try
			{
				Function = "WStaticDataManager::GetDropDowns()";
			
				XmlUtilities xml = new XmlUtilities();
				XmlDocument dropDowns = new XmlDocument();
				dropDowns.LoadXml("<DROP_DOWNS></DROP_DOWNS>");

				if(StaticData.Tables[TN.UserTypes]==null)
					dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns,TN.UserTypes, null));
				if(StaticData.Tables[TN.UserFunctions]==null)
					dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns,TN.UserFunctions, null));

				if(dropDowns.DocumentElement.ChildNodes.Count>0)
				{
					DataSet ds = StaticDataManager.GetDropDownData(dropDowns.DocumentElement, out Error);
					if(Error.Length>0)
						ShowError(Error);
					else
					{
						foreach(DataTable dt in ds.Tables)
						{
							StaticData.Tables[dt.TableName] = dt;
						}
					}
				}

				foreach(DataRow row in ((DataTable)StaticData.Tables[TN.UserTypes]).Rows)
				{
					lvUserTypes.Items.Add(new ListViewItem(new string[] {row["code"].ToString(), row["codedescript"].ToString()}));
				}
				foreach(DataRow row in ((DataTable)StaticData.Tables[TN.UserFunctions]).Rows)
				{
					lvFunctions.Items.Add(new ListViewItem(new string[] {row["TaskName"].ToString()}));
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(MenuMaintenance));
			this.grpUserTypes = new System.Windows.Forms.GroupBox();
			this.btnExit = new System.Windows.Forms.Button();
			this.btnSave = new System.Windows.Forms.Button();
			this.lvUserTypes = new System.Windows.Forms.ListView();
			this.userType = new System.Windows.Forms.ColumnHeader();
			this.userDescription = new System.Windows.Forms.ColumnHeader();
			this.label3 = new System.Windows.Forms.Label();
			this.lvFunctions = new System.Windows.Forms.ListView();
			this.function = new System.Windows.Forms.ColumnHeader();
			this.label2 = new System.Windows.Forms.Label();
			this.menuFile = new Crownwood.Magic.Menus.MenuCommand();
			this.menuSave = new Crownwood.Magic.Menus.MenuCommand();
			this.menuExit = new Crownwood.Magic.Menus.MenuCommand();
			this.grpUserTypes.SuspendLayout();
			this.SuspendLayout();
			// 
			// grpUserTypes
			// 
			this.grpUserTypes.BackColor = System.Drawing.SystemColors.Control;
			this.grpUserTypes.Controls.AddRange(new System.Windows.Forms.Control[] {
																					   this.btnExit,
																					   this.btnSave,
																					   this.lvUserTypes,
																					   this.label3,
																					   this.lvFunctions,
																					   this.label2});
			this.grpUserTypes.Location = new System.Drawing.Point(8, 0);
			this.grpUserTypes.Name = "grpUserTypes";
			this.grpUserTypes.Size = new System.Drawing.Size(776, 472);
			this.grpUserTypes.TabIndex = 1;
			this.grpUserTypes.TabStop = false;
			// 
			// btnExit
			// 
			this.btnExit.Location = new System.Drawing.Point(88, 16);
			this.btnExit.Name = "btnExit";
			this.btnExit.Size = new System.Drawing.Size(48, 24);
			this.btnExit.TabIndex = 38;
			this.btnExit.Text = "Exit";
			this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
			// 
			// btnSave
			// 
			this.btnSave.Location = new System.Drawing.Point(24, 16);
			this.btnSave.Name = "btnSave";
			this.btnSave.Size = new System.Drawing.Size(48, 24);
			this.btnSave.TabIndex = 37;
			this.btnSave.Text = "Save";
			this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
			// 
			// lvUserTypes
			// 
			this.lvUserTypes.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
																						  this.userType,
																						  this.userDescription});
			this.lvUserTypes.FullRowSelect = true;
			this.lvUserTypes.HideSelection = false;
			this.lvUserTypes.Location = new System.Drawing.Point(24, 64);
			this.lvUserTypes.MultiSelect = false;
			this.lvUserTypes.Name = "lvUserTypes";
			this.lvUserTypes.Size = new System.Drawing.Size(296, 392);
			this.lvUserTypes.TabIndex = 5;
			this.lvUserTypes.View = System.Windows.Forms.View.Details;
			this.lvUserTypes.SelectedIndexChanged += new System.EventHandler(this.lvUserTypes_SelectedIndexChanged);
			// 
			// userType
			// 
			this.userType.Text = "User Type";
			this.userType.Width = 77;
			// 
			// userDescription
			// 
			this.userDescription.Text = "Description";
			this.userDescription.Width = 207;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(384, 48);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(112, 16);
			this.label3.TabIndex = 3;
			this.label3.Text = "Permitted Functions:";
			// 
			// lvFunctions
			// 
			this.lvFunctions.CheckBoxes = true;
			this.lvFunctions.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
																						  this.function});
			this.lvFunctions.FullRowSelect = true;
			this.lvFunctions.HideSelection = false;
			this.lvFunctions.Location = new System.Drawing.Point(384, 64);
			this.lvFunctions.Name = "lvFunctions";
			this.lvFunctions.Size = new System.Drawing.Size(368, 392);
			this.lvFunctions.TabIndex = 2;
			this.lvFunctions.View = System.Windows.Forms.View.Details;
			// 
			// function
			// 
			this.function.Text = "Function Description";
			this.function.Width = 364;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(24, 48);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(100, 16);
			this.label2.TabIndex = 1;
			this.label2.Text = "User Types:";
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
			// MenuMaintenance
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.BackColor = System.Drawing.SystemColors.ControlDark;
			this.ClientSize = new System.Drawing.Size(792, 477);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.grpUserTypes});
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "MenuMaintenance";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Customise Menus";
			this.grpUserTypes.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void menuExit_Click(object sender, System.EventArgs e)
		{
			CloseTab();
		}

		private void lvUserTypes_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			try
			{
				string userType = "";
				Wait();

				foreach(ListViewItem item in lvFunctions.Items)
				{
					item.Selected = false;
					item.Checked = false;
				}
				
				foreach(int i in lvUserTypes.SelectedIndices)	//should only be one
					userType = lvUserTypes.Items[i].SubItems[0].Text;

				DataSet ds = SystemConfig.GetFunctionsForType(userType, out Error);
				if(Error.Length>0)
					ShowError(Error);
				else
				{
					foreach(DataRow row in ds.Tables["Functions"].Rows)
					{
						string function = (string)row["TaskName"];
						foreach(ListViewItem item in lvFunctions.Items)
						{
							if(item.SubItems[0].Text == function)
							{
								//item.Selected = true;
								item.Checked = true;
							}
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

		private void btnSave_Click(object sender, System.EventArgs e)
		{
			try
			{
				string userType = "";
				string[] functions = null;
				int x=0;
				Wait();

				if(lvUserTypes.SelectedIndices.Count > 0)
				{

					foreach(int i in lvUserTypes.SelectedIndices)	//should only be one
						userType = lvUserTypes.Items[i].SubItems[0].Text;

					functions = new string[lvFunctions.CheckedIndices.Count];

					foreach(int i in lvFunctions.CheckedIndices)
					{
						functions[x] = lvFunctions.Items[i].SubItems[0].Text;
						x++;
					}

					SystemConfig.UpdateFunctionsForRole(userType, functions, out Error);
					if(Error.Length>0)
						ShowError(Error);
				}
				else
				{
					ShowInfo("M_SELECTUSERROLE");
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

		private void btnExit_Click(object sender, System.EventArgs e)
		{
			CloseTab();
		}
	}
}
