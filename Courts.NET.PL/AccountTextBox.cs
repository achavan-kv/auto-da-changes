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
	/// Entry field for a customer account number that uses the 000-0000-0000-0
	/// template to format the account number. The template is applied as the
	/// user types into the field and when pasting into the field.
	/// </summary>
	public class AccountTextBox : System.Windows.Forms.TextBox
	{
		private System.Windows.Forms.ErrorProvider errorProvider1;
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.ContextMenu context = null;
		private System.Windows.Forms.MenuItem menuCopy = null;
		private System.Windows.Forms.MenuItem menuPaste = null;
		private string pattern = "[0-9]{3}-[0-9]{4}-[0-9]{4}-[0-9]";	//num in 000-0000-0000-0 format
		private string pattern2 = "^[0-9]{12}$";	//exactly 12 digit num
		//private CommonForm form = null;
		private bool suspend = false;

		public static string UnSet = "000000000000";

		public AccountTextBox()
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

		/// <summary>
		/// Property to strip the account number formatting out of the account no
		/// </summary>
		public string UnformattedText
		{
			get{return Text.Replace("-", "");}
		}

		public short AccountBranchNo
		{
			get
			{
				string br = Text.Substring(0,3);
				return Convert.ToInt16(br);
			}
		}

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
                if (value == null)
                    value = string.Empty;

				if(!suspend)
				{
					value = value.Replace("-","");
                    base.Text = CommonForm.FormatAccountNo(value);
				}
				else
					base.Text = value;
			}
		}

		/// <summary>
		/// Implementation of the virtual Control.ResetText method to 
		/// set the text of the control to "000-0000-0000-0"
		/// </summary>
		public override void ResetText()
		{
			Text = "000000000000";
		}

		private void OnMenuCopy(object sender, System.EventArgs e)
		{
            try
            {
                Clipboard.SetDataObject(this.Text.Replace("-", ""), true, 3, 500);
            }
            catch (System.Runtime.InteropServices.ExternalException)
            {
                MessageBox.Show("Could not copy the account number to the clipboard because it was being used by another application.");
            }
		}

		private void OnMenuPaste(object sender, System.EventArgs e)
		{
			suspend = true;

			var text = "";
			var data = Clipboard.GetDataObject();
            if (data == null)
                return;

			if (data.GetDataPresent(DataFormats.Text))
			{
                var v = data.GetData(DataFormats.Text);
                if (v == null)
                    return;
				text = v.ToString();
				var reg = new Regex(pattern);
				var m = reg.Match(text);
				if(m.Success)
					this.Text = text;
				else
				{
					var reg2 = new Regex(pattern2);
					var m2 = reg2.Match(text);
					if(m2.Success)
					{
						CommonForm.FormatAccountNo(ref text);
						this.Text = text;
					}
				}
			}

			suspend = false;
		}

		/// <summary>
		/// intercepts key presses and makes sure they are appropriate
		/// for an account number. 
		/// Also intercepts copy and paste. For pastes the clipboard data
		/// must match an account number regular expression before the 
		/// paste is allowed.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnKeyPress(object sender, KeyPressEventArgs e)
		{
			suspend = true;

			string text = "";
			try
			{
				if(!this.ReadOnly)
				{
					AccountTextBox sd = (AccountTextBox) sender;
					if(e.KeyChar!=22 && e.KeyChar!=3)		//CTRL-V / CTRL-C
					{
						sd.MaskAccountNo(e);
					}
				}

				/* copy and paste allowed regardless of readOnly property */
                if (e.KeyChar == 22 && !_preventPaste)
				{
					//make sure it's accountNo format
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
					e.Handled = true;
				}
                if (e.KeyChar == 3)
                {
                    try
                    {
                        Clipboard.SetDataObject(this.Text.Replace("-", ""), true, 3, 500);
                    }
                    catch (System.Runtime.InteropServices.ExternalException)
                    {
                        MessageBox.Show("Copy Failed. Another application is using the clipboard.");
                    }
                }
			}
			catch(Exception)		//just to avoid unhandled exceptions
			{
			}
			finally
			{
				suspend = false;
			}
		}

		private void MaskAccountNo(KeyPressEventArgs e)
		{
			string tmp;
			int insert = 0;

			if(this.SelectionLength>0)
			{
				this.SelectionLength = 0;
				this.SelectionStart = 0;
			}

			if(Char.IsDigit(e.KeyChar) || e.KeyChar == 8)
			{
				errorProvider1.SetError(this, "");
				if (e.KeyChar != 8)
				{
					if(this.SelectionStart<=14)
					{
						if(this.SelectionStart == 3 || this.SelectionStart==8 || this.SelectionStart==13)
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
						if(this.SelectionStart == 4 || this.SelectionStart==9 || this.SelectionStart==14)
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

        private bool _preventPaste;
        public bool PreventPaste
        {
            get { return _preventPaste; }
            set
            {
                if (value)
                    this.ContextMenu = null;
                else
                    this.ContextMenu = context;
                _preventPaste = value;
            }
        }
	}
}


