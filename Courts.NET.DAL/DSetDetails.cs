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
    /// Data Access methods for the SetDetails table.
    /// </summary>
    public class DSetDetails : DALObject
    {
        private string _setname = "";
        public string SetName
        {
            get{return _setname;}
            set{_setname = value;}
        }

        private string _data = "";
        public string Data
        {
            get{return _data;}
            set{_data = value;}
        }

        private string _tname = "";
        public string TName
        {
            get{return _tname;}
            set{_tname = value;}
        }

		private string _branchno = "";
		public string branchNo
		{
			get{return _branchno;}
			set{_branchno = value;}
		}


        public void Insert(SqlConnection conn, SqlTransaction trans)
        {
            try 
            {
                parmArray = new SqlParameter[3];
                parmArray[0] = new SqlParameter("@setname", SqlDbType.VarChar,64);
                parmArray[0].Value = this.SetName;
                parmArray[1] = new SqlParameter("@data", SqlDbType.VarChar,32);
                parmArray[1].Value = this.Data;
                parmArray[2] = new SqlParameter("@tname", SqlDbType.VarChar,24);
                parmArray[2].Value = this.TName;
                this.RunSP(conn, trans, "DN_SetDetailsInsertSP", parmArray);
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
                this.RunSP(conn, trans, "DN_SetDetailsDeleteAllSP", parmArray);
            }
            catch(SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }			
        }

		public void InsertBranch(SqlConnection conn, SqlTransaction trans)
		{
			try 
			{
				parmArray = new SqlParameter[3];
				parmArray[0] = new SqlParameter("@setname", SqlDbType.VarChar,64);
				parmArray[0].Value = this.SetName;
				parmArray[1] = new SqlParameter("@tname", SqlDbType.VarChar,24);
				parmArray[1].Value = this.TName;
				parmArray[2] = new SqlParameter("@branchno", SqlDbType.SmallInt);
				parmArray[2].Value = this.branchNo;
				this.RunSP(conn, trans, "DN_SetDetailsInsertBranchSP", parmArray);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}			
		}

		public void DeleteBranch(SqlConnection conn, SqlTransaction trans)
		{
			try
			{
				parmArray = new SqlParameter[2];
				parmArray[0] = new SqlParameter("@setname", SqlDbType.VarChar,64);
				parmArray[0].Value = this.SetName;
				parmArray[1] = new SqlParameter("@tname", SqlDbType.VarChar,24);
				parmArray[1].Value = this.TName;
				this.RunSP(conn, trans, "DN_SetDetailsDeleteBranchSP", parmArray);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}			
		}

        public DataSet GetSetDetailsForSetName(string setName, string tName)
        {
			DataSet detailsSet = new DataSet();

            try
            {
                parmArray = new SqlParameter[2];
                parmArray[0] = new SqlParameter("@setname", SqlDbType.VarChar,64);
                parmArray[0].Value = setName;
                parmArray[1] = new SqlParameter("@tname", SqlDbType.VarChar,24);
                parmArray[1].Value = tName;
						
                result = this.RunSP("DN_SetDetailsForSetNameGetSP", parmArray, detailsSet);

				if (detailsSet.Tables.Count > 0)
					detailsSet.Tables[0].TableName = TN.SetDetailsData;

				if (detailsSet.Tables.Count > 1)
					detailsSet.Tables[1].TableName = TN.SetBranchData;
            }
            catch(SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }			
            return detailsSet;
        }

        public CategoryItem GetCategoryItem(string categoryCode)
        {
            DataSet categorySet = new DataSet();
            CategoryItem categoryItem;

            try
            {
                parmArray = new SqlParameter[1];
                parmArray[0] = new SqlParameter("@code", SqlDbType.VarChar, 12);
                parmArray[0].Value = categoryCode;


                result = this.RunSP("DN_GetCategoryItemSP", parmArray, categorySet);

                if (categorySet.Tables[0].Rows.Count > 0)
                { 
                    DataRow currRow = categorySet.Tables[0].Rows[0];
                    categoryItem = new CategoryItem(currRow["category"].ToString(),
                                                    currRow["code"].ToString(),
                                                    currRow["codeDescript"].ToString());
                }
                else
                    // if not found return an empty struct
                    categoryItem = new CategoryItem();
                    
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return categoryItem;    
        }

        public DSetDetails()
        {
            //
            // TODO: Add constructor logic here
            //
        }
    }

    public struct CategoryItem
    {
        public string Category;
        public string Code;
        public string CodeDescript;
        public CategoryItem(string category,string code, string codeDescript)
        { 
            this.Category = category;
            this.Code = code;
            this.CodeDescript = codeDescript;
        }
    }
}
