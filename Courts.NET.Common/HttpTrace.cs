using System;
using System.Xml;
using System.Web;
using System.IO;
using System.Configuration;

namespace STL.Common
{
	/// <summary>
	/// This class will be used to record the details of each HTTP 
	/// request in an XML file so that the HTTP history of an application
	/// session can be reproduced for load testing purposes.
	/// </summary>
	public class HttpTrace
	{
		private static XmlDocument httpLog = null;
		private static string traceFile = "";
		private static bool traceSwitch = false;

		public HttpTrace()
		{
		}

		public void Log(HttpContext context)
		{
			traceSwitch = Convert.ToBoolean(ConfigurationManager.AppSettings["httpTraceSwitch"]);

			if(traceSwitch)
			{
                traceFile = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + ConfigurationManager.AppSettings["httpTrace"];

				//Read in the trace file
				httpLog = new XmlDocument();	
			
				lock(this)	/* apply critical section */
				{
					FileInfo fi = new FileInfo(traceFile);
					if(!fi.Exists)
					{
						FileStream fs = fi.Create();
						httpLog.LoadXml("<HTTP/>");
						httpLog.Save(fs);
						fs.Close();
					}

					httpLog.Load(traceFile);

					//Extract the required information from the HttpRequest
					long posStream = context.Request.InputStream.Position;
					StreamReader input = new StreamReader(context.Request.InputStream);
					string _url = context.Request.Url.AbsoluteUri;
					string _soapAction = context.Request.Headers["SOAPAction"];
					string _body = input.ReadToEnd();			
					context.Request.InputStream.Position = posStream;				

					XmlNode root = httpLog.DocumentElement;

					//Create a new request node
					XmlNode req = CreateRequestNode(httpLog);
					req.AppendChild(CreateUrlNode(httpLog, _url));
					req.AppendChild(CreateSoapActionNode(httpLog, _soapAction));
					req.AppendChild(CreateBodyNode(httpLog, _body));

					root.AppendChild(req);
					httpLog.Save(traceFile);
				}
			}
		}

		private static XmlNode CreateRequestNode(XmlDocument doc)
		{
			return doc.CreateElement("REQUEST");
		}

		private static XmlNode CreateUrlNode(XmlDocument doc, string url)
		{
			XmlNode node = doc.CreateElement("URL");
			node.Attributes.Append(doc.CreateAttribute("VALUE"));
			node.Attributes["VALUE"].Value=url;
			return node;
		}

		private static XmlNode CreateSoapActionNode(XmlDocument doc, string soapAction)
		{
			XmlNode node = doc.CreateElement("SOAPACTION");
			node.Attributes.Append(doc.CreateAttribute("VALUE"));
			node.Attributes["VALUE"].Value=soapAction;
			return node;
		}

		private static XmlNode CreateBodyNode(XmlDocument doc, string body)
		{
			XmlNode node = doc.CreateElement("BODY");
			node.Attributes.Append(doc.CreateAttribute("VALUE"));
			node.Attributes["VALUE"].Value=body;
			return node;
		}
	}
}
