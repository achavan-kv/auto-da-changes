using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.IO;

namespace STL.Batch
{
    internal class CSVWriter
    {
        private string _OutputFile = "";
        private DataTable _SourceTable = null;

        public CSVWriter(string filePath, DataTable dt)
        {
            _OutputFile = filePath;
            _SourceTable = dt;
        }

        public void WriteFile()
        {
            StringBuilder sb = new StringBuilder();
            //Create the column headings
            foreach (DataColumn dc in _SourceTable.Columns)
            {
                sb.Append(dc.ColumnName);
                if (_SourceTable.Columns.IndexOf(dc) != _SourceTable.Columns.Count - 1)
                    sb.Append(",");
            }

            sb.AppendLine();
            //Create Data
            foreach (DataRow dr in _SourceTable.Rows)
            {
                foreach (DataColumn dc in _SourceTable.Columns)
                {
                    sb.Append(dr[dc].ToString().Replace(",", " "));
                    sb.Append(",");
                }
                sb.AppendLine();
            }


            FileInfo fi = new FileInfo(_OutputFile);
            FileStream fs = fi.OpenWrite();
            byte[] blob = System.Text.Encoding.UTF8.GetBytes(sb.ToString());
            fs.Write(blob, 0, blob.Length);
            fs.Close();
            sb = null;
        }
    }
}



