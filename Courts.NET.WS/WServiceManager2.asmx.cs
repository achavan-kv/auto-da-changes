using System;
using System.Web;
using System.Collections;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Xml.Serialization;
using STL.DAL;
using STL.Common;
using STL.BLL;
using System.Collections.Generic;
using STL.Common.ServiceRequest;
using Blue.Cosacs;



namespace STL.WS
{
   
    partial class WServiceManager
    {
        #region [----------------------TECHNICIAN--------------------------------]
        
        #region [ DeleteTechnician ]
        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public void DeleteTechnician(int technicianId, out string err)
        {
            Function = "BServiceRequest::DeleteTechnician()";
            BServiceRequest serviceRequest = null;
            SqlConnection conn = null;
            SqlTransaction trans = null;
            err = "";
            try
            {
                conn = new SqlConnection(Connections.Default);
                conn.Open();
                trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted);

                serviceRequest = new BServiceRequest();
                serviceRequest.DeleteTechnician(conn, trans, technicianId);

                trans.Commit();
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
            }
        }

        #endregion

        //IP - 14/02/11 - Sprint 5.10 - #2975
        #region [ActivateTechnician ]
        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public void ActivateTechnician(int technicianId, out string err)
        {
            Function = "ActivateTechnician()";
            err = "";

            try
            {

                using (var conn = new SqlConnection(Connections.Default))
                {
                    do
                    {
                        try
                        {
                            conn.Open();
                            using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                            {

                                SRActivateTechnician sr = new SRActivateTechnician(conn, trans);
                                sr.ExecuteNonQuery(technicianId);

                                trans.Commit();
                            }
                            break;
                        }
                        catch (SqlException ex)
                        {
                            CatchDeadlock(ex, conn);
                        }
                    } while (retries <= maxRetries);
                }

            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
        }

        #endregion

         #region [ GetTechnician ]
        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataTable GetTechnician(int technicianId, out string err)
        {
            Function = "BServiceRequest::GetTechnician()";
            BServiceRequest serviceRequest = null;
            err = "";
            DataTable dt = new DataTable();

            try
            {
                serviceRequest = new BServiceRequest();
                dt = serviceRequest.GetTechnician(technicianId);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            finally
            {
               
            }
            return dt;
        }

        #endregion


        #region [ SaveTechnician ]
        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public void SaveTechnician(DataSet technicianDataSet, out string err)
        {
            Function = "BServiceRequest::SaveTechnician()";
            BServiceRequest serviceRequest = null;
            SqlConnection conn = null;
            SqlTransaction trans = null;
            err = "";

            try
            {
                conn = new SqlConnection(Connections.Default);
                conn.Open();
                trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted);
                serviceRequest = new BServiceRequest();
                serviceRequest.SaveTechnician(conn, trans, technicianDataSet);

                trans.Commit();

            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
            }
        }
        #endregion

        #region [ GetTechnicianList ]
        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet GetTechnicianList(out String err)
        {
            Function = "BServiceRequest::GetTechnicianList()";
            BServiceRequest serviceRequest = null;
            DataSet ds = null;
            err = "";

            try
            {
                serviceRequest = new BServiceRequest();
                ds = serviceRequest.GetTechnicianList();
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }

            return ds;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet GetTechniciansForPaymentSceen(out String err)
        {
            Function = "BServiceRequest::GetTechniciansForPaymentSceen()";
            BServiceRequest serviceRequest = null;
            DataSet ds = null;
            err = "";

            try
            {
                serviceRequest = new BServiceRequest();
                ds = serviceRequest.GetTechniciansForPaymentScreen();
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }

            return ds;
        }
        #endregion

        #region [ GetTechnicianPayments ]
        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet GetTechnicianPayments(int technicianId, out string err)
        {
            Function = "BServiceRequest::GetTechnicianPayments()";
            BServiceRequest serviceRequest = null;
            DataSet ds = null;
            err = "";

            try
            {
                serviceRequest = new BServiceRequest();
                ds = serviceRequest.GetTechnicianPayments(technicianId);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }

            return ds;
        }

       [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
       public void GetTechnicianPaymentSummary(int technicianId,ref List<decimal> technicianSummaryData, ref string err)
        {
           Function = "BServiceRequest::GetTechnicianPaymentSummary()";
            BServiceRequest serviceRequest = null;

            err = "";

            try
            {
                serviceRequest = new BServiceRequest();
                technicianSummaryData = serviceRequest.GetTechnicianPaymentSummary(technicianId);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }

        }
       
        #endregion

        #region [ SaveDetailsForAllTechnicians ]
        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public int SaveDetailsForAllTechnicians(string csvZones, string hoursFrom, string hoursTo,
            int callsPerDay, out int noTechniciansExceedingCallsPerDay, out int technicianWithHighestNoSlots, out string err)
        {
            Function = "BServiceRequest::ApplyDetailsToAllTechnicians()";
            BServiceRequest serviceRequest = null;
            err = "";
            noTechniciansExceedingCallsPerDay = 0;
            technicianWithHighestNoSlots = 0;
            try
            {
                using (var conn = new SqlConnection(Connections.Default))
                {
                    conn.Open();

                    using (var trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                    {

                        serviceRequest = new BServiceRequest();
                        serviceRequest.SaveDetailsForAllTechnicians(conn, trans, csvZones, hoursFrom, hoursTo,
                                                                    callsPerDay,
                                                                    out noTechniciansExceedingCallsPerDay,
                                                                    out technicianWithHighestNoSlots);
                        trans.Commit();
                    }
                }
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return 0;
        }
        #endregion

        #region [ SaveTechnicianPayment ]
        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public int SaveTechnicianPayment(int technicianId, int serviceRequestNo, DateTime DateClosed, decimal totalCost, string status, out string err)
        {
            Function = "BServiceRequest::ApplyDetailsToAllTechnicians()";
            SqlConnection conn = null;
            SqlTransaction trans = null;
            BServiceRequest serviceRequest = null;
            err = "";
            try
            {
                using (conn = new SqlConnection(Connections.Default))
                {
                    conn.Open();

                    trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted);

                    serviceRequest = new BServiceRequest();
                    serviceRequest.SaveTechnicianPayment(conn, trans, technicianId, serviceRequestNo, DateClosed,
                                                         totalCost, status);

                    trans.Commit();
                }
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return 0; 

        }

        #endregion

        #endregion

        #region [----------------------SERVICE SEARCH--------------------------------]

        #region ServiceSearch
        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet ServiceSearch(long serviceNo, string custId, string firstName, string lastName, bool showCourts, bool showNonCourts, bool showInternal,  //CR1030 jec
           bool showCashGo, int seachLimit, bool exactMatch)
        {
              BServiceRequest serviceRequest = new BServiceRequest();
              return serviceRequest.ServiceSearch(serviceNo, custId, firstName, lastName, showCourts, showNonCourts, showInternal, showCashGo,       //CR1030 jec
                seachLimit, exactMatch);

          }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public bool CheckSerialNoDup(string itemno, string serialno, string modelno, string accountNo)      //CR1030 jec
        {
            BServiceRequest serviceRequest = new BServiceRequest();
            return serviceRequest.CheckSerialNoDup(itemno, serialno, modelno, accountNo);
        }


        #endregion

        #endregion

        #region [----------------------FOOD LOSS--------------------------------]
         
        #region GetFoodLoss
        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet GetFoodLoss(int serviceRequestNo, out string err)
        {
            Function = "BServiceRequest::GetTechnicianList()";
            BServiceRequest serviceRequest = null;
            DataSet ds = null;
            err = "";

            try
            {
                serviceRequest = new BServiceRequest();
                ds = serviceRequest.GetFoodLoss(serviceRequestNo);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }

            return ds;
        }
        #endregion

        #region SaveFoodLoss
        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public void SaveFoodLoss(int serviceRequestNo, DataSet foodLossDataSet, out string err)
        {
            Function = "BServiceRequest::SaveFoodLoss()";
            BServiceRequest serviceRequest = null;
            SqlTransaction trans = null;
            err = "";

            try
            {
                using (var conn = new SqlConnection(Connections.Default))
                {
                    conn.Open();
                    trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted);
                    serviceRequest = new BServiceRequest();
                    serviceRequest.SaveFoodLoss(conn, trans, serviceRequestNo, foodLossDataSet);

                    trans.Commit();
                }
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
        }
        #endregion
        
        #endregion

        #region [----------------------CUSTOMER INTERACTION--------------------------------]

        #region GetCustomerInteraction
        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet GetCustomerInteraction(string custId, out string custFullName, out string err)
        {
            Function = "BServiceRequest::GetCustomerInteraction()";
            DataSet ds = null;
            err = "";
            custFullName = "";
            try
            {
                ds = new BServiceRequest().GetCustomerInteraction(custId, out custFullName);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }

            return ds;
        }
        #endregion

        #region SaveCustomerInteraction
        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public int SaveCutomerInteraction(string custId, DataSet customerInteractionDataSet, out string err)
        {
            Function = "BServiceRequest::SaveCutomerInteraction()";
            BServiceRequest serviceRequest = null;
            SqlConnection conn = null;
            SqlTransaction trans = null;
            err = "";

            try
            {
                conn = new SqlConnection(Connections.Default);
                conn.Open();
                trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted);
                serviceRequest = new BServiceRequest();
                serviceRequest.SaveCustomerInteraction(conn, trans, custId, customerInteractionDataSet);
                
                trans.Commit();
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
            }
            return 0;
        }
        #endregion

        #endregion

        #region [----------------------REPORTS--------------------------------]
        
        #region GetOutstandingServiceReport
        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet GetOutstandingServiceReport(string reportType, object minLogged, object maxLogged, object minServiceNo, object maxServiceNo,
                    object technicianId, object notUpdatedSince, int daysOutstanding, object serviceStatus, object viewTop, string err)   //CR1030 jec
        {
            Function = "BServiceRequest::GetOutstandingServiceReport()";
            BServiceRequest serviceRequest = null;
            DataSet ds = null;
            err = "";
           
            try
            {
                serviceRequest = new BServiceRequest();
                ds = serviceRequest.GetOutstandingServiceReport(reportType, minLogged, maxLogged, minServiceNo, maxServiceNo, technicianId, notUpdatedSince, daysOutstanding, serviceStatus, viewTop);      //CR1030 jec
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }

            return ds;
        }
        #endregion

        #region GetBatchPrintReports
        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
        [TraceExtension]
#endif
        public DataSet GetBatchPrintReports(int? technichianID, DateTime? fromDate, DateTime? toDate, int? minSR, int? maxSR, 
                                            int printLocation, bool reprintOnly, bool showAll, out string err)
        {
            Function = "BServiceRequest::GetBatchPrintReports()";
            
            err = "";

            try
            {
                return new BServiceRequest().GetBatchPrintReports(technichianID, fromDate, toDate, minSR, maxSR, 
                                                                printLocation, reprintOnly, showAll);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }

            return null;
        }
        #endregion

        #region GetServiceProgressReport
        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet GetServiceProgressReport(object minLogged, object maxLogged, object productCode, object productGroup, bool outstanding, bool allocated, bool completed,string fault,int serviceBranch, out string err)
        {
            Function = "BServiceRequest::GetServiceProgressReport()";
            BServiceRequest serviceRequest = null;
            DataSet ds = null;
            err = "";

            try
            {
                serviceRequest = new BServiceRequest();
                ds = serviceRequest.GetServiceProgressReport(minLogged, maxLogged, productCode, productGroup, outstanding, allocated, completed, fault, serviceBranch);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }

            return ds;
        }
        #endregion

        #region GetServiceFailureReport
        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet GetServiceFailureReport(DateTime minLogged, DateTime maxLogged,string fault,string product,bool quarters, out string err)
        {
            Function = "BServiceRequest::GetServiceFailureReport()";
            BServiceRequest serviceRequest = null;
            DataSet ds = null;
            err = "";

            try
            {
                serviceRequest = new BServiceRequest();
                ds = serviceRequest.GetServiceFailureReport(minLogged, maxLogged,fault,product,quarters);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }

            return ds;
        }
        #endregion

        #region ClaimsSubmissionReport
        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet GetClaimsSubmissionReport(object minLogged, object maxLogged, object minServiceNo, object maxServiceNo,string fault, out string err)
        {
            Function = "BServiceRequest::GetClaimsSubmissionReport()";
            BServiceRequest serviceRequest = null;
            DataSet ds = null;
            err = "";

            try
            {
                serviceRequest = new BServiceRequest();
                ds = serviceRequest.GetClaimsSubmissionReport(minLogged, maxLogged, minServiceNo, maxServiceNo, fault);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }

            return ds;
        }
        #endregion
        
        #endregion

        #region [----------------------SERVICE REQUEST--------------------------------]
        
        #region RemoveDepositFromSR
        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public int RemoveDepositFromSR(int serviceRequestNo, out string err)
        {
            Function = "BServiceRequest::RemoveDepositFromSR()";
            BServiceRequest serviceRequest = null;
            SqlConnection conn = null;
            SqlTransaction trans = null;
            err = "";

            try
            {
                conn = new SqlConnection(Connections.Default);
                conn.Open();
                trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted);
                serviceRequest = new BServiceRequest();
                serviceRequest.RemoveDepositFromSR(conn, trans, serviceRequestNo);

                trans.Commit();
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
            }
            return 0;
        }


        #endregion
 
        #region GetServiceRequestSummaryForAccount
        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet GetServiceRequestSummaryForAccount(string accountNo, out string err)
        {

            Function = "GetServiceRequestSummaryForAccount()";
            BServiceRequest serviceRequest = null;
            DataSet ds = null;
            err = "";

            try
            {
                serviceRequest = new BServiceRequest();
                ds = serviceRequest.GetServiceRequestSummaryForAccount(accountNo);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }

            return ds;
        }

        #endregion

        #endregion

        #region [----------------------SERVICE RESOLUTION--------------------------------]
        #region GetChargeToAuthorisations

       [WebMethod]
       [SoapHeader("authentication")]

       public DataSet ChargeToAuthorisationLoad(out string err)
       {
          Function = "BServiceRequest::ChargeToAuthorisationLoad()";

          DataSet ds = new DataSet();
          err = String.Empty; ;

          try
          {
             BServiceRequest serviceRequest = new BServiceRequest();
             ds = serviceRequest.ChargeToAuthorisationLoad();
          }
          catch (Exception ex)
          {
             Catch(ex, Function, ref err);
          }

          return ds;
       }
        #endregion

        #region SaveChargeToAuthorisations

       //[WebMethod]
       //[SoapHeader("authentication")]

       //public void UpdateChargeToAuthorisation(string defaultchargeto,string customerchargeto,string aigchargeto, 
       //   string supplierchargeto,string internalchargeto,string delivererchargeto,out string err)
       //{
       //   Function = "BServiceRequest::UpdateChargeToAuthorisation()";
       //   err = String.Empty; ;
       //   SqlConnection conn = new SqlConnection(Connections.Default);
       //   SqlTransaction trans;

       //   try
       //   {
       //      conn.Open();
       //      trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted);
       //      BServiceRequest serviceRequest = new BServiceRequest();
       //      ArrayList al = new ArrayList();
       //      al.Add(defaultchargeto);
       //      al.Add(customerchargeto);
       //      al.Add(aigchargeto);
       //      al.Add(supplierchargeto);
       //      al.Add(internalchargeto);
       //      al.Add(delivererchargeto);

       //      serviceRequest.UpdateChargeToAuthorisation(conn,trans,al);

       //      trans.Commit();
       //   }
       //   catch (Exception ex)
       //   {
       //      Catch(ex, Function, ref err);
       //   }
       //   finally
       //   {
       //      if (conn.State != ConnectionState.Closed)
       //         conn.Close();
       //   }
       //}

       public struct Changes
       {
          public string defaultChargeTo;
          public string customerChargeTo;
          public string aigChargeTo;
          public string supplierChargeTo;
          public string internalChargeTo;
          public string delivererChargeTo;
       }

       [WebMethod]
       [SoapHeader("authentication")]
       [XmlInclude(typeof(Changes))]

       public void UpdateChargeToAuthorisation(Changes changedValues, out string err)
       {
          Function = "BServiceRequest::UpdateChargeToAuthorisation()";
          err = String.Empty; ;
          SqlConnection conn = new SqlConnection(Connections.Default);
          SqlTransaction trans;

          try
          {
             conn.Open();
             trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted);
             BServiceRequest serviceRequest = new BServiceRequest();
             ArrayList al = new ArrayList();
             al.Add(changedValues.defaultChargeTo);
             al.Add(changedValues.customerChargeTo);
             al.Add(changedValues.aigChargeTo);
             al.Add(changedValues.supplierChargeTo);
             al.Add(changedValues.internalChargeTo);
             al.Add(changedValues.delivererChargeTo);

             serviceRequest.UpdateChargeToAuthorisation(conn, trans, al);

             trans.Commit();
          }
          catch (Exception ex)
          {
             Catch(ex, Function, ref err);
          }
          finally
          {
             if (conn.State != ConnectionState.Closed)
                conn.Close();
          }
       }

        #endregion

        #endregion

        #region [----------------------SERVICE REQUEST AUDIT--------------------------------]
        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet GetServiceRequestAudit(int serviceRequestNo, out string err)
        {
            Function = "BServiceRequest::RemoveDepositFromSR()";
            BServiceRequest serviceRequest = null;
            SqlConnection conn = null;
            SqlTransaction trans = null;
            err = "";
            DataSet ds = null;
            try
            {
                conn = new SqlConnection(Connections.Default);
                conn.Open();
                trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted);
                serviceRequest = new BServiceRequest();
                ds = serviceRequest.GetServiceRequestAudit(serviceRequestNo);

                trans.Commit();
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
            }
            return ds;

        }
        #endregion

        #region[--------------------SERVICE CASH AND GO INVOICE------------------------------]

       [WebMethod]
       [SoapHeader("authentication")]
       [XmlInclude(typeof(CustomerDetails))]

       public int GetInvoiceNumber(CustomerDetails cusDetails, out string err)
       {
          Function = "BServiceRequest::GetInvoiceNumber()";
          BServiceRequest serviceRequest = null;
          SqlConnection conn = null;
          SqlTransaction trans = null;
          err = "";
          int invoice = 0;

          try
          {
             conn = new SqlConnection(Connections.Default);
             conn.Open();
             trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted);
             serviceRequest = new BServiceRequest();

             invoice = serviceRequest.GetInvoiceNumber(conn, trans, cusDetails);

             trans.Commit();
          }
          catch (Exception ex)
          {
             Catch(ex, Function, ref err);
          }
          finally
          {
             if (conn.State != ConnectionState.Closed)
                conn.Close();
          }
          return invoice;
       }

        #endregion
     }
}
