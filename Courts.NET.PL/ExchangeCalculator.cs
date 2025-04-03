using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Data;
using System.Xml;
using System.Globalization;
using STL.Common;
using STL.Common.Static;
using STL.Common.Constants;
using STL.Common.Constants.Categories;
using STL.Common.Constants.TableNames;
using STL.Common.Constants.ColumnNames;
using STL.Common.Constants.ExchangeRate;


namespace STL.PL
{
	/// <summary>
	/// Popup calculator to convert between local currency and foreign currencies.
	/// The foreign currency is selected from a list of payment methods that
	/// is limited to show the foreign currency payment methods only.
	/// The exchange rate for the selected currency is displayed as the amount of
	/// local currency that equals one unit of foreign currency.
	/// The user may enter either the loacal or foreign amount, and the 
	/// corresponding amount is automatically calculated. Upon clicking 'OK'
	/// the local amount is returned to the calling screen.
	/// </summary>
	public class ExchangeCalculator : STL.PL.CommonForm
	{
		//
		// Local properties
		//
		private string _error = "";
		private int _currency = 0;		    // Copy of input param
		private bool _convert = false;		// Result param
		private string _payMethod = "";		// Result param
		private decimal _newAmount = 0.0M;	// Result param
		// Change event control
		private bool _userChanged = false;
		private decimal _lastLocalAmount = 0.0M;
		private decimal _lastForeignAmount = 0.0M;

		public bool convert
		{
			get{return _convert;}
			set{_convert = value;}
		}

		public string payMethod
		{
			get{return _payMethod;}
			set{_payMethod = value;}
		}

		public decimal newAmount
		{
			get{return _newAmount;}
			set{_newAmount = value;}
		}

		//
		// Form Properties
		//
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.GroupBox gbForeign;
		private System.Windows.Forms.ComboBox drpCurrency;
		private System.Windows.Forms.GroupBox gbLocal;
		private System.Windows.Forms.Label lPayMethod;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Label lLocalCurrency;
		private System.Windows.Forms.Label lAmount;
		private System.Windows.Forms.TextBox txtRate;
		private System.Windows.Forms.Label lRate;
		private System.Windows.Forms.TextBox txtLocalAmount;
		private System.Windows.Forms.TextBox txtForeignAmount;
		private System.ComponentModel.IContainer components = null;

