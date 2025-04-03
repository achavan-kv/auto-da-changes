using System;
using System.Data;
using STL.DAL;
using STL.Common;
using STL.Common.Static;
using STL.Common.Constants.ColumnNames;


namespace STL.BLL
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	public class BCustomerMailing : CommonObject
	{
		public BCustomerMailing()
		{
			//
			// TODO: Add constructor logic here
			//
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
        public DataSet CustomerMailingQuery(string CustomerCodeSet, string NoCustomerCodeSet, string AccountCodeSet, string NoAccountCodeSet, string ArrearsRestriction, double Arrears, string maxcurrstatus, string maxeverstatus, string branchset, string accttypes, string itemset, DateTime itemsetstartdate, DateTime itemsetenddate, string noitemset, DateTime noitemsetstartdate, DateTime noitemsetenddate, string itemcatset, DateTime itemCatsetstartdate, DateTime itemCatsetenddate, string noitemCatset, DateTime noitemCatsetstartdate, DateTime noitemCatsetenddate, int itemsdelivered, string itemstartswithset, DateTime itemstartswithstartdate, DateTime itemstartswithenddate, string noitemstartswithset, DateTime noitemstartswithstartdate, DateTime noitemstartswithenddate, string noletterset, DateTime nolettersetStartdate, DateTime nolettersetEnddate, string letterset, DateTime lettersetstartdate, DateTime lettersetenddate, int customerstartage, int customerEndage, string totals, string resulttype, int excludecancellations)
		{
			
            DataSet ds = null;
            
            int Return =0;
			DCustomerMailing da = new DCustomerMailing();
			Return = da.CustomerMailingQuery(CustomerCodeSet, NoCustomerCodeSet, AccountCodeSet, NoAccountCodeSet, ArrearsRestriction, Arrears, maxcurrstatus, maxeverstatus, branchset, accttypes, itemset, itemsetstartdate, itemsetenddate, noitemset, noitemsetstartdate, noitemsetenddate, itemcatset, itemCatsetstartdate, itemCatsetenddate, noitemCatset, noitemCatsetstartdate, noitemCatsetenddate, itemsdelivered, itemstartswithset, itemstartswithstartdate, itemstartswithenddate, noitemstartswithset, noitemstartswithstartdate, noitemstartswithenddate, noletterset, nolettersetStartdate, nolettersetEnddate, letterset, lettersetstartdate, lettersetenddate, customerstartage, customerEndage, totals, resulttype, excludecancellations);
            ds = new DataSet();
            ds.Tables.Add(da.Customer);

            
            return ds;
		}

		/// <summary>
		/// BCustomerMailingQuerySave
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
        public DataSet BCustomerMailingQuerySave(int EmpeenoSave, DateTime datesaved, string QueryName, string CustomerCodeSet, string NoCustomerCodeSet, string AccountCodeSet, string NoAccountCodeSet, string ArrearsRestriction, double Arrears, string maxcurrstatus, string maxeverstatus, string branchset, string accttypes, string itemset, DateTime itemsetstartdate, DateTime itemsetenddate, string noitemset, DateTime noitemsetstartdate, DateTime noitemsetenddate, string itemcatset, DateTime itemCatsetstartdate, DateTime itemCatsetenddate, string noitemCatset, DateTime noitemCatsetstartdate, DateTime noitemCatsetenddate, int itemsdelivered, string itemstartswithset, DateTime itemstartswithstartdate, DateTime itemstartswithenddate, string noitemstartswithset, DateTime noitemstartswithstartdate, DateTime noitemstartswithenddate, string noletterset, DateTime nolettersetStartdate, DateTime nolettersetEnddate, string letterset, DateTime lettersetstartdate, DateTime lettersetenddate, int customerstartage, int customerEndage, string totals, string resulttype, int excludecancellations)
		{
			DataSet ds = new DataSet(); 
			DCustomerMailing da = new DCustomerMailing();
			ds = da.CustomerMailingQuerySave(EmpeenoSave, datesaved, QueryName, CustomerCodeSet, NoCustomerCodeSet, AccountCodeSet, NoAccountCodeSet, ArrearsRestriction, Arrears, maxcurrstatus, maxeverstatus, branchset, accttypes, itemset, itemsetstartdate, itemsetenddate, noitemset, noitemsetstartdate, noitemsetenddate, itemcatset, itemCatsetstartdate, itemCatsetenddate, noitemCatset, noitemCatsetstartdate, noitemCatsetenddate, itemsdelivered, itemstartswithset, itemstartswithstartdate, itemstartswithenddate, noitemstartswithset, noitemstartswithstartdate, noitemstartswithenddate, noletterset, nolettersetStartdate, nolettersetEnddate, letterset, lettersetstartdate, lettersetenddate, customerstartage, customerEndage, totals, resulttype, excludecancellations);
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
			DCustomerMailing da = new DCustomerMailing();
			ds = da.QueryLoadbyEmpeeno(EmpeenoSave);
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
        public int QueryGet(int EmpeenoSave, out DateTime datesaved, string QueryName, out string CustomerCodeSet, out string NoCustomerCodeSet, out string AccountCodeSet, out string NoAccountCodeSet, out string ArrearsRestriction, out double Arrears, out string maxcurrstatus, out string maxeverstatus, out string branchset, out string accttypes, out string itemset, out DateTime itemsetstartdate, out DateTime itemsetenddate, out string noitemset, out DateTime noitemsetstartdate, out DateTime noitemsetenddate, out string itemcatset, out DateTime itemCatsetstartdate, out DateTime itemCatsetenddate, out string noitemCatset, out DateTime noitemCatsetstartdate, out DateTime noitemCatsetenddate, out int itemsdelivered, out string itemstartswithset, out DateTime itemstartswithstartdate, out DateTime itemstartswithenddate, out string noitemstartswithset, out DateTime noitemstartswithstartdate, out DateTime noitemstartswithenddate, out string noletterset, out DateTime nolettersetStartdate, out DateTime nolettersetEnddate, out string letterset, out DateTime lettersetstartdate, out DateTime lettersetenddate, out int customerstartage, out int customerEndage, out string totals, out string resulttype, out int excludecancellations)
		{
			int status = 0; 
			DCustomerMailing da = new DCustomerMailing();
			status = da.QueryGet(EmpeenoSave, out datesaved, QueryName, out CustomerCodeSet, out NoCustomerCodeSet, out AccountCodeSet, out NoAccountCodeSet, out ArrearsRestriction, out Arrears, out maxcurrstatus, out maxeverstatus, out branchset, out accttypes, out itemset, out itemsetstartdate, out itemsetenddate, out noitemset, out noitemsetstartdate, out noitemsetenddate, out itemcatset, out itemCatsetstartdate, out itemCatsetenddate, out noitemCatset, out noitemCatsetstartdate, out noitemCatsetenddate, out itemsdelivered, out itemstartswithset, out itemstartswithstartdate, out itemstartswithenddate, out noitemstartswithset, out noitemstartswithstartdate, out noitemstartswithenddate, out noletterset, out nolettersetStartdate, out nolettersetEnddate, out letterset, out lettersetstartdate, out lettersetenddate, out customerstartage, out customerEndage, out totals, out resulttype, out excludecancellations);
			
			return status;
		}
	}
}