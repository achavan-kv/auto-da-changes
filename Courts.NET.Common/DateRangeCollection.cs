using System;
using System.Collections;

namespace STL.Common
{
	/// <summary>
	/// Collection class to hold date ranges
	/// </summary>
	public class DateRangeCollection : CollectionBase
	{
		public void Add(DateRange dr)
		{
			List.Add(dr);
		}

		public void Remove(int index)
		{
			if(index >= 0 && index < Count)
				List.RemoveAt(index);
		}

		public DateRange Item(int index)
		{
			return (DateRange)List[index];
		}

		public DateRangeCollection()
		{
		}
	}
}
