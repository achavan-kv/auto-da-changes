using System;
using STL.Common;
using STL.Common.Printing.CustomerCard;
using STL.DAL;
using System.Data;
using System.Data.SqlClient;
using STL.Common.Constants.TableNames;
using STL.Common.Constants.ColumnNames;
using System.Collections.Generic;  //IP - 25/02/10 - CR1072 - Malaysia 3PL for Version 5.2
using Blue.Cosacs.Repositories; //IP - 7/12/10 - Store Card
using Blue.Cosacs.Shared; //IP - 7/12/10 - Store Card



namespace STL.BLL
{
	/// <summary>
	/// Summary description for BBranch.
	/// </summary>
	public class BBranch : CommonObject
	{
		public DataSet GetBranchAddress(int branchNo, int updhissn)
		{
			DataSet ds = null;
			DBranch branch = new DBranch();
			if(0 == branch.GetBranchAddress(branchNo, updhissn))
			{
				ds = new DataSet();
                // 25/04/08 rdb added telno
				DataTable details = new DataTable("BranchDetails");
				details.Columns.AddRange(new DataColumn[] {	new DataColumn("BranchName"),
															new DataColumn("BranchAddr1"),
															new DataColumn("BranchAddr2"),
															new DataColumn("BranchAddr3"),
															new DataColumn("Hissn"),
															new DataColumn("BuffNo"),
                                                            new DataColumn("TelNo")     //IP - 11/05/10 - UAT(135) UAT5.2.1.0 log - Merged from 4.3
														  });
            
				DataRow row = details.NewRow();
				row["BranchName"] = branch.BranchName;
				row["BranchAddr1"] = branch.BranchAddr1;
				row["BranchAddr2"] = branch.BranchAddr2;
				row["BranchAddr3"] = branch.BranchAddr3;
				row["Hissn"] = branch.Hissn;
				row["BuffNo"] = branch.BuffNo;
                // 25/04/08 rdb added telno
                row["TelNo"] = branch.TelephoneNo;                  //IP - 11/05/10 - UAT(135) UAT5.2.1.0 log - Merged from 4.3
				details.Rows.Add(row);
				ds.Tables.Add(details);
			}
			return ds;
		}

        public int GetBuffNo(SqlConnection conn, SqlTransaction trans, short branchNo) 
        {
            DBranch branch = new DBranch();
            int buffNo = branch.GetBuffNo(conn, trans, branchNo);       
            return buffNo;
        }
        
        public int GetTransRefNo(SqlConnection conn, SqlTransaction trans,short branchNo)
        {
            DBranch branch = new DBranch();
            return branch.GetTransRefNo(conn, trans, branchNo);
        }



		public BBranch()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		/// <summary>
		/// Update
		/// </summary>
		/// <param name="branchno">int</param>
		/// <param name="branchname">string</param>
		/// <param name="branchaddr1">string</param>
		/// <param name="branchaddr2">string</param>
		/// <param name="branchaddr3">string</param>
		/// <param name="branchpocode">string</param>
		/// <param name="telno">string</param>
		/// <param name="countrycode">string</param>
		/// <param name="croffno">int</param>
		/// <param name="oldpctype">string</param>
		/// <param name="newpctype">string</param>
		/// <param name="datepcchange">Datetime</param>
		/// <param name="hissn">int</param>
		/// <param name="hibuffno">int</param>
		/// <param name="warehouseno">string</param>
		/// <param name="hirefno">int</param>
		/// <param name="as400branchno">int</param>
		/// <param name="region">string</param>
		/// <returns>int</returns>
		/// 
		public int Update (SqlConnection conn, SqlTransaction trans, int branchno, string branchname, 
			string branchaddr1, string branchaddr2, string branchaddr3, string branchpocode, 
			string telno, string countrycode, int croffno, string oldpctype, string newpctype, 
			DateTime datepcchange, int hissn, int hibuffno, string warehouseno, int hirefno, 
			int as400branchno, string region, bool depositScreenLocked, DataSet depositSet, string warehouseregion,
            string storeType, bool createRF, bool createCash, bool scoreHPbefore, bool createHP,string Fact2000BranchLetter, //CR903  jec
            bool serviceRepairCentre, bool behavioural, //CR1034 //IP - 08/04/10 - CR1034 - back aa
            int? defaultPrintLocation, //IP - 23/02/10 - CR1072 - Malaysia 3PL for Version 5.2
            bool isThirdPartyWarehouse, bool createStore, bool isCashLoanBranch, 
            bool? luckyDollarStore, bool? ashleyStore) //IP - 23/02/10 - CR1072 - Malaysia 3PL for Version 5.2
            
