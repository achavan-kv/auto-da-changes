using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Unicomer.Cosacs.Business.Interfaces;
using Unicomer.Cosacs.Model;
using Unicomer.Cosacs.Repository;

namespace Unicomer.Cosacs.Business
{
    public class CustomerBusiness : ICustomer
    {
        public CustomerBusiness()
        {
            log4net.GlobalContext.Properties["LogFileName"] = ConfigurationManager.AppSettings["LogFolderPath"] != null ? ConfigurationManager.AppSettings["LogFolderPath"] : @"C://MVE_Unicomer//Log//VE_COSACS_";
            log4net.Config.XmlConfigurator.Configure();
        }
        private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public JResponse CreateCustomer(UserJson objCreateCustomer)
        {
            _log.Info("Info : Create Customer Json request :" + JsonConvert.SerializeObject(objCreateCustomer));
            //string obj = XmlObjectSerializer.Serialize<List<Address>>(objJSON.Address);
            string obj = XmlObjectSerializer.Serialize<UserJson>(objCreateCustomer);
            _log.Info("Info : Create Customer Xml Request :" + PrintXmlsingleLine(obj));
            JResponse objJResponse = new JResponse();
            CustomerRepository objCustomer = new CustomerRepository();
            List<string> Result = objCustomer.CreateUsers(obj);
            if (Result != null && Result.Count > 1)
            {
                if (Result[1].Equals("201"))
                {
                    _log.Info("Info" + " - Create Customer Result " + Result);
                    objJResponse.Result = string.Empty;
                    objJResponse.Status = true;
                    objJResponse.StatusCode = (int)HttpStatusCode.Created;
                    objJResponse.Message = "Customer created successfully";//Need to create resource file.
                }
                else
                {
                    _log.Info("Info" + " - Create Customer else result " + Result);
                    objJResponse.Result = string.Empty;
                    objJResponse.Status = false;
                    objJResponse.StatusCode = Convert.ToInt32(Result[1]);
                    objJResponse.Message = Result[0];//Need to create resource file.
                }
            }
            else
            {
                objJResponse.Result = string.Empty;
                objJResponse.Status = false;
                objJResponse.StatusCode = (int)HttpStatusCode.NotFound;
                objJResponse.Message = "No record found.";
            }
            return objJResponse;
        }

        public JResponse GetParentSKUMaster()
        {
            ParentSKUResult parentSKU = new ParentSKUResult();
            CustomerRepository objCustomer = new CustomerRepository();
            JResponse objJResponse = new JResponse();
            parentSKU = objCustomer.GetParentSKUMaster();
            if (parentSKU != null)
            {
                if (parentSKU.StatusCode.Equals(200))
                {
                    objJResponse.Result = JsonConvert.SerializeObject(parentSKU.ParentSKU);
                    objJResponse.Status = true;
                }
                else
                {
                    objJResponse.Result = string.Empty;
                    objJResponse.Status = false;
                }
                objJResponse.StatusCode = parentSKU.StatusCode;
                objJResponse.Message = parentSKU.Message;


            }
            else
            {
                objJResponse.Result = string.Empty;
                objJResponse.Status = false;
                objJResponse.StatusCode = (int)HttpStatusCode.NotFound;
                objJResponse.Message = "No record found.";
            }


            return objJResponse;
        }
        public JResponse GetParentSKUMasterEOD()
        {
            ParentSKUResult parentSKU = new ParentSKUResult();
            CustomerRepository objCustomer = new CustomerRepository();
            JResponse objJResponse = new JResponse();
            parentSKU = objCustomer.GetParentSKUMasterEOD();
            if (parentSKU != null)
            {
                if (parentSKU.StatusCode.Equals(200))
                {
                    objJResponse.Result = JsonConvert.SerializeObject(parentSKU.ParentSKU);
                    objJResponse.Status = true;
                }
                else
                {
                    objJResponse.Result = string.Empty;
                    objJResponse.Status = false;
                }
                objJResponse.StatusCode = parentSKU.StatusCode;
                objJResponse.Message = parentSKU.Message;


            }
            else
            {
                objJResponse.Result = string.Empty;
                objJResponse.Status = false;
                objJResponse.StatusCode = (int)HttpStatusCode.NotFound;
                objJResponse.Message = "No record found.";
            }


            return objJResponse;
        }

        

