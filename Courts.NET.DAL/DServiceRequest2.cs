using BBSL.Libraries.Printing;
using STL.Common.Constants.ColumnNames;
using STL.Common.Constants.TableNames;
using STL.Common.ServiceRequest;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace STL.DAL
{
    /*
     *  A partial class to enable this class to be worked on by more than one person
     */
    partial class DServiceRequest
    {

        #region [ ---- Private Variables ---- ]

        private object _reportDateLoggedMin = null;
        private object _reportDateLoggedMax = null;
        private object _reportProductSoldMin = null;
        private object _reportProductSoldMax = null;
        private object _reportDateNotUpdatedSince = null;
        private object _reportServiceRequestNoMin = null;
        private object _reportServiceRequestNoMax = null;
        private object _reportTechnicianId = null;
        private object _reportServiceStatus = null;       //jec 19/01/11
        private object _reportViewTop = null;       //jec 21/01/11

        private bool _reportOutstanding;
        private bool _reportAllocated;
        private bool _reportCompleted;

        private object _reportProductCode;
        private object _reportProductGroup;
        #endregion

        #region [ ---- Public Properties ---- ]
        public DateTime ReportDateLoggedMin { set { _reportDateLoggedMin = value; } }
        public DateTime ReportDateLoggedMax { set { _reportDateLoggedMax = value; } }
        public DateTime ReportProductSoldMin { set { _reportProductSoldMin = value; } }
        public DateTime ReportProductSoldMax { set { _reportProductSoldMax = value; } }
        public DateTime ReportDateNotUpdatedSince { set { _reportDateNotUpdatedSince = value; } }
        public Int32 ReportServiceRequestNoMin { set { _reportServiceRequestNoMin = value; } }
        public Int32 ReportServiceRequestNoMax { set { _reportServiceRequestNoMax = value; } }
        public Int32 ReportTechnicianId { set { _reportTechnicianId = value; } }
        public string ReportServiceStatus { set { _reportServiceStatus = value; } }      //jec 19/01/11
        public bool ReportViewTop { set { _reportViewTop = value; } }      //jec 19/01/11
        public bool ReportOutstanding { set { _reportOutstanding = value; } }
        public bool ReportAllocated { set { _reportAllocated = value; } }
        public bool ReportCompleted { set { _reportCompleted = value; } }

        public string ReportProductCode { set { _reportProductCode = value; } }
        public Int32 ReportProductGroup { set { _reportProductGroup = value; } }

        #endregion

        #region [ Technician ]
        public DataTable GetTechnician(int technicianUniqueId)
        {
            DataTable technician = new DataTable();
            try
            {
                parmArray = new SqlParameter[1];
                parmArray[0] = new SqlParameter("@TechnicianId", SqlDbType.Int);
                parmArray[0].Value = technicianUniqueId;


                this.RunSP("DN_SRGetTechnicianSP", parmArray, technician);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return technician;
        }

        public DataTable GetTechnician(string firstName, string lastName)
        {
            DataTable technician = new DataTable();
            try
            {
                parmArray = new SqlParameter[2];
                parmArray[0] = new SqlParameter("@firstName", SqlDbType.NVarChar, 30);
                parmArray[0].Value = firstName;

                parmArray[1] = new SqlParameter("@lastName", SqlDbType.NVarChar, 30);
                parmArray[1].Value = lastName;

                this.RunSP("DN_SRGetTechnicianSP", parmArray, technician);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return technician;

        }

        public DataTable GetAllTechnicianZones()
        {
            DataTable zones = new DataTable(TN.ServiceZone);
            try
            {
                this.RunSP("DN_SRGetTechnicianZonesSP", zones);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return zones;

        }

        public DataTable GetTechnicianPayments(int technicianUniqueId)
        {
            DataTable technician = new DataTable();
            try
            {
                parmArray = new SqlParameter[1];
                parmArray[0] = new SqlParameter("@TechnicianId", SqlDbType.Int);
                parmArray[0].Value = technicianUniqueId;

                this.RunSP("DN_SRGetTechnicianPayments", parmArray, technician);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return technician;
        }

        public List<decimal> GetTechnicianPaymentSummary(int technicianUniqueId)
        {
            List<decimal> technician;
            try
            {
                parmArray = new SqlParameter[1];
                parmArray[0] = new SqlParameter("@TechnicianId", SqlDbType.Int);
                parmArray[0].Value = technicianUniqueId;

                technician = this.RunSPdrList("DN_SRGetTechnicianPaymentSummary", parmArray);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return technician;
        }

        /// <summary>
        /// Returns all technicians and the total each is due
        /// </summary>
        /// <returns></returns>
        public DataTable GetTechniciansForPaymentsScreen()
        {
            DataTable technicians = new DataTable();
            try
            {
                this.RunSP("DN_SRGetTechniciansForPaymentScreenSP", technicians);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return technicians;
        }

        /// <summary>
        /// This will delete all the zones for a specific technician
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="trans"></param>
        /// <param name="technicianId"></param>
        public void DeleteTechnicianZones(SqlConnection conn, SqlTransaction trans, int technicianId)
        {
            try
            {
                parmArray = new SqlParameter[1];
                parmArray[0] = new SqlParameter("@TechnicianId", SqlDbType.Int);
                parmArray[0].Value = technicianId;

                this.RunSP(conn, trans, "DN_SRDeleteTechnicianZones", parmArray);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }
        /// <summary>
        /// This will delete all zones for all technicians
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="trans"></param>
        public void DeleteTechnicianZones(SqlConnection conn, SqlTransaction trans)
        {
            try
            {
                this.RunSP(conn, trans, "DN_SRDeleteTechnicianZones");
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

        public void DeleteTechnician(SqlConnection conn, SqlTransaction trans, int technicianId)
        {
            try
            {
                parmArray = new SqlParameter[1];
                parmArray[0] = new SqlParameter("@technicianId", SqlDbType.Int);
                parmArray[0].Value = technicianId;

                this.RunSP(conn, trans, "DN_SRDeleteTechnicianSP", parmArray);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

        public void SaveDetailsForAllTechnicians(SqlConnection conn, SqlTransaction trans,
            string hoursFrom, string hoursTo, int callsPerDay, out int noTechniciansExceedingCallsPerDay, out int technicianWithHighestNoSlots)
        {
            try
            {
                parmArray = new SqlParameter[5];
                parmArray[0] = new SqlParameter("@HoursFrom", SqlDbType.Char, 5);
                parmArray[0].Value = hoursFrom;

                parmArray[1] = new SqlParameter("@HoursTo", SqlDbType.Char, 5);
                parmArray[1].Value = hoursTo;

                parmArray[2] = new SqlParameter("@CallsPerDay", SqlDbType.Int);
                parmArray[2].Value = callsPerDay;

                parmArray[3] = new SqlParameter("@NoTechniciansExceedingCallsPerDay", SqlDbType.Int);
                parmArray[3].Direction = ParameterDirection.Output;

                parmArray[4] = new SqlParameter("@TechnicianWithHighestNoSlots", SqlDbType.Int);
                parmArray[4].Direction = ParameterDirection.Output;

                this.RunSP(conn, trans, "DN_SRUpdateAllTechnicianDetails", parmArray);

                noTechniciansExceedingCallsPerDay = Convert.ToInt32(parmArray[3].Value);
                technicianWithHighestNoSlots = Convert.ToInt32(parmArray[4].Value);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }

        }

        /// <summary>
        /// Saves a new technician
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="trans"></param>
        /// <returns></returns>
        public int SaveTechnician(SqlConnection conn, SqlTransaction trans)
        {
            try
            {
                parmArray = new SqlParameter[17];

                parmArray[0] = new SqlParameter("@TechnicianId", SqlDbType.Int);
                parmArray[0].Direction = ParameterDirection.Output;

                parmArray[1] = new SqlParameter("@Title", SqlDbType.NVarChar, 25);
                parmArray[1].Value = this.TechTitle;

                parmArray[2] = new SqlParameter("@FirstName", SqlDbType.NVarChar, 30);
                parmArray[2].Value = this.TechFirstName;

                parmArray[3] = new SqlParameter("@LastName", SqlDbType.NVarChar, 25);
                parmArray[3].Value = this.TechLastName;

                parmArray[4] = new SqlParameter("@Address1", SqlDbType.NVarChar, 26);
                parmArray[4].Value = this.TechAddress1;

                parmArray[5] = new SqlParameter("@Address2", SqlDbType.NVarChar, 26);
                parmArray[5].Value = this.TechAddress2;

                parmArray[6] = new SqlParameter("@Address3", SqlDbType.NVarChar, 26);
                parmArray[6].Value = this.TechAddress3;

                parmArray[7] = new SqlParameter("@AddressPC", SqlDbType.NVarChar, 10);
                parmArray[7].Value = this.TechAddressPC;

                parmArray[8] = new SqlParameter("@PhoneNo", SqlDbType.VarChar, 20);
                parmArray[8].Value = this.TechPhoneNo;

                parmArray[9] = new SqlParameter("@MobileNo", SqlDbType.NVarChar, 20);
                parmArray[9].Value = this.TechMobileNo;

                parmArray[10] = new SqlParameter("@Internal", SqlDbType.Char, 1);
                parmArray[10].Value = this.TechInternal;

                parmArray[11] = new SqlParameter("@HoursFrom", SqlDbType.Char, 5);
                parmArray[11].Value = this.TechHoursFrom;

                parmArray[12] = new SqlParameter("@HoursTo", SqlDbType.Char, 5);
                parmArray[12].Value = this.TechHoursTo;

                parmArray[13] = new SqlParameter("@CallsPerDay", SqlDbType.NVarChar, 25);
                parmArray[13].Value = this.TechCallsPerDay;

                parmArray[14] = new SqlParameter("@VacationFrom", SqlDbType.SmallDateTime);
                parmArray[14].Value = this.VacationFrom;

                parmArray[15] = new SqlParameter("@VacationTo", SqlDbType.SmallDateTime);
                parmArray[15].Value = this.VacationTo;

                parmArray[16] = new SqlParameter("@Comments", SqlDbType.NVarChar, 2000);
                parmArray[16].Value = this.TechComments;

                // Gurpreet - CR2018-010 - 31/10/18 - Setting of max no. of Jobs & Validation with allocated jobs for a technician.
                parmArray[17] = new SqlParameter("@MaxJobs", SqlDbType.Int);
                parmArray[17].Value = this.MaxJobs;
                //CR2018-010 Changes End


                this.RunSP(conn, trans, "DN_SRSaveTechnicianSP", parmArray);

                return Convert.ToInt32((parmArray[0].Value));
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

        /// <summary>
        /// Saves an existing technician
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="trans"></param>
        public void SaveTechnician(SqlConnection conn, SqlTransaction trans, int technicianId)
        {
            try
            {
                parmArray = new SqlParameter[17];

                parmArray[0] = new SqlParameter("@TechnicianId", SqlDbType.Int);
                parmArray[0].Value = technicianId;

                parmArray[1] = new SqlParameter("@Title", SqlDbType.NVarChar, 25);
                parmArray[1].Value = this.TechTitle;

                parmArray[2] = new SqlParameter("@FirstName", SqlDbType.NVarChar, 30);
                parmArray[2].Value = this.TechFirstName;

                parmArray[3] = new SqlParameter("@LastName", SqlDbType.NVarChar, 25);
                parmArray[3].Value = this.TechLastName;

                parmArray[4] = new SqlParameter("@Address1", SqlDbType.NVarChar, 26);
                parmArray[4].Value = this.TechAddress1;

                parmArray[5] = new SqlParameter("@Address2", SqlDbType.NVarChar, 26);
                parmArray[5].Value = this.TechAddress2;

                parmArray[6] = new SqlParameter("@Address3", SqlDbType.NVarChar, 26);
                parmArray[6].Value = this.TechAddress3;

                parmArray[7] = new SqlParameter("@AddressPC", SqlDbType.NVarChar, 10);
                parmArray[7].Value = this.TechAddressPC;

                parmArray[8] = new SqlParameter("@PhoneNo", SqlDbType.VarChar, 20);
                parmArray[8].Value = this.TechPhoneNo;

                parmArray[9] = new SqlParameter("@MobileNo", SqlDbType.NVarChar, 20);
                parmArray[9].Value = this.TechMobileNo;

                parmArray[10] = new SqlParameter("@Internal", SqlDbType.Char, 1);
                parmArray[10].Value = this.TechInternal;

                parmArray[11] = new SqlParameter("@HoursFrom", SqlDbType.Char, 5);
                parmArray[11].Value = this.TechHoursFrom;

                parmArray[12] = new SqlParameter("@HoursTo", SqlDbType.Char, 5);
                parmArray[12].Value = this.TechHoursTo;

                parmArray[13] = new SqlParameter("@CallsPerDay", SqlDbType.NVarChar, 25);
                parmArray[13].Value = this.TechCallsPerDay;

                parmArray[14] = new SqlParameter("@VacationFrom", SqlDbType.SmallDateTime);
                parmArray[14].Value = this.VacationFrom;

                parmArray[15] = new SqlParameter("@VacationTo", SqlDbType.SmallDateTime);
                parmArray[15].Value = this.VacationTo;

                parmArray[16] = new SqlParameter("@Comments", SqlDbType.NVarChar, 2000);
                parmArray[16].Value = this.TechComments;

                // Gurpreet - CR2018-010 - 31/10/18 - Setting of max no. of Jobs & Validation with allocated jobs for a technician.
                parmArray[17] = new SqlParameter("@MaxJobs", SqlDbType.Int);
                parmArray[17].Value = this.MaxJobs;
                //CR2018-010 Changes End

                this.RunSP(conn, trans, "DN_SRSaveTechnicianSP", parmArray);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }

        }

        public void SaveTechnicianPayment(SqlConnection conn, SqlTransaction trans, int serviceRequestNo)
        {
            try
            {
                parmArray = new SqlParameter[5];

                parmArray[0] = new SqlParameter("@TechnicianId", SqlDbType.Int);
                parmArray[0].Value = this.technicianId;

                parmArray[1] = new SqlParameter("@ServiceRequestNo", SqlDbType.Int);
                parmArray[1].Value = serviceRequestNo;

                parmArray[2] = new SqlParameter("@DateClosed", SqlDbType.DateTime);
                parmArray[2].Value = this.dateClosed;

                parmArray[3] = new SqlParameter("@TotalCost", SqlDbType.Money);
                parmArray[3].Value = this.totalCost;

                parmArray[4] = new SqlParameter("@Status", SqlDbType.Char);
                parmArray[4].Value = this.status;


                this.RunSP(conn, trans, "DN_SRSaveTechnicianPayment", parmArray);

            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

        // Gurpreet - CR2018-010 - 31/10/18 - Setting of max no. of Jobs & Validation with allocated jobs for a technician.

        /// <summary>
        /// Gets the no. of jobs allocated to the technician using SP "DN_SRGetTechnicianJobsDetails"
        /// </summary>
        /// <param name="technicianId"></param>
        /// <returns></returns>
        public DataTable GetAllocatedTechnicianJobs(int technicianId)
        {


            DataTable dt_Jobs = new DataTable();
            try
            {
                parmArray = new SqlParameter[1];
                parmArray[0] = new SqlParameter("@technicianId", SqlDbType.Int);
                parmArray[0].Value = technicianId;

                this.RunSP("DN_SRGetTechnicianJobsDetails", parmArray, dt_Jobs);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return dt_Jobs;
        }

        /// <summary>
        /// Deletes the selected job which the user choses to override using SP "DN_SROverrideTechnicianJob"
        /// </summary>
        /// <param name="techId"></param>
        /// <param name="requestId"></param>
        public void OverrideBookingByRequestId(int techId, int requestId)
        {
            try
            {
                parmArray = new SqlParameter[2];
                parmArray[0] = new SqlParameter("@TechnicianId", SqlDbType.Int);
                parmArray[0].Value = techId;
                parmArray[1] = new SqlParameter("@RequestId", SqlDbType.Int);
                parmArray[1].Value = requestId;

                this.RunSP("DN_SROverrideTechnicianJob", parmArray);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

        /// <summary>
        /// Audits the process of job overriding done by the user using SP "DN_SRTechnicianJobOverrideAudit"
        /// </summary>
        /// <param name="oldRequestId"></param>
        /// <param name="overrideByUserId"></param>
        /// <param name="overideDate"></param>
        /// <param name="newRequestId"></param>
        public void JobOverrideAudit(int oldRequestId, int overrideByUserId, DateTime overideDate, int newRequestId)
        {
            try
            {
                parmArray = new SqlParameter[4];
                parmArray[0] = new SqlParameter("@OldRequestId", SqlDbType.Int);
                parmArray[0].Value = oldRequestId;
                parmArray[1] = new SqlParameter("@OverrideByUserId", SqlDbType.Int);
                parmArray[1].Value = overrideByUserId;
                parmArray[2] = new SqlParameter("@OverideDate", SqlDbType.SmallDateTime);
                parmArray[2].Value = overideDate;
                parmArray[3] = new SqlParameter("@NewRequestId", SqlDbType.Int);
                parmArray[3].Value = newRequestId;

                this.RunSP("DN_SRTechnicianJobOverrideAudit", parmArray);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }


        public DataTable GetUserAuthForOverride(int id, string pwd)
        {
            DataTable dt_Auth = new DataTable();
            try
            {
                parmArray = new SqlParameter[2];
                parmArray[0] = new SqlParameter("@UserId", SqlDbType.Int);
                parmArray[0].Value = id;
                parmArray[1] = new SqlParameter("@Password", SqlDbType.NVarChar);
                parmArray[1].Value = pwd;
                this.RunSP("DN_GetUserAuthentication", parmArray, dt_Auth);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return dt_Auth;
        }

        //CR2018-010 Changes End

        #endregion

        #region [ Search ]

        public DataTable ServiceSearch(long serviceNo, string custId, string firstName, string lastName, bool showCourts, bool showNonCourts, bool showInternal,    //CR1030 jec
           bool showCashGo, int seachLimit, bool exactMatch, string serviceRequestNoStr)
        {
            parmArray = new SqlParameter[11];

            parmArray[0] = new SqlParameter("@ServiceRequestNo", SqlDbType.Int);
            parmArray[0].Value = serviceNo;

            parmArray[1] = new SqlParameter("@CustId", SqlDbType.NVarChar, 20);
            parmArray[1].Value = custId;

            parmArray[2] = new SqlParameter("@FirstName", SqlDbType.NVarChar, 30);
            parmArray[2].Value = firstName;

            parmArray[3] = new SqlParameter("@LastName", SqlDbType.NVarChar, 60);
            parmArray[3].Value = lastName;

            parmArray[4] = new SqlParameter("@ShowCourts", SqlDbType.Bit);
            parmArray[4].Value = showCourts;

            parmArray[5] = new SqlParameter("@ShowNonCourts", SqlDbType.Bit);
            parmArray[5].Value = showNonCourts;

            parmArray[6] = new SqlParameter("@ShowInternal", SqlDbType.Bit);
            parmArray[6].Value = showInternal;

            parmArray[7] = new SqlParameter("@ShowCashGo", SqlDbType.Bit);
            parmArray[7].Value = showCashGo;

            parmArray[8] = new SqlParameter("@ExactMatch", SqlDbType.Bit);
            parmArray[8].Value = exactMatch;

            parmArray[9] = new SqlParameter("@SearchLimit", SqlDbType.Int);
            parmArray[9].Value = seachLimit;

            parmArray[10] = new SqlParameter("@ServiceRequestNoStr", SqlDbType.VarChar, 16);
            parmArray[10].Value = serviceRequestNoStr;

            DataTable dt = new DataTable(TN.ServiceRequest);

            this.RunSP("DN_SRServiceSearchSP", parmArray, dt);

            return dt;
        }

        public bool CheckSerialNoDup(string itemno, string serialno, string modelno, string accountNo)      //CR1030 jec
        {
            parmArray = new SqlParameter[4];

            //parmArray[0] = new SqlParameter("@itemno", SqlDbType.VarChar, 8);
            //parmArray[0].Value = itemno;
            parmArray[0] = new SqlParameter("@itemno", SqlDbType.VarChar, 18);                              //IP - 26/07/11 - RI
            parmArray[0].Value = itemno;
            parmArray[1] = new SqlParameter("@serialno", SqlDbType.VarChar, 30);
            parmArray[1].Value = serialno;
            parmArray[2] = new SqlParameter("@modelno", SqlDbType.VarChar, 30);
            parmArray[2].Value = modelno;
            parmArray[3] = new SqlParameter("@accountno", SqlDbType.Char, 12);      //CR1030 jec
            parmArray[3].Value = accountNo;

            return ReturnBool("SR_ServiceCheckSerialDup", parmArray);

        }

        #endregion

        #region [ Food Loss ]
        //
        public DataTable GetFoodLoss(int serviceRequestNo)
        {
            DataTable foodLoss = new DataTable(TN.ServiceFoodLoss);
            try
            {
                parmArray = new SqlParameter[1];

                parmArray[0] = new SqlParameter("@ServiceRequestNo", SqlDbType.Int);
                parmArray[0].Value = serviceRequestNo;

                this.RunSP("DN_SRGetFoodLossSP", parmArray, foodLoss);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }

            return foodLoss;

        }

        public void SaveFoodLoss(SqlConnection conn, SqlTransaction trans, int serviceRequestNo,
            string itemDescription, decimal itemValue)
        {
            try
            {
                parmArray = new SqlParameter[3];

                parmArray[0] = new SqlParameter("@ServiceRequestNo", SqlDbType.Int);
                parmArray[0].Value = serviceRequestNo;

                parmArray[1] = new SqlParameter("@ItemDescription", SqlDbType.NVarChar, 25);
                parmArray[1].Value = itemDescription;

                parmArray[2] = new SqlParameter("@ItemValue", SqlDbType.Money);
                parmArray[2].Value = itemValue;

                this.RunSP(conn, trans, "DN_SRSaveFoodLossSP", parmArray);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }

        }

        /// <summary>
        /// Deletes all food loss items that have the passed ServiceRequestNo
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="trans"></param>
        /// <param name="technicianId"></param>
        public void DeleteFoodLoss(SqlConnection conn, SqlTransaction trans, int serviceRequestNo)
        {
            try
            {
                parmArray = new SqlParameter[1];
                parmArray[0] = new SqlParameter("@serviceRequestNo", SqlDbType.Int);
                parmArray[0].Value = serviceRequestNo;

                this.RunSP(conn, trans, "DN_SRDeleteFoodLossSP", parmArray);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

        #endregion

        #region [ Customer Interaction ]
        public DataTable GetCustomerInteraction(string custId)
        {
            DataTable custInteraction = new DataTable();
            try
            {
                parmArray = new SqlParameter[1];
                parmArray[0] = new SqlParameter("@custId", SqlDbType.NVarChar, 20);
                parmArray[0].Value = custId;

                this.RunSP("DN_SRGetCustomerInteractionSP", parmArray, custInteraction);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return custInteraction;
        }

        public DataTable GetCustomerServiceRequests(string custId)
        {
            DataTable custServiceRequests = new DataTable();
            try
            {
                parmArray = new SqlParameter[1];
                parmArray[0] = new SqlParameter("@custId", SqlDbType.NVarChar, 20);
                parmArray[0].Value = custId;

                this.RunSP("DN_SRGetServiceRequestsForCustomerSP", parmArray, custServiceRequests);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return custServiceRequests;
        }

        public DataTable GetCustomerServiceAccounts(string custId)
        {
            DataTable custServiceAccounts = new DataTable();
            try
            {
                parmArray = new SqlParameter[1];
                parmArray[0] = new SqlParameter("@custId", SqlDbType.NVarChar, 20);
                parmArray[0].Value = custId;

                this.RunSP("DN_SRGetCustomerServiceAccountsSP", parmArray, custServiceAccounts);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return custServiceAccounts;
        }

        public void SaveCustomerInteraction(SqlConnection conn, SqlTransaction trans, string custId,
           DateTime date, string code, int EmployeeNo, string AccountNo, int ServiceRequestNo, string Comments)
        {
            try
            {
                parmArray = new SqlParameter[7];

                parmArray[0] = new SqlParameter("@CustId", SqlDbType.NVarChar, 20);
                parmArray[0].Value = custId;

                parmArray[1] = new SqlParameter("@Date", SqlDbType.DateTime);
                parmArray[1].Value = date;

                parmArray[2] = new SqlParameter("@Code", SqlDbType.VarChar, 12);
                parmArray[2].Value = code;

                parmArray[3] = new SqlParameter("@EmployeeNo", SqlDbType.Int);
                parmArray[3].Value = EmployeeNo;

                parmArray[4] = new SqlParameter("@AccountNo", SqlDbType.Char, 12);
                parmArray[4].Value = AccountNo;

                parmArray[5] = new SqlParameter("@ServiceRequestNo", SqlDbType.Int);
                parmArray[5].Value = ServiceRequestNo;

                parmArray[6] = new SqlParameter("@Comments", SqlDbType.NVarChar, 400);
                parmArray[6].Value = Comments;

                this.RunSP(conn, trans, "DN_SRSaveCustomerInteraction", parmArray);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }

        }
        #endregion

        #region [ Reports ]

        public DataTable GetOutstandingServiceReport(string serviceReport, int daysOutstanding)     //CR1030 jec
        {
            ArrayList al = new ArrayList();
            SqlParameter p = null;

            DateTime dtMinDate = Convert.ToDateTime((Convert.ToDateTime(_reportDateLoggedMin)).ToShortDateString());
            DateTime dtMaxDate = Convert.ToDateTime((Convert.ToDateTime(_reportDateLoggedMax)).ToShortDateString());

            if (_reportDateLoggedMin != null)
            {
                p = new SqlParameter("@MinDateLogged", SqlDbType.DateTime);
                p.Value = dtMinDate;
                al.Add(p);
            }
            if (_reportDateLoggedMax != null)
            {
                p = new SqlParameter("@MaxDateLogged", SqlDbType.DateTime);
                p.Value = dtMaxDate;
                al.Add(p);
            }
            //if (_reportServiceRequestNoMin != null)
            //{
            p = new SqlParameter("@MinSRNo", SqlDbType.Int);
            p.Value = _reportServiceRequestNoMin;
            al.Add(p);
            //}
            //if (_reportServiceRequestNoMax != null)
            //{
            p = new SqlParameter("@MaxSRNo", SqlDbType.Int);
            p.Value = _reportServiceRequestNoMax;
            al.Add(p);
            //}
            //View top      jec 21/01/11
            p = new SqlParameter("@ViewTop", SqlDbType.Bit);
            p.Value = _reportViewTop;
            al.Add(p);

            if (_reportTechnicianId != null)
            {
                p = new SqlParameter("@TechnicianId", SqlDbType.Int);
                p.Value = _reportTechnicianId;
                al.Add(p);
            }
            if (_reportDateNotUpdatedSince != null)
            {
                p = new SqlParameter("@NotUpdatedSinceDate", SqlDbType.DateTime);
                p.Value = _reportDateNotUpdatedSince;
                al.Add(p);
            }
            // Pass days outstanding for specific reports
            if (serviceReport == ServiceReport.AwaitingEstimate || serviceReport == ServiceReport.Unallocated || serviceReport == ServiceReport.ByDateLodged)
            {
                p = new SqlParameter("@DaysOutstanding", SqlDbType.Int);    //CR1030 jec
                p.Value = daysOutstanding;
                al.Add(p);
            }

            if (_reportServiceStatus != null)
            {
                p = new SqlParameter("@ServiceStatus", SqlDbType.VarChar);
                p.Value = _reportServiceStatus;
                al.Add(p);
            }
            parmArray = new SqlParameter[al.Count];
            foreach (object o in al)
            {
                parmArray[al.IndexOf(o)] = (SqlParameter)o;
            }
            DataTable dtResults = new DataTable();
            string spName = "";

            switch (serviceReport)
            {
                case ServiceReport.AwaitingEstimate:
                    spName = "DN_SRReportAwaitingEstimateSP";
                    break;
                case ServiceReport.AwaitingPayment:
                    spName = "DN_SRReportAwaitingPaymentSP";
                    break;
                case ServiceReport.AwaitingSpareParts:
                    spName = "DN_SRReportAwaitingSparePartsSP";
                    break;
                case ServiceReport.AwaitingDeposit:
                    spName = "DN_SRReportAwaitingDepositSP";
                    break;
                case ServiceReport.ByDateLodged:
                    spName = "DN_SRReportDateLoggedSP";
                    break;
                case ServiceReport.ByTechnician:
                    spName = "DN_SRReportByTechnicianSP";
                    break;
                case ServiceReport.NotUpdatedSince:
                    spName = "DN_SRReportNotUpdatedSinceSP";
                    break;
                case ServiceReport.Unallocated:
                    spName = "DN_SRReportUnallocatedSP";
                    break;
                case ServiceReport.RepairOverdue:
                    spName = "DN_SRReportRepairOverdueSP";        //CR1030 jec
                    break;
                case ServiceReport.FoodLoss:
                    spName = "DN_SRReportFoodLossSP";        //CR1030 jec
                    break;
                case ServiceReport.ReassignTechnician:
                    spName = "DN_SRReportReassignTechnicianSP";        //CR1030 jec
                    break;
                case ServiceReport.RepairTotalExceeded:
                    spName = "DN_SRReportRepairExceededSP";        //CR1030 jec
                    break;
            }
            base.RunSP(spName, parmArray, dtResults);

            return dtResults;

        }

        public DataSet GetBatchPrintReports(int? technichianID, DateTime? fromDate, DateTime? toDate,
                                            int? minSR, int? maxSR, bool reprintOnly,
                                            int printLocation, bool showAll)
        {
            var paramList = new List<SqlParameter>();

            if (fromDate.HasValue)
            {
                paramList.Add(new SqlParameter("@MinDateLogged", fromDate.Value.Date));
            }

            if (toDate.HasValue)
            {
                paramList.Add(new SqlParameter("@MaxDateLogged", toDate.Value.Date));
            }

            if (minSR.HasValue)
            {
                paramList.Add(new SqlParameter("@MinSRNo", minSR.Value));
            }

            if (maxSR.HasValue)
            {
                paramList.Add(new SqlParameter("@MaxSRNo", maxSR.Value));
            }

            if (technichianID.HasValue)
            {
                paramList.Add(new SqlParameter("@tech", technichianID.Value));
            }

            paramList.Add(new SqlParameter("@printLocn", printLocation));
            paramList.Add(new SqlParameter("@reprintOnly", reprintOnly)); // CR 1024 (NM 23/04/2009)   
            paramList.Add(new SqlParameter("@showAll", showAll)); // CR 1056

            DataSet ds = new DataSet();
            RunSP("DN_SRBatchPrintReport", paramList.ToArray(), ds);

            return ds;
        }

        public DataTable GetServiceProgressReport()
        {

            ArrayList al = new ArrayList();
            SqlParameter p = null;
            DataTable dtResults = new DataTable();

            DateTime dtMinDate = Convert.ToDateTime((Convert.ToDateTime(_reportDateLoggedMin)).ToShortDateString());
            DateTime dtMaxDate = Convert.ToDateTime((Convert.ToDateTime(_reportDateLoggedMax)).ToShortDateString());

            try
            {
                if (_reportDateLoggedMin != null)
                {
                    p = new SqlParameter("@MinDateLogged", SqlDbType.DateTime);
                    p.Value = dtMinDate;
                    al.Add(p);
                }
                if (_reportDateLoggedMax != null)
                {
                    p = new SqlParameter("@MaxDateLogged", SqlDbType.DateTime);
                    p.Value = dtMaxDate;
                    al.Add(p);
                }
                if (_reportProductCode != null)
                {
                    p = new SqlParameter("@ProductCode", SqlDbType.NVarChar, 18);           // RI
                    p.Value = _reportProductCode;
                    al.Add(p);
                }
                if (_reportProductGroup != null)
                {
                    p = new SqlParameter("@ProductGroup", SqlDbType.Int);
                    p.Value = _reportProductGroup;
                    al.Add(p);
                }
                p = new SqlParameter("@Outstanding", SqlDbType.Bit);
                p.Value = _reportOutstanding;
                al.Add(p);

                p = new SqlParameter("@Allocated", SqlDbType.Bit);
                p.Value = _reportAllocated;
                al.Add(p);

                p = new SqlParameter("@Completed", SqlDbType.Bit);
                p.Value = _reportCompleted;
                al.Add(p);

                p = new SqlParameter("@fault", SqlDbType.Char, 4);
                p.Value = _fault;
                al.Add(p);

                p = new SqlParameter("@branch", SqlDbType.Int);
                p.Value = _serviceBranch;
                al.Add(p);

                parmArray = new SqlParameter[al.Count];
                foreach (object o in al)
                {
                    parmArray[al.IndexOf(o)] = (SqlParameter)o;
                }

                base.RunSP("DN_SRReportServiceProgressSP", parmArray, dtResults);

            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return dtResults;
        }

        public DataTable GetServiceFailureReport()
        {
            DataTable dtResults = new DataTable();

            try
            {
                parmArray = new SqlParameter[5];
                parmArray[0] = new SqlParameter("@Mindate", SqlDbType.DateTime);
                parmArray[0].Value = _reportProductSoldMin;

                parmArray[1] = new SqlParameter("@Maxdate", SqlDbType.DateTime);
                parmArray[1].Value = _reportProductSoldMax;

                parmArray[2] = new SqlParameter("@fault", SqlDbType.Char, 4);
                parmArray[2].Value = _fault;

                //parmArray[3] = new SqlParameter("@product", SqlDbType.VarChar, 8);
                parmArray[3] = new SqlParameter("@product", SqlDbType.VarChar, 18);             //IP - 22/07/11 - RI - #4381
                parmArray[3].Value = _productCode;

                parmArray[4] = new SqlParameter("@quarters", SqlDbType.Bit);
                parmArray[4].Value = _quarters;

                this.RunSP("DN_SRReportServiceFailureSP", parmArray, dtResults);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return dtResults;
        }

        public DataTable GetClaimsSubmissionReport()
        {
            DataTable dtResults = new DataTable();
            try
            {
                //Create an array list of parameters incase some values are null
                ArrayList al = new ArrayList();
                SqlParameter p = null;

                DateTime dtMinDate = Convert.ToDateTime((Convert.ToDateTime(_reportDateLoggedMin)).ToShortDateString());
                DateTime dtMaxDate = Convert.ToDateTime((Convert.ToDateTime(_reportDateLoggedMax)).ToShortDateString());

                if (_reportDateLoggedMin != null)
                {
                    p = new SqlParameter("@MinDateLogged", SqlDbType.DateTime);
                    p.Value = dtMinDate;
                    al.Add(p);
                }
                if (_reportDateLoggedMax != null)
                {
                    p = new SqlParameter("@MaxDateLogged", SqlDbType.DateTime);
                    p.Value = dtMaxDate;
                    al.Add(p);
                }
                if (_reportServiceRequestNoMin != null)
                {
                    p = new SqlParameter("@MinSRNo", SqlDbType.Int);
                    p.Value = _reportServiceRequestNoMin;
                    al.Add(p);
                }
                if (_reportServiceRequestNoMax != null)
                {
                    p = new SqlParameter("@MaxSRNo", SqlDbType.Int);
                    p.Value = _reportServiceRequestNoMax;
                    al.Add(p);
                }
                //UAT 453
                p = new SqlParameter("@fault", SqlDbType.Char, 4);
                p.Value = _fault;
                al.Add(p);

                parmArray = new SqlParameter[al.Count];
                foreach (object o in al)
                {
                    parmArray[al.IndexOf(o)] = (SqlParameter)o;
                }

                this.RunSP("DN_SRReportClaimsSubmissionSP", parmArray, dtResults);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return dtResults;

        }

        //-- CR 1024 (NM 23/04/2009) -----------------------------------------
        public void UpdateBatchPrintFlag(int SRNo, int branchNo)
        {
            try
            {
                parmArray = new SqlParameter[2];
                parmArray[0] = new SqlParameter("@SRNo", SqlDbType.Int);
                parmArray[0].Value = SRNo;
                parmArray[1] = new SqlParameter("@BranchNo", SqlDbType.Int);
                parmArray[1].Value = branchNo;

                this.RunSP("DN_SRUpdateBatchPrintFlag", parmArray);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }
        //--------------------------------------------------------------------

        #endregion

        #region [ Service Request ]

        public void RemoveDepositFromSR(SqlConnection conn, SqlTransaction trans, int serviceRequestNo)
        {


            try
            {
                parmArray = new SqlParameter[1];

                parmArray[0] = new SqlParameter("@ServiceRequestNo", SqlDbType.Int);
                parmArray[0].Value = serviceRequestNo;

                this.RunSP(conn, trans, "DN_SRRemoveDepositSP", parmArray);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

        public DataTable GetServiceRequestSummaryForAccount(string accountNo)
        {
            DataTable dtResults = new DataTable();
            try
            {
                parmArray = new SqlParameter[1];
                parmArray[0] = new SqlParameter("@AcctNo", SqlDbType.NVarChar, CW.AccountNo);
                parmArray[0].Value = accountNo;

                this.RunSP("DN_SRGetServiceRequestSummaryForAccount", parmArray, dtResults);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return dtResults;
        }

        #endregion

        public List<ServiceItem> ServiceGetItemPrintInfo(string accountNo, string itemno, string stocklocn)
        {

            var serviceinfo = new List<ServiceItem>();


            parmArray = new SqlParameter[3];
            parmArray[0] = new SqlParameter("@acctno", SqlDbType.VarChar, 12) { Value = accountNo };
            parmArray[1] = new SqlParameter("@itemno", SqlDbType.VarChar, 8) { Value = itemno };
            parmArray[2] = new SqlParameter("@stocklocn", SqlDbType.VarChar, 3) { Value = stocklocn };

            using (var connection = new SqlConnection(connectionStr))
            {
                connection.Open();
                var command = CreateCommand("ServiceGetItemPrintInfo", parmArray, connection);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var sitem = new ServiceItem
                        {
                            serialno = reader["serialno"].ToString(),
                            modelno = reader["modelno"].ToString()
                        };
                        serviceinfo.Add(sitem);
                    }
                }
            }
            return serviceinfo;
        }


        #region [ Service Audit ]
        public DataSet GetServiceRequestAudit(int serviceRequestNo)
        {
            DataSet dsResults = new DataSet();
            try
            {
                parmArray = new SqlParameter[1];
                parmArray[0] = new SqlParameter("@serviceRequestNo", SqlDbType.Int);
                parmArray[0].Value = serviceRequestNo;



                this.RunSP("DN_SRGetServiceRequestAuditSP", parmArray, dsResults);

            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return dsResults;

        }
        #endregion

        #region [ Service Resolution ]
        public void SaveResolutionStatus(SqlConnection conn, SqlTransaction trans, string acctNo, int itemID, //IP - 22/07/11 - RI - string itemNo, - changed to use itemID
                                            short stockLocn, string replacementStatus)
        {


            try
            {
                parmArray = new SqlParameter[4];

                parmArray[0] = new SqlParameter("@AccountNo", SqlDbType.NChar, CW.AccountNo);
                parmArray[0].Value = acctNo;
                //parmArray[1] = new SqlParameter("@ItemNo", SqlDbType.NVarChar, CW.ItemNo);
                //parmArray[1].Value = itemNo;
                parmArray[1] = new SqlParameter("@ItemID", SqlDbType.Int);                              //IP - 22/07/11 - RI
                parmArray[1].Value = itemID;
                parmArray[2] = new SqlParameter("@StockLocn", SqlDbType.SmallInt);
                parmArray[2].Value = stockLocn;
                parmArray[3] = new SqlParameter("@ReplacementStatus", SqlDbType.NChar, 1);
                parmArray[3].Value = replacementStatus;

                this.RunSP(conn, trans, "DN_SRSaveResolutionStatusSP", parmArray);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

        public DataTable ChargeToAuthorisationLoad()
        {
            DataTable dt;
            try
            {
                dt = this.RunSPNoReturn("DN_SRChargeToAuthorisationLoadSP");
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return dt;
        }

        public void UpdateChargeToAuthorisation(SqlConnection conn, SqlTransaction trans, ArrayList al)
        {
            try
            {
                parmArray = new SqlParameter[6];

                parmArray[0] = new SqlParameter("@defaultChargeTo", SqlDbType.VarChar, 10);
                parmArray[0].Value = al[0].ToString();
                parmArray[1] = new SqlParameter("@chargeToCustomer", SqlDbType.Char, 1);
                parmArray[1].Value = al[1].ToString();
                parmArray[2] = new SqlParameter("@chargeToAIG", SqlDbType.Char, 1);
                parmArray[2].Value = al[2].ToString();
                parmArray[3] = new SqlParameter("@chargeToSupplier", SqlDbType.Char, 1);
                parmArray[3].Value = al[3].ToString();
                parmArray[4] = new SqlParameter("@chargeToInternal", SqlDbType.Char, 1);
                parmArray[4].Value = al[4].ToString();
                parmArray[5] = new SqlParameter("@chargeToDeliverer", SqlDbType.Char, 1);
                parmArray[5].Value = al[5].ToString();

                this.RunSP(conn, trans, "DN_SRChargeToAuthorisationUpdateSP", parmArray);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }


        #endregion
    }
}
