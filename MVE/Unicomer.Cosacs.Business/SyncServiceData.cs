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
using Unicomer.Cosacs.Business.Interfaces;
using Unicomer.Cosacs.Model;
using Unicomer.Cosacs.Repository;

namespace Unicomer.Cosacs.Business
{
    public class SyncServiceData : Interfaces.ISyncData
    {
        public SyncServiceData()
        {
            log4net.GlobalContext.Properties["LogFileName"] = ConfigurationManager.AppSettings["LogFolderPath"] != null ? ConfigurationManager.AppSettings["LogFolderPath"] : @"C://MVE_Unicomer//Log//VE_COSACS_";
            log4net.Config.XmlConfigurator.Configure();
        }
        private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //public JResponse getSyncServiceData()
        //{
        //    SyncServiceModel syncModel = new SyncServiceModel();
        //    SyncServiceRepository objCustomer = new SyncServiceRepository();
        //    JResponse objJResponse = new JResponse();
        //    syncModel = objCustomer.GetSyncService();
        //    if (syncModel != null)
        //    {
        //        if (syncModel.StatusCode.Equals(200))
        //        {
        //            objJResponse.Result = JsonConvert.SerializeObject(syncModel.SyncDataList);
        //            objJResponse.Status = true;
        //        }
        //        else
        //        {
        //            objJResponse.Result = string.Empty;
        //            objJResponse.Status = false;
        //        }
        //        objJResponse.StatusCode = syncModel.StatusCode;
        //        objJResponse.Message = syncModel.Message;


        //    }
        //    else
        //    {
        //        objJResponse.Result = string.Empty;
        //        objJResponse.Status = false;
        //        objJResponse.StatusCode = (int)HttpStatusCode.NotFound;
        //        objJResponse.Message = "No record found.";
        //    }


        //    return objJResponse;
        //}

        public dynamic getSyncServiceData()
        {
            SyncServiceRepository objSyncService = new SyncServiceRepository();
            string result = objSyncService.GetSyncService();
            _log.Info("Info" + " - " + "Sync Service ");
            return result;
        }
    }
}