		//
		// Form Constructors
		//
		public ExchangeCalculator(Form root, Form parent, int piCurrency, decimal piLocalAmount)
		{
			// This call is required by the Windows Form Designer.
			InitializeComponent();

			// Set up
			FormRoot = root;
			FormParent = parent;

			this.txtLocalAmount.Text = piLocalAmount.ToString(DecimalPlaces);
			this._lastLocalAmount = piLocalAmount;
			this._currency = piCurrency;
			this.lLocalCurrency.Text = NumberFormatInfo.CurrentInfo.CurrencySymbol;
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

		#region Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(ExchangeCalculator));
			this.btnCancel = new System.Windows.Forms.Button();
			this.btnOK = new System.Windows.Forms.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.gbForeign = new System.Windows.Forms.GroupBox();
			this.lRate = new System.Windows.Forms.Label();
			this.txtRate = new System.Windows.Forms.TextBox();
			this.txtForeignAmount = new System.Windows.Forms.TextBox();
			this.drpCurrency = new System.Windows.Forms.ComboBox();
			this.gbLocal = new System.Windows.Forms.GroupBox();
			this.lLocalCurrency = new System.Windows.Forms.Label();
			this.txtLocalAmount = new System.Windows.Forms.TextBox();
			this.lAmount = new System.Windows.Forms.Label();
			this.lPayMethod = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.groupBox1.SuspendLayout();
			this.gbForeign.SuspendLayout();
			this.gbLocal.SuspendLayout();
			this.SuspendLayout();
			// 
			// btnCancel
			// 
			this.btnCancel.Location = new System.Drawing.Point(344, 128);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.TabIndex = 50;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// btnOK
			// 
			this.btnOK.Location = new System.Drawing.Point(176, 128);
			this.btnOK.Name = "btnOK";
			this.btnOK.TabIndex = 40;
			this.btnOK.Text = "OK";
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.AddRange(new System.Windows.Forms.Control[] {
																					this.gbForeign,
																					this.gbLocal,
																					this.lAmount,
																					this.lPayMethod,
																					this.label1});
			this.groupBox1.Location = new System.Drawing.Point(8, 8);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(584, 112);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			// 
			// gbForeign
			// 
			this.gbForeign.Controls.AddRange(new System.Windows.Forms.Control[] {
																					this.lRate,
																					this.txtRate,
																					this.txtForeignAmount,
																					this.drpCurrency});
			this.gbForeign.Location = new System.Drawing.Point(112, 16);
			this.gbForeign.Name = "gbForeign";
			this.gbForeign.Size = new System.Drawing.Size(304, 88);
			this.gbForeign.TabIndex = 0;
			this.gbForeign.TabStop = false;
			this.gbForeign.Text = "Foreign Currency";
			// 
			// lRate
			// 
			this.lRate.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lRate.Location = new System.Drawing.Point(144, 24);
			this.lRate.Name = "lRate";
			this.lRate.Size = new System.Drawing.Size(40, 16);
			this.lRate.TabIndex = 0;
			this.lRate.Text = "Rate";
			this.lRate.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// txtRate
			// 
			this.txtRate.BackColor = System.Drawing.SystemColors.Window;
			this.txtRate.Location = new System.Drawing.Point(192, 24);
			this.txtRate.MaxLength = 10;
			this.txtRate.Name = "txtRate";
			this.txtRate.ReadOnly = true;
			this.txtRate.Size = new System.Drawing.Size(104, 20);
			this.txtRate.TabIndex = 0;
			this.txtRate.TabStop = false;
			this.txtRate.Text = "";
			// 
			// txtForeignAmount
			// 
			this.txtForeignAmount.BackColor = System.Drawing.SystemColors.Window;
			this.txtForeignAmount.Location = new System.Drawing.Point(8, 56);
			this.txtForeignAmount.MaxLength = 10;
			this.txtForeignAmount.Name = "txtForeignAmount";
			this.txtForeignAmount.Size = new System.Drawing.Size(104, 20);
			this.txtForeignAmount.TabIndex = 20;
			this.txtForeignAmount.Text = "";
			this.txtForeignAmount.Leave += new System.EventHandler(this.txtForeignAmount_Leave);
			// 
			// drpCurrency
			// 
			this.drpCurrency.BackColor = System.Drawing.SystemColors.Window;
			this.drpCurrency.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.drpCurrency.Location = new System.Drawing.Point(8, 24);
			this.drpCurrency.Name = "drpCurrency";
			this.drpCurrency.Size = new System.Drawing.Size(136, 21);
			this.drpCurrency.TabIndex = 10;
			this.drpCurrency.SelectedIndexChanged += new System.EventHandler(this.drpCurrency_SelectedIndexChanged);
			// 
			// gbLocal
			// 
			this.gbLocal.Controls.AddRange(new System.Windows.Forms.Control[] {
																				  this.lLocalCurrency,
																				  this.txtLocalAmount});
			this.gbLocal.Location = new System.Drawing.Point(456, 16);
			this.gbLocal.Name = "gbLocal";
			this.gbLocal.Size = new System.Drawing.Size(120, 88);
			this.gbLocal.TabIndex = 0;
			this.gbLocal.TabStop = false;
			this.gbLocal.Text = "Local Currency";
			// 
			// lLocalCurrency
			// 
			this.lLocalCurrency.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lLocalCurrency.Location = new System.Drawing.Point(8, 24);
			this.lLocalCurrency.Name = "lLocalCurrency";
			this.lLocalCurrency.Size = new System.Drawing.Size(104, 16);
			this.lLocalCurrency.TabIndex = 0;
			this.lLocalCurrency.Text = "{currency symbol}";
			this.lLocalCurrency.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// txtLocalAmount
			// 
			this.txtLocalAmount.BackColor = System.Drawing.SystemColors.Window;
			this.txtLocalAmount.Location = new System.Drawing.Point(8, 56);
			this.txtLocalAmount.MaxLength = 10;
			this.txtLocalAmount.Name = "txtLocalAmount";
			this.txtLocalAmount.Size = new System.Drawing.Size(104, 20);
			this.txtLocalAmount.TabIndex = 30;
			this.txtLocalAmount.Text = "";
			this.txtLocalAmount.Leave += new System.EventHandler(this.txtLocalAmount_Leave);
			// 
			// lAmount
			// 
			this.lAmount.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lAmount.Location = new System.Drawing.Point(8, 72);
			this.lAmount.Name = "lAmount";
			this.lAmount.Size = new System.Drawing.Size(96, 16);
			this.lAmount.TabIndex = 0;
			this.lAmount.Text = "Amount Tendered";
			this.lAmount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// lPayMethod
			// 
			this.lPayMethod.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lPayMethod.Location = new System.Drawing.Point(32, 40);
			this.lPayMethod.Name = "lPayMethod";
			this.lPayMethod.Size = new System.Drawing.Size(72, 16);
			this.lPayMethod.TabIndex = 0;
			this.lPayMethod.Text = "Pay Method";
			this.lPayMethod.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label1
			// 
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.label1.Location = new System.Drawing.Point(416, 48);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(32, 16);
			this.label1.TabIndex = 0;
			this.label1.Text = " = ";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// ExchangeCalculator
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(600, 163);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.groupBox1,
																		  this.btnCancel,
																		  this.btnOK});
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ExchangeCalculator";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Exchange Calculator";
			this.Load += new System.EventHandler(this.ExchangeCalculator_Load);
			this.groupBox1.ResumeLayout(false);
			this.gbForeign.ResumeLayout(false);
			this.gbLocal.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion


