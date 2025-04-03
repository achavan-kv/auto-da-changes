using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Data;




namespace STL.DAL
{
    public class DTransfer
    {
        public DTransfer()
        { 
        }
        /// <summary>
        /// WriteBinarFile -- copies file from server to client. 
        /// </summary>
        /// <param name="fs"></param>
        /// <param name="path"></param>
        public void WriteBinarFile(byte[] fs, string path)
        {
            CompDec C = new CompDec();
          
            
            try
            {
                MemoryStream memoryStream = new MemoryStream(fs);
                FileStream fileStream = new FileStream(path, FileMode.Create);

                Stream decompressedStream = C.DecompressStream(ConvertStreamToByteBuffer(memoryStream));
                //decompressedStream.Position = 0;
                byte[] byteBuffer = ConvertStreamToByteBuffer(decompressedStream);
                fileStream.Write(byteBuffer, 0, (int)byteBuffer.Length);
                //memoryStream.WriteTo(C.DecompressStream(ConvertStreamToByteBuffer(memoryStream)));
             
                memoryStream.Close();
                fileStream.Close();
                fileStream = null;
                memoryStream = null;
             }
            catch
            {
               throw;
            }
        }

        public byte[] ReadBinaryFile(string path)
        {
            if (File.Exists(path))
            {
                try
                {
                    CompDec C = new CompDec();

                    ///Open and read a file
                    FileStream fileStream = File.OpenRead(path);
                    return ConvertStreamToByteBuffer(C.CompressStream(ConvertStreamToByteBuffer(fileStream)));
                   

                }
                catch 
                {
                    return new byte[0];
                }
            }
            else
            {
                return new byte[0];
            }
        }


        public byte[] ConvertStreamToByteBuffer(System.IO.Stream theStream)
        {
            int b1;
            System.IO.MemoryStream tempStream = new System.IO.MemoryStream();
            while ((b1 = theStream.ReadByte()) != -1)
            {
                tempStream.WriteByte(((byte)b1));
            }
            return tempStream.ToArray();
        }

        public Stream ConvertByteBufferToStream(byte[] byteArray)
        {
            
            Stream constream = new MemoryStream(byteArray);

            return constream;
        }

      public void ExportToCSV(DataTable dt, string strFilePath)
         {
            var sw = new StreamWriter(strFilePath , false);

            // Write the headers.
            int iColCount = dt.Columns.Count;
            for (int i = 0; i < iColCount; i++)
            {
            sw.Write(dt.Columns[i]);
            if (i < iColCount - 1) sw.Write(",");
            }

            sw.Write(sw.NewLine);

            // Write rows.
            foreach (DataRow dr in dt.Rows)
            {
            for (int i = 0; i < iColCount; i++)
            {
            if (!Convert.IsDBNull(dr[i]))
            {
            if(dr[i].ToString().StartsWith("0"))
            {
                sw.Write(dr[i].ToString());
            //sw.Write(@"="""+dr[i]+@""""); // Remove this for now as don't think required. 
            }
            else
            {
            sw.Write(dr[i].ToString());
            }
            }

            if (i < iColCount - 1) sw.Write(",");
            }
            sw.Write(sw.NewLine);
            }

            sw.Close();
             }

      //#13719
      public void ExportToCSV(DataTable dt, string strFilePath, char seperator)
      {
          var sw = new StreamWriter(strFilePath, false);

          // Write the headers.
          int iColCount = dt.Columns.Count;
          for (int i = 0; i < iColCount; i++)
          {
              sw.Write(dt.Columns[i]);
              if (i < iColCount - 1) sw.Write(seperator);
          }

          sw.Write(sw.NewLine);

          // Write rows.
          foreach (DataRow dr in dt.Rows)
          {
              for (int i = 0; i < iColCount; i++)
              {
                  if (!Convert.IsDBNull(dr[i]))
                  {
                      if (dr[i].ToString().StartsWith("0"))
                      {
                          sw.Write(dr[i].ToString());
                      }
                      else
                      {
                          sw.Write(dr[i].ToString());
                      }
                  }

                  if (i < iColCount - 1) sw.Write(seperator);
              }
              sw.Write(sw.NewLine);
          }

          sw.Close();
      }



    }
}
