using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Text.RegularExpressions;


namespace STL.PL.StoreCard
{
    public partial class StoreCardTextBox : System.Windows.Forms.TextBox
	{
       // private System.ComponentModel.Container components = null;
        private System.Windows.Forms.ContextMenu context = null;
        private System.Windows.Forms.MenuItem menuCopy = null;
        private System.Windows.Forms.MenuItem menuPaste = null;
        private string pattern = "[0-9]{4}-[0-9]{4}-[0-9]{4}-[0-9]{4}";	//num in 0000-0000-0000-0000 format
        private string pattern2 = "^[0-9]{16}$";	//exactly 16 digit num
        private CommonForm form = null;
        private bool suspend = false;

        public static string UnSet = "000000000000";
   
        public StoreCardTextBox()
        {
            InitializeComponent();
            form = new CommonForm();
        }



        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Property to strip the account number formatting out of the account no
        /// </summary>
        public string UnformattedText
        {
            get { return Text.Replace("-", ""); }
        }

        /// <summary>
        /// Override of the base.Text property to ensure
        /// that the text can only be written in the 
        /// correct format.
        /// </summary>
        public override string Text
        {
            get { return base.Text; }
            set
            {
                if (!suspend)
                {
                    value = value.Replace("-", "");
                    CommonForm.FormatCreditCardNo(ref value);
                     base.Text = value;
                }
                else
                    base.Text = value;
            }
        }

        /// <summary>
        /// Implementation of the virtual Control.ResetText method to 
        /// set the text of the control to "0000-0000-0000-0000"
        /// </summary>
        public override void ResetText()
        {
            Text = "000000000000000";
        }

        private void OnMenuCopy(object sender, System.EventArgs e)
        {
            Clipboard.SetDataObject(this.Text.Replace("-", ""), true);
        }

        private void OnMenuPaste(object sender, System.EventArgs e)
        {
            suspend = true;

            string text = "";
            IDataObject data = Clipboard.GetDataObject();
            if (data.GetDataPresent(DataFormats.Text))
            {
                text = data.GetData(DataFormats.Text).ToString();
                Regex reg = new Regex(pattern);
                Match m = reg.Match(text);
                if (m.Success)
                    this.Text = text;
                else
                {
                    Regex reg2 = new Regex(pattern2);
                    Match m2 = reg2.Match(text);
                    if (m2.Success)
                    {
                        CommonForm.FormatCreditCardNo(ref text);
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
                if (!this.ReadOnly)
                {
                    StoreCardTextBox sd = (StoreCardTextBox)sender;
                    if (e.KeyChar != 22 && e.KeyChar != 3)		//CTRL-V / CTRL-C
                    {
                        sd.MaskCreditCardNo(e);
                        
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
                        if (m.Success)
                            this.Text = text;
                        else
                        {
                            Regex reg2 = new Regex(pattern2);
                            Match m2 = reg2.Match(text);
                            if (m2.Success)
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
                    Clipboard.SetDataObject(this.Text.Replace("-", ""), true);
                }
            }
            catch (Exception)		//just to avoid unhandled exceptions
            {
            }
            finally
            {
                suspend = false;
            }
        }

        private void MaskCreditCardNo(KeyPressEventArgs e)
        {
            string tmp;
            int insert = 0;

            if (this.SelectionLength > 0)
            {
                this.SelectionLength = 0;
                this.SelectionStart = 0;
            }

            if (Char.IsDigit(e.KeyChar) || e.KeyChar == 8)
            {
                errorProvider1.SetError(this, "");
                if (e.KeyChar != 8)
                {
                    if (this.SelectionStart <= 14)
                    {  // 000-0000-0000-0 0000-0000-0000-0000
                        if (this.SelectionStart == 4 || this.SelectionStart == 9 || this.SelectionStart == 14)
                        {
                            insert = this.SelectionStart + 1;
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
                    if (this.SelectionStart > 0)
                    {
                        if (this.SelectionStart == 5 || this.SelectionStart == 10 || this.SelectionStart == 15)
                        {
                            insert = this.SelectionStart - 1;
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
