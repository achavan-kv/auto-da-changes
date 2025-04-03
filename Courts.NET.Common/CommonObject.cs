using System;
using System.Globalization;
using System.Text;
using System.Threading;
using System.Collections.Specialized;
using System.Data.SqlClient;
using System.Collections;
using STL.Common.Static;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Data;
using System.Configuration;

namespace STL.Common
{
    /// <summary>
    /// This is the base class for all data access and non-web service
    /// buiness components
    /// It provides standardised logging functionality.
    /// </summary>
    public class CommonObject : ITranslatable
    {
        private Logging log;
        private static readonly object sync = new object();

        private readonly StaticDataSingleton _staticData = StaticDataSingleton.Instance();
        public HybridDictionary Cache
        {
            get
            {
                return _staticData.Data;
            }
            set
            {
                _staticData.Data = value;
            }
        }

        //private IDictionary _cache = null;
        //public IDictionary Cache 
        //{
        //	get
        //	{
        //		if(_cache==null)
        //		{
        //			if(System.Web.HttpContext.Current != null)
        //			{
        //				if(System.Web.HttpContext.Current.Cache["Cache"]==null)
        //					System.Web.HttpContext.Current.Cache["Cache"] = new Hashtable();
        //
        //				_cache = (IDictionary)System.Web.HttpContext.Current.Cache["Cache"];
        //			}
        //			else
        //			{
        //				_cache = new Hashtable();
        //			}
        //		}
        //		return _cache;
        //	}
        //	set
        //	{
        //		_cache = value;
        //		if(System.Web.HttpContext.Current != null)
        //		{
        //			System.Web.HttpContext.Current.Cache["Cache"] = _cache;
        //		}
        //	}
        //}

        public CountryParameterCollection Country
        {
            get
            {
                if (Cache["Country"] == null) // Cache == null || 
                {
                    lock (sync)
                    {
                        if (Cache["Country"] == null) // Cache == null || 
                        {
                            var connectionString = ConfigurationManager.ConnectionStrings["Default"].ConnectionString;
                            DataTable dt = new DataTable("CountryParameters");

                            //using (var conn = new SqlConnection(connectionString))
                            //{
                                //using (var command = new SqlCommand("DN_CountryMaintenanceGetParametersSP", conn) { CommandType = CommandType.StoredProcedure })
                                //{
                                //    using (var da = new SqlDataAdapter(command))
                                //    {
                                //        da.Fill(dt);
                                //        Cache["Country"] = new CountryParameterCollection(dt);
                                //    }
                                //}

                                GetCountryParams(dt);
                                //Cache["Country"] = new CountryParameterCollection(dt);
                            //}
                        }
                    }
                }
                return (CountryParameterCollection)Cache["Country"];
            }
        }

        public DataTable GetCountryParams(DataTable dt)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["Default"].ConnectionString;
            using (var conn = new SqlConnection(connectionString))
            {
                using (var command = new SqlCommand("DN_CountryMaintenanceGetParametersSP", conn) { CommandType = CommandType.StoredProcedure })
                {
                    using (var da = new SqlDataAdapter(command))
                    {
                        da.Fill(dt);
                        Cache["Country"] = new CountryParameterCollection(dt);
                    }
                }
            }


            return dt;

        }

        public string DecimalPlaces
        {
            get
            {
                return (string)Country[CountryParameterNames.DecimalPlaces];
            }
        }

        private static bool IsNumeric(string text)
        {
            var reg = new Regex("((^[+-][0-9]+$)|(^[0-9]*$))");
            return reg.IsMatch(text);
        }

        public int DecimalPlacesNo
        {
            get
            {
                // Try to extract country precision from Decimal Places format (or default as 2)
                if (Country.ContainsKey(CountryParameterNames.DecimalPlaces) && IsNumeric(((string)Country[CountryParameterNames.DecimalPlaces]).Substring(1, 1)))
                    return Convert.ToInt32(((string)Country[CountryParameterNames.DecimalPlaces]).Substring(1, 1));
                else
                    return 2;

            }
        }

        public void CountryRound(ref decimal moneyValue)
        {
            // Round money to Country Decimal Places
            moneyValue = Math.Round(moneyValue, this.DecimalPlacesNo);
        }

        public decimal CountryRound(object moneyValue)
        {
            // Round money to Country Decimal Places
            return Math.Round(Convert.ToDecimal(moneyValue), this.DecimalPlacesNo);
        }

        protected int retries = 0;
        protected int maxRetries = 10;
        protected int Deadlock = 1205;

        protected int _user;
        public int User
        {
            get { return _user; }
            set { _user = value; }
        }

        public string Function { get; set; }

        public void logMessage(string message, string user, EventLogEntryType type)
        {
            log = new Logging();
            log.logMessage(message, user, type);
        }

        public void logMessage(string[] message, string user, EventLogEntryType type)
        {
            log = new Logging();
            log.logMessage(message, user, type);
        }

        public void logMessage(StringCollection message, string user, EventLogEntryType type)
        {
            log = new Logging();
            log.logMessage(message, user, type);
        }

