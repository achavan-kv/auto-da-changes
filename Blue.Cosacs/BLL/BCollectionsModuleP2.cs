using System;
using STL.Common;
using System.Diagnostics;
using System.Configuration;
using System.Xml;
using System.IO;
using System.Xml.Schema;
using STL.BLL.CreditBureau;
using System.Xml.Xsl;
using System.Data;
using System.Data.SqlClient;
using STL.DAL;
using STL.Common.Constants.TableNames;
using STL.Common.Constants.ColumnNames;

namespace STL.BLL
{
   public partial class BCollectionsModule : CommonObject
   {
      public DataSet GetConditions()
      {

         DCollectionsModule collections = new DCollectionsModule();

         DataSet ds = new DataSet();
         ds = collections.GetConditions();
         
         return ds;

      }

      public DataSet GetActions()
      {
         DCollectionsModule collections = new DCollectionsModule();

         DataSet ds = new DataSet();
         ds = collections.GetLoadActions();

         return ds;
      }


      public DataSet GetActionsWithStrategy()
      {
          DCollectionsModule collections = new DCollectionsModule();

          DataSet ds = new DataSet();
          ds = collections.GetActions();

          return ds;
      }

      public DataSet GetStrategyActions(string strategy)
      {

         DCollectionsModule collections = new DCollectionsModule();

         DataSet ds = new DataSet();
         ds = collections.GetStrategyActions(strategy);

         return ds;

      }

      public DataSet GetStrategyConditions(string strategy)
      {

         DCollectionsModule collections = new DCollectionsModule();

         DataSet ds = new DataSet();
         ds = collections.GetStrategyConditions(strategy);

         return ds;

      }

      public DataSet GetStrategies()
      {
         DCollectionsModule collections = new DCollectionsModule();

         DataSet ds = new DataSet();
         ds = collections.GetStrategies();

         return ds;
      }

      //IP - 20/10/08 - UAT5.2 - UAT(551)
      //Return the 'Exit Condition Strategies' for the selected strategy.
      public DataSet GetStrategiesToSendTo(string strategy)
      {
          DCollectionsModule collections = new DCollectionsModule();

          DataSet dsStrategiesToSendTo = new DataSet();
          dsStrategiesToSendTo = collections.GetStrategiesToSendTo(strategy);

          return dsStrategiesToSendTo;
      }

      public DataSet GetStrategiesForBailiff()
      {
         DCollectionsModule collections = new DCollectionsModule();

         DataSet ds = new DataSet();
         ds = collections.GetStrategiesForBailiff();

         return ds;
      }

      public DataSet GetLetters()
      {
         DCollectionsModule collections = new DCollectionsModule();

         DataSet ds = new DataSet();
         ds = collections.GetLetters();

         return ds;
      }

      public DataSet GetSMS()
      {
         DCollectionsModule collections = new DCollectionsModule();

         DataSet ds = new DataSet();
         ds = collections.GetSMS();

         return ds;
      }

      public DataSet GetWorklist()
      {
         DCollectionsModule collections = new DCollectionsModule();

         DataSet ds = new DataSet();
         ds = collections.GetWorklist();

         return ds;
      }

      public DataSet GetStrategyWorklists()
      {
         DCollectionsModule collections = new DCollectionsModule();

         DataSet ds = new DataSet();
         ds = collections.GetStrategyWorkLists();

         return ds;
      }

      //IP - 02/06/09 - Credit Collection Walkthrough Changes
      //Added canBeAllocated for Allocation Check
      public void SaveStrategyCondition(SqlConnection conn, SqlTransaction trans, string strategy, string description, DataSet dsStrategy, bool canBeAllocated, bool manual, int empeeno)
      {          
           DCollectionsModule collections = new DCollectionsModule();
           if (description != String.Empty)
           {
               collections.SaveStrategy(conn, trans, strategy, description, manual);
           }
           else
           {
               //IP - 21/09/09 - UAT(856) - //Method that updates an existing strategy as manual or not manual.
               collections.UpdateStrategyManualCheck(conn, trans, strategy, manual);
           }

           //Update the strategy as a strategy that can have its accounts allocated to a Bailiff / Collector.
           collections.SaveStrategyAllocationCheck(conn, trans, strategy, canBeAllocated);

        

           //First remove all data from the database table for the strategy being passed
           if (dsStrategy != null)
           {
              collections.DeleteStrategy(conn, trans, strategy);
              collections.DeleteStrategyActions(conn, trans, strategy);
           }

           foreach (DataTable dtStrategy in dsStrategy.Tables)
           {
              if (dtStrategy.Rows.Count > 0)
              {
                 foreach (DataRow row in dtStrategy.Rows)
                 {
                    if (dtStrategy.TableName != TN.ChosenActions)
                    {
                    collections.condition = row[CN.ConditionCode].ToString();
                    collections.actionCode = row[CN.ActionCode].ToString();
                    collections.nextStepFalse = row[CN.NextStepFalse];
                    collections.nextStepTrue = row[CN.NextStepTrue];
                    collections.operand = row[CN.Operand].ToString();
                    collections.operator1 = row[CN.Operator1].ToString();
                    collections.operator2 = row[CN.Operator2].ToString();
                    collections.orClause = row[CN.OrClause].ToString();
                    collections.step = row[CN.Step];
                    collections.stepActionType = row[CN.StepActionType].ToString();
                    collections.savedType = row[CN.SavedType].ToString();

                    collections.SaveStrategyCondition(conn, trans, strategy);
                    }
                    else
                    {
                       collections.action = row[CN.Action].ToString();
                       collections.strategyAction = row[CN.ActionCode].ToString();
                       //collections

                       collections.SaveStrategyActions(conn, trans, strategy, empeeno);     //UAT987
                    }
                 }
              }
           }

           collections.DeleteStrategyActionsRights(conn, trans, strategy);     //UAT987 jec Delete Action rights for removed Actions

          // Now Apply the triggers which will auto assign strategies given certain actions on the database tables
           collections.CreateStrategyTriggers(conn, trans);
       }

