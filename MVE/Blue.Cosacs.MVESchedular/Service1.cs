using System;
using System.Configuration;
using System.ServiceProcess;
using System.Timers;
using Unicomer.Cosacs.Business;

namespace Blue.Cosacs.MVESchedular
{
    //public partial class Service1 : ServiceBase
    //{
    //    Timer tmrExecutor = new Timer();
    //    private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    //    public Service1()
    //    {
    //        InitializeComponent();
    //    }

    //    protected override void OnStart(string[] args)
    //    {
    //        string intervalTime = ConfigurationSettings.AppSettings["intervalTime"].ToString();
    //        tmrExecutor.Elapsed += new ElapsedEventHandler(tmrExecutor_Elapsed); // adding Event
    //        tmrExecutor.Interval = Convert.ToDouble(intervalTime); // Set your time here 
    //        tmrExecutor.Enabled = true;
    //        tmrExecutor.Start();

    //    }
    //    private void tmrExecutor_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
    //    {
    //        //Do your work here 
    //        //WriteLog wr = new WriteLog();
    //    }

    //    protected override void OnStop()
    //    {
    //        tmrExecutor.Enabled = false;
    //    }

    //    private void OnInstall()
    //    {
    //        try
    //        {
    //            string postUrl = string.Empty;
    //            var result = "";
    //            MVEWebClient mVEWebClient = null;

    //            _log.Error("Entered in install service Block");
    //            ServiceInstaller.InstallAndStart(serviceName, serviceName, @"C:\Program Files\Blue Bridge Solutions\Cosacs\Modules\MVE\views\Schedular\MVESchedular.exe");
    //            //ServiceInstaller.InstallAndStart("EODSchedular", "EODSchedular", @"D:\MVE\EODSchedular - Copy\Template\bin\Debug\Blue.Cosacs.MVESchedular.exe");
    //            _log.Error("out from install service Block");

    //            try
    //            {
    //                _log.Error("Entered in GetVendor Block");
    //                postUrl = string.Format("{0}/{1}", "Vendor", "GetVendor");
    //                mVEWebClient = new MVEWebClient(postUrl, false, true);
    //                result = mVEWebClient.ExecuteGetWebClient("application/json; charset=utf-8", true);
    //                _log.Error("Out from GetVendor Block");

    //                _log.Error("Entered in GetParentSKU Block");
    //                postUrl = string.Format("{0}/{1}", "ParentSKU", "GetParentSKU");
    //                mVEWebClient = new MVEWebClient(postUrl, false, true);
    //                result = mVEWebClient.ExecuteGetWebClient("application/json; charset=utf-8", true);
    //                _log.Error("Out from GetParentSKU Block");
    //            }
    //            catch (Exception ex)
    //            {
    //            }
    //        }
    //        catch (Exception ex)
    //        { }
    //    }
    //}

    public partial class Service1 : ServiceBase
    {
        Timer tmrExecutor = new Timer();
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            string intervalTime = ConfigurationSettings.AppSettings["intervalTime"].ToString();
            tmrExecutor.Elapsed += new ElapsedEventHandler(tmrExecutor_Elapsed); // adding Event
            tmrExecutor.Interval = Convert.ToDouble(intervalTime); // Set your time here 
            tmrExecutor.Enabled = true;
            tmrExecutor.Start();

        }
        private void tmrExecutor_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            //Do your work here 
            WriteLog wr = new WriteLog();
        }

        protected override void OnStop()
        {
            tmrExecutor.Enabled = false;
        }
    }
}