		//
		// Local Procedures
		//
		private bool ValidMoneyField(TextBox moneyField, out decimal moneyValue)
		{
			// Validate a money field for the Country format
			return ValidMoneyField(moneyField, out moneyValue, DecimalPlaces, true);
		}  // End of ValidMoneyField

		private bool ValidMoneyField(TextBox moneyField, out decimal moneyValue, string decimalPlaces, bool showCurrency)
		{
			// Validate a money field for a custom format (for Exchange Rates)
			// Check a blank or zero money value entered
			moneyValue = 0.0M;
			moneyField.Text = moneyField.Text.Trim();
			if (!IsStrictMoney(moneyField.Text))
			{
				ShowInfo("M_NUMERIC");
				// Trap the focus in this field
				moneyField.Focus();
				return false;
			}

			// Reformat
			moneyValue = MoneyStrToDecimal(moneyField.Text, decimalPlaces);
			if (showCurrency)
				moneyField.Text = moneyValue.ToString(decimalPlaces);
			else
				// Do not use country currency symbol (but do use decimal places)
				moneyField.Text = StripCurrency(moneyValue.ToString(decimalPlaces));

			return true;
		}  // End of ValidMoneyField

		private void CalculateFields (bool ToForeign)
		{
			decimal curLocalAmount = 0.0M;
			decimal curForeignAmount = 0.0M;
			decimal curExchangeRate = MoneyStrToDecimal(this.txtRate.Text, RateFormat.DecimalPlaces);

			if (ToForeign)
			{
				curLocalAmount = MoneyStrToDecimal(this.txtLocalAmount.Text, DecimalPlaces);
				if (curExchangeRate != 0)
					curForeignAmount = curLocalAmount / curExchangeRate;
				else
					curForeignAmount = 0;

				 // Do not use country currency symbol for Foreign Currency
				this.txtForeignAmount.Text = StripCurrency(curForeignAmount.ToString(DecimalPlaces));
				this._lastForeignAmount = curForeignAmount;
			}
			else
			{
				curForeignAmount = MoneyStrToDecimal(this.txtForeignAmount.Text, DecimalPlaces);
				curLocalAmount = curForeignAmount * curExchangeRate;
				this.txtLocalAmount.Text = curLocalAmount.ToString(DecimalPlaces);
				this._lastLocalAmount = curLocalAmount;
			}

		}

		//
		// Form Events
		//
		private void ExchangeCalculator_Load(object sender, System.EventArgs e)
		{
			// Initial Form Load
			try
			{
				Function = "Exchange Calculator Screen: Form Load";
				Wait();

				// Load the current Exchange Rates
				DataSet ExchangeRateSet = PaymentManager.GetExchangeRates(out _error);
				DataTable ExchangeRateTable = ExchangeRateSet.Tables[TN.ExchangeRates];

				if (_error.Length > 0)
				{
					ShowError(_error);
					Close();
				}

#region Get drop down data
				//Get the required static data for the drop down lists
			
				XmlUtilities xml = new XmlUtilities();
				XmlDocument dropDowns = new XmlDocument();
				dropDowns.LoadXml("<DROP_DOWNS></DROP_DOWNS>");

				if (StaticData.Tables[TN.PayMethod] == null)
					dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.PayMethod, new string[]{CAT.FintransPayMethod, "L"}));

				if (dropDowns.DocumentElement.ChildNodes.Count > 0)
				{
					DataSet ds = StaticDataManager.GetDropDownData(dropDowns.DocumentElement, out _error);
					if (_error.Length > 0)
						ShowError(_error);
					else
					{
						foreach (DataTable dt in ds.Tables)
						{
							StaticData.Tables[dt.TableName] = dt;
						}
					}
				}
#endregion

				// Take a copy of the PayMethod table and delete everthing
				// except foreign currencies (should also delete blank and 'Not Applicable')
				// The rows remaining will be populated with their exchange rate.
				DataTable dtPayMethod = ((DataTable)StaticData.Tables[TN.PayMethod]).Copy();
				dtPayMethod.Columns.Add(CN.ExchangeRate, Type.GetType("System.Decimal"));

