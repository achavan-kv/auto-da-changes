using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using STL.Common;
using STL.DAL;

namespace STL.BLL
{
    /// <summary>
    /// This class handles user validation and assigning 
    /// custom roles to the user object.
    /// The Valid method is called every time a web service
    /// is called.
    /// </summary>
    public class BLogin : CommonObject
    {
        private DLogin valid;
        private string[] rolesArray = null;

        public bool Valid(string UserID, string pass)
        {
            Function = "Login::Valid";
            int result = 0;

            valid = new DLogin();
            result = valid.Validate(UserID, pass);

            if (result >= 0)	//This means we have a valid user
            {
                rolesArray = valid.Roles;
            }

            return (result == 0);
        }


        public List<UserDetails> GetUserDetails()
        {
            DataTable dtUserDetails = new DataTable();
            DLogin login = new DLogin();
            List<UserDetails> details = new List<UserDetails>();

            try
            {
                dtUserDetails = login.GetUserDetails();

                foreach (DataRow dr in dtUserDetails.Rows)
                {
                    UserDetails user = new UserDetails();
                    user.empeeno = Convert.ToInt32(dr["empeeno"]);
                    user.password = dr["password"].ToString();
                    user.factEmployeeNo = dr["FactEmployeeNo"].ToString();
                    user.roles = new string[] { dr["empeetype"].ToString() };
                    details.Add(user);
                }
            }
            catch
            {
                throw;
            }
            return details;
        }

        public string[] GetRoles(string user, out string empeeNo)
        {
            Function = "Login::GetRoles()";
            DLogin login = new DLogin();
            return login.GetRoles(user, out empeeNo);
        }

        public string GetEmployeeName(SqlConnection conn, SqlTransaction trans, int employeeNo)
        {
            DataSet ds = new DataSet();
            DEmployee emp = new DEmployee();
            emp.GetEmployeeDetails(conn, trans, employeeNo);
            return emp.EmployeeName;
        }

        public DataSet GetSalesStaffByType(string employeeType, int branchno)
        {
            DataSet ds = new DataSet();
            DEmployee emp = new DEmployee();
            emp.GetSalesStaffByType(employeeType, branchno);
            ds.Tables.Add(emp.SalesStaff);
            return ds;
        }

        //IP 3/10/2007 - Method that retrieves the logon history for a selected
        //employee. The data table returned is placed into a new
        //instance of a data table which is placed in a dataset that 
        //is returned.
        public DataSet GetEmployeeLogonHistory(int empeeno)
        {
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            DEmployee empLogonHist = new DEmployee();
            dt = empLogonHist.GetEmployeeLogonHistory(empeeno);
            ds.Tables.Add(dt);
            return ds;
        }

        public DataSet GetStaffandAllocationsByType(int employeeType, int branchno)
        {
            DataSet ds = new DataSet();
            DEmployee emp = new DEmployee();
            emp.GetStaffandAllocationByType(employeeType, branchno);
            ds.Tables.Add(emp.SalesStaff);
            return ds;
        }
        public DataSet GetStaffDetails(int employeeNo)
        {
            DataSet ds = new DataSet();
            DEmployee details = new DEmployee();
            details.GetStaffDetails(employeeNo);
            ds.Tables.Add(details.Employees);
            return ds;
        }
        public int SaveStaff(bool changePassword, short branchno, int empeeNo, string fACTEmpeeNo,
            string firstName, string lastName, string employeeType, string password,
            string dutyFree, int maxRow, short loggedIn, bool defaultRate, Single upliftRate, int empeechange) //IP - 22/03/11 - #3299-LW73340- Changed upliftRate from int to float
        {
            DEmployee details = new DEmployee();
            return details.SaveStaff(changePassword, branchno, empeeNo, fACTEmpeeNo,
             firstName, lastName, employeeType, password, dutyFree, maxRow, loggedIn, defaultRate, upliftRate, empeechange);
        }

        public DateTime GetDatePasswordChanged(int employeeNo)
        {
            DataSet ds = new DataSet();
            DEmployee emp = new DEmployee();
            emp.GetEmployeeDetails(null, null, employeeNo);
            return emp.DatePassChge;
        }

        public void SavePassword(SqlConnection conn, SqlTransaction trans, string password, int empeeNo)
        {
            DEmployee emp = new DEmployee();
            emp.SavePassword(conn, trans, password, empeeNo);
        }

        public bool SetLoggedIn(string machineName, int employeeNo, bool loggedIn)
        {
            return new DEmployee().SetLoggedIn(machineName, employeeNo, loggedIn);
        }

        public BLogin()
        {
        }

        public string[] Roles
        {
            get
            {
                return rolesArray;
            }
        }

        /// <summary>
        /// TerminateEmployee
        /// </summary>
        /// <param name="empeeno">int</param>
        /// <returns>int</returns>
        /// 
        public int TerminateEmployee(SqlConnection conn, SqlTransaction trans, int empeeno, int empeechange)
        {
            int status = 0;
            DEmployee da = new DEmployee();
            status = da.TerminateEmployee(conn, trans, empeeno, empeechange);
            return status;
        }

        /// <summary>
        /// CashTillOpenLoadEmployee
        /// </summary>
        /// <param name="user">int</param>
        /// <returns>int</returns>
        /// 
        public int CashTillOpenLoadEmployee(int user, string tillid)
        {
            DEmployee da = new DEmployee();
            return da.CashTillOpenLoadEmployee(user, tillid); ;
        }

        public DataSet LoadCommissionDetailsByType(string employeeType, int branchNo)
        {
            DataSet ds = new DataSet();
            DEmployee emp = new DEmployee();
            emp.LoadCommissionDetailsByType(employeeType, branchNo);
            ds.Tables.Add(emp.SalesStaff);
            return ds;
        }

        public bool CheckRolePermission(int empeeNo, string password, string permissionName, out string message)
        {
            return (new DLogin()).CheckRolePermission(empeeNo, password, permissionName, out message);
        }

        public bool CheckTimeMatch(DateTime ClientTime)
        {
            var diff = ClientTime - DateTime.Now;
            if (Math.Abs(diff.TotalMinutes) > 10)
                return false;
            else
                return true;

        }
    }

    public struct UserDetails
    {
        public int empeeno;
        public string password;
        public string[] roles;
        public string factEmployeeNo;
    }
}
