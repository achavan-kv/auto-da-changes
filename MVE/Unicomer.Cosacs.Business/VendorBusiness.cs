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
    public class VendorBusiness : IVendor
    {
        private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public VendorBusiness()
        {
            log4net.GlobalContext.Properties["LogFileName"] = ConfigurationManager.AppSettings["LogFolderPath"] != null ? ConfigurationManager.AppSettings["LogFolderPath"] : @"C://MVE_Unicomer//Log//VE_COSACS_";
            log4net.Config.XmlConfigurator.Configure();
        }
        public JResponse GetSupplierMaster()
        {
            SupplierMasterResult supplier = new SupplierMasterResult();
            CustomerRepository objCustomer = new CustomerRepository();
            JResponse objJResponse = new JResponse();
            supplier = objCustomer.GetSupplierMaster();
            if (supplier != null && supplier.SupplierMaster.Count > 0)
            {
               
                if (supplier.StatusCode.Equals(200))
                {
                    objJResponse.Result = JsonConvert.SerializeObject(supplier.SupplierMaster);
                    _log.Info("Info" + " -  Vendor Master Result" + objJResponse.Result);
                    objJResponse.Status = true;
                }
                else
                {
                    objJResponse.Result = string.Empty;
                    objJResponse.Status = false;
                }
                _log.Info("Info" + " -Vendor Status " + supplier.StatusCode);
                objJResponse.StatusCode = supplier.StatusCode;
                objJResponse.Message = supplier.Message;
            }
            else
            {
                _log.Info("Info" + " - Vendor Record" + (int)HttpStatusCode.NotFound);
                objJResponse.Result = string.Empty;
                objJResponse.Status = false;
                objJResponse.StatusCode = (int)HttpStatusCode.NotFound;
                objJResponse.Message = "No record found.";
            }
            _log.Info("Info" + " - Vendor Record" + objJResponse);
            return objJResponse;
        }

        public dynamic UpdateSupplierMaster(UpdateSupplier objJSON)
        {
            _log.Info("Info : Update Update Supplier Master Json request :" + JsonConvert.SerializeObject(objJSON));
            string obj = XmlObjectSerializer.Serialize<UpdateSupplier>(objJSON);
            JResponse objJResponse = new JResponse();
            VendorRepository objVendor = new VendorRepository();
            List<string> Result = objVendor.UpdateVendorMaster(obj);
            if (Result != null && Result.Count > 1)
            {
                if (Result[1].Equals("200"))
                {
                    _log.Info("Info" + " - Update Vendor Master " + Result);
                    objJResponse.Result = string.Empty;
                    objJResponse.Status = true;
                    objJResponse.StatusCode = (int)HttpStatusCode.OK;
                    objJResponse.Message = "Vendor updated successfully";//Need to create resource file.
                }
                else
                {
                    _log.Info("Info" + " - Update Vendor Master Else " + Result[0]);
                    _log.Info("Info" + " - Update Vendor Code Else " + Result[1]);
                    objJResponse.Result = string.Empty;
                    objJResponse.Status = false;
                    objJResponse.StatusCode = Convert.ToInt32(Result[1]);
                    objJResponse.Message = Result[0];//Need to create resource file.
                }
            }
            else
            {
                _log.Info("Info" + " - Vendor Record" + (int)HttpStatusCode.NotFound);
                objJResponse.Result = string.Empty;
                objJResponse.Status = false;
                objJResponse.StatusCode = (int)HttpStatusCode.NotFound;
                objJResponse.Message = "No record found.";
            }
            return objJResponse;
        }

        public dynamic GetSupplierEOD(int spanInMinutes)
        {
            VendorRepository objVendor = new VendorRepository();
            _log.Info("Info" + " - " + "Get Vendor EOD" + " - " + objVendor.GetSupplierEOD(spanInMinutes));
            return objVendor.GetSupplierEOD(spanInMinutes);
        }

        public dynamic GetSupplierRTS(string vendorCode)
        {
            VendorRepository objVendor = new VendorRepository();
            _log.Info("Info" + " - " + "Get Vendor RTS");
            return objVendor.GetSupplierRTS(vendorCode);
        }
    }
}
