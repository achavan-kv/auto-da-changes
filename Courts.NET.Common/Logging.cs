using System;
using System.IO;
using System.Configuration;
using System.Collections.Specialized;
using System.Reflection;
using System.Diagnostics;

namespace STL.Common
{
	/// <summary>
	/// This class is used for logging messages and exceptions
	/// to the server log file.
	/// </summary>
	public class Logging
	{
        //private string logLevel;
        //private string _app = "Cosacs.NET";
        //private EventLog _eventLog = null;
        //private EventLog m_eventLog = null;

		public Logging()
		{
            //if (!EventLog.SourceExists(_app))
            //{
            //    EventLog.CreateEventSource(_app, "Application");
            //}

            //_eventLog = new EventLog();
            //_eventLog.Source = _app;

            //logLevel = ConfigurationManager.AppSettings["logLevel"];
		}

	  public Logging(int i)
	  {      
            //if (!EventLog.SourceExists(_app))
            //{
            //   EventLog.CreateEventSource(_app, "CosacsPerformance");
            //}
            //m_eventLog = new EventLog();
            //m_eventLog.Source = _app;

            //logLevel = ConfigurationManager.AppSettings["logLevel"];
            //if (logLevel == null)
            //{
            //    logLevel = "none";
            //}
	  }

	  public void logPerformanceMessage(string message, string user,string stack, EventLogEntryType type)
	  {
         //try
         //{
         //    if (ConfigurationManager.AppSettings["performanceLog"] == "true")
         //   {
         //      if (logLevel.ToLower() != "none")
         //      {
         //         string msg = "UserID: " + user + Environment.NewLine + message + ". Stack: " + stack;
         //         m_eventLog.WriteEntry(msg, type);
         //      }
         //   }
         //}
         //finally
         //{
         //   m_eventLog.Close();
         //}
	  }

	  public void logPerformanceMessage(string message, string stack, EventLogEntryType type)
	  {
         //try
         //{
         //    if (ConfigurationManager.AppSettings["performanceLog"] == "true")
         //   {
         //      if (logLevel.ToLower() != "none")
         //      {
         //         string msg = message + ". Stack: " + stack;
         //         m_eventLog.WriteEntry(msg, type);
         //      }
         //   }
         //}
         //finally
         //{
         //   m_eventLog.Close();
         //}
	  }

		public void logMessage(string message, string user, EventLogEntryType type)
		{
            //try
            //{
            //    if(logLevel.ToLower() != "none" && user !=null  & message != null)
            //    {
            //        string msg = "UserID: "+user+Environment.NewLine+message;
            //        _eventLog.WriteEntry(msg, type);
            //    }
            //}
            //finally
            //{
            //    _eventLog.Close();
            //}
		}

		public void logMessage(string[] message, string user, EventLogEntryType type)
		{
            //try
            //{
            //    if(logLevel.ToLower() != "none")
            //    {
            //        string msg = "UserID: "+user+Environment.NewLine;
            //        foreach(string m in message)
            //            msg += m+Environment.NewLine;
            //        _eventLog.WriteEntry(msg, type);
            //    }
            //}
            //finally
            //{
            //    _eventLog.Close();
            //}
		}

		public void logMessage(StringCollection message, string user, EventLogEntryType type)
		{
            //try
            //{
            //    if(logLevel.ToLower() != "none")
            //    {
            //        string msg = "UserID: "+user+Environment.NewLine;
            //        foreach(string m in message)
            //            msg += m+Environment.NewLine;
            //        _eventLog.WriteEntry(msg, type);
            //    }
            //}
            //finally
            //{
            //    _eventLog.Close();
            //}
		}

		public void logException(Exception ex, string user, string function)
		{
            //string[] error;
            //if(logLevel=="verbose")
            //    error = new string[4];
            //else
            //    error = new String[3];
            //error[0] = "Exception raised from " + function;
            //error[1] = "Message: " + ex.Message;	
            //error[2] = "Source: " + ex.Source;	
            //if(logLevel=="verbose")
            //    error[3] = "Stack Trace: " + ex.StackTrace;						
            //logMessage(error, user, EventLogEntryType.Error); 
		}

		public string LogLevel
		{
			get
			{
                return "";
			}
		}
	}
}
