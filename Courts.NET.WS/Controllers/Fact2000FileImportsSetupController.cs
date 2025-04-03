using Blue.Cosacs.Repositories;
using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace Cosacs.Web.Controllers
{
    public class Fact2000ImportFilesController : Controller
    {
        public JsonResult SetupEODFile()
        {
            var eodFiles = ReadRequestBase64InputStream();

            var repository = new ConfigRepository();

            var systemDrive = repository.GetSystemDrive();

            WriteFact2000ToSystemDrive(eodFiles, systemDrive);

            var retVals =
                new
                {
                    systemDrive
                };

            return Json(retVals, JsonRequestBehavior.AllowGet);
        }

        private void WriteFact2000ToSystemDrive(NonStocksFactExport eodFile, string systemDrive)
        {
            var formatFileName = string.Empty;
            var fileName = string.Empty;
            if (Directory.Exists(systemDrive))
            {
                if (eodFile.ProdFileContent.Length > 0)
                {
                    fileName = Path.Combine(systemDrive, eodFile.ProdFileName);
                    // overwrite
                    using (var writer = new StreamWriter(fileName, false))
                    {
                        writer.Write(eodFile.ProdFileContent);
                        writer.Close();
                    }
                }

                if (eodFile.PromoFileContent.Length > 0)
                {
                    fileName = Path.Combine(systemDrive, eodFile.PromoFileName);
                    // overwrite
                    using (var writer = new StreamWriter(fileName, false))
                    {
                        writer.Write(eodFile.PromoFileContent);
                        writer.Close();
                    }
                }

                if (eodFile.ProdAssocFileContent.Length > 0)
                {
                    fileName = Path.Combine(systemDrive, eodFile.ProdAssocFileName);
                    // overwrite
                    using (var writer = new StreamWriter(fileName, false))
                    {
                        writer.Write(eodFile.ProdAssocFileContent);
                        writer.Close();
                    }
                }
            }
        }

        private NonStocksFactExport ReadRequestBase64InputStream()
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
                var eodFile = (NonStocksFactExport)new Newtonsoft.Json.JsonSerializer()
                    .Deserialize(stringReader, new NonStocksFactExport().GetType());

                return eodFile;
            }
        }

        internal class NonStocksFactExport
        {
            public string ProdFileContent { get; set; }
            public string PromoFileContent { get; set; }
            public string ProdAssocFileContent { get; set; }

            public string ProdFileName
            {
                get
                {
                    return "nonstocks_prod.dat";
                }
            }

            public string PromoFileName
            {
                get
                {
                    return "nonstocks_promo.dat";
                }
            }

            public string ProdAssocFileName
            {
                get
                {
                    return "nonstocks_prodAssoc.dat";
                }
            }
        }

        public bool IsBase64String(string s)
        {
            s = s.Trim();
            return (s.Length % 4 == 0) && Regex.IsMatch(s, @"^[a-zA-Z0-9\+/]*={0,3}$", RegexOptions.None);
        }
    }
}