using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Configuration;
using STL.Common;
using STL.Common.Constants.Enums;
using STL.Common.Constants.TableNames;
using STL.Common.Constants.ColumnNames;
using STL.Common.Constants.EOD;
using System.Diagnostics;
using System.Text;
using System.IO;


namespace STL.DAL
{
   public partial class DCollectionsModule : DALObject
   {
      private string m_condition;
      private string m_operand;
      private string m_operator1;
      private string m_operator2;
      private string m_orClause;
      private object m_nextStepTrue;
      private object m_nextStepFalse;
      private string m_actionCode;
      private string m_stepActionType;
      private object m_step;
      private string m_action;
      private string m_startegyAction;
      private string m_savedType;

      public string condition
      {
         get
         {
            return m_condition;
         }
         set
         {
            m_condition = value;
         }
      }

      public string operand
      {
         get
         {
            return m_operand;
         }
         set
         {
            m_operand = value;
         }
      }

      public string operator1
      {
         get
         {
            return m_operator1;
         }
         set
         {
            m_operator1 = value;
         }
      }

      public string operator2
      {
         get
         {
            return m_operator2;
         }
         set
         {
            m_operator2 = value;
         }
      }

      public string orClause
      {
         get
         {
            return m_orClause;
         }
         set
         {
            m_orClause = value;
         }
      }

      public string actionCode
      {
         get
         {
            return m_actionCode;
         }
         set
         {
            m_actionCode = value;
         }
      }

      public string stepActionType
      {
         get
         {
            return m_stepActionType;
         }
         set
         {
            m_stepActionType = value;
         }
      }

      public object nextStepTrue
      {
         get
         {
            return m_nextStepTrue;
         }
         set
         {
            m_nextStepTrue = value;
         }
      }

      public object nextStepFalse
      {
         get
         {
            return m_nextStepFalse;
         }
         set
         {
            m_nextStepFalse = value;
         }
      }

      public object step
      {
         get
         {
            return m_step;
         }
         set
         {
            m_step = value;
         }
      }

      public string action
      {
         get
         {
            return m_action;
         }
         set
         {
            m_action = value;
         }
      }

      public string strategyAction
      {
         get
         {
            return m_startegyAction;
         }
         set
         {
            m_startegyAction = value;
         }
      }

      public string savedType
      {
         get
         {
            return m_savedType;
         }
         set
         {
            m_savedType = value;
         }
      }

       public int runno;
      

