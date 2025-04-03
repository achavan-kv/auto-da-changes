using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Configuration;
using STL.Common.Constants.TableNames;
using STL.Common;


namespace STL.DAL
{
	/// <summary>
	/// Summary description for DCustomerMailing.
	/// </summary>
	public class DCustomerMailing : DALObject
	{
		public DCustomerMailing()
		{
			//
			// TODO: Add constructor logic here
			//
		}

        private DataTable _customer;
        public DataTable Customer
        {
            get { return _customer; }
        }
			

		/// <summary>
		/// CustomerMailingQuery
		/// </summary>
		/// <param name="CustomerCodeSet">string</param>
		/// <param name="NoCustomerCodeSet">string</param>
		/// <param name="AccountCodeSet">string</param>
		/// <param name="NoAccountCodeSet">string</param>
		/// <param name="ArrearsRestriction">string</param>
		/// <param name="Arrears">double</param>
		/// <param name="maxcurrstatus">string</param>
		/// <param name="maxeverstatus">string</param>
		/// <param name="branchset">string</param>
		/// <param name="accttypes">string</param>
		/// <param name="itemset">string</param>
		/// <param name="itemsetstartdate">DateTime</param>
		/// <param name="itemsetenddate">DateTime</param>
		/// <param name="noitemset">string</param>
		/// <param name="noitemsetstartdate">DateTime</param>
		/// <param name="noitemsetenddate">DateTime</param>
		/// <param name="itemcatset">string</param>
		/// <param name="itemCatsetstartdate">DateTime</param>
		/// <param name="itemCatsetenddate">DateTime</param>
		/// <param name="noitemCatset">string</param>
		/// <param name="noitemCatsetstartdate">DateTime</param>
		/// <param name="noitemCatsetenddate">DateTime</param>
		/// <param name="itemsdelivered">int</param>
		/// <param name="itemstartswithset">string</param>
		/// <param name="itemstartswithstartdate">DateTime</param>
		/// <param name="itemstartswithenddate">DateTime</param>
		/// <param name="noitemstartswithset">string</param>
		/// <param name="noitemstartswithstartdate">DateTime</param>
		/// <param name="noitemstartswithenddate">DateTime</param>
		/// <param name="noletterset">string</param>
		/// <param name="nolettersetStartdate">DateTime</param>
		/// <param name="nolettersetEnddate">DateTime</param>
		/// <param name="letterset">string</param>
		/// <param name="lettersetstartdate">DateTime</param>
		/// <param name="lettersetenddate">DateTime</param>
		/// <param name="customerstartage">int</param>
		/// <param name="customerEndage">int</param>
		/// <param name="totals">string</param>
		/// <param name="resulttype">string</param>
		/// <param name="excludecancellations">int</param>
		/// <returns>DataSet</returns>
		/// 
		public int CustomerMailingQuery (string CustomerCodeSet, string NoCustomerCodeSet, string AccountCodeSet, string NoAccountCodeSet, string ArrearsRestriction, double Arrears, string maxcurrstatus, string maxeverstatus, string branchset, string accttypes, string itemset, DateTime itemsetstartdate, DateTime itemsetenddate, string noitemset, DateTime noitemsetstartdate, DateTime noitemsetenddate, string itemcatset, DateTime itemCatsetstartdate, DateTime itemCatsetenddate, string noitemCatset, DateTime noitemCatsetstartdate, DateTime noitemCatsetenddate, int itemsdelivered, string itemstartswithset, DateTime itemstartswithstartdate, DateTime itemstartswithenddate, string noitemstartswithset, DateTime noitemstartswithstartdate, DateTime noitemstartswithenddate, string noletterset, DateTime nolettersetStartdate, DateTime nolettersetEnddate, string letterset, DateTime lettersetstartdate, DateTime lettersetenddate, int customerstartage, int customerEndage, string totals, string resulttype, int excludecancellations)
		{
//	DataSet ds = new DataSet();
            int Return = 0;
            _customer = new DataTable(TN.Customer);
			
			try
			{
				parmArray = new SqlParameter[40];
				
				parmArray[0] = new SqlParameter("@CustomerCodeSet", SqlDbType.NVarChar, 128);
				parmArray[0].Value = CustomerCodeSet;
				
				parmArray[1] = new SqlParameter("@NoCustomerCodeSet", SqlDbType.NVarChar, 128);
				parmArray[1].Value = NoCustomerCodeSet;
				
				parmArray[2] = new SqlParameter("@AccountCodeSet", SqlDbType.NVarChar, 128);
				parmArray[2].Value = AccountCodeSet;
				
				parmArray[3] = new SqlParameter("@NoAccountCodeSet", SqlDbType.NVarChar, 128);
				parmArray[3].Value = NoAccountCodeSet;
				
				parmArray[4] = new SqlParameter("@ArrearsRestriction", SqlDbType.VarChar, 4);
				parmArray[4].Value = ArrearsRestriction;
				
				parmArray[5] = new SqlParameter("@Arrears", SqlDbType.Money);
				parmArray[5].Value = Arrears;
				
				parmArray[6] = new SqlParameter("@maxcurrstatus", SqlDbType.Char, 1);
				parmArray[6].Value = maxcurrstatus;
				
				parmArray[7] = new SqlParameter("@maxeverstatus", SqlDbType.Char, 1);
				parmArray[7].Value = maxeverstatus;
				
				parmArray[8] = new SqlParameter("@branchset", SqlDbType.NVarChar,128);
				parmArray[8].Value = branchset;
				
				parmArray[9] = new SqlParameter("@accttypes", SqlDbType.VarChar, 2);
				parmArray[9].Value = accttypes;
				
				parmArray[10] = new SqlParameter("@itemset", SqlDbType.NVarChar, 128);
				parmArray[10].Value = itemset;
				
				parmArray[11] = new SqlParameter("@itemsetstartdate", SqlDbType.DateTime);
				parmArray[11].Value = itemsetstartdate;
				
				parmArray[12] = new SqlParameter("@itemsetenddate", SqlDbType.DateTime);
				parmArray[12].Value = itemsetenddate;
				
				parmArray[13] = new SqlParameter("@noitemset", SqlDbType.NVarChar, 128);
				parmArray[13].Value = noitemset;
				
				parmArray[14] = new SqlParameter("@noitemsetstartdate", SqlDbType.DateTime);
				parmArray[14].Value = noitemsetstartdate;
				
				parmArray[15] = new SqlParameter("@noitemsetenddate", SqlDbType.DateTime);
				parmArray[15].Value = noitemsetenddate;
				
				parmArray[16] = new SqlParameter("@itemcatset", SqlDbType.NVarChar, 128);
				parmArray[16].Value = itemcatset;
				
				parmArray[17] = new SqlParameter("@itemCatsetstartdate", SqlDbType.DateTime);
				parmArray[17].Value = itemCatsetstartdate;
				
				parmArray[18] = new SqlParameter("@itemCatsetenddate", SqlDbType.DateTime);
				parmArray[18].Value = itemCatsetenddate;
				
				parmArray[19] = new SqlParameter("@noitemCatset", SqlDbType.NVarChar, 128);
				parmArray[19].Value = noitemCatset;
				
				parmArray[20] = new SqlParameter("@noitemCatsetstartdate", SqlDbType.DateTime);
				parmArray[20].Value = noitemCatsetstartdate;
				
				parmArray[21] = new SqlParameter("@noitemCatsetenddate", SqlDbType.DateTime);
				parmArray[21].Value = noitemCatsetenddate;
				
				parmArray[22] = new SqlParameter("@itemsdelivered", SqlDbType.SmallInt);
				parmArray[22].Value = itemsdelivered;
				
				parmArray[23] = new SqlParameter("@itemstartswithset", SqlDbType.NVarChar, 128);
				parmArray[23].Value = itemstartswithset;
				
				parmArray[24] = new SqlParameter("@itemstartswithstartdate", SqlDbType.DateTime);
				parmArray[24].Value = itemstartswithstartdate;
				
				parmArray[25] = new SqlParameter("@itemstartswithenddate", SqlDbType.DateTime);
				parmArray[25].Value = itemstartswithenddate;
				
				parmArray[26] = new SqlParameter("@noitemstartswithset", SqlDbType.NVarChar, 128);
				parmArray[26].Value = noitemstartswithset;
				
				parmArray[27] = new SqlParameter("@noitemstartswithstartdate", SqlDbType.DateTime);
				parmArray[27].Value = noitemstartswithstartdate;
				
				parmArray[28] = new SqlParameter("@noitemstartswithenddate", SqlDbType.DateTime);
				parmArray[28].Value = noitemstartswithenddate;
				
				parmArray[29] = new SqlParameter("@noletterset", SqlDbType.NVarChar, 128);
				parmArray[29].Value = noletterset;
				
				parmArray[30] = new SqlParameter("@nolettersetStartdate", SqlDbType.DateTime);
				parmArray[30].Value = nolettersetStartdate;
				
				parmArray[31] = new SqlParameter("@nolettersetEnddate", SqlDbType.DateTime);
				parmArray[31].Value = nolettersetEnddate;
				
				parmArray[32] = new SqlParameter("@letterset", SqlDbType.NVarChar, 128);
				parmArray[32].Value = letterset;
				
				parmArray[33] = new SqlParameter("@lettersetstartdate", SqlDbType.DateTime);
				parmArray[33].Value = lettersetstartdate;
				
				parmArray[34] = new SqlParameter("@lettersetenddate", SqlDbType.DateTime);
				parmArray[34].Value = lettersetenddate;
				
				parmArray[35] = new SqlParameter("@customerstartage", SqlDbType.SmallInt);
				parmArray[35].Value = customerstartage;
				
				parmArray[36] = new SqlParameter("@customerEndage", SqlDbType.SmallInt);
				parmArray[36].Value = customerEndage;
				
				parmArray[37] = new SqlParameter("@totals", SqlDbType.Char, 1);
				parmArray[37].Value = totals;
				
				parmArray[38] = new SqlParameter("@resulttype", SqlDbType.VarChar, 10);
				parmArray[38].Value = resulttype;
				
				parmArray[39] = new SqlParameter("@excludecancellations", SqlDbType.SmallInt);
				parmArray[39].Value = excludecancellations;


                this.RunSP("dn_CustomerMailing", parmArray, _customer);
	
				
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			
			return Return;
		}

		/// <summary>
		/// CustomerMailingQuerySave
		/// </summary>
		/// <param name="EmpeenoSave">int</param>
		/// <param name="datesaved">DateTime</param>
		/// <param name="QueryName">string</param>
		/// <param name="CustomerCodeSet">string</param>
		/// <param name="NoCustomerCodeSet">string</param>
		/// <param name="AccountCodeSet">string</param>
		/// <param name="NoAccountCodeSet">string</param>
		/// <param name="ArrearsRestriction">string</param>
		/// <param name="Arrears">double</param>
		/// <param name="maxcurrstatus">string</param>
		/// <param name="maxeverstatus">string</param>
		/// <param name="branchset">string</param>
		/// <param name="accttypes">string</param>
		/// <param name="itemset">string</param>
		/// <param name="itemsetstartdate">DateTime</param>
		/// <param name="itemsetenddate">DateTime</param>
		/// <param name="noitemset">string</param>
		/// <param name="noitemsetstartdate">DateTime</param>
		/// <param name="noitemsetenddate">DateTime</param>
		/// <param name="itemcatset">string</param>
		/// <param name="itemCatsetstartdate">DateTime</param>
		/// <param name="itemCatsetenddate">DateTime</param>
		/// <param name="noitemCatset">string</param>
		/// <param name="noitemCatsetstartdate">DateTime</param>
		/// <param name="noitemCatsetenddate">DateTime</param>
		/// <param name="itemsdelivered">int</param>
		/// <param name="itemstartswithset">string</param>
		/// <param name="itemstartswithstartdate">DateTime</param>
		/// <param name="itemstartswithenddate">DateTime</param>
		/// <param name="noitemstartswithset">string</param>
		/// <param name="noitemstartswithstartdate">DateTime</param>
		/// <param name="noitemstartswithenddate">DateTime</param>
		/// <param name="noletterset">string</param>
		/// <param name="nolettersetStartdate">DateTime</param>
		/// <param name="nolettersetEnddate">DateTime</param>
		/// <param name="letterset">string</param>
		/// <param name="lettersetstartdate">DateTime</param>
		/// <param name="lettersetenddate">DateTime</param>
		/// <param name="customerstartage">int</param>
		/// <param name="customerEndage">int</param>
		/// <param name="totals">string</param>
		/// <param name="resulttype">string</param>
		/// <param name="excludecancellations">int</param>
		/// <returns>DataSet</returns>
		/// 
		public DataSet CustomerMailingQuerySave (int EmpeenoSave, DateTime datesaved, string QueryName, string CustomerCodeSet, string NoCustomerCodeSet, string AccountCodeSet, string NoAccountCodeSet, string ArrearsRestriction, double Arrears, string maxcurrstatus, string maxeverstatus, string branchset, string accttypes, string itemset, DateTime itemsetstartdate, DateTime itemsetenddate, string noitemset, DateTime noitemsetstartdate, DateTime noitemsetenddate, string itemcatset, DateTime itemCatsetstartdate, DateTime itemCatsetenddate, string noitemCatset, DateTime noitemCatsetstartdate, DateTime noitemCatsetenddate, int itemsdelivered, string itemstartswithset, DateTime itemstartswithstartdate, DateTime itemstartswithenddate, string noitemstartswithset, DateTime noitemstartswithstartdate, DateTime noitemstartswithenddate, string noletterset, DateTime nolettersetStartdate, DateTime nolettersetEnddate, string letterset, DateTime lettersetstartdate, DateTime lettersetenddate, int customerstartage, int customerEndage, string totals, string resulttype, int excludecancellations)
		{
			DataSet ds = new DataSet();
						
			try
			{
				parmArray = new SqlParameter[43];
				
				parmArray[0] = new SqlParameter("@EmpeenoSave", SqlDbType.Int);
				parmArray[0].Value = EmpeenoSave;
				
				parmArray[1] = new SqlParameter("@datesaved", SqlDbType.DateTime);
				parmArray[1].Value = datesaved;
				
				parmArray[2] = new SqlParameter("@QueryName", SqlDbType.NVarChar, 256);
				parmArray[2].Value = QueryName;
				
				parmArray[3] = new SqlParameter("@CustomerCodeSet", SqlDbType.NVarChar, 128);
				parmArray[3].Value = CustomerCodeSet;
				
				parmArray[4] = new SqlParameter("@NoCustomerCodeSet", SqlDbType.NVarChar, 128);
				parmArray[4].Value = NoCustomerCodeSet;
				
				parmArray[5] = new SqlParameter("@AccountCodeSet", SqlDbType.NVarChar, 128);
				parmArray[5].Value = AccountCodeSet;
				
				parmArray[6] = new SqlParameter("@NoAccountCodeSet", SqlDbType.NVarChar, 128);
				parmArray[6].Value = NoAccountCodeSet;
				
				parmArray[7] = new SqlParameter("@ArrearsRestriction", SqlDbType.VarChar, 4);
				parmArray[7].Value = ArrearsRestriction;
				
				parmArray[8] = new SqlParameter("@Arrears", SqlDbType.Money);
				parmArray[8].Value = Arrears;
				
				parmArray[9] = new SqlParameter("@maxcurrstatus", SqlDbType.Char, 1);
				parmArray[9].Value = maxcurrstatus;
				
				parmArray[10] = new SqlParameter("@maxeverstatus", SqlDbType.Char, 1);
				parmArray[10].Value = maxeverstatus;
				
				parmArray[11] = new SqlParameter("@branchset", SqlDbType.NVarChar,128);
				parmArray[11].Value = branchset;
				
				parmArray[12] = new SqlParameter("@accttypes", SqlDbType.VarChar, 2);
				parmArray[12].Value = accttypes;
				
				parmArray[13] = new SqlParameter("@itemset", SqlDbType.NVarChar, 128);
				parmArray[13].Value = itemset;
				
				parmArray[14] = new SqlParameter("@itemsetstartdate", SqlDbType.DateTime);
				parmArray[14].Value = itemsetstartdate;
				
				parmArray[15] = new SqlParameter("@itemsetenddate", SqlDbType.DateTime);
				parmArray[15].Value = itemsetenddate;
				
				parmArray[16] = new SqlParameter("@noitemset", SqlDbType.NVarChar, 128);
				parmArray[16].Value = noitemset;
				
				parmArray[17] = new SqlParameter("@noitemsetstartdate", SqlDbType.DateTime);
				parmArray[17].Value = noitemsetstartdate;
				
				parmArray[18] = new SqlParameter("@noitemsetenddate", SqlDbType.DateTime);
				parmArray[18].Value = noitemsetenddate;
				
				parmArray[19] = new SqlParameter("@itemcatset", SqlDbType.NVarChar, 128);
				parmArray[19].Value = itemcatset;
				
				parmArray[20] = new SqlParameter("@itemCatsetstartdate", SqlDbType.DateTime);
				parmArray[20].Value = itemCatsetstartdate;
				
				parmArray[21] = new SqlParameter("@itemCatsetenddate", SqlDbType.DateTime);
				parmArray[21].Value = itemCatsetenddate;
				
				parmArray[22] = new SqlParameter("@noitemCatset", SqlDbType.NVarChar, 128);
				parmArray[22].Value = noitemCatset;
				
				parmArray[23] = new SqlParameter("@noitemCatsetstartdate", SqlDbType.DateTime);
				parmArray[23].Value = noitemCatsetstartdate;
				
				parmArray[24] = new SqlParameter("@noitemCatsetenddate", SqlDbType.DateTime);
				parmArray[24].Value = noitemCatsetenddate;
				
				parmArray[25] = new SqlParameter("@itemsdelivered", SqlDbType.SmallInt);
				parmArray[25].Value = itemsdelivered;
				
				parmArray[26] = new SqlParameter("@itemstartswithset", SqlDbType.NVarChar, 128);
				parmArray[26].Value = itemstartswithset;
				
				parmArray[27] = new SqlParameter("@itemstartswithstartdate", SqlDbType.DateTime);
				parmArray[27].Value = itemstartswithstartdate;
				
				parmArray[28] = new SqlParameter("@itemstartswithenddate", SqlDbType.DateTime);
				parmArray[28].Value = itemstartswithenddate;
				
				parmArray[29] = new SqlParameter("@noitemstartswithset", SqlDbType.NVarChar, 128);
				parmArray[29].Value = noitemstartswithset;
				
				parmArray[30] = new SqlParameter("@noitemstartswithstartdate", SqlDbType.DateTime);
				parmArray[30].Value = noitemstartswithstartdate;
				
				parmArray[31] = new SqlParameter("@noitemstartswithenddate", SqlDbType.DateTime);
				parmArray[31].Value = noitemstartswithenddate;
				
				parmArray[32] = new SqlParameter("@noletterset", SqlDbType.NVarChar, 128);
				parmArray[32].Value = noletterset;
				
				parmArray[33] = new SqlParameter("@nolettersetStartdate", SqlDbType.DateTime);
				parmArray[33].Value = nolettersetStartdate;
				
				parmArray[34] = new SqlParameter("@nolettersetEnddate", SqlDbType.DateTime);
				parmArray[34].Value = nolettersetEnddate;
				
				parmArray[35] = new SqlParameter("@letterset", SqlDbType.NVarChar, 128);
				parmArray[35].Value = letterset;
				
				parmArray[36] = new SqlParameter("@lettersetstartdate", SqlDbType.DateTime);
				parmArray[36].Value = lettersetstartdate;
				
				parmArray[37] = new SqlParameter("@lettersetenddate", SqlDbType.DateTime);
				parmArray[37].Value = lettersetenddate;
				
				parmArray[38] = new SqlParameter("@customerstartage", SqlDbType.SmallInt);
				parmArray[38].Value = customerstartage;
				
				parmArray[39] = new SqlParameter("@customerEndage", SqlDbType.SmallInt);
				parmArray[39].Value = customerEndage;
				
				parmArray[40] = new SqlParameter("@totals", SqlDbType.Char, 1);
				parmArray[40].Value = totals;
				
				parmArray[41] = new SqlParameter("@resulttype", SqlDbType.VarChar, 10);
				parmArray[41].Value = resulttype;
				
				parmArray[42] = new SqlParameter("@excludecancellations", SqlDbType.SmallInt);
				parmArray[42].Value = excludecancellations;
				 
				
				this.RunSP("dn_CustomerMailingQuerySave", parmArray, ds);
	
				
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			
			return ds;
		}

		/// <summary>
		/// QueryLoadbyEmpeeno
		/// </summary>
		/// <param name="EmpeenoSave">int</param>
		/// <returns>DataSet</returns>
		/// 
		public DataSet QueryLoadbyEmpeeno (int EmpeenoSave)
		{
			DataSet ds = new DataSet();
			
			
			try
			{
				parmArray = new SqlParameter[1];
				
				parmArray[0] = new SqlParameter("@EmpeenoSave", SqlDbType.Int);
				parmArray[0].Value = EmpeenoSave;
				 
				
				this.RunSP("dn_CustomerMailingQueryLoadbyEmpeeno", parmArray, ds);
	
				
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			
			return ds;
		}

		/// <summary>
		/// QueryGet
		/// </summary>
		/// <param name="EmpeenoSave">int</param>
		/// <param name="datesaved">DateTime</param>
		/// <param name="QueryName">string</param>
		/// <param name="CustomerCodeSet">string</param>
		/// <param name="NoCustomerCodeSet">string</param>
		/// <param name="AccountCodeSet">string</param>
		/// <param name="NoAccountCodeSet">string</param>
		/// <param name="ArrearsRestriction">string</param>
		/// <param name="Arrears">double</param>
		/// <param name="maxcurrstatus">string</param>
		/// <param name="maxeverstatus">string</param>
		/// <param name="branchset">string</param>
		/// <param name="accttypes">string</param>
		/// <param name="itemset">string</param>
		/// <param name="itemsetstartdate">DateTime</param>
		/// <param name="itemsetenddate">DateTime</param>
		/// <param name="noitemset">string</param>
		/// <param name="noitemsetstartdate">DateTime</param>
		/// <param name="noitemsetenddate">DateTime</param>
		/// <param name="itemcatset">string</param>
		/// <param name="itemCatsetstartdate">DateTime</param>
		/// <param name="itemCatsetenddate">DateTime</param>
		/// <param name="noitemCatset">string</param>
		/// <param name="noitemCatsetstartdate">DateTime</param>
		/// <param name="noitemCatsetenddate">DateTime</param>
		/// <param name="itemsdelivered">int</param>
		/// <param name="itemstartswithset">string</param>
		/// <param name="itemstartswithstartdate">DateTime</param>
		/// <param name="itemstartswithenddate">DateTime</param>
		/// <param name="noitemstartswithset">string</param>
		/// <param name="noitemstartswithstartdate">DateTime</param>
		/// <param name="noitemstartswithenddate">DateTime</param>
		/// <param name="noletterset">string</param>
		/// <param name="nolettersetStartdate">DateTime</param>
		/// <param name="nolettersetEnddate">DateTime</param>
		/// <param name="letterset">string</param>
		/// <param name="lettersetstartdate">DateTime</param>
		/// <param name="lettersetenddate">DateTime</param>
		/// <param name="customerstartage">int</param>
		/// <param name="customerEndage">int</param>
		/// <param name="totals">string</param>
		/// <param name="resulttype">string</param>
		/// <param name="excludecancellations">int</param>
		/// <returns>int</returns>
		/// 
		public int QueryGet (int EmpeenoSave, out DateTime datesaved, string QueryName, out string CustomerCodeSet, out string NoCustomerCodeSet, out string AccountCodeSet, out string NoAccountCodeSet, out string ArrearsRestriction, out double Arrears, out string maxcurrstatus, out string maxeverstatus, out string branchset, out string accttypes, out string itemset, out DateTime itemsetstartdate, out DateTime itemsetenddate, out string noitemset, out DateTime noitemsetstartdate, out DateTime noitemsetenddate, out string itemcatset, out DateTime itemCatsetstartdate, out DateTime itemCatsetenddate, out string noitemCatset, out DateTime noitemCatsetstartdate, out DateTime noitemCatsetenddate, out int itemsdelivered, out string itemstartswithset, out DateTime itemstartswithstartdate, out DateTime itemstartswithenddate, out string noitemstartswithset, out DateTime noitemstartswithstartdate, out DateTime noitemstartswithenddate, out string noletterset, out DateTime nolettersetStartdate, out DateTime nolettersetEnddate, out string letterset, out DateTime lettersetstartdate, out DateTime lettersetenddate, out int customerstartage, out int customerEndage, out string totals, out string resulttype, out int excludecancellations)
		{
			int status = 0;
			
			datesaved = new DateTime();
			
			CustomerCodeSet = "";
			NoCustomerCodeSet = "";
			AccountCodeSet = "";
			NoAccountCodeSet = "";
			ArrearsRestriction = "";
			Arrears = new double();
			maxcurrstatus = "";
			maxeverstatus = "";
			branchset = "";
			accttypes = "";
			itemset = "";
			itemsetstartdate = new DateTime();
			itemsetenddate = new DateTime();
			noitemset = "";
			noitemsetstartdate = new DateTime();
			noitemsetenddate = new DateTime();
			itemcatset = "";
			itemCatsetstartdate = new DateTime();
			itemCatsetenddate = new DateTime();
			noitemCatset = "";
			noitemCatsetstartdate = new DateTime();
			noitemCatsetenddate = new DateTime();
			itemsdelivered = 0;
			itemstartswithset = "";
			itemstartswithstartdate = new DateTime();
			itemstartswithenddate = new DateTime();
			noitemstartswithset = "";
			noitemstartswithstartdate = new DateTime();
			noitemstartswithenddate = new DateTime();
			noletterset = "";
			nolettersetStartdate = new DateTime();
			nolettersetEnddate = new DateTime();
			letterset = "";
			lettersetstartdate = new DateTime();
			lettersetenddate = new DateTime();
			customerstartage = 0;
			customerEndage = 0;
			totals = "";
			resulttype = "";
			excludecancellations = 0;
			
			try
			{
				parmArray = new SqlParameter[43];
				
				parmArray[0] = new SqlParameter("@EmpeenoSave", SqlDbType.Int);
				parmArray[0].Value = EmpeenoSave;
				
				parmArray[1] = new SqlParameter("@datesaved", SqlDbType.DateTime);
				parmArray[1].Value = datesaved;
				parmArray[1].Direction = ParameterDirection.Output;
				parmArray[2] = new SqlParameter("@QueryName", SqlDbType.NVarChar, 256);
				parmArray[2].Value = QueryName;
				
				parmArray[3] = new SqlParameter("@CustomerCodeSet", SqlDbType.NVarChar, 128);
				parmArray[3].Value = CustomerCodeSet;
				parmArray[3].Direction = ParameterDirection.Output;
				parmArray[4] = new SqlParameter("@NoCustomerCodeSet", SqlDbType.NVarChar, 128);
				parmArray[4].Value = NoCustomerCodeSet;
				parmArray[4].Direction = ParameterDirection.Output;
				parmArray[5] = new SqlParameter("@AccountCodeSet", SqlDbType.NVarChar, 128);
				parmArray[5].Value = AccountCodeSet;
				parmArray[5].Direction = ParameterDirection.Output;
				parmArray[6] = new SqlParameter("@NoAccountCodeSet", SqlDbType.NVarChar, 128);
				parmArray[6].Value = NoAccountCodeSet;
				parmArray[6].Direction = ParameterDirection.Output;
				parmArray[7] = new SqlParameter("@ArrearsRestriction", SqlDbType.VarChar, 4);
				parmArray[7].Value = ArrearsRestriction;
				parmArray[7].Direction = ParameterDirection.Output;
				parmArray[8] = new SqlParameter("@Arrears", SqlDbType.Money);
				parmArray[8].Value = Arrears;
				parmArray[8].Direction = ParameterDirection.Output;
				parmArray[9] = new SqlParameter("@maxcurrstatus", SqlDbType.Char, 1);
				parmArray[9].Value = maxcurrstatus;
				parmArray[9].Direction = ParameterDirection.Output;
				parmArray[10] = new SqlParameter("@maxeverstatus", SqlDbType.Char, 1);
				parmArray[10].Value = maxeverstatus;
				parmArray[10].Direction = ParameterDirection.Output;
				parmArray[11] = new SqlParameter("@branchset", SqlDbType.NVarChar,128 );
				parmArray[11].Value = branchset;
				parmArray[11].Direction = ParameterDirection.Output;
				parmArray[12] = new SqlParameter("@accttypes", SqlDbType.VarChar, 2);
				parmArray[12].Value = accttypes;
				parmArray[12].Direction = ParameterDirection.Output;
				parmArray[13] = new SqlParameter("@itemset", SqlDbType.NVarChar, 128);
				parmArray[13].Value = itemset;
				parmArray[13].Direction = ParameterDirection.Output;
				parmArray[14] = new SqlParameter("@itemsetstartdate", SqlDbType.DateTime);
				parmArray[14].Value = itemsetstartdate;
				parmArray[14].Direction = ParameterDirection.Output;
				parmArray[15] = new SqlParameter("@itemsetenddate", SqlDbType.DateTime);
				parmArray[15].Value = itemsetenddate;
				parmArray[15].Direction = ParameterDirection.Output;
				parmArray[16] = new SqlParameter("@noitemset", SqlDbType.NVarChar, 128);
				parmArray[16].Value = noitemset;
				parmArray[16].Direction = ParameterDirection.Output;
				parmArray[17] = new SqlParameter("@noitemsetstartdate", SqlDbType.DateTime);
				parmArray[17].Value = noitemsetstartdate;
				parmArray[17].Direction = ParameterDirection.Output;
				parmArray[18] = new SqlParameter("@noitemsetenddate", SqlDbType.DateTime);
				parmArray[18].Value = noitemsetenddate;
				parmArray[18].Direction = ParameterDirection.Output;
				parmArray[19] = new SqlParameter("@itemcatset", SqlDbType.NVarChar, 128);
				parmArray[19].Value = itemcatset;
				parmArray[19].Direction = ParameterDirection.Output;
				parmArray[20] = new SqlParameter("@itemCatsetstartdate", SqlDbType.DateTime);
				parmArray[20].Value = itemCatsetstartdate;
				parmArray[20].Direction = ParameterDirection.Output;
				parmArray[21] = new SqlParameter("@itemCatsetenddate", SqlDbType.DateTime);
				parmArray[21].Value = itemCatsetenddate;
				parmArray[21].Direction = ParameterDirection.Output;
				parmArray[22] = new SqlParameter("@noitemCatset", SqlDbType.NVarChar, 128);
				parmArray[22].Value = noitemCatset;
				parmArray[22].Direction = ParameterDirection.Output;
				parmArray[23] = new SqlParameter("@noitemCatsetstartdate", SqlDbType.DateTime);
				parmArray[23].Value = noitemCatsetstartdate;
				parmArray[23].Direction = ParameterDirection.Output;
				parmArray[24] = new SqlParameter("@noitemCatsetenddate", SqlDbType.DateTime);
				parmArray[24].Value = noitemCatsetenddate;
				parmArray[24].Direction = ParameterDirection.Output;
				parmArray[25] = new SqlParameter("@itemsdelivered", SqlDbType.SmallInt);
				parmArray[25].Value = itemsdelivered;
				parmArray[25].Direction = ParameterDirection.Output;
				parmArray[26] = new SqlParameter("@itemstartswithset", SqlDbType.NVarChar, 128);
				parmArray[26].Value = itemstartswithset;
				parmArray[26].Direction = ParameterDirection.Output;
				parmArray[27] = new SqlParameter("@itemstartswithstartdate", SqlDbType.DateTime);
				parmArray[27].Value = itemstartswithstartdate;
				parmArray[27].Direction = ParameterDirection.Output;
				parmArray[28] = new SqlParameter("@itemstartswithenddate", SqlDbType.DateTime);
				parmArray[28].Value = itemstartswithenddate;
				parmArray[28].Direction = ParameterDirection.Output;
				parmArray[29] = new SqlParameter("@noitemstartswithset", SqlDbType.NVarChar, 128);
				parmArray[29].Value = noitemstartswithset;
				parmArray[29].Direction = ParameterDirection.Output;
				parmArray[30] = new SqlParameter("@noitemstartswithstartdate", SqlDbType.DateTime);
				parmArray[30].Value = noitemstartswithstartdate;
				parmArray[30].Direction = ParameterDirection.Output;
				parmArray[31] = new SqlParameter("@noitemstartswithenddate", SqlDbType.DateTime);
				parmArray[31].Value = noitemstartswithenddate;
				parmArray[31].Direction = ParameterDirection.Output;
				parmArray[32] = new SqlParameter("@noletterset", SqlDbType.NVarChar, 128);
				parmArray[32].Value = noletterset;
				parmArray[32].Direction = ParameterDirection.Output;
				parmArray[33] = new SqlParameter("@nolettersetStartdate", SqlDbType.DateTime);
				parmArray[33].Value = nolettersetStartdate;
				parmArray[33].Direction = ParameterDirection.Output;
				parmArray[34] = new SqlParameter("@nolettersetEnddate", SqlDbType.DateTime);
				parmArray[34].Value = nolettersetEnddate;
				parmArray[34].Direction = ParameterDirection.Output;
				parmArray[35] = new SqlParameter("@letterset", SqlDbType.NVarChar, 128);
				parmArray[35].Value = letterset;
				parmArray[35].Direction = ParameterDirection.Output;
				parmArray[36] = new SqlParameter("@lettersetstartdate", SqlDbType.DateTime);
				parmArray[36].Value = lettersetstartdate;
				parmArray[36].Direction = ParameterDirection.Output;
				parmArray[37] = new SqlParameter("@lettersetenddate", SqlDbType.DateTime);
				parmArray[37].Value = lettersetenddate;
				parmArray[37].Direction = ParameterDirection.Output;
				parmArray[38] = new SqlParameter("@customerstartage", SqlDbType.SmallInt);
				parmArray[38].Value = customerstartage;
				parmArray[38].Direction = ParameterDirection.Output;
				parmArray[39] = new SqlParameter("@customerEndage", SqlDbType.SmallInt);
				parmArray[39].Value = customerEndage;
				parmArray[39].Direction = ParameterDirection.Output;
				parmArray[40] = new SqlParameter("@totals", SqlDbType.Char, 1);
				parmArray[40].Value = totals;
				parmArray[40].Direction = ParameterDirection.Output;
				parmArray[41] = new SqlParameter("@resulttype", SqlDbType.VarChar, 10);
				parmArray[41].Value = resulttype;
				parmArray[41].Direction = ParameterDirection.Output;
				parmArray[42] = new SqlParameter("@excludecancellations", SqlDbType.SmallInt);
				parmArray[42].Value = excludecancellations;
				parmArray[42].Direction = ParameterDirection.Output; 
				status = 
				this.RunSP("dn_CustomerMailingQueryGet", parmArray);
	
				if(parmArray[1].Value!=DBNull.Value)
					datesaved = (DateTime)parmArray[1].Value;
				if(parmArray[3].Value!=DBNull.Value)
					CustomerCodeSet = (string)parmArray[3].Value;
				if(parmArray[4].Value!=DBNull.Value)
					NoCustomerCodeSet = (string)parmArray[4].Value;
				if(parmArray[5].Value!=DBNull.Value)
					AccountCodeSet = (string)parmArray[5].Value;
				if(parmArray[6].Value!=DBNull.Value)
					NoAccountCodeSet = (string)parmArray[6].Value;
				if(parmArray[7].Value!=DBNull.Value)
					ArrearsRestriction = (string)parmArray[7].Value;
				if(parmArray[8].Value!=DBNull.Value)
                    Arrears = Convert.ToDouble(parmArray[8].Value);
					//Arrears = (double)parmArray[8].Value;
				if(parmArray[9].Value!=DBNull.Value)
					maxcurrstatus = (string)parmArray[9].Value;
				if(parmArray[10].Value!=DBNull.Value)
					maxeverstatus = (string)parmArray[10].Value;
				if(parmArray[11].Value!=DBNull.Value)
					branchset = (string)parmArray[11].Value;
				if(parmArray[12].Value!=DBNull.Value)
					accttypes = (string)parmArray[12].Value;
				if(parmArray[13].Value!=DBNull.Value)
					itemset = (string)parmArray[13].Value;
				if(parmArray[14].Value!=DBNull.Value)
					itemsetstartdate = (DateTime)parmArray[14].Value;
				if(parmArray[15].Value!=DBNull.Value)
					itemsetenddate = (DateTime)parmArray[15].Value;
				if(parmArray[16].Value!=DBNull.Value)
					noitemset = (string)parmArray[16].Value;
				if(parmArray[17].Value!=DBNull.Value)
					noitemsetstartdate = (DateTime)parmArray[17].Value;
				if(parmArray[18].Value!=DBNull.Value)
					noitemsetenddate = (DateTime)parmArray[18].Value;
				if(parmArray[19].Value!=DBNull.Value)
					itemcatset = (string)parmArray[19].Value;
				if(parmArray[20].Value!=DBNull.Value)
					itemCatsetstartdate = (DateTime)parmArray[20].Value;
				if(parmArray[21].Value!=DBNull.Value)
					itemCatsetenddate = (DateTime)parmArray[21].Value;
				if(parmArray[22].Value!=DBNull.Value)
					noitemCatset = (string)parmArray[22].Value;
				if(parmArray[23].Value!=DBNull.Value)
					noitemCatsetstartdate = (DateTime)parmArray[23].Value;
				if(parmArray[24].Value!=DBNull.Value)
					noitemCatsetenddate = (DateTime)parmArray[24].Value;
				if(parmArray[25].Value!=DBNull.Value)
					itemsdelivered = Convert.ToInt32(parmArray[25].Value);
				if(parmArray[26].Value!=DBNull.Value)
					itemstartswithset = (string)parmArray[26].Value;
				if(parmArray[27].Value!=DBNull.Value)
					itemstartswithstartdate = (DateTime)parmArray[27].Value;
				if(parmArray[28].Value!=DBNull.Value)
					itemstartswithenddate = (DateTime)parmArray[28].Value;
				if(parmArray[29].Value!=DBNull.Value)
					noitemstartswithset = (string)parmArray[29].Value;
				if(parmArray[30].Value!=DBNull.Value)
					noitemstartswithstartdate = (DateTime)parmArray[30].Value;
				if(parmArray[31].Value!=DBNull.Value)
					noitemstartswithenddate = (DateTime)parmArray[31].Value;
				if(parmArray[32].Value!=DBNull.Value)
					noletterset = (string)parmArray[32].Value;
				if(parmArray[33].Value!=DBNull.Value)
					nolettersetStartdate = (DateTime)parmArray[33].Value;
				if(parmArray[34].Value!=DBNull.Value)
					nolettersetEnddate = (DateTime)parmArray[34].Value;
				if(parmArray[35].Value!=DBNull.Value)
					letterset = (string)parmArray[35].Value;
				if(parmArray[36].Value!=DBNull.Value)
					lettersetstartdate = (DateTime)parmArray[36].Value;
				if(parmArray[37].Value!=DBNull.Value)
					lettersetenddate = (DateTime)parmArray[37].Value;
				if(parmArray[38].Value!=DBNull.Value)
					customerstartage = Convert.ToInt32(parmArray[38].Value);
				if(parmArray[39].Value!=DBNull.Value)
					customerEndage = Convert.ToInt32(parmArray[39].Value);
				if(parmArray[40].Value!=DBNull.Value)
					totals = (string)parmArray[40].Value;
				if(parmArray[41].Value!=DBNull.Value)
					resulttype = (string)parmArray[41].Value;
				if(parmArray[42].Value!=DBNull.Value)
					excludecancellations = Convert.ToInt32(parmArray[42].Value);
				
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			
			return status;
		}
	}
}