      //NM & IP - 06/01/09 - CR976 - Included 'worklist' parameter, for manually sending an account to a worklist.
      //NM & IP - 07/01/09 - CR976 - Extra Telephone Actions - 'STW' (Send to WriteOff) - added parameters 'reasonForWriteOff' and 'empeeno'
      public void UpdateStrategyAccounts(SqlConnection conn, SqlTransaction trans, string acct, string strategy, string worklist, string reasonForWriteOff, int empeeno)
      {
         DCollectionsModule collections = new DCollectionsModule();
         collections.UpdateStrategyAccounts(conn, trans, acct, strategy, worklist, reasonForWriteOff, empeeno); 
      }

      public void SetStrategyActive(SqlConnection conn, SqlTransaction trans, string strategy, int activeValue)
      {
         DCollectionsModule collections = new DCollectionsModule();

         collections.SetStrategyActive(conn, trans, strategy, activeValue);
      }

      public void DeleteWorkList(SqlConnection conn, SqlTransaction trans, string worklist)
      {
         DCollectionsModule collections = new DCollectionsModule();

         collections.DeleteWorkList(conn, trans, worklist);
         collections.DeleteWorkListActions(conn, trans, worklist);
      }

      public void DeleteSMS(SqlConnection conn, SqlTransaction trans, string sms)
      {
         DCollectionsModule collections = new DCollectionsModule();

         collections.DeleteSMS(conn, trans, sms);
         
      }

      public void LockAccount(SqlConnection conn, SqlTransaction trans, string acct, int user)
      {
         DCollectionsModule collections = new DCollectionsModule();

         collections.LockAccount(conn, trans,acct,user);

      }

      public void UnlockAccount(SqlConnection conn, SqlTransaction trans, string acct, int user)
      {
         DCollectionsModule collections = new DCollectionsModule();

         collections.UnlockAccount(conn, trans, acct, user);

      }

      //IP - UAT(514) - Delete the selected existing Strategy 
      public void DeleteExistingStrategy(SqlConnection conn, SqlTransaction trans, string strategy)
      {
         DCollectionsModule collections = new DCollectionsModule();

         collections.DeleteExistingStrategy(conn, trans, strategy);
        
      }

      //public void SaveWorklistRights(SqlConnection conn, SqlTransaction trans, 
      //    int Employee, string Worklist, string EmployeeType, int user)
      //{
      //    DCollectionsModule collections = new DCollectionsModule();

      //    collections.SaveWorklistRights(conn, trans, Employee, Worklist, EmployeeType, user);

      //}

        //public void SaveActionRights(SqlConnection conn, SqlTransaction trans,
        // int Employee,  string Strategy, string Action, string EmployeeType, bool CycleToNextFlag,
        // int MinNotesLength, int user)
        //  {
        //       DCollectionsModule collections = new DCollectionsModule();

        //       collections.SaveActionRights(conn, trans, Employee, Strategy, Action, EmployeeType,
        //           CycleToNextFlag, MinNotesLength, user);
        //}

        public DataSet LoadAllocateableStaffandTypes()
        {
             DCollectionsModule collections = new DCollectionsModule();
             DataSet ds = new DataSet();
             ds = collections.LoadAllocateableStaffandTypes();

             return ds;

        }

        //public void UpdateCycleToforActions(SqlConnection conn, SqlTransaction trans, string strategy,
        //    string actioncode, bool CycleToNextFlag)
        //{
        //    DCollectionsModule collections = new DCollectionsModule();
        //    collections.UpdateCycleToforActions(conn, trans, strategy, actioncode, CycleToNextFlag);
        //}

        public void ApplyZones(SqlConnection conn, SqlTransaction trans, string Zone)
        {
            var collections = new DCollectionsModule();
            collections.ApplyZones(conn, trans, Zone);
        }

        public DataSet LoadUnallocatedAddressZones()
        {
            DataSet ds = new DataSet();
            var collections = new DCollectionsModule();
            ds = collections.LoadUnallocatedAddressZones();
            return ds;
        }

        public DataSet LoadZoneRules(string zone)
        {
            DataSet ds = new DataSet();
            var collections = new DCollectionsModule();
            return ds = collections.LoadZoneRules(zone);
            
            
        }

        public DataSet BailiffAllocationRulesLoad()
        {
            DataSet ds = new DataSet();
            var collections = new DCollectionsModule();
            return ds = collections.BailiffAllocationRulesLoad();
        }

        //public void BailiffAllocationRulesSave(SqlConnection conn, SqlTransaction trans,
        //       int empeeno, string empeetype, string branchorZone, bool isZone, Int16 allocationorder, int empeenochange,
        //        bool reallocate)
        //{
        //    DCollectionsModule collections = new DCollectionsModule();

        //    collections.BailiffAllocationRulesSave(conn, trans,empeeno,empeetype,branchorZone,isZone,allocationorder,empeenochange,reallocate);
        //}

        public DataSet GetFollupAllocForActionSheet(string acctNo, int empeeNo)
        {
            DCollectionsModule collections = new DCollectionsModule();
            return collections.GetFollupAllocForActionSheet(acctNo, empeeNo);
        }

   }
}
