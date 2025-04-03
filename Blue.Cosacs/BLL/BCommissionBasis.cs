using System;
using STL.DAL;
using STL.Common;
using STL.Common.Static;
using System.Data;
using System.Xml;
using System.Data.SqlClient;
using STL.Common.Constants.ColumnNames;
using STL.Common.Constants.TableNames;

namespace STL.BLL
{
	/// <summary>
	/// Summary description for BCommissionBasis.
	/// </summary>
	public class BCommissionBasis : CommonObject
	{
		public BCommissionBasis()
		{
			//
			// TODO: Add constructor logic here
			//
		}
		public DataSet GetDefaultCommissionBasis(string empoyeeType)
		{
			DataSet ds = new DataSet();
			DBailCommnBas bc =  new DBailCommnBas();
			ds.Tables.Add(bc.GetDefaultCommissionBasis(empoyeeType));
			return ds;
		}
		public DataSet GetBailiffCommissionBasis(int empoyeeNo)
		{
			DataSet ds = new DataSet();
			DBailCommnBas bc =  new DBailCommnBas();
			ds.Tables.Add(bc.GetBailiffCommissionBasis(empoyeeNo));
			return ds;
		}

		public void SaveCommissionBasis(
			SqlConnection conn, 
			SqlTransaction trans, 
			decimal allocpercent,
			string collecttype,
			decimal collectionpercent,
			decimal commnpercent,
			string countrycode ,
			short debitaccount,
			string empeetype,
			decimal maxvalue,
			decimal minvalue,
			decimal reposspercent,
			decimal reppercent,
			string statuscode)
		{
			DBailCommnBas bc =  new DBailCommnBas();
			bc.SaveCommissionBasis(
				conn, 
				trans, 
				allocpercent,
				collecttype,
				collectionpercent,
				commnpercent,
				countrycode ,
				debitaccount,
				empeetype,
				maxvalue,
				minvalue,
				reposspercent,
				reppercent,
				statuscode);
		}

		public void SaveBailiffCommissionBasis(
			SqlConnection conn, 
			SqlTransaction trans,
			int empeeno,
			decimal allocpercent,
			string collecttype,
			decimal collectionpercent,
			decimal commnpercent,
			short debitaccount,
			string empeetype,
			decimal maxvalue,
			decimal minvalue,
			decimal reposspercent,
			decimal reppercent,
			string statuscode)
		{
			DBailCommnBas bc =  new DBailCommnBas();
			//CR101 RD 02/12/05 Added user
			bc.User = this.User;
			bc.SaveBailiffCommissionBasis(
				conn, 
				trans,
				empeeno,
				allocpercent,
				collecttype,
				collectionpercent,
				commnpercent,
				debitaccount,
				empeetype,
				maxvalue,
				minvalue,
				reposspercent,
				reppercent,
				statuscode);
		}

		public void DeleteCommissionBasis(
			SqlConnection conn, 
			SqlTransaction trans,
			string countryCode,
			string statusCode,
			string collectType,
			string empeeType)
		{
			DBailCommnBas bc =  new DBailCommnBas();
			bc.DeleteCommissionBasis(
				conn, 
				trans,
				countryCode,
				statusCode,
				collectType,
				empeeType);
		}

		public void DeleteBailiffCommissionBasis(
			SqlConnection conn, 
			SqlTransaction trans,
			int empeeNo,
			string statusCode,
			string collectType)
		{
			DBailCommnBas bc =  new DBailCommnBas();
			bc.DeleteBailiffCommissionBasis(
				conn, 
				trans,
				empeeNo,
				statusCode,
				collectType);
		}

		public void CalculateBailiffCommission(SqlConnection conn, SqlTransaction trans,
			int branchNo, string employeeType)
		{
			DBailCommnBas bc = new DBailCommnBas();
			bc.CalculateBailiffCommission(conn, trans, branchNo, employeeType);
		}

		public DataSet GetCommissionTransactions(int employeeNo)//, string type)
		{
			//DataSet ds = new DataSet();
			DBailCommnBas bc =  new DBailCommnBas();
         bc.GetCommissionTransactions(employeeNo);//, type);
			//ds.Tables.Add(bc.Transactions);
         return bc.Transactions;
		}

		public void UpdateCommissionTransactionStatus(SqlConnection conn, SqlTransaction trans,
			DataSet ds)
		{
			DBailCommnBas bc = new DBailCommnBas();

			foreach(DataRow row in ds.Tables[TN.Transactions].Rows)
			{
				bc.UpdateCommissionTransactionStatus(conn, trans, 
					Convert.ToInt32(row[CN.EmployeeNo]),
					Convert.ToInt32(row[CN.TransRefNo]),
					Convert.ToDateTime(row[CN.DateTrans]),
					(string)row[CN.Status]);
			}
		}

