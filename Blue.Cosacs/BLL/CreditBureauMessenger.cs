using System;
using System.Xml;
using System.Configuration;
using System.Net;
using System.IO;

namespace STL.BLL.CreditBureau
{
	/// <summary>
	/// This class will take care of sending request messages to the credit bureau
	/// and processing response messages. This level of abstraction is provided so 
	/// that the connection mechanism and transport protocol used is decoupled
	/// from the message construction
	/// </summary>
	public class CreditBureauMessenger
	{
		private string				bureauUrl = "";
		private string				bureauUser = "";
		private string				bureauPassword = "";
		private string				bureauPrefix = "";
		private bool				useProxy = false;
		private string				proxyHost = "";
		private string				proxyPort = "";
		private CredentialCache		credCache = null;

		/// <summary>
		/// The constructor will make sure that the MQSERVER environment 
		/// variable is set since we will not be able to connect to the 
		/// qmanager without it
		/// </summary>
		public CreditBureauMessenger()
		{
			bureauUrl = ConfigurationManager.AppSettings["bureauUrl"];
            bureauUser = ConfigurationManager.AppSettings["bureauUser"];
            bureauPassword = ConfigurationManager.AppSettings["bureauPassword"];
            bureauPrefix = ConfigurationManager.AppSettings["bureauPrefix"];
            useProxy = Convert.ToBoolean(ConfigurationManager.AppSettings["bureauUseProxy"]);
			if(useProxy)
			{
                proxyHost = ConfigurationManager.AppSettings["proxyHost"];
                proxyPort = ConfigurationManager.AppSettings["proxyPort"];
			}

			credCache = new CredentialCache();
			credCache.Add(new Uri(bureauPrefix), "Basic", new NetworkCredential(bureauUser, bureauPassword));

			ServicePointManager.CertificatePolicy = new TrustedCertificatePolicy();
		}

		public XmlDocument SendRequest(XmlDocument request)
		{			
			Stream requestStream = null;
			Stream respStream = null;
			XmlDocument response = null;

			try
			{
				response = new XmlDocument();

				WebRequest webReq = WebRequest.Create(bureauUrl);

				// test timeout
                webReq.Timeout = Convert.ToInt32(ConfigurationManager.AppSettings["bureau1Timeout"]);

				if(useProxy)
					webReq.Proxy = new WebProxy("http://"+proxyHost+":"+proxyPort+"/");

				webReq.Credentials = credCache;
				webReq.Method = "POST";
				webReq.ContentType = "text/html";
				requestStream = webReq.GetRequestStream();
				request.Save(requestStream);
				requestStream.Close();	

				WebResponse webResp = webReq.GetResponse();
				respStream = webResp.GetResponseStream();
				response.Load(respStream);
			
				respStream.Close();
			}
			catch(WebException ex)
			{
				string error = "There has been a problem communicating with the Credit Bureau. The exact details of the error follow: " + Environment.NewLine + Environment.NewLine;
				throw new CreditBureauException(error + ex.Message, ex);
			}
			finally
			{
				if(requestStream != null)
					requestStream.Close();
				if(respStream != null)
					respStream.Close();
			}

			return response;
		}
	}
}
