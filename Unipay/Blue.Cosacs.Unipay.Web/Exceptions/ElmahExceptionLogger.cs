using Elmah;
using System;
using System.Net.Http;
using System.Web;
using System.Web.Http.ExceptionHandling;

namespace Blue.Cosacs.Unipay.Web.Exceptions
{
	public class ElmahExceptionLogger : ExceptionLogger
	{
		public override void Log(ExceptionLoggerContext context)
		{
			HttpContext httpContext = GetHttpContext(context.Request);

			if (httpContext == null)
				return;

			Exception exceptionToRaise = new HttpUnhandledException(context.Exception.Message, context.Exception);

			ErrorSignal signal = ErrorSignal.FromContext(httpContext);
			signal.Raise(exceptionToRaise, httpContext);
		}

		private static HttpContext GetHttpContext(HttpRequestMessage request)
		{
			if (request == null)
				return null;

			object value;
			if (request.Properties.TryGetValue("MS_HttpContext", out value))
			{
				HttpContextBase context = value as HttpContextBase;
				if (context != null)
					return context.ApplicationInstance.Context;
			}

			return HttpContext.Current;
		}
	}
}