				int selectedIndex = 0;
				for (int i = dtPayMethod.Rows.Count -1; i >= 0; i--)
				{

					// Populate the exchange rate
					int j = 0;
					bool found = false;
					while (j < ExchangeRateTable.Rows.Count && !found)
					{
						if (System.Convert.ToInt16(ExchangeRateTable.Rows[j][CN.Code]) == System.Convert.ToInt16(dtPayMethod.Rows[i][CN.Code]))
						{
							found = true;
							dtPayMethod.Rows[i][CN.ExchangeRate] = System.Convert.ToDecimal(ExchangeRateTable.Rows[j][CN.ExchangeRate]);
						}
						j++;
					}

					if (!found)
					{
						// No exchange rate was found so delete
						dtPayMethod.Rows.RemoveAt(i);
						if (selectedIndex > i) selectedIndex--;
					}
					else if (System.Convert.ToInt16(dtPayMethod.Rows[i][CN.Code]) == this._currency)
					{
						// Match the selected index from the calling form
						selectedIndex = i;
					}
				}

				if (dtPayMethod.Rows.Count == 0)
				{
					ShowInfo("M_NOEXCHANGERATE");
					this.Close();
				}
				else
				{
					drpCurrency.DataSource = dtPayMethod;
					drpCurrency.ValueMember = CN.Code;
					drpCurrency.DisplayMember = CN.CodeDescription;
					drpCurrency.SelectedIndex = selectedIndex;

					decimal curExchangeRate = System.Convert.ToDecimal(((DataRowView)drpCurrency.SelectedItem)[CN.ExchangeRate]);
					this.txtRate.Text = curExchangeRate.ToString(RateFormat.DecimalPlaces);
					this.CalculateFields(true);
					this.txtForeignAmount.Focus();
					this._userChanged = true;
				}
			}
			catch(Exception ex)
			{
				Catch(ex, Function);
			}
			finally
			{
				StopWait();
			}

		}

		private void txtLocalAmount_Leave(object sender, System.EventArgs e)
		{
			try
			{
				Function = "Exchange Calculator Screen: Validate Local Amount";
				decimal curLocalAmount = 0.0M;

				// Check a numeric Local Amount has been entered
				if (!this._userChanged ||
					!this.ValidMoneyField(this.txtLocalAmount, out curLocalAmount)) return;

				// Check the value has been changed
				if (curLocalAmount == this._lastLocalAmount) return;
				this._lastLocalAmount = curLocalAmount;

				this.CalculateFields(true);
			}
			catch(Exception ex)
			{
				Catch(ex, Function);
			}
			finally
			{
				StopWait();
			}
		
		}  // End of txtLocalAmount_Leave

		private void txtForeignAmount_Leave(object sender, System.EventArgs e)
		{
			try
			{
				Function = "Exchange Calculator Screen: Validate Foreign Amount";
				decimal curForeignAmount = 0.0M;

				// Check a numeric Local Amount has been entered
				if (!this._userChanged ||
					!this.ValidMoneyField(this.txtForeignAmount, out curForeignAmount, DecimalPlaces, false)) return;

				// Check the value has been changed
				if (curForeignAmount == this._lastForeignAmount) return;
				this._lastForeignAmount = curForeignAmount;

				this.CalculateFields(false);
			}
			catch(Exception ex)
			{
				Catch(ex, Function);
			}
			finally
			{
				StopWait();
			}
		
		}  // End of txtForeignAmount_Leave

		private void drpCurrency_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			try
			{
				Function = "Exchange Calculator Screen: Currency selected";

				if (!this._userChanged) return;

				decimal curExchangeRate = System.Convert.ToDecimal(((DataRowView)drpCurrency.SelectedItem)[CN.ExchangeRate]);
				this.txtRate.Text = curExchangeRate.ToString(RateFormat.DecimalPlaces);
				this.CalculateFields(false);
			}
			catch(Exception ex)
			{
				Catch(ex, Function);
			}
			finally
			{
				StopWait();
			}

		}  // End of drpCurrency_SelectedIndexChanged

		private void btnOK_Click(object sender, System.EventArgs e)
		{
			this._convert = true;
			this._payMethod = this.drpCurrency.SelectedValue.ToString();
			this._newAmount = MoneyStrToDecimal(this.txtLocalAmount.Text, DecimalPlaces);
			Close();
		}  // End of btnOK_Click

		private void btnCancel_Click(object sender, System.EventArgs e)
		{
			this._convert = false;
			this._newAmount = 0.0M;
			this._payMethod = "";
			Close();
		}  // End of btnCancel_Click

	}
}

