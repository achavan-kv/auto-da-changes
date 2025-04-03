using System;
using System.Collections;

namespace STL.BLL.CreditBureau
{
	/// <summary>
	/// Collection class to hold consumer objects for credit bureau requests
	/// </summary>
	public class ConsumerCollection : CollectionBase
	{
		public void Add(Consumer aConsumer)
		{
			List.Add(aConsumer);
		}

		public void Remove(int index)
		{
			if(index >= 0 && index < Count)
				List.RemoveAt(index);
		}

		public Consumer Item(int index)
		{
			return (Consumer)List[index];
		}

		public ConsumerCollection()
		{
		}
	}
}
