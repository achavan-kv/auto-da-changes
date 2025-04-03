using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using STL.Common;
using STL.Common.Constants.TableNames;
using STL.Common.Constants.ColumnNames;
using STL.Common.Static;
using System.Xml;


namespace STL.PL
{
    /// <summary>
    /// CustIdTextBox to mask and format a Customer Id.
    /// Format strings are set up in Code Maintenance.
    /// The user can enter a simple format to define each char position
    /// as follows:
    /// L = letter only
    /// 0 = digit only
    /// A = letter or digit
    /// Any other type of character is interpreted as a literal char.
    /// eg: AAA\00000-LL format would allow DEF\12345-A1
    /// </summary>
    public class CustIdTextBox : System.Windows.Forms.TextBox
    {
        private System.Windows.Forms.ErrorProvider _errorProvider1;
        private System.Windows.Forms.ContextMenu _context = null;
        private System.Windows.Forms.MenuItem _menuCopy = null;
        private System.Windows.Forms.MenuItem _menuPaste = null;
        private CommonForm _form = new CommonForm();
        private DataTable _formatList = null;
        private string _curFormat = "";
        private string _curForRef = "";
        public string curNationality = "";
        private string _strippedCustId = "";
        private bool _suspend = false;
        //private string _error;

        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

        public CustIdTextBox()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

            // Load the list of possible Customer Id formats
            if (StaticData.Tables[TN.CustomerIdFormats] != null)
                this._formatList = ((DataTable)StaticData.Tables[TN.CustomerIdFormats]);

            this.InitFormat();
        }

        public System.Windows.Forms.ErrorProvider errorProvider1
        {
            get { return _errorProvider1; }
            set { _errorProvider1 = value; }
        }

        public void SetError(string errMsg)
        {
            this._errorProvider1.SetError(this, errMsg);
        }

        public bool IsBlank(bool setMandatory)
        {
            // Return true if the field is blank
            if (this._strippedCustId == "" || base.Text == this._curFormat)
            {
                if (setMandatory)
                {
                    this._errorProvider1.SetError(this, CommonForm.GetResource("M_ENTERMANDATORY"));
                }
                return true;
            }
            else return false;
        }

        public bool IsValid(bool setMandatory)
        {
            // Return true if no error flag is displayed
            if (setMandatory && (this._strippedCustId == "" || base.Text == this._curFormat))
            {
                // Set to error if mandatory and blank
                this._errorProvider1.SetError(this, CommonForm.GetResource("M_ENTERMANDATORY"));
            }
            // Could already be in error if the wrong format
            return (this._errorProvider1.GetError(this) == "");
        }

