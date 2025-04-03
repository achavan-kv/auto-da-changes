using System;
using System.Web;
using System.Collections;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using STL.DAL;
using STL.Common;
using STL.Common.Static;
using STL.BLL;
using Blue.Cosacs;
using System.Text;


namespace STL.WS
{
    /// <summary>
    /// WServiceManager is the web service for the Service Request subsytem that replaces CIRIS
    /// </summary>
    [WebService(Namespace = "http://strategicthought.com/webservices/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public partial class WServiceManager : CommonService
    {

        public WServiceManager()
        {

            //Uncomment the following line if using designed components 
            //InitializeComponent(); 
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet GetServiceRequest(
            string serviceTypeSearch,
            long serviceRequestNo,
            string custId,
            string acctNo,
            int invoiceNo,
            short branchNo,
            string user,
            out string serviceType,
            out string err,
            out bool isPaidAndTakenAcct) //IP - 31/07/09 - UAT(741) - added bool to check if the account being searched on is a Paid and Taken account.
        {
            Function = "BServiceManager::GetServiceRequest()";
            DataSet ds = null;
            serviceType = "";
            isPaidAndTakenAcct = false;
            err = "";

            try
            {
                BServiceRequest serviceRequest = new BServiceRequest();
                ds = serviceRequest.GetServiceRequest(
                        serviceTypeSearch,
                        serviceRequestNo,
                        custId,
                        acctNo,
                        invoiceNo,
                        branchNo,
                        Convert.ToInt32(user),
                        out serviceType,
                        out isPaidAndTakenAcct);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return ds;
        }

        [WebMethod]
        public DataSet GetServiceRequestDetails(
           string acctNo,
          string itemno
          )
        {
            Function = "BServiceManager::GetServiceRequestDetails()";
            DataSet ds = null;
  
            try
            {
                BServiceRequest serviceRequest = new BServiceRequest();
                ds = serviceRequest.GetServiceRequest(acctNo, itemno);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return ds;
        }

        [WebMethod]
        public void UpdateServiceRequest( int id, string DeliveryDateUpdated)
        {
            Function = "BServiceManager::UpdateServiceRequest()";
            try
            {
                BServiceRequest serviceRequest = new BServiceRequest();
                serviceRequest.UpdateServiceRequest(id, DeliveryDateUpdated);
            }
            catch (Exception ex)
            {
                throw ex;
            }
           
        }


        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public int SaveServiceRequest(DataSet serviceRequestSet, out String err)
        {
            Function = "BServiceRequest::SaveServiceRequest()";
            SqlConnection conn = null;

            BServiceRequest serviceRequest = null;
            err = "";
            try
            {
                conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {
                            serviceRequest = new BServiceRequest();
                            serviceRequest.User = STL.Common.Static.Credential.UserId;
                            serviceRequest.SaveServiceRequest(conn, trans, serviceRequestSet);
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


        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public int UpdateSRCustomer(int serviceRequestNo, string custID, string title, string firstName, string lastName, decimal arrears,
           string address1, string address2, string address3, string postCode, string directions, string telHome, string telWork,
           string telMobile, out String err)
        {
            Function = "BServiceRequest::UpdateSRCustomer()";
            SqlConnection conn = null;

            BServiceRequest serviceRequest = null;
            err = "";
            try
            {
                conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {
                            serviceRequest = new BServiceRequest();
                            //UAT 367 need to put back carriage return characters \r
                            directions = directions.Replace("\n", "\r\n");
                            serviceRequest.UpdateSRCustomer(conn, trans, serviceRequestNo, custID, title, firstName, lastName, arrears, address1,
                               address2, address3, postCode, directions, telHome, telWork, telMobile);
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


        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public int SavePriceIndexMatrix(DataSet priceIndexMatrix, out string err)
        {
            Function = "BServiceManager::SavePriceIndexMatrix()";
            SqlConnection conn = null;

            BServiceRequest serviceRequest = null;
            err = "";

            try
            {
                conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {
                            serviceRequest = new BServiceRequest();
                            serviceRequest.SavePriceIndexMatrix(conn, trans, priceIndexMatrix);
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

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public bool CheckTechMainPermissions()
        {
            Function = "BServiceRequest::CheckTechMainPermissions()";
            BServiceRequest serviceRequest = null;
            bool permit = false;
            string err = "";
            try
            {
                int userId = STL.Common.Static.Credential.UserId;
                serviceRequest = new BServiceRequest();
                permit = serviceRequest.CheckTechMainPermissions(userId);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }

            return permit;
        }



        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet GetServiceAllocationPayment(int technicianId, out String err)
        {
            Function = "BServiceRequest::GetTechnicianList()";
            BServiceRequest serviceRequest = null;
            DataSet ds = null;
            err = "";

            try
            {
                serviceRequest = new BServiceRequest();
                ds = serviceRequest.GetServiceAllocationPayment(technicianId);
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
        public string GetChargeToDeposit(int serviceRequestNo, short serviceBranchNo,
            out decimal depositAmount, out decimal depositPaid, out String err)
        {
            Function = "BServiceManager::GetChargeToDeposit()";
            SqlConnection conn = null;

            string customerAcctNo = "";
            depositAmount = -1;
            depositPaid = 0;
            err = "";

            try
            {
                conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {
                            BServiceRequest serviceRequest = new BServiceRequest();
                            serviceRequest.User = STL.Common.Static.Credential.UserId;
                            serviceRequest.GetChargeToDeposit(conn, trans,
                                            serviceRequestNo,
                                            serviceBranchNo,
                                            out customerAcctNo,
                                            out depositAmount,
                                            out depositPaid);
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
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
            }
            return customerAcctNo;
        }


        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet GetTechnicianDiary(int technicianId, out string err)
        {
            Function = "BServiceManager::GetTechnicianDiary()";
            DataSet ds = null;
            err = "";

            try
            {
                BServiceRequest serviceRequest = new BServiceRequest();
                ds = serviceRequest.GetTechnicianDiary(technicianId);
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
        public DataSet CheckTechnicianSlots(int technicianId, int calls, out string err)
        {
            Function = "BServiceManager::CheckTechnicianSlots()";
            DataSet ds = new DataSet();
            err = "";

            try
            {
                BServiceRequest serviceRequest = new BServiceRequest();
                ds = serviceRequest.CheckTechnicianSlots(technicianId, calls);
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
        public DataSet CheckPartsDate(long serviceRequestNo, out string err)
        {
            Function = "BServiceManager::CheckPartsDate()";
            DataSet ds = new DataSet();
            err = "";

            try
            {
                BServiceRequest serviceRequest = new BServiceRequest();
                ds = serviceRequest.CheckPartsDate(serviceRequestNo);
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
        public int BookServiceRequest(
            string zone, int technicianId, DateTime slotDate, short slotNo, short multiSlot, string bookingType, long serviceRequestNo, string IsAM,
            string allocatedBy, string reassignCode, string reassignedBy, out bool notFound, out bool alreadyBooked, out DateTime curSlotDate, out string curBookingType, out String err)
        {
            Function = "BServiceRequest::BookServiceRequest()";
            SqlConnection conn = null;

            BServiceRequest serviceRequest = null;
            err = "";
            notFound = false;
            alreadyBooked = false;
            curSlotDate = Date.blankDate;
            curBookingType = "";

            try
            {
                conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {
                            serviceRequest = new BServiceRequest();
                            serviceRequest.BookServiceRequest(conn, trans,
                                zone, technicianId, slotDate, slotNo, multiSlot, bookingType, serviceRequestNo, IsAM, Convert.ToInt32(allocatedBy),
                                out notFound, out alreadyBooked, out curSlotDate, out curBookingType, reassignCode, reassignedBy);      //CR1030 jec
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


        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public int FreeServiceRequest(
            int technicianId, DateTime slotDate, short slotNo, out String err)
        {
            Function = "BServiceRequest::FreeServiceRequest()";
            SqlConnection conn = null;

            BServiceRequest serviceRequest = null;
            err = "";

            try
            {
                conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {
                            serviceRequest = new BServiceRequest();
                            serviceRequest.FreeServiceRequest(conn, trans,
                                technicianId, slotDate, slotNo);
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

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public int GetWarrantyLength(string prodCode, int stockLocn, out string err)
        {
            //rm CR1051 method to get MAN warranty length based on product code and stocklocn (refcode)
            Function = "BServiceManager::GetWarrantyLength()";
            int len = 1;
            err = "";

            try
            {
                BServiceRequest serviceRequest = new BServiceRequest();
                len = serviceRequest.GetWarrantyLength(prodCode, stockLocn);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return len;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet GetTechnicians(DateTime dateAvailable, out string err)
        {
            Function = "BServiceManager::GetTechnicians()";
            DataSet ds = null;
            err = "";

            try
            {
                BServiceRequest serviceRequest = new BServiceRequest();
                ds = serviceRequest.GetTechnicians(dateAvailable);
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
        public DataSet GetTechniciansByZone(DateTime dateAvailable, out string err)
        {
            Function = "BServiceManager::GetTechniciansByZone()";
            DataSet ds = null;
            err = "";

            try
            {
                BServiceRequest serviceRequest = new BServiceRequest();
                ds = serviceRequest.GetTechniciansByZone(dateAvailable);
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
        public DataSet CreateServiceRequest(
            short serviceBranchNo,
            string serviceType,
            string custId,
            string accountNo,
            int invoiceNo,
            string user,
            short stockLocn,
            string productCode,
            string description,
            DateTime purchaseDate,
            decimal unitPrice,
            string serialNo,
            int? printLocn,
            int itemId,             // RI
            out string err)
        {
            Function = "BServiceManager::CreateServiceRequest()";
            SqlConnection conn = null;

            DataSet ds = null;
            err = "";

            try
            {
                conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {
                            BServiceRequest serviceRequest = new BServiceRequest();
                            ds = serviceRequest.CreateServiceRequest(conn, trans,
                                    serviceBranchNo,
                                    serviceType,
                                    custId,
                                    accountNo,
                                    invoiceNo,
                                    user,
                                    stockLocn,
                                    productCode,
                                    description,
                                    purchaseDate,
                                    unitPrice,
                                    serialNo,
                                    printLocn,
                                    itemId             // RI
                                    );
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


        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet LoadPriceIndexMatrix(bool screenDisplay, out string err)
        {
            Function = "BServiceManager::LoadPriceIndexMatrix()";
            DataSet ds = null;
            err = "";

            try
            {
                BServiceRequest serviceRequest = new BServiceRequest();
                ds = serviceRequest.LoadPriceIndexMatrix(screenDisplay);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return ds;
        }


        //IP - 09/02/11 - Sprint 5.10 - #2977 - Method to write off Service Charge To account
        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public void WriteOffSRChargeAcct(int serviceRequestNo, string chargeToAcct, decimal outstBal, string transType, string countryCode, short branchNo, out string err)
        {

            Function = "BServiceManager::WriteOffSRChargeAcct()";
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
                                //First we need to write off the Service Charge Acct by posting a SDW (Service Debt Writeoff) credit against the account to settle the account.
                                BTransaction b = new BTransaction();
                                b.User = STL.Common.Static.Credential.UserId;

                                b.WriteGeneralTransaction(conn, trans, chargeToAcct, branchNo,
                                   outstBal, transType, "",
                                   "", "", 0,
                                   countryCode, "", 0);

                                string comments = "The Charge To account for this Service Request has been written off";
                                //Finally we need to add a comment to the SR to indicate that the balance oweing was written off.
                                //ServiceRequestAddComments sr = new ServiceRequestAddComments(conn, trans).ExecuteNonQuery(serviceRequestNo, user, date, comments);
                                ServiceRequestAddComments sr = new ServiceRequestAddComments(conn, trans);
                                sr.ExecuteNonQuery(serviceRequestNo, STL.Common.Static.Credential.UserId, DateTime.Now, comments);

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

        //IP - 10/02/11 - Sprint 5.10 - #2978 - Method to cancel service charge to account
        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public void CancelSRChargeAcct(int serviceRequestNo, string chargeToAcct, decimal transValue, string countryCode, out string err)
        {
            Function = "BServiceManager::CancelSRChargeAcct()";
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

                                BAccount acct = new BAccount();
                                acct.User = STL.Common.Static.Credential.UserId;

                                //First we need to cancel the charge to account (transfer of the remaining balance to sundry is handled within the CancelAccount method).
                                acct.CancelAccount(conn, trans, chargeToAcct, "",
                                                   Convert.ToInt16(chargeToAcct.Substring(0, 3)), "SRE", transValue,
                                                   countryCode, "",0);

                                StringBuilder sb = new StringBuilder();
                                sb.Append(
                                    "The Charge To account for this Service Request has been cancelled. The balance has been transferred to sundry on ");
                                if (Convert.ToString(DateTime.Now.Day).Length == 1)
                                {
                                    sb.Append("0");
                                }
                                sb.Append(Convert.ToString(DateTime.Now.Day));
                                sb.Append("/");
                                if (Convert.ToString(DateTime.Now.Month).Length == 1)
                                {
                                    sb.Append("0");
                                }
                                sb.Append(Convert.ToString(DateTime.Now.Month));
                                sb.Append("/");
                                sb.Append(Convert.ToString(DateTime.Now.Year));
                                sb.Append(" because the customer did not return.");

                                string comments = Convert.ToString(sb);
                                //Finally we need to add a comment to the SR to indicate that the balance was transferred to sundry.
                                //ServiceRequestAddComments sr = new ServiceRequestAddComments(conn, trans).ExecuteNonQuery(serviceRequestNo, user, date, comments);
                                ServiceRequestAddComments sr = new ServiceRequestAddComments(conn, trans);
                                sr.ExecuteNonQuery(serviceRequestNo, STL.Common.Static.Credential.UserId, DateTime.Now,
                                                   comments);

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

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public void SRBatchPrintUpdatePrinted(string[] srPrinted, Int16 branchno)
        {
            Function = "BServiceManager::PrintedServiceBatch()";
            string err = "";
            string srlist = "";
            char sep = ',';


            foreach (string sr in srPrinted)
            {
                srlist += sr + sep;
            }
            srlist = srlist.Remove(srlist.Length -1 );

            try
            {
                new DServiceRequest().SRBatchPrintUpdatePrinted(srlist, branchno);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
        }

    }
}

