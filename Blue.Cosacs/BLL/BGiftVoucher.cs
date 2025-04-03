using System;
using STL.DAL;
using STL.Common;
using System.Data;
using System.Xml;
using System.Data.SqlClient;
using STL.Common.Constants.Enums;
using STL.Common.Constants.AccountTypes;
using STL.Common.Constants.TableNames;
using STL.Common.Constants.ColumnNames;
using System.Collections.Specialized;
using STL.Common.Constants.FTransaction;
using STL.Common.Constants.AccountCodes;

namespace STL.BLL
{
	/// <summary>
	/// Summary description for BGiftVoucher.
	/// </summary>
	public class BGiftVoucher : CommonObject
	{
		public BGiftVoucher()
		{
		}

		/// <summary>
		/// Validate
		/// </summary>
		/// <param name="reference">string</param>
		/// <param name="courts">int</param>
		/// <param name="value">double</param>
		/// <param name="expiry">DateTime</param>
		/// <returns>void</returns>
		/// 
		public void Validate (string reference, bool courts, out decimal voucherValue, out DateTime expiry, bool includeRedeemed, out bool redeemed)
		{			 
			DGiftVoucher da = new DGiftVoucher();
			da.Validate(reference, courts, out voucherValue, out expiry, includeRedeemed, out redeemed);			
		}

		/// <summary>
		/// Sell
		/// </summary>
		/// <param name="reference">string</param>
		/// <param name="value">double</param>
		/// <param name="empeenosold">int</param>
		/// <param name="dateexpiry">DateTime</param>
		/// <param name="countrycode">string</param>
		/// <returns>void</returns>
		/// 
		public void Sell (	SqlConnection conn, SqlTransaction trans, 
			string reference, decimal voucherValue, 
			DateTime dateexpiry, 
			string countrycode, string bankCode, 
			string bankAcctNo, string chequeNo,
			short payMethod, short branchNo,
			bool free, bool privilegeClub, DataSet accountSet )
		{
			DateTime timeToday = DateTime.Now;
			BBranch b = new BBranch();

			if(!free)
			{
				int refNo = b.GetTransRefNo(conn, trans, branchNo);

				new BTransaction(conn, trans, 
					(string)Country[CountryParameterNames.GiftVoucherAccount], 
					branchNo,
					refNo, -voucherValue, User, TransType.Payment, bankCode, 
					bankAcctNo, chequeNo, payMethod, countrycode, timeToday,
					"", 0);
			}

			DGiftVoucher da = new DGiftVoucher();
			da.InsertCourts(conn, trans, reference, voucherValue, User, dateexpiry, countrycode, free);

			if (privilegeClub)
			{
				BAccount acct = new BAccount();
				DFinTrans freeInstalment = new DFinTrans();
				foreach (DataTable accountList in accountSet.Tables)
					foreach (DataRow row in accountList.Rows)
					{
						// Add the Account Code (or update the date) for a Free Instalment
						acct.AddCodeToAccount(conn, trans, (string)row[CN.AccountNo], AC.FreeInstalment, User, timeToday);

						// Save the free instalment
						freeInstalment.SaveFreeInstalment(conn, trans, (string)row[CN.AccountNo], branchNo, timeToday, -(decimal)row[CN.FreeInstalment],  true);
					}
			}
		}

		public void RedeemOther(SqlConnection conn, SqlTransaction trans, 
								string reference, decimal voucherValue, string acctNoCompany)
		{
			DGiftVoucher da = new DGiftVoucher();
			da.InsertOther(conn, trans, reference, voucherValue, acctNoCompany);
		}
	}
}