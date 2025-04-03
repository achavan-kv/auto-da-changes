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
	/// Summary description for DTransType.
	/// </summary>
	public class DTransType : DALObject
	{
		private DataTable _transtypes = null;
		public DataTable TransTypes
		{
			get{return _transtypes;}
			set{_transtypes = value;}
		}

		private string _transtypecode = "";
		public string TransTypeCode
		{
			get{return _transtypecode;}
			set{_transtypecode = value;}
		}

		private string _description = "";
		public string Description
		{
			get{return _description;}
			set{_description = value;}
		}

		private string _balordue = "";
		public string BalOrDue
		{
			get{return _balordue;}
			set{_balordue = value;}
		}
		
		private string _exportfilesuffix = "";
		public string ExportFileSuffix
		{
			get{return _exportfilesuffix;}
			set{_exportfilesuffix = value;}
		}
		
		private string _batchtype = "";
		public string Batchtype
		{
			get{return _batchtype;}
			set{_batchtype = value;}
		}
		
		private string _interfacesecaccount = "";
		public string InterfaceSecAccount
		{
			get{return _interfacesecaccount;}
			set{_interfacesecaccount = value;}
		}
		
		private string _interfaceaccount = "";
		public string InterfaceAccount
		{
			get{return _interfaceaccount;}
			set{_interfaceaccount = value;}
		}
		
		private string _interfacebalancing = "";
		public string InterfaceBalancing
		{
			get{return _interfacebalancing;}
			set{_interfacebalancing = value;}
		}

		private int _tccodedr = 0;
		public int TCCodeDR
		{
			get{return _tccodedr;}
			set{_tccodedr = value;}
		}

		private int _tccodecr = 0;
		public int TCCodeCR
		{
			get{return _tccodecr;}
			set{_tccodecr = value;}
		}

		private int _isdeposit = 0;
		public int IsDeposit
		{
			get{return _isdeposit;}
			set{_isdeposit = value;}
		}

		private int _includeingft = 0;
		public int IncludeINGFT
		{
			get{return _includeingft;}
			set{_includeingft = value;}
		}
		
		private int _branchsplit = 0;
		public int BranchSplit
		{
			get{return _branchsplit;}
			set{_branchsplit = value;}
		}

		private new int _user = 0;
		public new int User
		{
			get{return _user;}
			set{_user = value;}
		}

		private short _isunique = 0;
		public short IsUnique
		{
			get{return _isunique;}
			set{_isunique = value;}
		}

		private short _isMandatory = 0;
		public short IsMandatory
		{
			get{return _isMandatory;}
			set{_isMandatory = value;}
		}

		private string _interfacesecbalancing = "";
		public string InterfaceSecBalancing
		{
			get{return _interfacesecbalancing;}
			set{_interfacesecbalancing = value;}
		}
		
		private short _branchsplitbalancing = 0;
		public short BranchSplitBalancing
		{
			get{return _branchsplitbalancing;}
			set{_branchsplitbalancing = value;}
		}

        //IP - 11/04/12 - CR9863 - #9885
        private string _scInterfaceAccount = "";
        public string SCInterfaceAccount
        {
            get { return _scInterfaceAccount; }
            set { _scInterfaceAccount = value; }
        }

        //IP - 11/04/12 - CR9863 - #9885
        private string _scInterfaceBalacing = "";
        public string SCInterfaceBalancing
        {
            get { return _scInterfaceBalacing; }
            set { _scInterfaceBalacing = value; }
        }

		public DTransType()
		{

		}

		/// <summary>
		/// GetGeneralTransactionTypes
		/// </summary>
		/// <returns>DataTable</returns>
		/// 
		public DataTable GetGeneralTransactionTypes ()
		{	
			DataTable dt = new DataTable(TN.GeneralTransactions);
			try
			{				
				this.RunSP("DN_TranstypeGetGeneralTypesSP", dt);				
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return dt;
		}

		public int GetTranstypeByCode()
		{
			try
			{
				_transtypes = new DataTable(TN.TransTypes);
				parmArray = new SqlParameter[1];
				parmArray[0] = new SqlParameter("@transtypecode", SqlDbType.NVarChar,3);
				parmArray[0].Value = this.TransTypeCode;
				result = this.RunSP("DN_TranstypeGetByCodeSP", parmArray, _transtypes);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}			
			return result;
		}

		public string GetDescription(string transType)
		{
			string description = "";
			try
			{
				parmArray = new SqlParameter[2];
				parmArray[0] = new SqlParameter("@transtypecode", SqlDbType.NVarChar,3);
				parmArray[0].Value = transType;
				parmArray[1] = new SqlParameter("@description", SqlDbType.NVarChar,80);
				parmArray[1].Value = description;
				parmArray[1].Direction = ParameterDirection.Output;

				result = this.RunSP("DN_TransTypeGetDescriptionSP", parmArray);

				if(parmArray[1].Value != DBNull.Value)
					description = (string)parmArray[1].Value;
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}			
			return description;
		}

		public int Save(SqlConnection conn, SqlTransaction trans)
		{
			try
			{
				parmArray = new SqlParameter[20];
				parmArray[0] = new SqlParameter("@transtypecode", SqlDbType.NVarChar,3);
				parmArray[0].Value = this.TransTypeCode;
				parmArray[1] = new SqlParameter("@tccodedr", SqlDbType.Int);
				parmArray[1].Value = this.TCCodeDR;
				parmArray[2] = new SqlParameter("@tccodecr", SqlDbType.Int);
				parmArray[2].Value = this.TCCodeCR;
				parmArray[3] = new SqlParameter("@description", SqlDbType.NVarChar,40);
				parmArray[3].Value = this.Description;
				parmArray[4] = new SqlParameter("@balordue", SqlDbType.NChar,1);
				parmArray[4].Value = this.BalOrDue;
				parmArray[5] = new SqlParameter("@exportfilesuffix", SqlDbType.NChar,1);
				parmArray[5].Value = this.ExportFileSuffix;
				parmArray[6] = new SqlParameter("@batchtype", SqlDbType.NVarChar,3);
				parmArray[6].Value = this.Batchtype;
				parmArray[7] = new SqlParameter("@includeingft", SqlDbType.SmallInt);
				parmArray[7].Value = this.IncludeINGFT;
				parmArray[8] = new SqlParameter("@interfacesecaccount", SqlDbType.NVarChar,10);
				parmArray[8].Value = this.InterfaceSecAccount;
				parmArray[9] = new SqlParameter("@interfaceaccount", SqlDbType.NVarChar,10);
				parmArray[9].Value = this.InterfaceAccount;
				parmArray[10] = new SqlParameter("@branchsplit", SqlDbType.SmallInt);
				parmArray[10].Value = this.BranchSplit;
				parmArray[11] = new SqlParameter("@isdeposit", SqlDbType.SmallInt);
				parmArray[11].Value = this.IsDeposit;
				parmArray[12] = new SqlParameter("@interfacebalancing", SqlDbType.NVarChar,10);
				parmArray[12].Value = this.InterfaceBalancing;
				parmArray[13] = new SqlParameter("@ismandatory", SqlDbType.SmallInt);
				parmArray[13].Value = this.IsMandatory;
				parmArray[14] = new SqlParameter("@isunique", SqlDbType.SmallInt);
				parmArray[14].Value = this.IsUnique;
				parmArray[15] = new SqlParameter("@user", SqlDbType.Int);
				parmArray[15].Value = this.User;
				parmArray[16] = new SqlParameter("@interfacesecbalancing", SqlDbType.NVarChar,10);
				parmArray[16].Value = this.InterfaceSecBalancing;
				parmArray[17] = new SqlParameter("@branchsplitbalancing", SqlDbType.SmallInt);
				parmArray[17].Value = this.BranchSplitBalancing;
                parmArray[18] = new SqlParameter("@scInterfaceAccount", SqlDbType.NVarChar, 10);                    //IP - 11/04/12 - CR9863 - #9885
                parmArray[18].Value = this.SCInterfaceAccount;
                parmArray[19] = new SqlParameter("@scInterfaceBalancing", SqlDbType.NVarChar, 10);                   //IP - 11/04/12 - CR9863 - #9885
                parmArray[19].Value = this.SCInterfaceBalancing;
				result = this.RunSP(conn, trans, "DN_TranstypeUpdateSP", parmArray);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}			
			return result;
		}

		/// <summary>
		/// GetDepositTypes
		/// </summary>
		/// <returns>DataTable</returns>
		/// 
		public DataTable GetDepositTypes (string tableName)
		{
			DataTable dt = new DataTable(tableName);
			try
			{
				this.RunSP("DN_TransTypeGetDepositTypesSP", dt);				
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return dt;
		}

        public DataTable GetCashLoanDisbursementMethods(string tableName)
        {
            DataTable dt = new DataTable(tableName);
            try
            {
                this.RunSP("GetCashLoanDisbursementMethods", dt);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return dt;
        }

        /// <summary>
        /// GetNonDepositTypes
        /// </summary>
        /// <returns>DataTable</returns>
        /// 
        public DataTable GetNonDepositTypes (string tableName)
        {
            DataTable dt = new DataTable(tableName);
            try
            {
                this.RunSP("DN_TransTypeGetNonDepositTypesSP", dt);				
            }
            catch(SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return dt;
        }

		public int LoadTransTypes()
		{
			try
			{
				_transtypes = new DataTable(TN.TransTypes);
				parmArray = new SqlParameter[1];
				parmArray[0] = new SqlParameter("@transtypecode", SqlDbType.NVarChar,3);
				parmArray[0].Value = this.TransTypeCode;
				result = this.RunSP("DN_TranstypeLoadCodesSP", parmArray, _transtypes);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}			
			return result;
		}	
	}
}