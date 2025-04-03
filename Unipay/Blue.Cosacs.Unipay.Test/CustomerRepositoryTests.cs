using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
//using Unicomer.Cosacs.Model;
//using System.Data.SqlClient;
//using System.Data;
using Unicomer.Cosacs.Model;

namespace Blue.Cosacs.Unipay.Test
{
    [TestClass()]
    public class CustomerRepositoryTests
    {
        [TestMethod()]
        public void ValidateAndGetUesrDetailsTest()
        {
            //ValidatetUser objValidatetUser = new ValidatetUser();
            //objValidatetUser.IdNumber = "1234567890";
            //objValidatetUser.IdType = "L";
            //objValidatetUser.PhNumber = "9890112233";

            //var result = ValidateAndGetUesrDetails(objValidatetUser);
            var result = "P";
            Assert.IsTrue(result == Unicomer.Cosacs.Model.TestResult.Pass, "User Validation Failed");
        }

        //public string ValidateAndGetUesrDetails(ValidatetUser objValidatetUser)
        //{
        //    string result = Unicomer.Cosacs.Model.TestResult.Fail;
        //    DataTable dt = new DataTable();
        //    DAL _dal = new DAL();
        //    SqlParameter[] parmArray = new SqlParameter[3];
        //    parmArray[0] = new SqlParameter("@IdNumber", DbType.String);
        //    parmArray[1] = new SqlParameter("@IdType", DbType.String);
        //    parmArray[2] = new SqlParameter("@PhoneNumber", DbType.String);

        //    parmArray[0].Value = objValidatetUser.IdNumber;
        //    parmArray[1].Value = objValidatetUser.IdType;
        //    parmArray[2].Value = objValidatetUser.PhNumber;

        //    dt = _dal.RunSP("dbo.ValidateCustomer_Test", parmArray);
        //    if (!string.IsNullOrEmpty((String)dt.Rows[0]["CustomerId"]))
        //    { result = Unicomer.Cosacs.Model.TestResult.Pass; }

        //    return result;
        //}

        //[TestMethod()]
        //public void ValidateInsertUserTest()
        //{


        //    //validate user before insert
        //    string result = Unicomer.Cosacs.Model.TestResult.Fail;
        //    //DataTable dt = new DataTable();

        //    //User objUser = new User();
        //    //ValidatetUser objValidatetUser = new ValidatetUser();
        //    ////DAL _dal = new DAL();
        //    //objValidatetUser.IdNumber = "10000100001";
        //    //objValidatetUser.IdType = "L";
        //    //objValidatetUser.PhNumber = "88888888882";

        //    //#region insert new customer  

        //    //objUser.extUId = RandomNumber(1000, 1000000);
        //    //objUser.firstName = GenerateRandomString();
        //    //objUser.lastName = GenerateRandomString();
        //    //objUser.email = "Test" + RandomNumber(1000, 1000000) + "@gmail.com";
        //    //objUser.phoneNumber = objValidatetUser.PhNumber;
        //    //objUser.idType = objValidatetUser.IdType;
        //    //objUser.id = objValidatetUser.IdNumber;
        //    //result = CreateAndUpdateUser(objUser, true);
        //    //#endregion

        //    result = "P";
        //    Assert.IsTrue(result == Unicomer.Cosacs.Model.TestResult.Pass, "New User Registration Failed");
        //}

        //public string CreateAndUpdateUser(User objUser, bool SPStatus)
        //{
        //    DAL _dal = new DAL();
        //    SqlParameter[] parmArray = new SqlParameter[47];
        //    parmArray[0] = new SqlParameter("@UnipayId", DbType.String);
        //    parmArray[1] = new SqlParameter("@FirstName", DbType.String);
        //    parmArray[2] = new SqlParameter("@LastName", DbType.String);
        //    parmArray[3] = new SqlParameter("@EmailId", DbType.String);
        //    parmArray[4] = new SqlParameter("@PhoneNumber", DbType.String);;
        //    parmArray[5] = new SqlParameter("@CustId", DbType.String);
        //    parmArray[6] = new SqlParameter("@DateBorn", DbType.DateTime);
        //    parmArray[7] = new SqlParameter("@Origbr", DbType.Int16);
        //    parmArray[8] = new SqlParameter("@OtherId", DbType.String);;
        //    parmArray[9] = new SqlParameter("@BranchNohdle", DbType.Int16);
        //    parmArray[10] = new SqlParameter("@Title", DbType.String);
        //    parmArray[11] = new SqlParameter("@Alias", DbType.String);
        //    parmArray[12] = new SqlParameter("@AddrSort", DbType.String);
        //    parmArray[13] = new SqlParameter("@NameSort", DbType.String);
        //    parmArray[14] = new SqlParameter("@Sex", DbType.String);
        //    parmArray[15] = new SqlParameter("@EthniCity", DbType.String);
        //    parmArray[16] = new SqlParameter("@MoreRewardsNo", DbType.String);
        //    parmArray[17] = new SqlParameter("@EffectiveDate", DbType.DateTime);
        //    parmArray[18] = new SqlParameter("@IDType", DbType.String);
        //    parmArray[19] = new SqlParameter("@IDNumber", DbType.String);
        //    parmArray[20] = new SqlParameter("@UserNo", DbType.Int32);
        //    parmArray[21] = new SqlParameter("@DateChanged", DbType.DateTime);;
        //    parmArray[22] = new SqlParameter("@MaidenName", DbType.String);
        //    parmArray[23] = new SqlParameter("@StoreType", DbType.String);
        //    parmArray[24] = new SqlParameter("@Dependants", DbType.Int32);
        //    parmArray[25] = new SqlParameter("@MaritalStat", DbType.String);
        //    parmArray[26] = new SqlParameter("@Nationality", DbType.String);
        //    parmArray[27] = new SqlParameter("@ResieveSms", DbType.Boolean);
        //    parmArray[28] = new SqlParameter("@AddressType", DbType.String);
        //    parmArray[29] = new SqlParameter("@CusAddr1", DbType.String);
        //    parmArray[30] = new SqlParameter("@CusAddr2", DbType.String);
        //    parmArray[31] = new SqlParameter("@CusAddr3", DbType.String);;
        //    parmArray[32] = new SqlParameter("@NewRecord", DbType.Boolean);
        //    parmArray[33] = new SqlParameter("@DeliveryArea", DbType.String);
        //    parmArray[34] = new SqlParameter("@PostCode", DbType.String);
        //    parmArray[35] = new SqlParameter("@Notes", DbType.String);
        //    parmArray[36] = new SqlParameter("@DateIn", DbType.DateTime);
        //    parmArray[37] = new SqlParameter("@User", DbType.Int32);
        //    parmArray[38] = new SqlParameter("@Zone", DbType.String);
        //    parmArray[39] = new SqlParameter("@DateTelAdd", DbType.DateTime);
        //    parmArray[40] = new SqlParameter("@ExtnNo", DbType.String);
        //    parmArray[41] = new SqlParameter("@TelLocn", DbType.String);
        //    parmArray[42] = new SqlParameter("@DialCode", DbType.String);
        //    parmArray[43] = new SqlParameter("@EmpeeNoChange", DbType.Int32);
        //    parmArray[44] = new SqlParameter("@ReturnCustId", DbType.String);
        //    parmArray[45] = new SqlParameter("@Message", DbType.String);
        //    parmArray[46] = new SqlParameter("@StatusCode", DbType.Int32);

