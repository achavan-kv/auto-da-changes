using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using STL.Common.Constants.ColumnNames;
using STL.Common.Constants.Enums;

namespace STL.DAL
{
	/// <summary>
	/// Summary description for DLogin.
	/// </summary>
	public class DLogin: DALObject
	{
		private string[] _roles = null;
		private DataTable _rolesTab = null;
		public DLogin()
		{
	
		}

		public string[] GetRoles(string user, out string empeeNo)
		{
			try
			{
				empeeNo = "0";
				_rolesTab = new DataTable("Roles");
				parmArray = new SqlParameter[1];
				parmArray[0] = new SqlParameter("@user", SqlDbType.NVarChar,10);
				parmArray[0].Value = user;
				result = this.RunSP("DN_UserRolesGetSP", parmArray, _rolesTab);
				if(result==0)
				{
					if (_rolesTab.Rows.Count > 0)
					{
						// If a FACT employee number was supplied then
						// the CoSACS employee no will have been returned.
						empeeNo = (string)_rolesTab.Rows[0][CN.EmployeeNo];
						_roles = new string[_rolesTab.Rows.Count];
						for (int i = 0; i < _rolesTab.Rows.Count; i++)
						{
							_roles[i] = (string)_rolesTab.Rows[i][CN.EmployeeType];
						}
					}
				}
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return _roles;
		}

		public int Validate(string UserID, string pass)
		{
			try
			{
				parmArray = new SqlParameter[3];
				parmArray[0] = new SqlParameter("@password", SqlDbType.NVarChar,12);
				parmArray[0].Value = pass.ToLower();
				parmArray[1] = new SqlParameter("@empeeno", SqlDbType.NVarChar,10);
				parmArray[1].Value = UserID.ToUpper();
				parmArray[2] = new SqlParameter("@role", SqlDbType.NVarChar,1);
				parmArray[2].Value = "";
				parmArray[2].Direction = ParameterDirection.Output;
			
				result = this.RunSP("DN_CheckPasswordSP", parmArray);
				if(result==0)
				{
					_roles = new string[]{(string)parmArray[2].Value};
				}
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return result;
		}

      public DataTable GetUserDetails()
      {
         DataTable dtUserDetails = new DataTable();
         try
         {
            dtUserDetails = this.RunSPNoReturn("CourtsPersonGetUserDetailsSP");
         }
         catch (SqlException ex)
         {
            LogSqlException(ex);
            throw ex;
         }
         return dtUserDetails;
      }

		public string[] Roles
		{
			get
			{
				return _roles;
			}
			set
			{
				_roles = value;
			}
		}

        public bool CheckRolePermission(int empeeNo, string password, string taskName, out string message)
        {
            try
            {
                parmArray = new SqlParameter[5];
                parmArray[0] = new SqlParameter("@EmpeeNo", SqlDbType.Int);
                parmArray[0].Value = empeeNo;
                parmArray[1] = new SqlParameter("@Password", SqlDbType.VarChar, 12);
                parmArray[1].Value = password;
                parmArray[2] = new SqlParameter("@TaskName", SqlDbType.VarChar, 100);
                parmArray[2].Value = taskName.Trim();
                parmArray[3] = new SqlParameter("@IsPermitted", SqlDbType.Bit);
                parmArray[3].Value = DBNull.Value;
                parmArray[3].Direction = ParameterDirection.Output;
                parmArray[4] = new SqlParameter("@Message", SqlDbType.VarChar, 200);
                parmArray[4].Value = "";
                parmArray[4].Direction = ParameterDirection.Output;

                result = this.RunSP("dbo.CheckRolePermission", parmArray);

                message = parmArray[4].Value != DBNull.Value ? parmArray[4].Value.ToString() : "";

                return parmArray[3].Value != DBNull.Value ? Convert.ToBoolean(parmArray[3].Value) : false;                
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }
    }
}
