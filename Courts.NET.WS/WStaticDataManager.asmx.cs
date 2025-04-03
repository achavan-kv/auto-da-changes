using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml;
using Blue.Cosacs;
using Blue.Cosacs.Repositories;
using Blue.Cosacs.Shared;
using STL.BLL;
using STL.Common;
using STL.Common.Constants.ColumnNames;
using STL.DAL;

namespace STL.WS
{
    [WebService(Namespace = "http://strategicthought.com/webservices/")]
    public class WStaticDataManager : CommonService
    {
        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
        [TraceExtension]
#endif
        public DataSet GetDropDownData(XmlNode dropDowns, out string err)
        {
            Function = "BStaticDataManager::GetDropDownData()";
            DataSet ds = null;
            err = "";

            //IP - 26/09/08 - UAT5.2 - UAT(529) - Passing in the user as 'BDropDown'
            //as previously did not have access to the employee that is logged in.
            int user = 0;
            user = Convert.ToInt32("99999");

            try
            {
                //IP - 26/09/08 - UAT5.2 - UAT(529) - Created a new constructor in
                //'BDropDown' that passes in the user
                BDropDown drop = new BDropDown(user);
                ds = drop.GetDropDownData(dropDowns);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return ds;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
        [TraceExtension]
#endif
        public DataSet GetDynamicMenus(int id, string screen, out string err)
        {
            Function = "BStaticDataManager::GetDynamicMenus()";
            DataSet ds = null;
            err = "";

            try
            {
                BMenu menu = new BMenu();
                ds = menu.GetDynamicMenus(id, screen);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }

            return ds;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
        [TraceExtension]
#endif
        public int? ControlPermissionCheck(string login, string screen, string control)
        {
            Function = "BStaticDataManager::ControlPermissionCheck()";
            return new BMenu().ControlPermissionCheck(login, screen, control);
        }

     
        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
        [TraceExtension]
#endif
        public int? ControlPermissionPasswordCheck(string login, string password, string screen, string control)
        {
            Function = "BStaticDataManager::ControlPermissionCheck()";
            var user = new Blue.Admin.UserPasswordValidation(
                new Blue.Admin.UserRepository(EventStore.Instance),
                new Blue.DateTimeClock()).Validate(login, password);

            if (user.IsValid)
                return new BMenu().ControlPermissionCheck(login, screen, control);
            else
                return null;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
        [TraceExtension]
#endif
        public string[] ControlPermissionPasswordCheckMultiple(string login, string password, string screen, string[] controls)
        {
            Function = "BStaticDataManager::ControlPermissionCheck()";
            var user = new Blue.Admin.UserPasswordValidation(
                new Blue.Admin.UserRepository(EventStore.Instance),
                new Blue.DateTimeClock()).Validate(login, password);

            List<string> rights = new List<string>();
            var bmenu = new BMenu();

            if (user.IsValid)
                foreach (var control in controls)
                {
                    if (bmenu.ControlPermissionCheck(login, screen, control).HasValue)
                    {
                        rights.Add(control);
                    }
                }
            return rights.ToArray();
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
        [TraceExtension]
#endif
        public DataSet GetMandatoryFields(string country, string screen, out string err)
        {
            Function = "WStaticDataManager::GetMandatoryFields()";
            DataSet ds = null;
            err = "";

            try
            {
                BMandatoryFields mf = new BMandatoryFields();
                ds = mf.GetMandatoryFields(country, screen);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return ds;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
        [TraceExtension]
#endif
        public int SaveMandatoryFields(DataSet fields, out string err)
        {
            Function = "WStaticDataManager::SaveMandatoryFields()";
            SqlConnection conn = null;

            err = "";

            try
            {
                conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {
                            BMandatoryFields mf = new BMandatoryFields();
                            mf.Save(conn, trans, fields);
                            trans.Commit();
                        }
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
            return 0;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
        [TraceExtension]
#endif
        public int GetVersion(out string server, out string db, out string dbVersion, out string err)
        {
            Function = "BStaticDataManager::GetVersion()";
            err = "";
            server = "";
            db = "";
            dbVersion = "";

            try
            {
                int start = 0;
                int end = 0;

                Assembly a = Assembly.GetExecutingAssembly();
                start = a.FullName.IndexOf("=") + 1;
                end = a.FullName.IndexOf(",", start);
                server = a.FullName.Substring(start, (end - start)) + "  (.Net Framework " + Environment.Version.ToString() + ")";
                string conn = Connections.Default;
                start = conn.IndexOf(";") + 10;
                end = conn.IndexOf(";", start);
                db = conn.Substring(start, (end - start));

                BCountry c = new BCountry();
                dbVersion = c.GetDataBaseVersion();
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return 0;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
        [TraceExtension]
#endif
        public DataSet PostCodeLookUp(string postCode, out string err)
        {
            Function = "BStaticDataManager::PostCodeLookUp()";
            DataSet ds = null;
            err = "";

            try
            {
                BAddress add = new BAddress();
                add.PostCodeLookUp(postCode);
                ds = add.Address;
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }

            return ds;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
        [TraceExtension]
#endif
        public DataSet GetScoringOperands(out string err)
        {
            Function = "BStaticDataManager::GetScoringOperands()";
            DataSet ds = null;
            err = "";

            try
            {
                BScoring operands = new BScoring();
                ds = operands.GetOperands();
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return ds;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
        [TraceExtension]
#endif
        public DataSet GetScreens(XmlNode screens, out string err)
        {
            Function = "BStaticDataManager::GetScreens()";
            DataSet ds = null;
            err = "";

            try
            {
                BMandatoryFields mf = new BMandatoryFields();
                ds = mf.GetScreens(screens);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return ds;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
        [TraceExtension]
#endif
        public int SaveStockItemTranslations(DataSet fields, out string err)
        {
            Function = "WStaticDataManager::SaveStockItemTranslations()";
            SqlConnection conn = null;

            err = "";

            try
            {
                conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {
                            BItem item = new BItem();
                            item.SaveStockItemTranslations(conn, trans, fields);
                            trans.Commit();
                        }
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
            return 0;
        }


        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
        [TraceExtension]
#endif
        public int GetStockItemTranslations(out DataSet fields, out string err, string itemno,
                                            string descr1_en, string descr1, string descr2_en,
                                            string descr2)
        {
            Function = "WStaticDataManager::GetStockItemTranslations()";
            SqlConnection conn = null;

            err = "";
            fields = new DataSet("Translations");

            try
            {
                conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {
                            BItem item = new BItem();
                            fields = item.GetStockItemTranslations(conn, trans, itemno, descr1_en, descr1, descr2_en, descr2);
                            trans.Commit();
                        }
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
            return 0;
        }

        [WebMethod(Description = "Returns a dictionary for the spcified culture, does not require authentication")]
#if(TRACE)
        [TraceExtension]
#endif
        public DataSet GetDictionary(string culture, out string err)
        {
            Function = "WStaticDataManager::GetDictionary()";
            DataSet dictionary = null;
            err = "";

            try
            {
                BDictionary d = new BDictionary();
                dictionary = d.GetDictionary(culture);

                /* add the dictionary to the Cache object */
                if (Cache["Dictionaries"] == null)
                {
                    Cache["Dictionaries"] = new Hashtable();
                }

                if (((Hashtable)Cache["Dictionaries"])[culture] == null)
                {
                    Hashtable ht = new Hashtable();
                    foreach (DataTable dt in dictionary.Tables)
                        foreach (DataRow r in dt.Rows)
                            ht[r[CN.English]] = r[CN.Translation];
                    ((Hashtable)Cache["Dictionaries"])[culture] = ht;
                }
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return dictionary;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
        [TraceExtension]
#endif
        public int SaveDictionary(string culture, DataSet translation, out string err)
        {
            Function = "WStaticDataManager::SaveDictionary()";
            SqlConnection conn = null;

            err = "";

            try
            {
                conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {
                            BDictionary dict = new BDictionary();
                            dict.SaveDictionary(conn, trans, culture, translation);
                            ((Hashtable)Cache["Dictionaries"])[culture] = null;

                            trans.Commit();
                        }
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
            return 0;
        }


        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
        [TraceExtension]
#endif
        public DataSet GetAllCodesAndCategories(out string err)
        {
            Function = "WStaticDataManager::GetAllCodesAndCategories()";
            err = "";
            DataSet ds = new DataSet();

            try
            {
                BCode code = new BCode();
                ds = code.GetAllCodesAndCategories();
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }

            return ds;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
        [TraceExtension]
#endif
        public DataSet GetAllRepairCentre(out string err)
        {
            Function = "WStaticDataManager::GetAllRepairCentre()";
            err = "";
            DataSet ds = new DataSet();

            try
            {
                BBranch branch = new BBranch();
                ds = branch.GetAllRepairCentre();
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }

            return ds;
        }


        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
        [TraceExtension]
#endif
        public int CodeDelete(string Code, string Category, DataSet Deletes, out string err)         // #8926 
        {
            Function = "WStaticDataManager::CodeDelete()";
            SqlConnection conn = null;

            int ret = 0;
            err = "";

            try
            {
                conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {
                            BCode code = new BCode();
                            ret = code.Delete(conn, trans, Code, Category);


                            EventStore.Instance.Log(Deletes, "CodeDelete", EventCategory.SystemMaintenance
                                                 , new { empeeno = User });
                            trans.Commit();
                        }
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

            return ret;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
        [TraceExtension]
#endif
        public int CodeUpdate(DataSet Changes, out string err)
        {
            Function = "WStaticDataManager::CodeUpdate()";
            SqlConnection conn = null;

            int ret = 0;
            err = "";

            try
            {
                conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {
                            BCode code = new BCode();
                            ret = code.Update(conn, trans, Changes);
                            EventStore.Instance.Log(Changes, "CodeUpdate", EventCategory.SystemMaintenance
                                                 , new { empeeno = User });


                            trans.Commit();
                        }
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

            return ret;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
        [TraceExtension]
#endif
        public int SaveCountryMaintenanceChanges(string countryCode, DataSet changes, out string err, int user)
        {
            Function = "WStaticDataManager::SaveCountryMaintenanceChanges()";
            SqlConnection conn = null;

            err = "";

            try
            {
                conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {
                            BCountry c = new BCountry();
                            EventStore.Instance.Log(changes, "CountryMaintenance", EventCategory.SystemMaintenance
                                          , new { empeeno = user });
                            c.SaveCountryMaintenanceParameters(conn, trans, countryCode, changes, user);
                            System.Web.HttpContext.Current.Cache.Remove("Culture");
                            trans.Commit();
                            ///This next step will refresh the cached copy of the parameters
                            DataSet ds = c.GetMaintenanceParameters(conn, trans, countryCode);     // #13465
                        }
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

            return 0;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
        [TraceExtension]
#endif
        public DataSet GetCountryMaintenanceParameters(string countryCode, out string err)
        {
            Function = "WStaticDataManager::GetCountryMaintenanceParameters()";
            err = "";
            DataSet ds = null;

            try
            {
                BCountry country = new BCountry();
                ds = country.GetMaintenanceParameters(null, null, countryCode);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }

            return ds;
        }

        [WebMethod]
        [SoapHeader("authentication")]
        public DataSet GetStcokItemCache(out string err)
        {
            Function = "WStaticDataManager::GetStcokItemCache()";
            err = "";

            var dt = new DataTable();
            dt.Columns.Add("Key", typeof(string));
            dt.Columns.Add("Value", typeof(int));

            var ds = new DataSet();
            ds.Tables.Add(dt);

            try
            {
                var dict = new StockRepository().GetStockItemCache();

                foreach (var entry in dict)
                {
                    var dr = dt.NewRow();
                    dt.Rows.Add(dr);

                    dr["Key"] = entry.Key;
                    dr["Value"] = entry.Value;
                }
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }

            return ds;
        }


        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
        [TraceExtension]
#endif
        public DataSet LoadTermsTypeDetails(string termsType, out string err)
        {
            Function = "WStaticDataManager::LoadTermsTypeDetails()";
            err = "";
            DataSet ds = null;

            try
            {
                BTermsType tt = new BTermsType();
                ds = tt.LoadTermsTypeDetails(termsType);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }

            return ds;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
        [TraceExtension]
#endif
        public DataSet TermsTypeBandsOverview(out string err)
        {
            Function = "WStaticDataManager::TermsTypeBandsOverview()";
            err = "";
            DataSet ds = null;

            try
            {
                BTermsType tt = new BTermsType();
                ds = tt.TermsTypeBandsOverview();
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }

            return ds;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
        [TraceExtension]
#endif
        public int TermsTypeBandsAdjust(DateTime adjustDate, decimal adjustIns, decimal adjustSC, out string err)
        {
            Function = "WStaticDataManager::TermsTypeBandsAdjust()";
            SqlConnection conn = null;

            err = "";
            int user = 0;
            user = STL.Common.Static.Credential.UserId;
            try
            {
                conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {

                            BTermsType tt = new BTermsType();
                            tt.TermsTypeBandsAdjust(conn, trans, adjustDate, adjustIns, adjustSC, user);

                            trans.Commit();
                        }
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
                err = ex.Message;
                logException(ex, Function);
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
            }

            return 0;
        }


        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
        [TraceExtension]
#endif
        public short GetBranchServiceLocation(short branchNo, out string err)
        {
            Function = "WStaticDataManager::GetBranchServiceLocation()";
            err = "";
            short sl = 0;

            try
            {
                BBranch b = new BBranch();
                sl = b.GetServiceLocation(branchNo);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }

            return sl;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
        [TraceExtension]
#endif
        public DateTime GetServerDateTime()
        {
            return DateTime.Now;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
        [TraceExtension]
#endif
        public DateTime GetServerDate()
        {
            return DateTime.Today;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
        [TraceExtension]
#endif
        public int SaveTermsTypeDetails(string termsType, DataSet termsTypeDetails, out string err)
        {
            Function = "WStaticDataManager::SaveTermsTypeDetails()";
            SqlConnection conn = null;

            err = "";
            int user = 0;
            user = STL.Common.Static.Credential.UserId;
            //	user=Convert.ToInt32(STL.Common.Static.Credential.User);
            try
            {
                conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {

                            BTermsType tt = new BTermsType();
                            EventStore.Instance.Log(termsTypeDetails, "TermsTypeUpdate", EventCategory.SystemMaintenance
                                         , new { empeeno = User });

                            tt.SaveTermsTypeDetails(conn, trans, termsType, termsTypeDetails, user);

                            trans.Commit();
                        }
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
                err = ex.Message;
                logException(ex, Function);
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
            }

            return 0;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
        [TraceExtension]
#endif
        public int SetSystemStatus(string countryCode, string status, out string err)
        {
            Function = "WStaticDataManager::SetSystemStatus()";
            SqlConnection conn = null;

            err = "";

            try
            {
                conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {

                            BCountry c = new BCountry();
                            c.SetSystemStatus(conn, trans, countryCode, status);
                            trans.Commit();
                        }
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
                err = ex.Message;
                logException(ex, Function);
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
            }

            return 0;
        }


        [WebMethod(Description = "Method to retrieve bank details")]
        [SoapHeader("authentication")]
#if(TRACE)
        [TraceExtension]
#endif
        public DataSet GetBankDetails(out string err)
        {
            Function = "WStaticDataManager::GetBankDetails()";
            err = "";

            DataSet bankSet = new DataSet();

            try
            {
                BBank bank = new BBank();
                bankSet = bank.GetBankDetails();
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }

            return bankSet;
        }

        //Web service call to update Bank details.
        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
        [TraceExtension]
#endif
        public int UpdateBank(string bankcode, string bankname,
            string bankaddr1, string bankaddr2, string bankaddr3, string bankpocode, out string err)
        {
            Function = "WStaticDataManager::UpdateBank()";
            int status = 0;
            err = "";

            SqlConnection conn = null;


            try
            {
                conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {
                            BBank bank = new BBank();
                            status = bank.UpdateBank(conn, trans, bankcode, bankname, bankaddr1, bankaddr2, bankaddr3, bankpocode);

                            trans.Commit();
                        }
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
            return status;
        }

        //Web service call to delete Bank details
        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
        [TraceExtension]
#endif
        public int DeleteBank(string bankcode, out string err)
        {
            Function = "WStaticDataManager::DeleteBank()";
            int status = 0;
            err = "";

            SqlConnection conn = null;


            try
            {
                conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {
                            BBank bank = new BBank();
                            status = bank.DeleteBank(conn, trans, bankcode);

                            trans.Commit();
                        }
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
            return status;
        }

        [WebMethod]
        [SoapHeader("authentication")]
        public DataSet GetLoyaltyDropData()
        {
            Function = "WStaticDataManager::GetLoyaltyDropData()";
            BLoyalty bloyalty = new BLoyalty();
            return bloyalty.GetLoyaltyDropData();
        }
        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
        [TraceExtension]
#endif
        public DataSet GetTopSellingCashandGo(short branchNo, out string err)
        {
            Function = "BStaticDataManager::GetTopSellingCashandGo()";
            DataSet ds = null;
            err = "";

            try
            {
                BItem items = new BItem();
                ds = items.GetTopSellingCashandGo(branchNo);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return ds;
        }

        [WebMethod]
        [SoapHeader("authentication")]
        public List<DropDownItem> LoadAllTechnician(out string err)
        {
            err = "";

            try
            {
                return new TechnicianRepository().LoadAllTechnician().ToList();
            }
            catch (Exception ex)
            {
                Catch(ex, "WStaticDataManager::LoadTechnicianDrowDown()", ref err);
            }

            return new List<DropDownItem>();
        }

        [WebMethod]
        [SoapHeader("authentication")]
        public List<ZonedTechniciansResult> LoadZoneWithTechnician(out string err)
        {
            err = "";

            try
            {
                return new TechnicianRepository().LoadZoneWithTechnician();
            }
            catch (Exception ex)
            {
                Catch(ex, "WStaticDataManager::LoadTechnicianDrowDown()", ref err);
            }

            return new List<ZonedTechniciansResult>();
        }

        [WebMethod]
        [SoapHeader("authentication")]
        public void NonStockDeletionDatesDataSave(short branchno, string itemno, DateTime deletionDate, out string err)
        {
            err = "";

            try
            {
                NonStockDeletionDatesSave nsd = new NonStockDeletionDatesSave();
                nsd.ExecuteNonQuery(itemno, branchno, deletionDate);

            }
            catch (Exception ex)
            {
                Catch(ex, "WStaticDataManager::NonStockDeletionDatesDataSave()", ref err);
            }


        }

        [WebMethod(Description = "Method to retrieve Product Association data")]
        [SoapHeader("authentication")]
#if(TRACE)
        [TraceExtension]
#endif
        public DataSet ProductAssociationGetDetails()
        {
            Function = "WStaticDataManager::ProductAssociationGetDetails()";

            DataSet ds = null;
            string err = "";

            try
            {
                BItem paDropSet = new BItem();
                ds = paDropSet.ProductAssociationGetDetails();
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }

            return ds;
        }

        [WebMethod(Description = "Method to save Product Association data")]
        [SoapHeader("authentication")]
#if(TRACE)
        [TraceExtension]
#endif
        public void ProductAssociationSaveDetails(DataTable associated)
        {
            Function = "WStaticDataManager::ProductAssociationSaveDetails()";

            string err = "";
            SqlConnection conn = null;

            try
            {
                conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {
                            BItem pa = new BItem();
                            pa.ProductAssociationSaveDetails(associated);
                            trans.Commit();
                        }
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

        }
        public WStaticDataManager()
        {
            //CODEGEN: This call is required by the ASP.NET Web Services Designer
            InitializeComponent();
        }

        #region Component Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
        }
        #endregion

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {

        }
    }
}
