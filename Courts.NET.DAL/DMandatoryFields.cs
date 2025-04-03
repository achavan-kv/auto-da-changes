using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using STL.Common.Constants.Enums;
using STL.Common.Constants.TableNames;

namespace STL.DAL
{
	/// <summary>
	/// Summary description for DMandatoryFields.
	/// </summary>
	public class DMandatoryFields : DALObject
	{
		private string _country = "";
		public new string Country
		{
			get{return _country;}
			set{_country = value;}
		}
		private string _screen = "";
		public string Screen
		{
			get{return _screen;}
			set{_screen = value;}
		}
		private string _control = "";
		public string Control
		{
			get{return _control;}
			set{_control = value;}
		}
		private string _description = "";
		public string Description
		{
			get{return _description;}
			set{_description = value;}
		}
		private bool _enabled = true;
		public bool Enabled
		{
			get{return _enabled;}
			set{_enabled = value;}
		}
		private bool _visible = true;
		public bool Visible
		{
			get{return _visible;}
			set{_visible = value;}
		}
		private bool _mandatory = true;
		public bool Mandatory
		{
			get{return _mandatory;}
			set{_mandatory = value;}
		}
		private DataTable _fields = null;
		public DataTable Fields
		{
			get{return _fields;}
			set{_fields = value;}
		}

		private DataTable _screens = null;
		public DataTable Screens
		{
			get{return _screens;}
			set{_screens = value;}
		}

		public void GetMandatoryFields(string country, string screen)
		{
			try
			{
				_fields = new DataTable("Fields");

				parmArray = new SqlParameter[2];
				parmArray[0] = new SqlParameter("@country", SqlDbType.NVarChar,2);
				parmArray[0].Value = country;
				parmArray[1] = new SqlParameter("@screen", SqlDbType.NVarChar,50);
				parmArray[1].Value = screen;

				this.RunSP("DN_MandatoryFieldsGetSP", parmArray, _fields);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

		public void Save(SqlConnection conn, SqlTransaction trans)
		{
			try
			{
				parmArray = new SqlParameter[7];
				parmArray[0] = new SqlParameter("@country", SqlDbType.NVarChar,2);
				parmArray[0].Value = this.Country;
				parmArray[1] = new SqlParameter("@screen", SqlDbType.NVarChar,50);
				parmArray[1].Value = this.Screen;
				parmArray[2] = new SqlParameter("@control", SqlDbType.NVarChar,200);
				parmArray[2].Value = this.Control;
				parmArray[3] = new SqlParameter("@description", SqlDbType.NVarChar,50);
				parmArray[3].Value = this.Description;
				parmArray[4] = new SqlParameter("@enabled", SqlDbType.SmallInt);
				parmArray[4].Value = Convert.ToInt16(this.Enabled);
				parmArray[5] = new SqlParameter("@visible", SqlDbType.SmallInt);
				parmArray[5].Value = Convert.ToInt16(this.Visible);
				parmArray[6] = new SqlParameter("@mandatory", SqlDbType.SmallInt);
				parmArray[6].Value = Convert.ToInt16(this.Mandatory);

				this.RunSP(conn, trans, "DN_MandatoryFieldsSaveSP", parmArray);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

		public int GetScreens()
		{
			try
			{
				_screens = new DataTable(TN.Screens);
				result = this.RunSP("DN_ScreensGetSP", _screens);

				if(result == 0)
					result = (int)Return.Success;
				else
					result = (int)Return.Fail;
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}

			return result;
		}

		public DMandatoryFields()
		{

		}
	}
}
