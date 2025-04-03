using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using STL.Common;
using STL.Common.Constants.Enums;
using STL.Common.Constants.TableNames;
using STL.Common.Constants.ColumnNames;
using STL.Common.Static;
using System.Collections.Generic;

namespace STL.DAL
{
   public partial class DCollectionsModule : DALObject
    {
      
        public DCollectionsModule()
        {
        }
        
       private DataTable _dataTableCollections;
       public DataTable DataTableCollections
       {
           get { return _dataTableCollections; }
       }

       private DataTable _worklists;
       public DataTable Worklists
       {
           get { return _worklists;}
       }

       private DataTable _strategies;
       public DataTable Strategies
       {
           get { return _strategies; }
       }

       //NM & IP - 23/12/08 - CR976
       private DataSet _strategyActions;
       public DataSet StrategyActions
       {
           get { return _strategyActions; }
       }

       //NM & IP - 02/01/09 - CR976
       private DataSet _callReminders;
       public DataSet CallReminders
       {
           get { return _callReminders; }
       }

       //NM & IP - 08/01/09 - CR976 - Extra Telephone Actions
       private DataSet _legalFraudInsuranceDetails;
       public DataSet LegalFraudInsuranceDetails
       {
           get { return _legalFraudInsuranceDetails; }
       }

       //IP & JC - 12/01/09 - CR976 - Special Arrangements
       private DataSet _combinedSPADetails;
       public DataSet CombinedSPADetails
       {
           get { return _combinedSPADetails; }
       }

       public void GetWorkListEmployees()
       {
           try
           {
               _dataTableCollections = new DataTable(TN.WorkListEmployeeTypes);
               RunSP("CM_GetWorkListEmployeesSP", _dataTableCollections);
           }
           catch (SqlException ex)
           {
               LogSqlException(ex);
               throw ex;
           }
       }

       public void GetWorkList()
       {
           try
           {
               _dataTableCollections = new DataTable(TN.WorkList);
               RunSP("CM_GetWorkListsSP", _dataTableCollections);
           }
           catch (SqlException ex)
           {
               LogSqlException(ex);
               throw ex;
           }
       }

      public void GetWorkListActions()
      {
         try
         {
            _dataTableCollections = new DataTable(TN.WorkListActions);
            RunSP("CM_GetWorkListActionsSP", _dataTableCollections);
         }
         catch (SqlException ex)
         {
            LogSqlException(ex);
            throw ex;
         }
      }

      public DataSet GetActions()
      {
          DataSet dsActions = new DataSet();
          try
          {
              this.RunSP("CM_GetActionsSP", dsActions);
          }
          catch (SqlException ex)
          {
              LogSqlException(ex);
              throw ex;
          }
          return dsActions;
      }

      public void GetEmployeeTypes()
      {
         try
         {
            _dataTableCollections = new DataTable(TN.EmployeeTypes);
            RunSP("CM_GetEmployeeTypesSP", _dataTableCollections);
         }
         catch (SqlException ex)
         {
            LogSqlException(ex);
            throw ex;
         }
      }

       public void GetWorkListData()
       {
           try
           {
               _dataTableCollections = new DataTable(TN.Data);
               RunSP("CM_GetWorkListDataSP", _dataTableCollections);
           }
           catch (SqlException ex)
           {
               LogSqlException(ex);
               throw ex;
           }
       }

       //public void SaveWorkList(SqlConnection conn, SqlTransaction trans, string workList, 
       //                         string description, string empeeType)
       //{
       //    try
       //    {
       //        parmArray = new SqlParameter[3];
       //        parmArray[0] = new SqlParameter("@worklist", SqlDbType.NVarChar, 10);
       //        parmArray[0].Value = workList;
       //        parmArray[1] = new SqlParameter("@description", SqlDbType.NVarChar, 30);
       //        parmArray[1].Value = description;
       //        parmArray[2] = new SqlParameter("@empeetype", SqlDbType.NVarChar, 3); //IP - 22/05/08 - Credit Collections - Need to cater for (3) character Employee Types.
       //        parmArray[2].Value = empeeType;
       //        this.RunSP(conn, trans, "CM_SaveWorkListSP", parmArray);
       //    }
       //    catch (SqlException ex)
       //    {
       //        LogSqlException(ex);
       //        throw ex;
       //    }
       //}

       public void SaveWorkListAction(SqlConnection conn, SqlTransaction trans, string workList,
                                        string action, bool effect)
       {
           try
           {
               parmArray = new SqlParameter[3];
               parmArray[0] = new SqlParameter("@worklist", SqlDbType.NVarChar, 10);
               parmArray[0].Value = workList;
               parmArray[1] = new SqlParameter("@action", SqlDbType.NVarChar, 20);
               parmArray[1].Value = action;
               parmArray[2] = new SqlParameter("@effect", SqlDbType.Bit);
               parmArray[2].Value = effect;
               this.RunSP(conn, trans, "CM_SaveWorkListActionsSP", parmArray);
           }
           catch (SqlException ex)
           {
               LogSqlException(ex);
               throw ex;
           }
       }

