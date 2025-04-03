using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;

namespace SampleApp
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class Form1 : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button button1;
		private Microsoft.Samples.AppUpdater.AppUpdater appUpdater1;
		private System.ComponentModel.IContainer components;

		public Form1()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
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
			this.components = new System.ComponentModel.Container();
			this.button1 = new System.Windows.Forms.Button();
			this.appUpdater1 = new Microsoft.Samples.AppUpdater.AppUpdater(this.components);
			((System.ComponentModel.ISupportInitialize)(this.appUpdater1)).BeginInit();
			this.SuspendLayout();
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(120, 104);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(120, 56);
			this.button1.TabIndex = 0;
			this.button1.Text = "My Button";
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// appUpdater1
			// 
			this.appUpdater1.AutoFileLoad = true;
			this.appUpdater1.ChangeDetectionMode = Microsoft.Samples.AppUpdater.ChangeDetectionModes.ServerManifestCheck;
			this.appUpdater1.ShowDefaultUI = true;
			this.appUpdater1.UpdateUrl = "http://localhost/SampleApp_ServerSetup/UpdateVersion.xml";
			// 
			// Form1
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(8, 19);
			this.BackColor = System.Drawing.Color.LightBlue;
			this.ClientSize = new System.Drawing.Size(360, 278);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.button1});
			this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.Name = "Form1";
			this.Text = "Form1";
			((System.ComponentModel.ISupportInitialize)(this.appUpdater1)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new Form1());
		}

		private void button1_Click(object sender, System.EventArgs e)
		{
			SimpleForm.Form1 F = new SimpleForm.Form1();
			F.Show();
		}
	}
}
