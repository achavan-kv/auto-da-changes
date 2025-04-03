using System;

namespace STL.Common
{
	/// <summary>
	/// Summary description for ApplicationException.
	/// </summary>
	public class STLException : System.Exception
	{
		public STLException()
		{

		}
		public STLException(string message)	: base(message)
		{
		}
	}


}
