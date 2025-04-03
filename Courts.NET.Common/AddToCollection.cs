using System;
using System.Collections;

namespace STL.Common
{
	public class AddToAccount
	{
		public AddToAccount(string accountNo,
			string accountType,
			decimal agreementTotal,
			decimal outstandingBalance,
			decimal cashPrice)
		{
			AccountNo = accountNo;
			AccountType = accountType;
			AgreementTotal = agreementTotal;
			OutstandingBalance = outstandingBalance;
			CashPrice = cashPrice;
		}

		public string AccountNo = "";
		public string AccountType = "";
		public decimal AgreementTotal = 0M;
		public decimal OutstandingBalance = 0M;
		public decimal CashPrice = 0M;
	}

	public class AddToCollection : CollectionBase
	{
		public AddToCollection()
		{
		}
		public void Add(AddToAccount addTo)
		{
			List.Add(addTo);
		}
		public void Remove(int index)
		{
			if (index <= Count - 1 && index < 0)
				List.RemoveAt(index); 
		}
		public AddToAccount Item(int index)
		{
			return (AddToAccount) List[index];
		}
	}
}
