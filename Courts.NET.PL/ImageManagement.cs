using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.IO;
using System.Data;
using STL.Common.Constants.ColumnNames;

namespace STL.PL
{
	/// <summary>
	/// A browser screen to find customer photographs and view them on screen.
	/// </summary>
	public class ImageManagement : CommonForm
	{
		//private Crownwood.Magic.Menus.MenuControl menuMain;
		private Crownwood.Magic.Menus.MenuCommand menuFile;
		private Crownwood.Magic.Menus.MenuCommand menuExit;
		private System.Windows.Forms.GroupBox groupBox1;
		private HyperCoder.Win.FileSystemControls.FolderTree ftBrowser;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Splitter splitter1;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.TextBox txtFirstName;
		private System.Windows.Forms.TextBox txtAlias;
		private System.Windows.Forms.TextBox txtLastName;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		public System.Windows.Forms.TextBox txtCustID;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.PictureBox pbSelectedImage;
		private HyperCoder.Win.FileSystemControls.FileList flFiles;
		private System.Windows.Forms.Button btnSave;
		private new string Error = "";
		private System.Windows.Forms.TextBox txtTitle;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public ImageManagement(TranslationDummy d)
		{
			InitializeComponent();
			menuMain = new Crownwood.Magic.Menus.MenuControl();
			menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {menuFile});
		}