       public void DeleteWorkList(SqlConnection conn, SqlTransaction trans, string workList)
       {
           try
           {
               parmArray = new SqlParameter[1];
               parmArray[0] = new SqlParameter("@worklist", SqlDbType.NVarChar, 10);
               parmArray[0].Value = workList;
               this.RunSP(conn, trans, "CM_DeleteWorkListSP", parmArray);
           }
           catch (SqlException ex)
           {
               LogSqlException(ex);
               throw ex;
           }
       }

       public void DeleteWorkListActions(SqlConnection conn, SqlTransaction trans, string workList)
       {
           try
           {
               parmArray = new SqlParameter[1];
               parmArray[0] = new SqlParameter("@worklist", SqlDbType.NVarChar, 10);
               parmArray[0].Value = workList;
               this.RunSP(conn, trans, "CM_DeleteActionsSP", parmArray);
           }
           catch (SqlException ex)
           {
               LogSqlException(ex);
               throw ex;
           }
       }

      public void DeleteSMS(SqlConnection conn, SqlTransaction trans, string sms)
      {
         try
         {
            parmArray = new SqlParameter[1];
            parmArray[0] = new SqlParameter("@SMS", SqlDbType.NVarChar, 10);
            parmArray[0].Value = sms;
            this.RunSP(conn, trans, "CM_DeleteSMSSP", parmArray);
         }
         catch (SqlException ex)
         {
            LogSqlException(ex);
            throw ex;
         }
      }

       public void SaveSMS(SqlConnection conn, SqlTransaction trans, string smsName, string smsBody, string description)
       {
           try
           {
               parmArray = new SqlParameter[3];
               parmArray[0] = new SqlParameter("@name", SqlDbType.NVarChar, 10);
               parmArray[0].Value = smsName;
               parmArray[1] = new SqlParameter("@body", SqlDbType.NVarChar, 3000);
               parmArray[1].Value = smsBody;
               parmArray[2] = new SqlParameter("@description", SqlDbType.NVarChar, 64);
               parmArray[2].Value = description;
               this.RunSP(conn, trans, "CM_SaveSMSSP", parmArray);
           }
           catch (SqlException ex)
           {
               LogSqlException(ex);
               throw ex;
           }
       }

       //NM & IP - 06/01/09 - CR976 - Included 'worklist' parameter, for manually sending an account to a worklist.
       //NM & IP - 07/01/09 - CR976 - Extra Telephone Actions - 'STW' (Send to WriteOff) - added parameters 'reasonForWriteOff' and 'empeeno'
      public void UpdateStrategyAccounts(SqlConnection conn, SqlTransaction trans, string acct, string strategy, string worklist, string reasonForWriteOff, int empeeno)
      {
         try
         {
            parmArray = new SqlParameter[5];
            parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar, 12);
            parmArray[0].Value = acct;
            parmArray[1] = new SqlParameter("@sendToStrategy", SqlDbType.NVarChar, 6);
            parmArray[1].Value = strategy;
            parmArray[2] = new SqlParameter("@sendToWorklist", SqlDbType.NVarChar, 10);
            parmArray[2].Value = worklist;
            parmArray[3] = new SqlParameter("@reasonForWriteOff", SqlDbType.NVarChar, 12);
            parmArray[3].Value = reasonForWriteOff;
            parmArray[4] = new SqlParameter("@empeeno", SqlDbType.Int);
            parmArray[4].Value = empeeno;
            this.RunSP(conn, trans, "CMStrategyAcctUpdateSP", parmArray);
         }
         catch (SqlException ex)
         {
            LogSqlException(ex);
            throw ex;
         }
      }

       /// <summary>
       /// Returns details from the CMWorklistAccts table for a given AcctNo.
       /// </summary>
       /// <param name="acctNo">The acct no to retrieve data for</param>
       /// <returns>int Status value</returns>
       public int GetWorklistsbyacctno(string acctNo)
       {
           try
           {
               _worklists = new DataTable(TN.AccountDetails);
               parmArray = new SqlParameter[1];
               parmArray[0] = new SqlParameter("@acctno", SqlDbType.VarChar, 12);
               parmArray[0].Value = acctNo;
               result = this.RunSP("CM_WorklistsAcctLoadbyAcctno", parmArray, _worklists);

               if (result == 0)
               {
                   result = (int)Return.Success;
               }
               else
               {
                   result = (int)Return.Fail;
               }
           }
           catch (SqlException ex)
           {
               LogSqlException(ex);
               throw ex;
           }
           return result;
       }

       /// <summary>
       /// Returns details from the CMStrategyAcct table for a given AcctNo.
       /// </summary>
       /// <param name="acctNo">The acct no to retrieve data for</param>
       /// <returns>int Status value</returns>
       
