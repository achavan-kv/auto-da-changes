using System;
using STL.DAL;
using STL.Common;
using System.Data;

namespace STL.BLL
{
	/// <summary>
	/// Summary description for BCustomerImage.
	/// </summary>
	public class BCustomerImage : CommonObject
	{
		public BCustomerImage()
		{
		}

		/// <summary>
		/// Get
		/// </summary>
		/// <param name="custid">string</param>
		/// <returns>Byte[]</returns>
		/// 
		public Byte[] Get (string custid)
		{			 
			DCustomerImage da = new DCustomerImage();
			return da.Get(custid);			
		}

		public void Update(string custid, byte[] image)
		{
			DCustomerImage da = new DCustomerImage();
			da.Update(custid, image, DateTime.Now);
		}
	}
}