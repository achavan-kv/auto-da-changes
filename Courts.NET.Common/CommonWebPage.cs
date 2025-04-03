using System;
using System.Data.SqlClient;
using System.Data;
using STL.Common.Static;
using System.Collections;
using System.Diagnostics;
using System.Collections.Specialized;
using System.Globalization;

namespace STL.Common
{
	/// <summary>
	/// Summary description for CommonWebPage.
	/// </summary>
	public class CommonWebPage : System.Web.UI.Page, ITranslatable
	{
		
		public CommonWebPage()
		{
		}

		protected int retries = 0;
		protected int maxRetries = 10;
		protected int Deadlock = 1205;

		private Logging log;
		private string _function;
		public string Function
		{
			get{return _function;}
			set{_function = value;}
		}

		private StaticDataSingleton _staticData = StaticDataSingleton.Instance();
		public new HybridDictionary Cache
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

		public void logMessage(string message, string user, EventLogEntryType type)
		{
			if(log==null)
				log = new Logging();
			log.logMessage(message, user, type);
		}

		public void logMessage(string[] message, string user, EventLogEntryType type)
		{
			if(log==null)
				log = new Logging();
			log.logMessage(message, user, type);
		}

		public void logException (Exception ex, string function)
		{
			if(log==null)
				log = new Logging();
            log.logException(ex, STL.Common.Static.Credential.User, function);
		}

		public void CatchDeadlock(SqlException ex, SqlConnection conn)
		{
			if(ex.Number==Deadlock && retries<maxRetries)
			{
				retries++;
				logMessage("Deadlock captured in "+Function+" Retry count = "+retries, User.Identity.Name, EventLogEntryType.Error);
				if(conn.State != ConnectionState.Closed)
					conn.Close();
			}
			else
				throw ex;
		}

		public string CalculateTimeSpan(DateTime endDate, DateTime startDate)
		{
			int y = endDate.Year - startDate.Year;
			int m = endDate.Month - startDate.Month;
			if(m<0)
			{
				y--;
				m += 12;
			}
			return y.ToString() + " year(s) " + m.ToString() + " month(s)";
		}

		public string GetResource(string msgName)
		{
			string culture = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
			string complete = (string)Messages.List[msgName];
			//if(culture.IndexOf("en-")!=-1)
			//	complete = (string)Messages.List[msgName];
			//else
			//{
				string trans = Translate(msgName, culture);
				complete = trans==msgName?complete:trans;	//in case there is no translation
			//}
			string[] lines = complete.Split('\\');
			string msg="";
			foreach(string s in lines)
				msg+=s+"\n";
			msg = msg.Substring(0, msg.Length-1);
			return msg;
		}

		public string GetResource(string msgName, object[] parms)
		{
			string culture = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
			string complete = ""; 
			//if(culture.IndexOf("en-")!=-1)
			//	complete = String.Format((string)Messages.List[msgName], parms);
			//else
			//{
				string trans = String.Format(Translate(msgName, culture), parms);
				complete = trans==msgName?String.Format((string)Messages.List[msgName], parms):trans;	//in case there is no translation
			//}

			string[] lines = complete.Split('\\');
			string msg="";
			foreach(string s in lines)
				msg+=s+"\n";
			msg = msg.Substring(0, msg.Length-1);
			return msg;
		}

		public string Translate(string text, string culture)
		{
			object translation = null;
			if(Cache["Dictionaries"]!=null)
			{				
				Hashtable dictionary  = (Hashtable)((Hashtable)Cache["Dictionaries"])[culture];
				if(dictionary!=null)
				{
					translation = dictionary[text];
					text = translation==null ? text:translation.ToString();
				}
			}
			return text;
		}

		protected decimal CalculateMonthlyInterest(decimal annual)
		{
			double m = 0;
			double y = Convert.ToDouble(annual);

			m = 100 * (Math.Pow(((y + 100D) / 100D), (1D/12D)) - 1D);

			return Convert.ToDecimal(m);
		}
        
        protected void SetCulture()
        {
            // lw 69221 en-CB has been updated to en-029 in vista and microsft updates
            // if server has not been updated will cause problems
            try
            {
                System.Threading.Thread.CurrentThread.CurrentCulture = ((CultureInfo)System.Web.HttpContext.Current.Cache["Culture"]);
            }
            catch
            {
                //if (culture.ToLower() == "en-cb")
                //    culture = "en-029";
                //else if (culture.ToLower() == "en-029")
                //    culture = "en-CB";

                //System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(culture);
            }
        }
	}
}
