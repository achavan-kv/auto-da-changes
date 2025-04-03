using Blue.Cosacs.MVESchedular;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Configuration.Install;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using Unicomer.Cosacs.Business;

namespace Blue.Cosacs.MVESchedular
{
    public static class Program
    {
        private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public static void Main()
        {
            try
            {
                string serviceName = Convert.ToString(ConfigurationSettings.AppSettings["ServiceName"]);
                string servicePath = Convert.ToString(ConfigurationSettings.AppSettings["ServicePath"]);
                string ErrorserviceName = Convert.ToString(ConfigurationSettings.AppSettings["ErrorServiceName"]);
                string ErrorservicePath = Convert.ToString(ConfigurationSettings.AppSettings["ErrorServicePath"]);
                //servicePath = System.IO.Directory.GetCurrentDirectory() + servicePath.Trim();

                servicePath = servicePath.Trim();
                serviceName = serviceName.Trim();
                if (string.IsNullOrWhiteSpace(serviceName) || string.IsNullOrWhiteSpace(servicePath))
                {
                    string errorMessage = string.IsNullOrWhiteSpace(serviceName) ? ErrorserviceName : string.Empty;
                    errorMessage += string.IsNullOrWhiteSpace(serviceName) ? ErrorservicePath : string.Empty;
                    _log.Info("Info" + " - " + errorMessage);
                    throw (new Exception(errorMessage));
                }

                if (MVESchedular.ServiceInstaller.ServiceIsInstalled(serviceName))
                {
                    ServiceState newServiceState = MVESchedular.ServiceInstaller.GetServiceStatus(serviceName);
                    if (!newServiceState.Equals(ServiceState.Stopped) && !newServiceState.Equals(ServiceState.StopPending) && !newServiceState.Equals(ServiceState.Unknown))
                    {
                        MVESchedular.ServiceInstaller.Uninstall(serviceName);
                    }
                }
                ServiceInstaller.InstallAndStart(serviceName, serviceName, servicePath);
                _log.Info("Info" + " - Service Installed successfully");
            }
            catch (Exception ex)
            {
                _log.Info("Info" + " - " + ex.Message);
            }

            if (!Environment.UserInteractive)
            {
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[]
                {
                    new Service1()
                };
                ServiceBase.Run(ServicesToRun);
            }
            else
            {
                //WriteLog c = new WriteLog();
            }

        }
    }
}