       //IP - 26/08/09 - UAT(819) - Check if the strategy the account currently is in has worklists linked to it.
       public int GetStrategiesbyacctno(string acctNo, out bool strategyHasWorklists)
       {
           strategyHasWorklists = false;

           try
           {
               _strategies = new DataTable(TN.Strategies);
               parmArray = new SqlParameter[2];
               parmArray[0] = new SqlParameter("@acctno", SqlDbType.VarChar, 12);
               parmArray[0].Value = acctNo;
               parmArray[1] = new SqlParameter("@strategyHasWorklists", SqlDbType.Bit);
               parmArray[1].Value = strategyHasWorklists;
               parmArray[1].Direction = ParameterDirection.Output;
               result = this.RunSP("CM_StrategyAcctLoadbyAcctno", parmArray, _strategies);

               if (result == 0)
               {
                   if (parmArray[1].Value != DBNull.Value)
                       strategyHasWorklists = Convert.ToBoolean(parmArray[1].Value);
                   result = (int)Return.Success;
               }
               else
               {
                   result = (int)Return.Fail;
               }
           }
           catch (SqlException ex)
           {
               LogSqlException(ex);
               throw ex;
           }
           return result;
       }

      public void GetAllStrategyActions()
      {
         try
         {
            _dataTableCollections = new DataTable(TN.StrategyActions);
            RunSP("CM_GetAllStrategyActionsSP", _dataTableCollections);
         }
         catch (SqlException ex)
         {
            LogSqlException(ex);
            throw ex;
         }
      }

       //IP - 25/09/08 - UAT5.2 - UAT(529)
       public void GetStrategyWorklistActions(int empeeno)
       {
         

           try
           {
               _dataTableCollections = new DataTable(TN.StrategyActions);
               parmArray = new SqlParameter[1];
               parmArray[0] = new SqlParameter("@empeeno", SqlDbType.Int);
               parmArray[0].Value = empeeno;
               RunSP("CM_GetStrategyWorklistActionsSP", parmArray, _dataTableCollections);
           }
           catch (SqlException ex)
           {
               LogSqlException(ex);
               throw ex;
           }
       }

       //NM & IP - 23/12/08 - CR976
       //Method which retrieves all the actions that the employee has rights to
       //for the strategy associated with the selected account
       public void GetStrategyActionsForEmployee(int empeeno, string strategy, bool checkForSupervisorRight)
       {

           try
           {
               _strategyActions = new DataSet(TN.StrategyActions);
               parmArray = new SqlParameter[3];
               parmArray[0] = new SqlParameter("@empeeno", SqlDbType.Int);
               parmArray[0].Value = empeeno;
               parmArray[1] = new SqlParameter("@strategy", SqlDbType.VarChar, 10);
               parmArray[1].Value = strategy;
               parmArray[2] = new SqlParameter("@checkForSupervisorRight", SqlDbType.Bit);
               parmArray[2].Value = checkForSupervisorRight;
               RunSP("CM_GetStrategyActionsForEmployeeSP", parmArray, _strategyActions);
           }
           catch (SqlException ex)
           {
               LogSqlException(ex);
               throw ex;
           }
       }



       //NM 20/01/2009 - To update Work List/Action rights --------------
       public void AddWorkListRights(SqlConnection conn, SqlTransaction trans, int empeeNo, string workList, int user)
       {
           try
           {
               parmArray = new SqlParameter[3];

               parmArray[0] = new SqlParameter("@EmpeeNo", SqlDbType.Int);
               parmArray[0].Value = empeeNo;
               parmArray[1] = new SqlParameter("@WorkList", SqlDbType.VarChar, 10);
               parmArray[1].Value = workList;
               //parmArray[2] = new SqlParameter("@EmpeeType", SqlDbType.VarChar, 10);
               //parmArray[2].Value = empeeType;
               parmArray[2] = new SqlParameter("@Empeenochange", SqlDbType.Int);
               parmArray[2].Value = user;

               RunSP(conn, trans, "CM_SaveWorklistRightsSP", parmArray);
           }
           catch (SqlException ex)
           {
               LogSqlException(ex);
               throw ex;
           }
       }

       public void RemoveWorkListRights(SqlConnection conn, SqlTransaction trans, int empeeNo,  string workList)
       {
           try
           {
               parmArray = new SqlParameter[2];

               parmArray[0] = new SqlParameter("@EmpeeNo", SqlDbType.Int);
               parmArray[0].Value = empeeNo;
               parmArray[1] = new SqlParameter("@WorkList", SqlDbType.VarChar, 10);
               parmArray[1].Value = workList;
               //parmArray[2] = new SqlParameter("@EmpeeType", SqlDbType.VarChar, 10);
               //parmArray[2].Value = empeeType;

               RunSP(conn, trans, "CM_DeleteWorkListRights", parmArray);
           }
           catch (SqlException ex)
           {
               LogSqlException(ex);
               throw ex;
           }
       }

