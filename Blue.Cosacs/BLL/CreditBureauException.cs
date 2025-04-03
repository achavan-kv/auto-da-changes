using System;

namespace STL.BLL
{
	/// <summary>
	/// Exception class to deal wih exceptions which may occur while 
	/// communicating with the credit bureau. Generally this will 
	/// either be validation errors at our end, parsing errors at 
	/// the bureau, business rule errors at the bureau or 
	/// communication errors (WebExceptions).
	/// </summary>
	public class CreditBureauException : ApplicationException
	{
		public CreditBureauException(string message, Exception inner) : base(message, inner)
		{
		}

		public CreditBureauException(string message) : base(message)
		{ 
		}
	}
}
