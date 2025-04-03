using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace STL.PL
{
	/// <summary>
	/// Summary description for AccountTextBox.
	/// </summary>
	public class CustomerID : System.Windows.Forms.TextBox
	{
		private System.Windows.Forms.ErrorProvider errorProvider1;
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.ContextMenu context = null;
		private System.Windows.Forms.MenuItem menuCopy = null;
		private System.Windows.Forms.MenuItem menuPaste = null;
		private string pattern = "[0-9]{3}-[0-9]{4}-[0-9]{4}-[0-9]";	//num in 000-0000-0000-0 format
		private string pattern2 = "^[0-9]{12}$";	//exactly 12 digit num
		//private CommonForm form = null;
		//private bool suspend = false;

		private string _pattern = "[0-9]{3}-[0-9]{4}-[0-9]{4}-[0-9]";
		public string Pattern 
		{
			get { return _pattern; }
			set { _pattern = value; }
		}

		public CustomerID()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();
			//form = new CommonForm();
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

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			// 
			// AccountTextBox
			// 
			this.errorProvider1 = new System.Windows.Forms.ErrorProvider();
			this.context = new ContextMenu();
			this.errorProvider1.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
			this.Validating += new System.ComponentModel.CancelEventHandler(this.OnValidate);

			this.menuCopy = new MenuItem();
			this.menuPaste = new MenuItem();
			this.context.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {this.menuCopy,
																					this.menuPaste});
			this.menuCopy.Index = 0;
			this.menuCopy.Text = "Copy";
			this.menuCopy.Click += new System.EventHandler(this.OnMenuCopy);

			this.menuPaste.Index = 1;
			this.menuPaste.Text = "Paste";
			this.menuPaste.Click += new System.EventHandler(this.OnMenuPaste);

			this.ContextMenu = context;

		}
		#endregion

	
		/// <summary>
		/// Override of the base.Text property to ensure
		/// that the text can only be written in the 
		/// correct format.
		/// </summary>
		public override string Text
		{
			get{return base.Text;}
			set
			{
				base.Text = value;
				this.OnValidate(this, null);
			}
		}

		public override void ResetText()
		{
			Text = "";
		}

		private void OnMenuCopy(object sender, System.EventArgs e)
		{
			Clipboard.SetDataObject(this.Text.Replace("-",""), true);
		}

		private void OnMenuPaste(object sender, System.EventArgs e)
		{
			//suspend = true;

			string text = "";
			IDataObject data = Clipboard.GetDataObject();
			if (data.GetDataPresent(DataFormats.Text))
			{
				text = data.GetData(DataFormats.Text).ToString();
				Regex reg = new Regex(pattern);
				Match m = reg.Match(text);
				if(m.Success)
					this.Text = text;
				else
				{
					Regex reg2 = new Regex(pattern2);
					Match m2 = reg2.Match(text);
					if(m2.Success)
					{
                        CommonForm.FormatAccountNo(ref text);
						this.Text = text;
					}
				}
			}

			//suspend = false;
		}

		private string _oldtext = "";

		private void OnValidate(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if(Text!=_oldtext)		/* make sure text has changed */
			{
				/* if the new text does not match the regular expression
				 * required, then reinstate the old text */
				Regex reg = new Regex(_pattern);
				Match m = reg.Match(Text);
				if(!m.Success)
					this.Text = _oldtext;
				else
					_oldtext = Text;
			}			
		}
	}
}