        protected string StripCurrency(string field)
        {
            /* this method needs to cope with all possible negative currency patterns */
            NumberFormatInfo num = Thread.CurrentThread.CurrentCulture.NumberFormat;
            string separator = num.CurrencyGroupSeparator;
            string currency = num.CurrencySymbol;
            field = field.Replace(currency, "");
            field = field.Replace(separator, "");
            //int i =0;

            switch (num.CurrencyNegativePattern)
            {
                case 0: field = field.Replace("(", "-").Replace(")", "");	/* ($n) */
                    break;
                case 3: field = field.IndexOf("-") == -1 ? field : "-" + field.Replace("-", "");	/* $n-	*/
                    break;
                case 4: field = field.Replace("(", "-").Replace(")", "");	/* (n$) */
                    break;
                case 6: field = field = field.IndexOf("-") == -1 ? field : "-" + field.Replace("-", "");					/* n-$  */
                    break;
                case 7: field = "-" + field.Replace("-", "");					/* n$-	*/
                    break;
                case 10: field = field = field.IndexOf("-") == -1 ? field : "-" + field.Replace("-", "");					/* n $-	*/
                    break;
                case 11: field = field = field.IndexOf("-") == -1 ? field : "-" + field.Replace("-", "");					/* $ n-	*/
                    break;
                case 13: field = field = field.IndexOf("-") == -1 ? field : "-" + field.Replace("-", "");					/* n- $ */
                    break;
                case 14: field = field.Replace("(", "-").Replace(")", "");	/* ($ n) */
                    break;
                case 15: field = field.Replace("(", "-").Replace(")", "");	/* (n $) */
                    break;
                default:
                    break;
            }
            return field.Trim();
        }

        /*
        protected void LogSqlException(SqlException e)
        {
            SqlErrorCollection errors = e.Errors;
            StringCollection message = new StringCollection();
            message.Add("A SqlException has occurred:");
            message.Add("Error no: "+e.Number.ToString()+": "+e.Message+" on line "+e.LineNumber.ToString()+" in procedure "+ e.Procedure+".");
            message.Add("Error reported by "+e.Source+" while connected to "+e.Server+".");
            logMessage(message, "", EventLogEntryType.Error);
        }
        */

        protected void LogSqlException(SqlException e)
        {
            //SqlErrorCollection errors = e.Errors;
            var message = new StringCollection
			                  {
			                      "A SqlException has occurred:",
			                      "Error no: " + e.Number.ToString() + ": " + e.Message + " on line " + e.LineNumber.ToString() +
			                      " in procedure " + e.Procedure + ".",
			                      "Error reported by " + e.Source + " while connected to " + e.Server + "."
			                  };
            logMessage(message, "", EventLogEntryType.Error);

            if (e.Number == 50000)
                throw new STLException(e.Message);

            throw e;
        }

        public string GetResource(string msgName)
        {
            var culture = Thread.CurrentThread.CurrentCulture.Name;
            var complete = (string)Messages.List[msgName];
            //if(culture.IndexOf("en-")!=-1)
            //	complete = (string)Messages.List[msgName];
            //else
            //{
            var trans = Translate(msgName, culture);
            complete = trans == msgName ? complete : trans;	//in case there is no translation
            //}
            //var lines = complete.Split('\\');
            //var msg="";
            //foreach(var s in lines)
            //    msg+=s+"\n";
            //msg = msg.Substring(0, msg.Length-1);
            //return msg;
            return AssembleResource(complete);
        }

        private static string AssembleResource(string resourceString)
        {
            var lines = resourceString.Split('\\');
            var msg = new StringBuilder();
            foreach (var s in lines)
                msg.AppendLine(s);
            if (msg.Length > 0)
                msg.Length--;
            return msg.ToString();
        }

        public string GetResource(string msgName, object[] parms)
        {
            var culture = Thread.CurrentThread.CurrentCulture.Name;
            //var complete = "";
            //if(culture.IndexOf("en-")!=-1)
            //	complete = String.Format((string)Messages.List[msgName], parms);
            //else
            //{
            var trans = String.Format(Translate(msgName, culture), parms);
            var complete = trans == msgName ? String.Format((string)Messages.List[msgName], parms) : trans;	//in case there is no translation
            //}

            return AssembleResource(complete);
            //var lines = complete.Split('\\');
            //var msg="";
            //foreach(var s in lines)
            //    msg+=s+"\n";
            //msg = msg.Substring(0, msg.Length-1);
            //return msg;
        }

        public string Translate(string text, string culture)
        {
            //object translation = null;
            if (Cache["Dictionaries"] != null)
            {
                var dictionary = (Hashtable)((Hashtable)Cache["Dictionaries"])[culture];
                if (dictionary != null)
                {
                    var translation = dictionary[text];
                    text = translation == null ? text : translation.ToString();
                }
            }
            return text;
        }

        public void LogPerformanceMessage(string message, string stack, EventLogEntryType type)
        {
            var perfLog = new Logging(1);
            perfLog.logPerformanceMessage(message, stack, type);
        }
    }
}
