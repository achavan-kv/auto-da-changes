using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using STL.Common;
using STL.Common.Constants.Enums;
using STL.Common.Constants.TableNames;


namespace STL.DAL
{
    /// <summary>
    /// Data Access methods for the Sets table.
    /// </summary>
    public class DSets : DALObject
    {
        private string _setname = "";
        public string SetName
        {
            get{return _setname;}
            set{_setname = value;}
        }

        private string _setdescript = "";
        public string SetDescript
        {
            get{return _setdescript;}
            set{_setdescript = value;}
        }

        private int _empeeno = 0;
        public int EmployeeNo
        {
            get{return _empeeno;}
            set{_empeeno = value;}
        }

        private decimal _value = 0;         // #13691
        public decimal Value
        {
            get { return _value; }
            set { _value = value; }
        }

        private string _tname = "";
        public string TName
        {
            get{return _tname;}
            set{_tname = value;}
        }

        private DateTime _dateamend;
        public DateTime DateAmend
        {
            get{return _dateamend;}
            set{_dateamend = value;}
        }

        private string _columntype = "";
        public string ColumnType
        {
            get{return _columntype;}
            set{_columntype = value;}
        }

        private DataTable _setsdata = null;
        public DataTable SetsData
        {
            get{return _setsdata;}
        }

        public void Save(SqlConnection conn, SqlTransaction trans)
        {
            try
            {
                parmArray = new SqlParameter[6];
                parmArray[0] = new SqlParameter("@setname", SqlDbType.VarChar,64);
                parmArray[0].Value = this.SetName;
                parmArray[1] = new SqlParameter("@setdescript", SqlDbType.VarChar,80);
                parmArray[1].Value = this.SetDescript;
                parmArray[2] = new SqlParameter("@empeeno", SqlDbType.Int);
                parmArray[2].Value = this.EmployeeNo;
                parmArray[3] = new SqlParameter("@tname", SqlDbType.VarChar,24);
                parmArray[3].Value = this.TName;
                parmArray[4] = new SqlParameter("@columntype", SqlDbType.Char,1);
                parmArray[4].Value = this.ColumnType;
                parmArray[5] = new SqlParameter("@value", SqlDbType.Money);         // #13691
                parmArray[5].Value = this.Value;
                this.RunSP(conn, trans, "DN_SetsSaveSP", parmArray);
            }
            catch(SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }			
        }

        public void Delete(SqlConnection conn, SqlTransaction trans)
        {
            try
            {
                parmArray = new SqlParameter[2];
                parmArray[0] = new SqlParameter("@setname", SqlDbType.VarChar,64);
                parmArray[0].Value = this.SetName;
                parmArray[1] = new SqlParameter("@tname", SqlDbType.VarChar,24);
                parmArray[1].Value = this.TName;
                this.RunSP(conn, trans, "DN_SetsDeleteSP", parmArray);
            }
            catch(SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }			
        }

        public int GetSets(string setName,string tName)
        {
            _setsdata = new DataTable("SetsData");

            try
            {
                parmArray = new SqlParameter[2];
                parmArray[0] = new SqlParameter("@setname", SqlDbType.VarChar,64);
                parmArray[0].Value = setName;
                parmArray[1] = new SqlParameter("@tname", SqlDbType.VarChar,24);
                parmArray[1].Value = tName;						
                result = this.RunSP("DN_SetsGetSP", parmArray, _setsdata);
            }
            catch(SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }			
            return result;
        }

        public int GetSetsForTName(string tName)
        {
            _setsdata = new DataTable("SetsData");

            try
            {
                parmArray = new SqlParameter[1];
                parmArray[0] = new SqlParameter("@tname", SqlDbType.VarChar,24);
                parmArray[0].Value = tName;
						
                result = this.RunSP("DN_SetsForTnameGetSP", parmArray, _setsdata);
            }
            catch(SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }			
            return result;
        }

		public int GetSetsForTNameBranch(string tName, short branchNo)
		{
			_setsdata = new DataTable("SetsData");

			try
			{
				parmArray = new SqlParameter[2];
				parmArray[0] = new SqlParameter("@tname", SqlDbType.VarChar,24);
				parmArray[0].Value = tName;
				parmArray[1] = new SqlParameter("@branchno", SqlDbType.SmallInt);
				parmArray[1].Value = branchNo;
						
				result = this.RunSP("DN_SetsForTnameBranchGetSP", parmArray, _setsdata);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}			
			return result;
		}

		public int GetSetsForTNameBranchAll(string tName)
		{
			_setsdata = new DataTable("SetsData");

			try
			{
				parmArray = new SqlParameter[1];
				parmArray[0] = new SqlParameter("@tname", SqlDbType.VarChar,24);
				parmArray[0].Value = tName;
						
				result = this.RunSP("DN_SetsForTnameBranchGetAllSP", parmArray, _setsdata);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}			
			return result;
		}


        public DSets()
        {
            //
            // TODO: Add constructor logic here
            //
        }
    }
}
