using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using STL.Common.Static;
using STL.PL.WS5;

namespace STL.PL
{
	/// <summary>
	/// Version and connection information for the application. The version of the
	/// server and the client are shown. These version numbers should be the same.
	/// The name of the web server and the database server are shown and may be
	/// different servers.
	/// </summary>
	public class About : CommonForm
	{
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.TextBox txtUrl;
		private System.Windows.Forms.TextBox txtClientVersion;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.TextBox txtServerVersion;
		private System.Windows.Forms.GroupBox groupBox4;
		private System.Windows.Forms.TextBox txtDatabase;
        private new string Error = "";
		private System.Windows.Forms.GroupBox groupBox5;
		private System.Windows.Forms.TextBox txtDatabaseVersion;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public About(TranslationDummy d)
		{
			InitializeComponent();
			//TranslateControls();
		}

		public About()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			Populate();
		}

		private void Populate()
		{
			try
			{
				Function = "Populate";
				Wait();

				txtDatabase.BackColor = SystemColors.Window;
				txtDatabaseVersion.BackColor = SystemColors.Window;
				txtServerVersion.BackColor = SystemColors.Window;
				txtUrl.Text = Config.Url;
				txtUrl.BackColor = SystemColors.Window;
				txtClientVersion.Text = Application.ProductVersion + "  (.Net Framework " + Environment.Version.ToString()+")";
				txtClientVersion.BackColor = SystemColors.Window;

				if(Credential.Name!=null)
				{
					string server = "";
					string db = "";
					string dbVersion = "";
					StaticDataManager.GetVersion(out server, out db, out dbVersion, out Error);
					if(Error.Length>0)
						ShowError(Error);
					else
					{
						txtServerVersion.Text = server;
						txtDatabase.Text = db;
						txtDatabaseVersion.Text = dbVersion;
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
				Function = "End of Populate()";
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(About));
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.txtClientVersion = new System.Windows.Forms.TextBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.txtUrl = new System.Windows.Forms.TextBox();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.txtServerVersion = new System.Windows.Forms.TextBox();
			this.groupBox4 = new System.Windows.Forms.GroupBox();
			this.txtDatabase = new System.Windows.Forms.TextBox();
			this.groupBox5 = new System.Windows.Forms.GroupBox();
			this.txtDatabaseVersion = new System.Windows.Forms.TextBox();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.groupBox4.SuspendLayout();
			this.groupBox5.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.AccessibleDescription = ((string)(resources.GetObject("groupBox1.AccessibleDescription")));
			this.groupBox1.AccessibleName = ((string)(resources.GetObject("groupBox1.AccessibleName")));
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("groupBox1.Anchor")));
			this.groupBox1.BackColor = System.Drawing.SystemColors.Control;
			this.groupBox1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("groupBox1.BackgroundImage")));
			this.groupBox1.Controls.AddRange(new System.Windows.Forms.Control[] {
																					this.txtClientVersion});
			this.groupBox1.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("groupBox1.Dock")));
			this.groupBox1.Enabled = ((bool)(resources.GetObject("groupBox1.Enabled")));
			this.groupBox1.Font = ((System.Drawing.Font)(resources.GetObject("groupBox1.Font")));
			this.groupBox1.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("groupBox1.ImeMode")));
			this.groupBox1.Location = ((System.Drawing.Point)(resources.GetObject("groupBox1.Location")));
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("groupBox1.RightToLeft")));
			this.groupBox1.Size = ((System.Drawing.Size)(resources.GetObject("groupBox1.Size")));
			this.groupBox1.TabIndex = ((int)(resources.GetObject("groupBox1.TabIndex")));
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = resources.GetString("groupBox1.Text");
			this.groupBox1.Visible = ((bool)(resources.GetObject("groupBox1.Visible")));
			// 
			// txtClientVersion
			// 
			this.txtClientVersion.AccessibleDescription = ((string)(resources.GetObject("txtClientVersion.AccessibleDescription")));
			this.txtClientVersion.AccessibleName = ((string)(resources.GetObject("txtClientVersion.AccessibleName")));
			this.txtClientVersion.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("txtClientVersion.Anchor")));
			this.txtClientVersion.AutoSize = ((bool)(resources.GetObject("txtClientVersion.AutoSize")));
			this.txtClientVersion.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("txtClientVersion.BackgroundImage")));
			this.txtClientVersion.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("txtClientVersion.Dock")));
			this.txtClientVersion.Enabled = ((bool)(resources.GetObject("txtClientVersion.Enabled")));
			this.txtClientVersion.Font = ((System.Drawing.Font)(resources.GetObject("txtClientVersion.Font")));
			this.txtClientVersion.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("txtClientVersion.ImeMode")));
			this.txtClientVersion.Location = ((System.Drawing.Point)(resources.GetObject("txtClientVersion.Location")));
			this.txtClientVersion.MaxLength = ((int)(resources.GetObject("txtClientVersion.MaxLength")));
			this.txtClientVersion.Multiline = ((bool)(resources.GetObject("txtClientVersion.Multiline")));
			this.txtClientVersion.Name = "txtClientVersion";
			this.txtClientVersion.PasswordChar = ((char)(resources.GetObject("txtClientVersion.PasswordChar")));
			this.txtClientVersion.ReadOnly = true;
			this.txtClientVersion.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("txtClientVersion.RightToLeft")));
			this.txtClientVersion.ScrollBars = ((System.Windows.Forms.ScrollBars)(resources.GetObject("txtClientVersion.ScrollBars")));
			this.txtClientVersion.Size = ((System.Drawing.Size)(resources.GetObject("txtClientVersion.Size")));
			this.txtClientVersion.TabIndex = ((int)(resources.GetObject("txtClientVersion.TabIndex")));
			this.txtClientVersion.TabStop = false;
			this.txtClientVersion.Text = resources.GetString("txtClientVersion.Text");
			this.txtClientVersion.TextAlign = ((System.Windows.Forms.HorizontalAlignment)(resources.GetObject("txtClientVersion.TextAlign")));
			this.txtClientVersion.Visible = ((bool)(resources.GetObject("txtClientVersion.Visible")));
			this.txtClientVersion.WordWrap = ((bool)(resources.GetObject("txtClientVersion.WordWrap")));
			// 
			// groupBox2
			// 
			this.groupBox2.AccessibleDescription = ((string)(resources.GetObject("groupBox2.AccessibleDescription")));
			this.groupBox2.AccessibleName = ((string)(resources.GetObject("groupBox2.AccessibleName")));
			this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("groupBox2.Anchor")));
			this.groupBox2.BackColor = System.Drawing.SystemColors.Control;
			this.groupBox2.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("groupBox2.BackgroundImage")));
			this.groupBox2.Controls.AddRange(new System.Windows.Forms.Control[] {
																					this.txtUrl});
			this.groupBox2.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("groupBox2.Dock")));
			this.groupBox2.Enabled = ((bool)(resources.GetObject("groupBox2.Enabled")));
			this.groupBox2.Font = ((System.Drawing.Font)(resources.GetObject("groupBox2.Font")));
			this.groupBox2.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("groupBox2.ImeMode")));
			this.groupBox2.Location = ((System.Drawing.Point)(resources.GetObject("groupBox2.Location")));
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("groupBox2.RightToLeft")));
			this.groupBox2.Size = ((System.Drawing.Size)(resources.GetObject("groupBox2.Size")));
			this.groupBox2.TabIndex = ((int)(resources.GetObject("groupBox2.TabIndex")));
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = resources.GetString("groupBox2.Text");
			this.groupBox2.Visible = ((bool)(resources.GetObject("groupBox2.Visible")));
			// 
			// txtUrl
			// 
			this.txtUrl.AccessibleDescription = ((string)(resources.GetObject("txtUrl.AccessibleDescription")));
			this.txtUrl.AccessibleName = ((string)(resources.GetObject("txtUrl.AccessibleName")));
			this.txtUrl.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("txtUrl.Anchor")));
			this.txtUrl.AutoSize = ((bool)(resources.GetObject("txtUrl.AutoSize")));
			this.txtUrl.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("txtUrl.BackgroundImage")));
			this.txtUrl.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("txtUrl.Dock")));
			this.txtUrl.Enabled = ((bool)(resources.GetObject("txtUrl.Enabled")));
			this.txtUrl.Font = ((System.Drawing.Font)(resources.GetObject("txtUrl.Font")));
			this.txtUrl.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("txtUrl.ImeMode")));
			this.txtUrl.Location = ((System.Drawing.Point)(resources.GetObject("txtUrl.Location")));
			this.txtUrl.MaxLength = ((int)(resources.GetObject("txtUrl.MaxLength")));
			this.txtUrl.Multiline = ((bool)(resources.GetObject("txtUrl.Multiline")));
			this.txtUrl.Name = "txtUrl";
			this.txtUrl.PasswordChar = ((char)(resources.GetObject("txtUrl.PasswordChar")));
			this.txtUrl.ReadOnly = true;
			this.txtUrl.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("txtUrl.RightToLeft")));
			this.txtUrl.ScrollBars = ((System.Windows.Forms.ScrollBars)(resources.GetObject("txtUrl.ScrollBars")));
			this.txtUrl.Size = ((System.Drawing.Size)(resources.GetObject("txtUrl.Size")));
			this.txtUrl.TabIndex = ((int)(resources.GetObject("txtUrl.TabIndex")));
			this.txtUrl.TabStop = false;
			this.txtUrl.Text = resources.GetString("txtUrl.Text");
			this.txtUrl.TextAlign = ((System.Windows.Forms.HorizontalAlignment)(resources.GetObject("txtUrl.TextAlign")));
			this.txtUrl.Visible = ((bool)(resources.GetObject("txtUrl.Visible")));
			this.txtUrl.WordWrap = ((bool)(resources.GetObject("txtUrl.WordWrap")));
			// 
			// groupBox3
			// 
			this.groupBox3.AccessibleDescription = ((string)(resources.GetObject("groupBox3.AccessibleDescription")));
			this.groupBox3.AccessibleName = ((string)(resources.GetObject("groupBox3.AccessibleName")));
			this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("groupBox3.Anchor")));
			this.groupBox3.BackColor = System.Drawing.SystemColors.Control;
			this.groupBox3.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("groupBox3.BackgroundImage")));
			this.groupBox3.Controls.AddRange(new System.Windows.Forms.Control[] {
																					this.txtServerVersion});
			this.groupBox3.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("groupBox3.Dock")));
			this.groupBox3.Enabled = ((bool)(resources.GetObject("groupBox3.Enabled")));
			this.groupBox3.Font = ((System.Drawing.Font)(resources.GetObject("groupBox3.Font")));
			this.groupBox3.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("groupBox3.ImeMode")));
			this.groupBox3.Location = ((System.Drawing.Point)(resources.GetObject("groupBox3.Location")));
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("groupBox3.RightToLeft")));
			this.groupBox3.Size = ((System.Drawing.Size)(resources.GetObject("groupBox3.Size")));
			this.groupBox3.TabIndex = ((int)(resources.GetObject("groupBox3.TabIndex")));
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = resources.GetString("groupBox3.Text");
			this.groupBox3.Visible = ((bool)(resources.GetObject("groupBox3.Visible")));
			// 
			// txtServerVersion
			// 
			this.txtServerVersion.AccessibleDescription = ((string)(resources.GetObject("txtServerVersion.AccessibleDescription")));
			this.txtServerVersion.AccessibleName = ((string)(resources.GetObject("txtServerVersion.AccessibleName")));
			this.txtServerVersion.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("txtServerVersion.Anchor")));
			this.txtServerVersion.AutoSize = ((bool)(resources.GetObject("txtServerVersion.AutoSize")));
			this.txtServerVersion.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("txtServerVersion.BackgroundImage")));
			this.txtServerVersion.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("txtServerVersion.Dock")));
			this.txtServerVersion.Enabled = ((bool)(resources.GetObject("txtServerVersion.Enabled")));
			this.txtServerVersion.Font = ((System.Drawing.Font)(resources.GetObject("txtServerVersion.Font")));
			this.txtServerVersion.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("txtServerVersion.ImeMode")));
			this.txtServerVersion.Location = ((System.Drawing.Point)(resources.GetObject("txtServerVersion.Location")));
			this.txtServerVersion.MaxLength = ((int)(resources.GetObject("txtServerVersion.MaxLength")));
			this.txtServerVersion.Multiline = ((bool)(resources.GetObject("txtServerVersion.Multiline")));
			this.txtServerVersion.Name = "txtServerVersion";
			this.txtServerVersion.PasswordChar = ((char)(resources.GetObject("txtServerVersion.PasswordChar")));
			this.txtServerVersion.ReadOnly = true;
			this.txtServerVersion.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("txtServerVersion.RightToLeft")));
			this.txtServerVersion.ScrollBars = ((System.Windows.Forms.ScrollBars)(resources.GetObject("txtServerVersion.ScrollBars")));
			this.txtServerVersion.Size = ((System.Drawing.Size)(resources.GetObject("txtServerVersion.Size")));
			this.txtServerVersion.TabIndex = ((int)(resources.GetObject("txtServerVersion.TabIndex")));
			this.txtServerVersion.TabStop = false;
			this.txtServerVersion.Text = resources.GetString("txtServerVersion.Text");
			this.txtServerVersion.TextAlign = ((System.Windows.Forms.HorizontalAlignment)(resources.GetObject("txtServerVersion.TextAlign")));
			this.txtServerVersion.Visible = ((bool)(resources.GetObject("txtServerVersion.Visible")));
			this.txtServerVersion.WordWrap = ((bool)(resources.GetObject("txtServerVersion.WordWrap")));
			// 
			// groupBox4
			// 
			this.groupBox4.AccessibleDescription = ((string)(resources.GetObject("groupBox4.AccessibleDescription")));
			this.groupBox4.AccessibleName = ((string)(resources.GetObject("groupBox4.AccessibleName")));
			this.groupBox4.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("groupBox4.Anchor")));
			this.groupBox4.BackColor = System.Drawing.SystemColors.Control;
			this.groupBox4.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("groupBox4.BackgroundImage")));
			this.groupBox4.Controls.AddRange(new System.Windows.Forms.Control[] {
																					this.txtDatabase});
			this.groupBox4.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("groupBox4.Dock")));
			this.groupBox4.Enabled = ((bool)(resources.GetObject("groupBox4.Enabled")));
			this.groupBox4.Font = ((System.Drawing.Font)(resources.GetObject("groupBox4.Font")));
			this.groupBox4.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("groupBox4.ImeMode")));
			this.groupBox4.Location = ((System.Drawing.Point)(resources.GetObject("groupBox4.Location")));
			this.groupBox4.Name = "groupBox4";
			this.groupBox4.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("groupBox4.RightToLeft")));
			this.groupBox4.Size = ((System.Drawing.Size)(resources.GetObject("groupBox4.Size")));
			this.groupBox4.TabIndex = ((int)(resources.GetObject("groupBox4.TabIndex")));
			this.groupBox4.TabStop = false;
			this.groupBox4.Text = resources.GetString("groupBox4.Text");
			this.groupBox4.Visible = ((bool)(resources.GetObject("groupBox4.Visible")));
			// 
			// txtDatabase
			// 
			this.txtDatabase.AccessibleDescription = ((string)(resources.GetObject("txtDatabase.AccessibleDescription")));
			this.txtDatabase.AccessibleName = ((string)(resources.GetObject("txtDatabase.AccessibleName")));
			this.txtDatabase.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("txtDatabase.Anchor")));
			this.txtDatabase.AutoSize = ((bool)(resources.GetObject("txtDatabase.AutoSize")));
			this.txtDatabase.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("txtDatabase.BackgroundImage")));
			this.txtDatabase.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("txtDatabase.Dock")));
			this.txtDatabase.Enabled = ((bool)(resources.GetObject("txtDatabase.Enabled")));
			this.txtDatabase.Font = ((System.Drawing.Font)(resources.GetObject("txtDatabase.Font")));
			this.txtDatabase.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("txtDatabase.ImeMode")));
			this.txtDatabase.Location = ((System.Drawing.Point)(resources.GetObject("txtDatabase.Location")));
			this.txtDatabase.MaxLength = ((int)(resources.GetObject("txtDatabase.MaxLength")));
			this.txtDatabase.Multiline = ((bool)(resources.GetObject("txtDatabase.Multiline")));
			this.txtDatabase.Name = "txtDatabase";
			this.txtDatabase.PasswordChar = ((char)(resources.GetObject("txtDatabase.PasswordChar")));
			this.txtDatabase.ReadOnly = true;
			this.txtDatabase.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("txtDatabase.RightToLeft")));
			this.txtDatabase.ScrollBars = ((System.Windows.Forms.ScrollBars)(resources.GetObject("txtDatabase.ScrollBars")));
			this.txtDatabase.Size = ((System.Drawing.Size)(resources.GetObject("txtDatabase.Size")));
			this.txtDatabase.TabIndex = ((int)(resources.GetObject("txtDatabase.TabIndex")));
			this.txtDatabase.TabStop = false;
			this.txtDatabase.Text = resources.GetString("txtDatabase.Text");
			this.txtDatabase.TextAlign = ((System.Windows.Forms.HorizontalAlignment)(resources.GetObject("txtDatabase.TextAlign")));
			this.txtDatabase.Visible = ((bool)(resources.GetObject("txtDatabase.Visible")));
			this.txtDatabase.WordWrap = ((bool)(resources.GetObject("txtDatabase.WordWrap")));
			// 
			// groupBox5
			// 
			this.groupBox5.AccessibleDescription = ((string)(resources.GetObject("groupBox5.AccessibleDescription")));
			this.groupBox5.AccessibleName = ((string)(resources.GetObject("groupBox5.AccessibleName")));
			this.groupBox5.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("groupBox5.Anchor")));
			this.groupBox5.BackColor = System.Drawing.SystemColors.Control;
			this.groupBox5.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("groupBox5.BackgroundImage")));
			this.groupBox5.Controls.AddRange(new System.Windows.Forms.Control[] {
																					this.txtDatabaseVersion});
			this.groupBox5.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("groupBox5.Dock")));
			this.groupBox5.Enabled = ((bool)(resources.GetObject("groupBox5.Enabled")));
			this.groupBox5.Font = ((System.Drawing.Font)(resources.GetObject("groupBox5.Font")));
			this.groupBox5.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("groupBox5.ImeMode")));
			this.groupBox5.Location = ((System.Drawing.Point)(resources.GetObject("groupBox5.Location")));
			this.groupBox5.Name = "groupBox5";
			this.groupBox5.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("groupBox5.RightToLeft")));
			this.groupBox5.Size = ((System.Drawing.Size)(resources.GetObject("groupBox5.Size")));
			this.groupBox5.TabIndex = ((int)(resources.GetObject("groupBox5.TabIndex")));
			this.groupBox5.TabStop = false;
			this.groupBox5.Text = resources.GetString("groupBox5.Text");
			this.groupBox5.Visible = ((bool)(resources.GetObject("groupBox5.Visible")));
			// 
			// txtDatabaseVersion
			// 
			this.txtDatabaseVersion.AccessibleDescription = ((string)(resources.GetObject("txtDatabaseVersion.AccessibleDescription")));
			this.txtDatabaseVersion.AccessibleName = ((string)(resources.GetObject("txtDatabaseVersion.AccessibleName")));
			this.txtDatabaseVersion.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("txtDatabaseVersion.Anchor")));
			this.txtDatabaseVersion.AutoSize = ((bool)(resources.GetObject("txtDatabaseVersion.AutoSize")));
			this.txtDatabaseVersion.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("txtDatabaseVersion.BackgroundImage")));
			this.txtDatabaseVersion.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("txtDatabaseVersion.Dock")));
			this.txtDatabaseVersion.Enabled = ((bool)(resources.GetObject("txtDatabaseVersion.Enabled")));
			this.txtDatabaseVersion.Font = ((System.Drawing.Font)(resources.GetObject("txtDatabaseVersion.Font")));
			this.txtDatabaseVersion.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("txtDatabaseVersion.ImeMode")));
			this.txtDatabaseVersion.Location = ((System.Drawing.Point)(resources.GetObject("txtDatabaseVersion.Location")));
			this.txtDatabaseVersion.MaxLength = ((int)(resources.GetObject("txtDatabaseVersion.MaxLength")));
			this.txtDatabaseVersion.Multiline = ((bool)(resources.GetObject("txtDatabaseVersion.Multiline")));
			this.txtDatabaseVersion.Name = "txtDatabaseVersion";
			this.txtDatabaseVersion.PasswordChar = ((char)(resources.GetObject("txtDatabaseVersion.PasswordChar")));
			this.txtDatabaseVersion.ReadOnly = true;
			this.txtDatabaseVersion.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("txtDatabaseVersion.RightToLeft")));
			this.txtDatabaseVersion.ScrollBars = ((System.Windows.Forms.ScrollBars)(resources.GetObject("txtDatabaseVersion.ScrollBars")));
			this.txtDatabaseVersion.Size = ((System.Drawing.Size)(resources.GetObject("txtDatabaseVersion.Size")));
			this.txtDatabaseVersion.TabIndex = ((int)(resources.GetObject("txtDatabaseVersion.TabIndex")));
			this.txtDatabaseVersion.TabStop = false;
			this.txtDatabaseVersion.Text = resources.GetString("txtDatabaseVersion.Text");
			this.txtDatabaseVersion.TextAlign = ((System.Windows.Forms.HorizontalAlignment)(resources.GetObject("txtDatabaseVersion.TextAlign")));
			this.txtDatabaseVersion.Visible = ((bool)(resources.GetObject("txtDatabaseVersion.Visible")));
			this.txtDatabaseVersion.WordWrap = ((bool)(resources.GetObject("txtDatabaseVersion.WordWrap")));
			// 
			// About
			// 
			this.AccessibleDescription = ((string)(resources.GetObject("$this.AccessibleDescription")));
			this.AccessibleName = ((string)(resources.GetObject("$this.AccessibleName")));
			this.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("$this.Anchor")));
			this.AutoScaleBaseSize = ((System.Drawing.Size)(resources.GetObject("$this.AutoScaleBaseSize")));
			this.AutoScroll = ((bool)(resources.GetObject("$this.AutoScroll")));
			this.AutoScrollMargin = ((System.Drawing.Size)(resources.GetObject("$this.AutoScrollMargin")));
			this.AutoScrollMinSize = ((System.Drawing.Size)(resources.GetObject("$this.AutoScrollMinSize")));
			this.BackColor = System.Drawing.SystemColors.ActiveCaption;
			this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
			this.ClientSize = ((System.Drawing.Size)(resources.GetObject("$this.ClientSize")));
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.groupBox5,
																		  this.groupBox4,
																		  this.groupBox3,
																		  this.groupBox2,
																		  this.groupBox1});
			this.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("$this.Dock")));
			this.Enabled = ((bool)(resources.GetObject("$this.Enabled")));
			this.Font = ((System.Drawing.Font)(resources.GetObject("$this.Font")));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("$this.ImeMode")));
			this.Location = ((System.Drawing.Point)(resources.GetObject("$this.Location")));
			this.MaximizeBox = false;
			this.MaximumSize = ((System.Drawing.Size)(resources.GetObject("$this.MaximumSize")));
			this.MinimizeBox = false;
			this.MinimumSize = ((System.Drawing.Size)(resources.GetObject("$this.MinimumSize")));
			this.Name = "About";
			this.Opacity = 0.85000002384185791;
			this.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("$this.RightToLeft")));
			this.StartPosition = ((System.Windows.Forms.FormStartPosition)(resources.GetObject("$this.StartPosition")));
			this.Text = resources.GetString("$this.Text");
			this.Visible = ((bool)(resources.GetObject("$this.Visible")));
			this.groupBox1.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.groupBox3.ResumeLayout(false);
			this.groupBox4.ResumeLayout(false);
			this.groupBox5.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

	}
}
