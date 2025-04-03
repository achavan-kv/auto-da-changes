using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using STL.Common.Static;
using STL.Common;


namespace STL.PL
{
	/// <summary>
	/// A user control that appears as a date picker with associated fields
	/// to show the number of years and months since the date entered.
	/// Either the date or the years and months fields can be altered by the
	/// user and the related fields are automatically updated.
	/// This control can be linked to another instance of the same control.
	/// For example the current date in an address can be linked to the 
	/// previous date in an address. When the current date is altered the number
	/// of years and months at the previous address will be adjusted accordingly.
	/// </summary>
   
	public class DatePicker : CommonUserControl
	{
		private bool _eventsEnabled = true;
        
	//	private Color _color;
        //public override Color BackColor
        //{
        //    get{return _color;}
        //    set
        //    {
        //        dtDate.BackColor = value;
        //        noYears.BackColor = value;
        //        noMonths.BackColor = value;
        //    }
        //}	

        //private Control _tabTo;
		/* Not used
		public Control TabTo
		{
			get{return _tabTo;}
			set{_tabTo = value;}
		}
		*/

		[Browsable(false)]
		public static DateTime MinValue
		{
			get
			{
				return DateTimePicker.MinDateTime;
			}
		}

		private DateTime _dateFrom = DateTime.Today;
		
        [Browsable(false)]
        public DateTime DateFrom
		{
			get{return _dateFrom;}
			set
			{
				_dateFrom = value;
				this.dtDate_ValueChanged(null,null);
			}
		}

        [Browsable(false)]
        public DateTime Value
		{
			get{return dtDate.Value;}
			set
			{
				dtDate.Value = value;
				this.CheckLinkedBias();
			}
		}

      
		public decimal Years
		{
			get{return noYears.Value;}
			set
			{
				noYears.Value = value;
				this.CheckLinkedBias();
			}
		}

		public decimal Months
		{
			get{return noMonths.Value;}
			set{noMonths.Value = value;}
		}

		public string Label
		{
			get{return lDate.Text;}
			set{lDate.Text = value;}
		}

		// DSR 3 Dec 2002 - UAT fixes M34 and M35
		// _linkedBias is True when the linked control should be
		// invisible when this control years >= Country.SanctionMinYears
		private bool _linkedBias = false;
		public bool LinkedBias
		{
			get{return _linkedBias;}
			set{_linkedBias = value;}
		}

		private DatePicker _linked = null;
		public DatePicker LinkedDatePicker
		{
			get{return _linked;}
			set{_linked = value;}
		}

		private System.Windows.Forms.ComboBox _linked2 = null;
		
        public System.Windows.Forms.ComboBox LinkedComboBox
		{
			get{return _linked2;}
			set{_linked2 = value;}
		}

		private System.Windows.Forms.Label _linked3 = null;
		public System.Windows.Forms.Label LinkedLabel
		{
			get{return _linked3;}
			set{_linked3 = value;}
		}

		private System.Windows.Forms.NumericUpDown noMonths;
		private System.Windows.Forms.NumericUpDown noYears;
		private System.Windows.Forms.Label lMonths;
		private System.Windows.Forms.Label lYears;
		private System.Windows.Forms.Label lDate;
		private System.Windows.Forms.DateTimePicker dtDate;


        public event EventHandler ValueChanged;
        
       


		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public DatePicker()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();
			
            // DSR 24 Oct 2002 - UAT fixes J7 and J11
			// Ensure abitrary default dates on the control properties
			// are reset to today. (Though unfortunately this has to be
			// be repeated for each instance of this control on each form.)
			
            this._eventsEnabled = false;
			this._dateFrom = DateTime.Today;
            this.dtDate.Value = DateTime.Today;
			this.Months = 0;
			this.Years = 0;
			this._eventsEnabled = true;
            this.dtDate.ValueChanged +=new EventHandler(OnValueChanged);
		}

        //RM 26/08/09 71326 add new event to bubble up value changed on datepicker
        public void OnValueChanged(object sender, EventArgs e)
        {
            if (ValueChanged != null)
            {
                ValueChanged(sender, e);
            }

        }

