using System.Collections.ObjectModel;
//using Blue.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blue.Cosacs.Sales.Common
{
    public class ProductDepartment
    {
        private static ReadOnlyDictionary<string, string> department = null;

        public static ReadOnlyDictionary<string, string> Data
        {
            get
            {
                return department ?? (department = new ReadOnlyDictionary<string, string>(GetDepartmentCodes()));
            }
        }

        private static Dictionary<string, string> GetDepartmentCodes()
        {
            return new Dictionary<string, string>
            {
                { "DISCOUNT" , "PCDIS" },
                { "ELECTRICAL" , "PCE" },
                { "ELECTRONICS" , "PCE" },
                { "FURNITURE" , "PCF" },
                { "WORKSTATION" , "PCW" },
                { "OTHER" , "PCO" }
            };
        }
    }
}