		public ImageManagement(Form root, Form parent)
		{
			InitializeComponent();

			FormRoot = root;
			FormParent = parent;
			txtFirstName.BackColor = SystemColors.Window;
			txtLastName.BackColor = SystemColors.Window;
			txtAlias.BackColor = SystemColors.Window;
			txtTitle.BackColor = SystemColors.Window;
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(ImageManagement));
			this.menuFile = new Crownwood.Magic.Menus.MenuCommand();
			this.menuExit = new Crownwood.Magic.Menus.MenuCommand();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.panel1 = new System.Windows.Forms.Panel();
			this.flFiles = new HyperCoder.Win.FileSystemControls.FileList();
			this.ftBrowser = new HyperCoder.Win.FileSystemControls.FolderTree();
			this.splitter1 = new System.Windows.Forms.Splitter();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.txtTitle = new System.Windows.Forms.TextBox();
			this.btnSave = new System.Windows.Forms.Button();
			this.txtFirstName = new System.Windows.Forms.TextBox();
			this.txtAlias = new System.Windows.Forms.TextBox();
			this.txtLastName = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.txtCustID = new System.Windows.Forms.TextBox();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.pbSelectedImage = new System.Windows.Forms.PictureBox();
			this.groupBox1.SuspendLayout();
			this.panel1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupBox3.SuspendLayout();
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
			this.menuExit.Click += new System.EventHandler(this.menuExit_Click);
			// 
			// groupBox1
			// 
			this.groupBox1.BackColor = System.Drawing.SystemColors.Control;
			this.groupBox1.Controls.AddRange(new System.Windows.Forms.Control[] {
																					this.panel1});
			this.groupBox1.Location = new System.Drawing.Point(8, 192);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(776, 280);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Browse Files";
			// 
			// panel1
			// 
			this.panel1.Controls.AddRange(new System.Windows.Forms.Control[] {
																				 this.flFiles,
																				 this.splitter1,
																				 this.ftBrowser});
			this.panel1.Location = new System.Drawing.Point(16, 24);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(744, 240);
			this.panel1.TabIndex = 2;
			// 
			// flFiles
			// 
			this.flFiles.Activation = System.Windows.Forms.ItemActivation.Standard;
			this.flFiles.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.flFiles.ColumnDateModified = "Date Modified";
			this.flFiles.ColumnFileName = "Name";
			this.flFiles.ColumnFileSize = "Size";
			this.flFiles.ColumnFileType = "Type";
			this.flFiles.Cursor = System.Windows.Forms.Cursors.Default;
			this.flFiles.Dock = System.Windows.Forms.DockStyle.Fill;
			this.flFiles.FileNameBackColor = System.Drawing.Color.WhiteSmoke;
			this.flFiles.Filter = "*.jpg";
			this.flFiles.Folder = "C:\\Documents and Settings\\davidr\\Desktop";
			this.flFiles.FolderTree = this.ftBrowser;
			this.flFiles.FullRowSelect = false;
			this.flFiles.GridLines = false;
			this.flFiles.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Clickable;
			this.flFiles.HideSelection = true;
			this.flFiles.HoverSelection = false;
			this.flFiles.LabelEdit = false;
			this.flFiles.LabelWrap = true;
			this.flFiles.Location = new System.Drawing.Point(226, 0);
			this.flFiles.MultiSelect = true;
			this.flFiles.Name = "flFiles";
			this.flFiles.ShowHiddenItems = true;
			this.flFiles.ShowSystemItems = true;
			this.flFiles.Size = new System.Drawing.Size(518, 240);
			this.flFiles.TabIndex = 3;
			this.flFiles.Text = "fileList1";
			this.flFiles.View = System.Windows.Forms.View.Details;
			this.flFiles.ItemActivate += new HyperCoder.Win.FileSystemControls.FileList.ItemActivateEventHandler(this.flFiles_ItemActivate);
			// 
			// ftBrowser
			// 
			this.ftBrowser.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.ftBrowser.Dock = System.Windows.Forms.DockStyle.Left;
			this.ftBrowser.FullRowSelect = false;
			this.ftBrowser.HideSelection = false;
			this.ftBrowser.HotTracking = false;
			this.ftBrowser.IconSize = HyperCoder.Win.FileSystemControls.FolderTree.IconSize2Display.Small;
			this.ftBrowser.IncludeFiles = false;
			this.ftBrowser.LabelEdit = false;
			this.ftBrowser.Name = "ftBrowser";
			this.ftBrowser.RootFolder = "Desktop";
			this.ftBrowser.ShowHiddenItems = true;
			this.ftBrowser.ShowLines = false;
			this.ftBrowser.ShowPlusMinus = true;
			this.ftBrowser.ShowRootLines = true;
			this.ftBrowser.ShowSystemItems = true;
			this.ftBrowser.Size = new System.Drawing.Size(224, 240);
			this.ftBrowser.TabIndex = 0;
			this.ftBrowser.Text = "folderTree1";
			// 
			// splitter1
			// 
			this.splitter1.Location = new System.Drawing.Point(224, 0);
			this.splitter1.Name = "splitter1";
			this.splitter1.Size = new System.Drawing.Size(2, 240);
			this.splitter1.TabIndex = 2;
			this.splitter1.TabStop = false;
			// 
			// groupBox2
			// 
			this.groupBox2.BackColor = System.Drawing.SystemColors.Control;
			this.groupBox2.Controls.AddRange(new System.Windows.Forms.Control[] {
																					this.txtTitle,
																					this.btnSave,
																					this.txtFirstName,
																					this.txtAlias,
																					this.txtLastName,
																					this.label5,
																					this.label4,
																					this.label3,
																					this.label2,
																					this.label1,
																					this.txtCustID});
			this.groupBox2.Location = new System.Drawing.Point(8, 0);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(384, 192);
			this.groupBox2.TabIndex = 1;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Customer Details";
			// 
			// txtTitle
			// 
			this.txtTitle.Location = new System.Drawing.Point(32, 96);
			this.txtTitle.MaxLength = 30;
			this.txtTitle.Name = "txtTitle";
			this.txtTitle.ReadOnly = true;
			this.txtTitle.Size = new System.Drawing.Size(96, 20);
			this.txtTitle.TabIndex = 39;
			this.txtTitle.TabStop = false;
			this.txtTitle.Text = "";
			// 
			// btnSave
			// 
			this.btnSave.BackgroundImage = ((System.Drawing.Bitmap)(resources.GetObject("btnSave.BackgroundImage")));
			this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.btnSave.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.btnSave.Location = new System.Drawing.Point(312, 40);
			this.btnSave.Name = "btnSave";
			this.btnSave.Size = new System.Drawing.Size(24, 24);
			this.btnSave.TabIndex = 38;
			this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
			// 
			// txtFirstName
			// 
			this.txtFirstName.Location = new System.Drawing.Point(208, 96);
			this.txtFirstName.MaxLength = 30;
			this.txtFirstName.Name = "txtFirstName";
			this.txtFirstName.ReadOnly = true;
			this.txtFirstName.Size = new System.Drawing.Size(128, 20);
			this.txtFirstName.TabIndex = 30;
			this.txtFirstName.TabStop = false;
			this.txtFirstName.Text = "";
			// 
			// txtAlias
			// 
			this.txtAlias.Location = new System.Drawing.Point(208, 144);
			this.txtAlias.MaxLength = 25;
			this.txtAlias.Name = "txtAlias";
			this.txtAlias.ReadOnly = true;
			this.txtAlias.Size = new System.Drawing.Size(128, 20);
			this.txtAlias.TabIndex = 32;
			this.txtAlias.TabStop = false;
			this.txtAlias.Text = "";
			// 
			// txtLastName
			// 
			this.txtLastName.Location = new System.Drawing.Point(32, 144);
			this.txtLastName.MaxLength = 60;
			this.txtLastName.Name = "txtLastName";
			this.txtLastName.ReadOnly = true;
			this.txtLastName.Size = new System.Drawing.Size(152, 20);
			this.txtLastName.TabIndex = 31;
			this.txtLastName.TabStop = false;
			this.txtLastName.Text = "";
			// 
			// label5
			// 
			this.label5.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.label5.Location = new System.Drawing.Point(32, 80);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(48, 16);
			this.label5.TabIndex = 37;
			this.label5.Text = "Title:";
			// 
			// label4
			// 
			this.label4.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.label4.Location = new System.Drawing.Point(208, 80);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(100, 16);
			this.label4.TabIndex = 36;
			this.label4.Text = "First Name:";
			// 
			// label3
			// 
			this.label3.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.label3.Location = new System.Drawing.Point(32, 128);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(100, 16);
			this.label3.TabIndex = 35;
			this.label3.Text = "Last Name:";
			// 
			// label2
			// 
			this.label2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.label2.Location = new System.Drawing.Point(208, 128);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(100, 16);
			this.label2.TabIndex = 34;
			this.label2.Text = "Alias:";
			// 
			// label1
			// 
			this.label1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.label1.Location = new System.Drawing.Point(32, 32);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(100, 16);
			this.label1.TabIndex = 33;
			this.label1.Text = "Customer ID:";
			// 
			// txtCustID
			// 
			this.txtCustID.Location = new System.Drawing.Point(32, 48);
			this.txtCustID.MaxLength = 20;
			this.txtCustID.Name = "txtCustID";
			this.txtCustID.Size = new System.Drawing.Size(120, 20);
			this.txtCustID.TabIndex = 28;
			this.txtCustID.Text = "";
			this.txtCustID.Leave += new System.EventHandler(this.txtCustID_Leave);
			// 
			// groupBox3
			// 
			this.groupBox3.BackColor = System.Drawing.SystemColors.Control;
			this.groupBox3.Controls.AddRange(new System.Windows.Forms.Control[] {
																					this.pbSelectedImage});
			this.groupBox3.Location = new System.Drawing.Point(392, 0);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(392, 192);
			this.groupBox3.TabIndex = 2;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Current Image";
			// 
			// pbSelectedImage
			// 
			this.pbSelectedImage.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.pbSelectedImage.Location = new System.Drawing.Point(88, 32);
			this.pbSelectedImage.Name = "pbSelectedImage";
			this.pbSelectedImage.Size = new System.Drawing.Size(224, 136);
			this.pbSelectedImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.pbSelectedImage.TabIndex = 0;
			this.pbSelectedImage.TabStop = false;
			// 
			// ImageManagement
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.BackColor = System.Drawing.SystemColors.ControlDark;
			this.ClientSize = new System.Drawing.Size(792, 477);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.groupBox3,
																		  this.groupBox2,
																		  this.groupBox1});
			this.Name = "ImageManagement";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "ImageManagement";
			this.groupBox1.ResumeLayout(false);
			this.panel1.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.groupBox3.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void menuExit_Click(object sender, System.EventArgs e)
		{
			CloseTab();
		}

		private string path = "";

		private void flFiles_ItemActivate(HyperCoder.Win.FileSystemControls.FileList.FileListItem SelectedItem)
		{
			try
			{
				if(File.Exists(flFiles.SelectedItem.FullName))
				{
					path = flFiles.SelectedItem.FullName;
					pbSelectedImage.Image = new Bitmap(flFiles.SelectedItem.FullName);
				}
			}
			catch(ArgumentException)
			{
				ShowInfo("M_INVALIDIMAGEFILE");
			}
			catch(Exception ex)
			{
				Catch(ex, "flFiles_ItemActivate");
			}
		}

		private void btnSave_Click(object sender, System.EventArgs e)
		{
			try
			{
				Wait();

				if(txtCustID.Text.Length!=0 &&
					pbSelectedImage.Image != null)
				{

					MemoryStream stream = new MemoryStream();				
					pbSelectedImage.Image.Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg);

					stream.Position = 0;

					byte[] image = new byte[stream.Length];
					int read = stream.Read(image, 0, image.Length);

					CustomerManager.UpdateImage(txtCustID.Text, image, out Error);
					if(Error.Length > 0)
						ShowError(Error);
				}

			}
			catch(Exception ex)
			{
				Catch(ex, "btnSave_Click");
			}
			finally
			{
				StopWait();
			}
		}

		private void txtCustID_Leave(object sender, System.EventArgs e)
		{
			try
			{
				Wait();

				DataSet ds = CustomerManager.GetBasicCustomerDetails(txtCustID.Text, "", "H", out Error);
				if(Error.Length>0)
					ShowError(Error);
				else
				{
					if(ds!=null)
					{
						foreach(DataTable dt in ds.Tables)
						{
							if(dt.TableName == "BasicDetails")
							{
								foreach (DataRow row in dt.Rows)
								{
									txtFirstName.Text = (string)row[CN.FirstName];
									txtLastName.Text = (string)row[CN.LastName];
									txtTitle.Text = (string)row[CN.Title];
									txtAlias.Text = (string)row[CN.Alias];
								}
							}
						}
					}
					else
					{
						txtCustID.Text = "";
						txtFirstName.Text = "";
						txtLastName.Text = "";
						txtTitle.Text = "";
						txtAlias.Text = "";
					}
				}

				byte[] image = CustomerManager.GetImage(txtCustID.Text, out Error);
				if(Error.Length>0)
					ShowError(Error);
				else
				{
					if(image != null)
					{
						MemoryStream stream = new MemoryStream();	
						stream.Write(image, 0, image.Length);

						pbSelectedImage.Image = new Bitmap(stream);
					}
					else
					{
						pbSelectedImage.Image = null;
					}
				}
			}
			catch(Exception ex)
			{
				Catch(ex, "btnSave_Click");
			}
			finally
			{
				StopWait();
			}
		}
	}
}
