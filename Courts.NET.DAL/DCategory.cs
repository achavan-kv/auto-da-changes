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
	/// Summary description for DCategory.
	/// </summary>
	public class DCategory:DALObject
	{
		private DataTable _table;
		private DataTable _custCodes;
		private DataTable _acctCodes;
		private DataTable _relationships = null;
		private DataTable _addressType = null;
        private DataTable _insuranceTypes; //NM & IP - 08/01/09 - CR976

		public int GetCategoryCodes()
		{
			try
			{
				_table = new DataTable("Categories");
				result = this.RunSP("DN_CodesGetCategorySP", _table);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return result;
		}

		public int GetCustomerCodes()
		{
			try
			{
				_custCodes = new DataTable(TN.CustomerCodes);
				result = this.RunSP("DN_CodesGetCustomerSP", _custCodes);
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

		public int GetCustomerRelationships()
		{
			try
			{
				_relationships = new DataTable("Relationships");
				result = this.RunSP("DN_CodesGetLinkedCustomerSP", _relationships);
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

		public int GetAddressTypes()
		{
			try
			{
				_addressType = new DataTable(TN.AddressType);
				result = this.RunSP("DN_CodesGetAddressTypesSP", _addressType);
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

		public int GetAccountCodes()
		{
			try
			{
				_acctCodes = new DataTable(TN.AccountCodes);
				result = this.RunSP("DN_CodesGetAccountSP", _acctCodes);
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

        //NM & IP - 08/01/09 - CR976 - Return Insurance Types
        public int GetInsuranceTypes()
        {
            try
            {
                _insuranceTypes = new DataTable(TN.InsuranceTypes);
                result = this.RunSP("DN_GetInsuranceTypes", _insuranceTypes);
                if (result == 0)
                    result = (int)Return.Success;
                else
                    result = (int)Return.Fail;
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return result;
        }

		public DataTable Table
		{
			get 
			{
				return _table;
			}
		}
		public DataTable CustCodes
		{
			get 
			{
				return _custCodes;
			}
		}
		public DataTable AcctCodes
		{
			get 
			{
				return _acctCodes;
			}
		}
		public DataTable Relationships
		{
			get 
			{
				return _relationships;
			}
		}
		public DataTable AddressType
		{
			get 
			{
				return _addressType;
			}
		}

        //NM & IP - 08/01/09 - CR976
        public DataTable InsuranceTypes
        {
            get
            {
                return _insuranceTypes;
            }
        }
        
		public DCategory()
		{
			//
			// TODO: Add constructor logic here
			//
		}
	}
}
