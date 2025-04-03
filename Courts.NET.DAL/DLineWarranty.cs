using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using STL.Common.Constants.Enums;
using STL.Common.Constants.TableNames;

namespace STL.DAL
{
	/// <summary>
	/// Summary description for DLineWarranty.
	/// </summary>
	public class DLineWarranty : DALObject
	{
		private string _acctNo = "";
		public string AccountNumber
		{
			get{return _acctNo;}
			set{_acctNo = value;}
		}
		private int _agreementNo = 0;
		public int AgreementNumber
		{
			get{return _agreementNo;}
			set{_agreementNo = value;}
		}
		private string _warrantyNo = "";
		public string WarrantyNumber
		{
			get{return _warrantyNo;}
			set{_warrantyNo = value;}
		}
		private string _warrantyID = "";
		public string WarrantyID
		{
			get{return _warrantyID;}
			set{_warrantyID = value;}
		}
		private short _stockLocn = 0;
		public short StockLocation
		{
			get{return _stockLocn;}
			set{_stockLocn = value;}
		}
		private string _itemNo = "";
		public string ItemNumber
		{
			get{return _itemNo;}
			set{_itemNo = value;}
		}
		private string _contractNo = "";
		public string ContractNumber
		{
			get{return _contractNo;}
			set{_contractNo = value;}
		}
		private int _delToFact = 0;
		public int DelToFact
		{
			get{return _delToFact;}
			set{_delToFact = value;}
		}
		private string _kitNo = "";
		public string KitNumber
		{
			get{return _kitNo;}
			set{_kitNo = value;}
		}
		private int _empeeno = 0;
		public int EmployeeNumber
		{
			get{return _empeeno;}
			set{_empeeno = value;}
		}



		public void AutoWarranty(string branchno, out string contractno)
			// Method to get auto warranty contract number from db procedure
		{
			contractno="";
			try
			{
				parmArray = new SqlParameter[2];
				parmArray[0] = new SqlParameter("@branchno", SqlDbType.SmallInt);
				parmArray[0].Value = branchno;
				parmArray[1] = new SqlParameter("@contractno", SqlDbType.Int);
				parmArray[1].Value = 0;
				parmArray[1].Direction = ParameterDirection.Output;
			
				this.RunSP("DN_BranchWarrantyGetContractNoSP", parmArray);

				if(parmArray[1].Value!=DBNull.Value)
				{
					contractno = branchno + ((int)parmArray[1].Value).ToString().PadLeft(5,'0');
				}
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

		public DataTable GetWarrantyContractDetails(string accountNo, int agreementNo, string contractNo)
		{
			DataTable dt = new DataTable();
			try
			{
				parmArray = new SqlParameter[3];
				parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar, 12);
				parmArray[0].Value = accountNo;
				parmArray[1] = new SqlParameter("@agreementNo", SqlDbType.Int);
				parmArray[1].Value = agreementNo;
				parmArray[2] = new SqlParameter("@contractNo", SqlDbType.NVarChar, 10);
				parmArray[2].Value = contractNo;
		
				this.RunSP("DN_WarrantyGetContractDetailsSP", parmArray, dt);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return dt;
		}



        public DataTable GetAssociatedWarranties(SqlConnection conn, SqlTransaction trans, string accountNo, int itemId, short stockLocn)       // RI
		{
			DataTable dt = new DataTable(TN.Warranties);
			try
			{
				parmArray = new SqlParameter[3];
				parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar, 12);
				parmArray[0].Value = accountNo;
                //parmArray[1] = new SqlParameter("@itemno", SqlDbType.NVarChar, 8);
                //parmArray[1].Value = itemNo;
                parmArray[1] = new SqlParameter("@itemid", SqlDbType.Int);          // RI
                parmArray[1].Value = itemId;
				parmArray[2] = new SqlParameter("@stocklocn", SqlDbType.SmallInt);
				parmArray[2].Value = stockLocn;
		
				if(conn!=null && trans !=null)
					RunSP(conn, trans, "DN_LineItemGetAssociatedWarrantiesSP", parmArray, dt);
				else
					RunSP("DN_LineItemGetAssociatedWarrantiesSP", parmArray, dt);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return dt;
		}

		public int GetContractNos(short branchno, int number)
		{
			int last = 0;
			try
			{
				parmArray = new SqlParameter[3];
				parmArray[0] = new SqlParameter("@branchno", SqlDbType.SmallInt);
				parmArray[0].Value = branchno;
				parmArray[1] = new SqlParameter("@number", SqlDbType.Int);
				parmArray[1].Value = number;
				parmArray[2] = new SqlParameter("@last", SqlDbType.Int);
				parmArray[2].Value = 0;
				parmArray[2].Direction = ParameterDirection.Output;
			
				this.RunSP("DN_BranchWarrantyGetMultipleContractNosSP", parmArray);

				if(parmArray[2].Value!=DBNull.Value)
					last = (int)parmArray[2].Value;
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return last;
		}

		public DLineWarranty()
		{

		}
	}
}