		{
			int status = 0; 
			DBranch da = new DBranch();
            //StoreCardRepository storeCard = new StoreCardRepository();

			status = da.Update(conn, trans, branchno, branchname, branchaddr1, branchaddr2, branchaddr3,
                branchpocode, telno, countrycode, croffno, oldpctype, newpctype, datepcchange, hissn,
                hibuffno, warehouseno, hirefno, as400branchno, region, depositScreenLocked, warehouseregion,
                Fact2000BranchLetter, storeType, createRF, createCash, scoreHPbefore, createHP, serviceRepairCentre, //CR903
                behavioural, 
                defaultPrintLocation, isThirdPartyWarehouse,createStore, isCashLoanBranch,
                luckyDollarStore, ashleyStore); //IP - 23/02/10 - CR1072 - Malaysia 3PL for Version 5.2
			
			foreach (DataTable dt in depositSet.Tables)
			{
				// CR696 Save the deposit type per paymethod (there will only be one data table)
				foreach(DataRow r in dt.Rows)
				{
					da.SaveDepositType(conn, trans, branchno, (string)r[CN.PayMethod], (string)r[CN.Deposit]);
				}
			}

           
			return status;
		}

		/// <summary>
		/// BGet
		/// </summary>
		/// <param name="branchno">int</param>
		/// <returns>DataSet</returns>
		/// 
		public DataSet Get (int branchno)
		{
			DataSet ds = new DataSet(); 
			DBranch da = new DBranch();
			ds = da.Get(branchno);
			if (ds.Tables.Count > 0)
			{
				ds.Tables[0].TableName = TN.BranchDetails;
				DataTable bankDeposit = da.GetDepositList(branchno);
				if (bankDeposit != null)
				{
					bankDeposit.TableName = TN.BranchDeposits;
					ds.Tables.Add(bankDeposit);
				}

                if (Convert.ToBoolean(Country[CountryParameterNames.StoreCardEnabled])) //IP - 9/12/10 - Only get if Store Card is enabled.
                {
                    DataTable storeCardQualRulesDt = new StoreCardRepository().StoreCardBranchQualRulesGet(Convert.ToInt16(branchno)); //IP - 8/12/10 - Store Card
                    if (storeCardQualRulesDt.Rows.Count > 0)
                    {
                        storeCardQualRulesDt.TableName = TN.StoreCardQualRules;
                        ds.Tables.Add(storeCardQualRulesDt);
                    }
                }
                
			}
			return ds;
		}

		public short GetServiceLocation (int branchno)
		{
			DBranch da = new DBranch();
			da.Populate(null, null, Convert.ToInt16(branchno));
			return da.ServiceLocation;
		}

		/// <summary>
		/// GetPaymentCardType
		/// Returns index zero for short otherwise one
		/// to be used to default card type drop down
		/// </summary>
		/// <param name="branchno">int</param>
		/// <param name="dtAccountOpen">DateTime</param>
		/// <returns>int</returns>
		/// 
		public int GetPaymentCardType (int branchno, DateTime dtAccountOpen)
		{
			string cardType = "";
			DBranch da = new DBranch();
			da.Populate(null, null, Convert.ToInt16(branchno));

			if (dtAccountOpen < da.DatePCChange)
				cardType = da.OldPCType;
			else
				cardType = da.NewPCType;

			if (cardType == CardType.typeShort)
				return 0;
			else
				return 1;
		}

		public string GetDepositType (int branchNo, string payMethod)
		{
			// Return the deposit type (transtypecode) for this
			// paymethod for this branch.
			DBranch da = new DBranch();
			return da.GetDepositType(branchNo, payMethod);
		}

        public string GetStoreType(short branchNo)
        {
            DBranch branch = new DBranch();
            return branch.GetStoreType(branchNo);
        }

        public DataSet GetAllRepairCentre()
        {
            DataSet ds = new DataSet();
            DBranch branch = new DBranch();
            ds.Tables.Add(branch.GetAllRepairCentre());
            return ds;
        }

        //IP - 25/02/10 - CR1072 - Malaysia 3PL for Version 5.2 - Merged from v4.3
        public List<BranchDefaultPrintLocation> GetBranchDefaultPrintLocation(SqlConnection conn, SqlTransaction trans)
        {
            DBranch da = new DBranch();
            SqlDataReader rdr = da.GetBranchDefaultPrintLocation(conn, trans);
            List<BranchDefaultPrintLocation> list = new List<BranchDefaultPrintLocation>();
            while (rdr.Read())
            {
                BranchDefaultPrintLocation branch = new BranchDefaultPrintLocation();
                branch.BranchNo = Convert.ToInt32(rdr[CN.BranchNo] == DBNull.Value ? 0 : rdr[CN.BranchNo]);
                branch.DefaultPrintLocation = Convert.ToInt32(rdr[CN.DefaultPrintLocation] == DBNull.Value ? 0 : rdr[CN.DefaultPrintLocation]);

                list.Add(branch);
            }
            rdr.Close();

            return list;
        }

       
	}
}
