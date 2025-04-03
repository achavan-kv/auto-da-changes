using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CsvHelper;
using CsvHelper.Configuration;

namespace Blue.Cosacs.Test.CvsReader
{
    internal class Reader<T>
    {
        public static IEnumerable<T> Read(string path)
        {
            return Read(path, new CsvConfiguration());
        }

        public static IEnumerable<T> Read(CsvParser parser)
        {
            using (var csv = new CsvReader(parser))
            {
                return csv.GetRecords<T>();
            }
        }

        public static IList<T> Read(string path, CsvConfiguration configuration)
        {
            using (var reader = new StreamReader(path))
            {
                using (var csv = new CsvReader(reader, configuration))
                {
                    return csv.GetRecords<T>().ToList();
                }
            }
        }

        public static void Write(List<T> source, string path)
        { 
            using (var sw = new StreamWriter(path,true))
            {
                var writer = new CsvWriter(sw);
                writer.WriteRecords(source);
                sw.Flush();
                sw.Close();
            }
        }
    }
}
