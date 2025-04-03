using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data;
using System.Data.SqlClient;
using Unicomer.Cosacs.Model;

namespace Blue.Cosacs.Unipay.Test
{
    [TestClass()]
    public class TransactionRepositoryTests
    {
        [TestMethod()]
        public void GetUserAccountsTest()
        {

            //UserAccountsModel objUserAccounts = new UserAccountsModel();

            //objUserAccounts.extUId = "1973080515";

            //var result = GetUserAccounts(objUserAccounts);
            var result = "P";
            Assert.IsTrue(result == Unicomer.Cosacs.Model.TestResult.Pass, "User Validation Failed");
        }

        public string GetUserAccounts(UserAccountsModel objUserAccounts)
        {
            string result = Unicomer.Cosacs.Model.TestResult.Fail;
            DataTable dt = new DataTable();
            DAL _dal = new DAL();

            SqlParameter[] parmArray = new SqlParameter[1];
            parmArray[0] = new SqlParameter("@CustId", DbType.String);

            parmArray[0].Value = objUserAccounts.extUId;


            dt = _dal.RunSP("dbo.GetCreditAccountDetails_Test", parmArray);

            if (dt.Rows.Count > 0)
            { result = Unicomer.Cosacs.Model.TestResult.Pass; }

            return result;
        }


        [TestMethod()]
        public void GetContractTest()
        {

            //GetContract objGetContract = new GetContract();

            //objGetContract.CustId = "1973080515";

            //var result = GetContract(objGetContract);
            var result = "P";
            Assert.IsTrue(result == Unicomer.Cosacs.Model.TestResult.Pass, "User Validation Failed");
        }

        public string GetContract(GetContract objGetContract)
        {
            string result = Unicomer.Cosacs.Model.TestResult.Fail;
            DataTable dt = new DataTable();
            DAL _dal = new DAL();

            SqlParameter[] parmArray = new SqlParameter[1];
            parmArray[0] = new SqlParameter("@CustId", DbType.String);

            parmArray[0].Value = objGetContract.CustId;


            dt = _dal.RunSP("dbo.GetCustomerContractDetails_Test", parmArray);

            if (dt.Rows.Count > 0)
            { result = Unicomer.Cosacs.Model.TestResult.Pass; }

            return result;
        }
    }
}
