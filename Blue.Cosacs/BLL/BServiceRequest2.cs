using STL.Common.Constants.ColumnNames;
using STL.Common.Constants.TableNames;
using STL.Common.ServiceRequest;
using STL.DAL;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace STL.BLL
{
    partial class BServiceRequest
    {
        #region [ Technician ]
        public DataSet GetTechnicianList()
        {
            DServiceRequest sr = new DServiceRequest();
            DataTable dtTech = sr.GetTechnicianList();
            DataTable dtZones = sr.GetAllTechnicianZones();

            DataSet ds = new DataSet();
            ds.Tables.Add(dtTech);
            ds.Tables.Add(dtZones);
            return ds;
        }

        public DataSet GetTechnicianPayments(int technicianId)
        {
            DServiceRequest sr = new DServiceRequest();
            DataTable dt = sr.GetTechnicianPayments(technicianId);
            dt.TableName = TN.ServiceTechnicianPayments;

            DataSet ds = new DataSet();
            ds.Tables.Add(dt);

            return ds;
        }

        public List<decimal> GetTechnicianPaymentSummary(int technicianId)
        {
            DServiceRequest sr = new DServiceRequest();
            List<decimal> technicianSummary = sr.GetTechnicianPaymentSummary(technicianId);

            return technicianSummary;
        }

        public DataSet GetTechniciansForPaymentScreen()
        {
            DServiceRequest sr = new DServiceRequest();
            DataTable dt = sr.GetTechniciansForPaymentsScreen();
            dt.TableName = TN.ServiceTechniciansForPaymentScreen;

            DataSet ds = new DataSet();
            ds.Tables.Add(dt);

            return ds;
        }

        public void SaveDetailsForAllTechnicians(SqlConnection conn, SqlTransaction trans, string csvZones,
            string hoursFrom, string hoursTo, int callsPerDay, out int noTechniciansExceedingCallsPerDay, out int technicianWithHighestNoSlots)
        {
            DServiceRequest sr = new DServiceRequest();
            sr.SaveDetailsForAllTechnicians(conn, trans, hoursFrom, hoursTo, callsPerDay,
                out noTechniciansExceedingCallsPerDay, out technicianWithHighestNoSlots);

            //This means we cannot continue as some technicians have more booked slots than the required calls per day
            if (noTechniciansExceedingCallsPerDay > 0)
            {
                return;
            }

            //Delete all existing zones
            sr.DeleteTechnicianZones(conn, trans);

            //Set all technicians to have the same zones       
            sr.SaveTechnicianZones(conn, trans, csvZones);
        }

        public DataSet GetServiceAllocationPayment(int technicianId)
        {
            DServiceRequest sr = new DServiceRequest();
            DataTable dt = sr.GetServiceAllocationPayment(technicianId);
            DataSet ds = new DataSet();
            ds.Tables.Add(dt);
            return ds;
        }

        public DataSet CheckTechnicianSlots(int technicianId, int calls)
        {
            DServiceRequest sr = new DServiceRequest();
            DataTable dt = sr.CheckTechnicianSlots(technicianId, calls);

            DataSet ds = new DataSet();
            ds.Tables.Add(dt);
            return ds;
        }

        public DataSet CheckPartsDate(long serviceRequestNo)
        {
            DServiceRequest sr = new DServiceRequest();
            int srNo = this.GetSRNo(serviceRequestNo);
            DataTable dt = sr.CheckPartsDate(srNo);

            DataSet ds = new DataSet();
            ds.Tables.Add(dt);
            return ds;
        }

        public DataTable GetTechnician(int technicianUniqueId)
        {
            DServiceRequest sr = new DServiceRequest();
            return sr.GetTechnician(technicianUniqueId);
        }

        public void SaveTechnician(SqlConnection conn, SqlTransaction trans, DataSet dsTechnician)
        {
            if (dsTechnician.Tables.IndexOf(TN.Technician) < 0)
            {
                return;
            }

            if (dsTechnician.Tables[TN.Technician].Rows.Count == 0)
            {
                return;
            }

            //Save technician
            DServiceRequest sr = new DServiceRequest();
            DataRow dr = dsTechnician.Tables[TN.Technician].Rows[0];

            sr.TechTitle = (string)dr[CN.Title];
            sr.TechFirstName = (string)dr[CN.FirstName];
            sr.TechLastName = (string)dr[CN.LastName];
            sr.TechPhoneNo = (string)dr[CN.PhoneNo];
            sr.TechMobileNo = (string)dr[CN.MobileNo];
            sr.TechAddress1 = (string)dr[CN.Address1];
            sr.TechAddress2 = (string)dr[CN.Address2];
            sr.TechAddress3 = (string)dr[CN.Address3];
            sr.TechAddressPC = (string)dr[CN.AddressPC];
            sr.TechHoursFrom = (string)dr[CN.HoursFrom];
            sr.TechHoursTo = (string)dr[CN.HoursTo];
            sr.TechCallsPerDay = Convert.ToInt32(dr[CN.CallsPerDay]);
            sr.TechInternal = (string)dr[CN.Internal];
            sr.TechComments = (string)dr[CN.TechComments];
            if (dr[CN.VacationStartDate] != DBNull.Value)
            {
                sr.VacationFrom = (DateTime)dr[CN.VacationStartDate];
            }
            else
            {
                sr.VacationFrom = DBNull.Value;
            }

            if (dr[CN.VacationEndDate] != DBNull.Value)
            {
                sr.VacationTo = (DateTime)dr[CN.VacationEndDate];
            }
            else
            {
                sr.VacationTo = DBNull.Value;
            }

            int technicianId = Convert.ToInt32(dr[CN.TechnicianId]);
            if (technicianId == -1)
            {
                technicianId = sr.SaveTechnician(conn, trans);
                dr[CN.TechnicianId] = technicianId;
            }
            else
            {
                sr.SaveTechnician(conn, trans, Convert.ToInt32(dr[CN.TechnicianId]));
            }

            //Save Zones
            if (dsTechnician.Tables.IndexOf(TN.ServiceZone) >= 0)
            {
                StringBuilder sbZones = new StringBuilder("");
                foreach (DataRow drZone in dsTechnician.Tables[TN.ServiceZone].Rows)
                {
                    sbZones.Append(drZone[CN.Code].ToString() + ",");
                }
                string zones = sbZones.ToString();
                sr.SaveTechnicianZones(conn, trans, technicianId, zones);
            }
        }

        public void DeleteTechnician(SqlConnection conn, SqlTransaction trans, int technicianId)
        {
            DServiceRequest sr = new DServiceRequest();
            sr.DeleteTechnician(conn, trans, technicianId);
        }

        public void SaveTechnicianPayment(SqlConnection conn, SqlTransaction trans, int technicianId, long serviceRequestNo, DateTime DateClosed, decimal totalCost, string status)
        {
            DServiceRequest sr = new DServiceRequest();
            sr.technicianId = technicianId;
            sr.dateClosed = DateClosed;
            sr.totalCost = totalCost;
            sr.status = status;
            int srNo = this.GetSRNo(serviceRequestNo);
            sr.SaveTechnicianPayment(conn, trans, srNo);

        }

        // Gurpreet - CR2018-010 - 31/10/18 - Setting of max no. of Jobs & Validation with allocated jobs for a technician.
        /// <summary>
        /// Gets the no of jobs allocated to a technician.
        /// </summary>
        /// <param name="technicianId"></param>
        /// <returns></returns>
        public DataTable GetAllocatedTechnicianJobs(int technicianId)
        {
            DServiceRequest sr = new DServiceRequest();
            return sr.GetAllocatedTechnicianJobs(technicianId);
        }
        //CR2018-010 Changes End
        #endregion

        #region [ Search ]
        public DataSet ServiceSearch(long serviceNo, string custId, string firstName, string lastName, bool showCourts, bool showNonCourts, bool showInternal,  //CR1030 jec
           bool showCashGo, int seachLimit, bool exactMatch)
        {
            DServiceRequest serviceRequest = new DServiceRequest();
            int srNo;
            short branchNo;
            this.SplitSRNo(serviceNo, out branchNo, out srNo);

            long srNoFinal = srNo;
            //This is deal with the branch number being part of the srno now.
            if (serviceNo.ToString().Length <= 3)
            {
                srNoFinal = serviceNo;
            }

            DataTable dt = serviceRequest.ServiceSearch(srNoFinal, custId, firstName, lastName, showCourts, showNonCourts, showInternal, showCashGo,       //CR1030 jec
              seachLimit, exactMatch, serviceNo.ToString());

            DataSet ds = new DataSet();
            ds.Tables.Add(dt);
            return ds;
        }

        public bool CheckSerialNoDup(string itemno, string serialno, string modelno, string accountNo)      //CR1030 jec
        {
            DServiceRequest serviceRequest = new DServiceRequest();
            return serviceRequest.CheckSerialNoDup(itemno, serialno, modelno, accountNo);       //CR1030 jec
        }
        #endregion

        #region [ FoodLoss ]

        public DataSet GetFoodLoss(int serviceRequestNo)
        {
            DataSet ds = new DataSet();
            DServiceRequest serviceRequest = new DServiceRequest();

            ds.Tables.Add(serviceRequest.GetFoodLoss(serviceRequestNo));

            return ds;
        }

        public void SaveFoodLoss(SqlConnection conn, SqlTransaction trans, int serviceRequestNo, DataSet dsFoodLoss)
        {
            DServiceRequest serviceRequest = new DServiceRequest();

            //Return if no food loss
            if (dsFoodLoss.Tables.IndexOf(TN.ServiceFoodLoss) < 0)
            {
                return;
            }

            // first delete existing items
            serviceRequest.DeleteFoodLoss(conn, trans, serviceRequestNo);

            // now save
            foreach (DataRow dr in dsFoodLoss.Tables[TN.ServiceFoodLoss].Rows)
            {
                serviceRequest.SaveFoodLoss(conn, trans, serviceRequestNo, dr[CN.ItemDescription].ToString(), (decimal)dr[CN.ItemValue]);
            }

        }

        #endregion

        #region [ Customer Interaction ]
        public DataSet GetCustomerInteraction(string custId, out string custFullName)
        {
            DServiceRequest sr = new DServiceRequest();
            DCustomer cust = new DCustomer();

            cust.GetCustomerDetails(null, null, custId);
            custFullName = string.Format("{0} {1} {2}", cust.Title, cust.FirstName, cust.Name);

            //If there is no customer name then we can assume the customer was not found
            if (custFullName.Trim().Equals(string.Empty))
            {
                return null;
            }

            DataTable dtCustomerInteraction = sr.GetCustomerInteraction(custId);
            DataTable dtCustServiceReq = sr.GetCustomerServiceRequests(custId);
            DataTable dtCustomerServiceAccounts = sr.GetCustomerServiceAccounts(custId);

            dtCustomerInteraction.TableName = TN.ServiceCustomerInteraction;
            dtCustServiceReq.TableName = TN.ServiceRequest;
            dtCustomerServiceAccounts.TableName = TN.CustomerAccounts;

            DataSet ds = new DataSet();
            ds.Tables.Add(dtCustomerInteraction);
            ds.Tables.Add(dtCustServiceReq);
            ds.Tables.Add(dtCustomerServiceAccounts);

            return ds;
        }

        public void SaveCustomerInteraction(SqlConnection conn, SqlTransaction trans, string custId, DataSet CustomerInteractionDataSet)
        {
            DServiceRequest sr = new DServiceRequest();

            foreach (DataRow dr in CustomerInteractionDataSet.Tables[TN.ServiceCustomerInteraction].Rows)
            {
                if (dr.RowState == DataRowState.Added)
                {

                    DateTime date = Convert.ToDateTime(dr[CN.Date]);
                    string code = dr[CN.Code].ToString();
                    int employeeNo = Convert.ToInt32(dr[CN.EmployeeNo]);
                    string accountNo = dr[CN.AcctNo].ToString();
                    int serviceRequestNo = this.GetSRNo(Convert.ToInt64(dr[CN.ServiceRequestNoStr]));
                    string comments = dr[CN.Comments].ToString();

                    sr.SaveCustomerInteraction(conn, trans, custId, date, code, employeeNo, accountNo, serviceRequestNo, comments);
                }
            }
        }

        #endregion

        #region [ Reports ]

        public DataSet GetOutstandingServiceReport(string reportType, object minLogged, object maxLogged, object minServiceNo, object maxServiceNo,
                        object technicianId, object notUpdatedSince, int daysOutstanding, object serviceStatus, object viewTop)   //CR1030 jec
        {
            DServiceRequest serviceRequest = new DServiceRequest();

            if (minLogged != null)
            {
                serviceRequest.ReportDateLoggedMin = Convert.ToDateTime(minLogged);
            }

            if (maxLogged != null)
            {
                serviceRequest.ReportDateLoggedMax = Convert.ToDateTime(maxLogged);
            }

            if (technicianId != null)
            {
                serviceRequest.ReportTechnicianId = Convert.ToInt32(technicianId);
            }

            if (notUpdatedSince != null)
            {
                serviceRequest.ReportDateNotUpdatedSince = Convert.ToDateTime(notUpdatedSince);
            }

            if (minServiceNo != null)
            {
                serviceRequest.ReportServiceRequestNoMin = this.GetSRNo(Convert.ToInt64(minServiceNo));
            }

            if (maxServiceNo != null)
            {
                serviceRequest.ReportServiceRequestNoMax = this.GetSRNo(Convert.ToInt64(maxServiceNo));
            }

            if (reportType == ServiceReport.RepairTotalExceeded || reportType == ServiceReport.ReassignTechnician)        //jec 28/01/11
            {
                serviceRequest.ReportServiceStatus = Convert.ToString(serviceStatus);
            }

            serviceRequest.ReportViewTop = Convert.ToBoolean(viewTop);

            DataTable dt = serviceRequest.GetOutstandingServiceReport(reportType, daysOutstanding);     //CR1030 jec
            dt.TableName = TN.Report;

            DataSet ds = new DataSet();
            ds.Tables.Add(dt);

            return ds;
        }

        public DataSet GetBatchPrintReports(int? technichianID, DateTime? fromDate, DateTime? toDate,
                                            int? minSR, int? maxSR, int printLocation, bool reprintOnly, bool showAll)
        {
            var ds = new DServiceRequest()
                            .GetBatchPrintReports(technichianID, fromDate, toDate,
                                                  minSR, maxSR, reprintOnly, printLocation, showAll);
            ds.Tables[0].TableName = TN.Report;
            return ds;
        }

        public DataSet GetServiceProgressReport(object minLogged, object maxLogged, object productCode, object productGroup, bool outstanding, bool allocated, bool completed, string fault, int serviceBranch)
        {
            DServiceRequest serviceRequest = new DServiceRequest();

            if (minLogged != null)
            {
                serviceRequest.ReportDateLoggedMin = Convert.ToDateTime(minLogged);
            }

            if (maxLogged != null)
            {
                serviceRequest.ReportDateLoggedMax = Convert.ToDateTime(maxLogged);
            }

            if (productCode != null)
            {
                serviceRequest.ReportProductCode = Convert.ToString(productCode);
            }

            if (productGroup != null)
            {
                serviceRequest.ReportProductGroup = Convert.ToInt32(productGroup);
            }

            serviceRequest.ReportOutstanding = outstanding;
            serviceRequest.ReportAllocated = allocated;
            serviceRequest.ReportCompleted = completed;
            serviceRequest.fault = fault;
            serviceRequest.serviceBranch = serviceBranch;

            DataTable dt = serviceRequest.GetServiceProgressReport();
            dt.TableName = TN.Report;

            DataSet ds = new DataSet();
            ds.Tables.Add(dt);

            return ds;

        }

        public DataSet GetServiceFailureReport(DateTime minLogged, DateTime maxLogged, string fault, string product, bool quarters)
        {
            DServiceRequest serviceRequest = new DServiceRequest();

            serviceRequest.ReportProductSoldMin = minLogged;
            serviceRequest.ReportProductSoldMax = maxLogged;
            serviceRequest.fault = fault;
            serviceRequest.productCode = product;
            serviceRequest.quarters = quarters;

            DataTable dt = serviceRequest.GetServiceFailureReport();
            dt.TableName = TN.Report;

            DataSet ds = new DataSet();
            ds.Tables.Add(dt);

            return ds;
        }

        public DataSet GetClaimsSubmissionReport(object minLogged, object maxLogged, object minServiceNo, object maxServiceNo, string fault)
        {
            DServiceRequest serviceRequest = new DServiceRequest();

            if (minLogged != null)
            {
                serviceRequest.ReportDateLoggedMin = Convert.ToDateTime(minLogged);
            }

            if (maxLogged != null)
            {
                serviceRequest.ReportDateLoggedMax = Convert.ToDateTime(maxLogged);
            }

            if (minServiceNo != null)
            {
                serviceRequest.ReportServiceRequestNoMin = this.GetSRNo(Convert.ToInt64(minServiceNo));
            }

            if (maxServiceNo != null)
            {
                serviceRequest.ReportServiceRequestNoMax = this.GetSRNo(Convert.ToInt64(maxServiceNo));
            }

            serviceRequest.fault = fault;

            DataTable dt = serviceRequest.GetClaimsSubmissionReport();
            dt.TableName = TN.Report;

            DataSet ds = new DataSet();
            ds.Tables.Add(dt);

            return ds;
        }

        //-- CR 1024 (NM 23/04/2009) -----------------------------------------
        public void UpdateBatchPrintFlag(string strSRNo, int branchNo)
        {
            int SRNo = Convert.ToInt32(strSRNo.Substring(3)); //To exclude 3-digit branch no
            DServiceRequest service = new DServiceRequest();
            service.UpdateBatchPrintFlag(SRNo, branchNo);
        }
        //--------------------------------------------------------------------
        #endregion

        #region [ Service Request ]

        public void RemoveDepositFromSR(SqlConnection conn, SqlTransaction trans, int serviceRequestNo)
        {
            DServiceRequest service = new DServiceRequest();
            service.RemoveDepositFromSR(conn, trans, serviceRequestNo);

        }
        public DataSet GetServiceRequestSummaryForAccount(string accountNo)
        {
            DServiceRequest serviceRequest = new DServiceRequest();
            DataTable dtResults = serviceRequest.GetServiceRequestSummaryForAccount(accountNo);

            dtResults.TableName = TN.ServiceRequest;

            DataSet ds = new DataSet();
            ds.Tables.Add(dtResults);

            return ds;
        }
        #endregion

        #region [ Service Request Audit ]
        public DataSet GetServiceRequestAudit(int serviceRequestNo)
        {

            DServiceRequest service = new DServiceRequest();

            DataSet ds = service.GetServiceRequestAudit(serviceRequestNo);
            ds.Tables[0].TableName = TN.ServiceRequestAudit;
            ds.Tables[1].TableName = TN.ServiceRequestAuditUpdate;
            return ds;

        }
        #endregion

        #region [ Service Resolution ]
        public DataSet ChargeToAuthorisationLoad()
        {
            DServiceRequest service = new DServiceRequest();

            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            dt = service.ChargeToAuthorisationLoad();
            ds.Tables.Add(dt);
            ds.Tables[0].TableName = TN.ServiceChargeToAuthorisation;
            return ds;
        }

        public void UpdateChargeToAuthorisation(SqlConnection conn, SqlTransaction trans, ArrayList al)
        {
            DServiceRequest service = new DServiceRequest();
            service.UpdateChargeToAuthorisation(conn, trans, al);
        }

        #endregion
        //Returns an service request no from a sr number that combines SR no with branch id
        private int GetSRNo(long combinedSRNo)
        {
            int srNo;
            short branchNo;

            this.SplitSRNo(combinedSRNo, out branchNo, out srNo);
            return srNo;
        }
    }

    //public class BServiceObjects
    //{
    //   public static List<BServiceObjects> GetTechnicianPaymentsSummary(int TechnicianID)
    //   {
    //      DServiceRequest dService = new DServiceRequest();

    //      DataReader dr = dService.GetByID(TechnicianID);

    //      BServiceObjects currentItem = new BServiceObjects();
    //      if (dr.Read())
    //      {
    //         currentItem.Name = dr["Name"];
    //         currentItem.DateService = dr["Date"];
    //      }

    //      return currentItem;
    //   }


    //}
}

