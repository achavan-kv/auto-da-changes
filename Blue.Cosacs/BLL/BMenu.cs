using System;
using STL.DAL;
using STL.Common;
using System.Data;
using System.Data.SqlClient;

namespace STL.BLL
{
	/// <summary>
	/// Summary description for BMenu.
	/// </summary>
	public class BMenu : CommonObject
	{
		public DataSet GetDynamicMenus(int id, string screen)
		{
			DataSet ds = new DataSet();
			DMenu menu = new DMenu();

				menu.GetMenusForRole(id, screen);
				ds.Tables.Add(menu.Menus);

			return ds;
		}

        public int? ControlPermissionCheck(string login, string screen, string control)
        {
            return new DMenu().ControlPermissionCheck(login, screen, control);
        }

		public BMenu()
		{

		}
	}
}
