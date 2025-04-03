using System;
using System.Collections;

namespace STL.BLL
{
	/// <summary>
	/// Collection class to hold address objects for credit bureau requests
	/// </summary>
	public class AddressCollection : CollectionBase
	{
		public void Add(Address aAddress)
		{
			List.Add(aAddress);
		}

		public void Remove(int index)
		{
			if(index >= 0 && index < Count)
				List.RemoveAt(index);
		}

		public Address Item(int index)
		{
			return (Address)List[index];
		}

		public AddressCollection()
		{
		}
	}
}
