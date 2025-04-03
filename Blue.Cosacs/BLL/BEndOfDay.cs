using System;
using STL.DAL;
using STL.Common;
using System.Data;
using STL.Common.Constants.Enums;
using STL.Common.Constants.TableNames;
using STL.Common.Constants.ColumnNames;
using System.Xml;
using STL.Common.Constants.Tags;
using STL.Common.Constants.Elements;

namespace STL.BLL
{
	/// <summary>
	/// Retrieves and saves End of Day data
	/// </summary>
	public class BEndOfDay : CommonObject
	{
		public BEndOfDay()
		{

		}

		public DataTable GetEodOptionList (string configuration)
		{
			DEndOfDay eodOptionList = new DEndOfDay();
			return eodOptionList.GetEodOptionList(configuration);
		}

	}
}