        public JResponse GetSupplierMaster()
        {
            SupplierMasterResult supplier = new SupplierMasterResult();
            CustomerRepository objCustomer = new CustomerRepository();
            JResponse objJResponse = new JResponse();
            supplier = objCustomer.GetSupplierMaster();
            if (supplier != null)
            {
                if (supplier.StatusCode.Equals(200))
                {
                    objJResponse.Result = JsonConvert.SerializeObject(supplier.SupplierMaster);
                    objJResponse.Status = true;
                }
                else
                {
                    objJResponse.Result = string.Empty;
                    objJResponse.Status = false;
                }

                objJResponse.StatusCode = supplier.StatusCode;
                objJResponse.Message = supplier.Message;
            }
            else
            {
                objJResponse.Result = string.Empty;
                objJResponse.Status = false;
                objJResponse.StatusCode = (int)HttpStatusCode.NotFound;
                objJResponse.Message = "No record found.";
            }
            return objJResponse;
        }

        public JResponse SearchCustomer(CustomerRequest objSearchCustomer)
        {
            _log.Info("Info : Search Customer :" + JsonConvert.SerializeObject(objSearchCustomer));
            List<Customer> customerList = new List<Customer>();
            CustomerRepository objCustomer = new CustomerRepository();
            JResponse objJResponse = new JResponse();
            customerList = objCustomer.SearchCustomer(objSearchCustomer);
            if (customerList != null && customerList.Count > 0)
            {
                objJResponse.Result = JsonConvert.SerializeObject(customerList);
                objJResponse.Status = true;
                objJResponse.StatusCode = (int)HttpStatusCode.OK;
                objJResponse.Message = "Customer records found.";
            }
            else
            {
                objJResponse.Result = string.Empty;
                objJResponse.Status = false;
                objJResponse.StatusCode = (int)HttpStatusCode.NotFound;
                objJResponse.Message = "No customer records found.";
            }
            return objJResponse;
        }

        public JResponse UpdateCustomer(UserJson objCustomerUpdate)
        {
            _log.Info("Info : Update Customer Json Request :" + JsonConvert.SerializeObject(objCustomerUpdate));
            string obj = XmlObjectSerializer.Serialize<UserJson>(objCustomerUpdate);
            _log.Info("Info : Update Customer Xml Request :" + PrintXmlsingleLine(obj));

            JResponse objJResponse = new JResponse();
            CustomerRepository objCustomer = new CustomerRepository();
            List<string> Result = objCustomer.UpdateUser(obj);
            if (Result != null && Result.Count > 1)
            {
                if (Result[1].Equals("200"))
                {
                    objJResponse.Result = string.Empty;
                    objJResponse.Status = true;
                    objJResponse.StatusCode = (int)HttpStatusCode.OK;
                    objJResponse.Message = "Customer data updated successfully";//Need to create resource file.
                }
                else
                {
                    objJResponse.Result = string.Empty;
                    objJResponse.Status = false;
                    objJResponse.StatusCode = Convert.ToInt32(Result[1]);
                    objJResponse.Message = Result[0];//Need to create resource file.
                }
            }
            else
            {
                objJResponse.Result = string.Empty;
                objJResponse.Status = false;
                objJResponse.StatusCode = (int)HttpStatusCode.NotFound;
                objJResponse.Message = "No customer records found.";
            }
            return objJResponse;
        }

        static string PrintXmlsingleLine(string xml)
        {
            var stringBuilder = new StringBuilder();
            var element = XElement.Parse(xml);
            var settings = new XmlWriterSettings();
            settings.OmitXmlDeclaration = false;
            settings.Indent = false;
            settings.NewLineOnAttributes = false;
            using (var xmlWriter = XmlWriter.Create(stringBuilder, settings))
            {
                element.Save(xmlWriter);
            }
            return stringBuilder.ToString();
        }

    }
}
