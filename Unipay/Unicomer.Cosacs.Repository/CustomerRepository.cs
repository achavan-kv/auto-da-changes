using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Unicomer.Cosacs.Model;

namespace Unicomer.Cosacs.Repository
{
    public partial class CustomerRepository
    {
        public ValidateCustomerResult ValidateAndGetUesrDetails(ValidatetUser objValidateUser)
        {
            var ds = new DataSet();
            var CV = new CustomerValidate();
            var retList = new ValidateCustomerResult();

            //Access database and return the required response parameter.
            CV.Fill(ds, objValidateUser.IdNumber, objValidateUser.IdType, objValidateUser.PhNumber);

            //Create the JSON response result.
            if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                retList = ds.Tables[0].Rows.OfType<DataRow>()
                    .Select(p => new ValidateCustomerResult()
                    {
                        UserResult = new ValidateUserResult
                        {
                            id = Convert.ToString(p["IdNumber"]).Trim(),
                            idType = Convert.ToString(p["IdType"]).Trim(),
                            extUId = Convert.ToString(p["CustomerId"]).Trim(),
                            lastName = Convert.ToString(p["LastName"]).Trim(),
                            firstName = Convert.ToString(p["FirstName"]).Trim(),
                            email = Convert.ToString(p["EmailId"]).Trim()
                        },
                        Message = (string)p["Message"],
                        StatusCode = (int)p["StatusCode"]
                    })
                    .FirstOrDefault();
            }

            return retList;

        }

        public List<string> CreateUser(User objUser)
        {
            var ICD = new CustomerInsertRepository();
            List<string> extUId = new List<string>();
            string ExtCustId = string.Empty;
            string Message = string.Empty;

            //Set the initial values for save the customer in cosacs Database.
            int StatusCode = 0;
            {
                extUId = ICD.InsertCustomer(objUser.extUId,
                                            objUser.firstName,
                                            objUser.lastName,
                                            objUser.email,
                                            string.IsNullOrWhiteSpace(objUser.phoneNumber) ? string.Empty : objUser.phoneNumber,
                                            objUser.CustId,
                                            null,//DOB,
                                            null,//origbr
                                            null, //otherid
                                            0,//objUser.BranchNoHdle.HasValue ? objUser.BranchNoHdle.Value : Convert.ToInt16(0),//branchnohdle
                                            null,//objUser.Title,
                                            null,//alias,
                                            null,//addrsort,
                                            null, //namesort,
                                            null, //sex,
                                            null, //ethnicity,
                                            null, //morerewardsno,
                                            null, //effectivedate,
                                            objUser.idType,
                                            objUser.id,
                                            null,//UserNo
                                            null,//datechange,
                                            null, //maidenname,
                                            "C",//StoreType,
                                            null, //dependants,
                                            null, //maritalstat,
                                            null, //Nationality,
                                            null, //ResieveSms, 
                                            null,//objUser.AddressType,
                                           null,// objUser.Address,
                                            null,//objUser.custaddresses.Select(a=>a.cusaddr2), 
                                            null,//objUser.custaddresses.Select(a=>a.cusaddr3),
                                            objUser.NewRecord, //new record                                         
                                            null,//objUser.DeliveryArea,//.custaddresses.Select(a=>a.deliveryarea), 
                                            null,//postcode, 
                                           null,// objUser.Notes,// notes, 
                                           null,//Convert.ToDateTime(objUser.DateIn),//dateIn 
                                            null,//User
                                            null,//zone,
                                            null, //dateteladd 
                                            null, //extnno,
                                            null,//tellocn, 
                                            null,//DialCode, 
                                            null,//empeenochang
                                            ExtCustId,
                                            Message,
                                            StatusCode);
            }
            return extUId;
        }

        public GetAuthQAndA GetAuthQAndA(string CustId)
        {
            GetAuthQAndA objGetAuthQAndA = new GetAuthQAndA();
            List<questionsAndAnswers> questionsAndAnswers = new List<questionsAndAnswers>();
            var ds = new DataSet();
            var SAR = new SecurityAnswerRepository();

            //Access database and return the required response parameter.
            SAR.GetAuthQAndA(ds, CustId);

            //Create the JSON response result.
            if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                questionsAndAnswers = ds.Tables[0].Rows.OfType<DataRow>()
                     .Select(p => new questionsAndAnswers()
                     {
                         qId = Convert.ToInt32(p["qId"]),
                         question = Convert.ToString(p["question"]).Trim(),
                         answers = GetAnswerList(Convert.ToString(p["answers"]).Trim()),
                         inputType = Convert.ToString(p["inputType"]),
                         inputCategory = Convert.ToString(p["inputCategory"])

                     }).ToList();
            }
            objGetAuthQAndA.questionsAndAnswers = questionsAndAnswers;
            objGetAuthQAndA.numCorrectRequired = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["Attempts"]);
            return objGetAuthQAndA;
        }

        private List<string> GetAnswerList(string v)
        {
            //Add the Security question's answers.
            List<string> result = new List<string>();
            if (!string.IsNullOrWhiteSpace(v))
            {
                if (v.Contains(";"))
                    result.AddRange(v.Split(';').ToList());
                else
                    result.Add(v);
            }
            return result;
        }

        public List<string> UpdateUser(UpdateUser objUpdateUser, string CustId)
        {
            string ExtCustId = string.Empty;
            string Message = string.Empty;
            string StatusCode = string.Empty;
            List<string> extUId = new List<string>();
            var CUR = new CustomerUpdateRepository();

            extUId = CUR.UpdateUser(null,
                                            objUpdateUser.firstName,
                                            objUpdateUser.lastName,
                                            objUpdateUser.email,
                                            objUpdateUser.Phone,//PhNumber,
                                            CustId,
                                            Convert.ToDateTime(System.DateTime.Now),// Convert.ToDateTime(objUser.DOB),
                                            null,//origbr
                                            null, //otherid
                                            Convert.ToInt16(0),//branchnohdle
                                            null,//Title,
                                            null,//alias,
                                            null,//addrsort,
                                            null, //namesort,
                                            null, //sex,
                                            null, //ethnicity,
                                            null, //morerewardsno,
                                            null, //effectivedate,
                                            null,//IDType,
                                            null,//IDNumber,
                                            null,//UserNo
                                            null,//datechange,
                                            null, //maidenname,
                                            null,//StoreType,
                                            null, //dependants,
                                            null, //maritalstat,
                                            null, //Nationality,
                                            null, //ResieveSms, 
                                            null, //AddressType,
                                            null, //Address,
                                            null,//objUser.custaddresses.Select(a=>a.cusaddr2), 
                                            null,//objUser.custaddresses.Select(a=>a.cusaddr3),
                                            false, //new record                                         
                                            null,//.custaddresses.Select(a=>a.deliveryarea), 
                                            null,//postcode, 
                                            null,// notes, 
                                            Convert.ToDateTime(System.DateTime.Now),//dateIn 
                                            null,//User
                                            null,//zone,
                                            null, //dateteladd 
                                            null, //extnno,
                                            null,//tellocn, 
                                            null,//DialCode, 
                                            null,//empeenochang
                                            ExtCustId,
                                            Message,
                                            StatusCode
                                            );
            return extUId;
        }
    }
}