		public void PayBailiffCommission(SqlConnection conn, SqlTransaction trans,
			int empeeNo, decimal commValue)
		{
			DBailiffCommn bc = new DBailiffCommn();
			bc.PayBailiffCommission(conn, trans, empeeNo);
			bc.ResetCommissionDue(conn, trans, empeeNo, commValue);
		}

		public void DeleteCommissionTransaction(SqlConnection conn, SqlTransaction trans,
			int empeeNo, DateTime dateTrans, int transRefNo)
		{
			DBailiffCommn bc = new DBailiffCommn();
			bc.DeleteCommissionTransaction(conn, trans, empeeNo, dateTrans, transRefNo);
		}

		public void RestoreCommissionTransaction(SqlConnection conn, SqlTransaction trans,
			int empeeNo, DateTime dateTrans, int transRefNo)
		{
			DBailiffCommn bc = new DBailiffCommn();
			bc.RestoreCommissionTransaction(conn, trans, empeeNo, dateTrans, transRefNo);
		}

        //IP - 09/06/10 - CR1083 Collection Commission
        public void SaveCollectionCommissionRule(
            SqlConnection conn,
            SqlTransaction trans,
            int id,
            string ruleName,
            string empeeType,
            char commissionType,
            string[] actionArr,
            float pCentArrearsColl,
            float pCentOfCalls,
            float pCentOfWorklist,
            int noOfCalls,
            int noOfDaysSinceAction,
            int noTimeFrameDays,
            decimal minBal,
            decimal maxBal,
            decimal minValColl,
            decimal maxValColl,
            int minMnthsArrears,
            int maxMnthsArrears,
            float pCentCommOnArrears,
            float pCentCommOnAmtPaid,
            float pCentCommOnFee,
            decimal commSetVal)
        {
            DBailCommnBas bc = new DBailCommnBas();

            int ruleID = 0;
            DateTime ruleDatechanged = System.DateTime.Now;

            bool newRule = Convert.ToBoolean(id == -1) ? true : false;
            bool lastAction = false;
            bool noActionsSelected = Convert.ToBoolean(actionArr.Length == 0);

            bc.User = this.User;
            bc.SaveCollectionCommissionRule(
                conn,
                trans,
                id,
                ruleName,
                empeeType,
                commissionType,
                pCentArrearsColl,
                pCentOfCalls,
                pCentOfWorklist,
                noOfCalls,
                noOfDaysSinceAction,
                noTimeFrameDays,
                minBal,
                maxBal,
                minValColl,
                maxValColl,
                minMnthsArrears,
                maxMnthsArrears,
                pCentCommOnArrears,
                pCentCommOnAmtPaid,
                pCentCommOnFee,
                commSetVal, 
                out ruleID,
                out ruleDatechanged
                );

            //If there are actions in the array then we need to insert these actions for the rule
            if (actionArr.Length > 0)
            {
                for (int i = 0; i < actionArr.Length; i++)
                {
                    if (i == (actionArr.Length - 1))
                    {
                        lastAction = true;
                    }

                    bc.SaveCollectionCommissionRuleActions(conn, trans, ruleID, Convert.ToString(actionArr[i]), ruleDatechanged, newRule, lastAction, noActionsSelected);
                }
            }
            
            else
            {
                bc.SaveCollectionCommissionRuleActions(conn, trans, ruleID, "", ruleDatechanged, newRule, lastAction, noActionsSelected);

            }
            
        }

        //IP - 15/06/10 - CR1083 - Collection Commissions
        public DataSet GetCollectionCommissionRules(string employeeType)
        {
            DataSet ds = new DataSet();
            DBailCommnBas bc = new DBailCommnBas();
            ds = bc.GetCollectionCommissionRules(employeeType);
            return ds;
        }

        //IP - 15/06/10 - CR1083 - Collection Commissions
        public void DeleteCollectionCommissionRule(
            SqlConnection conn,
            SqlTransaction trans,
            int id)
        {
            DBailCommnBas bc = new DBailCommnBas();
            bc.User = this.User;
            bc.DeleteCollectionCommissionRule(conn, trans, id);
        }


        //IP - 15/06/10 - CR1083 - Collection Commissions
        public void DeleteCollectionCommissionRuleActions(
            SqlConnection conn,
            SqlTransaction trans,
            int id)
        {
            DBailCommnBas bc = new DBailCommnBas();

            bc.DeleteCollectionCommissionRuleActions(conn, trans, id);
        }


	}
}