       public void AddActionRights(SqlConnection conn, SqlTransaction trans, int empeeNo, string empeeType, string strategy, string action, bool cycleToNextFlag, int minNoteLength, int user)
       {
           try
           {
               parmArray = new SqlParameter[7];

               parmArray[0] = new SqlParameter("@EmpeeNo", SqlDbType.Int);
               parmArray[0].Value = empeeNo;
               parmArray[1] = new SqlParameter("@Strategy", SqlDbType.VarChar, 10);
               parmArray[1].Value = strategy;
               parmArray[2] = new SqlParameter("@Action", SqlDbType.VarChar, 10);
               parmArray[2].Value = action;
               parmArray[3] = new SqlParameter("@EmpeeType", SqlDbType.VarChar, 100);
               parmArray[3].Value = empeeType;
               parmArray[4] = new SqlParameter("@Empeenochange", SqlDbType.Int);
               parmArray[4].Value = user;
               parmArray[5] = new SqlParameter("@CycleToNextFlag", SqlDbType.Bit);
               parmArray[5].Value = cycleToNextFlag;
               parmArray[6] = new SqlParameter("@MinNotesLength", SqlDbType.Int);
               parmArray[6].Value = minNoteLength;
               
               RunSP(conn, trans, "CM_SaveActionRights", parmArray);
           }
           catch (SqlException ex)
           {
               LogSqlException(ex);
               throw ex;
           }
       }

       public void RemoveActionRights(SqlConnection conn, SqlTransaction trans, int empeeNo,  string strategy, string action)
       {
           try
           {
               parmArray = new SqlParameter[3];

               parmArray[0] = new SqlParameter("@EmpeeNo", SqlDbType.Int);
               parmArray[0].Value = empeeNo;
               parmArray[1] = new SqlParameter("@Strategy", SqlDbType.VarChar, 10);
               parmArray[1].Value = strategy;
               parmArray[2] = new SqlParameter("@Action", SqlDbType.VarChar, 10);
               parmArray[2].Value = action;
               //parmArray[3] = new SqlParameter("@EmpeeType", SqlDbType.VarChar, 10);
               //parmArray[3].Value = empeeType;

               RunSP(conn, trans, "CM_DeleteActionRights", parmArray);
           }
           catch (SqlException ex)
           {
               LogSqlException(ex);
               throw ex;
           }
       }

       public void SaveWorkLists(SqlConnection conn, SqlTransaction trans, string workList, string description)
       {
           try
           {
               parmArray = new SqlParameter[2];

               parmArray[0] = new SqlParameter("@WorkList", SqlDbType.VarChar, 10);
               parmArray[0].Value = workList;
               parmArray[1] = new SqlParameter("@Description", SqlDbType.NVarChar, 30);
               parmArray[1].Value = description;
              
               RunSP(conn, trans, "CM_SaveWorkLists", parmArray);
           }
           catch (SqlException ex)
           {
               LogSqlException(ex);
               throw ex;
           }
       }

       public void DeleteWorkLists(SqlConnection conn, SqlTransaction trans, string workList)
       {
           try
           {
               parmArray = new SqlParameter[1];

               parmArray[0] = new SqlParameter("@WorkList", SqlDbType.VarChar, 10);
               parmArray[0].Value = workList;
            
               RunSP(conn, trans, "CM_DeleteWorkLists", parmArray);
           }
           catch (SqlException ex)
           {
               LogSqlException(ex);
               throw ex;
           }
       }

       public void SaveWorkListSortOrder(SqlConnection conn, SqlTransaction trans, string empeeType, string columnName, int sortOrder, string ascDesc)
       {
           try
           {
               parmArray = new SqlParameter[4];

               parmArray[0] = new SqlParameter("@EmpeeType", SqlDbType.VarChar, 3);
               parmArray[0].Value = empeeType;
               parmArray[1] = new SqlParameter("@SortColumnName", SqlDbType.VarChar, 32);
               parmArray[1].Value = columnName;
               parmArray[2] = new SqlParameter("@SortOrder", SqlDbType.SmallInt);
               parmArray[2].Value = sortOrder;
               parmArray[3] = new SqlParameter("@AscDesc", SqlDbType.VarChar, 4);
               parmArray[3].Value = ascDesc;

               RunSP(conn, trans, "CM_SaveWorkListSortOrder", parmArray);
           }
           catch (SqlException ex)
           {
               LogSqlException(ex);
               throw ex;
           }
       }

       public void DeleteWorkListSortOrder(SqlConnection conn, SqlTransaction trans, string empeeType)
       {
           try
           {
               parmArray = new SqlParameter[1];

               parmArray[0] = new SqlParameter("@EmpeeType", SqlDbType.VarChar, 3);
               parmArray[0].Value = empeeType;

               RunSP(conn, trans, "CM_DeleteWorkListSortOrder", parmArray);
           }
           catch (SqlException ex)
           {
               LogSqlException(ex);
               throw ex;
           }
       }


       //----------------------------------------------------------------

