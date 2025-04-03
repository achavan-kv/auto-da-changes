using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.Threading;
using System.Diagnostics;
using System.Reflection;

namespace STL.PL.WS1
{
    public partial class Error
    {
        public Error()
        { 
        }

        public Error(Exception exception)
        {
            if (exception == null)
                throw new ArgumentNullException("exception");

            //this.Id = Guid.NewGuid();
            this.MachineName = Environment.MachineName;
            this.ExceptionType = exception.GetType().FullName;
            this.Message = exception.Message;
            this.Source = exception.Source;
            this.Details = exception.ToString();
            this.IdentityName = Thread.CurrentPrincipal.Identity.Name;
            this.IdentityType = Thread.CurrentPrincipal.Identity.AuthenticationType;
            this.Timestamp = DateTime.Now;
            this.ProcessName = Process.GetCurrentProcess().ProcessName;

            var assembly = Assembly.GetExecutingAssembly();
            var assemblyName = assembly.GetName();
            var version = assemblyName.Version;
            this.Version = version.ToString();
        }
    }
}
