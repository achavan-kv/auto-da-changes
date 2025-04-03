using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml;
using Unicomer.Cosacs.Business;
//using System.Windows.Forms;

namespace Blue.Cosacs.MVESchedular
{
    class WriteLog
    {
        // private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly object padlock = new object();
        public WriteLog()
        {
            MVEWebClient mVEWebClient = null;
            var resultExchange = string.Empty;
            string aPIResult = string.Empty;
            string serviceCode = string.Empty;
            string code = string.Empty;
            string postUrl = string.Empty;
            bool isInsertRecord = false;
            bool isEODRecords = true;
            string Method = string.Empty;
            string ID = null;
            string Checkout = string.Empty;
            //Log("Log: " + System.DateTime.Now);
            //StreamReader stRead = new StreamReader(@"C:\Users\rs54786\Desktop\JSON.txt");            
            postUrl = string.Format("{0}/{1}", "SyncService", "getSyncServiceData");
            //_log.Info("Info URL " + " - " + postUrl);
            mVEWebClient = new MVEWebClient(postUrl, false, true);
            var result = mVEWebClient.ExecuteGetWebClient("application/xml; charset=utf-8", true);
            if (result != null)
            {
                DataTable dataTable = new DataTable();
                //= GetDataTableFromJsonString(result);

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(result);



                StringReader theReader = new StringReader(doc.InnerText);
                DataSet theDataSet = new DataSet();
                theDataSet.ReadXml(theReader);

                dataTable = theDataSet.Tables[0];
                if (dataTable.Rows.Count > 0)
                {
                    for (int i = 0; i < dataTable.Rows.Count; i++)
                    {
                        serviceCode = Convert.ToString(dataTable.Rows[i]["ServiceCode"]);
                        code = Convert.ToString(dataTable.Rows[i]["Code"]);
                        isInsertRecord = Convert.ToBoolean(dataTable.Rows[i]["IsInsertRecord"]);
                        isEODRecords = Convert.ToBoolean(dataTable.Rows[i]["IsEODRecords"]);
                        Method = Convert.ToString(dataTable.Rows[i]["Method"]);
                        ID = Convert.ToString(dataTable.Rows[i]["ID"]);
                        Checkout = Convert.ToString(dataTable.Rows[i]["CheckOutID"]);
                        if (isEODRecords)
                        {
                            string Time = Convert.ToString(ConfigurationSettings.AppSettings["EODTime"]);
                            DateTime Time1 = Convert.ToDateTime(Time);
                            DateTime TimeInterval = Time1.AddMinutes(10);
                            DateTime CurrentTime = DateTime.Now;
                            if ((CurrentTime.TimeOfDay >= Time1.TimeOfDay) && (CurrentTime.TimeOfDay <= TimeInterval.TimeOfDay))
                            {
                                int timeStamp = 24;
                                int.TryParse(code, out timeStamp);
                                postUrl = string.Format("{0}/{1}?spanInMinutes={2}", "EOD", "ExecuteEOD", timeStamp);
                                //   strResult.AppendLine("ExecuteEOD : " + postUrl);
                                //_log.Info("Info URL " + " - " + postUrl);
                                mVEWebClient = new MVEWebClient(postUrl, false, true);
                                //  strResult.AppendLine("WebClient : " + postUrl);
                                result = mVEWebClient.ExecuteWebClient("GET", "application/json; charset=utf-8", true);
                            }
                        }
                        else
                        {
                            /*
                               * 1. ServiceCode - 
                               *      {vdr-Vendor},
                               *      {pyt-Payment},
                               *      {grn-GRN}
                               *      {del-Delivery Authorization}
                               *      {delc-Delivery Confirmation}
                               *      {delc-Delivery Confirmation}
                               *      {delc-Delivery Confirmation}
                               *      {delc-Delivery Confirmation}
                               *      {delc-Delivery Confirmation}
                               * 2. Code - 
                               *      {vdr-Vendor code},
                               *      {pyt-Account number},
                               *      {grn-grnId}
                               * 3. isInsertRecord - 
                               *      {true : Insert record},   1
                               *      {false : Update records}  0
                               * 4. isEODRecords - 
                               *      {true : EOD record},      1
                               *      {false :RealTime record}  0
                               * 5. Status - 
                               *      {true : Synced},
                               *      {false :Not Synced}
                            */
                            switch (serviceCode)
                            {
                                #region vdr
                                case "vdr":
                                    try
                                    {
                                        postUrl = string.Format("{0}/{1}?isInsertRecord={2}&serviceCode={3}&code={4}&ID={5}", "RealTimeSync", "RealTimeSynchronize", isInsertRecord, serviceCode, code, ID);
                                        //_log.Info("Info URL " + " - " + postUrl);
                                        mVEWebClient = new MVEWebClient(postUrl, false, true);
                                        result = mVEWebClient.ExecuteGetWebClient("application/json; charset=utf-8", true);
                                    }
                                    catch (Exception ex)
                                    {
                                    }
                                    break;
                                #endregion

                                #region grn
                                case "grn":
                                    try
                                    {
                                        postUrl = string.Format("{0}/{1}?GRNNo={2}&ID={3}", "Transaction", "GetGRN", code, ID);
                                        //_log.Info("Info URL " + " - " + postUrl);
                                        mVEWebClient = new MVEWebClient(postUrl, false, true);
                                        result = mVEWebClient.ExecuteGetWebClient("application/json; charset=utf-8", true);
                                    }
                                    catch (Exception ex)
                                    {
                                    }
                                    break;
                                #endregion

                                #region EOD
                                case "EOD":
                                    try
                                    {
                                        postUrl = string.Format("{0}/{1}?ID={2}", "ParentSKU", "GetParentSKUUpdate", ID);
                                        //_log.Info("Info URL " + " - " + postUrl);
                                        mVEWebClient = new MVEWebClient(postUrl, false, true);
                                        result = mVEWebClient.ExecuteGetWebClient("application/json; charset=utf-8", true);
                                    }
                                    catch (Exception ex)
                                    {
                                    }
                                    break;
                                #endregion

                                #region pyt
                                case "pyt":
                                    try
                                    {
                                        postUrl = string.Format("{0}/{1}?AcctNo={2}&ID={3}", "Transaction", "GetPayments", code, ID);
                                        //_log.Info("Info URL " + " - " + postUrl);
                                        mVEWebClient = new MVEWebClient(postUrl, false, true);
                                        result = mVEWebClient.ExecuteGetWebClient("application/json; charset=utf-8", true);
                                    }
                                    catch (Exception ex)
                                    {
                                    }
                                    break;
                                #endregion

                                #region Auth
                                case "Auth":
                                    try
                                    {
                                        postUrl = string.Format("{0}/{1}?AcctNo={2}&DocType=Auth&ID={3}", "Transaction", "DeliveryAuthorization", code, ID);
                                        //_log.Info("Info URL " + " - " + postUrl);
                                        mVEWebClient = new MVEWebClient(postUrl, false, true);
                                        result = mVEWebClient.ExecuteGetWebClient("application/json; charset=utf-8", true);
                                    }
                                    catch (Exception ex)
                                    {
                                    }
                                    break;
                                #endregion

                                #region delc
                                case "delc":
                                    try
                                    {
                                        if (Method == "POST")
                                        {
                                            postUrl = string.Format("{0}/{1}?AcctNo={2}&ID={3}&CheckOut={4}", "Transaction", "GetPayments", code, ID, Checkout);
                                            //_log.Info("Info URL " + " - " + postUrl);
                                            mVEWebClient = new MVEWebClient(postUrl, false, true);
                                            //lock (padlock)
                                            Monitor.Enter(padlock);
                                            {
                                                try
                                                {
                                                    result = mVEWebClient.ExecuteGetWebClient("application/json; charset=utf-8", true);
                                                    //Thread.Sleep(3000);
                                                }
                                                catch (Exception ex)
                                                {
                                                }
                                                finally
                                                {
                                                    Monitor.Exit(padlock);
                                                }
                                            }
                                        }
                                        else if (Method == "PUT")
                                        {
                                            postUrl = string.Format("{0}/{1}?AcctNo={2}&ID={3}&CheckOut={4}", "Transaction", "GetPaymentsPut", code, ID, Checkout);
                                            //_log.Info("Info URL " + " - " + postUrl);
                                            mVEWebClient = new MVEWebClient(postUrl, false, true);
                                            //lock (padlock)
                                            Monitor.Enter(padlock);
                                            {
                                                try
                                                {
                                                    result = mVEWebClient.ExecuteGetWebClient("application/json; charset=utf-8", true);
                                                    //Thread.Sleep(3000);
                                                }
                                                catch (Exception ex)
                                                {
                                                }
                                                finally
                                                {
                                                    Monitor.Exit(padlock);
                                                }
                                            }
                                        }
                                        else if (Method == "ExchConfirm")
                                        {
                                            postUrl = string.Format("{0}/{1}?AcctNo={2}&ID={3}&CheckOut={4}", "Transaction", "DeliveryConfirmation", code, ID, Checkout);
                                            //_log.Info("Info URL " + " - " + postUrl);
                                            mVEWebClient = new MVEWebClient(postUrl, false, true);
                                            resultExchange = mVEWebClient.ExecuteGetWebClient("application/json; charset=utf-8", true);
                                            //Thread.Sleep(3000);
                                            return;
                                        }
                                        else if (Method == "ExchPayConfirm")
                                        {
                                            postUrl = string.Format("{0}/{1}?AcctNo={2}&ID={3}&CheckOut={4}", "Transaction", "GetPayments", code, ID, Checkout);
                                            //_log.Info("Info URL " + " - " + postUrl);
                                            mVEWebClient = new MVEWebClient(postUrl, false, true);
                                            //lock (padlock)
                                            Monitor.Enter(padlock);
                                            {
                                                try
                                                {
                                                    result = mVEWebClient.ExecuteGetWebClient("application/json; charset=utf-8", true);
                                                    //Thread.Sleep(3000);
                                                }
                                                catch (Exception ex)
                                                {
                                                }
                                                finally
                                                {
                                                    Monitor.Exit(padlock);
                                                }
                                            }

                                            if (result.Contains("Success"))
                                            {
                                                postUrl = string.Format("{0}/{1}?AcctNo={2}&ID={3}&CheckOut={4}", "Transaction", "DeliveryConfirmation", code, ID, Checkout);
                                                //_log.Info("Info URL " + " - " + postUrl);
                                                mVEWebClient = new MVEWebClient(postUrl, false, true);
                                                resultExchange = mVEWebClient.ExecuteGetWebClient("application/json; charset=utf-8", true);
                                                //Thread.Sleep(3000);
                                                return;
                                            }
                                        }
                                        if (result.Contains("Success"))
                                        {
                                            postUrl = string.Format("{0}/{1}?AcctNo={2}&ID={3}&CheckOut={4}", "Transaction", "DeliveryConfirmation", code, ID, Checkout);
                                            //_log.Info("Info URL " + " - " + postUrl);
                                            mVEWebClient = new MVEWebClient(postUrl, false, true);
                                            result = mVEWebClient.ExecuteGetWebClient("application/json; charset=utf-8", true);
                                            //Thread.Sleep(3000);
                                        }
                                    }
                                    catch (Exception ex)
                                    { }
                                    break;
                                #endregion

                                #region vrn
                                case "vrn":
                                    try
                                    {
                                        postUrl = string.Format("{0}/{1}?VendorReturnID={2}&ID={3}", "Transaction", "VendorReturns", code, ID);
                                        //_log.Info("Info URL " + " - " + postUrl);
                                        mVEWebClient = new MVEWebClient(postUrl, false, true);
                                        result = mVEWebClient.ExecuteGetWebClient("application/json; charset=utf-8", true);
                                    }
                                    catch (Exception ex)
                                    {
                                    }
                                    break;
                                #endregion

                                #region delcol
                                case "delcol":
                                    try
                                    {
                                        if (Method == "POST")
                                        {
                                            postUrl = string.Format("{0}/{1}?AcctNo={2}&ID={3}", "Transaction", "GetPayments", code, ID);
                                            //_log.Info("Info URL " + " - " + postUrl);
                                            mVEWebClient = new MVEWebClient(postUrl, false, true);
                                            result = mVEWebClient.ExecuteGetWebClient("application/json; charset=utf-8", true);
                                        }
                                        else
                                        {
                                            postUrl = string.Format("{0}/{1}?AcctNo={2}&ID={3}", "Transaction", "GetPaymentsPut", code, ID);
                                            //_log.Info("Info URL " + " - " + postUrl);
                                            mVEWebClient = new MVEWebClient(postUrl, false, true);
                                            result = mVEWebClient.ExecuteGetWebClient("application/json; charset=utf-8", true);
                                        }
                                        postUrl = string.Format("{0}/{1}?AcctNo={2}&ID={3}", "Transaction", "DeliveryConfirmation", code, ID);
                                        //_log.Info("Info URL " + " - " + postUrl);
                                        mVEWebClient = new MVEWebClient(postUrl, false, true);
                                        result = mVEWebClient.ExecuteGetWebClient("application/json; charset=utf-8", true);
                                    }
                                    catch (Exception ex)
                                    {
                                    }
                                    break;
                                #endregion

                                #region sch
                                case "sch":
                                    try
                                    {
                                        //postUrl = string.Format("{0}/{1}?AcctNo={2}", "Transaction", "DeliveryAuthorization", code);
                                        postUrl = string.Format("{0}/{1}?AcctNo={2}&DocType=Sch&ID={3}", "Transaction", "DeliveryAuthorization", code, ID);
                                        //_log.Info("Info URL " + " - " + postUrl);
                                        mVEWebClient = new MVEWebClient(postUrl, false, true);
                                        result = mVEWebClient.ExecuteGetWebClient("application/json; charset=utf-8", true);
                                    }
                                    catch (Exception ex)
                                    {
                                    }
                                    break;
                                #endregion

                                #region pck
                                case "pck":
                                    try
                                    {
                                        postUrl = string.Format("{0}/{1}?AcctNo={2}&DocType=Pck&ID={3}", "Transaction", "DeliveryAuthorization", code, ID);
                                        //_log.Info("Info URL " + " - " + postUrl);
                                        mVEWebClient = new MVEWebClient(postUrl, false, true);
                                        result = mVEWebClient.ExecuteGetWebClient("application/json; charset=utf-8", true);
                                    }
                                    catch (Exception ex)
                                    {
                                    }
                                    break;
                                #endregion

                                #region sku
                                case "sku":
                                    try
                                    {
                                        int timeStamp = 24;
                                        //int.TryParse(code, out timeStamp);
                                        postUrl = string.Format("{0}/{1}?spanInMinutes={2}&ProductId={3}&ID={4}", "EOD", "ExecuteEOD", timeStamp, code, ID);
                                        //postUrl = string.Format("{0}/{1}?spanInMinutes={2}", "EOD", "ExecuteEOD", timeStamp);
                                        //   strResult.AppendLine("ExecuteEOD : " + postUrl);
                                        //_log.Info("Info URL " + " - " + postUrl);
                                        mVEWebClient = new MVEWebClient(postUrl, false, true);
                                        //  strResult.AppendLine("WebClient : " + postUrl);
                                        result = mVEWebClient.ExecuteGetWebClient("application/json; charset=utf-8", true);
                                    }
                                    catch (Exception ex)
                                    {
                                    }
                                    break;
                                #endregion

                                #region StkTrf
                                case "StkTrf":
                                    try
                                    {
                                        postUrl = string.Format("{0}/{1}?StkTrfNo={2}&ID={3}", "Inventory", "StockTransfer", code, ID);
                                        //_log.Info("Info URL " + " - " + postUrl);
                                        mVEWebClient = new MVEWebClient(postUrl, false, true);
                                        result = mVEWebClient.ExecuteGetWebClient("application/json; charset=utf-8", true);
                                    }
                                    catch (Exception ex)
                                    {
                                    }
                                    break;
                                    #endregion 
                            }
                        }
                    }
                }
            }
        }

        private void Log(string content)
        {
            FileStream fs = new FileStream(string.Format("{0}//Log.txt", AppDomain.CurrentDomain.BaseDirectory), FileMode.OpenOrCreate, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs);
            sw.BaseStream.Seek(0, SeekOrigin.End);
            sw.WriteLine(content);
            sw.Flush();
            sw.Close();
            fs.Close();
        }
        public DataTable GetDataTableFromJsonString(string json)
        {
            var jsonLinq = JObject.Parse(json);

            // Find the first array using Linq
            var srcArray = jsonLinq.Descendants().Where(d => d is JArray).First();
            var trgArray = new JArray();
            foreach (JObject row in srcArray.Children<JObject>())
            {
                var cleanRow = new JObject();
                foreach (JProperty column in row.Properties())
                {
                    // Only include JValue types
                    if (column.Value is JValue)
                    {
                        cleanRow.Add(column.Name, column.Value);
                    }
                }
                trgArray.Add(cleanRow);
            }

            return JsonConvert.DeserializeObject<DataTable>(trgArray.ToString());
        }
    }
}
