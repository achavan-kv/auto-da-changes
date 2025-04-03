using System;

namespace STL.Common
{
	/// <summary>
	/// An exception class for errors in the operation of the receipt printer.
	/// All exception messages will be Resource names which must be translated by the catch statement in 
	/// the calling code.
	/// </summary>
	[Serializable]
	public class SlipPrinterException : ApplicationException
	{
		public SlipPrinterException()
		{
		}
		public SlipPrinterException(string message)	: base(message)
		{
		}
	}
}
