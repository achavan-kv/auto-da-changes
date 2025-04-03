using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using Unicomer.Cosacs.Business.Interfaces;
using Unicomer.Cosacs.Model;
using Unicomer.Cosacs.Repository;

namespace Unicomer.Cosacs.Business
{
    public class CustomerBuiness : ICustomer
    {

        public JResponse ValidateUser(ValidatetUser objValidateUser)
        {
            //Create repository object for access the database.
            ValidateCustomerResult Result = new ValidateCustomerResult();
            CustomerRepository objCustomer = new CustomerRepository();
            JResponse objJResponse = new JResponse();
            Result = objCustomer.ValidateAndGetUesrDetails(objValidateUser);

            //Create response format by using the JResponse return parameter.
            if (Result != null)
            {
                if (Result.UserResult != null && Result.StatusCode.Equals(200))
                {
                    objJResponse.Result = JsonConvert.SerializeObject(Result.UserResult);
                    objJResponse.Status = true;
                    objJResponse.StatusCode = (int)HttpStatusCode.OK;
                    objJResponse.Message = Result.Message;
                }
                else
                {
                    objJResponse.Result = string.Empty;
                    objJResponse.Status = false;
                    objJResponse.StatusCode = Result.StatusCode;
                    objJResponse.Message = Result.Message;
                }
            }
            else
            {
                objJResponse.Result = "";
                objJResponse.Status = false;
                objJResponse.StatusCode = (int)HttpStatusCode.NotFound;
                objJResponse.Message = "No user found";
            }
            return objJResponse;

        }

        public JResponse CreateUser(User objUser)
        {
            JResponse objJResponse = new JResponse();
            CustomerRepository objCustomer = new CustomerRepository();
            objUser.NewRecord = true;
            List<string> Result = objCustomer.CreateUser(objUser);

            //Create response format by using the JResponse return parameter.
            if (Result.Count > 0)
            {

                if (!string.IsNullOrWhiteSpace(Convert.ToString(Result[0])))
                {
                    objJResponse.Result = JsonConvert.SerializeObject(new { extUId = Result[0] });
                    objJResponse.Status = true;
                    objJResponse.StatusCode = (int)HttpStatusCode.Created;
                    objJResponse.Message = "User created successfully";//Need to create resource file.
                }
                else
                {
                    string message = (Result != null && Result.Count > 1) ? Convert.ToString(Result[1]) : "User already exists";
                    objJResponse.Result = string.Empty;
                    objJResponse.Status = false;
                    objJResponse.StatusCode = (int)HttpStatusCode.NotAcceptable;
                    objJResponse.Message = message;
                }
            }
            else
            {
                objJResponse.Result = JsonConvert.SerializeObject(Result);
                objJResponse.Status = false;
                objJResponse.StatusCode = (int)HttpStatusCode.NotFound;
                objJResponse.Message = "User not created";
            }
            return objJResponse;
        }

        public JResponse getAuthQAndA(string CustId)
        {
            JResponse objJResponse = new JResponse();
            CustomerRepository objCustomer = new CustomerRepository();
            GetAuthQAndA objGetAuthQAndA = new GetAuthQAndA();
            objGetAuthQAndA = objCustomer.GetAuthQAndA(CustId);

            //Create response format by using the JResponse return parameter.
            if (objGetAuthQAndA.questionsAndAnswers.Count > 0)
            {
                objJResponse.Result = JsonConvert.SerializeObject(objGetAuthQAndA);
                objJResponse.Status = true;
                objJResponse.StatusCode = (int)HttpStatusCode.OK;
                objJResponse.Message = "Security Questions";
            }
            else
            {
                objJResponse.Result = string.Empty;
                objJResponse.Status = false;
                objJResponse.StatusCode = (int)HttpStatusCode.NotFound;
                objJResponse.Message = "No questions found for user";
            }
            return objJResponse;
        }

        public JResponse UpdateUser(UpdateUser objUpdateUser, string CustId)
        {
            JResponse objJResponse = new JResponse();
            CustomerRepository objCustomer = new CustomerRepository();
            List<string> Result = objCustomer.UpdateUser(objUpdateUser, CustId);

            //Create response format by using the JResponse return parameter.
            if (Result.Count > 0)
                if (!string.IsNullOrWhiteSpace(Convert.ToString(Result[0])))
                {
                    objJResponse.Result = string.Empty;
                    objJResponse.Status = true;
                    objJResponse.StatusCode = (int)HttpStatusCode.Created;
                    objJResponse.Message = "Customer details updated successfully";
                }
                else
                {
                    //string message = (Result != null && Result.Count > 1) ? Convert.ToString(Result[1]) : "User does not exists";
                    objJResponse.Result = string.Empty;
                    objJResponse.Status = false;
                    objJResponse.StatusCode = (int)HttpStatusCode.NotFound;
                    objJResponse.Message = Convert.ToString(Result[1]);
                }
            return objJResponse;
        }
    }
}