       //NM 22/01/2009
       public DataSet GetWorkListRightsHierarchy()
       {
           DataSet ds = new DataSet();
           try
           {
               this.RunSP("CM_GetWorkListRightsHierarchy", ds);
           }
           catch (SqlException ex)
           {
               LogSqlException(ex);
               throw ex;
           }
           return ds;

       }

       //NM 22/01/2009
       public DataSet GetActionRightsHierarchy()
       {
           DataSet ds = new DataSet();
           try
           {
               this.RunSP("CM_GetActionRightsHierarchy", ds);
           }
           catch (SqlException ex)
           {
               LogSqlException(ex);
               throw ex;
           }
           return ds;

       }

       //NM 22/01/2009
       public DataSet LoadWorkListOrderColumns()
       {
           DataSet ds = new DataSet();
           try
           {
               this.RunSP("CM_GetWorkListOrderColumns", ds);
           }
           catch (SqlException ex)
           {
               LogSqlException(ex);
               throw ex;
           }
           return ds;

       }

       //NM 22/01/2009
       public DataSet LoadWorkListSortOrder()
       {
           DataSet ds = new DataSet();
           try
           {
               this.RunSP("CM_GetWorkListSortOrder", ds);
           }
           catch (SqlException ex)
           {
               LogSqlException(ex);
               throw ex;
           }
           return ds;

       }

       //NM 27/01/2009
       public DataSet LoadLetterFieldsbyCode(string letterCode, int runNo, char storeType, bool spouseDetailRequired, bool guarantorDetailRequired)
       {
           DataSet ds = new DataSet();
           try
           {
               parmArray = new SqlParameter[5];

               parmArray[0] = new SqlParameter("@letterCode", SqlDbType.VarChar, 10);
               parmArray[0].Value = letterCode;
               parmArray[1] = new SqlParameter("@runNo", SqlDbType.SmallInt);
               parmArray[1].Value = runNo;
               parmArray[2] = new SqlParameter("@storeType", SqlDbType.Char, 1);
               parmArray[2].Value = storeType;
               parmArray[3] = new SqlParameter("@spouseDetailRequired", SqlDbType.Bit);
               parmArray[3].Value = spouseDetailRequired;
               parmArray[4] = new SqlParameter("@guarantorDetailRequired", SqlDbType.Bit);
               parmArray[4].Value = guarantorDetailRequired;


               RunSP("DN_LoadLetterFieldsByCode", parmArray, ds);
           }
           catch (SqlException ex)
           {
               LogSqlException(ex);
               throw ex;
           }

           //Renaming the data tables
           if(ds.Tables.Count > 0)
               ds.Tables[0].TableName = "ACCOUNT";
           
           if (spouseDetailRequired && ds.Tables.Count > 1)
               ds.Tables[1].TableName = "SPOUSE";
           
           if(guarantorDetailRequired && ds.Tables.Count > 2)
               ds.Tables[2].TableName = "GUARANTOR";
           else if (guarantorDetailRequired && ds.Tables.Count == 2)
               ds.Tables[1].TableName = "GUARANTOR";
               
           return ds;
       }       

       //NM - 06/01/09 - CR976 - Plz note nullable types have been used
       //Method to insert Extra Telephone Action detail (Insurance)
       //TelephoneActions screen
       public void UpdateInsuranceDetail(SqlConnection conn, SqlTransaction trans, string acctno,int empeeno, 
                                         DateTime? initiatedDate, string fullOrPartClaim, decimal? insAmount,
                                            string insType, bool isApproved, string userNotes)
       {
           try
           {

               parmArray = new SqlParameter[8];

               parmArray[0] = new SqlParameter("@acctno", SqlDbType.Char, 12);
               parmArray[0].Value = acctno;
               parmArray[1] = new SqlParameter("@empeeno", SqlDbType.Int);
               parmArray[1].Value = empeeno;

               parmArray[2] = new SqlParameter("@initiatedDate", SqlDbType.DateTime);
               if (initiatedDate.HasValue)
                   parmArray[2].Value = initiatedDate;
               else
                   parmArray[2].Value = DBNull.Value;

               parmArray[3] = new SqlParameter("@fullOrPartClaim", SqlDbType.Char,4);
               if (String.IsNullOrEmpty(fullOrPartClaim))
                   parmArray[3].Value = DBNull.Value;
               else
                   parmArray[3].Value = fullOrPartClaim;

               parmArray[4] = new SqlParameter("@insAmount", SqlDbType.Money);
               if (insAmount.HasValue)
                   parmArray[4].Value = insAmount;
               else
                   parmArray[4].Value = DBNull.Value;

               parmArray[5] = new SqlParameter("@insType", SqlDbType.VarChar, 12);
               if (String.IsNullOrEmpty(insType))
                   parmArray[5].Value = DBNull.Value;
               else
                   parmArray[5].Value = insType;
               
               parmArray[6] = new SqlParameter("@isApproved", SqlDbType.Bit);
               parmArray[6].Value = isApproved;

               parmArray[7] = new SqlParameter("@userNotes", SqlDbType.VarChar, 300);
               if (String.IsNullOrEmpty(userNotes))
                   parmArray[7].Value = "";
               else
                   parmArray[7].Value = userNotes;
          
               RunSP(conn, trans, "CM_SaveInsuranceDetail", parmArray);
           }
           catch (SqlException ex)
           {
               LogSqlException(ex);
               throw ex;
           }
       }

