using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blue.Cosacs.Service.Utils
{
    public sealed class ServiceType
    {
        public string Name { get; set; }
        public string Type { get; set; }

        private ServiceType(string name, string type)
        {
            this.Name = name;
            this.Type = type;
        }

        public static readonly ServiceType Installation = new ServiceType("Internal Installation", "II");
        public static readonly ServiceType InstallationExternal = new ServiceType("External Customer Installation", "IE");
        public static readonly ServiceType StockRepair = new ServiceType("Stock Repair", "S");
        public static readonly ServiceType ServiceRequestInternal = new ServiceType("Service Request Internal", "SI");
        public static readonly ServiceType ServiceRequestExternal = new ServiceType("Service Request External", "SE");

        public override string ToString()
        {
            return string.Format("{0}", Name);
        }

        public override int GetHashCode()
        {
            return Type.GetHashCode();
        }

        public static ServiceType FromString(string code)
        {
            return AsEnumerable().FirstOrDefault(s => string.Compare(s.Type, code, true) == 0);
        }

        public static IEnumerable<ServiceType> AsEnumerable()
        {
            yield return Installation;
            yield return InstallationExternal;
            yield return StockRepair;
            yield return ServiceRequestInternal;
            yield return ServiceRequestExternal;
        }
    }
}
