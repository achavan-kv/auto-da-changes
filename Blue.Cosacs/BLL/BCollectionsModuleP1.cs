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
       public BCollectionsModule()
       {
       }

       public DataSet GetWorkLists()
       {
           DataSet ds = new DataSet();
           DCollectionsModule collections = new DCollectionsModule();

           collections.GetWorkList();
           ds.Tables.Add(collections.DataTableCollections);

           //collections.GetWorkListData();
           //ds.Tables.Add(collections.DataTableCollections);

           //collections.GetActions();
           //ds.Tables.Add(collections.DataTableCollections);

           //collections.GetWorkListActions();
           //ds.Tables.Add(collections.DataTableCollections);

           //collections.GetEmployeeTypes();
           //ds.Tables.Add(collections.DataTableCollections);

           //collections.GetWorkListEmployees();
           //ds.Tables.Add(collections.DataTableCollections);

           return ds;
       }

       //public void SaveWorkList(SqlConnection conn, SqlTransaction trans, string workList, string description, 
       //                             string action, DataSet dsWorkList)
       //{
       //    DCollectionsModule collections = new DCollectionsModule();

       //    foreach (DataTable dtWorkList in dsWorkList.Tables)
       //    {
       //        if (dtWorkList.TableName == TN.EmployeeTypes)
       //        {
       //            if (dtWorkList.Rows.Count > 0)
       //            {
       //                collections.DeleteWorkList(conn, trans, workList);
       //            }

       //            foreach (DataRow row in dtWorkList.Rows)
       //            {
       //                collections.SaveWorkList(conn, trans, workList, description, (string)row[CN.EmployeeType]);
       //            }
       //        }

       //        if (dtWorkList.TableName == TN.Actions)
       //        {
       //           // CR 852 If no rows exist in the DataTable then this indicates that all actions may have been removed
       //           //        You would then still want to delete from the database table

       //            //if (dtWorkList.Rows.Count > 0)
       //            //{
       //                collections.DeleteWorkListActions(conn, trans, workList);
       //            //}
                   
       //            foreach (DataRow row in dtWorkList.Rows)
       //            {
       //                collections.SaveWorkListAction(conn, trans, workList, (string)row[CN.Code], Convert.ToBoolean(row[CN.Exit].ToString()));
       //            }
       //        }
       //    }
       //}

       public void SaveSMS(SqlConnection conn, SqlTransaction trans, string smsName, string smsBody, string description)
       {
           DCollectionsModule collections = new DCollectionsModule();
          //First delete from CMSMS
           collections.DeleteSMS(conn, trans, smsName);
           collections.SaveSMS(conn, trans, smsName, smsBody, description);
       }

       //NM & IP - 23/12/08 - CR976
       //Method which retrieves all the actions that the employee has rights to
       //for the strategy associated with the selected account
       public DataSet GetStrategyActionsForEmployee(int empeeno, string strategy, bool checkForSupervisorRight)
       {
           DCollectionsModule collections = new DCollectionsModule();
           collections.GetStrategyActionsForEmployee(empeeno, strategy, checkForSupervisorRight);

           return collections.StrategyActions;
       }

       //NM - 29/12/08 - CR976
       //Method to updateor insert customer telephone numbers from 
       //TelephoneActions screen
       public void UpdateCustomerTelephoneNo(SqlConnection conn, SqlTransaction trans,string custid, string HTelNo, string HDialCode,
           string WTelNo, string WDialCode, string MTelNo, string MDialCode, int Empeeno, bool HomeTelephoneChanged, bool WorkTelephoneChanged, bool MobileTelephoneChanged)
       {
           DCollectionsModule collections = new DCollectionsModule();
           collections.UpdateCustomerTelephoneNo( conn, trans,custid,  HTelNo,  HDialCode,  WTelNo,  WDialCode ,  MTelNo,  MDialCode, Empeeno, HomeTelephoneChanged, WorkTelephoneChanged, MobileTelephoneChanged);
       }

       //NM - 06/01/09 - CR976
       //Method to insert Extra Telephone Actions details to a separate table
       //TelephoneActions screen
       public void UpdateExtraTelephoneActionDetail(SqlConnection conn, SqlTransaction trans, string actionCode, DataSet dsExtraDetails)
       {
           if (dsExtraDetails.Tables.Count == 0 || dsExtraDetails.Tables[0].Rows.Count == 0)
               throw new STLException("Incomplete Data");
           
           DCollectionsModule collections = new DCollectionsModule();
           
           DataRow dr = dsExtraDetails.Tables[0].Rows[0];  

           //NM - In the following lines casting to nullable type is unnecessary (Eg:casting a decimal to a 'decimal?'),
           //  if we omit the casting, compiler will fail the build (ternary if statement requires compatible types for both conditions)

           if (actionCode == "LEG") //Legal Detail
           {               
               collections.UpdateLegalDetail(conn, trans, dr[CN.CMAcctno].ToString(), Convert.ToInt32(dr[CN.CMEmpeeno]),
                                             dr[CN.CMSolicitorNo] == DBNull.Value ? null : dr[CN.CMSolicitorNo].ToString(),
                                             dr[CN.CMAuctionProceeds] == DBNull.Value ? null : (decimal?)Convert.ToDecimal(dr[CN.CMAuctionProceeds]),
                                             dr[CN.CMAuctionDate] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(dr[CN.CMAuctionDate]),
                                             (dr[CN.CMAuctionAmount] == DBNull.Value ? null : (decimal?)Convert.ToDecimal(dr[CN.CMAuctionAmount])),
                                             dr[CN.CMCourtDeposit] == DBNull.Value ? null : (decimal?)Convert.ToDecimal(dr[CN.CMCourtDeposit]),
                                             dr[CN.CMCourtAmount] == DBNull.Value ? null : (decimal?)Convert.ToDecimal(dr[CN.CMCourtAmount]),
                                             dr[CN.CMCourtDate] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(dr[CN.CMCourtDate]),
                                             Convert.ToBoolean(dr[CN.CMCaseClosed]),
                                             dr[CN.CMMentionDate] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(dr[CN.CMMentionDate]),
                                             dr[CN.CMMentionCost] == DBNull.Value ? null : (decimal?)Convert.ToDecimal(dr[CN.CMMentionCost]),
                                             dr[CN.CMPaymentRemittance] == DBNull.Value ? null : (decimal?)Convert.ToDecimal(dr[CN.CMPaymentRemittance]),
                                             dr[CN.CMJudgement] == DBNull.Value ? null : dr[CN.CMJudgement].ToString(),
                                             dr[CN.CMLegalAttachmentDate] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(dr[CN.CMLegalAttachmentDate]),
                                             dr[CN.CMLegalInitiatedDate] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(dr[CN.CMLegalInitiatedDate]),
                                             dr[CN.CMDefaultedDate] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(dr[CN.CMDefaultedDate]),
                                             dr[CN.CMUserNotes] == DBNull.Value ? null : dr[CN.CMUserNotes].ToString());                                             
           }
           else if (actionCode == "IND") //Insurance Detail
           {
               collections.UpdateInsuranceDetail(conn, trans, dr[CN.CMAcctno].ToString(), Convert.ToInt32(dr[CN.CMEmpeeno]),
                                                 dr[CN.CMInitiatedDate] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(dr[CN.CMInitiatedDate]),
                                                 dr[CN.CMFullOrPartClaim] == DBNull.Value ? null : dr[CN.CMFullOrPartClaim].ToString(),
                                                 dr[CN.CMInsAmount] == DBNull.Value ? null : (decimal?)Convert.ToDecimal(dr[CN.CMInsAmount]),
                                                 dr[CN.CMInsType] == DBNull.Value ? null : dr[CN.CMInsType].ToString(),
                                                 Convert.ToBoolean(dr[CN.CMIsApproved]),
                                                 dr[CN.CMUserNotes] == DBNull.Value ? null : dr[CN.CMUserNotes].ToString());                                             
           }
           else if (actionCode == "FRD") //Fraud Detail
           {
               collections.UpdateFraudDetail(conn, trans, dr[CN.CMAcctno].ToString(), Convert.ToInt32(dr[CN.CMEmpeeno]),
                                                dr[CN.CMFraudInitiatedDate] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(dr[CN.CMFraudInitiatedDate]),
                                                Convert.ToBoolean(dr[CN.CMIsResolved]),
                                                dr[CN.CMUserNotes] == DBNull.Value ? null : dr[CN.CMUserNotes].ToString());                                             
           }
       }

       //NM --20/01/2009 --------------------------------------------------------------
       public void UpdateWorkList_ActionRights(SqlConnection conn, SqlTransaction trans, DataSet dsRightsInfo, int user)
       {
           if(dsRightsInfo.Tables.Count < 2)
               throw new STLException("Incomplete Data");
           
           DCollectionsModule collections = new DCollectionsModule();

           DataTable dtWorkListRights = dsRightsInfo.Tables["WorkListRights"];
           DataTable dtActionRights = dsRightsInfo.Tables["ActionRights"];

           //-- Deleting WorkList Rights --
           DataRow[] drWorkListRightsArray = dtWorkListRights.Select("AddRemoveFlag = 'REMOVE'");
           foreach (DataRow dr in drWorkListRightsArray)
           {
               collections.RemoveWorkListRights(conn, trans, Convert.ToInt32(dr["Key"].ToString().Split(':')[0]), dr[CN.WorkList].ToString());
           }

           //-- Adding WorkList Rights --
           drWorkListRightsArray = dtWorkListRights.Select("AddRemoveFlag = 'ADD'");
           foreach (DataRow dr in drWorkListRightsArray)
           {
               collections.AddWorkListRights(conn, trans, Convert.ToInt32(dr["Key"].ToString().Split(':')[0]), dr[CN.WorkList].ToString(), user);
           }


           //-- Deleting Action Rights --
           DataRow[] drActionRightsArray = dtActionRights.Select("AddRemoveFlag = 'REMOVE'");
           foreach (DataRow dr in drActionRightsArray)
           {
               collections.RemoveActionRights(conn, trans, Convert.ToInt32(dr["Key"].ToString().Split(':')[0]), dr[CN.Strategy].ToString(), dr[CN.Action].ToString());
           }

           //-- Adding Action Rights --
           drActionRightsArray = dtActionRights.Select("AddRemoveFlag = 'ADD'");
           foreach (DataRow dr in drActionRightsArray)
           {
               collections.AddActionRights(conn, trans, Convert.ToInt32(dr["Key"].ToString().Split(':')[0]), dr[CN.EmployeeType].ToString(), dr[CN.Strategy].ToString(), 
                                dr[CN.Action].ToString(), Convert.ToBoolean(dr[CN.CycleToNextFlag].ToString()), Convert.ToInt16(dr[CN.MinNotesLength].ToString()), user);
           }
       }
       
       public void SaveWorkLists(SqlConnection conn, SqlTransaction trans, DataSet dsWorkList)
       {
           if (dsWorkList.Tables.Count < 2)
               throw new STLException("Incomplete Data");
           
           DCollectionsModule collections = new DCollectionsModule();

           foreach (DataRow dr in dsWorkList.Tables["DELETED"].Rows)
           {
               collections.DeleteWorkLists(conn, trans, dr["WorkListCode"].ToString());
           }

           foreach (DataRow dr in dsWorkList.Tables["MODIFIED"].Rows)
           {
               collections.SaveWorkLists(conn, trans, dr["WorkListCode"].ToString(), dr["description"].ToString());
           }
       }
   
       //------------------------------------------------------------------------------

       //NM
       public void UpdateWorkListSortOrder(SqlConnection conn, SqlTransaction trans, DataSet dsSortOrder)
       {
           if (dsSortOrder.Tables.Count < 2)
               throw new STLException("Incomplete Data");

           DCollectionsModule collections = new DCollectionsModule();

           foreach (DataRow dr in dsSortOrder.Tables["EMP_TYPE"].Rows)
           {
               collections.DeleteWorkListSortOrder(conn, trans, string.Empty);
           }

           foreach (DataRow dr in dsSortOrder.Tables["SORT_ORDER"].Rows)
           {
               collections.SaveWorkListSortOrder(conn, trans, string.Empty,dr["ColumnName"].ToString(),
                                                Convert.ToInt16(dr["SortOrder"].ToString()), dr["AscDesc"].ToString());
           }
       }

       //NM
       public DataSet LoadWorkListOrderColumns()
       {
           DCollectionsModule collections = new DCollectionsModule();
           DataSet ds = collections.LoadWorkListOrderColumns();
           return ds;
       }
       
       //NM
       public DataSet LoadWorkListSortOrder()
       {
           DCollectionsModule collections = new DCollectionsModule();
           DataSet ds = collections.LoadWorkListSortOrder();
           return ds;
       }

       //NM
       public DataSet GetWorkListRightsHierarchy()
       {
           DCollectionsModule collections = new DCollectionsModule();
           DataSet ds = collections.GetWorkListRightsHierarchy();
           return ds;
       }

       //NM
       public DataSet GetActionRightsHierarchy()
       {
           DCollectionsModule collections = new DCollectionsModule();
           DataSet ds = collections.GetActionRightsHierarchy();
           return ds;
       }       
           
       //NM
       public DataSet GetDistinctLetterCodesByRunNo(int runNo)
       {
           DataSet ds = new DataSet();
           DLetter da = new DLetter();
           ds = da.GetDistinctLetterCodesByRunNo(runNo);
           return ds;
       }

       //NM
       public DataSet LoadLetterFieldsbyCode(string letterCode, int runNo, char storeType, bool spouseDetailRequired, bool guarantorDetailRequired)
       {
           DCollectionsModule collections = new DCollectionsModule();
           DataSet ds = collections.LoadLetterFieldsbyCode(letterCode, runNo, storeType, spouseDetailRequired, guarantorDetailRequired);
           return ds;
       }

       //NM
       public DataSet GetBranchByStoreType(char storeType)
       {
           DBranch branch = new DBranch();
           DataSet ds = branch.GetBranchByStoreType(storeType);
           return ds;
       }

       //NM
       public DataSet GetZones()
       {
           DCollectionsModule collections = new DCollectionsModule();
           DataSet ds = collections.GetZones();
           return ds;
       }

       public void SaveZones(SqlConnection conn, SqlTransaction trans, string zone, string description)
       {
           DCollectionsModule collections = new DCollectionsModule();
           collections.SaveZones(conn, trans, zone, description);
       }

       public void SaveZoneRule(SqlConnection conn, SqlTransaction trans, string zone, DataSet dsZone)
       {
           if (dsZone.Tables.Count == 0)
               throw new STLException("Incomplete Data");
           DCollectionsModule collections = new DCollectionsModule();
           collections.SaveZoneRule(conn, trans, zone, dsZone.Tables[0]);
       }

       public void DeleteZone(SqlConnection conn, SqlTransaction trans, string Zone)
       {

           DCollectionsModule collections = new DCollectionsModule();
           collections.DeleteZone(conn, trans, Zone);

       }

       //NM
       public DataSet GetZoneAllocatableEmployeeInfo()
       {
           DCollectionsModule collections = new DCollectionsModule();
           DataSet ds = collections.GetZoneAllocatableEmployeeInfo();
           return ds;
       }

       public void SaveBailiffZoneAllocation(SqlConnection conn, SqlTransaction trans, int empeeNo, DataSet dsZoneAllocation, int user)
       {
           if (dsZoneAllocation.Tables.Count < 2)
               throw new STLException("Incomplete Data");
           DCollectionsModule collections = new DCollectionsModule();
           collections.SaveBailiffZoneAllocation(conn, trans, empeeNo, dsZoneAllocation.Tables[0], dsZoneAllocation.Tables[1], user);
       }

       //NM & IP - 02/01/09 - CR976 - Method that will retrieve all the call reminders
       //for the selected account for the current user
       public DataSet GetCallReminderInfo(string acctno, int empeeno)
       {
           DCollectionsModule collections = new DCollectionsModule();
           collections.GetCallReminderInfo(acctno, empeeno);

           return collections.CallReminders;
       }


       //NM & IP - 08/01/09 - CR976 -Method that will retrieve any 'Legal', 'Fraud', or 
       //'Insurance' details for a selected account if any have been entered.
       public DataSet GetLegalFraudInsuranceDetails(string acctno)
       {
           DCollectionsModule collections = new DCollectionsModule();
           collections.GetLegalFraudInsuranceDetails(acctno);

           return collections.LegalFraudInsuranceDetails;
       }


       //IP & JC - 12/01/09 - CR976 - Method which will retrieve the details for 
       //combined Ready Finance accounts for a Customer to be displayed
       //on the 'Special Arrangements Consolidated' screen.
        public DataSet AccountGetCombinedSPADetails(string custID)
       {
           DCollectionsModule collections = new DCollectionsModule();
           collections.AccountGetCombinedSPADetails(custID);

           return collections.CombinedSPADetails;
       }

        public DataSet GetRoles(int permission)
        {
           return new DCollectionsModule().GetRoles(permission);
        }

        // Address Standardization CR2019-025
        public DataSet GetVillages()
        {
            DCollectionsModule collections = new DCollectionsModule();
            DataSet ds = collections.GetVillages();
            return ds;
        }

        public DataSet GetRegions(string village)
        {
            DCollectionsModule collections = new DCollectionsModule();
            DataSet ds = collections.GetRegions(village);
            return ds;
        }

        public string GetZipCode(string region, string village)
        {
            DCollectionsModule collections = new DCollectionsModule();
            return collections.GetZipCode(region, village);
        }
        // Address Standardization CR2019-025
    }
}
