using System;
using System.Collections;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using STL.Common;
using STL.BLL;
using System.ComponentModel;
using STL.DAL;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;

namespace STL.WS
{
    /// <summary>
    /// Summary description for WebService
    /// </summary>
    [WebService(Namespace = "http://strategicthought.com/webservices/")]

    public class WPrinting : CommonService
    {

        public WPrinting()
        {

            //Uncomment the following line if using designed components 
            //InitializeComponent(); 
        }

        [WebMethod]
        [SoapHeader("authentication")]
        public void GetFileInfo(ref DateTime modified,ref double size,ref string path, string filename,string countrycode)
        {
            BPrinting BP = new BPrinting();
            BP.Verifyfile(ref modified, ref size,ref path, filename, HttpRuntime.AppDomainAppPath, countrycode);
        }

        [WebMethod]
        [SoapHeader("authentication")]
        public byte[] DownloadFile(string path)
        {
            DTransfer T = new DTransfer();
            return T.ReadBinaryFile(path);
        }

        //[WebMethod]
        //[SoapHeader("authentication")]
        //public string FindPath(string file)
        //{
        //    BPrinting BP = new BPrinting();
        //    return BP.FindPath(HttpRuntime.AppDomainAppPath,file);
        //}

        [WebMethod]
        [SoapHeader("authentication")]
        public PrintingAgreementResult GetAgreeementPrint(STL.Common.PrintingAgreementRequest input)
        {
            BPrinting BP = new BPrinting();

            var countryName = CachedItems.GetCountryParamters(input.countrycode).GetCountryParameterValue<object>(CountryParameterNames.CountryName).ToString();

            return BP.GetAgreeementPrint(input, countryName);
        }

        [WebMethod]
        [SoapHeader("authentication")]
        public List<STL.Common.PrintingDN> GetDNPrintInfo(STL.Common.DNparameters[] input)
        {
            BPrinting BP = new BPrinting();
            return BP.GetDNPrintInfo(input);
        }

        [WebMethod]
        [SoapHeader("authentication")]
        public List<STL.Common.PrintingDN> GetDNPrintScheduleItems(short loadNo, short branchNo, DateTime dateDel,int user)
        {
            BPrinting BP = new BPrinting();
            return BP.GetDNPrintScheduleItems(loadNo, branchNo, dateDel,user);
        }

        [WebMethod]
        [SoapHeader("authentication")]
        public string GetDecimalPlaces()
        {
            BPrinting BP = new BPrinting();
            return BP.GetDecimalPlaces();
        }

        [WebMethod]
        [SoapHeader("authentication")]
        public string HostName()
        {
            return System.Environment.MachineName;
        }


        [WebMethod]
        [SoapHeader("authentication")]
        public PrintingAction GetASPrintInfo(string acctno, int empeeno)
        {
            BPrinting BP = new BPrinting();
            
            SqlConnection conn = null;
            SqlTransaction trans = null;
            //BServiceRequest serviceRequest = null;
            PrintingAction PA = null;
            string err = "";
            try
            {
                conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted);
                        PA= BP.GetASPrintInfo(acctno, empeeno,conn,trans);
                        trans.Commit();
                        break;
                    }
                    catch (SqlException ex)
                    {
                        CatchDeadlock(ex, conn);
                    }
                } while (retries <= maxRetries);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
            }
            return PA;
        }


    }
}

