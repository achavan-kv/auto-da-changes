using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unicomer.Cosacs.Business
{
    public static class Log4NetHelper
    {
        private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static string _logFileName
        {
            get
            {
                return ConfigurationManager.AppSettings["LogFolderPath"] != null ? ConfigurationManager.AppSettings["LogFolderPath"] : @"C://MVE_Unicomer//Log//VE_COSACS_";
            }
        }

        static Log4NetHelper()
        {
            log4net.GlobalContext.Properties["LogFileName"] = _logFileName;  //@"C://MVE_Unicomer//Log//VE_COSACS_"; //log file path
            log4net.Config.XmlConfigurator.Configure();
        }



        #region Loggers
        public static void WriteInfoLog(String Message)
        {
            _log.Info("Info" + " - " + Message);
        }

        public static void WriteWarningLog(String Message)
        {
            _log.Warn("Worn" + " - " + Message);
        }

        public static void WriteErrorLog(String Message)
        {
            _log.Error("ERROR " + " - " + Message);
        }
        #endregion 
    }
}
