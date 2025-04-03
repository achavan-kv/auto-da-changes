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
	/// User control used where a credit card number field is required.
	/// The characters that can be entered into the field and the format
	/// of the field is controlled to adhere to a suitable format.
	/// </summary>
	public class CreditCardNo : System.Windows.Forms.TextBox
	{
		private System.Windows.Forms.ErrorProvider errorProvider1;
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.ContextMenu context = null;
		private System.Windows.Forms.MenuItem menuCopy = null;
		private System.Windows.Forms.MenuItem menuPaste = null;
		private string pattern = "[0-9]{4}-[0-9]{4}-[0-9]{4}-[0-9]{4}";	//num in 0000-0000-0000-0000 format
		private string pattern2 = "^[0-9]{16}$";	//exactly 16 digit num
		//private CommonForm form = null;

		public string One
		{
			get{return this.Text.Substring(0,4);}
			set
			{
				init();
				string tmp = this.Text.Remove(0,4);
				tmp = tmp.Insert(0, value);
				this.Text = tmp;
			}
		}
		public string Two
		{
			get{return this.Text.Substring(5,4);}
			set
			{
				init();
				string tmp = this.Text.Remove(5,4);
				tmp = tmp.Insert(5, value);
				this.Text = tmp;
			}
		}
		public string Three
		{
			get{return this.Text.Substring(10,4);}
			set
			{
				init();
				string tmp = this.Text.Remove(10,4);
				tmp = tmp.Insert(10, value);
				this.Text = tmp;
			}
		}
		public string Four
		{
			get{return this.Text.Substring(15,4);}
			set
			{
				init();
				string tmp = this.Text.Remove(15,4);
				tmp = tmp.Insert(15, value);
				this.Text = tmp;
			}
		}

		private void init()
		{
			if(this.Text.Length==0)
				this.Text = "0000-0000-0000-0000";
		}

		public CreditCardNo()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();
			//form = new CommonForm();
            this.init();
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
			this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.OnKeyPress);		

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

		private void OnMenuCopy(object sender, System.EventArgs e)
		{
			Clipboard.SetDataObject(this.Text, true);
		}

		private void OnMenuPaste(object sender, System.EventArgs e)
		{
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
                        CommonForm.FormatCreditCardNo(ref text);
						this.Text = text;
					}
				}
			}
		}

		/// <summary>
		/// intercepts key presses and makes sure they are appropriate
		/// for a credit card number;
		/// Also intercepts copy and paste. For pastes the clipboard data
		/// must match a credit card number regular expression before the 
		/// paste is allowed.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnKeyPress(object sender, KeyPressEventArgs e)
		{
			string text = "";
			try
			{
				if(!this.ReadOnly)
				{
					CreditCardNo sd = (CreditCardNo) sender;
					if(e.KeyChar!=22 && e.KeyChar!=3)		//CTRL-V / CTRL-C
					{
						sd.MaskCreditCardNo(e);
					}
					else
					{
						if(e.KeyChar==22)
						{
							//make sure it's credit card no format
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
                                        CommonForm.FormatCreditCardNo(ref text);
										this.Text = text;
									}
								}
							}
							e.Handled = true;
						}
						if(e.KeyChar==3)
						{
							Clipboard.SetDataObject(this.Text, true);
						}
					}
				}
			}
			catch(Exception)		//just to avoid unhandled exceptions
			{
			}
		}

		private void MaskCreditCardNo(KeyPressEventArgs e)
		{
			string tmp;
			int insert = 0;
			
			if(Char.IsDigit(e.KeyChar) || e.KeyChar == 8)
			{
				errorProvider1.SetError(this, "");
				if (e.KeyChar != 8)
				{
					if(this.SelectionStart<=18)
					{
						if(this.SelectionStart == 4 || this.SelectionStart==9 || this.SelectionStart==14)
						{
							insert = this.SelectionStart+1;
						}
						else
						{
							insert = this.SelectionStart;
						}

						tmp = this.Text;
						tmp = tmp.Remove(insert, 1);
						this.Text = tmp.Insert(insert, e.KeyChar.ToString());
						this.SelectionStart = ++insert;
						this.SelectionLength = 0;
					}
					else
					{
						this.SelectNextControl(this, true, true, true, true);
					}
					e.Handled = true;
				}
				else
				{
					if(this.SelectionStart>0)
					{
						if(this.SelectionStart == 5 || this.SelectionStart==10 || this.SelectionStart==15)
						{
							insert = this.SelectionStart-1;
						}
						else
						{
							insert = this.SelectionStart;
						}
						tmp = this.Text;
						this.Text = tmp.Insert(insert, "0");
						this.SelectionStart = insert;
						this.SelectionLength = 0;
					}					
				}
			}
			else
			{
				e.Handled = true;
			}
		}
	}
}


