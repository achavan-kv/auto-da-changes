using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.Threading;
using System.Diagnostics;
using System.Reflection;

namespace STL.Common
{
    [Serializable]
    [XmlRoot(Namespace = Namespace)]
    [XmlType(Namespace = Namespace)]
    public class Error
    {
        public const string Namespace = "http://schemas.bluebridgeltd.com/cosacs/exceptions/2010/09/";

        public Error() { }

        //public Error(Exception exception)
        //{
        //    if (exception == null)
        //        throw new ArgumentNullException("exception");

        //    //this.Id = Guid.NewGuid();
        //    this.MachineName = Environment.MachineName;
        //    this.ExceptionType = exception.GetType().FullName;
        //    this.Message = exception.Message;
        //    this.Source = exception.Source;
        //    this.Details = exception.ToString();
        //    this.IdentityName = Thread.CurrentPrincipal.Identity.Name;
        //    this.IdentityType = Thread.CurrentPrincipal.Identity.AuthenticationType;
        //    this.Timestamp = DateTime.Now;
        //    this.ProcessName = Process.GetCurrentProcess().ProcessName;

        //    var assembly = Assembly.GetExecutingAssembly();
        //    var assemblyName = assembly.GetName();
        //    var version = assemblyName.Version;
        //    this.Version = version.ToString();
        //}

        //[XmlElement(Order = 0)]
        //public Guid Id { get; set; }
        [XmlElement(Order = 1)]
        public string Message { get; set; }
        [XmlElement(Order = 2)]
        public string Source { get; set; }
        [XmlElement(Order = 3)]
        public string ExceptionType { get; set; }
        [XmlElement(Order = 4)]
        public string Details { get; set; }
        [XmlElement(Order = 5)]
        public string MachineName { get; set; }
        [XmlElement(Order = 6)]
        public string ProcessName { get; set; }
        [XmlElement(Order = 7)]
        public DateTime Timestamp { get; set; }
        [XmlElement(Order = 8)]
        public string IdentityName { get; set; }
        [XmlElement(Order = 9)]
        public string IdentityType { get; set; }
        [XmlElement(Order = 10)]
        public string Version { get; set; }
    }
}