       //NM - 06/01/09 - CR976 - Plz note nullable types have been used
       //Method to insert Extra Telephone Action detail (Fraud)
       //TelephoneActions screen
       public void UpdateFraudDetail(SqlConnection conn, SqlTransaction trans, string acctno,int empeeno, 
                                     DateTime? fraudInitiatedDate, bool isResolved, string userNotes)
       {
           try
           {
               parmArray = new SqlParameter[5];

               parmArray[0] = new SqlParameter("@acctno", SqlDbType.Char, 12);
               parmArray[0].Value = acctno;
               parmArray[1] = new SqlParameter("@empeeno", SqlDbType.Int);
               parmArray[1].Value = empeeno;

           	   parmArray[2] = new SqlParameter("@fraudInitiatedDate", SqlDbType.DateTime);
               if (fraudInitiatedDate.HasValue)
                   parmArray[2].Value = fraudInitiatedDate;
               else
                   parmArray[2].Value = DBNull.Value; 

               parmArray[3] = new SqlParameter("@isResolved", SqlDbType.Bit);
               parmArray[3].Value = isResolved;

               parmArray[4] = new SqlParameter("@userNotes", SqlDbType.VarChar, 300);
               if (String.IsNullOrEmpty(userNotes))
                   parmArray[4].Value = "";
               else
                   parmArray[4].Value = userNotes;

               RunSP(conn, trans, "CM_SaveFraudDetail", parmArray);

           }
           catch (SqlException ex)
           {
               LogSqlException(ex);
               throw ex;
           }
       }


       //NM - 06/01/09 - CR976 - Plz note nullable types have been used
       //Method to insert Extra Telephone Action detail (Legal)
       //TelephoneActions screen
       public void UpdateLegalDetail(SqlConnection conn, SqlTransaction trans, string acctno, int empeeno, string solicitorNo,
                                     decimal? auctionProceeds,DateTime? auctionDate,decimal? auctionAmount, decimal? courtDeposit,
                                     decimal? courtAmount,DateTime? courtDate, bool caseClosed,DateTime? mentionDate,decimal? mentionCost,
                                     decimal? paymentRemittance, string judgement,DateTime? legalAttachmentDate,DateTime? legalInitiatedDate,
                                     DateTime? defaultedDate, string userNotes)
       {
           try
           {

               parmArray = new SqlParameter[18];

               parmArray[0] = new SqlParameter("@acctno", SqlDbType.Char, 12);
               parmArray[0].Value = acctno;
               parmArray[1] = new SqlParameter("@empeeno", SqlDbType.Int);
               parmArray[1].Value = empeeno;
               
               parmArray[2] = new SqlParameter("@solicitorNo", SqlDbType.VarChar, 20);
               if(String.IsNullOrEmpty(solicitorNo))
                   parmArray[2].Value = "";
               else
                   parmArray[2].Value = solicitorNo;

               parmArray[3] = new SqlParameter("@auctionProceeds", SqlDbType.Money);
               if(auctionProceeds.HasValue)
                   parmArray[3].Value = auctionProceeds;
               else
                   parmArray[3].Value = DBNull.Value;

               parmArray[4] = new SqlParameter("@auctionDate", SqlDbType.DateTime);
               if (auctionDate.HasValue)
                   parmArray[4].Value = auctionDate;
               else
                   parmArray[4].Value = DBNull.Value;

               parmArray[5] = new SqlParameter("@auctionAmount", SqlDbType.Money);
               if(auctionAmount.HasValue)
                   parmArray[5].Value = auctionAmount;
               else
                   parmArray[5].Value = DBNull.Value;

               parmArray[6] = new SqlParameter("@courtDeposit", SqlDbType.Money);
               if(courtDeposit.HasValue)
                   parmArray[6].Value = courtDeposit;
               else 
                   parmArray[6].Value = DBNull.Value;

               parmArray[7] = new SqlParameter("@courtAmount", SqlDbType.Money);
               if (courtAmount.HasValue)
                   parmArray[7].Value = courtAmount;
               else
                   parmArray[7].Value = DBNull.Value;
               
               parmArray[8] = new SqlParameter("@courtDate", SqlDbType.DateTime);
               if (courtDate.HasValue)
                   parmArray[8].Value = courtDate;
               else
                   parmArray[8].Value = DBNull.Value;

               parmArray[9] = new SqlParameter("@caseClosed", SqlDbType.Bit);
               parmArray[9].Value = caseClosed;

               parmArray[10] = new SqlParameter("@mentionDate", SqlDbType.DateTime);
               if (mentionDate.HasValue)
                   parmArray[10].Value = mentionDate;
               else
                   parmArray[10].Value = DBNull.Value;

               parmArray[11] = new SqlParameter("@mentionCost", SqlDbType.Money);
               if (mentionCost.HasValue)
                   parmArray[11].Value = mentionCost;
               else
                   parmArray[11].Value = DBNull.Value;

               parmArray[12] = new SqlParameter("@paymentRemittance", SqlDbType.Money);
               if(paymentRemittance.HasValue)
                   parmArray[12].Value = paymentRemittance;
               else 
                   parmArray[12].Value = DBNull.Value;

               parmArray[13] = new SqlParameter("@judgement", SqlDbType.VarChar, 300);
               if (String.IsNullOrEmpty(judgement))
                   parmArray[13].Value = "";
               else 
                   parmArray[13].Value = judgement;

               parmArray[14] = new SqlParameter("@legalAttachmentDate", SqlDbType.DateTime);
               if (legalAttachmentDate.HasValue)
                   parmArray[14].Value = legalAttachmentDate;
               else 
                   parmArray[14].Value = DBNull.Value;

               parmArray[15] = new SqlParameter("@legalInitiatedDate", SqlDbType.DateTime);
               if (legalInitiatedDate.HasValue)
                   parmArray[15].Value = legalInitiatedDate;
               else
                   parmArray[15].Value = DBNull.Value;

               parmArray[16] = new SqlParameter("@defaultedDate", SqlDbType.DateTime);
               if (defaultedDate.HasValue)
                   parmArray[16].Value = defaultedDate;
               else 
                   parmArray[16].Value = DBNull.Value;

               parmArray[17] = new SqlParameter("@userNotes", SqlDbType.VarChar, 300);
               if (String.IsNullOrEmpty(userNotes))
                   parmArray[17].Value = "";
               else
                   parmArray[17].Value = userNotes;

               RunSP(conn, trans, "CM_SaveLegalDetail", parmArray);

           }
           catch (SqlException ex)
           {
               LogSqlException(ex);
               throw ex;
           }
       }


