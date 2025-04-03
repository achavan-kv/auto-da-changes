using System;
using System.Collections.Specialized;

namespace STL.Common
{
	/// <summary>
	/// Summary description for StaticDataSingleton.
	/// </summary>
	public sealed class StaticDataSingleton
	{
		private static volatile StaticDataSingleton _instance = null;
		private static object _instanceLock = new object();
		private HybridDictionary _allData;

		public HybridDictionary Data
		{
			get
			{
				return _allData;
			}
			set
			{
				_allData = value;
			}
		}

		public static StaticDataSingleton Instance()
		{
			if (_instance == null)
			{
				lock (_instanceLock)
				{
					if (_instance == null)
					{
						_instance = new StaticDataSingleton();
					}
				}
			}
			return _instance;
		}

		private StaticDataSingleton()
		{
			_allData = new HybridDictionary();
		}
	}
}
