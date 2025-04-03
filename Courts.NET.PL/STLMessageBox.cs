using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using STL.Common;

namespace STL.PL
{
	/// <summary>
	/// A generic popup message prompt. The message is passed in by parameter.
	/// The constructor is overloaded to pass optional parameters. These can
	/// specify button options for the prompt, such as 'OK'; 'Cancel', 'Retry',
	/// 'Abort' and 'Review'. Other parameters can specify icons to display and
	/// values to embed in the message text.
	/// </summary>
	public class STLMessageBox : CommonForm
	{
		private string _message;
		private System.Windows.Forms.Label label;
		public string Message
		{
			get{return _message;}
			set
			{
				_message = value;
				label.Text = value;
			}
		}

		private Hashtable _options = null;
		private int _optionsTotal = 0;

		private int _clicked = 0;
		public int Clicked
		{
			get{return _clicked;}
		}

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public STLMessageBox(TranslationDummy t)
		{
			InitializeComponent();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="message">code for the message to display</param>
		public STLMessageBox(string message)
		{
			InitializeComponent();
			Message = GetResource(message);
			_optionsTotal = MessageOption.OK;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="message">code for the message to display</param>
		/// <param name="buttons">integer whose individual bits describe the buttons required on the message box. The MessageOption class contains constants for which buttons correspond to which bits.</param>
		public STLMessageBox(string message, int buttons)
		{
			InitializeComponent();
			Message = GetResource(message);
			_optionsTotal = buttons;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="message">code for the message to display</param>
		/// <param name="buttons">integer whose individual bits describe the buttons required on the message box. The MessageOption class contains constants for which buttons correspond to which bits.</param>
		/// <param name="icon">Icon to display on the message box</param>
		public STLMessageBox(string message, int buttons, Icon icon)
		{
			InitializeComponent();
			Message = GetResource(message);
			Icon = icon;
			_optionsTotal = buttons;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="message">code for the message to display</param>
		/// <param name="parms">array of parameters to format into the message</param>
		public STLMessageBox(string message, object[] parms)
		{
			InitializeComponent();
			Message = GetResource(message, parms);
			_optionsTotal = MessageOption.OK;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="message">code for the message to display</param>
		/// <param name="parms">array of parameters to format into the message</param>
		/// <param name="buttons">integer whose individual bits describe the buttons required on the message box. The MessageOption class contains constants for which buttons correspond to which bits.</param>
		public STLMessageBox(string message, object[] parms, int buttons)
		{
			InitializeComponent();
			Message = GetResource(message, parms);
			_optionsTotal = buttons;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="message">code for the message to display</param>
		/// <param name="parms">array of parameters to format into the message</param>
		/// <param name="buttons">integer whose individual bits describe the buttons required on the message box. The MessageOption class contains constants for which buttons correspond to which bits.</param>
		/// <param name="icon">Icon to display on the message box</param>
		public STLMessageBox(string message, object[] parms, int buttons, Icon icon)
		{
			InitializeComponent();
			Message = GetResource(message, parms);
			Icon = icon;
			_optionsTotal = buttons;
		}

		private void CreateButtons()
		{
			int left = 25;

			foreach(DictionaryEntry d in _options)
			{
				if((_optionsTotal & (int)d.Key) > 0)
				{
					Button b = new Button();
					b.Text = (string)d.Value;
					b.Tag = (int)d.Key;
					b.Location = new Point(left, 100);
					left += b.Width + 10;
					b.Click += new System.EventHandler(OnButtonClick);
					Controls.Add(b);
				}
			}	
	
			this.Width = left + 25;
			label.Width = this.Width - 50;
		}

		private void LoadHashtable()
		{
			_options = new Hashtable();
			
			_options[MessageOption.OK] = MessageOptionText.OK;
			_options[MessageOption.Cancel] = MessageOptionText.Cancel;
			_options[MessageOption.Abort] = MessageOptionText.Abort;
			_options[MessageOption.Retry] = MessageOptionText.Retry;
			_options[MessageOption.Review] = MessageOptionText.Review;
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
			this.label = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// label
			// 
			this.label.Location = new System.Drawing.Point(25, 16);
			this.label.Name = "label";
			this.label.Size = new System.Drawing.Size(295, 64);
			this.label.TabIndex = 0;
			// 
			// STLMessageBox
			// 
            this.AutoScaleMode = AutoScaleMode.None;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(344, 141);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.label});
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "STLMessageBox";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Information";
			this.Load += new System.EventHandler(this.STLMessageBox_Load);
			this.ResumeLayout(false);

		}
		#endregion

		private void STLMessageBox_Load(object sender, System.EventArgs e)
		{
			try
			{
				Wait();

				LoadHashtable();
				CreateButtons();

			}
			catch(Exception ex)
			{
				Catch(ex, "CreateButtons");
			}
			finally
			{
				StopWait();
			}
		}

		private void OnButtonClick(object sender, System.EventArgs e)
		{
			try
			{
				_clicked = (int)((Button)sender).Tag;
				Close();
			
			}
			catch(Exception ex)
			{
				Catch(ex, "OnButtonClick");
			}
		}
	}

	public class MessageOption
	{
		public static int OK = 1;
		public static int Cancel = 2;
		public static int Retry = 4;
		public static int Abort = 8;
		public static int Review = 16;
	}

	public class MessageOptionText 
	{
		public static string OK = "OK";
		public static string Cancel = "Cancel";
		public static string Retry = "Retry";
		public static string Abort = "Abort";
		public static string Review = "Review";
	}
}
