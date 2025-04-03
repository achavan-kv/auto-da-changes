using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using STL.Common.Constants.Enums;

namespace STL.DAL
{
	/// <summary>
	/// Summary description for DMenu.
	/// </summary>
	public class DMenu : DALObject
	{
		private DataTable _menus = null;
		public DataTable Menus
		{
			get{ return _menus;}
		}

		public void GetMenusForRole(int id, string screen)
		{
			try
			{
				_menus = new DataTable("Menus");

				parmArray = new SqlParameter[2];
				parmArray[0] = new SqlParameter("@userId", SqlDbType.NVarChar,256);
				parmArray[0].Value = id;
				parmArray[1] = new SqlParameter("@screen", SqlDbType.NVarChar,50);
				parmArray[1].Value = screen;

				this.RunSP("DN_GetMenusForRoleSP", parmArray, _menus);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

        public int? ControlPermissionCheck(string login, string screen, string control)
        {
            try
            {
              
                parmArray = new SqlParameter[3];
                parmArray[0] = new SqlParameter("@login", SqlDbType.NVarChar, 256);
                parmArray[0].Value = login;
                parmArray[1] = new SqlParameter("@screen", SqlDbType.NVarChar, 50);
                parmArray[1].Value = screen;
                parmArray[2] = new SqlParameter("@control", SqlDbType.NVarChar, 50);
                parmArray[2].Value = control;

                return this.ReturnIntNoReturn("Admin.CheckControlPermission", parmArray);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

		public DMenu()
		{

		}
	}
}
