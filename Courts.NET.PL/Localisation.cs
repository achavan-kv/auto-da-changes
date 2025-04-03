using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Resources;
using System.Globalization;
using System.Threading;
using System.Collections.Specialized;
using STL.Common;
using STL.Common.Static;
using System.Reflection;
using System.Data;
using STL.Common.Constants.ColumnNames;
using STL.PL.WS5;
using System.Web.Services.Protocols;
using System.Text.RegularExpressions;

namespace STL.PL
{
	/// <summary>
	/// Maintenance screen to translate system messages and field titles
	/// to another language. CoSACS stores a translation dictionary on
	/// the database to translate all displayed and printed text, so 
	/// enabling the application to be used in different languages. Each
	/// unique word or phrase is held only once in the dictionary so that
	/// it only needs to be translated once. Any entries without a translation
	/// will appear in English in the application. This screen will list all
	/// system messages or list all field titles by screen. After a new release 
	/// the users can find the entries without a translation and update
	/// the dictionary.
	/// </summary>
	public class Localisation : CommonForm
	{
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.ComboBox drpCulture;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.DataGrid dgTranslation;
		//private Crownwood.Magic.Menus.MenuControl menuMain;
		private Crownwood.Magic.Menus.MenuCommand menuFile;
		private Crownwood.Magic.Menus.MenuCommand menuSave;
		private Crownwood.Magic.Menus.MenuCommand menuExit;
		private DataTable transTab =  null;
		private DataSet transDS = null;
		private System.Windows.Forms.Button btnSave;
		private new string Error = "";
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.RadioButton rbScreens;
		private System.Windows.Forms.RadioButton rbMessages;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ComboBox drpScreen;
		private System.Windows.Forms.TextBox txtMessages;
		private bool Loaded = false;
		private STL.PL.Alphabet Alphabet;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public Localisation(TranslationDummy d)
		{
			InitializeComponent();
			menuMain = new Crownwood.Magic.Menus.MenuControl();
			menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {menuFile});						
		}

