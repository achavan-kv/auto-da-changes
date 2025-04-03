using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using Unicomer.Cosacs.Business.Interfaces;
using Unicomer.Cosacs.Model;
using Unicomer.Cosacs.Repository;

namespace Unicomer.Cosacs.Business
{
    public class ParentSKUBusiness : IParentSKU
    {
        public ParentSKUBusiness()
        {
            log4net.GlobalContext.Properties["LogFileName"] = ConfigurationManager.AppSettings["LogFolderPath"] != null ? ConfigurationManager.AppSettings["LogFolderPath"] : @"C://MVE_Unicomer//Log//VE_COSACS_";
            log4net.Config.XmlConfigurator.Configure();
        }
        private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public dynamic GetParentSKUMaster()
        {
            ParentSKUResult parentSKU = new ParentSKUResult();
            CustomerRepository objCustomer = new CustomerRepository();
            JResponse objJResponse = new JResponse();
            parentSKU = objCustomer.GetParentSKUMaster();
            _log.Info("Info" + " - " + "Parent SKU Master" + " - " + parentSKU);
            if (parentSKU != null)
            {
                if (parentSKU.StatusCode.Equals(200))
                {
                    // _log.Info("Info" + " - " + "Parent SKU Response" + " - " + parentSKU.StatusCode);
                    objJResponse.Result = JsonConvert.SerializeObject(parentSKU.ParentSKU);
                    objJResponse.Status = true;
                    _log.Info("Info" + " - " + "Parent SKU Response Result" + " - " + objJResponse.Result);
                }
                else
                {
                    _log.Info("Info" + " - " + "Parent SKU Response else" + " - " + objJResponse.Result);
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
                _log.Info("Info" + " - " + "Parent SKU No record found" + " - " + objJResponse.StatusCode);
            }
            return objJResponse;
        }
        public dynamic GetParentSKUMasterEOD()
        {
            ParentSKUResult parentSKU = new ParentSKUResult();
            CustomerRepository objCustomer = new CustomerRepository();
            JResponse objJResponse = new JResponse();
            parentSKU = objCustomer.GetParentSKUMasterEOD();
            _log.Info("Info" + " - " + "POST-Parent SKU Master" + " - " + parentSKU);
            if (parentSKU != null)
            {
                if (parentSKU.StatusCode.Equals(200))
                {
                    // _log.Info("Info" + " - " + "Parent SKU Response" + " - " + parentSKU.StatusCode);
                    objJResponse.Result = JsonConvert.SerializeObject(parentSKU.ParentSKU);
                    objJResponse.Status = true;
                    _log.Info("Info" + " - " + "POST-Parent SKU Response Result" + " - " + objJResponse.Result);
                }
                else
                {
                    _log.Info("Info" + " - " + "POST-Parent SKU Response else" + " - " + objJResponse.Result);
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
                _log.Info("Info" + " - " + "POST-Parent SKU No record found" + " - " + objJResponse.StatusCode);
            }
            return objJResponse;
        }


        public dynamic UpdateParentSKUMaster(ParentSKUUpdate objParentSKUUpdate)
        {
            _log.Info("Info : Update ParentSKUMaster Json request :" + JsonConvert.SerializeObject(objParentSKUUpdate));
            string obj = XmlObjectSerializer.Serialize<ParentSKUUpdate>(objParentSKUUpdate);
            JResponse objJResponse = new JResponse();
            ParentSKURepository objParentSKU = new ParentSKURepository();
            List<string> Result = objParentSKU.UpdateParentSKUMaster(obj);
            if (Result != null && Result.Count > 1)
            {
                if (Result[1].Equals("200"))
                {
                    objJResponse.Result = string.Empty;
                    objJResponse.Status = true;
                    objJResponse.StatusCode = (int)HttpStatusCode.OK;
                    objJResponse.Message = "Parent SKU updated successfully";//Need to create resource file.
                    _log.Info("Info" + " - " + "Parent SKU Updated" + " - " + objJResponse.StatusCode + objJResponse.Message);
                }
                else
                {
                    objJResponse.Result = string.Empty;
                    objJResponse.Status = false;
                    objJResponse.StatusCode = Convert.ToInt32(Result[1]);
                    objJResponse.Message = Result[0];//Need to create resource file.
                    _log.Info("Info" + " - " + "Update Parent SKU " + " - " + objJResponse.StatusCode + objJResponse.Message);
                }
            }
            else
            {
                objJResponse.Result = string.Empty;
                objJResponse.Status = false;
                objJResponse.StatusCode = (int)HttpStatusCode.NotFound;
                objJResponse.Message = "No record found.";
                _log.Info("Info" + " - " + "Update Parent SKU " + " - " + objJResponse.StatusCode + objJResponse.Message);
            }
            return objJResponse;
        }
        public dynamic getParentSKUEOD(int spanInMinutes, string ProductID)
        {
            //List<ParentSKU> parentSKU = new List<ParentSKU>();
            ParentSKURepository objParentSKU = new ParentSKURepository();
            _log.Info("Info" + " - " + "get Parent SKU EOD" + " - " + objParentSKU.GetParentSKUEOD(spanInMinutes, ProductID));
            return objParentSKU.GetParentSKUEOD(spanInMinutes, ProductID);
        }
        public dynamic GetParentSKUUpdate()
        {
            UpdateParentSKUResult UpdateparentSKU = new UpdateParentSKUResult();
            ParentSKURepository objSkuRepository = new ParentSKURepository();
            JResponse objJResponse = new JResponse();
            UpdateparentSKU = objSkuRepository.GetParentSKUUpdate();
            _log.Info("Info" + " - " + "Put-Parent SKU Master" + " - " + UpdateparentSKU);
            if (UpdateparentSKU.UpdateParentSKU != null)
            {
                if (UpdateparentSKU.StatusCode.Equals(200))
                {
                    // _log.Info("Info" + " - " + "Parent SKU Response" + " - " + parentSKU.StatusCode);
                    objJResponse.Result = JsonConvert.SerializeObject(UpdateparentSKU.UpdateParentSKU);
                    objJResponse.Status = true;
                    _log.Info("Info" + " - " + "Put-Parent SKU Response Result" + " - " + objJResponse.Result);
                }
                else
                {
                    _log.Info("Info" + " - " + "Put-Parent SKU Response else" + " - " + objJResponse.Result);
                    objJResponse.Result = string.Empty;
                    objJResponse.Status = false;
                }
                objJResponse.StatusCode = UpdateparentSKU.StatusCode;
                objJResponse.Message = UpdateparentSKU.Message;
            }
            else
            {
                objJResponse.Result = string.Empty;
                objJResponse.Status = false;
                objJResponse.StatusCode = (int)HttpStatusCode.NotFound;
                objJResponse.Message = "No record found.";
                _log.Info("Info" + " - " + "Put-Parent SKU No record found" + " - " + objJResponse.StatusCode);
            }
            return objJResponse;
        }
    }
}
