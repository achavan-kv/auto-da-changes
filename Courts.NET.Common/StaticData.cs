using System;
using System.Collections;
using System.Data;

namespace STL.Common.Static
{
	/// <summary>
	/// This will be a static class accessed from the client. 
	///	It will contain a hashtable which will be used to store
	///	static drop down data. This will be used as a cache so 
	///	that static data is not unnecessarily retrieved from the 
	///	web server 
	///
	/// </summary>
	public class StaticData
	{
		private static Hashtable _statData = null;
		public static Hashtable Tables
		{
			get
			{
				if(_statData==null)
					_statData = new Hashtable();
				return _statData;
			}
			set
			{
				if(_statData==null)
					_statData = new Hashtable();
				_statData = value;
			}
		}

		private static Hashtable _dictionaries = null;
		public static Hashtable Dictionaries
		{
			get
			{
				if(_dictionaries==null)
					_dictionaries = new Hashtable();
				return _dictionaries;
			}
			set
			{
				if(_dictionaries==null)
					_dictionaries = new Hashtable();
				_dictionaries = value;
			}
		}

	}
}
