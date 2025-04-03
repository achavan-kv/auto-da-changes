using System;
using STL.Common.Static;

namespace STL.Common
{
	/// <summary>
	/// Class: DateDiff (DSR 22 Oct 2002)
	/// Extension of the DateTime class to provide the difference in
	/// Years and Months between this date and another date.
	/// (Note DateTime is sealed so inheritance cannot be used.)
	/// This class is used when saving a Proposal.
	/// TODO: Update DatePicker to use this class instead of repeating
	/// the YYMM calculation.
	/// </summary>
	public class DateDiff
	{
		private DateTime _date1;
		private DateTime _date2;
		private int _diffYY;
		private int _diffMM;

		//
		// Public ptoperties
		//
		public DateTime Date1
		{
			get
			{
				return _date1;
			}
		}

		public DateTime Date2
		{
			get
			{
				return _date2;
			}
		}

		public int DiffYY
		{
			get
			{
				return _diffYY;
			}
		}

		public int DiffMM
		{
			get
			{
				return _diffMM;
			}
		}

		//
		// Constructors
		//
		public DateDiff()
		{
			_date1 = DateTime.MinValue;
			_date2 = DateTime.MinValue;
			_diffYY = 0;
			_diffMM = 0;
		}

		public DateDiff(DateTime Date1, DateTime Date2)
		{
			CalcDiffYYMM(Date1, Date2);
		}

		public DateDiff(DateTime Date1, int diffYY, int diffMM)
		{
			DateRange(Date1, diffYY, diffMM);
		}

		//
		// Methods
		//

		// Calculate date difference in Years and Months
		public void CalcDiffYYMM(DateTime Date1, DateTime Date2)
		{
			_date1 = Date1;
			_date2 = Date2;

			if (_date1 == DateTime.MinValue || _date2 == DateTime.MinValue ||
                _date1 <= Date.blankDate || _date2 <= Date.blankDate)
			{
				_diffYY = 0;
				_diffMM = 0;
				return;
			}

			if (_date2 > _date1)
			{
				_diffYY = _date2.Year  - _date1.Year;
				_diffMM = _date2.Month - _date1.Month;
			}
			else
			{
				_diffYY = _date1.Year  - _date2.Year;
				_diffMM = _date1.Month - _date2.Month;
			}

			if(_diffMM < 0)
			{
				_diffYY--;
				_diffMM += 12;
			}
		}


		public void DateRange(DateTime Date1, int diffYY, int diffMM)
		{
			// Add the Years and Months to a date to get the date
			// at the other end of this date range. Note that the
			// date passed in can be the end date and the YY, MM 
			// parameters can be negative.
			_diffYY = diffYY >= 0 ? diffYY : -diffYY;
			_diffMM = diffMM >= 0 ? diffMM : -diffMM;

			_date2 = Date1.AddYears(diffYY);
			_date2 = _date2.AddMonths(diffMM);
		}

	}
}