      public DataSet GetConditions()
        {
            DataSet dsConditions = new DataSet();
            try
            {
               this.RunSP("CM_ConditionLoadSP", dsConditions);          
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return dsConditions;

        }

      public DataSet GetLoadActions()
      {
         DataSet dsActions = new DataSet();
         try
         {
            this.RunSP("CM_LoadActionsSP", dsActions);
         }
         catch (SqlException ex)
         {
            LogSqlException(ex);
            throw ex;
         }
         return dsActions;

      }

      public DataSet GetStrategyActions(string strategy)
      {
         DataSet dsStrategyActions = new DataSet();
         try
         {
            parmArray = new SqlParameter[1];
            parmArray[0] = new SqlParameter("@strategy", SqlDbType.VarChar, 7);           // #9521 jec 03/02/12
            parmArray[0].Value = strategy;

            this.RunSP("CM_LoadStrategyActionsSP", parmArray, dsStrategyActions);
         }
         catch (SqlException ex)
         {
            LogSqlException(ex);
            throw ex;
         }
         return dsStrategyActions;
      }

      public DataSet GetStrategyConditions(string strategy)
      {
         DataSet dsStrategyConditions = new DataSet();
         try
         {
               parmArray = new SqlParameter[1];
               parmArray[0] = new SqlParameter("@strategy", SqlDbType.VarChar, 7);           // #9521 jec 03/02/12
               parmArray[0].Value = strategy;

               this.RunSP("CM_StrategyConditionGetSP", parmArray, dsStrategyConditions);
         }
         catch (SqlException ex)
         {
            LogSqlException(ex);
            throw ex;
         }
         return dsStrategyConditions;

      }


      public DataSet GetStrategies()
      {
         DataSet dsStrategies = new DataSet();
         try
         {
            this.RunSP("CM_StrategyLoadSP", dsStrategies);
         }
         catch (SqlException ex)
         {
            LogSqlException(ex);
            throw ex;
         }
         return dsStrategies;
      }

       //IP - 20/10/08 - UAT5.2 - UAT(551) 
       //Return the 'Exit Condition Strategies' for the selected strategy.
       public DataSet GetStrategiesToSendTo(string strategy)
       {
           DataSet dsStrategiesToSendTo = new DataSet();
           try
           {
               parmArray = new SqlParameter[1];
               parmArray[0] = new SqlParameter("@strategy", SqlDbType.VarChar, 7);           // #9521 jec 03/02/12
               parmArray[0].Value = strategy;

               this.RunSP("CM_GetStrategiesToSendToSP", parmArray, dsStrategiesToSendTo);
           }
           catch (SqlException ex)
           {
               LogSqlException(ex);
               throw ex;
           }
           return dsStrategiesToSendTo;
       }

      public DataSet GetStrategiesForBailiff()
      {
         DataSet dsStrategies = new DataSet();
         try
         {
            this.RunSP("CM_StrategyLoadForBailiffSP", dsStrategies);
         }
         catch (SqlException ex)
         {
            LogSqlException(ex);
            throw ex;
         }
         return dsStrategies;
      }

      public DataSet GetLetters()
      {
         DataSet dsLetters = new DataSet();
         try
         {
            this.RunSP("CM_GetLettersSP", dsLetters);
         }
         catch (SqlException ex)
         {
            LogSqlException(ex);
            throw ex;
         }
         return dsLetters;
      }

      public DataSet GetSMS()
      {
         DataSet dsSMS = new DataSet();
         try
         {
            this.RunSP("CM_GetSMSSP", dsSMS);
         }
         catch (SqlException ex)
         {
            LogSqlException(ex);
            throw ex;
         }
         return dsSMS;
      }

      public DataSet GetWorklist()
      {
         DataSet dsWorklist = new DataSet();
         try
         {
            this.RunSP("CM_GetWorklistsSP", dsWorklist);
         }
         catch (SqlException ex)
         {
            LogSqlException(ex);
            throw ex;
         }
         return dsWorklist;
      }

      public DataSet GetStrategyWorkLists()
      {
         DataSet dsStrategyWorklist = new DataSet();
         try
         {
            this.RunSP("CM_GetStrategyWorkListsSP", dsStrategyWorklist);
         }
         catch (SqlException ex)
         {
            LogSqlException(ex);
            throw ex;
         }
         return dsStrategyWorklist;
      }

      public void SaveStrategyCondition(SqlConnection conn, SqlTransaction trans, string strategy )
      {
         try
           {
               parmArray = new SqlParameter[12];
               parmArray[0] = new SqlParameter("@Strategy", SqlDbType.VarChar, 7);           // #9521 jec 03/02/12
               parmArray[0].Value = strategy;
               parmArray[1] = new SqlParameter("@Condition", SqlDbType.VarChar, 12);
               parmArray[1].Value = condition;
               parmArray[2] = new SqlParameter("@Operand", SqlDbType.VarChar, 10);
               if (operand == "")
               {
                  parmArray[2].Value = DBNull.Value;
               }
               else
               {
                  parmArray[2].Value = operand;
               }
               parmArray[3] = new SqlParameter("@Operator1", SqlDbType.NVarChar, 24);
               if (operator1 == "")
               {
               parmArray[3].Value = DBNull.Value;
               }
               else
               {
                  parmArray[3].Value = operator1;
               }
               parmArray[4] = new SqlParameter("@Operator2", SqlDbType.NVarChar, 24);
               if (operator2 == "")
               {
                  parmArray[4].Value = DBNull.Value;
               }
               else
               {
                  parmArray[4].Value = operator2;
               }
               parmArray[5] = new SqlParameter("@OrClause", SqlDbType.Char, 1);
               if (orClause == "")
               {
                  parmArray[5].Value = DBNull.Value;
               }
               else
               {
                  parmArray[5].Value = orClause;
               }
               parmArray[6] = new SqlParameter("@NextStepTrue", SqlDbType.SmallInt);
               parmArray[6].Value = nextStepTrue;
               parmArray[7] = new SqlParameter("@NextStepFalse", SqlDbType.SmallInt);
               parmArray[7].Value = nextStepFalse;
               parmArray[8] = new SqlParameter("@ActionCode", SqlDbType.VarChar, 50);
               if (actionCode == "")
               {
                  parmArray[8].Value = DBNull.Value;
               }
               else
               {
                  parmArray[8].Value = actionCode;
               }
               parmArray[9] = new SqlParameter("@StepActionType", SqlDbType.Char, 1);
               if (stepActionType == "")
               {
                  parmArray[9].Value = DBNull.Value;
               }
               else
               {
                  parmArray[9].Value = stepActionType;
               }
               parmArray[10] = new SqlParameter("@Step", SqlDbType.SmallInt);
               parmArray[10].Value = step;
               parmArray[11] = new SqlParameter("@SavedType",SqlDbType.Char,1);
               parmArray[11].Value = savedType;
            
               this.RunSP(conn, trans, "CM_StrategyConditionSaveSP", parmArray);
           }
           catch (SqlException ex)
           {
               LogSqlException(ex);
               throw ex;
           }
      }

      public void SaveStrategyActions(SqlConnection conn, SqlTransaction trans, string strategy, int empeeno)       //UAT987
      {
         try
         {
             parmArray = new SqlParameter[4];                   //UAT987
             parmArray[0] = new SqlParameter("@strategy", SqlDbType.VarChar, 7);           // #9521 jec 03/02/12
            parmArray[0].Value = strategy;
            parmArray[1] = new SqlParameter("@actioncode", SqlDbType.VarChar, 12);
            parmArray[1].Value = strategyAction;
            parmArray[2] = new SqlParameter("@action", SqlDbType.VarChar, 64);
            parmArray[2].Value = action;
            parmArray[3] = new SqlParameter("@empeeno", SqlDbType.Int);                 //UAT987
            parmArray[3].Value = empeeno;

            this.RunSP(conn, trans, "CM_SaveStrategyActionsSP", parmArray);
         }
         catch (SqlException ex)
         {
            LogSqlException(ex);
            throw ex;
         }
      }
    
      public void DeleteStrategyActionsRights(SqlConnection conn, SqlTransaction trans, string strategy)       //UAT987
      {
          try
          {
              parmArray = new SqlParameter[1];
              parmArray[0] = new SqlParameter("@strategy", SqlDbType.VarChar, 7);           // #9521 jec 03/02/12
              parmArray[0].Value = strategy;

              this.RunSP(conn, trans, "CM_DeleteStrategyActionRights", parmArray);
          }
          catch (SqlException ex)
          {
              LogSqlException(ex);
              throw ex;
          }
      }

      public void SaveStrategy(SqlConnection conn, SqlTransaction trans, string strategy, string description, bool manual)
      {
         try
         {
            parmArray = new SqlParameter[3];
            parmArray[0] = new SqlParameter("@strategy", SqlDbType.VarChar, 7);           // #9521 jec 03/02/12
            parmArray[0].Value = strategy;
            parmArray[1] = new SqlParameter("@description", SqlDbType.NVarChar, 128);
            parmArray[1].Value = description;
            parmArray[2] = new SqlParameter("@manual", SqlDbType.Int);
            parmArray[2].Value = manual;

            this.RunSP(conn, trans, "CM_SaveStrategySP", parmArray);
         }
         catch (SqlException ex)
         {
            LogSqlException(ex);
            throw ex;
         }
      }

      public void DeleteStrategy(SqlConnection conn, SqlTransaction trans, string strategy)
      {
         try
         {
            parmArray = new SqlParameter[1];
            parmArray[0] = new SqlParameter("@strategy", SqlDbType.VarChar, 7);           // #9521 jec 03/02/12
            parmArray[0].Value = strategy;
            //parmArray[1] = new SqlParameter("@type", SqlDbType.Char, 1);
            //parmArray[1].Value = type;

            this.RunSP(conn, trans, "CM_DeleteStrategyConditionSP", parmArray);
         }
         catch (SqlException ex)
         {
            LogSqlException(ex);
            throw ex;
         }
      }

      public void DeleteStrategyActions(SqlConnection conn, SqlTransaction trans, string strategy)
      {
         try
         {
            parmArray = new SqlParameter[1];
            parmArray[0] = new SqlParameter("@strategy", SqlDbType.VarChar, 7);           // #9521 jec 03/02/12
            parmArray[0].Value = strategy;

            this.RunSP(conn, trans, "CM_DeleteStrategyActionsSP", parmArray);
         }
         catch (SqlException ex)
         {
            LogSqlException(ex);
            throw ex;
         }
      }

       public string DoEndofDayStrategies()
       {
           string eodResult = EODResult.Pass;
           using (SqlConnection conn = new SqlConnection(Connections.Default))
           {
               try
               {
                   conn.Open();
                   using (SqlTransaction trans = conn.BeginTransaction())
                   {
                       this.RunSP(conn, trans, "CMEod_Strategies");
                       trans.Commit();
                   }
               }
               catch (SqlException ex)
               {
                   LogSqlException(ex);
                   throw ex;
               }
               finally
               {

                   string progress = "Finished Collections Module ";
                   Console.WriteLine(progress);

               }
           }

           return eodResult;    
       }
       // Create SMS csv data file
       public string CreateSMSdatafile(DateTime rundate)                   //IP - 16/04/12 - #9929
       {
           //SqlConnection conn = null;
           //SqlTransaction trans = null;
           const string eodResult = EODResult.Pass;
           try
           {
               using (var conn = new SqlConnection(Connections.Default))
               {
                   conn.Open();
                   using (var trans = conn.BeginTransaction())
                   {
                       parmArray = new SqlParameter[2];
                       parmArray[0] = new SqlParameter("@runno", SqlDbType.Int)
                                          {
                                              Value = " ",
                                              Direction = ParameterDirection.Output
                                          };
                       parmArray[1] = new SqlParameter("@rundate", SqlDbType.DateTime);     //IP - 16/04/12 - #9929
                       parmArray[1].Value = rundate;                                        //IP - 16/04/12 - #9929

                       this.RunSP(conn, trans, "CMEod_CreateSMSdatafileSP", parmArray);

                       
                       if (parmArray[0].Value != DBNull.Value)
                           runno = Convert.ToInt32(parmArray[0].Value);

                       trans.Commit();

                   }
               }

               ExportSMS();
           }
           catch (SqlException ex)
           {
               LogSqlException(ex);
               throw;
           }
           finally
           {
               const string progress = "Finished SMS datafile ";
               Console.WriteLine(progress);
           }

           return eodResult;
       }

       private void ExportSMS()
       {
           string progress = "Exporting SMS data file...";
           Console.WriteLine(progress);

           //IP - 17/07/08 - UAT5.2 - UAT(24) - The SMS Data file was not being created.
           //Changed the below so that the server is cotrrectly retrieved from the connection string
           //which was needed for the BCP

           //string dbname = this.GetDatabaseName();

           //try
           //{
           //    System.Diagnostics.Process proc = new System.Diagnostics.Process();
           //    proc.EnableRaisingEvents = false;

           //    proc.StartInfo.WorkingDirectory = @"c:\program files\microsoft sql server\80\tools\binn\";
           //    proc.StartInfo.FileName = "bcp.exe";
           //    proc.StartInfo.Arguments = dbname + @".dbo.SMSdatafile out d:\users\default\SMSmessage_" + Convert.ToString(runno) + ".csv -c -t, -q -T";
           //    proc.StartInfo.UseShellExecute = true;
           //    proc.Start();

           try
           {
               string server = string.Empty;
               string username = string.Empty;
               string password = string.Empty;
               string dbname = string.Empty;
               string integrated = null;

               //Retrieve the connection string 
               string conn = Connections.Default;
               string[] connParts = conn.Split(';');
               foreach (string connPart in connParts)
               {
                   string[] splitPart = connPart.Split('=');

                   switch (splitPart[0].ToLower().Trim())
                   {
                       case "server":
                       case "data source":
                           server = splitPart[1];
                           break;
                       case "uid":
                       case "user id":
                           username = splitPart[1];
                           break;
                       case "pwd":
                       case "password":
                           password = splitPart[1];
                           break;
                       case "database":
                       case "initial catalog":
                           dbname = splitPart[1];
                           break;
                       case "integrated security":
                           integrated = splitPart[1];
                           break;
                   }
               }

               System.Diagnostics.Process proc = new System.Diagnostics.Process();
               proc.EnableRaisingEvents = false;

               var bcpPath = Convert.ToString(Country[CountryParameterNames.BCPpath]); //IP - 21/03/11 - #3352
               var sysDriveDirectory = Convert.ToString(Country[CountryParameterNames.SystemDrive]); //IP - 21/03/11 - #3352

               //proc.StartInfo.WorkingDirectory = @"c:\program files\microsoft sql server\100\tools\binn\";
               proc.StartInfo.WorkingDirectory = @bcpPath; //IP - 21/03/11 - #3352

               proc.StartInfo.FileName = "bcp";
               //proc.StartInfo.Arguments = dbname + @".dbo.SMSdatafile out d:\users\default\SMSmessage_" + Convert.ToString(runno) + ".csv   -c -t, -q -U" + username + " -P" + password + " -S" + server;
                
               var path = Path.Combine(sysDriveDirectory, "SMSmessage_" + Convert.ToString(runno) + ".csv"); //IP - 21/03/11 - #3352
               //var security = !string.IsNullOrEmpty(integrated) ? " -T " : " -U" + username + " -P" + password;
               var security = (!string.IsNullOrEmpty(integrated) && integrated.ToLower()=="true") ? " -T " : " -U" + username + " -P" + password;
               proc.StartInfo.Arguments = dbname + @".dbo.SMSdatafile out " + path + " -c -t, -q " + security + " -S" + server; //IP - 21/03/11 - #3352
               //proc.StartInfo.UseShellExecute = false;
               //proc.StartInfo.RedirectStandardOutput = true;
               //proc.StartInfo.RedirectStandardError = true;
               proc.StartInfo.UseShellExecute = true; 

               proc.Start();

               //string output = proc.StandardOutput.ReadToEnd();
               //string errormess = proc.StandardError.ReadToEnd();
               proc.WaitForExit();
           }
           catch (SqlException ex)
           {
               LogSqlException(ex);
               throw ex;
           }

       }

       public void CreateStrategyTriggers (SqlConnection conn, SqlTransaction trans)
       {
           try
           {
               this.RunSP(conn, trans, "CM_GenerateCollectionsTriggersSP");
           }
           catch (SqlException ex)
           {
               LogSqlException(ex);
               throw ex;
           }
       }


      public void SetStrategyActive(SqlConnection conn, SqlTransaction trans, string strategy,int activeValue)
      {
         try
         {
            parmArray = new SqlParameter[2];
            parmArray[0] = new SqlParameter("@strategy", SqlDbType.VarChar, 7);           // #9521 jec 03/02/12
            parmArray[0].Value = strategy;
            parmArray[1] = new SqlParameter("@activeValue", SqlDbType.Char, 1);
            parmArray[1].Value = activeValue;

            this.RunSP(conn, trans, "CM_SetStrategyActiveSP", parmArray);
         }
         catch (SqlException ex)
         {
            LogSqlException(ex);
            throw ex;
         }
      }

      public void LockAccount(SqlConnection conn, SqlTransaction trans, string acct, int user)
      {
         try
         {
            parmArray = new SqlParameter[2];
            parmArray[0] = new SqlParameter("@acctno", SqlDbType.VarChar, 12);
            parmArray[0].Value = acct;
            parmArray[1] = new SqlParameter("@user", SqlDbType.Int);
            parmArray[1].Value = user;

            //this.RunSP(conn, trans, "CM_LockAccountSP", parmArray);
            this.RunSP(conn, trans, "DN_LockAccountSP", parmArray);         //UAT945 jec 06/01/10
         }
         catch (SqlException ex)
         {
            LogSqlException(ex);
            throw ex;
         }
      }

      public void UnlockAccount(SqlConnection conn, SqlTransaction trans, string acct, int user)
      {
         try
         {
            parmArray = new SqlParameter[2];
            parmArray[0] = new SqlParameter("@acctno", SqlDbType.VarChar, 12);
            parmArray[0].Value = acct;
            parmArray[1] = new SqlParameter("@user", SqlDbType.Int);
            parmArray[1].Value = user;

            //this.RunSP(conn, trans, "CM_UnlockAccountSP", parmArray);
            this.RunSP(conn, trans, "DN_UnlockAccountSP", parmArray);       //UAT945 jec 06/01/10
         }
         catch (SqlException ex)
         {
            LogSqlException(ex);
            throw ex;
         }
      }

      // Get database name from configuration settings
      //private string GetDatabaseName()
      //{
      //    /*
      //    int start = 0;
      //    int end = 0;
      //    string db = "";

      //    string conn = Connections.Default;
      //    start = conn.IndexOf(";database") + 10;
      //    end = conn.Length; //conn.IndexOf(";", start);
      //    db = conn.Substring(start, (end - start));

      //    return db;*/
      //    return Connections.DefaultDatabaseName;
      //}

      //IP - UAT(514) - Delete the selected existing strategy
      public void DeleteExistingStrategy(SqlConnection conn, SqlTransaction trans, string strategy)
      {
          try
          {
              parmArray = new SqlParameter[1];
              parmArray[0] = new SqlParameter("@strategy", SqlDbType.VarChar, 7);           // #9521 jec 03/02/12
              parmArray[0].Value = strategy;
              this.RunSP(conn, trans, "CM_DeleteExistingStrategySP", parmArray);
          }
          catch (SqlException ex)
          {
              LogSqlException(ex);
              throw ex;
          }
      }

      // public void SaveWorklistRights(SqlConnection conn, SqlTransaction trans, 
      //int Employee, string Worklist, string EmployeeType, int user)
      //{
      //    try
      //    {
      //        parmArray = new SqlParameter[4];
      //        parmArray[0] = new SqlParameter("@EmpeeNo", SqlDbType.Int);
      //        parmArray[0].Value = Employee;
      //        parmArray[1] = new SqlParameter("@Worklist", SqlDbType.VarChar, 10);
      //        parmArray[1].Value = Worklist;
      //        parmArray[2] = new SqlParameter("@EmpeeType", SqlDbType.VarChar, 10);
      //        parmArray[2].Value = EmployeeType;
      //        parmArray[3] = new SqlParameter("@Empeenochange", SqlDbType.Int);
      //        parmArray[3].Value = user;

      //        this.RunSP(conn, trans, "CM_SaveWorklistRightsSP", parmArray);
      //    }
      //    catch (SqlException ex)
      //    {
      //        LogSqlException(ex);
      //        throw ex;
      //    }
      //}

     //  public void SaveActionRights(SqlConnection conn, SqlTransaction trans,
     //int Employee,  string Strategy, string Action, string EmployeeType, bool CycleToNextFlag,
     //int MinNotesLength, int user)
     // {
     //     try
     //     {
     //         parmArray = new SqlParameter[7];
     //         parmArray[0] = new SqlParameter("@EmpeeNo", SqlDbType.Int);
     //         parmArray[0].Value = Employee;
     //         parmArray[1] = new SqlParameter("@Strategy", SqlDbType.VarChar, 10);
     //         parmArray[1].Value = Strategy;
     //         parmArray[2] = new SqlParameter("@Action", SqlDbType.VarChar, 10);
     //         parmArray[2].Value = Action;
     //         parmArray[3] = new SqlParameter("@EmpeeType", SqlDbType.VarChar, 10);
     //         parmArray[3].Value = EmployeeType;
     //         parmArray[4] = new SqlParameter("@Empeenochange", SqlDbType.Int);
     //         parmArray[4].Value = user;
     //         parmArray[5] = new SqlParameter("@CycleToNextFlag", SqlDbType.Bit);
     //         parmArray[5].Value = CycleToNextFlag;
     //         parmArray[6] = new SqlParameter("@MinNotesLength", SqlDbType.Int);
     //         parmArray[6].Value = MinNotesLength;

     //         this.RunSP(conn, trans, "CM_SaveActionRightsSP", parmArray);
     //     }
     //     catch (SqlException ex)
     //     {
     //         LogSqlException(ex);
     //         throw ex;
     //     }
     // }


       public DataSet LoadAllocateableStaffandTypes()
       {
           DataSet Staff = new DataSet();
           try
           {
               this.RunSP("CM_LoadAllocatetableStaffandTypesSP", Staff);
           }
           catch (SqlException ex)
           {
               LogSqlException(ex);
               throw ex;
           }
           return Staff;

       }

       public DataSet GetZones()
       {
           DataSet ds = new DataSet();
           try
           {
               this.RunSP("CM_GetZoneSP", ds);
           }
           catch (SqlException ex)
           {
               LogSqlException(ex);
               throw ex;
           }
           return ds;
       }

       public void SaveZones(SqlConnection conn, SqlTransaction trans, string zone, string description)
       {
           try
           {
               parmArray = new SqlParameter[2];
               parmArray[0] = new SqlParameter("@zone", SqlDbType.VarChar, 4);
               parmArray[0].Value = zone;
               parmArray[1] = new SqlParameter("@zoneDescription", SqlDbType.VarChar, 64);
               parmArray[1].Value = description;
             
               this.RunSP(conn, trans, "CM_SaveZoneSP", parmArray);
           }
           catch (SqlException ex)
           {
               LogSqlException(ex);
               throw ex;
           }
       }


       public DataSet GetZoneAllocatableEmployeeInfo()
       {
           DataSet ds = new DataSet();
           try
           {
               this.RunSP("CM_GetZoneAllocatableEmpeeInfo", ds);

               if (ds.Tables.Count > 0)
                   ds.Tables[0].TableName = TN.EmployeeTypes;
               if (ds.Tables.Count > 1)
                   ds.Tables[1].TableName = TN.BranchDetails;
               if (ds.Tables.Count > 2)
                   ds.Tables[2].TableName = TN.Employee;
           }
           catch (SqlException ex)
           {
               LogSqlException(ex);
               throw ex;
           }
           return ds;
       }

       public void SaveBailiffZoneAllocation(SqlConnection conn, SqlTransaction trans, int empeeNo, DataTable dtZoneAllocation, DataTable dtEmployeeUpdate, int user)
       {
           try
           {
               //--Update Employee Detail---------------------------------------------
               foreach (DataRow dr in dtEmployeeUpdate.Rows)
               {
                   parmArray = new SqlParameter[4];
                   parmArray[0] = new SqlParameter("@EmpeeNo", SqlDbType.Int);
                   parmArray[0].Value = Convert.ToInt32(dr[CN.EmployeeNo]);
                   parmArray[1] = new SqlParameter("@minAccounts", SqlDbType.Int);
                   parmArray[1].Value = Convert.ToInt32(dr["MinAccounts"]);
                   parmArray[2] = new SqlParameter("@maxAccounts", SqlDbType.Int);
                   parmArray[2].Value = Convert.ToInt32(dr["MaxAccounts"]);
                   parmArray[3] = new SqlParameter("@allocationRank", SqlDbType.SmallInt);
                   parmArray[3].Value = Convert.ToInt16(dr["AllocationRank"]);

                   this.RunSP(conn, trans, "CM_UpdateEmployeeBailiffAlloc", parmArray);
               }
               //---------------------------------------------------------------------

               //--Delete existing zones ---------------------------------------------
               parmArray = new SqlParameter[2];
               parmArray[0] = new SqlParameter("@EmpeeNo", SqlDbType.Int);
               parmArray[0].Value = empeeNo;
               parmArray[1] = new SqlParameter("@All", SqlDbType.Bit);
               parmArray[1].Value = (empeeNo == 0);

               this.RunSP(conn, trans, "CM_BailiffAllocationRulesDelete", parmArray);
               //---------------------------------------------------------------------

               //--Save new zones ----------------------------------------------------
               foreach (DataRow dr in dtZoneAllocation.Rows)
               {
                   parmArray = new SqlParameter[8];

                   parmArray[0] = new SqlParameter("@empeeno", SqlDbType.Int);
                   parmArray[0].Value = Convert.ToInt32(dr[CN.EmployeeNo]);
                   parmArray[1] = new SqlParameter("@empeetype ", SqlDbType.VarChar, 2);
                   parmArray[1].Value = dr[CN.EmployeeType].ToString();
                   parmArray[2] = new SqlParameter("@BranchorZone", SqlDbType.VarChar, 10);
                   parmArray[2].Value = dr["BranchorZone"].ToString(); ;
                   parmArray[3] = new SqlParameter("@IsZone", SqlDbType.Bit);
                   parmArray[3].Value = Convert.ToBoolean(dr["IsZone"]);
                   parmArray[4] = new SqlParameter("@AllocationOrder", SqlDbType.SmallInt);
                   parmArray[4].Value = Convert.ToInt16(dr["AllocationOrder"]);
                   parmArray[5] = new SqlParameter("@empeenochange", SqlDbType.Int);
                   parmArray[5].Value = user;
                   parmArray[6] = new SqlParameter("@datechange", SqlDbType.DateTime);
                   parmArray[6].Value = DateTime.Now;
                   parmArray[7] = new SqlParameter("@reallocate", SqlDbType.Bit);
                   parmArray[7].Value = Convert.ToBoolean(dr["reallocate"]);

                   this.RunSP(conn, trans, "CM_BailiffAllocationRulesSave", parmArray);
               }
           }
           catch (SqlException ex)
           {
               LogSqlException(ex);
               throw ex;
           }
       }

       //public void UpdateCycleToforActions(SqlConnection conn, SqlTransaction trans, string strategy,
       //  string actioncode, bool CycleToNextFlag)
       //{
       //    try
       //    {
       //        parmArray = new SqlParameter[3];
       //        parmArray[0] = new SqlParameter("@strategy", SqlDbType.VarChar, 7);           // #9521 jec 03/02/12
       //        parmArray[0].Value = strategy;
       //        parmArray[1] = new SqlParameter("@actioncode", SqlDbType.NChar, 3);
       //        parmArray[1].Value = actioncode;
       //        parmArray[2] = new SqlParameter("@CycleToNextFlag", SqlDbType.Bit);
       //        parmArray[2].Value = CycleToNextFlag;
       //        this.RunSP(conn, trans, "CM_UpdateCycleToforActionsSP", parmArray);
       //    }
       //    catch (SqlException ex)
       //    {
       //        LogSqlException(ex);
       //        throw ex;
       //    }
       //}

       public void SaveZoneRule(SqlConnection conn, SqlTransaction trans, string zone, DataTable dtZone)
       {
           try
           {
               //--Delete existing zones ---------------------------------------------
               parmArray = new SqlParameter[2];
               parmArray[0] = new SqlParameter("@Zone", SqlDbType.VarChar, 4);
               parmArray[0].Value = zone;
               parmArray[1] = new SqlParameter("@All", SqlDbType.Bit);
               parmArray[1].Value = (zone == "");

               this.RunSP(conn, trans, "CM_DeleteZoneRuleSP", parmArray);
               //---------------------------------------------------------------------

               //--Delete existing zones ---------------------------------------------
               foreach (DataRow dr in dtZone.Rows)
               {
                   parmArray = new SqlParameter[5];
                   parmArray[0] = new SqlParameter("@Zone", SqlDbType.VarChar, 4);
                   parmArray[0].Value = dr[CN.Zone].ToString();
                   parmArray[1] = new SqlParameter("@column_name ", SqlDbType.VarChar, 32);
                   parmArray[1].Value = dr[CN.Column_Name].ToString();
                   parmArray[2] = new SqlParameter("@query", SqlDbType.VarChar, 128);
                   parmArray[2].Value = dr[CN.Query].ToString(); ;
                   parmArray[3] = new SqlParameter("@or_clause", SqlDbType.VarChar, 2);
                   parmArray[3].Value = dr[CN.Or_Clause].ToString();
                   parmArray[4] = new SqlParameter("@notlike", SqlDbType.Bit);
                   parmArray[4].Value = Convert.ToBoolean(dr[CN.NotLike]);

                   this.RunSP(conn, trans, "CM_SaveZoneRuleSP", parmArray);
               }
           }
           catch (SqlException ex)
           {
               LogSqlException(ex);
               throw ex;
           }
       }

        public void DeleteZone(SqlConnection conn, SqlTransaction trans, string zone)
        {
            try
            {
                parmArray = new SqlParameter[1];
                parmArray[0] = new SqlParameter("@Zone", SqlDbType.VarChar, 4);
                if (zone != null) parmArray[0].Value = zone;

                this.RunSP(conn, trans, "CMDeleteZoneSP", parmArray);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }
  
        public void ApplyZones(SqlConnection conn, SqlTransaction trans, string zone)
          {
              try
              {
                  parmArray = new SqlParameter[1];
                  parmArray[0] = new SqlParameter("@Zone", SqlDbType.VarChar, 4);
                  if (zone != null) parmArray[0].Value = zone;

                  this.RunSP(conn, trans, "CM_ApplyZonesSP", parmArray);
              }
              catch (SqlException ex)
              {
                  LogSqlException(ex);
                  throw ex;
              }
          }

        public DataSet LoadUnallocatedAddressZones()
         {
              DataSet ds = new DataSet();
              try
              {

                  this.RunSP("CM_LoadUnallocatedAddressZones", ds);
              }
              catch (SqlException ex)
              {
                  LogSqlException(ex);
                  throw ex;
              }
              return ds;
          }

        public DataSet LoadZoneRules(string Zone)
          {
              DataSet ds = new DataSet();

              try
              {
                  parmArray = new SqlParameter[1];
                  parmArray[0] = new SqlParameter("@Zone", SqlDbType.VarChar, 4);
                  if (Zone != null) parmArray[0].Value = Zone; 
              
                  this.RunSP("CM_LoadZoneRulesSP", parmArray, ds);
              }
              catch (SqlException ex)
              {
                  LogSqlException(ex);
                  throw ex;
              }
              return ds;
          }

        public DataSet GetFollupAllocForActionSheet(string acctNo, int empeeNo)
        {
            DataSet ds = new DataSet();

            try
            {
                parmArray = new SqlParameter[2];
                parmArray[0] = new SqlParameter("@empeeNo", SqlDbType.Int);
                parmArray[0].Value = empeeNo;
                parmArray[1] = new SqlParameter("@acctNo", SqlDbType.VarChar, 12);
                parmArray[1].Value = acctNo;

                this.RunSP("CM_GetFollupAllocForActionSheet", parmArray, ds);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return ds;
        }

       //IP - 02/06/09 - Credit Collection Walkthrough Changes
       //Method that saves a strategy as one where its accounts can be allocated to a Bailiff / Collector.
        public void SaveStrategyAllocationCheck(SqlConnection conn, SqlTransaction trans, string strategy, bool canBeAllocated)
        {
            try
            {
                parmArray = new SqlParameter[2];
                parmArray[0] = new SqlParameter("@strategy", SqlDbType.VarChar, 7);           // #9521 jec 03/02/12
                parmArray[0].Value = strategy;
                parmArray[1] = new SqlParameter("@canBeAllocated", SqlDbType.Bit);
                parmArray[1].Value = canBeAllocated;

                this.RunSP(conn, trans, "CM_SaveStrategyAllocationCheckSP", parmArray);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

        //IP - 21/09/09 - UAT(856)
        //Method that updates an existing strategy as manual or not manual.
        public void UpdateStrategyManualCheck(SqlConnection conn, SqlTransaction trans, string strategy, bool manual)
        {
            try
            {
                parmArray = new SqlParameter[2];
                parmArray[0] = new SqlParameter("@strategy", SqlDbType.VarChar, 7);           // #9521 jec 03/02/12
                parmArray[0].Value = strategy;
                parmArray[1] = new SqlParameter("@manual", SqlDbType.Bit);
                parmArray[1].Value = manual;

                this.RunSP(conn, trans, "CM_UpdateStrategyManualCheckSP", parmArray);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

       //IP - 30/06/10 - CR1083 - Collection Commissions
       /// <summary>
       /// Method that runs a procedure to calculate Collection Commissions since the last successful run.
       /// </summary>
       /// <param name="runNo"></param>
       /// <returns></returns>
        public string CalculateCollectionCommissions(int runNo)
        {
            string eodResult = EODResult.Pass;

            using (SqlConnection conn = new SqlConnection(Connections.Default))
            {
                try
                {
                    conn.Open();
                    SqlTransaction trans = conn.BeginTransaction();

                    parmArray = new SqlParameter[1];

                    parmArray[0] = new SqlParameter("@runNo", SqlDbType.Int);
                    parmArray[0].Value = runNo;

                    this.RunSP(conn, trans, "CalculateCollectionCommissionsEODSP", parmArray);
                    trans.Commit();
                }
                catch (SqlException ex)
                {
                    eodResult = EODResult.Fail;
                    LogSqlException(ex);
                    throw ex;
                }
                finally
                {
                    string progress = "Finished Collections Commissions Calculation";
                    Console.WriteLine(progress);
                }
            }

            return eodResult;
        }

        // Address Standardization CR2019 - 025
        public DataSet GetVillages()
        {
            DataSet ds = new DataSet();
            try
            {
                this.RunSP("CM_GetVillageSP", ds);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return ds;
        }

        public DataSet GetRegions(string village)
        {
            DataSet ds = new DataSet();
            try
            {
                parmArray = new SqlParameter[1];
                parmArray[0] = new SqlParameter("@Village", SqlDbType.VarChar, 50);
                parmArray[0].Value = village;

                this.RunSP("CM_GetRegionSP", parmArray, ds);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return ds;
        }

        public string GetZipCode(string region, string village)
        {
            DataSet ds = new DataSet();
            string zipCode = string.Empty;
            try
            {
                parmArray = new SqlParameter[2];
                parmArray[0] = new SqlParameter("@Region", SqlDbType.VarChar, 50);
                parmArray[0].Value = region;
                parmArray[1] = new SqlParameter("@Village", SqlDbType.VarChar, 50);
                parmArray[1].Value = village;

                this.RunSP("CM_GetZipCodeSP", parmArray, ds);

                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    zipCode = ds.Tables[0].Rows[0][0] as string;
                }
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return zipCode;
        }
        // Address Standardization CR2019 - 025
    }
}