        private string InitFormat(int x)
        {
            InitFormat();
            return this._curFormat;
        }
        private void InitFormat()
        {
            // No format is enforced until the leave event and then only
            // if a format can be matched from the list.
            this._curFormat = "";
            this._curForRef = "";

            // If there is only one format then enforce this straight away
            if (this._formatList != null)
            {
                foreach (DataRow row in this._formatList.Rows)
                {
                    if ((string)row[CN.Reference] == curNationality)
                    {
                        this._curFormat = (string)(row[CN.CodeDescript]);
                        this._curForRef = (string)(row[CN.Reference]);
                    }
                }

            }
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

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(CustIdTextBox));
            this._errorProvider1 = new System.Windows.Forms.ErrorProvider();
            this._context = new System.Windows.Forms.ContextMenu();
            this._menuCopy = new System.Windows.Forms.MenuItem();
            this._menuPaste = new System.Windows.Forms.MenuItem();
            // 
            // _errorProvider1
            // 
            this._errorProvider1.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
            this._errorProvider1.Icon = ((System.Drawing.Icon)(resources.GetObject("_errorProvider1.Icon")));
            // 
            // _context
            // 
            this._context.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					 this._menuCopy,
																					 this._menuPaste});
            // 
            // _menuCopy
            // 
            this._menuCopy.Index = 0;
            this._menuCopy.Text = "Copy";
            this._menuCopy.Click += new System.EventHandler(this.OnMenuCopy);
            // 
            // _menuPaste
            // 
            this._menuPaste.Index = 1;
            this._menuPaste.Text = "Paste";
            this._menuPaste.Click += new System.EventHandler(this.OnMenuPaste);
            // 
            // CustIdTextBox
            // 
            this.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.ContextMenu = this._context;
            this.MaxLength = 20;
            this.Size = new System.Drawing.Size(120, 20);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.CustIdTextBox_KeyDown);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.CustIdTextBox_KeyPress);
            this.Leave += new System.EventHandler(this.CustIdTextBox_Leave);

        }
        #endregion


        public string FormattedText
        {
            get
            {
                // Return the formatted string
                this.FormatCustId(base.Text, this._curFormat, this._curForRef);
                return base.Text.Trim();
            }
        }

        /// <summary>
        /// Override of the base.Text property to ensure
        /// that the text can only be written in the 
        /// correct format.
        /// </summary>
        public override string Text
        {
            // Return the string without any formatting
            get { return this._strippedCustId.Trim(); }
            set
           {
                // A new value will be formatted if necessary
                base.Text = value.Trim();
                this.FormatCustId(base.Text, this._curFormat, this._curForRef);
            }
        }


        private void MaskCustId(KeyPressEventArgs e)
        {
            // Return if no format currently enforced
            if (this._curFormat == "") return;

            string tmp;
            int insert = 0;

            if (this.SelectionLength > 0)
            {
                this.SelectionLength = 0;
                this.SelectionStart = 0;
            }

            // Intercept letters, digits and the delete key
            if (Char.IsLetterOrDigit(e.KeyChar) || e.KeyChar == 8)
            {
                if (e.KeyChar == 8) // BACKSPACE key
                {
                    insert = this.SelectionStart - 1;
                    if (insert >= 0)
                    {
                        if (insert < this._curFormat.Length)
                        {
                            if (this._curFormat.Substring(insert, 1) != "A" &&
                                this._curFormat.Substring(insert, 1) != "L" &&
                                this._curFormat.Substring(insert, 1) != "0")
                            {
                                --insert;
                            }
                        }
                    }
                    if (insert >= 0)
                    {
                        tmp = base.Text.Remove(insert, 1);
                        if (insert < this._curFormat.Length)
                        {
                            this.Text = tmp.Insert(insert, this._curFormat.Substring(insert, 1));
                        }
                        else
                        {
                            this.Text = tmp.Insert(insert, "");
                        }
                        this.SelectionStart = insert;
                        this.SelectionLength = 0;
                    }
                }
                else  // Letter or digit key
                {
                    insert = this.SelectionStart;
                    if (insert < this._curFormat.Length)
                    {
                        while (insert < this._curFormat.Length &&
                            this._curFormat.Substring(insert, 1) != "A" &&
                            this._curFormat.Substring(insert, 1) != "L" &&
                            this._curFormat.Substring(insert, 1) != "0")
                        {
                            ++insert;
                        }
                    }
                    if (insert < this._curFormat.Length)
                    {
                        if (insert + 1 <= base.Text.Length)
                            tmp = base.Text.Remove(insert, 1);
                        else
                            tmp = base.Text;

                        this.Text = tmp.Insert(insert, e.KeyChar.ToString());
                        this.SelectionStart = ++insert;
                        this.SelectionLength = 0;
                    }
                    else
                    {
                        this.SelectNextControl(this, true, true, true, true);
                    }
                }
            }
            e.Handled = true;
        }


        private bool FormatCustId(string custId, string _curFormat, string _curFormRef)
        {
            // Match a CustId to the user format string or reformat if necessary
            // eg: AAA\00000 format would allow DEF\12345
            // and: DEF12345 would be returned as DEF\12345
            // Note: this routine will always return valid when the format is blank.
            // Also saves this._strippedCustId as a copy stripped of literal chars.

            // Return when recursive (due to this.Text Set() method)
            if (this._suspend) return true;

            if (this.curNationality != null && _curFormRef != this.curNationality)
            {
                int x = 0;
                _curFormat = InitFormat(x);
            }
                
                // Leading and trailing spaces must be removed,
                // otherwise the fomatting (and the user) will be confused
                custId = custId.Trim();
            _curFormat = _curFormat.Trim();

            // Return if no format currently enforced
            if (_curFormat == "")
            {
                this._errorProvider1.SetError(this, "");
                this._strippedCustId = custId;
                return true;
            }

            // Return if the CustId is blank
            if (custId == "" || custId == _curFormat)
            {
                this._strippedCustId = "";
                base.Text = "";
                custId = "";
                // The CustId field will show any format being enforced
                if(this.curNationality != null && this._curForRef == this.curNationality)
                    base.Text = _curFormat;
                return true;
            }
            this._suspend = true;

            int i = 0;
            bool valid = true;
            Regex regExp = null;
            Match regMatch = null;
            string literalChar = "";
            string strippedCustId = "";

            // Match to the format or reformat if necessary
            while (i < _curFormat.Length && i < custId.Length && valid)
            {
                switch (_curFormat.Substring(i, 1))
                {
                    case "L":
                        // Match any letter
                        regExp = new Regex("[A-Z]|[a-z]");
                        regMatch = regExp.Match(custId, i, 1);
                        valid = regMatch.Success;
                        strippedCustId += custId.Substring(i, 1);
                        break;
                    case "0":
                        // Match any digit
                        regExp = new Regex("[0-9]");
                        regMatch = regExp.Match(custId, i, 1);
                        valid = regMatch.Success;
                        strippedCustId += custId.Substring(i, 1);
                        break;
                    case "A":
                        // Match any letter or digit
                        regExp = new Regex("[A-Z]|[a-z]|[0-9]");
                        regMatch = regExp.Match(custId, i, 1);
                        valid = regMatch.Success;
                        strippedCustId += custId.Substring(i, 1);
                        break;
                    default:
                        // Match literal character
                        literalChar = _curFormat.Substring(i, 1);
                        if (custId.Substring(i, 1) != literalChar)
                            custId = custId.Insert(i, literalChar);
                        break;
                }
                i++;
            }
            // If a valid CustId is being entered pad out the rest of the format
            //while (i < _curFormat.Length && valid)
            //{
            //	literalChar = _curFormat.Substring(i,1);
            //	custId += literalChar;
            //	i++;
            //}

            // Check the length of the CustId matches the format
            valid = (valid && (custId.Length == _curFormat.Length));
            if (valid)
            {
                // The Cust Id is formatted as expected
                base.Text = custId;
                this._strippedCustId = strippedCustId;
                this._errorProvider1.SetError(this, "");
            }
            else
            {
                // The Cust Id is invalid or a legacy format
                this._strippedCustId = base.Text;
                // If the error text is already set it must be cleared
                // to be able to set it do a different text.
                this._errorProvider1.SetError(this, "");
                this._errorProvider1.SetError(this, CommonForm.GetResource("M_INVALIDCUSTID"));
            }
            this._suspend = false;
            return valid;
        }

        private void OnMenuCopy(object sender, System.EventArgs e)
        {
            Clipboard.SetDataObject(this.Text.Trim(), true);
        }

        private void OnMenuPaste(object sender, System.EventArgs e)
        {
            IDataObject data = Clipboard.GetDataObject();
            if (data.GetDataPresent(DataFormats.Text))
            {
                this.Text = "";
                string tmpText = data.GetData(DataFormats.Text).ToString().Trim();
                // If the pasted text is longer than the current format
                // then init the format.
                if (tmpText.Length > this._curFormat.Length)
                    this.InitFormat();
                // If the format is not blank and the pasted text is still
                // longer than the current format then truncate the pasted text.
                if (this._curFormat != "" && tmpText.Length > this._curFormat.Length)
                    tmpText = tmpText.Substring(0, this._curFormat.Length);
                this.Text = tmpText;
            }
        }

        private void CustIdTextBox_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            // Intercept each key press in case it must be masked
            try
            {
                if (!this.ReadOnly)
                {
                    if (e.KeyChar != 22 && e.KeyChar != 3)
                    {
                        // Not CTRL-V or CTRL-C
                        this.MaskCustId(e);
                    }

                    // Paste CTRL-V
                    if (e.KeyChar == 22)
                    {
                        this.OnMenuPaste(sender, e);
                        e.Handled = true;
                    }
                }

                // Copy CTRL-C allowed regardless of ReadOnly property
                if (e.KeyChar == 3)
                {
                    this.OnMenuCopy(sender, e);
                    e.Handled = true;
                }
            }
            catch (Exception ex)		// Avoid unhandled exceptions
            {
                this._form.ShowError("Exception in CustId KEYPRESS \n\n" + ex);
            }
        }

        private void CustIdTextBox_Leave(object sender, System.EventArgs e)
        {
            // When the users have set up multiple formats then the format
            // is not enforced until the leave event.
            try
            {
                if (!this.ReadOnly)
                {
                    this._strippedCustId = base.Text.Trim();
                    this._errorProvider1.SetError(this, "");
                    if (this._strippedCustId == "")
                    {
                        // A blank field was entered so clear any formatting
                        // that may have been enforced by a previous entry
                        this.InitFormat();
                    }
                    else if (this._curFormat != "")
                    {
                        // Enforce the format that matched last time
                        this.FormatCustId(base.Text, this._curFormat, this._curForRef);
                    }
                    else if (this._formatList != null)
                    {
                        // Each format must now be checked to try to find one that matches
                        foreach (DataRow row in this._formatList.Rows)
                        {
                            if (this.FormatCustId(base.Text, (string)row[CN.CodeDescript], (string)row[CN.Reference]))
                            {
                                // Found a format that matches
                                // so enforce this format from now on
                                this._curFormat = row[CN.CodeDescript].ToString().Trim();
                                this._curForRef = row[CN.Reference].ToString().Trim();
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)		//just to avoid unhandled exceptions
            {
                this._form.ShowError("Exception in CustId LEAVE \n\n" + ex);
            }
        }

        private void CustIdTextBox_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            try
            {
                // Must trap the 'Delete' key (next to the 'End' key)
                if (e.KeyValue == 46) // Delete key
                {
                    // Reset the field and the format
                    // If there is only one format the field will be set to it
                    // otherwise the field and the format will be blanked.
                    this.InitFormat();
                    this.Text = "";
                    this._errorProvider1.SetError(this, "");
                    e.Handled = true;
                }
            }
            catch (Exception ex)		//just to avoid unhandled exceptions
            {
                this._form.ShowError("Exception in CustId KEYDOWN \n\n" + ex);
            }

        }

    }
}
