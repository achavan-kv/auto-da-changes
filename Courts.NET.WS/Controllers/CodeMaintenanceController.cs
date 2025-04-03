using Blue.Cosacs.Model;
using Blue.Cosacs.Repositories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace Cosacs.Web.Controllers
{
    public class CodeMaintenanceController : Controller
    {
        public void UpdateCodeMaintenance()
        {
            var nonStocks = ReadRequestBase64InputStream();
            new StockRepository().UpdateCodeMaintenanceNonStocks(nonStocks);
        }

        private List<NonStockModel> ReadRequestBase64InputStream()
        {
            using (var reader = new StreamReader(Request.InputStream, Encoding.UTF8))
            {
                var jsonFileString = string.Empty;
                var byteArrayString = reader.ReadToEnd();
                byteArrayString = byteArrayString.Trim(new char[] { '"' });

                if (IsBase64String(byteArrayString))
                {
                    byte[] hashBytes = Convert.FromBase64String(byteArrayString);
                    jsonFileString = System.Text.Encoding.Default.GetString(hashBytes);
                }

                var stringReader = new StringReader(jsonFileString);
                var nonStocks = (List<NonStockModel>)new Newtonsoft.Json.JsonSerializer()
                    .Deserialize(stringReader, new List<NonStockModel>().GetType());

                return nonStocks;
            }
        }

        public bool IsBase64String(string s)
        {
            s = s.Trim();
            return (s.Length % 4 == 0) && Regex.IsMatch(s, @"^[a-zA-Z0-9\+/]*={0,3}$", RegexOptions.None);
        }
    }
}
