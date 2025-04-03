using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Reflection;

namespace AdvancedSample
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class Form1 : System.Windows.Forms.Form
	{
		private Microsoft.Samples.AppUpdater.AppUpdater appUpdater1;
		private System.Windows.Forms.Label label1;
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
			this.appUpdater1 = new Microsoft.Samples.AppUpdater.AppUpdater(this.components);
			this.label1 = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.appUpdater1)).BeginInit();
			this.SuspendLayout();
			// 
			// appUpdater1
			// 
			this.appUpdater1.Poller.DownloadOnDetection = false;
			this.appUpdater1.OnCheckForUpdate += new Microsoft.Samples.AppUpdater.AppUpdater.CheckForUpdateEventHandler(this.appUpdater1_OnCheckForUpdate);
			this.appUpdater1.OnUpdateDetected += new Microsoft.Samples.AppUpdater.AppUpdater.UpdateDetectedEventHandler(this.appUpdater1_OnUpdateDetected);
			this.appUpdater1.OnUpdateComplete += new Microsoft.Samples.AppUpdater.AppUpdater.UpdateCompleteEventHandler(this.appUpdater1_OnUpdateComplete);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(48, 96);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(352, 72);
			this.label1.TabIndex = 0;
			this.label1.Text = "This is a one form app that demonstrates some advanced auto-updating capabilities" +
				".";
			// 
			// Form1
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(8, 19);
			this.BackColor = System.Drawing.Color.SlateBlue;
			this.ClientSize = new System.Drawing.Size(440, 326);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.label1});
			this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.Name = "Form1";
			this.Text = "AdvancedSample";
			this.Load += new System.EventHandler(this.Form1_Load);
			((System.ComponentModel.ISupportInitialize)(this.appUpdater1)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion

		//The value that will be returned to appstart
		private static int returnCode=0;

		[STAThread]
		//When using appdomains, make sure your main returns an int.
		static int Main() 
		{
			Application.Run(new Form1());
			return returnCode;
		}

		
		private void Form1_Load(object sender, System.EventArgs e)
		{
		
		}

		//**************************************************************
		// This event is called everytime the appupdater attempts to check for 
		// an update.  You can hook this event and perform your own update check.
		// Return a boolean value indicating whether an update was found.  
		// Notes:  
		//   * This event fires on the poller thread, so you can make network requests 
		//     to check for updates & it will be done asyncronously.
		//   * Because this event has a non void return value, it can only
		//     be handled by one event handler.  It can not be multi-cast.
		//**************************************************************
		private bool appUpdater1_OnCheckForUpdate(object sender, System.EventArgs e)
		{
			bool updateAvailable;
			string updateUrl;

			// Instantiate a web service to check for updates
			localhost.UpdateService updateService = new localhost.UpdateService();

			//Change the Url as appropriate for your web server
			updateService.Url = "http://localhost/AdvancedSample_WebService/UpdateService.asmx";
			
			//Call the web service.  Notice that this is a sync call, but it is still async since we're on
			//a seperate thread.
			updateAvailable = updateService.CheckForUpdate(
				   Microsoft.Samples.AppUpdater.AppUpdater.GetLatestInstalledVersion().ToString(), out updateUrl);

			//Set the UpdateUrl to the location of the current update
			if (updateAvailable == true)
				appUpdater1.UpdateUrl = updateUrl;

			//Indicate whether an update was found or not.
			return updateAvailable;
		}

		//**************************************************************
		// This event if fired whenever a new update is detected.
		// The default behavior is to start the update download immediatly.
		// However, in this case we have disabled that feature by setting
		// the DownloadOnDetection property to false. In this case we'll ask 
		// the user if they want to download the update & then use the ApplyUpdate()
		// method if they wish too.  Note that this event will fire on the main
		// UI thread to allow interaction with the app UI.  
		//**************************************************************
		private void appUpdater1_OnUpdateDetected(object sender, System.EventArgs e)
		{
			string message = "An update was detected.  This is custom UI that could " + 
				"be used to ask the user if they want to download the update.  " +
				"Do you want to download the udpate now?";
		
			if (MessageBox.Show(message,"Update Detected", MessageBoxButtons.YesNo) == DialogResult.Yes)
			{
				appUpdater1.DownloadUpdate();
			}
		}

		//**************************************************************
		// This event if fired whenever a new update is complete.
		// You could do whatever shut down logic your app needs to run
		// here (ex. saving files).  The RestartApp() method will close
		// the current app.
		//**************************************************************		{
		private void appUpdater1_OnUpdateComplete(object sender, Microsoft.Samples.AppUpdater.UpdateCompleteEventArgs e)
		{
			//If the udpate succeeded...
			if ( e.UpdateSucceeded)
			{
				string message = "An update was completed.  This is custom UI that could " + 
					"be used to ask the user if they want to start using the new update immediatly.  " +
					"Do you want to start using the new udpate now?";
		
				if (MessageBox.Show(message,"Update Complete", MessageBoxButtons.YesNo) == DialogResult.Yes)
				{
					//The return code must be set to tell appstart to restart the app
					returnCode = Microsoft.Samples.AppUpdater.AppUpdater.RestartAppReturnValue;
					//The app must shut itself down. Note:  Don't use methods like Environment.Exit that
					//will shut down the entire process, when using appstart in appdomain mode.
					this.Close();
				}
			} 
			//If the update failed....
			else
			{
				string message = "An attempt to udpate this application failed.  This applicaiton will attempt to update itself the next time the app is run.";
				MessageBox.Show(message);
			}
		}
	}
}
