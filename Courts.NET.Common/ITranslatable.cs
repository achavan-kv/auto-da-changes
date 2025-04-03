using System;

namespace STL.Common
{
	/// <summary>
	/// Summary description for ITranslatable.
	/// </summary>
	public interface ITranslatable
	{
		string GetResource(string msgName, object[] parms);
		string GetResource(string msgName);
		string Translate(string text, string culture);
	}
}