		public Localisation()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			menuMain = new Crownwood.Magic.Menus.MenuControl();
			menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {menuFile});	
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(Localisation));
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.txtMessages = new System.Windows.Forms.TextBox();
			this.rbScreens = new System.Windows.Forms.RadioButton();
			this.rbMessages = new System.Windows.Forms.RadioButton();
			this.label1 = new System.Windows.Forms.Label();
			this.drpScreen = new System.Windows.Forms.ComboBox();
			this.btnSave = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.drpCulture = new System.Windows.Forms.ComboBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.Alphabet = new STL.PL.Alphabet();
			this.dgTranslation = new System.Windows.Forms.DataGrid();
			this.menuFile = new Crownwood.Magic.Menus.MenuCommand();
			this.menuSave = new Crownwood.Magic.Menus.MenuCommand();
			this.menuExit = new Crownwood.Magic.Menus.MenuCommand();
			this.groupBox1.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.groupBox2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dgTranslation)).BeginInit();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.BackColor = System.Drawing.SystemColors.Control;
			this.groupBox1.Controls.AddRange(new System.Windows.Forms.Control[] {
																					this.groupBox3,
																					this.label2,
																					this.drpCulture});
			this.groupBox1.Location = new System.Drawing.Point(8, 0);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(776, 128);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Selection Criteria:";
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.AddRange(new System.Windows.Forms.Control[] {
																					this.txtMessages,
																					this.rbScreens,
																					this.rbMessages,
																					this.label1,
																					this.drpScreen,
																					this.btnSave});
			this.groupBox3.Location = new System.Drawing.Point(248, 16);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(520, 100);
			this.groupBox3.TabIndex = 7;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Translate";
			// 
			// txtMessages
			// 
			this.txtMessages.Location = new System.Drawing.Point(128, 14);
			this.txtMessages.Name = "txtMessages";
			this.txtMessages.ReadOnly = true;
			this.txtMessages.TabIndex = 11;
			this.txtMessages.Text = "System Messages";
			// 
			// rbScreens
			// 
			this.rbScreens.Location = new System.Drawing.Point(88, 62);
			this.rbScreens.Name = "rbScreens";
			this.rbScreens.Size = new System.Drawing.Size(16, 24);
			this.rbScreens.TabIndex = 10;
			// 
			// rbMessages
			// 
			this.rbMessages.Checked = true;
			this.rbMessages.Location = new System.Drawing.Point(88, 14);
			this.rbMessages.Name = "rbMessages";
			this.rbMessages.Size = new System.Drawing.Size(16, 24);
			this.rbMessages.TabIndex = 9;
			this.rbMessages.TabStop = true;
			this.rbMessages.CheckedChanged += new System.EventHandler(this.rbMessages_CheckedChanged);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(128, 46);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(48, 16);
			this.label1.TabIndex = 8;
			this.label1.Text = "Screens:";
			// 
			// drpScreen
			// 
			this.drpScreen.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.drpScreen.Enabled = false;
			this.drpScreen.ItemHeight = 13;
			this.drpScreen.Location = new System.Drawing.Point(128, 62);
			this.drpScreen.Name = "drpScreen";
			this.drpScreen.Size = new System.Drawing.Size(200, 21);
			this.drpScreen.TabIndex = 7;
			this.drpScreen.SelectedIndexChanged += new System.EventHandler(this.drpScreen_SelectedIndexChanged);
			// 
			// btnSave
			// 
			this.btnSave.BackgroundImage = ((System.Drawing.Bitmap)(resources.GetObject("btnSave.BackgroundImage")));
			this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.btnSave.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.btnSave.Location = new System.Drawing.Point(480, 32);
			this.btnSave.Name = "btnSave";
			this.btnSave.Size = new System.Drawing.Size(24, 24);
			this.btnSave.TabIndex = 2;
			this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(24, 48);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(88, 16);
			this.label2.TabIndex = 3;
			this.label2.Text = "Culture Setting:";
			// 
			// drpCulture
			// 
			this.drpCulture.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.drpCulture.Location = new System.Drawing.Point(24, 64);
			this.drpCulture.Name = "drpCulture";
			this.drpCulture.Size = new System.Drawing.Size(200, 21);
			this.drpCulture.TabIndex = 0;
			this.drpCulture.SelectedIndexChanged += new System.EventHandler(this.drpCulture_SelectedIndexChanged);
			// 
			// groupBox2
			// 
			this.groupBox2.BackColor = System.Drawing.SystemColors.Control;
			this.groupBox2.Controls.AddRange(new System.Windows.Forms.Control[] {
																					this.Alphabet,
																					this.dgTranslation});
			this.groupBox2.Location = new System.Drawing.Point(8, 128);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(776, 344);
			this.groupBox2.TabIndex = 1;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Translation";
			// 
			// Alphabet
			// 
			this.Alphabet.Location = new System.Drawing.Point(64, 24);
			this.Alphabet.Name = "Alphabet";
			this.Alphabet.Size = new System.Drawing.Size(656, 24);
			this.Alphabet.TabIndex = 1;
			this.Alphabet.Visible = false;
			this.Alphabet.Clicked += new STL.PL.AlphabetEventHandler(this.OnAlphabetClicked);
			// 
			// dgTranslation
			// 
			this.dgTranslation.CaptionText = "Translation";
			this.dgTranslation.DataMember = "";
			this.dgTranslation.HeaderForeColor = System.Drawing.SystemColors.ControlText;
			this.dgTranslation.Location = new System.Drawing.Point(24, 56);
			this.dgTranslation.Name = "dgTranslation";
			this.dgTranslation.Size = new System.Drawing.Size(736, 272);
			this.dgTranslation.TabIndex = 0;
			this.dgTranslation.TabStop = false;
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
			// 
			// menuExit
			// 
			this.menuExit.Description = "MenuItem";
			this.menuExit.Text = "E&xit";
			this.menuExit.Click += new System.EventHandler(this.menuExit_Click);
			// 
			// Localisation
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.BackColor = System.Drawing.SystemColors.ControlDark;
			this.ClientSize = new System.Drawing.Size(792, 478);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.groupBox2,
																		  this.groupBox1});
			this.Name = "Localisation";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Screen Translation";
			this.Load += new System.EventHandler(this.Localisation_Load);
			this.groupBox1.ResumeLayout(false);
			this.groupBox3.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dgTranslation)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion

		private void menuExit_Click(object sender, System.EventArgs e)
		{
			CloseTab();
		}

		private void drpScreen_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			//Create an instance of the selected type and list the text of all
			//the controls
			try
			{
				Function = "drpScreen_SelectedIndexChanged";
				Wait();

				Type t = (Type)drpScreen.SelectedItem;

				if(t!=null)
				{
					object target = t.InvokeMember(null, BindingFlags.CreateInstance, null, null, new object[] {new TranslationDummy()});
					StringCollection labels = (StringCollection)t.InvokeMember("ListLabels", BindingFlags.InvokeMethod, null, target, null);

					//retrieve the dictionary for this culture
					LoadDictionary(((CultureInfo)drpCulture.SelectedItem).Name);				
					Hashtable ht = (Hashtable)StaticData.Dictionaries[((CultureInfo)drpCulture.SelectedItem).Name];

					RefreshTable();
					foreach(string s in labels)
					{
						DataRow r = transTab.NewRow();
						r[CN.Culture] = ((CultureInfo)drpCulture.SelectedItem).Name;
						r[CN.English] = s;

						string trans = "";
						if(ht!=null)
							trans = (string)ht[s];
						r[CN.Translation] = trans;
						transTab.Rows.Add(r);
					}
					transTab.AcceptChanges();
					dgTranslation.DataSource = transTab.DefaultView;
				}
				else
					RefreshTable();
			}
			catch(Exception ex)
			{
				Catch(ex, Function);
			}
			finally
			{
				StopWait();
				Function  = "End of drpScreen_SelectedIndexChanged";
			}
		}

		private void Localisation_Load(object sender, System.EventArgs e)
		{
			try
			{
				Wait();
				Function = "Localisation_Load";				
				
				CultureInfo[] a = CultureInfo.GetCultures(CultureTypes.SpecificCultures);
				BubbleSort(a);
				drpCulture.DataSource = a;
				drpCulture.DisplayMember = "DisplayName";
				CultureInfo current = new CultureInfo(Config.Culture);
				drpCulture.SelectedIndex = drpCulture.FindStringExact(current.DisplayName);

				Assembly ass = Assembly.GetExecutingAssembly();
				Module[] m = ass.GetModules();
				Type[] t = m[0].GetTypes();

				Type form = typeof(STL.PL.CommonForm);
				Type userControl = typeof(STL.PL.CommonUserControl);
				foreach(Type type in t)
				{
					if(type.IsSubclassOf(form) || type.IsSubclassOf(userControl))
					{
						drpScreen.Items.Add(type);
					}
				}
				drpScreen.DisplayMember = "Name";
				txtMessages.BackColor = SystemColors.Window;
				LoadMessages();
				Loaded = true;
			}
			catch(Exception ex)
			{
				Catch(ex, Function);
			}
			finally
			{
				StopWait();
				Function = "End of Localisation_Load";
			}
		}

		private void RefreshTable()
		{
			transTab = null;
			transDS = null;

			transTab = new DataTable();
			transDS = new DataSet();
			transDS.Tables.Add(transTab);
			transTab.Columns.AddRange(new DataColumn[] { new DataColumn(CN.Culture),
														   new DataColumn(CN.English),
														   new DataColumn(CN.Translation),
														   new DataColumn(CN.Key) });
			transTab.DefaultView.AllowNew = false;
			dgTranslation.DataSource = transTab.DefaultView;

			if(dgTranslation.TableStyles.Count==0)
			{
				DataGridTableStyle tabStyle = new DataGridTableStyle();
				tabStyle.MappingName = transTab.TableName;				
				dgTranslation.TableStyles.Add(tabStyle);
		
				tabStyle.GridColumnStyles[CN.Culture].Width = 0;
				tabStyle.GridColumnStyles[CN.English].Width = 300;
				tabStyle.GridColumnStyles[CN.English].ReadOnly = true;
				tabStyle.GridColumnStyles[CN.Translation].Width = 300;
				tabStyle.GridColumnStyles[CN.Translation].NullText = "";
				tabStyle.GridColumnStyles[CN.Key].Width = 0;
			}
		}

		private void btnSave_Click(object sender, System.EventArgs e)
		{
			try
			{
				Function = "btnSave_Click";
				Wait();

				string culture = ((CultureInfo)drpCulture.SelectedItem).Name;
				StaticDataManager.SaveDictionary(culture, transDS, out Error);
				if(Error.Length > 0)
					ShowError(Error);
				else
				{
					StaticData.Dictionaries.Remove(culture);
					LoadDictionary(culture);
					transTab.AcceptChanges();
				}
			}
			catch(Exception ex)
			{
				Catch(ex, Function);
			}
			finally
			{
				StopWait();
				Function = "End of btnSave_Click";
			}
		
		}

		private void rbMessages_CheckedChanged(object sender, System.EventArgs e)
		{
			drpScreen.Enabled = !rbMessages.Checked;
			if(drpScreen.Enabled)
			{
				Alphabet.Visible = false;
				drpScreen_SelectedIndexChanged(this, new System.EventArgs());
			}
			else
			{
				LoadMessages();
			}
		}

		private void drpCulture_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if(this.Loaded)
			{
				//If the screen dropdown is enabled, call the screen index changed event
				//otherwise load the messages for this culture
				if(drpScreen.Enabled)
				{
					drpScreen_SelectedIndexChanged(this, new System.EventArgs());
				}
				else
				{
					LoadMessages();
				}
			}
		}

		private void LoadMessages()
		{
			Alphabet.Visible = true;

			LoadDictionary(((CultureInfo)drpCulture.SelectedItem).Name);				
			Hashtable ht = (Hashtable)StaticData.Dictionaries[((CultureInfo)drpCulture.SelectedItem).Name];
			RefreshTable();	
			IDictionaryEnumerator myEnumerator = Messages.List.GetEnumerator();
			while ( myEnumerator.MoveNext() )
			{
				bool addMessage = false;
				string english = (string)myEnumerator.Value;
				if(_letterClicked=="0")
				{
					Regex reg = new Regex("^[^a-z]$");	
					if (reg.IsMatch(english.ToLower().Substring(0,1)))
						addMessage = true;
				}
				else
					if(english.ToLower().StartsWith(_letterClicked.ToLower()))
						addMessage = true;

				if(addMessage)
				{
					DataRow r = transTab.NewRow();
					r[CN.Culture] = ((CultureInfo)drpCulture.SelectedItem).Name;
					r[CN.English] = english;
					r[CN.Key] = (string)myEnumerator.Key;

					string trans = "";
					if(ht!=null)
						trans = (string)ht[myEnumerator.Key];
					r[CN.Translation] = trans;
					transTab.Rows.Add(r);
				}
			}
			transTab.AcceptChanges();
			dgTranslation.DataSource = transTab.DefaultView;
		}

		private string _letterClicked = "0";

		private void OnAlphabetClicked(object sender, AlphabetEventArgs e)
		{
			try
			{
				Wait();

				if(_letterClicked != e.Clicked)
				{
					this.btnSave_Click(null, null);
					_letterClicked = e.Clicked;
					LoadMessages();
				}

			}
			catch(Exception ex)
			{
				Catch(ex, "OnAlphabetClicked");
			}
			finally
			{
				StopWait();
			}
		}

	}
}
