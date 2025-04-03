using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.IO.Compression;

namespace STL.DAL
{
    class CompDec
    {
        

        public Stream CompressStream(byte[] input)
        {
            try
            {
                MemoryStream ms = new MemoryStream();
                // Use the newly created memory stream for the compressed data.
                GZipStream compressedzipStream = new GZipStream(ms, CompressionMode.Compress, true);
                compressedzipStream.Write(input, 0, (int)input.Length);
                compressedzipStream.Close();
                ms.Position = 0;
                return ms;
            }
            catch
            {
                throw;
            }
        }

        public Stream DecompressStream(byte[] input)
        {
            try
            {
                MemoryStream ms = new MemoryStream();
                // Use the newly created memory stream for the compressed data.
                ms.Write(input, 0, (int)input.Length);
                ms.Position = 0;
                GZipStream compressedzipStream = new GZipStream(ms, CompressionMode.Decompress, true);
                return compressedzipStream;
            }
            catch
            {
                throw;
            }
        }

    }
}
