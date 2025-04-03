using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using STL.Common;
using STL.Common.Constants.Enums;
using System.Xml;
using STL.Common.Constants.Tags;
using STL.Common.Constants.TableNames;

namespace STL.DAL
{
	/// <summary>
	/// Summary description for DCode.
	/// </summary>
	public class DCode : DALObject
	{
		private string _category = "";
		public string Category
		{
			get{return _category;}
			set{_category = value;}
		}

		private string _flag = "L";
		public string StatusFlag
		{
			get{return _flag;}
			set{_flag = value;}
		}

		private DataTable _codes = null;
		public DataTable Codes
		{
			get{return _codes;}
			set{_codes = value;}
		}

        //IP - 22/04/08 - UAT(223) v.5.1
        private DataTable _eodcodes = null;
        public DataTable EodCodes
        {
            get { return _eodcodes; }
            set { _eodcodes = value; }
        }

		private DataTable _categories = null;
		public DataTable Categories
		{
			get{return _categories;}
			set{_categories = value;}
		}

        //IP - 22/04/08 - UAT(223) V.(5.1)
        public int GetEodOptions(XmlNode parmList, string tableName)
        {
            try
            {
                XmlNode category = parmList.FirstChild;
                XmlNode stat = category.NextSibling;

                _eodcodes = new DataTable(tableName);
                parmArray = new SqlParameter[2];
                parmArray[0] = new SqlParameter("@category", SqlDbType.NVarChar, 12);
                parmArray[0].Value = category.Attributes[Tags.Value].Value;
                parmArray[1] = new SqlParameter("@flag", SqlDbType.NVarChar, 1);
                parmArray[1].Value = stat.Attributes[Tags.Value].Value;
                result = this.RunSP("DN_GetEodOptionsSP", parmArray, _eodcodes);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return result;
        }

		public int GetCategoryCodes(string category, string statusFlag, string tableName)
		{
			try
			{
				_codes = new DataTable(tableName);
				parmArray = new SqlParameter[2];
				parmArray[0] = new SqlParameter("@category", SqlDbType.NVarChar,12);
				parmArray[0].Value = category;
				parmArray[1] = new SqlParameter("@flag", SqlDbType.NVarChar,1);
				parmArray[1].Value = statusFlag;
				result = this.RunSP("DN_CodeGetByCategorySP", parmArray, _codes);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return result;
		}

		public int GetCategoryCodes(XmlNode parmList, string tableName)
		{
			try
			{
				if(parmList.ChildNodes.Count < 3)
				{
					XmlNode category = parmList.FirstChild;
					XmlNode stat = category.NextSibling;

					_codes = new DataTable(tableName);
					parmArray = new SqlParameter[2];
					parmArray[0] = new SqlParameter("@category", SqlDbType.NVarChar,12);
					parmArray[0].Value = category.Attributes[Tags.Value].Value;
					parmArray[1] = new SqlParameter("@flag", SqlDbType.NVarChar,1);
					parmArray[1].Value = stat.Attributes[Tags.Value].Value;
					result = this.RunSP("DN_CodeGetByCategorySP", parmArray, _codes);
				}
				else
				{
					XmlNode category1 = parmList.FirstChild;
					XmlNode category2 = category1.NextSibling;
					XmlNode stat = category2.NextSibling;

					_codes = new DataTable(tableName);
					parmArray = new SqlParameter[3];
					parmArray[0] = new SqlParameter("@category1", SqlDbType.NVarChar,4);
					parmArray[0].Value = category1.Attributes[Tags.Value].Value;
					parmArray[1] = new SqlParameter("@category2", SqlDbType.NVarChar,4);
					parmArray[1].Value = category2.Attributes[Tags.Value].Value;
					parmArray[2] = new SqlParameter("@flag", SqlDbType.NVarChar,1);
					parmArray[2].Value = stat.Attributes[Tags.Value].Value;
					result = this.RunSP("DN_CodeGetByCategorySP2", parmArray, _codes);
				}
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return result;
		}

		public int GetProductCategories(string tableName)
		{
			try
			{
				_codes = new DataTable(tableName);
				result = this.RunSP("DN_CodesGetProductCategoriesSP", _codes);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return result;
		}

		public int GetAllCodesAndCategories()
		{
			try
			{
				DataSet ds = new DataSet();

				result = this.RunSP("DN_CodeGetAllSP", ds);

				_codes = ds.Tables[0];
				_codes.TableName = TN.Code;
				_categories = ds.Tables[1];
				_categories.TableName = TN.CodeCat;
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return result;
		}

		public int Delete(SqlConnection conn, SqlTransaction trans, string Code, string Category)
		{
			try
			{
				parmArray = new SqlParameter[2];
				parmArray[0] = new SqlParameter("@category", SqlDbType.NVarChar,12);
				parmArray[0].Value = Category;
				parmArray[1] = new SqlParameter("@code", SqlDbType.NVarChar,12);
				parmArray[1].Value = Code;
				result = this.RunSP(conn, trans, "DN_CodeDeleteSP", parmArray);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return result;
		}

		public int Update(SqlConnection conn, SqlTransaction trans, string Code, 
			string Category, string OldCode, string OldCategory, string CodedScript, 
			int SortOrder, string Reference,string Additional, string Additional2, bool IsMmiApplicable)              //IP - 07/12/11 - CR1234
		{
			try
			{
				parmArray = new SqlParameter[10];
				parmArray[0] = new SqlParameter("@category", SqlDbType.NVarChar,12);
				parmArray[0].Value = Category;
				parmArray[1] = new SqlParameter("@code", SqlDbType.NVarChar,18);        // RI jec
				parmArray[1].Value = Code;
				parmArray[2] = new SqlParameter("@oldcategory", SqlDbType.NVarChar,12);
				parmArray[2].Value = OldCategory;
                parmArray[3] = new SqlParameter("@oldcode", SqlDbType.NVarChar, 18);     // RI jec
				parmArray[3].Value = OldCode;
				parmArray[4] = new SqlParameter("@codedescript", SqlDbType.NVarChar,64);
				parmArray[4].Value = CodedScript;
				parmArray[5] = new SqlParameter("@sortorder", SqlDbType.SmallInt);
				parmArray[5].Value = SortOrder;
				parmArray[6] = new SqlParameter("@reference", SqlDbType.NVarChar,12);
				parmArray[6].Value = Reference;
                parmArray[7] = new SqlParameter("@Additional", SqlDbType.NVarChar, 15);
                parmArray[7].Value = Additional;
                parmArray[8] = new SqlParameter("@Additional2", SqlDbType.NVarChar, 15);         //IP - 07/12/11 - CR1234
                parmArray[8].Value = Additional2;
                parmArray[9] = new SqlParameter("@IsMmiApplicable", SqlDbType.Bit); 
                parmArray[9].Value = IsMmiApplicable;
                result = this.RunSP(conn, trans, "DN_CodeUpdateSP", parmArray);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return result;
		}

		public int GetWriteOffCategories(string tableName)
		{
			try
			{
				_codes = new DataTable(tableName);
				result = this.RunSP("DN_CodesGetWriteOffCategoriesSP", _codes);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return result;
		}

		public DCode()
		{
			//
			// TODO: Add constructor logic here
			//
		}
	}
}