       //NM - 29/12/08 - CR976
       //Method to updateor insert customer telephone numbers from 
       //TelephoneActions screen
       //IP - 30/12/08 - Need to pass in the employee number that is updating the record.
       public void UpdateCustomerTelephoneNo(SqlConnection conn, SqlTransaction trans, string custid, string HTelNo, string HDialCode, string WTelNo,
           string WDialCode, string MTelNo, string MDialCode, int Empeeno, bool HomeTelephoneChanged, bool WorkTelephoneChanged, bool MobileTelephoneChanged)
       {
           try
           {
               parmArray = new SqlParameter[11];
               parmArray[0] = new SqlParameter("@custid", SqlDbType.VarChar, 20);
               parmArray[0].Value = custid;
               parmArray[1] = new SqlParameter("@Htelno", SqlDbType.VarChar, 20);
               parmArray[1].Value = HTelNo;
               parmArray[2] = new SqlParameter("@Hdialcode", SqlDbType.VarChar, 8);
               parmArray[2].Value = HDialCode;
               parmArray[3] = new SqlParameter("@Wtelno", SqlDbType.VarChar, 20);
               parmArray[3].Value = WTelNo;
               parmArray[4] = new SqlParameter("@Wdialcode", SqlDbType.VarChar, 8);
               parmArray[4].Value = WDialCode;
               parmArray[5] = new SqlParameter("@Mtelno", SqlDbType.VarChar, 20);
               parmArray[5].Value = MTelNo;
               parmArray[6] = new SqlParameter("@Mdialcode", SqlDbType.VarChar, 8);
               parmArray[6].Value = MDialCode;
               parmArray[7] = new SqlParameter("@Empeeno", SqlDbType.Int);
               parmArray[7].Value = Empeeno;
               parmArray[8] = new SqlParameter("@HChanged", SqlDbType.Bit);
               parmArray[8].Value = HomeTelephoneChanged;
               parmArray[9] = new SqlParameter("@WChanged", SqlDbType.Bit);
               parmArray[9].Value = WorkTelephoneChanged;
               parmArray[10] = new SqlParameter("@MChanged", SqlDbType.Bit);
               parmArray[10].Value = MobileTelephoneChanged;
               
               RunSP(conn, trans,"DN_TelephoneActionUpdateCustTelephone", parmArray);

           }
           catch (SqlException ex)
           {
               LogSqlException(ex);
               throw ex;
           }
       }

       //NM & IP - 02/01/09 - CR976 - Method that will retrieve all the call reminders
       //for the selected account for the current user
       public void GetCallReminderInfo(string acctno, int empeeno)
       {

           try
           {
               _callReminders = new DataSet(TN.CallReminders);

               parmArray = new SqlParameter[2];
               parmArray[0] = new SqlParameter("@acctno", SqlDbType.VarChar, 12);
               parmArray[0].Value = acctno;
               parmArray[1] = new SqlParameter("@empeeno", SqlDbType.Int);
               parmArray[1].Value = empeeno;
               RunSP("CM_GetCallReminderInfoSP", parmArray, _callReminders);
           }
           catch (SqlException ex)
           {
               LogSqlException(ex);
               throw ex;
           }
       }

