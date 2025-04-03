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
	/// Summary description for DBailCommBas.
	/// </summary>
	public class DBailCommnBas : DALObject
	{
		private DataTable _dt = null;
		//private DataTable _transactions = null;
      private DataSet _dsTransactions = null;

      //public DataTable Transactions
      //{
      //   get{return _transactions;}
      //}

      public DataSet Transactions
      {
         get { return _dsTransactions; }
      }

      private DataSet collCommnRulesAndActions = null;       //IP - 15/06/10 - CR1083 - Collection Commissions#
      public DataSet CollCommnRulesAndActions
      {
          get { return collCommnRulesAndActions; }
      }

		public DataRow GetCollectionRates(SqlConnection conn, SqlTransaction trans, 
			int employeeNo, string accountNo, 
			string collectionType)
		{
			DataRow row;
			try
			{
				_dt = new DataTable(TN.BailiffCommission);
				parmArray = new SqlParameter[3];
				parmArray[0] = new SqlParameter("@empeeno", SqlDbType.Int);
				parmArray[0].Value = employeeNo;
				parmArray[1] = new SqlParameter("@acctno", SqlDbType.NVarChar,12);
				parmArray[1].Value = accountNo;
				parmArray[2] = new SqlParameter("@collType", SqlDbType.NVarChar,1);
				parmArray[2].Value = collectionType;
						
				this.RunSP(conn, trans, "DN_BailCommnBasGetCommissionPercentSP", parmArray, _dt);
				if(_dt.Rows.Count > 0)
					row = _dt.Rows[0];
				else
				{
					row = _dt.NewRow();
					row["collectionpercent"] = 0;
					row["commnpercent"] = 0;
					row["reposspercent"] = 0;
					row["allocpercent"] = 0;
					row["reppercent"] = 0;
					row["minvalue"] = 0;
					row["maxvalue"] = 0;
					row["debitaccount"] = 0;
				}
				
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}			
			return row;
		}
		public DataTable GetDefaultCommissionBasis(string employeeType)
		{
			try
			{
				_dt = new DataTable(TN.BailiffCommission);
				parmArray = new SqlParameter[1];
				parmArray[0] = new SqlParameter("@empeetype", SqlDbType.NVarChar,4);
				parmArray[0].Value = employeeType;
						
				this.RunSP( "DN_GetDelfaultCommissionBasisSP", parmArray, _dt);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}			
			return _dt;
		}
		public DataTable GetBailiffCommissionBasis(int employeeNo)
		{
			try
			{
				_dt = new DataTable(TN.BailiffCommission);
				parmArray = new SqlParameter[1];
				parmArray[0] = new SqlParameter("@empeeno", SqlDbType.Int);
				parmArray[0].Value = employeeNo;
						
				this.RunSP( "DN_GetBailiffCommissionBasisSP", parmArray, _dt);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}			
			return _dt;
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
			try
			{
				parmArray = new SqlParameter[12];
				parmArray[0] = new SqlParameter("@allocpercent", SqlDbType.Float);
				parmArray[0].Value = allocpercent;
				parmArray[1] = new SqlParameter("@collecttype", SqlDbType.Char,1);
				parmArray[1].Value = collecttype;
				parmArray[2] = new SqlParameter("@collectionpercent", SqlDbType.Float);
				parmArray[2].Value = collectionpercent;
				parmArray[3] = new SqlParameter("@commnpercent", SqlDbType.Float);
				parmArray[3].Value = commnpercent;
				parmArray[4] = new SqlParameter("@countrycode", SqlDbType.Char,1);
				parmArray[4].Value = countrycode;
				parmArray[5] = new SqlParameter("@debitaccount", SqlDbType.SmallInt);
				parmArray[5].Value = debitaccount;
				parmArray[6] = new SqlParameter("@empeetype", SqlDbType.VarChar,4);
				parmArray[6].Value = empeetype;
				parmArray[7] = new SqlParameter("@maxvalue", SqlDbType.Float);
				parmArray[7].Value = maxvalue;
				parmArray[8] = new SqlParameter("@minvalue", SqlDbType.Float);
				parmArray[8].Value = minvalue;
				parmArray[9] = new SqlParameter("@reposspercent", SqlDbType.Float);
				parmArray[9].Value = reposspercent;
				parmArray[10] = new SqlParameter("@reppercent", SqlDbType.Float);
				parmArray[10].Value = reppercent;
				parmArray[11] = new SqlParameter("@statuscode", SqlDbType.Char,1);
				parmArray[11].Value = statuscode;
						
				this.RunSP(conn, trans, "DN_SaveCommissionBasisSP", parmArray);
				
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}			
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
			try
			{
				parmArray = new SqlParameter[13];
				parmArray[0] = new SqlParameter("@empeeno", SqlDbType.Int);
				parmArray[0].Value = empeeno;
				parmArray[1] = new SqlParameter("@allocpercent", SqlDbType.Float);
				parmArray[1].Value = allocpercent;
				parmArray[2] = new SqlParameter("@collecttype", SqlDbType.Char,1);
				parmArray[2].Value = collecttype;
				parmArray[3] = new SqlParameter("@collectionpercent", SqlDbType.Float);
				parmArray[3].Value = collectionpercent;
				parmArray[4] = new SqlParameter("@commnpercent", SqlDbType.Float);
				parmArray[4].Value = commnpercent;
				parmArray[5] = new SqlParameter("@debitaccount", SqlDbType.SmallInt);
				parmArray[5].Value = debitaccount;
				parmArray[6] = new SqlParameter("@empeetype", SqlDbType.VarChar,4);
				parmArray[6].Value = empeetype;
				parmArray[7] = new SqlParameter("@maxvalue", SqlDbType.Float);
				parmArray[7].Value = maxvalue;
				parmArray[8] = new SqlParameter("@minvalue", SqlDbType.Float);
				parmArray[8].Value = minvalue;
				parmArray[9] = new SqlParameter("@reposspercent", SqlDbType.Float);
				parmArray[9].Value = reposspercent;
				parmArray[10] = new SqlParameter("@reppercent", SqlDbType.Float);
				parmArray[10].Value = reppercent;
				parmArray[11] = new SqlParameter("@statuscode", SqlDbType.Char,1);
				parmArray[11].Value = statuscode;
				parmArray[12] = new SqlParameter("@empeenochange", SqlDbType.Int);
				parmArray[12].Value = this.User;  //CR101 RD 02/12/05 Added user
				
						
				this.RunSP(conn, trans, "DN_SaveBailiffCommissionBasisSP", parmArray);
				
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}			
		}

		public void DeleteCommissionBasis(
			SqlConnection conn, 
			SqlTransaction trans,
			string countryCode,
			string statusCode,
			string collectType,
			string empeeType)
		{
			try
			{
				parmArray = new SqlParameter[4];
				parmArray[0] = new SqlParameter("@CountryCode", SqlDbType.NChar,1);
				parmArray[0].Value = countryCode;
				parmArray[1] = new SqlParameter("@StatusCode", SqlDbType.NChar,1);
				parmArray[1].Value = statusCode;
				parmArray[2] = new SqlParameter("@CollectType", SqlDbType.NChar,1);
				parmArray[2].Value = collectType;
				parmArray[3] = new SqlParameter("@EmpeeType", SqlDbType.VarChar,4);
				parmArray[3].Value = empeeType;
						
				this.RunSP(conn, trans, "DN_DeleteCommissionBasisSP", parmArray);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}			
		}

		public void DeleteBailiffCommissionBasis(
			SqlConnection conn, 
			SqlTransaction trans,
			int empeeNo,
			string statusCode,
			string collectType)
		{
			try
			{
				parmArray = new SqlParameter[3];
				parmArray[0] = new SqlParameter("@EmpeeNo", SqlDbType.Int);
				parmArray[0].Value = empeeNo;
				parmArray[1] = new SqlParameter("@StatusCode", SqlDbType.NChar,1);
				parmArray[1].Value = statusCode;
				parmArray[2] = new SqlParameter("@CollectType", SqlDbType.NChar,1);
				parmArray[2].Value = collectType;
						
				this.RunSP(conn, trans, "DN_DeleteBailiffCommissionBasisSP", parmArray);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}			
		}

		public void CalculateBailiffCommission(SqlConnection conn, SqlTransaction trans,
												int branchNo, string employeeType)
		{
			try
			{
				parmArray = new SqlParameter[2];
				parmArray[0] = new SqlParameter("@branchno", SqlDbType.Int);
				parmArray[0].Value = branchNo;
				parmArray[1] = new SqlParameter("@empeetype", SqlDbType.NVarChar, 3); //IP - Credit Collections - Need to cater for (3) character Employee Types.
				parmArray[1].Value = employeeType;
						
				this.RunSP(conn, trans, "DN_CalculateCommissionSP", parmArray);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}			
		}

		public int GetCommissionTransactions(int employeeNo)//, string type)
		{
			try
			{
				//_transactions = new DataTable(TN.Transactions);
            _dsTransactions = new DataSet();

				parmArray = new SqlParameter[1];
				parmArray[0] = new SqlParameter("@empeeno", SqlDbType.Int);
				parmArray[0].Value = employeeNo;
            //parmArray[1] = new SqlParameter("@type", SqlDbType.NChar, 1);
            //parmArray[1].Value = type;

            result = this.RunSP("DN_EmployeeGetCommissionTransactionsSP", parmArray, _dsTransactions);
				if(result==0)
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

		public void UpdateCommissionTransactionStatus(SqlConnection conn, SqlTransaction trans,
			int empeeNo, int transRefNo, DateTime dateTrans, string status)
		{
			try
			{
				parmArray = new SqlParameter[4];
				parmArray[0] = new SqlParameter("@empeeno", SqlDbType.Int);
				parmArray[0].Value = empeeNo;
				parmArray[1] = new SqlParameter("@transrefno", SqlDbType.Int);
				parmArray[1].Value = transRefNo;
				parmArray[2] = new SqlParameter("@datetrans", SqlDbType.DateTime);
				parmArray[2].Value = dateTrans;
				parmArray[3] = new SqlParameter("@status", SqlDbType.NChar, 1);
				parmArray[3].Value = status;
						
				this.RunSP(conn, trans, "DN_BailiffCommissionUpdateStatusSP", parmArray);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}			
		}

        //IP - 09/06/10 - CR1083 Collection Commissions
        public void SaveCollectionCommissionRule(
            SqlConnection conn,
            SqlTransaction trans,
            int id,
            string ruleName,
            string empeeType,
            char commissionType,
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
            decimal commSetVal,
            out int ruleID,
            out DateTime ruleDatechanged
            )
        {
            ruleID = 0;
            ruleDatechanged = System.DateTime.Now;

            try
            {
                parmArray = new SqlParameter[23];
                parmArray[0] = new SqlParameter("@id", SqlDbType.Int);
                parmArray[0].Value = id;
                parmArray[1] = new SqlParameter("@ruleName", SqlDbType.VarChar, 50);
                parmArray[1].Value = ruleName;
                parmArray[2] = new SqlParameter("@empeeType", SqlDbType.VarChar, 3);
                parmArray[2].Value = empeeType;
                parmArray[3] = new SqlParameter("@commissionType", SqlDbType.Char, 1);
                parmArray[3].Value = commissionType;
                parmArray[4] = new SqlParameter("@pCentArrearsColl", SqlDbType.Float);
                parmArray[4].Value = pCentArrearsColl;
                parmArray[5] = new SqlParameter("@pCentOfCalls", SqlDbType.Float);
                parmArray[5].Value = pCentOfCalls;
                parmArray[6] = new SqlParameter("@pCentOfWorklist", SqlDbType.Float);
                parmArray[6].Value = pCentOfWorklist;
                parmArray[7] = new SqlParameter("@noOfCalls", SqlDbType.Int);
                parmArray[7].Value = noOfCalls;
                parmArray[8] = new SqlParameter("@noOfDaysSinceAction", SqlDbType.Int);
                parmArray[8].Value = noOfDaysSinceAction;
                parmArray[9] = new SqlParameter("@noTimeFrameDays", SqlDbType.Int);
                parmArray[9].Value = noTimeFrameDays;
                parmArray[10] = new SqlParameter("@minBal", SqlDbType.Money);
                parmArray[10].Value = minBal;
                parmArray[11] = new SqlParameter("@maxBal", SqlDbType.Money);
                parmArray[11].Value = maxBal;
                parmArray[12] = new SqlParameter("@minValColl", SqlDbType.Money);
                parmArray[12].Value = minValColl;
                parmArray[13] = new SqlParameter("@maxValColl", SqlDbType.Money);
                parmArray[13].Value = maxValColl;
                parmArray[14] = new SqlParameter("@minMnthsArrears", SqlDbType.Int);
                parmArray[14].Value = minMnthsArrears;
                parmArray[15] = new SqlParameter("@maxMnthsArrears", SqlDbType.Int);
                parmArray[15].Value = maxMnthsArrears;
                parmArray[16] = new SqlParameter("@pCentCommOnArrears", SqlDbType.Float);
                parmArray[16].Value = pCentCommOnArrears;
                parmArray[17] = new SqlParameter("@pCentCommOnAmtPaid", SqlDbType.Float);
                parmArray[17].Value = pCentCommOnAmtPaid;
                parmArray[18] = new SqlParameter("@pCentCommOnFee", SqlDbType.Float);
                parmArray[18].Value = pCentCommOnFee;
                parmArray[19] = new SqlParameter("@commSetVal", SqlDbType.Money);
                parmArray[19].Value = commSetVal;
                parmArray[20] = new SqlParameter("@empeenoChange", SqlDbType.Int);
                parmArray[20].Value = this.User;
                parmArray[21] = new SqlParameter("@ruleID", SqlDbType.Int);
                parmArray[21].Value = ruleID;
                parmArray[21].Direction = ParameterDirection.Output;
                parmArray[22] = new SqlParameter("@ruleDatechanged", SqlDbType.DateTime);
                parmArray[22].Value = ruleDatechanged;
                parmArray[22].Direction = ParameterDirection.Output;

                this.RunSP(conn, trans, "SaveCollectionCommissionRuleSP", parmArray);

                if (parmArray[21].Value != DBNull.Value)
                {
                    ruleID = Convert.ToInt32(parmArray[21].Value);
                }

                if (parmArray[22].Value != DBNull.Value)
                {
                    ruleDatechanged = Convert.ToDateTime(parmArray[22].Value);
                }

            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

        //IP - 15/06/10 - CR1083 Collection Commissions
        public DataSet GetCollectionCommissionRules(string employeeType)
        {
            try
            {
                collCommnRulesAndActions = new DataSet();

                parmArray = new SqlParameter[1];
                parmArray[0] = new SqlParameter("@empeetype", SqlDbType.NVarChar, 3);
                parmArray[0].Value = employeeType;

                this.RunSP("GetCollectionCommissionRulesSP", parmArray, collCommnRulesAndActions);

                collCommnRulesAndActions.Tables[0].TableName = TN.CollectionCommission;
                collCommnRulesAndActions.Tables[1].TableName = TN.CollectionCommissionActions;
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return collCommnRulesAndActions;
        }

        //IP - 15/06/10 - CR1083 - Collection Commissions
        public void DeleteCollectionCommissionRule(
            SqlConnection conn,
            SqlTransaction trans,
            int ruleID
            )
        {
            try
            {
                parmArray = new SqlParameter[2];
                parmArray[0] = new SqlParameter("@ruleID", SqlDbType.Int);
                parmArray[0].Value = ruleID;
                parmArray[1] = new SqlParameter("@empeenoChange", SqlDbType.Int);
                parmArray[1].Value = this.User;


                this.RunSP(conn, trans, "DeleteCollectionCommissionRuleSP", parmArray);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

        //IP - 14/06/10 - CR1083 - Collection Commissions
        public void SaveCollectionCommissionRuleActions(
            SqlConnection conn,
            SqlTransaction trans,
            int ruleID,
            string action,
            DateTime datechanged,
            bool newRule,
            bool lastAction,
            bool noActionsSelected    
            )
        {
            
            try
            {
                parmArray = new SqlParameter[7];
                parmArray[0] = new SqlParameter("@ruleID", SqlDbType.Int);
                parmArray[0].Value = ruleID;
                parmArray[1] = new SqlParameter("@action", SqlDbType.VarChar, 4);
                parmArray[1].Value = action;
                parmArray[2] = new SqlParameter("@datechanged", SqlDbType.DateTime);
                parmArray[2].Value = datechanged;
                parmArray[3] = new SqlParameter("@empeenoChange", SqlDbType.Int);
                parmArray[3].Value = this.User;
                parmArray[4] = new SqlParameter("@newRule", SqlDbType.Bit);
                parmArray[4].Value = newRule;
                parmArray[5] = new SqlParameter("@lastAction", SqlDbType.Bit);
                parmArray[5].Value = lastAction;
                parmArray[6] = new SqlParameter("@noActionsSelected", SqlDbType.Bit);
                parmArray[6].Value = noActionsSelected;

                this.RunSP(conn, trans, "SaveCollectionCommissionRuleActionsSP", parmArray);

            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

        //IP - 15/06/10 - CR1083 - Collection Commissions
        public void DeleteCollectionCommissionRuleActions(
            SqlConnection conn,
            SqlTransaction trans,
            int ruleID
            )
        {

            try
            {
                parmArray = new SqlParameter[1];
                parmArray[0] = new SqlParameter("@ruleID", SqlDbType.Int);
                parmArray[0].Value = ruleID;

                this.RunSP(conn, trans, "DeleteCollectionCommissionRuleActionsSP", parmArray);

            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }


		public DBailCommnBas()
		{
		}
	}
}
