using Blue.Cosacs.NonStocks.Zip;
using ICSharpCode.SharpZipLib.Zip;
using System.IO;

namespace Blue.Cosacs.NonStocks.Models.WinCosacs
{
    public class NonStocksFactExport
    {
        private IClock clock { get; set; }

        public NonStocksFactExport(IClock clock)
        {
            this.clock = clock;
        }

        public string Result { get; set; }

        public static string ProdFileName
        {
            get
            {
                return "nonstocks_prod.dat";
            }
        }

        public static string PromoFileName
        {
            get
            {
                return "nonstocks_promo.dat";
            }
        }

        public static string ProdAssocFileName
        {
            get
            {
                return "nonstocks_prodAssoc.dat";
            }
        }

        public string ProdFileContent { get; set; }
        public string PromoFileContent { get; set; }
        public string ProdAssocFileContent { get; set; }

        public MemoryStream GetZipFileStream()
        {
            var ms = new MemoryStream();
            var zipStream = new ZipOutputStream(ms);
            zipStream.SetLevel(9);

            ZipEntry dirEntry;
            dirEntry = new ZipEntry(@"\");
            dirEntry.DateTime = clock.Now;

            if (this.ProdFileContent.Length > 0)
            {
                Zip.ICSharpCodeZip.AddFileToZip(zipStream, clock, @"\" + ProdFileName, ProdFileContent);
            }

            if (this.PromoFileContent.Length > 0)
            {
                Zip.ICSharpCodeZip.AddFileToZip(zipStream, clock, @"\" + PromoFileName, PromoFileContent);
            }

            if (this.ProdAssocFileContent.Length > 0)
            {
                Zip.ICSharpCodeZip.AddFileToZip(zipStream, clock, @"\" + ProdAssocFileName, ProdAssocFileContent);
            }
            zipStream.Finish();

            ms.Position = 0;
            //var reader = new StreamReader(ms);
            //var zipFile = reader.ReadToEnd();

            zipStream.Close();

            return ms;
        }

    }
}
