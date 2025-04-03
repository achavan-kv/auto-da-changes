using System;
using System.Data.SqlClient;
using STL.Common;
using STL.Common.Constants.Enums;

namespace STL.Common
{
	/// <summary>
	/// This class is used to specify which
	/// blocks of static data are required to 
	/// populate the drop down lists on each screen
	/// </summary>
	public class DropDownParm
	{
		public DropDown listName;
		public string[] parmList;

		public DropDownParm()
		{
		}
	}

	public class DropDownParmList
	{
		private DropDownParm[] _parmList = null;
		public DropDownParmList()
		{
		}
		public void Add(DropDown name, string[] parmlist)
		{
			DropDownParm p = new DropDownParm();
			p.listName = name;
			p.parmList = parmlist;
			int count = _parmList.Length;
			DropDownParm[] newList = new DropDownParm[count+1];
			_parmList.CopyTo(newList, 0);
			newList[count] = p;
		}
		public DropDownParm[] Array
		{
			get{return _parmList;}
		}
	}
}
