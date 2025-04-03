using System;
using System.Configuration;
using System.Web.Services;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Collections;
using System.Collections.Specialized;

namespace STL.Common
{
	/// <summary>
	/// This is the base class for all web services. 
	/// It inherits from System.Web.Services.WebService
	/// and implements an authentication header instance.
	/// It also provides logging functions.
	/// </summary>
	[WebService(Namespace="http://strategicthought.com/webservices/")]
	public class CommonService : System.Web.Services.WebService
	{
		public Authentication authentication;
		private Logging log;
		private string _function;
		protected string _error;

		protected int retries = 0;
		protected int maxRetries = 10;
		protected int Deadlock = 1205;

		private StaticDataSingleton _staticData = StaticDataSingleton.Instance();
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

		/*private IDictionary _cache = null;
		public IDictionary Cache 
		{
			get
			{
				if(_cache==null)
				{
					if(System.Web.HttpContext.Current != null)
					{
						if(System.Web.HttpContext.Current.Cache["Cache"]==null)
							System.Web.HttpContext.Current.Cache["Cache"] = new Hashtable();

						_cache = (IDictionary)System.Web.HttpContext.Current.Cache["Cache"];
					}
					else
					{
						_cache = new Hashtable();
					}
				}
				return _cache;
			}
			set
			{
				_cache = value;
				if(System.Web.HttpContext.Current != null)
				{
					System.Web.HttpContext.Current.Cache["Cache"] = _cache;
				}
			}
		}*/

		protected CountryParameterCollection Country
		{
			get
			{
				CountryParameterCollection c = null;
				if(Cache!=null)
					c = (CountryParameterCollection)Cache["Country"];
				return c;
			}
		}

		public string DecimalPlaces
		{
			get
			{
				return (string)Country[CountryParameterNames.DecimalPlaces];
			}
		}

		public string Function
		{
			get
			{
				return _function;
			}
			set
			{
				_function = value;
			}
		}

		protected void Catch(Exception ex, string function, ref string err)
		{			
			logException(ex, function);

            if (ex is STLException)
                err = ex.Message;
            else
            {
                if (ex is SqlException)
                {
                    Exception SqlEx = new Exception(Environment.NewLine + Environment.NewLine +
                           "Error:" + ((SqlException)ex).Message + Environment.NewLine +
                           "Error number:" + ((SqlException)ex).Number + Environment.NewLine +
                           "Stored Procedure:" + ((SqlException)ex).Procedure + Environment.NewLine +
                           "Line:" + ((SqlException)ex).LineNumber + Environment.NewLine +
                           "Server:" + ((SqlException)ex).Server + Environment.NewLine +
                           "StackTrace:" + Environment.NewLine + ex.StackTrace + Environment.NewLine);
                
                    throw SqlEx;

                }
                else
                {
                    throw new Exception("See inner exception details.", ex);
                }
            }
		}

		public void CatchDeadlock(SqlException ex, SqlConnection conn)
		{
			if(ex.Number==Deadlock && retries<maxRetries)
			{
				retries++;
				logMessage("Deadlock captured in "+Function+" Retry count = "+retries, STL.Common.Static.Credential.User, EventLogEntryType.Error);
				if(conn.State != ConnectionState.Closed)
					conn.Close();
			}
			else
				throw ex;
		}

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

		public void logException (Exception ex, string function)
		{
			log = new Logging();
            log.logException(ex, STL.Common.Static.Credential.User, function);
		}

      public void logPerformanceMessage(string message, string user,string stack, EventLogEntryType type)
      {
         Logging perfLog = new Logging(1);
         perfLog.logPerformanceMessage(message, user,stack, type);
      }
	}
}