       //NM & IP - 08/01/09 - CR976 - Method that will retrieve any 'Legal', 'Fraud', or 
       //'Insurance' details for a selected account if any have been entered.

       public void GetLegalFraudInsuranceDetails(string acctno)
       {

           try
           {
               _legalFraudInsuranceDetails = new DataSet();

               parmArray = new SqlParameter[1];
               parmArray[0] = new SqlParameter("@acctno", SqlDbType.VarChar, 12);
               parmArray[0].Value = acctno;

               RunSP("CM_GetLegalFraudInsuranceDetails", parmArray, _legalFraudInsuranceDetails);

               //Make sure that there are four tables returned, then set the table names. 
               if (_legalFraudInsuranceDetails.Tables.Count == 4)
               {
                   _legalFraudInsuranceDetails.Tables[0].TableName = TN.LegalDetails;
                   _legalFraudInsuranceDetails.Tables[1].TableName = TN.FraudDetails;
                   _legalFraudInsuranceDetails.Tables[2].TableName = TN.InsuranceDetails;
                   _legalFraudInsuranceDetails.Tables[3].TableName = TN.TraceDetails;
               }
           }
           catch (SqlException ex)
           {
               LogSqlException(ex);
               throw ex;
           }
       }

       //IP & JC - 12/01/09 - CR976 - Method which will retrieve the details for 
       //combined Ready Finance accounts for a Customer to be displayed
       //on the 'Special Arrangements Consolidated' screen.
       public void AccountGetCombinedSPADetails(string custID)
       {
           try
           {
               _combinedSPADetails = new DataSet();

               parmArray = new SqlParameter[1];
               parmArray[0] = new SqlParameter("@custid", SqlDbType.VarChar, 20);
               parmArray[0].Value = custID;

               RunSP("AccountGetCombinedSPADetailsSP", parmArray, _combinedSPADetails);

               _combinedSPADetails.Tables[0].TableName = TN.Arrangements;
  
           }
           catch (SqlException ex)
           {
               LogSqlException(ex);
               throw ex;
           }
       }

       public DataSet BailiffAllocationRulesLoad()
       {
           DataSet ds = new DataSet();
           try
           {
               this.RunSP("CM_BailiffAllocationRulesLoad", ds);

               ds.Tables[1].TableName = "ZoneBailiffAccs";
           }
           catch (SqlException ex)
           {
               LogSqlException(ex);
               throw ex;
           }
           return ds;

       }


       public DataSet GetRoles(int permission)
       {
           DataSet ds = new DataSet();
           try
           {
               parmArray = new SqlParameter[1];
               parmArray[0] = new SqlParameter("@permission", SqlDbType.Int);
               parmArray[0].Value = permission;
               this.RunSP("RolesGet", parmArray, ds);
           }
           catch (SqlException ex)
           {
               LogSqlException(ex);
               throw ex;
           }
           return ds;
       }

       //public void BailiffAllocationRulesSave(SqlConnection conn, SqlTransaction trans,
       //    int empeeno,string empeetype,string branchorZone, bool isZone,Int16 allocationorder, int empeenochange,
       //    bool reallocate )
       //{
       //    try
       //    {
               
               
       //        parmArray = new SqlParameter[7];
       //        parmArray[0] = new SqlParameter("@empeeno", SqlDbType.Int);
       //        parmArray[0].Value = empeeno;
       //        parmArray[1] = new SqlParameter("@empeetype", SqlDbType.VarChar,3 );
       //        parmArray[1].Value = empeetype;
       //        parmArray[2] = new SqlParameter("@branchorZone", SqlDbType.VarChar, 10);
       //        parmArray[2].Value = branchorZone;
       //        parmArray[3] = new SqlParameter("@isZone", SqlDbType.Bit);
       //        parmArray[3].Value = isZone;
       //        parmArray[4] = new SqlParameter("@allocationorder", SqlDbType.SmallInt);
       //        parmArray[4].Value = allocationorder;
       //        parmArray[5] = new SqlParameter("@empeenochange", SqlDbType.Int);
       //        parmArray[5].Value = empeenochange;
       //        parmArray[6] = new SqlParameter("@reallocate", SqlDbType.Bit);
       //        parmArray[6].Value = reallocate;



       //        RunSP("CM_BailiffAllocationRulesSave", parmArray );


       //    }
       //    catch (SqlException ex)
       //    {
       //        LogSqlException(ex);
       //        throw ex;
       //    }
       //}

   }
}
/*
(SqlConnection conn, SqlTransaction trans, string custid, string HTelNo, string HDialCode, string WTelNo,
           string WDialCode, string MTelNo, string MDialCode, int Empeeno, bool HomeTelephoneChanged, bool WorkTelephoneChanged, bool MobileTelephoneChanged)
*/