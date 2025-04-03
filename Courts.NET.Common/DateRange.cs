using System;

namespace STL.Common
{
	/// <summary>
	/// Summary description for DateRange.
	/// </summary>
	public struct DateRange
	{
		public DateTime From;
		public DateTime To;

		public DateRange(DateTime _from, DateTime _to)
		{
			From = _from;
			To = _to;
		}
	}
}
