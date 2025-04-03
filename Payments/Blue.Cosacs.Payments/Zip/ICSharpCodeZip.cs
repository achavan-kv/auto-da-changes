using ICSharpCode.SharpZipLib.Zip;
using System.IO;
using System.Text;

namespace Blue.Cosacs.Payments.Zip
{
    public class ICSharpCodeZip
    {
        public static void AddFileToZip(ZipOutputStream zipStream, IClock clock, string relativePath, string file)
        {
            if (relativePath.Length > 0)
            {
                var entry = new ZipEntry(relativePath);
                entry.DateTime = clock.Now;
                zipStream.PutNextEntry(entry);

                var buffer = Encoding.UTF8.GetBytes(file);
                zipStream.Write(buffer, 0, buffer.Length);
            }
        }
    }
}