		public DatePicker(TranslationDummy d)
		{
			InitializeComponent();
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
			this.noMonths = new System.Windows.Forms.NumericUpDown();
			this.noYears = new System.Windows.Forms.NumericUpDown();
			this.lMonths = new System.Windows.Forms.Label();
			this.lYears = new System.Windows.Forms.Label();
			this.lDate = new System.Windows.Forms.Label();
			this.dtDate = new System.Windows.Forms.DateTimePicker();
			((System.ComponentModel.ISupportInitialize)(this.noMonths)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.noYears)).BeginInit();
			this.SuspendLayout();
			// 
			// noMonths
			// 
			this.noMonths.Location = new System.Drawing.Point(208, 24);
			this.noMonths.Maximum = new System.Decimal(new int[] {
																	 11,
																	 0,
																	 0,
																	 0});
			this.noMonths.Name = "noMonths";
			this.noMonths.Size = new System.Drawing.Size(40, 20);
			this.noMonths.TabIndex = 2;
			this.noMonths.Tag = "";
			this.noMonths.ValueChanged += new System.EventHandler(this.noMonths_ValueChanged);
			this.noMonths.Leave += new System.EventHandler(this.noMonths_Leave);
			// 
			// noYears
			// 
			this.noYears.Location = new System.Drawing.Point(160, 24);
			this.noYears.Maximum = new System.Decimal(new int[] {
																	1000,
																	0,
																	0,
																	0});
			this.noYears.Name = "noYears";
			this.noYears.Size = new System.Drawing.Size(40, 20);
			this.noYears.TabIndex = 1;
			this.noYears.Tag = "";
			this.noYears.ValueChanged += new System.EventHandler(this.noYears_ValueChanged);
			this.noYears.Leave += new System.EventHandler(this.noYears_ValueChanged);
			// 
			// lMonths
			// 
			this.lMonths.Location = new System.Drawing.Point(208, 0);
			this.lMonths.Name = "lMonths";
			this.lMonths.Size = new System.Drawing.Size(36, 16);
			this.lMonths.TabIndex = 69;
			this.lMonths.Text = "mnths";
			// 
			// lYears
			// 
			this.lYears.Location = new System.Drawing.Point(160, 0);
			this.lYears.Name = "lYears";
			this.lYears.Size = new System.Drawing.Size(24, 16);
			this.lYears.TabIndex = 68;
			this.lYears.Text = "yrs";
			// 
			// lDate
			// 
			this.lDate.Location = new System.Drawing.Point(8, 0);
			this.lDate.Name = "lDate";
			this.lDate.Size = new System.Drawing.Size(144, 16);
			this.lDate.TabIndex = 67;
			this.lDate.Text = "Date Label:";
			// 
			// dtDate
			// 
			this.dtDate.CustomFormat = "ddd dd MMM yyyy";
			this.dtDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
			this.dtDate.Location = new System.Drawing.Point(8, 24);
			this.dtDate.Name = "dtDate";
			this.dtDate.Size = new System.Drawing.Size(112, 20);
			this.dtDate.TabIndex = 0;
			this.dtDate.Tag = "";
			this.dtDate.Value = new System.DateTime(2002, 5, 8, 14, 5, 24, 830);
			this.dtDate.ValueChanged += new System.EventHandler(this.dtDate_ValueChanged);
			// 
			// DatePicker
			// 
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.noMonths,
																		  this.noYears,
																		  this.lMonths,
																		  this.lYears,
																		  this.lDate,
																		  this.dtDate});
			this.Name = "DatePicker";
			this.Size = new System.Drawing.Size(256, 56);
			((System.ComponentModel.ISupportInitialize)(this.noMonths)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.noYears)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion

		public void Highlite(HighliteBox h)
		{
			HighliteBox hbDate = new HighliteBox();
			HighliteBox hbYear = new HighliteBox();
			HighliteBox hbMonth = new HighliteBox();
			this.Controls.AddRange(new Control[] {hbDate, hbYear, hbMonth});

			hbDate.Color = hbYear.Color = hbMonth.Color = h.Color;
			hbDate.Alpha = hbYear.Alpha = hbMonth.Alpha = h.Alpha;
			hbDate.Border = hbYear.Border = hbMonth.Border = h.Border;

			hbDate.Location = dtDate.Location;
			hbDate.Size = dtDate.Size;

			hbYear.Location = noYears.Location;
			hbYear.Size = noYears.Size;

			hbMonth.Location = noMonths.Location;
			hbMonth.Size = noMonths.Size;
		}

		public void Reset()
		{
			this._eventsEnabled = false;
			if (Value > DateFrom) Value = DateFrom;
			DateDiff newPeriod = new DateDiff(this.Value, this.DateFrom);
			this.Years = newPeriod.DiffYY;
			this.Months = newPeriod.DiffMM;
			this.CheckLinkedBias();
			this._eventsEnabled = true;
		}

		private void dtDate_ValueChanged(object sender, System.EventArgs e)
		{
			if (this._eventsEnabled)
			{
				this._eventsEnabled = false;
				if (this.Value > this.DateFrom) this.Value = this.DateFrom;
				DateDiff newPeriod = new DateDiff(this.Value, this.DateFrom);
				this.Years = newPeriod.DiffYY;
				this.Months = newPeriod.DiffMM;
				this.CheckRange();
				this._eventsEnabled = true;
			}
		}

		private void noYears_ValueChanged(object sender, System.EventArgs e)
		{
			if (this._eventsEnabled)
			{
				this._eventsEnabled = false;
				if (this.Years < 0) this.Years = 0;
				DateDiff newDate = new DateDiff(this.DateFrom, (int)-this.Years, (int)-this.Months);
				this.Value = newDate.Date2;
				this.CheckRange();
				this._eventsEnabled = true;
			}
		}

		private void noMonths_ValueChanged(object sender, System.EventArgs e)
		{
			if (this._eventsEnabled)
			{
				this._eventsEnabled = false;
				if (this.Months < 0) this.Months = 0;
				DateDiff newDate = new DateDiff(this.DateFrom, (int)-this.Years, (int)-this.Months);
				this.Value = newDate.Date2;
				this.CheckRange();
				this._eventsEnabled = true;
			}
		}

		private void noMonths_Leave(object sender, System.EventArgs e)
		{
			this.noMonths_ValueChanged(this, null);
			//if (_tabTo != null)	_tabTo.Focus();
		}

		private void CheckRange()
		{
			if (_linked != null)
			{
				_linked.DateFrom = Value;
				_linked.Reset();
			}
			this.CheckLinkedBias();
		}

        //This was causing an error with sanction stage 1, so show link is now only set at runtime
        
         
		public void CheckLinkedBias()
		{
			if (_linkedBias)
			{
                bool showLink = true;
                if (!this.DesignMode)
                    showLink = !(Years >= (decimal)Country[CountryParameterNames.SanctionMinYears]);

				if (_linked  != null) _linked.Visible  = showLink;
				if (_linked  != null) _linked.Enabled  = showLink;

				if (_linked2 != null) _linked2.Visible = showLink;
				if (_linked2 != null) _linked2.Enabled = showLink;

				if (_linked3 != null) _linked3.Visible = showLink;
				if (_linked3 != null) _linked3.Enabled = showLink;
			}
		}
		
	}

    

}