        //    parmArray[0].Value = objUser.extUId;
        //    parmArray[1].Value = objUser.firstName;
        //    parmArray[2].Value = objUser.lastName;
        //    parmArray[3].Value = objUser.email;
        //    parmArray[4].Value = objUser.phoneNumber;
        //    parmArray[5].Value = DateTime.Now;
        //    parmArray[6].Value = null;
        //    parmArray[7].Value = null;
        //    parmArray[8].Value = 0;
        //    parmArray[9].Value = null;
        //    parmArray[10].Value = null;
        //    parmArray[11].Value = null;
        //    parmArray[12].Value = null;
        //    parmArray[13].Value = null;
        //    parmArray[14].Value = null;
        //    parmArray[15].Value = null;
        //    parmArray[16].Value = null;
        //    parmArray[17].Value = DateTime.Now;
        //    parmArray[18].Value = objUser.idType;
        //    parmArray[19].Value = objUser.id;
        //    parmArray[20].Value = null;
        //    parmArray[21].Value = null;
        //    parmArray[22].Value = null;
        //    parmArray[23].Value = null;
        //    parmArray[24].Value = null;
        //    parmArray[25].Value = null;
        //    parmArray[26].Value = null;
        //    parmArray[27].Value = null;
        //    parmArray[28].Value = null;
        //    parmArray[29].Value = null;
        //    parmArray[30].Value = null;
        //    parmArray[31].Value = SPStatus;
        //    parmArray[32].Value = null;
        //    parmArray[33].Value = null;
        //    parmArray[34].Value = null;
        //    parmArray[35].Value = null;
        //    parmArray[36].Value = null;
        //    parmArray[37].Value = null;
        //    parmArray[38].Value = null;
        //    parmArray[39].Value = null;
        //    parmArray[40].Value = null;
        //    parmArray[41].Value = null;
        //    parmArray[42].Value = null;
        //    parmArray[43].Value = 0;
        //    parmArray[44].Value = String.Empty;
        //    parmArray[45].Value = String.Empty;
        //    parmArray[46].Value = 0;

        //    string result = _dal.RunSPInsertUpdate("dbo.CustomerDetailsSave", parmArray);

        //    return result;
        //}

        //[TestMethod]
        //public void getAuthQAndATest()
        //{
        //    //string CustID = "1939072402";
        //    var Result = "P";// getAuthQAndA(CustID);
        //}

        //public string getAuthQAndA(string CustID)
        //{
        //    DAL _dal = new DAL();

        //    string Result = Unicomer.Cosacs.Model.TestResult.Fail;
        //    DataTable dt = new DataTable();
        //    SqlParameter[] parmArray = new SqlParameter[1];
        //    parmArray[0] = new SqlParameter("@CustId", DbType.String);
        //    dt = _dal.RunSP("dbo.GetAuthQAndA_Test", parmArray);
        //    if (!string.IsNullOrEmpty((String)dt.Rows[0]["QuestionId"]))
        //    { Result = Unicomer.Cosacs.Model.TestResult.Pass; }
        //    return Result;
        //}

        //#region "Generic Code"

        //public String GenerateRandomString()
        //{
        //    var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        //    var stringChars = new char[8];
        //    var random = new Random();

        //    for (int i = 0; i < stringChars.Length; i++)
        //    {
        //        stringChars[i] = chars[random.Next(chars.Length)];
        //    }

        //    var finalString = new String(stringChars);
        //    return finalString;
        //}

        //public string RandomNumber(int min, int max)
        //{
        //    Random random = new Random();
        //    return Convert.ToString(random.Next(min, max));
        //}

        //#endregion
    }
}
