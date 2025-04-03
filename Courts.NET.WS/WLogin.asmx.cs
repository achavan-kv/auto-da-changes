using System;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Web.Services;
using System.Web.Services.Protocols;
using Blue;
using Blue.Admin;
using STL.BLL;
using STL.Common;
using STL.DAL;
using Blue.Admin.Password;

namespace STL.WS
{
    /// <summary>
    /// Summary description for BLogin.
    /// </summary>
    /// 
    [WebService(Namespace = "http://strategicthought.com/webservices/")]
    public class WLogin : CommonService
    {
        public WLogin()
        {
            //CODEGEN: This call is required by the ASP.NET Web Services Designer
            InitializeComponent();
        }

        #region Component Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
        }
        #endregion

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
        }

        /// <summary>
        /// This web method returns a boolean value depending on whether or
        /// not the credentials in the SOAP header were successfully
        /// authenticated
        /// </summary>
        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public string[] IsValid(DateTime clientdate, out string empeeNo, out string name, out string err)
        {
            Function = "BLogin::isValid()";
            string[] roles = null;
            empeeNo = "";
            name = "";
            err = "";

            try
            {
                BLL.BLogin login = new BLL.BLogin();
                if (User.Identity.IsAuthenticated)
                {
                    roles = login.GetRoles(STL.Common.Static.Credential.UserId.ToString(), out empeeNo);
                    name = login.GetEmployeeName(null, null, Convert.ToInt32(empeeNo));
                }
                else
                {
                    err = "User has not been Authenticated!";
                }

                if (!login.CheckTimeMatch(clientdate))
                    err = "There is a time difference between the client and server.";

            }
            catch (Exception ex)
            {
                err = ex.Message;
                logException(ex, Function);
            }
            return roles;
        }


        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public string[] GetRoles(string user, string pass, out int empeeNo)
        {
            Function = "BLogin::isValid()";
            empeeNo = 0;

            string[] roles = { };
            try
            {
                BLL.BLogin login = new BLL.BLogin();
                if (login.Valid(user, pass))
                {
                    string strEmpeeNo = "";
                    roles = login.GetRoles(user, out strEmpeeNo);
                    Int32.TryParse(strEmpeeNo, out empeeNo);
                }
            }
            catch (Exception ex)
            {

                logException(ex, Function);
            }

            return roles;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public int LoggedIn(string machineName, out bool loggedIn, out string err)
        {
            Function = "BLogin::LoggeIn()";
            //string [] roles = null;
            loggedIn = false;
            err = "";

            try
            {
                BLL.BLogin login = new BLL.BLogin();
                // rdb 10/04/08 spotted that this will use serverName we need to pick up machineName client side
                //loggedIn = login.SetLoggedIn(Environment.MachineName, STL.Common.Static.Credential.UserId, true);
                loggedIn = login.SetLoggedIn(machineName, STL.Common.Static.Credential.UserId, true);
            }
            catch (Exception ex)
            {
                err = ex.Message;
                logException(ex, Function);
            }
            return 0;
        }

        [WebMethod(Description = "this method will return the users name as part of logging in")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public string GetEmployeeName(int employeeNo, out string err)
        {
            Function = "BLogin::GetEmployeeName()";
            err = "";
            string name = "";

            try
            {
                BLogin login = new BLogin();
                name = login.GetEmployeeName(null, null, employeeNo);
            }
            catch (Exception ex)
            {
                err = Function + ":" + ex.Message;
                logException(ex, Function);
            }
            return name;
        }

        [WebMethod(Description = "This method will return the date that the password was changed.")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DateTime GetDatePasswordChanged(int employeeNo, out string err)
        {
            Function = "BLogin::GetDatePasswordChanged()";
            err = "";
            DateTime datePassChge = DateTime.Today;

            try
            {
                BLogin login = new BLogin();
                datePassChge = login.GetDatePasswordChanged(employeeNo);
            }
            catch (Exception ex)
            {
                err = Function + ":" + ex.Message;
                logException(ex, Function);
            }
            return datePassChge;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet GetEmployeeDetails(int EmployeeNo, string err)
        {
            Function = "BLogin::GetSalesStaffByType()";
            err = "";
            DataSet ds = null;

            try
            {
                BLogin login = new BLogin();
                ds = login.GetStaffDetails(EmployeeNo);
            }
            catch (Exception ex)
            {
                err = Function + ":" + ex.Message;
                logException(ex, Function);
            }
            return ds;
        }
        [WebMethod(Description = "this method will update or insert basic employee details")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public int SaveStaff(bool changePassword, short branchno, int empeeNo, string fACTEmpeeNo,
                                string firstName, string lastName, string employeeType, string password,
                                string dutyFree, int maxRow, short loggedIn, bool defaultRate, Single upliftRate, int empeechange, string err) //IP - 22/03/11 - #3299-LW73340- Changed upliftRate from int to float
        {
            Function = "BLogin::SaveStaff()";
            err = "";
            int status;


            try
            {
                BLogin login = new BLogin();
                status = login.SaveStaff(changePassword, branchno, empeeNo, fACTEmpeeNo,
                                            firstName, lastName, employeeType, password,
                                            dutyFree, maxRow, loggedIn, defaultRate, upliftRate, empeechange);

                // set the Cache object to null so that the next person to access it will renew it's contents via a trip to to the database

                Cache["Users"] = null;
            }
            catch (Exception ex)
            {
                err = Function + ":" + ex.Message;
                logException(ex, Function);
            }
            return 0;
        }
        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet GetSalesStaffByType(string employeeType, int branchno, out string err)
        {
            Function = "BLogin::GetSalesStaffByType()";
            err = "";
            DataSet ds = null;

            try
            {
                BLogin login = new BLogin();
                ds = login.GetSalesStaffByType(employeeType, branchno);
            }
            catch (Exception ex)
            {
                err = Function + ":" + ex.Message;
                logException(ex, Function);
            }
            return ds;
        }

        //IP - 03/10/2007 - Web Service call to method that returns a dataset
        //that contains logon history for the selected employee.
        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet GetEmployeeLogonHistory(int empeeno, out string err)
        {
            Function = "BLogin::GetEmployeeLogonHistory()";
            err = "";

            DataSet ds = null;

            try
            {
                BLogin login = new BLogin();
                ds = login.GetEmployeeLogonHistory(empeeno);
            }
            catch (Exception ex)
            {
                err = Function + ":" + ex.Message;
                logException(ex, Function);
            }
            return ds;
        }

        [WebMethod(Description = "this method will return the employees and number of allocations for credit staff allocation screen")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet GetStaffAllocationByType(int employeeType, int branchno, out string err)
        {
            Function = "BLogin::GetStaffAllocationByType()";
            err = "";
            DataSet ds = null;

            try
            {
                BLogin login = new BLogin();
                ds = login.GetStaffandAllocationsByType(employeeType, branchno);
            }
            catch (Exception ex)
            {
                err = Function + ":" + ex.Message;
                logException(ex, Function);
            }
            return ds;
        }


        public class SavePasswordResult
        {
            public bool IsValid
            {
                get;
                set;
            }

            public string ErrorMessage
            {
                get;
                set;
            }
        }

        private Blue.Admin.Password.ChangePasswordResult HandleChangePassword(string username, string newPassword, string loggedInUser)
        {
            var change = new ChangePassword(new DateTimeClock(), new BCryptPasswordHashingAlgorithm(), new Blue.Admin.Repositories.PasswordRepository());
            return change.ValidateChange(UserRepository.GetUserByLogin(username), newPassword, loggedInUser);
        }

        private const string NewOldPasswordEqual = "New password and current password should not be the same.";

        [WebMethod(Description = "This method will save a new password for the current user.")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public SavePasswordResult SavePassword(string newPassword, string oldPassword, string userLogin, string loggedInUser)
        {
            var returnValue = new SavePasswordResult()
            {
                IsValid = false
            };

            var result = new UserPasswordValidation(
                new Blue.Admin.UserRepository(Blue.Cosacs.EventStore.Instance),
                new Blue.DateTimeClock()).Validate(userLogin, oldPassword);

            if (result.IsValid)
            {
                switch (this.HandleChangePassword(userLogin, newPassword, loggedInUser))
                {
                    case ChangePasswordResult.Ok:
                        returnValue.IsValid = true;
                        break;

                    case ChangePasswordResult.TooSimple:
                        returnValue.ErrorMessage = PasswordComplexityParameters.Current.GetFrendlyUserText();
                        break;

                    case ChangePasswordResult.EqualsOld:
                        returnValue.ErrorMessage = NewOldPasswordEqual;
                        break;

                    case ChangePasswordResult.TooCommon:
                        returnValue.ErrorMessage = "Your password is too common. Please change.";
                        break;
                }
            }
            else
            {
                returnValue.ErrorMessage = "The current password is not correct.";
            }

            return returnValue;
        }

        [WebMethod(Description = "this method will set the users loggedin parm to 0")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public void LogOff(string machineName, int userId, string userName)
        {
            Function = "WLogin::LogOff()";
            new Blue.Cosacs.Repositories.ConfigRepository().LogOff(userName, machineName, userId);
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public int TerminateEmployee(int empeeno, int empeechange, out string err)
        {
            Function = "WLogin::TerminateEmployee()";
            int status = 0;
            err = "";

            SqlConnection conn = null;


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
                            BLogin bo = new BLogin();
                            status = bo.TerminateEmployee(conn, trans, empeeno, empeechange);

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
                err = Function + ": " + ex.Message;
                logException(ex, Function);
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
            }
            return status;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public int CashTillOpenLoadEmployee(int user, string tillid, out string err)
        {
            Function = "WLogin::CashTillOpenLoadEmployee()";
            int empeeno = 0;
            err = "";

            try
            {
                BLogin bo = new BLogin();
                empeeno = bo.CashTillOpenLoadEmployee(user, tillid);
            }
            catch (Exception ex)
            {
                err = Function + ": " + ex.Message;
                logException(ex, Function);
            }

            return empeeno;
        }

        [WebMethod(Description = "this method will return the employees and commission due for the Calculate Commission Screen.")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet LoadCommissionDetailsByType(string employeeType, int branchNo, out string err)
        {
            Function = "BLogin::LoadCommissionDetailsByType()";
            err = "";
            DataSet ds = null;

            try
            {
                BLogin login = new BLogin();
                ds = login.LoadCommissionDetailsByType(employeeType, branchNo);
            }
            catch (Exception ex)
            {
                err = Function + ": " + ex.Message;
                logException(ex, Function);
            }
            return ds;
        }

        [WebMethod(Description = "this method will return the employees and commission due for the Calculate Commission Screen.")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public int CalculateBailiffCommission(string employeeType, int branchNo, out string err)
        {
            Function = "BLogin::CalculateBailiffCommission()";
            err = "";
            DataSet ds = null;

            SqlConnection conn = null;


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
                            BCommissionBasis bc = new BCommissionBasis();
                            bc.CalculateBailiffCommission(conn, trans, branchNo, employeeType);

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
                err = Function + ": " + ex.Message;
                logException(ex, Function);
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
            }
            return 0;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public bool CheckRolePermission(int empeeNo, string password, string permissionName, out string message, out string err)
        {
            Function = "BLogin::CheckRolePermission()";
            err = "";
            message = "";

            bool rtnValue = false;

            try
            {
                BLogin login = new BLogin();
                rtnValue = login.CheckRolePermission(empeeNo, password, permissionName, out message);
            }
            catch (Exception ex)
            {
                err = Function + ":" + ex.Message;
                logException(ex, Function);
            }

            return rtnValue;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public bool CheckConfig(char country, int branch)
        {
            Function = "BLogin::CheckConfig()";

            BCountry bc = new BCountry();
            return bc.CheckConfig(country, branch);
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public void Exception(Common.Error e) //Common.ExceptionDetail e)
        {
            var errorLog = Elmah.ErrorLog.GetDefault(System.Web.HttpContext.Current);

            var error = new Elmah.Error();
            error.Message = e.Message;
            error.Source = e.Source;
            error.Detail = e.Details;

            error.ApplicationName = string.Format("{0} ({1})", e.ProcessName, e.Version);
            error.HostName = e.MachineName;
            // ProcessName
            error.Time = e.Timestamp;
            error.User = e.IdentityName;
            error.Type = e.ExceptionType;

            errorLog.Log(error);
        }
    }
}
