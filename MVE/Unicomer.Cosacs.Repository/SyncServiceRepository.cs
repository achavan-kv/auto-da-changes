using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using Unicomer.Cosacs.Model;

namespace Unicomer.Cosacs.Repository
{
    public partial class SyncServiceRepository
    {
        public SyncServiceRepository()
        {
            log4net.GlobalContext.Properties["LogFileName"] = ConfigurationManager.AppSettings["LogFolderPath"] != null ? ConfigurationManager.AppSettings["LogFolderPath"] : @"C://MVE_Unicomer//Log//VE_COSACS_";
            log4net.Config.XmlConfigurator.Configure();
        }

        public dynamic GetSyncService()
        {
            string result = string.Empty;
            var ds = new DataSet();
            var CV = new SyncData();
            SyncDataList SyncService = new SyncDataList();
            CV.Fill(ds);


            if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                //           grnItems = ds.Tables[0].Rows.OfType<DataRow>()
                //               .Select(p => new SyncDataList
                //               {
                //                   ServiceCode = Convert.ToString(p["ServiceCode"]),
                //                   Code = Convert.ToString(p["Code"]),
                //                   IsInsertRecord = Convert.ToBoolean(p["IsInsertRecord"]),
                //                   IsEODRecords = Convert.ToBoolean(p["IsEODRecords"]),
                //	Method= Convert.ToString(p["Method"])
                //}).ToList();

                //var view = ds.Tables[0].DefaultView;
                StringWriter sw = new StringWriter();
                ds.WriteXml(sw);
                result = sw.ToString();
            }
            //SyncServiceModel ss = new SyncServiceModel();
            //ss.SyncDataList = grnItems;

            return result;
        }
    }
}
